using Avalonia.Controls;

namespace DigitalDash.UserControls;

public partial class Temperatures : UserControl
{
    public Temperatures()
    {
        InitializeComponent();
        
        App.Logic.LowSpeedTimer.Tick += (_, _) =>
        {
            Coolant.Text = App.Logic.Coolant;
            Intake.Text = App.Logic.Intake;
        };
    }
}