namespace Mx5NcStn1110;

public interface IMetrics
{
    public ushort Rpm { get; }
    public ushort SpeedKmh { get; }
    public byte AcceleratorPedalPositionPct { get; }
    public byte CalculatedEngineLoadPct { get; }
    public short EngineCoolantTempC { get; }
    public byte ThrottleValvePositionPct { get; }
    public short IntakeAirTempC { get; }
    public byte FuelLevelPct { get; }
    
    public void Setup();
    public Task CollectAsync(CancellationToken cancellationToken);
}