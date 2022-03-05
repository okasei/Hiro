using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Windows.Security.Credentials;
using Windows.Security.Credentials.UI;

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

        public int Page {
            get { return p; }
            set
            {
                p = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Page)));
            }
        }
        public int Id {
            get { return i; }
            set
            {
                i = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Id)));
            }
        }
        public string Name { 
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

        public Cmditem()
        {
            Page = -1;
            Id = -1;
            Name = string.Empty;
            Command = string.Empty;
            p = -1;
            Id = -1;
            na = string.Empty;
            co = string.Empty;
        }
        public Cmditem(int a, int b, string c, string d)
        {
            Page = a;
            Id = b;
            Name = c;
            Command = d;
            p = a;
            Id = b;
            na = c;
            co = d;
        }
        public Cmditem(Cmditem c)
        {
            if (c == null)
            {
                Page = -1;
                Id = -1;
                Name = string.Empty;
                Command = string.Empty;
                p = -1;
                Id = -1;
                na = string.Empty;
                co = string.Empty;
            }
            else
            {
                Page = c.Page;
                Id = c.Id;
                Name = c.Name;
                Command = c.Command;
                p = c.Page;
                Id = c.Id;
                na = c.Name;
                co = c.Command;
            }
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
        public Scheduleitem(Scheduleitem c)
        {
            if (c == null)
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
            else
            {
                Time = c.Time;
                Id = c.Id;
                Name = c.Name;
                Command = c.Command;
                re = c.re;
                ti = c.Time;
                i = c.Id;
                na = c.Name;
                co = c.Command;
            }
        }

    }
    #endregion

    #region 通知窗口项目定义
    public class alarmwin
    {
        public Alarm win;
        public int id;
        public alarmwin(Alarm a, int b)
        {
            win = a;
            id = b;
        }
        public alarmwin(int a, Alarm b)
        {
            win = b;
            id = a;
        }
    }
    #endregion

    #region 通知项目定义
    public class noticeitem
    {
        public string? title;
        public string msg;
        public int time;
        public noticeitem(string ms = "NULL", int ti = 1,string? tit = null)
        {
            msg = ms;
            time = ti;
            title = tit;
        }
    }
    #endregion

    #region 语言项目定义
    public class language : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private string na;
        private string la;
        public string name
        {
            get { return na; }
            set
            {
                na = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(name)));
            }
        }
        public string langname
        {
            get { return la; }
            set
            {
                la = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(langname)));
            }
        }

        public language()
        {
            name = "null";
            langname = "null";
            na = "null";
            la = "null";
        }

        public language(string Name, string LangName)
        {
            name = Name;
            langname = LangName;
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
            var re = appID == null ? "null" : appID;
            ret = ret + re + ", Package: ";
            re = appPackage == null ? "null" : appPackage;
            ret = ret + re + ", Name: ";
            re = appName == null ? "null" : appName;
            ret = ret + re + ", Msg: ";
            re = msg == null ? "null" : msg;
            ret += re;
            return ret;
        }
    }

    #endregion

    public partial class utils : Component
    {
        #region 自动生成
        public utils()
        {
            InitializeComponent();
        }

        public utils(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
        #endregion

        #region 配置相关

        #region 读Ini文件
        public static string Read_Ini(string iniFilePath, string Section, string Key, string defaultText)
        {
            if (System.IO.File.Exists(iniFilePath))
            {
                byte[] buffer = new byte[1024];
                int ret = GetPrivateProfileString(Encoding.GetEncoding("utf-8").GetBytes(Section), Encoding.GetEncoding("utf-8").GetBytes(Key), Encoding.GetEncoding("utf-8").GetBytes(defaultText), buffer, 1024, iniFilePath);
                return Encoding.GetEncoding("utf-8").GetString(buffer, 0, ret).Trim();
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
                if (!System.IO.File.Exists(iniFilePath))
                    System.IO.File.Create(iniFilePath).Close();
                long OpStation = WritePrivateProfileString(Encoding.GetEncoding("utf-8").GetBytes(Section), Encoding.GetEncoding("utf-8").GetBytes(Key), Encoding.GetEncoding("utf-8").GetBytes(Value), iniFilePath);
                if (OpStation == 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                LogtoFile("[ERROR]" + ex.Message);
                return false;
            }

        }
        #endregion

        #region 写日志相关
        public static void LogtoFile(string val)
        {
            try
            {
                if (!System.IO.File.Exists(App.LogFilePath))
                    System.IO.File.Create(App.LogFilePath).Close();
                System.IO.FileStream fs = new(App.LogFilePath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite);
                System.IO.StreamReader sr = new(fs);
                var str = sr.ReadToEnd();
                System.IO.StreamWriter sw = new(fs);
                sw.Write(DateTime.Now.ToString("[HH:mm:ss]") + val + Environment.NewLine);
                sw.Flush();
                sw.Close();
                sr.Close();
                sr.Dispose();
                fs.Close();

            }
            catch (Exception ex)
            {
                LogtoFile("[ERROR]" + ex.Message);

            }
        }
        #endregion

        #region 翻译文件
        public static string Get_Transalte(string val)
        {
            return Read_Ini(App.CurrentDirectory + "\\system\\lang\\" + App.lang + ".hlp", "translate", val, "<???>").Replace("\\n", Environment.NewLine).Replace("%b", " ");
        }
        #endregion

        #region UI 相关
        public static void Get_Text_Visual_Width(System.Windows.Controls.ContentControl sender, double pixelPerDip, out System.Windows.Size size)
        {
            var formattedText = new System.Windows.Media.FormattedText(
                sender.Content.ToString(), System.Globalization.CultureInfo.CurrentUICulture, System.Windows.FlowDirection.LeftToRight, new System.Windows.Media.Typeface(sender.FontFamily, sender.FontStyle, sender.FontWeight, sender.FontStretch),
                sender.FontSize, System.Windows.Media.Brushes.Black, pixelPerDip);
            size.Width = formattedText.Width + sender.Padding.Left + sender.Padding.Right;
            size.Height = formattedText.Height + sender.Padding.Top + sender.Padding.Bottom;
        }
        public static void Set_Bgimage(System.Windows.Controls.Control sender)
        {
            var strFileName = Read_Ini(App.dconfig, "config", "backimage", "");
            if (Read_Ini(App.dconfig, "config", "background", "1").Equals("1") || !System.IO.File.Exists(strFileName))
            {
                if (!Read_Ini(App.dconfig, "config", "ani", "1").Equals("0"))
                {
                    System.Windows.Media.Animation.Storyboard? sb = new();
                    try
                    {
                        sb = AddColorAnimaton(App.AppAccentColor, 150, sender, "Background.Color", sb);
                        sb.Completed += delegate
                        {
                            sender.Background = new System.Windows.Media.SolidColorBrush(App.AppAccentColor);
                            sb = null;
                        };
                        sb.Begin();
                    }catch (Exception ex)
                    {
                        LogtoFile("[ERROR]" + ex.Message);
                        sender.Background = new System.Windows.Media.SolidColorBrush(App.AppAccentColor);
                    }
                    
                }
                else
                    sender.Background = new System.Windows.Media.SolidColorBrush(App.AppAccentColor);
            }
            else
            {
                System.Windows.Media.ImageBrush ib = new()
                {
                    Stretch = System.Windows.Media.Stretch.UniformToFill,
                    ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri(strFileName))
                };
                sender.Background = ib;
            }
        }
        public static void Set_Control_Location(System.Windows.Controls.Control sender, string val, bool extra = false, string? path = null, bool right = false, bool bottom = false,bool location = true)
        {
            if (extra == false || path == null || !System.IO.File.Exists(path))
                path = App.LangFilePath;
            try
            {
                if (sender != null)
                {
                    if (right == true)
                        sender.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    if (bottom == true)
                        sender.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    var loc = Read_Ini(path, "location", val, string.Empty);
                    loc = loc.Trim().Replace("%b", " ");
                    loc = loc[(loc.IndexOf("{") + 1)..];
                    loc = loc[..loc.LastIndexOf("}")];
                    var fontname = loc[..loc.IndexOf(",")];
                    loc = loc[(fontname.Length + 1)..];
                    var fontsize = loc[..loc.IndexOf(",")];
                    loc = loc[(fontsize.Length + 1)..];
                    var fontweight = loc[..loc.IndexOf(",")];
                    loc = loc[(fontweight.Length + 1)..];
                    var fontstyle = loc[..loc.IndexOf(",")];
                    loc = loc[(fontstyle.Length + 1)..];
                    var left = loc.Substring(0, loc.IndexOf(","));
                    loc = loc[(left.Length + 1)..];
                    var top = loc[..loc.IndexOf(",")];
                    loc = loc[(top.Length + 1)..];
                    var width = loc.Substring(0, loc.IndexOf(","));
                    loc = loc[(width.Length + 1)..];
                    var height = loc;
                    if (fontname != "-1")
                        sender.FontFamily = new System.Windows.Media.FontFamily(fontname);
                    if (fontsize != "-1")
                        sender.FontSize = double.Parse(fontsize);
                    sender.FontWeight = fontweight switch
                    {
                        "0" => System.Windows.FontWeights.Light,
                        "1" => System.Windows.FontWeights.Bold,
                        _ => System.Windows.FontWeights.Normal,
                    };
                    sender.FontStyle = fontstyle switch
                    {
                        "0" => System.Windows.FontStyles.Italic,
                        "1" => System.Windows.FontStyles.Oblique,
                        _ => System.Windows.FontStyles.Normal,
                    };
                    if (location)
                    {
                        sender.Width = (!width.Equals("-1")) ? double.Parse(width) : sender.Width;
                        sender.Height = (!height.Equals("-1")) ? double.Parse(height) : sender.Height;
                        System.Windows.Thickness thickness = new();
                        thickness.Left = (!left.Equals("-1")) ? right ? 0.0 : double.Parse(left) : sender.Margin.Left;
                        thickness.Right = (!left.Equals("-1")) ? !right ? sender.Margin.Right : double.Parse(left) : sender.Margin.Right;
                        thickness.Top = (!top.Equals("-1")) ? bottom ? 0.0 : double.Parse(top) : sender.Margin.Top;
                        thickness.Bottom = (!top.Equals("-1")) ? !bottom ? sender.Margin.Bottom : double.Parse(top) : sender.Margin.Bottom;
                        sender.Margin = thickness;
                    }
                }
            }
            catch (Exception ex)
            {
                LogtoFile("[ERROR]" + "sender " + val + "|" + ex.Message);
            }

        }

        public static void Set_Grid_Location(System.Windows.Controls.Grid sender, string val)
        {
            try
            {
                if (sender != null)
                {
                    var loc = Read_Ini(App.LangFilePath, "location", val, string.Empty);
                    loc = loc.Replace(" ", "").Replace("%b", " ");
                    loc = loc.Substring(loc.IndexOf("{") + 1);
                    loc = loc.Substring(0, loc.LastIndexOf("}"));
                    var left = loc.Substring(0, loc.IndexOf(","));
                    loc = loc.Substring(left.Length + 1);
                    var top = loc.Substring(0, loc.IndexOf(","));
                    loc = loc.Substring(top.Length + 1);
                    var width = loc.Substring(0, loc.IndexOf(","));
                    loc = loc.Substring(width.Length + 1);
                    var height = loc;
                    if (!width.Equals("-1"))
                        sender.Width = double.Parse(width);
                    if (!height.Equals("-1"))
                        sender.Height = double.Parse(height);
                    System.Windows.Thickness thickness = sender.Margin;
                    if (!left.Equals("-1"))
                        thickness.Left = double.Parse(left);
                    if (!top.Equals("-1"))
                        thickness.Top = double.Parse(top);
                    sender.Margin = thickness;

                }
            }
            catch (System.Exception ex)
            {
                LogtoFile("[ERROR]" + "sender " + val + "|" + ex.Message);
            }

        }
        #endregion

        #endregion

        #region 字符串处理
        public static String Path_Replace(String path, String toReplace, String replaced)
        {
            var resu = (replaced.EndsWith("\\")) ? replaced.Substring(0, replaced.Length - 1) : replaced;
            resu = Microsoft.VisualBasic.Strings.Replace(path, toReplace, resu, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
            if (resu != null)
                return resu;
            else
                return "";
        }

        private static String Anti_Path_Replace(String path, String replaced, String toReplace)
        {
            var resu = (toReplace.EndsWith("\\")) ? toReplace.Substring(0, toReplace.Length - 1) : toReplace;
            resu = Microsoft.VisualBasic.Strings.Replace(path, resu, replaced, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
            if (resu != null)
                return resu;
            else
                return String.Empty;
        }

        public static String Anti_Path_Prepare(String path)
        {
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

        public static String Path_Prepare(String path)
        {
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
            path = Path_Replace(path, "<MM>", DateTime.Now.ToString("MM"));
            path = Path_Replace(path, "<M>", DateTime.Now.ToString("M"));
            path = Path_Replace(path, "<dddd>", DateTime.Now.ToString("dddd"));
            path = Path_Replace(path, "<ddd>", DateTime.Now.ToString("ddd"));
            path = Path_Replace(path, "<dd>", DateTime.Now.ToString("dd"));
            path = Path_Replace(path, "<d>", DateTime.Now.ToString("d"));
            path = Path_Replace(path, "<HH>", DateTime.Now.ToString("HH"));
            path = Path_Replace(path, "<hh>", DateTime.Now.ToString("hh"));
            path = Path_Replace(path, "<mm>", DateTime.Now.ToString("mm"));
            path = Path_Replace(path, "<m>", DateTime.Now.ToString("m"));
            path = Path_Replace(path, "<ss>", DateTime.Now.ToString("ss"));
            path = Path_Replace(path, "<s>", DateTime.Now.ToString("s"));
            path = Path_Replace(path, "<version>", res.ApplicationVersion);
            path = Path_Replace(path, "<lang>", App.lang);
            path = Path_Replace(path, "<date>", DateTime.Now.ToString("yyyyMMdd"));
            path = Path_Replace(path, "<time>", DateTime.Now.ToString("HHmmss"));
            path = Path_Replace(path, "<now>", DateTime.Now.ToString("yyMMddHHmmss"));
            path = Path_Replace(path, "<me>", App.Username);
            path = Path_Replace(path, "<hiro>", App.AppTitle);
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
        public static void RunExe(String path, string? notification = null)
        {
            path = Path_Prepare_EX(Path_Prepare(path));
            if (System.IO.File.Exists(path) && path.ToLower().EndsWith(".hiro"))
                path = "seq(" + path + ")";
            if (path.ToLower().StartsWith("hiroad("))
            {
                try
                {
                    String titile, mes, toolstr;
                    toolstr = path.Substring(7, path.LastIndexOf(")") - 7);
                    titile = toolstr.Substring(0, toolstr.IndexOf(","));
                    toolstr = toolstr.Substring(titile.Length + 1);
                    mes = toolstr.Substring(0, toolstr.IndexOf(","));
                    toolstr = Path_Prepare_EX(toolstr.Substring(mes.Length + 1));
                    if (toolstr.Equals("<product>"))
                        toolstr = Get_Transalte("dlproduct");
                    Download dl = new(1, toolstr);
                    dl.textBoxHttpUrl.Text = titile;
                    dl.SavePath.Text = mes;
                    dl.autorun.IsChecked = true;
                    dl.autorun.IsEnabled = false;
                    dl.rurl = titile;
                    dl.rpath = mes;
                    dl.Show();
                    dl.StartDownload();
                    return;
                }
                catch (Exception ex)
                {
                    LogtoFile("[ERROR]" + ex.Message);
                    return;
                }
            }
            if (path.ToLower().StartsWith("download("))
            {
                String titile, mes, toolstr;
                if (path.LastIndexOf(")") != -1)
                {
                    toolstr = path.Substring(9, path.LastIndexOf(")") - 9);
                    if (toolstr.LastIndexOf(",") != -1)
                    {
                        titile = toolstr.Substring(0, toolstr.LastIndexOf(","));
                        if (titile.Length < toolstr.Length - 1)
                        {
                            mes = toolstr.Substring(titile.Length + 1);
                            Download dl = new(0, "");
                            dl.textBoxHttpUrl.Text = titile;
                            dl.SavePath.Text = mes;
                            dl.Show();
                            return;
                        }
                        else
                        {
                            Download dl = new(0, "");
                            dl.textBoxHttpUrl.Text = titile;
                            dl.Show();
                            return;
                        }

                    }
                    else
                    {
                        Download dl = new(0, "");
                        dl.textBoxHttpUrl.Text = toolstr;
                        dl.Show();
                        return;
                    }
                }
                else
                {
                    App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("dltitle")));
                    return;
                }
            }
            if (path.Length > 22 && path.ToLower().StartsWith("save("))
            {
                String titile, mes, toolstr;
                if (path.LastIndexOf(")") != -1)
                {
                    toolstr = path.Substring(5, path.LastIndexOf(")") - 5);
                    if (toolstr.LastIndexOf(",") != -1)
                    {
                        titile = toolstr.Substring(0, toolstr.LastIndexOf(","));
                        if (titile.Length < toolstr.Length - 1)
                        {
                            mes = toolstr.Substring(titile.Length + 1);
                        }
                        else
                        {
                            App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("execute")));
                            return;
                        }

                    }

                    else
                    {
                        App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("execute")));
                        return;
                    }
                }
                else
                {
                    App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("execute")));
                    return;
                }
                BackgroundWorker bw = new();
                mes = Path_Prepare(mes);
                CreateFolder(mes);
                bw.DoWork += delegate
                {
                    toolstr = GetWebContent(titile, true, mes);
                };
                bw.RunWorkerCompleted += delegate
                {
                    if (toolstr.ToLower().Equals("error"))
                    {
                        App.Notify(new noticeitem(Get_Transalte("error"), 2, Get_Transalte("execute")));
                    }
                    else
                    {
                        App.Notify(new noticeitem(Get_Transalte("success"), 2, Get_Transalte("execute")));
                    }
                };
                bw.RunWorkerAsync();
                return;
            }
            if (path.ToLower().Equals("memory()"))
            {
                GC.Collect();
                return;
            }
            if (path.ToLower().Equals("debug()"))
            {
                App.dflag = !App.dflag;
                notification = Get_Transalte("debug");
                if (App.dflag)
                    path = "notify(" + Get_Transalte("debugon") + ",2)";
                else
                    path = "notify(" + Get_Transalte("debugoff") + ",2)";
            }
            if (path.Length == 10 && path.ToLower().StartsWith("weather("))
            {
                notification = Get_Transalte("weather");
                path = path.ToLower() switch
                {
                    "weather(0)" => "alarm(" + Get_Transalte("weather") + ",https://api.rexio.cn/v1/rex.php?r=weather&k=6725dccca57b2998e8fc47cee2a8f72f&lang=" + App.lang + ")",
                    "weather(1)" => "notify(https://api.rexio.cn/v1/rex.php?r=weather&k=6725dccca57b2998e8fc47cee2a8f72f&lang=" + App.lang + ",2)",
                    _ => "notify(" + Get_Transalte("syntax") + ",2)"
                };
            }
            if (path.Length > 7 && path.ToLower().StartsWith("debug("))
            {
                notification = Get_Transalte("debug");
                path = path.Substring(6);
                path = path.Substring(0, path.Length - 1);
                path = "notify(" + path + ")";
            }
            if (path.Length > 7 && path.ToLower().StartsWith("alarm("))
            {
                path = path[6..];
                path = path[0..^1];
                if (path.IndexOf(",") != -1)
                {
                    var pa = path.Substring(0, path.IndexOf(","));
                    path = path.Substring(pa.Length + 1);
                    if (pa.ToLower().StartsWith("http://") || pa.ToLower().StartsWith("https://"))
                    {
                        BackgroundWorker bw = new();
                        bw.DoWork += delegate
                        {
                            pa = GetWebContent(pa);
                        };
                        bw.RunWorkerCompleted += delegate
                        {
                            RunExe("alarm(" + pa + "," + path + ")");
                        };
                        bw.RunWorkerAsync();
                        return;
                    }
                    if (path.ToLower().StartsWith("http://") || path.ToLower().StartsWith("https://"))
                    {
                        BackgroundWorker bw = new();
                        bw.DoWork += delegate
                        {
                            path = GetWebContent(path);
                        };
                        bw.RunWorkerCompleted += delegate
                        {
                            RunExe("alarm(" + pa + "," + path.Replace("<br>", "\\n") + ")");
                        };
                        bw.RunWorkerAsync();
                        return;
                    }
                    if (Read_Ini(App.dconfig, "config", "toast", "0").Equals("1"))
                    {
                        new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                            .AddArgument("Launch", App.AppTitle)
                            .AddText(pa)
                            .AddText(path.Replace("\\n", Environment.NewLine))
                            .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                        .SetContent(Get_Transalte("alarmone")))
                            .Show();
                    }
                    else
                    {
                        Alarm ala = new(-1, CustomedTitle: pa, CustomedContnet: utils.Path_Prepare_EX(path.Replace("\\n", Environment.NewLine)), OneButtonOnly: 1);
                        ala.Show();
                    }
                }
                else
                {
                    if (path.ToLower().StartsWith("http://") || path.ToLower().StartsWith("https://"))
                    {
                        BackgroundWorker bw = new();
                        bw.DoWork += delegate
                        {
                            path = GetWebContent(path);
                        };
                        bw.RunWorkerCompleted += delegate
                        {
                            RunExe("alarm(" + path.Replace("<br>", "\\n") + ")");
                        };
                        bw.RunWorkerAsync();
                        return;
                    }
                    if (Read_Ini(App.dconfig, "config", "toast", "0").Equals("1"))
                    {
                        new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                            .AddText(Get_Transalte("alarmtitle"))
                            .AddText(path.Replace("\\n", Environment.NewLine))
                            .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                        .SetContent(Get_Transalte("alarmone")))
                            .Show();
                    }
                    else
                    {
                        Alarm ala = new(-1, CustomedContnet: utils.Path_Prepare_EX(path.Replace("\\n", Environment.NewLine)), OneButtonOnly: 1);
                        ala.Show();
                    }

                }
                return;
            }
            if (path.Length > 4 && path.ToLower() == "hello" || path.ToLower() == "hello()")
            {
                var hr = DateTime.Now.Hour;
                var morning = Read_Ini(App.LangFilePath, "local", "morning", "[6,7,8,9,10]");
                var noon = Read_Ini(App.LangFilePath, "local", "noon", "[11,12,13]");
                var afternoon = Read_Ini(App.LangFilePath, "local", "afternoon", "[14,15,16,17,18]");
                var evening = Read_Ini(App.LangFilePath, "local", "evening", "[19,20,21,22]");
                var night = Read_Ini(App.LangFilePath, "local", "night", "[23,0,1,2,3,4,5]");
                morning = "," + morning.Replace("[", "").Replace("]", "").Replace(" ", "") + ",";
                noon = "," + noon.Replace("[", "").Replace("]", "").Replace(" ", "") + ",";
                afternoon = "," + afternoon.Replace("[", "").Replace("]", "").Replace(" ", "") + ",";
                evening = "," + evening.Replace("[", "").Replace("]", "").Replace(" ", "") + ",";
                night = "," + night.Replace("[", "").Replace("]", "").Replace(" ", "") + ",";
                if (morning.IndexOf("," + hr + ",") != -1)
                {
                    if (App.CustomUsernameFlag == 0)
                        App.Notify(new noticeitem(Get_Transalte("morning").Replace("%u", App.EnvironmentUsername), 2));
                    else
                        App.Notify(new noticeitem(Get_Transalte("morningcus").Replace("%u", App.Username), 2));

                }
                else if (noon.IndexOf("," + hr + ",") != -1)
                {
                    if (App.CustomUsernameFlag == 0)
                        App.Notify(new noticeitem(Get_Transalte("noon").Replace("%u", App.EnvironmentUsername), 2));
                    else
                        App.Notify(new noticeitem(Get_Transalte("nooncus").Replace("%u", App.Username), 2));

                }
                else if (afternoon.IndexOf("," + hr + ",") != -1)
                {
                    if (App.CustomUsernameFlag == 0)
                        App.Notify(new noticeitem(Get_Transalte("afternoon").Replace("%u", App.EnvironmentUsername), 2));
                    else
                        App.Notify(new noticeitem(Get_Transalte("afternooncus").Replace("%u", App.Username), 2));

                }
                else if (evening.IndexOf("," + hr + ",") != -1)
                {
                    if (App.CustomUsernameFlag == 0)
                        App.Notify(new noticeitem(Get_Transalte("evening").Replace("%u", App.EnvironmentUsername), 2));
                    else
                        App.Notify(new noticeitem(Get_Transalte("eveningcus").Replace("%u", App.Username), 2));
                }
                else
                {
                    if (App.CustomUsernameFlag == 0)
                        App.Notify(new noticeitem(Get_Transalte("night").Replace("%u", App.EnvironmentUsername), 2));
                    else
                        App.Notify(new noticeitem(Get_Transalte("nightcus").Replace("%u", App.Username), 2));
                }
                return;
            }
            if (path.Length > 2 && path.ToLower() == "nop" || path.ToLower() == "nop()")
            {
                return;
            }
            if (path.Length > 14 && path.ToLower().StartsWith("wallpaper("))
            {
                String titile, mes, toolstr;
                if (path.LastIndexOf(")") != -1)
                {
                    toolstr = path.Substring(10, path.LastIndexOf(")") - 10);
                    if (toolstr.LastIndexOf(",") != -1)
                    {
                        titile = toolstr.Substring(0, toolstr.LastIndexOf(","));
                        mes = titile.Length < toolstr.Length - 1 ? toolstr.Substring(titile.Length + 1) : "3";
                    }
                    else
                    {
                        App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("wallpaper")));
                        return;
                    }
                }
                else
                {
                    App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("wallpaper")));
                    return;
                }
                using (Microsoft.Win32.RegistryKey? key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
                {
                    int[] para = new int[] { 10, 6, 22, 2, 0, 0 };
                    int[] par = new int[] { 0, 0, 0, 0, 1, 0 };
                    var v = Convert.ToInt32(mes);
                    v = v < 0 ? 0 : v > 5 ? 5 : v;
                    if (key != null)
                    {
                        key.SetValue(@"WallpaperStyle", para[v].ToString());
                        key.SetValue(@"TileWallpaper", par[v].ToString());
                    }
                }
                SystemParametersInfo(20, 0, titile, 0x01 | 0x02);
                App.Notify(new noticeitem(Get_Transalte("wpchanged"), 2, Get_Transalte("wallpaper")));
                return;
            }
            if (path.Length > 11 && path.ToLower().StartsWith("bingw("))
            {
                String toolstr;
                if (path.LastIndexOf(")") != -1)
                {
                    toolstr = path.Substring(6, path.LastIndexOf(")") - 6);
                    if (toolstr.StartsWith("\""))
                        toolstr = toolstr.Substring(1);
                    if (toolstr.EndsWith("\""))
                        toolstr = toolstr.Substring(0, toolstr.Length - 1);
                    try
                    {
                        if (!System.IO.File.Exists(toolstr))
                        {
                            System.Net.Http.HttpRequestMessage request = new(System.Net.Http.HttpMethod.Get, "https://api.rexio.cn/v1/rex.php?r=wallpaper");
                            request.Headers.Add("UserAgent", "Rex/2.1.0 (Hiro Inside)");
                            request.Content = new System.Net.Http.StringContent("");
                            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                            BackgroundWorker bw = new();
                            bw.DoWork += delegate
                            {
                                try
                                {
                                    System.Net.Http.HttpResponseMessage response = App.hc.Send(request);

                                    if (response.Content != null)
                                    {
                                        System.IO.Stream stream = response.Content.ReadAsStream();
                                        System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                                        CreateFolder(toolstr);
                                        image.Save(toolstr);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Windows.MessageBox.Show(ex.ToString(), Get_Transalte("error") + " - " + App.AppTitle);
                                    LogtoFile("[ERROR]" + ex.Message);
                                }
                            };
                            bw.RunWorkerCompleted += delegate
                            {
                                if (!System.IO.File.Exists(toolstr))
                                    App.Notify(new noticeitem(Get_Transalte("unknown"), 2, Get_Transalte("wallpaper")));
                                else
                                    App.Notify(new noticeitem(Get_Transalte("wpsaved"), 2, Get_Transalte("wallpaper")));
                            };
                            bw.RunWorkerAsync();
                        }
                        else
                            App.Notify(new noticeitem(Get_Transalte("wpexist"), 2, Get_Transalte("wallpaper")));
                    }
                    catch (Exception ex)
                    {
                        App.Notify(new noticeitem(Get_Transalte("unknown"), 2, Get_Transalte("wallpaper")));
                        System.Windows.MessageBox.Show(ex.ToString(), Get_Transalte("error") + " - " + App.AppTitle);
                        LogtoFile("[ERROR]" + ex.Message);

                    }
                }
                else
                {
                    App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("wallpaper")));
                    return;
                }
                return;
            }
            if (path.Length > 5 && path.ToLower().StartsWith("exit()"))
            {
                try
                {
                    Environment.Exit(Environment.ExitCode);
                    System.Windows.Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    LogtoFile("[ERROR]" + ex.Message);
                    throw;
                }

            }
            if (path.Length > 5 && path.ToLower().StartsWith("show()"))
            {
                if (App.mn != null)
                {
                    App.mn.Visibility = System.Windows.Visibility.Visible;
                    if (!Read_Ini(App.dconfig, "config", "ani", "1").Equals("0"))
                    {
                        Blur_Animation(ConvertInt(Read_Ini(App.dconfig, "config", "blur", "0")), true, App.mn.bgimage, App.mn);
                    }

                }
                else
                {
                    App.mn = new();
                    App.mn.Show();
                }
                return;
            }
            if (path.Length > 5 && path.ToLower().StartsWith("lock()"))
            {
                if (App.mn != null)
                {
                    App.Locked = true;
                    if (0 < App.mn.tc.SelectedIndex && App.mn.tc.SelectedIndex < 4)
                        App.mn.Set_Label(App.mn.homex);
                    App.mn.versionlabel.Content = App.AppVersion + " 🔒";
                }
                return;
            }
            if (path.Length > 5 && path.ToLower().StartsWith("auth()"))
            {
                BackgroundWorker sc = new();
                BackgroundWorker fa = new();
                sc.RunWorkerCompleted += delegate
                 {
                     if (App.mn != null)
                     {
                         App.mn.versionlabel.Content = App.AppVersion;
                         App.Locked = false;
                     }
                 };
                fa.RunWorkerCompleted += delegate
                {
                    if (App.mn != null)
                    {
                        if (App.Locked)
                            App.mn.versionlabel.Content = App.AppVersion + " 🔒";
                    }
                };
                Register(sc, fa, fa);
                return;
            }
            if (path.Length > 5 && path.ToLower().StartsWith("home()"))
            {
                if (App.mn != null)
                {
                    App.mn.Visibility = System.Windows.Visibility.Visible;
                    App.mn.Set_Label(App.mn.homex);
                }
                return;
            }
            if (path.Length > 5 && path.ToLower().StartsWith("item()"))
            {
                if (App.mn != null)
                {
                    App.mn.Visibility = System.Windows.Visibility.Visible;
                    App.mn.Set_Label(App.mn.itemx);
                }
                return;
            }
            if (path.Length > 9 && path.ToLower().StartsWith("schedule()"))
            {
                if (App.mn != null)
                {
                    App.mn.Visibility = System.Windows.Visibility.Visible;
                    App.mn.Set_Label(App.mn.schedulex);
                }
                return;
            }
            if (path.Length > 7 && path.ToLower().StartsWith("config()"))
            {
                if (App.mn != null)
                {
                    App.mn.Visibility = System.Windows.Visibility.Visible;
                    App.mn.Set_Label(App.mn.configx);
                }
                return;
            }
            if (path.Length > 5 && path.ToLower().StartsWith("help()"))
            {
                if (App.mn != null)
                {
                    App.mn.Visibility = System.Windows.Visibility.Visible;
                    App.mn.Set_Label(App.mn.helpx);
                }
                return;
            }
            if (path.Length > 6 && path.ToLower().StartsWith("about()"))
            {
                if (App.mn != null)
                {
                    App.mn.Visibility = System.Windows.Visibility.Visible;
                    App.mn.Set_Label(App.mn.aboutx);
                }
                return;
            }
            if (path.Length > 5 && path.ToLower().StartsWith("hide()"))
            {
                if (App.mn != null)
                {
                    App.mn.Visibility = System.Windows.Visibility.Hidden;
                }
                return;
            }
            if (path.Length > 8 && path.ToLower().StartsWith("restart()"))
            {
                if (App.mn != null)
                    App.mn.Close();
                App.mn = null;
                System.Threading.Thread.Sleep(100);
                App.mn = new();
                App.mn.Show();
                return;
            }
            if (path.Length > 5 && path.ToLower().StartsWith("menu()"))
            {
                if (App.wnd != null && App.wnd.cm != null)
                {
                    App.Load_Menu();
                    App.wnd.cm.IsOpen = true;
                }
                return;
            }
            if (path.Length > 7 && path.ToLower().StartsWith("editor()"))
            {
                if (App.ed == null)
                    App.ed = new Editor();
                App.ed.Show();
                return;
            }
            if (path.Length > 8 && path.ToLower().StartsWith("lockscr()"))
            {
                if (App.ls == null)
                    App.ls = new Lockscr();
                App.ls.Show();
                return;
            }
            if (path.Length > 8 && path.ToLower().StartsWith("message("))
            {
                String toolstr;
                if (path.LastIndexOf(")") != -1)
                {
                    toolstr = path.Substring(8, path.LastIndexOf(")") - 8);
                    toolstr = Path_Prepare(toolstr);
                }
                else
                {
                    toolstr = "syntax error";
                }
                Background? bg = null;
                if (Read_Ini(toolstr, "Action", "Background", "true").ToLower().Equals("true"))
                    bg = new();
                Message msg = new();
                msg.bg = bg;
                msg.Title = Path_Prepare(Path_Prepare_EX(Read_Ini(toolstr, "Message", "title", Get_Transalte("syntax")))) + " - " + App.AppTitle;
                msg.backtitle.Content = Path_Prepare(Path_Prepare_EX(Path_Prepare_EX(Read_Ini(toolstr, "Message", "title", Get_Transalte("syntax")))));
                msg.acceptbtn.Content = Read_Ini(toolstr, "Message", "accept", Get_Transalte("msgaccept"));
                msg.rejectbtn.Content = Read_Ini(toolstr, "Message", "reject", Get_Transalte("msgreject"));
                msg.cancelbtn.Content = Read_Ini(toolstr, "Message", "cancel", Get_Transalte("msgcancel"));
                msg.toolstr = toolstr;
                toolstr = Path_Prepare_EX(Path_Prepare(Read_Ini(toolstr, "Message", "content", Get_Transalte("syntax"))));
                if (toolstr.ToLower().StartsWith("http://") || toolstr.ToLower().StartsWith("https://"))
                {
                    msg.sv.Content = Get_Transalte("loading");
                    BackgroundWorker bw = new();
                    bw.DoWork += delegate
                    {
                        toolstr = GetWebContent(toolstr).Replace("<br>", "\\n");
                    };
                    bw.RunWorkerCompleted += delegate
                    {
                        msg.sv.Content = toolstr.Replace("\\n", Environment.NewLine);
                    };
                    bw.RunWorkerAsync();
                }
                else if (System.IO.File.Exists(toolstr))
                    msg.sv.Content = Path_Prepare(Path_Prepare_EX(System.IO.File.ReadAllText(toolstr))).Replace("\\n", Environment.NewLine);
                else
                    msg.sv.Content = toolstr.Replace("\\n", Environment.NewLine);
                msg.Load_Position();
                msg.Show();
                return;
            }
            if (path.Length > 7 && path.ToLower().StartsWith("notify("))
            {
                String titile, mes, toolstr;
                if (path.LastIndexOf(")") != -1)
                {
                    toolstr = path[7..path.LastIndexOf(")")];
                    if (toolstr.LastIndexOf(",") != -1)
                    {
                        titile = toolstr[..toolstr.LastIndexOf(",")];
                        if (titile.Length < toolstr.Length - 1)
                        {
                            mes = toolstr[(titile.Length + 1)..];
                        }
                        else
                        {
                            mes = "2";
                        }

                    }

                    else
                    {
                        titile = toolstr;
                        mes = "2";
                    }
                }
                else
                {
                    titile = Get_Transalte("syntax");
                    mes = "2";
                }
                try
                {
                    int duration = Convert.ToInt32(mes);
                    if (titile.ToLower().StartsWith("http://") || titile.ToLower().StartsWith("https://"))
                    {
                        BackgroundWorker bw = new();
                        bw.DoWork += delegate
                        {
                            titile = GetWebContent(titile).Replace("<br>", "\\n");
                        };
                        bw.RunWorkerCompleted += delegate
                        {
                            RunExe("notify(" + titile + "," + duration.ToString() + ")", notification);
                        };
                        bw.RunWorkerAsync();
                        return;
                    }
                    if (System.IO.File.Exists(titile))
                        titile = System.IO.File.ReadAllText(titile).Replace(Environment.NewLine, "\\n");
                    App.Notify(new(titile, duration));
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString(), Get_Transalte("error") + " - " + App.AppTitle);
                    LogtoFile("[ERROR]" + ex.Message);
                }
                return;
            }
            if (path.Length > 4 && path.ToLower().StartsWith("seq("))
            {
                String toolstr;
                if (path.LastIndexOf(")") != -1)
                {
                    toolstr = path[4..path.LastIndexOf(")")];
                    if (toolstr.StartsWith("\""))
                        toolstr = toolstr[1..];
                    if (toolstr.EndsWith("\""))
                        toolstr = toolstr[0..^1];
                    if (!System.IO.File.Exists(toolstr))
                    {
                        App.Notify(new(Get_Transalte("syntax"), 2, Get_Transalte("seqtitle")));
                        return;
                    }
                    Sequence sq = new();
                    sq.Show();
                    sq.SeqExe(toolstr);


                }
                else
                {
                    App.Notify(new(Get_Transalte("syntax"), 2, Get_Transalte("seqtitle")));
                    return;
                }
                return;
            }
            if (path.Length > 4 && path.ToLower().StartsWith("run("))
            {
                String titile, mes, toolstr;
                if (path.LastIndexOf(")") != -1)
                {
                    toolstr = path[4..path.LastIndexOf(")")];
                    if (toolstr.LastIndexOf(",") != -1)
                    {
                        titile = toolstr[..toolstr.LastIndexOf(",")];
                        if (titile.Length < toolstr.Length - 1)
                        {
                            mes = toolstr[(titile.Length + 1)..];
                        }
                        else
                        {
                            App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("execute")));
                            return;
                        }

                    }

                    else
                    {
                        App.Notify(new(Get_Transalte("syntax"), 2, Get_Transalte("execute")));
                        return;
                    }
                }
                else
                {
                    App.Notify(new(Get_Transalte("syntax"), 2, Get_Transalte("execute")));
                    return;
                }
                try
                {
                    ProcessStartInfo pinfo = new();
                    pinfo.UseShellExecute = true;
                    List<string> blank = new();
                    var a = 0;
                    while (titile.IndexOf("\"") != -1)
                    {
                        a++;
                        var lef = titile[..titile.IndexOf("\"")];
                        titile = titile[(lef.Length + 1)..];
                        if (titile.IndexOf("\"") == -1)
                            break;
                        var inside = titile[..titile.IndexOf("\"")];
                        titile = titile[(inside.Length + 1)..];
                        blank.Add(inside);
                        titile = lef + "[" + a.ToString() + "]" + titile;
                    }
                    if (titile.IndexOf(" ") == -1)
                    {
                        pinfo.FileName = titile;
                    }
                    else
                    {
                        pinfo.FileName = titile.Substring(0, titile.IndexOf(" "));
                        pinfo.Arguments = titile.Substring(titile.IndexOf(" ") + 1);
                    }
                    a = 1;
                    while (blank.Count > 0 && pinfo.FileName.IndexOf("[" + a.ToString() + "]") != -1)
                    {
                        pinfo.FileName = pinfo.FileName.Replace("[" + a.ToString() + "]", blank[0]);
                        blank.RemoveAt(0);
                        a++;
                    }
                    while (blank.Count > 0 && pinfo.Arguments.IndexOf("[" + a.ToString() + "]") != -1)
                    {
                        pinfo.Arguments = pinfo.Arguments.Replace("[" + a.ToString() + "]", "\"" + blank[0]) + "\"";
                        blank.RemoveAt(0);
                        a++;
                    }
                    if (mes.StartsWith("\""))
                        mes = mes.Substring(1);
                    if (mes.EndsWith("\""))
                        mes = mes.Substring(0, mes.Length - 1);
                    if (titile.StartsWith("\""))
                        titile = titile.Substring(1);
                    if (mes.EndsWith("\""))
                        titile = titile.Substring(0, titile.Length - 1);
                    mes = mes.ToLower();
                    if (mes.IndexOf("a") != -1)
                        pinfo.Verb = "runas";
                    if (mes.IndexOf("h") != -1)
                        pinfo.WindowStyle = ProcessWindowStyle.Hidden;
                    if (mes.IndexOf("i") != -1)
                        pinfo.WindowStyle = ProcessWindowStyle.Minimized;
                    if (mes.IndexOf("x") != -1)
                        pinfo.WindowStyle = ProcessWindowStyle.Maximized;
                    if (mes.IndexOf("n") != -1)
                        pinfo.CreateNoWindow = true;
                    //启动进程
                    Process? p = Process.Start(pinfo);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString(), Get_Transalte("error") + " - " + App.AppTitle);
                    LogtoFile("[ERROR]" + ex.Message);
                }
                if (App.mn == null)
                    RunExe("exit()");
                return;
            }
            if (path.Length > 12 && path.ToLower().StartsWith("delete("))
            {
                String toolstr;
                if (path.LastIndexOf(")") != -1)
                {
                    toolstr = path.Substring(7, path.LastIndexOf(")") - 7);
                    if (toolstr.StartsWith("\""))
                        toolstr = toolstr.Substring(1);
                    if (toolstr.EndsWith("\""))
                        toolstr = toolstr.Substring(0, toolstr.Length - 1);
                    if (!System.IO.File.Exists(toolstr))
                    {
                        if (System.IO.Directory.Exists(toolstr))
                            try
                            {
                                System.IO.Directory.Delete(toolstr, true);
                            }
                            catch (Exception ex)
                            {
                                App.Notify(new noticeitem(Get_Transalte("failed"), 2, Get_Transalte("file")));
                                utils.LogtoFile("[ERROR]" + ex.Message);
                            }
                        else
                            App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("file")));
                        return;
                    }
                    try
                    {
                        System.IO.File.Delete(toolstr);
                    }
                    catch  (Exception ex)
                    {
                        App.Notify(new noticeitem(Get_Transalte("failed"), 2, Get_Transalte("file")));
                        utils.LogtoFile("[ERROR]" + ex.Message);
                    }
                }
                return;
            }
            if (path.ToLower().StartsWith("web("))
            {
                if (path.LastIndexOf(")") != -1)
                {
                    path = path[4..path.LastIndexOf(")")];
                    if (path.StartsWith("\""))
                        path = path[1..];
                    if (path.EndsWith("\""))
                        path = path[0..^1];
                    try
                    {
                        Web web;
                        string para = "";
                        if (path.IndexOf(",") != -1)
                        {
                            para = path.Substring(path.IndexOf(",") + 1, path.Length - path.IndexOf(",") - 1).ToLower();
                            path = path.Substring(0, path.Length - 1 - para.Length);
                            if (System.IO.File.Exists(path) && para.ToLower().IndexOf("f") != -1)
                            {
                                string? title = null;
                                if (!Read_Ini(path, "Web", "Title", String.Empty).Equals(String.Empty))
                                    title = Read_Ini(path, "Web", "Title", String.Empty).Replace("%b", " ");
                                web = new(Read_Ini(path, "Web", "URI", "about:blank"), title);
                                web.Height = Double.Parse(Read_Ini(path, "Web", "Height", "450"));
                                web.Width = Double.Parse(Read_Ini(path, "Web", "Width", "800"));
                                para = Read_Ini(path, "Web", "Parameters", "");
                            }
                            else
                                web = new(path);
                        }
                        else
                            web = new(path);
                        if (para.IndexOf("i") != -1)
                            web.WindowState = System.Windows.WindowState.Minimized;
                        else if (para.IndexOf("x") != -1)
                            web.WindowState = System.Windows.WindowState.Maximized;
                        if (para.IndexOf("s") != -1)
                            web.self = true;
                        if (para.IndexOf("-m") != -1)
                            web.ResizeMode = System.Windows.ResizeMode.CanMinimize;
                        else if (para.IndexOf("-r") != -1)
                            web.ResizeMode = System.Windows.ResizeMode.NoResize;
                        if (para.IndexOf("-c") != -1)
                            web.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                    }
                    catch (Exception ex)
                    {
                        LogtoFile("[ERROR]" + ex.Message);
                    }
                }
                return;
            }
            if (path.Length > 10 && path.ToLower().StartsWith("move("))
            {
                String titile, mes, toolstr;
                if (path.LastIndexOf(")") != -1)
                {
                    toolstr = path.Substring(5, path.LastIndexOf(")") - 5);
                    if (toolstr.LastIndexOf(",") != -1)
                    {
                        titile = toolstr.Substring(0, toolstr.LastIndexOf(","));
                        if (titile.Length < toolstr.Length - 1)
                        {
                            mes = toolstr.Substring(titile.Length + 1);
                            if (!System.IO.File.Exists(titile))
                            {
                                if (System.IO.Directory.Exists(titile))
                                    try
                                    {
                                        System.IO.Directory.Move(titile, mes);
                                    }
                                    catch (Exception ex)
                                    {
                                        App.Notify(new noticeitem(Get_Transalte("failed"), 2, Get_Transalte("file")));
                                        utils.LogtoFile("[ERROR]" + ex.Message);
                                    }
                                else
                                    App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("file")));
                                return;
                            }
                            try
                            {
                                System.IO.File.Move(titile, mes);
                            }
                            catch (Exception ex)
                            {
                                App.Notify(new noticeitem(Get_Transalte("failed"), 2, Get_Transalte("file")));
                                utils.LogtoFile("[ERROR]" + ex.Message);
                            }
                            return;
                        }
                        else
                        {
                            App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("file")));
                            return;
                        }

                    }
                    else
                    {
                        App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("file")));
                        return;
                    }
                }
                else
                {
                    App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("file")));
                    return;
                }
            }
            if (path.Length > 10 && path.ToLower().StartsWith("copy("))
            {
                String titile, mes, toolstr;
                if (path.LastIndexOf(")") != -1)
                {
                    toolstr = path.Substring(5, path.LastIndexOf(")") - 5);
                    if (toolstr.LastIndexOf(",") != -1)
                    {
                        titile = toolstr.Substring(0, toolstr.LastIndexOf(","));
                        if (titile.Length < toolstr.Length - 1)
                        {
                            mes = toolstr.Substring(titile.Length + 1);
                            if (!System.IO.File.Exists(titile))
                            {
                                if (System.IO.Directory.Exists(titile))
                                    try
                                    {
                                        CopyDirectory(titile, mes);
                                    }
                                    catch (Exception ex)
                                    {
                                        App.Notify(new noticeitem(Get_Transalte("failed"), 2, Get_Transalte("file")));
                                        utils.LogtoFile("[ERROR]" + ex.Message);
                                    }
                                else
                                    App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("file")));
                                return;
                            }
                            try
                            {
                                System.IO.File.Copy(titile, mes);
                            }
                            catch (Exception ex)
                            {
                                App.Notify(new noticeitem(Get_Transalte("failed"), 2, Get_Transalte("file")));
                                utils.LogtoFile("[ERROR]" + ex.Message);
                            }
                            return;
                        }
                        else
                        {
                            App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("file")));
                            return;
                        }

                    }
                    else
                    {
                        App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("file")));
                        return;
                    }
                }
                else
                {
                    App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("file")));
                    return;
                }
            }
            if (path.ToLower().StartsWith("bluetooth("))
            {
                bool? situation = path.ToLower() switch
                {
                    "bluetooth(0)" => false,
                    "bluetooth(off)" => false,
                    "bluetooth(1)" => true,
                    "bluetooth(on)" => true,
                    _ => null,
                };
                SetBthState(situation);
                return;
            }
            if (path.ToLower().StartsWith("wifi("))
            {
                int situation = path.ToLower() switch
                {
                    "wifi(0)" => 0,
                    "wifi(off)" => 0,
                    "wifi(1)" => 1,
                    "wifi(on)" => 1,
                    "wifi(2)" => 2,
                    "wifi(dis)" => 2,
                    "wifi(disconnect)" => 2,
                    "wifi(3)" => 3,
                    "wifi(con)" => 3,
                    "wifi(connect)" => 3,
                    _ => -1,
                };
                if (situation == -1)
                {
                    try
                    {
                        path = path[5..^1];
                        if (path.ToLower().IndexOf(",o") != -1)
                        {
                            path = path.Replace(",o", "").Replace(",O", "");
                            SetWiFiState(3, path, true);
                        }
                        else
                            SetWiFiState(3, path);
                    }
                    catch (Exception ex)
                    {
                        utils.LogtoFile("[ERROR]" + ex.Message);
                    }
                }
                else
                    SetWiFiState(situation);
                return;
            }
            try
            {
                ProcessStartInfo pinfo = new();
                pinfo.UseShellExecute = true;
                List<string> blank = new();
                var a = 0;
                while (path.IndexOf("\"") != -1)
                {
                    a++;
                    var lef = path[..path.IndexOf("\"")];
                    path = path[(lef.Length + 1)..];
                    if (path.IndexOf("\"") == -1)
                        break;
                    var inside = path[..path.IndexOf("\"")];
                    path = path[(inside.Length + 1)..];
                    blank.Add(inside);
                    path = lef + "[" + a.ToString() + "]" + path;
                }
                if (path.IndexOf(" ") == -1)
                {
                    pinfo.FileName = path;
                }
                else
                {
                    pinfo.FileName = path[..path.IndexOf(" ")];
                    pinfo.Arguments = path[(path.IndexOf(" ") + 1)..];
                }
                a = 1;
                while (blank.Count > 0 && pinfo.FileName.IndexOf("[" + a.ToString() + "]") != -1)
                {
                    pinfo.FileName = pinfo.FileName.Replace("[" + a.ToString() + "]", blank[0]);
                    blank.RemoveAt(0);
                    a++;
                }
                while (blank.Count > 0 && pinfo.Arguments.IndexOf("[" + a.ToString() + "]") != -1)
                {
                    pinfo.Arguments = pinfo.Arguments.Replace("[" + a.ToString() + "]", "\"" + blank[0]) + "\"";
                    blank.RemoveAt(0);
                    a++;
                }
                //启动进程
                Process? p = Process.Start(pinfo);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), Get_Transalte("error") + " - " + App.AppTitle);
                LogtoFile("[ERROR]" + ex.Message);
            }
            if (App.mn == null)
                RunExe("exit()");
            return;
        }

        private async static void SetBthState(bool? bluetoothState)
        {
            try
            {
                var access = await Windows.Devices.Radios.Radio.RequestAccessAsync();
                if (access != Windows.Devices.Radios.RadioAccessStatus.Allowed)
                {
                    App.Notify(new noticeitem(Get_Transalte("bth") + Get_Transalte("dcreject"), 2, Get_Transalte("bth")));
                    return;
                }
                Windows.Devices.Bluetooth.BluetoothAdapter adapter = await Windows.Devices.Bluetooth.BluetoothAdapter.GetDefaultAsync();
                if (null != adapter)
                {
                    var btRadio = await adapter.GetRadioAsync();
                    switch (bluetoothState)
                    {
                        case true:
                            await btRadio.SetStateAsync(Windows.Devices.Radios.RadioState.On);
                            App.Notify(new noticeitem(Get_Transalte("bth") + Get_Transalte("dcon"), 2, Get_Transalte("bth")));
                            break;
                        case false:
                            await btRadio.SetStateAsync(Windows.Devices.Radios.RadioState.Off);
                            App.Notify(new noticeitem(Get_Transalte("bth") + Get_Transalte("dcoff"), 2, Get_Transalte("bth")));
                            break;
                        default:
                            App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("bth")));
                            break;
                    }
                }
                else
                {
                    App.Notify(new noticeitem(Get_Transalte(Get_Transalte("bth") + Get_Transalte("dcnull")), 2, Get_Transalte("bth")));
                }

            }
            catch (Exception ex)
            {
                App.Notify(new noticeitem(Get_Transalte("error"), 2));
                LogtoFile("[ERROR]" + ex.Message);
            }
        }

        private async static void SetWiFiState(int? WiFiState, string? Ssid = null, bool omit = false)
        {
            try
            {
                if (await Windows.Devices.WiFi.WiFiAdapter.RequestAccessAsync() != Windows.Devices.WiFi.WiFiAccessStatus.Allowed)
                {
                    App.Notify(new noticeitem(Get_Transalte("wifi") + Get_Transalte("dcreject"), 2, Get_Transalte("wifi")));
                    return;
                }
                var adapters = await Windows.Devices.WiFi.WiFiAdapter.FindAllAdaptersAsync();
                if (adapters.Count <= 0)
                {
                    App.Notify(new noticeitem(Get_Transalte("wifi") + Get_Transalte("dcnull"), 2, Get_Transalte("wifi")));
                    return;
                }
                var adapter = adapters[0];
                if (null == adapter)
                {
                    App.Notify(new noticeitem(Get_Transalte("wifi") + Get_Transalte("dcnull"), 2, Get_Transalte("wifi")));
                    return;
                }
                Windows.Devices.Radios.Radio? ra = null;
                foreach (var radio in await Windows.Devices.Radios.Radio.GetRadiosAsync())
                {
                    if (radio.Kind == Windows.Devices.Radios.RadioKind.WiFi)
                    {
                        ra = radio;
                        break;
                    }
                }
                if(null == ra)
                {
                    App.Notify(new noticeitem(Get_Transalte("wifi") + Get_Transalte("dcnull"), 2, Get_Transalte("wifi")));
                    return;
                }
                switch (WiFiState)
                {
                    case 0:
                        await ra.SetStateAsync(Windows.Devices.Radios.RadioState.Off);
                        App.Notify(new noticeitem(Get_Transalte("wifi") + Get_Transalte("dcoff"), 2, Get_Transalte("wifi")));
                        break;
                    case 1:
                        await ra.SetStateAsync(Windows.Devices.Radios.RadioState.On);
                        App.Notify(new noticeitem(Get_Transalte("wifi") + Get_Transalte("dcon"), 2, Get_Transalte("wifi")));
                        await adapter.ScanAsync();
                        break;
                    case 2:
                        adapter.Disconnect();
                        App.Notify(new noticeitem(Get_Transalte("wifi") + Get_Transalte("dcdiscon"), 2, Get_Transalte("wifi")));
                        break;
                    case 3:
                        await adapter.ScanAsync();
                        if (adapter.NetworkReport.AvailableNetworks.Count > 0)
                        {
                            var connect = true;
                            Windows.Devices.WiFi.WiFiAvailableNetwork? savedan = null;
                            foreach (var an in adapter.NetworkReport.AvailableNetworks)
                            {
                                if (Ssid != null && an.Ssid.ToLower().Equals(Ssid.ToLower()))
                                {
                                    if (savedan == null || !savedan.Ssid.ToLower().Equals(Ssid.ToLower()))
                                    {
                                        savedan = an;
                                        if (omit)
                                            break;
                                    }
                                    else
                                    {
                                        connect = false;
                                        break;
                                    }
                                    if (an.SecuritySettings.NetworkAuthenticationType.ToString().ToLower().StartsWith("open") && savedan == null)
                                        savedan = an;
                                }
                                else if (an.SecuritySettings.NetworkAuthenticationType.ToString().ToLower().StartsWith("open"))
                                {
                                    savedan = an;
                                    break;
                                }
                            }
                            if (!connect)
                                App.Notify(new noticeitem(Get_Transalte("wifimis").Replace("%s", Ssid), 2, Get_Transalte("wifi")));
                            else
                            {
                                if (savedan == null)
                                    App.Notify(new noticeitem(Get_Transalte("wifina").Replace("%s", Ssid), 2, Get_Transalte("wifi")));
                                else
                                {
                                    await adapter.ConnectAsync(savedan, Windows.Devices.WiFi.WiFiReconnectionKind.Automatic);
                                    if (Ssid != null && !savedan.Ssid.ToLower().Equals(Ssid.ToLower()))
                                        App.Notify(new noticeitem(Get_Transalte("wifi") + Get_Transalte("dcrecon").Replace("%s1", Ssid).Replace("%s2", savedan.Ssid), 2, Get_Transalte("wifi")));
                                    else
                                        App.Notify(new noticeitem(Get_Transalte("wifi") + Get_Transalte("dccon").Replace("%s", savedan.Ssid), 2, Get_Transalte("wifi")));
                                }
                            }
                        }
                        else
                            App.Notify(new noticeitem(Get_Transalte("wifina"), 2, Get_Transalte("wifi")));
                        break;
                    default:
                        App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("wifi")));
                        break;
                }
            }
            catch (Exception ex)
            {
                App.Notify(new noticeitem(Get_Transalte("error"), 2, Get_Transalte("wifi")));
                LogtoFile("[ERROR]" + ex.Message);
            }
        }

        private static void CopyDirectory(string srcdir, string desdir)
        {
            if (srcdir.EndsWith("\\"))
                srcdir = srcdir.Substring(0, srcdir.Length - 1);
            if (desdir.ToLower().StartsWith(srcdir.ToLower()))
            {
                App.Notify(new noticeitem(Get_Transalte("syntax"), 2, Get_Transalte("file")));
                return;
            }
            string desfolderdir = desdir;
            if (!desfolderdir.EndsWith("\\"))
            {
                desfolderdir = desfolderdir + "\\";
            }
            CreateFolder(desfolderdir);
            string[] filenames = System.IO.Directory.GetFileSystemEntries(srcdir);
            foreach (string file in filenames)
            {
                string newdest = desfolderdir + file.Replace(srcdir, "");
                CreateFolder(newdest);
                if (System.IO.Directory.Exists(file))
                    CopyDirectory(file, newdest);
                else
                    System.IO.File.Copy(file, newdest);
            }
        }

        #endregion

        #region Windows Hello
        private async static void NewUser(String AccountId, BackgroundWorker success, BackgroundWorker falied, BackgroundWorker cancel)
        {
            var keyCreationResult = await Windows.Security.Credentials.KeyCredentialManager.RequestCreateAsync(AccountId, Windows.Security.Credentials.KeyCredentialCreationOption.FailIfExists);
            if (keyCreationResult.Status == KeyCredentialStatus.CredentialAlreadyExists)
            {
                //label5.Text = "User Already Created.";
                UserConsentVerificationResult consentResult = await UserConsentVerifier.RequestVerificationAsync(AccountId);
                if (consentResult.Equals(UserConsentVerificationResult.Verified))
                {
                    success.RunWorkerAsync();
                }
                else
                {
                    falied.RunWorkerAsync();
                }
            }
            else if (keyCreationResult.Status == KeyCredentialStatus.Success)
            {

                var userKey = keyCreationResult.Credential;

                var keyAttestationResult = await userKey.GetAttestationAsync();
                /*KeyCredentialAttestationStatus keyAttestationRetryType = 0;*/

                if (keyAttestationResult.Status == KeyCredentialAttestationStatus.Success)
                {
                    success.RunWorkerAsync();
                }
                else if (keyAttestationResult.Status == KeyCredentialAttestationStatus.TemporaryFailure)
                {
                    falied.RunWorkerAsync();
                }
                else if (keyAttestationResult.Status == KeyCredentialAttestationStatus.NotSupported)
                {
                    success.RunWorkerAsync();
                }
            }
            else if (keyCreationResult.Status == KeyCredentialStatus.UserCanceled ||
                keyCreationResult.Status == KeyCredentialStatus.UserPrefersPassword)
            {
                cancel.RunWorkerAsync();
            }
        }
        private async void DeleteUser(String AccountId)
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
        public async static void Register(BackgroundWorker success, BackgroundWorker falied, BackgroundWorker cancel)
        {
            var keyCredentialAvailable = await KeyCredentialManager.IsSupportedAsync();
            if (!keyCredentialAvailable)
            {
                success.RunWorkerAsync();
                return;
            }
            NewUser("N+@" + App.EnvironmentUsername, success, falied, cancel);
            //Auth(null, "aki-helper@" + textBox1.Text);
        }
        #endregion

        #region 动画相关

        #region 模糊动画
        public static void Blur_Animation(int direction, bool animation, System.Windows.Controls.Label label, System.Windows.Window win, System.ComponentModel.BackgroundWorker? bw = null)
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
                System.Windows.Media.Animation.Storyboard? sb = new();
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
        public static void Blur_Out(System.Windows.Controls.Control ct, BackgroundWorker? bw = null)
        {
            if (!Read_Ini(App.dconfig, "config", "ani", "1").Equals("0"))
            {
                ct.Effect = new System.Windows.Media.Effects.BlurEffect()
                {
                    Radius = App.blurradius,
                    RenderingBias = System.Windows.Media.Effects.RenderingBias.Performance
                };
                System.Windows.Media.Animation.Storyboard? sb = new ();
                System.Windows.Media.Animation.DoubleAnimation da = new ();
                da.From = App.blurradius;
                da.To = 0.0;
                da.Duration = TimeSpan.FromMilliseconds(App.blursec);
                System.Windows.Media.Animation.Storyboard.SetTarget(da, ct);
                System.Windows.Media.Animation.Storyboard.SetTargetProperty(da, new System.Windows.PropertyPath("Effect.Radius"));
                sb.Children.Add(da);
                sb.Completed += delegate
                {
                    ct.Effect = null;
                    if (bw != null)
                        bw.RunWorkerAsync();
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
        private static void Set_Animation_Label(double rd, System.Windows.Controls.Label label, System.Windows.Window win)
        {
            label.Effect = new System.Windows.Media.Effects.BlurEffect()
            {
                Radius = rd,
                RenderingBias = System.Windows.Media.Effects.RenderingBias.Performance
            };
            System.Windows.Thickness tn = label.Margin;
            if (win.Width > win.Height)
            {
                tn.Top = -rd;
                tn.Left = -rd * win.Width / win.Height;
            }
            else
            {
                tn.Left = -rd;
                tn.Top = -rd * win.Height / win.Width;
            }
            label.Margin = tn;
            label.Width = win.Width - tn.Left * 2;
            label.Height = win.Height - tn.Top * 2;
        }
        #endregion

        #region 添加double动画
        public static System.Windows.Media.Animation.Storyboard AddDoubleAnimaton(double to, double mstime, System.Windows.DependencyObject value, string PropertyPath, System.Windows.Media.Animation.Storyboard? sb, double? from = null)
        {
            if (sb == null)
                sb = new();
            System.Windows.Media.Animation.DoubleAnimation da;
            if (from != null)
                da = new((double)from, to, TimeSpan.FromMilliseconds(mstime));
            else
                da = new(to, TimeSpan.FromMilliseconds(mstime));
            System.Windows.Media.Animation.Storyboard.SetTarget(da, value);
            System.Windows.Media.Animation.Storyboard.SetTargetProperty(da, new System.Windows.PropertyPath(PropertyPath));
            sb.Children.Add(da);
            sb.FillBehavior = System.Windows.Media.Animation.FillBehavior.Stop;
            sb.Completed += delegate
            {
                sb = null;
            };
            return sb;
        }
        #endregion

        #region 添加thickness动画
        public static System.Windows.Media.Animation.Storyboard AddThicknessAnimaton(System.Windows.Thickness to, double mstime, System.Windows.DependencyObject value, string PropertyPath, System.Windows.Media.Animation.Storyboard? sb, System.Windows.Thickness? from = null)
        {
            if (sb == null)
                sb = new();
            System.Windows.Media.Animation.ThicknessAnimation da;
            if (from != null)
                da = new((System.Windows.Thickness)from, to, TimeSpan.FromMilliseconds(mstime));
            else
                da = new(to, TimeSpan.FromMilliseconds(mstime));
            System.Windows.Media.Animation.Storyboard.SetTarget(da, value);
            System.Windows.Media.Animation.Storyboard.SetTargetProperty(da, new System.Windows.PropertyPath(PropertyPath));
            sb.Children.Add(da);
            sb.FillBehavior = System.Windows.Media.Animation.FillBehavior.Stop;
            sb.Completed += delegate
            {
                sb = null;
            };
            return sb;
        }
        #endregion 

        #region 添加Color动画
        public static System.Windows.Media.Animation.Storyboard AddColorAnimaton(System.Windows.Media.Color to, double mstime, System.Windows.DependencyObject value, string PropertyPath, System.Windows.Media.Animation.Storyboard? sb, System.Windows.Media.Color? from = null)
        {
            if (sb == null)
                sb = new();
            System.Windows.Media.Animation.ColorAnimation da;
            if (from != null)
                da = new((System.Windows.Media.Color)from, to, TimeSpan.FromMilliseconds(mstime));
            else
                da = new(to, TimeSpan.FromMilliseconds(mstime));
            System.Windows.Media.Animation.Storyboard.SetTarget(da, value);
            System.Windows.Media.Animation.Storyboard.SetTargetProperty(da, new System.Windows.PropertyPath(PropertyPath));
            sb.Children.Add(da);
            sb.FillBehavior = System.Windows.Media.Animation.FillBehavior.Stop;
            sb.Completed += delegate
            {
                sb = null;
            };
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
        public static string GetWebContent(String url, bool save = false, String? savepath = null)
        {
            System.Net.Http.HttpRequestMessage request = new(System.Net.Http.HttpMethod.Get, url);
            request.Headers.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.71 Safari/537.36");
            request.Content = new System.Net.Http.StringContent("");
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            request.Options.TryAdd("AllowAutoRedirect", true);
            request.Options.TryAdd("KeppAlive", true);
            request.Options.TryAdd("ProtocolVersion", System.Net.HttpVersion.Version11);
            //这里设置协议
            System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072;// SecurityProtocolType.Tls1.2; 
            System.Net.ServicePointManager.CheckCertificateRevocationList = true;
            System.Net.ServicePointManager.DefaultConnectionLimit = 100;
            System.Net.ServicePointManager.Expect100Continue = false;

            try
            {
                System.Net.Http.HttpResponseMessage response = App.hc.Send(request);
                if (response.Content != null)
                {
                    System.IO.Stream stream = response.Content.ReadAsStream();
                    string result = string.Empty;
                    if (save == true && savepath != null)
                    {
                        try
                        {
                            using (var fileStream = System.IO.File.Create(savepath))
                            {
                                stream.CopyTo(fileStream);
                            }
                            return "saved";
                        }
                        catch (Exception ex)
                        {
                            LogtoFile("[ERROR]" + ex.Message);
                            return "error";
                        }
                    }
                    else
                    {
                        System.IO.StreamReader sr = new(stream);
                        result = sr.ReadToEnd();
                        return result;
                    }
                }
                else
                {
                    LogtoFile("[ERROR]Response is null");
                    return Get_Transalte("error");
                }
            }
            catch (Exception ex)
            {
                LogtoFile("[ERROR]" + ex.Message);
                return Get_Transalte("error");
            }


        }
        private static bool CheckValidationResult()
        {
            return true;
        }
        #endregion

        #region API函数声明

        #region 读文件
        [System.Runtime.InteropServices.DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(byte[] section, byte[] key, byte[] val, string filePath);
        [System.Runtime.InteropServices.DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern int GetPrivateProfileString(byte[] section, byte[] key, byte[] def, byte[] retVal, int size, string filePath);
        #endregion

        #region 窗口拖动
        [System.Runtime.InteropServices.DllImport("user32.dll")]//拖动无窗体的控件
        public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        #endregion

        #region 设置壁纸

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
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

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        public static extern bool SystemParametersInfo(uint uAction, uint uParam, StringBuilder lpvParam, uint init);

        #endregion

        #region 获取用户头像
        [System.Runtime.InteropServices.DllImport("shell32.dll", EntryPoint = "#261",
           CharSet = System.Runtime.InteropServices.CharSet.Unicode, PreserveSig = false)]
        public static extern void GetUserTilePath(
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
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int SetClassLong(IntPtr hwnd, int nIndex, int dwNewLong);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int GetClassLong(IntPtr hwnd, int nIndex);

        public static void SetShadow(IntPtr hwnd)
        {
            SetClassLong(hwnd, GCL_STYLE, GetClassLong(hwnd, GCL_STYLE) | CS_DropSHADOW);
        }

        #endregion

        #region 获取系统电量

        public struct SYSTEM_POWER_STATUS
        {
            public byte ACLineStatus;
            public byte BatteryFlag;
            public byte BatteryLifePercent;
            public byte Reserved1;
            public int BatteryLifeTime;
            public int BatteryFullLifeTime;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern bool GetSystemPowerStatus(out SYSTEM_POWER_STATUS lpSystemPowerStatus);
        #endregion

        #region 隐藏鼠标 0/1 隐藏/显示
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern void ShowCursor(int status);
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
                System.Globalization.DateTimeFormatInfo dtFormat = new();
                dtFormat.ShortDatePattern = "yyyy/MM/dd HH:mm:ss";
                try
                {
                    DateTime dt = Convert.ToDateTime(App.scheduleitems[id].Time, dtFormat);
                    switch (App.scheduleitems[id].re)
                    {
                        case -2.0:
                            break;
                        case -1.0:
                            App.scheduleitems[id].Time = dt.AddDays(1.0).ToString("yyyy/MM/dd HH:mm:ss");
                            Write_Ini(App.sconfig, (id + 1).ToString(), "time", App.scheduleitems[id].Time);
                            break;
                        case 0.0:
                            App.scheduleitems[id].Time = dt.AddDays(7.0).ToString("yyyy/MM/dd HH:mm:ss");
                            Write_Ini(App.sconfig, (id + 1).ToString(), "time", App.scheduleitems[id].Time);
                            break;
                        default:
                            App.scheduleitems[id].Time = dt.AddDays(Math.Abs(App.scheduleitems[id].re)).ToString("yyyy/MM/dd HH:mm:ss");
                            Write_Ini(App.sconfig, (id + 1).ToString(), "time", App.scheduleitems[id].Time);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LogtoFile("[ERROR]" + ex.Message);
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
                    Write_Ini(inipath, (id + 1).ToString(), "name", Read_Ini(inipath, (id + 2).ToString(), "name", " "));
                    Write_Ini(inipath, (id + 1).ToString(), "command", Read_Ini(inipath, (id + 2).ToString(), "command", " "));
                    Write_Ini(inipath, (id + 1).ToString(), "time", Read_Ini(inipath, (id + 2).ToString(), "time", " "));
                    id++;
                    System.Windows.Forms.Application.DoEvents();
                }
                Write_Ini(inipath, (id + 1).ToString(), "name", " ");
                Write_Ini(inipath, (id + 1).ToString(), "command", " ");
                Write_Ini(inipath, (id + 1).ToString(), "time", " ");
                App.scheduleitems.RemoveAt(id);
            }
            else
                App.Notify(new noticeitem(Get_Transalte("alarmmissing"), 2, Get_Transalte("schedule")));

        }

        public static void Delay_Alarm(int id)
        {
            if (id > -1)
                App.scheduleitems[id].Time = DateTime.Now.AddMinutes(5.0).ToString("yyyy/MM/dd HH:mm:ss");
            else
                App.Notify(new noticeitem(Get_Transalte("alarmmissing"), 2, Get_Transalte("schedule")));
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
                    if (Environment.ProcessPath != null)
                    {
                        string strName = "\"" + Environment.ProcessPath + "\"";//获取要自动运行的应用程序名
                        if (!System.IO.File.Exists(strName))//判断要自动运行的应用程序文件是否存在
                            return;
                        Microsoft.Win32.RegistryKey? registry = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//检索指定的子项
                        if (registry == null)//若指定的子项不存在
                            registry = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");//则创建指定的子项
                        registry.SetValue("Hiro_Autostart", strName + " silent");//设置该子项的新的“键值对”
                        Write_Ini(App.dconfig, "config", "autorun", "1");
                        LogtoFile("[HIROWEGO]Enable Autorun");
                    }

                }
                catch (Exception ex)
                {
                    LogtoFile("[ERROR]" + ex.Message);
                }
            }
            else
            {
                if (Environment.ProcessPath != null)
                {
                    string strName = "\"" + Environment.ProcessPath + "\"";//获取要自动运行的应用程序名
                    if (!System.IO.File.Exists(strName))//判断要取消的应用程序文件是否存在
                        return;
                    Microsoft.Win32.RegistryKey? registry = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//读取指定的子项
                    if (registry == null)//若指定的子项不存在
                    {
                        return;
                    }
                    registry = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");//则创建指定的子项
                    registry.DeleteValue("Hiro_Autostart", false);//删除指定“键名称”的键/值对
                    Write_Ini(App.dconfig, "config", "autorun", "0");
                    LogtoFile("[HIROWEGO]Disable Autorun");
                }
            }
        }
        #endregion

        #region 新建完全限定路径文件夹
        public static bool CreateFolder(string path)
        {
            int pos = path.IndexOf("\\") + 1;
            string vpath;
            System.IO.DirectoryInfo? di;
            try
            {
                while (pos > 0)
                {
                    vpath = path.Substring(0, pos);
                    pos = path.IndexOf("\\", pos) + 1;
                    di = new System.IO.DirectoryInfo(vpath);
                    if (!di.Exists)
                        di.Create();
                }
            }
            catch (Exception ex)
            {
                utils.LogtoFile("[ERROR]" + ex.Message);
                return false;
            }
            return true;
            
        }
        #endregion

        #region 获取系统主题色

        public static void IntializeColorParameters()
        {
            if (utils.Read_Ini(App.dconfig, "config", "lock", "0").Equals("1"))
            {
                try
                {
                    App.AppAccentColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(utils.Read_Ini(App.dconfig, "config", "lockcolor", "#00C4FF"));

                }
                catch (Exception ex)
                {
                    utils.LogtoFile("[ERROR]" + ex.Message);
                    App.AppAccentColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#00C4FF");
                }
            }
            else
            {
                App.AppAccentColor = GetThemeColor();
            }
            double luminance = (0.299 * App.AppAccentColor.R + 0.587 * App.AppAccentColor.G + 0.114 * App.AppAccentColor.B) / 255;
            if (luminance > 0.5)
            {
                if (utils.Read_Ini(App.dconfig, "config", "reverse", "0").Equals("1"))
                    App.AppForeColor = System.Windows.Media.Colors.White;
                else
                    App.AppForeColor = System.Windows.Media.Colors.Black;
            }
            else
            {
                if (utils.Read_Ini(App.dconfig, "config", "reverse", "0").Equals("1"))
                    App.AppForeColor = System.Windows.Media.Colors.Black;
                else
                    App.AppForeColor = System.Windows.Media.Colors.White;
            }


            utils.LogtoFile("[HIROWEGO]Accent Color: " + App.AppAccentColor.ToString());
            utils.LogtoFile("[HIROWEGO]Fore Color: " + App.AppForeColor.ToString());
        }

        [System.Runtime.InteropServices.DllImport("uxtheme.dll", EntryPoint = "#95")]
        public static extern uint GetImmersiveColorFromColorSetEx(uint dwImmersiveColorSet, uint dwImmersiveColorType, bool bIgnoreHighContrast, uint dwHighContrastCacheMode);
        [System.Runtime.InteropServices.DllImport("uxtheme.dll", EntryPoint = "#96")]
        public static extern uint GetImmersiveColorTypeFromName(IntPtr pName);
        [System.Runtime.InteropServices.DllImport("uxtheme.dll", EntryPoint = "#98")]
        public static extern int GetImmersiveUserColorSetPreference(bool bForceCheckRegistry, bool bSkipCheckOnFail);
        // Get theme color
        public static System.Windows.Media.Color GetThemeColor()
        {
            var colorSetEx = GetImmersiveColorFromColorSetEx(
                (uint)GetImmersiveUserColorSetPreference(false, false),
                GetImmersiveColorTypeFromName(System.Runtime.InteropServices.Marshal.StringToHGlobalUni("ImmersiveStartSelectionBackground")),
                false, 0);

            var colour = System.Windows.Media.Color.FromArgb((byte)((0xFF000000 & colorSetEx) >> 24), (byte)(0x000000FF & colorSetEx),
                (byte)((0x0000FF00 & colorSetEx) >> 8), (byte)((0x00FF0000 & colorSetEx) >> 16));

            return colour;
        }
        #endregion

        public static string DeleteUnVisibleChar(string sourceString)
        {
            System.Text.StringBuilder sBuilder = new System.Text.StringBuilder(131);
            for (int i = 0; i < sourceString.Length; i++)
            {
                int Unicode = sourceString[i];
                if (Unicode >= 16)
                {
                    sBuilder.Append(sourceString[i].ToString());
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

    }


}
