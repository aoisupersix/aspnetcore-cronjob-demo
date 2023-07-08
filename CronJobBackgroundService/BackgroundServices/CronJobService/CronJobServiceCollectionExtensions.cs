using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace CronJobBackgroundService.BackgroundServices.CronJobService;

public static class CronJobServiceCollectionExtensions
{
    public static IServiceCollection AddCronJob<TWorker>(
        this IServiceCollection services,
        Action<CronJobOptions> configureOptions)
        where TWorker : IJobWorker
    {
        return services.AddCronJob(configureOptions, provider => ActivatorUtilities.CreateInstance<TWorker>(provider));
    }

    public static IServiceCollection AddCronJob<TWorker>(
        this IServiceCollection services,
        Action<CronJobOptions> configureOptions,
        Func<IServiceProvider, TWorker> workerFactory)
        where TWorker : IJobWorker
    {
        // Use GUID to identify configuration values for each instance of a CRON job.
        var instanceId = Guid.NewGuid();
        services.Configure(instanceId.ToString(), configureOptions);
        services.TryAddSingleton<IValidateOptions<CronJobOptions>, CronJobOptionsValidator>();
        services.AddHostedService(provider =>
            ActivatorUtilities.CreateInstance<CronJobService<TWorker>>(provider, instanceId, workerFactory));
        return services;
    }
}
