-- Module 3: Payroll / Salary Calculation
-- Run this in SQL Server Management Studio

USE PayrollDB;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PayrollRecords' AND xtype='U')
BEGIN
    CREATE TABLE PayrollRecords (
        PayrollID       INT           IDENTITY(1,1) PRIMARY KEY,
        EmployeeID      INT           NOT NULL,
        PayMonth        INT           NOT NULL,   -- 1 = January ... 12 = December
        PayYear         INT           NOT NULL,
        BasicSalary     DECIMAL(10,2) NOT NULL,
        Allowances      DECIMAL(10,2) NOT NULL DEFAULT 0,
        GrossSalary     DECIMAL(10,2) NOT NULL,
        EPF             DECIMAL(10,2) NOT NULL DEFAULT 0,  -- 8% employee contribution
        ETF             DECIMAL(10,2) NOT NULL DEFAULT 0,  -- 3% employer contribution
        Tax             DECIMAL(10,2) NOT NULL DEFAULT 0,
        OtherDeductions DECIMAL(10,2) NOT NULL DEFAULT 0,
        NetSalary       DECIMAL(10,2) NOT NULL,
        Remarks         NVARCHAR(255) NULL,
        CreatedDate     DATETIME      NOT NULL DEFAULT GETDATE(),

        CONSTRAINT FK_PayrollRecords_Employees
            FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID),
        CONSTRAINT UQ_PayrollRecords_EmpMonthYear
            UNIQUE (EmployeeID, PayMonth, PayYear)
    );
    PRINT 'PayrollRecords table created successfully.';
END
ELSE
    PRINT 'PayrollRecords table already exists.';
GO
