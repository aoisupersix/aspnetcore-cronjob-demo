using System.Globalization;

namespace CronJobBackgroundService.BackgroundServices;

public class DisposableCronJob : IJobWorker, IDisposable
{
    private readonly ILogger<SimpleCronJob> logger;

    public DisposableCronJob(ILogger<SimpleCronJob> logger)
    {
        this.logger = logger;
    }

    public Task DoWorkAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("{Now} DisposableCronJob is working.", DateTime.Now.ToString("T", CultureInfo.InvariantCulture));
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        logger.LogInformation("The DisposableCronJob has been disposed.");
        GC.SuppressFinalize(this);
    }
}
