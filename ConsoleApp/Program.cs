using Mx5NcStn1110;
using Mx5NcStn1110.Shm;

var unixDomainSocketName = "";
var serialPortName = "";
var baudRate = 921600;

bool uds = false, sp = false, br = false;
        
if (args.Contains("--uds"))
{
    uds = true;
    unixDomainSocketName = args[Array.IndexOf(args, "--uds") + 1];
}
        
if (args.Contains("--serial-port"))
{
    sp = true;
    serialPortName = args[Array.IndexOf(args, "--serial-port") + 1];
}
        
if (args.Contains("--baud-rate"))
{
    br = true;
    baudRate = int.Parse(args[Array.IndexOf(args, "--baud-rate") + 1]);
}

if (uds && (sp || br))
    throw new ArgumentException("Can't use UDS and Serial options at the same time", nameof(args));

if (br && !sp)
    throw new ArgumentException("Must also provide Serial Port when providing Baud Rate", nameof(args));

//var metrics = MetricsFactory.GetMetrics(unixDomainSocketName, serialPortName, baudRate);
var metrics = new Mx5NcShmMetrics();

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    Console.WriteLine("Stopping...");
    cts.Cancel();
    e.Cancel = true;
};

var cancellationToken = cts.Token;

Console.WriteLine("Setup");
metrics.Setup();
Console.WriteLine("Collect");
var run = metrics.CollectAsync(cancellationToken);

while (!cancellationToken.IsCancellationRequested && !run.IsCompleted)
{
    Console.WriteLine($"RPM {metrics.Rpm}");
    Console.WriteLine($"Speed {metrics.SpeedKmh} Kmh");
    Console.WriteLine($"Accel pedal {metrics.AcceleratorPedalPositionPct} %");
    Console.WriteLine($"Throttle valve {metrics.ThrottleValvePositionPct} %");
    Console.WriteLine($"Calculated engine load {metrics.CalculatedEngineLoadPct} %");
    Console.WriteLine($"Brakes {metrics.BrakesPct} %");
    Console.WriteLine($"Coolant temp {metrics.EngineCoolantTempC} °C");
    Console.WriteLine($"Intake air temp {metrics.IntakeAirTempC} °C");
    Console.WriteLine($"Fuel level {metrics.FuelLevelPct} %");
    Console.WriteLine($"FL speed {metrics.FlSpeedKmh} kmh");
    Console.WriteLine($"FR speed {metrics.FrSpeedKmh} kmh");
    Console.WriteLine($"RL speed {metrics.RlSpeedKmh} kmh");
    Console.WriteLine($"RR speed {metrics.RrSpeedKmh} kmh");
    Console.WriteLine($"Can msg parsing avg {TimeSpan.FromTicks(metrics.AvgParsingTicks).TotalMilliseconds} ms");
    Console.WriteLine("=====");

    try
    {
        await Task.Delay(1000, cancellationToken);
    }
    catch (TaskCanceledException)
    {
    }
}

await run; // collect any exceptions
