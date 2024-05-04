using Mx5NcStn1110;

if (args.Length != 1)
{
    Console.WriteLine("missing portName arg");
    return;
}

var portName = args[0];

var stn = new Stn1110(portName, 921600);

var metrics = new Mx5NcMetrics(stn);

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    Console.WriteLine("Stopping...");
    cts.Cancel();
    e.Cancel = true;
};

var cancellationToken = cts.Token;

metrics.Setup();
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
    stn.Close(); // release serial port
}
