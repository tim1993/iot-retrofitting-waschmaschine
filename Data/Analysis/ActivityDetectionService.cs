using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WashingIot.Data.Persistence;

namespace WashingIot.Data;

public class ActivityDetectionService : IHostedService
{
    private readonly VelocityAggregationService velocityAggregationService;
    private readonly CsvPersistenceService persistenceService;
    private readonly ILogger<ActivityDetectionService> logger;



    private DateTimeOffset? _lastDetectionRun;

    private double? referenceVelocityThreshold;

    public delegate void ActivityStateUpdatedHandler(bool activityDetected);

    public event ActivityStateUpdatedHandler? ActivityStateUpdated;

    public ActivityDetectionService(VelocityAggregationService velocityAggregationService, CsvPersistenceService persistenceService, ILogger<ActivityDetectionService> logger)
    {
        this.velocityAggregationService = velocityAggregationService;
        this.persistenceService = persistenceService;
        this.logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        velocityAggregationService.AggregatedVelocityHistoryUpdated += VelocityUpdatedHandler;

        return Task.CompletedTask;
    }

    private void VelocityUpdatedHandler(IEnumerable<(DateTimeOffset, double)> data)
    {
        TryCalculateInitialThreshold(data.Select(d => d.Item2));
        if (referenceVelocityThreshold is not null)
        {
            var monitoringValues = data.Where(d => _lastDetectionRun != null ? d.Item1 > _lastDetectionRun : true).TakeLast(10);
            if (monitoringValues.Count() == 10)
            {
                var hitList = monitoringValues.Select(mv => HasExceededReferenceThresholdBy(60, referenceVelocityThreshold.Value, mv.Item2));
                var hitCount = hitList.Where(hit => hit).Count();
                var hitPercent = (double)hitCount / (double)hitList.Count();

                logger.LogInformation("Got following statistics: Hits: {hitcount}, HitPercentage: {hitPercentage}", hitCount, hitPercent);

                if (hitPercent > 0.7)
                {
                    logger.LogInformation("Activity start detected.");
                    ActivityStateUpdated?.Invoke(true);
                }

                if (hitPercent < 0.3)
                {
                    logger.LogInformation("Activity stopp detected.");
                    ActivityStateUpdated?.Invoke(false);
                }

                _lastDetectionRun = DateTimeOffset.UtcNow;
            }
        }
    }

    private bool HasExceededReferenceThresholdBy(int percent, double reference, double value)
        => (value - reference) / Math.Abs(reference) * 100 > percent;

    private bool TryCalculateInitialThreshold(IEnumerable<double> avgVelocityStdDev)
    {
        if (avgVelocityStdDev.Count() > 5
            && referenceVelocityThreshold is null)
        {
            referenceVelocityThreshold = avgVelocityStdDev.Average();
            logger.LogInformation("Set ReferenceVelocityThreshold to {reference}", referenceVelocityThreshold);

            return true;
        }

        return false;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        velocityAggregationService.AggregatedVelocityHistoryUpdated -= VelocityUpdatedHandler;
        return Task.CompletedTask;
    }
}