using System.Drawing;
using System.Windows.Forms;

namespace PayrollManagementSystem.Forms;

partial class frmPaySlip
{
    private System.ComponentModel.IContainer components = null;

    private Panel        pnlToolbar;
    private Button       btnPrint;
    private Button       btnPreview;
    private Button       btnClose;
    private Label        lblPreviewTitle;
    private Panel        pnlContainer;
    private Panel        pnlSlip;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var blue    = Color.FromArgb(0, 120, 212);
        var lightBg = Color.FromArgb(232, 235, 240);

        pnlToolbar      = new Panel();
        btnPrint        = new Button();
        btnPreview      = new Button();
        btnClose        = new Button();
        lblPreviewTitle = new Label();
        pnlContainer    = new Panel();
        pnlSlip         = new Panel();

        SuspendLayout();
        pnlToolbar.SuspendLayout();
        pnlContainer.SuspendLayout();

        // ── Toolbar ────────────────────────────────────────────
        pnlToolbar.BackColor   = Color.FromArgb(245, 247, 250);
        pnlToolbar.BorderStyle = BorderStyle.FixedSingle;
        pnlToolbar.Dock        = DockStyle.Top;
        pnlToolbar.Height      = 52;
        pnlToolbar.Controls.Add(btnPrint);
        pnlToolbar.Controls.Add(btnPreview);
        pnlToolbar.Controls.Add(btnClose);
        pnlToolbar.Controls.Add(lblPreviewTitle);

        SetBtn(btnPrint,   "Print",         12,  12, 85,  28, blue,                          Color.White);
        SetBtn(btnPreview, "Print Preview", 105, 12, 120, 28, Color.FromArgb(60, 120, 180),  Color.White);
        SetBtn(btnClose,   "Close",         234, 12, 75,  28, Color.FromArgb(100, 100, 100), Color.White);
        btnPrint.Click   += new EventHandler(btnPrint_Click);
        btnPreview.Click += new EventHandler(btnPreview_Click);
        btnClose.Click   += new EventHandler(btnClose_Click);

        lblPreviewTitle.AutoSize  = true;
        lblPreviewTitle.Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblPreviewTitle.ForeColor = Color.FromArgb(100, 100, 100);
        lblPreviewTitle.Location  = new Point(328, 17);
        lblPreviewTitle.Text      = "Pay Slip Preview";

        // ── Container ──────────────────────────────────────────
        pnlContainer.BackColor  = lightBg;
        pnlContainer.Dock       = DockStyle.Fill;
        pnlContainer.AutoScroll = true;
        pnlContainer.Controls.Add(pnlSlip);

        // ── Slip panel (white "paper") ─────────────────────────
        pnlSlip.BackColor = Color.White;
        pnlSlip.Location  = new Point(30, 20);
        pnlSlip.Size      = new Size(618, 448);
        pnlSlip.Paint    += new PaintEventHandler(pnlSlip_Paint);

        // ── Form ───────────────────────────────────────────────
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode       = AutoScaleMode.Font;
        BackColor           = lightBg;
        ClientSize          = new Size(678, 540);
        Controls.Add(pnlContainer);
        Controls.Add(pnlToolbar);
        Font            = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        Name            = "frmPaySlip";
        StartPosition   = FormStartPosition.CenterScreen;
        Text            = "Pay Slip";

        pnlToolbar.ResumeLayout(false);
        pnlToolbar.PerformLayout();
        pnlContainer.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
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
}
