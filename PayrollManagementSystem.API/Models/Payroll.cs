namespace PayrollManagementSystem.API.Models;

public class PayrollRecord
{
    public int      PayrollID       { get; set; }
    public int      EmployeeID      { get; set; }
    public string?  EmployeeName    { get; set; }
    public string?  Department      { get; set; }
    public int      PayMonth        { get; set; }
    public int      PayYear         { get; set; }
    public decimal  BasicSalary     { get; set; }
    public decimal  Allowances      { get; set; }
    public decimal  GrossSalary     { get; set; }
    public decimal  EPF             { get; set; }
    public decimal  ETF             { get; set; }
    public decimal  Tax             { get; set; }
    public decimal  OtherDeductions { get; set; }
    public decimal  NetSalary       { get; set; }
    public string?  Remarks         { get; set; }
    public DateTime CreatedDate     { get; set; }
}

public record CreatePayrollDto(
    int     EmployeeID,
    int     PayMonth,
    int     PayYear,
    decimal BasicSalary,
    decimal Allowances,
    decimal GrossSalary,
    decimal EPF,
    decimal ETF,
    decimal Tax,
    decimal OtherDeductions,
    decimal NetSalary,
    string? Remarks
);

public record UpdatePayrollDto(
    decimal Allowances,
    decimal GrossSalary,
    decimal EPF,
    decimal ETF,
    decimal Tax,
    decimal OtherDeductions,
    decimal NetSalary,
    string? Remarks
);

public record BulkPayrollDto(int PayMonth, int PayYear);
