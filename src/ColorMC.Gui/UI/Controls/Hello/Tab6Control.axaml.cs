using Avalonia.Controls;
using Avalonia.Interactivity;
using ColorMC.Gui.UI.Windows;
using ColorMC.Gui.UIBinding;

namespace ColorMC.Gui.UI.Controls.Hello;

public partial class Tab6Control : UserControl
{
    private HelloWindow Window;
    public Tab6Control()
    {
        InitializeComponent();

        Button_AddGame.Click += AddGame_Click;
        Button_AddJava.Click += AddJava_Click;
        Button_AddUser.Click += AddUser_Click;
        Button_LoadConfig.Click += LoadConfig_Click;
        Button_Next.Click += Button_Next_Click;

        Load();
    }

    private void Button_Next_Click(object? sender, RoutedEventArgs e)
    {
        Window.Done();
    }

    private void LoadConfig_Click(object? sender, RoutedEventArgs e)
    {
        Window.SwitchTab(1);
    }

    private void AddUser_Click(object? sender, RoutedEventArgs e)
    {
        Window.SwitchTab(3);
    }

    private void AddJava_Click(object? sender, RoutedEventArgs e)
    {
        Window.SwitchTab(2);
    }

    private void AddGame_Click(object? sender, RoutedEventArgs e)
    {
        Window.SwitchTab(4);
    }

    public void SetWindow(HelloWindow window)
    {
        Window = window;
    }

    public void Load()
    {
        Label_Count1.Content = JavaBinding.GetJavaInfo().Count;
        Label_Count2.Content = UserBinding.GetAllUser().Count;
        Label_Count3.Content = GameBinding.GetGames().Count;
    }
}
