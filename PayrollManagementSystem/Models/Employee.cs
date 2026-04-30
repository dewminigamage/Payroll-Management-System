namespace PayrollManagementSystem.Models;

public class Employee
{
    public int EmployeeID { get; set; }
    public string FullName { get; set; } = "";
    public string NIC { get; set; } = "";
    public string Department { get; set; } = "";
    public string Position { get; set; } = "";
    public decimal BasicSalary { get; set; }
    public DateTime JoinDate { get; set; }
    public string ContactNumber { get; set; } = "";
    public string Email { get; set; } = "";
}
