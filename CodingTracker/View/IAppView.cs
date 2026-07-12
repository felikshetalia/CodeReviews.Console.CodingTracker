namespace CodeReviews.Console.CodingTracker;

public interface IAppView
{
    MenuOption DisplayMainMenu();
    void DisplaySessions(List<CodingSession> sessions);
    void DisplayMessage(string message);
    void DisplayGoodbye();
}