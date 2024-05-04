using System.IO.Ports;
using System.Text.RegularExpressions;
// ReSharper disable InconsistentNaming

namespace Stn1110Simulator;

public partial class Stn1110Simulator
{
    private readonly HashSet<string> _filteredCanIds = [];
    private readonly SerialPort _port;

    public Stn1110Simulator(string portName, int baudRate)
    {
        _port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
        _port.ReadTimeout = 1000;
        _port.WriteTimeout = 1000;
        _port.NewLine = "\r";
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        _port.Open();
        
        while (!cancellationToken.IsCancellationRequested)
        {
            string line;
            try
            {
                line = ReadLine();
            }
            catch (TimeoutException)
            {
                continue;
            }

            if (line == "ATZ")
            {
                await RespondToATZ();
            }
            else if (line == "ATE0")
            {
                RespondToATE0();
            }
            else if (line == "ATH1")
            {
                RespondToATH1();
            }
            else if (line == "ATS0")
            {
                RespondToATS0();
            }
            else if (line.StartsWith("STFPA"))
            {
                HandleSTFPA(line);
            }
            else
            {
                WriteLine("?");
                Write(">");
            }
        }

        _port.Close();
    }

    private async Task RespondToATZ()
    {
        await Task.Delay(TimeSpan.FromSeconds(5));
        WriteLine("ELM327 v1.5simu");
        Write(">");
        _port.DiscardInBuffer();
    }

    private void RespondToATE0()
    {
        WriteLine("OK");
        Write(">");
    }
    
    private void RespondToATH1()
    {
        WriteLine("OK");
        Write(">");
    }
    
    private void RespondToATS0()
    {
        WriteLine("OK");
        Write(">");
    }

    private void HandleSTFPA(string cmd)
    {
        var m = CanFilterRegex().Match(cmd);
        if (!m.Success)
        {
            WriteLine("ERROR");
            Write(">");
            return;
        }

        var filterCanId = m.Groups[1].Value;
        Console.WriteLine($"got filter can id {filterCanId}");
        _filteredCanIds.Add(filterCanId);
        WriteLine("OK");
        Write(">");
    }

    private void WriteLine(string text)
    {
        Console.WriteLine($"out:{text}");
        _port.WriteLine(text);
    }

    private void Write(string text)
    {
        Console.WriteLine($"out:{text}");
        _port.Write(text);
    }

    private string ReadLine()
    {
        var line = _port.ReadLine();
        Console.WriteLine($"in:{line}"
            .Replace("\r", "[CR]")
            .Replace("\n", "[LF]"));
        return line;
    }

    [GeneratedRegex("STFPA([0-9A-F]{3}),[0-9A-F]{3}")]
    private static partial Regex CanFilterRegex();
}