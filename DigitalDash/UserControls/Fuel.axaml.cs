using Avalonia.Controls;

namespace DigitalDash.UserControls;

public partial class Fuel : UserControl
{
    public Fuel()
    {
        InitializeComponent();
        
        App.Logic.LowSpeedTimer.Tick += (_, _) =>
        {
            FuelGauge.Text = App.Logic.FuelGauge;
        };
    }
}