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
        if(!ViewSessions())
        {
            _codingView.DisplayMessage("No coding sessions available to delete.");
            return;
        }
        long id = _codingView.GetSessionId("Enter the ID of the session to delete:");
        try
        {
            bool deleted = _codingService.Delete(id);
            if (!deleted)
            {
                _codingView.DisplayError($"Coding session {id} was not found.");
                return;
            }
            _codingView.DisplayMessage("Coding session deleted successfully.");
        }
        catch (SqliteException ex)
        {
            _codingView.DisplayError($"Unexpected SQLite error {ex.SqliteErrorCode}: {ex.Message}");
        }
    }

    public void UpdateSession()
    {
        if (!ViewSessions())
        {
            _codingView.DisplayMessage("No coding sessions available to delete.");
            return;
        }
        long id = _codingView.GetSessionId("Enter the ID of the session to update:");

        CodingSession? session = _codingService.GetOne(id);
        if (session == null)
        {
            _codingView.DisplayError($"Coding session {id} was not found.");
            return;
        }

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

    public bool ViewSessions()
    {
        List<CodingSession> sessions;

        try
        {
            sessions = _codingService.GetAll();
            _codingView.DisplaySessions(sessions);
            return sessions.Count > 0;
        }
        catch (SqliteException ex)
        {
            _codingView.DisplayError($"Unexpected SQLite error {ex.SqliteErrorCode}: {ex.Message}");
        }
        return false;
    }
}