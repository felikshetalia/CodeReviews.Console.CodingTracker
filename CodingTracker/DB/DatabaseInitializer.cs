using Dapper;
namespace CodeReviews.Console.CodingTracker;

public sealed class DatabaseInitializer
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    public DatabaseInitializer(IDatabaseConnectionFactory _cf) => _connectionFactory = _cf;

    public void Initialize()
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS CodingSessions
            (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StartTime TEXT NOT NULL,
                EndTime TEXT NOT NULL
            );
            """;

        using var connection = _connectionFactory.CreateConnection();

        connection.Open();
        connection.Execute(sql);
        connection.Close();
    }
}