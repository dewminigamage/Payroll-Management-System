using System.Drawing;
using System.Windows.Forms;

namespace PayrollManagementSystem.Forms;

partial class frmOvertime
{
    private System.ComponentModel.IContainer components = null;

    // Header
    private Panel  pnlHeader;
    private Label  lblHeaderTitle;
    private Label  lblHeaderSub;

    // Left – entry form
    private Panel       pnlLeft;
    private Label       lblOTID;
    private TextBox     txtOTID;
    private Label       lblEmployee;
    private ComboBox    cmbEmployee;
    private Label       lblPayMonth;
    private ComboBox    cmbPayMonth;
    private Label       lblPayYear;
    private NumericUpDown nudPayYear;
    private Label       lblOTHours;
    private NumericUpDown nudOTHours;
    private Label       lblWorkHoursRef;
    private Label       lblOTMultiplier;
    private NumericUpDown nudOTMultiplier;
    private Label       lblNotes;
    private TextBox     txtNotes;
    private Label       lblHourlyRate;
    private Label       lblOTAmount;
    private Button      btnSave;
    private Button      btnUpdate;
    private Button      btnDelete;
    private Button      btnClear;

    // Right – filter + grid
    private Panel       pnlRight;
    private Label       lblFilterEmpL;
    private ComboBox    cmbFilterEmp;
    private Label       lblFilterMonthL;
    private ComboBox    cmbFilterMonth;
    private Label       lblFilterYearL;
    private NumericUpDown nudFilterYear;
    private Button      btnFilter;
    private Button      btnClearFilter;
    private DataGridView dgvOT;
    private Label       lblStatus;

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
        var sumBg   = Color.FromArgb(240, 252, 240);

        // ── Instantiate ────────────────────────────────────────
        pnlHeader       = new Panel();
        lblHeaderTitle  = new Label();
        lblHeaderSub    = new Label();
        pnlLeft         = new Panel();
        lblOTID         = new Label();
        txtOTID         = new TextBox();
        lblEmployee     = new Label();
        cmbEmployee     = new ComboBox();
        lblPayMonth     = new Label();
        cmbPayMonth     = new ComboBox();
        lblPayYear      = new Label();
        nudPayYear      = new NumericUpDown();
        lblOTHours      = new Label();
        nudOTHours      = new NumericUpDown();
        lblWorkHoursRef = new Label();
        lblOTMultiplier = new Label();
        nudOTMultiplier = new NumericUpDown();
        lblNotes        = new Label();
        txtNotes        = new TextBox();
        lblHourlyRate   = new Label();
        lblOTAmount     = new Label();
        btnSave         = new Button();
        btnUpdate       = new Button();
        btnDelete       = new Button();
        btnClear        = new Button();
        pnlRight        = new Panel();
        lblFilterEmpL   = new Label();
        cmbFilterEmp    = new ComboBox();
        lblFilterMonthL = new Label();
        cmbFilterMonth  = new ComboBox();
        lblFilterYearL  = new Label();
        nudFilterYear   = new NumericUpDown();
        btnFilter       = new Button();
        btnClearFilter  = new Button();
        dgvOT           = new DataGridView();
        lblStatus       = new Label();
        lblFooter       = new Label();

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
        lblHeaderTitle.Text      = "Overtime Management";

        lblHeaderSub.AutoSize  = true;
        lblHeaderSub.Font      = new Font("Segoe UI", 9.5F);
        lblHeaderSub.ForeColor = Color.FromArgb(200, 230, 255);
        lblHeaderSub.Location  = new Point(27, 48);
        lblHeaderSub.Text      = "Record and manage employee overtime hours per pay period";

        // ── Left panel ─────────────────────────────────────────
        pnlLeft.BackColor   = Color.White;
        pnlLeft.BorderStyle = BorderStyle.FixedSingle;
        pnlLeft.Location    = new Point(12, 90);
        pnlLeft.Size        = new Size(392, 480);

        const int lx = 10, ix = 145, iw = 232, rh = 26, gap = 6;
        int ly = 15;

        // OT ID (readonly)
        SetLbl(lblOTID, "OT ID", lx, ly + 3);
        txtOTID.Location  = new Point(ix, ly);
        txtOTID.Size      = new Size(80, rh);
        txtOTID.ReadOnly  = true;
        txtOTID.BackColor = inputBg;
        txtOTID.TabStop   = false;

