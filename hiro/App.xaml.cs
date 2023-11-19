using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace hiro
{

    public partial class App : Application
    {
        #region 全局参数
        internal static string lang = "en-US";
        internal static string CurrentDirectory = "C:\\";
        internal static string EnvironmentUsername = Environment.UserName;
        internal static string Username = Environment.UserName;
        internal static int CustomUsernameFlag = 0;
        internal static string AppTitle = Hiro_Resources.ApplicationName;
        internal static Color AppAccentColor = Colors.Coral;
        internal static Color AppForeColor = Colors.White;
        internal static System.Collections.ObjectModel.ObservableCollection<Scheduleitem> scheduleitems = new();
        internal static System.Collections.ObjectModel.ObservableCollection<Hiro_AlarmWin> aw = new();
        internal static System.Collections.ObjectModel.ObservableCollection<Language> la = new();
        internal static string LogFilePath = "C:\\1.log";
        internal static string LangFilePath = "C:\\1.hlp";
        internal static string dconfig = "C:\\";
        internal static string sconfig = "C:\\";
        internal static bool Locked = true;
        internal static MainWindow? wnd;
        internal static Hiro_MainUI? mn = null;
        internal static Hiro_Notification? noti = null;
        internal static Hiro_Island? hisland = null;
        internal static Hiro_Box? hiBox = null;
        internal static Hiro_Editor? ed = null;
        internal static Hiro_LockScreen? ls = null;
        internal static List<Hiro_Notice> noticeitems = new();
        internal static double blurradius = 50.0;
        internal static double blursec = 500.0;
        internal static byte trval = 160;
        internal static System.Windows.Threading.DispatcherTimer? timer;
        internal static System.Collections.ObjectModel.ObservableCollection<Cmditem> cmditems = new();
        internal static System.Collections.ObjectModel.ObservableCollection<int> vs = new();
        internal static int page = 0;
        internal static bool dflag = false;
        internal static System.Net.Http.HttpClient? hc = null;
        internal static int ColorCD = -1;
        internal static int ChatCD = 5;
        internal static IntPtr WND_Handle = IntPtr.Zero;
        internal static bool FirstUse = false;
        internal static bool? Logined = false;
        internal static string LoginedUser = string.Empty;
        internal static string LoginedToken = string.Empty;
        internal static System.Threading.Thread? serverThread = null;
        internal static int flashFlag = -1;
        internal static int[] notificationNums = { -1, -1 };
        private static UserNotificationListener? listener = null;
        #endregion

        #region 私有参数
        //QQ,Netease,Kuwo,Spotify,Kugou
        private static IntPtr?[] Ptrs = { null, null, null, null, null, null };
        private static string[] musicTitles = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        private static bool AutoChat = false;
        #endregion

        private async void Hiro_We_Go(object sender, StartupEventArgs e)
        {
            InitializeInnerParameters();
            Initialize_Notify_Recall();
            InitializeStartParameters(e);
            Initialize_NotificationListener();
            Hiro_Utils.SetFrame(Convert.ToInt32(double.Parse(Hiro_Utils.Read_Ini(App.dconfig, "Config", "FPS", "60"))));
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        async private void Initialize_NotificationListener()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.Notifications.Management.UserNotificationListener"))
            {
                if (Hiro_Utils.Read_Ini(dconfig, "Config", "Toast", "0").Equals("1"))
                    Hiro_Utils.Write_Ini(dconfig, "Config", "Toast", "0");
                listener = UserNotificationListener.Current;
                // And request access to the user's notifications (must be called from UI thread)
                UserNotificationListenerAccessStatus accessStatus = await listener.RequestAccessAsync();
                switch (accessStatus)
                {
                    case UserNotificationListenerAccessStatus.Allowed:
                        {
                            listener = UserNotificationListener.Current;
                            Hiro_Utils.LogtoFile("[INFO]Notification Listener Enabled");
                            break;
                        }
                    case UserNotificationListenerAccessStatus.Denied:
                        Hiro_Utils.LogtoFile("[INFO]Notification Listener Disabled");
                        break;
                    case UserNotificationListenerAccessStatus.Unspecified:
                        Hiro_Utils.LogtoFile("[INFO]Notification Listener Unspecified");
                        break;
                }
            }
            else
            {
                Hiro_Utils.LogtoFile("Notification Listener Not Supported");
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = e.ExceptionObject as Exception;
                if (exception != null)
                {
                    Hiro_Utils.LogError(exception, "Hiro.Exception.UnhandledException");
                }
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.UnhandledException.Catch");
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                Hiro_Utils.LogError(e.Exception, "Hiro.Exception.Dispatcher");
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.Dispatcher.Catch");
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
                    Hiro_Utils.LogError(exception, "Hiro.Exception.UnobservedTask");
                }
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.UnobservedTask.Catch");
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
                var recStr = Hiro_Utils.DeleteUnVisibleChar(socketConnection.receivedMsg);
                var outputb = Convert.FromBase64String(recStr);
                recStr = Encoding.Default.GetString(outputb);
                recStr = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(recStr)).Trim();
                if (System.IO.File.Exists(recStr))
                {
                    HiroApp ha = new();
                    ha.msg = Hiro_Utils.Read_Ini(recStr, "App", "Msg", "nop");
                    ha.appID = Hiro_Utils.Read_Ini(recStr, "App", "ID", "null");
                    ha.appName = Hiro_Utils.Read_Ini(recStr, "App", "Name", "null");
                    ha.appPackage = Hiro_Utils.Read_Ini(recStr, "App", "Package", "null");
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        Hiro_Utils.RunExe(ha.msg, ha.appName);
                    });
                    Hiro_Utils.LogtoFile("[SERVER]" + ha.ToString());
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
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            Hiro_Utils.RunExe(ha.msg, ha.appName);
                        });
                        Hiro_Utils.LogtoFile("[SERVER]" + ha.ToString());
                    }
                    else
                        Hiro_Utils.LogtoFile("[SERVER]" + recStr);
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
            Hiro_Utils.Write_Ini(dconfig, "Config", "Port", port.ToString());
            try
            {
                socketLister.Listen(MaxConnection);
                StartServerListener(socketLister, clientSessionTable, clientSessionLock);
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.Socket");
            }
        }

        private static void InitializeStartParameters(StartupEventArgs e)
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
                                        Hiro_Utils.RunExe(para, "Windows");
                                        return;
                                }
                            }
                        }
                        wnd = new MainWindow();
                        wnd.InitializeInnerParameters();
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
                                Hiro_Utils.LogtoFile("[HIROWEGO]Silent Start");
                            else
                                mn.Show();
                            if (Hiro_Utils.Read_Ini(dconfig, "Config", "AutoLogin", "0").Equals("1"))
                            {
                                Hiro_Utils.LogtoFile("[INFO]AutoLogin enabled");
                                new System.Threading.Thread(() =>
                                {
                                    Logined = null;
                                    var tmp = System.IO.Path.GetTempFileName();
                                    var r = Hiro_Utils.Login(LoginedUser, LoginedToken, true, tmp);
                                    if (r.Equals("success") && Hiro_Utils.Read_Ini(tmp, "Login", "res", "-1").Equals("0"))
                                    {
                                        Logined = true;
                                        Hiro_Utils.LogtoFile("[INFO]AutoLogin as " + LoginedUser);
                                        Logined = true;
                                    }
                                    else
                                    {
                                        Hiro_Utils.LogtoFile("[INFO]AutoLogin Failed " + Hiro_Utils.Read_Ini(tmp, "Config", "msg", string.Empty));
                                        Logined = false;
                                    }
                                    if (System.IO.File.Exists(tmp))
                                        System.IO.File.Delete(tmp);
                                    if (Logined == true)
                                        Hiro_Utils.SyncProfile(LoginedUser, LoginedToken);

                                }).Start();
                            }
                            if (FirstUse)
                            {
                                FirstUse = false;
                                Hiro_Utils.RunExe("message(<current>\\users\\default\\app\\" + lang + "\\welcome.hws)", Hiro_Utils.Get_Translate("infofirst").Replace("%h", AppTitle));
                            }
                            if (autoexe)
                            {
                                if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "AutoExe", "1").Equals("2"))
                                    Hiro_Utils.RunExe(Hiro_Utils.Read_Ini(App.dconfig, "Config", "AutoAction", "nop"), Hiro_Utils.Get_Translate("autoexe"));
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

        public static void Notify(Hiro_Notice i)
        {
            string title = AppTitle;
            i.msg = Hiro_Utils.Path_Prepare_EX(i.msg);
            if (i.title != null)
                title = Hiro_Utils.Path_Prepare_EX(i.title);
            int disturb = int.Parse(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Disturb", "2"));
            if ((disturb == 1 && !Hiro_Utils.IsForegroundFullScreen()) || (disturb != 1 && disturb != 0))
            {
                var os = Hiro_Utils.Get_OSVersion();
                if (os.IndexOf(".") != -1)
                    os = os[..os.IndexOf(".")];
                if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Toast", "0").Equals("1") && int.TryParse(os, out int a) && a >= 10)
                {
                    Hiro_Utils.HiroInvoke(() =>
                    {
                        new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                                        .AddText(title)
                                        .AddText(i.msg.Replace("\\n", Environment.NewLine))
                                        .AddArgument("category", "notification")
                                        .Show();
                    });

                }
                else if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Toast", "0").Equals("2") || Hiro_Utils.Read_Ini(App.dconfig, "Config", "Toast", "0").Equals("3"))
                {
                    noticeitems.Add(i);
                    Hiro_Utils.HiroInvoke(() =>
                    {
                        hisland ??= new();
                        hisland.Show();
                    });
                }
                else if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Toast", "0").Equals("4"))
                {
                    noticeitems.Add(i);
                    Hiro_Utils.HiroInvoke(() =>
                    {
                        hiBox ??= new();
                        hiBox.Show();
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
                                noti.timer.Interval = new TimeSpan(10000000);
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
                        + "\t" + Hiro_Utils.Get_Translate("infocmd") + ":" + "\tnotify(" + i.msg + ")" + Environment.NewLine
                        + "\t" + Hiro_Utils.Get_Translate("infosource") + ":" + "\t" + i.title + Environment.NewLine);
            }
            Hiro_Utils.LogtoFile("[NOTIFICATION]" + i.msg);
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
                                Hiro_Utils.RunExe(url, Hiro_Utils.Get_Translate("alarm"));
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
            CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Hiro_Utils.CreateFolder(CurrentDirectory + "\\users\\" + EnvironmentUsername + "\\editor\\");
            Hiro_Utils.CreateFolder(CurrentDirectory + "\\users\\" + EnvironmentUsername + "\\log\\");
            Hiro_Utils.CreateFolder(CurrentDirectory + "\\users\\" + EnvironmentUsername + "\\config\\");
            Hiro_Utils.CreateFolder(CurrentDirectory + "\\users\\" + EnvironmentUsername + "\\app\\");
            Hiro_Utils.CreateFolder(CurrentDirectory + "\\system\\lang\\");
            Hiro_Utils.CreateFolder(CurrentDirectory + "\\system\\wallpaper\\");
            LogFilePath = CurrentDirectory + "\\users\\" + EnvironmentUsername + "\\log\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            System.IO.File.Delete(LogFilePath);
            Hiro_Utils.LogtoFile("[HIROWEGO]InitializeInnerParameters");
            dconfig = CurrentDirectory + "\\users\\" + EnvironmentUsername + "\\config\\" + EnvironmentUsername + ".hus";
            sconfig = CurrentDirectory + "\\users\\" + EnvironmentUsername + "\\config\\" + EnvironmentUsername + ".hsl";
            dconfig = dconfig.Replace("\\\\", "\\");
            sconfig = sconfig.Replace("\\\\", "\\");
            Hiro_Utils.LogtoFile("[HIROWEGO]DConfig at " + dconfig);
            Hiro_Utils.LogtoFile("[HIROWEGO]SConfig at " + sconfig);
            FirstUse = !System.IO.File.Exists(dconfig) && !System.IO.File.Exists(sconfig);
            var str = Hiro_Utils.Read_Ini(dconfig, "Config", "Lang", "");
            if (str.Equals("") || str.Equals("default"))
            {
                lang = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
                if (!System.IO.File.Exists(CurrentDirectory + "\\system\\lang\\" + lang + ".hlp"))
                {
                    lang = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToString();
                    Hiro_Utils.LogError(new System.IO.FileNotFoundException(), "Hiro.Exception.Globalization");
                    if (!System.IO.File.Exists(CurrentDirectory + "\\system\\lang\\" + lang + ".hlp"))
                    {
                        lang = "zh-CN";
                    }
                }
            }
            else
            {
                lang = str;
                if (!System.IO.File.Exists(CurrentDirectory + "\\system\\lang\\" + lang + ".hlp"))
                {
                    if (str.IndexOf("-") != -1)
                        lang = str[..str.IndexOf("-")];
                    Hiro_Utils.LogError(new System.IO.FileNotFoundException(), "Hiro.Exception.Globalization");
                    if (!System.IO.File.Exists(CurrentDirectory + "\\system\\lang\\" + lang + ".hlp"))
                    {
                        lang = "zh-CN";
                    }
                }
            }
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(lang);
            Hiro_Utils.LogtoFile("[HIROWEGO]Current language: " + lang);
            LangFilePath = CurrentDirectory + "\\system\\lang\\" + lang + ".hlp";
            AppTitle = Hiro_Utils.Read_Ini(dconfig, "Config", "CustomNick", "1").Equals("2") ? Hiro_Utils.Read_Ini(dconfig, "Config", "CustomHIRO", "Hiro") : Hiro_Resources.ApplicationName;
            System.IO.DirectoryInfo di = new(CurrentDirectory + "\\system\\lang\\");
            foreach (System.IO.FileInfo fi in di.GetFiles())
            {
                if (fi.Name.ToLower().EndsWith(".hlp"))
                {
                    string langname = Hiro_Utils.Read_Ini(fi.FullName, "Translate", "Lang", "null");
                    string name = fi.Name.Replace(".hlp", "");
                    if (!langname.Equals("null"))
                    {
                        la.Add(new Language(name, langname));
                    }
                }
            }
            switch (Hiro_Utils.Read_Ini(dconfig, "Config", "CustomUser", "1"))
            {
                case "2":
                    CustomUsernameFlag = 1;
                    Username = Hiro_Utils.Read_Ini(dconfig, "Config", "CustomName", "");
                    break;
                default:
                    CustomUsernameFlag = 0;
                    Username = EnvironmentUsername;
                    break;
            }
            Hiro_Utils.version = "v" + Hiro_Utils.Read_Ini(dconfig, "Config", "AppVer", "1");
            if (Hiro_Utils.Read_Ini(dconfig, "Config", "CustomNick", "1").Equals("2"))
                AppTitle = Hiro_Utils.Read_Ini(dconfig, "Config", "CustomHIRO", "Hiro");
            if (Hiro_Utils.Read_Ini(dconfig, "Config", "TRBtn", "0").Equals("1"))
                trval = 0;
            if (Hiro_Utils.Read_Ini(dconfig, "Network", "Proxy", "0").Equals("1"))//IE Proxy
            {
                hc = new();
            }
            else if (Hiro_Utils.Read_Ini(dconfig, "Network", "Proxy", "0").Equals("2"))//Proxy
            {
                try
                {
                    System.Net.Http.HttpClientHandler hch = new();
                    int ProxyPort = int.Parse(Hiro_Utils.Read_Ini(dconfig, "Network", "Port", string.Empty));
                    hch.Proxy = new System.Net.WebProxy(Hiro_Utils.Read_Ini(dconfig, "Network", "Server", string.Empty), ProxyPort);
                    hch.UseProxy = true;
                    string? ProxyUsername = Hiro_Utils.Read_Ini(dconfig, "Network", "Username", string.Empty);
                    string? ProxyPwd = Hiro_Utils.Read_Ini(dconfig, "Network", "Password", string.Empty);
                    ProxyUsername = ProxyUsername.Equals(string.Empty) ? null : ProxyUsername;
                    ProxyPwd = ProxyPwd.Equals(string.Empty) ? null : ProxyPwd;
                    hch.PreAuthenticate = true;
                    hch.UseDefaultCredentials = false;
                    hch.Credentials = new System.Net.NetworkCredential(ProxyUsername, ProxyPwd);
                    hc = new(hch);
                }
                catch (Exception ex)
                {
                    Hiro_Utils.LogError(ex, "Hiro.Exception.Proxy");
                    hc = new();
                }
            }
            else
            {
                System.Net.Http.HttpClientHandler hch = new();
                hch.UseProxy = false;
                hc = new(hch);
            }
            Hiro_Utils.LogtoFile("[DEVICE]Current OS: " + Hiro_Utils.Get_OSVersion());
            LoginedUser = Hiro_Utils.Read_Ini(dconfig, "Config", "User", string.Empty);
            LoginedToken = Hiro_Utils.Read_Ini(dconfig, "Config", "Token", string.Empty);
            AutoChat = Hiro_Utils.Read_Ini(dconfig, "Config", "AutoChat", "1").Equals("1");
        }

        private static void InitializeMethod()
        {
            timer = new()
            {
                Interval = new TimeSpan(10000000)
            };
            timer.Tick += delegate
            {
                TimerTick();
            };
            timer.Start();
        }

        public static void TimerTick()
        {
            #region Hello
            var hr = DateTime.Now.Hour;
            var morning = Hiro_Utils.Read_Ini(LangFilePath, "local", "morning", "[6,7,8,9,10]");
            var noon = Hiro_Utils.Read_Ini(LangFilePath, "local", "noon", "[11,12,13]");
            var afternoon = Hiro_Utils.Read_Ini(LangFilePath, "local", "afternoon", "[14,15,16,17,18]");
            var evening = Hiro_Utils.Read_Ini(LangFilePath, "local", "evening", "[19,20,21,22]");
            var night = Hiro_Utils.Read_Ini(LangFilePath, "local", "night", "[23,0,1,2,3,4,5]");
            morning = morning.Replace("[", "[,").Replace("]", ",]").Trim();
            noon = noon.Replace("[", "[,").Replace("]", ",]").Trim();
            afternoon = afternoon.Replace("[", "[,").Replace("]", ",]").Trim();
            evening = evening.Replace("[", "[,").Replace("]", ",]").Trim();
            night = night.Replace("[", "[,").Replace("]", ",]").Trim();
            if (mn != null)
            {
                if (morning.IndexOf("," + hr + ",") != -1)
                {
                    mn.Set_Home_Labels("morning");
                }
                else if (noon.IndexOf("," + hr + ",") != -1)
                {
                    mn.Set_Home_Labels("noon");
                }
                else if (afternoon.IndexOf("," + hr + ",") != -1)
                {
                    mn.Set_Home_Labels("afternoon");
                }
                else if (evening.IndexOf("," + hr + ",") != -1)
                {
                    mn.Set_Home_Labels("evening");
                }
                else if (night.IndexOf("," + hr + ",") != -1)
                {
                    mn.Set_Home_Labels("night");
                }
                #endregion
                if (AutoChat && Logined == true)
                {
                    if (mn.hiro_chat == null)
                    {
                        Hiro_Utils.LogtoFile("[INFO]AutoChat enabled");
                        mn.hiro_chat ??= new(mn);
                        mn.hiro_chat.Load_Friend_Info_First();
                    }
                    else
                    {
                        ChatCD--;
                        if (ChatCD == 0)
                        {
                            mn.hiro_chat?.Hiro_Get_Chat();
                            ChatCD = 5;
                        }
                    }

                }
            }
            var tim = Hiro_Utils.Read_Ini(LangFilePath, "local", "locktime", "HH:mm");
            var dat = Hiro_Utils.Read_Ini(LangFilePath, "local", "lockdate", "MM/dd (ddd)");
            if (ls != null)
            {
                ls.timelabel.Content = DateTime.Now.ToString(tim);
                ls.datelabel.Content = DateTime.Now.ToString(dat);
            }
            var i = 1;
            while (i <= scheduleitems.Count)
            {
                System.Windows.Forms.Application.DoEvents();
                if (scheduleitems[i - 1].Time.Equals(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")))
                {
                    int disturb = int.Parse(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Disturb", "2"));
                    var a = scheduleitems[i - 1].Command.ToLower().Equals("alarm") || scheduleitems[i - 1].Command.ToLower().Equals("alarm()");
                    var b = (disturb == 1 && !Hiro_Utils.IsForegroundFullScreen()) || (disturb != 1 && disturb != 0);
                    if (a && b)
                    {
                        var os = Hiro_Utils.Get_OSVersion();
                        if (os.IndexOf(".") != -1)
                            os = os[..os.IndexOf(".")];
                        if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Toast", "0").Equals("1") && int.TryParse(os, out int r) && r >= 10)
                        {
                            new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                            .SetToastScenario(Microsoft.Toolkit.Uwp.Notifications.ToastScenario.Alarm)
                            .AddText(Hiro_Utils.Get_Translate("alarmtitle"))
                            .AddText(Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(scheduleitems[i - 1].Name.Replace("\\n", Environment.NewLine))))
                            .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                        .SetContent(Hiro_Utils.Get_Translate("alarmok"))
                                        .AddArgument("action", "ok"))
                            .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                        .SetContent(Hiro_Utils.Get_Translate("alarmdelay"))
                                        .AddArgument("action", "delay"))
                            .AddArgument("alarmid", (i - 1).ToString())
                            .AddArgument("category", "alarm")
                        .Show();
                        }
                        else
                        {
                            Hiro_Alarm ala = new(aw.Count, CustomedContnet: Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(scheduleitems[i - 1].Name.Replace("\\n", Environment.NewLine))));
                            aw.Add(new Hiro_AlarmWin(ala, i - 1));
                            ala.Show();
                        }
                    }
                    else
                    {
                        if (!b)
                        {
                            mn?.AddToInfoCenter(
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + Environment.NewLine
                            + "\t" + Hiro_Utils.Get_Translate("infocmd") + ":" + "\t" + Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(scheduleitems[i - 1].Name.Replace("\\n", Environment.NewLine))) + Environment.NewLine
                            + "\t" + Hiro_Utils.Get_Translate("infosource") + ":" + "\t" + Hiro_Utils.Get_Translate("alarm") + Environment.NewLine);
                        }
                        switch (scheduleitems[i - 1].re)
                        {
                            case -2.0:
                                break;
                            case -1.0:
                                scheduleitems[i - 1].Time = DateTime.Now.AddDays(1.0).ToString("yyyy/MM/dd HH:mm:ss");
                                Hiro_Utils.Write_Ini(sconfig, i.ToString(), "Time", scheduleitems[i - 1].Time);
                                break;
                            case 0.0:
                                scheduleitems[i - 1].Time = DateTime.Now.AddDays(7.0).ToString("yyyy/MM/dd HH:mm:ss");
                                Hiro_Utils.Write_Ini(sconfig, i.ToString(), "Time", scheduleitems[i - 1].Time);
                                break;
                            default:
                                scheduleitems[i - 1].Time = DateTime.Now.AddDays(Math.Abs(scheduleitems[i - 1].re)).ToString("yyyy/MM/dd HH:mm:ss");
                                Hiro_Utils.Write_Ini(sconfig, i.ToString(), "Time", scheduleitems[i - 1].Time);
                                break;
                        }
                        Hiro_Utils.RunExe(scheduleitems[i - 1].Command, Hiro_Utils.Get_Translate("alarm"));
                    }
                }
                i++;
            }
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
                        wnd.Hiro_Tray.IconSource = new BitmapImage(new Uri("/hiro_circle.ico", UriKind.Relative));
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
            //Music
            if (Hiro_Utils.Read_Ini(dconfig, "Config", "Verbose", "0").Equals("1"))
            {
                Music_Tick();
            }
            if (listener != null)
            {
                Notification_Tick();
            }
        }

        async private static void Notification_Tick()
        {
            try
            {
                if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "MonitorSys", "1").Equals("1"))
                    return;
                if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Toast", "0").Equals("1"))
                    Hiro_Utils.Write_Ini(App.dconfig, "Config", "Toast", "0");
                var a = Hiro_Utils.Read_Ini(dconfig, "Config", "MonitorSysPara", "0").Trim();
                if (a.Equals("1") || a.Equals("0"))
                    notificationNums[0] = Do_Notifications(notificationNums[0], await listener?.GetNotificationsAsync(NotificationKinds.Unknown));
                if (a.Equals("2") || a.Equals("0"))
                    notificationNums[1] = Do_Notifications(notificationNums[1], await listener?.GetNotificationsAsync(NotificationKinds.Toast));
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Notification.Listener");
            }
        }

        private static int Do_Notifications(int origin, IReadOnlyList<UserNotification> notifs)
        {
            var ret = origin;
            if (ret >= 0 && ret < notifs.Count)
            {
                for (int i = Math.Max(ret - 1, 0); i < notifs.Count; i++)
                {
                    var appName = notifs[i].AppInfo.PackageFamilyName.Equals("") ? notifs[i].AppInfo.DisplayInfo.DisplayName : notifs[i].AppInfo.PackageFamilyName;
                    // Get the toast binding, if present
                    NotificationBinding toastBinding = notifs[i].Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric);
                    if (toastBinding != null)
                    {
                        // And then get the text elements from the toast binding
                        IReadOnlyList<AdaptiveNotificationText> textElements = toastBinding.GetTextElements();
                        // Treat the first text element as the title text
                        string? titleText = textElements.FirstOrDefault()?.Text;
                        // We'll treat all subsequent text elements as body text,
                        // joining them together via newlines.
                        string bodyText = string.Join(Environment.NewLine, textElements.Skip(1).Select(t => t.Text));
                        var iconFile = Hiro_Utils.Path_Prepare(Hiro_Utils.Path_Prepare_EX("<current>\\system\\icons\\clsids\\")) + notifs[i].AppInfo.AppUserModelId + ".hsic";
                        if (!System.IO.File.Exists(iconFile))
                        {
                            Hiro_Utils.CreateFolder(iconFile);
                            RandomAccessStreamReference stream = notifs[i].AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(128, 128));
                            if (stream is not null)
                            {
                                IAsyncOperation<IRandomAccessStreamWithContentType> streamOperation = stream.OpenReadAsync();
                                Task<IRandomAccessStreamWithContentType> streamTask = streamOperation.AsTask();
                                streamTask.Wait();
                                IRandomAccessStreamWithContentType content = streamTask.Result;
                                System.Threading.Tasks.Task.Run(() => {
                                    try
                                    {
                                        FileStream TheFile = File.Create(iconFile);
                                        content.AsStream().Seek(0, SeekOrigin.Begin);
                                        content.AsStream().CopyTo(TheFile);
                                        TheFile.Dispose();
                                    }
                                    catch (Exception E)
                                    {
                                        Hiro_Utils.LogError(E, "Hiro.Notification.SaveIcon");
                                    }
                                });
                            }
                        }
                            Hiro_Icon icon = new();
                            icon.Location = iconFile;
                            Notify(new Hiro_Notice(bodyText, 1, appName + " - " + titleText, null, icon));
                    }
                }
            }
            return notifs.Count;
        }

        private static void Music_Tick()
        {
            //QQ,Netease,Kuwo,Spotify,Kugou
            if (Initialize_Title(Ptrs[0], out string? qtitle) == 0)
            {
                Initialize_Ptr("QQMusic", 0);
            }
            else if (qtitle != musicTitles[0] && qtitle != null && !qtitle.Equals("QQ音乐"))
            {
                musicTitles[0] = qtitle;
                Notify(new(Hiro_Utils.Get_Translate("qqmusic").Replace("%m", qtitle), 2, Hiro_Utils.Get_Translate("music"), null, new Hiro_Icon() { Location = "<current>\\system\\icons\\qqmusic.png" }));
            }
            if (Initialize_Title(Ptrs[1], out string? ntitle) == 0)
                Initialize_Ptr("cloudmusic", 1);
            else if (ntitle != musicTitles[1] && ntitle != null && !ntitle.Equals(string.Empty) && !ntitle.Equals("网易云音乐"))
            {
                musicTitles[1] = ntitle;
                Notify(new(Hiro_Utils.Get_Translate("netmusic").Replace("%m", ntitle), 2, Hiro_Utils.Get_Translate("music"), null, new Hiro_Icon() { Location = "<current>\\system\\icons\\neteasemusic.png" }));
            }
            if (Initialize_Title(Ptrs[2], out string? kwtitle) == 0)
                Initialize_Ptr("kwmusic", 2);
            else if (kwtitle != musicTitles[2] && kwtitle != null && !kwtitle.Equals("KwStartPageDlg") && !kwtitle.Equals("酷我音乐"))
            {
                if (musicTitles[2].Length > 2)
                {
                    if (kwtitle.IndexOf(musicTitles[2].Substring(2)) != -1)
                    {
                        musicTitles[2] = kwtitle;
                        return;
                    }
                }
                musicTitles[2] = kwtitle;
                Notify(new(Hiro_Utils.Get_Translate("kwmusic").Replace("%m", kwtitle.Replace("-酷我音乐", "")), 2, Hiro_Utils.Get_Translate("music"), null, new Hiro_Icon() { Location = "<current>\\system\\icons\\kuwomusic.png" }));
            }
            if (Initialize_Title(Ptrs[3], out string? kgtitle) == 0)
                Initialize_Ptr("KuGou", 3);
            else if (kgtitle != musicTitles[3] && kgtitle != null && !kgtitle.Equals(string.Empty) && !kgtitle.Equals("酷狗音乐"))
            {
                musicTitles[3] = kgtitle;
                Notify(new(Hiro_Utils.Get_Translate("kgmusic").Replace("%m", kgtitle.Replace("- 酷狗音乐", "").Trim()), 2, Hiro_Utils.Get_Translate("music"), null, new Hiro_Icon() { Location = "<current>\\system\\icons\\kgmusic.png" }));
            }
            if (Initialize_Title(Ptrs[4], out string? sptitle) == 0)
                Ptrs[4] = Initialize_Ptr("Spotify");
            else if (sptitle != musicTitles[4] && sptitle != null && !sptitle.Equals("Spotify") && !sptitle.Equals("Spotify Premium"))
            {
                musicTitles[4] = sptitle;
                Notify(new(Hiro_Utils.Get_Translate("spotifymusic").Replace("%m", sptitle), 2, Hiro_Utils.Get_Translate("music"), null, new Hiro_Icon() { Location = "<current>\\system\\icons\\spotify.png" }));
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
                    Foreground = new SolidColorBrush(AppForeColor),
                    Background = new SolidColorBrush(AppAccentColor),
                    BorderBrush = null,
                    Style = (Style)Current.Resources["HiroContextMenu"],
                    Padding = new(1, 10, 1, 10)
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
                    MenuItem mu = new()
                    {
                        Background = new SolidColorBrush(Colors.Transparent)
                    };
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
                                Hiro_Utils.RunExe(cmditems[int.Parse(str) - 1].Command, Hiro_Utils.Get_Translate("menusource").Replace("%t", cmditems[int.Parse(str) - 1].Name));

                        }
                        catch (Exception ex)
                        {
                            Hiro_Utils.LogError(ex, "Hiro.Exception.Menu.Run");
                        }
                    };
                    wnd.cm.Items.Add(mu);
                }
                if (cmditems.Count > 0)
                {
                    wnd.cm.Items.Add(new Separator());
                    MenuItem pre = new()
                    {
                        Background = new SolidColorBrush(Colors.Transparent)
                    };
                    if (page <= 0)
                        pre.IsEnabled = false;
                    pre.Header = Hiro_Utils.Get_Translate("menupre");
                    pre.Click += delegate
                    {
                        page--;
                        Load_Menu();
                        wnd.cm.IsOpen = true;
                    };
                    wnd.cm.Items.Add(pre);
                    MenuItem pageid = new()
                    {
                        Background = new SolidColorBrush(Colors.Transparent)
                    };
                    pageid.Header = (page + 1).ToString() + "/" + total.ToString();
                    pageid.IsEnabled = false;
                    wnd.cm.Items.Add(pageid);
                    MenuItem next = new()
                    {
                        Background = new SolidColorBrush(Colors.Transparent)
                    };
                    if (page >= total - 1)
                        next.IsEnabled = false;
                    next.Header = Hiro_Utils.Get_Translate("menunext");
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
                    pageid.Header = Hiro_Utils.Get_Translate("menunull");
                    wnd.cm.Items.Add(pageid);
                }
                wnd.cm.Items.Add(new Separator());
                MenuItem show = new()
                {
                    Background = new SolidColorBrush(Colors.Transparent)
                };
                show.Header = Hiro_Utils.Get_Translate("menushow");
                show.Click += delegate
                {
                    Hiro_Utils.RunExe("show()", Hiro_Utils.Get_Translate("menu"));
                };
                wnd.cm.Items.Add(show);
                MenuItem exit = new()
                {
                    Background = new SolidColorBrush(Colors.Transparent)
                };
                exit.Header = Hiro_Utils.Get_Translate("menuexit");
                exit.Click += delegate
                {
                    Hiro_Utils.RunExe("exit()", Hiro_Utils.Get_Translate("menu"));
                };
                wnd.cm.Items.Add(exit);
                foreach (var obj in wnd.cm.Items)
                {
                    if (obj is MenuItem mi)
                        Hiro_Utils.Set_Control_Location(mi, "context", location: false);
                }
            }
            GC.Collect();
        }

        private static IntPtr Initialize_Ptr(string ProcessName)
        {
            var pns = Process.GetProcessesByName(ProcessName);
            foreach (var pn in pns)
            {
                if (pn.MainWindowTitle.IndexOf("桌面歌词") == -1)
                {
                    return pn.MainWindowHandle;
                }
            }
            return IntPtr.Zero;
        }

        private static int Initialize_Title(IntPtr? intPtr, out string? Title)
        {
            if (intPtr == IntPtr.Zero || intPtr == null)
            {
                Title = string.Empty;
                return 0;
            }
            StringBuilder windowName = new(512);
            GetWindowText((IntPtr)intPtr, windowName, windowName.Capacity);
            Title = windowName.ToString().Trim();
            return Title.Length;
        }

        [DllImport("user32.dll")] static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);
        [DllImport("user32.dll")] private static extern bool IsWindowVisible(int hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)] private static extern int GetWindowText(int hWnd, StringBuilder title, int size);


        delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);


        public static void Initialize_Ptr(string ProcessName, int category)
        {
            var pns = Process.GetProcessesByName(ProcessName);
            foreach (var pn in pns)
                foreach (ProcessThread processThread in pn.Threads)
                {
                    EnumThreadWindows(processThread.Id,
                     (hWnd, lParam) =>
                     {
                         //Check if Window is Visible or not.
                         if (!IsWindowVisible((int)hWnd))
                             return true;

                         //Get the Window's Title.
                         StringBuilder title = new StringBuilder(256);
                         _ = GetWindowText((int)hWnd, title, 256);

                         //Check if Window has Title.
                         if (title.Length == 0)
                             return true;
                         var t = title.ToString();
                         if (!t.Equals(string.Empty) && !t.Contains("桌面歌词", StringComparison.CurrentCulture))
                         {
                             switch (category)
                             {
                                 case 0:
                                 case 1:
                                 case 2:
                                 case 3:
                                 case 4:
                                     Ptrs[category] = hWnd;
                                     break;
                                 default:
                                     break;
                             }
                         }

                         return true;
                     }, IntPtr.Zero);
                }
        }


        #region 获取窗口标题
        [DllImport("user32")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lptrString, int nMaxCount);
        #endregion
    }
}
