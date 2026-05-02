using System.Drawing;
using System.Windows.Forms;

namespace PayrollManagementSystem.Forms;

partial class frmLoanManagement
{
    private System.ComponentModel.IContainer components = null;

    // Header
    private Panel  pnlHeader;
    private Label  lblHeaderTitle;
    private Label  lblHeaderSub;

    // Left – entry form
    private Panel        pnlLeft;
    private Label        lblLoanID;
    private TextBox      txtLoanID;
    private Label        lblEmployee;
    private ComboBox     cmbEmployee;
    private Label        lblLoanType;
    private ComboBox     cmbLoanType;
    private Label        lblAmount;
    private NumericUpDown nudAmount;
    private Label        lblInstallment;
    private NumericUpDown nudInstallment;
    private Label        lblStartMonth;
    private ComboBox     cmbStartMonth;
    private Label        lblStartYear;
    private NumericUpDown nudStartYear;
    private Label        lblNotes;
    private TextBox      txtNotes;
    private Panel        pnlSummary;
    private Label        lblSumRemaining;
    private Label        lblSumStatus;
    private Label        lblSumMonths;
    private Button       btnSave;
    private Button       btnUpdate;
    private Button       btnDelete;
    private Button       btnClear;
    private Button       btnRecordPayment;

    // Right – filter + grid
    private Panel        pnlRight;
    private Label        lblFilterEmpL;
    private ComboBox     cmbFilterEmp;
    private Label        lblFilterStatusL;
    private ComboBox     cmbFilterStatus;
    private Button       btnFilter;
    private Button       btnClearFilter;
    private DataGridView dgvLoans;
    private Label        lblStatus;

    private Label  lblFooter;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var blue    = Color.FromArgb(0, 120, 212);
        var lightBg = Color.FromArgb(245, 247, 250);
        var inputBg = Color.FromArgb(240, 242, 245);
        var sumBg   = Color.FromArgb(255, 248, 235);

        pnlHeader        = new Panel();
        lblHeaderTitle   = new Label();
        lblHeaderSub     = new Label();
        pnlLeft          = new Panel();
        lblLoanID        = new Label();
        txtLoanID        = new TextBox();
        lblEmployee      = new Label();
        cmbEmployee      = new ComboBox();
        lblLoanType      = new Label();
        cmbLoanType      = new ComboBox();
        lblAmount        = new Label();
        nudAmount        = new NumericUpDown();
        lblInstallment   = new Label();
        nudInstallment   = new NumericUpDown();
        lblStartMonth    = new Label();
        cmbStartMonth    = new ComboBox();
        lblStartYear     = new Label();
        nudStartYear     = new NumericUpDown();
        lblNotes         = new Label();
        txtNotes         = new TextBox();
        pnlSummary       = new Panel();
        lblSumRemaining  = new Label();
        lblSumStatus     = new Label();
        lblSumMonths     = new Label();
        btnSave          = new Button();
        btnUpdate        = new Button();
        btnDelete        = new Button();
        btnClear         = new Button();
        btnRecordPayment = new Button();
        pnlRight         = new Panel();
        lblFilterEmpL    = new Label();
        cmbFilterEmp     = new ComboBox();
        lblFilterStatusL = new Label();
        cmbFilterStatus  = new ComboBox();
        btnFilter        = new Button();
        btnClearFilter   = new Button();
        dgvLoans         = new DataGridView();
        lblStatus        = new Label();
        lblFooter        = new Label();

        SuspendLayout();
        pnlHeader.SuspendLayout();
        pnlLeft.SuspendLayout();
        pnlRight.SuspendLayout();

        // ── Header ─────────────────────────────────────────────
        pnlHeader.BackColor = blue;
        pnlHeader.Controls.AddRange(new Control[] { lblHeaderSub, lblHeaderTitle });
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Size = new Size(1060, 80);

        lblHeaderTitle.AutoSize  = true;
        lblHeaderTitle.Font      = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblHeaderTitle.ForeColor = Color.White;
        lblHeaderTitle.Location  = new Point(25, 12);
        lblHeaderTitle.Text      = "Loan & Advance Management";

        lblHeaderSub.AutoSize  = true;
        lblHeaderSub.Font      = new Font("Segoe UI", 9.5F);
        lblHeaderSub.ForeColor = Color.FromArgb(200, 230, 255);
        lblHeaderSub.Location  = new Point(27, 48);
        lblHeaderSub.Text      = "Track employee loans and salary advances with repayment schedules";

        // ── Left panel ─────────────────────────────────────────
        pnlLeft.BackColor   = Color.White;
        pnlLeft.BorderStyle = BorderStyle.FixedSingle;
        pnlLeft.Location    = new Point(12, 90);
        pnlLeft.Size        = new Size(392, 490);

        const int lx = 10, ix = 145, iw = 232, rh = 26, gap = 6;
        int ly = 15;

