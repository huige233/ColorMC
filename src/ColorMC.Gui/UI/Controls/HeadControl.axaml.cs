using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace ColorMC.Gui.UI.Controls;

public partial class HeadControl : UserControl
{
    public static readonly StyledProperty<object> TitleProperty =
            AvaloniaProperty.Register<Label, object>(nameof(Title), defaultBindingMode: BindingMode.TwoWay);

    private bool min;
    private bool max;
    public object Title
    {
        get { return GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public bool Max
    {
        get { return max; }
        set
        {
            max = value;
            Button_Max.IsVisible = max;
        }
    }

    public bool Min
    {
        get { return min; }
        set
        {
            min = value;
            Button_Min.IsVisible = min;
        }
    }

    private Window? BindWindow;

    public HeadControl()
    {
        InitializeComponent();

        DataContext = this;

        Rectangle1.PointerPressed += HeadControl_PointerPressed;
        TitleShow.PointerPressed += HeadControl_PointerPressed;

        Button_Min.Click += ButtonMin_Click;
        Button_Max.Click += ButtonMax_Click;
        Button_Close.Click += ButtonClose_Click;
    }

    private void ButtonClose_Click(object? sender, RoutedEventArgs e)
    {
        BindWindow ??= this.FindWindow();
        if (BindWindow == null)
        {
            return;
        }

        BindWindow.Close();
    }

    private void ButtonMax_Click(object? sender, RoutedEventArgs e)
    {
        BindWindow ??= this.FindWindow();
        if (BindWindow == null)
        {
            return;
        }

        if (BindWindow.WindowState == WindowState.Maximized)
            BindWindow.WindowState = WindowState.Normal;
        else
            BindWindow.WindowState = WindowState.Maximized;
    }

    private void ButtonMin_Click(object? sender, RoutedEventArgs e)
    {
        BindWindow ??= this.FindWindow();
        if (BindWindow == null)
        {
            return;
        }

        BindWindow.WindowState = WindowState.Minimized;
    }

    private void HeadControl_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BindWindow ??= this.FindWindow();
        if (BindWindow == null)
        {
            return;
        }

        BindWindow.BeginMoveDrag(e);
    }
}
