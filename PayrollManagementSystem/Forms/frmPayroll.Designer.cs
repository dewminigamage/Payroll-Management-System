using System.Drawing;
using System.Windows.Forms;

namespace PayrollManagementSystem.Forms;

partial class frmPayroll
{
    private System.ComponentModel.IContainer components = null;

    // Header
    private Panel        pnlHeader;
    private Label        lblTitle;
    private Label        lblSubtitle;

    // Left panel – entry form
    private Panel        pnlLeft;
    private Label        lblPayrollID;
    private TextBox      txtPayrollID;
    private Label        lblEmployee;
    private ComboBox     cmbEmployee;
    private Label        lblPayMonth;
    private ComboBox     cmbPayMonth;
    private Label        lblPayYear;
    private NumericUpDown nudPayYear;
    private Label        lblBasicSalary;
    private TextBox      txtBasicSalary;
    private Label        lblAllowances;
    private TextBox      txtAllowances;
    private Label        lblOtherDeductions;
    private TextBox      txtOtherDeductions;
    private Label        lblTax;
    private TextBox      txtTax;
    private Label        lblRemarks;
    private TextBox      txtRemarks;

    // Summary panel
    private Panel        pnlSummary;
    private Label        lblGrossLbl;
    private Label        lblGrossVal;
    private Label        lblEPFLbl;
    private Label        lblEPFVal;
    private Label        lblETFLbl;
    private Label        lblETFVal;
    private Label        lblNetLbl;
    private Label        lblNetVal;

    // Action buttons
    private Button       btnCalculate;
    private Button       btnSave;
    private Button       btnUpdate;
    private Button       btnDelete;
    private Button       btnClear;

    // Right panel – filter + grid
    private Panel        pnlRight;
    private Label        lblFilterEmployee;
    private ComboBox     cmbFilterEmployee;
    private Label        lblFilterMonth;
    private ComboBox     cmbFilterMonth;
    private Label        lblFilterYear;
    private NumericUpDown nudFilterYear;
    private Button       btnFilter;
    private Button       btnClearFilter;
    private DataGridView dgvPayroll;
    private Label        lblStatus;
    private Button       btnPaySlip;

    private Label        lblFooter;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var blue    = Color.FromArgb(0, 120, 212);
        var lightBg = Color.FromArgb(245, 247, 250);
        var inputBg = Color.FromArgb(240, 242, 245);
        var sumBg   = Color.FromArgb(240, 246, 255);

        // ── Instantiate controls ───────────────────────────────
        pnlHeader          = new Panel();
        lblTitle           = new Label();
        lblSubtitle        = new Label();
        pnlLeft            = new Panel();
        lblPayrollID       = new Label();
        txtPayrollID       = new TextBox();
        lblEmployee        = new Label();
        cmbEmployee        = new ComboBox();
        lblPayMonth        = new Label();
        cmbPayMonth        = new ComboBox();
        lblPayYear         = new Label();
        nudPayYear         = new NumericUpDown();
        lblBasicSalary     = new Label();
        txtBasicSalary     = new TextBox();
        lblAllowances      = new Label();
        txtAllowances      = new TextBox();
        lblOtherDeductions = new Label();
        txtOtherDeductions = new TextBox();
        lblTax             = new Label();
        txtTax             = new TextBox();
        lblRemarks         = new Label();
        txtRemarks         = new TextBox();
        pnlSummary         = new Panel();
        lblGrossLbl        = new Label();
        lblGrossVal        = new Label();
        lblEPFLbl          = new Label();
        lblEPFVal          = new Label();
        lblETFLbl          = new Label();
        lblETFVal          = new Label();
        lblNetLbl          = new Label();
        lblNetVal          = new Label();
        btnCalculate       = new Button();
        btnSave            = new Button();
        btnUpdate          = new Button();
        btnDelete          = new Button();
        btnClear           = new Button();
        pnlRight           = new Panel();
        lblFilterEmployee  = new Label();
        cmbFilterEmployee  = new ComboBox();
        lblFilterMonth     = new Label();
        cmbFilterMonth     = new ComboBox();
        lblFilterYear      = new Label();
        nudFilterYear      = new NumericUpDown();
        btnFilter          = new Button();
        btnClearFilter     = new Button();
        dgvPayroll         = new DataGridView();
        lblStatus          = new Label();
        btnPaySlip         = new Button();
        lblFooter          = new Label();

