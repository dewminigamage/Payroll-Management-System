using System.Data;
using Microsoft.Data.SqlClient;
using PayrollManagementSystem.Database;
using PayrollManagementSystem.Helpers;

namespace PayrollManagementSystem.Forms;

public partial class frmPayroll : Form
{
    private int selectedPayrollID = 0;

    public frmPayroll()
    {
        InitializeComponent();
    }

    private void frmPayroll_Load(object sender, EventArgs e)
    {
        LoadEmployeeCombo();
        LoadFilterEmployeeCombo();
        LoadPayroll();
    }

    // ── Data loading ───────────────────────────────────────────────────────

    private void LoadEmployeeCombo()
    {
        const string query = "SELECT EmployeeID, FullName FROM Employees ORDER BY FullName";
        var dt = DatabaseHelper.ExecuteQuery(query);
        cmbEmployee.DisplayMember = "FullName";
        cmbEmployee.ValueMember   = "EmployeeID";
        cmbEmployee.DataSource    = dt;
        cmbEmployee.SelectedIndex = -1;
    }

    private void LoadFilterEmployeeCombo()
    {
        const string query = "SELECT EmployeeID, FullName FROM Employees ORDER BY FullName";
        var dt = DatabaseHelper.ExecuteQuery(query);

        var allRow = dt.NewRow();
        allRow["EmployeeID"] = 0;
        allRow["FullName"]   = "-- All Employees --";
        dt.Rows.InsertAt(allRow, 0);

        cmbFilterEmployee.DisplayMember = "FullName";
        cmbFilterEmployee.ValueMember   = "EmployeeID";
        cmbFilterEmployee.DataSource    = dt;
        cmbFilterEmployee.SelectedIndex = 0;
    }

    private void LoadPayroll()
    {
        int filterEmpID = cmbFilterEmployee.SelectedValue != null
            ? Convert.ToInt32(cmbFilterEmployee.SelectedValue) : 0;
        int filterMonth = cmbFilterMonth.SelectedIndex;   // 0 = All Months, 1–12 = specific month
        int filterYear  = (int)nudFilterYear.Value;

        var parameters = new List<SqlParameter> { new("@PayYear", filterYear) };

        string query = @"
            SELECT p.PayrollID, e.FullName, p.PayMonth, p.PayYear,
                   p.BasicSalary, p.Allowances, p.GrossSalary,
                   p.EPF, p.ETF, p.Tax, p.OtherDeductions, p.NetSalary,
                   p.Remarks, p.EmployeeID
            FROM   PayrollRecords p
            INNER JOIN Employees e ON p.EmployeeID = e.EmployeeID
            WHERE  p.PayYear = @PayYear";

        if (filterMonth > 0)
        {
            query += " AND p.PayMonth = @PayMonth";
            parameters.Add(new("@PayMonth", filterMonth));
        }
        if (filterEmpID > 0)
        {
            query += " AND p.EmployeeID = @EmployeeID";
            parameters.Add(new("@EmployeeID", filterEmpID));
        }

        query += " ORDER BY p.PayMonth DESC, e.FullName";

        var dt = DatabaseHelper.ExecuteQuery(query, parameters.ToArray());
        BindGrid(dt);
        lblStatus.Text = $"Records: {dt.Rows.Count}";
    }

