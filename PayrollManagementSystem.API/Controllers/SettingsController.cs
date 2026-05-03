using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PayrollManagementSystem.API.Helpers;

namespace PayrollManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly DatabaseHelper _db;
    public SettingsController(DatabaseHelper db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var dt = await _db.QueryAsync("SELECT SettingKey, SettingValue FROM CompanySettings ORDER BY SettingKey");
        var dict = dt.Rows.Cast<System.Data.DataRow>()
            .ToDictionary(
                r => DatabaseHelper.Get<string>(r, "SettingKey")!,
                r => DatabaseHelper.GetString(r, "SettingValue"));
        return Ok(dict);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAll([FromBody] Dictionary<string, string> settings)
    {
        foreach (var (key, value) in settings)
        {
            await _db.NonQueryAsync(
                @"IF EXISTS (SELECT 1 FROM CompanySettings WHERE SettingKey=@k)
                      UPDATE CompanySettings SET SettingValue=@v WHERE SettingKey=@k
                  ELSE
                      INSERT INTO CompanySettings (SettingKey, SettingValue) VALUES (@k, @v)",
                new SqlParameter("@k", key),
                new SqlParameter("@v", value));
        }
        return NoContent();
    }

    [HttpPut("{key}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(string key, [FromBody] string value)
    {
        await _db.NonQueryAsync(
            @"IF EXISTS (SELECT 1 FROM CompanySettings WHERE SettingKey=@k)
                  UPDATE CompanySettings SET SettingValue=@v WHERE SettingKey=@k
              ELSE
                  INSERT INTO CompanySettings (SettingKey, SettingValue) VALUES (@k, @v)",
            new SqlParameter("@k", key),
            new SqlParameter("@v", value));
        return NoContent();
    }
}
