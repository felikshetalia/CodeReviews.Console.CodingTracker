using CodeReviews.Console.CodingTracker;

internal sealed class FakeCodingSessionView : ICodingSessionView
{
    public List<ViewEvent> Events { get; } = [];
    public AddSessionOption AddSessionOptionToReturn { get; set; }
    public Queue<FilterOption> FilterOptionsToReturn { get; } = new();
    public DateTime FilterDateToReturn { get; set; }
    public Month MonthToReturn { get; set; }
    public int YearToReturn { get; set; }
    public long SessionIdToReturn { get; set; }
    public (DateTime StartTime, DateTime EndTime) SessionTimesToReturn { get; set; }

    public int DisplaySessionsCalls { get; private set; }
    public List<List<CodingSession>> DisplayedSessions { get; } = new();
    public int DisplayFilterMenuCalls { get; private set; }
    public int GetSessionIdCalls { get; private set; }
    public int GetSessionTimesCalls { get; private set; }
    public int DisplayMessageCount { get; private set; }
    public int DisplayErrorCount { get; private set; }
    public bool WaitStopwatchCalled { get; private set; }
    public string? LastMessage { get; private set; }
    public string? LastError { get; private set; }

    public void DisplaySessions(List<CodingSession> sessions)
    {
        Events.Add(ViewEvent.DisplaySessions);
        DisplaySessionsCalls++;
        DisplayedSessions.Add(new List<CodingSession>(sessions));
    }

    public (DateTime StartTime, DateTime EndTime) GetSessionTimes()
    {
        Events.Add(ViewEvent.GetSessionTimes);
        GetSessionTimesCalls++;
        return SessionTimesToReturn;
    }

    public long GetSessionId(string prompt)
    {
        Events.Add(ViewEvent.GetSessionId);
        GetSessionIdCalls++;
        return SessionIdToReturn;
    }

    public void DisplayMessage(string message)
    {
        DisplayMessageCount++;
        LastMessage = message;
    }

    public void DisplayError(string message)
    {
        DisplayErrorCount++;
        LastError = message;
    }

    public Month GetMonth() => MonthToReturn;
    public int GetYear() => YearToReturn;

    public FilterOption DisplayFilterMenu()
    {
        DisplayFilterMenuCalls++;
        if (FilterOptionsToReturn.Count == 0)
            throw new InvalidOperationException("No filter option was configured for this test.");

        return FilterOptionsToReturn.Dequeue();
    }

    public DateTime GetFilterDate(string prompt) => FilterDateToReturn;

    public AddSessionOption DisplayAddSessionMenu() => AddSessionOptionToReturn;

    public void WaitStopwatch() => WaitStopwatchCalled = true;
}