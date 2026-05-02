-- Module 5: User Login / Authentication
-- Run this in SQL Server Management Studio

USE PayrollDB;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE Users (
        UserID       INT           IDENTITY(1,1) PRIMARY KEY,
        Username     NVARCHAR(50)  NOT NULL UNIQUE,
        PasswordHash NVARCHAR(64)  NOT NULL,   -- SHA-256 hex (64 chars)
        FullName     NVARCHAR(100) NOT NULL,
        Role         NVARCHAR(20)  NOT NULL DEFAULT 'Viewer',  -- 'Admin' or 'Viewer'
        IsActive     BIT           NOT NULL DEFAULT 1,
        CreatedDate  DATETIME      NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Users table created successfully.';

    -- Default admin account  (password: admin123)
    -- Hash = SHA-256 of UTF-8 bytes of 'admin123'
    INSERT INTO Users (Username, PasswordHash, FullName, Role)
    VALUES (
        'admin',
        LOWER(CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', 'admin123'), 2)),
        'System Administrator',
        'Admin'
    );
    PRINT 'Default admin user created.  Username: admin  |  Password: admin123';
END
ELSE
    PRINT 'Users table already exists.';
GO
