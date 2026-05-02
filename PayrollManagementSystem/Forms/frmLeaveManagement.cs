using System.Data;
using Microsoft.Data.SqlClient;
using PayrollManagementSystem.Database;
using PayrollManagementSystem.Models;

namespace PayrollManagementSystem.Forms;

public partial class frmLeaveManagement : Form
{
    private int selectedRequestID = 0;
    private int selectedTypeID    = 0;

    public frmLeaveManagement()
    {
        InitializeComponent();
    }

    // ── Form load ──────────────────────────────────────────────────────────

    private void frmLeaveManagement_Load(object sender, EventArgs e)
    {
        LoadEmployeeCombos();
        LoadLeaveTypeCombos();
        LoadRequests();
        LoadLeaveTypes();
        LoadBalances();

        // Viewers cannot approve/reject/delete
        if (!UserSession.IsAdmin)
        {
            btnReqSave.Enabled    = false;
            btnReqApprove.Enabled = false;
            btnReqReject.Enabled  = false;
            btnReqDelete.Enabled  = false;
            btnBalInitYear.Enabled = false;
            btnTypeSave.Enabled   = false;
            btnTypeUpdate.Enabled = false;
            btnTypeDelete.Enabled = false;
        }
    }

    // ── Combo loaders ──────────────────────────────────────────────────────

    private void LoadEmployeeCombos()
    {
        const string q = "SELECT EmployeeID, FullName FROM Employees ORDER BY FullName";

        // Request form combo
        var dt1 = DatabaseHelper.ExecuteQuery(q);
        cmbReqEmployee.DisplayMember = "FullName";
        cmbReqEmployee.ValueMember   = "EmployeeID";
        cmbReqEmployee.DataSource    = dt1;
        cmbReqEmployee.SelectedIndex = -1;

        // Request filter combo
        var dt2 = DatabaseHelper.ExecuteQuery(q);
        var allRow = dt2.NewRow();
        allRow["EmployeeID"] = 0;
        allRow["FullName"]   = "-- All Employees --";
        dt2.Rows.InsertAt(allRow, 0);
        cmbReqFilterEmp.DisplayMember = "FullName";
        cmbReqFilterEmp.ValueMember   = "EmployeeID";
        cmbReqFilterEmp.DataSource    = dt2;
        cmbReqFilterEmp.SelectedIndex = 0;

        // Balance filter combo
        var dt3 = DatabaseHelper.ExecuteQuery(q);
        var allRow3 = dt3.NewRow();
        allRow3["EmployeeID"] = 0;
        allRow3["FullName"]   = "-- All Employees --";
        dt3.Rows.InsertAt(allRow3, 0);
        cmbBalEmp.DisplayMember = "FullName";
        cmbBalEmp.ValueMember   = "EmployeeID";
        cmbBalEmp.DataSource    = dt3;
        cmbBalEmp.SelectedIndex = 0;
    }

    private void LoadLeaveTypeCombos()
    {
        const string q = "SELECT LeaveTypeID, TypeName FROM LeaveTypes ORDER BY TypeName";
        var dt = DatabaseHelper.ExecuteQuery(q);
        cmbReqLeaveType.DisplayMember = "TypeName";
        cmbReqLeaveType.ValueMember   = "LeaveTypeID";
        cmbReqLeaveType.DataSource    = dt;
        cmbReqLeaveType.SelectedIndex = -1;
    }

    // ── TAB 1 – Leave Requests ─────────────────────────────────────────────

    private void LoadRequests()
    {
        int empID = cmbReqFilterEmp.SelectedValue != null
            ? Convert.ToInt32(cmbReqFilterEmp.SelectedValue) : 0;
        string status = cmbReqFilterStatus.SelectedItem?.ToString() ?? "All";

        var parameters = new List<SqlParameter>();
        string query = @"
            SELECT r.RequestID, e.FullName AS Employee, lt.TypeName AS LeaveType,
                   r.StartDate, r.EndDate, r.TotalDays, r.Status,
                   r.Reason, r.ApprovedBy, r.ApprovedDate, r.Remarks, r.CreatedDate,
                   r.EmployeeID, r.LeaveTypeID
            FROM   LeaveRequests r
            INNER JOIN Employees  e  ON r.EmployeeID  = e.EmployeeID
            INNER JOIN LeaveTypes lt ON r.LeaveTypeID = lt.LeaveTypeID
            WHERE  1=1";

        if (empID > 0)
        {
            query += " AND r.EmployeeID = @EmployeeID";
            parameters.Add(new("@EmployeeID", empID));
        }
        if (status != "All")
        {
            query += " AND r.Status = @Status";
            parameters.Add(new("@Status", status));
        }
        query += " ORDER BY r.CreatedDate DESC";

        var dt = DatabaseHelper.ExecuteQuery(query, parameters.ToArray());
        BindRequestGrid(dt);
        lblReqStatus.Text = $"Records: {dt.Rows.Count}";
    }

