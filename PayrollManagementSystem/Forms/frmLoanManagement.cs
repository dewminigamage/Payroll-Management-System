using System.Data;
using Microsoft.Data.SqlClient;
using PayrollManagementSystem.Database;
using PayrollManagementSystem.Models;

namespace PayrollManagementSystem.Forms;

public partial class frmLoanManagement : Form
{
    private int     _selectedLoanID  = 0;
    private string  _selectedStatus  = "";
    private decimal _selectedInstallment = 0m;

    public frmLoanManagement()
    {
        InitializeComponent();
    }

    private void frmLoanManagement_Load(object sender, EventArgs e)
    {
        LoadEmployeeCombos();
        LoadLoanTypes();
        LoadLoans();
    }

    // ── Data loading ───────────────────────────────────────────────────────

    private void LoadEmployeeCombos()
    {
        const string q = "SELECT EmployeeID, FullName FROM Employees ORDER BY FullName";

        var dt1 = DatabaseHelper.ExecuteQuery(q);
        cmbEmployee.DisplayMember = "FullName";
        cmbEmployee.ValueMember   = "EmployeeID";
        cmbEmployee.DataSource    = dt1;
        cmbEmployee.SelectedIndex = -1;

        var dt2 = DatabaseHelper.ExecuteQuery(q);
        var all = dt2.NewRow();
        all["EmployeeID"] = 0; all["FullName"] = "-- All Employees --";
        dt2.Rows.InsertAt(all, 0);
        cmbFilterEmp.DisplayMember = "FullName";
        cmbFilterEmp.ValueMember   = "EmployeeID";
        cmbFilterEmp.DataSource    = dt2;
        cmbFilterEmp.SelectedIndex = 0;
    }

    private void LoadLoanTypes()
    {
        var dt = DatabaseHelper.ExecuteQuery(
            "SELECT LoanTypeID, TypeName FROM LoanTypes WHERE IsActive=1 ORDER BY TypeName");
        cmbLoanType.DisplayMember = "TypeName";
        cmbLoanType.ValueMember   = "LoanTypeID";
        cmbLoanType.DataSource    = dt;
        cmbLoanType.SelectedIndex = -1;
    }

    private void LoadLoans()
    {
        int empID     = cmbFilterEmp.SelectedValue != null
            ? Convert.ToInt32(cmbFilterEmp.SelectedValue) : 0;
        string status = cmbFilterStatus.SelectedItem?.ToString() ?? "All";

        string query = @"
            SELECT l.LoanID, e.FullName AS Employee, lt.TypeName AS LoanType,
                   l.LoanAmount, l.MonthlyInstallment, l.RemainingBalance,
                   l.StartMonth, l.StartYear, l.Status, l.Notes,
                   l.EmployeeID, l.LoanTypeID
            FROM   EmployeeLoans l
            INNER JOIN Employees e  ON l.EmployeeID  = e.EmployeeID
            INNER JOIN LoanTypes  lt ON l.LoanTypeID = lt.LoanTypeID
            WHERE  1=1";

        var parameters = new List<SqlParameter>();
        if (status != "All")
        {
            query += " AND l.Status = @Status";
            parameters.Add(new("@Status", status));
        }
        if (empID > 0)
        {
            query += " AND l.EmployeeID = @EmpID";
            parameters.Add(new("@EmpID", empID));
        }
        query += " ORDER BY l.Status, e.FullName, l.LoanID DESC";

        var dt = DatabaseHelper.ExecuteQuery(query, parameters.ToArray());
        BindGrid(dt);
        lblStatus.Text = $"Records: {dt.Rows.Count}";
    }

