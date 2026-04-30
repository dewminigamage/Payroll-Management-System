-- Payroll Management System - Database Schema
-- Run this in SQL Server Management Studio before starting the application

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'PayrollDB')
    CREATE DATABASE PayrollDB;
GO

USE PayrollDB;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Employees' AND xtype='U')
BEGIN
    CREATE TABLE Employees (
        EmployeeID      INT IDENTITY(1,1) PRIMARY KEY,
        FullName        NVARCHAR(100) NOT NULL,
        NIC             NVARCHAR(20)  NOT NULL UNIQUE,
        Department      NVARCHAR(50)  NULL,
        Position        NVARCHAR(50)  NULL,
        BasicSalary     DECIMAL(10,2) NOT NULL,
        JoinDate        DATE          NOT NULL,
        ContactNumber   NVARCHAR(15)  NULL,
        Email           NVARCHAR(100) NULL
    );
    PRINT 'Employees table created successfully.';
END
ELSE
    PRINT 'Employees table already exists.';
GO
