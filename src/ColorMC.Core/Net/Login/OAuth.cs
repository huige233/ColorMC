using ColorMC.Core.Game.Auth;
using ColorMC.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace ColorMC.Core.Net.Login;

public record OAuthObj
{
    public string user_code { get; set; }
    public string device_code { get; set; }
    public string verification_uri { get; set; }
    public int expires_in { get; set; }
    public int interval { get; set; }
    public string message { get; set; }
}

public record OAuth1Obj
{
    public string token_type { get; set; }
    public string scope { get; set; }
    public string access_token { get; set; }
    public int expires_in { get; set; }
    public int ext_expires_in { get; set; }
    public string id_token { get; set; }
    public string refresh_token { get; set; }
}

internal static class OAuthAPI
{
    private const string OAuthCode = "https://login.microsoftonline.com/consumers/oauth2/v2.0/devicecode";
    private const string OAuthToken = "https://login.microsoftonline.com/consumers/oauth2/v2.0/token";
    private const string XboxLive = "https://user.auth.xboxlive.com/user/authenticate";
    private const string XSTS = "https://xsts.auth.xboxlive.com/xsts/authorize";
    private const string XBoxProfile = "https://api.minecraftservices.com/authentication/login_with_xbox";

    private readonly static Dictionary<string, string> Arg1 = new()
    {
        { "client_id", "aa0dd576-d717-4950-b257-a478d2c20968"},
        { "scope", "XboxLive.signin offline_access"}
    };

    private readonly static Dictionary<string, string> Arg2 = new()
    {
        { "client_id", "aa0dd576-d717-4950-b257-a478d2c20968"},
        { "grant_type", "urn:ietf:params:oauth:grant-type:device_code"},
        { "code", "" }
    };

    private readonly static Dictionary<string, string> Arg3 = new()
    {
        { "client_id", "aa0dd576-d717-4950-b257-a478d2c20968"},
        { "grant_type", "refresh_token"},
        { "refresh_token", "" }
    };

    private static string code;
    private static string url;
    private static string device_code;
    private static int expires_in;
    private static bool cancel;

    private static async Task<string> PostString(string url, Dictionary<string, string> arg)
    {
        FormUrlEncodedContent content = new(arg);
        var message = await BaseClient.LoginClient.PostAsync(url, content);

        return await message.Content.ReadAsStringAsync();
    }

    private static async Task<JObject> PostObj(string url, object arg)
    {
        var data1 = JsonConvert.SerializeObject(arg);
        StringContent content = new(data1, MediaTypeHeaderValue.Parse("application/json"));
        var message = await BaseClient.LoginClient.PostAsync(url, content);
        var data = await message.Content.ReadAsStringAsync();
        return JObject.Parse(data);
    }

    private static async Task<JObject> PostObj(string url, Dictionary<string, string> arg)
    {
        FormUrlEncodedContent content = new(arg);
        var message = await BaseClient.LoginClient.PostAsync(url, content);
        var data = await message.Content.ReadAsStringAsync();
        return JObject.Parse(data);
    }

    /// <summary>
    /// 获取登录码
    /// </summary>
    public static async Task<(LoginState Done, string? Code, string? Url)> GetCode()
    {
        var data = await PostString(OAuthCode, Arg1);
        if (data.Contains("error"))
        {
            Logs.Error(data);
            return (LoginState.Error,
                LanguageHelper.GetName("Core.Login.Error21"), null);
        }
        var obj1 = JObject.Parse(data);
        var obj2 = obj1.ToObject<OAuthObj>();
        if (obj2 == null)
        {
            return (LoginState.JsonError,
                LanguageHelper.GetName("Core.Login.Error22"), null);
        }
        code = obj2.user_code;
        url = obj2.verification_uri;
        device_code = obj2.device_code;
        expires_in = obj2.expires_in;

        return (LoginState.Done, code, url);
    }

