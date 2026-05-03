using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PayrollManagementSystem.API.Helpers;
using PayrollManagementSystem.API.Models;

namespace PayrollManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly DatabaseHelper _db;
    public EmployeesController(DatabaseHelper db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = false)
    {
        var sql = activeOnly
            ? "SELECT * FROM Employees WHERE IsActive=1 ORDER BY FullName"
            : "SELECT * FROM Employees ORDER BY FullName";
        var dt = await _db.QueryAsync(sql);
        return Ok(dt.Rows.Cast<System.Data.DataRow>().Select(MapEmployee));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dt = await _db.QueryAsync("SELECT * FROM Employees WHERE EmployeeID=@id",
            new SqlParameter("@id", id));
        if (dt.Rows.Count == 0) return NotFound();
        return Ok(MapEmployee(dt.Rows[0]));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
    {
        var id = await _db.ScalarAsync(
            @"INSERT INTO Employees (FullName,NIC,Department,Position,BasicSalary,JoinDate,ContactNumber,Email,IsActive)
              VALUES (@fn,@nic,@dept,@pos,@sal,@jd,@cn,@em,1);
              SELECT SCOPE_IDENTITY();",
            new SqlParameter("@fn",   dto.FullName),
            new SqlParameter("@nic",  dto.NIC),
            new SqlParameter("@dept", (object?)dto.Department    ?? DBNull.Value),
            new SqlParameter("@pos",  (object?)dto.Position      ?? DBNull.Value),
            new SqlParameter("@sal",  dto.BasicSalary),
            new SqlParameter("@jd",   dto.JoinDate.ToDateTime(TimeOnly.MinValue)),
            new SqlParameter("@cn",   (object?)dto.ContactNumber ?? DBNull.Value),
            new SqlParameter("@em",   (object?)dto.Email         ?? DBNull.Value));

        var newId = Convert.ToInt32(id);
        return await GetById(newId) is OkObjectResult ok
            ? CreatedAtAction(nameof(GetById), new { id = newId }, ok.Value)
            : StatusCode(500);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto)
    {
        var rows = await _db.NonQueryAsync(
            @"UPDATE Employees SET FullName=@fn,NIC=@nic,Department=@dept,Position=@pos,
              BasicSalary=@sal,JoinDate=@jd,ContactNumber=@cn,Email=@em,IsActive=@active
              WHERE EmployeeID=@id",
            new SqlParameter("@fn",     dto.FullName),
            new SqlParameter("@nic",    dto.NIC),
            new SqlParameter("@dept",   (object?)dto.Department    ?? DBNull.Value),
            new SqlParameter("@pos",    (object?)dto.Position      ?? DBNull.Value),
            new SqlParameter("@sal",    dto.BasicSalary),
            new SqlParameter("@jd",     dto.JoinDate.ToDateTime(TimeOnly.MinValue)),
            new SqlParameter("@cn",     (object?)dto.ContactNumber ?? DBNull.Value),
            new SqlParameter("@em",     (object?)dto.Email         ?? DBNull.Value),
            new SqlParameter("@active", dto.IsActive),
            new SqlParameter("@id",     id));
        return rows == 0 ? NotFound() : NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var rows = await _db.NonQueryAsync(
            "UPDATE Employees SET IsActive=0 WHERE EmployeeID=@id",
            new SqlParameter("@id", id));
        return rows == 0 ? NotFound() : NoContent();
    }

    private static Employee MapEmployee(System.Data.DataRow r) => new()
    {
        EmployeeID    = DatabaseHelper.Get<int>(r, "EmployeeID"),
        FullName      = DatabaseHelper.Get<string>(r, "FullName")!,
        NIC           = DatabaseHelper.Get<string>(r, "NIC")!,
        Department    = DatabaseHelper.GetString(r, "Department"),
        Position      = DatabaseHelper.GetString(r, "Position"),
        BasicSalary   = DatabaseHelper.Get<decimal>(r, "BasicSalary"),
        JoinDate      = DateOnly.FromDateTime(DatabaseHelper.Get<DateTime>(r, "JoinDate")),
        ContactNumber = DatabaseHelper.GetString(r, "ContactNumber"),
        Email         = DatabaseHelper.GetString(r, "Email"),
        IsActive      = DatabaseHelper.Get<bool>(r, "IsActive")
    };
}
