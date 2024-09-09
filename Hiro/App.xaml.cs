using Hiro.ModelViews;
using Hiro.Resources;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Windows.Foundation.Metadata;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;
using Hiro.Helpers;
using static Hiro.Helpers.HClass;
using static Hiro.Helpers.HText;
using static Hiro.Helpers.HSet;
using static Hiro.Helpers.HLogger;
using System.Windows;
using System.Windows.Media;
using Hiro.Widgets;
using Hiro.Helpers.Database;

namespace Hiro
{

    public partial class App : System.Windows.Application
    {
        #region 全局参数
        internal static string lang = "en-US";
        internal static string appTitle = Hiro_Resources.ApplicationName;
        internal static string eUserName = Environment.UserName;
        internal static string username = Environment.UserName;
        internal static string sConfig = "C:\\";
        internal static string dConfig = "C:\\";
        internal static string currentDir = "C:\\";
        internal static string logFilePath = "C:\\1.log";
        internal static string langFilePath = "C:\\1.hlp";
        internal static int CustomUsernameFlag = 0;
        internal static System.Windows.Media.Color AppAccentColor = System.Windows.Media.Colors.Coral;
        internal static System.Windows.Media.Color AppForeColor = System.Windows.Media.Colors.White;
        internal static System.Collections.ObjectModel.ObservableCollection<Scheduleitem> scheduleitems = new();
        internal static System.Collections.ObjectModel.ObservableCollection<Hiro_AlarmWin> aw = new();
        internal static System.Collections.ObjectModel.ObservableCollection<Language> la = new();
        internal static bool Locked = true;
        internal static MainWindow? wnd;
        internal static Hiro_MainUI? mn = null;
        internal static Hiro_Notification? noti = null;
        internal static Hiro_Island? hisland = null;
        internal static Hiro_Box? hiBox = null;
        internal static Hiro_Boxie? hiBoxie = null;
        internal static Hiro_Editor? ed = null;
        internal static Hiro_LockScreen? ls = null;
        internal static Hiro_Taskbar? tb = null;
        internal static List<Hiro_Notice> noticeitems = new();
        internal static double blurradius = 50.0;
        internal static double blursec = 500.0;
        internal static byte trval = 160;
        internal static System.Windows.Threading.DispatcherTimer? timer;
        internal static System.Collections.ObjectModel.ObservableCollection<Cmditem> cmditems = new();
        internal static int page = 0;
        internal static bool dflag = false;
        internal static System.Net.Http.HttpClient? hc = null;
        internal static int ColorCD = -1;
        internal static int ChatCD = 5;
        internal static IntPtr WND_Handle = IntPtr.Zero;
        internal static bool FirstUse = false;
        internal static bool? Logined = false;
        internal static string loginedUser = string.Empty;
        internal static string loginedToken = string.Empty;
        internal static System.Threading.Thread? serverThread = null;
        internal static int flashFlag = -1;
        internal static DateTimeOffset formerTime = DateTimeOffset.Now;
        internal static DateTime formerT = DateTime.Now;
        //internal static PrivateFontCollection pf = new();
        internal static Dictionary<string, string> pfIndex = new();
        private static Dictionary<string, string> times = new()
        {
            {"morning",string.Empty},
            {"noon",string.Empty},
            {"afternoon",string.Empty},
            {"evening",string.Empty},
            {"night",string.Empty},
        };
        private static UserNotificationListener? listener = null;
        #endregion

        #region 私有参数
        private static bool AutoChat = false;
        #endregion

        private async void Hiro_We_Go(object sender, System.Windows.StartupEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                dflag = true;
            }
            else
            {
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
                DispatcherUnhandledException += App_DispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            }
            InitializeInnerParameters();
            InitializeStartParameters(e);
            await Task.Run(() =>
            {
                Initialize_Notify_Recall();
                Initialize_NotificationListener();
            });
            DB_HIRO_PLUGINS.InitializePlugins();
            Hiro_Utils.SetFrame(Convert.ToInt32(double.Parse(Read_DCIni("FPS", "60"))));
            Unosquare.FFME.Library.FFmpegDirectory = @Path_PPX("<current>") + @"\runtimes\win-x64\ffmpeg";
        }

