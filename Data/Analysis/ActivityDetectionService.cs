using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WashingIot.Data.Persistence;

namespace WashingIot.Data;

public class ActivityDetectionService : IHostedService
{
    private readonly VelocityAggregationService _velocityAggregationService;
    private readonly PersistenceService _persistenceService;
    private readonly ILogger<ActivityDetectionService> _logger;



    private DateTimeOffset? _lastDetectionRun;

    private double? referenceVelocityThreshold;

    public delegate void ActivityStateUpdatedHandler(bool activityDetected);

    public event ActivityStateUpdatedHandler? ActivityStateUpdated;

    public ActivityDetectionService(VelocityAggregationService velocityAggregationService, PersistenceService persistenceService, ILogger<ActivityDetectionService> logger)
    {
        _velocityAggregationService = velocityAggregationService;
        _persistenceService = persistenceService;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _velocityAggregationService.AggregatedVelocityHistoryUpdated += VelocityUpdatedHandler;

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _velocityAggregationService.AggregatedVelocityHistoryUpdated -= VelocityUpdatedHandler;
        return Task.CompletedTask;
    }

    private void VelocityUpdatedHandler(IEnumerable<(DateTimeOffset, double)> data)
    {
        TryCalculateInitialThreshold(data.Select(d => d.Item2));
        if (referenceVelocityThreshold is not null)
        {
            var monitoringValues = GetVelocityVarianceForObservationPeroid(data);
            TryDetectActivity(monitoringValues, referenceVelocityThreshold.Value);
        }
    }

    private void TryDetectActivity(IEnumerable<double> monitoringValues, double activityThreshold)
    {
        if (monitoringValues.Count() == 10)
        {
            var hitList = monitoringValues.Select(mv => HasExceededReferenceThresholdBy(80, activityThreshold, mv));
            var hitCount = hitList.Where(hit => hit).Count();
            var hitPercent = (double)hitCount / (double)hitList.Count();

            _logger.LogInformation("Got following statistics: Hits: {hitcount}, HitPercentage: {hitPercentage}", hitCount, hitPercent);

            if (hitPercent > 0.7)
            {
                _logger.LogInformation("Activity start detected.");
                ActivityStateUpdated?.Invoke(true);
            }

            if (hitPercent < 0.3)
            {
                _logger.LogInformation("Activity stopp detected.");
                ActivityStateUpdated?.Invoke(false);
            }

            _lastDetectionRun = DateTimeOffset.UtcNow;
        }
    }

    private IEnumerable<double> GetVelocityVarianceForObservationPeroid(IEnumerable<(DateTimeOffset, double)> data)
        => data.Where(d => _lastDetectionRun != null ? d.Item1 > _lastDetectionRun : true).TakeLast(10).Select(x => x.Item2);

    private bool HasExceededReferenceThresholdBy(int percent, double reference, double value)
        => (value - reference) / Math.Abs(reference) * 100 > percent;

    private bool TryCalculateInitialThreshold(IEnumerable<double> avgVelocityStdDev)
    {
        if (avgVelocityStdDev.Count() > 5
            && referenceVelocityThreshold is null)
        {
            referenceVelocityThreshold = avgVelocityStdDev.Average();
            _logger.LogInformation("Set ReferenceVelocityThreshold to {reference}", referenceVelocityThreshold);

            return true;
        }

        return false;
    }
}