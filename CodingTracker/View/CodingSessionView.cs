
using System.Globalization;
using Spectre.Console;
namespace CodeReviews.Console.CodingTracker;

public sealed class CodingSessionView : ICodingSessionView
{
    private const string _inputDateFormat = "dd-MM-yyyy HH:mm";
    private const string _filterDateFormat = "dd-MM-yyyy";
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
        DateTime startTime = GetDateTime("Enter the start time:", _inputDateFormat);
        DateTime endTime = GetDateTime("Enter the end time:", _inputDateFormat);

        return (startTime, endTime);
    }

    private static DateTime GetDateTime(string prompt, string format)
    {
        while (true)
        {
            string input = AnsiConsole.Ask<string>(
                $"{prompt} [grey]({format})[/]");

            if (DateTime.TryParseExact(
                    input,
                    format,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime result))
            {
                return result;
            }

            AnsiConsole.MarkupLine($"[red]Use the format {format}.[/]");
        }
    }
    private static string FormatDuration(TimeSpan duration)
        => $"{(int)duration.TotalHours:D2}" + $":{duration.Minutes:D2}";


    private static void WaitForInput()
    {
        AnsiConsole.MarkupLine("\n[grey]Press any key to continue.[/]");
        AnsiConsole.Console.Input.ReadKey(true);
    }

    public Month GetMonth()
        => AnsiConsole.Prompt(
            new SelectionPrompt<Month>()
                .Title("Select a month:")
                .AddChoices(Enum.GetValues<Month>()));

    public int GetYear()
        => AnsiConsole.Prompt(
            new TextPrompt<int>("Enter the year:")
                .Validate(year =>
                    year is >= 1 and <= 9999
                        ? ValidationResult.Success()
                        : ValidationResult.Error(
                            "[red]Enter a valid year.[/]")));

    public FilterOption DisplayFilterMenu()
        => AnsiConsole.Prompt(
            new SelectionPrompt<FilterOption>()
                .Title("\nHow would you like to filter the sessions?")
                .AddChoices(Enum.GetValues<FilterOption>()));

    public DateTime GetFilterDate(string prompt) => GetDateTime(prompt, _filterDateFormat);
}