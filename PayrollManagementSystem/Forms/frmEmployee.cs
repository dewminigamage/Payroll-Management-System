using System.Data;
using Microsoft.Data.SqlClient;
using PayrollManagementSystem.Database;
using PayrollManagementSystem.Models;

namespace PayrollManagementSystem.Forms;

public partial class frmEmployee : Form
{
    private int selectedEmployeeID = 0;

    public frmEmployee()
    {
        InitializeComponent();
    }

    private void frmEmployee_Load(object sender, EventArgs e)
    {
        LoadAllEmployees();
    }

    // ── Data loading ───────────────────────────────────────────────────────

    private void LoadAllEmployees()
    {
        const string query = @"
            SELECT EmployeeID, FullName, NIC, Department, Position,
                   BasicSalary, JoinDate, ContactNumber, Email
            FROM   Employees
            ORDER  BY EmployeeID";
        BindGrid(DatabaseHelper.ExecuteQuery(query));
        UpdateStatusCount();
    }

    private void BindGrid(DataTable dt)
    {
        dgvEmployees.DataSource = dt;
        if (dgvEmployees.Columns.Count == 0) return;

        dgvEmployees.Columns["EmployeeID"].HeaderText = "ID";
        dgvEmployees.Columns["EmployeeID"].Width = 45;
        dgvEmployees.Columns["FullName"].HeaderText = "Full Name";
        dgvEmployees.Columns["BasicSalary"].HeaderText = "Basic Salary";
        dgvEmployees.Columns["BasicSalary"].DefaultCellStyle.Format = "N2";
        dgvEmployees.Columns["BasicSalary"].DefaultCellStyle.Alignment =
            DataGridViewContentAlignment.MiddleRight;
        dgvEmployees.Columns["JoinDate"].HeaderText = "Join Date";
        dgvEmployees.Columns["JoinDate"].DefaultCellStyle.Format = "yyyy-MM-dd";
        dgvEmployees.Columns["ContactNumber"].HeaderText = "Contact No.";
        dgvEmployees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvEmployees.Columns["EmployeeID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
    }

    private void UpdateStatusCount()
    {
        int count = dgvEmployees.Rows.Count;
        lblStatus.Text = $"Total Employees: {count}";
    }

    // ── Form helpers ───────────────────────────────────────────────────────

    private void ClearForm()
    {
        selectedEmployeeID = 0;
        txtEmployeeID.Text = "";
        txtFullName.Text = "";
        txtNIC.Text = "";
        txtDepartment.Text = "";
        txtPosition.Text = "";
        txtBasicSalary.Text = "";
        dtpJoinDate.Value = DateTime.Today;
        txtContactNumber.Text = "";
        txtEmail.Text = "";
        txtFullName.Focus();
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(txtFullName.Text))
        {
            ShowWarning("Full Name is required.", txtFullName);
            return false;
        }
        if (string.IsNullOrWhiteSpace(txtNIC.Text))
        {
            ShowWarning("NIC is required.", txtNIC);
            return false;
        }
        if (!decimal.TryParse(txtBasicSalary.Text, out decimal salary) || salary < 0)
        {
            ShowWarning("Enter a valid Basic Salary (numeric, >= 0).", txtBasicSalary);
            return false;
        }
        return true;
    }

