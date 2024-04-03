using System.Buffers.Binary;

namespace Mx5NcStn1110;

public class Mx5NcMetrics(Stn1110 stn) : IMetrics
{
    public ushort RedLine => 7000;
    
    private ushort _rpm;
    public ushort Rpm => (ushort)(_rpm / 4);

    private ushort _speed;
    public ushort SpeedKmh => (ushort)(_speed/100 - 100);

    private byte _acceleratorPedalPosition;
    public byte AcceleratorPedalPositionPct => (byte)(_acceleratorPedalPosition * 2);

    private byte _calculatedEngineLoad;
    public byte CalculatedEngineLoadPct => (byte)(_calculatedEngineLoad / 2.55f);

    private byte _engineCoolantTemp;
    public short EngineCoolantTempC => (short)(_engineCoolantTemp - 40);

    private byte _throttleValvePosition;
    public byte ThrottleValvePositionPct => (byte)(_throttleValvePosition / 2.55f);

    private byte _intakeAirTemp;
    public short IntakeAirTempC => (short)(_intakeAirTemp - 40);
    
    private byte _fuelLevel;
    public byte FuelLevelPct => (byte)(_fuelLevel / 2.55f);

    public void Setup()
    {
        stn.SetupConnection();
        stn.AddFilter(CanId.RpmSpeedAccel);
        stn.AddFilter(CanId.LoadCoolantThrottleIntake);
        stn.AddFilter(CanId.FuelLevel);
    }

    public async Task CollectAsync(CancellationToken cancellationToken)
    {
        stn.StartMonitoring();
        while (!cancellationToken.IsCancellationRequested)
        {
            var message = stn.ReadCanMessage();
            if (message is null)
            {
                // no message, wait a bit
                await Task.Delay(100, cancellationToken);
                continue;
            }

            switch (message.Id)
            {
                case CanId.RpmSpeedAccel:
                    ParseRpmSpeedAccel(message.Data);
                    break;
                case CanId.LoadCoolantThrottleIntake:
                    ParseLoadCoolantThrottleIntake(message.Data);
                    break;
                case CanId.FuelLevel:
                    ParseFuelLevelData(message.Data);
                    break;
            }
        }
        stn.StopMonitoring();
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
    
    private void ParseFuelLevelData(byte[] data)
    {
        _fuelLevel = data[0];
    }
}