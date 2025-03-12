namespace SmsRateLimiter.Config
{
    public class RateLimiterSettings
    {
        public string Strategy { get; set; } = "SlidingWindowBySortedSet";
        public int MaxPerNumber { get; set; } = 5;
        public int MaxPerAccount { get; set; } = 10;
    }
}
