﻿using ColorMC.Core.Net;
using ColorMC.Core.Objs;
using ColorMC.Core.Utils;
using ColorMC.Gui.Objs;
using ColorMC.Gui.Utils.LaunchSetting;
using System.Threading.Tasks;

namespace ColorMC.Gui.UIBinding;

public static class ConfigBinding
{
    public static bool LoadAuthDatabase(string dir)
    {
        return AuthDatabase.LoadData(dir);
    }

    public static bool LoadConfig(string dir)
    {
        return ConfigUtils.Load(dir, true);
    }

    public static bool LoadGuiConfig(string dir)
    {
        return GuiConfigUtils.Load(dir, true);
    }

    public static (ConfigObj, GuiConfigObj) GetAllConfig()
    {
        return (ConfigUtils.Config, GuiConfigUtils.Config);
    }

    public static void SetRgb(bool enable)
    {
        GuiConfigUtils.Config.RGB = enable;

        GuiConfigUtils.Save();
        ColorSel.Instance.Load();
    }

    public static void SetRgb(int v1, int v2)
    {
        GuiConfigUtils.Config.RGBS = v1;
        GuiConfigUtils.Config.RGBV = v2;
        GuiConfigUtils.Save();
        ColorSel.Instance.Load();
    }

    public static void SetColor(string main, string back, string back1, string font1, string font2)
    {
        GuiConfigUtils.Config.ColorMain = main;
        GuiConfigUtils.Config.ColorBack = back;
        GuiConfigUtils.Config.ColorTranBack = back1;
        GuiConfigUtils.Config.ColorFont1 = font1;
        GuiConfigUtils.Config.ColorFont2 = font2;
        GuiConfigUtils.Save();
        ColorSel.Instance.Load();
    }

    public static void DeleteGuiImageConfig()
    {
        App.RemoveImage();
        GuiConfigUtils.Config.BackImage = null;
        GuiConfigUtils.Save();
        App.OnPicUpdate();
    }

    public static async Task SetBackPic(string dir, int data)
    {
        GuiConfigUtils.Config.BackEffect = data;
        GuiConfigUtils.Config.BackImage = dir;
        GuiConfigUtils.Save();

        await App.LoadImage(dir, data);

        App.OnPicUpdate();
    }

    public static void SetBackTran(int data)
    {
        GuiConfigUtils.Config.BackTran = data;
        GuiConfigUtils.Save();

        App.OnPicUpdate();
    }

    public static void SetBl(bool open, int type)
    {
        GuiConfigUtils.Config.WindowTranType = type;
        GuiConfigUtils.Config.WindowTran = open;
        GuiConfigUtils.Save();

        App.OnPicUpdate();
    }

    public static void SetHttpConfig(HttpObj obj)
    {
        ConfigUtils.Config.Http = obj;
        ConfigUtils.Save();

        BaseClient.Init();
    }

    public static void SetJvmArgConfig(JvmArgObj obj)
    {
        ConfigUtils.Config.DefaultJvmArg = obj;
        ConfigUtils.Save();

        BaseClient.Init();
    }

    public static void SetJvmArgMemConfig(uint min, uint max)
    {
        ConfigUtils.Config.DefaultJvmArg.MinMemory = min;
        ConfigUtils.Config.DefaultJvmArg.MaxMemory = max;
        ConfigUtils.Save();

        BaseClient.Init();
    }

    public static void SetWindowSettingConfig(WindowSettingObj obj)
    {
        ConfigUtils.Config.Window = obj;
        ConfigUtils.Save();

        BaseClient.Init();
    }

    public static void SetServerCustom(ServerCustom obj)
    {
        GuiConfigUtils.Config.ServerCustom = obj;
        GuiConfigUtils.Save();

        App.MainWindow?.MotdLoad();

        ColorSel.Instance.Load();
    }

    public static void SetServerCustom(bool enable, string? name)
    {
        GuiConfigUtils.Config.ServerCustom.LockGame = enable;
        GuiConfigUtils.Config.ServerCustom.GameName = name;

        GuiConfigUtils.Save();

        App.MainWindow?.Load();
    }

    public static void SetUIFile(string text)
    {
        GuiConfigUtils.Config.ServerCustom.UIFile = text;
        GuiConfigUtils.Save();
    }

    public static void SetGameCheckConfig(GameCheckObj obj)
    {
        ConfigUtils.Config.GameCheck = obj;
        ConfigUtils.Save();
    }

    public static void SetFont(string? name, bool def)
    {
        GuiConfigUtils.Config.FontName = name;
        GuiConfigUtils.Config.FontDefault = def;

        GuiConfigUtils.Save();

        FontSel.Instance.Load();
    }

    public static void ResetConfig()
    {
        GuiConfigUtils.Config = GuiConfigUtils.MakeDefaultConfig();

        GuiConfigUtils.Save();
    }
}
