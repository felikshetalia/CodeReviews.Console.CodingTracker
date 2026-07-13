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
}