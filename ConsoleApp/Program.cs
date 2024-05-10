using Mx5NcStn1110;

var sharedMemory = false;
var unixDomainSocketName = "";
var serialPortName = "";
var baudRate = 921600;

if (args.Contains("--shm"))
{
    sharedMemory = true;
}

if (args.Contains("--uds"))
{
    unixDomainSocketName = args[Array.IndexOf(args, "--uds") + 1];
}
        
if (args.Contains("--serial-port"))
{
    serialPortName = args[Array.IndexOf(args, "--serial-port") + 1];
}
        
if (args.Contains("--baud-rate"))
{
    baudRate = int.Parse(args[Array.IndexOf(args, "--baud-rate") + 1]);
}

var metrics = MetricsFactory.GetMetrics(sharedMemory, unixDomainSocketName, serialPortName, baudRate);

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

try
{
    await run; // collect any exceptions
}
finally
{
    (metrics as IDisposable)?.Dispose();
}

