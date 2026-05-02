using PayrollManagementSystem.Database;
using PayrollManagementSystem.Helpers;
using PayrollManagementSystem.Models;

namespace PayrollManagementSystem.Forms;

public partial class frmLogin : Form
{
    public frmLogin()
    {
        InitializeComponent();
    }

    private void btnLogin_Click(object sender, EventArgs e)
    {
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowError("Please enter your username and password.");
            return;
        }

        try
        {
            string hash = PasswordHelper.Hash(password);

            var dt = DatabaseHelper.ExecuteQuery(
                @"SELECT UserID, Username, FullName, Role
                  FROM   Users
                  WHERE  Username = @Username
                    AND  PasswordHash = @Hash
                    AND  IsActive = 1",
                [
                    new("@Username", username),
                    new("@Hash",     hash)
                ]);

            if (dt.Rows.Count == 0)
            {
                ShowError("Invalid username or password.");
                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            var row = dt.Rows[0];
            UserSession.UserID   = Convert.ToInt32(row["UserID"]);
            UserSession.Username = row["Username"].ToString()!;
            UserSession.FullName = row["FullName"].ToString()!;
            UserSession.Role     = row["Role"].ToString()!;

            lblError.Text  = "";
            DialogResult   = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            ShowError($"Login error: {ex.Message}");
        }
    }

    private void ShowError(string message)
    {
        lblError.Text = message;
    }

    // Enter on username moves focus to password
    private void txtUsername_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            e.Handled = true;
            txtPassword.Focus();
        }
    }
}
