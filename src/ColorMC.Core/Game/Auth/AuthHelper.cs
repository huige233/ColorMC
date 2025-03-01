﻿using ColorMC.Core.LaunchPath;
using ColorMC.Core.Net;
using ColorMC.Core.Net.Downloader;
using ColorMC.Core.Objs.Login;
using ColorMC.Core.Utils;
using Newtonsoft.Json;

namespace ColorMC.Core.Game.Auth;

public static class AuthHelper
{
    /// <summary>
    /// AuthlibInjector 物理位置
    /// </summary>
    public static string NowAuthlibInjector { get; private set; }
    /// <summary>
    /// Nide8Injector 物理位置
    /// </summary>
    public static string NowNide8Injector { get; private set; }

    /// <summary>
    /// 创建Nide8Injector下载实例
    /// </summary>
    /// <returns>下载实例</returns>
    private static DownloadItem BuildNide8Item()
    {
        return new()
        {
            Url = "https://login.mc-user.com:233/download/nide8auth.jar",
            Name = "com.nide8.login2:nide8auth:2.3",
            Local = $"{LibrariesPath.BaseDir}/com/nide8/login2/nide8auth/2.3/nide8auth.2.3.jar",
        };
    }
    /// <summary>
    /// 创建AuthlibInjector下载实例
    /// </summary>
    /// <param name="obj">AuthlibInjector信息</param>
    /// <returns>下载实例</returns>
    private static DownloadItem BuildAuthlibInjectorItem(AuthlibInjectorObj obj)
    {
        return new()
        {
            SHA256 = obj.checksums.sha256,
            Url = UrlHelper.DownloadAuthlibInjector(obj, BaseClient.Source),
            Name = $"moe.yushi:authlibinjector:{obj.version}",
            Local = $"{LibrariesPath.BaseDir}/moe/yushi/authlibinjector/" +
            $"{obj.version}/authlib-injector-{obj.version}.jar",
        };
    }

    /// <summary>
    /// 初始化Nide8Injector，存在不下载
    /// </summary>
    /// <returns>Nide8Injector下载实例</returns>
    public static DownloadItem? ReadyNide8()
    {
        var item = BuildNide8Item();
        NowNide8Injector = item.Local;

        if (!string.IsNullOrWhiteSpace(NowNide8Injector)
            && File.Exists(NowNide8Injector))
        {
            return null;
        }

        return item;
    }

    /// <summary>
    /// 初始化AuthlibInjector，存在不下载
    /// </summary>
    /// <returns>AuthlibInjector下载实例</returns>
    public static async Task<DownloadItem?> ReadyAuthlibInjector()
    {
        var meta = await BaseClient.GetString(UrlHelper.AuthlibInjectorMeta(BaseClient.Source));
        var obj = JsonConvert.DeserializeObject<AuthlibInjectorMetaObj>(meta)
            ?? throw new Exception(LanguageHelper.GetName("AuthlibInjector.Error1"));
        var item = obj.artifacts.Where(a => a.build_number == obj.latest_build_number).First();

        var info = await BaseClient.GetString(UrlHelper.AuthlibInjector(item, BaseClient.Source));
        var obj1 = JsonConvert.DeserializeObject<AuthlibInjectorObj>(info)
            ?? throw new Exception(LanguageHelper.GetName("AuthlibInjector.Error1"));
        var item1 = BuildAuthlibInjectorItem(obj1);

        NowAuthlibInjector = item1.Local;

        if (!string.IsNullOrWhiteSpace(NowAuthlibInjector)
            && File.Exists(NowAuthlibInjector))
            return null;

        return item1;
    }
}
