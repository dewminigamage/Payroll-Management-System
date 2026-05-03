using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollManagementSystem.API.Helpers;
using PayrollManagementSystem.API.Models;

namespace PayrollManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly DatabaseHelper _db;
    public DashboardController(DatabaseHelper db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var now   = DateTime.Now;
        var month = now.Month;
        var year  = now.Year;

        var summary = new DashboardSummary { PayrollMonth = month, PayrollYear = year };

        var empDt = await _db.QueryAsync("SELECT COUNT(*) AS Total, SUM(CASE WHEN IsActive=1 THEN 1 ELSE 0 END) AS Active FROM Employees");
        if (empDt.Rows.Count > 0)
        {
            summary.TotalEmployees  = DatabaseHelper.Get<int>(empDt.Rows[0], "Total");
            summary.ActiveEmployees = DatabaseHelper.Get<int>(empDt.Rows[0], "Active");
        }

        var prDt = await _db.QueryAsync(
            "SELECT ISNULL(SUM(NetSalary),0) AS Total FROM PayrollRecords WHERE PayMonth=@m AND PayYear=@y",
            new Microsoft.Data.SqlClient.SqlParameter("@m", month),
            new Microsoft.Data.SqlClient.SqlParameter("@y", year));
        if (prDt.Rows.Count > 0)
            summary.MonthlyPayroll = DatabaseHelper.Get<decimal>(prDt.Rows[0], "Total");

        var leaveDt = await _db.QueryAsync("SELECT COUNT(*) AS Total FROM LeaveRequests WHERE Status='Pending'");
        if (leaveDt.Rows.Count > 0)
            summary.PendingLeave = DatabaseHelper.Get<int>(leaveDt.Rows[0], "Total");

        var loanDt = await _db.QueryAsync(
            "SELECT COUNT(*) AS Cnt, ISNULL(SUM(RemainingBalance),0) AS Bal FROM EmployeeLoans WHERE Status='Active'");
        if (loanDt.Rows.Count > 0)
        {
            summary.ActiveLoans      = DatabaseHelper.Get<int>(loanDt.Rows[0], "Cnt");
            summary.TotalLoanBalance = DatabaseHelper.Get<decimal>(loanDt.Rows[0], "Bal");
        }

        var deptDt = await _db.QueryAsync(
            "SELECT Department, COUNT(*) AS HC, SUM(BasicSalary) AS TS FROM Employees WHERE IsActive=1 GROUP BY Department ORDER BY Department");
        foreach (System.Data.DataRow r in deptDt.Rows)
            summary.ByDepartment.Add(new DeptSummary
            {
                Department  = DatabaseHelper.GetString(r, "Department") ?? "Unassigned",
                HeadCount   = DatabaseHelper.Get<int>(r, "HC"),
                TotalSalary = DatabaseHelper.Get<decimal>(r, "TS")
            });

        var actDt = await _db.QueryAsync(
            @"SELECT TOP 10 'Payroll processed for ' + CAST(PayMonth AS NVARCHAR) + '/' + CAST(PayYear AS NVARCHAR) AS Activity, CreatedDate
              FROM PayrollRecords ORDER BY CreatedDate DESC");
        foreach (System.Data.DataRow r in actDt.Rows)
            summary.RecentActivity.Add(new RecentActivity
            {
                Description = DatabaseHelper.Get<string>(r, "Activity")!,
                Date        = DatabaseHelper.Get<DateTime>(r, "CreatedDate")
            });

        return Ok(summary);
    }
}
