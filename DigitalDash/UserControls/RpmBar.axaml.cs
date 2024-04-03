using Avalonia.Controls;
using Avalonia.Media;

namespace DigitalDash.UserControls;

public partial class RpmBar : UserControl
{
    private const int BarLines = 46;
    private const float BarLinePct = BarLines / 100f;
    
    private static readonly IBrush Green = new SolidColorBrush(Colors.LawnGreen);
    private static readonly IBrush Yellow = new SolidColorBrush(Colors.Yellow);
    private static readonly IBrush Red = new SolidColorBrush(Colors.Red);
    
    public RpmBar()
    {
        InitializeComponent();
        
        App.Logic.HighSpeedTimer.Tick += (_, _) =>
        {
            var rpmPct = App.Logic.RpmPct;

            Bar.Foreground = rpmPct switch
            {
                < 80 => Green,
                < 90 => Yellow,
                _ => Red
            };
            Bar.Text = new string('|', (int)(rpmPct * BarLinePct));
        };
    }
}