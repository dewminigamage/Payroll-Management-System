using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PayrollManagementSystem.API.Helpers;
using PayrollManagementSystem.API.Models;

namespace PayrollManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OvertimeController : ControllerBase
{
    private readonly DatabaseHelper _db;
    public OvertimeController(DatabaseHelper db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? month, [FromQuery] int? year, [FromQuery] int? employeeId)
    {
        var sql = @"SELECT ot.*, e.FullName AS EmployeeName
                    FROM OvertimeRecords ot JOIN Employees e ON e.EmployeeID=ot.EmployeeID
                    WHERE 1=1";
        var ps = new List<SqlParameter>();
        if (month.HasValue)      { sql += " AND ot.PayMonth=@m";      ps.Add(new("@m", month)); }
        if (year.HasValue)       { sql += " AND ot.PayYear=@y";       ps.Add(new("@y", year)); }
        if (employeeId.HasValue) { sql += " AND ot.EmployeeID=@eid";  ps.Add(new("@eid", employeeId)); }
        sql += " ORDER BY ot.PayYear DESC, ot.PayMonth DESC, e.FullName";
        var dt = await _db.QueryAsync(sql, [.. ps]);
        return Ok(dt.Rows.Cast<System.Data.DataRow>().Select(Map));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Upsert([FromBody] UpsertOvertimeDto dto)
    {
        await _db.NonQueryAsync(
            @"MERGE OvertimeRecords AS t
              USING (VALUES (@eid,@m,@y,@hrs,@rate,@amt,@notes))
                AS s(EmployeeID,PayMonth,PayYear,OTHours,OTRateMultiplier,OTAmount,Notes)
              ON t.EmployeeID=s.EmployeeID AND t.PayMonth=s.PayMonth AND t.PayYear=s.PayYear
              WHEN MATCHED THEN UPDATE SET OTHours=s.OTHours, OTRateMultiplier=s.OTRateMultiplier,
                   OTAmount=s.OTAmount, Notes=s.Notes
              WHEN NOT MATCHED THEN INSERT (EmployeeID,PayMonth,PayYear,OTHours,OTRateMultiplier,OTAmount,Notes)
                   VALUES (s.EmployeeID,s.PayMonth,s.PayYear,s.OTHours,s.OTRateMultiplier,s.OTAmount,s.Notes);",
            new SqlParameter("@eid",   dto.EmployeeID),
            new SqlParameter("@m",     dto.PayMonth),
            new SqlParameter("@y",     dto.PayYear),
            new SqlParameter("@hrs",   dto.OTHours),
            new SqlParameter("@rate",  dto.OTRateMultiplier),
            new SqlParameter("@amt",   dto.OTAmount),
            new SqlParameter("@notes", (object?)dto.Notes ?? DBNull.Value));
        return Ok(new { message = "Saved" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var rows = await _db.NonQueryAsync("DELETE FROM OvertimeRecords WHERE OvertimeID=@id",
            new SqlParameter("@id", id));
        return rows == 0 ? NotFound() : NoContent();
    }

    private static OvertimeRecord Map(System.Data.DataRow r) => new()
    {
        OvertimeID       = DatabaseHelper.Get<int>(r, "OvertimeID"),
        EmployeeID       = DatabaseHelper.Get<int>(r, "EmployeeID"),
        EmployeeName     = DatabaseHelper.GetString(r, "EmployeeName"),
        PayMonth         = DatabaseHelper.Get<int>(r, "PayMonth"),
        PayYear          = DatabaseHelper.Get<int>(r, "PayYear"),
        OTHours          = DatabaseHelper.Get<decimal>(r, "OTHours"),
        OTRateMultiplier = DatabaseHelper.Get<decimal>(r, "OTRateMultiplier"),
        OTAmount         = DatabaseHelper.Get<decimal>(r, "OTAmount"),
        Notes            = DatabaseHelper.GetString(r, "Notes"),
        CreatedDate      = DatabaseHelper.Get<DateTime>(r, "CreatedDate")
    };
}
