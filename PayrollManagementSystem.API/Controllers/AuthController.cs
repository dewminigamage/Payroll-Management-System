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
public class AuthController : ControllerBase
{
    private readonly DatabaseHelper _db;
    private readonly JwtHelper      _jwt;

    public AuthController(DatabaseHelper db, JwtHelper jwt) { _db = db; _jwt = jwt; }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(dto.Password))).ToLower();

        var dt = await _db.QueryAsync(
            "SELECT UserID, Username, FullName, Role FROM Users WHERE Username=@u AND PasswordHash=@h AND IsActive=1",
            new SqlParameter("@u", dto.Username),
            new SqlParameter("@h", hash));

        if (dt.Rows.Count == 0) return Unauthorized(new { message = "Invalid credentials" });

        var row  = dt.Rows[0];
        var uid  = DatabaseHelper.Get<int>(row, "UserID");
        var un   = DatabaseHelper.Get<string>(row, "Username")!;
        var fn   = DatabaseHelper.Get<string>(row, "FullName")!;
        var role = DatabaseHelper.Get<string>(row, "Role")!;

        var (token, exp) = _jwt.GenerateToken(uid, un, role);
        return Ok(new LoginResponseDto(token, un, fn, role, exp));
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var uid  = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        var un   = User.Identity?.Name;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        return Ok(new { userId = uid, username = un, role });
    }
}
