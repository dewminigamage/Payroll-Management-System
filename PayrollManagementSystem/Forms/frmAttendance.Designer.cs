using System.Drawing;
using System.Windows.Forms;

namespace PayrollManagementSystem.Forms;

partial class frmAttendance
{
    private System.ComponentModel.IContainer components = null;

    private System.Windows.Forms.Panel pnlHeader;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.GroupBox grpMarkAttendance;
    private System.Windows.Forms.Label lblAttendanceIDLbl;
    private System.Windows.Forms.TextBox txtAttendanceID;
    private System.Windows.Forms.Label lblDate;
    private System.Windows.Forms.DateTimePicker dtpDate;
    private System.Windows.Forms.Label lblEmployee;
    private System.Windows.Forms.ComboBox cmbEmployee;
    private System.Windows.Forms.Label lblStatusLbl;
    private System.Windows.Forms.ComboBox cmbStatus;
    private System.Windows.Forms.Label lblRemarks;
    private System.Windows.Forms.TextBox txtRemarks;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnUpdate;
    private System.Windows.Forms.Button btnDelete;
    private System.Windows.Forms.Button btnClear;
    private System.Windows.Forms.GroupBox grpFilter;
    private System.Windows.Forms.Label lblFilterEmployee;
    private System.Windows.Forms.ComboBox cmbFilterEmployee;
    private System.Windows.Forms.Label lblFrom;
    private System.Windows.Forms.DateTimePicker dtpFilterFrom;
    private System.Windows.Forms.Label lblTo;
    private System.Windows.Forms.DateTimePicker dtpFilterTo;
    private System.Windows.Forms.Button btnFilter;
    private System.Windows.Forms.Button btnClearFilter;
    private System.Windows.Forms.DataGridView dgvAttendance;
    private System.Windows.Forms.Label lblStatus;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var primaryBlue = Color.FromArgb(0, 120, 212);
        var darkText    = Color.FromArgb(50, 50, 50);
        var ui9         = new Font("Segoe UI", 9F);
        var ui9b        = new Font("Segoe UI", 9F, FontStyle.Bold);

        pnlHeader         = new Panel();
        lblTitle          = new Label();
        grpMarkAttendance = new GroupBox();
        lblAttendanceIDLbl= new Label();
        txtAttendanceID   = new TextBox();
        lblDate           = new Label();
        dtpDate           = new DateTimePicker();
        lblEmployee       = new Label();
        cmbEmployee       = new ComboBox();
        lblStatusLbl      = new Label();
        cmbStatus         = new ComboBox();
        lblRemarks        = new Label();
        txtRemarks        = new TextBox();
        btnSave           = new Button();
        btnUpdate         = new Button();
        btnDelete         = new Button();
        btnClear          = new Button();
        grpFilter         = new GroupBox();
        lblFilterEmployee = new Label();
        cmbFilterEmployee = new ComboBox();
        lblFrom           = new Label();
        dtpFilterFrom     = new DateTimePicker();
        lblTo             = new Label();
        dtpFilterTo       = new DateTimePicker();
        btnFilter         = new Button();
        btnClearFilter    = new Button();
        dgvAttendance     = new DataGridView();
        lblStatus         = new Label();

