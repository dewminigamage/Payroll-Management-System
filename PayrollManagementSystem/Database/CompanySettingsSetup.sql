-- ============================================================
-- Configurable Payroll Settings – Database Setup
-- Run this once against PayrollDB
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CompanySettings')
BEGIN
    CREATE TABLE CompanySettings (
        SettingKey   NVARCHAR(50)  NOT NULL PRIMARY KEY,
        SettingValue NVARCHAR(500) NULL
    );

    INSERT INTO CompanySettings (SettingKey, SettingValue) VALUES
        ('company_name',    'My Company (Pvt) Ltd'),
        ('company_address', ''),
        ('company_phone',   ''),
        ('company_email',   ''),
        ('epf_rate',        '8.00'),   -- employee contribution, stored as percentage
        ('etf_rate',        '3.00');   -- employer contribution, stored as percentage
END
GO
