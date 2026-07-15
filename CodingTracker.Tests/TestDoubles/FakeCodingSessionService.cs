using CodeReviews.Console.CodingTracker;

internal sealed class FakeCodingSessionService : ICodingSessionService
{
    public List<CodingSession> SessionsToReturn { get; set; } = new();
    public List<CodingSession>? SessionsToReturnForGetOne { get; set; }
    public List<CodingSession>? SessionsToReturnForGetByDay { get; set; }
    public List<CodingSession>? SessionsToReturnForGetByWeek { get; set; }
    public List<CodingSession>? SessionsToReturnForGetByMonth { get; set; }
    public List<CodingSession>? SessionsToReturnForGetByYear { get; set; }

    public bool AddCalled { get; private set; }
    public DateTime? AddedStartTime { get; private set; }
    public DateTime? AddedEndTime { get; private set; }
    public Exception? AddExceptionToThrow { get; set; }
    public Exception? FilterExceptionToThrow { get; set; }

    public int GetAllCalls { get; private set; }
    public Exception? GetAllExceptionToThrow { get; set; }

    public long? DeletedId { get; private set; }
    public bool DeleteResult { get; set; } = true;
    public Exception? DeleteExceptionToThrow { get; set; }

    public long? UpdatedId { get; private set; }
    public DateTime? UpdatedStartTime { get; private set; }
    public DateTime? UpdatedEndTime { get; private set; }
    public bool UpdateResult { get; set; } = true;
    public Exception? UpdateExceptionToThrow { get; set; }

    public DateTime? DayArgument { get; private set; }
    public DateTime? WeekArgument { get; private set; }
    public (int Year, int Month)? MonthArguments { get; private set; }
    public int? YearArgument { get; private set; }

    public void Add(DateTime startTime, DateTime endTime)
    {
        AddCalled = true;
        AddedStartTime = startTime;
        AddedEndTime = endTime;
        if (AddExceptionToThrow != null)
        {
            throw AddExceptionToThrow;
        }
    }

    public bool Delete(long id)
    {
        if (DeleteExceptionToThrow != null)
        {
            throw DeleteExceptionToThrow;
        }
        DeletedId = id;
        return DeleteResult;
    }

    public List<CodingSession> GetAll()
    {
        if (GetAllExceptionToThrow != null)
        {
            throw GetAllExceptionToThrow;
        }
        GetAllCalls++;
        return SessionsToReturn;
    }

    public CodingSession? GetOne(long id)
    {
        if (SessionsToReturnForGetOne != null)
        {
            return SessionsToReturnForGetOne.SingleOrDefault(x => x.Id == id);
        }
        return SessionsToReturn.SingleOrDefault(x => x.Id == id);
    }

    public bool Update(long id, DateTime startTime, DateTime endTime)
    {
        if (UpdateExceptionToThrow != null)
        {
            throw UpdateExceptionToThrow;
        }
        UpdatedId = id;
        UpdatedStartTime = startTime;
        UpdatedEndTime = endTime;
        return UpdateResult;
    }

    public List<CodingSession> GetByDay(DateTime date)
    {
        if (FilterExceptionToThrow != null)
            throw FilterExceptionToThrow;

        DayArgument = date;

        return SessionsToReturnForGetByDay ?? SessionsToReturn;
    }

    public List<CodingSession> GetByWeek(DateTime date)
    {
        if (FilterExceptionToThrow != null)
            throw FilterExceptionToThrow;

        WeekArgument = date;

        return SessionsToReturnForGetByWeek ?? SessionsToReturn;
    }

    public List<CodingSession> GetByMonthOfYear(int year, int month)
    {
        if (FilterExceptionToThrow != null)
            throw FilterExceptionToThrow;

        MonthArguments = (year, month);

        return SessionsToReturnForGetByMonth ?? SessionsToReturn;
    }

    public List<CodingSession> GetByYear(int year)
    {
        if (FilterExceptionToThrow != null)
            throw FilterExceptionToThrow;

        YearArgument = year;

        return SessionsToReturnForGetByYear ?? SessionsToReturn;
    }
}