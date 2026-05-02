using System.Data;
using Microsoft.Data.SqlClient;
using PayrollManagementSystem.Database;
using PayrollManagementSystem.Helpers;

namespace PayrollManagementSystem.Forms;

public partial class frmBulkPayroll : Form
{
    // Column name constants — single source of truth for string keys
    private const string ColInclude    = "Include";
    private const string ColEmpID      = "EmployeeID";
    private const string ColName       = "EmployeeName";
    private const string ColBasic      = "BasicSalary";
    private const string ColAllowances = "Allowances";
    private const string ColTax        = "Tax";
    private const string ColOtherDed   = "OtherDeductions";
    private const string ColGross      = "GrossSalary";
    private const string ColEPF        = "EPF";
    private const string ColETF        = "ETF";
    private const string ColNet        = "NetSalary";
    private const string ColStatus     = "Status";

    private static readonly Color ClrNew       = Color.FromArgb(240, 252, 240);
    private static readonly Color ClrProcessed = Color.FromArgb(238, 238, 238);
    private static readonly Color ClrDone      = Color.FromArgb(220, 240, 220);

    private DataTable? _table;

    public frmBulkPayroll()
    {
        InitializeComponent();
    }

    // ── Load ───────────────────────────────────────────────────────────────

    private void btnLoad_Click(object sender, EventArgs e) => LoadEmployees();

