using SmsRateLimiter.Config;
using StackExchange.Redis;

namespace SmsRateLimiter.Services.RateLimiterAlgorithmStrategy
{
    public class SlidingWindowBySortedSetStrategy : BaseStrategy
    {
        public SlidingWindowBySortedSetStrategy(IDatabase redis, RateLimiterSettings setting) : base(redis, setting)
        {
            
        }
        public override async Task<bool> CanSendMessageAsync(string phoneNumber, string accountId)
        {
            return await CanSendSlidingWindowBySortedSetAsync(accountId, phoneNumber);
        }

        // Sliding Window Algorithm using sorted set
        private async Task<bool> CanSendSlidingWindowBySortedSetAsync(string phoneNumber, string accountId)
        {
            //_maxPerNumber and _maxPerAccount can be retrieved from database (can be cashed) based on phoneNumber and accountId

            string numberKey = $"sms:{phoneNumber}";
            string accountKey = $"sms:account:{accountId}";
            double now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0; // Current time in seconds

            // Define a sliding window of 1 second
            double windowSize = 1.0;

            // Remove timestamps older than the sliding window
            await _redisDb.SortedSetRemoveRangeByScoreAsync(numberKey, double.NegativeInfinity, now - windowSize);
            await _redisDb.SortedSetRemoveRangeByScoreAsync(accountKey, double.NegativeInfinity, now - windowSize);

            // Get the current request count
            long numberCount = await _redisDb.SortedSetLengthAsync(numberKey);
            long accountCount = await _redisDb.SortedSetLengthAsync(accountKey);

            // Check limits
            if (numberCount >= _maxPerNumber || accountCount >= _maxPerAccount)
            {
                return false;
            }

            // Add new request with the current timestamp
            await _redisDb.SortedSetAddAsync(numberKey, now, now);
            await _redisDb.SortedSetAddAsync(accountKey, now, now);

            // Set expiration for cleanup
            await _redisDb.KeyExpireAsync(numberKey, TimeSpan.FromSeconds(2));
            await _redisDb.KeyExpireAsync(accountKey, TimeSpan.FromSeconds(2));

            return true;
        }
    }
}
