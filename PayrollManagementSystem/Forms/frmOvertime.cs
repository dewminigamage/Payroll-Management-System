using System.Data;
using Microsoft.Data.SqlClient;
using PayrollManagementSystem.Database;
using PayrollManagementSystem.Helpers;

namespace PayrollManagementSystem.Forms;

public partial class frmOvertime : Form
{
    private int     selectedOTID  = 0;
    private decimal _basicSalary  = 0m;

    public frmOvertime()
    {
        InitializeComponent();
    }

    private void frmOvertime_Load(object sender, EventArgs e)
    {
        LoadEmployeeCombos();
        LoadFilterMonth();
        LoadOTRecords();
        UpdateHoursLabel();
    }

    // ── Data loading ───────────────────────────────────────────────────────

    private void LoadEmployeeCombos()
    {
        const string q = "SELECT EmployeeID, FullName FROM Employees ORDER BY FullName";

        var dt1 = DatabaseHelper.ExecuteQuery(q);
        cmbEmployee.DisplayMember = "FullName";
        cmbEmployee.ValueMember   = "EmployeeID";
        cmbEmployee.DataSource    = dt1;
        cmbEmployee.SelectedIndex = -1;

        var dt2 = DatabaseHelper.ExecuteQuery(q);
        var all = dt2.NewRow();
        all["EmployeeID"] = 0; all["FullName"] = "-- All Employees --";
        dt2.Rows.InsertAt(all, 0);
        cmbFilterEmp.DisplayMember = "FullName";
        cmbFilterEmp.ValueMember   = "EmployeeID";
        cmbFilterEmp.DataSource    = dt2;
        cmbFilterEmp.SelectedIndex = 0;
    }

    private void LoadFilterMonth()
    {
        cmbFilterMonth.Items.Add("-- All Months --");
        cmbFilterMonth.Items.AddRange(new object[] {
            "January","February","March","April","May","June",
            "July","August","September","October","November","December" });
        cmbFilterMonth.SelectedIndex = 0;
    }

    private void LoadOTRecords()
    {
        int empID = cmbFilterEmp.SelectedValue != null
            ? Convert.ToInt32(cmbFilterEmp.SelectedValue) : 0;
        int year   = (int)nudFilterYear.Value;
        int month  = cmbFilterMonth.SelectedIndex; // 0 = all, 1-12 = month

        var parameters = new List<SqlParameter> { new("@Year", year) };
        string query = @"
            SELECT ot.OvertimeID, e.FullName AS Employee,
                   ot.PayMonth, ot.PayYear,
                   ot.OTHours, ot.OTRateMultiplier, ot.OTAmount, ot.Notes,
                   ot.EmployeeID
            FROM   OvertimeRecords ot
            INNER JOIN Employees e ON ot.EmployeeID = e.EmployeeID
            WHERE  ot.PayYear = @Year";

        if (month > 0)
        {
            query += " AND ot.PayMonth = @Month";
            parameters.Add(new("@Month", month));
        }
        if (empID > 0)
        {
            query += " AND ot.EmployeeID = @EmpID";
            parameters.Add(new("@EmpID", empID));
        }
        query += " ORDER BY ot.PayYear DESC, ot.PayMonth DESC, e.FullName";

        var dt = DatabaseHelper.ExecuteQuery(query, parameters.ToArray());
        BindGrid(dt);
        lblStatus.Text = $"Records: {dt.Rows.Count}";
    }

