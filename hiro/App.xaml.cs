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
        internal static bool SilentStart = false;
        internal static string lang = "en-US";
        internal static string CurrentDirectory = "C:\\";
        internal static string EnvironmentUsername = Environment.UserName;
        internal static string Username = Environment.UserName;
        internal static int CustomUsernameFlag = 0;
        internal static string AppTitle = res.ApplicationName;
        internal static string AppVersion = res.ApplicationVersion;
        internal static Color AppAccentColor = Colors.Azure;
        internal static Color AppForeColor = Colors.White;
        internal static bool DarkModeEnabled = false;
        internal static bool restartflag = false;
        internal static System.Collections.ObjectModel.ObservableCollection<scheduleitem> scheduleitems = new();
        internal static System.Collections.ObjectModel.ObservableCollection<alarmwin> aw = new();
        internal static System.Collections.ObjectModel.ObservableCollection<language> la = new();
        internal static string LogFilePath = "C:\\1.log";
        internal static string LangFilePath = "C:\\1.hlp";
        internal static string dconfig = "C:\\";
        internal static string sconfig = "C:\\";
        internal static bool Locked = true;
        internal static MainWindow? wnd;
        internal static Mainui? mn = null;
        internal static notification? noti = null;
        internal static Editor? ed = null;
        internal static Lockscr? ls = null;
        internal static TimePicker? tp = null;
        internal static List<noticeitem> noticeitems = new();
        internal static int editpage = 0;
        internal const double blurradius = 50.0;
        internal const double blursec = 25.0;
        internal const int blurdelay = 1;
        internal static System.Windows.Threading.DispatcherTimer? timer;
        internal static ContextMenu? cm = null;
        internal static System.Collections.ObjectModel.ObservableCollection<cmditem> cmditems = new();
        internal static int page = 0;
        internal static bool dflag = false;
        internal static System.Net.Http.HttpClient hc = new();
        internal static SolidColorBrush ForeBrush = new();
        #endregion

        private void Hiro_We_Go(object sender, StartupEventArgs e)
        {
            InitializeInnerParameters();
            Initialize_Notiy_Recall();
            InitializeMethod();
            wnd = new MainWindow();
            if (e.Args.Length == 1 && e.Args[0].ToLower().Equals("autostart_on"))
            {
                wnd.Set_Autorun(true);
            }
            else if (e.Args.Length == 1 && e.Args[0].ToLower().Equals("autostart_off"))
            {
                wnd.Set_Autorun(false);
            }
            else
            {
                wnd.InitializeInnerParameters();
                wnd.Show();
                wnd.Hide();
                mn = new Mainui();
                if (e.Args.Length == 1 && e.Args[0].ToLower().Equals("silent"))
                {
                    SilentStart = true;
                    utils.LogtoFile("[HIROWEGO]Silent Start");
                }
                else
                    mn.Show();
            }
            if (utils.Read_Ini(App.dconfig, "Configuration", "autorun", "1").Equals("2"))
                utils.RunExe(utils.Read_Ini(App.dconfig, "Configuration", "autoaction", "nop"));
        }

        public static void Notify(noticeitem i)
        {
            if (utils.Read_Ini(App.dconfig, "Configuration", "toast", "0").Equals("1"))
            {
                new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
            .AddText(AppTitle)
            .AddText(i.msg.Replace("\\n", Environment.NewLine))
            .Show();
            }
            else
            {
                noticeitems.Add(i);
                if (noti == null)
                {
                    noti = new notification();
                    noti.Show();
                }
            }
            utils.LogtoFile("[NOTIFICATION]" + i.msg);
        }
        private void Initialize_Notiy_Recall()
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

        private void InitializeInnerParameters()
        {
            CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var path = CurrentDirectory + "\\users\\";
            System.IO.Directory.CreateDirectory(path);
            path = path + EnvironmentUsername + "\\";
            System.IO.Directory.CreateDirectory(path);
            System.IO.Directory.CreateDirectory(path + "\\editor\\");
            System.IO.Directory.CreateDirectory(path + "\\log\\");
            System.IO.Directory.CreateDirectory(path + "\\config\\");

            path = CurrentDirectory + "\\system\\";
            System.IO.Directory.CreateDirectory(path);
            System.IO.Directory.CreateDirectory(path + "\\lang\\");
            System.IO.Directory.CreateDirectory(path + "\\wallpaper\\");

            LogFilePath = CurrentDirectory + "\\users\\" + EnvironmentUsername + "\\log\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            System.IO.File.Delete(LogFilePath);
            utils.LogtoFile("[HIROWEGO]InitializeInnerParameters");
            dconfig = CurrentDirectory + "\\users\\" + EnvironmentUsername + "\\config\\" + EnvironmentUsername + ".has";
            sconfig = CurrentDirectory + "\\users\\" + EnvironmentUsername + "\\config\\" + EnvironmentUsername + ".hsl";
            var str = utils.Read_Ini(dconfig, "Configuration", "lang", "");
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
                        lang = str.Substring(0, str.IndexOf("-"));
                    utils.LogtoFile("[ERROR]Translateion not found, try to initialize as " + lang.ToString());
                    if (!System.IO.File.Exists(CurrentDirectory + "\\system\\lang\\" + lang + ".hlp"))
                    {
                        lang = "zh-CN";
                    }
                }
            }   
            utils.LogtoFile("[HIROWEGO]Current language: " + lang);
            LangFilePath = CurrentDirectory + "\\system\\lang\\" + lang + ".hlp";
            if (utils.Read_Ini(dconfig, "Configuration", "customnick", "1").Equals("2"))
            {
                AppTitle = utils.Read_Ini(dconfig, "Configuration", "customhiro", "Hiro");
            }
            else
            {
                AppTitle = res.ApplicationName;
            }
            var page = int.Parse(utils.Read_Ini(dconfig, "Configuration", "EditPage", "0"));
            editpage = page;
            System.IO.DirectoryInfo di = new(CurrentDirectory + "\\system\\lang\\");
            foreach (System.IO.FileInfo fi in di.GetFiles())
            {
                if(fi.Name.ToLower().EndsWith(".hlp"))
                {
                    string langname = utils.Read_Ini(fi.FullName, "Translate", "lang", "null");
                    string name = fi.Name.Replace(".hlp", "");
                    if(!langname.Equals("null"))
                    {
                        la.Add(new language(name, langname));
                    }
                }
            }
        }

        private void InitializeMethod()
        {
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(10000000);
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
            else
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
                if (scheduleitems[i - 1].time.Equals(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")))
                {
                    if (scheduleitems[i - 1].command.ToLower().Equals("alarm") || scheduleitems[i - 1].command.ToLower().Equals("alarm()"))
                    {
                        if (utils.Read_Ini(dconfig, "Configuration", "toast", "0").Equals("1"))
                        {
                            new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                            .SetToastScenario(Microsoft.Toolkit.Uwp.Notifications.ToastScenario.Alarm)
                            .AddText(utils.Get_Transalte("alarmtitle"))
                            .AddText(scheduleitems[i - 1].name.Replace("\\n", Environment.NewLine))
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
                            Alarm ala = new(aw.Count, CustomContent: true, CustomedContnet: scheduleitems[i - 1].name.Replace("\\n", Environment.NewLine));
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
                                scheduleitems[i - 1].time = DateTime.Now.AddDays(1.0).ToString("yyyy/MM/dd HH:mm:ss");
                                utils.Write_Ini(sconfig, i.ToString(), "time", scheduleitems[i - 1].time);
                                break;
                            case 0.0:
                                scheduleitems[i - 1].time = DateTime.Now.AddDays(7.0).ToString("yyyy/MM/dd HH:mm:ss");
                                utils.Write_Ini(sconfig, i.ToString(), "time", scheduleitems[i - 1].time);
                                break;
                            default:
                                scheduleitems[i - 1].time = DateTime.Now.AddDays(Math.Abs(scheduleitems[i - 1].re)).ToString("yyyy/MM/dd HH:mm:ss");
                                utils.Write_Ini(sconfig, i.ToString(), "time", scheduleitems[i - 1].time);
                                break;
                        }
                        utils.RunExe(scheduleitems[i - 1].command);
                    }
                }
                i++;
            }

        }

        public static void UpdateHomeLabel1(string val)
        {
            if (mn != null)
            {
                if (CustomUsernameFlag == 0)
                    val = utils.Get_Transalte(val).Replace("%u", EnvironmentUsername);
                else
                    val = utils.Get_Transalte(val + "cus").Replace("%u", Username);
                if (mn.homelabel1.Content.Equals(val))
                {

                }
                else
                {
                    mn.homelabel1.Content = val;
                }
            }
        }

        public static void Load_Menu()
        {
            if (cm != null)
                cm.Items.Clear();
            cm = new ContextMenu();
            cm.Foreground = new SolidColorBrush(AppForeColor);
            cm.Background = new SolidColorBrush(AppAccentColor);
            var total = (cmditems.Count % 10 == 0) ? cmditems.Count / 10 : cmditems.Count / 10 + 1;
            for (int c = 1; c <= 10; c++)
            {
                if (c + page * 10 > cmditems.Count)
                    break;
                var name = cmditems[c - 1 + page * 10].name;
                if (name.Equals("-"))
                {
                    Separator sp = new();
                    cm.Items.Add(sp);
                    continue;
                }
                MenuItem mu = new();
                if (name.ToLower().IndexOf("[s]") != -1)
                    mu.IsChecked = true;
                else
                    mu.IsChecked = false;
                name = name.Replace("[S]", "").Replace("[s]", "");
                if (name.ToLower().IndexOf("[h]") != -1)
                    mu.Visibility = Visibility.Hidden;
                else
                    mu.Visibility = Visibility.Visible;
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
                            utils.RunExe(cmditems[int.Parse(str) - 1].command);

                    }
                    catch (Exception ex)
                    {
                        utils.LogtoFile("[ERROR]" + ex.Message);
                    }
                };


                cm.Items.Add(mu);
            }
            if(cmditems.Count > 0)
            {
                cm.Items.Add(new Separator());
                MenuItem pre = new();
                if (page <= 0)
                    pre.IsEnabled = false;
                pre.Header = utils.Get_Transalte("menupre");
                pre.Click += delegate
                {
                    page--;
                    Load_Menu();
                    cm.IsOpen = true;
                };
                cm.Items.Add(pre);
                MenuItem pageid = new();
                pageid.Header = (page + 1).ToString() + "/" + total.ToString();
                pageid.IsEnabled = false;
                cm.Items.Add(pageid);
                MenuItem next = new();
                if (page >= total - 1)
                    next.IsEnabled = false;
                next.Header = utils.Get_Transalte("menunext");
                next.Click += delegate
                {
                    page++;
                    Load_Menu();
                    cm.IsOpen = true;
                };
                cm.Items.Add(next);
            }
            else
            {
                MenuItem pageid = new();
                pageid.IsEnabled = false;
                pageid.Header = utils.Get_Transalte("menunull");
                cm.Items.Add(pageid);
            }
            cm.Items.Add(new Separator());
            MenuItem show = new();
            show.Header = utils.Get_Transalte("menushow");
            show.Click += delegate
            {
                utils.RunExe("show()");
            };
            cm.Items.Add(show);
            MenuItem exit = new();
            exit.Header = utils.Get_Transalte("menuexit");
            exit.Click += delegate
            {
                utils.RunExe("exit()");
            };
            cm.Items.Add(exit);
            GC.Collect();
        }

    }
}
