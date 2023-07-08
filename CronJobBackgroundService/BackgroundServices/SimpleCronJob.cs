using System.Globalization;

namespace CronJobBackgroundService.BackgroundServices;

public class SimpleCronJob : IJobWorker
{
    private readonly ILogger<SimpleCronJob> logger;

    public SimpleCronJob(ILogger<SimpleCronJob> logger)
    {
        this.logger = logger;
    }

    public Task DoWorkAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("{Now} CronJob 1 is working.", DateTime.Now.ToString("T", CultureInfo.InvariantCulture));
        return Task.CompletedTask;
    }
}