    private void BindGrid(DataTable dt)
    {
        dgvOT.DataSource = dt;
        if (dgvOT.Columns.Count == 0) return;

        dgvOT.Columns["OvertimeID"].Visible = false;
        dgvOT.Columns["EmployeeID"].Visible = false;

        string[] months = { "Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec" };
        dt.Columns.Add("Period", typeof(string));
        foreach (DataRow r in dt.Rows)
            r["Period"] = $"{months[Convert.ToInt32(r["PayMonth"]) - 1]} {r["PayYear"]}";

        dgvOT.Columns["Employee"].HeaderText        = "Employee";
        dgvOT.Columns["Period"].HeaderText          = "Period";
        dgvOT.Columns["OTHours"].HeaderText         = "OT Hours";
        dgvOT.Columns["OTRateMultiplier"].HeaderText = "Rate ×";
        dgvOT.Columns["OTAmount"].HeaderText        = "OT Amount";
        dgvOT.Columns["Notes"].HeaderText           = "Notes";
        dgvOT.Columns["PayMonth"].Visible           = false;
        dgvOT.Columns["PayYear"].Visible            = false;

        dgvOT.Columns["OTHours"].DefaultCellStyle.Format         = "0.##";
        dgvOT.Columns["OTRateMultiplier"].DefaultCellStyle.Format = "0.##";
        dgvOT.Columns["OTAmount"].DefaultCellStyle.Format        = "N2";
        dgvOT.Columns["OTAmount"].DefaultCellStyle.Alignment     = DataGridViewContentAlignment.MiddleRight;
        dgvOT.Columns["OTHours"].DefaultCellStyle.Alignment      = DataGridViewContentAlignment.MiddleRight;

        dgvOT.Columns["OvertimeID"].AutoSizeMode        = DataGridViewAutoSizeColumnMode.AllCells;
        dgvOT.Columns["Period"].AutoSizeMode            = DataGridViewAutoSizeColumnMode.AllCells;
        dgvOT.Columns["OTHours"].AutoSizeMode           = DataGridViewAutoSizeColumnMode.AllCells;
        dgvOT.Columns["OTRateMultiplier"].AutoSizeMode  = DataGridViewAutoSizeColumnMode.AllCells;
        dgvOT.Columns["OTAmount"].AutoSizeMode          = DataGridViewAutoSizeColumnMode.AllCells;

        dgvOT.Columns["Period"].DisplayIndex   = 1;
        dgvOT.Columns["Employee"].DisplayIndex = 0;
    }

    // ── Employee selected – fetch basic salary ─────────────────────────────

