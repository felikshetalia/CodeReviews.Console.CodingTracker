// data access layer: write sql queries here
namespace CodeReviews.Console.CodingTracker;

using Dapper;
using Spectre.Console;
using System.Globalization;
using System.Linq;

public sealed class CodingSessionRepo : ICodingSessionRepo
{
    private const string _dateFormat = "yyyy-MM-dd HH:mm:ss";
    private readonly IDatabaseConnectionFactory _connectionFactory;

    public CodingSessionRepo(IDatabaseConnectionFactory _cf) => _connectionFactory = _cf;

    public List<CodingSession> GetAll()
    {
        List<CodingSession> sessions = new();

        const string query = @"
            SELECT Id, StartTime, EndTime
            FROM CodingSessions
            ORDER BY StartTime DESC;
        ";

        using (var connection = _connectionFactory.CreateConnection())
        {
            connection.Open();

            IEnumerable<CodingSessionDTO> rows =
                connection.Query<CodingSessionDTO>(query);

            foreach (var row in rows)
            {
                sessions.Add(new CodingSession
                {
                    Id = (int)row.Id,
                    StartTime = ParseDateFromString(row.StartTime),
                    EndTime = ParseDateFromString(row.EndTime),
                });
            }

            connection.Close();
        }

        return sessions;
    }
    public CodingSession? GetOne(long id)
    {
        CodingSession? session;
        const string query = @"
            SELECT Id, StartTime, EndTime
            FROM CodingSessions
            WHERE Id = @Id;
        ";

        using (var connection = _connectionFactory.CreateConnection())
        {
            connection.Open();

            var record = connection.QuerySingleOrDefault<CodingSessionDTO>(query, new { Id = id });

            session = record == null
            ? null
            : new CodingSession
            {
                Id = (int)record.Id,
                StartTime = ParseDateFromString(record.StartTime),
                EndTime = ParseDateFromString(record.EndTime),
            };
            connection.Close();
        }

        return session;
    }
    public void Add(CodingSession session)
    {
        if (session == null)
            throw new ArgumentNullException();

        const string query = @"
            INSERT INTO CodingSessions (StartTime, EndTime)
            VALUES (@StartTime, @EndTime)
        ";

        using (var connection = _connectionFactory.CreateConnection())
        {
            var dto = new CodingSessionDTO
            {
                StartTime = FormatDateTimeToString(session.StartTime),
                EndTime = FormatDateTimeToString(session.EndTime)
            };
            connection.Open();
            connection.Execute(query, dto);
            connection.Close();
        }

    }
    public void Delete(long id)
    {
        const string query = @"
            DELETE FROM CodingSessions
            WHERE Id = @Id;
        ";

        using (var connection = _connectionFactory.CreateConnection())
        {
            connection.Open();
            connection.Execute(query, new { Id = id });
            connection.Close();
        }
    }
    public void Update(CodingSession session)
    {
        if (session == null)
            throw new ArgumentNullException();

        const string query = @"
            UPDATE CodingSessions
            SET StartTime = @StartTime, EndTime = @EndTime
            WHERE Id = @Id;
        ";

        using (var connection = _connectionFactory.CreateConnection())
        {
            var dto = new CodingSessionDTO
            {
                StartTime = FormatDateTimeToString(session.StartTime),
                EndTime = FormatDateTimeToString(session.EndTime)
            };
            connection.Open();
            connection.Execute(query, dto);
            connection.Close();
        }
    }

    private static DateTime ParseDateFromString(string value)
        => DateTime.ParseExact(
            value,
            _dateFormat,
            CultureInfo.InvariantCulture);


    private static string FormatDateTimeToString(DateTime obj)
        => obj.ToString(_dateFormat, CultureInfo.InvariantCulture);

}