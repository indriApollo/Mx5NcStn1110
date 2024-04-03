using System;
using System.Diagnostics;
using Avalonia.Threading;
using Mx5NcStn1110;

namespace DigitalDash;

public class Logic
{
    public readonly DispatcherTimer HighSpeedTimer = new ();
    public readonly DispatcherTimer LowSpeedTimer = new ();

    private readonly IMetrics _metrics = new FakeMetrics();
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    public Logic()
    {
        HighSpeedTimer.Interval = TimeSpan.FromMilliseconds(1000d/30d);
        LowSpeedTimer.Interval = TimeSpan.FromMilliseconds(1000);
        
        HighSpeedTimer.Start();
        LowSpeedTimer.Start();
    }

    public string SpeedKmh => _metrics.SpeedKmh.ToString();
    public string Rpm => _metrics.Rpm.ToString();
    public int RpmPct => _metrics.Rpm / (_metrics.RedLine/100);
    public string FuelGauge => _metrics.FuelLevelPct.ToString();
    public string Coolant => _metrics.EngineCoolantTempC.ToString();
    public string Intake => _metrics.IntakeAirTempC.ToString();
    public int Accelerator => _metrics.AcceleratorPedalPositionPct;
    public int Throttle => _metrics.ThrottleValvePositionPct;
    public int EngineLoad => _metrics.CalculatedEngineLoadPct;
    public string Stint => $"{(int)_stopwatch.Elapsed.TotalMinutes:00}:{_stopwatch.Elapsed.Seconds:00}";
}