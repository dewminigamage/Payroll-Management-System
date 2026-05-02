namespace PayrollManagementSystem.Models;

public static class UserSession
{
    public static int    UserID          { get; set; }
    public static string Username        { get; set; } = "";
    public static string FullName        { get; set; } = "";
    public static string Role            { get; set; } = "";   // "Admin" | "Viewer"
    public static bool   LogoutRequested { get; set; }

    public static bool IsAdmin => Role == "Admin";

    public static void Clear()
    {
        UserID   = 0;
        Username = "";
        FullName = "";
        Role     = "";
    }
}
