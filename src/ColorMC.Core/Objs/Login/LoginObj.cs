﻿using ColorMC.Core.Game.Auth;

namespace ColorMC.Core.Objs.Login;

/// <summary>
/// 保存的账户
/// </summary>
public record LoginObj
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// UUID
    /// </summary>
    public string UUID { get; set; }
    /// <summary>
    /// 登录密匙
    /// </summary>
    public string AccessToken { get; set; }
    /// <summary>
    /// 客户端标识
    /// </summary>
    public string ClientToken { get; set; }
    /// <summary>
    /// 账户类型
    /// </summary>
    public AuthType AuthType { get; set; }
    /// <summary>
    /// 数据1
    /// </summary>
    public string Text1 { get; set; }
    /// <summary>
    /// 数据2
    /// </summary>
    public string Text2 { get; set; }
}
