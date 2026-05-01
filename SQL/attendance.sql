-- Module 2: Attendance Table
-- Run this in SSMS after schema.sql

USE PayrollDB;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Attendance' AND xtype='U')
BEGIN
    CREATE TABLE Attendance (
        AttendanceID    INT IDENTITY(1,1) PRIMARY KEY,
        EmployeeID      INT           NOT NULL,
        AttendanceDate  DATE          NOT NULL,
        Status          NVARCHAR(20)  NOT NULL,   -- Present, Absent, Late, Half Day, Leave
        Remarks         NVARCHAR(200) NULL,
        CONSTRAINT FK_Attendance_Employee FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID),
        CONSTRAINT UQ_Attendance UNIQUE (EmployeeID, AttendanceDate)
    );
    PRINT 'Attendance table created successfully.';
END
ELSE
    PRINT 'Attendance table already exists.';
GO
