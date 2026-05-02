using System.Drawing;
using System.Windows.Forms;

namespace PayrollManagementSystem.Forms;

partial class frmSettings
{
    private System.ComponentModel.IContainer components = null;

    private Panel     pnlHeader;
    private Label     lblTitle;
    private Label     lblSubtitle;
    private TabControl tabMain;
    private TabPage   tabCompany;
    private TabPage   tabRates;

    // ── Company Info tab ───────────────────────────────────────
    private Label     lblCompanyName;
    private TextBox   txtCompanyName;
    private Label     lblAddress;
    private TextBox   txtAddress;
    private Label     lblPhone;
    private TextBox   txtPhone;
    private Label     lblEmail;
    private TextBox   txtEmail;

    // ── Payroll Rates tab ──────────────────────────────────────
    private Label          lblEpfRate;
    private NumericUpDown  nudEpfRate;
    private Label          lblEpfPct;
    private Label          lblEpfNote;
    private Label          lblEtfRate;
    private NumericUpDown  nudEtfRate;
    private Label          lblEtfPct;
    private Label          lblEtfNote;
    private Panel          pnlRateExample;
    private Label          lblExampleTitle;
    private Label          lblExampleText;

    // ── Bottom buttons ─────────────────────────────────────────
    private Button btnSave;
    private Button btnCancel;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var primaryBlue = Color.FromArgb(0, 120, 212);

        // ── Header ─────────────────────────────────────────────
        pnlHeader   = new Panel();
        lblTitle    = new Label();
        lblSubtitle = new Label();

