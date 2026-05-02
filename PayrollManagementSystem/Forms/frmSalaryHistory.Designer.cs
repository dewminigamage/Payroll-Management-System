using System.Drawing;
using System.Windows.Forms;

namespace PayrollManagementSystem.Forms;

partial class frmSalaryHistory
{
    private System.ComponentModel.IContainer components = null;

    private Panel       pnlHeader;
    private Label       lblTitle;
    private Label       lblEmployee;
    private DataGridView dgvHistory;
    private Label       lblStatus;
    private Button      btnClose;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var blue = Color.FromArgb(0, 120, 212);

        pnlHeader   = new Panel();
        lblTitle    = new Label();
        lblEmployee = new Label();
        dgvHistory  = new DataGridView();
        lblStatus   = new Label();
        btnClose    = new Button();

        // ── Header ─────────────────────────────────────────────
        pnlHeader.BackColor = blue;
        pnlHeader.Dock      = DockStyle.Top;
        pnlHeader.Size      = new Size(760, 65);
        pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblEmployee });

        lblTitle.AutoSize  = true;
        lblTitle.Font      = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location  = new Point(20, 8);
        lblTitle.Text      = "Salary Increment History";

        lblEmployee.AutoSize  = true;
        lblEmployee.Font      = new Font("Segoe UI", 9F);
        lblEmployee.ForeColor = Color.FromArgb(200, 230, 255);
        lblEmployee.Location  = new Point(22, 38);
        lblEmployee.Text      = "";

        // ── Grid ───────────────────────────────────────────────
        dgvHistory.Location                                = new Point(10, 75);
        dgvHistory.Size                                    = new Size(738, 380);
        dgvHistory.ReadOnly                                = true;
        dgvHistory.SelectionMode                           = DataGridViewSelectionMode.FullRowSelect;
        dgvHistory.MultiSelect                             = false;
        dgvHistory.AllowUserToAddRows                      = false;
        dgvHistory.AllowUserToDeleteRows                   = false;
        dgvHistory.BackgroundColor                         = Color.White;
        dgvHistory.BorderStyle                             = BorderStyle.Fixed3D;
        dgvHistory.RowHeadersVisible                       = false;
        dgvHistory.Font                                    = new Font("Segoe UI", 9F);
        dgvHistory.AutoSizeColumnsMode                     = DataGridViewAutoSizeColumnsMode.Fill;
        dgvHistory.ColumnHeadersDefaultCellStyle.BackColor = blue;
        dgvHistory.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvHistory.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        dgvHistory.EnableHeadersVisualStyles               = false;

        // ── Status + Close ─────────────────────────────────────
        lblStatus.AutoSize  = true;
        lblStatus.Font      = new Font("Segoe UI", 8.5F);
        lblStatus.ForeColor = Color.FromArgb(80, 80, 80);
        lblStatus.Location  = new Point(12, 463);
        lblStatus.Text      = "";

        btnClose.Text      = "Close";
        btnClose.Location  = new Point(658, 458);
        btnClose.Size      = new Size(90, 30);
        btnClose.BackColor = Color.FromArgb(100, 100, 100);
        btnClose.ForeColor = Color.White;
        btnClose.FlatStyle = FlatStyle.Flat;
        btnClose.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnClose.Cursor    = Cursors.Hand;
        btnClose.Click    += (s, e) => Close();

        // ── Form ───────────────────────────────────────────────
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode       = AutoScaleMode.Font;
        BackColor           = Color.FromArgb(245, 247, 250);
        ClientSize          = new Size(760, 500);
        Controls.Add(btnClose);
        Controls.Add(lblStatus);
        Controls.Add(dgvHistory);
        Controls.Add(pnlHeader);
        Font            = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        Name            = "frmSalaryHistory";
        StartPosition   = FormStartPosition.CenterParent;
        Text            = "Salary Increment History";
        Load           += frmSalaryHistory_Load;
    }
}
