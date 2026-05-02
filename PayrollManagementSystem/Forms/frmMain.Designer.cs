using System.Drawing;
using System.Windows.Forms;

namespace PayrollManagementSystem.Forms;

partial class frmMain
{
    private System.ComponentModel.IContainer components = null;

    private System.Windows.Forms.Panel pnlHeader;
    private System.Windows.Forms.Label lblMainTitle;
    private System.Windows.Forms.Label lblSubtitle;
    private System.Windows.Forms.Label lblModulesHeading;
    private System.Windows.Forms.Button btnEmployeeManagement;
    private System.Windows.Forms.Button btnAttendanceManagement;
    private System.Windows.Forms.Button btnPayroll;
    private System.Windows.Forms.Button btnReports;
    private System.Windows.Forms.Label lblWelcome;
    private System.Windows.Forms.Button btnLogout;
    private System.Windows.Forms.Label lblFooter;
    private System.Windows.Forms.Button btnUserManagement;
    private System.Windows.Forms.Button btnBulkPayroll;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var primaryBlue = Color.FromArgb(0, 120, 212);

        pnlHeader = new Panel();
        lblMainTitle = new Label();
        lblSubtitle = new Label();
        lblModulesHeading = new Label();
        btnEmployeeManagement = new Button();
        btnAttendanceManagement = new Button();
        btnPayroll = new Button();
        btnReports = new Button();
        lblWelcome = new Label();
        btnLogout  = new Button();
        lblFooter          = new Label();
        btnUserManagement  = new Button();
        btnBulkPayroll     = new Button();

        SuspendLayout();
        pnlHeader.SuspendLayout();

        // ── pnlHeader ──────────────────────────────────────────
        pnlHeader.BackColor = primaryBlue;
        pnlHeader.Controls.Add(lblSubtitle);
        pnlHeader.Controls.Add(lblMainTitle);
        pnlHeader.Controls.Add(lblWelcome);
        pnlHeader.Controls.Add(btnLogout);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Size = new Size(820, 80);

        lblMainTitle.AutoSize = true;
        lblMainTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblMainTitle.ForeColor = Color.White;
        lblMainTitle.Location = new Point(25, 12);
        lblMainTitle.Text = "Payroll Management System";

        lblSubtitle.AutoSize = true;
        lblSubtitle.Font = new Font("Segoe UI", 9.5F);
        lblSubtitle.ForeColor = Color.FromArgb(200, 230, 255);
        lblSubtitle.Location = new Point(27, 48);
        lblSubtitle.Text = "Manage your workforce efficiently";

        // ── Welcome label (top-right of header) ───────────────
        lblWelcome.AutoSize  = false;
        lblWelcome.Font      = new Font("Segoe UI", 9F);
        lblWelcome.ForeColor = Color.FromArgb(200, 230, 255);
        lblWelcome.Location  = new Point(490, 50);
        lblWelcome.Size      = new Size(240, 18);
        lblWelcome.Text      = "";
        lblWelcome.TextAlign = ContentAlignment.MiddleRight;

        // ── Logout button (top-right of header) ───────────────
        btnLogout.BackColor                 = Color.FromArgb(0, 95, 185);
        btnLogout.FlatAppearance.BorderColor = Color.FromArgb(180, 220, 255);
        btnLogout.FlatAppearance.BorderSize = 1;
        btnLogout.FlatStyle                 = FlatStyle.Flat;
        btnLogout.Font                      = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        btnLogout.ForeColor                 = Color.White;
        btnLogout.Location                  = new Point(715, 22);
        btnLogout.Size                      = new Size(85, 28);
        btnLogout.Text                      = "Sign Out";
        btnLogout.UseVisualStyleBackColor   = false;
        btnLogout.Cursor                    = Cursors.Hand;
        btnLogout.Click += new System.EventHandler(btnLogout_Click);

        // ── Modules heading ────────────────────────────────────
        lblModulesHeading.AutoSize = true;
        lblModulesHeading.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblModulesHeading.ForeColor = Color.FromArgb(80, 80, 80);
        lblModulesHeading.Location = new Point(25, 105);
        lblModulesHeading.Text = "Modules";

        // ── Employee Management tile ───────────────────────────
        btnEmployeeManagement.BackColor = Color.White;
        btnEmployeeManagement.FlatAppearance.BorderColor = primaryBlue;
        btnEmployeeManagement.FlatAppearance.BorderSize = 2;
        btnEmployeeManagement.FlatStyle = FlatStyle.Flat;
        btnEmployeeManagement.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnEmployeeManagement.ForeColor = primaryBlue;
        btnEmployeeManagement.Location = new Point(25, 130);
        btnEmployeeManagement.Size = new Size(210, 100);
        btnEmployeeManagement.Text = "Employee\r\nManagement";
        btnEmployeeManagement.UseVisualStyleBackColor = false;
        btnEmployeeManagement.Cursor = Cursors.Hand;
        btnEmployeeManagement.Click += new System.EventHandler(btnEmployeeManagement_Click);

