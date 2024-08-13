using Hiro.Resources;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using static Hiro.Helpers.Hiro_Settings;

namespace Hiro.Helpers
{
    internal class Hiro_ID
    {
        internal static string version = "v1";
        #region 个人资料操作
        public static string Login(string account, string pwd, bool token = false, string? saveto = null)
        {
            var url = "https://id.rexio.cn/login.php";
            try
            {
                if (App.hc == null)
                    throw new Exception(Hiro_Text.Get_Translate("webnotinitial"));
                string boundary = DateTime.Now.Ticks.ToString("X");
                string Enter = "\r\n";
                string t = token ? "token" : "pwd";
                byte[] eof = Encoding.UTF8.GetBytes(
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"account\"" + Enter + Enter + "" + account + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"pwd\"" + Enter + Enter + "" + pwd + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"lang\"" + Enter + Enter + "" + App.lang + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"token\"" + Enter + Enter + "" + t + "" + Enter + "--" + boundary + "--"
                    );
                byte[] ndata = new byte[eof.Length];
                eof.CopyTo(ndata, 0);
                HttpRequestMessage request = new(HttpMethod.Post, url);
                request.Headers.Add("UserAgent", Hiro_Resources.AppUserAgent);
                request.Content = new ByteArrayContent(ndata);
                request.Content.Headers.Remove("Content-Type");
                request.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data;boundary=" + boundary);
                HttpResponseMessage response = App.hc.Send(request);
                if (response.Content != null)
                {
                    if (saveto != null)
                    {
                        try
                        {
                            using (Stream stream = response.Content.ReadAsStream())
                            {
                                using (var fileStream = File.Create(saveto))
                                {
                                    stream.CopyTo(fileStream);
                                }
                            }
                            return "success";
                        }
                        catch (Exception ex)
                        {
                            Hiro_Logger.LogError(ex, "Hiro.Exception.Login.Save");
                            throw new Exception(ex.Message);
                        }
                    }
                    else
                    {
                        using (Stream stream = response.Content.ReadAsStream())
                        {
                            string result = string.Empty;
                            using (StreamReader sr = new(stream))
                            {
                                result = sr.ReadToEnd();
                                return result;
                            }
                        }
                    }
                }
                else
                    return "null";
            }
            catch (Exception ex)
            {
                Hiro_Logger.LogError(ex, "Hiro.Exception.Login");
                return "error";
            }
        }

        public static void Logout()
        {
            App.Logined = false;
            App.loginedToken = string.Empty;
            App.username = App.eUserName;
            App.CustomUsernameFlag = 0;
            Write_Ini(App.dConfig, "Config", "Token", string.Empty);
            Write_Ini(App.dConfig, "Config", "AutoLogin", "0");
            Write_Ini(App.dConfig, "Config", "CustomUser", "0");
            Write_Ini(App.dConfig, "Config", "CustomName", string.Empty);
            Write_Ini(App.dConfig, "Config", "CustomSign", string.Empty);
            Write_Ini(App.dConfig, "Config", "UserAvatarStyle", string.Empty);
        }

        public static string UploadProfileImage(string file, string user, string token, string type)
        {
            var url = "https://hiro.rexio.cn/Chat/upload.php";
            try
            {
                if (App.hc == null)
                    throw new Exception(Hiro_Text.Get_Translate("webnotinitial"));
                FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                byte[] bytebuffer;
                bytebuffer = new byte[fs.Length];
                fs.Read(bytebuffer, 0, (int)fs.Length);
                fs.Close();
                string filename = Path.GetFileName(file);
                filename = filename ?? "null";
                string boundary = DateTime.Now.Ticks.ToString("X");
                string Enter = "\r\n";
                byte[] send = Encoding.UTF8.GetBytes(
                    "--" + boundary + Enter + "Content-Type: application/octet-stream" + Enter + "Content-Disposition: form-data; filename=\"" + "" + filename + "" + "\"; name=\"file\"" + Enter + Enter
                    );
                byte[] eof = Encoding.UTF8.GetBytes(
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"user\"" + Enter + Enter + "" + user + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"token\"" + Enter + Enter + "" + token + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"type\"" + Enter + Enter + "" + type + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"version\"" + Enter + Enter + "" + version + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"md5\"" + Enter + Enter + "" + Hiro_Utils.GetMD5(file).Replace("-", "") + "" + Enter + "--" + boundary + "--"
                    );
                byte[] ndata = new byte[send.Length + bytebuffer.Length + eof.Length];
                send.CopyTo(ndata, 0);
                bytebuffer.CopyTo(ndata, send.Length);
                eof.CopyTo(ndata, send.Length + bytebuffer.Length);
                HttpRequestMessage request = new(HttpMethod.Post, url);
                request.Headers.Add("UserAgent", Hiro_Resources.AppUserAgent);
                request.Content = new ByteArrayContent(ndata);
                request.Content.Headers.Remove("Content-Type");
                request.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data;boundary=" + boundary);
                HttpResponseMessage response = App.hc.Send(request);
                if (response.Content != null)
                {
                    using (Stream stream = response.Content.ReadAsStream())
                    {
                        string result = string.Empty;
                        using (StreamReader sr = new(stream))
                        {
                            result = sr.ReadToEnd();
                            return result;
                        }
                    }
                }
                else
                    return "null";
            }
            catch (Exception ex)
            {
                Hiro_Logger.LogError(ex, "Hiro.Exception.Update.Profile");
                return "error";
            }
        }

