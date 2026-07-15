using CodeReviews.Console.CodingTracker;
internal sealed class FakeCodingSessionController : ICodingSessionController
{
    public int ViewSessionsCalls { get; private set; }
    public int AddSessionCalls { get; private set; }
    public int UpdateSessionCalls { get; private set; }
    public int DeleteSessionCalls { get; private set; }

    public void ViewSessions() => ViewSessionsCalls++;
    public void AddSession() => AddSessionCalls++;
    public void UpdateSession() => UpdateSessionCalls++;
    public void DeleteSession() => DeleteSessionCalls++;
}