using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        #endregion

        #region 私有参数
        private static IntPtr? QQMusicPtr = null;
        private static IntPtr? NeteasePtr = null;
        private static IntPtr? KuwoPtr = null;
        private static string QQTitle = string.Empty;
        private static string NeteaseTitle = string.Empty;
        private static string KuwoTitle = string.Empty;
        #endregion

        private void Hiro_We_Go(object sender, StartupEventArgs e)
        {
            _ = new System.Threading.Mutex(true, "0x415417hiro", out bool ret);
            if (!ret)
            {
                Hiro_Utils.RunExe("exit()");
                return;
            }
                InitializeInnerParameters();
                Initialize_Notify_Recall();
                InitializeStartParameters(e);
                Build_Socket();
            


        }

        private void Socket_Communication(System.Net.Sockets.Socket socketLister, System.Collections.Hashtable clientSessionTable, object clientSessionLock)
        {
                System.Net.Sockets.Socket clientSocket = socketLister.Accept();
                Hiro_Socket clientSession = new(clientSocket);
                lock (clientSessionLock)
                {
                    if (!clientSessionTable.ContainsKey(clientSession.IP))
                    {
                        clientSessionTable.Add(clientSession.IP, clientSession);
                    }
                }
                SocketConnection socketConnection = new(clientSocket);
                socketConnection.ReceiveData();
                socketConnection.DataReceiveCompleted += delegate
                {
                    var recStr = Hiro_Utils.DeleteUnVisibleChar(Encoding.ASCII.GetString(socketConnection.msgBuffer));
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
                        Dispatcher.Invoke(delegate
                        {
                            Hiro_Utils.RunExe(ha.msg, ha.appName);
                        });
                        Hiro_Utils.LogtoFile("[SERVER]" + ha.ToString());
                    }
                    else
                    {
                        Hiro_Utils.LogtoFile("[SERVER]" + recStr);
                    }
                };
                System.ComponentModel.BackgroundWorker bw = new();
                bw.DoWork += delegate
                {
                    Socket_Communication(socketLister, clientSessionTable, clientSessionLock);
                };
                bw.RunWorkerAsync();
        }

        private void Build_Socket()
        {
            var port = Hiro_Utils.GetRandomUnusedPort();
            int MaxConnection = 69;
            System.Collections.Hashtable clientSessionTable = new ();
            object clientSessionLock = new();
            System.Net.IPEndPoint localEndPoint = new (System.Net.IPAddress.Any, port);
            System.Net.Sockets.Socket socketLister = new (System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            socketLister.Bind(localEndPoint);
            Hiro_Utils.Write_Ini(dconfig, "Config", "Port", port.ToString());
            try
            {
                socketLister.Listen(MaxConnection);
                System.ComponentModel.BackgroundWorker bw = new();
                bw.DoWork += delegate
                {
                    Socket_Communication(socketLister, clientSessionTable, clientSessionLock);
                };
                bw.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.Socket: " + ex.Message);
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
                                        continue;
                                    case "silent":
                                        silent = true;
                                        continue;
                                    case "utils":
                                        create = false;
                                        continue;
                                    case "update":
                                        autoexe = false;
                                        continue;
                                    case "pure":
                                        autoexe |= false;
                                        continue;
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
                            mn = new();
                            if (silent)
                                Hiro_Utils.LogtoFile("[HIROWEGO]Silent Start");
                            else
                                mn.Show();
                            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "AutoChat", "1").Equals("1"))
                            {
                                Hiro_Utils.LogtoFile("[INFO]AutoChat enabled");
                                mn.hiro_chat ??= new(mn);
                                mn.hiro_chat.Load_Friend_Info_First();
                            }
                            if (FirstUse)
                            {
                                FirstUse = false;
                                Hiro_Utils.RunExe("message(<current>\\users\\default\\app\\" + lang + "\\welcome.hws)", Hiro_Utils.Get_Transalte("infofirst").Replace("%h", AppTitle));
                            }
                            if (autoexe)
                            {
                                if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "AutoExe", "1").Equals("2"))
                                    Hiro_Utils.RunExe(Hiro_Utils.Read_Ini(App.dconfig, "Config", "AutoAction", "nop"), Hiro_Utils.Get_Transalte("autoexe"));
                            }
                            InitializeMethod();
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
                    new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                .AddText(title)
                .AddText(i.msg.Replace("\\n", Environment.NewLine))
                .Show();
                }
                else
                {
                    noticeitems.Add(i);
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
                }
            }
            else
            {
                mn?.AddToInfoCenter(
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + Environment.NewLine
                        + "\t" + Hiro_Utils.Get_Transalte("infocmd") + ":" + "\tnotify(" + i.msg + ")" + Environment.NewLine
                        + "\t" + Hiro_Utils.Get_Transalte("infosource") + ":" + "\t" + i.title + Environment.NewLine);
            }
            Hiro_Utils.LogtoFile("[NOTIFICATION]" + i.msg);
        }

        private static void Initialize_Notify_Recall()
        {
            Microsoft.Toolkit.Uwp.Notifications.ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                var args = Microsoft.Toolkit.Uwp.Notifications.ToastArguments.Parse(toastArgs.Argument);
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
                            Hiro_Utils.RunExe(url, Hiro_Utils.Get_Transalte("alarm"));
                            break;
                        case "uskip":
                            break;
                        default:
                            break;
                    }
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
            Hiro_Utils.CreateFolder(CurrentDirectory + "\\users\\default\\cache\\");
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
                    Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.Globalization: Translateion not found, try to initialize as " + lang.ToString());
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
                    Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.Globalization: Translateion not found, try to initialize as " + lang.ToString());
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
                if(fi.Name.ToLower().EndsWith(".hlp"))
                {
                    string langname = Hiro_Utils.Read_Ini(fi.FullName, "Translate", "Lang", "null");
                    string name = fi.Name.Replace(".hlp", "");
                    if(!langname.Equals("null"))
                    {
                        la.Add(new Language(name, langname));
                    }
                }
            }
            switch (Hiro_Utils.Read_Ini(dconfig, "Config", "CustomUser", "1"))
            {
                case "2":
                    CustomUsernameFlag = 1;
                    Username = Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomName", "");
                    break;
                default:
                    CustomUsernameFlag = 0;
                    Username = EnvironmentUsername;
                    break;
            }
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
                    Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.Proxy: " + ex.Message);
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
                if (mn.hiro_chat != null)
                {
                    ChatCD--;
                    if (ChatCD == 0)
                    {
                        mn.hiro_chat?.Hiro_Get_Chat();
                        ChatCD = 5;
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
                            .AddText(Hiro_Utils.Get_Transalte("alarmtitle"))
                            .AddText(Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(scheduleitems[i - 1].Name.Replace("\\n", Environment.NewLine))))
                            .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                        .SetContent(Hiro_Utils.Get_Transalte("alarmok"))
                                        .AddArgument("action", "ok"))
                            .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                        .SetContent(Hiro_Utils.Get_Transalte("alarmdelay"))
                                        .AddArgument("action", "delay"))
                            .AddArgument("alarmid", (i - 1).ToString())
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
                            + "\t" + Hiro_Utils.Get_Transalte("infocmd") + ":" + "\t" + Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(scheduleitems[i - 1].Name.Replace("\\n", Environment.NewLine))) + Environment.NewLine
                            + "\t" + Hiro_Utils.Get_Transalte("infosource") + ":" + "\t" + Hiro_Utils.Get_Transalte("alarm") + Environment.NewLine);
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
                        Hiro_Utils.RunExe(scheduleitems[i - 1].Command, Hiro_Utils.Get_Transalte("alarm"));
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
            //Music
            if (Hiro_Utils.Read_Ini(dconfig, "Config", "Verbose", "0").Equals("1"))
            {
                Music_Tick();
            }
        }

        private static void Music_Tick()
        {
            if (Initialize_Title(QQMusicPtr, out string? qtitle) == 0)
            {
                QQMusicPtr = Initialize_Ptr("QQMusic");
            }
            else if (qtitle != QQTitle && qtitle != null && !qtitle.Equals("QQ音乐") && !qtitle.Equals("桌面歌词"))
            {
                QQTitle = qtitle;
                Notify(new(Hiro_Utils.Get_Transalte("qqmusic").Replace("%m", qtitle), 2, AppTitle));
            }
            if (Initialize_Title(NeteasePtr, out string? ntitle) == 0)
                NeteasePtr = Initialize_Ptr("cloudmusic");
            else if (ntitle != NeteaseTitle && ntitle != null && !ntitle.Equals(string.Empty) && !ntitle.Equals("网易云音乐") && !ntitle.Equals("桌面歌词"))
            {
                NeteaseTitle = ntitle;
                Notify(new(Hiro_Utils.Get_Transalte("netmusic").Replace("%m", ntitle), 2, AppTitle));
            }
            if (Initialize_Title(KuwoPtr, out string? kwtitle) == 0)
                KuwoPtr = Initialize_Ptr("kwmusic");
            else if (kwtitle != KuwoTitle && kwtitle != null && !kwtitle.Equals("KwStartPageDlg") && !kwtitle.Equals("酷我音乐") && !kwtitle.Equals("桌面歌词"))
            {
                if (KuwoTitle.Length > 2)
                {
                    if (kwtitle.IndexOf(KuwoTitle.Substring(2)) != -1)
                    {
                        KuwoTitle = kwtitle;
                        return;
                    }
                }
                KuwoTitle = kwtitle;
                Notify(new(Hiro_Utils.Get_Transalte("kwmusic").Replace("%m", kwtitle.Replace("-酷我音乐", "")), 2, AppTitle));
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
                    Style = (Style) Current.Resources["HiroContextMenu"],
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
                                Hiro_Utils.RunExe(cmditems[int.Parse(str) - 1].Command, Hiro_Utils.Get_Transalte("menu"));

                        }
                        catch (Exception ex)
                        {
                            Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.Menu.Run: " + ex.Message);
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
                    pre.Header = Hiro_Utils.Get_Transalte("menupre");
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
                    next.Header = Hiro_Utils.Get_Transalte("menunext");
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
                    pageid.Header = Hiro_Utils.Get_Transalte("menunull");
                    wnd.cm.Items.Add(pageid);
                }
                wnd.cm.Items.Add(new Separator());
                MenuItem show = new()
                {
                    Background = new SolidColorBrush(Colors.Transparent)
                };
                show.Header = Hiro_Utils.Get_Transalte("menushow");
                show.Click += delegate
                {
                    Hiro_Utils.RunExe("show()", Hiro_Utils.Get_Transalte("menu"));
                };
                wnd.cm.Items.Add(show);
                MenuItem exit = new()
                {
                    Background = new SolidColorBrush(Colors.Transparent)
                };
                exit.Header = Hiro_Utils.Get_Transalte("menuexit");
                exit.Click += delegate
                {
                    Hiro_Utils.RunExe("exit()", Hiro_Utils.Get_Transalte("menu"));
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
            var pns = System.Diagnostics.Process.GetProcessesByName(ProcessName);
            foreach (var pn in pns)
            {
                if (pn.MainWindowTitle.IndexOf("桌面歌词") != -1)
                {
                    Notify(new(Hiro_Utils.Get_Transalte("delyrics"), 2, AppTitle));
                }
                return pn.MainWindowHandle;
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

        #region 获取窗口标题
        [DllImport("user32")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lptrString, int nMaxCount);
        #endregion
    }
}
