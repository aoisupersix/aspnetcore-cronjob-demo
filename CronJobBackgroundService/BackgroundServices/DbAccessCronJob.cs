using Microsoft.EntityFrameworkCore;

namespace CronJobBackgroundService.BackgroundServices;

public class DbAccessCronJob : IJobWorker
{
    private readonly ILogger<DbAccessCronJob> logger;
    private readonly SampleDbContext dbContext;

    public DbAccessCronJob(ILogger<DbAccessCronJob> logger, SampleDbContext dbContext)
    {
        this.logger = logger;
        this.dbContext = dbContext;
    }

    public async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        var books = await dbContext.Books.ToListAsync(cancellationToken);
        logger.LogInformation(
            "Retrieved {Count} books from DB. Records: {Records}",
            books.Count,
            string.Join(",", books.Select(b => $"{{ ID: {b.ID}, Name: {b.Name} }}")));
        return;
    }
}
