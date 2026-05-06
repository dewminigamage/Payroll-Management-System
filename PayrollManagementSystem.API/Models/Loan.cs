namespace PayrollManagementSystem.API.Models;

public class LoanType
{
    public int    LoanTypeID { get; set; }
    public string TypeName   { get; set; } = "";
    public bool   IsActive   { get; set; }
}

public class EmployeeLoan
{
    public int      LoanID             { get; set; }
    public int      EmployeeID         { get; set; }
    public string?  EmployeeName       { get; set; }
    public int      LoanTypeID         { get; set; }
    public string?  LoanTypeName       { get; set; }
    public decimal  LoanAmount         { get; set; }
    public decimal  RemainingBalance   { get; set; }
    public decimal  MonthlyInstallment { get; set; }
    public int      StartMonth         { get; set; }
    public int      StartYear          { get; set; }
    public string   Status             { get; set; } = "Active";
    public string?  Notes              { get; set; }
    public DateTime CreatedDate        { get; set; }
}

public class LoanRepayment
{
    public int      RepaymentID { get; set; }
    public int      LoanID      { get; set; }
    public int      PayMonth    { get; set; }
    public int      PayYear     { get; set; }
    public decimal  AmountPaid  { get; set; }
    public string?  Notes       { get; set; }
    public DateTime CreatedDate { get; set; }
}

public record CreateLoanDto(
    int     EmployeeID,
    int     LoanTypeID,
    decimal LoanAmount,
    decimal MonthlyInstallment,
    int     StartMonth,
    int     StartYear,
    string? Notes
);

public record UpdateLoanDto(
    decimal RemainingBalance,
    string  Status,
    string? Notes
);

public record RecordRepaymentDto(
    int     PayMonth,
    int     PayYear,
    decimal AmountPaid,
    string? Notes
);
