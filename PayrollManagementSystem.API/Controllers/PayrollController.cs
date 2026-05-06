using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PayrollManagementSystem.API.Helpers;
using PayrollManagementSystem.API.Models;

namespace PayrollManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PayrollController : ControllerBase
{
    private readonly DatabaseHelper _db;
    public PayrollController(DatabaseHelper db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? month, [FromQuery] int? year, [FromQuery] int? employeeId)
    {
        var sql = @"SELECT pr.*, e.FullName AS EmployeeName, e.Department
                    FROM PayrollRecords pr
                    JOIN Employees e ON e.EmployeeID = pr.EmployeeID
                    WHERE 1=1";
        var ps = new List<SqlParameter>();
        if (month.HasValue)      { sql += " AND pr.PayMonth=@m";  ps.Add(new("@m", month)); }
        if (year.HasValue)       { sql += " AND pr.PayYear=@y";   ps.Add(new("@y", year)); }
        if (employeeId.HasValue) { sql += " AND pr.EmployeeID=@e"; ps.Add(new("@e", employeeId)); }
        sql += " ORDER BY pr.PayYear DESC, pr.PayMonth DESC, e.FullName";
        var dt = await _db.QueryAsync(sql, [.. ps]);
        return Ok(dt.Rows.Cast<System.Data.DataRow>().Select(MapRecord));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dt = await _db.QueryAsync(
            @"SELECT pr.*, e.FullName AS EmployeeName, e.Department
              FROM PayrollRecords pr JOIN Employees e ON e.EmployeeID=pr.EmployeeID
              WHERE pr.PayrollID=@id",
            new SqlParameter("@id", id));
        if (dt.Rows.Count == 0) return NotFound();
        return Ok(MapRecord(dt.Rows[0]));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePayrollDto dto)
    {
        var exists = await _db.ScalarAsync(
            "SELECT COUNT(1) FROM PayrollRecords WHERE EmployeeID=@e AND PayMonth=@m AND PayYear=@y",
            new SqlParameter("@e", dto.EmployeeID),
            new SqlParameter("@m", dto.PayMonth),
            new SqlParameter("@y", dto.PayYear));
        if (Convert.ToInt32(exists) > 0)
            return Conflict(new { message = "Payroll already exists for this employee and period" });

        var id = await _db.ScalarAsync(
            @"INSERT INTO PayrollRecords (EmployeeID,PayMonth,PayYear,BasicSalary,Allowances,GrossSalary,EPF,ETF,Tax,OtherDeductions,NetSalary,Remarks)
              VALUES (@eid,@m,@y,@bs,@al,@gs,@epf,@etf,@tax,@od,@ns,@rem); SELECT SCOPE_IDENTITY();",
            new SqlParameter("@eid", dto.EmployeeID),
            new SqlParameter("@m",   dto.PayMonth),
            new SqlParameter("@y",   dto.PayYear),
            new SqlParameter("@bs",  dto.BasicSalary),
            new SqlParameter("@al",  dto.Allowances),
            new SqlParameter("@gs",  dto.GrossSalary),
            new SqlParameter("@epf", dto.EPF),
            new SqlParameter("@etf", dto.ETF),
            new SqlParameter("@tax", dto.Tax),
            new SqlParameter("@od",  dto.OtherDeductions),
            new SqlParameter("@ns",  dto.NetSalary),
            new SqlParameter("@rem", (object?)dto.Remarks ?? DBNull.Value));
        var newId = Convert.ToInt32(id);
        return await GetById(newId) is OkObjectResult ok
            ? CreatedAtAction(nameof(GetById), new { id = newId }, ok.Value)
            : StatusCode(500);
    }

