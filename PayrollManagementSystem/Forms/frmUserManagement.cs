using Microsoft.Data.SqlClient;
using PayrollManagementSystem.Database;
using PayrollManagementSystem.Helpers;
using PayrollManagementSystem.Models;

namespace PayrollManagementSystem.Forms;

public partial class frmUserManagement : Form
{
    private int _selectedUserID;

    public frmUserManagement()
    {
        InitializeComponent();
        LoadUsers();
    }

    // ── Data loading ───────────────────────────────────────────────────────

    private void LoadUsers()
    {
        var dt = DatabaseHelper.ExecuteQuery(@"
            SELECT UserID, Username, FullName, Role,
                   CASE WHEN IsActive=1 THEN 'Active' ELSE 'Inactive' END AS Status,
                   CONVERT(VARCHAR(12), CreatedDate, 106) AS Created
            FROM   Users
            ORDER  BY UserID");

        dgvUsers.DataSource = dt;

        if (dgvUsers.Columns.Count > 0)
        {
            dgvUsers.Columns["UserID"].HeaderText   = "ID";
            dgvUsers.Columns["UserID"].FillWeight    = 40;
            dgvUsers.Columns["Username"].FillWeight  = 100;
            dgvUsers.Columns["FullName"].HeaderText  = "Full Name";
            dgvUsers.Columns["FullName"].FillWeight  = 160;
            dgvUsers.Columns["Role"].FillWeight      = 70;
            dgvUsers.Columns["Status"].FillWeight    = 70;
            dgvUsers.Columns["Created"].FillWeight   = 90;
        }

        lblStatus.Text = $"  {dt.Rows.Count} user(s) loaded.";
    }

    // ── Grid click → populate form ─────────────────────────────────────────

    private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var row = dgvUsers.Rows[e.RowIndex];

        _selectedUserID        = Convert.ToInt32(row.Cells["UserID"].Value);
        txtUserID.Text         = _selectedUserID.ToString();
        txtUsername.Text       = row.Cells["Username"].Value?.ToString() ?? "";
        txtFullName.Text       = row.Cells["FullName"].Value?.ToString() ?? "";
        cboRole.SelectedItem   = row.Cells["Role"].Value?.ToString();
        chkIsActive.Checked    = row.Cells["Status"].Value?.ToString() == "Active";
        txtPassword.Clear();
        txtConfirmPassword.Clear();

        // Username read-only while editing an existing record
        txtUsername.ReadOnly  = true;
        txtUsername.BackColor = Color.FromArgb(235, 238, 242);

        lblStatus.Text = $"  Editing: {txtUsername.Text}";
    }

    // ── Add ────────────────────────────────────────────────────────────────

