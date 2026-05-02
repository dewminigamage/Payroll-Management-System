using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing.Text;
using Microsoft.Data.SqlClient;
using PayrollManagementSystem.Database;

namespace PayrollManagementSystem.Forms;

public partial class frmPaySlip : Form
{
    // ── Pay slip data ──────────────────────────────────────────────────────
    private int     _employeeID;
    private string  _employeeName    = "";
    private string  _department      = "";
    private string  _position        = "";
    private string  _payPeriod       = "";
    private decimal _basicSalary;
    private decimal _allowances;
    private decimal _grossSalary;
    private decimal _epf;
    private decimal _etf;
    private decimal _tax;
    private decimal _otherDeductions;
    private decimal _totalDeductions;
    private decimal _netSalary;
    private string  _remarks         = "";

    private readonly PrintDocument _pd = new();

    public frmPaySlip(int payrollID)
    {
        InitializeComponent();
        _pd.PrintPage += PrintPage;
        LoadPaySlip(payrollID);
    }

    // ── Load from DB ───────────────────────────────────────────────────────

    private void LoadPaySlip(int payrollID)
    {
        var dt = DatabaseHelper.ExecuteQuery(@"
            SELECT e.EmployeeID, e.FullName, e.Department, e.Position,
                   p.PayMonth, p.PayYear,
                   p.BasicSalary, p.Allowances, p.GrossSalary,
                   p.EPF, p.ETF, p.Tax, p.OtherDeductions, p.NetSalary, p.Remarks
            FROM   PayrollRecords p
            INNER JOIN Employees e ON p.EmployeeID = e.EmployeeID
            WHERE  p.PayrollID = @PayrollID",
            [new SqlParameter("@PayrollID", payrollID)]);

        if (dt.Rows.Count == 0)
        {
            MessageBox.Show("Payroll record not found.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
            return;
        }

        var r = dt.Rows[0];
        _employeeID      = Convert.ToInt32(r["EmployeeID"]);
        _employeeName    = r["FullName"].ToString()!;
        _department      = r["Department"].ToString() ?? "";
        _position        = r["Position"].ToString() ?? "";
        _basicSalary     = Convert.ToDecimal(r["BasicSalary"]);
        _allowances      = Convert.ToDecimal(r["Allowances"]);
        _grossSalary     = Convert.ToDecimal(r["GrossSalary"]);
        _epf             = Convert.ToDecimal(r["EPF"]);
        _etf             = Convert.ToDecimal(r["ETF"]);
        _tax             = Convert.ToDecimal(r["Tax"]);
        _otherDeductions = Convert.ToDecimal(r["OtherDeductions"]);
        _netSalary       = Convert.ToDecimal(r["NetSalary"]);
        _totalDeductions = _epf + _tax + _otherDeductions;
        _remarks         = r["Remarks"]?.ToString() ?? "";

        string[] months = { "January","February","March","April","May","June",
                            "July","August","September","October","November","December" };
        int m = Convert.ToInt32(r["PayMonth"]);
        int y = Convert.ToInt32(r["PayYear"]);
        _payPeriod = $"{months[m - 1]} {y}";

        Text = $"Pay Slip — {_employeeName} — {_payPeriod}";
    }

    // ── Panel paint (screen preview) ───────────────────────────────────────

    private void pnlSlip_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode     = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        DrawPaySlip(e.Graphics, new RectangleF(2, 2, pnlSlip.Width - 4, pnlSlip.Height - 4));
    }

    // ── Print ──────────────────────────────────────────────────────────────

    private void btnPrint_Click(object sender, EventArgs e)
    {
        using var dlg = new PrintDialog { Document = _pd };
        if (dlg.ShowDialog() == DialogResult.OK)
            _pd.Print();
    }

    private void btnPreview_Click(object sender, EventArgs e)
    {
        using var ppd = new PrintPreviewDialog
        {
            Document    = _pd,
            WindowState = FormWindowState.Maximized
        };
        ppd.ShowDialog();
    }

    private void PrintPage(object? sender, PrintPageEventArgs e)
    {
        e.Graphics!.SmoothingMode = SmoothingMode.AntiAlias;
        DrawPaySlip(e.Graphics, new RectangleF(
            e.MarginBounds.X, e.MarginBounds.Y,
            e.MarginBounds.Width, e.MarginBounds.Height));
        e.HasMorePages = false;
    }

