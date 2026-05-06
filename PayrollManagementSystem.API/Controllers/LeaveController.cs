using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PayrollManagementSystem.API.Helpers;
using PayrollManagementSystem.API.Models;

namespace PayrollManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaveController : ControllerBase
{
    private readonly DatabaseHelper _db;
    public LeaveController(DatabaseHelper db) => _db = db;

    [HttpGet("types")]
    public async Task<IActionResult> GetTypes()
    {
        var dt = await _db.QueryAsync("SELECT * FROM LeaveTypes ORDER BY TypeName");
        return Ok(dt.Rows.Cast<System.Data.DataRow>().Select(r => new LeaveType
        {
            LeaveTypeID        = DatabaseHelper.Get<int>(r, "LeaveTypeID"),
            TypeName           = DatabaseHelper.Get<string>(r, "TypeName")!,
            DefaultDaysPerYear = DatabaseHelper.Get<int>(r, "DefaultDaysPerYear"),
            Description        = DatabaseHelper.GetString(r, "Description")
        }));
    }

    [HttpGet("balances")]
    public async Task<IActionResult> GetBalances([FromQuery] int? employeeId, [FromQuery] int? year)
    {
        var y   = year ?? DateTime.Now.Year;
        var sql = @"SELECT lb.*, e.FullName AS EmployeeName, lt.TypeName
                    FROM LeaveBalances lb
                    JOIN Employees  e  ON e.EmployeeID  = lb.EmployeeID
                    JOIN LeaveTypes lt ON lt.LeaveTypeID = lb.LeaveTypeID
                    WHERE lb.Year=@y";
        var ps = new List<SqlParameter> { new("@y", y) };
        if (employeeId.HasValue) { sql += " AND lb.EmployeeID=@eid"; ps.Add(new("@eid", employeeId)); }
        sql += " ORDER BY e.FullName, lt.TypeName";
        var dt = await _db.QueryAsync(sql, [.. ps]);
        return Ok(dt.Rows.Cast<System.Data.DataRow>().Select(r => new LeaveBalance
        {
            BalanceID   = DatabaseHelper.Get<int>(r, "BalanceID"),
            EmployeeID  = DatabaseHelper.Get<int>(r, "EmployeeID"),
            EmployeeName = DatabaseHelper.GetString(r, "EmployeeName"),
            LeaveTypeID = DatabaseHelper.Get<int>(r, "LeaveTypeID"),
            TypeName    = DatabaseHelper.GetString(r, "TypeName"),
            Year        = DatabaseHelper.Get<int>(r, "Year"),
            TotalDays   = DatabaseHelper.Get<int>(r, "TotalDays"),
            UsedDays    = DatabaseHelper.Get<int>(r, "UsedDays")
        }));
    }

    [HttpGet("requests")]
    public async Task<IActionResult> GetRequests([FromQuery] int? employeeId, [FromQuery] string? status)
    {
        var sql = @"SELECT lr.*, e.FullName AS EmployeeName, lt.TypeName
                    FROM LeaveRequests lr
                    JOIN Employees  e  ON e.EmployeeID  = lr.EmployeeID
                    JOIN LeaveTypes lt ON lt.LeaveTypeID = lr.LeaveTypeID
                    WHERE 1=1";
        var ps = new List<SqlParameter>();
        if (employeeId.HasValue)          { sql += " AND lr.EmployeeID=@eid"; ps.Add(new("@eid", employeeId)); }
        if (!string.IsNullOrEmpty(status)) { sql += " AND lr.Status=@st";     ps.Add(new("@st", status)); }
        sql += " ORDER BY lr.CreatedDate DESC";
        var dt = await _db.QueryAsync(sql, [.. ps]);
        return Ok(dt.Rows.Cast<System.Data.DataRow>().Select(MapRequest));
    }