    private void BindGrid(DataTable dt)
    {
        dgvLoans.DataSource = dt;
        if (dgvLoans.Columns.Count == 0) return;

        dgvLoans.Columns["LoanID"].Visible     = false;
        dgvLoans.Columns["EmployeeID"].Visible = false;
        dgvLoans.Columns["LoanTypeID"].Visible = false;
        dgvLoans.Columns["Notes"].Visible      = false;
        dgvLoans.Columns["StartMonth"].Visible = false;
        dgvLoans.Columns["StartYear"].Visible  = false;

        string[] months = { "Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec" };
        dt.Columns.Add("StartPeriod", typeof(string));
        foreach (DataRow r in dt.Rows)
            r["StartPeriod"] = $"{months[Convert.ToInt32(r["StartMonth"]) - 1]} {r["StartYear"]}";

        dgvLoans.Columns["Employee"].HeaderText        = "Employee";
        dgvLoans.Columns["LoanType"].HeaderText        = "Type";
        dgvLoans.Columns["LoanAmount"].HeaderText      = "Loan Amount";
        dgvLoans.Columns["MonthlyInstallment"].HeaderText = "Installment";
        dgvLoans.Columns["RemainingBalance"].HeaderText = "Remaining";
        dgvLoans.Columns["StartPeriod"].HeaderText     = "From";
        dgvLoans.Columns["Status"].HeaderText          = "Status";

        foreach (string col in new[] { "LoanAmount", "MonthlyInstallment", "RemainingBalance" })
        {
            dgvLoans.Columns[col].DefaultCellStyle.Format    = "N2";
            dgvLoans.Columns[col].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        dgvLoans.Columns["StartPeriod"].DisplayIndex = 3;

        // Colour-code the Status cell
        foreach (DataGridViewRow row in dgvLoans.Rows)
        {
            string s = row.Cells["Status"].Value?.ToString() ?? "";
            row.Cells["Status"].Style.ForeColor = s switch
            {
                "Active"    => Color.FromArgb(0, 130, 70),
                "Completed" => Color.FromArgb(80, 80, 80),
                "Cancelled" => Color.FromArgb(196, 43, 28),
                _           => Color.Black
            };
            row.Cells["Status"].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            decimal remaining = Convert.ToDecimal(row.Cells["RemainingBalance"].Value);
            if (remaining <= 0 && s == "Active")
                row.DefaultCellStyle.BackColor = Color.FromArgb(240, 255, 240);
        }
    }

    // ── Live calculation hint ──────────────────────────────────────────────

    private void nudAmount_ValueChanged(object sender, EventArgs e)      => UpdateSummaryHint();
    private void nudInstallment_ValueChanged(object sender, EventArgs e) => UpdateSummaryHint();

    private void UpdateSummaryHint()
    {
        decimal amount      = nudAmount.Value;
        decimal installment = nudInstallment.Value;

        if (_selectedLoanID > 0)
        {
            // Editing existing: show stored remaining
            return;
        }

        lblSumRemaining.Text = $"Remaining Balance: LKR {amount:N2}";
        if (installment > 0)
        {
            int months = (int)Math.Ceiling((double)(amount / installment));
            lblSumMonths.Text = $"Estimated months to repay: {months}";
        }
        else
        {
            lblSumMonths.Text = "Estimated months: —";
        }
        lblSumStatus.Text = "";
    }

    // ── CRUD ───────────────────────────────────────────────────────────────

    private void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateForm()) return;
        try
        {
            DatabaseHelper.ExecuteNonQuery(@"
                INSERT INTO EmployeeLoans
                    (EmployeeID, LoanTypeID, LoanAmount, RemainingBalance,
                     MonthlyInstallment, StartMonth, StartYear, Notes)
                VALUES
                    (@EID, @TypeID, @Amount, @Amount, @Inst, @Month, @Year, @Notes)",
            [
                new("@EID",    Convert.ToInt32(cmbEmployee.SelectedValue)),
                new("@TypeID", Convert.ToInt32(cmbLoanType.SelectedValue)),
                new("@Amount", nudAmount.Value),
                new("@Inst",   nudInstallment.Value),
                new("@Month",  cmbStartMonth.SelectedIndex + 1),
                new("@Year",   (int)nudStartYear.Value),
                new("@Notes",  txtNotes.Text.Trim())
            ]);
            MessageBox.Show("Loan record saved.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            LoadLoans();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving loan:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        if (_selectedLoanID == 0) { ShowSelectFirst(); return; }
        if (!ValidateForm()) return;
        try
        {
            DatabaseHelper.ExecuteNonQuery(@"
                UPDATE EmployeeLoans SET
                    EmployeeID         = @EID,
                    LoanTypeID         = @TypeID,
                    LoanAmount         = @Amount,
                    MonthlyInstallment = @Inst,
                    StartMonth         = @Month,
                    StartYear          = @Year,
                    Notes              = @Notes
                WHERE LoanID = @ID",
            [
                new("@EID",    Convert.ToInt32(cmbEmployee.SelectedValue)),
                new("@TypeID", Convert.ToInt32(cmbLoanType.SelectedValue)),
                new("@Amount", nudAmount.Value),
                new("@Inst",   nudInstallment.Value),
                new("@Month",  cmbStartMonth.SelectedIndex + 1),
                new("@Year",   (int)nudStartYear.Value),
                new("@Notes",  txtNotes.Text.Trim()),
                new("@ID",     _selectedLoanID)
            ]);
            MessageBox.Show("Loan record updated.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            LoadLoans();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating loan:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
        if (_selectedLoanID == 0) { ShowSelectFirst(); return; }
        if (MessageBox.Show("Delete this loan record and all its repayment history?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                != DialogResult.Yes) return;
        try
        {
            DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM EmployeeLoans WHERE LoanID = @ID",
                [new("@ID", _selectedLoanID)]);
            MessageBox.Show("Record deleted.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            LoadLoans();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting loan:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnClear_Click(object sender, EventArgs e)  => ClearForm();
    private void btnFilter_Click(object sender, EventArgs e) => LoadLoans();

    private void btnClearFilter_Click(object sender, EventArgs e)
    {
        cmbFilterEmp.SelectedIndex    = 0;
        cmbFilterStatus.SelectedIndex = 1;
        LoadLoans();
    }

    // ── Record Payment ────────────────────────────────────────────────────

    private void btnRecordPayment_Click(object sender, EventArgs e)
    {
        if (_selectedLoanID == 0 || _selectedStatus != "Active") return;

        int today    = DateTime.Today.Month;
        int yearNow  = DateTime.Today.Year;
        decimal inst = _selectedInstallment;

        if (MessageBox.Show(
                $"Record payment of LKR {inst:N2} for {DateTime.Today:MMMM yyyy}?\n\n" +
                $"This will reduce the remaining balance by LKR {inst:N2}.",
                "Confirm Payment",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        try
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            // Insert repayment record
            using var insCmd = new SqlCommand(@"
                INSERT INTO LoanRepayments (LoanID, PayMonth, PayYear, AmountPaid)
                VALUES (@LID, @Month, @Year, @Amt)", conn, tx);
            insCmd.Parameters.AddWithValue("@LID",   _selectedLoanID);
            insCmd.Parameters.AddWithValue("@Month", today);
            insCmd.Parameters.AddWithValue("@Year",  yearNow);
            insCmd.Parameters.AddWithValue("@Amt",   inst);
            insCmd.ExecuteNonQuery();

            // Deduct from remaining balance; auto-complete if paid off
            using var updCmd = new SqlCommand(@"
                UPDATE EmployeeLoans
                SET RemainingBalance = RemainingBalance - @Inst,
                    Status = CASE
                        WHEN RemainingBalance - @Inst <= 0 THEN 'Completed'
                        ELSE Status END
                WHERE LoanID = @ID", conn, tx);
            updCmd.Parameters.AddWithValue("@Inst", inst);
            updCmd.Parameters.AddWithValue("@ID",   _selectedLoanID);
            updCmd.ExecuteNonQuery();

            tx.Commit();

            MessageBox.Show("Payment recorded successfully.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            LoadLoans();
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            MessageBox.Show("A payment for this loan and month is already recorded.",
                "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error recording payment:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ── Grid click ────────────────────────────────────────────────────────

    private void dgvLoans_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var row = dgvLoans.Rows[e.RowIndex];

        _selectedLoanID      = Convert.ToInt32(row.Cells["LoanID"].Value);
        _selectedStatus      = row.Cells["Status"].Value?.ToString() ?? "";
        _selectedInstallment = Convert.ToDecimal(row.Cells["MonthlyInstallment"].Value);

        txtLoanID.Text = _selectedLoanID.ToString();

        int empID = Convert.ToInt32(row.Cells["EmployeeID"].Value);
        foreach (DataRowView item in ((DataTable)cmbEmployee.DataSource!).DefaultView)
        {
            if (Convert.ToInt32(item["EmployeeID"]) == empID)
            { cmbEmployee.SelectedItem = item; break; }
        }

        int typeID = Convert.ToInt32(row.Cells["LoanTypeID"].Value);
        foreach (DataRowView item in ((DataTable)cmbLoanType.DataSource!).DefaultView)
        {
            if (Convert.ToInt32(item["LoanTypeID"]) == typeID)
            { cmbLoanType.SelectedItem = item; break; }
        }

        nudAmount.Value        = Convert.ToDecimal(row.Cells["LoanAmount"].Value);
        nudInstallment.Value   = _selectedInstallment;
        cmbStartMonth.SelectedIndex = Convert.ToInt32(row.Cells["StartMonth"].Value) - 1;
        nudStartYear.Value     = Convert.ToInt32(row.Cells["StartYear"].Value);
        txtNotes.Text          = row.Cells["Notes"].Value?.ToString() ?? "";

        // Update summary panel
        decimal remaining = Convert.ToDecimal(row.Cells["RemainingBalance"].Value);
        lblSumRemaining.Text = $"Remaining Balance: LKR {remaining:N2}";
        decimal inst         = _selectedInstallment;
        if (inst > 0 && remaining > 0)
        {
            int months = (int)Math.Ceiling((double)(remaining / inst));
            lblSumMonths.Text = $"Estimated months to repay: {months}";
        }
        else
        {
            lblSumMonths.Text = remaining <= 0 ? "Fully repaid" : "—";
        }
        lblSumStatus.Text      = _selectedStatus;
        lblSumStatus.ForeColor = _selectedStatus switch
        {
            "Active"    => Color.FromArgb(0, 130, 70),
            "Completed" => Color.FromArgb(80, 80, 80),
            _           => Color.FromArgb(196, 43, 28)
        };

        btnRecordPayment.Enabled = (_selectedStatus == "Active");
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private void ClearForm()
    {
        _selectedLoanID      = 0;
        _selectedStatus      = "";
        _selectedInstallment = 0m;
        txtLoanID.Text       = "";
        cmbEmployee.SelectedIndex  = -1;
        cmbLoanType.SelectedIndex  = -1;
        nudAmount.Value            = 0;
        nudInstallment.Value       = 0;
        cmbStartMonth.SelectedIndex = DateTime.Today.Month - 1;
        nudStartYear.Value         = DateTime.Today.Year;
        txtNotes.Text              = "";
        lblSumRemaining.Text       = "Remaining Balance: —";
        lblSumMonths.Text          = "Estimated months: —";
        lblSumStatus.Text          = "";
        btnRecordPayment.Enabled   = false;
    }

    private bool ValidateForm()
    {
        if (cmbEmployee.SelectedValue == null || Convert.ToInt32(cmbEmployee.SelectedValue) == 0)
        {
            MessageBox.Show("Please select an employee.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (cmbLoanType.SelectedValue == null)
        {
            MessageBox.Show("Please select a loan type.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (nudAmount.Value <= 0)
        {
            MessageBox.Show("Loan amount must be greater than zero.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (nudInstallment.Value <= 0)
        {
            MessageBox.Show("Monthly installment must be greater than zero.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }

    private static void ShowSelectFirst() =>
        MessageBox.Show("Please select a record from the list first.", "No Selection",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
