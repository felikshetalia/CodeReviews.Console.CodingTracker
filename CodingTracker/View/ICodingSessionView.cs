namespace CodeReviews.Console.CodingTracker;

public interface ICodingSessionView
{
    void DisplaySessions(List<CodingSession> sessions);
    (DateTime StartTime, DateTime EndTime) GetSessionTimes();
    long GetSessionId(string prompt);
    void DisplayMessage(string message);
    void DisplayError(string message);
    Month GetMonth();
    int GetYear();
    FilterOption DisplayFilterMenu();
    DateTime GetFilterDate(string prompt);
}