    /// <summary>
    /// 获取token
    /// </summary>
    public static async Task<(LoginState Done, OAuth1Obj? Obj)> RunGetCode()
    {
        Arg2["code"] = device_code;
        long startTime = DateTime.Now.Ticks;
        int delay = 2;
        cancel = false;
        do
        {
            await Task.Delay(delay * 1000);
            if (cancel)
            {
                return (LoginState.Error, null);
            }
            long estimatedTime = DateTime.Now.Ticks - startTime;
            long sec = estimatedTime / 10000000;
            if (sec > expires_in)
            {
                return (LoginState.TimeOut, null);
            }
            var data = await PostString(OAuthToken, Arg2);
            var obj3 = JObject.Parse(data);
            if (obj3.ContainsKey("error"))
            {
                string? error = obj3["error"]?.ToString();
                if (error == "authorization_pending")
                {
                    continue;
                }
                else if (error == "slow_down")
                {
                    delay += 5;
                }
                else if (error == "expired_token")
                {
                    return (LoginState.Error, null);
                }
            }
            else
            {
                var obj4 = obj3.ToObject<OAuth1Obj>();
                if (obj4 == null)
                {
                    return (LoginState.JsonError, null);
                }

                return (LoginState.Done, obj4);
            }
        } while (true);
    }

    /// <summary>
    /// 刷新密匙
    /// </summary>
    public static async Task<(LoginState Done, OAuth1Obj? Auth)> RefreshTokenAsync(string token)
    {
        var dir = new Dictionary<string, string>(Arg3);
        dir["refresh_token"] = token;

        var obj1 = await PostObj(OAuthToken, dir);
        if (obj1.ContainsKey("error"))
        {
            return (LoginState.Error, null);
        }
        return (LoginState.Done, obj1.ToObject<OAuth1Obj>());
    }

    /// <summary>
    /// Get Xbox live token & userhash
    /// </summary>
    /// <returns></returns>
    public static async Task<(LoginState Done, string? XNLToken, string? XBLUhs)> GetXBLAsync(string token)
    {
        var json = await PostObj(XboxLive, new
        {
            Properties = new
            {
                AuthMethod = "RPS",
                SiteName = "user.auth.xboxlive.com",
                RpsTicket = $"d={token}"
            },
            RelyingParty = "http://auth.xboxlive.com",
            TokenType = "JWT"
        });
        var xblToken = json.GetValue("Token")?.ToString();
        var list = json["DisplayClaims"]?["xui"] as JArray;
        var xblUhs = (list?.First() as JObject)?.GetValue("uhs")?.ToString();

        if (string.IsNullOrWhiteSpace(xblToken) ||
            string.IsNullOrWhiteSpace(xblUhs))
        {
            return (LoginState.JsonError, null, null);
        }

        return (LoginState.Done, xblToken, xblUhs);
    }

    /// <summary>
    /// Get Xbox security token service token & userhash
    /// </summary>
    /// <returns></returns>
    /// <exception cref="FailedAuthenticationException"></exception>
    public static async Task<(LoginState Done, string? XSTSToken, string? XSTSUhs)> GetXSTSAsync(string token)
    {
        var json = await PostObj(XSTS, new
        {
            Properties = new
            {
                SandboxId = "RETAIL",
                UserTokens = new[] { token }
            },
            RelyingParty = "rp://api.minecraftservices.com/",
            TokenType = "JWT"
        });
        var xstsToken = json.GetValue("Token")?.ToString();
        var list = json["DisplayClaims"]?["xui"] as JArray;
        var xstsUhs = (list?.First() as JObject)?.GetValue("uhs")?.ToString();

        if (string.IsNullOrWhiteSpace(xstsToken) ||
            string.IsNullOrWhiteSpace(xstsUhs))
        {
            return (LoginState.JsonError, null, null);
        }

        return (LoginState.Done, xstsToken, xstsUhs);
    }

    /// <summary>
    /// 获取账户信息
    /// </summary>
    public static async Task<(LoginState Done, string? AccessToken)> GetMinecraftAsync(string token, string token1)
    {
        var json = await PostObj(XBoxProfile, new
        {
            identityToken = $"XBL3.0 x={token};{token1}"
        });
        var accessToken = json.GetValue("access_token")?.ToString();
        var expireTime = json.GetValue("expires_in")?.ToString();

        if (string.IsNullOrWhiteSpace(accessToken) ||
            string.IsNullOrWhiteSpace(expireTime))
        {
            return (LoginState.JsonError, null);
        }

        return (LoginState.Done, accessToken);
    }

    public static void Cancel()
    {
        cancel = true;
    }
}
