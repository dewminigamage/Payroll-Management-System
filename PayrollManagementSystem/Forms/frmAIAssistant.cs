using System.Data;
using PayrollManagementSystem.Database;
using PayrollManagementSystem.Helpers;

namespace PayrollManagementSystem.Forms;

public partial class frmAIAssistant : Form
{
    private readonly List<(string Role, string Text)> _history = new();
    private string _systemPrompt = BuildFallbackPrompt();

    private static readonly Font _boldFont   = new("Segoe UI", 9.5F, FontStyle.Bold);
    private static readonly Font _normalFont = new("Segoe UI", 9.5F, FontStyle.Regular);

    public frmAIAssistant()
    {
        InitializeComponent();
        UpdateKeyStatus();
    }

    private void UpdateKeyStatus()
    {
        if (!string.IsNullOrWhiteSpace(PayrollSettings.GroqApiKey))
        {
            lblStatus.ForeColor = Color.FromArgb(20, 120, 40);
            lblStatus.Text      = $"Groq ({GroqHelper.Model}) ready. Click 'Load Context' to pull live payroll data.";
        }
        else if (!string.IsNullOrWhiteSpace(PayrollSettings.GeminiApiKey))
        {
            lblStatus.ForeColor = Color.FromArgb(20, 120, 40);
            lblStatus.Text      = $"Gemini ({GeminiHelper.Model}) ready. Click 'Load Context' to pull live payroll data.";
        }
        else
        {
            lblStatus.ForeColor = Color.FromArgb(160, 80, 0);
            lblStatus.Text      = "No AI key set. Click 'Configure AI' — Groq is free and works globally.";
        }
    }

