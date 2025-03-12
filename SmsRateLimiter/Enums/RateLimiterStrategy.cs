namespace SmsRateLimiter.Enums
{
    public enum RateLimiterStrategy
    {
        FixedWindow,
        SlidingWindowByQueue,
        SlidingWindowBySortedSet
    }
}
