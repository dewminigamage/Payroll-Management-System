namespace PayrollManagementSystem.API.Models;

public class LeaveType
{
    public int     LeaveTypeID        { get; set; }
    public string  TypeName           { get; set; } = "";
    public int     DefaultDaysPerYear { get; set; }
    public string? Description        { get; set; }
}

public class LeaveBalance
{
    public int     BalanceID    { get; set; }
    public int     EmployeeID   { get; set; }
    public string? EmployeeName { get; set; }
    public int     LeaveTypeID  { get; set; }
    public string? TypeName     { get; set; }
    public int     Year         { get; set; }
    public int     TotalDays    { get; set; }
    public int     UsedDays     { get; set; }
    public int     RemainingDays => TotalDays - UsedDays;
}

public class LeaveRequest
{
    public int       RequestID    { get; set; }
    public int       EmployeeID   { get; set; }
    public string?   EmployeeName { get; set; }
    public int       LeaveTypeID  { get; set; }
    public string?   TypeName     { get; set; }
    public DateOnly  StartDate    { get; set; }
    public DateOnly  EndDate      { get; set; }
    public int       TotalDays    { get; set; }
    public string?   Reason       { get; set; }
    public string    Status       { get; set; } = "Pending";
    public string?   ApprovedBy   { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string?   Remarks      { get; set; }
    public DateTime  CreatedDate  { get; set; }
}

public record CreateLeaveRequestDto(
    int      EmployeeID,
    int      LeaveTypeID,
    DateOnly StartDate,
    DateOnly EndDate,
    int      TotalDays,
    string?  Reason
);

public record ApproveLeaveDto(string Status, string? Remarks);
