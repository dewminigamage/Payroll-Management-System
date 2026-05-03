using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using PayrollManagementSystem.Database;
using PayrollManagementSystem.Helpers;

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

    private void btnNarrate_Click(object sender, EventArgs e) => _ = NarrateReportAsync();

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

    private async Task NarrateReportAsync()
    {
        if (dgvPayrollReport.Rows.Count == 0)
        {
            MessageBox.Show("Generate the payroll report first, then click Narrate.",
                "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        bool hasGroq   = !string.IsNullOrWhiteSpace(PayrollSettings.GroqApiKey);
        bool hasGemini = !string.IsNullOrWhiteSpace(PayrollSettings.GeminiApiKey);
        if (!hasGroq && !hasGemini)
        {
            MessageBox.Show("No AI key configured.\nOpen AI Payroll Assistant → Configure Key first.",
                "API Key Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        btnNarrate.Enabled = false;
        btnNarrate.Text    = "Writing...";

        try
        {
            int month = cmbPRMonth.SelectedIndex + 1;
            int year  = (int)nudPRYear.Value;

            // Previous month
            int prevMonth = month == 1 ? 12 : month - 1;
            int prevYear  = month == 1 ? year - 1 : year;
            var dtPrev = DatabaseHelper.ExecuteQuery(@"
                SELECT COUNT(*) AS EmpCount,
                       ISNULL(SUM(GrossSalary),0) AS TotalGross,
                       ISNULL(SUM(NetSalary),0)   AS TotalNet,
                       ISNULL(SUM(EPF),0)          AS TotalEPF
                FROM PayrollRecords
                WHERE PayMonth = @M AND PayYear = @Y",
                [new("@M", prevMonth), new("@Y", prevYear)]);

            // OT this month
            var dtOT = DatabaseHelper.ExecuteQuery(@"
                SELECT e.Department,
                       COUNT(DISTINCT o.EmployeeID) AS EmpCount,
                       SUM(o.OTHours)  AS TotalHours,
                       SUM(o.OTAmount) AS TotalAmount
                FROM OvertimeRecords o JOIN Employees e ON o.EmployeeID = e.EmployeeID
                WHERE o.PayMonth = @M AND o.PayYear = @Y
                GROUP BY e.Department",
                [new("@M", month), new("@Y", year)]);

            // Department breakdown from current grid
            var deptTotals = new Dictionary<string, (int Count, decimal Gross, decimal Net)>();
            foreach (DataGridViewRow row in dgvPayrollReport.Rows)
            {
                if (row.IsNewRow) continue;
                var dept  = row.Cells["Department"].Value?.ToString() ?? "Unknown";
                var gross = Convert.ToDecimal(row.Cells["GrossSalary"].Value ?? 0);
                var net   = Convert.ToDecimal(row.Cells["NetSalary"].Value ?? 0);
                if (!deptTotals.TryGetValue(dept, out var t)) t = (0, 0, 0);
                deptTotals[dept] = (t.Count + 1, t.Gross + gross, t.Net + net);
            }

            decimal totalGross = deptTotals.Values.Sum(t => t.Gross);
            decimal totalNet   = deptTotals.Values.Sum(t => t.Net);
            int     totalEmps  = dgvPayrollReport.Rows.Cast<DataGridViewRow>()
                                    .Count(r => !r.IsNewRow);

            var sb = new StringBuilder();
            sb.AppendLine($"Write a professional 2-3 paragraph payroll narrative for {cmbPRMonth.SelectedItem} {year}.");
            sb.AppendLine($"Company: {PayrollSettings.CompanyName}. Write in third person, start with 'The {cmbPRMonth.SelectedItem} {year} payroll...'");
            sb.AppendLine();
            sb.AppendLine($"CURRENT MONTH — {cmbPRMonth.SelectedItem} {year}:");
            sb.AppendLine($"Employees paid: {totalEmps}");
            sb.AppendLine($"Total gross: LKR {totalGross:N0}");
            sb.AppendLine($"Total net payable: LKR {totalNet:N0}");
            sb.AppendLine();
            sb.AppendLine("By department:");
            foreach (var kv in deptTotals)
                sb.AppendLine($"  {kv.Key}: {kv.Value.Count} employees, gross LKR {kv.Value.Gross:N0}, net LKR {kv.Value.Net:N0}");

            if (dtPrev.Rows.Count > 0 && Convert.ToInt32(dtPrev.Rows[0]["EmpCount"]) > 0)
            {
                decimal prevGross = Convert.ToDecimal(dtPrev.Rows[0]["TotalGross"]);
                decimal prevNet   = Convert.ToDecimal(dtPrev.Rows[0]["TotalNet"]);
                double  grossChg  = prevGross > 0 ? (double)((totalGross - prevGross) / prevGross * 100) : 0;
                sb.AppendLine();
                sb.AppendLine($"PREVIOUS MONTH — {prevMonth}/{prevYear}:");
                sb.AppendLine($"Total gross: LKR {prevGross:N0} ({grossChg:+0.0;-0.0}% change this month)");
                sb.AppendLine($"Total net: LKR {prevNet:N0}");
                sb.AppendLine($"Employees paid: {dtPrev.Rows[0]["EmpCount"]}");
            }

            if (dtOT.Rows.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("OVERTIME BY DEPARTMENT:");
                foreach (DataRow r in dtOT.Rows)
                    sb.AppendLine($"  {r["Department"]}: {r["EmpCount"]} employees, {r["TotalHours"]} hrs, LKR {Convert.ToDecimal(r["TotalAmount"]):N0}");
            }

            sb.AppendLine();
            sb.AppendLine("Include: headcount, payroll totals, month-over-month change, department highlights, overtime if any, and a brief outlook note.");

            const string sys = "You are a professional HR analyst. Write clear, concise payroll narrative reports in flowing prose. No bullet points — paragraphs only.";
            var history = new List<(string, string)> { ("user", sb.ToString()) };

            string narrative = hasGroq
                ? await GroqHelper.AskAsync(PayrollSettings.GroqApiKey, sys, history)
                : await GeminiHelper.AskAsync(PayrollSettings.GeminiApiKey, sys, history);

            ShowNarrativeDialog(narrative, $"{cmbPRMonth.SelectedItem} {year} Payroll Narrative");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Narration failed:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnNarrate.Enabled = true;
            btnNarrate.Text    = "Narrate Report";
        }
    }

    private static void ShowNarrativeDialog(string text, string title)
    {
        using var frm = new Form
        {
            Text            = title,
            ClientSize      = new Size(680, 380),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition   = FormStartPosition.CenterParent,
            MaximizeBox     = false
        };
        var rtb = new RichTextBox
        {
            Text       = text,
            Location   = new Point(12, 12),
            Size       = new Size(654, 300),
            ReadOnly   = true,
            BackColor  = Color.White,
            Font       = new Font("Segoe UI", 10F),
            ScrollBars = RichTextBoxScrollBars.Vertical,
            BorderStyle = BorderStyle.FixedSingle
        };
        var btnCopy = new Button
        {
            Text                    = "Copy to Clipboard",
            Location                = new Point(12, 324),
            Size                    = new Size(150, 30),
            UseVisualStyleBackColor = true
        };
        var btnClose = new Button
        {
            Text                    = "Close",
            Location                = new Point(580, 324),
            Size                    = new Size(86, 30),
            DialogResult            = DialogResult.OK,
            UseVisualStyleBackColor = true
        };
        btnCopy.Click += (_, _) =>
        {
            Clipboard.SetText(text);
            btnCopy.Text = "Copied!";
        };
        frm.Controls.AddRange(new Control[] { rtb, btnCopy, btnClose });
        frm.AcceptButton = btnClose;
        frm.ShowDialog();
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

    // ── Tab 4: AI Anomaly Detection ───────────────────────────────────────

    private void btnAnalyseAnomalies_Click(object sender, EventArgs e) => _ = AnalyseAnomaliesAsync();

    private async Task AnalyseAnomaliesAsync()
    {
        int month = cmbAIMonth.SelectedIndex + 1;
        int year  = (int)nudAIYear.Value;

        bool hasGroq   = !string.IsNullOrWhiteSpace(PayrollSettings.GroqApiKey);
        bool hasGemini = !string.IsNullOrWhiteSpace(PayrollSettings.GeminiApiKey);
        if (!hasGroq && !hasGemini)
        {
            MessageBox.Show("No AI key configured.\nOpen AI Payroll Assistant → Configure Key first.",
                "API Key Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        btnAnalyseAnomalies.Enabled = false;
        btnAnalyseAnomalies.Text    = "Analysing...";
        lblAIStatus.Text            = "Loading payroll data...";
        rtbAnomalies.Clear();

        try
        {
            var dtCurrent = DatabaseHelper.ExecuteQuery(@"
                SELECT e.FullName, e.Department,
                       e.BasicSalary  AS EmployeeBasic,
                       p.BasicSalary  AS PaidBasic,
                       p.Allowances, p.GrossSalary,
                       p.EPF, p.Tax, p.OtherDeductions, p.NetSalary
                FROM PayrollRecords p
                JOIN Employees e ON p.EmployeeID = e.EmployeeID
                WHERE p.PayMonth = @M AND p.PayYear = @Y
                ORDER BY e.FullName",
                [new("@M", month), new("@Y", year)]);

            if (dtCurrent.Rows.Count == 0)
            {
                lblAIStatus.Text = "No payroll records found for the selected period.";
                rtbAnomalies.Text = "No data to analyse. Run payroll for this period first.";
                return;
            }

            int prevMonth = month == 1 ? 12 : month - 1;
            int prevYear  = month == 1 ? year - 1 : year;
            var dtPrev = DatabaseHelper.ExecuteQuery(@"
                SELECT e.FullName, p.GrossSalary, p.NetSalary
                FROM PayrollRecords p JOIN Employees e ON p.EmployeeID = e.EmployeeID
                WHERE p.PayMonth = @M AND p.PayYear = @Y",
                [new("@M", prevMonth), new("@Y", prevYear)]);

            var dtOT = DatabaseHelper.ExecuteQuery(@"
                SELECT e.FullName, o.OTHours, o.OTAmount
                FROM OvertimeRecords o JOIN Employees e ON o.EmployeeID = e.EmployeeID
                WHERE o.PayMonth = @M AND o.PayYear = @Y",
                [new("@M", month), new("@Y", year)]);

            // Build the analysis prompt
            var sb = new StringBuilder();
            sb.AppendLine($"You are a professional payroll auditor for {PayrollSettings.CompanyName}.");
            sb.AppendLine($"Analyse the payroll data below for {cmbAIMonth.SelectedItem} {year} and flag anomalies.");
            sb.AppendLine("Check for: incorrect EPF (should be 8% of basic salary), unusual allowances (gross > 1.5× basic),");
            sb.AppendLine("zero or negative net pay, salary changes > 20% vs previous month, overtime > 50 hours, and any other irregularities.");
            sb.AppendLine();

            sb.AppendLine($"PAYROLL — {cmbAIMonth.SelectedItem} {year}:");
            sb.AppendLine("Name | Dept | Emp.Basic | Paid Basic | Allowances | Gross | EPF | Tax | Other Ded. | Net");
            foreach (DataRow r in dtCurrent.Rows)
                sb.AppendLine($"{r["FullName"]} | {r["Department"]} | {r["EmployeeBasic"]:F0} | " +
                              $"{r["PaidBasic"]:F0} | {r["Allowances"]:F0} | {r["GrossSalary"]:F0} | " +
                              $"{r["EPF"]:F0} | {r["Tax"]:F0} | {r["OtherDeductions"]:F0} | {r["NetSalary"]:F0}");

            if (dtPrev.Rows.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine($"PREVIOUS MONTH ({prevMonth}/{prevYear}) for comparison:");
                foreach (DataRow r in dtPrev.Rows)
                    sb.AppendLine($"{r["FullName"]} | Gross: {r["GrossSalary"]:F0} | Net: {r["NetSalary"]:F0}");
            }

            if (dtOT.Rows.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("OVERTIME THIS MONTH:");
                foreach (DataRow r in dtOT.Rows)
                    sb.AppendLine($"{r["FullName"]}: {r["OTHours"]} hrs, LKR {r["OTAmount"]:F0}");
            }

            sb.AppendLine();
            sb.AppendLine("Respond with:");
            sb.AppendLine("• ANOMALIES FOUND: numbered list (employee, finding, risk: Low/Medium/High)");
            sb.AppendLine("• OVERALL ASSESSMENT: one paragraph summary");
            sb.AppendLine("• RECOMMENDED ACTIONS: bullet list");
            sb.AppendLine("If no anomalies found, say so clearly.");

            lblAIStatus.Text = "Sending to AI for analysis...";

            var history = new List<(string, string)> { ("user", sb.ToString()) };
            const string sys = "You are a professional payroll auditor. Give structured, concise anomaly reports.";

            string reply = hasGroq
                ? await GroqHelper.AskAsync(PayrollSettings.GroqApiKey, sys, history)
                : await GeminiHelper.AskAsync(PayrollSettings.GeminiApiKey, sys, history);

            rtbAnomalies.Text = reply;
            lblAIStatus.Text  = $"Analysis complete — {cmbAIMonth.SelectedItem} {year} — {dtCurrent.Rows.Count} employees analysed";
        }
        catch (Exception ex)
        {
            lblAIStatus.Text  = $"Error: {ex.Message}";
            rtbAnomalies.Text = $"Analysis failed:\n\n{ex.Message}";
        }
        finally
        {
            btnAnalyseAnomalies.Enabled = true;
            btnAnalyseAnomalies.Text    = "Analyse Anomalies";
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