        SuspendLayout();
        pnlHeader.SuspendLayout();
        pnlLeft.SuspendLayout();
        pnlSummary.SuspendLayout();
        pnlRight.SuspendLayout();

        // ── Header ─────────────────────────────────────────────
        pnlHeader.BackColor = blue;
        pnlHeader.Controls.Add(lblSubtitle);
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Size = new Size(1060, 80);

        lblTitle.AutoSize  = true;
        lblTitle.Font      = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location  = new Point(25, 12);
        lblTitle.Text      = "Payroll / Salary Calculation";

        lblSubtitle.AutoSize  = true;
        lblSubtitle.Font      = new Font("Segoe UI", 9.5F);
        lblSubtitle.ForeColor = Color.FromArgb(200, 230, 255);
        lblSubtitle.Location  = new Point(27, 48);
        lblSubtitle.Text      = "Calculate and record monthly employee payroll";

        // ── Left panel ─────────────────────────────────────────
        pnlLeft.BackColor   = Color.White;
        pnlLeft.BorderStyle = BorderStyle.FixedSingle;
        pnlLeft.Location    = new Point(12, 90);
        pnlLeft.Size        = new Size(392, 460);
        foreach (Control c in new Control[] {
            lblPayrollID, txtPayrollID,
            lblEmployee, cmbEmployee,
            lblPayMonth, cmbPayMonth, lblPayYear, nudPayYear,
            lblBasicSalary, txtBasicSalary,
            lblAllowances, txtAllowances,
            lblOtherDeductions, txtOtherDeductions,
            lblTax, txtTax,
            lblRemarks, txtRemarks,
            pnlSummary,
            btnCalculate, btnSave, btnUpdate, btnDelete, btnClear })
            pnlLeft.Controls.Add(c);

        // Layout constants
        const int lx  = 10;   // label x
        const int ix  = 145;  // input x
        const int iw  = 232;  // input width
        const int rh  = 26;   // row height
        const int gap = 6;    // vertical gap
        int ly = 15;          // current row y

        // Row 1 – Payroll ID (read-only)
        SetLbl(lblPayrollID, "Payroll ID", lx, ly + 3);
        txtPayrollID.Location  = new Point(ix, ly);
        txtPayrollID.Size      = new Size(80, rh);
        txtPayrollID.ReadOnly  = true;
        txtPayrollID.BackColor = inputBg;
        txtPayrollID.TabStop   = false;

        // Row 2 – Employee
        ly += rh + gap;
        SetLbl(lblEmployee, "Employee *", lx, ly + 3);
        cmbEmployee.Location      = new Point(ix, ly);
        cmbEmployee.Size          = new Size(iw, rh);
        cmbEmployee.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbEmployee.SelectedIndexChanged += new EventHandler(cmbEmployee_SelectedIndexChanged);

        // Row 3 – Month + Year (shared row)
        ly += rh + gap;
        SetLbl(lblPayMonth, "Pay Month *", lx, ly + 3);
        cmbPayMonth.Location      = new Point(ix, ly);
        cmbPayMonth.Size          = new Size(115, rh);
        cmbPayMonth.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPayMonth.Items.AddRange(new object[] {
            "January","February","March","April","May","June",
            "July","August","September","October","November","December" });
        cmbPayMonth.SelectedIndex = DateTime.Today.Month - 1;

        SetLbl(lblPayYear, "Year", ix + 123, ly + 3);
        nudPayYear.Location      = new Point(ix + 158, ly);
        nudPayYear.Size          = new Size(74, rh);
        nudPayYear.Minimum       = 2000;
        nudPayYear.Maximum       = 2099;
        nudPayYear.Value         = DateTime.Today.Year;
        nudPayYear.DecimalPlaces = 0;

