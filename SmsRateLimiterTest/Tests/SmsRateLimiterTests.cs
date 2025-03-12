using NUnit.Framework;
using StackExchange.Redis;
using SmsRateLimiter.Services;
using SmsRateLimiter.Config;
using SmsRateLimiter.Services.RateLimiterAlgorithmStrategy.StrategyFactory;

namespace SmsRateLimiterTest.Tests
{
    [TestFixture]
    public class SmsRateLimiterTests
    {
        private SmsRateLimiterService _service;
        private void Setup()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            var rateLimiterSetting = new RateLimiterSettings
            {
                MaxPerNumber = 5,
                MaxPerAccount = 10,
                Strategy = "SlidingWindowBySortedSet"
            };
            var algorithmFactory = new AlgorithmFactory(redis.GetDatabase(), rateLimiterSetting);
            _service = new SmsRateLimiterService(algorithmFactory);
        }

        [Test]
        public async Task Run_if_api_works_test()
        {
            Setup();
            bool result = await _service.CanSendMessageAsync("1234", "Rayah");

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task Exceed_the_limit_test()
        {
            Setup();
            for (int i = 0; i < 7; i++)  // Exceed the limit
            {
                bool result = await _service.CanSendMessageAsync("1234", "Rayah");

                if (i < 5)
                {
                    Assert.That(result, Is.True, $"Request {i + 1} should be allowed.");
                }
                else
                {
                    Assert.That(result, Is.False, $"Request {i + 1} should be blocked by rate limiting.");
                }

                Console.WriteLine(result ? "Request Allowed" : "Rate Limit Exceeded");
                //await Task.Delay(1000);
            }
        }
    }
}
