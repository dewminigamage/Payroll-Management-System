using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PayrollManagementSystem.API.Helpers;

namespace PayrollManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly DatabaseHelper _db;
    public ReportsController(DatabaseHelper db) => _db = db;

    [HttpGet("payroll-summary")]
    public async Task<IActionResult> PayrollSummary([FromQuery] int month, [FromQuery] int year)
    {
        var dt = await _db.QueryAsync(
            @"SELECT pr.*, e.FullName AS EmployeeName, e.Department, e.Position
              FROM PayrollRecords pr JOIN Employees e ON e.EmployeeID=pr.EmployeeID
              WHERE pr.PayMonth=@m AND pr.PayYear=@y
              ORDER BY e.Department, e.FullName",
            new SqlParameter("@m", month),
            new SqlParameter("@y", year));

        var rows = dt.Rows.Cast<System.Data.DataRow>().Select(r => new
        {
            payrollId       = DatabaseHelper.Get<int>(r, "PayrollID"),
            employeeId      = DatabaseHelper.Get<int>(r, "EmployeeID"),
            employeeName    = DatabaseHelper.GetString(r, "EmployeeName"),
            department      = DatabaseHelper.GetString(r, "Department"),
            position        = DatabaseHelper.GetString(r, "Position"),
            basicSalary     = DatabaseHelper.Get<decimal>(r, "BasicSalary"),
            allowances      = DatabaseHelper.Get<decimal>(r, "Allowances"),
            grossSalary     = DatabaseHelper.Get<decimal>(r, "GrossSalary"),
            epf             = DatabaseHelper.Get<decimal>(r, "EPF"),
            etf             = DatabaseHelper.Get<decimal>(r, "ETF"),
            tax             = DatabaseHelper.Get<decimal>(r, "Tax"),
            otherDeductions = DatabaseHelper.Get<decimal>(r, "OtherDeductions"),
            netSalary       = DatabaseHelper.Get<decimal>(r, "NetSalary"),
            remarks         = DatabaseHelper.GetString(r, "Remarks")
        }).ToList();

        var totals = new
        {
            basicSalary     = rows.Sum(r => r.basicSalary),
            allowances      = rows.Sum(r => r.allowances),
            grossSalary     = rows.Sum(r => r.grossSalary),
            epf             = rows.Sum(r => r.epf),
            etf             = rows.Sum(r => r.etf),
            tax             = rows.Sum(r => r.tax),
            otherDeductions = rows.Sum(r => r.otherDeductions),
            netSalary       = rows.Sum(r => r.netSalary)
        };

        return Ok(new { month, year, records = rows, totals });
    }

    [HttpGet("department-summary")]
    public async Task<IActionResult> DeptSummary([FromQuery] int month, [FromQuery] int year)
    {
        var dt = await _db.QueryAsync(
            @"SELECT e.Department, COUNT(*) AS HeadCount,
                     SUM(pr.BasicSalary) AS TotalBasic, SUM(pr.Allowances) AS TotalAllowances,
                     SUM(pr.GrossSalary) AS TotalGross, SUM(pr.EPF) AS TotalEPF,
                     SUM(pr.NetSalary) AS TotalNet
              FROM PayrollRecords pr JOIN Employees e ON e.EmployeeID=pr.EmployeeID
              WHERE pr.PayMonth=@m AND pr.PayYear=@y
              GROUP BY e.Department ORDER BY e.Department",
            new SqlParameter("@m", month),
            new SqlParameter("@y", year));

        return Ok(dt.Rows.Cast<System.Data.DataRow>().Select(r => new
        {
            department      = DatabaseHelper.GetString(r, "Department") ?? "Unassigned",
            headCount       = DatabaseHelper.Get<int>(r, "HeadCount"),
            totalBasic      = DatabaseHelper.Get<decimal>(r, "TotalBasic"),
            totalAllowances = DatabaseHelper.Get<decimal>(r, "TotalAllowances"),
            totalGross      = DatabaseHelper.Get<decimal>(r, "TotalGross"),
            totalEPF        = DatabaseHelper.Get<decimal>(r, "TotalEPF"),
            totalNet        = DatabaseHelper.Get<decimal>(r, "TotalNet")
        }));
    }

    [HttpGet("attendance-summary")]
    public async Task<IActionResult> AttendanceSummary([FromQuery] int month, [FromQuery] int year)
    {
        var dt = await _db.QueryAsync(
            @"SELECT e.FullName, e.Department,
                     SUM(CASE WHEN a.Status='Present'  THEN 1 ELSE 0 END) AS Present,
                     SUM(CASE WHEN a.Status='Absent'   THEN 1 ELSE 0 END) AS Absent,
                     SUM(CASE WHEN a.Status='Late'     THEN 1 ELSE 0 END) AS Late,
                     SUM(CASE WHEN a.Status='Half Day' THEN 1 ELSE 0 END) AS HalfDay,
                     SUM(CASE WHEN a.Status='Leave'    THEN 1 ELSE 0 END) AS OnLeave
              FROM Attendance a JOIN Employees e ON e.EmployeeID=a.EmployeeID
              WHERE MONTH(a.AttendanceDate)=@m AND YEAR(a.AttendanceDate)=@y
              GROUP BY e.FullName, e.Department ORDER BY e.FullName",
            new SqlParameter("@m", month),
            new SqlParameter("@y", year));

        return Ok(dt.Rows.Cast<System.Data.DataRow>().Select(r => new
        {
            employeeName = DatabaseHelper.GetString(r, "FullName"),
            department   = DatabaseHelper.GetString(r, "Department"),
            present      = DatabaseHelper.Get<int>(r, "Present"),
            absent       = DatabaseHelper.Get<int>(r, "Absent"),
            late         = DatabaseHelper.Get<int>(r, "Late"),
            halfDay      = DatabaseHelper.Get<int>(r, "HalfDay"),
            onLeave      = DatabaseHelper.Get<int>(r, "OnLeave")
        }));
    }

    [HttpGet("overtime-summary")]
    public async Task<IActionResult> OvertimeSummary([FromQuery] int month, [FromQuery] int year)
    {
        var dt = await _db.QueryAsync(
            @"SELECT e.FullName, e.Department, ot.OTHours, ot.OTRateMultiplier, ot.OTAmount
              FROM OvertimeRecords ot JOIN Employees e ON e.EmployeeID=ot.EmployeeID
              WHERE ot.PayMonth=@m AND ot.PayYear=@y ORDER BY ot.OTAmount DESC",
            new SqlParameter("@m", month),
            new SqlParameter("@y", year));

        return Ok(dt.Rows.Cast<System.Data.DataRow>().Select(r => new
        {
            employeeName     = DatabaseHelper.GetString(r, "FullName"),
            department       = DatabaseHelper.GetString(r, "Department"),
            otHours          = DatabaseHelper.Get<decimal>(r, "OTHours"),
            otRateMultiplier = DatabaseHelper.Get<decimal>(r, "OTRateMultiplier"),
            otAmount         = DatabaseHelper.Get<decimal>(r, "OTAmount")
        }));
    }
}
