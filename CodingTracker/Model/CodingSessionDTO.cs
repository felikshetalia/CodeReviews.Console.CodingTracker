namespace CodeReviews.Console.CodingTracker;
// the version that is returned by SQL
public class CodingSessionDTO
{
    public long Id { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }

}