    private void LoadEmployees()
    {
        int month = cboMonth.SelectedIndex + 1;
        int year  = (int)nudYear.Value;

        // All active employees; flag those already processed this month/year
        var dt = DatabaseHelper.ExecuteQuery(@"
            SELECT e.EmployeeID,
                   e.FullName,
                   e.BasicSalary,
                   CASE WHEN p.PayrollID IS NOT NULL THEN 1 ELSE 0 END AS AlreadyDone
            FROM   Employees e
            LEFT JOIN PayrollRecords p
                   ON  p.EmployeeID = e.EmployeeID
                   AND p.PayMonth   = @Month
                   AND p.PayYear    = @Year
            WHERE  e.IsActive = 1
            ORDER  BY e.FullName",
            [
                new SqlParameter("@Month", month),
                new SqlParameter("@Year",  year)
            ]);

        // Pre-load OT amounts for this period
        var otDt = DatabaseHelper.ExecuteQuery(
            "SELECT EmployeeID, OTAmount FROM OvertimeRecords WHERE PayMonth=@Month AND PayYear=@Year",
            [new SqlParameter("@Month", month), new SqlParameter("@Year", year)]);
        var otAmounts = otDt.Rows.Cast<DataRow>()
            .ToDictionary(r => Convert.ToInt32(r["EmployeeID"]), r => Convert.ToDecimal(r["OTAmount"]));

        // Pre-load active loan installments
        var loanDt = DatabaseHelper.ExecuteQuery(@"
            SELECT EmployeeID, SUM(MonthlyInstallment) AS TotalInst
            FROM   EmployeeLoans WHERE Status='Active'
            GROUP  BY EmployeeID");
        var loanAmounts = loanDt.Rows.Cast<DataRow>()
            .ToDictionary(r => Convert.ToInt32(r["EmployeeID"]), r => Convert.ToDecimal(r["TotalInst"]));

        _table = BuildDataTable();

        foreach (DataRow src in dt.Rows)
        {
            bool done     = Convert.ToInt32(src["AlreadyDone"]) == 1;
            int empID     = Convert.ToInt32(src["EmployeeID"]);
            decimal basic = Convert.ToDecimal(src["BasicSalary"]);
            decimal allow = otAmounts.GetValueOrDefault(empID, 0m);   // OT as allowance
            decimal other = loanAmounts.GetValueOrDefault(empID, 0m); // loan installment
            decimal gross = basic + allow;
            decimal epf   = Math.Round(gross * PayrollSettings.EpfRate, 2);
            decimal etf   = Math.Round(gross * PayrollSettings.EtfRate, 2);
            decimal tax   = PayrollSettings.CalculateTax(gross);      // auto-calculated
            decimal net   = gross - epf - tax - other;

            _table.Rows.Add(
                !done,
                empID,
                src["FullName"].ToString(),
                basic,
                allow,    // Allowances (OT)
                tax,      // Tax (auto from brackets)
                other,    // Other Deductions (loan installments)
                gross,
                epf,
                etf,
                net,
                done ? "Already Processed" : "New"
            );
        }

        dgvBulk.DataSource = null;
        dgvBulk.DataSource = _table;
        ApplyColumnSettings();
        lblStatus.Text = $"  {dt.Rows.Count} employees loaded for {cboMonth.Text} {year}.";
    }

    private static DataTable BuildDataTable()
    {
        var t = new DataTable();
        t.Columns.Add(ColInclude,    typeof(bool));
        t.Columns.Add(ColEmpID,      typeof(int));
        t.Columns.Add(ColName,       typeof(string));
        t.Columns.Add(ColBasic,      typeof(decimal));
        t.Columns.Add(ColAllowances, typeof(decimal));
        t.Columns.Add(ColTax,        typeof(decimal));
        t.Columns.Add(ColOtherDed,   typeof(decimal));
        t.Columns.Add(ColGross,      typeof(decimal));
        t.Columns.Add(ColEPF,        typeof(decimal));
        t.Columns.Add(ColETF,        typeof(decimal));
        t.Columns.Add(ColNet,        typeof(decimal));
        t.Columns.Add(ColStatus,     typeof(string));
        return t;
    }

    private void ApplyColumnSettings()
    {
        if (dgvBulk.Columns.Count == 0) return;

        dgvBulk.Columns[ColEmpID].Visible = false;

        dgvBulk.Columns[ColInclude].HeaderText    = "✓";
        dgvBulk.Columns[ColInclude].Width         = 36;
        dgvBulk.Columns[ColInclude].AutoSizeMode  = DataGridViewAutoSizeColumnMode.None;

        dgvBulk.Columns[ColName].HeaderText       = "Employee";
        dgvBulk.Columns[ColName].AutoSizeMode     = DataGridViewAutoSizeColumnMode.Fill;
        dgvBulk.Columns[ColName].MinimumWidth     = 180;

        dgvBulk.Columns[ColStatus].HeaderText     = "Status";
        dgvBulk.Columns[ColStatus].AutoSizeMode   = DataGridViewAutoSizeColumnMode.AllCells;

        // Editable numeric columns
        foreach (string col in new[] { ColBasic, ColAllowances, ColTax, ColOtherDed })
        {
            dgvBulk.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvBulk.Columns[col].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvBulk.Columns[col].DefaultCellStyle.Format    = "N2";
        }
        dgvBulk.Columns[ColBasic].HeaderText      = "Basic Salary";
        dgvBulk.Columns[ColAllowances].HeaderText = "Allowances";
        dgvBulk.Columns[ColTax].HeaderText        = "Tax";
        dgvBulk.Columns[ColOtherDed].HeaderText   = "Other Ded.";

        // Calculated (readonly) numeric columns
        foreach (string col in new[] { ColGross, ColEPF, ColETF, ColNet })
        {
            dgvBulk.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvBulk.Columns[col].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvBulk.Columns[col].DefaultCellStyle.Format    = "N2";
        }
        dgvBulk.Columns[ColGross].HeaderText = "Gross";
        dgvBulk.Columns[ColEPF].HeaderText   = $"EPF ({PayrollSettings.EpfLabel})";
        dgvBulk.Columns[ColETF].HeaderText   = $"ETF ({PayrollSettings.EtfLabel})";
        dgvBulk.Columns[ColNet].HeaderText   = "Net Salary";
        dgvBulk.Columns[ColNet].DefaultCellStyle.Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        dgvBulk.Columns[ColNet].DefaultCellStyle.ForeColor = Color.FromArgb(0, 100, 0);

        // Column display order
        dgvBulk.Columns[ColInclude].DisplayIndex    = 0;
        dgvBulk.Columns[ColName].DisplayIndex       = 1;
        dgvBulk.Columns[ColStatus].DisplayIndex     = 2;
        dgvBulk.Columns[ColBasic].DisplayIndex      = 3;
        dgvBulk.Columns[ColAllowances].DisplayIndex = 4;
        dgvBulk.Columns[ColTax].DisplayIndex        = 5;
        dgvBulk.Columns[ColOtherDed].DisplayIndex   = 6;
        dgvBulk.Columns[ColGross].DisplayIndex      = 7;
        dgvBulk.Columns[ColEPF].DisplayIndex        = 8;
        dgvBulk.Columns[ColETF].DisplayIndex        = 9;
        dgvBulk.Columns[ColNet].DisplayIndex        = 10;
    }

    // ── Row styling after bind ─────────────────────────────────────────────

    private void dgvBulk_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
    {
        StyleAllRows();
        UpdateTotals();
    }

    private void StyleAllRows()
    {
        foreach (DataGridViewRow row in dgvBulk.Rows)
            StyleRow(row);
    }

    private static void StyleRow(DataGridViewRow row)
    {
        bool done    = row.Cells[ColStatus].Value?.ToString() == "Already Processed";
        bool checked_ = Convert.ToBoolean(row.Cells[ColInclude].Value);

        if (done)
        {
            row.DefaultCellStyle.BackColor = ClrProcessed;
            row.DefaultCellStyle.ForeColor = Color.Gray;
            // Disable the checkbox visually by making that cell read-only
            row.Cells[ColInclude].ReadOnly = true;
        }
        else
        {
            row.DefaultCellStyle.BackColor = checked_ ? ClrNew : Color.White;
            row.DefaultCellStyle.ForeColor = Color.FromArgb(22, 22, 22);
        }
    }

    // ── Prevent editing readonly / already-processed cells ─────────────────

    private void dgvBulk_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
    {
        if (_table == null) return;
        string col = dgvBulk.Columns[e.ColumnIndex].Name;

        // Calculated columns are never editable
        if (col is ColGross or ColEPF or ColETF or ColNet or ColName or ColStatus or ColEmpID)
        {
            e.Cancel = true;
            return;
        }

        // Already-processed rows: nothing editable (checkbox is already row.ReadOnly)
        var row = (DataRowView)dgvBulk.Rows[e.RowIndex].DataBoundItem!;
        if (row[ColStatus].ToString() == "Already Processed")
            e.Cancel = true;
    }

    // ── Recalculate a row when an editable cell changes ────────────────────

    private void dgvBulk_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
        if (_table == null || e.RowIndex < 0) return;
        string col = dgvBulk.Columns[e.ColumnIndex].Name;
        if (col is not (ColBasic or ColAllowances or ColTax or ColOtherDed)) return;

        RecalcRow(_table.Rows[e.RowIndex]);
        StyleRow(dgvBulk.Rows[e.RowIndex]);
        UpdateTotals();
    }

