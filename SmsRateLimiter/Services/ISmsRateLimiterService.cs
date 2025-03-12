namespace SmsRateLimiter.Services
{
    public interface ISmsRateLimiterService
    {
        public Task<bool> CanSendMessageAsync(string phoneNumber, string accountId);
    }
}