    private void BindRequestGrid(DataTable dt)
    {
        dgvRequests.DataSource = dt;
        if (dgvRequests.Columns.Count == 0) return;

        foreach (string col in new[] { "EmployeeID", "LeaveTypeID", "Reason", "ApprovedBy", "ApprovedDate", "Remarks", "CreatedDate" })
            if (dgvRequests.Columns.Contains(col))
                dgvRequests.Columns[col].Visible = false;

        dgvRequests.Columns["RequestID"].HeaderText   = "ID";
        dgvRequests.Columns["Employee"].HeaderText    = "Employee";
        dgvRequests.Columns["LeaveType"].HeaderText   = "Leave Type";
        dgvRequests.Columns["StartDate"].HeaderText   = "Start";
        dgvRequests.Columns["StartDate"].DefaultCellStyle.Format = "yyyy-MM-dd";
        dgvRequests.Columns["EndDate"].HeaderText     = "End";
        dgvRequests.Columns["EndDate"].DefaultCellStyle.Format   = "yyyy-MM-dd";
        dgvRequests.Columns["TotalDays"].HeaderText   = "Days";
        dgvRequests.Columns["Status"].HeaderText      = "Status";

        dgvRequests.Columns["RequestID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        dgvRequests.Columns["TotalDays"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        dgvRequests.Columns["Status"].AutoSizeMode    = DataGridViewAutoSizeColumnMode.AllCells;

        foreach (DataGridViewRow row in dgvRequests.Rows)
        {
            row.DefaultCellStyle.ForeColor = row.Cells["Status"].Value?.ToString() switch
            {
                "Approved" => Color.FromArgb(16, 124, 65),
                "Rejected" => Color.FromArgb(196, 43, 28),
                _          => Color.FromArgb(140, 100, 0)   // Pending
            };
        }
    }

    private void btnReqSave_Click(object sender, EventArgs e)
    {
        if (!ValidateRequest()) return;

        int empID       = Convert.ToInt32(cmbReqEmployee.SelectedValue);
        int leaveTypeID = Convert.ToInt32(cmbReqLeaveType.SelectedValue);
        DateTime start  = dtpReqStart.Value.Date;
        DateTime end    = dtpReqEnd.Value.Date;
        int days        = CalculateWorkingDays(start, end);

        if (days <= 0)
        {
            MessageBox.Show("End date must be on or after start date.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Check balance
        int year = start.Year;
        int remaining = GetRemainingBalance(empID, leaveTypeID, year);
        if (remaining >= 0 && days > remaining)
        {
            if (MessageBox.Show(
                    $"Insufficient leave balance.\nRequested: {days} day(s)  |  Remaining: {remaining} day(s)\n\nSave anyway?",
                    "Balance Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;
        }

        try
        {
            const string query = @"
                INSERT INTO LeaveRequests
                    (EmployeeID, LeaveTypeID, StartDate, EndDate, TotalDays, Reason, Status)
                VALUES
                    (@EmployeeID, @LeaveTypeID, @StartDate, @EndDate, @TotalDays, @Reason, 'Pending')";
            DatabaseHelper.ExecuteNonQuery(query, [
                new("@EmployeeID",   empID),
                new("@LeaveTypeID",  leaveTypeID),
                new("@StartDate",    start),
                new("@EndDate",      end),
                new("@TotalDays",    days),
                new("@Reason",       txtReqReason.Text.Trim())
            ]);
            MessageBox.Show("Leave request saved.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearRequestForm();
            LoadRequests();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving request:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnReqApprove_Click(object sender, EventArgs e)
    {
        if (selectedRequestID == 0) { ShowSelectFirst(); return; }

        if (MessageBox.Show("Approve this leave request?\nAttendance records will be created automatically.",
                "Confirm Approve", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();
        try
        {
            // Get request details
            var getReq = new SqlCommand(
                "SELECT EmployeeID, LeaveTypeID, StartDate, EndDate, TotalDays FROM LeaveRequests WHERE RequestID = @ID",
                conn, tx);
            getReq.Parameters.AddWithValue("@ID", selectedRequestID);
            using var rdr = getReq.ExecuteReader();
            if (!rdr.Read()) { rdr.Close(); tx.Rollback(); return; }

            int      empID       = rdr.GetInt32(0);
            int      leaveTypeID = rdr.GetInt32(1);
            DateTime start       = rdr.GetDateTime(2);
            DateTime end         = rdr.GetDateTime(3);
            int      days        = rdr.GetInt32(4);
            rdr.Close();

            // Update request status
            var updReq = new SqlCommand(
                @"UPDATE LeaveRequests SET Status='Approved', ApprovedBy=@By, ApprovedDate=GETDATE()
                  WHERE RequestID=@ID", conn, tx);
            updReq.Parameters.AddWithValue("@By", UserSession.FullName);
            updReq.Parameters.AddWithValue("@ID", selectedRequestID);
            updReq.ExecuteNonQuery();

            // Deduct from balance (upsert)
            int year = start.Year;
            var updBal = new SqlCommand(@"
                IF EXISTS (SELECT 1 FROM LeaveBalances WHERE EmployeeID=@EID AND LeaveTypeID=@LID AND Year=@Year)
                    UPDATE LeaveBalances SET UsedDays = UsedDays + @Days
                    WHERE  EmployeeID=@EID AND LeaveTypeID=@LID AND Year=@Year
                ELSE
                    INSERT INTO LeaveBalances (EmployeeID, LeaveTypeID, Year, TotalDays, UsedDays)
                    VALUES (@EID, @LID, @Year,
                        (SELECT DefaultDaysPerYear FROM LeaveTypes WHERE LeaveTypeID=@LID),
                        @Days)", conn, tx);
            updBal.Parameters.AddWithValue("@EID",  empID);
            updBal.Parameters.AddWithValue("@LID",  leaveTypeID);
            updBal.Parameters.AddWithValue("@Year", year);
            updBal.Parameters.AddWithValue("@Days", days);
            updBal.ExecuteNonQuery();

            // Create attendance records for each day
            string leaveTypeName = GetLeaveTypeName(leaveTypeID);
            for (DateTime d = start; d <= end; d = d.AddDays(1))
            {
                if (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                var insAtt = new SqlCommand(@"
                    IF NOT EXISTS (SELECT 1 FROM Attendance WHERE EmployeeID=@EID AND AttendanceDate=@Date)
                        INSERT INTO Attendance (EmployeeID, AttendanceDate, Status, Remarks)
                        VALUES (@EID, @Date, 'Leave', @Remarks)", conn, tx);
                insAtt.Parameters.AddWithValue("@EID",     empID);
                insAtt.Parameters.AddWithValue("@Date",    d);
                insAtt.Parameters.AddWithValue("@Remarks", leaveTypeName);
                insAtt.ExecuteNonQuery();
            }

            tx.Commit();
            MessageBox.Show("Leave approved and attendance records created.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearRequestForm();
            LoadRequests();
            LoadBalances();
        }
        catch (Exception ex)
        {
            tx.Rollback();
            MessageBox.Show($"Error approving leave:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnReqReject_Click(object sender, EventArgs e)
    {
        if (selectedRequestID == 0) { ShowSelectFirst(); return; }
        if (MessageBox.Show("Reject this leave request?", "Confirm Reject",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        try
        {
            DatabaseHelper.ExecuteNonQuery(@"
                UPDATE LeaveRequests SET Status='Rejected', ApprovedBy=@By, ApprovedDate=GETDATE()
                WHERE RequestID=@ID",
                [new("@By", UserSession.FullName), new("@ID", selectedRequestID)]);
            MessageBox.Show("Leave request rejected.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearRequestForm();
            LoadRequests();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnReqDelete_Click(object sender, EventArgs e)
    {
        if (selectedRequestID == 0) { ShowSelectFirst(); return; }
        if (MessageBox.Show("Delete this leave request?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        try
        {
            DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM LeaveRequests WHERE RequestID = @ID",
                [new("@ID", selectedRequestID)]);
            MessageBox.Show("Request deleted.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearRequestForm();
            LoadRequests();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnReqClear_Click(object sender, EventArgs e) => ClearRequestForm();

    private void btnReqClearFilter_Click(object sender, EventArgs e)
    {
        cmbReqFilterEmp.SelectedIndex    = 0;
        cmbReqFilterStatus.SelectedIndex = 0;
        LoadRequests();
    }

    private void dgvRequests_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var row = dgvRequests.Rows[e.RowIndex];

        selectedRequestID = Convert.ToInt32(row.Cells["RequestID"].Value);
        txtReqID.Text     = selectedRequestID.ToString();

        int empID = Convert.ToInt32(row.Cells["EmployeeID"].Value);
        SelectComboByValue(cmbReqEmployee, empID);

        int ltID = Convert.ToInt32(row.Cells["LeaveTypeID"].Value);
        SelectComboByValue(cmbReqLeaveType, ltID);

        if (DateTime.TryParse(row.Cells["StartDate"].Value?.ToString(), out var s)) dtpReqStart.Value = s;
        if (DateTime.TryParse(row.Cells["EndDate"].Value?.ToString(), out var en)) dtpReqEnd.Value   = en;

        txtReqReason.Text = row.Cells["Reason"].Value?.ToString() ?? "";
        UpdateDayCount();
    }

    private void dtpReq_ValueChanged(object sender, EventArgs e) => UpdateDayCount();

    private void UpdateDayCount()
    {
        int days = CalculateWorkingDays(dtpReqStart.Value.Date, dtpReqEnd.Value.Date);
        lblReqDaysValue.Text = days > 0 ? days.ToString() : "0";
    }

    private void ClearRequestForm()
    {
        selectedRequestID           = 0;
        txtReqID.Text               = "";
        cmbReqEmployee.SelectedIndex = -1;
        cmbReqLeaveType.SelectedIndex = -1;
        dtpReqStart.Value           = DateTime.Today;
        dtpReqEnd.Value             = DateTime.Today;
        txtReqReason.Text           = "";
        lblReqDaysValue.Text        = "0";
    }

    private bool ValidateRequest()
    {
        if (cmbReqEmployee.SelectedValue == null || Convert.ToInt32(cmbReqEmployee.SelectedValue) == 0)
        {
            MessageBox.Show("Please select an employee.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (cmbReqLeaveType.SelectedValue == null || Convert.ToInt32(cmbReqLeaveType.SelectedValue) == 0)
        {
            MessageBox.Show("Please select a leave type.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }

    // ── TAB 2 – Leave Balances ─────────────────────────────────────────────

    private void LoadBalances()
    {
        int year  = (int)nudBalYear.Value;
        int empID = cmbBalEmp.SelectedValue != null ? Convert.ToInt32(cmbBalEmp.SelectedValue) : 0;

        var parameters = new List<SqlParameter> { new("@Year", year) };
        string query = @"
            SELECT e.FullName AS Employee, lt.TypeName AS LeaveType,
                   lb.TotalDays, lb.UsedDays,
                   (lb.TotalDays - lb.UsedDays) AS RemainingDays,
                   lb.BalanceID, lb.EmployeeID, lb.LeaveTypeID
            FROM   LeaveBalances lb
            INNER JOIN Employees  e  ON lb.EmployeeID  = e.EmployeeID
            INNER JOIN LeaveTypes lt ON lb.LeaveTypeID = lt.LeaveTypeID
            WHERE  lb.Year = @Year";

        if (empID > 0)
        {
            query += " AND lb.EmployeeID = @EmpID";
            parameters.Add(new("@EmpID", empID));
        }
        query += " ORDER BY e.FullName, lt.TypeName";

        var dt = DatabaseHelper.ExecuteQuery(query, parameters.ToArray());
        BindBalancesGrid(dt);
        lblBalStatus.Text = $"{dt.Rows.Count} balance records for {year}";
    }

    private void BindBalancesGrid(DataTable dt)
    {
        dgvBalances.DataSource = dt;
        if (dgvBalances.Columns.Count == 0) return;

        foreach (string col in new[] { "BalanceID", "EmployeeID", "LeaveTypeID" })
            if (dgvBalances.Columns.Contains(col))
                dgvBalances.Columns[col].Visible = false;

        dgvBalances.Columns["Employee"].HeaderText     = "Employee";
        dgvBalances.Columns["LeaveType"].HeaderText    = "Leave Type";
        dgvBalances.Columns["TotalDays"].HeaderText    = "Total Days";
        dgvBalances.Columns["UsedDays"].HeaderText     = "Used Days";
        dgvBalances.Columns["RemainingDays"].HeaderText = "Remaining";

        dgvBalances.Columns["TotalDays"].ReadOnly     = false;
        dgvBalances.Columns["Employee"].ReadOnly      = true;
        dgvBalances.Columns["LeaveType"].ReadOnly     = true;
        dgvBalances.Columns["UsedDays"].ReadOnly      = true;
        dgvBalances.Columns["RemainingDays"].ReadOnly = true;

        foreach (DataGridViewRow row in dgvBalances.Rows)
        {
            int remaining = Convert.ToInt32(row.Cells["RemainingDays"].Value);
            if (remaining <= 0)
                row.DefaultCellStyle.ForeColor = Color.FromArgb(196, 43, 28);
            else if (remaining <= 3)
                row.DefaultCellStyle.ForeColor = Color.FromArgb(180, 100, 0);
        }
    }

    private void btnBalInitYear_Click(object sender, EventArgs e)
    {
        int year = (int)nudBalYear.Value;
        if (MessageBox.Show(
                $"This will create leave balance records for ALL employees for {year}.\n" +
                "Existing records will NOT be overwritten.\n\nProceed?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        try
        {
            const string query = @"
                INSERT INTO LeaveBalances (EmployeeID, LeaveTypeID, Year, TotalDays, UsedDays)
                SELECT e.EmployeeID, lt.LeaveTypeID, @Year, lt.DefaultDaysPerYear, 0
                FROM   Employees  e
                CROSS JOIN LeaveTypes lt
                WHERE  NOT EXISTS (
                    SELECT 1 FROM LeaveBalances lb
                    WHERE lb.EmployeeID=e.EmployeeID AND lb.LeaveTypeID=lt.LeaveTypeID AND lb.Year=@Year
                )";
            int rows = DatabaseHelper.ExecuteNonQuery(query, [new("@Year", year)]);
            MessageBox.Show($"{rows} balance records created for {year}.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadBalances();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void dgvBalances_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
        if (!UserSession.IsAdmin) return;
        var row = dgvBalances.Rows[e.RowIndex];
        if (!int.TryParse(row.Cells["TotalDays"].Value?.ToString(), out int newTotal) || newTotal < 0)
        {
            MessageBox.Show("Total days must be a positive number.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            LoadBalances();
            return;
        }
        int balID = Convert.ToInt32(row.Cells["BalanceID"].Value);
        try
        {
            DatabaseHelper.ExecuteNonQuery(
                "UPDATE LeaveBalances SET TotalDays = @Total WHERE BalanceID = @ID",
                [new("@Total", newTotal), new("@ID", balID)]);
            LoadBalances();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating balance: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            LoadBalances();
        }
    }

    // ── TAB 3 – Leave Types ────────────────────────────────────────────────

    private void LoadLeaveTypes()
    {
        const string q = "SELECT LeaveTypeID, TypeName, DefaultDaysPerYear, Description FROM LeaveTypes ORDER BY TypeName";
        var dt = DatabaseHelper.ExecuteQuery(q);
        BindTypesGrid(dt);
        lblTypeStatus.Text = $"Records: {dt.Rows.Count}";
    }

    private void BindTypesGrid(DataTable dt)
    {
        dgvLeaveTypes.DataSource = dt;
        if (dgvLeaveTypes.Columns.Count == 0) return;

        dgvLeaveTypes.Columns["LeaveTypeID"].HeaderText         = "ID";
        dgvLeaveTypes.Columns["TypeName"].HeaderText            = "Type Name";
        dgvLeaveTypes.Columns["DefaultDaysPerYear"].HeaderText  = "Days/Year";
        dgvLeaveTypes.Columns["Description"].HeaderText         = "Description";

        dgvLeaveTypes.Columns["LeaveTypeID"].AutoSizeMode        = DataGridViewAutoSizeColumnMode.AllCells;
        dgvLeaveTypes.Columns["DefaultDaysPerYear"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
    }

    private void btnTypeSave_Click(object sender, EventArgs e)
    {
        if (!ValidateTypeForm()) return;
        try
        {
            DatabaseHelper.ExecuteNonQuery(
                "INSERT INTO LeaveTypes (TypeName, DefaultDaysPerYear, Description) VALUES (@Name, @Days, @Desc)",
                [
                    new("@Name", txtTypeName.Text.Trim()),
                    new("@Days", (int)nudTypeDays.Value),
                    new("@Desc", txtTypeDesc.Text.Trim())
                ]);
            MessageBox.Show("Leave type saved.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearTypeForm();
            LoadLeaveTypes();
            LoadLeaveTypeCombos();
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            MessageBox.Show("A leave type with this name already exists.", "Duplicate",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnTypeUpdate_Click(object sender, EventArgs e)
    {
        if (selectedTypeID == 0) { ShowSelectFirst(); return; }
        if (!ValidateTypeForm()) return;
        try
        {
            DatabaseHelper.ExecuteNonQuery(
                "UPDATE LeaveTypes SET TypeName=@Name, DefaultDaysPerYear=@Days, Description=@Desc WHERE LeaveTypeID=@ID",
                [
                    new("@Name", txtTypeName.Text.Trim()),
                    new("@Days", (int)nudTypeDays.Value),
                    new("@Desc", txtTypeDesc.Text.Trim()),
                    new("@ID",   selectedTypeID)
                ]);
            MessageBox.Show("Leave type updated.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearTypeForm();
            LoadLeaveTypes();
            LoadLeaveTypeCombos();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnTypeDelete_Click(object sender, EventArgs e)
    {
        if (selectedTypeID == 0) { ShowSelectFirst(); return; }
        if (MessageBox.Show("Delete this leave type? This will also delete all related balances and requests.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        try
        {
            DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM LeaveTypes WHERE LeaveTypeID = @ID",
                [new("@ID", selectedTypeID)]);
            MessageBox.Show("Leave type deleted.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearTypeForm();
            LoadLeaveTypes();
            LoadLeaveTypeCombos();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void dgvLeaveTypes_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var row = dgvLeaveTypes.Rows[e.RowIndex];
        selectedTypeID       = Convert.ToInt32(row.Cells["LeaveTypeID"].Value);
        txtTypeID.Text       = selectedTypeID.ToString();
        txtTypeName.Text     = row.Cells["TypeName"].Value?.ToString() ?? "";
        nudTypeDays.Value    = Convert.ToInt32(row.Cells["DefaultDaysPerYear"].Value);
        txtTypeDesc.Text     = row.Cells["Description"].Value?.ToString() ?? "";
    }

    private void ClearTypeForm()
    {
        selectedTypeID   = 0;
        txtTypeID.Text   = "";
        txtTypeName.Text = "";
        nudTypeDays.Value = 14;
        txtTypeDesc.Text = "";
    }

    private bool ValidateTypeForm()
    {
        if (string.IsNullOrWhiteSpace(txtTypeName.Text))
        {
            MessageBox.Show("Please enter a type name.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtTypeName.Focus();
            return false;
        }
        return true;
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private static int CalculateWorkingDays(DateTime start, DateTime end)
    {
        if (end < start) return 0;
        int days = 0;
        for (DateTime d = start; d <= end; d = d.AddDays(1))
            if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                days++;
        return days;
    }

    private static int GetRemainingBalance(int empID, int leaveTypeID, int year)
    {
        var dt = DatabaseHelper.ExecuteQuery(
            "SELECT TotalDays - UsedDays FROM LeaveBalances WHERE EmployeeID=@E AND LeaveTypeID=@L AND Year=@Y",
            [new("@E", empID), new("@L", leaveTypeID), new("@Y", year)]);
        if (dt.Rows.Count == 0) return -1; // no balance record = no restriction
        return dt.Rows[0][0] == DBNull.Value ? -1 : Convert.ToInt32(dt.Rows[0][0]);
    }

    private static string GetLeaveTypeName(int leaveTypeID)
    {
        var dt = DatabaseHelper.ExecuteQuery(
            "SELECT TypeName FROM LeaveTypes WHERE LeaveTypeID = @ID",
            [new("@ID", leaveTypeID)]);
        return dt.Rows.Count > 0 ? dt.Rows[0][0]?.ToString() ?? "Leave" : "Leave";
    }

    private static void SelectComboByValue(ComboBox cmb, int value)
    {
        if (cmb.DataSource is not DataTable dt) return;
        foreach (DataRowView item in dt.DefaultView)
        {
            if (Convert.ToInt32(item[cmb.ValueMember]) == value)
            {
                cmb.SelectedItem = item;
                return;
            }
        }
    }

    private static void ShowSelectFirst() =>
        MessageBox.Show("Please select a record from the list first.", "No Selection",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
