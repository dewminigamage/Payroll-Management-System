using System.Data;
using PayrollManagementSystem.Database;

namespace PayrollManagementSystem.Forms;

public partial class frmSalaryHistory : Form
{
    private readonly int    _employeeID;
    private readonly string _employeeName;

    public frmSalaryHistory(int employeeID, string employeeName)
    {
        InitializeComponent();
        _employeeID   = employeeID;
        _employeeName = employeeName;
    }

    private void frmSalaryHistory_Load(object sender, EventArgs e)
    {
        lblEmployee.Text = $"Employee: {_employeeName}";
        LoadHistory();
    }

    private void LoadHistory()
    {
        var dt = DatabaseHelper.ExecuteQuery(@"
            SELECT HistoryID,
                   EffectiveDate,
                   OldSalary,
                   NewSalary,
                   (NewSalary - OldSalary)                       AS Change,
                   CASE WHEN OldSalary > 0
                        THEN ROUND((NewSalary - OldSalary) * 100.0 / OldSalary, 2)
                        ELSE 0 END                               AS ChangePct,
                   Reason,
                   ChangedBy
            FROM   SalaryHistory
            WHERE  EmployeeID = @EID
            ORDER  BY EffectiveDate DESC, HistoryID DESC",
            [new Microsoft.Data.SqlClient.SqlParameter("@EID", _employeeID)]);

        dgvHistory.DataSource = dt;
        if (dgvHistory.Columns.Count == 0) return;

        dgvHistory.Columns["HistoryID"].Visible = false;

        dgvHistory.Columns["EffectiveDate"].HeaderText = "Effective Date";
        dgvHistory.Columns["EffectiveDate"].DefaultCellStyle.Format = "yyyy-MM-dd";
        dgvHistory.Columns["OldSalary"].HeaderText    = "Old Salary";
        dgvHistory.Columns["OldSalary"].DefaultCellStyle.Format    = "N2";
        dgvHistory.Columns["NewSalary"].HeaderText    = "New Salary";
        dgvHistory.Columns["NewSalary"].DefaultCellStyle.Format    = "N2";
        dgvHistory.Columns["Change"].HeaderText       = "Change";
        dgvHistory.Columns["Change"].DefaultCellStyle.Format       = "N2";
        dgvHistory.Columns["ChangePct"].HeaderText    = "Change %";
        dgvHistory.Columns["ChangePct"].DefaultCellStyle.Format    = "0.##";
        dgvHistory.Columns["Reason"].HeaderText       = "Reason";
        dgvHistory.Columns["ChangedBy"].HeaderText    = "Changed By";

        foreach (string col in new[] { "OldSalary", "NewSalary", "Change", "ChangePct" })
            dgvHistory.Columns[col].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        dgvHistory.Columns["EffectiveDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        dgvHistory.Columns["OldSalary"].AutoSizeMode     = DataGridViewAutoSizeColumnMode.AllCells;
        dgvHistory.Columns["NewSalary"].AutoSizeMode     = DataGridViewAutoSizeColumnMode.AllCells;
        dgvHistory.Columns["Change"].AutoSizeMode        = DataGridViewAutoSizeColumnMode.AllCells;
        dgvHistory.Columns["ChangePct"].AutoSizeMode     = DataGridViewAutoSizeColumnMode.AllCells;
        dgvHistory.Columns["ChangedBy"].AutoSizeMode     = DataGridViewAutoSizeColumnMode.AllCells;

        // Colour increments green, decrements red
        foreach (DataGridViewRow row in dgvHistory.Rows)
        {
            var change = row.Cells["Change"].Value;
            if (change != DBNull.Value)
            {
                decimal delta = Convert.ToDecimal(change);
                row.Cells["Change"].Style.ForeColor = delta >= 0
                    ? Color.FromArgb(16, 124, 65) : Color.FromArgb(196, 43, 28);
                row.Cells["ChangePct"].Style.ForeColor = row.Cells["Change"].Style.ForeColor;
            }
        }

        lblStatus.Text = $"{dt.Rows.Count} record(s)";
    }
}
