using System;
using Spectre.Console;

namespace CodeReviews.Console.CodingTracker;

public class AppView
{
    public MenuOption DisplayMainMenu()
    {
        AnsiConsole.Clear();

        return AnsiConsole.Prompt(
            new SelectionPrompt<MenuOption>()
                .Title("[green]Coding Tracker[/]")
                .AddChoices(Enum.GetValues<MenuOption>())
                .UseConverter(FormatMenuOption));
    }

    private static string FormatMenuOption(MenuOption option)
    {
        return option switch
        {
            MenuOption.View => "View coding sessions",
            MenuOption.Add => "Add coding session",
            MenuOption.Edit => "Update coding session",
            MenuOption.Delete => "Delete coding session",
            MenuOption.Close => "Exit",
            _ => option.ToString()
        };
    }

    public void DisplayGoodbye()
    {
        AnsiConsole.MarkupLine("[green]Goodbye![/]");
    }
}