    private static void ShowWarning(string message, Control focusControl)
    {
        MessageBox.Show(message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        focusControl.Focus();
    }

    private SqlParameter[] BuildParameters(bool includeID = false)
    {
        var list = new List<SqlParameter>
        {
            new("@FullName",       txtFullName.Text.Trim()),
            new("@NIC",            txtNIC.Text.Trim()),
            new("@Department",     txtDepartment.Text.Trim()),
            new("@Position",       txtPosition.Text.Trim()),
            new("@BasicSalary",    decimal.Parse(txtBasicSalary.Text)),
            new("@JoinDate",       dtpJoinDate.Value.Date),
            new("@ContactNumber",  txtContactNumber.Text.Trim()),
            new("@Email",          txtEmail.Text.Trim())
        };
        if (includeID) list.Add(new("@EmployeeID", selectedEmployeeID));
        return list.ToArray();
    }

    // ── Button events ──────────────────────────────────────────────────────

    private void btnAdd_Click(object sender, EventArgs e)
    {
        if (!ValidateInput()) return;
        try
        {
            const string query = @"
                INSERT INTO Employees
                    (FullName, NIC, Department, Position, BasicSalary, JoinDate, ContactNumber, Email)
                VALUES
                    (@FullName, @NIC, @Department, @Position, @BasicSalary, @JoinDate, @ContactNumber, @Email)";
            DatabaseHelper.ExecuteNonQuery(query, BuildParameters());
            MessageBox.Show("Employee added successfully.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            LoadAllEmployees();
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            MessageBox.Show("An employee with this NIC already exists.", "Duplicate NIC",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding employee:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        if (selectedEmployeeID == 0)
        {
            MessageBox.Show("Please select an employee from the list first.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!ValidateInput()) return;
        try
        {
            // Detect salary change before writing the UPDATE
            decimal newSalary = decimal.Parse(txtBasicSalary.Text);
            var salRow = DatabaseHelper.ExecuteQuery(
                "SELECT BasicSalary FROM Employees WHERE EmployeeID = @ID",
                [new("@ID", selectedEmployeeID)]);
            decimal oldSalary = salRow.Rows.Count > 0
                ? Convert.ToDecimal(salRow.Rows[0]["BasicSalary"]) : newSalary;

            string? reason = null;
            if (newSalary != oldSalary)
            {
                reason = PromptReason(
                    $"Salary is changing from {oldSalary:N2} → {newSalary:N2}.\nReason for this increment/change (optional):");
            }

            const string query = @"
                UPDATE Employees SET
                    FullName      = @FullName,
                    NIC           = @NIC,
                    Department    = @Department,
                    Position      = @Position,
                    BasicSalary   = @BasicSalary,
                    JoinDate      = @JoinDate,
                    ContactNumber = @ContactNumber,
                    Email         = @Email
                WHERE EmployeeID = @EmployeeID";
            DatabaseHelper.ExecuteNonQuery(query, BuildParameters(includeID: true));

            // Log salary change after successful update
            if (newSalary != oldSalary)
            {
                DatabaseHelper.ExecuteNonQuery(@"
                    INSERT INTO SalaryHistory (EmployeeID, OldSalary, NewSalary, EffectiveDate, Reason, ChangedBy)
                    VALUES (@EID, @Old, @New, @Date, @Reason, @By)",
                [
                    new("@EID",    selectedEmployeeID),
                    new("@Old",    oldSalary),
                    new("@New",    newSalary),
                    new("@Date",   DateTime.Today),
                    new("@Reason", reason ?? (object)DBNull.Value),
                    new("@By",     UserSession.FullName)
                ]);
            }

            MessageBox.Show("Employee updated successfully.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            LoadAllEmployees();
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            MessageBox.Show("An employee with this NIC already exists.", "Duplicate NIC",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating employee:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnHistory_Click(object sender, EventArgs e)
    {
        if (selectedEmployeeID == 0)
        {
            MessageBox.Show("Please select an employee from the list first.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        using var frm = new frmSalaryHistory(selectedEmployeeID, txtFullName.Text);
        frm.ShowDialog();
    }

    private static string? PromptReason(string prompt)
    {
        using var dlg   = new Form();
        using var lbl   = new Label  { Text = prompt, AutoSize = false, Dock = DockStyle.Top, Height = 52, Font = new Font("Segoe UI", 9F), Padding = new Padding(8, 8, 8, 0) };
        using var txt   = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F) };
        using var ok    = new Button  { Text = "OK",     DialogResult = DialogResult.OK,     Width = 80, Height = 28 };
        using var skip  = new Button  { Text = "Skip",   DialogResult = DialogResult.Cancel, Width = 80, Height = 28 };
        using var flow  = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 36, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(4, 2, 4, 2) };

        flow.Controls.Add(ok);
        flow.Controls.Add(skip);
        dlg.Controls.Add(txt);
        dlg.Controls.Add(flow);
        dlg.Controls.Add(lbl);
        dlg.AcceptButton     = ok;
        dlg.ClientSize       = new Size(420, 130);
        dlg.FormBorderStyle  = FormBorderStyle.FixedDialog;
        dlg.MaximizeBox      = false;
        dlg.MinimizeBox      = false;
        dlg.StartPosition    = FormStartPosition.CenterParent;
        dlg.Text             = "Salary Change Reason";
        dlg.Font             = new Font("Segoe UI", 9F);

        return dlg.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(txt.Text)
            ? txt.Text.Trim() : null;
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
        if (selectedEmployeeID == 0)
        {
            MessageBox.Show("Please select an employee from the list first.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var confirm = MessageBox.Show(
            $"Delete employee \"{txtFullName.Text}\"? This cannot be undone.",
            "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;
        try
        {
            const string query = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID";
            DatabaseHelper.ExecuteNonQuery(query, [new("@EmployeeID", selectedEmployeeID)]);
            MessageBox.Show("Employee deleted successfully.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            LoadAllEmployees();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting employee:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnClear_Click(object sender, EventArgs e) => ClearForm();

    private void btnSearch_Click(object sender, EventArgs e)
    {
        string term = txtSearch.Text.Trim();
        if (string.IsNullOrEmpty(term))
        {
            LoadAllEmployees();
            return;
        }
        try
        {
            const string query = @"
                SELECT EmployeeID, FullName, NIC, Department, Position,
                       BasicSalary, JoinDate, ContactNumber, Email
                FROM   Employees
                WHERE  FullName       LIKE @Search
                    OR NIC            LIKE @Search
                    OR Department     LIKE @Search
                    OR Position       LIKE @Search
                ORDER  BY EmployeeID";
            var dt = DatabaseHelper.ExecuteQuery(query, [new("@Search", $"%{term}%")]);
            BindGrid(dt);
            lblStatus.Text = $"Found: {dt.Rows.Count} employee(s)";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Search error:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnShowAll_Click(object sender, EventArgs e)
    {
        txtSearch.Text = "";
        LoadAllEmployees();
    }

    // ── Grid row click ─────────────────────────────────────────────────────

    private void dgvEmployees_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var row = dgvEmployees.Rows[e.RowIndex];

        selectedEmployeeID = Convert.ToInt32(row.Cells["EmployeeID"].Value);
        txtEmployeeID.Text = selectedEmployeeID.ToString();
        txtFullName.Text = row.Cells["FullName"].Value?.ToString() ?? "";
        txtNIC.Text = row.Cells["NIC"].Value?.ToString() ?? "";
        txtDepartment.Text = row.Cells["Department"].Value?.ToString() ?? "";
        txtPosition.Text = row.Cells["Position"].Value?.ToString() ?? "";
        txtBasicSalary.Text = row.Cells["BasicSalary"].Value is decimal sal
            ? sal.ToString("F2") : row.Cells["BasicSalary"].Value?.ToString() ?? "";

        if (DateTime.TryParse(row.Cells["JoinDate"].Value?.ToString(), out var date))
            dtpJoinDate.Value = date;

        txtContactNumber.Text = row.Cells["ContactNumber"].Value?.ToString() ?? "";
        txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
    }

    // ── Search box Enter key ───────────────────────────────────────────────

    private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            e.Handled = true;
            btnSearch_Click(sender, EventArgs.Empty);
        }
    }
}
