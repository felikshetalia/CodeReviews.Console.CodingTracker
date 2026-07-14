using CodeReviews.Console.CodingTracker;
using Microsoft.Data.Sqlite;
using Dapper;
namespace CodingTracker.Tests;

[TestFixture]
[NonParallelizable]
public abstract class RepositoryTestBase
{
    protected string dbPath;
    protected ICodingSessionRepo repository;

    [SetUp]
    public void SetUp()
    {
        dbPath = Path.Combine(Path.GetTempPath(), $"coding-tracker-tests-{Guid.NewGuid():N}.db");

        string connectionString = $"Data Source={dbPath};Pooling=False";

        IDatabaseConnectionFactory connectionFactory = new SQLiteConnectionFactory(connectionString);
        var initializer = new DatabaseInitializer(connectionFactory);

        initializer.Initialize();

        repository = new CodingSessionRepo(connectionFactory);
    }

    [TearDown]
    public void DeleteDatabase()
    {
        if (File.Exists(dbPath))
        {
            File.Delete(dbPath);
        }
    }
}
