using System.Buffers.Binary;
using System.Diagnostics;

namespace Mx5NcStn1110;

public class Mx5NcMetrics(Stn1110 stn) : IMetrics
{
    public ushort RedLine => 7000;

    private readonly SimpleMovingAverage _fuelSma = new(10);
    
    private ushort _rpm;
    public ushort Rpm => (ushort)(_rpm / 4);

    private ushort _speed;
    public ushort SpeedKmh => RawToSpeed(_speed);

    private byte _acceleratorPedalPosition;
    // TODO fix: reports 144 % when flat out lol, max was 220 % !
    public byte AcceleratorPedalPositionPct => (byte)(_acceleratorPedalPosition * 2);

    private byte _calculatedEngineLoad;
    public byte CalculatedEngineLoadPct => (byte)(_calculatedEngineLoad / 2.55f);

    private byte _engineCoolantTemp;
    public short EngineCoolantTempC => (short)(_engineCoolantTemp - 40);

    private byte _throttleValvePosition;
    // TODO observed between 11 <> 89 %
    public byte ThrottleValvePositionPct => (byte)(_throttleValvePosition / 2.55f);

    private byte _intakeAirTemp;
    public short IntakeAirTempC => (short)(_intakeAirTemp - 40);
    
    private byte _fuelLevel;
    public byte FuelLevelPct => (byte)(_fuelSma.Update(_fuelLevel) / 2.55f);

    private ushort _brakePressure;
    // TODO observed between 0 <> 72 %
    public byte BrakesPct => (byte)(0.2f * Math.Max(0, _brakePressure - 102));

    private ushort _flSpeed;
    public ushort FlSpeedKmh => RawToSpeed(_flSpeed);
    
    private ushort _frSpeed;
    public ushort FrSpeedKmh => RawToSpeed(_frSpeed);
    
    private ushort _rlSpeed;
    public ushort RlSpeedKmh => RawToSpeed(_rlSpeed);
    
    private ushort _rrSpeed;
    public ushort RrSpeedKmh => RawToSpeed(_rrSpeed);
    
    public long AvgParsingTicks { get; private set; }

    public void Setup()
    {
        stn.SetupConnection();
        stn.AddFilter(CanId.Brakes);
        stn.AddFilter(CanId.RpmSpeedAccel);
        stn.AddFilter(CanId.LoadCoolantThrottleIntake);
        stn.AddFilter(CanId.FuelLevel);
        stn.AddFilter(CanId.WheelSpeeds);
    }

    public async Task CollectAsync(CancellationToken cancellationToken)
    {
        var sw = new Stopwatch();
        
        stn.StartMonitoring();
        while (!cancellationToken.IsCancellationRequested)
        {
            sw.Restart();
            var message = await stn.ReadCanMessage(cancellationToken);
            if (message is null)
            {
                // no message, wait a bit
                try
                {
                    await Task.Delay(10, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }

                continue;
            }

            switch (message.Id)
            {
                case CanId.Brakes:
                    ParseBrakes(message.Data);
                    break;
                case CanId.RpmSpeedAccel:
                    ParseRpmSpeedAccel(message.Data);
                    break;
                case CanId.LoadCoolantThrottleIntake:
                    ParseLoadCoolantThrottleIntake(message.Data);
                    break;
                case CanId.FuelLevel:
                    ParseFuelLevel(message.Data);
                    break;
                case CanId.WheelSpeeds:
                    ParseWheelSpeeds(message.Data);
                    break;
            }

            AvgParsingTicks = (sw.ElapsedTicks + AvgParsingTicks) / 2;
        }
        stn.StopMonitoring();
    }

    private void ParseBrakes(byte[] data)
    {
        _brakePressure = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan()[..2]);
    }

    private void ParseRpmSpeedAccel(byte[] data)
    {
        _rpm = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan()[..2]);
        _speed = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan()[4..6]);
        _acceleratorPedalPosition = data[6];
    }

    private void ParseLoadCoolantThrottleIntake(byte[] data)
    {
        _calculatedEngineLoad = data[0];
        _engineCoolantTemp = data[1];
        _throttleValvePosition = data[3];
        _intakeAirTemp = data[4];
    }
    
    private void ParseFuelLevel(byte[] data)
    {
        _fuelLevel = data[0];
    }

    private void ParseWheelSpeeds(byte[] data)
    {
        _flSpeed = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan()[..2]);
        _frSpeed = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan()[2..4]);
        _rlSpeed = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan()[4..6]);
        _rrSpeed = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan()[6..]);
    }
    
    private static ushort RawToSpeed(ushort rawSpeed) => (ushort)Math.Max(0, rawSpeed/100f - 100);
}