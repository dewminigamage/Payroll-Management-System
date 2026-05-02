using System.Drawing;
using System.Windows.Forms;

namespace PayrollManagementSystem.Forms;

partial class frmUserManagement
{
    private System.ComponentModel.IContainer components = null;

    // Header
    private Panel  pnlHeader;
    private Label  lblTitle;
    private Label  lblSubtitle;

    // Left entry panel
    private Panel  pnlEntry;
    private Label  lblEntryTitle;
    private Label  lblUserID;
    private TextBox txtUserID;
    private Label  lblUsername;
    private TextBox txtUsername;
    private Label  lblFullName;
    private TextBox txtFullName;
    private Label  lblRole;
    private ComboBox cboRole;
    private Label  lblIsActive;
    private CheckBox chkIsActive;
    private Label  lblPassword;
    private TextBox txtPassword;
    private Label  lblConfirmPassword;
    private TextBox txtConfirmPassword;
    private Label  lblPwNote;
    private Button btnAdd;
    private Button btnUpdate;
    private Button btnDelete;
    private Button btnClear;

    // Right grid panel
    private Panel  pnlGrid;
    private Label  lblGridTitle;
    private DataGridView dgvUsers;
    private Label  lblStatus;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var blue     = Color.FromArgb(0, 120, 212);
        var lightBg  = Color.FromArgb(245, 247, 250);
        var panelBg  = Color.FromArgb(252, 253, 255);
        var borderCl = Color.FromArgb(210, 218, 230);

        // ── instantiate ─────────────────────────────────────────
        pnlHeader        = new Panel();
        lblTitle         = new Label();
        lblSubtitle      = new Label();
        pnlEntry         = new Panel();
        lblEntryTitle    = new Label();
        lblUserID        = new Label();
        txtUserID        = new TextBox();
        lblUsername      = new Label();
        txtUsername      = new TextBox();
        lblFullName      = new Label();
        txtFullName      = new TextBox();
        lblRole          = new Label();
        cboRole          = new ComboBox();
        lblIsActive      = new Label();
        chkIsActive      = new CheckBox();
        lblPassword      = new Label();
        txtPassword      = new TextBox();
        lblConfirmPassword = new Label();
        txtConfirmPassword = new TextBox();
        lblPwNote        = new Label();
        btnAdd           = new Button();
        btnUpdate        = new Button();
        btnDelete        = new Button();
        btnClear         = new Button();
        pnlGrid          = new Panel();
        lblGridTitle     = new Label();
        dgvUsers         = new DataGridView();
        lblStatus        = new Label();

