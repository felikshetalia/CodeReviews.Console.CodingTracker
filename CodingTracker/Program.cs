using System;
using Spectre.Console;

namespace CodeReviews.Console.CodingTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            AnsiConsole.MarkupLine("[bold green]Welcome to the Coding Tracker![/]");
            AnsiConsole.MarkupLine("[bold yellow]This application helps you track your coding sessions.[/]");
            AnsiConsole.MarkupLine("[bold blue]Press any key to exit...[/]");
            AnsiConsole.Console.Input.ReadKey(true);
        }
    }
}