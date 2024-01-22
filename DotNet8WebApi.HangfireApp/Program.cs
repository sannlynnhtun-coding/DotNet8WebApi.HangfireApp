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
