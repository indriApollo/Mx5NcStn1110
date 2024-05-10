using System.Diagnostics;
using System.IO.MemoryMappedFiles;

namespace Mx5NcStn1110.Shm;

public struct Metrics
{
    public ushort Rpm;
    public ushort SpeedKmh;
    public byte AcceleratorPedalPositionPct;
    public byte CalculatedEngineLoadPct;
    public short EngineCoolantTempC;
    public byte ThrottleValvePositionPct;
    public short IntakeAirTempC;
    public byte FuelLevelPct;
    public byte BrakesPct;
    public ushort FlSpeedKmh;
    public ushort FrSpeedKmh;
    public ushort RlSpeedKmh;
    public ushort RrSpeedKmh;
}

public class Mx5NcShmMetrics : IMetrics, IDisposable
{
    private readonly Stopwatch _sw = new();
    private readonly MemoryMappedViewAccessor _accessor = MemoryMappedFile
        .CreateFromFile("/dev/shm/mx5metrics", FileMode.Open, null, 0, MemoryMappedFileAccess.Read)
        .CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
    
    public ushort RedLine => 7000;
    public ushort Rpm => _accessor.ReadUInt16(0);
    public ushort SpeedKmh => _accessor.ReadUInt16(2);
    public byte AcceleratorPedalPositionPct => _accessor.ReadByte(4);
    public byte CalculatedEngineLoadPct => _accessor.ReadByte(5);
    public short EngineCoolantTempC => _accessor.ReadInt16(6);
    public byte ThrottleValvePositionPct => _accessor.ReadByte(8);
    public short IntakeAirTempC => _accessor.ReadInt16(9);
    public byte FuelLevelPct => _accessor.ReadByte(11);
    public byte BrakesPct => _accessor.ReadByte(12);
    public ushort FlSpeedKmh => _accessor.ReadByte(13);
    public ushort FrSpeedKmh => _accessor.ReadByte(15);
    public ushort RlSpeedKmh => _accessor.ReadByte(17);
    public ushort RrSpeedKmh => _accessor.ReadByte(19);

    public long AvgParsingTicks
    {
        get
        {
            _sw.Restart();
            var a = _accessor.ReadUInt16(0);
            return _sw.ElapsedTicks;
        }
    }

    public void Dispose()
    {
        _accessor.Dispose();
    }

    public void Setup()
    {
    }

    public async Task CollectAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(-1, cancellationToken);
        }
        catch (TaskCanceledException)
        {
        }
    }
}