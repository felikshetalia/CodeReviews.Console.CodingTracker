namespace CodeReviews.Console.CodingTracker;

public interface ICodingSessionService
{
    List<CodingSession> GetAll();
    CodingSession? GetOne(long id);
    void Add(DateTime startTime, DateTime endTime);
    bool Update(long id, DateTime startTime, DateTime endTime);
    bool Delete(long id);
}