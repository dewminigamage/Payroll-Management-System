namespace PayrollManagementSystem.API.Models;

public class DashboardSummary
{
    public int     TotalEmployees   { get; set; }
    public int     ActiveEmployees  { get; set; }
    public decimal MonthlyPayroll   { get; set; }
    public int     PendingLeave     { get; set; }
    public int     ActiveLoans      { get; set; }
    public decimal TotalLoanBalance { get; set; }
    public int     PayrollMonth     { get; set; }
    public int     PayrollYear      { get; set; }
    public List<DeptSummary>     ByDepartment   { get; set; } = [];
    public List<RecentActivity>  RecentActivity { get; set; } = [];
}

public class DeptSummary
{
    public string  Department  { get; set; } = "";
    public int     HeadCount   { get; set; }
    public decimal TotalSalary { get; set; }
}

public class RecentActivity
{
    public string   Description { get; set; } = "";
    public DateTime Date        { get; set; }
}
