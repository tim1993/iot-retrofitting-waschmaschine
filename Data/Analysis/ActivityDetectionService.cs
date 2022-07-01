using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WashingIot.Configuration;

namespace WashingIot.Data;

public class ActivityDetectionService : IHostedService
{
    private readonly VelocityAggregationService velocityAggregationService;
    private readonly ILogger<ActivityDetectionService> logger;

    public ActivityDetectionService(VelocityAggregationService velocityAggregationService, ILogger<ActivityDetectionService> logger)
    {
        this.velocityAggregationService = velocityAggregationService;
        this.logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        velocityAggregationService.AggregatedVelocityHistoryUpdated += VelocityUpdatedHandler;

        return Task.CompletedTask;
    }

    private void VelocityUpdatedHandler(IEnumerable<(DateTimeOffset, float)> data)
    {
        var youngestTs = data.Min(s => s.Item1);
        var observationTreshold = youngestTs + AnalysisConstants.ActivityDetectionComparisionBase;
        var avgReferenceVelocity = data.Where(s => s.Item1 < observationTreshold).Average(x => x.Item2);
        var velocityPoints = data.Where(s => s.Item1 > observationTreshold);

        if (velocityPoints.Any())
        {
            var comparisonVelocity = velocityPoints.Average(x => x.Item2);
            var procentualDifference = 1 - Math.Abs(comparisonVelocity) / Math.Abs(avgReferenceVelocity);

            if (procentualDifference > 0.1)
            {
                logger.LogInformation("Detected activity. ComparisionVelocity: {comparisonVelocity}; ObservationVelocity: {avgReferenceVelocity}", comparisonVelocity, avgReferenceVelocity);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        velocityAggregationService.AggregatedVelocityHistoryUpdated -= VelocityUpdatedHandler;

        return Task.CompletedTask;
    }
}