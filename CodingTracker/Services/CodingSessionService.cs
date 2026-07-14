namespace CodeReviews.Console.CodingTracker;

public sealed class CodingSessionService : ICodingSessionService
{
    private readonly ICodingSessionRepo _repository;
    public CodingSessionService(ICodingSessionRepo repo) => _repository = repo;

    public void Add(DateTime startTime, DateTime endTime)
    {
        Validators.ValidateSessionTimes(startTime, endTime);
        _repository.Add(new CodingSession
        {
            StartTime = startTime,
            EndTime = endTime
        });
    }

    public bool Delete(long id) => id <= 0 ? false : _repository.Delete(id);

    public List<CodingSession> GetAll() => _repository.GetAll();

    public CodingSession? GetOne(long id) => id <= 0 ? null : _repository.GetOne(id);

    public bool Update(long id, DateTime startTime, DateTime endTime)
    {
        if (id <= 0) return false;

        Validators.ValidateSessionTimes(startTime, endTime);

        CodingSession? session =
            _repository.GetOne(id);

        if (session is null) return false;

        session.StartTime = startTime;
        session.EndTime = endTime;

        return _repository.Update(session);
    }
    public List<CodingSession> GetByDay(DateTime date) => _repository.GetSessionsByDay(date);
    public List<CodingSession> GetByWeek(DateTime date)
    {
        int daysSinceMonday = ((int)date.DayOfWeek + 6) % 7;
        DateTime startOfWeek = date.Date.AddDays(-daysSinceMonday);
        DateTime endOfWeek = startOfWeek.AddDays(7);

        return _repository.GetSessionsInBetweenDates(startOfWeek, endOfWeek);
    }

    public List<CodingSession> GetByMonthOfYear(int year, int month)
    {
        if (year < 1 || year > 9999)
            throw new ArgumentOutOfRangeException(nameof(year));

        if (month < 1 || month > 12)
            throw new ArgumentOutOfRangeException(nameof(month));

        return _repository.GetSessionsByMonthOfYear(year, month);
    }
    public List<CodingSession> GetByYear(int year)
    {
        if (year < 1 || year > 9999)
            throw new ArgumentOutOfRangeException(nameof(year));

        return _repository.GetSessionsByYear(year);
    }
}