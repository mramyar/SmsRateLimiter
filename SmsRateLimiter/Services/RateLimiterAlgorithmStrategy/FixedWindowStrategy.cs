using SmsRateLimiter.Config;
using StackExchange.Redis;

namespace SmsRateLimiter.Services.RateLimiterAlgorithmStrategy
{
    public class FixedWindowStrategy : BaseStrategy
    {
        public FixedWindowStrategy(IDatabase redis, RateLimiterSettings settings) : base(redis, settings)
        {
            
        }
        public override async Task<bool> CanSendMessageAsync(string phoneNumber, string accountId)
        {
            return await CanSendFixedWindowCounterAsync(accountId, phoneNumber);
        }

        // Fixed Window Algorithm
        private async Task<bool> CanSendFixedWindowCounterAsync(string phoneNumber, string accountId)
        {
            //_maxPerNumber and _maxPerAccount can be retrieved from the database (can be cashed) based on phoneNumber and accountId

            string numberKey = $"sms:{phoneNumber}";
            string accountKey = $"sms:account:{accountId}";

            // Increment counters
            long numberCount = await _redisDb.StringIncrementAsync(numberKey);
            long accountCount = await _redisDb.StringIncrementAsync(accountKey);

            // Set expiration of 1 second for keys 
            if (numberCount == 1) await _redisDb.KeyExpireAsync(numberKey, TimeSpan.FromSeconds(1));
            if (accountCount == 1) await _redisDb.KeyExpireAsync(accountKey, TimeSpan.FromSeconds(1));

            // Check limits
            return numberCount <= _maxPerNumber && accountCount <= _maxPerAccount;
        }
    }
}
