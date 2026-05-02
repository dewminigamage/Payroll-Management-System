using System.Drawing;
using System.Windows.Forms;

namespace PayrollManagementSystem.Forms;

partial class frmLeaveManagement
{
    private System.ComponentModel.IContainer components = null;

    // Header
    private Panel pnlHeader;
    private Label lblTitle;
    private Label lblSubtitle;

    // Tab control
    private TabControl tabMain;
    private TabPage tabRequests;
    private TabPage tabBalances;
    private TabPage tabLeaveTypes;

    // ── TAB 1: Leave Requests ──────────────────────────────────
    private GroupBox grpRequestForm;
    private Label lblReqEmployee;
    private ComboBox cmbReqEmployee;
    private Label lblReqLeaveType;
    private ComboBox cmbReqLeaveType;
    private Label lblReqStart;
    private DateTimePicker dtpReqStart;
    private Label lblReqEnd;
    private DateTimePicker dtpReqEnd;
    private Label lblReqDays;
    private Label lblReqDaysValue;
    private Label lblReqReason;
    private TextBox txtReqReason;
    private Label lblReqID;
    private TextBox txtReqID;
    private Button btnReqSave;
    private Button btnReqApprove;
    private Button btnReqReject;
    private Button btnReqDelete;
    private Button btnReqClear;

    private GroupBox grpReqFilter;
    private Label lblReqFilterEmp;
    private ComboBox cmbReqFilterEmp;
    private Label lblReqFilterStatus;
    private ComboBox cmbReqFilterStatus;
    private Button btnReqFilter;
    private Button btnReqClearFilter;
    private Label lblReqStatus;

    private DataGridView dgvRequests;

    // ── TAB 2: Leave Balances ──────────────────────────────────
    private GroupBox grpBalFilter;
    private Label lblBalYear;
    private NumericUpDown nudBalYear;
    private Label lblBalEmp;
    private ComboBox cmbBalEmp;
    private Button btnBalLoad;
    private Button btnBalInitYear;
    private Label lblBalStatus;
    private DataGridView dgvBalances;

    // ── TAB 3: Leave Types ─────────────────────────────────────
    private GroupBox grpTypeForm;
    private Label lblTypeName;
    private TextBox txtTypeName;
    private Label lblTypeDays;
    private NumericUpDown nudTypeDays;
    private Label lblTypeDesc;
    private TextBox txtTypeDesc;
    private Label lblTypeID;
    private TextBox txtTypeID;
    private Button btnTypeSave;
    private Button btnTypeUpdate;
    private Button btnTypeDelete;
    private Button btnTypeClear;
    private Label lblTypeStatus;
    private DataGridView dgvLeaveTypes;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var primaryBlue = Color.FromArgb(0, 120, 212);
        var lightBlue   = Color.FromArgb(240, 248, 255);

        // ── Header ─────────────────────────────────────────────
        pnlHeader  = new Panel();
        lblTitle   = new Label();
        lblSubtitle = new Label();

