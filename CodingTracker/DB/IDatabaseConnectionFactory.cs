using System.Data.Common;
namespace CodeReviews.Console.CodingTracker;

public interface IDatabaseConnectionFactory
{
    DbConnection CreateConnection();
}