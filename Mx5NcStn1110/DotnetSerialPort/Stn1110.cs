using System.Globalization;
using System.IO.Ports;

namespace Mx5NcStn1110.DotnetSerialPort;

public class Stn1110
{
    private readonly SerialPort _port;
    private readonly byte[] _readBuffer = new byte[20 /* 3 id chars, 16 data chars, 1 CR char */];
    
    public Stn1110(string portName, int baudRate)
    {
        _port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
        _port.ReadTimeout = 1000;
        _port.WriteTimeout = 1000;
        _port.NewLine = "\r";
    }

    public void SetupConnection()
    {
        _port.Open();
        
        Reset();
        
        // disable echo
        SendCommand("ATE0");
        
        // enable can headers display
        SendCommand("ATH1");
        
        // disable spaces
        SendCommand("ATS0");
    }

    public void AddFilter(ushort canId)
    {
        if (canId > 0xFFF)
            throw new ArgumentException($"canId can be max 0xFFF, but is <{canId:X}>", nameof(canId));

        SendCommand($"STFPA{canId:X3},FFF");
    }

    public void StartMonitoring()
    {
        _port.DiscardInBuffer();
        WriteLine("STM");
    }

    public void StopMonitoring()
    {
        _port.DiscardInBuffer(); // TODO should drain until we read STOPPED or error
        // send a single \r
        SendCommand("", "STOPPED");
    }

    public async ValueTask<CanMessage?> ReadCanMessage(CancellationToken cancellationToken)
    {
        if (_port.BytesToRead < _readBuffer.Length)
            return null;
        
        Array.Clear(_readBuffer);
        
        // TODO: this only works if the first byte we read is the start of a line
        await _port.BaseStream.ReadExactlyAsync(_readBuffer, cancellationToken);

        if (_readBuffer.Last() != '\r')
        {
            Console.WriteLine($"not ending with CR {System.Text.Encoding.ASCII.GetString(_readBuffer).Replace("\r", "<CR>")}");
            return null;
        }

        var line = System.Text.Encoding.ASCII.GetString(_readBuffer).TrimEnd('\r');
        return ParseCanMessageString(line);
    }

    public void Close()
    {
        _port.Close();
    }

    private void Reset()
    {
        WriteLine("ATZ");

        var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = ReadLineNoTimeout();
            if (line != null && line.Contains("ELM327"))
                break;
        }
    }

    private void SendCommand(string cmd, string expectedAck = "OK")
    {
        WriteLine(cmd);
        var ack = ReadLine();
        if (ack == cmd) // discard echo and try to get ack from next line
            ack = ReadLine();

        if (ack != expectedAck)
            throw new Stn1110Exception($"cmd <{cmd}> expected ack<{expectedAck}> but got <{ack}>");
    }

    private static CanMessage? ParseCanMessageString(string message)
    {
        if (message.Length < 4) // we expect at least 3 id bytes and 1 data byte
            return null; // unhandled message
        var id = ushort.Parse(message[..3], NumberStyles.HexNumber);
        var data = Convert.FromHexString(message[3..]);

        return new CanMessage(id, data);
    }

    private void WriteLine(string text)
    {
        Console.WriteLine($"out: '{text}'");
        _port.WriteLine(text);
    }

    private string? ReadLine(bool log = true) 
    {
        while (true)
        {
            var text = _port.ReadLine()?.Trim('>'); // discard prompt char
            if (log) Console.WriteLine($"in: '{text}'");
            if (text == "") continue; // discard empty response
            return text;
        }
    }

    private string? ReadLineNoTimeout(bool log = true)
    {
        try
        {
            return ReadLine(log);
        }
        catch (TimeoutException)
        {
            return null;
        }
    }
}