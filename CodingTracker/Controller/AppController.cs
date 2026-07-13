using Spectre.Console;
namespace CodeReviews.Console.CodingTracker;

public sealed class AppController
{
    private readonly IAppView _appView;
    private readonly ICodingSessionController _codingController;
    public AppController(IAppView _view, ICodingSessionController _controller)
    {
        _appView = _view;
        _codingController = _controller;
    }

    public void Run()
    {
        bool isRunning = true;

        while (isRunning)
        {
            MenuOption selectedOption = _appView.DisplayMainMenu();

            switch (selectedOption)
            {
                case MenuOption.View:
                    _codingController.ViewSessions();
                    break;

                case MenuOption.Add:
                    _codingController.AddSession();
                    break;

                case MenuOption.Edit:
                    _codingController.UpdateSession();
                    break;

                case MenuOption.Delete:
                    _codingController.DeleteSession();
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
        _appView.DisplayGoodbye();
    }
}