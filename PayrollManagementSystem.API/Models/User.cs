namespace PayrollManagementSystem.API.Models;

public class User
{
    public int      UserID      { get; set; }
    public string   Username    { get; set; } = "";
    public string   FullName    { get; set; } = "";
    public string   Role        { get; set; } = "Viewer";
    public bool     IsActive    { get; set; }
    public DateTime CreatedDate { get; set; }
}

public record LoginDto(string Username, string Password);

public record LoginResponseDto(
    string   Token,
    string   Username,
    string   FullName,
    string   Role,
    DateTime ExpiresAt
);

public record CreateUserDto(
    string Username,
    string Password,
    string FullName,
    string Role
);

public record UpdateUserDto(
    string FullName,
    string Role,
    bool   IsActive
);

public record ChangePasswordDto(string CurrentPassword, string NewPassword);

public record AdminResetPasswordDto(string NewPassword);