        SuspendLayout();
        pnlHeader.SuspendLayout();
        grpMarkAttendance.SuspendLayout();
        grpFilter.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvAttendance).BeginInit();

        // ── Header ────────────────────────────────────────────────────────────
        pnlHeader.BackColor = primaryBlue;
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Size = new Size(980, 55);

        lblTitle.AutoSize  = true;
        lblTitle.Font      = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location  = new Point(20, 12);
        lblTitle.Text      = "Attendance Management";

        // ── GroupBox: Mark Attendance ─────────────────────────────────────────
        grpMarkAttendance.Font      = ui9;
        grpMarkAttendance.ForeColor = Color.FromArgb(0, 90, 160);
        grpMarkAttendance.Location  = new Point(10, 65);
        grpMarkAttendance.Size      = new Size(365, 340);
        grpMarkAttendance.Text      = "Mark Attendance";
        grpMarkAttendance.Controls.AddRange(new Control[] {
            lblAttendanceIDLbl, txtAttendanceID,
            lblDate,   dtpDate,
            lblEmployee, cmbEmployee,
            lblStatusLbl, cmbStatus,
            lblRemarks, txtRemarks,
            btnSave, btnUpdate, btnDelete, btnClear
        });

        const int LX = 12;
        const int TX = 130;
        const int TW = 210;
        const int RH = 35;
        int ry = 25;

        // Attendance ID (read-only)
        SetLabel(lblAttendanceIDLbl, "Attendance ID:", LX, ry + 4, darkText, ui9);
        txtAttendanceID.Font      = ui9;
        txtAttendanceID.Location  = new Point(TX, ry);
        txtAttendanceID.Size      = new Size(80, 23);
        txtAttendanceID.ReadOnly  = true;
        txtAttendanceID.BackColor = SystemColors.Control;
        txtAttendanceID.TabStop   = false;
        ry += RH;

        // Date
        SetLabel(lblDate, "Date: *", LX, ry + 4, darkText, ui9);
        dtpDate.Font     = ui9;
        dtpDate.Format   = DateTimePickerFormat.Short;
        dtpDate.Location = new Point(TX, ry);
        dtpDate.Size     = new Size(TW, 23);
        ry += RH;

        // Employee
        SetLabel(lblEmployee, "Employee: *", LX, ry + 4, darkText, ui9);
        cmbEmployee.DropDownStyle  = ComboBoxStyle.DropDownList;
        cmbEmployee.Font           = ui9;
        cmbEmployee.Location       = new Point(TX, ry);
        cmbEmployee.Size           = new Size(TW, 23);
        ry += RH;

        // Status
        SetLabel(lblStatusLbl, "Status: *", LX, ry + 4, darkText, ui9);
        cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbStatus.Font          = ui9;
        cmbStatus.Location      = new Point(TX, ry);
        cmbStatus.Size          = new Size(TW, 23);
        cmbStatus.Items.AddRange(new object[] { "Present", "Absent", "Late", "Half Day", "Leave" });
        cmbStatus.SelectedIndex = 0;
        ry += RH;

        // Remarks (multiline)
        SetLabel(lblRemarks, "Remarks:", LX, ry + 4, darkText, ui9);
        txtRemarks.Font      = ui9;
        txtRemarks.Location  = new Point(TX, ry);
        txtRemarks.Size      = new Size(TW, 55);
        txtRemarks.Multiline = true;
        txtRemarks.ScrollBars = ScrollBars.Vertical;
        ry += 70;

        // Buttons
        StyleButton(btnSave,   "Save",   primaryBlue,                  new Point(12,  ry), new Size(90, 35), ui9b, true);
        StyleButton(btnUpdate, "Update", Color.FromArgb(255, 140, 0),  new Point(118, ry), new Size(90, 35), ui9b, true);
        StyleButton(btnDelete, "Delete", Color.FromArgb(196, 43, 28),  new Point(224, ry), new Size(90, 35), ui9b, true);
        btnSave.Click   += new System.EventHandler(btnSave_Click);
        btnUpdate.Click += new System.EventHandler(btnUpdate_Click);
        btnDelete.Click += new System.EventHandler(btnDelete_Click);

        StyleButton(btnClear, "Clear", SystemColors.Control, new Point(118, ry + 45), new Size(90, 30), ui9, false);
        btnClear.ForeColor = darkText;
        btnClear.Click    += new System.EventHandler(btnClear_Click);

        // ── GroupBox: Filter Records ──────────────────────────────────────────
        grpFilter.Font      = ui9;
        grpFilter.ForeColor = Color.FromArgb(0, 90, 160);
        grpFilter.Location  = new Point(390, 65);
        grpFilter.Size      = new Size(575, 92);
        grpFilter.Text      = "Filter Records";
        grpFilter.Controls.AddRange(new Control[] {
            lblFilterEmployee, cmbFilterEmployee,
            lblFrom, dtpFilterFrom,
            lblTo, dtpFilterTo,
            btnFilter, btnClearFilter
        });

        // Row 1 — Employee filter
        SetLabel(lblFilterEmployee, "Employee:", 10, 24, darkText, ui9);
        cmbFilterEmployee.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbFilterEmployee.Font          = ui9;
        cmbFilterEmployee.Location      = new Point(78, 21);
        cmbFilterEmployee.Size          = new Size(170, 23);

        // Row 2 — Date range + buttons
        SetLabel(lblFrom, "From:", 10, 57, darkText, ui9);
        dtpFilterFrom.Font     = ui9;
        dtpFilterFrom.Format   = DateTimePickerFormat.Short;
        dtpFilterFrom.Location = new Point(55, 53);
        dtpFilterFrom.Size     = new Size(115, 23);

        SetLabel(lblTo, "To:", 182, 57, darkText, ui9);
        dtpFilterTo.Font     = ui9;
        dtpFilterTo.Format   = DateTimePickerFormat.Short;
        dtpFilterTo.Location = new Point(210, 53);
        dtpFilterTo.Size     = new Size(115, 23);

        StyleButton(btnFilter,      "Filter",       primaryBlue,        new Point(338, 52), new Size(85, 27), ui9, true);
        StyleButton(btnClearFilter, "Clear Filter", SystemColors.Control,new Point(431, 52), new Size(95, 27), ui9, false);
        btnClearFilter.ForeColor = darkText;
        btnFilter.Click      += new System.EventHandler(btnFilter_Click);
        btnClearFilter.Click += new System.EventHandler(btnClearFilter_Click);

        // ── DataGridView ──────────────────────────────────────────────────────
        var headerStyle = dgvAttendance.ColumnHeadersDefaultCellStyle;
        headerStyle.BackColor          = primaryBlue;
        headerStyle.ForeColor          = Color.White;
        headerStyle.Font               = ui9b;
        headerStyle.SelectionBackColor = primaryBlue;
        dgvAttendance.ColumnHeadersDefaultCellStyle = headerStyle;

        dgvAttendance.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);
        dgvAttendance.AllowUserToAddRows          = false;
        dgvAttendance.AllowUserToDeleteRows       = false;
        dgvAttendance.BackgroundColor             = Color.White;
        dgvAttendance.BorderStyle                 = BorderStyle.FixedSingle;
        dgvAttendance.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvAttendance.EnableHeadersVisualStyles   = false;
        dgvAttendance.Font                        = ui9;
        dgvAttendance.GridColor                   = Color.FromArgb(220, 220, 220);
        dgvAttendance.Location                    = new Point(390, 165);
        dgvAttendance.MultiSelect                 = false;
        dgvAttendance.ReadOnly                    = true;
        dgvAttendance.RowHeadersVisible           = false;
        dgvAttendance.SelectionMode               = DataGridViewSelectionMode.FullRowSelect;
        dgvAttendance.Size                        = new Size(575, 470);
        dgvAttendance.CellClick += new DataGridViewCellEventHandler(dgvAttendance_CellClick);

        // ── Status bar ────────────────────────────────────────────────────────
        lblStatus.AutoSize  = false;
        lblStatus.Font      = new Font("Segoe UI", 8.5F);
        lblStatus.ForeColor = Color.FromArgb(100, 100, 100);
        lblStatus.Location  = new Point(10, 648);
        lblStatus.Size      = new Size(960, 18);
        lblStatus.Text      = "Ready";

        // ── Form ──────────────────────────────────────────────────────────────
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode       = AutoScaleMode.Font;
        BackColor           = Color.White;
        ClientSize          = new Size(980, 670);
        Controls.Add(lblStatus);
        Controls.Add(dgvAttendance);
        Controls.Add(grpFilter);
        Controls.Add(grpMarkAttendance);
        Controls.Add(pnlHeader);
        Font            = ui9;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        Name            = "frmAttendance";
        StartPosition   = FormStartPosition.CenterScreen;
        Text            = "Attendance Management";
        Load           += new System.EventHandler(frmAttendance_Load);

        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        grpMarkAttendance.ResumeLayout(false);
        grpMarkAttendance.PerformLayout();
        grpFilter.ResumeLayout(false);
        grpFilter.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvAttendance).EndInit();
        ResumeLayout(false);
    }

    private static void SetLabel(Label lbl, string text, int x, int y, Color fore, Font font)
    {
        lbl.AutoSize  = true;
        lbl.Font      = font;
        lbl.ForeColor = fore;
        lbl.Location  = new Point(x, y);
        lbl.Text      = text;
    }

    private static void StyleButton(Button btn, string text, Color back, Point loc, Size size, Font font, bool whiteText)
    {
        btn.BackColor                   = back;
        btn.FlatAppearance.BorderSize   = 1;
        btn.FlatAppearance.BorderColor  = Color.FromArgb(180, 180, 180);
        btn.FlatStyle                   = FlatStyle.Flat;
        btn.Font                        = font;
        btn.ForeColor                   = whiteText ? Color.White : SystemColors.ControlText;
        btn.Location                    = loc;
        btn.Size                        = size;
        btn.Text                        = text;
        btn.UseVisualStyleBackColor     = false;
        btn.Cursor                      = Cursors.Hand;
    }
}
