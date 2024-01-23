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

Example Code : 
```csharp
using DotNet8WebApi.HangfireApp.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHangfire(configuration => configuration
       .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
       .UseSimpleAssemblyNameTypeSerializer()
       .UseRecommendedSerializerSettings()
       .UseSqlServerStorage(builder.Configuration.GetConnectionString("DbConnection")));

builder.Services.AddHangfireServer();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
});

var app = builder.Build();
var backgroundJobs = app.Services.GetRequiredService<IBackgroundJobClient>();
var jobId = backgroundJobs.Enqueue(() => Console.WriteLine("Hello from Hangfire!"));

backgroundJobs.Schedule(
   () => Console.WriteLine("Delayed!"),
   TimeSpan.FromSeconds(5));

RecurringJob.AddOrUpdate(
    "myrecurringjob",
    () => Console.WriteLine("Recurring!"),
    Cron.Minutely);

RecurringJob.RemoveIfExists("myrecurringjob");

backgroundJobs.ContinueJobWith(
    jobId,
    () => Console.WriteLine("Continuation!"));


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHangfireDashboard();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

```
