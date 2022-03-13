using System;
using System.Collections.Generic;
using System.Linq;
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
        internal static string AppTitle = res.ApplicationName;
        internal static Color AppAccentColor = Colors.Coral;
        internal static Color AppForeColor = Colors.White;
        internal static System.Collections.ObjectModel.ObservableCollection<Scheduleitem> scheduleitems = new();
        internal static System.Collections.ObjectModel.ObservableCollection<alarmwin> aw = new();
        internal static System.Collections.ObjectModel.ObservableCollection<Language> la = new();
        internal static string LogFilePath = "C:\\1.log";
        internal static string LangFilePath = "C:\\1.hlp";
        internal static string dconfig = "C:\\";
        internal static string sconfig = "C:\\";
        internal static bool Locked = true;
        internal static MainWindow? wnd;
        internal static Mainui? mn = null;
        internal static Notification? noti = null;
        internal static Editor? ed = null;
        internal static Lockscr? ls = null;
        internal static List<noticeitem> noticeitems = new();
        internal static double blurradius = 50.0;
        internal static double blursec = 500.0;
        internal static byte trval = 160;
        internal static System.Windows.Threading.DispatcherTimer? timer;
        internal static System.Collections.ObjectModel.ObservableCollection<Cmditem> cmditems = new();
        internal static System.Collections.ObjectModel.ObservableCollection<int> vs = new();
        internal static int page = 0;
        internal static bool dflag = false;
        internal static System.Net.Http.HttpClient hc = new();
        internal static int ColorCD = -1;
        internal static IntPtr WND_Handle = IntPtr.Zero;
        #endregion

        private void Hiro_We_Go(object sender, StartupEventArgs e)
        {
             _ = new System.Threading.Mutex(true, "0x415417hiro", out bool ret);
            if (!ret)
            {
                utils.RunExe("exit()");
                return;
            }
            InitializeInnerParameters();
            Initialize_Notiy_Recall();
            InitializeMethod();
            InitializeStartParameters(e);
            Build_Socket();
        }

        private void Socket_Communication(System.Net.Sockets.Socket socketLister, System.Collections.Hashtable clientSessionTable, object clientSessionLock)
        {
                System.Net.Sockets.Socket clientSocket = socketLister.Accept();
                HiroSocket clientSession = new(clientSocket);
                lock (clientSessionLock)
                {
                    if (!clientSessionTable.ContainsKey(clientSession.IP))
                    {
                        clientSessionTable.Add(clientSession.IP, clientSession);
                    }
                }
                SocketConnection socketConnection = new(clientSocket);
                socketConnection.ReceiveData();
                socketConnection.DataRecevieCompleted += delegate
                {
                    string recStr = utils.DeleteUnVisibleChar(System.Text.Encoding.ASCII.GetString(socketConnection.msgBuffer));
                    recStr = utils.Path_Prepare_EX(utils.Path_Prepare(recStr)).Trim();
                    if (System.IO.File.Exists(recStr))
                    {
                        HiroApp ha = new();
                        ha.msg = utils.Read_Ini(recStr, "App", "Msg", "nop");
                        ha.appID = utils.Read_Ini(recStr, "App", "ID", "null");
                        ha.appName = utils.Read_Ini(recStr, "App", "Name", "null");
                        ha.appPackage = utils.Read_Ini(recStr, "App", "Package", "null");
                        Dispatcher.Invoke(delegate
                        {
                            utils.RunExe(ha.msg);
                        });
                        utils.LogtoFile("[SERVER]" + ha.ToString());
                    }
                    else
                    {
                        utils.LogtoFile("[SERVER]" + recStr);
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
            var port = utils.GetRandomUnusedPort();
            int MaxConnection = 69;
            System.Collections.Hashtable clientSessionTable = new ();
            object clientSessionLock = new();
            System.Net.IPEndPoint localEndPoint = new (System.Net.IPAddress.Any, port);
            System.Net.Sockets.Socket socketLister = new (System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            socketLister.Bind(localEndPoint);
            utils.Write_Ini(dconfig, "config", "port", port.ToString());
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
                utils.LogtoFile("[ERROR]" + ex.Message);
            }
        }

        private static void InitializeStartParameters(StartupEventArgs e)
        {
            if (e.Args.Length >=1 && e.Args[0].ToLower().Equals("autostart_on"))
            {
                utils.Set_Autorun(true);
                goto Executed;
            }
            else if (e.Args.Length >= 1 && e.Args[0].ToLower().Equals("autostart_off"))
            {
                utils.Set_Autorun(false);
                goto Executed;
            }
            else
            {
                bool silent = false;
                if (e.Args.Length >= 1)
                {
                    foreach (string para in e.Args)
                    {
                        if (para.ToLower().Equals("debug"))
                        {
                            dflag = true;
                            continue;
                        }else if (para.ToLower().Equals("silent"))
                        {
                            silent = true;
                            continue;
                        }else
                        {
                            utils.IntializeColorParameters();
                            utils.RunExe(para);
                            return;
                        }
                    }
                }
                wnd = new MainWindow();
                wnd.InitializeInnerParameters();
                wnd.Show();
                wnd.Hide();
                mn = new Mainui();
                if (silent)
                    utils.LogtoFile("[HIROWEGO]Silent Start");
                else
                    mn.Show();
                if (utils.Read_Ini(App.dconfig, "config", "autoexe", "1").Equals("2"))
                    utils.RunExe(utils.Read_Ini(App.dconfig, "config", "autoaction", "nop"));
                return;
            }
        Executed:
            utils.RunExe("exit()");
            return;
        }

        public static void Notify(noticeitem i)
        {
            string title = AppTitle;
            i.msg = utils.Path_Prepare_EX(i.msg);
            if (i.title != null)
                title = utils.Path_Prepare_EX(i.title);
            if (utils.Read_Ini(App.dconfig, "config", "toast", "0").Equals("1"))
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
                    noti = new Notification();
                    noti.Show();
                }
                else
                {
                    if(noti.flag[0] == 2)
                    {
                        noti.flag[0] = 0;
                        noti.timer.Interval = new TimeSpan(10000000);
                        noti.timer.Start();
                    }
                    
                }
            }
            utils.LogtoFile("[NOTIFICATION]" + i.msg);
        }

        private static void Initialize_Notiy_Recall()
        {
            Microsoft.Toolkit.Uwp.Notifications.ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                Microsoft.Toolkit.Uwp.Notifications.ToastArguments args = Microsoft.Toolkit.Uwp.Notifications.ToastArguments.Parse(toastArgs.Argument);
                string action = "null";
                string url = "nop";
                int id = -1;
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
                                utils.Delay_Alarm(id);
                            }
                            break;
                        case "ok":
                            if (id > -1)
                            {
                                utils.OK_Alarm(id);
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
                            utils.RunExe(url);
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
            utils.CreateFolder(CurrentDirectory + "\\users\\" + EnvironmentUsername + "\\editor\\");
            utils.CreateFolder(CurrentDirectory + "\\users\\" + EnvironmentUsername + "\\log\\");
            utils.CreateFolder(CurrentDirectory + "\\users\\" + EnvironmentUsername + "\\config\\");
            utils.CreateFolder(CurrentDirectory + "\\users\\default\\cache\\");
            utils.CreateFolder(CurrentDirectory + "\\system\\lang\\");
            utils.CreateFolder(CurrentDirectory + "\\system\\wallpaper\\");
            LogFilePath = CurrentDirectory + "\\users\\" + EnvironmentUsername + "\\log\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            System.IO.File.Delete(LogFilePath);
            utils.LogtoFile("[HIROWEGO]InitializeInnerParameters");
            dconfig = CurrentDirectory + "\\users\\" + EnvironmentUsername + "\\config\\" + EnvironmentUsername + ".hus";
            sconfig = CurrentDirectory + "\\users\\" + EnvironmentUsername + "\\config\\" + EnvironmentUsername + ".hsl";
            var str = utils.Read_Ini(dconfig, "config", "lang", "");
            if (str.Equals("") || str.Equals("default"))
            {
                lang = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
                if (!System.IO.File.Exists(CurrentDirectory + "\\system\\lang\\" + lang + ".hlp"))
                {
                    lang = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToString();
                    utils.LogtoFile("[ERROR]Translateion not found, try to initialize as " + lang.ToString());
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
                    utils.LogtoFile("[ERROR]Translateion not found, try to initialize as " + lang.ToString());
                    if (!System.IO.File.Exists(CurrentDirectory + "\\system\\lang\\" + lang + ".hlp"))
                    {
                        lang = "zh-CN";
                    }
                }
            }   
            utils.LogtoFile("[HIROWEGO]Current language: " + lang);
            LangFilePath = CurrentDirectory + "\\system\\lang\\" + lang + ".hlp";
            if (utils.Read_Ini(dconfig, "config", "customnick", "1").Equals("2"))
            {
                AppTitle = utils.Read_Ini(dconfig, "config", "customhiro", "Hiro");
            }
            else
            {
                AppTitle = res.ApplicationName;
            }
            System.IO.DirectoryInfo di = new(CurrentDirectory + "\\system\\lang\\");
            foreach (System.IO.FileInfo fi in di.GetFiles())
            {
                if(fi.Name.ToLower().EndsWith(".hlp"))
                {
                    string langname = utils.Read_Ini(fi.FullName, "Translate", "lang", "null");
                    string name = fi.Name.Replace(".hlp", "");
                    if(!langname.Equals("null"))
                    {
                        la.Add(new Language(name, langname));
                    }
                }
            }
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
            var morning = utils.Read_Ini(LangFilePath, "local", "morning", "[6,7,8,9,10]");
            var noon = utils.Read_Ini(LangFilePath, "local", "noon", "[11,12,13]");
            var afternoon = utils.Read_Ini(LangFilePath, "local", "afternoon", "[14,15,16,17,18]");
            var evening = utils.Read_Ini(LangFilePath, "local", "evening", "[19,20,21,22]");
            var night = utils.Read_Ini(LangFilePath, "local", "night", "[23,0,1,2,3,4,5]");
            morning = morning.Replace("[", "[,").Replace("]", ",]").Trim();
            noon = noon.Replace("[", "[,").Replace("]", ",]").Trim();
            afternoon = afternoon.Replace("[", "[,").Replace("]", ",]").Trim();
            evening = evening.Replace("[", "[,").Replace("]", ",]").Trim();
            night = night.Replace("[", "[,").Replace("]", ",]").Trim();
            if (morning.IndexOf("," + hr + ",") != -1)
            {
                    UpdateHomeLabel1("morning");
            }
            else if (noon.IndexOf("," + hr + ",") != -1)
            {
                    UpdateHomeLabel1("noon");
            }
            else if (afternoon.IndexOf("," + hr + ",") != -1)
            {
                    UpdateHomeLabel1("afternoon");
            }
            else if (evening.IndexOf("," + hr + ",") != -1)
            {
                    UpdateHomeLabel1("evening");
            }
            else if (night.IndexOf("," + hr + ",") != -1)
            {
                    UpdateHomeLabel1("night");
            }
            var tim = utils.Read_Ini(LangFilePath, "local", "locktime", "HH:mm");
            var dat = utils.Read_Ini(LangFilePath, "local", "lockdate", "MM/dd (ddd)");
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
                    if (scheduleitems[i - 1].Command.ToLower().Equals("alarm") || scheduleitems[i - 1].Command.ToLower().Equals("alarm()"))
                    {
                        if (utils.Read_Ini(dconfig, "config", "toast", "0").Equals("1"))
                        {
                            new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                            .SetToastScenario(Microsoft.Toolkit.Uwp.Notifications.ToastScenario.Alarm)
                            .AddText(utils.Get_Transalte("alarmtitle"))
                            .AddText(scheduleitems[i - 1].Name.Replace("\\n", Environment.NewLine))
                            .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                        .SetContent(utils.Get_Transalte("alarmok"))
                                        .AddArgument("action", "ok"))
                            .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                        .SetContent(utils.Get_Transalte("alarmdelay"))
                                        .AddArgument("action", "delay"))
                            .AddArgument("alarmid", (i - 1).ToString())
                        .Show();
                        }
                        else
                        {
                            Alarm ala = new(aw.Count, CustomedContnet: utils.Path_Prepare_EX(scheduleitems[i - 1].Name.Replace("\\n", Environment.NewLine)));
                            aw.Add(new alarmwin(ala, i - 1));
                            ala.Show();
                        }

                        return;
                    }
                    else
                    {
                        switch (scheduleitems[i - 1].re)
                        {
                            case -2.0:
                                break;
                            case -1.0:
                                scheduleitems[i - 1].Time = DateTime.Now.AddDays(1.0).ToString("yyyy/MM/dd HH:mm:ss");
                                utils.Write_Ini(sconfig, i.ToString(), "time", scheduleitems[i - 1].Time);
                                break;
                            case 0.0:
                                scheduleitems[i - 1].Time = DateTime.Now.AddDays(7.0).ToString("yyyy/MM/dd HH:mm:ss");
                                utils.Write_Ini(sconfig, i.ToString(), "time", scheduleitems[i - 1].Time);
                                break;
                            default:
                                scheduleitems[i - 1].Time = DateTime.Now.AddDays(Math.Abs(scheduleitems[i - 1].re)).ToString("yyyy/MM/dd HH:mm:ss");
                                utils.Write_Ini(sconfig, i.ToString(), "time", scheduleitems[i - 1].Time);
                                break;
                        }
                        utils.RunExe(scheduleitems[i - 1].Command);
                    }
                }
                i++;
            }
            if (ColorCD > -1)
            {
                if (ColorCD == 0 && wnd != null)
                    wnd.Load_All_Colors();
                ColorCD--;
            }
        }

        public static void UpdateHomeLabel1(string val)
        {
            if (mn != null)
            {
                val = (CustomUsernameFlag == 0) ? utils.Get_Transalte(val).Replace("%u", EnvironmentUsername) : utils.Get_Transalte(val + "cus").Replace("%u", Username);
                if (!mn.homelabel1.Content.Equals(val))
                    mn.homelabel1.Content = val;
            }
        }

        public static void Load_Menu()
        {
            if (wnd != null)
            {
                if (wnd.cm != null)
                    wnd.cm.Items.Clear();
                wnd.cm = new()
                {
                    CacheMode = null,
                    Foreground = new SolidColorBrush(AppForeColor),
                    Background = new SolidColorBrush(AppAccentColor),
                    BorderBrush = new SolidColorBrush(AppAccentColor),
                    Style = (Style)Current.Resources["HiroContextMenu"],
                    Padding = new(1, 10, 1, 10)
                };
                var total = (cmditems.Count % 10 == 0) ? cmditems.Count / 10 : cmditems.Count / 10 + 1;
                for (int c = 1; c <= 10; c++)
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
                            String? str = mu.Tag.ToString();
                            if (str != null)
                                utils.RunExe(cmditems[int.Parse(str) - 1].Command);

                        }
                        catch (Exception ex)
                        {
                            utils.LogtoFile("[ERROR]" + ex.Message);
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
                    pre.Header = utils.Get_Transalte("menupre");
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
                    next.Header = utils.Get_Transalte("menunext");
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
                    pageid.Header = utils.Get_Transalte("menunull");
                    wnd.cm.Items.Add(pageid);
                }
                wnd.cm.Items.Add(new Separator());
                MenuItem show = new()
                {
                    Background = new SolidColorBrush(Colors.Transparent)
                };
                show.Header = utils.Get_Transalte("menushow");
                show.Click += delegate
                {
                    utils.RunExe("show()");
                };
                wnd.cm.Items.Add(show);
                MenuItem exit = new()
                {
                    Background = new SolidColorBrush(Colors.Transparent)
                };
                exit.Header = utils.Get_Transalte("menuexit");
                exit.Click += delegate
                {
                    utils.RunExe("exit()");
                };
                wnd.cm.Items.Add(exit);
                foreach (object obj in wnd.cm.Items)
                {
                    if (obj is MenuItem mi)
                        utils.Set_Control_Location(mi, "context", location: false);
                }
            }
            GC.Collect();
        }

    }
}
