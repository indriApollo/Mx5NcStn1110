using Avalonia;
using System;
using System.Linq;
using System.Threading;

namespace DigitalDash;

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once ArrangeTypeModifiers
class Program
{
    public static string UnixDomainSocketName { get; private set; } = "";
    public static string SerialPortName { get; private set; } = "";
    public static int BaudRate { get; private set; } = 921600;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static int Main(string[] args)
    {
        bool uds = false, sp = false, br = false;
        
        if (args.Contains("--uds"))
        {
            uds = true;
            UnixDomainSocketName = args[Array.IndexOf(args, "--uds") + 1];
        }
        
        if (args.Contains("--serial-port"))
        {
            sp = true;
            SerialPortName = args[Array.IndexOf(args, "--serial-port") + 1];
        }
        
        if (args.Contains("--baud-rate"))
        {
            br = true;
            BaudRate = int.Parse(args[Array.IndexOf(args, "--baud-rate") + 1]);
        }

        if (uds && (sp || br))
            throw new ArgumentException("Can't use UDS and Serial options at the same time", nameof(args));

        if (br && !sp)
            throw new ArgumentException("Must also provide Serial Port when providing Baud Rate", nameof(args));
        
        var builder = BuildAvaloniaApp();
        
        if (!args.Contains("--drm"))
            return builder.StartWithClassicDesktopLifetime(args);
        
        if (args.Contains("--silence-console"))
            SilenceConsole();
        
        // By default, Avalonia will try to detect output card automatically.
        // But you can specify one, for example "/dev/dri/card1".
        return builder.StartLinuxDrm(args: args, card: null, scaling: 1.0);

    }

    // Avalonia configuration, don't remove; also used by visual designer.
    // ReSharper disable once MemberCanBePrivate.Global
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    
    private static void SilenceConsole()
    {
        // ReSharper disable once FunctionNeverReturns
        new Thread(() =>
            {
                Console.CursorVisible = false;
                while (true)
                    Console.ReadKey(true);
            })
            { IsBackground = true }.Start();
    }
}