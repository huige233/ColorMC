using ColorMC.Core.LaunchPath;
using ColorMC.Core.Net.Apis;
using ColorMC.Core.Net.Downloader;
using ColorMC.Core.Objs;
using ColorMC.Core.Objs.Loader;
using ColorMC.Core.Objs.Minecraft;
using ColorMC.Core.Utils;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using System.Text;

namespace ColorMC.Core.Net.Download;

/// <summary>
/// 下载状态
/// </summary>
public enum GetDownloadState
{
    /// <summary>
    /// 初始化
    /// </summary>
    Init,
    /// <summary>
    /// 获取数据中
    /// </summary>
    GetInfo,
    /// <summary>
    /// 结束
    /// </summary>
    End
}

public static class GameDownload
{
    /// <summary>
    /// 下载哟游戏
    /// </summary>
    /// <param name="obj">版本数据</param>
    /// <returns></returns>
    public static async Task<(GetDownloadState State, List<DownloadItem>? List)> Download(VersionObj.Versions obj)
    {
        var list = new List<DownloadItem>();

        var obj1 = await GetHelper.GetGame(obj.url);
        if (obj1 == null)
            return (GetDownloadState.Init, null);

        VersionPath.AddGame(obj1);
        var obj2 = await GetHelper.GetAssets(obj1.assetIndex.url);
        if (obj2 == null)
            return (GetDownloadState.GetInfo, null);

        AssetsPath.AddIndex(obj2, obj1);
        list.Add(new()
        {
            Name = $"{obj.id}.jar",
            Url = UrlHelper.Download(obj1.downloads.client.url, BaseClient.Source),
            Local = LibrariesPath.GetGameFile(obj.id),
            SHA1 = obj1.downloads.client.sha1
        });

        list.AddRange(GameHelper.MakeGameLibs(obj1));

        foreach (var item1 in obj2.objects)
        {
            list.Add(new()
            {
                Name = item1.Key,
                Url = UrlHelper.DownloadAssets(item1.Value.hash, BaseClient.Source),
                Local = $"{AssetsPath.ObjectsDir}/{item1.Value.hash[..2]}/{item1.Value.hash}",
                SHA1 = item1.Value.hash
            });
        }

        return (GetDownloadState.End, list);
    }

    /// <summary>
    /// 获取Forge下载项目
    /// </summary>
    /// <param name="obj">游戏实例</param>
    public static Task<(GetDownloadState State, List<DownloadItem>? List)> DownloadForge(GameSettingObj obj)
    {
        return DownloadForge(obj.Version, obj.LoaderVersion);
    }

    /// <summary>
    /// 获取Forge下载项目
    /// </summary>
    /// <param name="mc">游戏版本</param>
    /// <param name="version"><forge版本/param>
    public static async Task<(GetDownloadState State, List<DownloadItem>? List)> DownloadForge(string mc, string version)
    {
        bool v2 = CheckRule.GameLaunchVersion(mc);

        var down = ForgeHelper.BuildForgeInster(mc, version);
        try
        {
            var res = await DownloadManager.Download(down);
            if (!res)
            {
                return (GetDownloadState.Init, null);
            }
        }
        catch (Exception e)
        {
            CoreMain.OnError?.Invoke(LanguageHelper.GetName("Core.Http.Error5"), e, false);
            return (GetDownloadState.Init, null);
        }

        string name = $"forge-{mc}-{version}";
        using ZipFile zFile = new(down.Local);
        using MemoryStream stream1 = new();
        using MemoryStream stream2 = new();
        bool find1 = false;
        bool find2 = false;
        foreach (ZipEntry e in zFile)
        {
            if (e.IsFile && e.Name == "version.json")
            {
                using var stream = zFile.GetInputStream(e);
                await stream.CopyToAsync(stream1);
                find1 = true;
            }
            else if (e.IsFile && e.Name == "install_profile.json")
            {
                using var stream = zFile.GetInputStream(e);
                await stream.CopyToAsync(stream2);
                find2 = true;
            }
        }

        var list = new List<DownloadItem>();
        //1.12.2以上
        if (find1 && find2)
        {
            byte[] array1 = stream1.ToArray();
            ForgeLaunchObj info;
            try
            {
                var data = Encoding.UTF8.GetString(array1);
                info = JsonConvert.DeserializeObject<ForgeLaunchObj>(data)!;
                File.WriteAllBytes($"{VersionPath.ForgeDir}/{name}.json", array1);
            }
            catch (Exception e)
            {
                Logs.Error(LanguageHelper.GetName("Core.Http.Error2"), e);
                return (GetDownloadState.GetInfo, null);
            }

            list.AddRange(ForgeHelper.MakeForgeLibs(info, mc, version));

            byte[] array2 = stream2.ToArray();
            ForgeInstallObj info1;
            var data1 = Encoding.UTF8.GetString(array2);
            try
            {
                info1 = JsonConvert.DeserializeObject<ForgeInstallObj>(data1)!;
                File.WriteAllBytes($"{VersionPath.ForgeDir}/{name}-install.json", stream2.ToArray());
            }
            catch (Exception e)
            {
                Logs.Error(LanguageHelper.GetName("Core.Http.Error3"), e);
                return (GetDownloadState.GetInfo, null);
            }

            foreach (var item1 in info1.libraries)
            {
                if (string.IsNullOrWhiteSpace(item1.downloads.artifact.url))
                    continue;

                list.Add(new()
                {
                    Url = UrlHelper.DownloadForgeLib(item1.downloads.artifact.url,
                             BaseClient.Source),
                    Name = item1.name,
                    Local = $"{LibrariesPath.BaseDir}/{item1.downloads.artifact.path}",
                    SHA1 = item1.downloads.artifact.sha1
                });
            }
        }
        //旧forge
        else
        {

            ForgeInstallObj1 obj;
            byte[] array1 = stream2.ToArray();
            ForgeLaunchObj info;
            try
            {
                var data = Encoding.UTF8.GetString(array1);
                obj = JsonConvert.DeserializeObject<ForgeInstallObj1>(data)!;
                info = new()
                {
                    id = obj.versionInfo.id,
                    time = obj.versionInfo.time,
                    releaseTime = obj.versionInfo.releaseTime,
                    type = obj.versionInfo.type,
                    mainClass = obj.versionInfo.mainClass,
                    inheritsFrom = obj.versionInfo.inheritsFrom,
                    minecraftArguments = obj.versionInfo.minecraftArguments,
                    libraries = new()
                };
                foreach (var item in obj.versionInfo.libraries)
                {
                    var item1 = ForgeHelper.MakeLibObj(item);
                    if (item1 != null)
                    {
                        info.libraries.Add(item1);
                    }
                    else if (!string.IsNullOrWhiteSpace(item.url))
                    {
                        var path = PathC.ToName(item.name);
                        info.libraries.Add(new()
                        {
                            name = item.name,
                            downloads = new()
                            {
                                artifact = new()
                                {
                                    url = item.url + path.Path,
                                    path = path.Path
                                }
                            }
                        });
                    }
                }

                File.WriteAllText($"{VersionPath.ForgeDir}/{name}.json", JsonConvert.SerializeObject(info));

                list.AddRange(ForgeHelper.MakeForgeLibs(info, mc, version));
            }
            catch (Exception e)
            {
                Logs.Error(LanguageHelper.GetName("Core.Http.Error4"), e);
                return (GetDownloadState.GetInfo, null);
            }
        }

        return (GetDownloadState.End, list);
    }