    [HttpPost("requests")]
    public async Task<IActionResult> CreateRequest([FromBody] CreateLeaveRequestDto dto)
    {
        var id = await _db.ScalarAsync(
            @"INSERT INTO LeaveRequests (EmployeeID,LeaveTypeID,StartDate,EndDate,TotalDays,Reason,Status)
              VALUES (@eid,@ltid,@sd,@ed,@td,@reason,'Pending'); SELECT SCOPE_IDENTITY();",
            new SqlParameter("@eid",    dto.EmployeeID),
            new SqlParameter("@ltid",   dto.LeaveTypeID),
            new SqlParameter("@sd",     dto.StartDate.ToDateTime(TimeOnly.MinValue)),
            new SqlParameter("@ed",     dto.EndDate.ToDateTime(TimeOnly.MinValue)),
            new SqlParameter("@td",     dto.TotalDays),
            new SqlParameter("@reason", (object?)dto.Reason ?? DBNull.Value));
        return Ok(new { requestId = Convert.ToInt32(id) });
    }

    [HttpPut("requests/{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(int id, [FromBody] ApproveLeaveDto dto)
    {
        var approver = User.Identity?.Name ?? "Admin";
        var rows = await _db.NonQueryAsync(
            @"UPDATE LeaveRequests SET Status=@st, ApprovedBy=@by, ApprovedDate=GETDATE(), Remarks=@rem
              WHERE RequestID=@id",
            new SqlParameter("@st",  dto.Status),
            new SqlParameter("@by",  approver),
            new SqlParameter("@rem", (object?)dto.Remarks ?? DBNull.Value),
            new SqlParameter("@id",  id));
        if (rows == 0) return NotFound();

        if (dto.Status == "Approved")
        {
            var reqDt = await _db.QueryAsync("SELECT * FROM LeaveRequests WHERE RequestID=@id",
                new SqlParameter("@id", id));
            if (reqDt.Rows.Count > 0)
            {
                var r       = reqDt.Rows[0];
                var eid     = DatabaseHelper.Get<int>(r, "EmployeeID");
                var ltid    = DatabaseHelper.Get<int>(r, "LeaveTypeID");
                var days    = DatabaseHelper.Get<int>(r, "TotalDays");
                var year    = DatabaseHelper.Get<DateTime>(r, "StartDate").Year;
                await _db.NonQueryAsync(
                    @"IF EXISTS (SELECT 1 FROM LeaveBalances WHERE EmployeeID=@eid AND LeaveTypeID=@ltid AND Year=@yr)
                          UPDATE LeaveBalances SET UsedDays=UsedDays+@d WHERE EmployeeID=@eid AND LeaveTypeID=@ltid AND Year=@yr;",
                    new SqlParameter("@eid",  eid),
                    new SqlParameter("@ltid", ltid),
                    new SqlParameter("@yr",   year),
                    new SqlParameter("@d",    days));
            }
        }
        return NoContent();
    }

    private static LeaveRequest MapRequest(System.Data.DataRow r) => new()
    {
        RequestID    = DatabaseHelper.Get<int>(r, "RequestID"),
        EmployeeID   = DatabaseHelper.Get<int>(r, "EmployeeID"),
        EmployeeName = DatabaseHelper.GetString(r, "EmployeeName"),
        LeaveTypeID  = DatabaseHelper.Get<int>(r, "LeaveTypeID"),
        TypeName     = DatabaseHelper.GetString(r, "TypeName"),
        StartDate    = DateOnly.FromDateTime(DatabaseHelper.Get<DateTime>(r, "StartDate")),
        EndDate      = DateOnly.FromDateTime(DatabaseHelper.Get<DateTime>(r, "EndDate")),
        TotalDays    = DatabaseHelper.Get<int>(r, "TotalDays"),
        Reason       = DatabaseHelper.GetString(r, "Reason"),
        Status       = DatabaseHelper.Get<string>(r, "Status")!,
        ApprovedBy   = DatabaseHelper.GetString(r, "ApprovedBy"),
        ApprovedDate = DatabaseHelper.GetNullable<DateTime>(r, "ApprovedDate"),
        Remarks      = DatabaseHelper.GetString(r, "Remarks"),
        CreatedDate  = DatabaseHelper.Get<DateTime>(r, "CreatedDate")
    };
}
