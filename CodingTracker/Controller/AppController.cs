using Spectre.Console;
namespace CodeReviews.Console.CodingTracker;

public sealed class AppController
{
    private readonly IAppView _appView;

    public AppController(IAppView _view) => _appView = _view;

    public void Run()
    {
        bool isRunning = true;

        while (isRunning)
        {
            MenuOption selectedOption = _appView.DisplayMainMenu();

            switch (selectedOption)
            {
                case MenuOption.View:
                    ShowPlaceholder("View sessions");
                    break;

                case MenuOption.Add:
                    ShowPlaceholder("Add session");
                    break;

                case MenuOption.Edit:
                    ShowPlaceholder("Update session");
                    break;

                case MenuOption.Delete:
                    ShowPlaceholder("Delete session");
                    break;

                case MenuOption.Close:
                    isRunning = false;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(selectedOption),
                        selectedOption,
                        "Unknown menu option.");
            }
        }
    }

    private static void ShowPlaceholder(string action)
    {
        AnsiConsole.WriteLine($"{action} is not implemented yet.");
        AnsiConsole.WriteLine("Press any key to return to the menu.");
        AnsiConsole.Console.Input.ReadKey(true);
    }
}