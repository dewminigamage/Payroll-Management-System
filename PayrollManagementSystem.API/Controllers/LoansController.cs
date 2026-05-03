using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PayrollManagementSystem.API.Helpers;
using PayrollManagementSystem.API.Models;

namespace PayrollManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LoansController : ControllerBase
{
    private readonly DatabaseHelper _db;
    public LoansController(DatabaseHelper db) => _db = db;

    [HttpGet("types")]
    public async Task<IActionResult> GetTypes()
    {
        var dt = await _db.QueryAsync("SELECT * FROM LoanTypes WHERE IsActive=1 ORDER BY TypeName");
        return Ok(dt.Rows.Cast<System.Data.DataRow>().Select(r => new LoanType
        {
            LoanTypeID = DatabaseHelper.Get<int>(r, "LoanTypeID"),
            TypeName   = DatabaseHelper.Get<string>(r, "TypeName")!,
            IsActive   = DatabaseHelper.Get<bool>(r, "IsActive")
        }));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? employeeId, [FromQuery] string? status)
    {
        var sql = @"SELECT el.*, e.FullName AS EmployeeName, lt.TypeName AS LoanTypeName
                    FROM EmployeeLoans el
                    JOIN Employees e  ON e.EmployeeID   = el.EmployeeID
                    JOIN LoanTypes lt ON lt.LoanTypeID  = el.LoanTypeID
                    WHERE 1=1";
        var ps = new List<SqlParameter>();
        if (employeeId.HasValue) { sql += " AND el.EmployeeID=@eid"; ps.Add(new("@eid", employeeId)); }
        if (!string.IsNullOrEmpty(status)) { sql += " AND el.Status=@st"; ps.Add(new("@st", status)); }
        sql += " ORDER BY el.CreatedDate DESC";
        var dt = await _db.QueryAsync(sql, [.. ps]);
        return Ok(dt.Rows.Cast<System.Data.DataRow>().Select(MapLoan));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dt = await _db.QueryAsync(
            @"SELECT el.*, e.FullName AS EmployeeName, lt.TypeName AS LoanTypeName
              FROM EmployeeLoans el
              JOIN Employees e  ON e.EmployeeID  = el.EmployeeID
              JOIN LoanTypes lt ON lt.LoanTypeID = el.LoanTypeID
              WHERE el.LoanID=@id",
            new SqlParameter("@id", id));
        if (dt.Rows.Count == 0) return NotFound();
        return Ok(MapLoan(dt.Rows[0]));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateLoanDto dto)
    {
        var id = await _db.ScalarAsync(
            @"INSERT INTO EmployeeLoans (EmployeeID,LoanTypeID,LoanAmount,RemainingBalance,MonthlyInstallment,StartMonth,StartYear,Status,Notes)
              VALUES (@eid,@ltid,@amt,@amt,@inst,@sm,@sy,'Active',@notes); SELECT SCOPE_IDENTITY();",
            new SqlParameter("@eid",   dto.EmployeeID),
            new SqlParameter("@ltid",  dto.LoanTypeID),
            new SqlParameter("@amt",   dto.LoanAmount),
            new SqlParameter("@inst",  dto.MonthlyInstallment),
            new SqlParameter("@sm",    dto.StartMonth),
            new SqlParameter("@sy",    dto.StartYear),
            new SqlParameter("@notes", (object?)dto.Notes ?? DBNull.Value));
        var newId = Convert.ToInt32(id);
        return await GetById(newId) is OkObjectResult ok
            ? CreatedAtAction(nameof(GetById), new { id = newId }, ok.Value)
            : StatusCode(500);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateLoanDto dto)
    {
        var rows = await _db.NonQueryAsync(
            "UPDATE EmployeeLoans SET RemainingBalance=@bal, Status=@st, Notes=@notes WHERE LoanID=@id",
            new SqlParameter("@bal",   dto.RemainingBalance),
            new SqlParameter("@st",    dto.Status),
            new SqlParameter("@notes", (object?)dto.Notes ?? DBNull.Value),
            new SqlParameter("@id",    id));
        return rows == 0 ? NotFound() : NoContent();
    }

