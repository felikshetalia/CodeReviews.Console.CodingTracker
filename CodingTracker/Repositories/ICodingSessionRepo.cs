namespace CodeReviews.Console.CodingTracker;

public interface ICodingSessionRepo
{
    List<CodingSession> GetAll();
    CodingSession? GetOne(long id);
    void Add(CodingSession session);
    void Update(CodingSession session);
    void Delete(long id);
}