    private void btnAdd_Click(object sender, EventArgs e)
    {
        if (!ValidateInput(isAdd: true)) return;

        string username = txtUsername.Text.Trim();
        string fullName = txtFullName.Text.Trim();
        string role     = cboRole.SelectedItem?.ToString() ?? "Viewer";
        bool   isActive = chkIsActive.Checked;
        string pwHash   = PasswordHelper.Hash(txtPassword.Text);

        // Check duplicate username
        var existing = DatabaseHelper.ExecuteQuery(
            "SELECT 1 FROM Users WHERE Username = @Username",
            [new SqlParameter("@Username", username)]);

        if (existing.Rows.Count > 0)
        {
            MessageBox.Show("Username already exists. Choose a different one.",
                "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtUsername.Focus();
            return;
        }

        int rows = DatabaseHelper.ExecuteNonQuery(@"
            INSERT INTO Users (Username, PasswordHash, FullName, Role, IsActive)
            VALUES (@Username, @Hash, @FullName, @Role, @IsActive)",
            [
                new SqlParameter("@Username", username),
                new SqlParameter("@Hash",     pwHash),
                new SqlParameter("@FullName", fullName),
                new SqlParameter("@Role",     role),
                new SqlParameter("@IsActive", isActive)
            ]);

        if (rows > 0)
        {
            lblStatus.Text = $"  User '{username}' added successfully.";
            ClearForm();
            LoadUsers();
        }
    }

    // ── Update ─────────────────────────────────────────────────────────────

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        if (_selectedUserID == 0)
        {
            MessageBox.Show("Select a user from the list first.",
                "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!ValidateInput(isAdd: false)) return;

        string fullName = txtFullName.Text.Trim();
        string role     = cboRole.SelectedItem?.ToString() ?? "Viewer";
        bool   isActive = chkIsActive.Checked;

        // Prevent deactivating own account
        if (_selectedUserID == UserSession.UserID && !isActive)
        {
            MessageBox.Show("You cannot deactivate your own account.",
                "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Ensure at least one active Admin remains
        if (!isActive || role != "Admin")
        {
            bool wouldRemoveLastAdmin = WouldRemoveLastAdmin(_selectedUserID, role, isActive);
            if (wouldRemoveLastAdmin)
            {
                MessageBox.Show("There must be at least one active Admin account.",
                    "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        bool changePassword = !string.IsNullOrWhiteSpace(txtPassword.Text);

        if (changePassword)
        {
            string pwHash = PasswordHelper.Hash(txtPassword.Text);
            DatabaseHelper.ExecuteNonQuery(@"
                UPDATE Users
                SET    FullName=@FullName, Role=@Role, IsActive=@IsActive, PasswordHash=@Hash
                WHERE  UserID=@UserID",
                [
                    new SqlParameter("@FullName", fullName),
                    new SqlParameter("@Role",     role),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@Hash",     pwHash),
                    new SqlParameter("@UserID",   _selectedUserID)
                ]);
        }
        else
        {
            DatabaseHelper.ExecuteNonQuery(@"
                UPDATE Users
                SET    FullName=@FullName, Role=@Role, IsActive=@IsActive
                WHERE  UserID=@UserID",
                [
                    new SqlParameter("@FullName", fullName),
                    new SqlParameter("@Role",     role),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserID",   _selectedUserID)
                ]);
        }

        lblStatus.Text = $"  User updated successfully.";
        ClearForm();
        LoadUsers();
    }

    // ── Delete ─────────────────────────────────────────────────────────────

    private void btnDelete_Click(object sender, EventArgs e)
    {
        if (_selectedUserID == 0)
        {
            MessageBox.Show("Select a user from the list first.",
                "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (_selectedUserID == UserSession.UserID)
        {
            MessageBox.Show("You cannot delete your own account.",
                "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (WouldRemoveLastAdmin(_selectedUserID, "", false))
        {
            MessageBox.Show("There must be at least one active Admin account.",
                "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string username = txtUsername.Text;
        if (MessageBox.Show($"Delete user '{username}'? This cannot be undone.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
            != DialogResult.Yes) return;

        DatabaseHelper.ExecuteNonQuery("DELETE FROM Users WHERE UserID=@UserID",
            [new SqlParameter("@UserID", _selectedUserID)]);

        lblStatus.Text = $"  User '{username}' deleted.";
        ClearForm();
        LoadUsers();
    }

    // ── Clear ──────────────────────────────────────────────────────────────

    private void btnClear_Click(object sender, EventArgs e) => ClearForm();

    private void ClearForm()
    {
        _selectedUserID            = 0;
        txtUserID.Clear();
        txtUsername.Text           = "";
        txtUsername.ReadOnly       = false;
        txtUsername.BackColor      = SystemColors.Window;
        txtFullName.Clear();
        cboRole.SelectedIndex      = -1;
        chkIsActive.Checked        = true;
        txtPassword.Clear();
        txtConfirmPassword.Clear();
        lblStatus.Text             = "";
    }

    // ── Validation ─────────────────────────────────────────────────────────

    private bool ValidateInput(bool isAdd)
    {
        if (isAdd && string.IsNullOrWhiteSpace(txtUsername.Text))
        {
            MessageBox.Show("Username is required.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtUsername.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtFullName.Text))
        {
            MessageBox.Show("Full Name is required.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtFullName.Focus();
            return false;
        }

        if (cboRole.SelectedIndex < 0)
        {
            MessageBox.Show("Please select a Role.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            cboRole.Focus();
            return false;
        }

        bool hasPassword = !string.IsNullOrWhiteSpace(txtPassword.Text);

        if (isAdd && !hasPassword)
        {
            MessageBox.Show("Password is required when adding a new user.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtPassword.Focus();
            return false;
        }

        if (hasPassword)
        {
            if (txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return false;
            }
        }

        return true;
    }

    // ── Guard: last active Admin ───────────────────────────────────────────

    private bool WouldRemoveLastAdmin(int excludeUserID, string newRole, bool newIsActive)
    {
        // Count active admins excluding the user being edited
        var dt = DatabaseHelper.ExecuteQuery(
            "SELECT COUNT(*) AS Cnt FROM Users WHERE Role='Admin' AND IsActive=1 AND UserID <> @UID",
            [new SqlParameter("@UID", excludeUserID)]);

        int otherAdmins = Convert.ToInt32(dt.Rows[0]["Cnt"]);

        // If no other active admins remain, and this user won't be an active admin either
        bool thisUserStillAdmin = (newRole == "Admin") && newIsActive;
        return otherAdmins == 0 && !thisUserStillAdmin;
    }
}
