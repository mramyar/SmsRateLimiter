using NUnit.Framework;
using StackExchange.Redis;
using SmsRateLimiter.Services;
using SmsRateLimiter.Config;
using System.Diagnostics;
using SmsRateLimiter.Services.RateLimiterAlgorithmStrategy.StrategyFactory;

namespace SmsRateLimiterTest.Tests
{
    [TestFixture]
    public class SmsRateLimiterPerformanceTests
    {
        private SmsRateLimiterService _fixedWindowService;
        private SmsRateLimiterService _slidingWindowQService;
        private SmsRateLimiterService _slidingWindowSSService;
        private AlgorithmFactory Setup(string strategy)
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            var fixedWindowSetting = new RateLimiterSettings
            {
                MaxPerNumber = 50000,
                MaxPerAccount = 50000,
                Strategy = strategy
            };
            var algorithmFactory = new AlgorithmFactory(redis.GetDatabase(), fixedWindowSetting);
            return algorithmFactory;
        }

        [Test]
        public async Task Performance_test()
        {
            _fixedWindowService = new SmsRateLimiterService(Setup("FixedWindow"));
            _slidingWindowQService = new SmsRateLimiterService(Setup("SlidingWindowByQueue"));
            _slidingWindowSSService = new SmsRateLimiterService(Setup("SlidingWindowBySortedSet"));
            //===============================  fixed_window  ========
            var stopwatch_fixed_window = new Stopwatch();
            stopwatch_fixed_window.Start();

            for (int i = 0; i < 1000; i++)
            {
                bool result = await _fixedWindowService.CanSendMessageAsync("1234", "Rayah");
            }

            stopwatch_fixed_window.Stop();
            Console.WriteLine($"Time taken fixed window: {stopwatch_fixed_window.Elapsed.TotalSeconds:F2} seconds");
            //================================ sliding_window_sorted_set =======
            var stopwatch_sliding_window_sorted_set = new Stopwatch();
            stopwatch_sliding_window_sorted_set.Start();

            for (int i = 0; i < 1000; i++)
            {
                bool result = await _slidingWindowSSService.CanSendMessageAsync("12345", "Rayah5");
            }

            stopwatch_sliding_window_sorted_set.Stop();
            Console.WriteLine($"Time taken sliding window by sorted set: {stopwatch_sliding_window_sorted_set.Elapsed.TotalSeconds:F2} seconds");
            //==================================  sliding_window_queue =====
            var stopwatch_sliding_window_queue = new Stopwatch();
            stopwatch_sliding_window_queue.Start();

            for (int i = 0; i < 1000; i++)
            {
                bool result = await _slidingWindowQService.CanSendMessageAsync("12346", "Rayah6");
            }

            stopwatch_sliding_window_queue.Stop();
            Console.WriteLine($"Time taken sliding window by queue: {stopwatch_sliding_window_queue.Elapsed.TotalSeconds:F2} seconds");
            //=======================================
            Assert.Pass("Test Completed");
        }
    }
}
