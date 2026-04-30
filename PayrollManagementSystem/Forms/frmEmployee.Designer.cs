using System.Drawing;
using System.Windows.Forms;

namespace PayrollManagementSystem.Forms;

partial class frmEmployee
{
    private System.ComponentModel.IContainer components = null;

    private System.Windows.Forms.Panel pnlHeader;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.GroupBox grpEmployeeInfo;
    private System.Windows.Forms.Label lblEmployeeID;
    private System.Windows.Forms.TextBox txtEmployeeID;
    private System.Windows.Forms.Label lblFullName;
    private System.Windows.Forms.TextBox txtFullName;
    private System.Windows.Forms.Label lblNIC;
    private System.Windows.Forms.TextBox txtNIC;
    private System.Windows.Forms.Label lblDepartment;
    private System.Windows.Forms.TextBox txtDepartment;
    private System.Windows.Forms.Label lblPosition;
    private System.Windows.Forms.TextBox txtPosition;
    private System.Windows.Forms.Label lblBasicSalary;
    private System.Windows.Forms.TextBox txtBasicSalary;
    private System.Windows.Forms.Label lblJoinDate;
    private System.Windows.Forms.DateTimePicker dtpJoinDate;
    private System.Windows.Forms.Label lblContactNumber;
    private System.Windows.Forms.TextBox txtContactNumber;
    private System.Windows.Forms.Label lblEmail;
    private System.Windows.Forms.TextBox txtEmail;
    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.Button btnUpdate;
    private System.Windows.Forms.Button btnDelete;
    private System.Windows.Forms.Button btnClear;
    private System.Windows.Forms.Panel pnlSearch;
    private System.Windows.Forms.Label lblSearch;
    private System.Windows.Forms.TextBox txtSearch;
    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.Button btnShowAll;
    private System.Windows.Forms.DataGridView dgvEmployees;
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
        grpEmployeeInfo   = new GroupBox();
        lblEmployeeID     = new Label();
        txtEmployeeID     = new TextBox();
        lblFullName       = new Label();
        txtFullName       = new TextBox();
        lblNIC            = new Label();
        txtNIC            = new TextBox();
        lblDepartment     = new Label();
        txtDepartment     = new TextBox();
        lblPosition       = new Label();
        txtPosition       = new TextBox();
        lblBasicSalary    = new Label();
        txtBasicSalary    = new TextBox();
        lblJoinDate       = new Label();
        dtpJoinDate       = new DateTimePicker();
        lblContactNumber  = new Label();
        txtContactNumber  = new TextBox();
        lblEmail          = new Label();
        txtEmail          = new TextBox();
        btnAdd            = new Button();
        btnUpdate         = new Button();
        btnDelete         = new Button();
        btnClear          = new Button();
        pnlSearch         = new Panel();
        lblSearch         = new Label();
        txtSearch         = new TextBox();
        btnSearch         = new Button();
        btnShowAll        = new Button();
        dgvEmployees      = new DataGridView();
        lblStatus         = new Label();