        public static string UploadProfileSettings(string user, string token, string name, string signature, string avatar, string iavatar, string back, string method = "update", string? saveto = null)
        {
            var url = "https://hiro.rexio.cn/Chat/update.php";
            try
            {
                if (App.hc == null)
                    throw new Exception(Hiro_Text.Get_Translate("webnotinitial"));
                string boundary = DateTime.Now.Ticks.ToString("X");
                string Enter = "\r\n";
                byte[] send = Encoding.UTF8.GetBytes(
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"user\"" + Enter + Enter + "" + user + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"token\"" + Enter + Enter + "" + token + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"version\"" + Enter + Enter + "" + version + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"name\"" + Enter + Enter + "" + name + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"sign\"" + Enter + Enter + "" + signature + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"avatar\"" + Enter + Enter + "" + avatar + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"iavatar\"" + Enter + Enter + "" + iavatar + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"back\"" + Enter + Enter + "" + back + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"method\"" + Enter + Enter + "" + method + "" + Enter + "--" + boundary + "--"
                    );
                HttpRequestMessage request = new(HttpMethod.Post, url);
                request.Headers.Add("UserAgent", Hiro_Resources.AppUserAgent);
                request.Content = new ByteArrayContent(send);
                request.Content.Headers.Remove("Content-Type");
                request.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data;boundary=" + boundary);
                HttpResponseMessage response = App.hc.Send(request);
                if (response.Content != null)
                {
                    if (method.Equals("check") && saveto != null)
                    {
                        try
                        {
                            using (Stream stream = response.Content.ReadAsStream())
                            {
                                using (var fileStream = File.Create(saveto))
                                {
                                    stream.CopyTo(fileStream);
                                }
                            }
                            return "success";
                        }
                        catch (Exception ex)
                        {
                            Hiro_Logger.LogError(ex, "Hiro.Exception.Update.Profile.Settings.Save");
                            throw new Exception(ex.Message);
                        }
                    }
                    else
                    {
                        using (Stream stream = response.Content.ReadAsStream())
                        {
                            string result = string.Empty;
                            using (StreamReader sr = new(stream))
                            {
                                result = sr.ReadToEnd();
                                return result;
                            }
                        }
                    }
                }
                else
                    return "null";
            }
            catch (Exception ex)
            {
                Hiro_Logger.LogError(ex, "Hiro.Exception.Update.Profile.Settings");
                return "error";
            }
        }

        public static string SyncProfile(string user, string token)
        {
            var url = "https://hiro.rexio.cn/Chat/sync.php";
            try
            {
                if (App.hc == null)
                    throw new Exception(Hiro_Text.Get_Translate("webnotinitial"));
                string boundary = DateTime.Now.Ticks.ToString("X");
                string Enter = "\r\n";
                byte[] send = Encoding.UTF8.GetBytes(
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"user\"" + Enter + Enter + "" + user + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"token\"" + Enter + Enter + "" + token + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"version\"" + Enter + Enter + "" + version + "" + Enter + "--" + boundary + "--"
                    );
                HttpRequestMessage request = new(HttpMethod.Post, url);
                request.Headers.Add("UserAgent", Hiro_Resources.AppUserAgent);
                request.Content = new ByteArrayContent(send);
                request.Content.Headers.Remove("Content-Type");
                request.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data;boundary=" + boundary);
                HttpResponseMessage response = App.hc.Send(request);
                if (response.Content != null)
                {
                    var saveto = Path.GetTempFileName();
                    try
                    {
                        using (Stream stream = response.Content.ReadAsStream())
                        {
                            using (var fileStream = File.Create(saveto))
                            {
                                stream.CopyTo(fileStream);
                            }
                        }
                        Write_Ini(App.dConfig, "Config", "CustomUser", "2");
                        Write_Ini(App.dConfig, "Config", "CustomName", Read_Ini(saveto, "Profile", "Name", string.Empty));
                        Write_Ini(App.dConfig, "Config", "CustomSign", Read_Ini(saveto, "Profile", "Sign", string.Empty));
                        Write_Ini(App.dConfig, "Config", "UserAvatarStyle", Read_Ini(saveto, "Profile", "Avatar", "1"));
                        App.username = Read_Ini(saveto, "Profile", "Name", string.Empty);
                        App.CustomUsernameFlag = 1;
                        var usrAvatar = "<hiapp>\\images\\avatar\\" + user + ".hap";
                        var usrBack = "<hiapp>\\images\\background\\" + user + ".hpp";
                        Write_Ini(App.dConfig, "Config", "UserAvatar", usrAvatar);
                        Write_Ini(App.dConfig, "Config", "UserBackground", usrBack);
                        usrAvatar = Hiro_Text.Path_Prepare(usrAvatar);
                        usrBack = Hiro_Text.Path_Prepare(usrBack);
                        Hiro_File.CreateFolder(usrAvatar);
                        Hiro_File.CreateFolder(usrBack);
                        if (File.Exists(usrAvatar))
                            File.Delete(usrAvatar);
                        if (File.Exists(usrBack))
                            File.Delete(usrBack);
                        Hiro_Net.GetWebContent(Read_Ini(saveto, "Profile", "Iavavtar", "https://hiro.rexio.cn/Chat/Profile/" + user + "/" + user + "." + version + ".hap"), true, usrAvatar);
                        Hiro_Net.GetWebContent(Read_Ini(saveto, "Profile", "Back", "https://hiro.rexio.cn/Chat/Profile/" + user + "/" + user + "." + version + ".hpp"), true, usrBack);
                        if (File.Exists(saveto))
                            File.Delete(saveto);
                        return "success";
                    }
                    catch (Exception ex)
                    {
                        Hiro_Logger.LogError(ex, "Hiro.Exception.Update.Profile.Sync.Save");
                        if (File.Exists(saveto))
                            File.Delete(saveto);
                        throw new Exception(ex.Message);
                    }
                }
                else
                    return "null";
            }
            catch (Exception ex)
            {
                Hiro_Logger.LogError(ex, "Hiro.Exception.Update.Profile.Sync");
                return "error";
            }
        }