        // Loan ID
        SetLbl(lblLoanID, "Loan ID", lx, ly + 3);
        txtLoanID.Location  = new Point(ix, ly);
        txtLoanID.Size      = new Size(80, rh);
        txtLoanID.ReadOnly  = true;
        txtLoanID.BackColor = inputBg;
        txtLoanID.TabStop   = false;

        // Employee
        ly += rh + gap;
        SetLbl(lblEmployee, "Employee *", lx, ly + 3);
        cmbEmployee.Location      = new Point(ix, ly);
        cmbEmployee.Size          = new Size(iw, rh);
        cmbEmployee.DropDownStyle = ComboBoxStyle.DropDownList;

        // Loan Type
        ly += rh + gap;
        SetLbl(lblLoanType, "Loan Type *", lx, ly + 3);
        cmbLoanType.Location      = new Point(ix, ly);
        cmbLoanType.Size          = new Size(iw, rh);
        cmbLoanType.DropDownStyle = ComboBoxStyle.DropDownList;

        // Loan Amount
        ly += rh + gap;
        SetLbl(lblAmount, "Loan Amount *", lx, ly + 3);
        nudAmount.Location      = new Point(ix, ly);
        nudAmount.Size          = new Size(iw, rh);
        nudAmount.Minimum       = 0;
        nudAmount.Maximum       = 99999999;
        nudAmount.DecimalPlaces = 2;
        nudAmount.Increment     = 1000m;
        nudAmount.ValueChanged += nudAmount_ValueChanged;

        // Monthly Installment
        ly += rh + gap;
        SetLbl(lblInstallment, "Monthly Installment *", lx, ly + 3);
        nudInstallment.Location      = new Point(ix, ly);
        nudInstallment.Size          = new Size(iw, rh);
        nudInstallment.Minimum       = 0;
        nudInstallment.Maximum       = 99999999;
        nudInstallment.DecimalPlaces = 2;
        nudInstallment.Increment     = 500m;
        nudInstallment.ValueChanged += nudInstallment_ValueChanged;

        // Start Month + Year
        ly += rh + gap;
        SetLbl(lblStartMonth, "Start Month *", lx, ly + 3);
        cmbStartMonth.Location      = new Point(ix, ly);
        cmbStartMonth.Size          = new Size(115, rh);
        cmbStartMonth.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbStartMonth.Items.AddRange(new object[] {
            "January","February","March","April","May","June",
            "July","August","September","October","November","December" });
        cmbStartMonth.SelectedIndex = DateTime.Today.Month - 1;

        SetLbl(lblStartYear, "Year", ix + 123, ly + 3);
        nudStartYear.Location      = new Point(ix + 158, ly);
        nudStartYear.Size          = new Size(74, rh);
        nudStartYear.Minimum       = 2000;
        nudStartYear.Maximum       = 2099;
        nudStartYear.Value         = DateTime.Today.Year;
        nudStartYear.DecimalPlaces = 0;

        // Notes
        ly += rh + gap;
        SetLbl(lblNotes, "Notes", lx, ly + 3);
        txtNotes.Location = new Point(ix, ly);
        txtNotes.Size     = new Size(iw, rh);

        // ── Summary panel ──────────────────────────────────────
        ly += rh + 14;
        pnlSummary.BackColor   = sumBg;
        pnlSummary.BorderStyle = BorderStyle.FixedSingle;
        pnlSummary.Location    = new Point(10, ly);
        pnlSummary.Size        = new Size(370, 72);

        lblSumRemaining.AutoSize  = true;
        lblSumRemaining.Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblSumRemaining.ForeColor = Color.FromArgb(150, 70, 0);
        lblSumRemaining.Location  = new Point(10, 8);
        lblSumRemaining.Text      = "Remaining Balance: —";

        lblSumMonths.AutoSize  = true;
        lblSumMonths.Font      = new Font("Segoe UI", 8.5F);
        lblSumMonths.ForeColor = Color.FromArgb(80, 80, 80);
        lblSumMonths.Location  = new Point(10, 30);
        lblSumMonths.Text      = "Estimated months: —";

        lblSumStatus.AutoSize  = true;
        lblSumStatus.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblSumStatus.ForeColor = Color.FromArgb(0, 130, 70);
        lblSumStatus.Location  = new Point(10, 50);
        lblSumStatus.Text      = "";

        pnlSummary.Controls.AddRange(new Control[] { lblSumRemaining, lblSumMonths, lblSumStatus });

        // ── Action buttons ─────────────────────────────────────
        ly += 82;
        SetBtn(btnSave,   "Save",   10,  ly, 80, 32, Color.FromArgb(0, 150, 80),   Color.White);
        SetBtn(btnUpdate, "Update", 98,  ly, 72, 32, Color.FromArgb(210, 130, 0),  Color.White);
        SetBtn(btnDelete, "Delete", 178, ly, 72, 32, Color.FromArgb(196, 43, 28),  Color.White);
        SetBtn(btnClear,  "Clear",  258, ly, 72, 32, Color.FromArgb(100, 100, 100),Color.White);
        btnSave.Click   += btnSave_Click;
        btnUpdate.Click += btnUpdate_Click;
        btnDelete.Click += btnDelete_Click;
        btnClear.Click  += btnClear_Click;

