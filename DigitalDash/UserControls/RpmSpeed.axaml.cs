using Avalonia.Controls;

namespace DigitalDash.UserControls;

public partial class RpmSpeed : UserControl
{
    public RpmSpeed()
    {
        InitializeComponent();

        App.Logic.HighSpeedTimer.Tick += (_, _) =>
        {
            Rpm.Text = App.Logic.Rpm;
            Speed.Text = App.Logic.SpeedKmh;
        };
    }
}