        // Employee
        ly += rh + gap;
        SetLbl(lblEmployee, "Employee *", lx, ly + 3);
        cmbEmployee.Location      = new Point(ix, ly);
        cmbEmployee.Size          = new Size(iw, rh);
        cmbEmployee.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbEmployee.SelectedIndexChanged += cmbEmployee_SelectedIndexChanged;

        // Month + Year
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

        // OT Hours
        ly += rh + gap;
        SetLbl(lblOTHours, "OT Hours *", lx, ly + 3);
        nudOTHours.Location      = new Point(ix, ly);
        nudOTHours.Size          = new Size(90, rh);
        nudOTHours.Minimum       = 0;
        nudOTHours.Maximum       = 999;
        nudOTHours.DecimalPlaces = 2;
        nudOTHours.Increment     = 0.5m;
        nudOTHours.ValueChanged += nudOTHours_ValueChanged;

        lblWorkHoursRef.AutoSize  = true;
        lblWorkHoursRef.Font      = new Font("Segoe UI", 7.5F);
        lblWorkHoursRef.ForeColor = Color.FromArgb(120, 120, 120);
        lblWorkHoursRef.Location  = new Point(ix + 96, ly + 5);
        lblWorkHoursRef.Text      = "";

        // Rate Multiplier
        ly += rh + gap;
        SetLbl(lblOTMultiplier, "Rate Multiplier *", lx, ly + 3);
        nudOTMultiplier.Location      = new Point(ix, ly);
        nudOTMultiplier.Size          = new Size(90, rh);
        nudOTMultiplier.Minimum       = 0.5m;
        nudOTMultiplier.Maximum       = 10m;
        nudOTMultiplier.DecimalPlaces = 2;
        nudOTMultiplier.Increment     = 0.5m;
        nudOTMultiplier.Value         = 1.5m;
        nudOTMultiplier.ValueChanged += nudOTMultiplier_ValueChanged;

        // Notes
        ly += rh + gap;
        SetLbl(lblNotes, "Notes", lx, ly + 3);
        txtNotes.Location = new Point(ix, ly);
        txtNotes.Size     = new Size(iw, rh);

        // ── Calculation summary ────────────────────────────────
        ly += rh + 16;
        var pnlCalc = new Panel
        {
            BackColor   = sumBg,
            BorderStyle = BorderStyle.FixedSingle,
            Location    = new Point(10, ly),
            Size        = new Size(370, 58)
        };
        lblHourlyRate.AutoSize  = true;
        lblHourlyRate.Font      = new Font("Segoe UI", 9F);
        lblHourlyRate.ForeColor = Color.FromArgb(60, 60, 60);
        lblHourlyRate.Location  = new Point(10, 8);
        lblHourlyRate.Text      = "Hourly rate: —";

        lblOTAmount.AutoSize  = true;
        lblOTAmount.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblOTAmount.ForeColor = Color.FromArgb(0, 100, 0);
        lblOTAmount.Location  = new Point(10, 30);
        lblOTAmount.Text      = "OT Amount: LKR 0.00";

        pnlCalc.Controls.AddRange(new Control[] { lblHourlyRate, lblOTAmount });

        // ── Action buttons ─────────────────────────────────────
        ly += 68;
        SetBtn(btnSave,   "Save",   10,  ly, 80, 32, Color.FromArgb(0, 150, 80),   Color.White);
        SetBtn(btnUpdate, "Update", 98,  ly, 72, 32, Color.FromArgb(210, 130, 0),   Color.White);
        SetBtn(btnDelete, "Delete", 178, ly, 72, 32, Color.FromArgb(196, 43, 28),   Color.White);
        SetBtn(btnClear,  "Clear",  258, ly, 72, 32, Color.FromArgb(100, 100, 100), Color.White);
        btnSave.Click   += btnSave_Click;
        btnUpdate.Click += btnUpdate_Click;
        btnDelete.Click += btnDelete_Click;
        btnClear.Click  += btnClear_Click;

        pnlLeft.Controls.AddRange(new Control[] {
            lblOTID, txtOTID,
            lblEmployee, cmbEmployee,
            lblPayMonth, cmbPayMonth, lblPayYear, nudPayYear,
            lblOTHours, nudOTHours, lblWorkHoursRef,
            lblOTMultiplier, nudOTMultiplier,
            lblNotes, txtNotes,
            pnlCalc,
            btnSave, btnUpdate, btnDelete, btnClear
        });

