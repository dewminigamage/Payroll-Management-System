-- Test Data for Payroll Management System
-- Run once in PayrollDB

USE PayrollDB;
GO

-- ── Add IsActive column if missing ──────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME='Employees' AND COLUMN_NAME='IsActive')
    ALTER TABLE Employees ADD IsActive BIT NOT NULL DEFAULT 1;
GO

-- ── Employees ───────────────────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM Employees)
BEGIN
    INSERT INTO Employees (FullName, NIC, Department, Position, BasicSalary, JoinDate, ContactNumber, Email)
    VALUES
        (N'Kasun Perera',        '199012345671', 'IT',         'Software Engineer',   85000.00, '2020-03-15', '0771234567', 'kasun@company.lk'),
        (N'Dilani Fernando',     '199212345672', 'HR',         'HR Manager',          95000.00, '2019-06-01', '0772345678', 'dilani@company.lk'),
        (N'Nuwan Silva',         '199112345673', 'Finance',    'Accountant',          75000.00, '2021-01-10', '0773456789', 'nuwan@company.lk'),
        (N'Sanduni Jayawardena', '199312345674', 'IT',         'Junior Developer',    60000.00, '2022-08-20', '0774567890', 'sanduni@company.lk'),
        (N'Chamara Bandara',     '199412345675', 'Operations', 'Operations Manager',  90000.00, '2018-11-05', '0775678901', 'chamara@company.lk'),
        (N'Malsha Wickrama',     '199512345676', 'Finance',    'Finance Analyst',     70000.00, '2023-02-14', '0776789012', 'malsha@company.lk');
    PRINT 'Employees inserted.';
END
ELSE
    PRINT 'Employees already exist – skipped.';
GO

-- ── Payroll Records – April 2026 (last month) ───────────────────────────────
IF NOT EXISTS (SELECT 1 FROM PayrollRecords WHERE PayMonth=4 AND PayYear=2026)
BEGIN
    INSERT INTO PayrollRecords
        (EmployeeID, PayMonth, PayYear, BasicSalary, Allowances, GrossSalary, EPF, ETF, Tax, OtherDeductions, NetSalary, Remarks)
    SELECT
        e.EmployeeID,
        4, 2026,
        e.BasicSalary,
        v.Allowances,
        e.BasicSalary + v.Allowances,
        ROUND(e.BasicSalary * 0.08, 2),   -- EPF 8%
        ROUND(e.BasicSalary * 0.03, 2),   -- ETF 3%
        0.00,
        0.00,
        e.BasicSalary + v.Allowances - ROUND(e.BasicSalary * 0.08, 2),
        'April 2026 payroll'
    FROM Employees e
    JOIN (VALUES
        ('kasun@company.lk',    10000.00),
        ('dilani@company.lk',   15000.00),
        ('nuwan@company.lk',     8000.00),
        ('sanduni@company.lk',   5000.00),
        ('chamara@company.lk',  20000.00),
        ('malsha@company.lk',    7000.00)
    ) AS v(Email, Allowances) ON e.Email = v.Email;
    PRINT 'April 2026 payroll inserted.';
END
ELSE
    PRINT 'April 2026 payroll already exists – skipped.';
GO

-- ── Overtime Records – May 2026 (current month) ─────────────────────────────
IF NOT EXISTS (SELECT 1 FROM OvertimeRecords WHERE PayMonth=5 AND PayYear=2026)
BEGIN
    INSERT INTO OvertimeRecords (EmployeeID, PayMonth, PayYear, OTHours, OTRateMultiplier, OTAmount, Notes)
    SELECT
        e.EmployeeID,
        5, 2026,
        v.OTHours,
        1.5,
        ROUND((e.BasicSalary / 160.0) * v.OTHours * 1.5, 2),
        'May 2026 overtime'
    FROM Employees e
    JOIN (VALUES
        ('chamara@company.lk',  20.00),   -- most OT
        ('kasun@company.lk',    12.00),
        ('nuwan@company.lk',     8.00),
        ('sanduni@company.lk',   5.00)
    ) AS v(Email, OTHours) ON e.Email = v.Email;
    PRINT 'May 2026 overtime inserted.';
END
ELSE
    PRINT 'May 2026 overtime already exists – skipped.';
GO

-- ── Employee Loans ───────────────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM EmployeeLoans)
BEGIN
    INSERT INTO EmployeeLoans
        (EmployeeID, LoanTypeID, LoanAmount, RemainingBalance, MonthlyInstallment, StartMonth, StartYear, Status, Notes)
    SELECT e.EmployeeID, lt.LoanTypeID, v.LoanAmount, v.Balance, v.Installment, v.SMonth, 2026, 'Active', v.Note
    FROM (VALUES
        ('dilani@company.lk',  'Personal Loan',   200000.00, 170000.00, 15000.00, 1, 'Personal loan for home renovation'),
        ('chamara@company.lk', 'Salary Advance',  100000.00,  75000.00, 25000.00, 2, 'Vehicle repair advance'),
        ('nuwan@company.lk',   'Emergency Loan',   50000.00,  40000.00, 10000.00, 3, 'Medical emergency')
    ) AS v(Email, LoanType, LoanAmount, Balance, Installment, SMonth, Note)
    JOIN Employees  e  ON e.Email    = v.Email
    JOIN LoanTypes  lt ON lt.TypeName = v.LoanType;
    PRINT 'Loans inserted.';
END
ELSE
    PRINT 'Loans already exist – skipped.';
GO

-- ── Summary ──────────────────────────────────────────────────────────────────
SELECT 'Employees'      AS [Table], COUNT(*) AS [Rows] FROM Employees
UNION ALL
SELECT 'PayrollRecords',                COUNT(*)        FROM PayrollRecords
UNION ALL
SELECT 'OvertimeRecords',               COUNT(*)        FROM OvertimeRecords
UNION ALL
SELECT 'EmployeeLoans',                 COUNT(*)        FROM EmployeeLoans;
GO
