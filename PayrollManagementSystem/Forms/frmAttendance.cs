using System.Data;
using Microsoft.Data.SqlClient;
using PayrollManagementSystem.Database;

namespace PayrollManagementSystem.Forms;

public partial class frmAttendance : Form
{
    private int selectedAttendanceID = 0;

    public frmAttendance()
    {
        InitializeComponent();
    }

    private void frmAttendance_Load(object sender, EventArgs e)
    {
        dtpFilterFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        dtpFilterTo.Value   = DateTime.Today;
        LoadEmployeeCombo();
        LoadFilterEmployeeCombo();
        LoadAttendance();
    }

    // ── Data loading ───────────────────────────────────────────────────────

    private void LoadEmployeeCombo()
    {
        const string query = "SELECT EmployeeID, FullName FROM Employees ORDER BY FullName";
        var dt = DatabaseHelper.ExecuteQuery(query);
        cmbEmployee.DisplayMember = "FullName";
        cmbEmployee.ValueMember   = "EmployeeID";
        cmbEmployee.DataSource    = dt;
        cmbEmployee.SelectedIndex = -1;
    }

    private void LoadFilterEmployeeCombo()
    {
        const string query = "SELECT EmployeeID, FullName FROM Employees ORDER BY FullName";
        var dt = DatabaseHelper.ExecuteQuery(query);

        var allRow = dt.NewRow();
        allRow["EmployeeID"] = 0;
        allRow["FullName"]   = "-- All Employees --";
        dt.Rows.InsertAt(allRow, 0);

        cmbFilterEmployee.DisplayMember = "FullName";
        cmbFilterEmployee.ValueMember   = "EmployeeID";
        cmbFilterEmployee.DataSource    = dt;
        cmbFilterEmployee.SelectedIndex = 0;
    }

    private void LoadAttendance()
    {
        int filterEmpID = cmbFilterEmployee.SelectedValue != null
            ? Convert.ToInt32(cmbFilterEmployee.SelectedValue) : 0;

        var parameters = new List<SqlParameter>
        {
            new("@FromDate", dtpFilterFrom.Value.Date),
            new("@ToDate",   dtpFilterTo.Value.Date)
        };

        string query = @"
            SELECT a.AttendanceID, e.FullName, a.AttendanceDate, a.Status, a.Remarks, a.EmployeeID
            FROM   Attendance a
            INNER JOIN Employees e ON a.EmployeeID = e.EmployeeID
            WHERE  a.AttendanceDate BETWEEN @FromDate AND @ToDate";

        if (filterEmpID > 0)
        {
            query += " AND a.EmployeeID = @EmployeeID";
            parameters.Add(new("@EmployeeID", filterEmpID));
        }

        query += " ORDER BY a.AttendanceDate DESC, e.FullName";

        var dt = DatabaseHelper.ExecuteQuery(query, parameters.ToArray());
        BindGrid(dt);
        lblStatus.Text = $"Records: {dt.Rows.Count}";
    }

    private void BindGrid(DataTable dt)
    {
        dgvAttendance.DataSource = dt;
        if (dgvAttendance.Columns.Count == 0) return;

        if (dgvAttendance.Columns.Contains("EmployeeID"))
            dgvAttendance.Columns["EmployeeID"].Visible = false;

        dgvAttendance.Columns["AttendanceID"].HeaderText  = "ID";
        dgvAttendance.Columns["FullName"].HeaderText       = "Employee Name";
        dgvAttendance.Columns["AttendanceDate"].HeaderText = "Date";
        dgvAttendance.Columns["AttendanceDate"].DefaultCellStyle.Format = "yyyy-MM-dd";
        dgvAttendance.Columns["Status"].HeaderText         = "Status";
        dgvAttendance.Columns["Remarks"].HeaderText        = "Remarks";

        dgvAttendance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvAttendance.Columns["AttendanceID"].AutoSizeMode    = DataGridViewAutoSizeColumnMode.AllCells;
        dgvAttendance.Columns["AttendanceDate"].AutoSizeMode  = DataGridViewAutoSizeColumnMode.AllCells;
        dgvAttendance.Columns["Status"].AutoSizeMode          = DataGridViewAutoSizeColumnMode.AllCells;

        // Color-code status rows
        foreach (DataGridViewRow row in dgvAttendance.Rows)
        {
            row.DefaultCellStyle.ForeColor = row.Cells["Status"].Value?.ToString() switch
            {
                "Absent"   => Color.FromArgb(196, 43, 28),
                "Late"     => Color.FromArgb(180, 100, 0),
                "Half Day" => Color.FromArgb(150, 80, 0),
                "Leave"    => Color.FromArgb(100, 100, 180),
                _          => Color.FromArgb(20, 130, 20)   // Present
            };
        }
    }

