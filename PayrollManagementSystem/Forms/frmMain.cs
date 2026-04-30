using PayrollManagementSystem.Forms;

namespace PayrollManagementSystem.Forms;

public partial class frmMain : Form
{
    public frmMain()
    {
        InitializeComponent();
    }

    private void btnEmployeeManagement_Click(object sender, EventArgs e)
    {
        using var frm = new frmEmployee();
        frm.ShowDialog();
    }
}
