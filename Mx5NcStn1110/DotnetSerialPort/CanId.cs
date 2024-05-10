namespace Mx5NcStn1110.DotnetSerialPort;

public static class CanId
{
    public const ushort Brakes = 0x085; // 100hz
    public const ushort RpmSpeedAccel = 0x201; // 100hz
    public const ushort LoadCoolantThrottleIntake = 0x240; // 10hz
    public const ushort FuelLevel = 0x430; // 10 hz
    public const ushort WheelSpeeds = 0x4b0; // 100hz
}