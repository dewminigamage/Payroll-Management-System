using System.Data;
using Microsoft.Data.SqlClient;

namespace PayrollManagementSystem.Database;

public static class DatabaseHelper
{
    // Update "Server" to match your SQL Server instance.
    // Common values: "localhost\\SQLEXPRESS"  |  ".\\SQLEXPRESS"  |  "(localdb)\\MSSQLLocalDB"
    private const string ConnectionString =
        "Server=LAPTOP-K5R81VS7\\SQLEXPRESS01;Database=PayrollDB;Integrated Security=True;TrustServerCertificate=True;";

    public static SqlConnection GetConnection() => new(ConnectionString);

    public static DataTable ExecuteQuery(string query, SqlParameter[]? parameters = null)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand(query, conn);
        if (parameters != null) cmd.Parameters.AddRange(parameters);
        using var adapter = new SqlDataAdapter(cmd);
        var table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    public static int ExecuteNonQuery(string query, SqlParameter[]? parameters = null)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new SqlCommand(query, conn);
        if (parameters != null) cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteNonQuery();
    }
}
