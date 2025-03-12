using StackExchange.Redis;
using SmsRateLimiter.Services;
using SmsRateLimiter.Config;
using SmsRateLimiter.Services.RateLimiterAlgorithmStrategy.StrategyFactory;

namespace SmsRateLimiter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy.AllowAnyOrigin()
                                   .AllowAnyMethod()
                                   .AllowAnyHeader());
            });

            var redisSettings = builder.Configuration.GetSection("Redis").Get<RedisSettings>()
                               ?? throw new InvalidOperationException("Redis settings are missing from appsettings.json.");
            var rateLimiterSettings = builder.Configuration.GetSection("RateLimiter").Get<RateLimiterSettings>()
                                      ?? throw new InvalidOperationException("RateLimiter settings are missing from appsettings.json.");

            builder.Services.AddSingleton<IDatabase>(sp =>
            {
                return ConnectionMultiplexer.Connect(redisSettings.ConnectionString).GetDatabase();
            });
            builder.Services.AddSingleton(rateLimiterSettings);

            builder.Services.AddTransient<IAlgorithmFactory, AlgorithmFactory>();
            builder.Services.AddTransient<ISmsRateLimiterService, SmsRateLimiterService>();

            builder.Services.AddControllers();
            var app = builder.Build();
            app.UseCors("AllowAll");
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();

        }
    }
}