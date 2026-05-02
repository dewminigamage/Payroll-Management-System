-- ============================================================
-- Salary Increment History + Overtime Management – Setup
-- Run once against PayrollDB
-- ============================================================

-- 1. Salary History
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SalaryHistory')
BEGIN
    CREATE TABLE SalaryHistory (
        HistoryID     INT IDENTITY(1,1) PRIMARY KEY,
        EmployeeID    INT            NOT NULL REFERENCES Employees(EmployeeID) ON DELETE CASCADE,
        OldSalary     DECIMAL(10,2)  NOT NULL,
        NewSalary     DECIMAL(10,2)  NOT NULL,
        EffectiveDate DATE           NOT NULL DEFAULT GETDATE(),
        Reason        NVARCHAR(300)  NULL,
        ChangedBy     NVARCHAR(100)  NULL,
        CreatedDate   DATETIME       NOT NULL DEFAULT GETDATE()
    );
END
GO

-- 2. Overtime Records
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'OvertimeRecords')
BEGIN
    CREATE TABLE OvertimeRecords (
        OvertimeID       INT IDENTITY(1,1) PRIMARY KEY,
        EmployeeID       INT           NOT NULL REFERENCES Employees(EmployeeID) ON DELETE CASCADE,
        PayMonth         INT           NOT NULL,
        PayYear          INT           NOT NULL,
        OTHours          DECIMAL(6,2)  NOT NULL DEFAULT 0,
        OTRateMultiplier DECIMAL(4,2)  NOT NULL DEFAULT 1.5,  -- e.g. 1.5x normal rate
        OTAmount         DECIMAL(10,2) NOT NULL DEFAULT 0,    -- calculated and stored
        Notes            NVARCHAR(300) NULL,
        CreatedDate      DATETIME      NOT NULL DEFAULT GETDATE(),
        CONSTRAINT UQ_OTRecord UNIQUE (EmployeeID, PayMonth, PayYear),
        CONSTRAINT CHK_OTMonth CHECK (PayMonth BETWEEN 1 AND 12)
    );
END
GO

-- 3. Add working hours per month to CompanySettings (if not already there)
IF NOT EXISTS (SELECT 1 FROM CompanySettings WHERE SettingKey = 'working_hours_per_month')
    INSERT INTO CompanySettings (SettingKey, SettingValue) VALUES ('working_hours_per_month', '160');
GO
