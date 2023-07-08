using System.Globalization;
using Cronos;
using Microsoft.Extensions.Options;

namespace CronJobBackgroundService.BackgroundServices.CronJobService;

public class CronJobService<TWorker> : IHostedService, IDisposable where TWorker : IJobWorker
{
    private bool disposed;
    private System.Timers.Timer? timer;

    private readonly ILogger<CronJobService<TWorker>> logger;
    private readonly IServiceScopeFactory scopeFactory;
    private readonly Func<IServiceProvider, TWorker> workerFactory;

    protected Guid InstanceId { get; }

    protected CronExpression Expression { get; }

    protected TimeZoneInfo TimeZoneInfo { get; }

    public CronJobService(
        Guid instanceId,
        ILogger<CronJobService<TWorker>> logger,
        IOptionsMonitor<CronJobOptions> options,
        IServiceScopeFactory scopeFactory,
        Func<IServiceProvider, TWorker> workerFactory)
    {
        InstanceId = instanceId;
        this.logger = logger;
        this.scopeFactory = scopeFactory;
        this.workerFactory = workerFactory;

        var config = options.Get(instanceId.ToString());
        Expression = CronExpression.Parse(config.CronExpression);
        TimeZoneInfo = config.TimeZoneInfo;
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "CRON job (Name: {JobName}, ID: {JobId}) execution has started. The CRON expression is \"{CronExpression}\" and the time zone is {TimeZone}.",
            typeof(TWorker).Name,
            InstanceId,
            Expression,
            TimeZoneInfo);
        await ScheduleJob(cancellationToken);
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("CRON job (Name: {JobName}, ID: {JobId}) execution has stopped.", GetType().Name, InstanceId);
        timer?.Stop();
        await Task.CompletedTask;
    }

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public Task DoWorkAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var worker = workerFactory(scope.ServiceProvider);

        try
        {
            return worker.DoWorkAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                "The cron job (Name: {JobName}, ID: {JobId}) execution failed. Error message: {ErrorMessage}",
                typeof(TWorker).Name,
                InstanceId,
                ex.Message);

            return Task.CompletedTask;
        }
        finally
        {
            if (typeof(TWorker).GetInterface(nameof(IDisposable)) is not null)
            {
                ((IDisposable)worker).Dispose();
            }
        }
    }

    protected virtual async Task ScheduleJob(CancellationToken cancellationToken)
    {
        var next = Expression.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo);
        if (next is null)
        {
            return;
        }

        var delay = next.Value - DateTimeOffset.Now;
        if (delay.TotalMilliseconds <= 0)
        {
            await ScheduleJob(cancellationToken); // Prevent non-positive values from being passed into Timer.
        }

        timer = new System.Timers.Timer(delay.TotalMilliseconds);
        timer.Elapsed += async (_, _) =>
        {
            timer.Dispose();
            timer = null;
            if (!cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation(
                    "The processing execution of the CRON job (Name: {JobName}, ID: {JobId}) has been triggered. The CRON expression is \"{CronExpression}\" and the start time is {NextTime}.",
                    typeof(TWorker).Name,
                    InstanceId,
                    Expression,
                    next.Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                await DoWorkAsync(cancellationToken);
            }
            if (!cancellationToken.IsCancellationRequested)
            {
                await ScheduleJob(cancellationToken); // Reschedule next
            }
        };

        timer.Start();
        return;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                timer?.Dispose();
            }
            disposed = true;
        }
    }
}