    /// <summary>
    /// 获取Fabric下载项目
    /// </summary>
    /// <param name="obj">游戏实例</param>
    public static Task<(GetDownloadState State, List<DownloadItem>? List)> DownloadFabric(GameSettingObj obj)
    {
        return DownloadFabric(obj.Version, obj.LoaderVersion);
    }

    /// <summary>
    /// 获取Fabric下载项目
    /// </summary>
    /// <param name="mc">游戏版本</param>
    /// <param name="version">fabric版本</param>
    public static async Task<(GetDownloadState State, List<DownloadItem>? List)> DownloadFabric(string mc, string? version = null)
    {
        var list = new List<DownloadItem>();
        var meta = await FabricHelper.GetMeta(BaseClient.Source);
        if (meta == null)
        {
            return (GetDownloadState.Init, null);
        }

        FabricMetaObj.Loader? fabric;

        if (version != null)
        {
            fabric = meta.loader.Where(a => a.version == version).FirstOrDefault();
        }
        else
        {
            fabric = meta.loader.Where(a => a.stable == true).FirstOrDefault();
        }
        if (fabric == null)
        {
            return (GetDownloadState.GetInfo, null);
        }

        version = fabric.version;

        FabricLoaderObj? meta1 = await FabricHelper.GetLoader(mc, version, BaseClient.Source);
        if (meta1 == null)
        {
            return (GetDownloadState.GetInfo, null);
        }

        File.WriteAllText($"{VersionPath.FabricDir}/{meta1.id}.json",
            JsonConvert.SerializeObject(meta1));

        foreach (var item in meta1.libraries)
        {
            var name = PathC.ToName(item.name);
            list.Add(new()
            {
                Url = UrlHelper.DownloadQuilt(item.url + name.Path, BaseClient.Source),
                Name = name.Name,
                Local = $"{LibrariesPath.BaseDir}/{name.Path}"
            });

        }

        return (GetDownloadState.End, list);
    }

    /// <summary>
    /// 获取Quilt下载项目
    /// </summary>
    /// <param name="obj">游戏实例</param>
    public static Task<(GetDownloadState State, List<DownloadItem>? List)> DownloadQuilt(GameSettingObj obj)
    {
        return DownloadQuilt(obj.Version, obj.LoaderVersion);
    }

    /// <summary>
    /// 获取Quilt下载项目
    /// </summary>
    /// <param name="mc">游戏版本</param>
    /// <param name="version">quilt版本</param>
    public static async Task<(GetDownloadState State, List<DownloadItem>? List)> DownloadQuilt(string mc, string? version = null)
    {
        var list = new List<DownloadItem>();
        var meta = await QuiltHelper.GetMeta(BaseClient.Source);
        if (meta == null)
        {
            return (GetDownloadState.Init, null);
        }

        QuiltMetaObj.Loader? quilt;

        if (version != null)
        {
            quilt = meta.loader.Where(a => a.version == version).FirstOrDefault();
        }
        else
        {
            quilt = meta.loader.FirstOrDefault();
        }
        if (quilt == null)
        {
            return (GetDownloadState.GetInfo, null);
        }

        version = quilt.version;

        QuiltLoaderObj? meta1 = await QuiltHelper.GetLoader(mc, version, BaseClient.Source);
        if (meta1 == null)
        {
            return (GetDownloadState.GetInfo, null);
        }

        File.WriteAllText($"{VersionPath.QuiltDir}/{meta1.id}.json",
            JsonConvert.SerializeObject(meta1));

        foreach (var item in meta1.libraries)
        {
            var name = PathC.ToName(item.name);
            list.Add(new()
            {
                Url = UrlHelper.DownloadQuilt(item.url + name.Path, BaseClient.Source),
                Name = name.Name,
                Local = $"{LibrariesPath.BaseDir}/{name.Path}"
            });

        }

        return (GetDownloadState.End, list);
    }
}