    [HttpPost("{id}/repayments")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RecordRepayment(int id, [FromBody] RecordRepaymentDto dto)
    {
        var loanDt = await _db.QueryAsync("SELECT RemainingBalance FROM EmployeeLoans WHERE LoanID=@id AND Status='Active'",
            new SqlParameter("@id", id));
        if (loanDt.Rows.Count == 0) return NotFound(new { message = "Active loan not found" });

        var balance = DatabaseHelper.Get<decimal>(loanDt.Rows[0], "RemainingBalance");
        var newBal  = balance - dto.AmountPaid;
        var status  = newBal <= 0 ? "Completed" : "Active";

        await _db.NonQueryAsync(
            @"INSERT INTO LoanRepayments (LoanID,PayMonth,PayYear,AmountPaid,Notes) VALUES (@lid,@m,@y,@amt,@notes);
              UPDATE EmployeeLoans SET RemainingBalance=@bal, Status=@st WHERE LoanID=@lid;",
            new SqlParameter("@lid",   id),
            new SqlParameter("@m",     dto.PayMonth),
            new SqlParameter("@y",     dto.PayYear),
            new SqlParameter("@amt",   dto.AmountPaid),
            new SqlParameter("@notes", (object?)dto.Notes ?? DBNull.Value),
            new SqlParameter("@bal",   Math.Max(0, newBal)),
            new SqlParameter("@st",    status));

        return Ok(new { newBalance = Math.Max(0, newBal), status });
    }

    [HttpGet("{id}/repayments")]
    public async Task<IActionResult> GetRepayments(int id)
    {
        var dt = await _db.QueryAsync(
            "SELECT * FROM LoanRepayments WHERE LoanID=@id ORDER BY PayYear DESC, PayMonth DESC",
            new SqlParameter("@id", id));
        return Ok(dt.Rows.Cast<System.Data.DataRow>().Select(r => new LoanRepayment
        {
            RepaymentID = DatabaseHelper.Get<int>(r, "RepaymentID"),
            LoanID      = DatabaseHelper.Get<int>(r, "LoanID"),
            PayMonth    = DatabaseHelper.Get<int>(r, "PayMonth"),
            PayYear     = DatabaseHelper.Get<int>(r, "PayYear"),
            AmountPaid  = DatabaseHelper.Get<decimal>(r, "AmountPaid"),
            Notes       = DatabaseHelper.GetString(r, "Notes"),
            CreatedDate = DatabaseHelper.Get<DateTime>(r, "CreatedDate")
        }));
    }

    private static EmployeeLoan MapLoan(System.Data.DataRow r) => new()
    {
        LoanID             = DatabaseHelper.Get<int>(r, "LoanID"),
        EmployeeID         = DatabaseHelper.Get<int>(r, "EmployeeID"),
        EmployeeName       = DatabaseHelper.GetString(r, "EmployeeName"),
        LoanTypeID         = DatabaseHelper.Get<int>(r, "LoanTypeID"),
        LoanTypeName       = DatabaseHelper.GetString(r, "LoanTypeName"),
        LoanAmount         = DatabaseHelper.Get<decimal>(r, "LoanAmount"),
        RemainingBalance   = DatabaseHelper.Get<decimal>(r, "RemainingBalance"),
        MonthlyInstallment = DatabaseHelper.Get<decimal>(r, "MonthlyInstallment"),
        StartMonth         = DatabaseHelper.Get<int>(r, "StartMonth"),
        StartYear          = DatabaseHelper.Get<int>(r, "StartYear"),
        Status             = DatabaseHelper.Get<string>(r, "Status")!,
        Notes              = DatabaseHelper.GetString(r, "Notes"),
        CreatedDate        = DatabaseHelper.Get<DateTime>(r, "CreatedDate")
    };
}