        // ── Attendance Management tile ─────────────────────────
        btnAttendanceManagement.BackColor = Color.White;
        btnAttendanceManagement.FlatAppearance.BorderColor = primaryBlue;
        btnAttendanceManagement.FlatAppearance.BorderSize = 2;
        btnAttendanceManagement.FlatStyle = FlatStyle.Flat;
        btnAttendanceManagement.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnAttendanceManagement.ForeColor = primaryBlue;
        btnAttendanceManagement.Location = new Point(255, 130);
        btnAttendanceManagement.Size = new Size(210, 100);
        btnAttendanceManagement.Text = "Attendance\r\nManagement";
        btnAttendanceManagement.UseVisualStyleBackColor = false;
        btnAttendanceManagement.Cursor = Cursors.Hand;
        btnAttendanceManagement.Click += new System.EventHandler(btnAttendanceManagement_Click);

        // ── Payroll tile ───────────────────────────────────────
        btnPayroll.BackColor = Color.White;
        btnPayroll.FlatAppearance.BorderColor = primaryBlue;
        btnPayroll.FlatAppearance.BorderSize = 2;
        btnPayroll.FlatStyle = FlatStyle.Flat;
        btnPayroll.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnPayroll.ForeColor = primaryBlue;
        btnPayroll.Location = new Point(485, 130);
        btnPayroll.Size = new Size(210, 100);
        btnPayroll.Text = "Payroll /\r\nSalary Calculation";
        btnPayroll.UseVisualStyleBackColor = false;
        btnPayroll.Cursor = Cursors.Hand;
        btnPayroll.Click += new System.EventHandler(btnPayroll_Click);

        // ── Reports tile (row 2) ───────────────────────────────
        btnReports.BackColor = Color.White;
        btnReports.FlatAppearance.BorderColor = primaryBlue;
        btnReports.FlatAppearance.BorderSize = 2;
        btnReports.FlatStyle = FlatStyle.Flat;
        btnReports.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnReports.ForeColor = primaryBlue;
        btnReports.Location = new Point(25, 250);
        btnReports.Size = new Size(210, 100);
        btnReports.Text = "Reports";
        btnReports.UseVisualStyleBackColor = false;
        btnReports.Cursor = Cursors.Hand;
        btnReports.Click += new System.EventHandler(btnReports_Click);

        // ── User Management tile (row 2) ──────────────────────────
        btnUserManagement.BackColor = Color.White;
        btnUserManagement.FlatAppearance.BorderColor = primaryBlue;
        btnUserManagement.FlatAppearance.BorderSize = 2;
        btnUserManagement.FlatStyle = FlatStyle.Flat;
        btnUserManagement.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnUserManagement.ForeColor = primaryBlue;
        btnUserManagement.Location = new Point(255, 250);
        btnUserManagement.Size = new Size(210, 100);
        btnUserManagement.Text = "User\r\nManagement";
        btnUserManagement.UseVisualStyleBackColor = false;
        btnUserManagement.Cursor = Cursors.Hand;
        btnUserManagement.Click += new System.EventHandler(btnUserManagement_Click);

        // ── Bulk Payroll tile (row 2) ─────────────────────────────
        btnBulkPayroll.BackColor = Color.White;
        btnBulkPayroll.FlatAppearance.BorderColor = primaryBlue;
        btnBulkPayroll.FlatAppearance.BorderSize = 2;
        btnBulkPayroll.FlatStyle = FlatStyle.Flat;
        btnBulkPayroll.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnBulkPayroll.ForeColor = primaryBlue;
        btnBulkPayroll.Location = new Point(485, 250);
        btnBulkPayroll.Size = new Size(210, 100);
        btnBulkPayroll.Text = "Bulk Payroll\r\nProcessing";
        btnBulkPayroll.UseVisualStyleBackColor = false;
        btnBulkPayroll.Cursor = Cursors.Hand;
        btnBulkPayroll.Click += new System.EventHandler(btnBulkPayroll_Click);

        // ── Footer ─────────────────────────────────────────────
        lblFooter.AutoSize = false;
        lblFooter.Font = new Font("Segoe UI", 8F);
        lblFooter.ForeColor = Color.FromArgb(150, 150, 150);
        lblFooter.Location = new Point(0, 468);
        lblFooter.Size = new Size(820, 20);
        lblFooter.Text = "   Payroll Management System  |  2025";
        lblFooter.TextAlign = ContentAlignment.MiddleLeft;

        // ── Form ───────────────────────────────────────────────
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(245, 247, 250);
        ClientSize = new Size(820, 490);
        Controls.Add(lblFooter);
        Controls.Add(btnBulkPayroll);
        Controls.Add(btnUserManagement);
        Controls.Add(btnReports);
        Controls.Add(btnPayroll);
        Controls.Add(btnAttendanceManagement);
        Controls.Add(btnEmployeeManagement);
        Controls.Add(lblModulesHeading);
        Controls.Add(pnlHeader);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "frmMain";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Payroll Management System";

        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
