using CodeReviews.Console.CodingTracker;

public static class Validators
{
    public static void ValidateSessionTimes(
        DateTime startTime,
        DateTime endTime)
    {
        if (endTime <= startTime)
        {
            throw new ArgumentException(
                "The end time must be later than the start time.");
        }
    }

}