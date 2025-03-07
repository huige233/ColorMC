﻿using ColorMC.Core.LaunchPath;
using ColorMC.Core.Net.Apis;
using ColorMC.Core.Net.Downloader;
using ColorMC.Core.Objs;
using ColorMC.Core.Objs.CurseForge;
using ColorMC.Core.Utils;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using System.Text;

namespace ColorMC.Core.Net.Download;

public static class PackDownload
{
    public static int Size { get; private set; }
    public static int Now { get; private set; }
    /// <summary>
    /// 下载整合包
    /// </summary>
    /// <param name="zip">压缩包路径</param>

    public static async Task<(GetDownloadState State, List<DownloadItem>? List,
        List<CurseForgeModObj.Data>? Pack, GameSettingObj? Game)> DownloadCurseForgeModPack(string zip)
    {
        var list = new List<DownloadItem>();
        var list2 = new List<CurseForgeModObj.Data>();

        CoreMain.PackState?.Invoke(CoreRunState.Init);
        using ZipFile zFile = new(zip);
        using MemoryStream stream1 = new();
        bool find = false;
        //获取主信息
        foreach (ZipEntry e in zFile)
        {
            if (e.IsFile && e.Name == "manifest.json")
            {
                using var stream = zFile.GetInputStream(e);
                await stream.CopyToAsync(stream1);
                find = true;
                break;
            }
        }

        if (!find)
        {
            return (GetDownloadState.Init, null, null, null);
        }
        CurseForgePackObj info;
        byte[] array1 = stream1.ToArray();
        try
        {
            var data = Encoding.UTF8.GetString(array1);
            info = JsonConvert.DeserializeObject<CurseForgePackObj>(data)!;
        }
        catch (Exception e)
        {
            Logs.Error(LanguageHelper.GetName("Core.Pack.Error1"), e);
            return (GetDownloadState.Init, null, null, null);
        }
        if (info == null)
            return (GetDownloadState.Init, null, null, null);

        //获取版本数据
        Loaders loaders = Loaders.Normal;
        string loaderversion = "";
        foreach (var item in info.minecraft.modLoaders)
        {
            if (item.id.StartsWith("forge"))
            {
                loaders = Loaders.Forge;
                loaderversion = item.id.Replace("forge-", "");
            }
            else if (item.id.StartsWith("fabric"))
            {
                loaders = Loaders.Fabric;
                loaderversion = item.id.Replace("fabric-", "");
            }
        }
        string name = $"{info.name}-{info.version}";

        //创建游戏实例
        var game = await InstancesPath.CreateVersion(new()
        {
            Name = name,
            Version = info.minecraft.version,
            ModPack = true,
            Loader = loaders,
            LoaderVersion = loaderversion,
            CurseForgeMods = new()
        });

        if (game == null)
        {
            return (GetDownloadState.GetInfo, null, null, game);
        }

        //解压文件
        foreach (ZipEntry e in zFile)
        {
            if (e.IsFile && e.Name.StartsWith(info.overrides + "/"))
            {
                using var stream = zFile.GetInputStream(e);
                string file = Path.GetFullPath(game.GetGamePath() +
                    e.Name[info.overrides.Length..]);
                FileInfo info2 = new(file);
                info2.Directory?.Create();
                using FileStream stream2 = new(file, FileMode.Create,
                    FileAccess.ReadWrite, FileShare.ReadWrite);
                await stream.CopyToAsync(stream2);
            }
        }

        File.WriteAllBytes(game.GetModJsonFile(), array1);

        CoreMain.PackState?.Invoke(CoreRunState.GetInfo);

        //获取Mod信息
        Size = info.files.Count;
        Now = 0;
        var res = await CurseForge.GetMods(info.files);
        if (res != null)
        {
            var res1 = res.Distinct(CurseDataComparer.Instance);
            foreach (var item in res1)
            {
                item.downloadUrl ??= $"https://edge.forgecdn.net/files/{item.id / 1000}/{item.id % 1000}/{item.fileName}";

                var item11 = new DownloadItem()
                {
                    Url = item.downloadUrl,
                    Name = item.fileName,
                    Local = game.GetGamePath() + "/mods/" + item.fileName,
                    SHA1 = item.hashes.Where(a => a.algo == 1)
                            .Select(a => a.value).FirstOrDefault()
                };

                list.Add(item11);

                game.CurseForgeMods.Add(item.modId, new()
                {
                    Name = item.displayName,
                    File = item.fileName,
                    SHA1 = item11.SHA1!,
                    Id = item.id,
                    ModId = item.modId,
                    Url = item.downloadUrl
                });

                Now++;

                CoreMain.PackUpdate?.Invoke(Size, Now);
            }
            list2.AddRange(res);
        }
        else
        {
            bool done = true;
            await Parallel.ForEachAsync(info.files, async (item, token) =>
            {
                var res = await CurseForge.GetMod(item);
                if (res == null)
                {
                    done = false;
                    return;
                }

                res.data.downloadUrl ??= $"https://edge.forgecdn.net/files/{res.data.id / 1000}/{res.data.id % 1000}/{res.data.fileName}";

                var item11 = new DownloadItem()
                {
                    Url = res.data.downloadUrl,
                    Name = res.data.displayName,
                    Local = InstancesPath.GetGamePath(game) + "/mods/" + res.data.fileName,
                    SHA1 = res.data.hashes.Where(a => a.algo == 1)
                        .Select(a => a.value).FirstOrDefault()
                };

                list.Add(item11);

                game.CurseForgeMods.Add(res.data.modId, new()
                {
                    Name = res.data.displayName,
                    File = res.data.fileName,
                    SHA1 = item11.SHA1!,
                    Id = res.data.id,
                    ModId = res.data.modId,
                    Url = res.data.downloadUrl
                });

                list2.Add(res.data);

                Now++;

                CoreMain.PackUpdate?.Invoke(Size, Now);
            });
            if (!done)
            {
                return (GetDownloadState.GetInfo, list, list2, game);
            }
        }

        game.SaveCurseForgeMod();

        return (GetDownloadState.End, list, list2, game);
    }
}
