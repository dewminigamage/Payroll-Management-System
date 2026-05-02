-- ============================================================
-- Leave Management Module – Database Setup
-- Run this against PayrollDB
-- ============================================================

-- 1. Leave Types (Annual, Sick, Casual, …)
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'LeaveTypes')
BEGIN
    CREATE TABLE LeaveTypes (
        LeaveTypeID      INT IDENTITY(1,1) PRIMARY KEY,
        TypeName         NVARCHAR(50)  NOT NULL UNIQUE,
        DefaultDaysPerYear INT         NOT NULL DEFAULT 14,
        Description      NVARCHAR(200) NULL
    );

    INSERT INTO LeaveTypes (TypeName, DefaultDaysPerYear, Description) VALUES
        ('Annual Leave',   14, 'Yearly paid leave entitlement'),
        ('Sick Leave',     10, 'Medical / illness leave'),
        ('Casual Leave',    7, 'Short-notice personal leave');
END
GO

-- 2. Leave Balances per employee per year
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'LeaveBalances')
BEGIN
    CREATE TABLE LeaveBalances (
        BalanceID    INT IDENTITY(1,1) PRIMARY KEY,
        EmployeeID   INT NOT NULL REFERENCES Employees(EmployeeID) ON DELETE CASCADE,
        LeaveTypeID  INT NOT NULL REFERENCES LeaveTypes(LeaveTypeID),
        Year         INT NOT NULL,
        TotalDays    INT NOT NULL DEFAULT 0,
        UsedDays     INT NOT NULL DEFAULT 0,
        CONSTRAINT UQ_LeaveBalance UNIQUE (EmployeeID, LeaveTypeID, Year)
    );
END
GO

-- 3. Leave Requests
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'LeaveRequests')
BEGIN
    CREATE TABLE LeaveRequests (
        RequestID    INT IDENTITY(1,1) PRIMARY KEY,
        EmployeeID   INT          NOT NULL REFERENCES Employees(EmployeeID) ON DELETE CASCADE,
        LeaveTypeID  INT          NOT NULL REFERENCES LeaveTypes(LeaveTypeID),
        StartDate    DATE         NOT NULL,
        EndDate      DATE         NOT NULL,
        TotalDays    INT          NOT NULL,
        Reason       NVARCHAR(500) NULL,
        Status       NVARCHAR(20) NOT NULL DEFAULT 'Pending',   -- Pending | Approved | Rejected
        ApprovedBy   NVARCHAR(100) NULL,
        ApprovedDate DATETIME     NULL,
        Remarks      NVARCHAR(255) NULL,
        CreatedDate  DATETIME     NOT NULL DEFAULT GETDATE(),
        CONSTRAINT CHK_LeaveStatus CHECK (Status IN ('Pending','Approved','Rejected')),
        CONSTRAINT CHK_LeaveDates  CHECK (EndDate >= StartDate)
    );
END
GO
