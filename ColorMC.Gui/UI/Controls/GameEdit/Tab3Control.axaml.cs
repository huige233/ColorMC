using Avalonia.Controls;
using Avalonia.Input;
using ColorMC.Core.Objs;
using ColorMC.Core.Objs.Minecraft;
using ColorMC.Gui.Objs;
using ColorMC.Gui.UI.Windows;
using ColorMC.Gui.UIBinding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Interactivity;
using System.Linq;
using ColorMC.Core.LaunchPath;
using System.IO;
using Avalonia.Threading;
using AvaloniaEdit.Indentation.CSharp;
using AvaloniaEdit;
using TextMateSharp.Grammars;
using AvaloniaEdit.TextMate;
using DynamicData;
using AvaloniaEdit.Document;
using System.Resources;
using System.Security.Cryptography.X509Certificates;
using ColorMC.Gui.Utils.LaunchSetting;

namespace ColorMC.Gui.UI.Controls.GameEdit;

public partial class Tab3Control : UserControl
{
    private readonly ObservableCollection<string> List = new();
    private readonly List<string> Items = new();
    private GameEditWindow Window;
    private GameSettingObj Obj;
    private TextMate.Installation textMateInstallation;
    private RegistryOptions registryOptions;
    public Tab3Control()
    {
        InitializeComponent();

        ComboBox1.Items = List;
        ComboBox1.SelectionChanged += ComboBox1_SelectionChanged;

        Button1.Click += Button1_Click;
        Button2.Click += Button2_Click;
        Button3.Click += Button3_Click;

        TextEditor1.Options.ShowBoxForControlCharacters = true;
        TextEditor1.TextArea.IndentationStrategy = new CSharpIndentationStrategy(TextEditor1.Options);

        registryOptions = new RegistryOptions(ThemeName.LightPlus);
        textMateInstallation = TextEditor1.InstallTextMate(registryOptions);
    }

    private void Button3_Click(object? sender, RoutedEventArgs e)
    {
        var item = ComboBox1.SelectedItem as string;
        if (item == null)
            return;

        GameBinding.SaveConfigFile(Obj, item, TextEditor1.Document.Text);
    }

    private void Button2_Click(object? sender, RoutedEventArgs e)
    {
        var item = ComboBox1.SelectedItem as string;
        if (item == null)
            return;

        var dir = Obj.GetGameDir();
        GameBinding.OpFile(Path.GetFullPath(dir + "/" + item));
    }

    private void Button1_Click(object? sender, RoutedEventArgs e)
    {
        Load();
    }

    private void ComboBox1_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var item = ComboBox1.SelectedItem as string;
        if (item == null)
            return;

        var text = GameBinding.ReadConfigFile(Obj, item);
        var ex = item[item.LastIndexOf('.')..];

        EditGa(ex);
        TextEditor1.Document = new AvaloniaEdit.Document.TextDocument(text); 
    }

    public void EditGa(string name) 
    {
        var item = registryOptions.GetLanguageByExtension(name);
        if (item == null)
        {
            textMateInstallation.SetGrammar(null);
            return;
        }
        var item1 = registryOptions.GetScopeByLanguageId(item.Id);
        if (item == null)
            return;
        textMateInstallation.SetGrammar(item1);
    }

    public void Load() 
    {
        string fil = TextBox1.Text;
        List.Clear();
        if (string.IsNullOrWhiteSpace(fil))
        {
            List.AddRange(Items);
        }
        else
        {
            var list = from item in Items
                       where item.Contains(fil)
                       select item;
            List.AddRange(list);
        }

        if (List.Count != 0)
        {
            ComboBox1.SelectedIndex = 0;
        }
        else
        {
            TextEditor1.Document = null;
        }
    }

    public void SetWindow(GameEditWindow window)
    {
        Window = window;
    }

    public void SetGame(GameSettingObj obj)
    {
        Obj = obj;
    }

    public void Update()
    {
        if (Obj == null)
            return;

        Items.Clear();
        var list = GameBinding.GetAllConfig(Obj);
        Items.AddRange(list);

        Load();
    }
}
