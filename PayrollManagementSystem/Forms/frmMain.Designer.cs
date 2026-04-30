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
    private System.Windows.Forms.Label lblFooter;

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
        lblFooter = new Label();

        SuspendLayout();
        pnlHeader.SuspendLayout();

        // ── pnlHeader ──────────────────────────────────────────
        pnlHeader.BackColor = primaryBlue;
        pnlHeader.Controls.Add(lblSubtitle);
        pnlHeader.Controls.Add(lblMainTitle);
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
