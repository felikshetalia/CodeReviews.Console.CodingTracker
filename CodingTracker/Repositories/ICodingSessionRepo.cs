namespace CodeReviews.Console.CodingTracker;

public interface ICodingSessionRepo
{
    // CRUD methods
    List<CodingSession> GetAll();
    CodingSession? GetOne(long id);
    void Add(CodingSession session);
    bool Update(CodingSession session);
    bool Delete(long id);

    // Filtering methods
    List<CodingSession> GetSessionsByDay(DateTime day);
    List<CodingSession> GetSessionsInBetweenDates(DateTime start, DateTime end);
    List<CodingSession> GetSessionsByMonthOfYear(int year, int monthIndicator);
    List<CodingSession> GetSessionsByYear(int year);
}