    private void cmbEmployee_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbEmployee.SelectedValue == null || Convert.ToInt32(cmbEmployee.SelectedValue) == 0)
        { _basicSalary = 0m; UpdateCalculation(); return; }

        var dt = DatabaseHelper.ExecuteQuery(
            "SELECT BasicSalary FROM Employees WHERE EmployeeID = @ID",
            [new("@ID", Convert.ToInt32(cmbEmployee.SelectedValue))]);
        _basicSalary = dt.Rows.Count > 0 ? Convert.ToDecimal(dt.Rows[0]["BasicSalary"]) : 0m;
        UpdateCalculation();
    }

    // ── Live calculation ───────────────────────────────────────────────────

    private void nudOTHours_ValueChanged(object sender, EventArgs e)     => UpdateCalculation();
    private void nudOTMultiplier_ValueChanged(object sender, EventArgs e) => UpdateCalculation();

    private void UpdateCalculation()
    {
        int wh = PayrollSettings.WorkingHoursPerMonth;
        decimal hourlyRate = wh > 0 && _basicSalary > 0
            ? Math.Round(_basicSalary / wh, 4) : 0m;
        decimal otAmount   = Math.Round(hourlyRate * nudOTMultiplier.Value * nudOTHours.Value, 2);

        lblHourlyRate.Text = hourlyRate > 0 ? $"Hourly rate: {hourlyRate:N2}" : "Hourly rate: —";
        lblOTAmount.Text   = $"OT Amount: LKR {otAmount:N2}";
    }

    private void UpdateHoursLabel()
    {
        lblWorkHoursRef.Text = $"(working {PayrollSettings.WorkingHoursPerMonth} hrs/month)";
    }

    // ── CRUD ───────────────────────────────────────────────────────────────

    private void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateForm()) return;
        decimal otAmount = CalculateOTAmount();
        try
        {
            DatabaseHelper.ExecuteNonQuery(@"
                INSERT INTO OvertimeRecords
                    (EmployeeID, PayMonth, PayYear, OTHours, OTRateMultiplier, OTAmount, Notes)
                VALUES
                    (@EID, @Month, @Year, @Hours, @Rate, @Amount, @Notes)",
            [
                new("@EID",    Convert.ToInt32(cmbEmployee.SelectedValue)),
                new("@Month",  cmbPayMonth.SelectedIndex + 1),
                new("@Year",   (int)nudPayYear.Value),
                new("@Hours",  nudOTHours.Value),
                new("@Rate",   nudOTMultiplier.Value),
                new("@Amount", otAmount),
                new("@Notes",  txtNotes.Text.Trim())
            ]);
            MessageBox.Show("Overtime record saved.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            LoadOTRecords();
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            MessageBox.Show(
                "An overtime record for this employee and month already exists.\nSelect it and use Update to modify.",
                "Duplicate Record", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving record:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        if (selectedOTID == 0) { ShowSelectFirst(); return; }
        if (!ValidateForm()) return;
        decimal otAmount = CalculateOTAmount();
        try
        {
            DatabaseHelper.ExecuteNonQuery(@"
                UPDATE OvertimeRecords SET
                    EmployeeID       = @EID,
                    PayMonth         = @Month,
                    PayYear          = @Year,
                    OTHours          = @Hours,
                    OTRateMultiplier = @Rate,
                    OTAmount         = @Amount,
                    Notes            = @Notes
                WHERE OvertimeID = @ID",
            [
                new("@EID",    Convert.ToInt32(cmbEmployee.SelectedValue)),
                new("@Month",  cmbPayMonth.SelectedIndex + 1),
                new("@Year",   (int)nudPayYear.Value),
                new("@Hours",  nudOTHours.Value),
                new("@Rate",   nudOTMultiplier.Value),
                new("@Amount", otAmount),
                new("@Notes",  txtNotes.Text.Trim()),
                new("@ID",     selectedOTID)
            ]);
            MessageBox.Show("Overtime record updated.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            LoadOTRecords();
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            MessageBox.Show("An overtime record for this employee and month already exists.",
                "Duplicate Record", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating record:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
        if (selectedOTID == 0) { ShowSelectFirst(); return; }
        if (MessageBox.Show("Delete this overtime record?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        try
        {
            DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM OvertimeRecords WHERE OvertimeID = @ID",
                [new("@ID", selectedOTID)]);
            MessageBox.Show("Record deleted.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            LoadOTRecords();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting record:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnClear_Click(object sender, EventArgs e)   => ClearForm();
    private void btnFilter_Click(object sender, EventArgs e)  => LoadOTRecords();

    private void btnClearFilter_Click(object sender, EventArgs e)
    {
        cmbFilterEmp.SelectedIndex    = 0;
        cmbFilterMonth.SelectedIndex  = 0;
        nudFilterYear.Value           = DateTime.Today.Year;
        LoadOTRecords();
    }

    // ── Grid click ─────────────────────────────────────────────────────────

    private void dgvOT_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var row = dgvOT.Rows[e.RowIndex];

        selectedOTID = Convert.ToInt32(row.Cells["OvertimeID"].Value);
        txtOTID.Text = selectedOTID.ToString();

        int empID = Convert.ToInt32(row.Cells["EmployeeID"].Value);
        foreach (DataRowView item in ((DataTable)cmbEmployee.DataSource!).DefaultView)
        {
            if (Convert.ToInt32(item["EmployeeID"]) == empID)
            { cmbEmployee.SelectedItem = item; break; }
        }

        cmbPayMonth.SelectedIndex = Convert.ToInt32(row.Cells["PayMonth"].Value) - 1;
        nudPayYear.Value          = Convert.ToInt32(row.Cells["PayYear"].Value);
        nudOTHours.Value          = Convert.ToDecimal(row.Cells["OTHours"].Value);
        nudOTMultiplier.Value     = Convert.ToDecimal(row.Cells["OTRateMultiplier"].Value);
        txtNotes.Text             = row.Cells["Notes"].Value?.ToString() ?? "";
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private decimal CalculateOTAmount()
    {
        int wh = PayrollSettings.WorkingHoursPerMonth;
        decimal hourlyRate = wh > 0 && _basicSalary > 0 ? _basicSalary / wh : 0m;
        return Math.Round(hourlyRate * nudOTMultiplier.Value * nudOTHours.Value, 2);
    }

    private void ClearForm()
    {
        selectedOTID              = 0;
        txtOTID.Text              = "";
        cmbEmployee.SelectedIndex = -1;
        cmbPayMonth.SelectedIndex = DateTime.Today.Month - 1;
        nudPayYear.Value          = DateTime.Today.Year;
        nudOTHours.Value          = 0;
        nudOTMultiplier.Value     = 1.5m;
        txtNotes.Text             = "";
        _basicSalary              = 0m;
        UpdateCalculation();
    }

    private bool ValidateForm()
    {
        if (cmbEmployee.SelectedValue == null || Convert.ToInt32(cmbEmployee.SelectedValue) == 0)
        {
            MessageBox.Show("Please select an employee.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (nudOTHours.Value <= 0)
        {
            MessageBox.Show("OT hours must be greater than zero.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }

    private static void ShowSelectFirst() =>
        MessageBox.Show("Please select a record from the list first.", "No Selection",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
