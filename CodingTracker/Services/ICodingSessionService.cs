namespace CodeReviews.Console.CodingTracker;

public interface ICodingSessionService
{
    List<CodingSession> GetAll();
    CodingSession? GetOne(long id);
    void Add(DateTime startTime, DateTime endTime);
    bool Update(long id, DateTime startTime, DateTime endTime);
    bool Delete(long id);
    List<CodingSession> GetByDay(DateTime date);
    List<CodingSession> GetByWeek(DateTime date);
    List<CodingSession> GetByMonthOfYear(int year, int month);
    List<CodingSession> GetByYear(int year);
}