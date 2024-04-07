using Mx5NcStn1110;

var portName = args[0];

var stn = new Stn1110(portName, 115200);

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
var run = Task.Run(async () => await metrics.CollectAsync(cancellationToken));

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
    Console.WriteLine($"FL speed {metrics.FlSpeed} kmh");
    Console.WriteLine($"FR speed {metrics.FrSpeed} kmh");
    Console.WriteLine($"RL speed {metrics.RlSpeed} kmh");
    Console.WriteLine($"RR speed {metrics.RrSpeed} kmh");
    Console.WriteLine("=====");
    
    await Task.Delay(1000, cancellationToken);
}

await run;