# SmsRateLimiter

A rate-limiting SMS requests solution.

## Description

This challenge required a rate-limiting algorithm, typically implemented using one of four main approaches: Fixed Window Counter, Sliding Window Log, Token Bucket, and Leaky Bucket. I implemented the first two—Fixed Window Counter and Sliding Window Log—with the Sliding Window approach in two variations: Queue-based and Sorted Set-based, all using Redis.
Although the Fixed Window algorithm best fits the requirements, it has a window edge issue—it processes requests within fixed intervals (e.g., 0–1 second, 1–2 seconds), and so on. In an overlapping window like (1.5, 2.5), more requests could be allowed than intended. The Sliding Window algorithm solves this by using a rolling time window, ensuring smoother rate limiting. The choice between these approaches depends on business requirements.
Further research confirmed that Redis also recommends the Sliding Window (Sorted Set-based) approach for rate limiting.
To enable dynamic algorithm selection, I applied the Strategy Pattern. Additionally, I wrote unit tests using NUnit and conducted performance tests to compare the implementations. The results showed that the Sliding Window (Sorted Set-based) approach was significantly faster than Queue-based, while Fixed Window Counter remained the fastest overall.
I installed Redis using Docker, simplifying the setup. The Redis connection string, selected algorithm, MaxPerNumber, and MaxPerAccount values are all configurable in the appsettings.json file.
If I had more time, I would have stored MaxPerNumber and MaxPerAccount per account ID and phone number directly in Redis. In a real-world project, these values could be retrieved from a database and dynamically injected into Redis.
