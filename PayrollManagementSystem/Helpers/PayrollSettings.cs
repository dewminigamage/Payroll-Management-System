using System.Data;
using PayrollManagementSystem.Database;

namespace PayrollManagementSystem.Helpers;

/// <summary>
/// Cached company and payroll rate settings loaded from CompanySettings table.
/// Call Reload() once after login and after saving settings.
/// </summary>
public static class PayrollSettings
{
    public static string  CompanyName    { get; private set; } = "Payroll Management System";
    public static string  CompanyAddress { get; private set; } = "";
    public static string  CompanyPhone   { get; private set; } = "";
    public static string  CompanyEmail   { get; private set; } = "";

    // Stored as percentage values (e.g. 8.00 = 8%), exposed as decimal fractions (0.08)
    public static decimal EpfRate { get; private set; } = 0.08m;
    public static decimal EtfRate { get; private set; } = 0.03m;

    // Formatted labels for UI e.g. "8%", "3.5%"
    public static string EpfLabel => $"{EpfRate * 100:0.##}%";
    public static string EtfLabel => $"{EtfRate * 100:0.##}%";

    public static void Reload()
    {
        try
        {
            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT SettingKey, SettingValue FROM CompanySettings");

            var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow row in dt.Rows)
                d[row["SettingKey"].ToString()!] = row["SettingValue"]?.ToString() ?? "";

            CompanyName    = d.GetValueOrDefault("company_name",    "Payroll Management System");
            CompanyAddress = d.GetValueOrDefault("company_address", "");
            CompanyPhone   = d.GetValueOrDefault("company_phone",   "");
            CompanyEmail   = d.GetValueOrDefault("company_email",   "");

            // Convert from stored percentage (e.g. "8.00") to decimal fraction (0.08)
            EpfRate = decimal.TryParse(d.GetValueOrDefault("epf_rate", "8"), out var epf)
                ? epf / 100m : 0.08m;
            EtfRate = decimal.TryParse(d.GetValueOrDefault("etf_rate", "3"), out var etf)
                ? etf / 100m : 0.03m;
        }
        catch
        {
            // Keep defaults if the table doesn't exist yet (first run before SQL script)
        }
    }

    public static void Save(string companyName, string address, string phone,
                            string email, decimal epfPct, decimal etfPct)
    {
        Upsert("company_name",    companyName);
        Upsert("company_address", address);
        Upsert("company_phone",   phone);
        Upsert("company_email",   email);
        Upsert("epf_rate",        epfPct.ToString("0.00"));
        Upsert("etf_rate",        etfPct.ToString("0.00"));
        Reload();
    }

    private static void Upsert(string key, string value)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM CompanySettings WHERE SettingKey = @Key)
                UPDATE CompanySettings SET SettingValue = @Val WHERE SettingKey = @Key
            ELSE
                INSERT INTO CompanySettings (SettingKey, SettingValue) VALUES (@Key, @Val)";
        DatabaseHelper.ExecuteNonQuery(sql, [
            new Microsoft.Data.SqlClient.SqlParameter("@Key", key),
            new Microsoft.Data.SqlClient.SqlParameter("@Val", value)
        ]);
    }
}
