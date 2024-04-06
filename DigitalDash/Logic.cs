using System;
using System.Diagnostics;
using Avalonia.Threading;
using Mx5NcStn1110;

namespace DigitalDash;

public class Logic
{
    private readonly DispatcherTimer _highSpeedTimer = new ();
    private readonly DispatcherTimer _lowSpeedTimer = new ();

    private readonly IMetrics _metrics = new FakeMetrics();
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    public Logic()
    {
        _highSpeedTimer.Interval = TimeSpan.FromMilliseconds(1000d/30d); // 30hz
        _lowSpeedTimer.Interval = TimeSpan.FromMilliseconds(1000); // 1hz
        
        _highSpeedTimer.Start();
        _lowSpeedTimer.Start();
    }

    public const int CoolantAlertThrC = 100;
    public const int RpmWarningThrPct = 80;
    public const int RpmAlertThrPct = 90;
    public const int FuelGaugeAlertThrPct = 15;

    public string SpeedKmh => _metrics.SpeedKmh.ToString();
    public string Rpm => _metrics.Rpm.ToString();
    public int RpmPct => _metrics.Rpm / (_metrics.RedLine/100);
    public int FuelGauge => _metrics.FuelLevelPct;
    public int Coolant => _metrics.EngineCoolantTempC;
    public string Intake => _metrics.IntakeAirTempC.ToString();
    public int Accelerator => _metrics.AcceleratorPedalPositionPct;
    public int Throttle => _metrics.ThrottleValvePositionPct;
    public int EngineLoad => _metrics.CalculatedEngineLoadPct;
    public int Brakes => _metrics.BrakesPct;
    public string Stint => $"{(int)_stopwatch.Elapsed.TotalMinutes:00}:{_stopwatch.Elapsed.Seconds:00}";

    public void RegisterHighSpeedRefresh(Action action)
    {
        _highSpeedTimer.Tick += (_, _) => action.Invoke();
    }
    
    public void RegisterLowSpeedRefresh(Action action)
    {
        _lowSpeedTimer.Tick += (_, _) => action.Invoke();
    }
}