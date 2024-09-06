using Hiro.Resources;
using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Hiro.Helpers
{
    internal class HText
    {
        internal static bool StartsWith(string text, string start)
        {
            return text.StartsWith(start, StringComparison.CurrentCultureIgnoreCase);
        }

        #region 字符串处理
        public static string Path_Replace(string path, string toReplace, string replaced, bool CaseSensitive = false)
        {
            var resu = (replaced.EndsWith("\\")) ? replaced[0..^1] : replaced;
            if (CaseSensitive)
                resu = path.Replace(toReplace, resu);
            else
                resu = Strings.Replace(path, toReplace, resu, 1, -1, CompareMethod.Text);
            if (resu != null)
                return resu;
            else
                return "";
        }

        private static string Anti_Path_Replace(String path, String replaced, String toReplace, bool CaseSensitive = false)
        {
            var resu = (toReplace.EndsWith("\\")) ? toReplace[..^1] : toReplace;
            if (CaseSensitive)
                resu = path.Replace(resu, replaced);
            else
                resu = Strings.Replace(path, resu, replaced, 1, -1, CompareMethod.Text);
            if (resu != null)
                return resu;
            else
                return string.Empty;
        }

        public static string Hiro_ToString(object? obj)
        {
            if (obj == null)
                return string.Empty;
            else
            {
                var ret = obj.ToString();
                if (ret != null)
                    return ret;
                else
                    return string.Empty;
            }
        }

        public static string Anti_Path_Prepare(string path)
        {
            path = Anti_Path_Replace(path, "<hiapp>", ($"{AppDomain.CurrentDomain.BaseDirectory}\\users\\{App.eUserName}\\app").Replace("\\\\", "\\"));
            path = Anti_Path_Replace(path, "<current>", AppDomain.CurrentDomain.BaseDirectory);
            path = Anti_Path_Replace(path, "<system>", Environment.SystemDirectory);
            path = Anti_Path_Replace(path, "<idesktop>", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            path = Anti_Path_Replace(path, "<ideskdir>", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            path = Anti_Path_Replace(path, "<cdeskdir>", Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
            path = Anti_Path_Replace(path, "<idocument>", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            path = Anti_Path_Replace(path, "<cdocument>", Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));
            path = Anti_Path_Replace(path, "<iappdata>", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            path = Anti_Path_Replace(path, "<cappdata>", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            path = Anti_Path_Replace(path, "<imusic>", Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            path = Anti_Path_Replace(path, "<cmusic>", Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic));
            path = Anti_Path_Replace(path, "<ipicture>", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            path = Anti_Path_Replace(path, "<cpicture>", Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures));
            path = Anti_Path_Replace(path, "<istart>", Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
            path = Anti_Path_Replace(path, "<cstart>", Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu));
            path = Anti_Path_Replace(path, "<istartup>", Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            path = Anti_Path_Replace(path, "<cstartup>", Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup));
            path = Anti_Path_Replace(path, "<ivideo>", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
            path = Anti_Path_Replace(path, "<cvideo>", Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos));
            path = Anti_Path_Replace(path, "<iprogx86>", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
            path = Anti_Path_Replace(path, "<cprogx86>", Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86));
            path = Anti_Path_Replace(path, "<iprog>", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            path = Anti_Path_Replace(path, "<cprog>", Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));
            path = Anti_Path_Replace(path, "<win>", Environment.GetFolderPath(Environment.SpecialFolder.Windows));
            path = Anti_Path_Replace(path, "<recent>", Environment.GetFolderPath(Environment.SpecialFolder.Recent));
            path = Anti_Path_Replace(path, "<profile>", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            path = Anti_Path_Replace(path, "<sendto>", Environment.GetFolderPath(Environment.SpecialFolder.SendTo));
            HWin.TryCatch("Hiro.Exception.AntiPathPrepare", () =>
            {
                path = Anti_Path_Replace(path, "<systemx86>", Microsoft.WindowsAPICodePack.Shell.KnownFolders.SystemX86?.Path ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            });
            HWin.TryCatch("Hiro.Exception.AntiPathPrepare", () =>
            {
                path = Anti_Path_Replace(path, "<idownload>", Microsoft.WindowsAPICodePack.Shell.KnownFolders.Downloads?.Path ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            });
            HWin.TryCatch("Hiro.Exception.PathPrepare", () =>
            {
                path = Anti_Path_Replace(path, "<cdownload>", Microsoft.WindowsAPICodePack.Shell.KnownFolders.PublicDownloads?.Path ?? Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));
            });
            return path;
        }

        public static string Path_Prepare(string path)
        {
            path = Path_Replace(path, "<hiapp>", ($"{AppDomain.CurrentDomain.BaseDirectory}\\users\\{App.eUserName}\\app").Replace("\\\\", "\\"));
            path = Path_Replace(path, "<capp>", ($"{AppDomain.CurrentDomain.BaseDirectory}\\users\\default\\app").Replace("\\\\", "\\"));
            path = Path_Replace(path, "<current>", AppDomain.CurrentDomain.BaseDirectory);
            path = Path_Replace(path, "<system>", Environment.SystemDirectory);
            path = Path_Replace(path, "<idesktop>", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            path = Path_Replace(path, "<ideskdir>", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            path = Path_Replace(path, "<cdeskdir>", Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
            path = Path_Replace(path, "<idocument>", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            path = Path_Replace(path, "<cdocument>", Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));
            path = Path_Replace(path, "<iappdata>", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            path = Path_Replace(path, "<cappdata>", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            path = Path_Replace(path, "<imusic>", Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            path = Path_Replace(path, "<cmusic>", Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic));
            path = Path_Replace(path, "<ipicture>", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            path = Path_Replace(path, "<cpicture>", Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures));
            path = Path_Replace(path, "<istart>", Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
            path = Path_Replace(path, "<cstart>", Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu));
            path = Path_Replace(path, "<istartup>", Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            path = Path_Replace(path, "<cstartup>", Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup));
            path = Path_Replace(path, "<ivideo>", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
            path = Path_Replace(path, "<cvideo>", Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos));
            path = Path_Replace(path, "<iprogx86>", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
            path = Path_Replace(path, "<cprogx86>", Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86));
            path = Path_Replace(path, "<iprog>", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            path = Path_Replace(path, "<cprog>", Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));
            path = Path_Replace(path, "<win>", Environment.GetFolderPath(Environment.SpecialFolder.Windows));
            path = Path_Replace(path, "<recent>", Environment.GetFolderPath(Environment.SpecialFolder.Recent));
            path = Path_Replace(path, "<profile>", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            path = Path_Replace(path, "<sendto>", Environment.GetFolderPath(Environment.SpecialFolder.SendTo));
            path = Path_Replace(path, "<hiuser>", App.eUserName);
            path = Path_Replace(path, "<nop>", "");
            HWin.TryCatch("Hiro.Exception.PathPrepare", () =>
            {
                path = Path_Replace(path, "<systemx86>", Microsoft.WindowsAPICodePack.Shell.KnownFolders.SystemX86?.Path ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            });
            HWin.TryCatch("Hiro.Exception.PathPrepare", () =>
            {
                path = Path_Replace(path, "<idownload>", Microsoft.WindowsAPICodePack.Shell.KnownFolders.Downloads?.Path ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            });
            HWin.TryCatch("Hiro.Exception.PathPrepare", () =>
            {
                path = Path_Replace(path, "<idownload>", Microsoft.WindowsAPICodePack.Shell.KnownFolders.Downloads?.Path ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            });
            return path;
        }

        public static string Path_Prepare_EX(string path)
        {
            path = Path_Prepare(path);
            path = Path_Replace(path, "<yyyyy>", DateTime.Now.ToString("yyyyy"));
            path = Path_Replace(path, "<yyyy>", DateTime.Now.ToString("yyyy"));
            path = Path_Replace(path, "<yyy>", DateTime.Now.ToString("yyy"));
            path = Path_Replace(path, "<yy>", DateTime.Now.ToString("yy"));
            path = Path_Replace(path, "<MMMM>", DateTime.Now.ToString("MMMM"));
            path = Path_Replace(path, "<MMM>", DateTime.Now.ToString("MMM"));
            path = Path_Replace(path, "<MM>", DateTime.Now.ToString("MM"), true);
            path = Path_Replace(path, "<M>", DateTime.Now.ToString("M"), true);
            path = Path_Replace(path, "<dddd>", DateTime.Now.ToString("dddd"));
            path = Path_Replace(path, "<ddd>", DateTime.Now.ToString("ddd"));
            path = Path_Replace(path, "<dd>", DateTime.Now.ToString("dd"));
            path = Path_Replace(path, "<d>", DateTime.Now.ToString("d"));
            path = Path_Replace(path, "<HH>", DateTime.Now.ToString("HH"), true);
            path = Path_Replace(path, "<hh>", DateTime.Now.ToString("hh"), true);
            path = Path_Replace(path, "<mm>", DateTime.Now.ToString("mm"), true);
            path = Path_Replace(path, "<m>", DateTime.Now.ToString("m"), true);
            path = Path_Replace(path, "<ss>", DateTime.Now.ToString("ss"));
            path = Path_Replace(path, "<s>", DateTime.Now.ToString("s"));
            path = Path_Replace(path, "<version>", Hiro_Resources.ApplicationVersion);
            path = Path_Replace(path, "<lang>", App.lang);
            path = Path_Replace(path, "<date>", DateTime.Now.ToString("yyyyMMdd"));
            path = Path_Replace(path, "<time>", DateTime.Now.ToString("HHmmss"));
            path = Path_Replace(path, "<now>", DateTime.Now.ToString("yyMMddHHmmss"));
            path = Path_Replace(path, "<me>", App.username);
            path = Path_Replace(path, "<hiro>", App.appTitle);
            path = Path_Replace(path, "<product>", Get_Translate("dlproduct"));
            HWin.TryCatch("Hiro.Exception.PathPrepareX", () =>
            {
                path = Path_Replace(path, "<volume>", HWin.GetSystemVolume().ToString());
            });
            HWin.TryCatch("Hiro.Exception.PathPrepareX", () =>
            {
                path = Path_Replace(path, "<battery>", ((int)HWin.GetBatteryPercentage()).ToString());
            });
            HWin.TryCatch("Hiro.Exception.PathPrepareX", () =>
            {
                path = Path_Replace(path, "<memory>", ((int)HWin.GetMemoryUsagePercentage()).ToString());
            });
            var pi = path;
            if (pi.ToLower().EndsWith("<any>"))
            {
                pi = pi[..^5];
                if (Directory.Exists(pi))
                {
                    DirectoryInfo directory = new DirectoryInfo(pi);
                    var files = directory.GetFiles("*", SearchOption.TopDirectoryOnly);
                    var ImgList = files.Select(s => s.FullName).ToList();
                    if (ImgList.Count > 1)
                        path = ImgList[new Random().Next(0, ImgList.Count - 1)];
                }
            }
            if (pi.ToLower().EndsWith("<xany>"))
            {
                pi = pi[..^6];
                if (Directory.Exists(pi))
                {
                    DirectoryInfo directory = new DirectoryInfo(pi);
                    var files = directory.GetFiles("*", SearchOption.AllDirectories);
                    var ImgList = files.Select(s => s.FullName).ToList();
                    if (ImgList.Count > 1)
                        path = ImgList[new Random().Next(0, ImgList.Count - 1)];
                }
            }
            return path;
        }

        public static string Path_PPX(string path)
        {
            return Path_Prepare(Path_Prepare_EX(path));
        }
        #endregion

        #region 翻译文件
        public static string Get_Translate(string val)
        {
            return HSet.Read_Ini($"{App.currentDir}\\system\\lang\\{App.lang}.hlp", "translate", val, "<???>").Replace("\\n", Environment.NewLine).Replace("%b", " ");
        }
        #endregion

        public static string DeleteUnVisibleChar(string sourceString)
        {
            StringBuilder sBuilder = new(131);
            for (int i = 0; i < sourceString.Length; i++)
            {
                int Unicode = sourceString[i];
                if (Unicode >= 16)
                {
                    sBuilder.Append(sourceString[i]);
                }
            }
            return sBuilder.ToString();
        }

        internal static bool IsOnlyBlank(string? str)
        {
            if (str == null)
                return true;
            return str.Trim().Length == 0;
        }
    }
}