        SuspendLayout();
        pnlHeader.SuspendLayout();
        grpEmployeeInfo.SuspendLayout();
        pnlSearch.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvEmployees).BeginInit();

        // ── Header ───────────────────────────────────────────────────────────
        pnlHeader.BackColor = primaryBlue;
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Size = new Size(980, 55);

        lblTitle.AutoSize = true;
        lblTitle.Font     = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(20, 12);
        lblTitle.Text     = "Employee Management";

        // ── GroupBox: Employee Information ───────────────────────────────────
        grpEmployeeInfo.Font      = ui9;
        grpEmployeeInfo.ForeColor = Color.FromArgb(0, 90, 160);
        grpEmployeeInfo.Location  = new Point(10, 65);
        grpEmployeeInfo.Size      = new Size(365, 490);
        grpEmployeeInfo.Text      = "Employee Information";
        grpEmployeeInfo.Controls.AddRange(new Control[] {
            lblEmployeeID,   txtEmployeeID,
            lblFullName,     txtFullName,
            lblNIC,          txtNIC,
            lblDepartment,   txtDepartment,
            lblPosition,     txtPosition,
            lblBasicSalary,  txtBasicSalary,
            lblJoinDate,     dtpJoinDate,
            lblContactNumber,txtContactNumber,
            lblEmail,        txtEmail,
            btnAdd, btnUpdate, btnDelete, btnClear
        });

        // Row helper constants
        const int LX = 12;    // label x
        const int TX = 130;   // textbox x
        const int TW = 210;   // textbox width
        const int RH = 35;    // row height step
        int ry = 25;          // current row y

        // Employee ID  (read-only reference)
        SetLabel(lblEmployeeID, "Employee ID:", LX, ry + 4, darkText, ui9);
        txtEmployeeID.Font       = ui9;
        txtEmployeeID.Location   = new Point(TX, ry);
        txtEmployeeID.Size       = new Size(80, 23);
        txtEmployeeID.ReadOnly   = true;
        txtEmployeeID.BackColor  = SystemColors.Control;
        txtEmployeeID.TabStop    = false;
        ry += RH;

        // Full Name
        SetLabel(lblFullName, "Full Name: *", LX, ry + 4, darkText, ui9);
        SetTextBox(txtFullName, TX, ry, TW, ui9); ry += RH;

        // NIC
        SetLabel(lblNIC, "NIC: *", LX, ry + 4, darkText, ui9);
        SetTextBox(txtNIC, TX, ry, TW, ui9); ry += RH;

        // Department
        SetLabel(lblDepartment, "Department:", LX, ry + 4, darkText, ui9);
        SetTextBox(txtDepartment, TX, ry, TW, ui9); ry += RH;

        // Position
        SetLabel(lblPosition, "Position:", LX, ry + 4, darkText, ui9);
        SetTextBox(txtPosition, TX, ry, TW, ui9); ry += RH;

        // Basic Salary
        SetLabel(lblBasicSalary, "Basic Salary: *", LX, ry + 4, darkText, ui9);
        SetTextBox(txtBasicSalary, TX, ry, TW, ui9); ry += RH;

        // Join Date
        SetLabel(lblJoinDate, "Join Date: *", LX, ry + 4, darkText, ui9);
        dtpJoinDate.Font     = ui9;
        dtpJoinDate.Format   = DateTimePickerFormat.Short;
        dtpJoinDate.Location = new Point(TX, ry);
        dtpJoinDate.Size     = new Size(TW, 23);
        ry += RH;

        // Contact Number
        SetLabel(lblContactNumber, "Contact Number:", LX, ry + 4, darkText, ui9);
        SetTextBox(txtContactNumber, TX, ry, TW, ui9); ry += RH;

        // Email
        SetLabel(lblEmail, "Email:", LX, ry + 4, darkText, ui9);
        SetTextBox(txtEmail, TX, ry, TW, ui9); ry += RH + 10;

        // ── Action buttons ────────────────────────────────────────────────────
        StyleButton(btnAdd,    "Add",    primaryBlue,                         new Point(12,  ry), new Size(90, 35), ui9b, true);
        StyleButton(btnUpdate, "Update", Color.FromArgb(255, 140, 0),        new Point(118, ry), new Size(90, 35), ui9b, true);
        StyleButton(btnDelete, "Delete", Color.FromArgb(196, 43, 28),        new Point(224, ry), new Size(90, 35), ui9b, true);
        btnAdd.Click    += new System.EventHandler(btnAdd_Click);
        btnUpdate.Click += new System.EventHandler(btnUpdate_Click);
        btnDelete.Click += new System.EventHandler(btnDelete_Click);

        StyleButton(btnClear, "Clear", SystemColors.Control, new Point(118, ry + 45), new Size(90, 30), ui9, false);
        btnClear.ForeColor = darkText;
        btnClear.Click += new System.EventHandler(btnClear_Click);

        // ── Search panel ──────────────────────────────────────────────────────
        pnlSearch.BackColor   = Color.FromArgb(245, 248, 252);
        pnlSearch.BorderStyle = BorderStyle.FixedSingle;
        pnlSearch.Location    = new Point(390, 65);
        pnlSearch.Size        = new Size(575, 50);
        pnlSearch.Controls.AddRange(new Control[] { lblSearch, txtSearch, btnSearch, btnShowAll });

        lblSearch.AutoSize  = true;
        lblSearch.Font      = ui9;
        lblSearch.Location  = new Point(10, 15);
        lblSearch.Text      = "Search:";

        txtSearch.Font     = ui9;
        txtSearch.Location = new Point(68, 12);
        txtSearch.Size     = new Size(295, 23);
        txtSearch.KeyPress += new KeyPressEventHandler(txtSearch_KeyPress);

        StyleButton(btnSearch,  "Search",   primaryBlue,        new Point(373, 10), new Size(85, 28), ui9, true);
        StyleButton(btnShowAll, "Show All", SystemColors.Control,new Point(466, 10), new Size(95, 28), ui9, false);
        btnShowAll.ForeColor = darkText;
        btnSearch.Click  += new System.EventHandler(btnSearch_Click);
        btnShowAll.Click += new System.EventHandler(btnShowAll_Click);

        // ── DataGridView ──────────────────────────────────────────────────────
        var headerStyle = dgvEmployees.ColumnHeadersDefaultCellStyle;
        headerStyle.BackColor          = primaryBlue;
        headerStyle.ForeColor          = Color.White;
        headerStyle.Font               = ui9b;
        headerStyle.SelectionBackColor = primaryBlue;
        dgvEmployees.ColumnHeadersDefaultCellStyle = headerStyle;

        dgvEmployees.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);
        dgvEmployees.AllowUserToAddRows          = false;
        dgvEmployees.AllowUserToDeleteRows       = false;
        dgvEmployees.BackgroundColor             = Color.White;
        dgvEmployees.BorderStyle                 = BorderStyle.FixedSingle;
        dgvEmployees.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvEmployees.EnableHeadersVisualStyles   = false;
        dgvEmployees.Font                        = ui9;
        dgvEmployees.GridColor                   = Color.FromArgb(220, 220, 220);
        dgvEmployees.Location                    = new Point(390, 125);
        dgvEmployees.MultiSelect                 = false;
        dgvEmployees.ReadOnly                    = true;
        dgvEmployees.RowHeadersVisible           = false;
        dgvEmployees.SelectionMode               = DataGridViewSelectionMode.FullRowSelect;
        dgvEmployees.Size                        = new Size(575, 510);
        dgvEmployees.CellClick += new DataGridViewCellEventHandler(dgvEmployees_CellClick);

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
        Controls.Add(dgvEmployees);
        Controls.Add(pnlSearch);
        Controls.Add(grpEmployeeInfo);
        Controls.Add(pnlHeader);
        Font              = ui9;
        FormBorderStyle   = FormBorderStyle.FixedSingle;
        MaximizeBox       = false;
        Name              = "frmEmployee";
        StartPosition     = FormStartPosition.CenterScreen;
        Text              = "Employee Management";
        Load += new System.EventHandler(frmEmployee_Load);

        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        grpEmployeeInfo.ResumeLayout(false);
        grpEmployeeInfo.PerformLayout();
        pnlSearch.ResumeLayout(false);
        pnlSearch.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvEmployees).EndInit();
        ResumeLayout(false);
    }

    // ── Local helpers (used only inside InitializeComponent) ─────────────────
    private static void SetLabel(Label lbl, string text, int x, int y, Color fore, Font font)
    {
        lbl.AutoSize  = true;
        lbl.Font      = font;
        lbl.ForeColor = fore;
        lbl.Location  = new Point(x, y);
        lbl.Text      = text;
    }

    private static void SetTextBox(TextBox txt, int x, int y, int width, Font font)
    {
        txt.Font     = font;
        txt.Location = new Point(x, y);
        txt.Size     = new Size(width, 23);
    }

    private static void StyleButton(Button btn, string text, Color back, Point loc, Size size, Font font, bool whiteText)
    {
        btn.BackColor              = back;
        btn.FlatAppearance.BorderSize = 1;
        btn.FlatAppearance.BorderColor = Color.FromArgb(180, 180, 180);
        btn.FlatStyle              = FlatStyle.Flat;
        btn.Font                   = font;
        btn.ForeColor              = whiteText ? Color.White : SystemColors.ControlText;
        btn.Location               = loc;
        btn.Size                   = size;
        btn.Text                   = text;
        btn.UseVisualStyleBackColor = false;
        btn.Cursor                 = Cursors.Hand;
    }
}
