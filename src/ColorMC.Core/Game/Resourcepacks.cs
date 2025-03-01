﻿using ColorMC.Core.LaunchPath;
using ColorMC.Core.Objs;
using ColorMC.Core.Objs.Minecraft;
using ColorMC.Core.Utils;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using System.Text;

namespace ColorMC.Core.Game;

public static class Resourcepacks
{
    /// <summary>
    /// 获取材质包列表
    /// </summary>
    /// <param name="game">游戏实例</param>
    /// <returns></returns>
    public static async Task<List<ResourcepackObj>> GetResourcepacks(this GameSettingObj game)
    {
        var list = new List<ResourcepackObj>();
        var dir = game.GetResourcepacksPath();

        DirectoryInfo info = new(dir);
        if (!info.Exists)
            return list;

        await Parallel.ForEachAsync(info.GetFiles(), async (item, cancel) =>
        {
            bool find = false;
            if (item.Extension is not (".zip" or ".disable"))
                return;
            try
            {
                using ZipFile zFile = new(item.FullName);
                var item1 = zFile.GetEntry("pack.mcmeta");
                if (item1 != null)
                {
                    using var stream1 = zFile.GetInputStream(item1);
                    using var stream = new MemoryStream();
                    await stream1.CopyToAsync(stream, cancel);
                    var data = Encoding.UTF8.GetString(stream.ToArray());
                    var obj1 = JsonConvert.DeserializeObject<ResourcepackObj>(data);
                    if (obj1 != null)
                    {
                        //obj1.Disable = item.Extension is ".disable";
                        obj1.Local = Path.GetFullPath(item.FullName);
                        item1 = zFile.GetEntry("pack.png");
                        if (item1 != null)
                        {
                            using var stream2 = zFile.GetInputStream(item1);
                            using var stream3 = new MemoryStream();
                            await stream2.CopyToAsync(stream3, cancel);
                            obj1.Icon = stream3.ToArray();
                        }
                        list.Add(obj1);
                        find = true;
                    }
                }
            }
            catch (Exception e)
            {
                Logs.Error(LanguageHelper.GetName("Core.Game.Error2"), e);
            }

            if (!find)
            {
                list.Add(new()
                {
                    //Disable = item.Extension is ".disable",
                    Local = Path.GetFullPath(item.FullName),
                    Broken = true
                });
            }
        });

        return list;
    }

    /// <summary>
    /// 导入材质包
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    public static async Task<bool> ImportResourcepack(this GameSettingObj obj, string file)
    {
        var path = obj.GetResourcepacksPath();
        var name = Path.GetFileName(file);
        var local = Path.GetFullPath(path + "/" + name);
        if (File.Exists(local))
            return false;

        bool ok = true;
        await Task.Run(() =>
        {
            try
            {
                File.Copy(file, local);
            }
            catch (Exception e)
            {
                Logs.Error(LanguageHelper.GetName("Core.Game.Error3"), e);
                ok = false;
            }
        });
        return ok;
    }


    //public static void Disable(this ResourcepackObj pack)
    //{
    //    if (pack.Disable)
    //        return;

    //    var file = new FileInfo(pack.Local);
    //    pack.Disable = true;
    //    pack.Local = Path.GetFullPath($"{file.DirectoryName}/{file.Name
    //        .Replace(".zip", ".disable")}");
    //    File.Move(file.FullName, pack.Local);
    //}

    //public static void Enable(this ResourcepackObj pack)
    //{
    //    if (!pack.Disable)
    //        return;

    //    var file = new FileInfo(pack.Local);
    //    pack.Disable = false;
    //    pack.Local = Path.GetFullPath($"{file.DirectoryName}/{file.Name
    //        .Replace(".disable", ".zip")}");
    //    File.Move(file.FullName, pack.Local);
    //}
}
