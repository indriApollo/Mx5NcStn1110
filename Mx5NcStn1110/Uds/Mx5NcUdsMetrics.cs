using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace Mx5NcStn1110.Uds;

public class Mx5NcUdsMetrics : IMetrics, IDisposable
{
    private const string ClientSocketName = "/tmp/mx5metrics.client.sock";
    private readonly Stopwatch _sw = new();

    private readonly Socket _clientSocket;

    public Mx5NcUdsMetrics(string socketName)
    {
        Unlink(ClientSocketName);
        
        var metricsEndpoint = new UnixDomainSocketEndPoint(socketName);

        var clientEndpoint = new UnixDomainSocketEndPoint(ClientSocketName);
        _clientSocket = new Socket(AddressFamily.Unix, SocketType.Dgram, ProtocolType.IP);
        _clientSocket.ReceiveTimeout = 1;

        _clientSocket.Bind(clientEndpoint);
        _clientSocket.Connect(metricsEndpoint);
    }
    
    public ushort RedLine => 7000;
    public ushort Rpm => BitConverter.ToUInt16(GetMetric(Command.GetRpm));
    public ushort SpeedKmh => BitConverter.ToUInt16(GetMetric(Command.GetSpeedKmh));
    public byte AcceleratorPedalPositionPct => GetMetric(Command.GetAcceleratorPedalPositionPct)[0];
    public byte CalculatedEngineLoadPct => GetMetric(Command.GetCalculatedEngineLoadPct)[0];
    public short EngineCoolantTempC => BitConverter.ToInt16(GetMetric(Command.GetEngineCoolantTempC));
    public byte ThrottleValvePositionPct => GetMetric(Command.GetThrottleValvePositionPct)[0];
    public short IntakeAirTempC => BitConverter.ToInt16(GetMetric(Command.GetIntakeAirTempC));
    public byte FuelLevelPct => GetMetric(Command.GetFuelLevelPct)[0];
    public byte BrakesPct => GetMetric(Command.GetBrakesPct)[0];
    public ushort FlSpeedKmh => BitConverter.ToUInt16(GetMetric(Command.GetFlSpeedKmh));
    public ushort FrSpeedKmh => BitConverter.ToUInt16(GetMetric(Command.GetFrSpeedKmh));
    public ushort RlSpeedKmh => BitConverter.ToUInt16(GetMetric(Command.GetRlSpeedKmh));
    public ushort RrSpeedKmh => BitConverter.ToUInt16(GetMetric(Command.GetRrSpeedKmh));
    public long AvgParsingTicks { get; private set; }

    public void Dispose()
    {
        _clientSocket.Close();
    }
    
    public void Setup()
    {
    }

    public async Task CollectAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(-1, cancellationToken);
        }
        catch (TaskCanceledException)
        {
        }
    }

    private ReadOnlySpan<byte> GetMetric(Command cmd)
    {
        _sw.Restart();
        var msg = new[] { (byte)cmd };
        _clientSocket.Send(msg);
        
        var rsp = new byte[32];
        _clientSocket.Receive(rsp);

        if (rsp[0] == (byte)Command.Error)
            throw new UdsMetricsException($"GetMetric error '{Encoding.ASCII.GetString(rsp.AsSpan()[1..])}' for cmd '{cmd}'");
        
        AvgParsingTicks = (_sw.ElapsedTicks + AvgParsingTicks) / 2;
        
        return rsp.AsSpan()[1..];
    }

    private static void Unlink(string socketName)
    {
        if (File.Exists(socketName))
            File.Delete(socketName);
    }
}