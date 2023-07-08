namespace CronJobBackgroundService.BackgroundServices;

public interface IJobWorker
{
    public Task DoWorkAsync(CancellationToken cancellationToken);
}
