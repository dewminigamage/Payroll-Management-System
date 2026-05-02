using System.Drawing;
using System.Windows.Forms;

namespace PayrollManagementSystem.Forms;

partial class frmLogin
{
    private System.ComponentModel.IContainer components = null;

    private Panel   pnlHeader;
    private Label   lblAppTitle;
    private Label   lblLoginSub;

    private Label   lblUsername;
    private TextBox txtUsername;
    private Label   lblPassword;
    private TextBox txtPassword;
    private Button  btnLogin;
    private Label   lblError;
    private Label   lblHint;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var blue = Color.FromArgb(0, 120, 212);

        pnlHeader  = new Panel();
        lblAppTitle = new Label();
        lblLoginSub = new Label();
        lblUsername = new Label();
        txtUsername = new TextBox();
        lblPassword = new Label();
        txtPassword = new TextBox();
        btnLogin    = new Button();
        lblError    = new Label();
        lblHint     = new Label();

        SuspendLayout();
        pnlHeader.SuspendLayout();

        // ── Header ─────────────────────────────────────────────
        pnlHeader.BackColor = blue;
        pnlHeader.Controls.Add(lblLoginSub);
        pnlHeader.Controls.Add(lblAppTitle);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Size = new Size(420, 80);

        lblAppTitle.AutoSize  = true;
        lblAppTitle.Font      = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblAppTitle.ForeColor = Color.White;
        lblAppTitle.Location  = new Point(20, 13);
        lblAppTitle.Text      = "Payroll Management System";

        lblLoginSub.AutoSize  = true;
        lblLoginSub.Font      = new Font("Segoe UI", 9.5F);
        lblLoginSub.ForeColor = Color.FromArgb(200, 230, 255);
        lblLoginSub.Location  = new Point(22, 50);
        lblLoginSub.Text      = "Sign in to continue";

        // ── Username ───────────────────────────────────────────
        lblUsername.AutoSize  = true;
        lblUsername.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblUsername.ForeColor = Color.FromArgb(60, 60, 60);
        lblUsername.Location  = new Point(25, 100);
        lblUsername.Text      = "Username";

        txtUsername.Font     = new Font("Segoe UI", 10.5F);
        txtUsername.Location = new Point(25, 118);
        txtUsername.Size     = new Size(370, 30);
        txtUsername.KeyPress += new KeyPressEventHandler(txtUsername_KeyPress);

        // ── Password ───────────────────────────────────────────
        lblPassword.AutoSize  = true;
        lblPassword.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPassword.ForeColor = Color.FromArgb(60, 60, 60);
        lblPassword.Location  = new Point(25, 162);
        lblPassword.Text      = "Password";

        txtPassword.Font         = new Font("Segoe UI", 10.5F);
        txtPassword.Location     = new Point(25, 180);
        txtPassword.Size         = new Size(370, 30);
        txtPassword.PasswordChar = '●';

        // ── Login button ───────────────────────────────────────
        btnLogin.BackColor                 = blue;
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.FlatStyle                 = FlatStyle.Flat;
        btnLogin.Font                      = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnLogin.ForeColor                 = Color.White;
        btnLogin.Location                  = new Point(25, 228);
        btnLogin.Size                      = new Size(370, 42);
        btnLogin.Text                      = "Sign In";
        btnLogin.UseVisualStyleBackColor   = false;
        btnLogin.Cursor                    = Cursors.Hand;
        btnLogin.Click                    += new EventHandler(btnLogin_Click);

        // ── Error label ────────────────────────────────────────
        lblError.AutoSize  = false;
        lblError.Font      = new Font("Segoe UI", 9F);
        lblError.ForeColor = Color.FromArgb(196, 43, 28);
        lblError.Location  = new Point(25, 280);
        lblError.Size      = new Size(370, 18);
        lblError.Text      = "";
        lblError.TextAlign = ContentAlignment.MiddleCenter;

        // ── Hint ───────────────────────────────────────────────
        lblHint.AutoSize  = false;
        lblHint.Font      = new Font("Segoe UI", 8F);
        lblHint.ForeColor = Color.FromArgb(170, 170, 170);
        lblHint.Location  = new Point(25, 308);
        lblHint.Size      = new Size(370, 16);
        lblHint.Text      = "Default account:   admin  /  admin123";
        lblHint.TextAlign = ContentAlignment.MiddleCenter;

        // ── Form ───────────────────────────────────────────────
        AcceptButton        = btnLogin;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode       = AutoScaleMode.Font;
        BackColor           = Color.White;
        ClientSize          = new Size(420, 338);
        Controls.Add(lblHint);
        Controls.Add(lblError);
        Controls.Add(btnLogin);
        Controls.Add(txtPassword);
        Controls.Add(lblPassword);
        Controls.Add(txtUsername);
        Controls.Add(lblUsername);
        Controls.Add(pnlHeader);
        Font            = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        MinimizeBox     = false;
        Name            = "frmLogin";
        StartPosition   = FormStartPosition.CenterScreen;
        Text            = "Login — Payroll Management System";

        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
