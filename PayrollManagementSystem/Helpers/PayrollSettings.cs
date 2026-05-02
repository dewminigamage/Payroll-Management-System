using System.Data;
using PayrollManagementSystem.Database;

namespace PayrollManagementSystem.Helpers;

public static class PayrollSettings
{
    public static string  CompanyName    { get; private set; } = "Payroll Management System";
    public static string  CompanyAddress { get; private set; } = "";
    public static string  CompanyPhone   { get; private set; } = "";
    public static string  CompanyEmail   { get; private set; } = "";
    public static string  GeminiApiKey  { get; private set; } = "";
    public static string  GroqApiKey    { get; private set; } = "";

    public static decimal EpfRate            { get; private set; } = 0.08m;
    public static decimal EtfRate            { get; private set; } = 0.03m;
    public static int     WorkingHoursPerMonth { get; private set; } = 160;

    public static string EpfLabel => $"{EpfRate * 100:0.##}%";
    public static string EtfLabel => $"{EtfRate * 100:0.##}%";

    // ── Tax Brackets ──────────────────────────────────────────────────────
    private static readonly List<(decimal Min, decimal Max, decimal Rate)> _taxBrackets = new();

    public static IReadOnlyList<(decimal Min, decimal Max, decimal Rate)> TaxBrackets
        => _taxBrackets.AsReadOnly();

    /// <summary>Marginal/progressive tax on monthly gross salary.</summary>
    public static decimal CalculateTax(decimal grossMonthly)
    {
        if (grossMonthly <= 0 || _taxBrackets.Count == 0) return 0m;
        decimal tax = 0m;
        foreach (var b in _taxBrackets)
        {
            if (grossMonthly <= b.Min) break;
            decimal upper = b.Max == decimal.MaxValue
                ? grossMonthly
                : Math.Min(grossMonthly, b.Max);
            tax += Math.Round((upper - b.Min) * b.Rate, 2);
        }
        return Math.Round(tax, 2);
    }

    // ── Reload ────────────────────────────────────────────────────────────
    public static void Reload()
    {
        try
        {
            var dt = DatabaseHelper.ExecuteQuery("SELECT SettingKey, SettingValue FROM CompanySettings");
            var d  = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow row in dt.Rows)
                d[row["SettingKey"].ToString()!] = row["SettingValue"]?.ToString() ?? "";

            CompanyName    = d.GetValueOrDefault("company_name",    "Payroll Management System");
            CompanyAddress = d.GetValueOrDefault("company_address", "");
            CompanyPhone   = d.GetValueOrDefault("company_phone",   "");
            CompanyEmail   = d.GetValueOrDefault("company_email",   "");
            GeminiApiKey   = d.GetValueOrDefault("gemini_api_key",  "");
            GroqApiKey     = d.GetValueOrDefault("groq_api_key",    "");

            EpfRate = decimal.TryParse(d.GetValueOrDefault("epf_rate", "8"), out var epf)
                ? epf / 100m : 0.08m;
            EtfRate = decimal.TryParse(d.GetValueOrDefault("etf_rate", "3"), out var etf)
                ? etf / 100m : 0.03m;
            WorkingHoursPerMonth = int.TryParse(
                d.GetValueOrDefault("working_hours_per_month", "160"), out var wh)
                ? Math.Max(1, wh) : 160;
        }
        catch { }

        ReloadTaxBrackets();
    }

    private static void ReloadTaxBrackets()
    {
        try
        {
            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT MinIncome, MaxIncome, TaxRate FROM TaxBrackets ORDER BY DisplayOrder");
            _taxBrackets.Clear();
            foreach (DataRow row in dt.Rows)
            {
                decimal min  = Convert.ToDecimal(row["MinIncome"]);
                decimal max  = row["MaxIncome"] == DBNull.Value
                    ? decimal.MaxValue
                    : Convert.ToDecimal(row["MaxIncome"]);
                decimal rate = Convert.ToDecimal(row["TaxRate"]);
                _taxBrackets.Add((min, max, rate));
            }
        }
        catch { }
    }

    // ── Save company + rates ──────────────────────────────────────────────
    public static void Save(string companyName, string address, string phone,
                            string email, decimal epfPct, decimal etfPct, int workingHours)
    {
        Upsert("company_name",            companyName);
        Upsert("company_address",         address);
        Upsert("company_phone",           phone);
        Upsert("company_email",           email);
        Upsert("epf_rate",                epfPct.ToString("0.00"));
        Upsert("etf_rate",                etfPct.ToString("0.00"));
        Upsert("working_hours_per_month", workingHours.ToString());
        Reload();
    }

    // ── Save tax brackets ─────────────────────────────────────────────────
    public static void SaveTaxBrackets(
        IEnumerable<(decimal Min, decimal? Max, decimal Rate, int Order)> brackets)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();
        using (var del = new Microsoft.Data.SqlClient.SqlCommand("DELETE FROM TaxBrackets", conn, tx))
            del.ExecuteNonQuery();
        const string ins = @"
            INSERT INTO TaxBrackets (MinIncome, MaxIncome, TaxRate, DisplayOrder)
            VALUES (@Min, @Max, @Rate, @Ord)";
        foreach (var b in brackets)
        {
            using var cmd = new Microsoft.Data.SqlClient.SqlCommand(ins, conn, tx);
            cmd.Parameters.AddWithValue("@Min", b.Min);
            cmd.Parameters.AddWithValue("@Max",
                b.Max.HasValue ? (object)b.Max.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@Rate", b.Rate);
            cmd.Parameters.AddWithValue("@Ord",  b.Order);
            cmd.ExecuteNonQuery();
        }
        tx.Commit();
        ReloadTaxBrackets();
    }

    // ── Save AI API keys ──────────────────────────────────────────────────
    public static void SaveGeminiApiKey(string apiKey)
    {
        Upsert("gemini_api_key", apiKey);
        GeminiApiKey = apiKey;
    }

    public static void SaveGroqApiKey(string apiKey)
    {
        Upsert("groq_api_key", apiKey);
        GroqApiKey = apiKey;
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
