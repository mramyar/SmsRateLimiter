# SmsRateLimiter

A rate-limiting SMS requests solution.

## Description

This challenge required a rate-limiting algorithm, and rate limiting typically involves four main approaches: Fixed Window Counter, Sliding Window Log, Token Bucket, and Leaky Bucket. I implemented the first two—Fixed Window Counter and Sliding Window Log—with the Sliding Window approach done in two ways: Queue-based and Sorted Set-based, all using Redis. To enable dynamic algorithm selection, I applied the Strategy Pattern. Also, I wrote unit tests using NUnit and ran a performance test to compare the three implementations. The results showed that the Sliding Window (Sorted Set-based) approach was significantly faster than the others. Further research confirmed that Redis also recommends this method for rate limiting.
I installed Redis using Docker, which made the setup much easier. The Redis connection string, selected algorithm, MaxPerNumber, and MaxPerAccount values are all configurable in the appsettings.json file. If I had more time, I would have declared MaxPerNumber and MaxPerAccount per accountId and phoneNumber directly in Redis. In a real-world project, these values could be retrieved from a database and injected into Redis dynamically.