        ly += 40;
        SetBtn(btnRecordPayment, "Record Payment (this month)", 10, ly, 220, 30,
            Color.FromArgb(150, 70, 0), Color.White);
        btnRecordPayment.Enabled = false;
        btnRecordPayment.Click  += btnRecordPayment_Click;

        pnlLeft.Controls.AddRange(new Control[] {
            lblLoanID, txtLoanID,
            lblEmployee, cmbEmployee,
            lblLoanType, cmbLoanType,
            lblAmount, nudAmount,
            lblInstallment, nudInstallment,
            lblStartMonth, cmbStartMonth, lblStartYear, nudStartYear,
            lblNotes, txtNotes,
            pnlSummary,
            btnSave, btnUpdate, btnDelete, btnClear,
            btnRecordPayment
        });

        // ── Right panel ─────────────────────────────────────────
        pnlRight.BackColor   = Color.White;
        pnlRight.BorderStyle = BorderStyle.FixedSingle;
        pnlRight.Location    = new Point(416, 90);
        pnlRight.Size        = new Size(634, 490);

        const int fy = 10;
        SetLbl(lblFilterEmpL, "Employee:", 10, fy + 3);
        cmbFilterEmp.Location      = new Point(78, fy);
        cmbFilterEmp.Size          = new Size(200, 26);
        cmbFilterEmp.DropDownStyle = ComboBoxStyle.DropDownList;

        SetLbl(lblFilterStatusL, "Status:", 290, fy + 3);
        cmbFilterStatus.Location      = new Point(335, fy);
        cmbFilterStatus.Size          = new Size(110, 26);
        cmbFilterStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbFilterStatus.Items.AddRange(new object[] { "All", "Active", "Completed", "Cancelled" });
        cmbFilterStatus.SelectedIndex = 1;

        SetBtn(btnFilter,      "Filter", 456, fy,      90, 26, blue,          Color.White);
        SetBtn(btnClearFilter, "Reset",  456, fy + 30, 90, 26, Color.DimGray, Color.White);
        btnFilter.Click      += btnFilter_Click;
        btnClearFilter.Click += btnClearFilter_Click;

        dgvLoans.Location                                = new Point(10, 48);
        dgvLoans.Size                                    = new Size(610, 410);
        dgvLoans.ReadOnly                                = true;
        dgvLoans.SelectionMode                           = DataGridViewSelectionMode.FullRowSelect;
        dgvLoans.MultiSelect                             = false;
        dgvLoans.AllowUserToAddRows                      = false;
        dgvLoans.AllowUserToDeleteRows                   = false;
        dgvLoans.BackgroundColor                         = Color.White;
        dgvLoans.BorderStyle                             = BorderStyle.None;
        dgvLoans.RowHeadersVisible                       = false;
        dgvLoans.Font                                    = new Font("Segoe UI", 9F);
        dgvLoans.ColumnHeadersDefaultCellStyle.BackColor = blue;
        dgvLoans.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvLoans.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        dgvLoans.EnableHeadersVisualStyles               = false;
        dgvLoans.AutoSizeColumnsMode                     = DataGridViewAutoSizeColumnsMode.Fill;
        dgvLoans.CellClick                              += dgvLoans_CellClick;

        lblStatus.AutoSize  = true;
        lblStatus.Font      = new Font("Segoe UI", 9F);
        lblStatus.ForeColor = Color.FromArgb(80, 80, 80);
        lblStatus.Location  = new Point(10, 465);
        lblStatus.Text      = "Records: 0";

        pnlRight.Controls.AddRange(new Control[] {
            lblFilterEmpL, cmbFilterEmp,
            lblFilterStatusL, cmbFilterStatus,
            btnFilter, btnClearFilter,
            dgvLoans, lblStatus
        });

        // ── Footer ─────────────────────────────────────────────
        lblFooter.AutoSize  = false;
        lblFooter.Font      = new Font("Segoe UI", 8F);
        lblFooter.ForeColor = Color.FromArgb(150, 150, 150);
        lblFooter.Location  = new Point(0, 598);
        lblFooter.Size      = new Size(1060, 20);
        lblFooter.Text      = "   Payroll Management System  |  2025";
        lblFooter.TextAlign = ContentAlignment.MiddleLeft;

        // ── Form ───────────────────────────────────────────────
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode       = AutoScaleMode.Font;
        BackColor           = lightBg;
        ClientSize          = new Size(1060, 620);
        Controls.Add(lblFooter);
        Controls.Add(pnlRight);
        Controls.Add(pnlLeft);
        Controls.Add(pnlHeader);
        Font            = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        Name            = "frmLoanManagement";
        StartPosition   = FormStartPosition.CenterScreen;
        Text            = "Loan & Advance Management";
        Load           += frmLoanManagement_Load;

        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        pnlLeft.ResumeLayout(false);
        pnlLeft.PerformLayout();
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