    // ── Configure AI Key ─────────────────────────────────────────────────
    private void btnConfigureKey_Click(object sender, EventArgs e)
    {
        using var frm = new Form
        {
            Text            = "Configure AI Provider",
            ClientSize      = new Size(480, 260),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition   = FormStartPosition.CenterParent,
            MaximizeBox     = false,
            MinimizeBox     = false
        };
        var f9 = new Font("Segoe UI", 9F);
        var f8 = new Font("Segoe UI", 8F);

        // ── Groq section (recommended) ────────────────────────────────────
        var lblGroq = new Label { Text = "Groq API Key  (recommended — free globally, no card needed)",
            Location = new Point(12, 14), AutoSize = true, Font = f9,
            ForeColor = Color.FromArgb(0, 110, 40) };
        var txtGroq = new TextBox { Location = new Point(12, 36), Size = new Size(450, 23),
            Text = PayrollSettings.GroqApiKey, Font = f9 };
        var noteGroq = new Label { Text = "Get a free key at console.groq.com → API Keys → Create API Key",
            Location = new Point(12, 64), AutoSize = true, Font = f8,
            ForeColor = Color.FromArgb(100, 100, 100) };

        // ── Divider ───────────────────────────────────────────────────────
        var sep = new Label { Text = "──── OR use Gemini (requires billing in some regions) ────",
            Location = new Point(12, 90), AutoSize = true, Font = f8,
            ForeColor = Color.FromArgb(150, 150, 150) };

        // ── Gemini section ────────────────────────────────────────────────
        var lblGem = new Label { Text = "Gemini API Key  (aistudio.google.com):",
            Location = new Point(12, 112), AutoSize = true, Font = f9 };
        var txtGem = new TextBox { Location = new Point(12, 132), Size = new Size(450, 23),
            Text = PayrollSettings.GeminiApiKey, Font = f9 };

        // ── Groq model selector ───────────────────────────────────────────
        var lblMdl = new Label { Text = "Groq model:",
            Location = new Point(12, 170), AutoSize = true, Font = f9 };
        var cmbMdl = new ComboBox { Location = new Point(90, 167), Size = new Size(260, 23),
            DropDownStyle = ComboBoxStyle.DropDownList, Font = f9 };
        cmbMdl.Items.AddRange(["llama-3.3-70b-versatile", "llama3-8b-8192",
                                "llama3-70b-8192", "mixtral-8x7b-32768"]);
        cmbMdl.SelectedItem = GroqHelper.Model;
        if (cmbMdl.SelectedIndex < 0) cmbMdl.SelectedIndex = 0;

        var btnOk     = new Button { Text = "Save",   Location = new Point(288, 210),
            Size = new Size(80, 28), DialogResult = DialogResult.OK,
            UseVisualStyleBackColor = true };
        var btnCancel = new Button { Text = "Cancel", Location = new Point(378, 210),
            Size = new Size(80, 28), DialogResult = DialogResult.Cancel,
            UseVisualStyleBackColor = true };

        frm.Controls.AddRange(new Control[]
            { lblGroq, txtGroq, noteGroq, sep, lblGem, txtGem, lblMdl, cmbMdl, btnOk, btnCancel });
        frm.AcceptButton = btnOk;
        frm.CancelButton = btnCancel;

        if (frm.ShowDialog(this) != DialogResult.OK) return;

        GroqHelper.Model = cmbMdl.SelectedItem?.ToString() ?? "llama-3.3-70b-versatile";
        try
        {
            PayrollSettings.SaveGroqApiKey(txtGroq.Text.Trim());
            PayrollSettings.SaveGeminiApiKey(txtGem.Text.Trim());
            UpdateKeyStatus();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not save key: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ── Load Context ─────────────────────────────────────────────────────
    private void btnLoadContext_Click(object sender, EventArgs e)
    {
        btnLoadContext.Enabled = false;
        try
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"You are an AI payroll assistant for {PayrollSettings.CompanyName}.");
            sb.AppendLine("Help HR staff understand payroll data. Be concise and professional.");
            sb.AppendLine($"Today's date: {DateTime.Today:dd MMM yyyy}");
            sb.AppendLine();

            // Active employees
            try
            {
                var dt  = DatabaseHelper.ExecuteQuery("SELECT COUNT(*) AS C FROM Employees WHERE IsActive=1");
                var cnt = Convert.ToInt32(dt.Rows[0]["C"]);
                sb.AppendLine($"Active employees: {cnt}");
            }
            catch { sb.AppendLine("Active employees: (unavailable)"); }

            // Recent payroll runs
            try
            {
                var dt = DatabaseHelper.ExecuteQuery(@"
                    SELECT TOP 3 PayMonth, PayYear,
                           COUNT(*) AS Records,
                           CAST(SUM(NetSalary) AS DECIMAL(18,2)) AS TotalNet
                    FROM PayrollRecords
                    GROUP BY PayMonth, PayYear
                    ORDER BY PayYear DESC, PayMonth DESC");
                if (dt.Rows.Count > 0)
                {
                    sb.AppendLine("Recent payroll runs:");
                    foreach (DataRow r in dt.Rows)
                        sb.AppendLine($"  {r["PayMonth"]}/{r["PayYear"]}: {r["Records"]} records, LKR {Convert.ToDecimal(r["TotalNet"]):N2} net");
                }
                else sb.AppendLine("No payroll records found.");
            }
            catch { sb.AppendLine("Payroll history: (unavailable)"); }

            // Active loans
            try
            {
                var dt = DatabaseHelper.ExecuteQuery(
                    "SELECT COUNT(*) AS Cnt, ISNULL(SUM(RemainingBalance),0) AS Bal FROM EmployeeLoans WHERE Status='Active'");
                sb.AppendLine($"Active loans: {dt.Rows[0]["Cnt"]}, outstanding: LKR {Convert.ToDecimal(dt.Rows[0]["Bal"]):N2}");
            }
            catch { sb.AppendLine("Loan data: (unavailable)"); }

            // Overtime this month
            try
            {
                var dt = DatabaseHelper.ExecuteQuery(@"
                    SELECT ISNULL(SUM(OTHours),0) AS Hours,
                           ISNULL(SUM(OTAmount),0) AS Amount,
                           COUNT(DISTINCT EmployeeID) AS EmpCount
                    FROM OvertimeRecords
                    WHERE PayMonth = MONTH(GETDATE()) AND PayYear = YEAR(GETDATE())");
                sb.AppendLine($"Overtime this month: {dt.Rows[0]["EmpCount"]} employees, {dt.Rows[0]["Hours"]} hrs, LKR {Convert.ToDecimal(dt.Rows[0]["Amount"]):N2}");
            }
            catch { sb.AppendLine("Overtime data: (unavailable)"); }

            // Leave this year
            try
            {
                var dt = DatabaseHelper.ExecuteQuery(@"
                    SELECT COUNT(*) AS Reqs, ISNULL(SUM(Days),0) AS Days
                    FROM LeaveRecords WHERE YEAR(StartDate) = YEAR(GETDATE())");
                sb.AppendLine($"Leave this year: {dt.Rows[0]["Reqs"]} requests, {dt.Rows[0]["Days"]} total days");
            }
            catch { sb.AppendLine("Leave data: (unavailable)"); }

            sb.AppendLine();
            sb.AppendLine("If asked about specific employee details not in this summary, direct users to the relevant module.");

            _systemPrompt       = sb.ToString();
            lblStatus.ForeColor = Color.FromArgb(20, 120, 40);
            lblStatus.Text      = $"Context loaded at {DateTime.Now:HH:mm}. Ready to answer questions.";

            AppendLine("System",
                "Context loaded from database. You can now ask questions about your payroll data.",
                Color.FromArgb(80, 80, 80));
        }
        catch (Exception ex)
        {
            lblStatus.ForeColor = Color.FromArgb(180, 0, 0);
            lblStatus.Text      = $"Context load failed: {ex.Message}";
        }
        finally
        {
            btnLoadContext.Enabled = true;
        }
    }

    // ── Clear Chat ────────────────────────────────────────────────────────
    private void btnClearChat_Click(object sender, EventArgs e)
    {
        _history.Clear();
        rtbChat.Clear();
    }

    // ── Input handling ────────────────────────────────────────────────────
    private void txtInput_KeyDown(object sender, KeyEventArgs e)
    {
        // Ctrl+Enter sends the message; plain Enter adds a new line
        if (e.KeyCode == Keys.Enter && e.Control)
        {
            e.SuppressKeyPress = true;
            _ = SendAsync();
        }
    }

    private void btnSend_Click(object sender, EventArgs e) => _ = SendAsync();

    private async Task SendAsync()
    {
        var userText = txtInput.Text.Trim();
        if (string.IsNullOrEmpty(userText)) return;

        bool hasGroq   = !string.IsNullOrWhiteSpace(PayrollSettings.GroqApiKey);
        bool hasGemini = !string.IsNullOrWhiteSpace(PayrollSettings.GeminiApiKey);
        if (!hasGroq && !hasGemini)
        {
            MessageBox.Show("Please configure an AI API key first.\nClick 'Configure Key' — Groq is free and works globally.",
                "API Key Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        txtInput.Clear();
        SetInputEnabled(false);

        AppendLine("You", userText, Color.FromArgb(0, 80, 160));
        _history.Add(("user", userText));

        try
        {
            string reply;
            if (hasGroq)
            {
                reply = await GroqHelper.AskAsync(PayrollSettings.GroqApiKey, _systemPrompt, _history);
                _history.Add(("model", reply));
                AppendLine("Groq", reply, Color.FromArgb(0, 120, 60));
            }
            else
            {
                reply = await GeminiHelper.AskAsync(PayrollSettings.GeminiApiKey, _systemPrompt, _history);
                _history.Add(("model", reply));
                AppendLine("Gemini", reply, Color.FromArgb(0, 120, 60));
            }
        }
        catch (Exception ex)
        {
            AppendLine("Error", ex.Message, Color.FromArgb(180, 0, 0));
        }
        finally
        {
            SetInputEnabled(true);
            txtInput.Focus();
        }
    }

    private void SetInputEnabled(bool enabled)
    {
        txtInput.Enabled = enabled;
        btnSend.Enabled  = enabled;
        btnSend.Text     = enabled ? "Send" : "...";
    }

    // ── Chat display ──────────────────────────────────────────────────────
    private void AppendLine(string sender, string text, Color labelColor)
    {
        if (rtbChat.TextLength > 0)
            rtbChat.AppendText(Environment.NewLine);

        // Bold coloured label
        rtbChat.SelectionStart  = rtbChat.TextLength;
        rtbChat.SelectionColor  = labelColor;
        rtbChat.SelectionFont   = _boldFont;
        rtbChat.AppendText($"{sender}: ");

        // Normal dark body
        rtbChat.SelectionStart  = rtbChat.TextLength;
        rtbChat.SelectionColor  = Color.FromArgb(35, 35, 35);
        rtbChat.SelectionFont   = _normalFont;
        rtbChat.AppendText(text + Environment.NewLine);

        rtbChat.SelectionStart = rtbChat.TextLength;
        rtbChat.ScrollToCaret();
    }

    private static string BuildFallbackPrompt() =>
        $"You are an AI payroll assistant. Today is {DateTime.Today:dd MMM yyyy}. " +
        "Help HR staff with payroll-related questions. Load the database context for specific data.";
}