        // Row 4 – Basic Salary (auto-filled, read-only)
        ly += rh + gap;
        SetLbl(lblBasicSalary, "Basic Salary", lx, ly + 3);
        txtBasicSalary.Location  = new Point(ix, ly);
        txtBasicSalary.Size      = new Size(iw, rh);
        txtBasicSalary.ReadOnly  = true;
        txtBasicSalary.BackColor = inputBg;
        txtBasicSalary.TabStop   = false;
        txtBasicSalary.TextAlign = HorizontalAlignment.Right;

        // Row 5 – Allowances
        ly += rh + gap;
        SetLbl(lblAllowances, "Allowances", lx, ly + 3);
        txtAllowances.Location  = new Point(ix, ly);
        txtAllowances.Size      = new Size(iw, rh);
        txtAllowances.Text      = "0";
        txtAllowances.TextAlign = HorizontalAlignment.Right;

        // Row 6 – Other Deductions
        ly += rh + gap;
        SetLbl(lblOtherDeductions, "Other Deductions", lx, ly + 3);
        txtOtherDeductions.Location  = new Point(ix, ly);
        txtOtherDeductions.Size      = new Size(iw, rh);
        txtOtherDeductions.Text      = "0";
        txtOtherDeductions.TextAlign = HorizontalAlignment.Right;

        // Row 7 – Tax
        ly += rh + gap;
        SetLbl(lblTax, "Tax", lx, ly + 3);
        txtTax.Location  = new Point(ix, ly);
        txtTax.Size      = new Size(iw, rh);
        txtTax.Text      = "0";
        txtTax.TextAlign = HorizontalAlignment.Right;

        // Row 8 – Remarks
        ly += rh + gap;
        SetLbl(lblRemarks, "Remarks", lx, ly + 3);
        txtRemarks.Location = new Point(ix, ly);
        txtRemarks.Size     = new Size(iw, rh);

        // ── Summary panel ──────────────────────────────────────
        ly += rh + 16;
        pnlSummary.BackColor   = sumBg;
        pnlSummary.BorderStyle = BorderStyle.FixedSingle;
        pnlSummary.Location    = new Point(10, ly);
        pnlSummary.Size        = new Size(370, 112);
        foreach (Control c in new Control[] {
            lblGrossLbl, lblGrossVal,
            lblEPFLbl,   lblEPFVal,
            lblETFLbl,   lblETFVal,
            lblNetLbl,   lblNetVal })
            pnlSummary.Controls.Add(c);

        const int sv = 195;  // summary value x
        const int sw = 165;  // summary value width

        SetSumLbl(lblGrossLbl, "Gross Salary:",      10, 10);
        SetSumVal(lblGrossVal, "0.00",               sv, 10, sw);
        SetSumLbl(lblEPFLbl,   "EPF (8% employee):", 10, 32);
        SetSumVal(lblEPFVal,   "0.00",               sv, 32, sw);
        SetSumLbl(lblETFLbl,   "ETF (3% employer):", 10, 54);
        SetSumVal(lblETFVal,   "0.00",               sv, 54, sw);

        lblNetLbl.AutoSize  = true;
        lblNetLbl.Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblNetLbl.ForeColor = Color.FromArgb(0, 100, 0);
        lblNetLbl.Location  = new Point(10, 80);
        lblNetLbl.Text      = "Net Salary:";

        lblNetVal.AutoSize  = false;
        lblNetVal.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblNetVal.ForeColor = Color.FromArgb(0, 100, 0);
        lblNetVal.Location  = new Point(sv, 78);
        lblNetVal.Size      = new Size(sw, 22);
        lblNetVal.TextAlign = ContentAlignment.MiddleRight;
        lblNetVal.Text      = "0.00";

