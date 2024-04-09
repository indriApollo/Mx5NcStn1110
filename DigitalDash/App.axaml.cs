using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace DigitalDash;

public class App : Application
{
    public static readonly Logic Logic = new(Program.SerialPortName, Program.BaudRate);
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var lifetime = ApplicationLifetime as IControlledApplicationLifetime
                ?? throw new ArgumentNullException(nameof(ApplicationLifetime));

        // Collect metrics until the application is exiting
        var cts = new CancellationTokenSource();
        var collectAsync = Logic.CollectAsync(cts.Token);
        lifetime.Exit += async (_, _) =>
        {
            await cts.CancelAsync();
            await collectAsync;
        };

        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.MainWindow = new Views.MainWindow();
                break;
            case ISingleViewApplicationLifetime singleView:
                singleView.MainView = new Views.MainSingleView();
                break;
        }

        base.OnFrameworkInitializationCompleted();
    }
}