    [HttpPost("bulk")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BulkProcess([FromBody] BulkPayrollDto dto)
    {
        var empDt = await _db.QueryAsync("SELECT * FROM Employees WHERE IsActive=1");
        int created = 0, skipped = 0;
        foreach (System.Data.DataRow r in empDt.Rows)
        {
            var eid = DatabaseHelper.Get<int>(r, "EmployeeID");
            var exists = await _db.ScalarAsync(
                "SELECT COUNT(1) FROM PayrollRecords WHERE EmployeeID=@e AND PayMonth=@m AND PayYear=@y",
                new SqlParameter("@e", eid),
                new SqlParameter("@m", dto.PayMonth),
                new SqlParameter("@y", dto.PayYear));
            if (Convert.ToInt32(exists) > 0) { skipped++; continue; }

            var basic = DatabaseHelper.Get<decimal>(r, "BasicSalary");
            var epf   = Math.Round(basic * 0.08m, 2);
            var etf   = Math.Round(basic * 0.03m, 2);
            var net   = basic - epf;

            await _db.NonQueryAsync(
                @"INSERT INTO PayrollRecords (EmployeeID,PayMonth,PayYear,BasicSalary,Allowances,GrossSalary,EPF,ETF,Tax,OtherDeductions,NetSalary,Remarks)
                  VALUES (@eid,@m,@y,@bs,0,@bs,@epf,@etf,0,0,@net,@rem)",
                new SqlParameter("@eid", eid),
                new SqlParameter("@m",   dto.PayMonth),
                new SqlParameter("@y",   dto.PayYear),
                new SqlParameter("@bs",  basic),
                new SqlParameter("@epf", epf),
                new SqlParameter("@etf", etf),
                new SqlParameter("@net", net),
                new SqlParameter("@rem", $"{System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dto.PayMonth)} {dto.PayYear} payroll"));
            created++;
        }
        return Ok(new { created, skipped, message = $"Processed: {created} new, {skipped} already existed" });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePayrollDto dto)
    {
        var rows = await _db.NonQueryAsync(
            @"UPDATE PayrollRecords SET Allowances=@al,GrossSalary=@gs,EPF=@epf,ETF=@etf,
              Tax=@tax,OtherDeductions=@od,NetSalary=@ns,Remarks=@rem WHERE PayrollID=@id",
            new SqlParameter("@al",  dto.Allowances),
            new SqlParameter("@gs",  dto.GrossSalary),
            new SqlParameter("@epf", dto.EPF),
            new SqlParameter("@etf", dto.ETF),
            new SqlParameter("@tax", dto.Tax),
            new SqlParameter("@od",  dto.OtherDeductions),
            new SqlParameter("@ns",  dto.NetSalary),
            new SqlParameter("@rem", (object?)dto.Remarks ?? DBNull.Value),
            new SqlParameter("@id",  id));
        return rows == 0 ? NotFound() : NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var rows = await _db.NonQueryAsync("DELETE FROM PayrollRecords WHERE PayrollID=@id",
            new SqlParameter("@id", id));
        return rows == 0 ? NotFound() : NoContent();
    }

    private static PayrollRecord MapRecord(System.Data.DataRow r) => new()
    {
        PayrollID       = DatabaseHelper.Get<int>(r, "PayrollID"),
        EmployeeID      = DatabaseHelper.Get<int>(r, "EmployeeID"),
        EmployeeName    = DatabaseHelper.GetString(r, "EmployeeName"),
        Department      = DatabaseHelper.GetString(r, "Department"),
        PayMonth        = DatabaseHelper.Get<int>(r, "PayMonth"),
        PayYear         = DatabaseHelper.Get<int>(r, "PayYear"),
        BasicSalary     = DatabaseHelper.Get<decimal>(r, "BasicSalary"),
        Allowances      = DatabaseHelper.Get<decimal>(r, "Allowances"),
        GrossSalary     = DatabaseHelper.Get<decimal>(r, "GrossSalary"),
        EPF             = DatabaseHelper.Get<decimal>(r, "EPF"),
        ETF             = DatabaseHelper.Get<decimal>(r, "ETF"),
        Tax             = DatabaseHelper.Get<decimal>(r, "Tax"),
        OtherDeductions = DatabaseHelper.Get<decimal>(r, "OtherDeductions"),
        NetSalary       = DatabaseHelper.Get<decimal>(r, "NetSalary"),
        Remarks         = DatabaseHelper.GetString(r, "Remarks"),
        CreatedDate     = DatabaseHelper.Get<DateTime>(r, "CreatedDate")
    };
}
