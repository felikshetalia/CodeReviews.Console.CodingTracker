using Microsoft.Data.Sqlite;

namespace CodeReviews.Console.CodingTracker;

public sealed class CodingSessionController : ICodingSessionController
{
    private readonly ICodingSessionService _codingService;
    private readonly ICodingSessionView _codingView;

    public CodingSessionController(ICodingSessionService service, ICodingSessionView view)
    {
        _codingService = service;
        _codingView = view;
    }
    public void AddSession()
    {
        var (startTime, endTime) = _codingView.GetSessionTimes();

        try
        {
            _codingService.Add(startTime, endTime);
            _codingView.DisplayMessage("Coding session added successfully.");
        }
        catch (ArgumentException ex)
        {
            _codingView.DisplayError(ex.Message);
        }
    }

    public void DeleteSession()
    {
        long id = _codingView.GetSessionId("Enter the ID of the session to delete:");
        bool deleted;
        try
        {
            deleted = _codingService.Delete(id);
            _codingView.DisplayMessage("Coding session deleted successfully.");
        }
        catch (SqliteException ex)
        {
            _codingView.DisplayError($"Unexpected SQLite error {ex.SqliteErrorCode}: {ex.Message}");
        }
    }

    public void UpdateSession()
    {
        long id = _codingView.GetSessionId("Enter the ID of the session to update:");
        var (startTime, endTime) = _codingView.GetSessionTimes();

        try
        {
            bool updated = _codingService.Update(id, startTime, endTime);
            if (!updated)
            {
                _codingView.DisplayError($"Coding session {id} was not found.");
                return;
            }
            _codingView.DisplayMessage("Coding session updated successfully.");
        }
        catch (ArgumentException ex)
        {
            _codingView.DisplayError(ex.Message);
        }
    }

    public void ViewSessions()
    {
        List<CodingSession> sessions;

        try
        {
            sessions = _codingService.GetAll();
            _codingView.DisplaySessions(sessions);
        }
        catch (SqliteException ex)
        {
            _codingView.DisplayError($"Unexpected SQLite error {ex.SqliteErrorCode}: {ex.Message}");
        }
    }
}