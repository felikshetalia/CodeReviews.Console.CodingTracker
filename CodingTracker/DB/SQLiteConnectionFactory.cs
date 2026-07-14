using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace CodeReviews.Console.CodingTracker;

public sealed class SQLiteConnectionFactory : IDatabaseConnectionFactory
{
    private readonly string _connectionString;

    public SQLiteConnectionFactory(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException();

        _connectionString = connectionString;
    }
    public DbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}