    private void btnClose_Click(object sender, EventArgs e) => Close();

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _pd.Dispose();
        base.OnFormClosed(e);
    }

    // ── Pay slip rendering ─────────────────────────────────────────────────

    private void DrawPaySlip(Graphics g, RectangleF area)
    {
        const float REF_W = 614f;
        float scale = area.Width / REF_W;

        var state = g.Save();
        g.TranslateTransform(area.Left, area.Top);
        g.ScaleTransform(scale, scale);
        DrawContent(g);
        g.Restore(state);
    }

    private void DrawContent(Graphics g)
    {
        // All coordinates are in reference space: 614px wide
        var blue       = Color.FromArgb(0, 120, 212);
        var lightBlue  = Color.FromArgb(238, 246, 255);
        var tableHdr   = Color.FromArgb(242, 245, 250);
        var lightGreen = Color.FromArgb(238, 252, 238);
        var green      = Color.FromArgb(0, 100, 0);
        var red        = Color.FromArgb(180, 40, 30);
        var divColor   = Color.FromArgb(200, 212, 228);
        var labelGray  = Color.FromArgb(115, 115, 125);
        var dark       = Color.FromArgb(22, 22, 22);

        using var fntCompany  = new Font("Segoe UI", 13f, FontStyle.Bold);
        using var fntSlipSub  = new Font("Segoe UI", 9f,  FontStyle.Bold);
        using var fntLabel    = new Font("Segoe UI", 7.5f, FontStyle.Bold);
        using var fntValue    = new Font("Segoe UI", 8.5f);
        using var fntColHdr   = new Font("Segoe UI", 8f,  FontStyle.Bold);
        using var fntSubTotal = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        using var fntNetLbl   = new Font("Segoe UI", 12f,  FontStyle.Bold);
        using var fntNetVal   = new Font("Segoe UI", 14f,  FontStyle.Bold);
        using var fntSmall    = new Font("Segoe UI", 7f);

        using var bBlue   = new SolidBrush(blue);
        using var bWhite  = new SolidBrush(Color.White);
        using var bDark   = new SolidBrush(dark);
        using var bGray   = new SolidBrush(labelGray);
        using var bGreen  = new SolidBrush(green);
        using var bRed    = new SolidBrush(red);
        using var pDiv    = new Pen(divColor, 0.8f);
        using var sfC     = new StringFormat { Alignment = StringAlignment.Center };
        using var sfR     = new StringFormat { Alignment = StringAlignment.Far };

        // ── Outer border ──────────────────────────────────────────────────
        g.DrawRectangle(new Pen(divColor, 1f), 0, 0, 613, 428);

        // ── Header (blue) ─────────────────────────────────────────────────
        g.FillRectangle(bBlue, 0, 0, 614, 60);
        g.DrawString("PAYROLL MANAGEMENT SYSTEM", fntCompany, bWhite,
            new RectangleF(0, 7, 614, 27), sfC);
        g.DrawString($"PAY SLIP  —  {_payPeriod}", fntSlipSub,
            new SolidBrush(Color.FromArgb(205, 232, 255)),
            new RectangleF(0, 38, 614, 18), sfC);

        // ── Employee info strip ───────────────────────────────────────────
        g.FillRectangle(new SolidBrush(lightBlue), 0, 60, 614, 88);

        const float lx = 15, rx = 318;  // left / right column x
        const float lv = 130, rv = 418; // left / right value x
        float iy = 68;

        Row(g, fntLabel, fntValue, bGray, bDark, lx, iy, "EMPLOYEE NAME", _employeeName, rx, "DEPARTMENT",   _department);
        Row(g, fntLabel, fntValue, bGray, bDark, lx, iy + 26, "POSITION", _position,    rx, "EMPLOYEE ID",  _employeeID.ToString("D4"));
        Row(g, fntLabel, fntValue, bGray, bDark, lx, iy + 52, "PAY PERIOD", _payPeriod, rx, "GENERATED ON", DateTime.Today.ToString("dd MMM yyyy"));

        g.DrawLine(pDiv, 14, 150, 600, 150);

        // ── Table header ──────────────────────────────────────────────────
        g.FillRectangle(new SolidBrush(tableHdr), 0, 153, 614, 24);
        g.DrawString("EARNINGS",   fntColHdr, bBlue, new RectangleF(lx, 157, 150, 16), new StringFormat());
        g.DrawString("AMOUNT (LKR)", fntSmall, bGray, new RectangleF(150, 159, 145, 14), sfR);
        g.DrawLine(pDiv, 307, 153, 307, 177);
        g.DrawString("DEDUCTIONS", fntColHdr, bRed,  new RectangleF(rx, 157, 150, 16), new StringFormat());
        g.DrawString("AMOUNT (LKR)", fntSmall, bGray, new RectangleF(460, 159, 139, 14), sfR);

        // ── Table rows ────────────────────────────────────────────────────
        const float ty = 183;
        const float tr = 22;

        TRow(g, fntValue, bDark,    lx, ty,       "Basic Salary",     _basicSalary,    rx, "EPF (8% employee)", _epf);
        TRow(g, fntValue, bDark,    lx, ty + tr,  "Allowances",       _allowances,     rx, "Tax",               _tax);
        TRow(g, fntValue, bDark,    lx, ty+tr*2,  null,               null,            rx, "Other Deductions",  _otherDeductions);

        // Subtotal dividers
        g.DrawLine(new Pen(divColor, 1f), lx, ty+tr*3+1, 295, ty+tr*3+1);
        g.DrawLine(new Pen(divColor, 1f), rx, ty+tr*3+1, 600, ty+tr*3+1);

        // Subtotal row
        TotRow(g, fntSubTotal, bBlue, bRed, lx, ty+tr*3+5,
            "Gross Salary", _grossSalary, rx, "Total Deductions", _totalDeductions);

        // ── Net salary ────────────────────────────────────────────────────
        float ny = ty + tr * 4 + 16;
        g.FillRectangle(new SolidBrush(lightGreen), 0, ny, 614, 42);
        g.DrawLine(new Pen(Color.FromArgb(155, 210, 155), 1f), 0, ny, 614, ny);
        g.DrawLine(new Pen(Color.FromArgb(155, 210, 155), 1f), 0, ny + 42, 614, ny + 42);
        g.DrawString("NET SALARY", fntNetLbl, bGreen, new RectangleF(lx, ny + 5, 260, 32));
        g.DrawString(_netSalary.ToString("N2"), fntNetVal, bGreen,
            new RectangleF(300, ny + 4, 300, 34), sfR);

        // ── ETF note ──────────────────────────────────────────────────────
        float noteY = ny + 50;
        g.DrawString(
            $"* ETF (employer contribution, 3%): {_etf:N2}  —  paid by employer, NOT deducted from your net salary.",
            fntSmall, bGray, new RectangleF(lx, noteY, 584, 16));

        if (!string.IsNullOrWhiteSpace(_remarks))
            g.DrawString($"  Remarks: {_remarks}", fntSmall, bGray,
                new RectangleF(lx, noteY + 13, 584, 16));

        // ── Signatures ────────────────────────────────────────────────────
        float footY = noteY + (string.IsNullOrWhiteSpace(_remarks) ? 24 : 38);
        g.DrawLine(pDiv, lx, footY, 600, footY);

        g.DrawString("Authorized Signature:", fntLabel, bGray, lx, footY + 8);
        g.DrawLine(new Pen(Color.Silver, 0.8f), 148, footY + 22, 295, footY + 22);

        g.DrawString("Employee Signature:", fntLabel, bGray, 370, footY + 8);
        g.DrawLine(new Pen(Color.Silver, 0.8f), 490, footY + 22, 598, footY + 22);
    }

    // ── Drawing helpers ────────────────────────────────────────────────────

    // One info row: two label+value pairs
    private static void Row(Graphics g, Font lf, Font vf, Brush lb, Brush vb,
        float x1, float y, string l1, string v1, float x2, string l2, string v2)
    {
        const float lv = 115, rv = 100; // label widths
        g.DrawString(l1, lf, lb, x1, y);
        g.DrawString(v1, vf, vb, x1 + lv, y);
        g.DrawString(l2, lf, lb, x2, y);
        g.DrawString(v2, vf, vb, x2 + rv, y);
    }

    // Earnings/deductions table row
    private static void TRow(Graphics g, Font f, Brush b,
        float lx, float y, string? ll, decimal? lv,
        float rx, string rl, decimal rv)
    {
        using var sfR = new StringFormat { Alignment = StringAlignment.Far };
        if (ll != null)
        {
            g.DrawString(ll, f, b, lx, y);
            if (lv.HasValue)
                g.DrawString(lv.Value.ToString("N2"), f, b,
                    new RectangleF(lx + 95, y, 200, 18), sfR);
        }
        g.DrawString(rl, f, b, rx, y);
        g.DrawString(rv.ToString("N2"), f, b,
            new RectangleF(rx + 110, y, 172, 18), sfR);
    }

    // Subtotal row (coloured)
    private static void TotRow(Graphics g, Font f, Brush lb, Brush rb,
        float lx, float y, string ll, decimal lv,
        float rx, string rl, decimal rv)
    {
        using var sfR = new StringFormat { Alignment = StringAlignment.Far };
        g.DrawString(ll, f, lb, lx, y);
        g.DrawString(lv.ToString("N2"), f, lb, new RectangleF(lx + 95, y, 200, 18), sfR);
        g.DrawString(rl, f, rb, rx, y);
        g.DrawString(rv.ToString("N2"), f, rb, new RectangleF(rx + 110, y, 172, 18), sfR);
    }
}
