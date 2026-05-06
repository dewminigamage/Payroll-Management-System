using System.Data;
using Microsoft.Data.SqlClient;

namespace PayrollManagementSystem.API.Helpers;

public class DatabaseHelper
{
    private readonly string _cs;
    public DatabaseHelper(IConfiguration config) => _cs = config.GetConnectionString("PayrollDB")!;

    public SqlConnection GetConnection() => new(_cs);

    public async Task<DataTable> QueryAsync(string sql, params SqlParameter[] p)
    {
        await using var conn = GetConnection();
        await using var cmd  = new SqlCommand(sql, conn);
        if (p.Length > 0) cmd.Parameters.AddRange(p);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        var dt = new DataTable();
        dt.Load(reader);
        return dt;
    }

    public async Task<int> NonQueryAsync(string sql, params SqlParameter[] p)
    {
        await using var conn = GetConnection();
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        if (p.Length > 0) cmd.Parameters.AddRange(p);
        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<object?> ScalarAsync(string sql, params SqlParameter[] p)
    {
        await using var conn = GetConnection();
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        if (p.Length > 0) cmd.Parameters.AddRange(p);
        return await cmd.ExecuteScalarAsync();
    }

    public static T Get<T>(DataRow row, string col, T def = default!)
    {
        var v = row[col];
        if (v == DBNull.Value || v is null) return def;
        if (v is T t) return t;
        return (T)Convert.ChangeType(v, typeof(T));
    }

    public static T? GetNullable<T>(DataRow row, string col) where T : struct
    {
        var v = row[col];
        return v == DBNull.Value || v is null ? null : (T)Convert.ChangeType(v, typeof(T));
    }

    public static string? GetString(DataRow row, string col)
    {
        var v = row[col];
        return v == DBNull.Value ? null : v?.ToString();
    }
}
