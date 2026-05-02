using System.Drawing;
using System.Windows.Forms;

namespace PayrollManagementSystem.Forms;

partial class frmReports
{
    private System.ComponentModel.IContainer components = null;

    private Panel        pnlHeader;
    private Label        lblTitle;
    private Label        lblSubtitle;

    private TabControl   tabControl;

    // ── Tab 1: Payroll Summary ─────────────────────────────────
    private TabPage      tabPayroll;
    private Panel        pnlPRFilter;
    private Label        lblPRMonth;
    private ComboBox     cmbPRMonth;
    private Label        lblPRYear;
    private NumericUpDown nudPRYear;
    private Button       btnGeneratePR;
    private Label        lblPRStatus;
    private Panel        pnlPRBottom;
    private Button       btnExportPR;
    private Label        lblPRTotals;
    private DataGridView dgvPayrollReport;

    // ── Tab 2: Attendance Summary ──────────────────────────────
    private TabPage      tabAttendance;
    private Panel        pnlATFilter;
    private Label        lblATMonth;
    private ComboBox     cmbATMonth;
    private Label        lblATYear;
    private NumericUpDown nudATYear;
    private Button       btnGenerateAT;
    private Label        lblATStatus;
    private Panel        pnlATBottom;
    private Button       btnExportAT;
    private DataGridView dgvAttReport;

    // ── Tab 3: Employee Directory ──────────────────────────────
    private TabPage      tabEmployees;
    private Panel        pnlEmpFilter;
    private Label        lblEmpDept;
    private ComboBox     cmbEmpDept;
    private Button       btnGenerateEmp;
    private Label        lblEmpStatus;
    private Panel        pnlEmpBottom;
    private Button       btnExportEmp;
    private DataGridView dgvEmpReport;

    private Label        lblFooter;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var blue      = Color.FromArgb(0, 120, 212);
        var lightBg   = Color.FromArgb(245, 247, 250);
        var filterBg  = Color.FromArgb(250, 250, 252);

        // ── Instantiate ────────────────────────────────────────
        pnlHeader        = new Panel();
        lblTitle         = new Label();
        lblSubtitle      = new Label();
        tabControl       = new TabControl();
        tabPayroll       = new TabPage();
        pnlPRFilter      = new Panel();
        lblPRMonth       = new Label();
        cmbPRMonth       = new ComboBox();
        lblPRYear        = new Label();
        nudPRYear        = new NumericUpDown();
        btnGeneratePR    = new Button();
        lblPRStatus      = new Label();
        pnlPRBottom      = new Panel();
        btnExportPR      = new Button();
        lblPRTotals      = new Label();
        dgvPayrollReport = new DataGridView();
        tabAttendance    = new TabPage();
        pnlATFilter      = new Panel();
        lblATMonth       = new Label();
        cmbATMonth       = new ComboBox();
        lblATYear        = new Label();
        nudATYear        = new NumericUpDown();
        btnGenerateAT    = new Button();
        lblATStatus      = new Label();
        pnlATBottom      = new Panel();
        btnExportAT      = new Button();
        dgvAttReport     = new DataGridView();
        tabEmployees     = new TabPage();
        pnlEmpFilter     = new Panel();
        lblEmpDept       = new Label();
        cmbEmpDept       = new ComboBox();
        btnGenerateEmp   = new Button();
        lblEmpStatus     = new Label();
        pnlEmpBottom     = new Panel();
        btnExportEmp     = new Button();
        dgvEmpReport     = new DataGridView();
        lblFooter        = new Label();

        SuspendLayout();
        pnlHeader.SuspendLayout();

        // ── Header ─────────────────────────────────────────────
        pnlHeader.BackColor = blue;
        pnlHeader.Controls.Add(lblSubtitle);
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Size = new Size(1040, 80);

        lblTitle.AutoSize  = true;
        lblTitle.Font      = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location  = new Point(25, 12);
        lblTitle.Text      = "Reports";

        lblSubtitle.AutoSize  = true;
        lblSubtitle.Font      = new Font("Segoe UI", 9.5F);
        lblSubtitle.ForeColor = Color.FromArgb(200, 230, 255);
        lblSubtitle.Location  = new Point(27, 48);
        lblSubtitle.Text      = "View and export payroll, attendance, and employee reports";

        // ── TabControl ─────────────────────────────────────────
        tabControl.Location = new Point(10, 90);
        tabControl.Size     = new Size(1020, 460);
        tabControl.Font     = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        tabControl.TabPages.Add(tabPayroll);
        tabControl.TabPages.Add(tabAttendance);
        tabControl.TabPages.Add(tabEmployees);

        // ── Tab 1: Payroll Summary ─────────────────────────────
        tabPayroll.Text      = "  Payroll Summary  ";
        tabPayroll.BackColor = Color.White;
        tabPayroll.Padding   = new Padding(4);
        // Add Dock=Top first, Dock=Bottom second, Dock=Fill LAST
        tabPayroll.Controls.Add(pnlPRFilter);
        tabPayroll.Controls.Add(pnlPRBottom);
        tabPayroll.Controls.Add(dgvPayrollReport);

