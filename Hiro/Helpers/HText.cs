using Hiro.Resources;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
        static string ProcessHiroText(string text)
        {
            Stack<int> stack = new Stack<int>();

            StringBuilder result = new StringBuilder(text);

            // 遍历字符串，找到所有的'['和']'
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] == '[')
                {
                    stack.Push(i); // 将索引压入栈中
                }
                else if (result[i] == ']' && stack.Count > 0)
                {
                    int start = stack.Pop(); // 获取匹配的'['索引
                    string content = result.ToString(start + 1, i - start - 1); // 提取方括号内的内容

                    // 调用 CustomProcess 处理内容
                    string processedContent = CustomProcess(content);

                    // 替换处理后的内容
                    result.Remove(start, i - start + 1);
                    result.Insert(start, processedContent);

                    // 调整索引位置，重新开始查找
                    i = start + processedContent.Length - 1;
                }
            }

            return result.ToString();
        }

        // 自定义处理函数，对提取的文本进行特定操作
        static string CustomProcess(string text)
        {
            // 检查是否包含需要处理的命令，如 Upper(...) 等
            if (text.StartsWith("Upper(", StringComparison.CurrentCultureIgnoreCase) && text.EndsWith(")"))
            {
                // 提取命令内部的文本
                string innerText = text.Substring(6, text.Length - 7); // 获取 Upper(...) 内的内容
                return innerText.ToUpper(); // 将内容转换为大写
            }
            else if (text.StartsWith("Lower(", StringComparison.CurrentCultureIgnoreCase) && text.EndsWith(")"))
            {
                // 提取命令内部的文本
                string innerText = text.Substring(6, text.Length - 7); // 获取 Lower(...) 内的内容
                return innerText.ToLower(); // 将内容转换为小写
            }
            else if (text.StartsWith("DateTime(", StringComparison.CurrentCultureIgnoreCase) && text.EndsWith(")"))
            {
                string innerText = text.Substring(9, text.Length - 10); // 获取 DateTime(...) 内的内容

                // 分割参数 t1 和 format
                string[] parts = innerText.Split([','], 2); // 按照第一个逗号进行分割
                if (parts.Length == 2)
                {
                    string t1 = parts[0].Trim(); // 第一个参数 t1
                    string format = parts[1].Trim(); // 第二个参数 format

                    // 尝试解析 t1 为 DateTime 并按照 format 格式化
                    if (!DateTime.TryParse(t1, out DateTime parsedDateTime))
                    {
                        if (t1.Equals("now", StringComparison.CurrentCultureIgnoreCase))
                            parsedDateTime = DateTime.Now;
                        parsedDateTime = new DateTime(2000, 4, 17, 0, 0, 0);
                    }
                    try
                    {
                        return parsedDateTime.ToString(format, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        return parsedDateTime.ToString("g", CultureInfo.InvariantCulture);
                    }
                }
            }
            else if (text.StartsWith("DateTimeSub(", StringComparison.CurrentCultureIgnoreCase) && text.EndsWith(")"))
            {
                string innerText = text.Substring(12, text.Length - 13); // 获取 DateTimeSub(...) 内的内容
                string[] parts = innerText.Split(new[] { ',' }, 2); // 按照第一个逗号进行分割
                if (parts.Length == 2)
                {
                    string t1 = parts[0].Trim(); // 第一个参数 t1
                    string t2 = parts[1].Trim(); // 第二个参数 t2
                    // 尝试解析 t1 和 t2 为 DateTime
                    if (DateTime.TryParse(t1, out DateTime dateTime1) && DateTime.TryParse(t2, out DateTime dateTime2))
                    {
                        TimeSpan difference = dateTime1.Subtract(dateTime2);
                        return difference.ToString(); // 返回时间差的字符串表示
                    }
                    else
                    {
                        return DateTime.Now.ToString();
                    }
                }
            }
            else if (text.Equals("DateTimeNow()", StringComparison.CurrentCultureIgnoreCase))
            {
                return DateTime.Now.ToString();
            }
            // 检查是否包含 TimeSpan(t1, format) 命令
            else if (text.StartsWith("TimeSpan(", StringComparison.CurrentCultureIgnoreCase) && text.EndsWith(")"))
            {
                string innerText = text.Substring(9, text.Length - 10);
                string[] parts = innerText.Split(new[] { ',' }, 2);
                if (parts.Length == 2)
                {
                    string t1 = parts[0].Trim();
                    string format = parts[1].Trim();

                    if (!TimeSpan.TryParse(t1, out TimeSpan parsedTimeSpan))
                    {
                        parsedTimeSpan = TimeSpan.FromSeconds(417);
                    }
                    try
                    {
                        return parsedTimeSpan.ToString(format, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        return parsedTimeSpan.ToString();
                    }
                }
            }
            // 检查是否包含 TimeSpan(t1, format) 命令
            else if (text.StartsWith("Schedule(", StringComparison.CurrentCultureIgnoreCase) && text.EndsWith(")"))
            {
                string innerText = text.Substring(9, text.Length - 10);
                var i = 0;
                int.TryParse(innerText, out i);
                if (App.scheduleitems.Count > i)
                {
                    return App.scheduleitems[i].Time;
                }
                else
                {
                    if (App.scheduleitems.Count > 0)
                    {
                        return App.scheduleitems[0].Time;
                    }
                    else
                    {
                        return DateTime.Now.ToString();
                    }
                }
            }
            else if (text.StartsWith("TimeLeft(", StringComparison.CurrentCultureIgnoreCase) && text.EndsWith(")"))
            {
                string innerText = text.Substring(9, text.Length - 10);
                string[] parts = innerText.Split(new[] { ',' }, 2);
                if (parts.Length == 2)
                {
                    string t1 = parts[0].Trim().ToLower(); // 获取时间单位（year/month/week/day/hour/minute）
                    if (int.TryParse(parts[1].Trim(), out int decimalPlaces))
                    {
                        double percentage = 0;
                        DateTime now = DateTime.Now;

                        switch (t1.ToLower())
                        {
                            case "year":
                                DateTime startOfYear = new DateTime(now.Year, 1, 1);
                                DateTime endOfYear = new DateTime(now.Year + 1, 1, 1);
                                percentage = (now - startOfYear).TotalSeconds / (endOfYear - startOfYear).TotalSeconds * 100;
                                break;
                            case "month":
                                DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
                                DateTime endOfMonth = startOfMonth.AddMonths(1);
                                percentage = (now - startOfMonth).TotalSeconds / (endOfMonth - startOfMonth).TotalSeconds * 100;
                                break;
                            case "week":
                                DateTime startOfWeek = now.AddDays(-(int)now.DayOfWeek); // 默认周从星期天开始
                                DateTime endOfWeek = startOfWeek.AddDays(7);
                                percentage = (now - startOfWeek).TotalSeconds / (endOfWeek - startOfWeek).TotalSeconds * 100;
                                break;
                            case "day":
                                DateTime startOfDay = now.Date; // 当天的开始时间（零点）
                                DateTime endOfDay = startOfDay.AddDays(1);
                                percentage = (now - startOfDay).TotalSeconds / (endOfDay - startOfDay).TotalSeconds * 100;
                                break;
                            case "hour":
                                DateTime startOfHour = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
                                DateTime endOfHour = startOfHour.AddHours(1);
                                percentage = (now - startOfHour).TotalSeconds / (endOfHour - startOfHour).TotalSeconds * 100;
                                break;
                            default:
                                DateTime startOfMinute = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
                                DateTime endOfMinute = startOfMinute.AddMinutes(1);
                                percentage = (now - startOfMinute).TotalSeconds / (endOfMinute - startOfMinute).TotalSeconds * 100;
                                break;
                        }

                        // 返回格式化的结果，保留小数点指定的位数
                        return percentage.ToString($"F{decimalPlaces}", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        return "0";
                    }
                }
            }

            // 其他命令可以继续在这里扩展
            return text;
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
            path = ProcessHiroText(path);
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
