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
        AddSessionOption opt = _codingView.DisplayAddSessionMenu();

        switch (opt)
        {
            case AddSessionOption.Manual:
                AddManualSession();
                break;

            case AddSessionOption.Stopwatch:
                AddTimedSession();
                break;

            case AddSessionOption.Back:
                return;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(opt),
                    opt,
                    "Unknown add-session option.");
        }
    }

    public void DeleteSession()
    {
        List<CodingSession>? sessions = GetSessionsSafely(_codingService.GetAll);
        if (sessions == null || sessions.Count == 0)
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
        List<CodingSession>? sessions = GetSessionsSafely(_codingService.GetAll);
        if (sessions == null || sessions.Count == 0)
        {
            _codingView.DisplayMessage("No coding sessions available to update.");
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

    public void ViewSessions()
    {
        List<CodingSession>? sessions = GetSessionsSafely(_codingService.GetAll);
        if (sessions is null) return;
        if (sessions.Count == 0) return;

        while (true)
        {
            FilterOption opt = _codingView.DisplayFilterMenu();
            if (opt == FilterOption.Back) return;

            List<CodingSession>? filtered = GetFilteredSessions(opt);

            if (filtered == null) return;

            sessions = filtered;
            _codingView.DisplaySessions(sessions);
        }

    }

    private List<CodingSession>? GetSessionsSafely(Func<List<CodingSession>> retrieval)
    {
        try
        {
            return retrieval();
        }
        catch (Exception ex)
        {
            _codingView.DisplayError($"Unexpected error: {ex.Message}");
            return null;
        }
    }
    private List<CodingSession>? GetFilteredSessions(FilterOption option)
    {
        switch (option)
        {
            case FilterOption.ShowAll:
                return GetSessionsSafely(_codingService.GetAll);
            case FilterOption.Day:
                {
                    DateTime date = _codingView.GetFilterDate("Enter the date:");
                    return GetSessionsSafely(() => _codingService.GetByDay(date));
                }
            case FilterOption.Week:
                {
                    DateTime date = _codingView.GetFilterDate("Enter any date within the desired week:");
                    return GetSessionsSafely(() => _codingService.GetByWeek(date));
                }
            case FilterOption.Month:
                {
                    Month month = _codingView.GetMonth();
                    int year = _codingView.GetYear();
                    return GetSessionsSafely(() => _codingService.GetByMonthOfYear(year, (int)month));
                }

            case FilterOption.Year:
                {
                    int year = _codingView.GetYear();
                    return GetSessionsSafely(() => _codingService.GetByYear(year));
                }

            default:
                throw new ArgumentOutOfRangeException(nameof(option), option, "Unknown filter option.");
        }
    }

    private void AddManualSession()
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

    private void AddTimedSession()
    {
        DateTime startTime = DateTime.Now;

        _codingView.WaitStopwatch();

        DateTime endTime = DateTime.Now;

        try
        {
            _codingService.Add(startTime, endTime);
            _codingView.DisplayMessage($"Session recorded. Duration: " + $"{endTime - startTime:hh\\:mm\\:ss}");
        }
        catch (ArgumentException ex)
        {
            _codingView.DisplayError(ex.Message);
        }
    }
}