    private void BindGrid(DataTable dt)
    {
        // Add a readable "Jan 2025" period column
        string[] monthNames = { "Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec" };
        dt.Columns.Add("PayPeriod", typeof(string));
        foreach (DataRow row in dt.Rows)
        {
            int m = Convert.ToInt32(row["PayMonth"]);
            row["PayPeriod"] = $"{monthNames[m - 1]} {row["PayYear"]}";
        }

        dgvPayroll.DataSource = dt;
        if (dgvPayroll.Columns.Count == 0) return;

        // Hidden columns (kept for click-to-load)
        dgvPayroll.Columns["EmployeeID"].Visible = false;
        dgvPayroll.Columns["PayMonth"].Visible   = false;
        dgvPayroll.Columns["PayYear"].Visible    = false;

        dgvPayroll.Columns["PayrollID"].HeaderText      = "ID";
        dgvPayroll.Columns["FullName"].HeaderText        = "Employee";
        dgvPayroll.Columns["PayPeriod"].HeaderText       = "Period";
        dgvPayroll.Columns["BasicSalary"].HeaderText     = "Basic";
        dgvPayroll.Columns["GrossSalary"].HeaderText     = "Gross";
        dgvPayroll.Columns["EPF"].HeaderText             = $"EPF ({PayrollSettings.EpfLabel})";
        dgvPayroll.Columns["ETF"].HeaderText             = $"ETF ({PayrollSettings.EtfLabel})";
        dgvPayroll.Columns["OtherDeductions"].HeaderText = "Other Ded.";
        dgvPayroll.Columns["NetSalary"].HeaderText       = "Net Salary";

        // Right-align and format all money columns
        foreach (string col in new[] { "BasicSalary","Allowances","GrossSalary","EPF","ETF","Tax","OtherDeductions","NetSalary" })
        {
            dgvPayroll.Columns[col].DefaultCellStyle.Format    = "N2";
            dgvPayroll.Columns[col].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        // Move Period right after Employee
        dgvPayroll.Columns["PayPeriod"].DisplayIndex = 2;

        dgvPayroll.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        // Pin small/fixed columns; the rest (FullName, Remarks) fill remaining space
        foreach (string col in new[] { "PayrollID","PayPeriod","EPF","ETF","Tax","OtherDeductions","BasicSalary","Allowances","GrossSalary","NetSalary" })
            dgvPayroll.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
    }

    // ── Employee selection – auto-fill BasicSalary ─────────────────────────

    private void cmbEmployee_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbEmployee.SelectedValue == null) return;
        int empID = Convert.ToInt32(cmbEmployee.SelectedValue);
        if (empID == 0) { txtBasicSalary.Text = ""; return; }

        var dt = DatabaseHelper.ExecuteQuery(
            "SELECT BasicSalary FROM Employees WHERE EmployeeID = @EmployeeID",
            [new("@EmployeeID", empID)]);

