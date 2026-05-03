namespace PayrollManagementSystem.API.Models;

public class OvertimeRecord
{
    public int      OvertimeID       { get; set; }
    public int      EmployeeID       { get; set; }
    public string?  EmployeeName     { get; set; }
    public int      PayMonth         { get; set; }
    public int      PayYear          { get; set; }
    public decimal  OTHours          { get; set; }
    public decimal  OTRateMultiplier { get; set; }
    public decimal  OTAmount         { get; set; }
    public string?  Notes            { get; set; }
    public DateTime CreatedDate      { get; set; }
}

public record UpsertOvertimeDto(
    int     EmployeeID,
    int     PayMonth,
    int     PayYear,
    decimal OTHours,
    decimal OTRateMultiplier,
    decimal OTAmount,
    string? Notes
);