        pnlHeader.BackColor = primaryBlue;
        pnlHeader.Dock      = DockStyle.Top;
        pnlHeader.Size      = new Size(960, 70);
        pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblSubtitle });

        lblTitle.AutoSize  = true;
        lblTitle.Font      = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location  = new Point(20, 10);
        lblTitle.Text      = "Leave Management";

        lblSubtitle.AutoSize  = true;
        lblSubtitle.Font      = new Font("Segoe UI", 9F);
        lblSubtitle.ForeColor = Color.FromArgb(200, 230, 255);
        lblSubtitle.Location  = new Point(22, 42);
        lblSubtitle.Text      = "Leave requests, balances and leave types";

        // ── Tab control ────────────────────────────────────────
        tabMain = new TabControl();
        tabMain.Font     = new Font("Segoe UI", 9.5F);
        tabMain.Location = new Point(10, 80);
        tabMain.Size     = new Size(940, 580);

        tabRequests   = new TabPage("Leave Requests");
        tabBalances   = new TabPage("Leave Balances");
        tabLeaveTypes = new TabPage("Leave Types");
        tabMain.TabPages.AddRange(new[] { tabRequests, tabBalances, tabLeaveTypes });

        BuildRequestsTab(primaryBlue, lightBlue);
        BuildBalancesTab(primaryBlue);
        BuildLeaveTypesTab(primaryBlue, lightBlue);

        // ── Form ───────────────────────────────────────────────
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode       = AutoScaleMode.Font;
        BackColor           = Color.FromArgb(245, 247, 250);
        ClientSize          = new Size(960, 670);
        Controls.Add(tabMain);
        Controls.Add(pnlHeader);
        Font              = new Font("Segoe UI", 9F);
        FormBorderStyle   = FormBorderStyle.FixedSingle;
        MaximizeBox       = false;
        Name              = "frmLeaveManagement";
        StartPosition     = FormStartPosition.CenterScreen;
        Text              = "Leave Management";
        Load             += frmLeaveManagement_Load;
    }

    // ── Tab 1 builder ──────────────────────────────────────────
    private void BuildRequestsTab(Color primaryBlue, Color lightBlue)
    {
        // Form group
        grpRequestForm = new GroupBox();
        grpRequestForm.Text     = "Leave Request";
        grpRequestForm.Font     = new Font("Segoe UI", 9F, FontStyle.Bold);
        grpRequestForm.Location = new Point(8, 8);
        grpRequestForm.Size     = new Size(430, 300);

        lblReqID  = MakeLabel("ID:", new Point(10, 25));
        txtReqID  = MakeReadonlyBox(new Point(120, 22), 80);

        lblReqEmployee = MakeLabel("Employee:*", new Point(10, 55));
        cmbReqEmployee = MakeCombo(new Point(120, 52), 290);

        lblReqLeaveType = MakeLabel("Leave Type:*", new Point(10, 85));
        cmbReqLeaveType = MakeCombo(new Point(120, 82), 200);

        lblReqStart = MakeLabel("Start Date:*", new Point(10, 115));
        dtpReqStart = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(120, 112), Size = new Size(130, 23) };
        dtpReqStart.ValueChanged += dtpReq_ValueChanged;

        lblReqEnd = MakeLabel("End Date:*", new Point(10, 145));
        dtpReqEnd = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(120, 142), Size = new Size(130, 23) };
        dtpReqEnd.ValueChanged += dtpReq_ValueChanged;

        lblReqDays      = MakeLabel("Working Days:", new Point(10, 175));
        lblReqDaysValue = new Label { AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = primaryBlue, Location = new Point(120, 175), Text = "0" };

        lblReqReason = MakeLabel("Reason:", new Point(10, 205));
        txtReqReason = new TextBox { Location = new Point(120, 202), Multiline = true, Size = new Size(290, 55), ScrollBars = ScrollBars.Vertical };

        grpRequestForm.Controls.AddRange(new Control[]
        {
            lblReqID, txtReqID,
            lblReqEmployee, cmbReqEmployee,
            lblReqLeaveType, cmbReqLeaveType,
            lblReqStart, dtpReqStart,
            lblReqEnd, dtpReqEnd,
            lblReqDays, lblReqDaysValue,
            lblReqReason, txtReqReason
        });

        // Buttons
        btnReqSave    = MakeButton("Save",    new Point(8,  315), primaryBlue);
        btnReqApprove = MakeButton("Approve", new Point(108, 315), Color.FromArgb(16, 124, 65));
        btnReqReject  = MakeButton("Reject",  new Point(208, 315), Color.FromArgb(196, 43, 28));
        btnReqDelete  = MakeButton("Delete",  new Point(308, 315), Color.FromArgb(130, 130, 130));
        btnReqClear   = MakeButton("Clear",   new Point(408, 315), Color.FromArgb(100, 100, 100));

        btnReqSave.Click    += btnReqSave_Click;
        btnReqApprove.Click += btnReqApprove_Click;
        btnReqReject.Click  += btnReqReject_Click;
        btnReqDelete.Click  += btnReqDelete_Click;
        btnReqClear.Click   += btnReqClear_Click;

        // Filter group
        grpReqFilter = new GroupBox();
        grpReqFilter.Text     = "Filter";
        grpReqFilter.Font     = new Font("Segoe UI", 9F, FontStyle.Bold);
        grpReqFilter.Location = new Point(450, 8);
        grpReqFilter.Size     = new Size(470, 75);

        lblReqFilterEmp = MakeLabel("Employee:", new Point(10, 25));
        cmbReqFilterEmp = MakeCombo(new Point(100, 22), 170);

        lblReqFilterStatus = MakeLabel("Status:", new Point(285, 25));
        cmbReqFilterStatus = MakeCombo(new Point(345, 22), 110);
        cmbReqFilterStatus.Items.AddRange(new object[] { "All", "Pending", "Approved", "Rejected" });
        cmbReqFilterStatus.SelectedIndex = 0;

        btnReqFilter      = MakeButton("Filter",       new Point(10,  40), primaryBlue, 85);
        btnReqClearFilter = MakeButton("Clear Filter", new Point(105, 40), Color.FromArgb(100, 100, 100), 100);
        btnReqFilter.Click      += (s, e) => LoadRequests();
        btnReqClearFilter.Click += btnReqClearFilter_Click;

        grpReqFilter.Controls.AddRange(new Control[]
        {
            lblReqFilterEmp, cmbReqFilterEmp,
            lblReqFilterStatus, cmbReqFilterStatus,
            btnReqFilter, btnReqClearFilter
        });

        lblReqStatus = new Label { AutoSize = true, Font = new Font("Segoe UI", 8.5F), ForeColor = Color.FromArgb(80, 80, 80), Location = new Point(450, 90), Text = "Records: 0" };

        dgvRequests = MakeGrid(new Point(450, 110), new Size(470, 410));
        dgvRequests.CellClick += dgvRequests_CellClick;

        tabRequests.Controls.AddRange(new Control[]
        {
            grpRequestForm,
            btnReqSave, btnReqApprove, btnReqReject, btnReqDelete, btnReqClear,
            grpReqFilter, lblReqStatus, dgvRequests
        });
    }

    // ── Tab 2 builder ──────────────────────────────────────────
    private void BuildBalancesTab(Color primaryBlue)
    {
        grpBalFilter = new GroupBox();
        grpBalFilter.Text     = "View / Initialise Balances";
        grpBalFilter.Font     = new Font("Segoe UI", 9F, FontStyle.Bold);
        grpBalFilter.Location = new Point(8, 8);
        grpBalFilter.Size     = new Size(910, 70);

        lblBalYear = MakeLabel("Year:", new Point(10, 30));
        nudBalYear = new NumericUpDown { Location = new Point(80, 27), Size = new Size(75, 23), Minimum = 2000, Maximum = 2100, Value = DateTime.Today.Year };

        lblBalEmp   = MakeLabel("Employee:", new Point(175, 30));
        cmbBalEmp   = MakeCombo(new Point(265, 27), 220);

        btnBalLoad     = MakeButton("Load",          new Point(505, 27), primaryBlue, 100);
        btnBalInitYear = MakeButton("Init Year Balances", new Point(615, 27), Color.FromArgb(16, 124, 65), 155);
        btnBalLoad.Click     += (s, e) => LoadBalances();
        btnBalInitYear.Click += btnBalInitYear_Click;

        grpBalFilter.Controls.AddRange(new Control[]
        {
            lblBalYear, nudBalYear, lblBalEmp, cmbBalEmp,
            btnBalLoad, btnBalInitYear
        });

        lblBalStatus = new Label { AutoSize = true, Font = new Font("Segoe UI", 8.5F), ForeColor = Color.FromArgb(80, 80, 80), Location = new Point(10, 85), Text = "" };

        dgvBalances = MakeGrid(new Point(8, 105), new Size(910, 400));
        dgvBalances.ReadOnly  = false;
        dgvBalances.EditMode  = DataGridViewEditMode.EditOnKeystrokeOrF2;
        dgvBalances.CellEndEdit += dgvBalances_CellEndEdit;

        tabBalances.Controls.AddRange(new Control[] { grpBalFilter, lblBalStatus, dgvBalances });
    }

    // ── Tab 3 builder ──────────────────────────────────────────
    private void BuildLeaveTypesTab(Color primaryBlue, Color lightBlue)
    {
        grpTypeForm = new GroupBox();
        grpTypeForm.Text     = "Leave Type";
        grpTypeForm.Font     = new Font("Segoe UI", 9F, FontStyle.Bold);
        grpTypeForm.Location = new Point(8, 8);
        grpTypeForm.Size     = new Size(350, 200);

        lblTypeID  = MakeLabel("ID:", new Point(10, 25));
        txtTypeID  = MakeReadonlyBox(new Point(140, 22), 60);

        lblTypeName = MakeLabel("Type Name:*", new Point(10, 55));
        txtTypeName = new TextBox { Location = new Point(140, 52), Size = new Size(190, 23) };

        lblTypeDays = MakeLabel("Days / Year:*", new Point(10, 85));
        nudTypeDays = new NumericUpDown { Location = new Point(140, 82), Size = new Size(75, 23), Minimum = 1, Maximum = 365, Value = 14 };

        lblTypeDesc = MakeLabel("Description:", new Point(10, 115));
        txtTypeDesc = new TextBox { Location = new Point(140, 112), Multiline = true, Size = new Size(190, 55), ScrollBars = ScrollBars.Vertical };

        grpTypeForm.Controls.AddRange(new Control[]
        {
            lblTypeID, txtTypeID,
            lblTypeName, txtTypeName,
            lblTypeDays, nudTypeDays,
            lblTypeDesc, txtTypeDesc
        });

        btnTypeSave   = MakeButton("Save",   new Point(8,  215), primaryBlue);
        btnTypeUpdate = MakeButton("Update", new Point(108, 215), Color.FromArgb(0, 100, 180));
        btnTypeDelete = MakeButton("Delete", new Point(208, 215), Color.FromArgb(196, 43, 28));
        btnTypeClear  = MakeButton("Clear",  new Point(308, 215), Color.FromArgb(100, 100, 100));

        btnTypeSave.Click   += btnTypeSave_Click;
        btnTypeUpdate.Click += btnTypeUpdate_Click;
        btnTypeDelete.Click += btnTypeDelete_Click;
        btnTypeClear.Click  += (s, e) => ClearTypeForm();

        lblTypeStatus = new Label { AutoSize = true, Font = new Font("Segoe UI", 8.5F), ForeColor = Color.FromArgb(80, 80, 80), Location = new Point(370, 15), Text = "Records: 0" };

        dgvLeaveTypes = MakeGrid(new Point(370, 35), new Size(550, 490));
        dgvLeaveTypes.CellClick += dgvLeaveTypes_CellClick;

        tabLeaveTypes.Controls.AddRange(new Control[]
        {
            grpTypeForm,
            btnTypeSave, btnTypeUpdate, btnTypeDelete, btnTypeClear,
            lblTypeStatus, dgvLeaveTypes
        });
    }

    // ── UI helpers ─────────────────────────────────────────────
    private static Label MakeLabel(string text, Point loc) =>
        new() { AutoSize = true, Text = text, Location = loc };

    private static TextBox MakeReadonlyBox(Point loc, int width) =>
        new() { ReadOnly = true, BackColor = Color.FromArgb(240, 240, 240), Location = loc, Size = new Size(width, 23) };

    private static ComboBox MakeCombo(Point loc, int width) =>
        new() { DropDownStyle = ComboBoxStyle.DropDownList, Location = loc, Size = new Size(width, 23) };

    private static Button MakeButton(string text, Point loc, Color back, int width = 90) =>
        new()
        {
            Text = text, Location = loc, Size = new Size(width, 30),
            BackColor = back, ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold)
        };

    private static DataGridView MakeGrid(Point loc, Size size)
    {
        var dgv = new DataGridView
        {
            Location              = loc,
            Size                  = size,
            ReadOnly              = true,
            AllowUserToAddRows    = false,
            AllowUserToDeleteRows = false,
            MultiSelect           = false,
            SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor       = Color.White,
            BorderStyle           = BorderStyle.Fixed3D,
            RowHeadersVisible     = false,
            Font                  = new Font("Segoe UI", 8.5F)
        };
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 212);
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgv.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        dgv.EnableHeadersVisualStyles = false;
        return dgv;
    }
}
