namespace PayrollManagementSystem.API.Models;

public class Employee
{
    public int      EmployeeID    { get; set; }
    public string   FullName      { get; set; } = "";
    public string   NIC           { get; set; } = "";
    public string?  Department    { get; set; }
    public string?  Position      { get; set; }
    public decimal  BasicSalary   { get; set; }
    public DateOnly JoinDate      { get; set; }
    public string?  ContactNumber { get; set; }
    public string?  Email         { get; set; }
    public bool     IsActive      { get; set; } = true;
}

public record CreateEmployeeDto(
    string   FullName,
    string   NIC,
    string?  Department,
    string?  Position,
    decimal  BasicSalary,
    DateOnly JoinDate,
    string?  ContactNumber,
    string?  Email
);

public record UpdateEmployeeDto(
    string   FullName,
    string   NIC,
    string?  Department,
    string?  Position,
    decimal  BasicSalary,
    DateOnly JoinDate,
    string?  ContactNumber,
    string?  Email,
    bool     IsActive
);
