using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using PayrollManagementSystem.Database;

namespace PayrollManagementSystem.Forms;

public partial class frmReports : Form
{
    public frmReports()
    {
        InitializeComponent();
    }

    private void frmReports_Load(object sender, EventArgs e)
    {
        LoadDepartmentFilter();
        GeneratePayrollReport();
        GenerateAttendanceReport();
        GenerateEmployeeReport();
    }

    // ── Department filter for Employee Directory tab ───────────────────────

    private void LoadDepartmentFilter()
    {
        var dt = DatabaseHelper.ExecuteQuery(
            "SELECT DISTINCT Department FROM Employees WHERE Department IS NOT NULL AND Department <> '' ORDER BY Department");

        cmbEmpDept.Items.Clear();
        cmbEmpDept.Items.Add("-- All Departments --");
        foreach (DataRow row in dt.Rows)
            cmbEmpDept.Items.Add(row["Department"].ToString()!);
        cmbEmpDept.SelectedIndex = 0;
    }

    // ── Tab 1: Payroll Summary ─────────────────────────────────────────────

    private void btnGeneratePR_Click(object sender, EventArgs e) => GeneratePayrollReport();

    private void GeneratePayrollReport()
    {
        try
        {
            int month = cmbPRMonth.SelectedIndex + 1;
            int year  = (int)nudPRYear.Value;

            const string query = @"
                SELECT e.FullName      AS [Employee],
                       e.Department,
                       e.Position,
                       p.BasicSalary,
                       p.Allowances,
                       p.GrossSalary,
                       p.EPF,
                       p.ETF,
                       p.Tax,
                       p.OtherDeductions,
                       p.NetSalary,
                       p.Remarks
                FROM   PayrollRecords p
                INNER JOIN Employees e ON p.EmployeeID = e.EmployeeID
                WHERE  p.PayMonth = @Month AND p.PayYear = @Year
                ORDER BY e.FullName";

            var dt = DatabaseHelper.ExecuteQuery(query, [
                new("@Month", month),
                new("@Year",  year)
            ]);

            dgvPayrollReport.DataSource = dt;

            if (dgvPayrollReport.Columns.Count > 0)
            {
                foreach (string col in new[] { "BasicSalary","Allowances","GrossSalary","EPF","ETF","Tax","OtherDeductions","NetSalary" })
                {
                    if (!dgvPayrollReport.Columns.Contains(col)) continue;
                    dgvPayrollReport.Columns[col].DefaultCellStyle.Format    = "N2";
                    dgvPayrollReport.Columns[col].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPayrollReport.Columns[col].AutoSizeMode               = DataGridViewAutoSizeColumnMode.AllCells;
                }

                if (dgvPayrollReport.Columns.Contains("BasicSalary"))
                    dgvPayrollReport.Columns["BasicSalary"].HeaderText = "Basic Salary";
                if (dgvPayrollReport.Columns.Contains("GrossSalary"))
                    dgvPayrollReport.Columns["GrossSalary"].HeaderText = "Gross Salary";
                if (dgvPayrollReport.Columns.Contains("OtherDeductions"))
                    dgvPayrollReport.Columns["OtherDeductions"].HeaderText = "Other Ded.";
                if (dgvPayrollReport.Columns.Contains("NetSalary"))
                    dgvPayrollReport.Columns["NetSalary"].HeaderText = "Net Salary";
                if (dgvPayrollReport.Columns.Contains("EPF"))
                    dgvPayrollReport.Columns["EPF"].HeaderText = "EPF (8%)";
                if (dgvPayrollReport.Columns.Contains("ETF"))
                    dgvPayrollReport.Columns["ETF"].HeaderText = "ETF (3%)";

                dgvPayrollReport.Columns["Employee"].AutoSizeMode  = DataGridViewAutoSizeColumnMode.AllCells;
                dgvPayrollReport.Columns["Position"].AutoSizeMode  = DataGridViewAutoSizeColumnMode.AllCells;
            }

            lblPRStatus.Text = $"Employees: {dt.Rows.Count}  |  " +
                               $"{cmbPRMonth.SelectedItem}  {year}";
            UpdatePayrollTotals(dt);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error generating payroll report:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void UpdatePayrollTotals(DataTable dt)
    {
        if (dt.Rows.Count == 0) { lblPRTotals.Text = "No records for the selected period."; return; }

        decimal gross = dt.AsEnumerable().Sum(r => r.Field<decimal>("GrossSalary"));
        decimal epf   = dt.AsEnumerable().Sum(r => r.Field<decimal>("EPF"));
        decimal etf   = dt.AsEnumerable().Sum(r => r.Field<decimal>("ETF"));
        decimal net   = dt.AsEnumerable().Sum(r => r.Field<decimal>("NetSalary"));

        lblPRTotals.Text = $"Total Gross: {gross:N2}     " +
                           $"Total EPF: {epf:N2}     " +
                           $"Total ETF: {etf:N2}     " +
                           $"Total Net Payable: {net:N2}";
    }

    // ── Tab 2: Attendance Summary ──────────────────────────────────────────

    private void btnGenerateAT_Click(object sender, EventArgs e) => GenerateAttendanceReport();

    private void GenerateAttendanceReport()
    {
        try
        {
            int month = cmbATMonth.SelectedIndex + 1;
            int year  = (int)nudATYear.Value;

            const string query = @"
                SELECT e.FullName      AS [Employee],
                       e.Department,
                       SUM(CASE WHEN a.Status = 'Present'  THEN 1 ELSE 0 END) AS [Present],
                       SUM(CASE WHEN a.Status = 'Absent'   THEN 1 ELSE 0 END) AS [Absent],
                       SUM(CASE WHEN a.Status = 'Late'     THEN 1 ELSE 0 END) AS [Late],
                       SUM(CASE WHEN a.Status = 'Half Day' THEN 1 ELSE 0 END) AS [Half Day],
                       SUM(CASE WHEN a.Status = 'Leave'    THEN 1 ELSE 0 END) AS [Leave],
                       COUNT(*) AS [Total Days]
                FROM   Attendance a
                INNER JOIN Employees e ON a.EmployeeID = e.EmployeeID
                WHERE  MONTH(a.AttendanceDate) = @Month
                  AND  YEAR(a.AttendanceDate)  = @Year
                GROUP BY e.EmployeeID, e.FullName, e.Department
                ORDER BY e.FullName";

            var dt = DatabaseHelper.ExecuteQuery(query, [
                new("@Month", month),
                new("@Year",  year)
            ]);

            dgvAttReport.DataSource = dt;

            if (dgvAttReport.Columns.Count > 0)
            {
                foreach (DataGridViewColumn col in dgvAttReport.Columns)
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                // Let Department and Employee fill remaining space
                if (dgvAttReport.Columns.Contains("Department"))
                    dgvAttReport.Columns["Department"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            lblATStatus.Text = $"Employees: {dt.Rows.Count}  |  " +
                               $"{cmbATMonth.SelectedItem}  {year}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error generating attendance report:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ── Tab 3: Employee Directory ──────────────────────────────────────────

    private void btnGenerateEmp_Click(object sender, EventArgs e) => GenerateEmployeeReport();

    private void GenerateEmployeeReport()
    {
        try
        {
            string dept = cmbEmpDept.SelectedIndex > 0
                ? cmbEmpDept.SelectedItem?.ToString() ?? "" : "";

            string query = @"
                SELECT EmployeeID     AS [ID],
                       FullName       AS [Full Name],
                       NIC,
                       Department,
                       Position,
                       BasicSalary    AS [Basic Salary],
                       JoinDate       AS [Join Date],
                       ContactNumber  AS [Contact No.],
                       Email
                FROM   Employees";

            SqlParameter[]? parameters = null;
            if (!string.IsNullOrEmpty(dept))
            {
                query += " WHERE Department = @Department";
                parameters = [new("@Department", dept)];
            }
            query += " ORDER BY FullName";

            var dt = DatabaseHelper.ExecuteQuery(query, parameters);

            dgvEmpReport.DataSource = dt;

            if (dgvEmpReport.Columns.Count > 0)
            {
                foreach (DataGridViewColumn col in dgvEmpReport.Columns)
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                if (dgvEmpReport.Columns.Contains("Basic Salary"))
                {
                    dgvEmpReport.Columns["Basic Salary"].DefaultCellStyle.Format    = "N2";
                    dgvEmpReport.Columns["Basic Salary"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                if (dgvEmpReport.Columns.Contains("Join Date"))
                    dgvEmpReport.Columns["Join Date"].DefaultCellStyle.Format = "yyyy-MM-dd";

                // Email fills leftover space
                if (dgvEmpReport.Columns.Contains("Email"))
                    dgvEmpReport.Columns["Email"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            lblEmpStatus.Text = $"Employees: {dt.Rows.Count}" +
                                (string.IsNullOrEmpty(dept) ? "" : $"  |  Dept: {dept}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error generating employee report:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ── Export to CSV ──────────────────────────────────────────────────────

    private void btnExportPR_Click(object sender, EventArgs e)
    {
        int month = cmbPRMonth.SelectedIndex + 1;
        ExportToCsv(dgvPayrollReport, $"PayrollSummary_{(int)nudPRYear.Value}_{month:D2}.csv");
    }

    private void btnExportAT_Click(object sender, EventArgs e)
    {
        int month = cmbATMonth.SelectedIndex + 1;
        ExportToCsv(dgvAttReport, $"AttendanceSummary_{(int)nudATYear.Value}_{month:D2}.csv");
    }

    private void btnExportEmp_Click(object sender, EventArgs e)
        => ExportToCsv(dgvEmpReport, "EmployeeDirectory.csv");

    private static void ExportToCsv(DataGridView dgv, string defaultFileName)
    {
        if (dgv.Rows.Count == 0)
        {
            MessageBox.Show("No data to export. Click Generate first.", "No Data",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var sfd = new SaveFileDialog
        {
            Filter   = "CSV files (*.csv)|*.csv",
            FileName = defaultFileName
        };
        if (sfd.ShowDialog() != DialogResult.OK) return;

        try
        {
            var sb          = new StringBuilder();
            var visibleCols = dgv.Columns.Cast<DataGridViewColumn>()
                .Where(c => c.Visible)
                .OrderBy(c => c.DisplayIndex)
                .ToList();

            // Header row
            sb.AppendLine(string.Join(",", visibleCols.Select(c => $"\"{c.HeaderText}\"")));

            // Data rows
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;
                sb.AppendLine(string.Join(",",
                    visibleCols.Select(c => $"\"{row.Cells[c.Index].FormattedValue}\"")));
            }

            File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
            MessageBox.Show($"Exported {dgv.Rows.Count} record(s) to:\n{sfd.FileName}",
                "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Export failed:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