        pnlPRFilter.Dock        = DockStyle.Top;
        pnlPRFilter.Height      = 46;
        pnlPRFilter.BackColor   = filterBg;
        pnlPRFilter.BorderStyle = BorderStyle.FixedSingle;
        pnlPRFilter.Controls.Add(lblPRMonth);
        pnlPRFilter.Controls.Add(cmbPRMonth);
        pnlPRFilter.Controls.Add(lblPRYear);
        pnlPRFilter.Controls.Add(nudPRYear);
        pnlPRFilter.Controls.Add(btnGeneratePR);
        pnlPRFilter.Controls.Add(lblPRStatus);

        SetLbl(lblPRMonth, "Month:", 10, 13);
        cmbPRMonth.Location      = new Point(60, 10);
        cmbPRMonth.Size          = new Size(115, 26);
        cmbPRMonth.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPRMonth.Items.AddRange(MonthItems());
        cmbPRMonth.SelectedIndex = DateTime.Today.Month - 1;

        SetLbl(lblPRYear, "Year:", 186, 13);
        nudPRYear.Location      = new Point(222, 10);
        nudPRYear.Size          = new Size(72, 26);
        nudPRYear.Minimum       = 2000;
        nudPRYear.Maximum       = 2099;
        nudPRYear.Value         = DateTime.Today.Year;
        nudPRYear.DecimalPlaces = 0;

        SetBtn(btnGeneratePR, "Generate", 306, 9, 92, 28, blue, Color.White);
        btnGeneratePR.Click += new EventHandler(btnGeneratePR_Click);

        lblPRStatus.AutoSize  = true;
        lblPRStatus.Font      = new Font("Segoe UI", 9F);
        lblPRStatus.ForeColor = Color.FromArgb(80, 80, 80);
        lblPRStatus.Location  = new Point(412, 14);
        lblPRStatus.Text      = "";

        pnlPRBottom.Dock        = DockStyle.Bottom;
        pnlPRBottom.Height      = 44;
        pnlPRBottom.BackColor   = filterBg;
        pnlPRBottom.BorderStyle = BorderStyle.FixedSingle;
        pnlPRBottom.Controls.Add(btnExportPR);
        pnlPRBottom.Controls.Add(lblPRTotals);

        SetBtn(btnExportPR, "Export to CSV", 10, 8, 115, 28, Color.FromArgb(0, 140, 70), Color.White);
        btnExportPR.Click += new EventHandler(btnExportPR_Click);

        lblPRTotals.AutoSize  = false;
        lblPRTotals.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPRTotals.ForeColor = Color.FromArgb(0, 80, 160);
        lblPRTotals.Location  = new Point(140, 13);
        lblPRTotals.Size      = new Size(860, 18);
        lblPRTotals.Text      = "";

        SetupDgv(dgvPayrollReport, blue);

        // ── Tab 2: Attendance Summary ──────────────────────────
        tabAttendance.Text      = "  Attendance Summary  ";
        tabAttendance.BackColor = Color.White;
        tabAttendance.Padding   = new Padding(4);
        tabAttendance.Controls.Add(pnlATFilter);
        tabAttendance.Controls.Add(pnlATBottom);
        tabAttendance.Controls.Add(dgvAttReport);

        pnlATFilter.Dock        = DockStyle.Top;
        pnlATFilter.Height      = 46;
        pnlATFilter.BackColor   = filterBg;
        pnlATFilter.BorderStyle = BorderStyle.FixedSingle;
        pnlATFilter.Controls.Add(lblATMonth);
        pnlATFilter.Controls.Add(cmbATMonth);
        pnlATFilter.Controls.Add(lblATYear);
        pnlATFilter.Controls.Add(nudATYear);
        pnlATFilter.Controls.Add(btnGenerateAT);
        pnlATFilter.Controls.Add(lblATStatus);

        SetLbl(lblATMonth, "Month:", 10, 13);
        cmbATMonth.Location      = new Point(60, 10);
        cmbATMonth.Size          = new Size(115, 26);
        cmbATMonth.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbATMonth.Items.AddRange(MonthItems());
        cmbATMonth.SelectedIndex = DateTime.Today.Month - 1;

        SetLbl(lblATYear, "Year:", 186, 13);
        nudATYear.Location      = new Point(222, 10);
        nudATYear.Size          = new Size(72, 26);
        nudATYear.Minimum       = 2000;
        nudATYear.Maximum       = 2099;
        nudATYear.Value         = DateTime.Today.Year;
        nudATYear.DecimalPlaces = 0;

        SetBtn(btnGenerateAT, "Generate", 306, 9, 92, 28, blue, Color.White);
        btnGenerateAT.Click += new EventHandler(btnGenerateAT_Click);

        lblATStatus.AutoSize  = true;
        lblATStatus.Font      = new Font("Segoe UI", 9F);
        lblATStatus.ForeColor = Color.FromArgb(80, 80, 80);
        lblATStatus.Location  = new Point(412, 14);
        lblATStatus.Text      = "";

