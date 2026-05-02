using System.Drawing;
using System.Windows.Forms;

namespace PayrollManagementSystem.Forms;

partial class frmAIAssistant
{
    private System.ComponentModel.IContainer components = null;

    private System.Windows.Forms.Panel      pnlHeader;
    private System.Windows.Forms.Label      lblTitle;
    private System.Windows.Forms.Label      lblSubtitle;
    private System.Windows.Forms.Button     btnConfigureKey;
    private System.Windows.Forms.Panel      pnlBar;
    private System.Windows.Forms.Label      lblStatus;
    private System.Windows.Forms.Button     btnLoadContext;
    private System.Windows.Forms.Button     btnClearChat;
    private System.Windows.Forms.RichTextBox rtbChat;
    private System.Windows.Forms.Panel      pnlInput;
    private System.Windows.Forms.TextBox    txtInput;
    private System.Windows.Forms.Button     btnSend;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var primaryBlue = Color.FromArgb(0, 120, 212);

        pnlHeader       = new Panel();
        lblTitle        = new Label();
        lblSubtitle     = new Label();
        btnConfigureKey = new Button();
        pnlBar          = new Panel();
        lblStatus       = new Label();
        btnLoadContext  = new Button();
        btnClearChat    = new Button();
        rtbChat         = new RichTextBox();
        pnlInput        = new Panel();
        txtInput        = new TextBox();
        btnSend         = new Button();

        SuspendLayout();
        pnlHeader.SuspendLayout();
        pnlBar.SuspendLayout();
        pnlInput.SuspendLayout();

        // ── Header ───────────────────────────────────────────────
        pnlHeader.BackColor = primaryBlue;
        pnlHeader.Controls.AddRange(new Control[] { lblSubtitle, lblTitle, btnConfigureKey });
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Size = new Size(880, 70);

        lblTitle.AutoSize  = true;
        lblTitle.Font      = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location  = new Point(25, 12);
        lblTitle.Text      = "AI Payroll Assistant";

        lblSubtitle.AutoSize  = true;
        lblSubtitle.Font      = new Font("Segoe UI", 9.5F);
        lblSubtitle.ForeColor = Color.FromArgb(200, 230, 255);
        lblSubtitle.Location  = new Point(27, 48);
        lblSubtitle.Text      = "Ask questions about your workforce and payroll data";

        btnConfigureKey.BackColor                  = Color.FromArgb(0, 95, 185);
        btnConfigureKey.FlatAppearance.BorderColor = Color.FromArgb(180, 220, 255);
        btnConfigureKey.FlatAppearance.BorderSize  = 1;
        btnConfigureKey.FlatStyle                  = FlatStyle.Flat;
        btnConfigureKey.Font                       = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        btnConfigureKey.ForeColor                  = Color.White;
        btnConfigureKey.Location                   = new Point(748, 20);
        btnConfigureKey.Size                       = new Size(110, 28);
        btnConfigureKey.Text                       = "Configure Key";
        btnConfigureKey.UseVisualStyleBackColor    = false;
        btnConfigureKey.Cursor                     = Cursors.Hand;
        btnConfigureKey.Click += new System.EventHandler(btnConfigureKey_Click);

        // ── Toolbar bar ──────────────────────────────────────────
        pnlBar.BackColor = Color.FromArgb(240, 242, 246);
        pnlBar.Controls.AddRange(new Control[] { lblStatus, btnLoadContext, btnClearChat });
        pnlBar.Location = new Point(0, 70);
        pnlBar.Size     = new Size(880, 36);

        lblStatus.AutoSize  = false;
        lblStatus.Font      = new Font("Segoe UI", 8.5F, FontStyle.Italic);
        lblStatus.ForeColor = Color.FromArgb(120, 120, 120);
        lblStatus.Location  = new Point(10, 10);
        lblStatus.Size      = new Size(620, 16);
        lblStatus.Text      = "No context loaded. Click 'Load Context' to pull live data.";

