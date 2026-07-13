// data access layer: write sql queries here
namespace CodeReviews.Console.CodingTracker;

using Dapper;
using System.Globalization;

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
                    Id = row.Id,
                    StartTime = ParseDateFromString(row.StartTime),
                    EndTime = ParseDateFromString(row.EndTime),
                });
            }
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
                Id = record.Id,
                StartTime = ParseDateFromString(record.StartTime),
                EndTime = ParseDateFromString(record.EndTime),
            };
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
        }

    }
    public bool Delete(long id)
    {
        const string query = @"
            DELETE FROM CodingSessions
            WHERE Id = @Id;
        ";

        int affectedRows;
        using (var connection = _connectionFactory.CreateConnection())
        {
            connection.Open();
            affectedRows = connection.Execute(query, new { Id = id });
        }
        return affectedRows == 1;
    }
    public bool Update(CodingSession session)
    {
        if (session == null)
            throw new ArgumentNullException();

        const string query = @"
            UPDATE CodingSessions
            SET StartTime = @StartTime, EndTime = @EndTime
            WHERE Id = @Id;
        ";

        int affectedRows;
        using (var connection = _connectionFactory.CreateConnection())
        {
            var dto = new CodingSessionDTO
            {
                Id = session.Id,
                StartTime = FormatDateTimeToString(session.StartTime),
                EndTime = FormatDateTimeToString(session.EndTime)
            };
            connection.Open();
            affectedRows = connection.Execute(query, dto);
        }
        return affectedRows == 1;
    }

    private static DateTime ParseDateFromString(string value)
        => DateTime.ParseExact(
            value,
            _dateFormat,
            CultureInfo.InvariantCulture);


    private static string FormatDateTimeToString(DateTime obj)
        => obj.ToString(_dateFormat, CultureInfo.InvariantCulture);

}