        if (dt.Rows.Count > 0)
            txtBasicSalary.Text = Convert.ToDecimal(dt.Rows[0]["BasicSalary"]).ToString("F2");
    }

    // ── Calculation ────────────────────────────────────────────────────────

    private void btnCalculate_Click(object sender, EventArgs e)
    {
        if (!TryParseInputs(out decimal basic, out decimal allowances, out decimal otherDed, out decimal tax))
            return;

        decimal gross = basic + allowances;
        decimal epf   = Math.Round(gross * PayrollSettings.EpfRate, 2);
        decimal etf   = Math.Round(gross * PayrollSettings.EtfRate, 2);
        decimal net   = gross - epf - tax - otherDed;

        lblGrossVal.Text   = gross.ToString("N2");
        lblEPFVal.Text     = epf.ToString("N2");
        lblETFVal.Text     = etf.ToString("N2");
        lblNetVal.Text     = net.ToString("N2");
        lblNetVal.ForeColor = net < 0
            ? Color.FromArgb(196, 43, 28)
            : Color.FromArgb(0, 100, 0);
    }

    // ── Save ───────────────────────────────────────────────────────────────

    private void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateForm()) return;
        if (!TryParseInputs(out decimal basic, out decimal allowances, out decimal otherDed, out decimal tax))
            return;

        decimal gross = basic + allowances;
        decimal epf   = Math.Round(gross * PayrollSettings.EpfRate, 2);
        decimal etf   = Math.Round(gross * PayrollSettings.EtfRate, 2);
        decimal net   = gross - epf - tax - otherDed;

        try
        {
            const string query = @"
                INSERT INTO PayrollRecords
                    (EmployeeID, PayMonth, PayYear, BasicSalary, Allowances, GrossSalary,
                     EPF, ETF, Tax, OtherDeductions, NetSalary, Remarks)
                VALUES
                    (@EmployeeID, @PayMonth, @PayYear, @BasicSalary, @Allowances, @GrossSalary,
                     @EPF, @ETF, @Tax, @OtherDeductions, @NetSalary, @Remarks)";

            DatabaseHelper.ExecuteNonQuery(query, BuildParams(gross, epf, etf, net));
            UpdateSummaryLabels(gross, epf, etf, net);
            MessageBox.Show("Payroll saved successfully.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            LoadPayroll();
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            MessageBox.Show(
                "A payroll record for this employee and month already exists.\nSelect the record and use Update to modify it.",
                "Duplicate Record", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving payroll:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ── Update ─────────────────────────────────────────────────────────────

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        if (selectedPayrollID == 0)
        {
            MessageBox.Show("Please select a record from the list first.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!ValidateForm()) return;
        if (!TryParseInputs(out decimal basic, out decimal allowances, out decimal otherDed, out decimal tax))
            return;

        decimal gross = basic + allowances;
        decimal epf   = Math.Round(gross * PayrollSettings.EpfRate, 2);
        decimal etf   = Math.Round(gross * PayrollSettings.EtfRate, 2);
        decimal net   = gross - epf - tax - otherDed;

        try
        {
            const string query = @"
                UPDATE PayrollRecords SET
                    EmployeeID      = @EmployeeID,
                    PayMonth        = @PayMonth,
                    PayYear         = @PayYear,
                    BasicSalary     = @BasicSalary,
                    Allowances      = @Allowances,
                    GrossSalary     = @GrossSalary,
                    EPF             = @EPF,
                    ETF             = @ETF,
                    Tax             = @Tax,
                    OtherDeductions = @OtherDeductions,
                    NetSalary       = @NetSalary,
                    Remarks         = @Remarks
                WHERE PayrollID = @PayrollID";

            DatabaseHelper.ExecuteNonQuery(query, BuildParams(gross, epf, etf, net, includeID: true));
            UpdateSummaryLabels(gross, epf, etf, net);
            MessageBox.Show("Payroll updated successfully.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            LoadPayroll();
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            MessageBox.Show("A payroll record for this employee and month already exists.",
                "Duplicate Record", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating payroll:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ── Delete ─────────────────────────────────────────────────────────────

    private void btnDelete_Click(object sender, EventArgs e)
    {
        if (selectedPayrollID == 0)
        {
            MessageBox.Show("Please select a record to delete.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (MessageBox.Show("Delete this payroll record?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        try
        {
            DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM PayrollRecords WHERE PayrollID = @PayrollID",
                [new("@PayrollID", selectedPayrollID)]);
            MessageBox.Show("Record deleted.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            LoadPayroll();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting record:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnClear_Click(object sender, EventArgs e) => ClearForm();

    // ── Filter ─────────────────────────────────────────────────────────────

    private void btnPaySlip_Click(object sender, EventArgs e)
    {
        if (selectedPayrollID == 0)
        {
            MessageBox.Show("Please select a payroll record from the list first.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        using var frm = new frmPaySlip(selectedPayrollID);
        frm.ShowDialog();
    }

    private void btnFilter_Click(object sender, EventArgs e)      => LoadPayroll();
    private void btnClearFilter_Click(object sender, EventArgs e)
    {
        cmbFilterEmployee.SelectedIndex = 0;
        cmbFilterMonth.SelectedIndex    = 0;
        nudFilterYear.Value             = DateTime.Today.Year;
        LoadPayroll();
    }

    // ── Grid row click → populate form ────────────────────────────────────

    private void dgvPayroll_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var row = dgvPayroll.Rows[e.RowIndex];

        selectedPayrollID    = Convert.ToInt32(row.Cells["PayrollID"].Value);
        txtPayrollID.Text    = selectedPayrollID.ToString();

        // Set employee combo
        int empID = Convert.ToInt32(row.Cells["EmployeeID"].Value);
        foreach (DataRowView item in ((DataTable)cmbEmployee.DataSource!).DefaultView)
        {
            if (Convert.ToInt32(item["EmployeeID"]) == empID)
            {
                cmbEmployee.SelectedItem = item;
                break;
            }
        }
        // Override BasicSalary with the stored (historical) value
        txtBasicSalary.Text = Convert.ToDecimal(row.Cells["BasicSalary"].Value).ToString("F2");

        cmbPayMonth.SelectedIndex = Convert.ToInt32(row.Cells["PayMonth"].Value) - 1;
        nudPayYear.Value          = Convert.ToInt32(row.Cells["PayYear"].Value);

        txtAllowances.Text      = Convert.ToDecimal(row.Cells["Allowances"].Value).ToString("F2");
        txtOtherDeductions.Text = Convert.ToDecimal(row.Cells["OtherDeductions"].Value).ToString("F2");
        txtTax.Text             = Convert.ToDecimal(row.Cells["Tax"].Value).ToString("F2");
        txtRemarks.Text         = row.Cells["Remarks"].Value?.ToString() ?? "";

        decimal gross = Convert.ToDecimal(row.Cells["GrossSalary"].Value);
        decimal epf   = Convert.ToDecimal(row.Cells["EPF"].Value);
        decimal etf   = Convert.ToDecimal(row.Cells["ETF"].Value);
        decimal net   = Convert.ToDecimal(row.Cells["NetSalary"].Value);
        UpdateSummaryLabels(gross, epf, etf, net);
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private void ClearForm()
    {
        selectedPayrollID       = 0;
        txtPayrollID.Text       = "";
        cmbEmployee.SelectedIndex = -1;
        cmbPayMonth.SelectedIndex = DateTime.Today.Month - 1;
        nudPayYear.Value        = DateTime.Today.Year;
        txtBasicSalary.Text     = "";
        txtAllowances.Text      = "0";
        txtOtherDeductions.Text = "0";
        txtTax.Text             = "0";
        txtRemarks.Text         = "";
        UpdateSummaryLabels(0, 0, 0, 0);
    }

    private void UpdateSummaryLabels(decimal gross, decimal epf, decimal etf, decimal net)
    {
        lblGrossVal.Text    = gross.ToString("N2");
        lblEPFVal.Text      = epf.ToString("N2");
        lblETFVal.Text      = etf.ToString("N2");
        lblNetVal.Text      = net.ToString("N2");
        lblNetVal.ForeColor = net < 0
            ? Color.FromArgb(196, 43, 28)
            : Color.FromArgb(0, 100, 0);
    }

    private bool ValidateForm()
    {
        if (cmbEmployee.SelectedValue == null || Convert.ToInt32(cmbEmployee.SelectedValue) == 0)
        {
            MessageBox.Show("Please select an employee.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            cmbEmployee.Focus();
            return false;
        }
        if (cmbPayMonth.SelectedIndex < 0)
        {
            MessageBox.Show("Please select a pay month.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            cmbPayMonth.Focus();
            return false;
        }
        return true;
    }

    private bool TryParseInputs(out decimal basic, out decimal allowances, out decimal otherDed, out decimal tax)
    {
        basic = allowances = otherDed = tax = 0;

        if (!decimal.TryParse(txtBasicSalary.Text, out basic) || basic < 0)
        {
            MessageBox.Show("Please select an employee to load their Basic Salary.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            cmbEmployee.Focus();
            return false;
        }
        if (!decimal.TryParse(txtAllowances.Text, out allowances) || allowances < 0)
        {
            MessageBox.Show("Enter a valid Allowances amount (>= 0).", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtAllowances.Focus();
            return false;
        }
        if (!decimal.TryParse(txtOtherDeductions.Text, out otherDed) || otherDed < 0)
        {
            MessageBox.Show("Enter a valid Other Deductions amount (>= 0).", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtOtherDeductions.Focus();
            return false;
        }
        if (!decimal.TryParse(txtTax.Text, out tax) || tax < 0)
        {
            MessageBox.Show("Enter a valid Tax amount (>= 0).", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtTax.Focus();
            return false;
        }
        return true;
    }

    private SqlParameter[] BuildParams(decimal gross, decimal epf, decimal etf, decimal net, bool includeID = false)
    {
        var list = new List<SqlParameter>
        {
            new("@EmployeeID",      Convert.ToInt32(cmbEmployee.SelectedValue)),
            new("@PayMonth",        cmbPayMonth.SelectedIndex + 1),
            new("@PayYear",         (int)nudPayYear.Value),
            new("@BasicSalary",     decimal.Parse(txtBasicSalary.Text)),
            new("@Allowances",      decimal.Parse(txtAllowances.Text)),
            new("@GrossSalary",     gross),
            new("@EPF",             epf),
            new("@ETF",             etf),
            new("@Tax",             decimal.Parse(txtTax.Text)),
            new("@OtherDeductions", decimal.Parse(txtOtherDeductions.Text)),
            new("@NetSalary",       net),
            new("@Remarks",         txtRemarks.Text.Trim())
        };
        if (includeID) list.Add(new("@PayrollID", selectedPayrollID));
        return list.ToArray();
    }
}
