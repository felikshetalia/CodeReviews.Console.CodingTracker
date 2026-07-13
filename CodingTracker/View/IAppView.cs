namespace CodeReviews.Console.CodingTracker;

public interface IAppView
{
    MenuOption DisplayMainMenu();
    void DisplayMessage(string message);
    void DisplayGoodbye();
}