        pnlATBottom.Dock        = DockStyle.Bottom;
        pnlATBottom.Height      = 44;
        pnlATBottom.BackColor   = filterBg;
        pnlATBottom.BorderStyle = BorderStyle.FixedSingle;
        pnlATBottom.Controls.Add(btnExportAT);

        SetBtn(btnExportAT, "Export to CSV", 10, 8, 115, 28, Color.FromArgb(0, 140, 70), Color.White);
        btnExportAT.Click += new EventHandler(btnExportAT_Click);

        SetupDgv(dgvAttReport, blue);

        // ── Tab 3: Employee Directory ──────────────────────────
        tabEmployees.Text      = "  Employee Directory  ";
        tabEmployees.BackColor = Color.White;
        tabEmployees.Padding   = new Padding(4);
        tabEmployees.Controls.Add(pnlEmpFilter);
        tabEmployees.Controls.Add(pnlEmpBottom);
        tabEmployees.Controls.Add(dgvEmpReport);

        pnlEmpFilter.Dock        = DockStyle.Top;
        pnlEmpFilter.Height      = 46;
        pnlEmpFilter.BackColor   = filterBg;
        pnlEmpFilter.BorderStyle = BorderStyle.FixedSingle;
        pnlEmpFilter.Controls.Add(lblEmpDept);
        pnlEmpFilter.Controls.Add(cmbEmpDept);
        pnlEmpFilter.Controls.Add(btnGenerateEmp);
        pnlEmpFilter.Controls.Add(lblEmpStatus);

        SetLbl(lblEmpDept, "Department:", 10, 13);
        cmbEmpDept.Location      = new Point(90, 10);
        cmbEmpDept.Size          = new Size(165, 26);
        cmbEmpDept.DropDownStyle = ComboBoxStyle.DropDownList;

        SetBtn(btnGenerateEmp, "Generate", 268, 9, 92, 28, blue, Color.White);
        btnGenerateEmp.Click += new EventHandler(btnGenerateEmp_Click);

        lblEmpStatus.AutoSize  = true;
        lblEmpStatus.Font      = new Font("Segoe UI", 9F);
        lblEmpStatus.ForeColor = Color.FromArgb(80, 80, 80);
        lblEmpStatus.Location  = new Point(374, 14);
        lblEmpStatus.Text      = "";

        pnlEmpBottom.Dock        = DockStyle.Bottom;
        pnlEmpBottom.Height      = 44;
        pnlEmpBottom.BackColor   = filterBg;
        pnlEmpBottom.BorderStyle = BorderStyle.FixedSingle;
        pnlEmpBottom.Controls.Add(btnExportEmp);

        SetBtn(btnExportEmp, "Export to CSV", 10, 8, 115, 28, Color.FromArgb(0, 140, 70), Color.White);
        btnExportEmp.Click += new EventHandler(btnExportEmp_Click);

        SetupDgv(dgvEmpReport, blue);

        // ── Footer ─────────────────────────────────────────────
        lblFooter.AutoSize  = false;
        lblFooter.Font      = new Font("Segoe UI", 8F);
        lblFooter.ForeColor = Color.FromArgb(150, 150, 150);
        lblFooter.Location  = new Point(0, 558);
        lblFooter.Size      = new Size(1040, 20);
        lblFooter.Text      = "   Payroll Management System  |  2025";
        lblFooter.TextAlign = ContentAlignment.MiddleLeft;

        // ── Form ───────────────────────────────────────────────
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode       = AutoScaleMode.Font;
        BackColor           = lightBg;
        ClientSize          = new Size(1040, 580);
        Controls.Add(lblFooter);
        Controls.Add(tabControl);
        Controls.Add(pnlHeader);
        Font            = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        Name            = "frmReports";
        StartPosition   = FormStartPosition.CenterScreen;
        Text            = "Reports";
        Load           += new EventHandler(frmReports_Load);

        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private static object[] MonthItems() => new object[]
    {
        "January","February","March","April","May","June",
        "July","August","September","October","November","December"
    };

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

    private static void SetupDgv(DataGridView dgv, Color headerColor)
    {
        dgv.Dock                                    = DockStyle.Fill;
        dgv.ReadOnly                                = true;
        dgv.SelectionMode                           = DataGridViewSelectionMode.FullRowSelect;
        dgv.MultiSelect                             = false;
        dgv.AllowUserToAddRows                      = false;
        dgv.AllowUserToDeleteRows                   = false;
        dgv.BackgroundColor                         = Color.White;
        dgv.BorderStyle                             = BorderStyle.None;
        dgv.RowHeadersVisible                       = false;
        dgv.Font                                    = new Font("Segoe UI", 9F);
        dgv.ColumnHeadersDefaultCellStyle.BackColor = headerColor;
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgv.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        dgv.EnableHeadersVisualStyles               = false;
        dgv.AutoSizeColumnsMode                     = DataGridViewAutoSizeColumnsMode.Fill;
    }
}
