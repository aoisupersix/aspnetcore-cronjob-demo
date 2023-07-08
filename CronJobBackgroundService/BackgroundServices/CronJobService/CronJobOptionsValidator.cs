using Cronos;
using Microsoft.Extensions.Options;

namespace CronJobBackgroundService.BackgroundServices.CronJobService;

public class CronJobOptionsValidator : IValidateOptions<CronJobOptions>
{
    public ValidateOptionsResult Validate(string name, CronJobOptions options)
    {
        try
        {
            CronExpression.Parse(options.CronExpression);
        }
        catch (Exception)
        {
            return ValidateOptionsResult.Fail($"invalid {nameof(options.CronExpression)} format: {options.CronExpression}.");
        }

        return ValidateOptionsResult.Success;
    }
}