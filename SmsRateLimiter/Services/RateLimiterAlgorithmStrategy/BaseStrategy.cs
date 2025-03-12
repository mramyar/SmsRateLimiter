using SmsRateLimiter.Config;
using SmsRateLimiter.Enums;
using StackExchange.Redis;

namespace SmsRateLimiter.Services.RateLimiterAlgorithmStrategy
{
    public abstract class BaseStrategy
    {
        protected readonly IDatabase _redisDb;
        protected readonly int _maxPerNumber;
        protected readonly int _maxPerAccount;
        protected readonly RateLimiterStrategy _strategy;
        public BaseStrategy(IDatabase redis, RateLimiterSettings setting)
        {
            _redisDb = redis;
            _maxPerNumber = setting.MaxPerNumber;
            _maxPerAccount = setting.MaxPerAccount;
        }
        public abstract Task<bool> CanSendMessageAsync(string phoneNumber, string accountId);
    }
}
