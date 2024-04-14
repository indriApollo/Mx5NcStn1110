using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using Mx5NcStn1110;

namespace DigitalDash;

public class Logic
{
    private readonly DispatcherTimer _highSpeedTimer = new();
    private readonly DispatcherTimer _lowSpeedTimer = new();

    private readonly IMetrics _metrics;
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    public Logic(string serialPortName, int baudRate)
    {
        _metrics = serialPortName == "fake"
            ? new FakeMetrics()
            : new Mx5NcMetrics(new Stn1110(serialPortName, baudRate));
        
        _metrics.Setup();
        
        _highSpeedTimer.Interval = TimeSpan.FromMilliseconds(1000d/30d); // 30hz
        _lowSpeedTimer.Interval = TimeSpan.FromMilliseconds(1000); // 1hz
        
        _highSpeedTimer.Start();
        _lowSpeedTimer.Start();
    }

    public const int CoolantAlertThrC = 100;
    public const int RpmWarningThrPct = 80;
    public const int RpmAlertThrPct = 90;
    public const int FuelGaugeAlertThrPct = 15;
    public const int WheelSpeedDiffAlertThrKmh = 3;

    public ushort SpeedKmh => _metrics.SpeedKmh;
    public ushort Rpm => _metrics.Rpm;
    public int RpmPct => _metrics.Rpm / (_metrics.RedLine/100);
    public int FuelGauge => _metrics.FuelLevelPct;
    public int Coolant => _metrics.EngineCoolantTempC;
    public short Intake => _metrics.IntakeAirTempC;
    public int Accelerator => _metrics.AcceleratorPedalPositionPct;
    public int Throttle => _metrics.ThrottleValvePositionPct;
    public int EngineLoad => _metrics.CalculatedEngineLoadPct;
    public int Brakes => _metrics.BrakesPct;
    public ushort FlSpeed => _metrics.FlSpeedKmh;
    public ushort FrSpeed => _metrics.FrSpeedKmh;
    public ushort RlSpeed => _metrics.RlSpeedKmh;
    public ushort RrSpeed => _metrics.RrSpeedKmh;
    public string Stint => $"{(int)_stopwatch.Elapsed.TotalMinutes:00}:{_stopwatch.Elapsed.Seconds:00}";

    public void RegisterHighSpeedRefresh(Action action)
    {
        _highSpeedTimer.Tick += (_, _) => action.Invoke();
    }
    
    public void RegisterLowSpeedRefresh(Action action)
    {
        _lowSpeedTimer.Tick += (_, _) => action.Invoke();
    }

    public Task CollectAsync(CancellationToken cancellationToken) => Task.Run(async () => await _metrics.CollectAsync(cancellationToken), cancellationToken);
}