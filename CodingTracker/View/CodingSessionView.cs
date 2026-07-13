
using System.Globalization;
using Spectre.Console;
namespace CodeReviews.Console.CodingTracker;

public sealed class CodingSessionView : ICodingSessionView
{
    private const string _inputDateFormat = "dd-MM-yyyy HH:mm";
    public void DisplayError(string message)
    {
        AnsiConsole.MarkupLine($"[red]{Markup.Escape(message)}[/]");
        WaitForInput();
    }

    public void DisplayMessage(string message)
        => AnsiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");

    public void DisplaySessions(List<CodingSession> sessions)
    {
        AnsiConsole.Clear();

        if (sessions.Count == 0)
        {
            DisplayMessage("No records found.");
            WaitForInput();
            return;
        }

        var table = new Table()
            .AddColumn("Id")
            .AddColumn("Start time")
            .AddColumn("End time")
            .AddColumn("Duration");

        foreach (var row in sessions)
        {
            table.AddRow(
                row.Id.ToString(),
                row.StartTime.ToString(_inputDateFormat),
                row.EndTime.ToString(_inputDateFormat),
                FormatDuration(row.Duration));
        }

        AnsiConsole.Write(table);
        WaitForInput();
    }

    public long GetSessionId(string prompt)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<long>(prompt)
                .ValidationErrorMessage(
                    "[red]Enter a valid numeric ID.[/]")
                .Validate(id =>
                    id > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error(
                            "[red]The ID must be positive.[/]")));
    }

    public (DateTime StartTime, DateTime EndTime) GetSessionTimes()
    {
        DateTime startTime = GetDateTime("Enter the start time:");
        DateTime endTime = GetDateTime("Enter the end time:");

        return (startTime, endTime);
    }

    private static DateTime GetDateTime(string prompt)
    {
        while (true)
        {
            string input =
                AnsiConsole.Ask<string>(
                    $"{prompt} [grey]({_inputDateFormat})[/]");

            bool isValid = DateTime.TryParseExact(
                input,
                _inputDateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime result);

            if (isValid) return result;

            AnsiConsole.MarkupLine(
                $"[red]Use the format {_inputDateFormat}.[/]");
        }
    }
    private static string FormatDuration(TimeSpan duration)
        => $"{(int)duration.TotalHours:D2}" + $":{duration.Minutes:D2}";


    private static void WaitForInput()
    {
        AnsiConsole.MarkupLine("\n[grey]Press any key to continue.[/]");
        AnsiConsole.Console.Input.ReadKey(true);
    }
}