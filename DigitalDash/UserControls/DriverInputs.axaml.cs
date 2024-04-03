using Avalonia.Controls;

namespace DigitalDash.UserControls;

public partial class DriverInputs : UserControl
{
    public DriverInputs()
    {
        InitializeComponent();
        
        App.Logic.HighSpeedTimer.Tick += (_, _) =>
        {
            Accelerator.Value = App.Logic.Accelerator;
            Throttle.Value = App.Logic.Throttle;
            EngineLoad.Value = App.Logic.EngineLoad;
        };
    }
}