using Avalonia.Controls;
using Avalonia.Input;
using ColorMC.Gui.Objs;

namespace ColorMC.Gui.UI.Controls.GameEdit;

public partial class WorldControl : UserControl
{
    private Tab5Control Tab;
    public WorldDisplayObj World { get; private set; }
    public WorldControl()
    {
        InitializeComponent();

        PointerPressed += WorldControl_PointerPressed;
    }

    private void WorldControl_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Tab.SetSelect(this);

        if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
        {
            new GameEditFlyout2(Tab, World).ShowAt(this, true);
        }
    }

    public void Load(WorldDisplayObj world)
    {
        World = world;

        Label1.Text = world.Name;
        Label2.Text = world.Mode;
        Label3.Text = world.Time;
        Label4.Text = world.Local;
        Label5.Text = world.Difficulty;
        Label6.Text = world.Hardcore.ToString();

        if (world.Pic != null)
        {
            Image1.Source = world.Pic;
        }

        Tab = this.FindTop<Tab5Control>()!;
    }

    public void SetSelect(bool select)
    {
        Rectangle1.IsVisible = select;
    }
}