        pnlHeader.BackColor = primaryBlue;
        pnlHeader.Dock      = DockStyle.Top;
        pnlHeader.Size      = new Size(480, 70);
        pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblSubtitle });

        lblTitle.AutoSize  = true;
        lblTitle.Font      = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location  = new Point(20, 10);
        lblTitle.Text      = "System Settings";

        lblSubtitle.AutoSize  = true;
        lblSubtitle.Font      = new Font("Segoe UI", 9F);
        lblSubtitle.ForeColor = Color.FromArgb(200, 230, 255);
        lblSubtitle.Location  = new Point(22, 42);
        lblSubtitle.Text      = "Company information and payroll rate configuration";

        // ── Tabs ───────────────────────────────────────────────
        tabMain     = new TabControl();
        tabCompany  = new TabPage("Company Info");
        tabRates    = new TabPage("Payroll Rates");
        tabMain.Font     = new Font("Segoe UI", 9.5F);
        tabMain.Location = new Point(10, 80);
        tabMain.Size     = new Size(455, 300);
        tabMain.TabPages.AddRange(new[] { tabCompany, tabRates });

        BuildCompanyTab(primaryBlue);
        BuildRatesTab(primaryBlue);

        // ── Buttons ────────────────────────────────────────────
        btnSave   = new Button();
        btnCancel = new Button();

        btnSave.Text      = "Save Settings";
        btnSave.Location  = new Point(260, 395);
        btnSave.Size      = new Size(120, 34);
        btnSave.BackColor = primaryBlue;
        btnSave.ForeColor = Color.White;
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnSave.Cursor    = Cursors.Hand;
        btnSave.Click    += btnSave_Click;

        btnCancel.Text      = "Cancel";
        btnCancel.Location  = new Point(390, 395);
        btnCancel.Size      = new Size(75, 34);
        btnCancel.BackColor = Color.FromArgb(100, 100, 100);
        btnCancel.ForeColor = Color.White;
        btnCancel.FlatStyle = FlatStyle.Flat;
        btnCancel.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnCancel.Cursor    = Cursors.Hand;
        btnCancel.Click    += (s, e) => Close();

        // ── Form ───────────────────────────────────────────────
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode       = AutoScaleMode.Font;
        BackColor           = Color.FromArgb(245, 247, 250);
        ClientSize          = new Size(480, 445);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(tabMain);
        Controls.Add(pnlHeader);
        Font              = new Font("Segoe UI", 9F);
        FormBorderStyle   = FormBorderStyle.FixedSingle;
        MaximizeBox       = false;
        Name              = "frmSettings";
        StartPosition     = FormStartPosition.CenterParent;
        Text              = "System Settings";
        Load             += frmSettings_Load;
    }

    private void BuildCompanyTab(Color primaryBlue)
    {
        const int lx = 12, vx = 140, vw = 290;

        lblCompanyName = MakeLabel("Company Name:*", new Point(lx, 20));
        txtCompanyName = new TextBox { Location = new Point(vx, 17), Size = new Size(vw, 23) };

        lblAddress = MakeLabel("Address:", new Point(lx, 55));
        txtAddress = new TextBox { Location = new Point(vx, 52), Multiline = true, Size = new Size(vw, 60), ScrollBars = ScrollBars.Vertical };

        lblPhone = MakeLabel("Phone:", new Point(lx, 125));
        txtPhone = new TextBox { Location = new Point(vx, 122), Size = new Size(vw, 23) };

        lblEmail = MakeLabel("Email:", new Point(lx, 158));
        txtEmail = new TextBox { Location = new Point(vx, 155), Size = new Size(vw, 23) };

        tabCompany.Controls.AddRange(new Control[]
        {
            lblCompanyName, txtCompanyName,
            lblAddress,     txtAddress,
            lblPhone,       txtPhone,
            lblEmail,       txtEmail
        });
    }

    private void BuildRatesTab(Color primaryBlue)
    {
        const int lx = 12, nx = 200, sx = 248;

        lblEpfRate = MakeLabel("EPF Employee Rate:", new Point(lx, 25));
        nudEpfRate = new NumericUpDown
        {
            Location      = new Point(nx, 22),
            Size          = new Size(75, 23),
            Minimum       = 0,
            Maximum       = 50,
            DecimalPlaces = 2,
            Increment     = 0.5m,
            Value         = 8.00m
        };
        lblEpfPct = MakeLabel("%", new Point(sx, 25));
        lblEpfNote = new Label
        {
            AutoSize  = true,
            Font      = new Font("Segoe UI", 8F),
            ForeColor = Color.FromArgb(100, 100, 100),
            Location  = new Point(lx, 50),
            Text      = "Deducted from employee gross salary and remitted to EPF."
        };

        lblEtfRate = MakeLabel("ETF Employer Rate:", new Point(lx, 85));
        nudEtfRate = new NumericUpDown
        {
            Location      = new Point(nx, 82),
            Size          = new Size(75, 23),
            Minimum       = 0,
            Maximum       = 50,
            DecimalPlaces = 2,
            Increment     = 0.5m,
            Value         = 3.00m
        };
        lblEtfPct = MakeLabel("%", new Point(sx, 85));
        lblEtfNote = new Label
        {
            AutoSize  = true,
            Font      = new Font("Segoe UI", 8F),
            ForeColor = Color.FromArgb(100, 100, 100),
            Location  = new Point(lx, 110),
            Text      = "Paid by employer — shown on pay slip but NOT deducted from employee net salary."
        };

        pnlRateExample = new Panel
        {
            BackColor = Color.FromArgb(240, 248, 255),
            BorderStyle = BorderStyle.FixedSingle,
            Location  = new Point(lx, 140),
            Size      = new Size(420, 90)
        };
        lblExampleTitle = new Label
        {
            AutoSize  = true,
            Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 80, 160),
            Location  = new Point(8, 8),
            Text      = "Live Preview"
        };
        lblExampleText = new Label
        {
            AutoSize  = false,
            Font      = new Font("Segoe UI", 8.5F),
            ForeColor = Color.FromArgb(30, 30, 30),
            Location  = new Point(8, 28),
            Size      = new Size(404, 55),
            Text      = ""
        };
        pnlRateExample.Controls.AddRange(new Control[] { lblExampleTitle, lblExampleText });

        nudEpfRate.ValueChanged += (s, e) => UpdatePreview();
        nudEtfRate.ValueChanged += (s, e) => UpdatePreview();

        tabRates.Controls.AddRange(new Control[]
        {
            lblEpfRate, nudEpfRate, lblEpfPct, lblEpfNote,
            lblEtfRate, nudEtfRate, lblEtfPct, lblEtfNote,
            pnlRateExample
        });
    }

    private static Label MakeLabel(string text, Point loc) =>
        new() { AutoSize = true, Text = text, Location = loc };
}
