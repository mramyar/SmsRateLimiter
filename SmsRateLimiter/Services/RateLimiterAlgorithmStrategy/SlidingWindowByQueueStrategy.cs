using SmsRateLimiter.Config;
using StackExchange.Redis;

namespace SmsRateLimiter.Services.RateLimiterAlgorithmStrategy
{
    public class SlidingWindowByQueueStrategy : BaseStrategy
    {
        public SlidingWindowByQueueStrategy(IDatabase redis, RateLimiterSettings setting) : base(redis, setting)
        {
            
        }
        public override async Task<bool> CanSendMessageAsync(string phoneNumber, string accountId)
        {
            return await CanSendSlidingWindowByQueueAsync(accountId, phoneNumber);
        }

        // Sliding Window Algorithm using queue
        private async Task<bool> CanSendSlidingWindowByQueueAsync(string phoneNumber, string accountId)
        {
            string numberKey = $"sms:{phoneNumber}";
            string accountKey = $"sms:account:{accountId}";
            double now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0; // Current time in seconds
            double windowSize = 1.0; // 1-second window

            // Remove old timestamps from the queue (O(1))
            while (true)
            {
                string oldest = await _redisDb.ListGetByIndexAsync(numberKey, -1); // Get the last (oldest) item
                if (oldest == null || double.Parse(oldest) > now - windowSize) break;
                await _redisDb.ListRightPopAsync(numberKey); // Remove from the end (O(1))
            }

            while (true)
            {
                string oldest = await _redisDb.ListGetByIndexAsync(accountKey, -1);
                if (oldest == null || double.Parse(oldest) > now - windowSize) break;
                await _redisDb.ListRightPopAsync(accountKey);
            }

            // Get current request count (O(1))
            long numberCount = await _redisDb.ListLengthAsync(numberKey);
            long accountCount = await _redisDb.ListLengthAsync(accountKey);

            // Check limit
            if (numberCount >= _maxPerNumber || accountCount >= _maxPerAccount)
            {
                return false; // Rate limit exceeded
            }

            // Add new request to the queue (O(1))
            await _redisDb.ListLeftPushAsync(numberKey, now.ToString());
            await _redisDb.ListLeftPushAsync(accountKey, now.ToString());

            // Set expiration for cleanup (O(1))
            await _redisDb.KeyExpireAsync(numberKey, TimeSpan.FromSeconds(2));
            await _redisDb.KeyExpireAsync(accountKey, TimeSpan.FromSeconds(2));

            return true;
        }
    }
}
