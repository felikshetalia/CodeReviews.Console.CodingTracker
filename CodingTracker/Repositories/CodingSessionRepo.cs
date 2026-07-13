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
                if (!TryParseDateFromString(row.StartTime, out var startTime) ||
                    !TryParseDateFromString(row.EndTime, out var endTime))
                {
                    continue;
                }

                sessions.Add(new CodingSession
                {
                    Id = row.Id,
                    StartTime = startTime,
                    EndTime = endTime,
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
            : TryParseDateFromString(record.StartTime, out var startTime) &&
              TryParseDateFromString(record.EndTime, out var endTime)
                ? new CodingSession
                {
                    Id = record.Id,
                    StartTime = startTime,
                    EndTime = endTime,
                }
                : null;
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

    private static bool TryParseDateFromString(string? value, out DateTime result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return DateTime.TryParseExact(
            value,
            _dateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out result);
    }

    private static string FormatDateTimeToString(DateTime obj)
        => obj.ToString(_dateFormat, CultureInfo.InvariantCulture);

}