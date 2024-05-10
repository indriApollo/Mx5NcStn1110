namespace Mx5NcStn1110.Uds;

public enum Command : byte
{
    Error = 0,
    GetRpm = 1,
    GetSpeedKmh = 2,
    GetAcceleratorPedalPositionPct = 3,
    GetCalculatedEngineLoadPct = 4,
    GetEngineCoolantTempC = 5,
    GetThrottleValvePositionPct = 6,
    GetIntakeAirTempC = 7,
    GetFuelLevelPct = 8,
    GetBrakesPct = 9,
    GetFlSpeedKmh = 10,
    GetFrSpeedKmh = 11,
    GetRlSpeedKmh = 12,
    GetRrSpeedKmh = 13
};