    // ── Form helpers ───────────────────────────────────────────────────────

    private void ClearForm()
    {
        selectedAttendanceID  = 0;
        txtAttendanceID.Text  = "";
        dtpDate.Value         = DateTime.Today;
        cmbEmployee.SelectedIndex = -1;
        cmbStatus.SelectedIndex   = 0;
        txtRemarks.Text       = "";
    }

    private bool ValidateInput()
    {
        if (cmbEmployee.SelectedValue == null || Convert.ToInt32(cmbEmployee.SelectedValue) == 0)
        {
            MessageBox.Show("Please select an employee.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            cmbEmployee.Focus();
            return false;
        }
        return true;
    }

    // ── Button events ──────────────────────────────────────────────────────

    private void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateInput()) return;
        try
        {
            const string query = @"
                INSERT INTO Attendance (EmployeeID, AttendanceDate, Status, Remarks)
                VALUES (@EmployeeID, @AttendanceDate, @Status, @Remarks)";
            DatabaseHelper.ExecuteNonQuery(query, [
                new("@EmployeeID",     Convert.ToInt32(cmbEmployee.SelectedValue)),
                new("@AttendanceDate", dtpDate.Value.Date),
                new("@Status",         cmbStatus.SelectedItem!.ToString()!),
                new("@Remarks",        txtRemarks.Text.Trim())
            ]);
            MessageBox.Show("Attendance saved successfully.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            LoadAttendance();
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            MessageBox.Show(
                "Attendance for this employee on this date already exists.\nSelect the record and use Update to change it.",
                "Duplicate Record", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving attendance:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        if (selectedAttendanceID == 0)
        {
            MessageBox.Show("Please select a record from the list first.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!ValidateInput()) return;
        try
        {
            const string query = @"
                UPDATE Attendance SET
                    EmployeeID     = @EmployeeID,
                    AttendanceDate = @AttendanceDate,
                    Status         = @Status,
                    Remarks        = @Remarks
                WHERE AttendanceID = @AttendanceID";
            DatabaseHelper.ExecuteNonQuery(query, [
                new("@EmployeeID",     Convert.ToInt32(cmbEmployee.SelectedValue)),
                new("@AttendanceDate", dtpDate.Value.Date),
                new("@Status",         cmbStatus.SelectedItem!.ToString()!),
                new("@Remarks",        txtRemarks.Text.Trim()),
                new("@AttendanceID",   selectedAttendanceID)
            ]);
            MessageBox.Show("Attendance updated successfully.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            LoadAttendance();
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            MessageBox.Show("Attendance for this employee on this date already exists.",
                "Duplicate Record", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating attendance:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
        if (selectedAttendanceID == 0)
        {
            MessageBox.Show("Please select a record to delete.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (MessageBox.Show("Delete this attendance record?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        try
        {
            DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM Attendance WHERE AttendanceID = @AttendanceID",
                [new("@AttendanceID", selectedAttendanceID)]);
            MessageBox.Show("Record deleted.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            LoadAttendance();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting record:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnClear_Click(object sender, EventArgs e)  => ClearForm();
    private void btnFilter_Click(object sender, EventArgs e) => LoadAttendance();

    private void btnClearFilter_Click(object sender, EventArgs e)
    {
        cmbFilterEmployee.SelectedIndex = 0;
        dtpFilterFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        dtpFilterTo.Value   = DateTime.Today;
        LoadAttendance();
    }

    // ── Grid row click ─────────────────────────────────────────────────────

    private void dgvAttendance_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var row = dgvAttendance.Rows[e.RowIndex];

        selectedAttendanceID = Convert.ToInt32(row.Cells["AttendanceID"].Value);
        txtAttendanceID.Text = selectedAttendanceID.ToString();

        // Load employee into combo
        int empID = Convert.ToInt32(row.Cells["EmployeeID"].Value);
        foreach (DataRowView item in (DataTable)cmbEmployee.DataSource!)
        {
            if (Convert.ToInt32(item["EmployeeID"]) == empID)
            {
                cmbEmployee.SelectedItem = item;
                break;
            }
        }

        if (DateTime.TryParse(row.Cells["AttendanceDate"].Value?.ToString(), out var date))
            dtpDate.Value = date;

        string status   = row.Cells["Status"].Value?.ToString() ?? "Present";
        int    statusIdx = cmbStatus.Items.IndexOf(status);
        cmbStatus.SelectedIndex = statusIdx >= 0 ? statusIdx : 0;

        txtRemarks.Text = row.Cells["Remarks"].Value?.ToString() ?? "";
    }
}
