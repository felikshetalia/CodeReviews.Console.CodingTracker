using CodeReviews.Console.CodingTracker;

internal sealed class FakeAppView : IAppView
{
    public Queue<MenuOption> OptionsToReturn { get; } = new();
    public int DisplayGoodbyeCalls { get; private set; }

    public MenuOption DisplayMainMenu() => OptionsToReturn.Count > 0
        ? OptionsToReturn.Dequeue()
        : MenuOption.Close;

    public void DisplayMessage(string message) { }
    public void DisplayGoodbye() => DisplayGoodbyeCalls++;
}