        SuspendLayout();
        pnlHeader.SuspendLayout();
        pnlEntry.SuspendLayout();
        pnlGrid.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvUsers).BeginInit();

        // ── Header ───────────────────────────────────────────────
        pnlHeader.BackColor = blue;
        pnlHeader.Dock      = DockStyle.Top;
        pnlHeader.Height    = 72;
        pnlHeader.Controls.Add(lblSubtitle);
        pnlHeader.Controls.Add(lblTitle);

        lblTitle.AutoSize  = true;
        lblTitle.Font      = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location  = new Point(20, 10);
        lblTitle.Text      = "User Management";

        lblSubtitle.AutoSize  = true;
        lblSubtitle.Font      = new Font("Segoe UI", 9F);
        lblSubtitle.ForeColor = Color.FromArgb(200, 230, 255);
        lblSubtitle.Location  = new Point(22, 44);
        lblSubtitle.Text      = "Add, edit and deactivate system users";

        // ── Entry panel (left) ───────────────────────────────────
        pnlEntry.BackColor   = panelBg;
        pnlEntry.BorderStyle = BorderStyle.FixedSingle;
        pnlEntry.Location    = new Point(12, 82);
        pnlEntry.Size        = new Size(350, 508);

        BuildLabel(lblEntryTitle, "USER DETAILS", new Font("Segoe UI", 9.5F, FontStyle.Bold),
            Color.FromArgb(0, 120, 212), new Point(14, 12));

        // UserID
        BuildLabel(lblUserID, "User ID", new Font("Segoe UI", 8.5F, FontStyle.Bold), Color.FromArgb(70, 70, 80), new Point(14, 38));
        txtUserID.Location  = new Point(14, 54);
        txtUserID.Size      = new Size(318, 23);
        txtUserID.ReadOnly  = true;
        txtUserID.BackColor = Color.FromArgb(235, 238, 242);
        txtUserID.TabStop   = false;

        // Username
        BuildLabel(lblUsername, "Username *", new Font("Segoe UI", 8.5F, FontStyle.Bold), Color.FromArgb(70, 70, 80), new Point(14, 82));
        txtUsername.Location    = new Point(14, 98);
        txtUsername.Size        = new Size(318, 23);
        txtUsername.MaxLength   = 50;

        // Full Name
        BuildLabel(lblFullName, "Full Name *", new Font("Segoe UI", 8.5F, FontStyle.Bold), Color.FromArgb(70, 70, 80), new Point(14, 126));
        txtFullName.Location    = new Point(14, 142);
        txtFullName.Size        = new Size(318, 23);
        txtFullName.MaxLength   = 100;

        // Role
        BuildLabel(lblRole, "Role *", new Font("Segoe UI", 8.5F, FontStyle.Bold), Color.FromArgb(70, 70, 80), new Point(14, 170));
        cboRole.DropDownStyle = ComboBoxStyle.DropDownList;
        cboRole.Location      = new Point(14, 186);
        cboRole.Size          = new Size(318, 23);
        cboRole.Items.AddRange(new object[] { "Admin", "Viewer" });

        // IsActive
        BuildLabel(lblIsActive, "Status", new Font("Segoe UI", 8.5F, FontStyle.Bold), Color.FromArgb(70, 70, 80), new Point(14, 214));
        chkIsActive.AutoSize  = true;
        chkIsActive.Checked   = true;
        chkIsActive.Font      = new Font("Segoe UI", 9F);
        chkIsActive.ForeColor = Color.FromArgb(40, 40, 40);
        chkIsActive.Location  = new Point(14, 232);
        chkIsActive.Text      = "Active";

        // Password
        BuildLabel(lblPassword, "Password", new Font("Segoe UI", 8.5F, FontStyle.Bold), Color.FromArgb(70, 70, 80), new Point(14, 258));
        txtPassword.Location     = new Point(14, 274);
        txtPassword.Size         = new Size(318, 23);
        txtPassword.PasswordChar = '●';
        txtPassword.MaxLength    = 128;

        // Confirm Password
        BuildLabel(lblConfirmPassword, "Confirm Password", new Font("Segoe UI", 8.5F, FontStyle.Bold), Color.FromArgb(70, 70, 80), new Point(14, 302));
        txtConfirmPassword.Location     = new Point(14, 318);
        txtConfirmPassword.Size         = new Size(318, 23);
        txtConfirmPassword.PasswordChar = '●';
        txtConfirmPassword.MaxLength    = 128;

        // Password note
        lblPwNote.AutoSize  = true;
        lblPwNote.Font      = new Font("Segoe UI", 7.5F, FontStyle.Italic);
        lblPwNote.ForeColor = Color.FromArgb(130, 130, 140);
        lblPwNote.Location  = new Point(14, 346);
        lblPwNote.Text      = "* Required for Add. Leave blank to keep current password (Update).";

        // Buttons
        SetBtn(btnAdd,    "Add User",    new Point(14,  372), new Size(148, 34), blue,                          Color.White);
        SetBtn(btnUpdate, "Update",      new Point(168, 372), new Size(164, 34), Color.FromArgb(0, 140, 80),    Color.White);
        SetBtn(btnDelete, "Delete",      new Point(14,  412), new Size(148, 34), Color.FromArgb(180, 40, 30),   Color.White);
        SetBtn(btnClear,  "Clear",       new Point(168, 412), new Size(164, 34), Color.FromArgb(100, 100, 100), Color.White);

        btnAdd.Click    += new EventHandler(btnAdd_Click);
        btnUpdate.Click += new EventHandler(btnUpdate_Click);
        btnDelete.Click += new EventHandler(btnDelete_Click);
        btnClear.Click  += new EventHandler(btnClear_Click);

        pnlEntry.Controls.AddRange(new Control[] {
            lblEntryTitle,
            lblUserID, txtUserID,
            lblUsername, txtUsername,
            lblFullName, txtFullName,
            lblRole, cboRole,
            lblIsActive, chkIsActive,
            lblPassword, txtPassword,
            lblConfirmPassword, txtConfirmPassword,
            lblPwNote,
            btnAdd, btnUpdate, btnDelete, btnClear
        });

        // ── Grid panel (right) ───────────────────────────────────
        pnlGrid.BackColor   = panelBg;
        pnlGrid.BorderStyle = BorderStyle.FixedSingle;
        pnlGrid.Location    = new Point(374, 82);
        pnlGrid.Size        = new Size(534, 508);

        BuildLabel(lblGridTitle, "USERS", new Font("Segoe UI", 9.5F, FontStyle.Bold),
            Color.FromArgb(0, 120, 212), new Point(10, 12));

        // DataGridView
        dgvUsers.AllowUserToAddRows    = false;
        dgvUsers.AllowUserToDeleteRows = false;
        dgvUsers.BackgroundColor       = Color.White;
        dgvUsers.BorderStyle           = BorderStyle.None;
        dgvUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvUsers.Location              = new Point(10, 36);
        dgvUsers.MultiSelect           = false;
        dgvUsers.ReadOnly              = true;
        dgvUsers.RowHeadersVisible     = false;
        dgvUsers.SelectionMode         = DataGridViewSelectionMode.FullRowSelect;
        dgvUsers.Size                  = new Size(512, 450);
        dgvUsers.GridColor             = borderCl;
        dgvUsers.DefaultCellStyle.Font = new Font("Segoe UI", 8.5F);
        dgvUsers.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 238, 248);
        dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(30, 30, 30);
        dgvUsers.EnableHeadersVisualStyles = false;
        dgvUsers.AutoSizeColumnsMode       = DataGridViewAutoSizeColumnsMode.Fill;
        dgvUsers.CellClick += new DataGridViewCellEventHandler(dgvUsers_CellClick);

        // Status bar
        lblStatus.AutoSize  = false;
        lblStatus.Dock      = DockStyle.Bottom;
        lblStatus.Font      = new Font("Segoe UI", 8.5F);
        lblStatus.ForeColor = Color.FromArgb(80, 80, 80);
        lblStatus.Height    = 22;
        lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        lblStatus.Padding   = new Padding(6, 0, 0, 0);

        pnlGrid.Controls.AddRange(new Control[] { lblGridTitle, dgvUsers, lblStatus });

        // ── Form ─────────────────────────────────────────────────
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode       = AutoScaleMode.Font;
        BackColor           = lightBg;
        ClientSize          = new Size(920, 602);
        Controls.Add(pnlGrid);
        Controls.Add(pnlEntry);
        Controls.Add(pnlHeader);
        Font            = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        Name            = "frmUserManagement";
        StartPosition   = FormStartPosition.CenterScreen;
        Text            = "User Management";

        ((System.ComponentModel.ISupportInitialize)dgvUsers).EndInit();
        pnlGrid.ResumeLayout(false);
        pnlGrid.PerformLayout();
        pnlEntry.ResumeLayout(false);
        pnlEntry.PerformLayout();
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private static void BuildLabel(Label lbl, string text, Font font, Color fore, Point loc)
    {
        lbl.AutoSize  = true;
        lbl.Font      = font;
        lbl.ForeColor = fore;
        lbl.Location  = loc;
        lbl.Text      = text;
    }

    private static void SetBtn(Button btn, string text, Point loc, Size sz, Color bg, Color fg)
    {
        btn.BackColor                 = bg;
        btn.FlatAppearance.BorderSize = 0;
        btn.FlatStyle                 = FlatStyle.Flat;
        btn.Font                      = new Font("Segoe UI", 9F, FontStyle.Bold);
        btn.ForeColor                 = fg;
        btn.Location                  = loc;
        btn.Size                      = sz;
        btn.Text                      = text;
        btn.UseVisualStyleBackColor   = false;
        btn.Cursor                    = Cursors.Hand;
    }
}
