using CodeReviews.Console.CodingTracker;

internal sealed class CodingSessionRepoDouble : ICodingSessionRepo
{
    public List<CodingSession> SessionsToReturn { get; set; } = new();
    public List<CodingSession> AddedSessions { get; } = new();
    public long? DeletedId { get; private set; }
    public bool DeleteResult { get; set; } = true;
    public long? GetOneId { get; private set; }
    public long? UpdatedId { get; private set; }
    public CodingSession? UpdatedSession { get; private set; }
    public bool UpdateResult { get; set; } = true;
    public DateTime? DayForGetByDay { get; private set; }
    public (DateTime Start, DateTime End)? BetweenDatesArgs { get; private set; }
    public (int Year, int Month)? MonthOfYearArgs { get; private set; }
    public int? YearArg { get; private set; }

    public List<CodingSession> GetAll() => SessionsToReturn;

    public CodingSession? GetOne(long id)
    {
        GetOneId = id;
        return SessionsToReturn.SingleOrDefault(x => x.Id == id);
    }

    public void Add(CodingSession session)
    {
        AddedSessions.Add(session);
        SessionsToReturn.Add(session);
    }

    public bool Update(CodingSession session)
    {
        UpdatedId = session.Id;
        UpdatedSession = session;
        return UpdateResult;
    }

    public bool Delete(long id)
    {
        DeletedId = id;
        return DeleteResult;
    }

    public List<CodingSession> GetSessionsByDay(DateTime day)
    {
        DayForGetByDay = day;
        return SessionsToReturn;
    }

    public List<CodingSession> GetSessionsInBetweenDates(DateTime start, DateTime end)
    {
        BetweenDatesArgs = (start, end);
        return SessionsToReturn;
    }

    public List<CodingSession> GetSessionsByMonthOfYear(int year, int monthIndicator)
    {
        MonthOfYearArgs = (year, monthIndicator);
        return SessionsToReturn;
    }

    public List<CodingSession> GetSessionsByYear(int year)
    {
        YearArg = year;
        return SessionsToReturn;
    }
}