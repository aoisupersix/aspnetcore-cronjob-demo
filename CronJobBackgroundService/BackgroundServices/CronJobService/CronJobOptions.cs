namespace CronJobBackgroundService.BackgroundServices.CronJobService;

public class CronJobOptions
{
    public string CronExpression { get; set; } = "";

    public TimeZoneInfo TimeZoneInfo { get; set; } = TimeZoneInfo.Local;
}