        public static string SendMsg(string user, string token, string to, string content)
        {
            var url = "https://hiro.rexio.cn/Chat/send.php";
            try
            {
                if (App.hc == null)
                    throw new Exception(Hiro_Text.Get_Translate("webnotinitial"));
                string boundary = DateTime.Now.Ticks.ToString("X");
                string Enter = "\r\n";
                byte[] eof = Encoding.UTF8.GetBytes(
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"user\"" + Enter + Enter + "" + user + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"token\"" + Enter + Enter + "" + token + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"to\"" + Enter + Enter + "" + to + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"content\"" + Enter + Enter + "" + content + "" + Enter + "--" + boundary + "--"
                    );
                byte[] ndata = new byte[eof.Length];
                eof.CopyTo(ndata, 0);
                HttpRequestMessage request = new(HttpMethod.Post, url);
                request.Headers.Add("UserAgent", Hiro_Resources.AppUserAgent);
                request.Content = new ByteArrayContent(ndata);
                request.Content.Headers.Remove("Content-Type");
                request.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data;boundary=" + boundary);
                HttpResponseMessage response = App.hc.Send(request);
                if (response.Content != null)
                {
                    using (Stream stream = response.Content.ReadAsStream())
                    {
                        string result = string.Empty;
                        using (StreamReader sr = new(stream))
                        {
                            result = sr.ReadToEnd();
                            return result;
                        }
                    }
                }
                else
                    return "null";
            }
            catch (Exception ex)
            {
                Hiro_Logger.LogError(ex, "Hiro.Exception.Chat.Send");
                return "error";
            }
        }


        public static string GetChat(string user, string token, string to, string saveto)
        {
            var url = "https://hiro.rexio.cn/Chat/log.php";
            try
            {
                if (App.hc == null)
                    throw new Exception(Hiro_Text.Get_Translate("webnotinitial"));
                string boundary = DateTime.Now.Ticks.ToString("X");
                string Enter = "\r\n";
                byte[] eof = Encoding.UTF8.GetBytes(
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"user\"" + Enter + Enter + "" + user + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"token\"" + Enter + Enter + "" + token + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"to\"" + Enter + Enter + "" + to + "" + Enter + "--" + boundary + "--"
                    );
                byte[] ndata = new byte[eof.Length];
                eof.CopyTo(ndata, 0);
                HttpRequestMessage request = new(HttpMethod.Post, url);
                request.Headers.Add("UserAgent", Hiro_Resources.AppUserAgent);
                request.Content = new ByteArrayContent(ndata);
                request.Content.Headers.Remove("Content-Type");
                request.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data;boundary=" + boundary);
                HttpResponseMessage response = App.hc.Send(request);
                if (response.Content != null)
                {
                    try
                    {
                        using (Stream stream = response.Content.ReadAsStream())
                        {
                            using (var fileStream = File.Create(saveto))
                            {
                                stream.CopyTo(fileStream);
                            }
                        }
                        return "success";
                    }
                    catch (Exception ex)
                    {
                        Hiro_Logger.LogError(ex, "Hiro.Exception.Chat.Get.Save");
                        throw new Exception(ex.Message);
                    }
                }
                else
                    return "null";
            }
            catch (Exception ex)
            {
                Hiro_Logger.LogError(ex, "Hiro.Exception.Chat.Get");
                return "error";
            }
        }
        #endregion
    }
}
