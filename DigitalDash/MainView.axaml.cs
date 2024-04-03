using Avalonia.Controls;

namespace DigitalDash;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        var f = new Fuel();
    }
}