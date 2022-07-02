using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WashingIot.Configuration;
using WashingIot.Data.Persistence;

namespace WashingIot.Data;

public class ActivityDetectionService : IHostedService
{
    private readonly VelocityAggregationService velocityAggregationService;
    private readonly CsvPersistenceService persistenceService;
    private readonly ILogger<ActivityDetectionService> logger;

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
        if (data.Any())
        {
            var newestTimestamp = data.Max(s => s.Item1);
            var observationTreshold = newestTimestamp - AnalysisConstants.ActivityDetectionPeriod;
            var samples = data.OrderBy(s => s.Item1);

            var bucketSize = 5;
            var currentPeriod = samples.TakeLast(bucketSize);
            var referencePeriod = samples.SkipLast(bucketSize).TakeLast(bucketSize);

            var currentBucketLength = currentPeriod.Count();
            var refrenceBucketLength = referencePeriod.Count();
            if (currentBucketLength != refrenceBucketLength)
            {
                // not enough samples
                logger.LogInformation(
                    "Activity detection still calibrating. Refrence {reference} != Current {current}.",
                    refrenceBucketLength, currentBucketLength);
                return;
            }



            if (currentPeriod.Any() && referencePeriod.Any())
            {
                var refrenceAvg = referencePeriod.Average(x => x.Item2);
                var currentAvg = currentPeriod.Average(x => x.Item2);

                var procentualDifference = (Math.Abs(currentAvg) - Math.Abs(refrenceAvg)) / Math.Abs(refrenceAvg);

                logger.LogInformation("ReferencePeriodAvgVelocity: {avgReferenceVelocity}, DetectionPeroidAvgVelocity: {comparisonVelocity}, Procentual Change: {procentualDifference}", refrenceAvg, currentAvg, procentualDifference * 100);

                persistenceService.Write(new ActivityDetectionRecord(DateTimeOffset.UtcNow, refrenceAvg, currentAvg, procentualDifference));

                if (procentualDifference > 10)
                {
                    logger.LogInformation("Detected activity. ComparisionVelocity: {comparisonVelocity}; ObservationVelocity: {avgReferenceVelocity}", currentAvg, refrenceAvg);
                }
            }
        }

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        velocityAggregationService.AggregatedVelocityHistoryUpdated -= VelocityUpdatedHandler;
        return Task.CompletedTask;
    }
}