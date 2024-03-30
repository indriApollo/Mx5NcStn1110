using System.Globalization;
using System.IO.Ports;

namespace Mx5NcStn1110;

public class Stn1110
{
    private readonly SerialPort _port;
    
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
        
        // reset
        SendCommand("ATZ", "ELM327 v1.4b");
        
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
        // send a single \r
        SendCommand("", "STOPPED");
    }

    public string? ReadCanMessage()
    {
        if (_port.BytesToRead <= 0)
            return null;
        
        try
        {
            return ReadLine();
        }
        catch (TimeoutException)
        {
            return null;
        }
    }

    private void SendCommand(string cmd, string expectedAck = "OK")
    {
        WriteLine(cmd);
        var ack = ReadLine();

        if (ack != expectedAck)
            throw new Stn1110Exception($"cmd <{cmd}> expected ack<{expectedAck}> but got <{ack}>");
    }

    private CanMessage? ParseCanMessageString(string message)
    {
        if (message.Length < 4) // we expect at least 3 id bytes and 1 data byte
            return null; // unhandled message
        var id = ushort.Parse(message[..3], NumberStyles.HexNumber);
        var data = Convert.FromHexString(message[4..]);

        return new CanMessage(id, data);
    }

    private void WriteLine(string text)
    {
        Console.WriteLine($"out: '{text}'");
        _port.WriteLine(text);
    }

    private string? ReadLine()
    {
        var text = _port.ReadLine();
        Console.WriteLine($"in: '{text}'");
        return text;
    }
}