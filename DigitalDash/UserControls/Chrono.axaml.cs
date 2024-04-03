using Avalonia.Controls;

namespace DigitalDash.UserControls;

public partial class Chrono : UserControl
{
    public Chrono()
    {
        InitializeComponent();
        
        App.Logic.HighSpeedTimer.Tick += (_, _) =>
        {
            Stint.Text = App.Logic.Stint;
        };
    }
}