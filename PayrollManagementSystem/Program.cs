using PayrollManagementSystem.Forms;
using PayrollManagementSystem.Models;

namespace PayrollManagementSystem;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Show login, then main — loop supports logout-and-re-login
        while (true)
        {
            using var login = new frmLogin();
            if (login.ShowDialog() != DialogResult.OK) break;

            using var main = new frmMain();
            Application.Run(main);

            if (!UserSession.LogoutRequested) break;
            UserSession.LogoutRequested = false;
            UserSession.Clear();
        }
    }
}
