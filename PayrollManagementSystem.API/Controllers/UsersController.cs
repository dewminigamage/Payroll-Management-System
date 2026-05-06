using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PayrollManagementSystem.API.Helpers;
using PayrollManagementSystem.API.Models;

namespace PayrollManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly DatabaseHelper _db;
    public UsersController(DatabaseHelper db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var dt = await _db.QueryAsync("SELECT UserID,Username,FullName,Role,IsActive,CreatedDate FROM Users ORDER BY Username");
        return Ok(dt.Rows.Cast<System.Data.DataRow>().Select(MapUser));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dt = await _db.QueryAsync(
            "SELECT UserID,Username,FullName,Role,IsActive,CreatedDate FROM Users WHERE UserID=@id",
            new SqlParameter("@id", id));
        if (dt.Rows.Count == 0) return NotFound();
        return Ok(MapUser(dt.Rows[0]));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var exists = await _db.ScalarAsync("SELECT COUNT(1) FROM Users WHERE Username=@u",
            new SqlParameter("@u", dto.Username));
        if (Convert.ToInt32(exists) > 0)
            return Conflict(new { message = "Username already exists" });

        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(dto.Password))).ToLower();
        var id = await _db.ScalarAsync(
            @"INSERT INTO Users (Username,PasswordHash,FullName,Role,IsActive)
              VALUES (@u,@h,@fn,@role,1); SELECT SCOPE_IDENTITY();",
            new SqlParameter("@u",    dto.Username),
            new SqlParameter("@h",    hash),
            new SqlParameter("@fn",   dto.FullName),
            new SqlParameter("@role", dto.Role));
        var newId = Convert.ToInt32(id);
        return await GetById(newId) is OkObjectResult ok
            ? CreatedAtAction(nameof(GetById), new { id = newId }, ok.Value)
            : StatusCode(500);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        var rows = await _db.NonQueryAsync(
            "UPDATE Users SET FullName=@fn, Role=@role, IsActive=@active WHERE UserID=@id",
            new SqlParameter("@fn",     dto.FullName),
            new SqlParameter("@role",   dto.Role),
            new SqlParameter("@active", dto.IsActive),
            new SqlParameter("@id",     id));
        return rows == 0 ? NotFound() : NoContent();
    }

    [HttpPut("{id}/password")]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] AdminResetPasswordDto dto)
    {
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(dto.NewPassword))).ToLower();
        var rows = await _db.NonQueryAsync(
            "UPDATE Users SET PasswordHash=@h WHERE UserID=@id",
            new SqlParameter("@h",  hash),
            new SqlParameter("@id", id));
        return rows == 0 ? NotFound() : NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var rows = await _db.NonQueryAsync("UPDATE Users SET IsActive=0 WHERE UserID=@id",
            new SqlParameter("@id", id));
        return rows == 0 ? NotFound() : NoContent();
    }

    private static User MapUser(System.Data.DataRow r) => new()
    {
        UserID      = DatabaseHelper.Get<int>(r, "UserID"),
        Username    = DatabaseHelper.Get<string>(r, "Username")!,
        FullName    = DatabaseHelper.Get<string>(r, "FullName")!,
        Role        = DatabaseHelper.Get<string>(r, "Role")!,
        IsActive    = DatabaseHelper.Get<bool>(r, "IsActive"),
        CreatedDate = DatabaseHelper.Get<DateTime>(r, "CreatedDate")
    };
}
