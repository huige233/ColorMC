using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using ColorMC.Gui.Objs;
using ColorMC.Gui.UI.Windows;
using ColorMC.Gui.UIBinding;

namespace ColorMC.Gui.UI.Controls.Setting;

public partial class FlyoutsControl : UserControl
{
    private JavaDisplayObj Obj;
    private FlyoutBase FlyoutBase;
    private SettingWindow Win;
    public FlyoutsControl()
    {
        InitializeComponent();

        Button1.Click += Button1_Click;
    }

    private void Button1_Click(object? sender, RoutedEventArgs e)
    {
        FlyoutBase.Hide();

        JavaBinding.RemoveJava(Obj.Name);
        Win.Tab5Load();
    }

    public void Set(FlyoutBase fb, JavaDisplayObj obj, SettingWindow win)
    {
        Win = win;
        Obj = obj;
        FlyoutBase = fb;
    }
}

public class SettingFlyout : FlyoutBase
{
    private JavaDisplayObj Obj;
    private SettingWindow Win;
    public SettingFlyout(SettingWindow win, JavaDisplayObj obj)
    {
        Win = win;
        Obj = obj;
    }
    protected override Control CreatePresenter()
    {
        var control = new FlyoutsControl();
        control.Set(this, Obj, Win);
        return control;
    }
}
