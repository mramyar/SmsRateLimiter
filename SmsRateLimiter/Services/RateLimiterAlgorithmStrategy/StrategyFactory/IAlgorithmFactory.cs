namespace SmsRateLimiter.Services.RateLimiterAlgorithmStrategy.StrategyFactory
{
    public interface IAlgorithmFactory
    {
        public  BaseStrategy CreateStrategy();
    }
}
