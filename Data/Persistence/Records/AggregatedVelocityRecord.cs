namespace WashingIot.Data.Persistence;

public record AggregatedVelocityRecord(DateTimeOffset Ts, double VarX, double VarY, double VarZ, double AggregatedVar);