        btnLoadContext.BackColor                 = Color.FromArgb(0, 120, 212);
        btnLoadContext.FlatAppearance.BorderSize = 0;
        btnLoadContext.FlatStyle                 = FlatStyle.Flat;
        btnLoadContext.Font                      = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        btnLoadContext.ForeColor                 = Color.White;
        btnLoadContext.Location                  = new Point(638, 5);
        btnLoadContext.Size                      = new Size(108, 26);
        btnLoadContext.Text                      = "Load Context";
        btnLoadContext.UseVisualStyleBackColor   = false;
        btnLoadContext.Cursor                    = Cursors.Hand;
        btnLoadContext.Click += new System.EventHandler(btnLoadContext_Click);

        btnClearChat.BackColor                 = Color.FromArgb(200, 70, 50);
        btnClearChat.FlatAppearance.BorderSize = 0;
        btnClearChat.FlatStyle                 = FlatStyle.Flat;
        btnClearChat.Font                      = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        btnClearChat.ForeColor                 = Color.White;
        btnClearChat.Location                  = new Point(752, 5);
        btnClearChat.Size                      = new Size(108, 26);
        btnClearChat.Text                      = "Clear Chat";
        btnClearChat.UseVisualStyleBackColor   = false;
        btnClearChat.Cursor                    = Cursors.Hand;
        btnClearChat.Click += new System.EventHandler(btnClearChat_Click);

        // ── Chat history ──────────────────────────────────────────
        rtbChat.BackColor   = Color.White;
        rtbChat.BorderStyle = BorderStyle.FixedSingle;
        rtbChat.Font        = new Font("Segoe UI", 9.5F);
        rtbChat.Location    = new Point(10, 116);
        rtbChat.ReadOnly    = true;
        rtbChat.ScrollBars  = RichTextBoxScrollBars.Vertical;
        rtbChat.Size        = new Size(860, 468);
        rtbChat.TabStop     = false;

        // ── Input panel ───────────────────────────────────────────
        pnlInput.BackColor   = Color.White;
        pnlInput.BorderStyle = BorderStyle.FixedSingle;
        pnlInput.Controls.AddRange(new Control[] { txtInput, btnSend });
        pnlInput.Location    = new Point(0, 594);
        pnlInput.Size        = new Size(880, 64);

        txtInput.Font       = new Font("Segoe UI", 10F);
        txtInput.Location   = new Point(10, 12);
        txtInput.Multiline  = true;
        txtInput.ScrollBars = ScrollBars.Vertical;
        txtInput.Size       = new Size(730, 40);
        txtInput.KeyDown   += new System.Windows.Forms.KeyEventHandler(txtInput_KeyDown);

        btnSend.BackColor                 = Color.FromArgb(0, 120, 212);
        btnSend.FlatAppearance.BorderSize = 0;
        btnSend.FlatStyle                 = FlatStyle.Flat;
        btnSend.Font                      = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnSend.ForeColor                 = Color.White;
        btnSend.Location                  = new Point(752, 10);
        btnSend.Size                      = new Size(110, 44);
        btnSend.Text                      = "Send";
        btnSend.UseVisualStyleBackColor   = false;
        btnSend.Cursor                    = Cursors.Hand;
        btnSend.Click += new System.EventHandler(btnSend_Click);

        // ── Form ──────────────────────────────────────────────────
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode       = AutoScaleMode.Font;
        BackColor           = Color.FromArgb(245, 247, 250);
        ClientSize          = new Size(880, 662);
        Controls.AddRange(new Control[] { rtbChat, pnlBar, pnlInput, pnlHeader });
        Font            = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        Name            = "frmAIAssistant";
        StartPosition   = FormStartPosition.CenterScreen;
        Text            = "AI Payroll Assistant";

        pnlInput.ResumeLayout(false);
        pnlInput.PerformLayout();
        pnlBar.ResumeLayout(false);
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