        // ── Action buttons ─────────────────────────────────────
        ly += 122;
        SetBtn(btnCalculate, "Calculate", 10,  ly, 80, 32, Color.FromArgb(0, 150, 80),   Color.White);
        SetBtn(btnSave,      "Save",      98,  ly, 62, 32, blue,                          Color.White);
        SetBtn(btnUpdate,    "Update",    168, ly, 62, 32, Color.FromArgb(210, 130, 0),   Color.White);
        SetBtn(btnDelete,    "Delete",    238, ly, 62, 32, Color.FromArgb(196, 43, 28),   Color.White);
        SetBtn(btnClear,     "Clear",     308, ly, 62, 32, Color.FromArgb(100, 100, 100), Color.White);
        btnCalculate.Click += new EventHandler(btnCalculate_Click);
        btnSave.Click      += new EventHandler(btnSave_Click);
        btnUpdate.Click    += new EventHandler(btnUpdate_Click);
        btnDelete.Click    += new EventHandler(btnDelete_Click);
        btnClear.Click     += new EventHandler(btnClear_Click);

        // ── Right panel ────────────────────────────────────────
        pnlRight.BackColor   = Color.White;
        pnlRight.BorderStyle = BorderStyle.FixedSingle;
        pnlRight.Location    = new Point(416, 90);
        pnlRight.Size        = new Size(634, 460);
        foreach (Control c in new Control[] {
            lblFilterEmployee, cmbFilterEmployee,
            lblFilterMonth,    cmbFilterMonth,
            lblFilterYear,     nudFilterYear,
            btnFilter, btnClearFilter,
            dgvPayroll, lblStatus, btnPaySlip })
            pnlRight.Controls.Add(c);

        // Filter row
        const int fy = 10;
        SetLbl(lblFilterEmployee, "Employee:", 10, fy + 3);
        cmbFilterEmployee.Location      = new Point(78, fy);
        cmbFilterEmployee.Size          = new Size(155, 26);
        cmbFilterEmployee.DropDownStyle = ComboBoxStyle.DropDownList;

        SetLbl(lblFilterMonth, "Month:", 243, fy + 3);
        cmbFilterMonth.Location      = new Point(292, fy);
        cmbFilterMonth.Size          = new Size(105, 26);
        cmbFilterMonth.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbFilterMonth.Items.Add("-- All Months --");
        cmbFilterMonth.Items.AddRange(new object[] {
            "January","February","March","April","May","June",
            "July","August","September","October","November","December" });
        cmbFilterMonth.SelectedIndex = 0;

        SetLbl(lblFilterYear, "Year:", 407, fy + 3);
        nudFilterYear.Location      = new Point(445, fy);
        nudFilterYear.Size          = new Size(68, 26);
        nudFilterYear.Minimum       = 2000;
        nudFilterYear.Maximum       = 2099;
        nudFilterYear.Value         = DateTime.Today.Year;
        nudFilterYear.DecimalPlaces = 0;

        SetBtn(btnFilter,      "Filter", 522, fy,      90, 26, blue,           Color.White);
        SetBtn(btnClearFilter, "Reset",  522, fy + 30, 90, 26, Color.DimGray,  Color.White);
        btnFilter.Click      += new EventHandler(btnFilter_Click);
        btnClearFilter.Click += new EventHandler(btnClearFilter_Click);

        // DataGridView
        dgvPayroll.Location                                = new Point(10, 48);
        dgvPayroll.Size                                    = new Size(610, 380);
        dgvPayroll.ReadOnly                                = true;
        dgvPayroll.SelectionMode                           = DataGridViewSelectionMode.FullRowSelect;
        dgvPayroll.MultiSelect                             = false;
        dgvPayroll.AllowUserToAddRows                      = false;
        dgvPayroll.AllowUserToDeleteRows                   = false;
        dgvPayroll.BackgroundColor                         = Color.White;
        dgvPayroll.BorderStyle                             = BorderStyle.None;
        dgvPayroll.RowHeadersVisible                       = false;
        dgvPayroll.Font                                    = new Font("Segoe UI", 9F);
        dgvPayroll.ColumnHeadersDefaultCellStyle.BackColor = blue;
        dgvPayroll.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvPayroll.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        dgvPayroll.EnableHeadersVisualStyles               = false;
        dgvPayroll.CellClick                              += new DataGridViewCellEventHandler(dgvPayroll_CellClick);

