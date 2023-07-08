using System.Globalization;

namespace CronJobBackgroundService.BackgroundServices;

public class ParameterizedCronJob : IJobWorker
{
    private readonly string param1;
    private readonly ILogger<SimpleCronJob> logger;

    public ParameterizedCronJob(string param1, ILogger<SimpleCronJob> logger)
    {
        this.param1 = param1;
        this.logger = logger;
    }

    public Task DoWorkAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "{Now} ParameterizedCronJob is working. Parameter is '{Param}'",
            DateTime.Now.ToString("T", CultureInfo.InvariantCulture),
            param1);
        return Task.CompletedTask;
    }
}
