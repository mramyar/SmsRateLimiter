using SmsRateLimiter.Services.RateLimiterAlgorithmStrategy.StrategyFactory;

namespace SmsRateLimiter.Services
{
    public class SmsRateLimiterService : ISmsRateLimiterService
    {
        private readonly IAlgorithmFactory _algorithmFactory;

        public SmsRateLimiterService(IAlgorithmFactory algorithmFactory)
        {
            _algorithmFactory = algorithmFactory;
        }

        // Main method that selects the correct algorithm
        public async Task<bool> CanSendMessageAsync(string phoneNumber, string accountId)
        {
            var rateLimiterStrategy = _algorithmFactory.CreateStrategy();
            return await rateLimiterStrategy.CanSendMessageAsync(phoneNumber, accountId);
        }
    }
}

