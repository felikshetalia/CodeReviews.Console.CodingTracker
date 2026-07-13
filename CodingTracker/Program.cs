using CodeReviews.Console.CodingTracker.Data;
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

            IAppView appView = new AppView();
            var appController = new AppController(appView);

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