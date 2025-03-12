using SmsRateLimiter.Config;
using SmsRateLimiter.Enums;
using StackExchange.Redis;

namespace SmsRateLimiter.Services.RateLimiterAlgorithmStrategy.StrategyFactory
{
    public class AlgorithmFactory : IAlgorithmFactory
    {
        private readonly IDatabase _redisDb;
        RateLimiterSettings _setting;
        private readonly RateLimiterStrategy _strategy;
        public AlgorithmFactory(IDatabase redis, RateLimiterSettings setting)
        {
            _redisDb = redis;
            _setting = setting;

            // Parse strategy from appsettings.json
            _strategy = Enum.TryParse(setting.Strategy, out RateLimiterStrategy strategy)
                ? strategy
                : RateLimiterStrategy.SlidingWindowBySortedSet;
        }
        public BaseStrategy CreateStrategy()
        {
            return _strategy switch
            {
                RateLimiterStrategy.FixedWindow => new FixedWindowStrategy(_redisDb, _setting),
                RateLimiterStrategy.SlidingWindowByQueue => new SlidingWindowByQueueStrategy(_redisDb, _setting),
                RateLimiterStrategy.SlidingWindowBySortedSet => new SlidingWindowBySortedSetStrategy(_redisDb, _setting),
                _ => throw new NotImplementedException("Invalid strategy")
            };
        }
    }
}
