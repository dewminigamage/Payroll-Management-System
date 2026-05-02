using System.Drawing;
using System.Windows.Forms;

namespace PayrollManagementSystem.Forms;

partial class frmBulkPayroll
{
    private System.ComponentModel.IContainer components = null;

    // Header
    private Panel  pnlHeader;
    private Label  lblTitle;
    private Label  lblSubtitle;

    // Top bar
    private Panel  pnlTopBar;
    private Label  lblMonth;
    private ComboBox cboMonth;
    private Label  lblYear;
    private NumericUpDown nudYear;
    private Button btnLoad;
    private Button btnCheckAll;
    private Button btnUncheckAll;
    private Label  lblRemarksLbl;
    private TextBox txtRemarks;

    // Grid
    private DataGridView dgvBulk;

    // Bottom bar
    private Panel  pnlBottom;
    private Label  lblSummaryCaption;
    private Label  lblTotalGrossLbl;
    private Label  lblTotalGrossVal;
    private Label  lblTotalEPFLbl;
    private Label  lblTotalEPFVal;
    private Label  lblTotalNetLbl;
    private Label  lblTotalNetVal;
    private Button btnProcess;
    private Label  lblStatus;

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

        pnlHeader       = new Panel();
        lblTitle        = new Label();
        lblSubtitle     = new Label();
        pnlTopBar       = new Panel();
        lblMonth        = new Label();
        cboMonth        = new ComboBox();
        lblYear         = new Label();
        nudYear         = new NumericUpDown();
        btnLoad         = new Button();
        btnCheckAll     = new Button();
        btnUncheckAll   = new Button();
        lblRemarksLbl   = new Label();
        txtRemarks      = new TextBox();
        dgvBulk         = new DataGridView();
        pnlBottom       = new Panel();
        lblSummaryCaption = new Label();
        lblTotalGrossLbl = new Label();
        lblTotalGrossVal = new Label();
        lblTotalEPFLbl  = new Label();
        lblTotalEPFVal  = new Label();
        lblTotalNetLbl  = new Label();
        lblTotalNetVal  = new Label();
        btnProcess      = new Button();
        lblStatus       = new Label();