    private static void RecalcRow(DataRow r)
    {
        decimal basic  = ToDecimal(r[ColBasic]);
        decimal allow  = ToDecimal(r[ColAllowances]);
        decimal other  = ToDecimal(r[ColOtherDed]);
        decimal gross  = basic + allow;
        decimal epf    = Math.Round(gross * PayrollSettings.EpfRate, 2);
        decimal etf    = Math.Round(gross * PayrollSettings.EtfRate, 2);
        decimal tax    = PayrollSettings.CalculateTax(gross);  // recalc from brackets
        decimal net    = gross - epf - tax - other;

        r[ColGross] = gross;
        r[ColEPF]   = epf;
        r[ColETF]   = etf;
        r[ColTax]   = tax;
        r[ColNet]   = net;
    }

    private static decimal ToDecimal(object v) =>
        v == DBNull.Value || v == null ? 0m : Convert.ToDecimal(v);

    // ── Commit checkbox edits immediately ──────────────────────────────────

    private void dgvBulk_CurrentCellDirtyStateChanged(object sender, EventArgs e)
    {
        if (dgvBulk.IsCurrentCellDirty &&
            dgvBulk.CurrentCell is DataGridViewCheckBoxCell)
        {
            dgvBulk.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }

    private void dgvBulk_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || dgvBulk.Columns[e.ColumnIndex].Name != ColInclude) return;
        StyleRow(dgvBulk.Rows[e.RowIndex]);
        UpdateTotals();
    }

    // ── Check / Uncheck All ────────────────────────────────────────────────

    private void btnCheckAll_Click(object sender, EventArgs e)
        => SetAllChecked(true);

    private void btnUncheckAll_Click(object sender, EventArgs e)
        => SetAllChecked(false);

    private void SetAllChecked(bool value)
    {
        if (_table == null) return;
        foreach (DataRow r in _table.Rows)
        {
            if (r[ColStatus].ToString() != "Already Processed")
                r[ColInclude] = value;
        }
        StyleAllRows();
        UpdateTotals();
    }

    // ── Totals footer ──────────────────────────────────────────────────────

    private void UpdateTotals()
    {
        if (_table == null)
        {
            lblTotalGrossVal.Text = lblTotalEPFVal.Text = lblTotalNetVal.Text = "—";
            return;
        }

        decimal gross = 0, epf = 0, net = 0;
        foreach (DataRow r in _table.Rows)
        {
            if (Convert.ToBoolean(r[ColInclude]) && r[ColStatus].ToString() == "New")
            {
                gross += ToDecimal(r[ColGross]);
                epf   += ToDecimal(r[ColEPF]);
                net   += ToDecimal(r[ColNet]);
            }
        }

        lblTotalGrossVal.Text = gross.ToString("N2");
        lblTotalEPFVal.Text   = epf.ToString("N2");
        lblTotalNetVal.Text   = net.ToString("N2");
        lblTotalNetVal.ForeColor = net < 0
            ? Color.FromArgb(196, 43, 28)
            : Color.FromArgb(0, 100, 0);
    }

    // ── Process ────────────────────────────────────────────────────────────

    private void btnProcess_Click(object sender, EventArgs e)
    {
        if (_table == null)
        {
            MessageBox.Show("Load employees first.", "No Data",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        int month = cboMonth.SelectedIndex + 1;
        int year  = (int)nudYear.Value;

        var toProcess = _table.Rows
            .Cast<DataRow>()
            .Where(r => Convert.ToBoolean(r[ColInclude]) &&
                        r[ColStatus].ToString() == "New")
            .ToList();

        if (toProcess.Count == 0)
        {
            MessageBox.Show("No new employees are checked for processing.",
                "Nothing to Do", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Confirm
        if (MessageBox.Show(
                $"Process payroll for {toProcess.Count} employee(s) for {cboMonth.Text} {year}?\n\nThis cannot be undone.",
                "Confirm Bulk Processing",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        string remarks = txtRemarks.Text.Trim();

        try
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            const string sql = @"
                INSERT INTO PayrollRecords
                    (EmployeeID, PayMonth, PayYear,
                     BasicSalary, Allowances, GrossSalary,
                     EPF, ETF, Tax, OtherDeductions, NetSalary, Remarks)
                VALUES
                    (@EmployeeID, @Month, @Year,
                     @Basic, @Allowances, @Gross,
                     @EPF, @ETF, @Tax, @OtherDed, @Net, @Remarks)";

            foreach (DataRow r in toProcess)
            {
                using var cmd = new SqlCommand(sql, conn, tx);
                cmd.Parameters.AddRange(new SqlParameter[]
                {
                    new("@EmployeeID", Convert.ToInt32(r[ColEmpID])),
                    new("@Month",      month),
                    new("@Year",       year),
                    new("@Basic",      ToDecimal(r[ColBasic])),
                    new("@Allowances", ToDecimal(r[ColAllowances])),
                    new("@Gross",      ToDecimal(r[ColGross])),
                    new("@EPF",        ToDecimal(r[ColEPF])),
                    new("@ETF",        ToDecimal(r[ColETF])),
                    new("@Tax",        ToDecimal(r[ColTax])),
                    new("@OtherDed",   ToDecimal(r[ColOtherDed])),
                    new("@Net",        ToDecimal(r[ColNet])),
                    new("@Remarks",    remarks)
                });
                cmd.ExecuteNonQuery();
            }

            tx.Commit();

            // Mark processed rows in the grid without a full reload
            foreach (DataRow r in toProcess)
                r[ColStatus] = "Already Processed";

            SetAllChecked(false);
            StyleAllRows();

            lblStatus.Text = $"  {toProcess.Count} record(s) saved for {cboMonth.Text} {year}.";
            lblStatus.ForeColor = Color.FromArgb(0, 100, 0);

            MessageBox.Show(
                $"Payroll processed successfully for {toProcess.Count} employee(s).",
                "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            MessageBox.Show(
                "One or more employees already have a payroll record for this period.\n" +
                "Reload the list to refresh statuses.",
                "Duplicate Detected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error during processing:\n{ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
