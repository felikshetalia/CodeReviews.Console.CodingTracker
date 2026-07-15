using Microsoft.Extensions.Configuration;

namespace CodeReviews.Console.CodingTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationRoot config = Setup();

            string connectionString = config.GetConnectionString("DefaultConnection")
             ?? throw new InvalidOperationException(
                "The DefaultConnection connection string is missing.");

            IDatabaseConnectionFactory connectionFactory = new SQLiteConnectionFactory(connectionString);

            var dbInitializer = new DatabaseInitializer(connectionFactory);
            dbInitializer.Initialize();

            ICodingSessionRepo repository = new CodingSessionRepo(connectionFactory);
            ICodingSessionService service = new CodingSessionService(repository);
            ICodingSessionView codingSessionView = new CodingSessionView();
            ICodingSessionController codingSessionController = new CodingSessionController(service, codingSessionView);
            IAppView appView = new AppView();
            var appController = new AppController(appView, codingSessionController);

            appController.Run();
        }

        private static IConfigurationRoot Setup()
        {
            IConfigurationRoot config =
                new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile(
                        "Configuration/appsettings.json",
                        optional: false)
                    .Build();

            return config;
        }
    }
}