        internal static void LoadDictionaryColors()
        {
            Application.Current.Resources["Hiro.Colors.Accent"] = new SolidColorBrush(AppAccentColor);
            Application.Current.Resources["Hiro.Colors.Accent.Dim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(AppAccentColor, 200));
            Application.Current.Resources["Hiro.Colors.Accent.Null"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(AppAccentColor, trval));
            Application.Current.Resources["Hiro.Colors.Accent.Disabled"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(AppAccentColor == Colors.White ? Colors.Black : Colors.White, 100));
            Application.Current.Resources["Hiro.Colors.Accent.Null.MouseOver"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(AppAccentColor, trval * 7 / 8));
            Application.Current.Resources["Hiro.Colors.Accent.Null.Pressed"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(AppAccentColor, trval * 5 / 4));
            Application.Current.Resources["Hiro.Colors.Accent.Null.Disabled"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(AppAccentColor == Colors.White ? Colors.Black : Colors.White, 100));
            Application.Current.Resources["Hiro.Colors.Fore"] = new SolidColorBrush(AppForeColor);
            Application.Current.Resources["Hiro.Colors.Fore.Dim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(AppForeColor, 200));
            Application.Current.Resources["Hiro.Colors.Fore.ExtraDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(AppForeColor, 100));
            Application.Current.Resources["Hiro.Colors.Fore.Disabled"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(AppForeColor, 50));
        }

        internal static string AddCustomFont(string path)
        {
            var p = Path_PPX(path).Trim();
            if (System.IO.File.Exists(p))
            {
                if (pfIndex.ContainsKey(p))
                {
                    return pfIndex[p];
                }
                else
                {
                    //pf.AddFontFile(p);
                    System.Windows.Media.GlyphTypeface glyphTypeface = new(new Uri(p));
                    string ffi = glyphTypeface.Win32FamilyNames[new System.Globalization.CultureInfo("en-us")];
                    if (dflag)
                        LogtoFile($"[Font]Font Added Location:{p}#{ffi} Index:{pfIndex.Count}");
                    pfIndex.Add(p, p + "#" + ffi);
                    return p + "#" + ffi;
                }
            }
            return string.Empty;
        }

        async private void Initialize_NotificationListener()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.Notifications.Management.UserNotificationListener"))
            {
                if (Read_DCIni("MonitorSys", "true").Equals("true", StringComparison.CurrentCultureIgnoreCase) && Read_DCIni("Toast", "0").Equals("1"))
                    Write_Ini(dConfig, "Config", "Toast", "0");
                listener = UserNotificationListener.Current;
                // And request access to the user's notifications (must be called from UI thread)
                UserNotificationListenerAccessStatus accessStatus = await listener.RequestAccessAsync();
                switch (accessStatus)
                {
                    case UserNotificationListenerAccessStatus.Allowed:
                        {
                            listener = UserNotificationListener.Current;
                            LogtoFile("[INFO]Notification Listener Enabled");
                            break;
                        }
                    case UserNotificationListenerAccessStatus.Denied:
                        LogtoFile("[INFO]Notification Listener Disabled");
                        break;
                    case UserNotificationListenerAccessStatus.Unspecified:
                        LogtoFile("[INFO]Notification Listener Unspecified");
                        break;
                }
            }
            else
            {
                LogtoFile("Notification Listener Not Supported");
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = e.ExceptionObject as Exception;
                if (exception != null)
                {
                    LogError(exception, "Hiro.Exception.UnhandledException");
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "Hiro.Exception.UnhandledException.Catch");
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                LogError(e.Exception, "Hiro.Exception.Dispatcher");
            }
            catch (Exception ex)
            {
                LogError(ex, "Hiro.Exception.Dispatcher.Catch");
            }
            finally
            {
                e.Handled = true;
            }

        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                var exception = e.Exception as Exception;
                if (exception != null)
                {
                    LogError(exception, "Hiro.Exception.UnobservedTask");
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "Hiro.Exception.UnobservedTask.Catch");
            }
            finally
            {
                e.SetObserved();
            }

        }

        private static void Socket_Communication(Socket socketLister, System.Collections.Hashtable clientSessionTable, object clientSessionLock)
        {
            Socket clientSocket = socketLister.Accept();
            Hiro_Socket clientSession = new(clientSocket);
            lock (clientSessionLock)
            {
                if (!clientSessionTable.ContainsKey(clientSession.IP))
                {
                    clientSessionTable.Add(clientSession.IP, clientSession);
                }
            }
            SocketConnection socketConnection = new(clientSocket);
            socketConnection.DataReceiveCompleted += delegate
            {
                var recStr = DeleteUnVisibleChar(socketConnection.receivedMsg);
                var outputb = Convert.FromBase64String(recStr);
                recStr = Encoding.Default.GetString(outputb);
                recStr = Path_PPX(recStr).Trim();
                if (System.IO.File.Exists(recStr))
                {
                    HiroApp ha = new();
                    ha.msg = Read_Ini(recStr, "App", "Msg", "nop");
                    ha.appID = Read_Ini(recStr, "App", "ID", "null");
                    ha.appName = Read_Ini(recStr, "App", "Name", "null");
                    ha.appPackage = Read_Ini(recStr, "App", "Package", "null");
                    Current.Dispatcher.Invoke(delegate
                    {
                        Hiro_Utils.RunExe(ha.msg, ha.appName);
                    });
                    LogtoFile("[SERVER]" + ha.ToString());
                }
                else
                {
                    var recStrs = recStr.Split("\t");
                    if (recStrs.Length >= 4)
                    {

                        HiroApp ha = new();
                        ha.msg = recStrs[3];
                        ha.appID = recStrs[0];
                        ha.appName = recStrs[2];
                        ha.appPackage = recStrs[1];
                        Current.Dispatcher.Invoke(delegate
                        {
                            Hiro_Utils.RunExe(ha.msg, ha.appName);
                        });
                        LogtoFile("[SERVER]" + ha.ToString());
                    }
                    else
                        LogtoFile("[SERVER]" + recStr);
                }
                StartServerListener(socketLister, clientSessionTable, clientSessionLock);
            };
            socketConnection.ReceiveData();
        }

        private static void StartServerListener(Socket socketLister, System.Collections.Hashtable clientSessionTable, object clientSessionLock)
        {
            serverThread?.Interrupt();
            serverThread = new System.Threading.Thread(() =>
            {
                Socket_Communication(socketLister, clientSessionTable, clientSessionLock);
            });
            serverThread.Start();
        }

        private static void Build_Socket()
        {
            var port = Hiro_Utils.GetRandomUnusedPort();
            int MaxConnection = 69;
            System.Collections.Hashtable clientSessionTable = new();
            object clientSessionLock = new();
            System.Net.IPEndPoint localEndPoint = new(System.Net.IPAddress.Any, port);
            Socket socketLister = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketLister.Bind(localEndPoint);
            Write_Ini(dConfig, "Config", "Port", port.ToString());
            try
            {
                socketLister.Listen(MaxConnection);
                StartServerListener(socketLister, clientSessionTable, clientSessionLock);
            }
            catch (Exception ex)
            {
                LogError(ex, "Hiro.Exception.Socket");
            }
        }

        private static void InitializeStartParameters(System.Windows.StartupEventArgs e)
        {
            switch (e.Args.Length)
            {
                case >= 1 when e.Args[0].ToLower().Equals("autostart_on"):
                    Hiro_Utils.Set_Autorun(true);
                    goto Executed;
                case >= 1 when e.Args[0].ToLower().Equals("autostart_off"):
                    Hiro_Utils.Set_Autorun(false);
                    goto Executed;
                default:
                    {
                        bool silent = false;
                        bool create = true;
                        bool autoexe = true;
                        if (e.Args.Length >= 1)
                        {
                            foreach (var para in e.Args)
                            {
                                switch (para.ToLower())
                                {
                                    case "debug":
                                        dflag = true;
                                        break;
                                    case "silent":
                                        silent = true;
                                        break;
                                    case "utils":
                                        create = false;
                                        break;
                                    case "update":
                                        autoexe = false;
                                        break;
                                    case "pure":
                                        autoexe = false;
                                        break;
                                    default:
                                        Hiro_Utils.IntializeColorParameters();
                                        LoadDictionaryColors();
                                        Hiro_Utils.RunExe(para, "Windows");
                                        return;
                                }
                            }
                        }
                        wnd = new MainWindow();
                        wnd.InitializeInnerParameters();
                        wnd.Load_All_Colors();
                        wnd.Show();
                        wnd.Hide();
                        if (create)
                        {
                            _ = new System.Threading.Mutex(true, "0x415417hiro", out bool ret);
                            if (!ret)
                            {
                                Hiro_Utils.RunExe("exit()");
                                return;
                            }
                            mn = new();
                            if (silent)
                                LogtoFile("[HIROWEGO]Silent Start");
                            else
                                mn.Show();
                            if (Read_DCIni("AutoLogin", "false").Equals("true", StringComparison.CurrentCultureIgnoreCase))
                            {
                                LogtoFile("[INFO]AutoLogin enabled");
                                new System.Threading.Thread(() =>
                                {
                                    Logined = null;
                                    var tmp = System.IO.Path.GetTempFileName();
                                    var r = Helpers.HID.Login(loginedUser, loginedToken, true, tmp);
                                    if (r.Equals("success") && Read_Ini(tmp, "Login", "res", "-1").Equals("0"))
                                    {
                                        Logined = true;
                                        LogtoFile("[INFO]AutoLogin as " + loginedUser);
                                        Logined = true;
                                    }
                                    else
                                    {
                                        LogtoFile("[INFO]AutoLogin Failed " + Read_Ini(tmp, "Config", "msg", string.Empty) + r.ToString());
                                        Logined = false;
                                    }
                                    if (System.IO.File.Exists(tmp))
                                        System.IO.File.Delete(tmp);
                                    if (Logined == true)
                                        HID.SyncProfile(loginedUser, loginedToken);

                                }).Start();
                            }
                            if (FirstUse)
                            {
                                FirstUse = false;
                                Hiro_Utils.RunExe("message(<current>\\users\\default\\app\\" + lang + "\\welcome.hws)", Get_Translate("infofirst").Replace("%h", appTitle));
                            }
                            if (autoexe)
                            {
                                if (Read_DCIni("AutoExe", "false").Equals("true", StringComparison.CurrentCultureIgnoreCase))
                                    Hiro_Utils.RunExe(Read_DCIni("AutoAction", "nop"), Get_Translate("autoexe"));
                            }
                            InitializeMethod();
                            Build_Socket();
                        }
                        return;
                    }
            }
        Executed:
            Hiro_Utils.RunExe("exit()");
            return;
        }

        internal static void Notify(Hiro_Notice i)
        {
            string title = appTitle;
            i.msg = Path_Prepare_EX(i.msg);
            if (i.title != null)
                title = Path_Prepare_EX(i.title);
            int disturb = int.Parse(Read_DCIni("Disturb", "2"));
            if ((disturb == 1 && !Hiro_Utils.IsForegroundFullScreen()) || (disturb != 1 && disturb != 0))
            {
                if (HWin.IsWindows10() && Read_DCIni("Toast", "0").Equals("1"))
                {
                    Hiro_Utils.HiroInvoke(() =>
                    {
                        var _h = Path_PPX(i.icon?.HeroImage ?? "");
                        if (File.Exists(_h))
                        {
                            try
                            {
                                new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                                        .AddText(title)
                                        .AddText(i.msg.Replace("\\n", Environment.NewLine))
                                        .AddArgument("category", "notification")
                                        .AddHeroImage(new Uri(_h))
                                        .Show();
                            }
                            catch (Exception ex)
                            {
                                HLogger.LogError(ex, "Hiro.Exception.Toast.HeroKind");
                            }

                        }
                        else
                        {
                            new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                                        .AddText(title)
                                        .AddText(i.msg.Replace("\\n", Environment.NewLine))
                                        .AddArgument("category", "notification")
                                        .Show();
                        }
                    });

                }
                else if (Read_DCIni("Toast", "0").Equals("2") || Read_DCIni("Toast", "0").Equals("3"))
                {
                    noticeitems.Add(i);
                    Hiro_Utils.HiroInvoke(() =>
                    {
                        hisland ??= new();
                        hisland.Show();
                    });
                }
                else if (Read_DCIni("Toast", "0").Equals("4"))
                {
                    noticeitems.Add(i);
                    Hiro_Utils.HiroInvoke(() =>
                    {
                        hiBox ??= new();
                        hiBox.Show();
                    });
                }
                else if (Read_DCIni("Toast", "0").Equals("5"))
                {
                    noticeitems.Add(i);
                    Hiro_Utils.HiroInvoke(() =>
                    {
                        hiBoxie ??= new();
                        hiBoxie.Show();
                    });
                }
                else if (Read_DCIni("Toast", "0").Equals("6"))
                {
                    noticeitems.Add(i);
                    Hiro_Utils.HiroInvoke(() =>
                    {
                        tb ??= new();
                        tb.Show();
                        tb.Load_Notification();
                    });
                }
                else
                {
                    noticeitems.Add(i);
                    Hiro_Utils.HiroInvoke(() =>
                    {
                        if (noti == null)
                        {
                            noti = new();
                            noti.Show();
                        }
                        else
                        {
                            if (noti.flag[0] == 2)
                            {
                                noti.flag[0] = 0;
                                noti.timer.Interval = TimeSpan.FromSeconds(1);
                                noti.timer.Start();
                            }
                        }
                    });
                }
            }
            else
            {
                mn?.AddToInfoCenter(
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + Environment.NewLine
                        + "\t" + Get_Translate("infocmd") + ":" + "\tnotify(" + i.msg + ")" + Environment.NewLine
                        + "\t" + Get_Translate("infosource") + ":" + "\t" + i.title + Environment.NewLine);
            }
            LogtoFile("[NOTIFICATION]" + i.msg);
        }

        private static void Initialize_Notify_Recall()
        {
            Microsoft.Toolkit.Uwp.Notifications.ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                var args = Microsoft.Toolkit.Uwp.Notifications.ToastArguments.Parse(toastArgs.Argument);
                var mo = 1;//0 for alarm 1 for notification
                for (int i = 0; i < args.Count; i++)
                {
                    if (args.ElementAt(i).Key.Equals("category") && args.ElementAt(i).Value.Equals("alarm"))
                    {
                        mo = 0;
                        break;
                    }
                }
                if (mo == 0)
                {
                    var action = "null";
                    var url = "nop";
                    var id = -1;
                    for (int i = 0; i < args.Count; i++)
                    {
                        if (args.ElementAt(i).Key.Equals("action"))
                            action = args.ElementAt(i).Value;
                        if (args.ElementAt(i).Key.Equals("alarmid"))
                            id = int.Parse(args.ElementAt(i).Value);
                        if (args.ElementAt(i).Key.Equals("url"))
                            url = args.ElementAt(i).Value;
                    }
                    if (!action.Equals("null") && !id.Equals("-1"))
                    {
                        switch (action)
                        {
                            case "delay":
                                if (id > -1)
                                {
                                    Hiro_Utils.Delay_Alarm(id);
                                }
                                break;
                            case "ok":
                                if (id > -1)
                                {
                                    Hiro_Utils.OK_Alarm(id);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    if (!action.Equals("null") && !url.Equals("nop"))
                    {
                        switch (action)
                        {
                            case "uok":
                                Hiro_Utils.RunExe(url, Get_Translate("alarm"));
                                break;
                            case "uskip":
                                break;
                            default:
                                break;
                        }
                    }
                }
                else if (mo == 1)
                {
                    ///TO-DO:目前不支持系统样式触发
                }
            };
        }

        private static void InitializeInnerParameters()
        {
            currentDir = AppDomain.CurrentDomain.BaseDirectory;
            HFile.CreateFolder(currentDir + "\\users\\" + eUserName + "\\editor\\");
            HFile.CreateFolder(currentDir + "\\users\\" + eUserName + "\\log\\");
            HFile.CreateFolder(currentDir + "\\users\\" + eUserName + "\\config\\");
            HFile.CreateFolder(currentDir + "\\users\\" + eUserName + "\\app\\");
            HFile.CreateFolder(currentDir + "\\system\\lang\\");
            HFile.CreateFolder(currentDir + "\\system\\wallpaper\\");
            logFilePath = currentDir + "\\users\\" + eUserName + "\\log\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            try
            {
                File.Delete(logFilePath);
            }
            catch { }
            LogtoFile("[HIROWEGO]InitializeInnerParameters");
            dConfig = currentDir + "\\users\\" + eUserName + "\\config\\" + eUserName + ".hus";
            sConfig = currentDir + "\\users\\" + eUserName + "\\config\\" + eUserName + ".hsl";
            dConfig = dConfig.Replace("\\\\", "\\");
            sConfig = sConfig.Replace("\\\\", "\\");
            LogtoFile("[HIROWEGO]DConfig at " + dConfig);
            LogtoFile("[HIROWEGO]SConfig at " + sConfig);
            FirstUse = !System.IO.File.Exists(dConfig) && !System.IO.File.Exists(sConfig);
            var str = Read_DCIni("Lang", "");
            if (str.Equals("") || str.Equals("default"))
            {
                lang = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
                if (!System.IO.File.Exists(currentDir + "\\system\\lang\\" + lang + ".hlp"))
                {
                    lang = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToString();
                    LogError(new System.IO.FileNotFoundException(), "Hiro.Exception.Globalization");
                    if (!System.IO.File.Exists(currentDir + "\\system\\lang\\" + lang + ".hlp"))
                    {
                        lang = "zh-CN";
                    }
                }
            }
            else
            {
                lang = str;
                if (!System.IO.File.Exists(currentDir + "\\system\\lang\\" + lang + ".hlp"))
                {
                    if (str.IndexOf("-") != -1)
                        lang = str[..str.IndexOf("-")];
                    LogError(new System.IO.FileNotFoundException(), "Hiro.Exception.Globalization");
                    if (!System.IO.File.Exists(currentDir + "\\system\\lang\\" + lang + ".hlp"))
                    {
                        lang = "zh-CN";
                    }
                }
            }
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(lang);
            LogtoFile("[HIROWEGO]Current language: " + lang);
            langFilePath = currentDir + "\\system\\lang\\" + lang + ".hlp";
            appTitle = Read_DCIni("CustomNick", "false").Equals("true", StringComparison.CurrentCultureIgnoreCase) ? Read_DCIni("CustomHIRO", "Hiro") : Hiro_Resources.ApplicationName;
            System.IO.DirectoryInfo di = new(currentDir + "\\system\\lang\\");
            foreach (System.IO.FileInfo fi in di.GetFiles())
            {
                if (fi.Name.ToLower().EndsWith(".hlp"))
                {
                    string langname = Read_Ini(fi.FullName, "Translate", "Lang", "null");
                    string name = fi.Name.Replace(".hlp", "");
                    if (!langname.Equals("null"))
                    {
                        la.Add(new Language(name, langname));
                    }
                }
            }
            switch (Read_DCIni("CustomUser", "false"))
            {
                case "true":
                    CustomUsernameFlag = 1;
                    username = Read_DCIni("CustomName", "");
                    break;
                default:
                    CustomUsernameFlag = 0;
                    username = eUserName;
                    break;
            }
            HID.version = "v" + Read_DCIni("AppVer", "1");
            if (Read_DCIni("CustomNick", "false").Equals("true", StringComparison.CurrentCultureIgnoreCase))
                appTitle = Read_DCIni("CustomHIRO", "Hiro");
            if (Read_DCIni("TRBtn", "false").Equals("true", StringComparison.CurrentCultureIgnoreCase))
                trval = 0;
            if (Read_Ini(dConfig, "Network", "Proxy", "0").Equals("1"))//IE Proxy
            {
                hc = new();
            }
            else if (Read_Ini(dConfig, "Network", "Proxy", "0").Equals("2"))//Proxy
            {
                try
                {
                    System.Net.Http.HttpClientHandler hch = new();
                    int ProxyPort = int.Parse(Read_Ini(dConfig, "Network", "Port", string.Empty));
                    hch.Proxy = new System.Net.WebProxy(Read_Ini(dConfig, "Network", "Server", string.Empty), ProxyPort);
                    hch.UseProxy = true;
                    string? ProxyUsername = Read_Ini(dConfig, "Network", "Username", string.Empty);
                    string? ProxyPwd = Read_Ini(dConfig, "Network", "Password", string.Empty);
                    ProxyUsername = ProxyUsername.Equals(string.Empty) ? null : ProxyUsername;
                    ProxyPwd = ProxyPwd.Equals(string.Empty) ? null : ProxyPwd;
                    hch.PreAuthenticate = true;
                    hch.UseDefaultCredentials = false;
                    hch.Credentials = new System.Net.NetworkCredential(ProxyUsername, ProxyPwd);
                    hc = new(hch);
                }
                catch (Exception ex)
                {
                    LogError(ex, "Hiro.Exception.Proxy");
                    hc = new();
                }
            }
            else
            {
                System.Net.Http.HttpClientHandler hch = new();
                hch.UseProxy = false;
                hc = new(hch);
            }
            LogtoFile("[DEVICE]Current OS: " + Hiro_Utils.Get_OSVersion());
            loginedUser = Read_DCIni("User", string.Empty);
            loginedToken = Read_DCIni("Token", string.Empty);
            AutoChat = Read_DCIni("AutoChat", "true").Equals("true", StringComparison.CurrentCultureIgnoreCase);
        }

        private static void InitializeMethod()
        {
            Load_LocalTime();
            timer = new()
            {
                Interval = Read_DCIni("Performance", "0") switch
                {
                    "1" => TimeSpan.FromSeconds(3),
                    "2" => TimeSpan.FromSeconds(10),
                    _ => TimeSpan.FromSeconds(1)
                }
            };
            switch (Read_DCIni("Performance", "0"))
            {
                case "1":
                    {
                        HWin.SetProcessPriority("idle", null, true);
                        break;
                    }
                case "2":
                    {
                        HWin.SetProcessPriority("idle", null, true);
                        HWin.SetEffiencyMode("on", null, true);
                        break;
                    }
            }
            timer.Tick += delegate
            {
                TimerTick();
            };
            timer.Start();
        }

        internal static void Load_LocalTime()
        {
            var morning = Read_Ini(langFilePath, "local", "morning", "[6,7,8,9,10]");
            var noon = Read_Ini(langFilePath, "local", "noon", "[11,12,13]");
            var afternoon = Read_Ini(langFilePath, "local", "afternoon", "[14,15,16,17,18]");
            var evening = Read_Ini(langFilePath, "local", "evening", "[19,20,21,22]");
            var night = Read_Ini(langFilePath, "local", "night", "[23,0,1,2,3,4,5]");
            morning = morning.Replace("[", "[,").Replace("]", ",]").Trim();
            noon = noon.Replace("[", "[,").Replace("]", ",]").Trim();
            afternoon = afternoon.Replace("[", "[,").Replace("]", ",]").Trim();
            evening = evening.Replace("[", "[,").Replace("]", ",]").Trim();
            night = night.Replace("[", "[,").Replace("]", ",]").Trim();
            times["morning"] = morning;
            times["noon"] = noon;
            times["afternoon"] = afternoon;
            times["evening"] = evening;
            times["night"] = night;
        }

        public static void TimerTick()
        {
            #region Hello
            var current = DateTime.Now;
            if (mn != null)
            {
                var hr = current.Hour;
                mn.Set_Home_Labels(times.Where(x => x.Value.IndexOf("," + hr + ",") != -1).FirstOrDefault().Key);
                #endregion
                if (AutoChat && Logined == true)
                {
                    if (mn.hiro_chat == null)
                    {
                        HLogger.LogtoFile("[INFO]AutoChat enabled");
                        mn.hiro_chat ??= new(mn);
                        mn.hiro_chat.Load_Friend_Info_First();
                    }
                    else
                    {
                        ChatCD--;
                        if (ChatCD == 0)
                        {
                            mn.hiro_chat?.Hiro_Get_Chat();
                            ChatCD = Read_DCIni("Performance", "0") switch
                            {
                                "1" => 10,
                                "2" => 20,
                                _ => 5
                            };
                        }
                    }

                }
            }
            if (ls != null)
            {
                ls.timelabel.Content = current.ToString(Read_Ini(langFilePath, "local", "locktime", "HH:mm"));
                ls.datelabel.Content = current.ToString(Read_Ini(langFilePath, "local", "lockdate", "MM/dd (ddd)"));
            }
            var i = 1;
            while (i <= scheduleitems.Count)
            {
                System.Windows.Forms.Application.DoEvents();
                var scTime = DateTime.Parse(scheduleitems[i - 1].Time);
                if (DateTime.Compare(formerT, scTime) < 0 && DateTime.Compare(current, scTime) > 0)
                {
                    int disturb = int.Parse(Read_DCIni("Disturb", "2"));
                    var a = scheduleitems[i - 1].Command.ToLower().Equals("alarm") || scheduleitems[i - 1].Command.ToLower().Equals("alarm()");
                    var b = (disturb == 1 && !Hiro_Utils.IsForegroundFullScreen()) || (disturb != 1 && disturb != 0);
                    if (a && b)
                    {
                        if (HWin.IsWindows10() && Read_DCIni("Toast", "0").Equals("1"))
                        {
                            new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                            .SetToastScenario(Microsoft.Toolkit.Uwp.Notifications.ToastScenario.Alarm)
                            .AddText(Get_Translate("alarmtitle"))
                            .AddText(Path_PPX(scheduleitems[i - 1].Name.Replace("\\n", Environment.NewLine)))
                            .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                        .SetContent(Get_Translate("alarmok"))
                                        .AddArgument("action", "ok"))
                            .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                        .SetContent(Get_Translate("alarmdelay"))
                                        .AddArgument("action", "delay"))
                            .AddArgument("alarmid", (i - 1).ToString())
                            .AddArgument("category", "alarm")
                        .Show();
                        }
                        else
                        {
                            Hiro_Alarm ala = new(aw.Count, CustomedContnet: Path_PPX(scheduleitems[i - 1].Name.Replace("\\n", Environment.NewLine)));
                            aw.Add(new Hiro_AlarmWin(ala, i - 1));
                            ala.Show();
                        }
                    }
                    else
                    {
                        if (!b)
                        {
                            mn?.AddToInfoCenter(
                            scTime.ToString("yyyy-MM-dd HH:mm:ss ") + Environment.NewLine
                            + "\t" + Get_Translate("infocmd") + ":" + "\t" + Path_PPX(scheduleitems[i - 1].Name.Replace("\\n", Environment.NewLine)) + Environment.NewLine
                            + "\t" + Get_Translate("infosource") + ":" + "\t" + Get_Translate("alarm") + Environment.NewLine);
                        }
                        switch (scheduleitems[i - 1].re)
                        {
                            case -2.0:
                                break;
                            case -1.0:
                                scheduleitems[i - 1].Time = scTime.AddDays(1.0).ToString("yyyy/MM/dd HH:mm:ss");
                                Write_Ini(sConfig, i.ToString(), "Time", scheduleitems[i - 1].Time);
                                break;
                            case 0.0:
                                scheduleitems[i - 1].Time = scTime.AddDays(7.0).ToString("yyyy/MM/dd HH:mm:ss");
                                Write_Ini(sConfig, i.ToString(), "Time", scheduleitems[i - 1].Time);
                                break;
                            default:
                                scheduleitems[i - 1].Time = scTime.AddDays(Math.Abs(scheduleitems[i - 1].re)).ToString("yyyy/MM/dd HH:mm:ss");
                                Write_Ini(sConfig, i.ToString(), "Time", scheduleitems[i - 1].Time);
                                break;
                        }
                        Hiro_Utils.RunExe(scheduleitems[i - 1].Command, Get_Translate("alarm"));
                    }
                }
                i++;
            }
            formerT = current;
            if (ColorCD > -1)
            {
                if (ColorCD == 0)
                    wnd?.Load_All_Colors();
                ColorCD--;
            }
            switch (flashFlag)
            {
                case 0:
                    if (wnd != null)
                        wnd.Hiro_Tray.IconSource = new BitmapImage(new Uri("Resources/hiro_circle.ico", UriKind.Relative));
                    flashFlag = 1;
                    break;
                case 1:
                    if (wnd != null)
                        wnd.Hiro_Tray.IconSource = null;
                    flashFlag = 0;
                    break;
                default:
                    break;
            }
            if (listener != null)
            {
                Notification_Tick();
            }
            if (mn != null && mn.DropInfo.Visibility == System.Windows.Visibility.Visible)
            {
                if (!mn._dragFlag)
                    mn.HideDropInfo();
                else
                {
                    mn._dragFlag = false;
                }
            }
            if (tb != null)
            {
                tb.UpdatePosition();
            }
        }

        async private static void Notification_Tick()
        {
            try
            {
                if (!Read_DCIni("MonitorSys", "true").Equals("true"))
                    return;
                if (Read_DCIni("Toast", "0").Equals("1"))
                    Write_Ini(dConfig, "Config", "Toast", "0");
                var a = Read_DCIni("MonitorSysPara", "0").Trim();
                var b = formerTime;
                formerTime = DateTimeOffset.Now;
                if (a.Equals("1") || a.Equals("0"))
                    Do_Notifications(await listener?.GetNotificationsAsync(NotificationKinds.Unknown), b);
                if (a.Equals("2") || a.Equals("0"))
                    Do_Notifications(await listener?.GetNotificationsAsync(NotificationKinds.Toast), b);
            }
            catch (Exception ex)
            {
                LogError(ex, "Hiro.Notification.Listener");
            }
        }

        private static void Do_Notifications(IReadOnlyList<UserNotification> notifs, DateTimeOffset dfs)
        {
            foreach (var notif in notifs.Where(x => x.CreationTime.CompareTo(dfs) >= 0))
            {
                var appName = notif.AppInfo.DisplayInfo.DisplayName.Equals("") ? notif.AppInfo.PackageFamilyName : notif.AppInfo.DisplayInfo.DisplayName;
                // Get the toast binding, if present
                NotificationBinding toastBinding = notif.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric);
                if (toastBinding != null)
                {
                    // And then get the text elements from the toast binding
                    IReadOnlyList<AdaptiveNotificationText> textElements = toastBinding.GetTextElements();
                    // Treat the first text element as the title text
                    string? titleText = textElements.FirstOrDefault()?.Text;
                    // We'll treat all subsequent text elements as body text,
                    // joining them together via newlines.
                    string bodyText = string.Join(Environment.NewLine, textElements.Skip(1).Select(t => t.Text));
                    var iconFile = Path_PPX("<current>\\system\\icons\\") + notif.AppInfo.AppUserModelId + ".hsic";
                    if (!System.IO.File.Exists(iconFile))
                    {
                        HFile.CreateFolder(iconFile);
                        RandomAccessStreamReference stream = notif.AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(128, 128));
                        if (stream is not null)
                        {
                            Windows.Foundation.IAsyncOperation<IRandomAccessStreamWithContentType> streamOperation = stream.OpenReadAsync();
                            Task<IRandomAccessStreamWithContentType> streamTask = streamOperation.AsTask();
                            streamTask.Wait();
                            IRandomAccessStreamWithContentType content = streamTask.Result;
                            System.Threading.Tasks.Task.Run(() =>
                            {
                                try
                                {
                                    System.IO.FileStream TheFile = System.IO.File.Create(iconFile);
                                    content.AsStream().Seek(0, SeekOrigin.Begin);
                                    content.AsStream().CopyTo(TheFile);
                                    TheFile.Dispose();
                                }
                                catch (Exception E)
                                {
                                    LogError(E, "Hiro.Notification.SaveIcon");
                                }
                            });
                        }
                    }
                    Hiro_Icon icon = new();
                    icon.Location = iconFile;
                    icon.HeroImage = iconFile;
                    Notify(new Hiro_Notice(bodyText, 1, Get_Translate("winNotice").Replace("%a", appName).Replace("%t", titleText), null, icon));
                }
            }
        }

        public static void Load_Menu()
        {
            if (wnd != null)
            {
                wnd.cm?.Items.Clear();
                wnd.cm ??= new()
                {
                    CacheMode = null,
                    BorderBrush = null
                };
                var total = (cmditems.Count % 10 == 0) ? cmditems.Count / 10 : cmditems.Count / 10 + 1;
                for (var c = 1; c <= 10; c++)
                {
                    if (c + page * 10 > cmditems.Count)
                        break;
                    var name = cmditems[c - 1 + page * 10].Name;
                    if (name.Equals("-"))
                    {
                        wnd.cm.Items.Add(new Separator());
                        continue;
                    }
                    MenuItem mu = new();
                    if (name.ToLower().IndexOf("[s]") != -1)
                    {
                        mu.IsChecked = true;
                        mu.IsCheckable = true;
                    }
                    name = name.Replace("[S]", "").Replace("[s]", "");
                    if (name.ToLower().IndexOf("[h]") != -1)
                        continue;
                    name = name.Replace("[H]", "").Replace("[h]", "");
                    if (name.ToLower().IndexOf("[n]") != -1)
                        mu.IsEnabled = false;
                    name = name.Replace("[N]", "").Replace("[n]", "");
                    mu.Header = name.Replace("[\\\\]", "");
                    mu.Tag = (c + page * 10).ToString();
                    mu.Click += delegate
                    {
                        try
                        {
                            string? str = mu.Tag.ToString();
                            if (str != null)
                                Hiro_Utils.RunExe(cmditems[int.Parse(str) - 1].Command, HText.Get_Translate("menusource").Replace("%t", cmditems[int.Parse(str) - 1].Name));

                        }
                        catch (Exception ex)
                        {
                            LogError(ex, "Hiro.Exception.Menu.Run");
                        }
                    };
                    wnd.cm.Items.Add(mu);
                }
                if (cmditems.Count > 0)
                {
                    wnd.cm.Items.Add(new Separator());
                    MenuItem pre = new();
                    if (page <= 0)
                        pre.IsEnabled = false;
                    pre.Header = HText.Get_Translate("menupre");
                    pre.Click += delegate
                    {
                        page--;
                        Load_Menu();
                        wnd.cm.IsOpen = true;
                    };
                    wnd.cm.Items.Add(pre);
                    MenuItem pageid = new();
                    pageid.Header = (page + 1).ToString() + "/" + total.ToString();
                    pageid.IsEnabled = false;
                    wnd.cm.Items.Add(pageid);
                    MenuItem next = new();
                    if (page >= total - 1)
                        next.IsEnabled = false;
                    next.Header = HText.Get_Translate("menunext");
                    next.Click += delegate
                    {
                        page++;
                        Load_Menu();
                        wnd.cm.IsOpen = true;
                    };
                    wnd.cm.Items.Add(next);
                }
                else
                {
                    MenuItem pageid = new();
                    pageid.IsEnabled = false;
                    pageid.Header = HText.Get_Translate("menunull");
                    wnd.cm.Items.Add(pageid);
                }
                wnd.cm.Items.Add(new Separator());
                MenuItem show = new();
                show.Header = HText.Get_Translate("menushow");
                show.Click += delegate
                {
                    Hiro_Utils.RunExe("show()", HText.Get_Translate("menu"));
                };
                wnd.cm.Items.Add(show);
                MenuItem exit = new();
                exit.Header = HText.Get_Translate("menuexit");
                exit.Click += delegate
                {
                    Hiro_Utils.RunExe("exit()", HText.Get_Translate("menu"));
                };
                wnd.cm.Items.Add(exit);
                foreach (var obj in wnd.cm.Items)
                {
                    if (obj is MenuItem mi)
                        HUI.Set_Control_Location(mi, "context", location: false);
                }
            }
            GC.Collect();
        }
    }
}
