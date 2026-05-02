using PayrollManagementSystem.Models;

namespace PayrollManagementSystem.Forms;

public partial class frmMain : Form
{
    public frmMain()
    {
        InitializeComponent();
        lblWelcome.Text = $"{UserSession.FullName}  |  {UserSession.Role}";

        if (!UserSession.IsAdmin)
        {
            btnEmployeeManagement.Enabled = false;
            btnAttendanceManagement.Enabled = false;
            btnPayroll.Enabled = false;
            btnUserManagement.Enabled = false;
            btnBulkPayroll.Enabled = false;
            btnLeaveManagement.Enabled = false;
            btnSettings.Enabled = false;
            btnOvertime.Enabled       = false;
            btnLoanManagement.Enabled = false;
            btnAIAssistant.Enabled    = false;
        }
    }

    private void btnEmployeeManagement_Click(object sender, EventArgs e)
    {
        using var frm = new frmEmployee();
        frm.ShowDialog();
    }

    private void btnAttendanceManagement_Click(object sender, EventArgs e)
    {
        using var frm = new frmAttendance();
        frm.ShowDialog();
    }

    private void btnPayroll_Click(object sender, EventArgs e)
    {
        using var frm = new frmPayroll();
        frm.ShowDialog();
    }

    private void btnReports_Click(object sender, EventArgs e)
    {
        using var frm = new frmReports();
        frm.ShowDialog();
    }

    private void btnUserManagement_Click(object sender, EventArgs e)
    {
        using var frm = new frmUserManagement();
        frm.ShowDialog();
    }

    private void btnBulkPayroll_Click(object sender, EventArgs e)
    {
        using var frm = new frmBulkPayroll();
        frm.ShowDialog();
    }

    private void btnLeaveManagement_Click(object sender, EventArgs e)
    {
        using var frm = new frmLeaveManagement();
        frm.ShowDialog();
    }

    private void btnSettings_Click(object sender, EventArgs e)
    {
        using var frm = new frmSettings();
        frm.ShowDialog();
    }

    private void btnOvertime_Click(object sender, EventArgs e)
    {
        using var frm = new frmOvertime();
        frm.ShowDialog();
    }

    private void btnLoanManagement_Click(object sender, EventArgs e)
    {
        using var frm = new frmLoanManagement();
        frm.ShowDialog();
    }

    private void btnAIAssistant_Click(object sender, EventArgs e)
    {
        using var frm = new frmAIAssistant();
        frm.ShowDialog();
    }

    private void btnLogout_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show("Are you sure you want to sign out?", "Sign Out",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        UserSession.LogoutRequested = true;
        Close();
    }
}
