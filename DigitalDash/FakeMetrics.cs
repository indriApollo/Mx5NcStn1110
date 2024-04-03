using System;
using System.Threading;
using System.Threading.Tasks;
using Mx5NcStn1110;

namespace DigitalDash;

public class FakeMetrics : IMetrics
{
    private readonly Random _rand = new();
    
    private ushort _rpm = 1000;

    public ushort Rpm
    {
        get
        {
            if (_rpm > 7000)
            {
                _rpm = 1000;
            }

            return _rpm+=5;
        }
    }

    public ushort RedLine => 7000;
    public ushort SpeedKmh => (ushort)_rand.Next(0, 200);
    public byte AcceleratorPedalPositionPct => (byte)_rand.Next(0, 100);
    public byte CalculatedEngineLoadPct => (byte)_rand.Next(0, 100);
    public short EngineCoolantTempC => (short)_rand.Next(0, 100);
    public byte ThrottleValvePositionPct => (byte)_rand.Next(0, 100);
    public short IntakeAirTempC => (short)_rand.Next(0, 100);
    public byte FuelLevelPct => (byte)_rand.Next(0, 100);
    
    public void Setup()
    {
    }

    public async Task CollectAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.MaxValue, cancellationToken);
    }
}