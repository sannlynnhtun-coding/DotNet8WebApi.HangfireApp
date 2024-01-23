# Hangfire

Fire-and-Forget Jobs
Fire-and-forget jobs are executed only once and almost immediately after creation.

```csharp
var jobId = BackgroundJob.Enqueue(
    () => Console.WriteLine("Fire-and-forget!"));
```

Delayed Jobs
Delayed jobs are executed only once too, but not immediately, after a certain time interval.

```csharp
var jobId = BackgroundJob.Schedule(
    () => Console.WriteLine("Delayed!"),
    TimeSpan.FromDays(7));
```

Recurring Jobs
Recurring jobs fire many times on the specified CRON schedule.


```csharp
RecurringJob.AddOrUpdate(
    "myrecurringjob",
    () => Console.WriteLine("Recurring!"),
    Cron.Daily);
```
