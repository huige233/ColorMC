using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using ColorMC.Gui.UI.Windows;
using ColorMC.Gui.UIBinding;

namespace ColorMC.Gui.UI.Controls.Main;

public partial class FlyoutsControl : UserControl
{
    private GameControl Obj;
    private FlyoutBase FlyoutBase;
    private MainWindow Win;
    public FlyoutsControl()
    {
        InitializeComponent();

        Button1.Click += Button1_Click;
        Button2.Click += Button2_Click;
        Button3.Click += Button3_Click;
        Button4.Click += Button4_Click;
        Button5.Click += Button5_Click;
        Button6.Click += Button6_Click;
        Button7.Click += Button7_Click;
        Button8.Click += Button8_Click;
        Button9.Click += Button9_Click;
        Button10.Click += Button10_Click;
    }

    private void Button10_Click(object? sender, RoutedEventArgs e)
    {
        FlyoutBase.Hide();
        BaseBinding.StopGame(Obj.Obj);
    }

    private async void Button9_Click(object? sender, RoutedEventArgs e)
    {
        FlyoutBase.Hide();
        await GameBinding.SetGameIconFromFile(Win, Obj.Obj);
        Obj.Reload();
    }

    private void Button8_Click(object? sender, RoutedEventArgs e)
    {
        FlyoutBase.Hide();
        App.ShowGameEdit(Obj.Obj, 5);
    }

    private void Button7_Click(object? sender, RoutedEventArgs e)
    {
        FlyoutBase.Hide();
        GameBinding.OpPath(Obj.Obj);
    }

    private void Button6_Click(object? sender, RoutedEventArgs e)
    {
        FlyoutBase.Hide();
        Win.DeleteGame(Obj.Obj);
    }

    private void Button5_Click(object? sender, RoutedEventArgs e)
    {
        FlyoutBase.Hide();
        Win.EditGroup(Obj.Obj);
    }

    private void Button4_Click(object? sender, RoutedEventArgs e)
    {
        FlyoutBase.Hide();
        App.ShowGameEdit(Obj.Obj, 3);
    }

    private void Button3_Click(object? sender, RoutedEventArgs e)
    {
        FlyoutBase.Hide();
        App.ShowGameEdit(Obj.Obj, 2);
    }

    private void Button2_Click(object? sender, RoutedEventArgs e)
    {
        FlyoutBase.Hide();
        App.ShowGameEdit(Obj.Obj, 1);
    }

    private void Button1_Click(object? sender, RoutedEventArgs e)
    {
        FlyoutBase.Hide();
        Win.Launch(true);
    }

    public void Set(FlyoutBase fb, GameControl obj, MainWindow win)
    {
        Obj = obj;
        FlyoutBase = fb;
        Win = win;

        var run = BaseBinding.IsGameRun(obj.Obj);
        if (run)
        {
            Button1.IsEnabled = false;
            Button6.IsEnabled = false;
            Button8.IsEnabled = false;
            Button10.IsEnabled = true;
        }
        else
        {
            Button10.IsEnabled = false;
        }
    }
}

public class MainFlyout : FlyoutBase
{
    private GameControl Obj;
    private MainWindow Win;
    public MainFlyout(MainWindow win, GameControl obj)
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
