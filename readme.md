# ASP.NET Core cron job BackgroundService demo

This is a demo app that executes CRON jobs using `IHostedService` and `BackgroundService` of ASP.NET Core.

Documentation of ASP.NET Core background services： https://learn.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/background-tasks-with-ihostedservice

- Job classes are regenerated each time they are executed
- If the job class implements `IDisposable`, `Dispose()` will be called after the execution is finished
- Dependency injection can be performed in the job class constructor
- You can specify any parameter to the job class

Customized based on [dotnet-labs/ServiceWorkerCronJob](https://github.com/dotnet-labs/ServiceWorkerCronJob).

# How to define a CRON job

### １. Define a job class that implements `IJobWorker`

```cs
public class CronJob : IJobWorker
{
    public Task DoWorkAsync(CancellationToken cancellationToken)
    {
        // Do work here
    }
}
```

### ２. Set up jobs with startup code (`Program.cs`)

```cs
builder.Services.AddCronJob(
    options =>
    {
        options.CronExpression = "*/1 * * * *";
    },
    provider => ActivatorUtilities.CreateInstance<CronJob>(provider));
```

# License

The MIT License(MIT)
