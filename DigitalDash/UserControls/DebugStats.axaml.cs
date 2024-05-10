using Avalonia.Controls;

namespace DigitalDash.UserControls;

public partial class DebugStats : UserControl
{
    private readonly Logic _logic = App.Logic;
    
    public DebugStats()
    {
        InitializeComponent();
        
        _logic.RegisterLowSpeedRefresh(Refresh);
    }
    
    private void Refresh()
    {
        AvgParsing.Text = _logic.AvgParsingMs.ToString();
    }
}