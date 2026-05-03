namespace PayrollManagementSystem.API.Models;

public class AttendanceRecord
{
    public int      AttendanceID   { get; set; }
    public int      EmployeeID     { get; set; }
    public string?  EmployeeName   { get; set; }
    public DateOnly AttendanceDate { get; set; }
    public string   Status         { get; set; } = "Present";
    public string?  Remarks        { get; set; }
}

public record UpsertAttendanceDto(
    int      EmployeeID,
    DateOnly AttendanceDate,
    string   Status,
    string?  Remarks
);