        SuspendLayout();
        pnlHeader.SuspendLayout();
        pnlTopBar.SuspendLayout();
        pnlBottom.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvBulk).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nudYear).BeginInit();

        // ── Header ────────────────────────────────────────────────
        pnlHeader.BackColor = blue;
        pnlHeader.Dock      = DockStyle.Top;
        pnlHeader.Height    = 72;
        pnlHeader.Controls.Add(lblSubtitle);
        pnlHeader.Controls.Add(lblTitle);

        lblTitle.AutoSize  = true;
        lblTitle.Font      = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location  = new Point(20, 10);
        lblTitle.Text      = "Bulk Payroll Processing";

        lblSubtitle.AutoSize  = true;
        lblSubtitle.Font      = new Font("Segoe UI", 9F);
        lblSubtitle.ForeColor = Color.FromArgb(200, 230, 255);
        lblSubtitle.Location  = new Point(22, 44);
        lblSubtitle.Text      = "Process payroll for all employees in one step";

        // ── Top bar ───────────────────────────────────────────────
        pnlTopBar.BackColor   = Color.FromArgb(238, 242, 248);
        pnlTopBar.BorderStyle = BorderStyle.FixedSingle;
        pnlTopBar.Dock        = DockStyle.Top;
        pnlTopBar.Height      = 58;

        Lbl(lblMonth, "Month:", new Point(12, 19));
        cboMonth.DropDownStyle   = ComboBoxStyle.DropDownList;
        cboMonth.Font            = new Font("Segoe UI", 9F);
        cboMonth.Location        = new Point(60, 15);
        cboMonth.Size            = new Size(115, 23);
        cboMonth.Items.AddRange(new object[] {
            "January","February","March","April","May","June",
            "July","August","September","October","November","December"
        });
        cboMonth.SelectedIndex = DateTime.Today.Month - 1;

        Lbl(lblYear, "Year:", new Point(186, 19));
        nudYear.Font       = new Font("Segoe UI", 9F);
        nudYear.Location   = new Point(224, 15);
        nudYear.Size       = new Size(78, 23);
        nudYear.Minimum    = 2000;
        nudYear.Maximum    = 2100;
        nudYear.Value      = DateTime.Today.Year;

        Btn(btnLoad,       "Load Employees",  new Point(314, 13), new Size(130, 28), blue, Color.White);
        Btn(btnCheckAll,   "Check All",        new Point(456, 13), new Size(95,  28), Color.FromArgb(0, 140, 80), Color.White);
        Btn(btnUncheckAll, "Uncheck All",      new Point(557, 13), new Size(95,  28), Color.FromArgb(100,100,100), Color.White);

        Lbl(lblRemarksLbl, "Batch Remarks:", new Point(666, 19));
        txtRemarks.Font     = new Font("Segoe UI", 9F);
        txtRemarks.Location = new Point(762, 15);
        txtRemarks.Size     = new Size(282, 23);
        txtRemarks.MaxLength = 255;

        btnLoad.Click       += new EventHandler(btnLoad_Click);
        btnCheckAll.Click   += new EventHandler(btnCheckAll_Click);
        btnUncheckAll.Click += new EventHandler(btnUncheckAll_Click);

        pnlTopBar.Controls.AddRange(new Control[] {
            lblMonth, cboMonth, lblYear, nudYear, btnLoad,
            btnCheckAll, btnUncheckAll, lblRemarksLbl, txtRemarks
        });

        // ── Grid ──────────────────────────────────────────────────
        dgvBulk.AllowUserToAddRows    = false;
        dgvBulk.AllowUserToDeleteRows = false;
        dgvBulk.BackgroundColor       = Color.White;
        dgvBulk.BorderStyle           = BorderStyle.None;
        dgvBulk.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvBulk.Dock                  = DockStyle.Fill;
        dgvBulk.GridColor             = Color.FromArgb(210, 218, 230);
        dgvBulk.MultiSelect           = false;
        dgvBulk.RowHeadersVisible     = false;
        dgvBulk.SelectionMode         = DataGridViewSelectionMode.FullRowSelect;
        dgvBulk.DefaultCellStyle.Font = new Font("Segoe UI", 8.5F);
        dgvBulk.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        dgvBulk.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 238, 248);
        dgvBulk.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(30, 30, 30);
        dgvBulk.EnableHeadersVisualStyles = false;
        dgvBulk.CellEndEdit     += new DataGridViewCellEventHandler(dgvBulk_CellEndEdit);
        dgvBulk.CellBeginEdit   += new DataGridViewCellCancelEventHandler(dgvBulk_CellBeginEdit);
        dgvBulk.CellValueChanged += new DataGridViewCellEventHandler(dgvBulk_CellValueChanged);
        dgvBulk.CurrentCellDirtyStateChanged += new EventHandler(dgvBulk_CurrentCellDirtyStateChanged);
        dgvBulk.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(dgvBulk_DataBindingComplete);

        // ── Bottom bar ────────────────────────────────────────────
        pnlBottom.BackColor   = Color.FromArgb(238, 242, 248);
        pnlBottom.BorderStyle = BorderStyle.FixedSingle;
        pnlBottom.Dock        = DockStyle.Bottom;
        pnlBottom.Height      = 58;

        Lbl(lblSummaryCaption, "CHECKED TOTALS:", new Point(12, 20));
        lblSummaryCaption.Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        lblSummaryCaption.ForeColor = Color.FromArgb(0, 120, 212);

        Lbl(lblTotalGrossLbl, "Gross:", new Point(160, 20));
        ValLbl(lblTotalGrossVal, new Point(200, 20), 110);

        Lbl(lblTotalEPFLbl, "EPF (8%):", new Point(320, 20));
        ValLbl(lblTotalEPFVal, new Point(382, 20), 100);

        Lbl(lblTotalNetLbl, "Net Salary:", new Point(492, 20));
        ValLbl(lblTotalNetVal, new Point(560, 20), 120);
        lblTotalNetVal.Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblTotalNetVal.ForeColor = Color.FromArgb(0, 100, 0);

        Btn(btnProcess, "Process Payroll", new Point(694, 13), new Size(148, 32),
            Color.FromArgb(0, 140, 80), Color.White);
        btnProcess.Font   = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnProcess.Click += new EventHandler(btnProcess_Click);

        lblStatus.AutoSize  = false;
        lblStatus.Font      = new Font("Segoe UI", 8.5F);
        lblStatus.ForeColor = Color.FromArgb(80, 80, 80);
        lblStatus.Location  = new Point(852, 20);
        lblStatus.Size      = new Size(250, 22);
        lblStatus.Text      = "";

        pnlBottom.Controls.AddRange(new Control[] {
            lblSummaryCaption,
            lblTotalGrossLbl, lblTotalGrossVal,
            lblTotalEPFLbl,   lblTotalEPFVal,
            lblTotalNetLbl,   lblTotalNetVal,
            btnProcess, lblStatus
        });

        // ── Form ──────────────────────────────────────────────────
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode       = AutoScaleMode.Font;
        BackColor           = lightBg;
        ClientSize          = new Size(1100, 620);
        Controls.Add(dgvBulk);
        Controls.Add(pnlTopBar);
        Controls.Add(pnlBottom);
        Controls.Add(pnlHeader);
        Font            = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        Name            = "frmBulkPayroll";
        StartPosition   = FormStartPosition.CenterScreen;
        Text            = "Bulk Payroll Processing";

        ((System.ComponentModel.ISupportInitialize)nudYear).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvBulk).EndInit();
        pnlBottom.ResumeLayout(false);
        pnlBottom.PerformLayout();
        pnlTopBar.ResumeLayout(false);
        pnlTopBar.PerformLayout();
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private static void Lbl(Label l, string text, Point loc)
    {
        l.AutoSize  = true;
        l.Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        l.ForeColor = Color.FromArgb(70, 70, 80);
        l.Location  = loc;
        l.Text      = text;
    }

    private static void ValLbl(Label l, Point loc, int width)
    {
        l.AutoSize  = false;
        l.Font      = new Font("Segoe UI", 9F);
        l.ForeColor = Color.FromArgb(30, 30, 30);
        l.Location  = loc;
        l.Size      = new Size(width, 22);
        l.Text      = "—";
        l.TextAlign = ContentAlignment.MiddleRight;
    }

    private static void Btn(Button b, string text, Point loc, Size sz, Color bg, Color fg)
    {
        b.BackColor                 = bg;
        b.FlatAppearance.BorderSize = 0;
        b.FlatStyle                 = FlatStyle.Flat;
        b.Font                      = new Font("Segoe UI", 9F, FontStyle.Bold);
        b.ForeColor                 = fg;
        b.Location                  = loc;
        b.Size                      = sz;
        b.Text                      = text;
        b.UseVisualStyleBackColor   = false;
        b.Cursor                    = Cursors.Hand;
    }
}