        // ── Right panel ─────────────────────────────────────────
        pnlRight.BackColor   = Color.White;
        pnlRight.BorderStyle = BorderStyle.FixedSingle;
        pnlRight.Location    = new Point(416, 90);
        pnlRight.Size        = new Size(634, 480);

        const int fy = 10;
        SetLbl(lblFilterEmpL, "Employee:", 10, fy + 3);
        cmbFilterEmp.Location      = new Point(78, fy);
        cmbFilterEmp.Size          = new Size(155, 26);
        cmbFilterEmp.DropDownStyle = ComboBoxStyle.DropDownList;

        SetLbl(lblFilterMonthL, "Month:", 243, fy + 3);
        cmbFilterMonth.Location      = new Point(293, fy);
        cmbFilterMonth.Size          = new Size(110, 26);
        cmbFilterMonth.DropDownStyle = ComboBoxStyle.DropDownList;

        SetLbl(lblFilterYearL, "Year:", 413, fy + 3);
        nudFilterYear.Location      = new Point(452, fy);
        nudFilterYear.Size          = new Size(68, 26);
        nudFilterYear.Minimum       = 2000;
        nudFilterYear.Maximum       = 2099;
        nudFilterYear.Value         = DateTime.Today.Year;
        nudFilterYear.DecimalPlaces = 0;

        SetBtn(btnFilter,      "Filter", 528, fy,      90, 26, blue,          Color.White);
        SetBtn(btnClearFilter, "Reset",  528, fy + 30, 90, 26, Color.DimGray, Color.White);
        btnFilter.Click      += btnFilter_Click;
        btnClearFilter.Click += btnClearFilter_Click;

        dgvOT.Location                                = new Point(10, 48);
        dgvOT.Size                                    = new Size(610, 400);
        dgvOT.ReadOnly                                = true;
        dgvOT.SelectionMode                           = DataGridViewSelectionMode.FullRowSelect;
        dgvOT.MultiSelect                             = false;
        dgvOT.AllowUserToAddRows                      = false;
        dgvOT.AllowUserToDeleteRows                   = false;
        dgvOT.BackgroundColor                         = Color.White;
        dgvOT.BorderStyle                             = BorderStyle.None;
        dgvOT.RowHeadersVisible                       = false;
        dgvOT.Font                                    = new Font("Segoe UI", 9F);
        dgvOT.ColumnHeadersDefaultCellStyle.BackColor = blue;
        dgvOT.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvOT.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        dgvOT.EnableHeadersVisualStyles               = false;
        dgvOT.AutoSizeColumnsMode                     = DataGridViewAutoSizeColumnsMode.Fill;
        dgvOT.CellClick                              += dgvOT_CellClick;

        lblStatus.AutoSize  = true;
        lblStatus.Font      = new Font("Segoe UI", 9F);
        lblStatus.ForeColor = Color.FromArgb(80, 80, 80);
        lblStatus.Location  = new Point(10, 457);
        lblStatus.Text      = "Records: 0";

        pnlRight.Controls.AddRange(new Control[] {
            lblFilterEmpL, cmbFilterEmp,
            lblFilterMonthL, cmbFilterMonth,
            lblFilterYearL, nudFilterYear,
            btnFilter, btnClearFilter,
            dgvOT, lblStatus
        });

        // ── Footer ─────────────────────────────────────────────
        lblFooter.AutoSize  = false;
        lblFooter.Font      = new Font("Segoe UI", 8F);
        lblFooter.ForeColor = Color.FromArgb(150, 150, 150);
        lblFooter.Location  = new Point(0, 578);
        lblFooter.Size      = new Size(1060, 20);
        lblFooter.Text      = "   Payroll Management System  |  2025";
        lblFooter.TextAlign = ContentAlignment.MiddleLeft;

        // ── Form ───────────────────────────────────────────────
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode       = AutoScaleMode.Font;
        BackColor           = lightBg;
        ClientSize          = new Size(1060, 600);
        Controls.Add(lblFooter);
        Controls.Add(pnlRight);
        Controls.Add(pnlLeft);
        Controls.Add(pnlHeader);
        Font            = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        Name            = "frmOvertime";
        StartPosition   = FormStartPosition.CenterScreen;
        Text            = "Overtime Management";
        Load           += frmOvertime_Load;

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
