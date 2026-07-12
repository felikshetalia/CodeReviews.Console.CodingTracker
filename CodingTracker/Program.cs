namespace CodeReviews.Console.CodingTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            IAppView appView = new AppView();
            var appController = new AppController(appView);

            appController.Run();
        }
    }
}