using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Media.Animation;
using Windows.Devices.WiFi;
using Windows.Devices.Radios;
using Windows.Security.Credentials;
using Windows.Security.Credentials.UI;
using System.Windows.Media.Imaging;
using Microsoft.VisualBasic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Net;
using System.Security.Cryptography;

namespace hiro
{
    #region 命令项目定义
    public class Cmditem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private int p;
        private int i;
        private string na;
        private string co;
        private string hk;

        public int Page
        {
            get { return p; }
            set
            {
                p = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Page)));
            }
        }
        public int Id
        {
            get { return i; }
            set
            {
                i = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Id)));
            }
        }
        public string Name
        {
            get { return na; }
            set
            {
                na = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
        public string Command
        {
            get { return co; }
            set
            {
                co = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Command)));
            }
        }

        public string HotKey
        {
            get { return hk; }
            set
            {
                hk = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HotKey)));
            }
        }
        public Cmditem()
        {
            Page = -1;
            Id = -1;
            Name = string.Empty;
            Command = string.Empty;
            HotKey = string.Empty;
            p = -1;
            Id = -1;
            na = string.Empty;
            co = string.Empty;
            hk = string.Empty;
        }
        public Cmditem(int a, int b, string c, string d, string e)
        {
            Page = a;
            Id = b;
            Name = c;
            Command = d;
            HotKey = e;
            p = a;
            Id = b;
            na = c;
            co = d;
            hk = e;
        }
    }
    #endregion

    #region 日程项目定义
    public class Scheduleitem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private int i;
        private string na;
        private string ti;
        private string co;
        public double re;
        public string Time
        {
            get { return ti; }
            set
            {
                ti = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Time)));
            }
        }
        public int Id
        {
            get { return i; }
            set
            {
                i = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Id)));
            }
        }
        public string Name
        {
            get { return na; }
            set
            {
                na = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
        public string Command
        {
            get { return co; }
            set
            {
                co = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Command)));
            }
        }

        public Scheduleitem()
        {
            Time = "19000101000000";
            Id = -1;
            Name = string.Empty;
            Command = string.Empty;
            re = -2.0;
            ti = "19000101000000";
            i = -1;
            na = string.Empty;
            co = string.Empty;
        }
        public Scheduleitem(int b, string a, string c, string d, double e)
        {
            Name = a;
            Id = b;
            Time = c;
            Command = d;
            re = e;
            na = a;
            i = b;
            ti = c;
            co = d;
        }
    }
    #endregion

    #region 通知窗口项目定义
    public class Hiro_AlarmWin
    {
        public Hiro_Alarm win;
        public int id;
        public Hiro_AlarmWin(Hiro_Alarm a, int b)
        {
            win = a;
            id = b;
        }
        public Hiro_AlarmWin(int a, Hiro_Alarm b)
        {
            win = b;
            id = a;
        }
    }
    #endregion

    #region 通知项目定义
    public class Hiro_Notice
    {
        public string? title;
        public string msg;
        public int time;
        public Action? act;
        public Hiro_Notice(string ms = "NULL", int ti = 1, string? tit = null, Action? ac = null)
        {
            msg = ms;
            time = ti;
            title = tit;
            act = ac;
        }
    }
    #endregion

    #region 语言项目定义
    public class Language : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private string na;
        private string la;
        public string Name
        {
            get { return na; }
            set
            {
                na = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
        public string Langname
        {
            get { return la; }
            set
            {
                la = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Langname)));
            }
        }

        public Language()
        {
            Name = "null";
            Langname = "null";
            na = "null";
            la = "null";
        }

        public Language(string Name, string LangName)
        {
            this.Name = Name;
            Langname = LangName;
            na = Name;
            la = LangName;
        }
    }
    #endregion

    #region 通信标准定义
    public class HiroApp
    {
        public int state = -1;
        public string? appID = null;
        public string? appPackage = null;
        public string? appName = null;
        public string? msg = null;
        public HiroApp(string? appID = null, string? appPackage = null, string? appName = null, string? msg = null, int state = 0)
        {
            this.appID = appID;
            this.appPackage = appPackage;
            this.appName = appName;
            this.msg = msg;
            this.state = state;
        }
        public bool CheckIntegrity()
        {
            if (appID == null || appPackage == null || appName == null || msg == null)
                return true;
            else
                return false;
        }
        public void Reset()
        {
            appID = null;
            appPackage = null;
            appName = null;
            msg = null;
            state = 0;
        }
        public override string ToString()
        {
            var ret = "ID: ";
            var re = appID ?? "null";
            ret = ret + re + ", Package: ";
            re = appPackage ?? "null";
            ret = ret + re + ", Name: ";
            re = appName ?? "null";
            ret = ret + re + ", Msg: ";
            re = msg ?? "null";
            ret += re;
            return ret;
        }
    }

    #endregion

    #region 自定义Image

    public class HiroUIContainer : RichTextBox
    {

    }

    #endregion

    public partial class Hiro_Utils : Component
    {

        static int keyid = 0;
        internal static string version = "v1";

        #region 自动生成
        public Hiro_Utils()
        {
            InitializeComponent();
        }

        public Hiro_Utils(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
        #endregion

        #region 配置相关

        #region 读Ini文件
        public static string Read_Ini(string iniFilePath, string Section, string Key, string defaultText)
        {
            if (File.Exists(iniFilePath))
            {
                byte[] buffer = new byte[1024];
                int ret = GetPrivateProfileString(Encoding.GetEncoding("utf-8").GetBytes(Section), Encoding.GetEncoding("utf-8").GetBytes(Key), Encoding.GetEncoding("utf-8").GetBytes(defaultText), buffer, 1024, iniFilePath);
                return DeleteUnVisibleChar(Encoding.GetEncoding("utf-8").GetString(buffer, 0, ret)).Trim();
            }
            else
            {
                return defaultText;
            }
        }
        #endregion

        #region 写Ini文件
        public static bool Write_Ini(string iniFilePath, string Section, string Key, string Value)
        {
            try
            {
                if (!File.Exists(iniFilePath))
                    File.Create(iniFilePath).Close();
                long OpStation = WritePrivateProfileString(Encoding.GetEncoding("utf-8").GetBytes(Section), Encoding.GetEncoding("utf-8").GetBytes(Key), Encoding.GetEncoding("utf-8").GetBytes(Value), iniFilePath);
                if (OpStation == 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                LogError(ex, "Hiro.Exception.Config.Update");
                return false;
            }

        }
        #endregion

        #region 写日志相关

        public static void LogError(Exception ex, string Module)
        {
            StringBuilder str = new StringBuilder();
            if (ex.InnerException == null)
            {
                str.Append($"{Environment.NewLine}[ERROR]{Module}{Environment.NewLine}");
                str.Append($"Object: {ex.Source}{Environment.NewLine}");
                str.Append($"Exception: {ex.GetType().Name}{Environment.NewLine}");
                str.Append($"Details: {ex.Message}");
                //str.Append($"StackTrace: {ex.StackTrace}");
            }
            else
            {
                str.Append($"{Environment.NewLine}[ERROR]{Module}.InnerException{Environment.NewLine}");
                str.Append($"Object: {ex.InnerException.Source}{Environment.NewLine}");
                str.Append($"Exception: {ex.InnerException.GetType().Name}{Environment.NewLine}");
                str.Append($"Details: {ex.InnerException.Message}");
                //str.Append($"StackTrace: {ex.InnerException.StackTrace}");
            }
            LogtoFile(str.ToString());
        }

        public static void LogtoFile(string val)
        {
            try
            {
                if (!File.Exists(App.LogFilePath))
                    File.Create(App.LogFilePath).Close();
                FileStream fs = new(App.LogFilePath, FileMode.Open, FileAccess.ReadWrite);
                StreamReader sr = new(fs);
                var str = sr.ReadToEnd();
                StreamWriter sw = new(fs);
                sw.Write(DateTime.Now.ToString("[HH:mm:ss]") + val + Environment.NewLine);
                sw.Flush();
                sw.Close();
                sr.Close();
                sr.Dispose();
                fs.Close();

            }
            catch (Exception ex)
            {
                try
                {
                    LogError(ex, "Hiro.Exception.Log");
                }
                catch
                {

                }
            }
        }
        #endregion

        #region 翻译文件
        public static string Get_Translate(string val)
        {
            return Read_Ini($"{App.CurrentDirectory}\\system\\lang\\{App.lang}.hlp", "translate", val, "<???>").Replace("\\n", Environment.NewLine).Replace("%b", " ");
        }
        #endregion

        #region UI 相关
        public static void Get_Text_Visual_Width(ContentControl sender, double pixelPerDip, out Size size)
        {
            var formattedText = new FormattedText(
                sender.Content.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface(sender.FontFamily, sender.FontStyle, sender.FontWeight, sender.FontStretch),
                sender.FontSize, Brushes.Black, pixelPerDip);
            size.Width = formattedText.Width + sender.Padding.Left + sender.Padding.Right;
            size.Height = formattedText.Height + sender.Padding.Top + sender.Padding.Bottom;
        }
        public static void Set_Bgimage(Control sender, Control win, string? strFileName = null)
        {
            Set_Opacity(sender, win);
            //Bgimage
            strFileName ??= Path_Prepare(Path_Prepare_EX(Read_Ini(App.dconfig, "Config", "BackImage", "")));
            if (Read_Ini(App.dconfig, "Config", "Background", "1").Equals("1") || !File.Exists(strFileName))
            {
                if (!Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
                {
                    Storyboard? sb = new();
                    try
                    {
                        sb = AddColorAnimaton(App.AppAccentColor, 150, sender, "Background.Color", sb);
                        sb.Completed += delegate
                        {
                            sender.Background = new SolidColorBrush(App.AppAccentColor);
                            sb = null;
                        };
                        sb.Begin();
                    }
                    catch (Exception ex)
                    {
                        LogError(ex, "Hiro.Exception.Animation");
                        sender.Background = new SolidColorBrush(App.AppAccentColor);
                    }

                }
                else
                    sender.Background = new SolidColorBrush(App.AppAccentColor);
            }
            else
            {
                BitmapImage? bi = Hiro_Utils.GetBitmapImage(strFileName);
                ImageBrush ib = new()
                {
                    Stretch = Stretch.UniformToFill,
                    ImageSource = bi
                };
                sender.Background = ib;
            }
        }
        public static Brush Set_Bgimage(Brush sender, Control win, string? strFileName = null)
        {
            //Bgimage
            strFileName ??= Path_Prepare(Path_Prepare_EX(Read_Ini(App.dconfig, "Config", "BackImage", "")));
            if (Read_Ini(App.dconfig, "Config", "Background", "1").Equals("1") || !File.Exists(strFileName))
            {
                if (!Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
                {
                    Storyboard? sb = new();
                    try
                    {
                        sb = AddColorAnimaton(App.AppAccentColor, 150, sender, "Color", sb);
                        sb.Completed += delegate
                        {
                            sender = new SolidColorBrush(App.AppAccentColor);
                            sb = null;
                        };
                        sb.Begin();
                    }
                    catch (Exception ex)
                    {
                        LogError(ex, "Hiro.Exception.Animation");
                        sender = new SolidColorBrush(App.AppAccentColor);
                    }

                }
                else
                    sender = new SolidColorBrush(App.AppAccentColor);
            }
            else
            {
                BitmapImage? bi = Hiro_Utils.GetBitmapImage(strFileName);
                ImageBrush ib = new()
                {
                    Stretch = Stretch.UniformToFill,
                    ImageSource = bi
                };
                sender = ib;
            }
            return sender;
        }

        public static void Set_Opacity(FrameworkElement sender, Control? win = null)
        {
            if (!double.TryParse(Read_Ini(App.dconfig, "Config", "OpacityMask", "255"), out double to))
                to = 255;
            Color bg = Colors.White;
            switch (to)
            {
                case > 255:
                    to = 510 - to;
                    bg = Colors.White;
                    break;
                case < 255:
                    bg = Colors.Black;
                    break;
                default:
                    break;
            }
            Color dest = (to >= 0 && to <= 255) ?
                Color.FromArgb(Convert.ToByte(to), 0, 0, 0) : Color.FromArgb(255, 0, 0, 0);
            if (win != null)
                win.Background = new SolidColorBrush(bg);
            sender.OpacityMask = new SolidColorBrush(dest);
        }
        public static void Set_Foreground_Opacity(Border sender, Control? win = null)
        {
            if (!double.TryParse(Read_Ini(App.dconfig, "Config", "OpacityMask", "255"), out double to))
                to = 255;
            Color bg = Colors.White;
            switch (to)
            {
                case > 255:
                    to = 510 - to;
                    bg = Colors.White;
                    break;
                case < 255:
                    bg = Colors.Black;
                    break;
                default:
                    break;
            }
            sender.Background = new SolidColorBrush(bg);
            sender.Opacity = 1 - to / 255;
            if (win != null)
                win.Background = new SolidColorBrush(bg);
            //sender.OpacityMask = new SolidColorBrush(dest);
        }
        public static void Set_Control_Location(Control sender, string val, bool extra = false, string? path = null, bool right = false, bool bottom = false, bool location = true, bool animation = false, double animationTime = 150)
        {
            if (extra == false || path == null || !File.Exists(path))
                path = App.LangFilePath;
            try
            {
                if (sender != null)
                {
                    if (right == true)
                        sender.HorizontalAlignment = HorizontalAlignment.Right;
                    if (bottom == true)
                        sender.VerticalAlignment = VerticalAlignment.Bottom;
                    var result = HiroParse(Read_Ini(path, "location", val, string.Empty).Trim().Replace("%b", " ").Replace("{", "(").Replace("}", ")"));
                    if (!result[0].Equals("-1"))
                        sender.FontFamily = new FontFamily(result[0]);
                    if (!result[1].Equals("-1"))
                        sender.FontSize = double.Parse(result[1]);
                    sender.FontStretch = result[2] switch
                    {
                        "1" => FontStretches.UltraCondensed,
                        "2" => FontStretches.ExtraCondensed,
                        "3" => FontStretches.Condensed,
                        "4" => FontStretches.SemiCondensed,
                        "5" => FontStretches.Medium,
                        "6" => FontStretches.SemiExpanded,
                        "7" => FontStretches.Expanded,
                        "8" => FontStretches.ExtraExpanded,
                        "9" => FontStretches.UltraExpanded,
                        _ => FontStretches.Normal
                    };
                    sender.FontWeight = result[3] switch
                    {
                        "1" => FontWeights.Thin,
                        "2" => FontWeights.UltraLight,
                        "3" => FontWeights.Light,
                        "4" => FontWeights.Medium,
                        "5" => FontWeights.SemiBold,
                        "6" => FontWeights.Bold,
                        "7" => FontWeights.UltraBold,
                        "8" => FontWeights.Black,
                        "9" => FontWeights.UltraBlack,
                        _ => FontWeights.Normal
                    };
                    sender.FontStyle = result[4] switch
                    {
                        "1" => FontStyles.Italic,
                        "2" => FontStyles.Oblique,
                        _ => FontStyles.Normal
                    };
                    if (location)
                    {
                        Size mSize = new();
                        var auto = result[7].ToLower().Equals("auto") || result[7].ToLower().Equals("nan") || result[7].Equals("-2") || result[7].Equals("0");
                        mSize.Width = auto ? double.NaN : (!result[7].Equals("-1")) ? double.Parse(result[7]) : sender.Width;
                        auto = result[8].ToLower().Equals("auto") || result[8].ToLower().Equals("nan") || result[8].Equals("-2") || result[8].Equals("0");
                        mSize.Height = auto ? double.NaN : (!result[8].Equals("-1")) ? double.Parse(result[8]) : sender.Height;
                        Thickness thickness = new()
                        {
                            Left = (!result[5].Equals("-1")) ? right ? 0.0 : double.Parse(result[5]) : sender.Margin.Left,
                            Right = (!result[5].Equals("-1")) ? !right ? sender.Margin.Right : double.Parse(result[5]) : sender.Margin.Right,
                            Top = (!result[6].Equals("-1")) ? bottom ? 0.0 : double.Parse(result[6]) : sender.Margin.Top,
                            Bottom = (!result[6].Equals("-1")) ? !bottom ? sender.Margin.Bottom : double.Parse(result[6]) : sender.Margin.Bottom
                        };
                        if (!animation)
                        {
                            sender.Width = mSize.Width;
                            sender.Height = mSize.Height;
                            sender.Margin = thickness;
                        }
                        else
                        {
                            Storyboard sb = new();
                            AddThicknessAnimaton(thickness, animationTime, sender, "Margin", sb);
                            if (!double.IsNaN(mSize.Height))
                                AddDoubleAnimaton(mSize.Height, animationTime, sender, "Height", sb);
                            if (!double.IsNaN(mSize.Width))
                                AddDoubleAnimaton(mSize.Width, animationTime, sender, "Width", sb);
                            sb.Completed += delegate
                            {
                                sender.Width = mSize.Width;
                                sender.Height = mSize.Height;
                                sender.Margin = thickness;
                            };
                            sb.Begin();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, $"Hiro.Exception.Location{Environment.NewLine}Path: {val}");
            }

        }

        public static void Set_Mac_Location(Control mac, string mval, Control name, bool animation = false, double animationTime = 150)
        {
            try
            {
                if (mac != null && name != null)
                {
                    var result = HiroParse(Read_Ini(App.LangFilePath, "location", mval, string.Empty).Trim().Replace("%b", " ").Replace("{", "(").Replace("}", ")"));
                    if (!result[0].Equals("-1"))
                        mac.FontFamily = new FontFamily(result[0]);
                    if (!result[1].Equals("-1"))
                        mac.FontSize = double.Parse(result[1]);
                    mac.FontStretch = result[2] switch
                    {
                        "1" => FontStretches.UltraCondensed,
                        "2" => FontStretches.ExtraCondensed,
                        "3" => FontStretches.Condensed,
                        "4" => FontStretches.SemiCondensed,
                        "5" => FontStretches.Medium,
                        "6" => FontStretches.SemiExpanded,
                        "7" => FontStretches.Expanded,
                        "8" => FontStretches.ExtraExpanded,
                        "9" => FontStretches.UltraExpanded,
                        _ => FontStretches.Normal
                    };
                    mac.FontWeight = result[3] switch
                    {
                        "1" => FontWeights.Thin,
                        "2" => FontWeights.UltraLight,
                        "3" => FontWeights.Light,
                        "4" => FontWeights.Medium,
                        "5" => FontWeights.SemiBold,
                        "6" => FontWeights.Bold,
                        "7" => FontWeights.UltraBold,
                        "8" => FontWeights.Black,
                        "9" => FontWeights.UltraBlack,
                        _ => FontWeights.Normal
                    };
                    mac.FontStyle = result[4] switch
                    {
                        "1" => FontStyles.Italic,
                        "2" => FontStyles.Oblique,
                        _ => FontStyles.Normal
                    };
                    Thickness thickness = new()
                    {
                        Left = (!result[5].Equals("-1")) ? double.Parse(result[5]) + name.Margin.Left + name.ActualWidth + 5 : name.Margin.Left + name.ActualWidth + 5,
                        Right = 0,
                        Top = (!result[6].Equals("-1")) ? double.Parse(result[6]) : name.Margin.Top,
                        Bottom = 0
                    };
                    if (!animation)
                    {
                        mac.Margin = thickness;
                    }
                    else
                    {
                        Storyboard sb = new();
                        AddThicknessAnimaton(thickness, animationTime, mac, "Margin", sb);
                        sb.Completed += delegate
                        {
                            mac.Margin = thickness;
                        };
                        sb.Begin();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, $"Hiro.Exception.Location{Environment.NewLine}Path: {mval}");
            }

        }

        public static void Set_FrameworkElement_Location(FrameworkElement sender, string val, bool animation = false, double animationTime = 150)
        {
            Size mSize = new();
            Thickness thickness = sender.Margin;
            try
            {
                if (sender != null)
                {
                    var loc = Read_Ini(App.LangFilePath, "location", val, string.Empty);
                    loc = loc.Replace(" ", "").Replace("%b", " ");
                    loc = loc[(loc.IndexOf("{") + 1)..];
                    loc = loc[..loc.LastIndexOf("}")];
                    var left = loc[..loc.IndexOf(",")];
                    loc = loc[(left.Length + 1)..];
                    var top = loc[..loc.IndexOf(",")];
                    loc = loc[(top.Length + 1)..];
                    var width = loc[..loc.IndexOf(",")];
                    loc = loc[(width.Length + 1)..];
                    var height = loc;
                    mSize.Width = width.Equals("-1") ? double.NaN : double.Parse(width);
                    mSize.Height = height.Equals("-1") ? double.NaN : double.Parse(height);
                    if (!left.Equals("-1"))
                        thickness.Left = double.Parse(left);
                    if (!top.Equals("-1"))
                        thickness.Top = double.Parse(top);
                    if (!animation)
                    {
                        sender.Width = mSize.Width;
                        sender.Height = mSize.Height;
                        sender.Margin = thickness;
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Storyboard sb = new();
                            AddThicknessAnimaton(thickness, animationTime, sender, "Margin", sb);
                            if (!double.IsNaN(mSize.Height))
                                AddDoubleAnimaton(mSize.Height, animationTime, sender, "Height", sb);
                            if (!double.IsNaN(mSize.Width))
                                AddDoubleAnimaton(mSize.Width, animationTime, sender, "Width", sb);
                            sb.Completed += delegate
                            {
                                sender.Width = mSize.Width;
                                sender.Height = mSize.Height;
                                sender.Margin = thickness;
                            };
                            sb.Begin();
                        });

                    }

                }
            }
            catch (Exception ex)
            {
                sender.Width = mSize.Width;
                sender.Height = mSize.Height;
                sender.Margin = thickness;
                LogError(ex, $"Hiro.Exception.Location.Grid{Environment.NewLine}Path: {val}");
            }

        }
        #endregion

        #endregion

        #region 字符串处理
        public static String Path_Replace(String path, String toReplace, String replaced, bool CaseSensitive = false)
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

        private static String Anti_Path_Replace(String path, String replaced, String toReplace, bool CaseSensitive = false)
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

        public static String Anti_Path_Prepare(string path)
        {
            path = Anti_Path_Replace(path, "<hiapp>", ($"{AppDomain.CurrentDomain.BaseDirectory}\\users\\{App.EnvironmentUsername}\\app").Replace("\\\\", "\\"));
            path = Anti_Path_Replace(path, "<current>", AppDomain.CurrentDomain.BaseDirectory);
            path = Anti_Path_Replace(path, "<system>", Environment.SystemDirectory);
            path = Anti_Path_Replace(path, "<systemx86>", Microsoft.WindowsAPICodePack.Shell.KnownFolders.SystemX86.Path);
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
            path = Anti_Path_Replace(path, "<idownload>", Microsoft.WindowsAPICodePack.Shell.KnownFolders.Downloads.Path);
            path = Anti_Path_Replace(path, "<cdownload>", Microsoft.WindowsAPICodePack.Shell.KnownFolders.PublicDownloads.Path);
            path = Anti_Path_Replace(path, "<win>", Environment.GetFolderPath(Environment.SpecialFolder.Windows));
            path = Anti_Path_Replace(path, "<recent>", Environment.GetFolderPath(Environment.SpecialFolder.Recent));
            path = Anti_Path_Replace(path, "<profile>", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            path = Anti_Path_Replace(path, "<sendto>", Environment.GetFolderPath(Environment.SpecialFolder.SendTo));
            return path;
        }

        public static String Path_Prepare(string path)
        {
            path = Path_Replace(path, "<hiapp>", ($"{AppDomain.CurrentDomain.BaseDirectory}\\users\\{App.EnvironmentUsername}\\app").Replace("\\\\", "\\"));
            path = Path_Replace(path, "<capp>", ($"{AppDomain.CurrentDomain.BaseDirectory}\\users\\default\\app").Replace("\\\\", "\\"));
            path = Path_Replace(path, "<current>", AppDomain.CurrentDomain.BaseDirectory);
            path = Path_Replace(path, "<system>", Environment.SystemDirectory);
            path = Path_Replace(path, "<systemx86>", Microsoft.WindowsAPICodePack.Shell.KnownFolders.SystemX86.Path);
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
            path = Path_Replace(path, "<idownload>", Microsoft.WindowsAPICodePack.Shell.KnownFolders.Downloads.Path);
            path = Path_Replace(path, "<cdownload>", Microsoft.WindowsAPICodePack.Shell.KnownFolders.PublicDownloads.Path);
            path = Path_Replace(path, "<win>", Environment.GetFolderPath(Environment.SpecialFolder.Windows));
            path = Path_Replace(path, "<recent>", Environment.GetFolderPath(Environment.SpecialFolder.Recent));
            path = Path_Replace(path, "<profile>", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            path = Path_Replace(path, "<sendto>", Environment.GetFolderPath(Environment.SpecialFolder.SendTo));
            path = Path_Replace(path, "<hiuser>", App.EnvironmentUsername);
            path = Path_Replace(path, "<nop>", "");
            return path;
        }

        public static String Path_Prepare_EX(String path)
        {
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
            path = Path_Replace(path, "<me>", App.Username);
            path = Path_Replace(path, "<hiro>", App.AppTitle);
            path = Path_Replace(path, "<product>", Get_Translate("dlproduct"));
            return path;
        }
        #endregion

        #region 延时
        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)//毫秒
            {
                System.Windows.Forms.Application.DoEvents();//可执行某无聊的操作
            }
        }
        #endregion

        #region 运行文件

        public static void HiroInvoke(Action callback)
        {
            Application.Current.Dispatcher.Invoke(callback);
        }

        private static bool isMediaFile(string file)
        {
            var ext = $",{Path.GetExtension(file).ToLower()},";
            var aext = ",.3g2,.3gp,.3gp2,.3gpp,.amv,.asf,.avi,.bik,.crf,.dav,.divx,.drc,.dv,.dvr-ms,.evo,.f4v,.flv,.gvi,.gxf,.m1v,.m2v,.m2t,.m2ts,.m4v,.mkv,.mov,.mp2,.mp2v,.mp4,.mp4v,.mpe,.mpeg,.mpeg1,.mpeg2,.mpeg4,.mpg,.mpv2,.mts,.mtv,.mxf,.mxg,.nsv,.nuv,.ogm,.ogv,.ogx,.ps,.rec,.rm,.rmvb,.rpl,.thp,.tod,.tp,.ts,.tts,.txd,.vob,.vro,.webm,.wm,.wmv,.wtv,.xesc,.3ga,.669,.a52,.aac,.ac3,.adt,.adts,.aif,.aifc,.aiff,.amb,.amr,.aob,.ape,.au,.awb,.caf,.dts,.flac,.it,.kar,.m4a,.m4b,.m4p,.m5p,.mid,.mka,.mlp,.mod,.mpa,.mp1,.mp2,.mp3,.mpc,.mpga,.mus,.oga,.ogg,.oma,.opus,.qcp,.ra,.rmi,.s3m,.sid,.spx,.tak,.thd,.tta,.voc,.vqf,.w64,.wav,.wma,.wv,.xa,.xm,";
            return aext.IndexOf(ext) != -1;
        }


        public static void RunExe(string RunPath, string? source = null, bool autoClose = true, bool urlCheck = true)
        {
            new System.Threading.Thread(() =>
            {
                if (App.dflag)
                    LogtoFile($"[RUN]Path: {RunPath}, Source: {source ?? "Null"}");
                var path = Path_Prepare_EX(Path_Prepare(RunPath));
                try
                {
                    var parameter = HiroCmdParse(path);
                    #region 预处理参数
                    for (var i = 0; i < parameter.Count; i++)
                    {
                        var pi = parameter[i];
                        if (pi.ToLower().EndsWith("<any>"))
                        {
                            pi = pi[..^5];
                            DirectoryInfo directory = new DirectoryInfo(pi);
                            var files = directory.GetFiles("*", SearchOption.TopDirectoryOnly);
                            var ImgList = files.Select(s => s.FullName).ToList();
                            parameter[i] = ImgList[new Random().Next(0, ImgList.Count - 1)];
                        }
                        if (pi.ToLower().EndsWith("<xany>"))
                        {
                            pi = pi[..^6];
                            DirectoryInfo directory = new DirectoryInfo(pi);
                            var files = directory.GetFiles("*", SearchOption.AllDirectories);
                            var ImgList = files.Select(s => s.FullName).ToList();
                            parameter[i] = ImgList[new Random().Next(0, ImgList.Count - 1)];
                        }
                    }
                    #endregion
                    int disturb = int.Parse(Read_Ini(App.dconfig, "Config", "Disturb", "2"));
                    #region 识别文件类型
                    if (File.Exists(path))
                    {
                        if (isMediaFile(path))
                        {
                            LogtoFile("[RUN]Media file detected");
                            path = $"play(\"{path}\")";
                            parameter = HiroCmdParse(path);
                        }
                        else
                        {
                            if (path.ToLower().EndsWith(".hiro"))
                            {
                                LogtoFile("[RUN]Hiro file detected");
                                path = $"seq(\"{path}\")";
                                parameter = HiroCmdParse(path);
                            }
                            else if (path.ToLower().EndsWith(".hef"))
                            {
                                LogtoFile("[RUN]Encrypted file detected");
                                path = $"decrypt(\"{path}\")";
                                parameter = HiroCmdParse(path);
                            }
                        }
                    }
                    #endregion
                    #region 不会造成打扰的命令
                    if (path.ToLower().Equals("memory()"))
                    {
                        GC.Collect();
                        goto RunOK;
                    }
                    #region 调试
                    if (path.ToLower().StartsWith("debug("))
                    {
                        source = Get_Translate("debug");
                        if (!path.ToLower().StartsWith("debug()"))
                        {
                            RunExe($"notify({path},2)", source);
                        }
                        else
                        {
                            App.dflag = !App.dflag;
                            if (App.dflag)
                                RunExe($"notify({Get_Translate("debugon")},2)", source);
                            else
                                RunExe($"notify({Get_Translate("debugoff")},2)", source);
                        }
                        goto RunOK;
                    }
                    #endregion
                    if (path.ToLower().StartsWith("save("))
                    {
                        source = Get_Translate("download");
                        string result = "";
                        CreateFolder(parameter[1]);
                        result = GetWebContent(parameter[0], true, parameter[1]);
                        if (result.ToLower().Equals("error"))
                            App.Notify(new Hiro_Notice(Get_Translate("error"), 2, source));
                        else
                            App.Notify(new Hiro_Notice(Get_Translate("success"), 2, source));
                        goto RunOK;
                    }
                    #region 壁纸相关
                    if (path.ToLower().StartsWith("bingw("))
                    {
                        if (!File.Exists(parameter[0]))
                        {
                            HttpRequestMessage request = new(HttpMethod.Get, "https://api.rexio.cn/v1/rex.php?r=wallpaper");
                            request.Headers.Add("UserAgent", $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.71 Safari/537.36 HiroApplication/{Hiro_Resources.ApplicationVersion}");
                            request.Content = new StringContent("");
                            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                            if (App.hc == null)
                                goto RunOK;
                            try
                            {
                                HttpResponseMessage response = App.hc.Send(request);

                                if (response.Content != null)
                                {
                                    Stream stream = response.Content.ReadAsStream();
                                    System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                                    CreateFolder(parameter[0]);
                                    image.Save(parameter[0]);
                                }
                            }
                            catch (Exception ex)
                            {
                                RunExe($"alarm({Get_Translate("error")},{ex})");
                                LogError(ex, $"Hiro.Exception.Wallpaper.HttpClient{Environment.NewLine}Path: {path}");
                            }
                            if (!File.Exists(parameter[0]))
                                App.Notify(new Hiro_Notice(Get_Translate("unknown"), 2, Get_Translate("wallpaper")));
                            else
                                App.Notify(new Hiro_Notice(Get_Translate("wpsaved"), 2, Get_Translate("wallpaper")));
                        }
                        else
                            App.Notify(new Hiro_Notice(Get_Translate("wpexist"), 2, Get_Translate("wallpaper")));
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("wallpaper("))
                    {
                        source = Get_Translate("wallpaper");
                        if (File.Exists(parameter[0]))
                        {
                            using (Microsoft.Win32.RegistryKey? key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
                            {
                                int[] para = new int[] { 10, 6, 22, 2, 0, 0 };
                                int[] par = new int[] { 0, 0, 0, 0, 1, 0 };
                                var v = Convert.ToInt32(parameter[1]);
                                v = v < 0 ? 0 : v > 5 ? 5 : v;
                                if (key != null)
                                {
                                    key.SetValue(@"WallpaperStyle", para[v].ToString());
                                    key.SetValue(@"TileWallpaper", par[v].ToString());
                                }
                            }
                            _ = SystemParametersInfo(20, 0, parameter[0], 0x01 | 0x02);
                            App.Notify(new Hiro_Notice(Get_Translate("wpchanged"), 2, source));
                        }
                        else
                        {
                            RunExe($"notify({Get_Translate("wpnexist")},2)", source);
                        }
                        goto RunOK;
                    }
                    #endregion
                    #region 文件操作
                    if (path.ToLower().StartsWith("delete("))
                    {
                        if (!File.Exists(parameter[0]))
                        {
                            if (Directory.Exists(parameter[0]))
                                try
                                {
                                    Directory.Delete(parameter[0], true);
                                }
                                catch (Exception ex)
                                {
                                    App.Notify(new Hiro_Notice(Get_Translate("failed"), 2, Get_Translate("file")));
                                    LogError(ex, $"Hiro.Exception.IO.Delete{Environment.NewLine}Path: {path}");
                                }
                            else
                                LogtoFile($"[WARNING]Hiro.Warning.IO.Delete: Warning at {path} | Details: {Get_Translate("filenotexist")}");
                            goto RunOK;
                        }
                        try
                        {
                            File.Delete(parameter[0]);
                        }
                        catch (Exception ex)
                        {
                            App.Notify(new Hiro_Notice(Get_Translate("failed"), 2, Get_Translate("file")));
                            LogError(ex, $"Hiro.Exception.IO.Delete{Environment.NewLine}Path: {path}");
                        }
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("move("))
                    {
                        if (!File.Exists(parameter[0]))
                        {
                            try
                            {
                                CreateFolder(parameter[1]);
                                Directory.Move(parameter[0], parameter[1]);
                            }
                            catch (Exception ex)
                            {
                                App.Notify(new Hiro_Notice(Get_Translate("failed"), 2, Get_Translate("file")));
                                LogError(ex, $"Hiro.Exception.IO.Move{Environment.NewLine}Path: {path}");
                            }
                            goto RunOK;
                        }
                        try
                        {
                            CreateFolder(parameter[1]);
                            File.Move(parameter[0], parameter[1]);
                        }
                        catch (Exception ex)
                        {
                            App.Notify(new Hiro_Notice(Get_Translate("failed"), 2, Get_Translate("file")));
                            LogError(ex, $"Hiro.Exception.IO.Move{Environment.NewLine}Path: {path}");
                        }
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("copy("))
                    {
                        if (!File.Exists(parameter[0]))
                        {
                            if (Directory.Exists(parameter[0]))
                                try
                                {
                                    CopyDirectory(parameter[0], parameter[1]);
                                }
                                catch (Exception ex)
                                {
                                    App.Notify(new Hiro_Notice(Get_Translate("failed"), 2, Get_Translate("file")));
                                    LogError(ex, $"Hiro.Exception.IO.Copy{Environment.NewLine}Path: {path}");
                                }
                            else
                            {
                                App.Notify(new Hiro_Notice(Get_Translate("syntax"), 2, Get_Translate("file")));
                                LogError(new FileNotFoundException(), $"Hiro.Exception.IO.Copy{Environment.NewLine}Path: {path}");
                            }
                            goto RunOK;
                        }
                        try
                        {
                            CreateFolder(parameter[1]);
                            File.Copy(parameter[0], parameter[1]);
                        }
                        catch (Exception ex)
                        {
                            App.Notify(new Hiro_Notice(Get_Translate("failed"), 2, Get_Translate("file")));
                            LogError(ex, $"Hiro.Exception.IO.Copy{Environment.NewLine}Path: {path}");
                        }
                        goto RunOK;
                    }
                    #endregion
                    #region 系统环境
                    if (path.ToLower().StartsWith("vol("))
                    {
                        switch (path.ToLower())
                        {
                            case "vol(up)":
                                keybd_event(0xAF, MapVirtualKey(0xAF, 0), 0x0001, 0);
                                keybd_event(0xAF, MapVirtualKey(0xAF, 0), 0x0001 | 0x0002, 0);
                                break;
                            case "vol(down)":
                                keybd_event(0xAE, MapVirtualKey(0xAE, 0), 0x0001, 0);
                                keybd_event(0xAE, MapVirtualKey(0xAE, 0), 0x0001 | 0x0002, 0);
                                break;
                            case "vol(mute)":
                                keybd_event(0xAD, MapVirtualKey(0xAD, 0), 0x0001, 0);
                                keybd_event(0xAD, MapVirtualKey(0xAD, 0), 0x0001 | 0x0002, 0);
                                break;
                            default:
                                break;
                        }
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("bluetooth("))
                    {
                        bool? situation = path.ToLower() switch
                        {
                            "bluetooth(0)" or "bluetooth(off)" => false,
                            "bluetooth(1)" or "bluetooth(on)" => true,
                            _ => null,
                        };
                        SetBthState(situation);
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("wifi("))
                    {
                        int situation = path.ToLower() switch
                        {
                            "wifi(0)" or "wifi(off)" => 0,
                            "wifi(1)" or "wifi(on)" => 1,
                            "wifi(2)" or "wifi(dis)" or "wifi(disconnect)" => 2,
                            "wifi(3)" or "wifi(con)" or "wifi(connect)" => 3,
                            _ => -1,
                        };
                        if (situation == -1)
                        {
                            if (parameter.Count > 1 && parameter[1].ToLower().IndexOf("o") != -1)
                            {
                                SetWiFiState(3, parameter[0], true);
                            }
                            else
                                SetWiFiState(3, parameter[0]);

                        }
                        else
                            SetWiFiState(situation);
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("media("))
                    {
                        System.Windows.Input.Key situation = path.ToLower() switch
                        {
                            "media(0)" or "media(off)" or "media(down)" or "media(↓)" or "media(stop)" or "media(end)" => System.Windows.Input.Key.MediaStop,
                            "media(1)" or "media(on)" or "media(up)" or "media(↑)" or "media(start)" or "media(begin)" or "media(invoke)" => System.Windows.Input.Key.MediaPlayPause,
                            "media(next)" or "media(2)" or "media(right)" or "media(→)" => System.Windows.Input.Key.MediaNextTrack,
                            "media(previous)" or "media(last)" or "media(3)" or "media(left)" or "media(←)" => System.Windows.Input.Key.MediaPreviousTrack,
                            _ => System.Windows.Input.Key.MediaStop,
                        };
                        var keyinfo = situation switch
                        {
                            System.Windows.Input.Key.MediaStop => "End",
                            System.Windows.Input.Key.MediaPlayPause => "Start",
                            System.Windows.Input.Key.MediaNextTrack => "Next",
                            System.Windows.Input.Key.MediaPreviousTrack => "Previous"
                        };
                        var keyi = (byte)System.Windows.Input.KeyInterop.VirtualKeyFromKey(situation);
                        if (App.dflag)
                            LogtoFile("[MEDIA]Media Control : " + keyinfo);
                        keybd_event(keyi, MapVirtualKey(keyi, 0), 0x0001, 0);
                        keybd_event(keyi, MapVirtualKey(keyi, 0), 0x0001 | 0x0002, 0);
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("key("))
                    {
                        List<byte> modi = new();
                        int pathi = int.Parse(parameter[0]);
                        if (pathi >= 8)
                        {
                            while (pathi >= 8)
                            {
                                pathi -= 8;
                            }
                            modi.Add(0x5B);//Windows
                        }
                        if (pathi >= 4)
                        {
                            while (pathi >= 4)
                            {
                                pathi -= 4;
                            }
                            modi.Add(0x10);
                        }
                        if (pathi >= 2)
                        {
                            while (pathi >= 2)
                            {
                                pathi -= 2;
                            }
                            modi.Add(0x11);
                        }
                        if (pathi >= 1)
                            modi.Add(0x12);
                        byte parai = (byte)System.Windows.Input.KeyInterop.VirtualKeyFromKey((System.Windows.Input.Key)int.Parse(parameter[1]));
                        for (int i = 0; i < modi.Count; i++)
                        {
                            keybd_event(modi[i], MapVirtualKey(modi[i], 0), 0x0001, 0);
                        }
                        keybd_event(parai, MapVirtualKey(parai, 0), 0x0001, 0);
                        keybd_event(parai, MapVirtualKey(parai, 0), 0x0001 | 0x0002, 0);
                        for (int i = modi.Count - 1; i >= 0; i--)
                        {
                            keybd_event(modi[i], MapVirtualKey(modi[i], 0), 0x0001 | 0x0002, 0);
                        }
                        goto RunOK;
                    }
                    #endregion
                    if (path.ToLower().StartsWith("ini("))
                    {
                        Write_Ini(parameter[0], parameter[1], parameter[2], parameter[3]);
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("zip("))
                    {
                        CreateFolder(parameter[1]);
                        System.IO.Compression.ZipFile.CreateFromDirectory(parameter[0], parameter[1]);
                        BackgroundWorker bw = new();
                        if (parameter.Count > 2)
                        {
                            var para = parameter[2].ToLower();
                            if (para.IndexOf("s") != -1)
                                RunExe(parameter[1]);
                            if (para.IndexOf("d") != -1)
                                RunExe($"Delete({parameter[0]})");
                        }
                        if (parameter.Count > 3)
                        {
                            string cmd = parameter[3];
                            for (var i = 4; i < parameter.Count; i++)
                            {
                                cmd += "," + parameter[i];
                            }
                            RunExe(cmd, source);
                        }
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("unzip("))
                    {
                        CreateFolder(parameter[1]);
                        System.IO.Compression.ZipFile.ExtractToDirectory(parameter[0], parameter[1]);
                        if (parameter.Count > 2)
                        {
                            var para = parameter[2].ToLower();
                            if (para.IndexOf("s") != -1)
                                RunExe(parameter[1]);
                            if (para.IndexOf("d") != -1)
                                RunExe($"Delete({parameter[0]})");
                        }
                        if (parameter.Count > 3)
                        {
                            string cmd = parameter[3];
                            for (var i = 4; i < parameter.Count; i++)
                            {
                                cmd += "," + parameter[i];
                            }
                            RunExe(cmd, source);
                        }
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("exit()"))
                    {
                        try
                        {
                            HiroInvoke(() =>
                            {
                                Environment.Exit(Environment.ExitCode);
                                Application.Current.Shutdown();
                            });
                        }
                        catch (Exception ex)
                        {
                            LogError(ex, $"Hiro.Exception.Exit{Environment.NewLine}Path: {path}");
                        }
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("hide()"))
                    {
                        HiroInvoke(() =>
                        {
                            if (App.mn != null)
                            {
                                App.mn.titlelabel.Visibility = Visibility.Hidden;
                                App.mn.versionlabel.Visibility = Visibility.Hidden;
                                App.mn.minbtn.Visibility = Visibility.Hidden;
                                App.mn.closebtn.Visibility = Visibility.Hidden;
                                App.mn.stack.Visibility = Visibility.Hidden;
                                App.mn.Visibility = Visibility.Hidden;
                                App.mn.Update_VlcPlayer_Status();
                            }
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("menu()"))
                    {
                        HiroInvoke(() =>
                        {
                            if (App.wnd != null && App.wnd.cm != null)
                            {
                                App.Load_Menu();
                                App.wnd.cm.IsOpen = true;
                            }
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("show()"))
                    {
                        HiroInvoke(() =>
                        {
                            App.mn ??= new();
                            App.mn.Show();
                            App.mn.Visibility = Visibility.Visible;
                            App.mn.HiHiro();
                        });
                        goto RunOK;
                    }
                    if (path.ToLower() == "hello" || path.ToLower() == "hello()")
                    {
                        var hr = DateTime.Now.Hour;
                        var morning = Read_Ini(App.LangFilePath, "local", "morning", "[6,7,8,9,10]");
                        var noon = Read_Ini(App.LangFilePath, "local", "noon", "[11,12,13]");
                        var afternoon = Read_Ini(App.LangFilePath, "local", "afternoon", "[14,15,16,17,18]");
                        var evening = Read_Ini(App.LangFilePath, "local", "evening", "[19,20,21,22]");
                        var night = Read_Ini(App.LangFilePath, "local", "night", "[23,0,1,2,3,4,5]");
                        morning = $",{morning.Replace("[", "").Replace("]", "").Replace(" ", "")},";
                        noon = $",{noon.Replace("[", "").Replace("]", "").Replace(" ", "")},";
                        afternoon = $",{afternoon.Replace("[", "").Replace("]", "").Replace(" ", "")},";
                        evening = $",{evening.Replace("[", "").Replace("]", "").Replace(" ", "")},";
                        night = $",{night.Replace("[", "").Replace("]", "").Replace(" ", "")},";
                        var trstrs = new string[] { "morning", "morningcus", "noon", "nooncus", "afternoon", "afternooncus", "evening", "eveningcus", "night", "nights" };
                        int trindex;
                        if (morning.IndexOf("," + hr + ",") != -1)
                            trindex = 0;
                        else if (noon.IndexOf($",{hr},") != -1)
                            trindex = 1;
                        else if (afternoon.IndexOf($",{hr},") != -1)
                            trindex = 2;
                        else if (evening.IndexOf($",{hr},") != -1)
                            trindex = 3;
                        else
                            trindex = 4;
                        if (App.CustomUsernameFlag == 0)
                            App.Notify(new Hiro_Notice(Get_Translate(trstrs[trindex * 2]).Replace("%u", App.EnvironmentUsername), 2, Get_Translate("hello")));
                        else
                            App.Notify(new Hiro_Notice(Get_Translate(trstrs[trindex * 2 + 1]).Replace("%u", App.Username), 2, Get_Translate("hello")));
                        goto RunOK;
                    }
                    //sequence(uri)
                    if (path.ToLower().StartsWith("seq("))
                    {

                        var ca = parameter.Count < 2 || (!parameter[1].ToLower().Equals("hide") && !parameter[1].ToLower().Equals("h"));
                        var cb = (disturb == 1 && IsForegroundFullScreen()) || disturb == 0;
                        HiroInvoke(() =>
                        {
                            Hiro_Sequence sq = new();
                            if (ca && !cb)
                                sq.Show();
                            sq.SeqExe(parameter[0]);
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("item(") && !path.ToLower().StartsWith("item()"))
                    {
                        var RealPath = parameter[0];
                        for (int i = 1; i < parameter.Count; i++)
                        {
                            RealPath += ($",{parameter[i]}");
                        }
                        HiroInvoke(() =>
                        {
                            foreach (var cmd in App.cmditems)
                            {
                                if (cmd.Name.Equals(RunPath) || cmd.Name.Equals(RealPath) || cmd.Name.Equals(path))
                                {
                                    RunExe(cmd.Command);
                                    break;
                                }
                            }
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("run("))
                    {
                        if (parameter.Count == 0)
                        {
                            App.Notify(new Hiro_Notice(Get_Translate("syntax"), 2, Get_Translate("execute")));
                            goto RunOK;
                        }
                        string? FileName = parameter.Count >= 1 ? parameter[0] : null;
                        string? Arguments = parameter.Count >= 3 ? parameter[2] : null;
                        string? HiroArguments = parameter.Count >= 2 ? parameter[1] : null;
                        HiroArguments = HiroArguments == null ? null : HiroArguments.ToLower();
                        if (FileName == null)
                        {
                            App.Notify(new Hiro_Notice(Get_Translate("syntax"), 2, Get_Translate("execute")));
                            goto RunOK;
                        }
                        try
                        {
                            ProcessStartInfo pinfo = new()
                            {
                                UseShellExecute = true,
                                FileName = FileName,
                                Arguments = Arguments
                            };
                            if (HiroArguments != null)
                            {
                                if (HiroArguments.IndexOf("a") != -1)
                                    pinfo.Verb = "runas";
                                if (HiroArguments.IndexOf("h") != -1)
                                    pinfo.WindowStyle = ProcessWindowStyle.Hidden;
                                if (HiroArguments.IndexOf("i") != -1)
                                    pinfo.WindowStyle = ProcessWindowStyle.Minimized;
                                if (HiroArguments.IndexOf("x") != -1)
                                    pinfo.WindowStyle = ProcessWindowStyle.Maximized;
                                if (HiroArguments.IndexOf("n") != -1)
                                    pinfo.CreateNoWindow = true;
                            }
                            Run_Process(pinfo, path, RunPath);
                        }
                        catch (Exception ex)
                        {
                            RunExe($"alarm({Get_Translate("error")},{ex})");
                            LogError(ex, $"Hiro.Exception.Run.Extra{Environment.NewLine}Path: {path}");
                        }
                        if (App.mn == null)
                            RunExe("exit()");
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("lock()"))
                    {
                        HiroInvoke(() =>
                        {
                            App.Locked = true;
                            if (App.mn == null)
                                return;
                            App.mn.Set_Label(App.mn.homex);
                            App.mn.versionlabel.Content = Hiro_Resources.ApplicationVersion + " 🔒";
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("weather("))
                    {
                        source = Get_Translate("weather");
                        path = path.ToLower() switch
                        {
                            "weather(0)" => $"alarm({Get_Translate("weather")},https://api.rexio.cn/v1/rex.php?r=weather&k=6725dccca57b2998e8fc47cee2a8f72f&lang={App.lang})",
                            "weather(1)" => $"notify(https://api.rexio.cn/v1/rex.php?r=weather&k=6725dccca57b2998e8fc47cee2a8f72f&lang={App.lang},2)",
                            _ => "notify(" + Get_Translate("syntax") + ",2)"
                        };
                        RunExe(path, source);
                        goto RunOK;
                    }
                    if (path.ToLower() == "nop" || path.ToLower() == "nop()") goto RunOK;
                    #endregion
                    if ((disturb == 1 && IsForegroundFullScreen()) || disturb == 0)
                    {
                        App.mn?.AddToInfoCenter($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ")}{Environment.NewLine}\t{Get_Translate("infocmd")}:\t{RunPath}{Environment.NewLine}\t{Get_Translate("infosource")}:\t{source} {Environment.NewLine} ");
                        goto RunOK;
                    }
                    #region 可能造成打扰的命令
                    if (path.ToLower().Equals("hirotest()"))
                    {
                        HiroInvoke(() =>
                        {
                            new Hiro_Test().Show();
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("invoke("))
                    {
                        if (parameter.Count > 0)
                        {
                            var pa = parameter[0];
                            if (pa.ToLower().StartsWith("http://") || pa.ToLower().StartsWith("https://"))
                            {
                                pa = GetWebContent(pa).Replace("\\n", string.Empty).Replace("<br>", string.Empty);
                                if (App.dflag)
                                    LogtoFile("[INVOKE]" + pa);
                            }
                            RunExe(pa, source, autoClose);
                        }
                        goto RunOK;
                    }
                    if (path.ToLower().Equals("decrypt"))
                    {
                        HiroInvoke(() =>
                        {
                            new Hiro_Encrypter(1).Show();
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("decrypt("))
                    {
                        HiroInvoke(() =>
                        {
                            string? p = null;
                            string? k = null;
                            if (parameter.Count > 0)
                            {
                                p = parameter[0];
                            }
                            if (parameter.Count > 1)
                            {
                                k = parameter[1];
                            }
                            var he = new Hiro_Encrypter(1, p, k);
                            if (parameter.Count > 2)
                            {
                                var pa = parameter[2].Trim().ToLower();
                                if (pa.Equals("r") || pa.Equals("run") || pa.Equals("1"))
                                    he.Autorun.IsChecked = true;
                                if (pa.Equals("l") || pa.Equals("locate") || pa.Equals("2"))
                                    he.Autorun.IsChecked = null;
                            }
                            if (parameter.Count > 3)
                            {
                                var pa = parameter[3].Trim().ToLower();
                                if (pa.Equals("d") || pa.Equals("delete") || pa.Equals("1"))
                                    he.Autodelete.IsChecked = true;
                            };
                            he.Show();
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("decrypto("))
                    {
                        HiroInvoke(() =>
                        {
                            string? p = null;
                            string? k = null;
                            if (parameter.Count > 1)
                            {
                                p = parameter[0];
                                k = parameter[1];
                                var he = new Hiro_Encrypter(1, p, k, true);
                                if (parameter.Count > 2)
                                {
                                    var pa = parameter[2].Trim().ToLower();
                                    if (pa.Equals("r") || pa.Equals("run") || pa.Equals("1"))
                                        he.Autorun.IsChecked = true;
                                    if (pa.Equals("l") || pa.Equals("locate") || pa.Equals("2"))
                                        he.Autorun.IsChecked = null;
                                }
                                if (parameter.Count > 3)
                                {
                                    var pa = parameter[3].Trim().ToLower();
                                    if (pa.Equals("d") || pa.Equals("delete") || pa.Equals("1"))
                                        he.Autodelete.IsChecked = true;
                                }
                                he.GoStart();
                            }
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().Equals("encrypt"))
                    {
                        HiroInvoke(() =>
                        {
                            new Hiro_Encrypter(0).Show();
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("encrypt("))
                    {
                        HiroInvoke(() =>
                        {
                            string? p = null;
                            string? k = null;
                            if (parameter.Count > 0)
                            {
                                p = parameter[0];
                            }
                            if (parameter.Count > 1)
                            {
                                k = parameter[1];
                            }
                            var he = new Hiro_Encrypter(0, p, k);
                            if (parameter.Count > 2)
                            {
                                var pa = parameter[2].Trim().ToLower();
                                if (pa.Equals("r") || pa.Equals("run") || pa.Equals("1"))
                                    he.Autorun.IsChecked = true;
                                if (pa.Equals("l") || pa.Equals("locate") || pa.Equals("2"))
                                    he.Autorun.IsChecked = null;
                            }
                            if (parameter.Count > 3)
                            {
                                var pa = parameter[3].Trim().ToLower();
                                if (pa.Equals("d") || pa.Equals("delete") || pa.Equals("1"))
                                    he.Autodelete.IsChecked = true;
                            };
                            he.Show();
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("encrypto("))
                    {
                        HiroInvoke(() =>
                        {
                            string? p = null;
                            string? k = null;
                            if (parameter.Count > 1)
                            {
                                p = parameter[0];
                                k = parameter[1];
                                var he = new Hiro_Encrypter(1, p, k, true);
                                if (parameter.Count > 2)
                                {
                                    var pa = parameter[2].Trim().ToLower();
                                    if (pa.Equals("r") || pa.Equals("run") || pa.Equals("1"))
                                        he.Autorun.IsChecked = true;
                                    if (pa.Equals("l") || pa.Equals("locate") || pa.Equals("2"))
                                        he.Autorun.IsChecked = null;
                                }
                                if (parameter.Count > 3)
                                {
                                    var pa = parameter[3].Trim().ToLower();
                                    if (pa.Equals("d") || pa.Equals("delete") || pa.Equals("1"))
                                        he.Autodelete.IsChecked = true;
                                }
                                he.GoStart();
                            }
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().Equals("island") || path.ToLower().Equals("island()"))
                    {
                        HiroInvoke(() =>
                        {
                            new Hiro_Island().Show();
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().Equals("idtracer") || path.ToLower().Equals("idtracer()"))
                    {
                        HiroInvoke(() =>
                        {
                            new Hiro_ID().Show();
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("ticker("))
                    {
                        var adflag = false;
                        if (parameter.Count < 1)
                            goto RunOK;
                        HiroInvoke(() =>
                        {
                            Hiro_Ticker? ht = null;
                            foreach (var win in Application.Current.Windows)
                            {
                                if (win is Hiro_Ticker hwin)
                                {
                                    if (hwin.id.Equals(parameter[0]))
                                    {
                                        ht = hwin;
                                        adflag = true;
                                    }
                                }
                            }
                            switch (parameter.Count)
                            {
                                case 1:
                                    ht ??= new(parameter[0]);
                                    break;
                                case 2:
                                    ht ??= new(parameter[0]);
                                    ht.format = parameter[1];
                                    ht.RefreshContent();
                                    break;
                                case 3:
                                    var p3 = int.TryParse(parameter[2], out int pa3) ? pa3 : 0;
                                    ht ??= new(parameter[0]);
                                    ht.format = parameter[1];
                                    if (adflag)
                                        ht.OffsetNum(p3);
                                    else
                                        ht.current = p3;
                                    ht.RefreshContent();
                                    break;
                                case 4:
                                    var p4 = int.TryParse(parameter[2], out int pa4) ? pa4 : 0;
                                    var p4a = int.TryParse(parameter[3], out int pa4a) ? pa4a : int.MaxValue;
                                    ht ??= new(parameter[0]);
                                    ht.format = parameter[1];
                                    if (adflag)
                                        ht.OffsetNum(p4a);
                                    else
                                        ht.current = p4;
                                    ht.RefreshContent();
                                    break;
                                case 5:
                                    var p5 = int.TryParse(parameter[2], out int pa5) ? pa5 : 0;
                                    var p5a = int.TryParse(parameter[3], out int pa5a) ? pa5a : int.MaxValue;
                                    var p5b = int.TryParse(parameter[4], out int pa5b) ? pa5b : int.MinValue;
                                    ht ??= new(parameter[0]);
                                    ht.format = parameter[1];
                                    if (adflag)
                                        ht.OffsetNum(p5a);
                                    else
                                        ht.current = p5;
                                    ht.min = p5b;
                                    ht.RefreshContent();
                                    break;
                                case > 5:
                                    var px = int.TryParse(parameter[2], out int pax) ? pax : 0;
                                    var pxa = int.TryParse(parameter[3], out int paxa) ? paxa : int.MaxValue;
                                    var pxb = int.TryParse(parameter[4], out int paxb) ? paxb : int.MinValue;
                                    var pxc = int.TryParse(parameter[5], out int paxc) ? paxc : int.MaxValue;
                                    ht ??= new(parameter[0]);
                                    ht.format = parameter[1];
                                    if (adflag)
                                        ht.OffsetNum(pxa);
                                    else
                                        ht.current = px;
                                    ht.min = pxb;
                                    ht.max = pxc;
                                    ht.RefreshContent();
                                    break;
                            }
                            ht?.Show();
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("hiroad("))
                    {
                        source = Get_Translate("update");
                        HiroInvoke(() =>
                        {
                            Hiro_Download dl = new(1, parameter[2]);
                            dl.textBoxHttpUrl.Text = parameter[0];
                            dl.SavePath.Text = parameter[1];
                            dl.Autorun.IsChecked = true;
                            dl.Autorun.IsEnabled = false;
                            dl.rurl = parameter[0];
                            dl.rpath = parameter[1];
                            dl.Show();
                            dl.StartDownload();
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("download("))
                    {
                        source = Get_Translate("download");
                        HiroInvoke(() =>
                        {
                            Hiro_Download dl = new(0, "");
                            if (parameter.Count > 0)
                                dl.textBoxHttpUrl.Text = parameter[0];
                            if (parameter.Count > 1)
                                dl.SavePath.Text = parameter[1];
                            dl.Show();
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("alarm("))
                    {
                        var pa = parameter[0];
                        var os = Get_OSVersion();
                        if (os.IndexOf(".") != -1)
                            os = os[..os.IndexOf(".")];
                        var boo = Read_Ini(App.dconfig, "Config", "Toast", "0").Equals("1") && int.TryParse(os, out int a) && a >= 10;
                        if (boo)
                        {
                            if (pa.ToLower().StartsWith("http://") || pa.ToLower().StartsWith("https://"))
                            {
                                pa = GetWebContent(pa);
                            }
                            if (parameter.Count > 1)
                            {
                                var par = parameter[1];

                                if ((par.ToLower().StartsWith("http://") || par.ToLower().StartsWith("https://")) && boo)
                                {
                                    par = GetWebContent(par).Replace("\\n", Environment.NewLine).Replace("<br>", Environment.NewLine);
                                }
                            }
                            if (parameter.Count > 1)
                            {
                                HiroInvoke(() =>
                                {
                                    new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                                .AddArgument("Launch", App.AppTitle)
                                .AddText(parameter[0])
                                .AddText(parameter[1].Replace("\\n", Environment.NewLine).Replace("<br>", Environment.NewLine))
                                .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                            .SetContent(Get_Translate("alarmone")))
                                .Show();
                                });

                            }
                            else
                            {
                                HiroInvoke(() =>
                                {
                                    new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                                .AddText(Get_Translate("alarmtitle"))
                                .AddText(parameter[0].Replace("\\n", Environment.NewLine).Replace("<br>", Environment.NewLine))
                                .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                            .SetContent(Get_Translate("alarmone")))
                                .Show();
                                });
                            }
                        }
                        else
                        {
                            HiroInvoke(() =>
                            {
                                if (parameter.Count > 1)
                                    new Hiro_Alarm(-1, CustomedTitle: parameter[0], CustomedContnet: parameter[1], OneButtonOnly: 1).Show();
                                else
                                    new Hiro_Alarm(-1, CustomedContnet: parameter[0], OneButtonOnly: 1).Show();
                            });
                        }
                        goto RunOK;
                    }
                    if (App.mn != null)
                    {
                        if (path.ToLower().StartsWith("home()"))
                        {
                            HiroInvoke(() =>
                            {
                                if (App.mn.Visibility != Visibility.Visible)
                                    RunExe("show()");
                                App.mn.Set_Label(App.mn.homex);
                            });
                            goto RunOK;
                        }
                        if (path.ToLower().StartsWith("item()"))
                        {
                            HiroInvoke(() =>
                            {
                                if (App.mn.Visibility != Visibility.Visible)
                                    RunExe("show()");
                                App.mn.Set_Label(App.mn.itemx);
                            });
                            goto RunOK;
                        }
                        if (path.ToLower().StartsWith("schedule()"))
                        {
                            HiroInvoke(() =>
                            {
                                if (App.mn.Visibility != Visibility.Visible)
                                    RunExe("show()");
                                App.mn.Set_Label(App.mn.schedulex);
                            });
                            goto RunOK;
                        }
                        if (path.ToLower().StartsWith("config()"))
                        {
                            HiroInvoke(() =>
                            {
                                if (App.mn.Visibility != Visibility.Visible)
                                    RunExe("show()");
                                App.mn.Set_Label(App.mn.configx);
                            });
                            goto RunOK;
                        }
                        if (path.ToLower().StartsWith("chat()"))
                        {
                            HiroInvoke(() =>
                            {
                                if (App.mn.Visibility != Visibility.Visible)
                                    RunExe("show()");
                                App.mn.Set_Label(App.mn.chatx);
                            });
                            goto RunOK;
                        }
                        if (path.ToLower().StartsWith("me()"))
                        {
                            HiroInvoke(() =>
                            {
                                if (App.mn.Visibility != Visibility.Visible)
                                    RunExe("show()");
                                App.mn.Set_Label(App.mn.profilex);
                            });
                            goto RunOK;
                        }
                        if (path.ToLower().StartsWith("about()"))
                        {
                            HiroInvoke(() =>
                            {
                                if (App.mn.Visibility != Visibility.Visible)
                                    RunExe("show()");
                                App.mn.Set_Label(App.mn.aboutx);
                            });
                            goto RunOK;
                        }
                    }
                    if (path.ToLower().StartsWith("restart("))
                    {
                        if (App.mn == null)
                        {
                            HiroInvoke(() =>
                            {
                                App.mn = new();
                                App.mn.Show();
                            });

                            goto RunOK;
                        }
                        int situation = path.ToLower() switch
                        {
                            "restart(0)" => 0,
                            _ => 1
                        };
                        HiroInvoke(() =>
                        {
                            if (situation == 0)
                            {
                                App.mn.MainUI_Initialize();
                                App.mn.HiHiro();

                            }
                            if (situation == 1)
                            {
                                App.mn.Close();
                                App.mn = new();
                                App.mn.Show();
                            }
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("message("))
                    {
                        HiroInvoke(() =>
                        {
                            Hiro_Background? bg = null;
                            if (Read_Ini(parameter[0], "Action", "Background", "true").ToLower().Equals("true"))
                                bg = new();
                            Hiro_Msg msg = new(parameter[0])
                            {
                                bg = bg,
                                Title = Path_Prepare(Path_Prepare_EX(Read_Ini(parameter[0], "Message", "Title", Get_Translate("syntax")))) + " - " + App.AppTitle
                            };
                            msg.backtitle.Content = Path_Prepare(Path_Prepare_EX(Path_Prepare_EX(Read_Ini(parameter[0], "Message", "Title", Get_Translate("syntax")))));
                            msg.acceptbtn.Content = Read_Ini(parameter[0], "Message", "accept", Get_Translate("msgaccept"));
                            msg.rejectbtn.Content = Read_Ini(parameter[0], "Message", "reject", Get_Translate("msgreject"));
                            msg.cancelbtn.Content = Read_Ini(parameter[0], "Message", "cancel", Get_Translate("msgcancel"));
                            parameter[0] = Path_Prepare_EX(Path_Prepare(Read_Ini(parameter[0], "Message", "content", Get_Translate("syntax"))));
                            if (parameter[0].ToLower().StartsWith("http://") || parameter[0].ToLower().StartsWith("https://"))
                            {
                                msg.sv.Content = Get_Translate("msgload");
                                BackgroundWorker bw = new();
                                bw.DoWork += delegate
                                {
                                    parameter[0] = GetWebContent(parameter[0]).Replace("<br>", "\\n");
                                };
                                bw.RunWorkerCompleted += delegate
                                {
                                    msg.sv.Content = parameter[0].Replace("\\n", Environment.NewLine);
                                };
                                bw.RunWorkerAsync();
                            }
                            else if (File.Exists(parameter[0]))
                                msg.sv.Content = Path_Prepare(Path_Prepare_EX(File.ReadAllText(parameter[0]))).Replace("\\n", Environment.NewLine);
                            else
                                msg.sv.Content = parameter[0].Replace("\\n", Environment.NewLine);
                            msg.Load_Position();
                            msg.Show();
                        });
                        goto RunOK;
                    }

                    if (path.Length > 7 && path.ToLower().StartsWith("notify("))
                    {
                        string titile = Get_Translate("syntax");
                        int duration = -1;
                        if (parameter.Count > 0)
                        {
                            try
                            {
                                duration = parameter.Count > 1 ? Convert.ToInt32(parameter[1]) : 2;
                                titile = parameter[0];
                                if (titile.ToLower().StartsWith("http://") || titile.ToLower().StartsWith("https://"))
                                {
                                    titile = GetWebContent(titile).Replace("<br>", "\\n");
                                }
                                Action? act = null;
                                if (parameter.Count > 3)
                                {
                                    var p3 = parameter[3];
                                    act = new(() =>
                                    {
                                        RunExe(p3, parameter[2], false);
                                    });
                                }
                                if (File.Exists(titile))
                                    titile = File.ReadAllText(titile).Replace(Environment.NewLine, "\\n");
                                if (parameter.Count > 2)
                                    source = parameter[2];
                                duration = duration <= 0 ? 2 : duration;
                                App.Notify(new(titile, duration, source, act));
                            }
                            catch (Exception ex)
                            {
                                RunExe("alarm(" + Get_Translate("error") + "," + ex.ToString() + ")");
                                LogError(ex, $"Hiro.Exception.Run.Notification{Environment.NewLine}Path: {path}");
                            }
                        }
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("play("))
                    {
                        if (App.mn != null)
                        {
                            RunExe("run(\"" + Hiro_Resources.ApplicationPath + "\",,\"" + path + "" + "\" utils)");
                        }
                        else
                        {
                            HiroInvoke(() =>
                            {
                                if (path.ToLower().Equals("play()"))
                                    new Hiro_Player().Show();
                                else
                                    new Hiro_Player(parameter[0]).Show();
                            });
                        }
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("web("))
                    {
                        if (Read_Ini(App.dconfig, "Config", "URLConfirm", "0").Equals("1") && urlCheck && App.mn == null)
                        {
                            var acbak = autoClose;
                            var confrimWin = Path_Prepare_EX(Path_Prepare("<capp>\\<lang>\\url.hms"));
                            autoClose = false;
                            HiroInvoke(() =>
                            {
                                Hiro_Background? bg = null;
                                if (Read_Ini(confrimWin, "Action", "Background", "true").ToLower().Equals("true"))
                                    bg = new();
                                Hiro_Msg msg = new(confrimWin)
                                {
                                    bg = bg,
                                    Title = Path_Prepare(Path_Prepare_EX(Read_Ini(confrimWin, "Message", "Title", Get_Translate("syntax")))) + " - " + App.AppTitle
                                };
                                msg.backtitle.Content = Path_Prepare(Path_Prepare_EX(Path_Prepare_EX(Read_Ini(confrimWin, "Message", "Title", Get_Translate("syntax")))));
                                msg.acceptbtn.Content = Read_Ini(confrimWin, "Message", "accept", Get_Translate("msgaccept"));
                                msg.rejectbtn.Content = Read_Ini(confrimWin, "Message", "reject", Get_Translate("msgreject"));
                                msg.cancelbtn.Content = Read_Ini(confrimWin, "Message", "cancel", Get_Translate("msgcancel"));
                                confrimWin = Path_Prepare_EX(Path_Prepare(Read_Ini(confrimWin, "Message", "content", Get_Translate("syntax"))));
                                if (confrimWin.ToLower().StartsWith("http://") || confrimWin.ToLower().StartsWith("https://"))
                                {
                                    msg.sv.Content = Get_Translate("msgload");
                                    BackgroundWorker bw = new();
                                    bw.DoWork += delegate
                                    {
                                        confrimWin = GetWebContent(confrimWin).Replace("<br>", "\\n");
                                    };
                                    bw.RunWorkerCompleted += delegate
                                    {
                                        msg.sv.Content = confrimWin.Replace("\\n", Environment.NewLine);
                                    };
                                    bw.RunWorkerAsync();
                                }
                                else if (File.Exists(confrimWin))
                                    msg.sv.Content = Path_Prepare(Path_Prepare_EX(File.ReadAllText(confrimWin))).Replace("\\n", Environment.NewLine);
                                else
                                    msg.sv.Content = confrimWin.Replace("\\n", Environment.NewLine);
                                msg.Load_Position();
                                msg.OKButtonPressed += delegate
                                {
                                    RunExe(path, source, acbak, false);
                                };
                                msg.CancelButtonPressed += delegate
                                {
                                    if (acbak && App.mn == null)
                                        RunExe("exit()");
                                };
                                msg.RejectButtonPressed += delegate
                                {
                                    if (acbak && App.mn == null)
                                        RunExe("exit()");
                                };
                                msg.Show();
                            });
                        }
                        else
                        {
                            if (App.mn != null)
                            {
                                RunExe(@"run(" + Hiro_Resources.ApplicationPath + ",,\"" + path + "" + "\" utils)");
                            }
                            else
                            {
                                HiroInvoke(() =>
                                {
                                    Hiro_Web web;
                                    string webpara = File.Exists(parameter[0]) && parameter[0].EndsWith(".hwb") ? Read_Ini(parameter[0], "Web", "Parameters", "") : parameter.Count > 1 ? parameter[1] : "";
                                    if (File.Exists(parameter[0]) && parameter[0].EndsWith(".hwb"))
                                    {
                                        string? title = null;
                                        if (!Read_Ini(parameter[0], "Web", "Title", string.Empty).Equals(string.Empty))
                                            title = Read_Ini(parameter[0], "Web", "Title", string.Empty).Replace("%b", " ");
                                        var uri = Read_Ini(parameter[0], "Web", "URI", "about:blank");
                                        var UPF = Read_Ini(parameter[0], "Web", "Folder", "<hiuser>");
                                        web = new(uri, title, UPF)
                                        {
                                            Height = Double.Parse(Read_Ini(parameter[0], "Web", "Height", "450")),
                                            Width = Double.Parse(Read_Ini(parameter[0], "Web", "Width", "800"))
                                        };
                                    }
                                    else
                                    {
                                        web = parameter.Count switch
                                        {
                                            1 or 2 => new(parameter[0]),
                                            > 2 => new(parameter[0], null, parameter[2]),
                                            _ => new("https://www.rexio.cn/"),
                                        };
                                    }
                                    if (webpara.IndexOf("s") != -1)
                                        web.self = true;
                                    if (webpara.IndexOf("-m") != -1)
                                    {
                                        web.maxbtn.Visibility = Visibility.Collapsed;
                                        web.ResizeMode = ResizeMode.CanMinimize;
                                    }
                                    if (webpara.IndexOf("-r") != -1)
                                    {
                                        web.maxbtn.Visibility = Visibility.Collapsed;
                                        web.minbtn.Visibility = Visibility.Collapsed;
                                        web.ResizeMode = ResizeMode.NoResize;
                                    }
                                    if (webpara.IndexOf("i") != -1)
                                        web.WindowState = WindowState.Minimized;
                                    else if (webpara.IndexOf("x") != -1)
                                        web.WindowState = WindowState.Maximized;
                                    if (webpara.IndexOf("-c") != -1)
                                        web.WindowStartupLocation = WindowStartupLocation.Manual;
                                    if (webpara.IndexOf("t") != -1)
                                    {
                                        web.Topmost = true;
                                        web.topbtn.Content = "\uE77A";
                                        web.topbtn.ToolTip = Get_Translate("webbottom");
                                    }
                                    if (webpara.IndexOf("b") != -1)
                                        web.URLGrid.Visibility = Visibility.Visible;
                                    web.Show();
                                    web.Refreash_Layout();
                                });
                            }
                        }


                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("editor()"))
                    {
                        HiroInvoke(() =>
                        {
                            App.ed ??= new Hiro_Editor();
                            App.ed.Show();
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("lockscr("))
                    {
                        var location = parameter.Count > 0 && !parameter[0].Trim().Equals(string.Empty) ? parameter[0] : null;
                        HiroInvoke(() =>
                        {
                            App.ls ??= new(location);
                            App.ls.Show();
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("auth("))
                    {
                        Action sc = new(() =>
                        {
                            if (App.mn != null)
                            {
                                HiroInvoke(() =>
                                {
                                    App.mn.versionlabel.Content = Hiro_Resources.ApplicationVersion;
                                    App.Locked = false;
                                });
                            }
                            if (parameter.Count >= 1)
                            {
                                foreach (var pr in parameter)
                                {
                                    if (pr.Equals(string.Empty) || pr.Trim().Equals(string.Empty))
                                        continue;
                                    RunExe(pr, source);
                                }
                            }
                        });
                        Action fa = new(() =>
                        {
                            HiroInvoke(() =>
                            {
                                if (App.mn != null && App.Locked)
                                    App.mn.versionlabel.Content = Hiro_Resources.ApplicationVersion + " 🔒";
                            });
                        });
                        HiroInvoke(() =>
                        {
                            Register(sc, fa, fa);
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("hirowego()") || path.ToLower().StartsWith("finder()") || path.ToLower().StartsWith("start()"))
                    {
                        HiroInvoke(() =>
                        {
                            if (App.ls == null)
                            {
                                Hiro_Finder hf = new();
                                hf.Show();
                            }
                        });
                        goto RunOK;
                    }
                    if (Read_Ini(App.dconfig, "Config", "URLConfirm", "0").Equals("1") &&
                    (parameter[0].ToLower().StartsWith("https://") || parameter[0].ToLower().StartsWith("http://")
                    || parameter[0].ToLower().Equals("firefox")
                    || parameter[0].ToLower().Equals("chrome")
                    || parameter[0].ToLower().Equals("msedge")
                    || parameter[0].ToLower().Equals("iexplore")
                    || (parameter[0].ToLower().Equals("explorer") && (parameter[2].ToLower().StartsWith("https://") || parameter[2].ToLower().StartsWith("http://"))))
                    && urlCheck)
                    {
                        var acbak = autoClose;
                        var confrimWin = Path_Prepare_EX(Path_Prepare("<capp>\\<lang>\\url.hms"));
                        autoClose = false;
                        HiroInvoke(() =>
                        {
                            Hiro_Background? bg = null;
                            if (Read_Ini(confrimWin, "Action", "Background", "true").ToLower().Equals("true"))
                                bg = new();
                            Hiro_Msg msg = new(confrimWin)
                            {
                                bg = bg,
                                Title = Path_Prepare(Path_Prepare_EX(Read_Ini(confrimWin, "Message", "Title", Get_Translate("syntax")))) + " - " + App.AppTitle
                            };
                            msg.backtitle.Content = Path_Prepare(Path_Prepare_EX(Path_Prepare_EX(Read_Ini(confrimWin, "Message", "Title", Get_Translate("syntax")))));
                            msg.acceptbtn.Content = Read_Ini(confrimWin, "Message", "accept", Get_Translate("msgaccept"));
                            msg.rejectbtn.Content = Read_Ini(confrimWin, "Message", "reject", Get_Translate("msgreject"));
                            msg.cancelbtn.Content = Read_Ini(confrimWin, "Message", "cancel", Get_Translate("msgcancel"));
                            confrimWin = Path_Prepare_EX(Path_Prepare(Read_Ini(confrimWin, "Message", "content", Get_Translate("syntax"))));
                            if (confrimWin.ToLower().StartsWith("http://") || confrimWin.ToLower().StartsWith("https://"))
                            {
                                msg.sv.Content = Get_Translate("msgload");
                                BackgroundWorker bw = new();
                                bw.DoWork += delegate
                                {
                                    confrimWin = GetWebContent(confrimWin).Replace("<br>", "\\n");
                                };
                                bw.RunWorkerCompleted += delegate
                                {
                                    msg.sv.Content = confrimWin.Replace("\\n", Environment.NewLine);
                                };
                                bw.RunWorkerAsync();
                            }
                            else if (File.Exists(confrimWin))
                                msg.sv.Content = Path_Prepare(Path_Prepare_EX(File.ReadAllText(confrimWin))).Replace("\\n", Environment.NewLine);
                            else
                                msg.sv.Content = confrimWin.Replace("\\n", Environment.NewLine);
                            msg.Load_Position();
                            msg.OKButtonPressed += delegate
                            {
                                RunExe(path, source, acbak, false);
                            };
                            msg.CancelButtonPressed += delegate
                            {
                                if (acbak && App.mn == null)
                                    RunExe("exit()");
                            };
                            msg.RejectButtonPressed += delegate
                            {
                                if (acbak && App.mn == null)
                                    RunExe("exit()");
                            };
                            msg.Show();
                        });
                        goto RunOK;
                    }
                    var parameter_ = HiroCmdParse(path, false);
                    string? FileName_ = parameter_.Count >= 1 ? parameter_[0] : null;
                    if (FileName_ == null)
                    {
                        App.Notify(new Hiro_Notice(Get_Translate("syntax"), 2, Get_Translate("execute")));
                        goto RunOK;
                    }
                    ProcessStartInfo pinfo_ = new()
                    {
                        UseShellExecute = true,
                        FileName = FileName_,
                    };
                    if (App.dflag)
                        LogtoFile("[DEBUG]FileName " + FileName_);
                    if (parameter_.Count > 1)
                    {
                        pinfo_.Arguments = "\"" + parameter_[1] + "\"";
                        LogtoFile("[DEBUG]Argument " + parameter_[1]);
                        for (var j = 2; j < parameter_.Count; j++)
                        {
                            pinfo_.Arguments += " " + "\"" + parameter_[j] + "\"";
                            if (App.dflag)
                                LogtoFile("[DEBUG]Argument " + parameter_[j]);
                        }
                    }
                    Run_Process(pinfo_, path, RunPath);
                #endregion
                RunOK:
                    var nowin = true;
                    HiroInvoke(() =>
                    {
                        nowin = Application.Current.Windows.Count <= 0;
                    });
                    if (App.mn == null && autoClose && nowin)
                        RunExe("exit()");
                }
                catch (Exception ex)
                {
                    LogError(ex, $"Hiro.Exception.Run{Environment.NewLine}Path: {path}");
                    var nowin = true;
                    HiroInvoke(() =>
                    {
                        nowin = Application.Current.Windows.Count <= 0;
                    });
                    if (App.mn == null && autoClose && nowin)
                        RunExe("exit()");
                    else
                        App.Notify(new Hiro_Notice(Get_Translate("syntax"), 2, source));
                }
            }).Start();
        }

        private static void Run_Process(ProcessStartInfo pinfo, string path, string RunPath)
        {
            pinfo.WorkingDirectory = Path_Prepare("<current>");
            try
            {
                _ = Process.Start(pinfo);
            }
            catch (Exception ex)
            {
                pinfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\users\\" + App.EnvironmentUsername + "\\app";
                pinfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\users\\" + App.EnvironmentUsername + "\\app\\" + pinfo.FileName;
                try
                {
                    _ = Process.Start(pinfo);
                }
                catch
                {
                    foreach (var cmd in App.cmditems)
                    {
                        if (Regex.IsMatch(cmd.Name, RunPath) || Regex.IsMatch(cmd.Name, path))
                        {
                            RunExe(cmd.Command);
                            return;
                        }
                    }
                    string? runstart = FindItemByName(RunPath.Replace("\"", ""), Path_Prepare("<cstart>"));
                    if (runstart != null)
                    {
                        pinfo.FileName = runstart;
                        pinfo.WorkingDirectory = Path_Prepare("<cstart>");
                        pinfo.Arguments = "";
                        try
                        {
                            _ = Process.Start(pinfo);
                            return;
                        }
                        catch
                        {
                        }
                        return;
                    }
                    runstart = FindItemByName(RunPath.Replace("\"", ""), Path_Prepare("<istart>"));
                    if (runstart != null)
                    {
                        pinfo.FileName = runstart;
                        pinfo.WorkingDirectory = Path_Prepare("<cstart>");
                        pinfo.Arguments = "";
                        try
                        {
                            _ = Process.Start(pinfo);
                            return;
                        }
                        catch
                        {
                        }
                    }
                    App.Notify(new(Get_Translate("notfound"), 2, Get_Translate("execute")));
                    LogError(ex, $"Hiro.Exception.Run.Process{Environment.NewLine}");
                }
            }
        }

        public static string? FindItemByName(string Name, string Location)
        {
            if (!Directory.Exists(Location))
            {
                return null;
            }
            string[] filenames = Directory.GetFileSystemEntries(Location);
            foreach (string file in filenames)
            {
                try
                {
                    if (Directory.Exists(file))
                    {
                        string? filename = FindItemByName(Name, file);
                        if (filename != null)
                            return filename;
                    }
                    else
                    {
                        if (Path.GetFileName(file).Equals("desktop.ini"))
                            continue;
                        var filename = Path.GetFileNameWithoutExtension(file);
                        if (filename != null && Regex.IsMatch(file, Name))
                            return file;
                    }
                }
                catch (Exception ex)
                {
                    LogError(ex, $"Hiro.Exception.FindItemByName");
                }
            }
            return null;
        }

        public static List<string> HiroParse(string val)
        {
            if (!val.Contains('(', StringComparison.CurrentCulture))
                return new List<string>();
            val = val[(val.IndexOf("(") + 1)..];
            if (val.EndsWith(")"))
                val = val[0..^1];
            return new List<string>(val.Split(','));
        }

        public static List<string> HiroCmdParse(string val, bool replaceBrackets = true)
        {
            if (val.IndexOf(")") != -1 && replaceBrackets)
            {
                val = val[(val.IndexOf("(") + 1)..];
                if (val.EndsWith(")"))
                    val = val[0..^1];
            }
            val = val.Replace("\\\\", "\uAAA3");
            val = val.Replace("\\\"", "\uAAA1");
            List<string> blank = new();
            var startIndex = val.IndexOf('\"');
            var a = 0;
            while (startIndex != -1)
            {
                var endIndex = val.IndexOf('\"', startIndex + 1);
                if (endIndex == -1)
                {
                    break;
                }
                else
                {
                    var left = startIndex == 0 ? "" : val[..startIndex];
                    var right = endIndex == val.Length ? "" : val[(endIndex + 1)..];
                    var tmp = val.Substring(startIndex + 1, endIndex - startIndex - 1);
                    val = left + "\uAAA5" + a.ToString() + "\uAAA6" + right;
                    blank.Add(tmp);
                    a++;
                    startIndex = val.IndexOf('\"');
                }
            }
            a--;
            var res = replaceBrackets ? new List<string>(val.Split(',')) : new List<string>(val.Split(' '));
            for (var r = res.Count - 1; r >= 0; r--)
            {
                if (a < 0)
                    break;
                res[r] = res[r].Replace("\uAAA1", "\"").Replace("\uAAA3", "\\");
                if (res[r].IndexOf("\uAAA5" + a.ToString() + "\uAAA6") != -1)
                {
                    res[r] = res[r].Replace("\uAAA5" + a.ToString() + "\uAAA6", blank[a]);
                    a--;
                }
            }
            return res;
        }

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, UInt32 dwFlags, UInt32 dwExtraInfo);

        [DllImport("user32.dll")]
        static extern Byte MapVirtualKey(UInt32 uCode, UInt32 uMapType);

        private async static void SetBthState(bool? bluetoothState)
        {
            try
            {
                var access = await Radio.RequestAccessAsync();
                if (access != RadioAccessStatus.Allowed)
                {
                    App.Notify(new Hiro_Notice(Get_Translate("bth") + Get_Translate("dcreject"), 2, Get_Translate("bth")));
                    return;
                }
                Windows.Devices.Bluetooth.BluetoothAdapter adapter = await Windows.Devices.Bluetooth.BluetoothAdapter.GetDefaultAsync();
                if (null != adapter)
                {
                    var btRadio = await adapter.GetRadioAsync();
                    switch (bluetoothState)
                    {
                        case true:
                            await btRadio.SetStateAsync(RadioState.On);
                            App.Notify(new Hiro_Notice(Get_Translate("bth") + Get_Translate("dcon"), 2, Get_Translate("bth")));
                            break;
                        case false:
                            await btRadio.SetStateAsync(RadioState.Off);
                            App.Notify(new Hiro_Notice(Get_Translate("bth") + Get_Translate("dcoff"), 2, Get_Translate("bth")));
                            break;
                        default:
                            App.Notify(new Hiro_Notice(Get_Translate("syntax"), 2, Get_Translate("bth")));
                            break;
                    }
                }
                else
                {
                    App.Notify(new Hiro_Notice(Get_Translate(Get_Translate("bth") + Get_Translate("dcnull")), 2, Get_Translate("bth")));
                }

            }
            catch (Exception ex)
            {
                App.Notify(new Hiro_Notice(Get_Translate("error"), 2));
                LogError(ex, "Hiro.Exception.Bluetooth");
            }
        }

        private async static void SetWiFiState(int? WiFiState, string? Ssid = null, bool omit = false)
        {
            try
            {
                if (await WiFiAdapter.RequestAccessAsync() != WiFiAccessStatus.Allowed)
                {
                    App.Notify(new Hiro_Notice(Get_Translate("wifi") + Get_Translate("dcreject"), 2, Get_Translate("wifi")));
                    return;
                }
                var adapters = await WiFiAdapter.FindAllAdaptersAsync();
                if (adapters.Count <= 0)
                {
                    App.Notify(new Hiro_Notice(Get_Translate("wifi") + Get_Translate("dcnull"), 2, Get_Translate("wifi")));
                    return;
                }
                var adapter = adapters[0];
                if (null == adapter)
                {
                    App.Notify(new Hiro_Notice(Get_Translate("wifi") + Get_Translate("dcnull"), 2, Get_Translate("wifi")));
                    return;
                }
                Radio? ra = null;
                foreach (var radio in await Radio.GetRadiosAsync())
                {
                    if (radio.Kind == RadioKind.WiFi)
                    {
                        ra = radio;
                        break;
                    }
                }
                if (null == ra)
                {
                    App.Notify(new Hiro_Notice(Get_Translate("wifi") + Get_Translate("dcnull"), 2, Get_Translate("wifi")));
                    return;
                }
                switch (WiFiState)
                {
                    case 0:
                        await ra.SetStateAsync(RadioState.Off);
                        App.Notify(new Hiro_Notice(Get_Translate("wifi") + Get_Translate("dcoff"), 2, Get_Translate("wifi")));
                        break;
                    case 1:
                        await ra.SetStateAsync(RadioState.On);
                        App.Notify(new Hiro_Notice(Get_Translate("wifi") + Get_Translate("dcon"), 2, Get_Translate("wifi")));
                        await adapter.ScanAsync();
                        break;
                    case 2:
                        adapter.Disconnect();
                        App.Notify(new Hiro_Notice(Get_Translate("wifi") + Get_Translate("dcdiscon"), 2, Get_Translate("wifi")));
                        break;
                    case 3:
                        await adapter.ScanAsync();
                        if (adapter.NetworkReport.AvailableNetworks.Count > 0)
                        {
                            if (App.dflag)
                                LogtoFile($"adapter.NetworkReport.AvailableNetworks.Count {adapter.NetworkReport.AvailableNetworks.Count}");
                            var connect = true;
                            WiFiAvailableNetwork? savedan = null;
                            foreach (var an in adapter.NetworkReport.AvailableNetworks)
                            {
                                if (Ssid != null && an.Ssid.Equals(Ssid))
                                {
                                    if (savedan == null || !savedan.Ssid.Equals(Ssid))
                                    {
                                        if (App.dflag)
                                            LogtoFile($"Matched Wifi Detected {an.Ssid}");
                                        savedan = an;
                                        if (omit)
                                            break;
                                    }
                                    else
                                    {
                                        if (App.dflag)
                                            LogtoFile($"Multi Wifi Detected {an.Ssid}");
                                        connect = false;
                                        break;
                                    }
                                }
                                else if (an.SecuritySettings.NetworkAuthenticationType.ToString().ToLower().StartsWith("open") && savedan == null)
                                {
                                    if (App.dflag)
                                        LogtoFile($"Open Wifi Detected {an.Ssid}");
                                    savedan = an;
                                    break;
                                }
                            }
                            if (!connect)
                                App.Notify(new Hiro_Notice(Get_Translate("wifimis").Replace("%s", Ssid), 2, Get_Translate("wifi")));
                            else
                            {
                                if (savedan == null)
                                    App.Notify(new Hiro_Notice(Get_Translate("wifina").Replace("%s", Ssid), 2, Get_Translate("wifi")));
                                else
                                {
                                    await adapter.ConnectAsync(savedan, Windows.Devices.WiFi.WiFiReconnectionKind.Automatic);
                                    if (Ssid != null && !savedan.Ssid.ToLower().Equals(Ssid.ToLower()))
                                        App.Notify(new Hiro_Notice(Get_Translate("wifi") + Get_Translate("dcrecon").Replace("%s1", Ssid).Replace("%s2", savedan.Ssid), 2, Get_Translate("wifi")));
                                    else
                                        App.Notify(new Hiro_Notice(Get_Translate("wifi") + Get_Translate("dccon").Replace("%s", savedan.Ssid), 2, Get_Translate("wifi")));
                                }
                            }
                        }
                        else
                            App.Notify(new Hiro_Notice(Get_Translate("wifina"), 2, Get_Translate("wifi")));
                        break;
                    default:
                        App.Notify(new Hiro_Notice(Get_Translate("syntax"), 2, Get_Translate("wifi")));
                        break;
                }
            }
            catch (Exception ex)
            {
                App.Notify(new Hiro_Notice(Get_Translate("error"), 2, Get_Translate("wifi")));
                LogError(ex, $"Hiro.Exception.Wifi");
            }
        }

        private static void CopyDirectory(string srcdir, string desdir)
        {
            if (srcdir.EndsWith("\\"))
                srcdir = srcdir[0..^1];
            if (desdir.ToLower().StartsWith(srcdir.ToLower()))
            {
                App.Notify(new Hiro_Notice(Get_Translate("syntax"), 2, Get_Translate("file")));
                return;
            }
            string desfolderdir = desdir;
            if (!desfolderdir.EndsWith("\\"))
            {
                desfolderdir += "\\";
            }
            CreateFolder(desfolderdir);
            string[] filenames = Directory.GetFileSystemEntries(srcdir);
            foreach (string file in filenames)
            {
                string newdest = desfolderdir + file.Replace(srcdir, "");
                CreateFolder(newdest);
                if (Directory.Exists(file))
                    CopyDirectory(file, newdest);
                else
                    File.Copy(file, newdest);
            }
        }

        #endregion

        #region Windows Hello
        private async static void NewUser(String AccountId, Action success, Action falied, Action cancel)
        {
            var keyCreationResult = await KeyCredentialManager.RequestCreateAsync(AccountId, KeyCredentialCreationOption.FailIfExists);
            if (keyCreationResult.Status == KeyCredentialStatus.CredentialAlreadyExists)
            {
                //label5.Text = "User Already Created.";
                UserConsentVerificationResult consentResult = await UserConsentVerifier.RequestVerificationAsync(AccountId);
                if (consentResult.Equals(UserConsentVerificationResult.Verified))
                {
                    success.Invoke();
                }
                else
                {
                    falied.Invoke();
                }
            }
            else if (keyCreationResult.Status == KeyCredentialStatus.Success)
            {

                var userKey = keyCreationResult.Credential;

                var keyAttestationResult = await userKey.GetAttestationAsync();
                /*KeyCredentialAttestationStatus keyAttestationRetryType = 0;*/

                if (keyAttestationResult.Status == KeyCredentialAttestationStatus.Success)
                {
                    success.Invoke();
                }
                else if (keyAttestationResult.Status == KeyCredentialAttestationStatus.TemporaryFailure)
                {
                    falied.Invoke();
                }
                else if (keyAttestationResult.Status == KeyCredentialAttestationStatus.NotSupported)
                {
                    success.Invoke();
                }
            }
            else if (keyCreationResult.Status == KeyCredentialStatus.UserCanceled ||
                keyCreationResult.Status == KeyCredentialStatus.UserPrefersPassword)
            {
                cancel.Invoke();
            }
        }
        private async void DeleteUser(string AccountId)
        {
            var openKeyResult = await KeyCredentialManager.OpenAsync(AccountId);
            if (openKeyResult.Status == KeyCredentialStatus.Success)
            {
                UserConsentVerificationResult consentResult = await UserConsentVerifier.RequestVerificationAsync(AccountId);
                if (consentResult.Equals(UserConsentVerificationResult.Verified))
                {
                    // continue
                    //label5.Text = "User Verified.\nUser Deleted.";
                    await KeyCredentialManager.DeleteAsync(AccountId);
                }
                else
                {
                    //label5.Text = "User Verifying falied.";
                }
                return;
            }
            if (openKeyResult.Status == KeyCredentialStatus.NotFound)
            {
                //label5.Text = AccountId + " doesn't exist!\n";
                return;
            }
            //label5.Text = "Error Occurred.";
            return;
        }
        public async static void Register(Action success, Action falied, Action cancel)
        {
            var os = Get_OSVersion();
            if (os.IndexOf(".") != -1)
                os = os[..os.IndexOf(".")];
            if (int.TryParse(os, out int a) && a >= 10)
            {
                var keyCredentialAvailable = await KeyCredentialManager.IsSupportedAsync();
                if (!keyCredentialAvailable)
                {
                    success.Invoke();
                    return;
                }
                NewUser("N+@" + App.EnvironmentUsername, success, falied, cancel);
                //Auth(null, "aki-helper@" + textBox1.Text);
            }
            else
            {
                success.Invoke();
                return;
            }
        }
        #endregion

        #region 动画相关

        #region 模糊动画
        public static void Blur_Animation(int direction, bool animation, Control label, Window win, BackgroundWorker? bw = null)
        {
            //0: 25->0 12s  1:0->50 25s 2:0->25 12s 3:50->25 12s
            double start = direction switch
            {
                1 => 0.0,
                2 => 0.0,
                3 => 50.0,
                _ => 25.0
            };
            double end = direction switch
            {
                1 => 50.0,
                2 => 25.0,
                3 => 25.0,
                _ => 0.0
            };
            double time = direction switch
            {
                1 => 500.0,
                _ => 250.0
            };
            if (!animation)
            {
                Set_Animation_Label(end, label, win);
                if (bw != null)
                    bw.RunWorkerAsync();
                return;
            }
            else
            {
                bool comp = win.Width > win.Height;
                double dest = comp ? -end : -end * win.Height / win.Width;
                double stat = comp ? -start : -start * win.Height / win.Width;
                double desl = !comp ? -end : -end * win.Width / win.Height;
                double stal = !comp ? -start : -start * win.Width / win.Height;
                Set_Animation_Label(start, label, win);
                Storyboard? sb = new();
                sb = AddDoubleAnimaton(end, time, label, "Effect.Radius", sb, start);
                sb = AddDoubleAnimaton(win.Height - dest * 2, time, label, "Height", sb, win.Height - stat * 2);
                sb = AddDoubleAnimaton(win.Width - desl * 2, time, label, "Width", sb, win.Width - stal * 2);
                sb = AddThicknessAnimaton(new(desl, dest, 0, 0), time, label, "Margin", sb, new(stal, stat, 0, 0));
                sb.Completed += delegate
                {
                    Set_Animation_Label(end, label, win);
                    if (bw != null)
                        bw.RunWorkerAsync();
                    sb = null;
                };
                sb.Begin();
            }
        }

        public static void Blur_Out(Control ct, BackgroundWorker? bw = null)
        {
            if (!Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
            {
                ct.Effect = new System.Windows.Media.Effects.BlurEffect()
                {
                    Radius = App.blurradius,
                    RenderingBias = System.Windows.Media.Effects.RenderingBias.Performance
                };
                Storyboard? sb = new();
                DoubleAnimation? da = new()
                {
                    From = App.blurradius,
                    To = 0.0,
                    Duration = TimeSpan.FromMilliseconds(App.blursec)
                };
                Storyboard.SetTarget(da, ct);
                Storyboard.SetTargetProperty(da, new PropertyPath("Effect.Radius"));
                sb.Children.Add(da);
                sb.Completed += delegate
                {
                    ct.Effect = null;
                    if (bw != null)
                        bw.RunWorkerAsync();
                    da = null;
                    sb = null;
                };
                sb.Begin();
            }
            else
            {
                if (bw != null)
                    bw.RunWorkerAsync();
            }
        }
        private static void Set_Animation_Label(double rd, Control label, Window win)
        {
            label.Effect = new System.Windows.Media.Effects.BlurEffect()
            {
                Radius = rd,
                RenderingBias = System.Windows.Media.Effects.RenderingBias.Performance
            };
            Thickness tn = label.Margin;
            var WinWidth = win.WindowState == WindowState.Maximized ? win.ActualWidth : win.Width;
            var WinHeight = win.WindowState == WindowState.Maximized ? win.ActualHeight : win.Height;
            if (WinWidth > WinHeight)
            {
                tn.Top = -rd;
                tn.Left = -rd * WinWidth / WinHeight;
            }
            else
            {
                tn.Left = -rd;
                tn.Top = -rd * WinHeight / WinWidth;
            }
            label.Margin = tn;
            label.Width = WinWidth - tn.Left * 2;
            label.Height = WinHeight - tn.Top * 2;
        }
        #endregion

        #region 添加double动画
        public static Storyboard AddDoubleAnimaton(double? to, double mstime, DependencyObject value, string PropertyPath, Storyboard? sb, double? from = null, double decelerationRatio = 0.9, double accelerationRatio = 0)
        {
            sb ??= new();
            DoubleAnimation? da = new();
            if (from != null)
                da.From = from;
            if (to != null)
                da.To = to;
            da.Duration = TimeSpan.FromMilliseconds(mstime);
            da.DecelerationRatio = decelerationRatio;
            da.AccelerationRatio = accelerationRatio;
            Storyboard.SetTarget(da, value);
            Storyboard.SetTargetProperty(da, new PropertyPath(PropertyPath));
            sb.Children.Add(da);
            sb.FillBehavior = FillBehavior.Stop;
            sb.Completed += (sender, args) =>
            {
                da = null;
                sb = null;
            };
            return sb;
        }
        #endregion

        #region 添加thickness动画
        public static Storyboard AddThicknessAnimaton(Thickness? to, double mstime, DependencyObject value, string PropertyPath, Storyboard? sb, Thickness? from = null, double DecelerationRatio = 0.9)
        {
            sb ??= new();
            ThicknessAnimation? da = new();
            if (from != null)
                da.From = from;
            if (to != null)
                da.To = to;
            da.Duration = TimeSpan.FromMilliseconds(mstime);
            da.DecelerationRatio = DecelerationRatio;
            Storyboard.SetTarget(da, value);
            Storyboard.SetTargetProperty(da, new PropertyPath(PropertyPath));
            sb.Children.Add(da);
            sb.FillBehavior = FillBehavior.Stop;
            sb.Completed += delegate
            {
                da = null;
                sb = null;
            };
            return sb;
        }
        #endregion 

        #region 添加Color动画
        public static Storyboard AddColorAnimaton(Color to, double mstime, DependencyObject value, string PropertyPath, Storyboard? sb, Color? from = null)
        {
            sb ??= new();
            ColorAnimation? da;
            if (from != null)
                da = new((Color)from, to, TimeSpan.FromMilliseconds(mstime));
            else
                da = new(to, TimeSpan.FromMilliseconds(mstime));
            da.DecelerationRatio = 0.9;
            Storyboard.SetTarget(da, value);
            Storyboard.SetTargetProperty(da, new PropertyPath(PropertyPath));
            sb.Children.Add(da);
            sb.FillBehavior = FillBehavior.Stop;
            sb.Completed += delegate
            {
                da = null;
                sb = null;
            };
            return sb;
        }
        #endregion

        #region 增强动效
        public static Storyboard AddPowerAnimation(int Direction, FrameworkElement value, Storyboard? sb, double? from = null, double? to = null)
        {
            sb ??= new();
            var th1 = value.Margin;
            var th2 = value.Margin;
            if (to != null && from != null)
            {
                if (Direction == 0)
                {
                    th1.Left += (double)from;
                    th2.Left += (double)to;
                }
                if (Direction == 1)
                {
                    th1.Top += (double)from;
                    th2.Top += (double)to;
                }
                if (Direction == 2)
                {
                    th1.Right += (double)from;
                    th2.Right += (double)to;
                }
                if (Direction == 3)
                {
                    th1.Bottom += (double)from;
                    th2.Bottom += (double)to;
                }
                AddThicknessAnimaton(th2, 450, value, "Margin", sb, th1);
            }
            if (to != null && from == null)
            {
                if (Direction == 0)
                {
                    th2.Left += (double)to;
                }
                if (Direction == 1)
                {
                    th2.Top += (double)to;
                }
                if (Direction == 2)
                {
                    th2.Right += (double)to;
                }
                if (Direction == 3)
                {
                    th2.Bottom += (double)to;
                }
                AddThicknessAnimaton(th2, 450, value, "Margin", sb, null);
            }
            if (to == null && from != null)
            {
                if (Direction == 0)
                {
                    th1.Left += (double)from;
                }
                if (Direction == 1)
                {
                    th1.Top += (double)from;
                }
                if (Direction == 2)
                {
                    th1.Right += (double)from;
                }
                if (Direction == 3)
                {
                    th1.Bottom += (double)from;
                }
                AddThicknessAnimaton(null, 450, value, "Margin", sb, th1);
            }
            AddDoubleAnimaton(null, 350, value, "Opacity", sb, 0);
            return sb;
        }
        #endregion

        #endregion

        #region 获取命令翻译
        public static string Get_CMD_Translation(string cmd)
        {
            string a = "";
            int b = 1;
            string c = Read_Ini(App.LangFilePath, "Command", b.ToString() + "_cmd", " ");
            string d = Read_Ini(App.LangFilePath, "Command", b.ToString() + "_content", " ").Replace("\\n", Environment.NewLine);
            while (!c.Equals(" ") && !d.Equals(" "))
            {
                if (cmd.ToLower().StartsWith(c.ToLower()))
                {
                    a = d;
                    break;
                }
                b++;
                c = Read_Ini(App.LangFilePath, "Command", b.ToString() + "_cmd", " ");
                d = Read_Ini(App.LangFilePath, "Command", b.ToString() + "_content", " ").Replace("\\n", Environment.NewLine);
            }
            return a;
        }
        #endregion

        #region 检查更新
        public static string GetWebContent(string url, bool save = false, string? savepath = null)
        {
            HttpRequestMessage request = new(HttpMethod.Get, url);
            request.Headers.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.71 Safari/537.36");
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
                throw new Exception(Get_Translate("webnotinitial"));
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
                                LogError(ex, $"Hiro.Exception.Web.Get");
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
                    LogError(new ArgumentNullException(), $"Hiro.Exception.Web.Respose");
                    return Get_Translate("error");
                }
            }
            catch (Exception ex)
            {
                LogError(ex, $"Hiro.Exception.Web.HttpClient");
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region API函数声明

        #region 读文件
        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(byte[] section, byte[] key, byte[] val, string filePath);
        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern int GetPrivateProfileString(byte[] section, byte[] key, byte[] def, byte[] retVal, int size, string filePath);
        #endregion

        #region 窗口拖动
        [DllImport("user32.dll")]//拖动无窗体的控件
        static extern bool ReleaseCapture();
        public static bool ReleaseCaptureImpl()
        {
            return ReleaseCapture();
        }
        [DllImport("user32.dll")]
        static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        #endregion

        #region 设置壁纸

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        public enum Style : int
        {
            Fill,
            Fit,
            Span,
            Stretch,
            Tile,
            Center
        }
        #endregion

        #region 获取壁纸

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool SystemParametersInfo(uint uAction, uint uParam, StringBuilder lpvParam, uint init);
        public static bool GetSystemParametersInfo(uint uAction, uint uParam, StringBuilder lpvParam, uint init)
        {
            return SystemParametersInfo(uAction, uParam, lpvParam, init);
        }

        #endregion

        #region 获取用户头像
        [DllImport("shell32.dll", EntryPoint = "#261", CharSet = CharSet.Unicode, PreserveSig = false)]
        static extern void GetUserTilePath(
          string username,
          UInt32 whatever, // 0x80000000
          StringBuilder picpath, int maxLength);

        public static string GetUserTilePath(string username)
        {   // username: use null for current user
            var sb = new StringBuilder(1000);
            GetUserTilePath(username, 0x80000000, sb, sb.Capacity);
            return sb.ToString();
        }
        #endregion

        #region 设置窗口阴影
        private const int CS_DropSHADOW = 0x20000;
        private const int GCL_STYLE = (-26);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SetClassLong(IntPtr hwnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int GetClassLong(IntPtr hwnd, int nIndex);

        public static void SetShadow(IntPtr hwnd)
        {
            _ = SetClassLong(hwnd, GCL_STYLE, GetClassLong(hwnd, GCL_STYLE) | CS_DropSHADOW);
        }

        #endregion

        #region 隐藏鼠标 0/1 隐藏/显示
        [DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = CharSet.Auto)]
        static extern void ShowCursor(int status);

        public static void SetCursor(int status)
        {
            ShowCursor(status);
        }
        #endregion

        #endregion

        #region 窗口拖动
        public static void Move_Window(IntPtr handle)
        {
            //拖动窗体
            ReleaseCapture();
            SendMessage(handle, 0x0112, 0xF010 + 0x0002, 0);
        }
        #endregion

        #region 闹钟功能

        public static void OK_Alarm(int id)
        {
            if (App.dflag)
                LogtoFile("[DEBUG]Alarm ID " + id.ToString());
            if (id > -1)
            {
                DateTimeFormatInfo dtFormat = new()
                {
                    ShortDatePattern = "yyyy/MM/dd HH:mm:ss"
                };
                try
                {
                    DateTime dt = Convert.ToDateTime(App.scheduleitems[id].Time, dtFormat);
                    switch (App.scheduleitems[id].re)
                    {
                        case -2.0:
                            break;
                        case -1.0:
                            App.scheduleitems[id].Time = dt.AddDays(1.0).ToString("yyyy/MM/dd HH:mm:ss");
                            Write_Ini(App.sconfig, (id + 1).ToString(), "Time", App.scheduleitems[id].Time);
                            break;
                        case 0.0:
                            App.scheduleitems[id].Time = dt.AddDays(7.0).ToString("yyyy/MM/dd HH:mm:ss");
                            Write_Ini(App.sconfig, (id + 1).ToString(), "Time", App.scheduleitems[id].Time);
                            break;
                        default:
                            App.scheduleitems[id].Time = dt.AddDays(Math.Abs(App.scheduleitems[id].re)).ToString("yyyy/MM/dd HH:mm:ss");
                            Write_Ini(App.sconfig, (id + 1).ToString(), "Time", App.scheduleitems[id].Time);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LogError(ex, "Hiro.Exception.Alarm.OK");
                }

            }
        }

        public static void Delete_Alarm(int id)
        {
            var inipath = App.sconfig;
            if (id > -1)
            {
                while (id < App.scheduleitems.Count - 1)
                {
                    App.scheduleitems[id].Name = App.scheduleitems[id + 1].Name;
                    App.scheduleitems[id].Command = App.scheduleitems[id + 1].Command;
                    App.scheduleitems[id].Time = App.scheduleitems[id + 1].Time;
                    App.scheduleitems[id].re = App.scheduleitems[id + 1].re;
                    Write_Ini(inipath, (id + 1).ToString(), "name", Read_Ini(inipath, (id + 2).ToString(), "name", " "));
                    Write_Ini(inipath, (id + 1).ToString(), "command", Read_Ini(inipath, (id + 2).ToString(), "command", " "));
                    Write_Ini(inipath, (id + 1).ToString(), "time", Read_Ini(inipath, (id + 2).ToString(), "time", " "));
                    Write_Ini(inipath, (id + 1).ToString(), "repeat", Read_Ini(inipath, (id + 2).ToString(), "repeat", " "));
                    id++;
                    System.Windows.Forms.Application.DoEvents();
                }
                Write_Ini(inipath, (id + 1).ToString(), "Name", " ");
                Write_Ini(inipath, (id + 1).ToString(), "Command", " ");
                Write_Ini(inipath, (id + 1).ToString(), "Time", " ");
                Write_Ini(inipath, (id + 1).ToString(), "repeat", " ");
                App.scheduleitems.RemoveAt(id);
            }
            else
                App.Notify(new Hiro_Notice(Get_Translate("alarmmissing"), 2, Get_Translate("schedule")));

        }

        public static void Delay_Alarm(int id)
        {
            if (id > -1)
                App.scheduleitems[id].Time = DateTime.Now.AddMinutes(5.0).ToString("yyyy/MM/dd HH:mm:ss");
            else
                App.Notify(new Hiro_Notice(Get_Translate("alarmmissing"), 2, Get_Translate("schedule")));
        }
        #endregion

        #region 数字转换
        public static int ConvertInt(string str, int Default = 0)
        {
            try
            {
                return int.Parse(str);
            }
            catch
            {
                return Default;
            }
        }
        #endregion

        #region 设置开机自启
        public static void Set_Autorun(bool boo)
        {
            if (boo)
            {
                try
                {
                    string strName = "\"" + Path_Prepare(Hiro_Resources.ApplicationPath) + "\"";//获取要自动运行的应用程序名
                    Microsoft.Win32.RegistryKey? registry = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//检索指定的子项
                    registry ??= Microsoft.Win32.Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");//则创建指定的子项
                    registry.SetValue("Hiro_Autostart", strName + " silent");//设置该子项的新的“键值对”
                    Write_Ini(App.dconfig, "Config", "AutoRun", "1");
                    LogtoFile("[HIROWEGO]Enable Autorun");
                }
                catch (Exception ex)
                {
                    LogError(ex, "Hiro.Exception.Config.Autorun");
                }
            }
            else
            {
                Microsoft.Win32.RegistryKey? registry = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//读取指定的子项
                if (registry == null)//若指定的子项不存在
                    return;
                registry = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");//则创建指定的子项
                registry.DeleteValue("Hiro_Autostart", false);//删除指定“键名称”的键/值对
                Write_Ini(App.dconfig, "Config", "AutoRun", "0");
                LogtoFile("[HIROWEGO]Disable Autorun");
            }
        }
        #endregion

        #region 新建完全限定路径文件夹
        public static bool CreateFolder(string path)
        {
            int pos = path.IndexOf("\\") + 1;
            string vpath;
            DirectoryInfo? di;
            try
            {
                while (pos > 0)
                {
                    vpath = path[..pos];
                    pos = path.IndexOf("\\", pos) + 1;
                    di = new DirectoryInfo(vpath);
                    if (!di.Exists)
                        di.Create();
                }
            }
            catch (Exception ex)
            {
                LogError(ex, $"Hiro.Exception.Directory.Create");
                return false;
            }
            return true;

        }
        #endregion

        #region 获取Windows版本
        public static string Get_OSVersion()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().OSVersion.Trim();
        }

        #endregion

        #region 获取系统主题色

        public static void IntializeColorParameters()
        {
            Color mAppAccentColor = (Color)ColorConverter.ConvertFromString("#00C4FF");
            if (!Read_Ini(App.dconfig, "Config", "LockColor", "default").Equals("default"))
            {
                try
                {
                    mAppAccentColor = (Color)ColorConverter.ConvertFromString(Read_Ini(App.dconfig, "Config", "LockColor", "#00C4FF"));
                }
                catch (Exception ex)
                {
                    LogError(ex, "Hiro.Exception.System.Color");
                }
            }
            else
            {
                mAppAccentColor = GetThemeColor();
            }
            App.AppAccentColor = mAppAccentColor;
            App.AppForeColor = Get_ForeColor(mAppAccentColor, Read_Ini(App.dconfig, "Config", "Reverse", "0").Equals("1"));
            LogtoFile("[HIROWEGO]Accent Color: " + App.AppAccentColor.ToString());
            LogtoFile("[HIROWEGO]Fore Color: " + App.AppForeColor.ToString());
        }

        public static Color Get_ForeColor(Color AccentColor, bool Reverse = false)
        {
            double luminance = (0.299 * AccentColor.R + 0.587 * AccentColor.G + 0.114 * AccentColor.B) / 255;
            if (luminance > 0.5)
            {
                return Reverse ? Colors.White : Colors.Black;
            }
            else
            {
                return Reverse ? Colors.Black : Colors.White;
            }
        }

        [DllImport("uxtheme.dll", EntryPoint = "#95")]
        static extern uint GetImmersiveColorFromColorSetEx(uint dwImmersiveColorSet, uint dwImmersiveColorType, bool bIgnoreHighContrast, uint dwHighContrastCacheMode);
        [DllImport("uxtheme.dll", EntryPoint = "#96")]
        static extern uint GetImmersiveColorTypeFromName(IntPtr pName);
        [DllImport("uxtheme.dll", EntryPoint = "#98")]
        static extern int GetImmersiveUserColorSetPreference(bool bForceCheckRegistry, bool bSkipCheckOnFail);
        // Get theme color
        public static Color GetThemeColor()
        {
            var colorSetEx = GetImmersiveColorFromColorSetEx(
                (uint)GetImmersiveUserColorSetPreference(false, false),
                GetImmersiveColorTypeFromName(Marshal.StringToHGlobalUni("ImmersiveStartSelectionBackground")),
                false, 0);

            var colour = Color.FromArgb((byte)((0xFF000000 & colorSetEx) >> 24), (byte)(0x000000FF & colorSetEx),
                (byte)((0x0000FF00 & colorSetEx) >> 8), (byte)((0x00FF0000 & colorSetEx) >> 16));

            return colour;
        }
        #endregion

        #region 颜色处理
        public static Color Color_Multiple(Color origin, int Multiple)
        {
            Multiple = (Multiple > 255) ? 255 : (Multiple < 0) ? 0 : Multiple;
            double new_R = origin.R * Multiple / 255;
            double new_B = origin.B * Multiple / 255;
            double new_G = origin.G * Multiple / 255;
            return Color.FromRgb((byte)new_R, (byte)new_G, (byte)new_B);
        }

        public static Color Color_Transparent(Color origin, int val)
        {
            return Color.FromArgb((byte)val, origin.R, origin.G, origin.B);
        }
        #endregion

        #region 图像压缩
        /// <summary>
        /// 压缩图片至200 Kb以下
        /// </summary>
        /// <param name="img">图片</param>
        /// <param name="format">图片格式</param>
        /// <param name="targetLen">压缩后大小</param>
        /// <param name="srcLen">原始大小</param>
        /// <returns>压缩后的图片</returns>
        public static System.Drawing.Image ZipImage(System.Drawing.Image img, ImageFormat format, long targetLen, long srcLen = 0)
        {
            //设置大小偏差幅度 10kb
            const long nearlyLen = 10240;
            //内存流  如果参数中原图大小没有传递 则使用内存流读取
            var ms = new MemoryStream();
            if (0 == srcLen)
            {
                img.Save(ms, format);
                srcLen = ms.Length;
            }

            //单位 由Kb转为byte 若目标大小高于原图大小，则满足条件退出
            targetLen *= 1024;
            if (targetLen > srcLen)
            {
                ms.SetLength(0);
                ms.Position = 0;
                img.Save(ms, format);
                img = System.Drawing.Image.FromStream(ms);
                return img;
            }

            //获取目标大小最低值
            var exitLen = targetLen - nearlyLen;

            //初始化质量压缩参数 图像 内存流等
            var quality = (long)Math.Floor(100.00 * targetLen / srcLen);
            var parms = new EncoderParameters(1);

            //获取编码器信息
            ImageCodecInfo? formatInfo = null;
            var encoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo icf in encoders)
            {
                if (icf.FormatID == format.Guid)
                {
                    formatInfo = icf;
                    break;
                }
            }

            //使用二分法进行查找 最接近的质量参数
            long startQuality = quality;
            long endQuality = 100;
            quality = (startQuality + endQuality) / 2;

            while (true)
            {
                //设置质量
                parms.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

                //清空内存流 然后保存图片
                ms.SetLength(0);
                ms.Position = 0;
                if (formatInfo != null)
                    img.Save(ms, formatInfo, parms);

                //若压缩后大小低于目标大小，则满足条件退出
                if (ms.Length >= exitLen && ms.Length <= targetLen)
                {
                    break;
                }
                else if (startQuality >= endQuality) //区间相等无需再次计算
                {
                    break;
                }
                else if (ms.Length < exitLen) //压缩过小,起始质量右移
                {
                    startQuality = quality;
                }
                else //压缩过大 终止质量左移
                {
                    endQuality = quality;
                }

                //重新设置质量参数 如果计算出来的质量没有发生变化，则终止查找。这样是为了避免重复计算情况{start:16,end:18} 和 {start:16,endQuality:17}
                var newQuality = (startQuality + endQuality) / 2;
                if (newQuality == quality)
                {
                    break;
                }
                quality = newQuality;
                //Console.WriteLine("start:{0} end:{1} current:{2}", startQuality, endQuality, quality);
            }
            img = System.Drawing.Image.FromStream(ms);
            return img;
        }

        /// <summary>
        ///获取图片格式
        /// </summary>
        /// <param name="img">图片</param>
        /// <returns>默认返回JPEG</returns>
        public static ImageFormat GetImageFormat(System.Drawing.Image img)
        {
            if (img.RawFormat.Equals(ImageFormat.Jpeg))
            {
                return ImageFormat.Jpeg;
            }
            if (img.RawFormat.Equals(ImageFormat.Gif))
            {
                return ImageFormat.Gif;
            }
            if (img.RawFormat.Equals(ImageFormat.Png))
            {
                return ImageFormat.Png;
            }
            if (img.RawFormat.Equals(ImageFormat.Bmp))
            {
                return ImageFormat.Bmp;
            }
            return ImageFormat.Jpeg;//根据实际情况选择返回指定格式还是null
        }

        /// <summary>
        /// 不管多大的图片都能在指定大小picturebox控件中显示
        /// </summary>
        /// <param name="bitmap">图片</param>
        /// <param name="destHeight">picturebox控件高</param>
        /// <param name="destWidth">picturebox控件宽</param>
        /// <returns></returns>
        public static System.Drawing.Image ZoomImage(System.Drawing.Image bitmap, int destHeight, int destWidth)
        {
            try
            {
                System.Drawing.Image sourImage = bitmap;
                int width = 0, height = 0;
                //按比例缩放             
                int sourWidth = sourImage.Width;
                int sourHeight = sourImage.Height;
                if (sourHeight > destHeight || sourWidth > destWidth)
                {
                    if ((sourWidth * destHeight) > (sourHeight * destWidth))
                    {
                        width = destWidth;
                        height = (destWidth * sourHeight) / sourWidth;
                    }
                    else
                    {
                        height = destHeight;
                        width = (sourWidth * destHeight) / sourHeight;
                    }
                }
                else
                {
                    width = sourWidth;
                    height = sourHeight;
                }
                System.Drawing.Bitmap destBitmap = new(destWidth, destHeight);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(destBitmap);
                g.Clear(System.Drawing.Color.Transparent);
                //设置画布的描绘质量           
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(sourImage, new System.Drawing.Rectangle((destWidth - width) / 2, (destHeight - height) / 2, width, height), 0, 0, sourImage.Width, sourImage.Height, System.Drawing.GraphicsUnit.Pixel);
                //g.DrawImage(sourImage, new Rectangle(0, 0, destWidth, destHeight), new Rectangle(0, 0, sourImage.Width, sourImage.Height), GraphicsUnit.Pixel);
                g.Dispose();
                //设置压缩质量       
                EncoderParameters encoderParams = new();
                long[] quality = new long[1];
                quality[0] = 100;
                EncoderParameter encoderParam = new(System.Drawing.Imaging.Encoder.Quality, quality);
                encoderParams.Param[0] = encoderParam;
                sourImage.Dispose();
                return destBitmap;
            }
            catch (Exception ex)
            {
                LogError(ex, "Hiro.Exception.Utils.ZoomImage");
                return bitmap;
            }
        }


        #endregion

        #region 热键相关

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public enum HotkeyModifiers
        {
            MOD_NONE = 0x0,
            MOD_ALT = 0x1,
            MOD_CONTROL = 0x2,
            MOD_SHIFT = 0x4,
            MOD_WIN = 0x8
        }

        public static int RegisterKey(uint modi, System.Windows.Input.Key id, int cid)
        {
            int kid = keyid;
            keyid += 2;
            var vk = System.Windows.Input.KeyInterop.VirtualKeyFromKey(id);
            if (!RegisterHotKey(App.WND_Handle, kid, modi, (uint)vk))
            {
                string msg = "";
                IntPtr tempptr = IntPtr.Zero;
                int sa = Marshal.GetLastWin32Error();
                _ = FormatMessage(0x1300, ref tempptr, sa, 0, ref msg, 255, ref tempptr);
                RunExe("notify(" + Get_Translate("regfailed").Replace("%n", sa.ToString()) + ",2)");
                LogError(new NotSupportedException(), $"Hiro.Exception.Hotkey.Register{Environment.NewLine}Extra: {sa} - {msg.Replace(Environment.NewLine, "")}");
            }
            App.vs.Add(kid);
            App.vs.Add(cid);
            return kid;
        }

        public static bool UnregisterKey(int id)
        {
            if (id < 0)
                return false;
            bool a = UnregisterHotKey(App.WND_Handle, App.vs[id]);
            App.vs.RemoveAt(id);
            App.vs.RemoveAt(id);
            if (!a)
            {
                string msg = "";
                IntPtr tempptr = IntPtr.Zero;
                int sa = Marshal.GetLastWin32Error();
                _ = FormatMessage(0x1300, ref tempptr, sa, 0, ref msg, 255, ref tempptr);
                RunExe("notify(" + Get_Translate("unregfailed").Replace("%n", sa.ToString()) + ",2)");
                LogError(new NotSupportedException(), $"Hiro.Exception.Hotkey.Unregister{Environment.NewLine}Extra: {sa} - {msg.Replace(Environment.NewLine, "")}");
            }
            else
                LogtoFile("[REGISTER]Successfully unregistered.");
            return a;
        }

        public static int Index_Modifier(bool direction, int val)
        {
            //all provide!
            //alt - 1
            //shft - 2
            //ctrl - 4
            //win - 8
            //shit+alt - 3
            //ctrl +alt - 5
            //ctrl+shift = 6
            //win + alt = 9
            //win + shift = 10
            //win + ctrl = 12

            //ctrl+alt+shift = 7
            //win+alt+shift = 11
            //win+ctrl+alt = 13

            //win+ctrl+shift+alt = 15
            int[] mo = { 0, 1, 2, 4, 8, 3, 5, 6, 9, 10, 12, 7, 11, 13, 15 };
            if (direction)
            {
                if (val > -1 && val < mo.Length)
                    return mo[val];
            }
            else
            {
                for (int mos = 0; mos < mo.Length; mos++)
                {
                    if (mo[mos] == val)
                    {
                        return mos;
                    }
                }
            }
            return 0;
        }
        public static int Index_vKey(bool direction, int val)
        {
            int[] mo = { 0, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69,
                                34, 35, 36, 37, 38, 39, 40, 41, 42, 43,
                                90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101,
                                18, 13,
                                23, 24, 25, 26};
            if (direction)
            {
                if (val > -1 && val < mo.Length)
                    return mo[val];
            }
            else
            {
                for (int mos = 0; mos < mo.Length; mos++)
                {
                    if (mo[mos] == val)
                    {
                        return mos;
                    }
                }
            }
            return 0;
        }

        public static int FindHotkeyById(int id)
        {
            for (int vsi = 0; vsi < App.vs.Count - 1; vsi += 2)
            {
                if (App.vs[vsi + 1] == id)
                    return vsi;
            }
            return -1;
        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        extern static int FormatMessage(int flag, ref IntPtr source, int msgid, int langid, ref string buf, int size, ref IntPtr args);
        #endregion

        #region 检测全屏程序
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(HandleRef hWnd, [In, Out] ref RECT rect);

        public static bool IsForegroundFullScreen()
        {
            return IsForegroundFullScreen(null);
        }

        public static bool IsForegroundFullScreen(System.Windows.Forms.Screen? screen)
        {
            screen ??= System.Windows.Forms.Screen.PrimaryScreen;
            RECT rect = new();
            IntPtr hWnd = (IntPtr)GetForegroundWindow();
            GetWindowRect(new HandleRef(null, hWnd), ref rect);
            return screen != null && screen.Bounds.Width == (rect.right - rect.left) && screen.Bounds.Height == (rect.bottom - rect.top);
        }
        #endregion

        #region 获取文件名
        public static string GetFileName(string file, bool ext = false)
        {
            return ext ? System.IO.Path.GetFileName(file) : System.IO.Path.GetFileNameWithoutExtension(file);
        }
        #endregion

        #region 计算MD5
        public static string GetMD5(string file)
        {
            if (!System.IO.File.Exists(file))
                return string.Empty;
            using (FileStream fs = File.OpenRead(file))
            {
                using (var crypto = System.Security.Cryptography.MD5.Create())
                {
                    var md5Hash = crypto.ComputeHash(fs);
                    return DeleteUnVisibleChar(BitConverter.ToString(md5Hash));
                }
            }
        }
        #endregion

        #region 个人资料操作
        public static string Login(string account, string pwd, bool token = false, string? saveto = null)
        {
            var url = "https://id.rexio.cn/login.php";
            try
            {
                if (App.hc == null)
                    throw new Exception(Get_Translate("webnotinitial"));
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
                request.Headers.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.71 Safari/537.36");
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
                            LogError(ex, "Hiro.Exception.Login.Save");
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
                LogError(ex, "Hiro.Exception.Login");
                return "error";
            }
        }

        public static void Logout()
        {
            App.Logined = false;
            App.LoginedToken = string.Empty;
            App.Username = App.EnvironmentUsername;
            App.CustomUsernameFlag = 0;
            Write_Ini(App.dconfig, "Config", "Token", string.Empty);
            Write_Ini(App.dconfig, "Config", "AutoLogin", "0");
            Write_Ini(App.dconfig, "Config", "CustomUser", "0");
            Write_Ini(App.dconfig, "Config", "CustomName", string.Empty);
            Write_Ini(App.dconfig, "Config", "CustomSign", string.Empty);
            Write_Ini(App.dconfig, "Config", "UserAvatarStyle", string.Empty);
        }

        public static string UploadProfileImage(string file, string user, string token, string type)
        {
            var url = "https://hiro.rexio.cn/Chat/upload.php";
            try
            {
                if (App.hc == null)
                    throw new Exception(Get_Translate("webnotinitial"));
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
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"md5\"" + Enter + Enter + "" + GetMD5(file).Replace("-", "") + "" + Enter + "--" + boundary + "--"
                    );
                byte[] ndata = new byte[send.Length + bytebuffer.Length + eof.Length];
                send.CopyTo(ndata, 0);
                bytebuffer.CopyTo(ndata, send.Length);
                eof.CopyTo(ndata, send.Length + bytebuffer.Length);
                HttpRequestMessage request = new(HttpMethod.Post, url);
                request.Headers.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.71 Safari/537.36");
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
                LogError(ex, "Hiro.Exception.Update.Profile");
                return "error";
            }
        }

        public static string UploadProfileSettings(string user, string token, string name, string signature, string avatar, string iavatar, string back, string method = "update", string? saveto = null)
        {
            var url = "https://hiro.rexio.cn/Chat/update.php";
            try
            {
                if (App.hc == null)
                    throw new Exception(Get_Translate("webnotinitial"));
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
                request.Headers.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.71 Safari/537.36");
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
                            LogError(ex, "Hiro.Exception.Update.Profile.Settings.Save");
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
                LogError(ex, "Hiro.Exception.Update.Profile.Settings");
                return "error";
            }
        }

        public static string SyncProfile(string user, string token)
        {
            var url = "https://hiro.rexio.cn/Chat/sync.php";
            try
            {
                if (App.hc == null)
                    throw new Exception(Get_Translate("webnotinitial"));
                string boundary = DateTime.Now.Ticks.ToString("X");
                string Enter = "\r\n";
                byte[] send = Encoding.UTF8.GetBytes(
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"user\"" + Enter + Enter + "" + user + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"token\"" + Enter + Enter + "" + token + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"version\"" + Enter + Enter + "" + version + "" + Enter + "--" + boundary + "--"
                    );
                HttpRequestMessage request = new(HttpMethod.Post, url);
                request.Headers.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.71 Safari/537.36");
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
                        Write_Ini(App.dconfig, "Config", "CustomUser", "2");
                        Write_Ini(App.dconfig, "Config", "CustomName", Read_Ini(saveto, "Profile", "Name", string.Empty));
                        Write_Ini(App.dconfig, "Config", "CustomSign", Read_Ini(saveto, "Profile", "Sign", string.Empty));
                        Write_Ini(App.dconfig, "Config", "UserAvatarStyle", Read_Ini(saveto, "Profile", "Avatar", "1"));
                        App.Username = Read_Ini(saveto, "Profile", "Name", string.Empty);
                        App.CustomUsernameFlag = 1;
                        var usrAvatar = "<hiapp>\\images\\avatar\\" + user + ".hap";
                        var usrBack = "<hiapp>\\images\\background\\" + user + ".hpp";
                        Write_Ini(App.dconfig, "Config", "UserAvatar", usrAvatar);
                        Write_Ini(App.dconfig, "Config", "UserBackground", usrBack);
                        CreateFolder(Path_Prepare(usrAvatar));
                        CreateFolder(Path_Prepare(usrBack));
                        if (File.Exists(Path_Prepare(usrAvatar)))
                            File.Delete(Path_Prepare(usrAvatar));
                        if (File.Exists(Path_Prepare(usrBack)))
                            File.Delete(Path_Prepare(usrBack));
                        GetWebContent(Read_Ini(saveto, "Profile", "Iavavtar", "https://hiro.rexio.cn/Chat/Profile/" + user + "/" + user + "." + version + ".hap"), true, Path_Prepare(usrAvatar));
                        GetWebContent(Read_Ini(saveto, "Profile", "Back", "https://hiro.rexio.cn/Chat/Profile/" + user + "/" + user + "." + version + ".hpp"), true, Path_Prepare(usrBack));
                        if (File.Exists(saveto))
                            File.Delete(saveto);
                        return "success";
                    }
                    catch (Exception ex)
                    {
                        LogError(ex, "Hiro.Exception.Update.Profile.Sync.Save");
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
                LogError(ex, "Hiro.Exception.Update.Profile.Sync");
                return "error";
            }
        }

        public static string SendMsg(string user, string token, string to, string content)
        {
            var url = "https://hiro.rexio.cn/Chat/send.php";
            try
            {
                if (App.hc == null)
                    throw new Exception(Get_Translate("webnotinitial"));
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
                request.Headers.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.71 Safari/537.36");
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
                LogError(ex, "Hiro.Exception.Chat.Send");
                return "error";
            }
        }


        public static string GetChat(string user, string token, string to, string saveto)
        {
            var url = "https://hiro.rexio.cn/Chat/log.php";
            try
            {
                if (App.hc == null)
                    throw new Exception(Get_Translate("webnotinitial"));
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
                request.Headers.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.71 Safari/537.36");
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
                        LogError(ex, "Hiro.Exception.Chat.Get.Save");
                        throw new Exception(ex.Message);
                    }
                }
                else
                    return "null";
            }
            catch (Exception ex)
            {
                LogError(ex, "Hiro.Exception.Chat.Get");
                return "error";
            }
        }
        #endregion

        #region 获取拓展名
        public static string GetExtensionName(string file)
        {
            var d = file.IndexOf(".");
            var s = file.IndexOf("\\");
            if (d <= s)
                return string.Empty;
            else
            {
                if (d >= file.Length - 1)
                    return string.Empty;
                else
                    return file.Substring(d + 1);
            }
        }
        #endregion

        #region 非占用读取图片
        public static BitmapImage? GetBitmapImage(string fileName)
        {
            if (File.Exists(fileName) == false)
                return null;
            BitmapImage bitmapimage = new();
            bitmapimage.BeginInit();
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            switch (Read_Ini(App.dconfig, "Config", "ImageRead", "1"))
            {
                case "0":
                    {
                        bitmapimage.StreamSource = new MemoryStream(File.ReadAllBytes(fileName));
                        break;
                    }

                default:
                    {
                        bitmapimage.UriSource = new Uri(fileName);
                        break;
                    }
            }

            bitmapimage.EndInit();
            bitmapimage.Freeze();
            return bitmapimage;
        }

        #endregion

        #region 杂项功能
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

        public static int GetRandomUnusedPort()
        {
            var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
            listener.Start();
            var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        #region 保持窗口最前

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern IntPtr SetCapture(IntPtr hWnd);
        public static IntPtr SetCaptureImpl(IntPtr hWnd)
        {
            return SetCapture(hWnd);
        }

        public static void SetWindowToForegroundWithAttachThreadInput(Window window)
        {
            var interopHelper = new System.Windows.Interop.WindowInteropHelper(window);
            var thisWindowThreadId = GetWindowThreadProcessId(interopHelper.Handle, IntPtr.Zero);
            var currentForegroundWindow = GetForegroundWindow();
            var currentForegroundWindowThreadId = GetWindowThreadProcessId(currentForegroundWindow, IntPtr.Zero);
            AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, true);
            window.Activate();
        }

        public static void CancelWindowToForegroundWithAttachThreadInput(Window window)
        {
            var interopHelper = new System.Windows.Interop.WindowInteropHelper(window);
            var thisWindowThreadId = GetWindowThreadProcessId(interopHelper.Handle, IntPtr.Zero);
            var currentForegroundWindow = GetForegroundWindow();
            var currentForegroundWindowThreadId = GetWindowThreadProcessId(currentForegroundWindow, IntPtr.Zero);
            AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, false);
        }

        #endregion

        #endregion

        #region 加密解密

        internal static int GetNumInt(byte[] b)
        {
            var str = Encoding.UTF8.GetString(b, 0, 8);
            return Convert.ToInt32(str, 16);
        }

        internal static byte[] GetNumBytes(int length)
        {
            var str = length.ToString("X8");
            return Encoding.UTF8.GetBytes(str);
        }

        public static byte[] EncryptFile(byte[] array, string key, string filename)
        {
            return Encrypt(GetEncryptPrefix(filename).Concat(array).ToArray(), key);
        }

        private static byte[] GetEncryptPrefix(string filename)
        {
            return GetNumBytes(Encoding.UTF8.GetBytes(filename).Length).Concat(Encoding.UTF8.GetBytes(filename)).ToArray();
        }

        public static byte[] DecryptFile(byte[] array, string key, out string filename)
        {
            var ret = Decrypt(array, key);
            var len = GetNumInt(ret.Take(8).ToArray());
            if (App.dflag)
                LogtoFile("[DECRYPT]Length:" + len.ToString());
            ret = ret.Skip(8).ToArray();
            filename = Encoding.UTF8.GetString(ret.Take(len).ToArray());
            if (App.dflag)
                LogtoFile("[DECRYPT]Filename:" + filename);
            ret = ret.Skip(len).ToArray();
            return ret;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="array">要加密的 byte[] 数组</param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] array, string key)
        {
            key = GetStringMD5(key);
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            Aes rDel = Aes.Create();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(array, 0, array.Length);
            return resultArray;
        }

        private static string GetStringMD5(string str)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
            StringBuilder sBuilder = new();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="array">要解密的 byte[] 数组</param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] array, string key)
        {
            key = GetStringMD5(key);
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            Aes rDel = Aes.Create();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(array, 0, array.Length);
            return resultArray;
        }


        #endregion

    }


}
