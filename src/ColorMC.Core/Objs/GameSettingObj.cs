﻿using ColorMC.Core.Objs.CurseForge;
using Newtonsoft.Json;

namespace ColorMC.Core.Objs;

/// <summary>
/// 加载器类型
/// </summary>
public enum Loaders
{
    /// <summary>
    /// 无Mod加载器
    /// </summary>
    Normal,
    /// <summary>
    /// Forge加载器
    /// </summary>
    Forge,
    /// <summary>
    /// Fabric加载器
    /// </summary>
    Fabric,
    /// <summary>
    /// Quilt加载器
    /// </summary>
    Quilt
}

/// <summary>
/// 加入服务器设置
/// </summary>
public record ServerObj
{
    /// <summary>
    /// 服务器地址
    /// </summary>
    public string IP { get; set; }
    /// <summary>
    /// 服务器端口
    /// </summary>
    public int Port { get; set; }
}

/// <summary>
/// 端口代理设置
/// </summary>
public record ProxyHostObj
{
    /// <summary>
    /// 服务器地址
    /// </summary>
    public string IP { get; set; }
    /// <summary>
    /// 服务器端口
    /// </summary>
    public ushort Port { get; set; }
    /// <summary>
    /// 用户名
    /// </summary>
    public string User { get; set; }
    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }
}

/// <summary>
/// 游戏实例
/// </summary>
public record GameSettingObj
{
    /// <summary>
    /// 游戏名
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 实例组名
    /// </summary>
    public string GroupName { get; set; }
    /// <summary>
    /// 路径名
    /// </summary>
    public string DirName { get; set; }
    /// <summary>
    /// 游戏版本
    /// </summary>
    public string Version { get; set; }
    /// <summary>
    /// Mod加载器类型
    /// </summary>
    public Loaders Loader { get; set; }
    /// <summary>
    /// Mod加载器版本
    /// </summary>
    public string LoaderVersion { get; set; }
    /// <summary>
    /// Jvm参数
    /// </summary>
    public JvmArgObj JvmArg { get; set; }
    /// <summary>
    /// Jvm名字
    /// </summary>
    public string JvmName { get; set; }
    /// <summary>
    /// Jvm路径
    /// </summary>
    public string JvmLocal { get; set; }
    /// <summary>
    /// 窗口设置
    /// </summary>
    public WindowSettingObj Window { get; set; }
    /// <summary>
    /// 加入服务器设置
    /// </summary>
    public ServerObj StartServer { get; set; }
    /// <summary>
    /// 端口代理设置
    /// </summary>
    public ProxyHostObj ProxyHost { get; set; }
    /// <summary>
    /// 是否为整合包
    /// </summary>
    public bool ModPack { get; set; }

    /// <summary>
    /// CurseForge_Mod信息
    /// </summary>
    [JsonIgnore]
    public Dictionary<long, CurseForgeModObj1> CurseForgeMods { get; set; }
}
