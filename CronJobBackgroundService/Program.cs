using CronJobBackgroundService;
using CronJobBackgroundService.BackgroundServices;
using CronJobBackgroundService.BackgroundServices.CronJobService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// The same worker class (SimpleCronJob) cannot be registered at the same time.
// This is because it is not usual to duplicate the same background service.
// In this case, only the first registered class is executed.
// IHostedService registration can be made registrable by replacing it with AddSingleton.
// Reference: https://github.com/dotnet/runtime/issues/38751
builder.Services.AddCronJob<SimpleCronJob>(options =>
{
    options.CronExpression = "*/1 * * * *";
});
builder.Services.AddCronJob<SimpleCronJob>(options =>
{
    options.CronExpression = "*/5 * * * *";
});

builder.Services.AddCronJob<DbAccessCronJob>(options =>
{
    options.CronExpression = "*/1 * * * *";
});

builder.Services.AddCronJob<DisposableCronJob>(options =>
{
    options.CronExpression = "*/1 * * * *";
});

builder.Services.AddCronJob(
    options =>
    {
        options.CronExpression = "*/1 * * * *";
    },
    provider => ActivatorUtilities.CreateInstance<ParameterizedCronJob>(provider, "parameter value"));

builder.Services.AddDbContext<SampleDbContext>(options =>
    options.UseInMemoryDatabase("SampleDatabase"));
builder.Services.AddControllers();
var app = builder.Build();

using var scope = app.Services.CreateScope();
using var context = scope.ServiceProvider.GetRequiredService<SampleDbContext>();
await context.Database.EnsureCreatedAsync();

app.UseAuthorization();

app.MapControllers();

app.Run();
