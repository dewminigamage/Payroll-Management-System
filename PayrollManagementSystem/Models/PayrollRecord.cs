namespace PayrollManagementSystem.Models;

public class PayrollRecord
{
    public int     PayrollID       { get; set; }
    public int     EmployeeID      { get; set; }
    public string  EmployeeName    { get; set; } = "";
    public int     PayMonth        { get; set; }
    public int     PayYear         { get; set; }
    public decimal BasicSalary     { get; set; }
    public decimal Allowances      { get; set; }
    public decimal GrossSalary     { get; set; }
    public decimal EPF             { get; set; }
    public decimal ETF             { get; set; }
    public decimal Tax             { get; set; }
    public decimal OtherDeductions { get; set; }
    public decimal NetSalary       { get; set; }
    public string  Remarks         { get; set; } = "";
}
