using System;
using Spectre.Console;

namespace CodeReviews.Console.CodingTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            var appView = new AppView();
            var appController = new AppController(appView);

            appController.Run();
        }
    }
}