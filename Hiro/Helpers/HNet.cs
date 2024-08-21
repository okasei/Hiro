using Hiro.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using static Hiro.Helpers.HClass;

namespace Hiro.Helpers
{
    internal class HNet
    {
        internal static void BingWp(string? path, List<string> parameter)
        {
            if (!File.Exists(parameter[0]))
            {
                HttpRequestMessage request = new(HttpMethod.Get, "https://api.rexio.cn/v1/rex.php?r=wallpaper");
                request.Headers.Add("UserAgent", Hiro_Resources.AppUserAgent);
                request.Content = new StringContent("");
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                if (App.hc == null)
                    return;
                try
                {
                    HttpResponseMessage response = App.hc.Send(request);

                    if (response.Content != null)
                    {
                        Stream stream = response.Content.ReadAsStream();
                        System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                        HFile.CreateFolder(parameter[0]);
                        image.Save(parameter[0]);
                    }
                }
                catch (Exception ex)
                {
                    Hiro_Utils.RunExe($"alarm({HText.Get_Translate("error")},{ex})");
                    HLogger.LogError(ex, $"Hiro.Exception.Wallpaper.HttpClient{Environment.NewLine}Path: {path}");
                }
                if (!File.Exists(parameter[0]))
                    App.Notify(new Hiro_Notice(HText.Get_Translate("unknown"), 2, HText.Get_Translate("wallpaper")));
                else
                    App.Notify(new Hiro_Notice(HText.Get_Translate("wpsaved"), 2, HText.Get_Translate("wallpaper")));
            }
            else
                App.Notify(new Hiro_Notice(HText.Get_Translate("wpexist"), 2, HText.Get_Translate("wallpaper")));
        }

        internal static void Save(string? source, List<string> parameter)
        {
            string result = "";
            HFile.CreateFolder(parameter[1]);
            result = GetWebContent(parameter[0], true, parameter[1]);
            if (result.ToLower().Equals("error"))
                App.Notify(new Hiro_Notice(HText.Get_Translate("error"), 2, source));
            else
                App.Notify(new Hiro_Notice(HText.Get_Translate("success"), 2, source));
        }

        #region 获取网络内容
        internal static string GetWebContent(string url, bool save = false, string? savepath = null)
        {
            HttpRequestMessage request = new(HttpMethod.Get, url);
            request.Headers.Add("UserAgent", Hiro_Resources.AppUserAgent);
            request.Content = new StringContent("");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            request.Options.TryAdd("AllowAutoRedirect", true);
            request.Options.TryAdd("KeppAlive", true);
            request.Options.TryAdd("ProtocolVersion", HttpVersion.Version11);
            //这里设置协议
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;// SecurityProtocolType.Tls1.2; 
            ServicePointManager.CheckCertificateRevocationList = true;
            ServicePointManager.DefaultConnectionLimit = 100;
            ServicePointManager.Expect100Continue = false;
            if (App.hc == null)
                throw new Exception(HText.Get_Translate("webnotinitial"));
            try
            {
                HttpResponseMessage response = App.hc.Send(request);
                if (response.Content != null)
                {
                    using (Stream stream = response.Content.ReadAsStream())
                    {
                        string result = string.Empty;
                        if (save == true && savepath != null)
                        {
                            try
                            {
                                using (var fileStream = File.Create(savepath))
                                {
                                    stream.CopyTo(fileStream);
                                }
                                return "saved";
                            }
                            catch (Exception ex)
                            {
                                HLogger.LogError(ex, $"Hiro.Exception.Web.Get");
                                throw new Exception(ex.Message);
                            }
                        }
                        else
                        {
                            StreamReader sr = new(stream);
                            result = sr.ReadToEnd();
                            return result;
                        }
                    }
                }
                else
                {
                    HLogger.LogError(new ArgumentNullException(), $"Hiro.Exception.Web.Respose");
                    return HText.Get_Translate("error");
                }
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, $"Hiro.Exception.Web.HttpClient");
                return HText.Get_Translate("error");
            }
        }
        #endregion
    }
}
