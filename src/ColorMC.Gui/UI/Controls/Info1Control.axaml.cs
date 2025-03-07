using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Threading;

namespace ColorMC.Gui.UI.Controls;

public partial class Info1Control : UserControl
{
    private Action? call;
    private bool Display = false;

    public Info1Control()
    {
        InitializeComponent();

        Button_Cancel.Click += Cancel_Click;
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Button_Cancel.IsEnabled = false;
        App.CrossFade300.Start(this, null, CancellationToken.None);

        call?.Invoke();
    }

    public void Close()
    {
        if (!Display)
            return;

        Button_Cancel.IsEnabled = false;
        App.CrossFade300.Start(this, null, CancellationToken.None);
    }

    public void Show()
    {
        Display = true;

        App.CrossFade300.Start(null, this, CancellationToken.None);
    }

    public void Show(string title)
    {
        Display = true;

        Button_Cancel.IsEnabled = true;
        TextBlock_Text.Text = title;
        call = null;

        Button_Cancel.IsVisible = false;

        App.CrossFade300.Start(null, this, CancellationToken.None);
    }

    public void Show(string title, Action cancel)
    {
        Button_Cancel.IsEnabled = true;
        TextBlock_Text.Text = title;
        call = cancel;

        App.CrossFade300.Start(null, this, CancellationToken.None);
    }

    public void Progress(double value)
    {
        if (value == -1)
        {
            ProgressBar_Value.IsIndeterminate = true;
            ProgressBar_Value.ShowProgressText = false;
        }
        else
        {
            ProgressBar_Value.IsIndeterminate = false;
            ProgressBar_Value.Value = value;
            ProgressBar_Value.ShowProgressText = true;
        }
    }

    public void NextText(string title)
    {
        TextBlock_Text.Text = title;
    }
}
