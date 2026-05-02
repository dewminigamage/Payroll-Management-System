-- Run once in PayrollDB

-- ── Loan Types ──────────────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LoanTypes')
BEGIN
    CREATE TABLE LoanTypes (
        LoanTypeID INT           IDENTITY(1,1) PRIMARY KEY,
        TypeName   NVARCHAR(100) NOT NULL CONSTRAINT UQ_LoanTypes_Name UNIQUE,
        IsActive   BIT           NOT NULL DEFAULT 1
    );
    INSERT INTO LoanTypes (TypeName) VALUES
        (N'Salary Advance'),
        (N'Personal Loan'),
        (N'Emergency Loan');
END;

-- ── Employee Loans ───────────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EmployeeLoans')
    CREATE TABLE EmployeeLoans (
        LoanID             INT           IDENTITY(1,1) PRIMARY KEY,
        EmployeeID         INT           NOT NULL
                           CONSTRAINT FK_Loans_Employee
                           REFERENCES Employees(EmployeeID) ON DELETE CASCADE,
        LoanTypeID         INT           NOT NULL
                           CONSTRAINT FK_Loans_Type
                           REFERENCES LoanTypes(LoanTypeID),
        LoanAmount         DECIMAL(18,2) NOT NULL,
        RemainingBalance   DECIMAL(18,2) NOT NULL,
        MonthlyInstallment DECIMAL(18,2) NOT NULL,
        StartMonth         INT           NOT NULL
                           CONSTRAINT CK_Loans_Month CHECK (StartMonth BETWEEN 1 AND 12),
        StartYear          INT           NOT NULL,
        Status             NVARCHAR(20)  NOT NULL DEFAULT 'Active'
                           CONSTRAINT CK_Loans_Status
                           CHECK (Status IN ('Active','Completed','Cancelled')),
        Notes              NVARCHAR(500),
        CreatedDate        DATETIME      NOT NULL DEFAULT GETDATE()
    );

-- ── Loan Repayments ──────────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LoanRepayments')
    CREATE TABLE LoanRepayments (
        RepaymentID INT           IDENTITY(1,1) PRIMARY KEY,
        LoanID      INT           NOT NULL
                    CONSTRAINT FK_Repayments_Loan
                    REFERENCES EmployeeLoans(LoanID) ON DELETE CASCADE,
        PayMonth    INT           NOT NULL,
        PayYear     INT           NOT NULL,
        AmountPaid  DECIMAL(18,2) NOT NULL,
        Notes       NVARCHAR(255),
        CreatedDate DATETIME      NOT NULL DEFAULT GETDATE(),
        CONSTRAINT UQ_LoanRepayment UNIQUE (LoanID, PayMonth, PayYear)
    );

-- ── Tax Brackets (monthly gross, Sri Lanka PAYE defaults) ───────────────
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TaxBrackets')
BEGIN
    CREATE TABLE TaxBrackets (
        BracketID    INT           IDENTITY(1,1) PRIMARY KEY,
        MinIncome    DECIMAL(18,2) NOT NULL,
        MaxIncome    DECIMAL(18,2) NULL,        -- NULL = no upper limit
        TaxRate      DECIMAL(5,4)  NOT NULL,    -- 0.0600 = 6%
        DisplayOrder INT           NOT NULL
    );
    INSERT INTO TaxBrackets (MinIncome, MaxIncome, TaxRate, DisplayOrder) VALUES
        (0,       150000, 0.0000, 1),
        (150000,  250000, 0.0600, 2),
        (250000,  375000, 0.1200, 3),
        (375000,  500000, 0.1800, 4),
        (500000,  625000, 0.2400, 5),
        (625000,  NULL,   0.3000, 6);
END;
