using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PayrollManagementSystem.API.Helpers;
using PayrollManagementSystem.API.Models;

namespace PayrollManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly DatabaseHelper _db;
    public AttendanceController(DatabaseHelper db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int?      employeeId,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        [FromQuery] int?      month,
        [FromQuery] int?      year)
    {
        var sql = @"SELECT a.*, e.FullName AS EmployeeName
                    FROM Attendance a JOIN Employees e ON e.EmployeeID=a.EmployeeID
                    WHERE 1=1";
        var ps = new List<SqlParameter>();
        if (employeeId.HasValue) { sql += " AND a.EmployeeID=@eid"; ps.Add(new("@eid", employeeId)); }
        if (from.HasValue)       { sql += " AND a.AttendanceDate>=@from"; ps.Add(new("@from", from.Value.ToDateTime(TimeOnly.MinValue))); }
        if (to.HasValue)         { sql += " AND a.AttendanceDate<=@to";   ps.Add(new("@to",   to.Value.ToDateTime(TimeOnly.MinValue))); }
        if (month.HasValue)      { sql += " AND MONTH(a.AttendanceDate)=@m"; ps.Add(new("@m", month)); }
        if (year.HasValue)       { sql += " AND YEAR(a.AttendanceDate)=@y";  ps.Add(new("@y", year)); }
        sql += " ORDER BY a.AttendanceDate DESC, e.FullName";
        var dt = await _db.QueryAsync(sql, [.. ps]);
        return Ok(dt.Rows.Cast<System.Data.DataRow>().Select(Map));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Upsert([FromBody] UpsertAttendanceDto dto)
    {
        await _db.NonQueryAsync(
            @"MERGE Attendance AS t
              USING (VALUES (@eid, @dt, @st, @rem)) AS s(EmployeeID, AttendanceDate, Status, Remarks)
              ON t.EmployeeID=s.EmployeeID AND t.AttendanceDate=s.AttendanceDate
              WHEN MATCHED THEN UPDATE SET Status=s.Status, Remarks=s.Remarks
              WHEN NOT MATCHED THEN INSERT (EmployeeID, AttendanceDate, Status, Remarks)
                VALUES (s.EmployeeID, s.AttendanceDate, s.Status, s.Remarks);",
            new SqlParameter("@eid", dto.EmployeeID),
            new SqlParameter("@dt",  dto.AttendanceDate.ToDateTime(TimeOnly.MinValue)),
            new SqlParameter("@st",  dto.Status),
            new SqlParameter("@rem", (object?)dto.Remarks ?? DBNull.Value));
        return Ok(new { message = "Saved" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var rows = await _db.NonQueryAsync("DELETE FROM Attendance WHERE AttendanceID=@id",
            new SqlParameter("@id", id));
        return rows == 0 ? NotFound() : NoContent();
    }

    private static AttendanceRecord Map(System.Data.DataRow r) => new()
    {
        AttendanceID   = DatabaseHelper.Get<int>(r, "AttendanceID"),
        EmployeeID     = DatabaseHelper.Get<int>(r, "EmployeeID"),
        EmployeeName   = DatabaseHelper.GetString(r, "EmployeeName"),
        AttendanceDate = DateOnly.FromDateTime(DatabaseHelper.Get<DateTime>(r, "AttendanceDate")),
        Status         = DatabaseHelper.Get<string>(r, "Status")!,
        Remarks        = DatabaseHelper.GetString(r, "Remarks")
    };
}