        lblStatus.AutoSize  = true;
        lblStatus.Font      = new Font("Segoe UI", 9F);
        lblStatus.ForeColor = Color.FromArgb(80, 80, 80);
        lblStatus.Location  = new Point(10, 437);
        lblStatus.Text      = "Records: 0";

        SetBtn(btnPaySlip, "View Pay Slip", 460, 430, 120, 26,
            Color.FromArgb(0, 120, 212), Color.White);
        btnPaySlip.Click += new EventHandler(btnPaySlip_Click);

        // ── Footer ─────────────────────────────────────────────
        lblFooter.AutoSize  = false;
        lblFooter.Font      = new Font("Segoe UI", 8F);
        lblFooter.ForeColor = Color.FromArgb(150, 150, 150);
        lblFooter.Location  = new Point(0, 558);
        lblFooter.Size      = new Size(1060, 20);
        lblFooter.Text      = "   Payroll Management System  |  2025";
        lblFooter.TextAlign = ContentAlignment.MiddleLeft;

        // ── Form ───────────────────────────────────────────────
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode       = AutoScaleMode.Font;
        BackColor           = lightBg;
        ClientSize          = new Size(1060, 580);
        Controls.Add(lblFooter);
        Controls.Add(pnlRight);
        Controls.Add(pnlLeft);
        Controls.Add(pnlHeader);
        Font            = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        Name            = "frmPayroll";
        StartPosition   = FormStartPosition.CenterScreen;
        Text            = "Payroll / Salary Calculation";
        Load           += new EventHandler(frmPayroll_Load);

        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        pnlLeft.ResumeLayout(false);
        pnlLeft.PerformLayout();
        pnlSummary.ResumeLayout(false);
        pnlSummary.PerformLayout();
        pnlRight.ResumeLayout(false);
        pnlRight.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private static void SetLbl(Label lbl, string text, int x, int y)
    {
        lbl.AutoSize  = true;
        lbl.Font      = new Font("Segoe UI", 9F);
        lbl.ForeColor = Color.FromArgb(60, 60, 60);
        lbl.Location  = new Point(x, y);
        lbl.Text      = text;
    }

    private static void SetSumLbl(Label lbl, string text, int x, int y)
    {
        lbl.AutoSize  = true;
        lbl.Font      = new Font("Segoe UI", 9F);
        lbl.ForeColor = Color.FromArgb(60, 60, 60);
        lbl.Location  = new Point(x, y);
        lbl.Text      = text;
    }

    private static void SetSumVal(Label lbl, string text, int x, int y, int w)
    {
        lbl.AutoSize  = false;
        lbl.Font      = new Font("Segoe UI", 9F);
        lbl.ForeColor = Color.FromArgb(30, 30, 30);
        lbl.Location  = new Point(x, y);
        lbl.Size      = new Size(w, 18);
        lbl.TextAlign = ContentAlignment.MiddleRight;
        lbl.Text      = text;
    }

    private static void SetBtn(Button btn, string text, int x, int y, int w, int h, Color bg, Color fg)
    {
        btn.BackColor                 = bg;
        btn.FlatAppearance.BorderSize = 0;
        btn.FlatStyle                 = FlatStyle.Flat;
        btn.Font                      = new Font("Segoe UI", 9F, FontStyle.Bold);
        btn.ForeColor                 = fg;
        btn.Location                  = new Point(x, y);
        btn.Size                      = new Size(w, h);
        btn.Text                      = text;
        btn.UseVisualStyleBackColor   = false;
        btn.Cursor                    = Cursors.Hand;
    }
}
