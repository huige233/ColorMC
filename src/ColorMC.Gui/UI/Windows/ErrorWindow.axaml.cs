using Avalonia.Controls;
using System;

namespace ColorMC.Gui.UI.Windows;

public partial class ErrorWindow : Window
{
    public ErrorWindow()
    {
        InitializeComponent();

        this.Init();
        Icon = App.Icon;
        Rectangle1.MakeResizeDrag(this);
    }

    public void Show(string data, Exception e, bool close)
    {
        Data.Text = $"{data}{Environment.NewLine}{e}";

        if (close)
        {
            Show();
            App.Close();
        }
        else
        {
            Show();
        }
    }

    public void Show(string data, string e, bool close)
    {
        Data.Text = $"{data}{Environment.NewLine}{e}";

        if (close)
        {
            Show();
            App.Close();
        }
        else
        {
            Show();
        }
    }
}
