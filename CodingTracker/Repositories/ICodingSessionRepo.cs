namespace CodeReviews.Console.CodingTracker;

public interface ICodingSessionRepo
{
    List<CodingSession> GetAll();
    CodingSession? GetOne(long id);
    void Add(CodingSession session);
    bool Update(CodingSession session);
    bool Delete(long id);
}