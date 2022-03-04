using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
{
    /// <summary>
    /// mainui.xaml の相互作用ロジック
    /// </summary>
    public partial class Mainui : Window
    {
        private Label? selected = null;
        private int newflag = 0;//0=new,1=modify
        private int bflag = 0;
        public static string ups = "latest";
        internal System.ComponentModel.BackgroundWorker? upbw = null;
        internal System.ComponentModel.BackgroundWorker? whatsbw = null;
        internal int touch = 0;
        int MaxDay = 28;
        public Mainui()
        {
            InitializeComponent();
            utils.LogtoFile("[HIROWEGO]Main UI: Initializing");
            SourceInitialized += OnSourceInitialized;
            InitializeUIWindow();
            utils.LogtoFile("[HIROWEGO]Main UI: Load Transaltation");
            Load_Translate();
            utils.LogtoFile("[HIROWEGO]Main UI: Load Border");
            Load_Border();
            utils.LogtoFile("[HIROWEGO]Main UI: Load Colors");
            Load_Colors();
            utils.LogtoFile("[HIROWEGO]Main UI: Load Position");
            Load_Position();
            utils.LogtoFile("[HIROWEGO]Main UI: Load Data");
            Load_Data();
            utils.LogtoFile("[HIROWEGO]Main UI: Set Home");
            Set_Label(homex);
            autorun.Tag = "1";
            Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - this.Width / 2);
            Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - this.Height / 2);
            if (utils.Read_Ini(App.dconfig, "config", "ani", "1").Equals("1"))
                Loaded += delegate
                {
                    Blurbgi(utils.ConvertInt(utils.Read_Ini(App.dconfig, "config", "blur", "0")));
                };
            utils.LogtoFile("[HIROWEGO]Main UI: Intitalized");
            utils.LogtoFile("[HIROWEGO]MainUI: Loaded");
        }
        public static void Load_Data()
        {
            App.cmditems.Clear();
            var i = 1;
            var p = 1;
            var inipath = App.dconfig;
            var ti = utils.Read_Ini(inipath, i.ToString(), "title", "");
            var co = utils.Read_Ini(inipath, i.ToString(), "command", "");
            while (!ti.Trim().Equals("") && co.StartsWith("(") && co.EndsWith(")"))
            {
                co = co[1..^1];
                App.cmditems.Add(new cmditem(p, i, ti, co));
                i++;
                p = (i % 10 == 0) ? i / 10 : i / 10 + 1;
                ti = utils.Read_Ini(inipath, i.ToString(), "title", "");
                co = utils.Read_Ini(inipath, i.ToString(), "command","");
            }

            App.scheduleitems.Clear();
            i = 1;
            inipath = App.sconfig;
            ti = utils.Read_Ini(inipath, i.ToString(), "time", "");
            co = utils.Read_Ini(inipath, i.ToString(), "command", "");
            var na = utils.Read_Ini(inipath, i.ToString(), "name", "");
            var re = utils.Read_Ini(inipath, i.ToString(), "repeat", "-2.0");
            if (co.Length >= 2)
                co = co[1..^1];
            while (!ti.Equals("") && !co.Equals("") && !na.Equals("") && !re.Equals(""))
            {
                System.Globalization.DateTimeFormatInfo dtFormat = new()
                {
                    ShortDatePattern = "yyyy/MM/dd HH:mm:ss"
                };
                if (double.Parse(re) == -1.0)
                {
                    DateTime dt = Convert.ToDateTime(ti, dtFormat);
                    DateTime now = DateTime.Now;
                    TimeSpan ts = dt - now;
                    while (ts.TotalMinutes < 0)
                    {
                        dt = dt.AddDays(1.0);
                        ts = dt - now;
                    }
                    ti = dt.ToString("yyyy/MM/dd HH:mm:ss");
                    utils.Write_Ini(inipath, i.ToString(), "time", ti);
                }else if (double.Parse(re) == 0.0)
                {
                    DateTime dt = Convert.ToDateTime(ti, dtFormat);
                    DateTime now = DateTime.Now;
                    TimeSpan ts = dt - now;
                    while (ts.TotalMinutes < 0)
                    {
                        dt = dt.AddDays(7.0);
                        ts = dt - now;
                    }
                    ti = dt.ToString("yyyy/MM/dd HH:mm:ss");
                    utils.Write_Ini(inipath, i.ToString(), "time", ti);
                }else if (double.Parse(re) == -2.0)
                {
                    
                }
                else
                {
                    DateTime dt = Convert.ToDateTime(ti, dtFormat);
                    DateTime now = DateTime.Now;
                    TimeSpan ts = dt - now;
                    while (ts.TotalMinutes < 0)
                    {
                        dt = dt.AddDays(double.Parse(re));
                        ts = dt - now;
                    }
                    ti = dt.ToString("yyyy/MM/dd HH:mm:ss");
                    utils.Write_Ini(inipath, i.ToString(), "time", ti);
                }
                App.scheduleitems.Add(new scheduleitem(i, na, ti, co, double.Parse(re)));
                i++;
                ti = utils.Read_Ini(inipath, i.ToString(), "time", "");
                co = utils.Read_Ini(inipath, i.ToString(), "command", "");
                na = utils.Read_Ini(inipath, i.ToString(), "name", "");
                re = utils.Read_Ini(inipath, i.ToString(), "repeat", "-2.0");
                if (co.Length >= 2)
                    co = co[1..^1];
            }
            App.Load_Menu();
        }
        public void InitializeUIWindow()
        {
            App.restartflag = false;
            closebtn.Background = new SolidColorBrush(Color.FromArgb(0, 255, 0, 0));
            dgi.ItemsSource = App.cmditems;
            dgs.ItemsSource = App.scheduleitems;
            if (App.Locked)
                versionlabel.Content = App.AppVersion + " 🔒";
            else
                versionlabel.Content = App.AppVersion;
            utils.SetShadow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
            switch (utils.Read_Ini(App.dconfig, "config", "leftclick", "1"))
            {
                case "2":
                    rbtn2.IsChecked = true;
                    break;
                case "3":
                    rbtn3.IsChecked = true;
                    break;
                default:
                    rbtn1.IsChecked = true;
                    break;
            }
            switch (utils.Read_Ini(App.dconfig, "config", "middleclick", "1"))
            {
                case "2":
                    rbtn5.IsChecked = true;
                    break;
                case "3":
                    rbtn6.IsChecked = true;
                    break;
                default:
                    rbtn4.IsChecked = true;
                    break;
            }
            switch (utils.Read_Ini(App.dconfig, "config", "rightclick", "2"))
            {
                case "2":
                    rbtn8.IsChecked = true;
                    break;
                case "3":
                    rbtn9.IsChecked = true;
                    break;
                default:
                    rbtn7.IsChecked = true;
                    break;
            }
            switch (utils.Read_Ini(App.dconfig, "config", "customuser", "1"))
            {
                case "2":
                    rbtn11.IsChecked = true;
                    App.CustomUsernameFlag = 1;
                    App.Username = utils.Read_Ini(App.dconfig, "config", "customname", "");
                    break;
                default:
                    rbtn10.IsChecked = true;
                    App.CustomUsernameFlag = 0;
                    App.Username = App.EnvironmentUsername;
                    break;
            }
            switch (utils.Read_Ini(App.dconfig, "config", "autoexe", "1"))
            {
                case "2":
                    rbtn13.IsChecked = true;
                    break;
                default:
                    rbtn12.IsChecked = true;
                    break;
            }
            if (utils.Read_Ini(App.dconfig, "config", "min", "1") == "1")
            {
                cb_box.IsChecked = true;
            }
            else
            {
                cb_box.IsChecked = false;
            }
            if (utils.Read_Ini(App.dconfig, "config", "background", "1") == "2")
            {
                rbtn15.IsChecked = true;
            }
            else
            {
                rbtn14.IsChecked = true;
            }
            if (utils.Read_Ini(App.dconfig, "config", "customnick", "1") == "2")
            {
                rbtn17.IsChecked = true;
                App.AppTitle = utils.Read_Ini(App.dconfig, "config", "customhiro", "Hiro");
                if (App.wnd != null)
                    App.wnd.ti.ToolTipText = utils.Read_Ini(App.dconfig, "config", "customhiro", "Hiro");
            }
            else
            {
                rbtn16.IsChecked = true;
            }
            if (utils.Read_Ini(App.dconfig, "config", "autorun", "0").Equals("1"))
                autorun.IsChecked = true;
            else
                autorun.IsChecked = false;
            blureff.IsChecked = utils.Read_Ini(App.dconfig, "config", "blur", "0") switch
            {
                "2" => null,
                "1" => true,
                _ => false,
            };
            if (utils.Read_Ini(App.dconfig, "config", "verbose", "1").Equals("1"))
                verbose.IsChecked = true;
            else
                verbose.IsChecked = false;
            if (utils.Read_Ini(App.dconfig, "config", "ani", "1").Equals("1"))
                animation.IsChecked = true;
            else
                animation.IsChecked = false;
            if (utils.Read_Ini(App.dconfig, "config", "toast", "0").Equals("1"))
                win_style.IsChecked = true;
            else
                win_style.IsChecked = false;
            if (utils.Read_Ini(App.dconfig, "config", "reverse", "0").Equals("1"))
                reverse_style.IsChecked = true;
            else
                reverse_style.IsChecked = false;
            if (utils.Read_Ini(App.dconfig, "config", "lock", "0").Equals("1"))
                lock_style.IsChecked = true;
            else
                lock_style.IsChecked = false;
            tb1.Text = utils.Read_Ini(App.dconfig, "config", "leftaction", "");
            tb2.Text = utils.Read_Ini(App.dconfig, "config", "middleaction", "");
            tb3.Text = utils.Read_Ini(App.dconfig, "config", "rightaction", "");
            tb4.Text = utils.Read_Ini(App.dconfig, "config", "customname", "");
            tb5.Text = utils.Read_Ini(App.dconfig, "config", "autoaction", "");
            tb10.Text = utils.Read_Ini(App.dconfig, "config", "customhiro", "");
        }
        private void Configbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Thickness thickness = configg.Margin;
            thickness.Top = -configbar.Value;
            configg.Margin = thickness;
        }
        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;
            System.Windows.Interop.HwndSource source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source.AddHook(WndProc);
            utils.LogtoFile("[HIROWEGO]Main UI: AddHook WndProc");
            WindowStyle = WindowStyle.SingleBorderWindow;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0083://prevent system from drawing outline
                    handled = true;
                    break;
                default:
                    //Console.WriteLine("Msg: " + m.Msg + ";LParam: " + m.LParam + ";WParam: " + m.WParam + ";Result: " + m.Result);
                    break;
            }
            return IntPtr.Zero;

        }

        #region UI相关
        public void Load_Position()
        {
            utils.Set_Control_Location(btn1, "inew");
            utils.Set_Control_Location(btn2, "iup");
            utils.Set_Control_Location(btn3, "idown");
            utils.Set_Control_Location(btn4, "ilaunch");
            utils.Set_Control_Location(btn5, "idelete");
            utils.Set_Control_Location(btn6, "imodify");
            utils.Set_Control_Location(btn7, "lock");
            utils.Set_Control_Location(btn8, "feedback");
            utils.Set_Control_Location(btn9, "whatsnew");
            utils.Set_Control_Location(btn10, "bgopen");
            utils.Set_Control_Location(rbtn1, "showwin");
            utils.Set_Control_Location(rbtn2, "showmenu");
            utils.Set_Control_Location(rbtn3, "runcmd");
            utils.Set_Control_Location(rbtn4, "showwin2");
            utils.Set_Control_Location(rbtn5, "showmenu2");
            utils.Set_Control_Location(rbtn6, "runcmd2");
            utils.Set_Control_Location(rbtn7, "showwin3");
            utils.Set_Control_Location(rbtn8, "showmenu3");
            utils.Set_Control_Location(rbtn9, "runcmd3");
            utils.Set_Control_Location(rbtn10, "calluser");
            utils.Set_Control_Location(rbtn11, "callcus");
            utils.Set_Control_Location(rbtn12, "disabled");
            utils.Set_Control_Location(rbtn13, "runcmd4");
            utils.Set_Control_Location(rbtn14, "systheme");
            utils.Set_Control_Location(rbtn15, "custheme");
            utils.Set_Control_Location(rbtn16, "namehiro");
            utils.Set_Control_Location(rbtn17, "namecus");
            utils.Set_Control_Location(rbtn18, "alarmonce");
            utils.Set_Control_Location(rbtn19, "alarmed");
            utils.Set_Control_Location(rbtn20, "alarmew");
            utils.Set_Control_Location(rbtn21, "alarmat");
            utils.Set_Control_Location(ntn1, "filename", right: true);
            utils.Set_Control_Location(ntn2, "hide", right: true);
            utils.Set_Control_Location(ntn3, "unformat", right: true);
            utils.Set_Control_Location(ntn4, "ban", right: true);
            utils.Set_Control_Location(ntn5, "select", right: true);
            utils.Set_Control_Location(ntn6, "quot", right: true);
            utils.Set_Control_Location(ntn7, "explorer", right: true);
            utils.Set_Control_Location(ntn8, "openfile", right: true);
            utils.Set_Control_Location(ntn9, "reset", bottom: true);
            utils.Set_Control_Location(ntnx, "clear", bottom: true, right: true);
            utils.Set_Control_Location(ntnx1, "ok", bottom: true, right: true);
            utils.Set_Control_Location(ntnx2, "cancel", bottom: true, right: true);
            utils.Set_Control_Location(chk_btn, "checkup");
            utils.Set_Control_Location(scbtn_1, "scnew");
            utils.Set_Control_Location(scbtn_2, "scdelete");
            utils.Set_Control_Location(scbtn_3, "scmodify");
            utils.Set_Control_Location(scbtn_4, "screset", bottom: true);
            utils.Set_Control_Location(scbtn_5, "scok", bottom: true, right: true);
            utils.Set_Control_Location(scbtn_6, "scclear", bottom: true, right: true);
            utils.Set_Control_Location(scbtn_7, "sccancel", bottom: true, right: true);
            utils.Set_Control_Location(scbtn_8, "sc15m", right: true);
            utils.Set_Control_Location(scbtn_9, "sc1h", right: true);
            utils.Set_Control_Location(scbtn_10, "sc1d", right: true);
            utils.Set_Control_Location(cb_box,"minclose");
            utils.Set_Control_Location(autorun,"autorun");
            utils.Set_Control_Location(blureff,"blurbox");
            utils.Set_Control_Location(win_style,"winbox");
            utils.Set_Control_Location(reverse_style,"reversebox");
            utils.Set_Control_Location(lock_style,"lockbox");
            utils.Set_Control_Location(verbose,"verbosebox");
            utils.Set_Control_Location(animation,"anibox");
            utils.Set_Control_Location(lc_label,"leftclick");
            utils.Set_Control_Location(mc_label,"middleclick");
            utils.Set_Control_Location(rc_label,"rightclick");
            utils.Set_Control_Location(call_label,"callmethod");
            utils.Set_Control_Location(name_label,"namelabel");
            utils.Set_Control_Location(ar_label,"autoexe");
            utils.Set_Control_Location(glabel,"itemname");
            utils.Set_Control_Location(glabel2,"command");
            utils.Set_Control_Location(sclabel1,"scname");
            utils.Set_Control_Location(sclabel2,"sctime");
            utils.Set_Control_Location(sclabel3,"sccommand");
            utils.Set_Control_Location(homex,"home", location: false);
            utils.Set_Control_Location(itemx,"item", location: false);
            utils.Set_Control_Location(schedulex,"schedule", location: false);
            utils.Set_Control_Location(configx,"config", location: false);
            utils.Set_Control_Location(helpx,"help", location: false);
            utils.Set_Control_Location(aboutx,"about", location: false);
            utils.Set_Control_Location(newx,"new", location: false);
            utils.Set_Control_Location(timex,"time", location: false);
            utils.Set_Control_Location(bg_label,"background");
            utils.Set_Control_Location(langlabel,"language");
            utils.Set_Control_Location(langbox,"langbox");
            utils.Set_Control_Location(moreandsoon,"morecome");

            utils.Set_Control_Location(tb1, "lefttb");
            utils.Set_Control_Location(tb2, "middletb");
            utils.Set_Control_Location(tb3, "righttb");
            utils.Set_Control_Location(tb4, "calltb");
            utils.Set_Control_Location(tb5, "autoexetb");
            utils.Set_Control_Location(tb6, "helptb");
            utils.Set_Control_Location(tb7, "nametb");
            utils.Set_Control_Location(tb8, "cmdtb");
            utils.Set_Control_Location(tb9, "detailtb");
            utils.Set_Control_Location(tb10, "hirotb");
            utils.Set_Control_Location(tb11, "scnametb");
            utils.Set_Control_Location(tb12, "sctimetb");
            utils.Set_Control_Location(tb13, "sccmdtb");
            utils.Set_Control_Location(tb14, "alarmattb");
            utils.Set_Control_Location(dgi, "data");
            utils.Set_Control_Location(dgs, "sdata");

            utils.Set_Control_Location(tp_title, "timet");
            utils.Set_Control_Location(year, "timeyear");
            utils.Set_Control_Location(month, "timemonth");
            utils.Set_Control_Location(day, "timeday");
            utils.Set_Control_Location(hour, "timehour");
            utils.Set_Control_Location(minute, "timemin");
            utils.Set_Control_Location(second, "timesec");
            utils.Set_Control_Location(tpbtn1, "timeok", bottom: true, right: true);
            utils.Set_Control_Location(tpbtn2, "timecancel", bottom: true, right: true);

            utils.Set_Grid_Location(homeg, "homeg");
            utils.Set_Grid_Location(itemg, "itemg");
            utils.Set_Grid_Location(scheduleg, "scheduleg");
            utils.Set_Grid_Location(newsg, "newsg");
            utils.Set_Grid_Location(configg, "configg");
            utils.Set_Grid_Location(helpg, "helpg");
            utils.Set_Grid_Location(aboutg, "aboutg");
            utils.Set_Grid_Location(newg, "newg");
            utils.Set_Grid_Location(rc_grid, "rightg");
            utils.Set_Grid_Location(mc_grid, "middleg");
            utils.Set_Grid_Location(lc_grid, "leftg");
            utils.Set_Grid_Location(ar_grid, "autog");
            utils.Set_Grid_Location(cm_grid, "callg");
            utils.Set_Grid_Location(bg_grid, "backg");
            utils.Set_Grid_Location(name_grid, "nameg");
            utils.Set_Grid_Location(timeg, "timeg");

            Thickness thickness = configg.Margin;
            thickness.Top = 0.0;
            configg.Margin = thickness;
            configbar.Maximum = configg.Height - homeg.Height;
            configbar.Value = 0.0;
            configbar.ViewportSize = homeg.Height;
            Thickness th2 = extended.Margin;
            th2.Left = Width / 2 - Height / 2;
            th2.Top = 0;
            extended.Margin = th2;
            extended.Width = Height;
            extended.Height = Height;
        }

        public void Load_Border()
        {
            btn1.BorderThickness = new Thickness(1, 1, 1, 1);
            btn2.BorderThickness = btn1.BorderThickness;
            btn3.BorderThickness = btn1.BorderThickness;
            btn4.BorderThickness = btn1.BorderThickness;
            btn5.BorderThickness = btn1.BorderThickness;
            btn6.BorderThickness = btn1.BorderThickness;
            btn7.BorderThickness = btn1.BorderThickness;
            btn8.BorderThickness = btn1.BorderThickness;
            btn9.BorderThickness = btn1.BorderThickness;
            btn10.BorderThickness = btn1.BorderThickness;
            chk_btn.BorderThickness = btn1.BorderThickness;
            ntn1.BorderThickness = btn1.BorderThickness;
            ntn2.BorderThickness = btn1.BorderThickness;
            ntn3.BorderThickness = btn1.BorderThickness;
            ntn4.BorderThickness = btn1.BorderThickness;
            ntn5.BorderThickness = btn1.BorderThickness;
            ntn6.BorderThickness = btn1.BorderThickness;
            ntn7.BorderThickness = btn1.BorderThickness;
            ntn8.BorderThickness = btn1.BorderThickness;
            ntn9.BorderThickness = btn1.BorderThickness;
            ntnx.BorderThickness = btn1.BorderThickness;
            ntnx1.BorderThickness = btn1.BorderThickness;
            ntnx2.BorderThickness = btn1.BorderThickness;
            scbtn_1.BorderThickness = btn1.BorderThickness;
            scbtn_2.BorderThickness = btn1.BorderThickness;
            scbtn_3.BorderThickness = btn1.BorderThickness;
            scbtn_4.BorderThickness = btn1.BorderThickness;
            scbtn_5.BorderThickness = btn1.BorderThickness;
            scbtn_6.BorderThickness = btn1.BorderThickness;
            scbtn_7.BorderThickness = btn1.BorderThickness;
            scbtn_8.BorderThickness = btn1.BorderThickness;
            scbtn_9.BorderThickness = btn1.BorderThickness;
            scbtn_10.BorderThickness = btn1.BorderThickness;

        }

        public void Load_Colors()
        {
            utils.IntializeColorParameters();
            utils.Set_Bgimage(bgimage);
            if (utils.Read_Ini(App.dconfig, "config", "background", "1").Equals("1"))
            {
                Blurbgi(0);
            }
            else
            {
                Blurbgi(utils.ConvertInt(utils.Read_Ini(App.dconfig, "config", "blur", "0")));
            }
            Foreground = new SolidColorBrush(App.AppForeColor);

            #region 颜色
            btn1.Background = new SolidColorBrush(Color.FromArgb(160, App.AppAccentColor.R, App.AppAccentColor.G, App.AppAccentColor.B));
            btn1.Foreground = Foreground;
            btn1.BorderBrush = new SolidColorBrush(App.AppForeColor);

            if (App.AppForeColor == Colors.White)
            {
                langbox.Foreground = new SolidColorBrush(Colors.Black);
                langbox.Background = new SolidColorBrush(Color.FromArgb(160,255,255,255));
            }
            else
            {
                langbox.Background = new SolidColorBrush(Colors.Transparent);
                langbox.Foreground = btn1.Foreground;
            }
            #endregion
            coloruse1.Background = new SolidColorBrush(Color.FromArgb(80, App.AppForeColor.R, App.AppForeColor.G, App.AppForeColor.B));
            minbtn.Background = new SolidColorBrush(Color.FromArgb(0, App.AppForeColor.R, App.AppForeColor.G, App.AppForeColor.B));
            if (App.wnd != null && App.wnd.cm != null)
            {
                App.wnd.cm.Foreground = new SolidColorBrush(App.AppForeColor);
                App.wnd.cm.Background = new SolidColorBrush(App.AppAccentColor);
            }
            Load_Labels(false);
        }

        public void Load_Translate()
        {
            Title = App.AppTitle + " - " + utils.Get_Transalte("version").Replace("%c", App.AppVersion);
            titlelabel.Content = App.AppTitle;
            minbtn.ToolTip = utils.Get_Transalte("min");
            closebtn.ToolTip = utils.Get_Transalte("close");
            homex.Content = utils.Get_Transalte("home");
            itemx.Content = utils.Get_Transalte("item");
            schedulex.Content = utils.Get_Transalte("schedule");
            configx.Content = utils.Get_Transalte("config");
            helpx.Content = utils.Get_Transalte("help");
            aboutx.Content = utils.Get_Transalte("about");
            newx.Content = utils.Get_Transalte("new");
            timex.Content = utils.Get_Transalte("time");
            tp_title.Content = utils.Get_Transalte("time");
            btn1.Content = utils.Get_Transalte("inew");
            btn2.Content = utils.Get_Transalte("iup");
            btn3.Content = utils.Get_Transalte("idown");
            btn4.Content = utils.Get_Transalte("ilaunch");
            btn5.Content = utils.Get_Transalte("idelete");
            btn6.Content = utils.Get_Transalte("imodify");
            btn7.Content = utils.Get_Transalte("lock");
            btn8.Content = utils.Get_Transalte("feedback");
            btn9.Content = utils.Get_Transalte("whatsnew");
            btn10.Content = utils.Get_Transalte("bgopen");
            rbtn1.Content = utils.Get_Transalte("showwin");
            rbtn2.Content = utils.Get_Transalte("showmenu");
            rbtn3.Content = utils.Get_Transalte("runcmd");
            rbtn4.Content = utils.Get_Transalte("showwin");
            rbtn5.Content = utils.Get_Transalte("showmenu");
            rbtn6.Content = utils.Get_Transalte("runcmd");
            rbtn7.Content = utils.Get_Transalte("showwin");
            rbtn8.Content = utils.Get_Transalte("showmenu");
            rbtn9.Content = utils.Get_Transalte("runcmd");
            rbtn10.Content = utils.Get_Transalte("calluser");
            rbtn11.Content = utils.Get_Transalte("callcus");
            rbtn12.Content = utils.Get_Transalte("disabled");
            rbtn13.Content = utils.Get_Transalte("runcmd");
            rbtn14.Content = utils.Get_Transalte("systheme");
            rbtn15.Content = utils.Get_Transalte("custheme");
            rbtn16.Content = utils.Get_Transalte("namehiro");
            rbtn17.Content = utils.Get_Transalte("namecus");
            rbtn18.Content = utils.Get_Transalte("alarmonce");
            rbtn19.Content = utils.Get_Transalte("alarmed");
            rbtn20.Content = utils.Get_Transalte("alarmew");
            rbtn21.Content = utils.Get_Transalte("alarmat");
            ntn1.Content = utils.Get_Transalte("filename");
            ntn2.Content = utils.Get_Transalte("hide");
            ntn3.Content = utils.Get_Transalte("unformat");
            ntn4.Content = utils.Get_Transalte("ban");
            ntn5.Content = utils.Get_Transalte("select");
            ntn6.Content = utils.Get_Transalte("quot");
            ntn7.Content = utils.Get_Transalte("explorer");
            ntn8.Content = utils.Get_Transalte("openfile");
            ntn9.Content = utils.Get_Transalte("reset");
            ntnx.Content = utils.Get_Transalte("clear");
            ntnx1.Content = utils.Get_Transalte("ok");
            ntnx2.Content = utils.Get_Transalte("cancel");
            scbtn_1.Content = utils.Get_Transalte("scnew");
            scbtn_2.Content = utils.Get_Transalte("scdelete");
            scbtn_3.Content = utils.Get_Transalte("scmodify");
            scbtn_4.Content = utils.Get_Transalte("screset");
            scbtn_5.Content = utils.Get_Transalte("scok");
            scbtn_6.Content = utils.Get_Transalte("scclear");
            scbtn_7.Content = utils.Get_Transalte("sccancel");
            scbtn_8.Content = utils.Get_Transalte("sc15m");
            scbtn_9.Content = utils.Get_Transalte("sc1h");
            scbtn_10.Content = utils.Get_Transalte("sc1d");
            chk_btn.Content = utils.Get_Transalte("checkup");
            cb_box.Content = utils.Get_Transalte("minclose");
            autorun.Content = utils.Get_Transalte("autorun");
            blureff.Content = utils.Get_Transalte("blurbox");
            win_style.Content = utils.Get_Transalte("winbox");
            reverse_style.Content = utils.Get_Transalte("reversebox");
            lock_style.Content = utils.Get_Transalte("lockbox");
            verbose.Content = utils.Get_Transalte("verbosebox");
            animation.Content = utils.Get_Transalte("anibox");
            lc_label.Content = utils.Get_Transalte("leftclick");
            mc_label.Content = utils.Get_Transalte("middleclick");
            rc_label.Content = utils.Get_Transalte("rightclick");
            call_label.Content = utils.Get_Transalte("callmethod");
            ar_label.Content = utils.Get_Transalte("autoexe");
            bg_label.Content = utils.Get_Transalte("background");
            langlabel.Content = utils.Get_Transalte("language");
            name_label.Content = utils.Get_Transalte("namelabel");
            moreandsoon.Content = utils.Get_Transalte("morecome");
            sclabel1.Content = utils.Get_Transalte("scname");
            sclabel2.Content = utils.Get_Transalte("sctime");
            sclabel3.Content = utils.Get_Transalte("sccmd");
            glabel.Content = utils.Get_Transalte("itemname");
            glabel2.Content = utils.Get_Transalte("cmd");
            dgi.Columns[0].Header = utils.Get_Transalte("page");
            dgi.Columns[1].Header = utils.Get_Transalte("id");
            dgi.Columns[2].Header = utils.Get_Transalte("name");
            dgi.Columns[3].Header = utils.Get_Transalte("command");
            dgs.Columns[0].Header = utils.Get_Transalte("sid");
            dgs.Columns[1].Header = utils.Get_Transalte("sname");
            dgs.Columns[2].Header = utils.Get_Transalte("stime");
            dgs.Columns[3].Header = utils.Get_Transalte("scommand");
            tpbtn1.Content = utils.Get_Transalte("timeok");
            tpbtn2.Content = utils.Get_Transalte("timecancel");
            tb6.Text = utils.Get_Transalte("helptext") + utils.Get_Transalte("helptext_ext") + utils.Get_Transalte("helptext_ext2");
        }
        public void Load_Labels(bool reload = true)
        {
            if (reload)
            {
                homex.Background = new SolidColorBrush(Colors.Transparent);
                itemx.Background = new SolidColorBrush(Colors.Transparent);
                schedulex.Background = new SolidColorBrush(Colors.Transparent);
                configx.Background = new SolidColorBrush(Colors.Transparent);
                helpx.Background = new SolidColorBrush(Colors.Transparent);
                aboutx.Background = new SolidColorBrush(Colors.Transparent);
                newx.Background = new SolidColorBrush(Colors.Transparent);
                timex.Background = new SolidColorBrush(Colors.Transparent);
                homex.IsEnabled = true;
                itemx.IsEnabled = true;
                schedulex.IsEnabled = true;
                configx.IsEnabled = true;
                helpx.IsEnabled = true;
                aboutx.IsEnabled = true;
                newx.IsEnabled = true;
                timex.IsEnabled = true;
                
            }
            homex.Foreground = Foreground;
            itemx.Foreground = Foreground;
            schedulex.Foreground = Foreground;
            configx.Foreground = Foreground;
            helpx.Foreground = Foreground;
            aboutx.Foreground = Foreground;
            newx.Foreground = Foreground;   
            timex.Foreground = Foreground;   
        }
        #endregion


        private void Ui_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Titlelabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }

        private void Bglabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }

        private void Closebtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(extended.Visibility == Visibility.Visible)
            {
                extended.IsEnabled = false;
                extend_background.IsEnabled = false;
                Storyboard? sb = new();
                sb = utils.AddDoubleAnimaton(0, App.blursec, extended, "Opacity", sb);
                sb = utils.AddDoubleAnimaton(0, App.blursec, extend_background, "Opacity", sb);
                sb.Completed += delegate
                {
                    extended.Opacity = 0;
                    extend_background.Opacity = 0;
                    extended.Visibility = Visibility.Hidden;
                    extend_background.Visibility = Visibility.Hidden;
                    sb = null;
                };
                sb.Begin();
            }
            else
            {
                if (utils.Read_Ini(App.dconfig, "config", "min", "1").Equals("1"))
                {
                    Visibility = Visibility.Hidden;
                }
                else
                {
                    utils.RunExe("exit()");
                }
            }
        }

        private void Homex_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Set_Label(homex);
        }

        public void Set_Label(Label label)
        {
            Load_Labels();
            if (label != newx && label != timex)
            {
                newx.Visibility = Visibility.Hidden;
            }
            if (label != timex)
            {
                timex.Visibility = Visibility.Hidden;
            }
            double duration = Math.Abs(label.Margin.Top - bgx.Margin.Top);
            if (!utils.Read_Ini(App.dconfig, "config", "ani", "1").Equals("0"))
            {
                duration = duration > label.Height * 2 ? 2 * duration : 6 * duration;
                Storyboard? sb = new();
                sb = utils.AddThicknessAnimaton(label.Margin, duration, bgx, "Margin", sb);
                sb.Completed += delegate
                {
                    bgx.Margin = new Thickness(label.Margin.Left, label.Margin.Top, 0, 0);
                };
                sb.Begin();
            }
            else
            {
                bgx.Margin = new Thickness(label.Margin.Left, label.Margin.Top, 0, 0);
            }
            selected = label;
            if (label == homex)
            {
                tc.SelectedIndex = 0;
            }
            if(label == itemx)
            {
                if(!App.Locked)
                    tc.SelectedIndex = 1;
                else
                {
                    System.ComponentModel.BackgroundWorker sc = new();
                    System.ComponentModel.BackgroundWorker fa = new();
                    sc.RunWorkerCompleted += delegate
                    {
                        App.Locked = false;
                        if (App.mn != null)
                        {
                            App.mn.versionlabel.Content = App.AppVersion;
                            App.mn.Set_Label(itemx);
                        }
                    };
                    fa.RunWorkerCompleted += delegate
                    {
                        if (App.mn != null)
                        {
                            if(App.Locked)
                                App.mn.versionlabel.Content = App.AppVersion + " 🔒";
                            else
                                App.mn.versionlabel.Content = App.AppVersion;
                            App.mn.Set_Label(homex);
                        }
                    };
                    utils.Register(sc, fa, fa);
                    return;
                }
            }
            if(label == schedulex)
            {
                if (!App.Locked)
                    tc.SelectedIndex = 2;
                else
                {
                    System.ComponentModel.BackgroundWorker sc = new();
                    System.ComponentModel.BackgroundWorker fa = new();
                    sc.RunWorkerCompleted += delegate
                    {
                        App.Locked = false;
                        if (App.mn != null)
                        {
                            App.mn.versionlabel.Content = App.AppVersion;
                            App.mn.Set_Label(schedulex);
                        }
                    };
                    fa.RunWorkerCompleted += delegate
                    {
                        if (App.mn != null)
                        {
                            if (App.Locked)
                                App.mn.versionlabel.Content = App.AppVersion + " 🔒";
                            else
                                App.mn.versionlabel.Content = App.AppVersion;
                            App.mn.Set_Label(homex);
                        }
                    };
                    utils.Register(sc, fa, fa);
                    return;
                }
            }
            if(label == configx)
            {
                if (!App.Locked)
                    tc.SelectedIndex = 3;
                else
                {
                    System.ComponentModel.BackgroundWorker sc = new();
                    System.ComponentModel.BackgroundWorker fa = new();
                    sc.RunWorkerCompleted += delegate
                    {
                        App.Locked = false;
                        if (App.mn != null)
                        {
                            App.mn.versionlabel.Content = App.AppVersion;
                            App.mn.Set_Label(configx);
                        }
                    };
                    fa.RunWorkerCompleted += delegate
                    {
                        if (App.mn != null)
                        {
                            if (App.Locked)
                                App.mn.versionlabel.Content = App.AppVersion + " 🔒";
                            else
                                App.mn.versionlabel.Content = App.AppVersion;
                            App.mn.Set_Label(homex);
                        }
                    };
                    utils.Register(sc, fa, fa);
                    return;
                }
            }
            if(label == helpx)
            {
                tc.SelectedIndex = 4;
            }
            if(label == aboutx)
            {
                tc.SelectedIndex = 5;
            }
            if(label == newx)
            {
                newx.Visibility = Visibility.Visible;
                if(newflag == 1)
                {
                    tc.SelectedIndex = 6;
                    ntn9.Visibility = Visibility.Visible;
                    newx.Content = utils.Get_Transalte("mod");
                }
                else if(newflag == 0)
                {
                    tc.SelectedIndex = 6;
                    ntn9.Visibility = Visibility.Hidden;
                    newx.Content = utils.Get_Transalte("new");
                    tb7.Text = "";
                    tb8.Text = "";
                }
                else if(newflag == 2)
                {
                    tc.SelectedIndex = 7;
                    scbtn_4.Visibility = Visibility.Visible;
                    newx.Content = utils.Get_Transalte("mod");
                }
                else if(newflag == 3)
                {
                    tc.SelectedIndex = 7;
                    scbtn_4.Visibility = Visibility.Hidden;
                    newx.Content = utils.Get_Transalte("new");
                }
            }
            if (label == timex)
            {
                newx.Visibility = Visibility.Visible;
                timex.Visibility = Visibility.Visible;
                tc.SelectedIndex = 8;
            }
            label.IsEnabled = false;
            System.ComponentModel.BackgroundWorker bw = new();
            bw.RunWorkerCompleted += delegate
            {
                if (App.mn != null)
                    label.IsEnabled = true;
            };
            utils.Blur_Out(tc, bw);
        }

        private void Itemx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(itemx);
        }

        private void Configx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(configx);
        }

        private void Helpx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(helpx);
        }

        private void Aboutx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(aboutx);
        }

        private void Newx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(newx);
        }

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            newflag = 0;
            Set_Label(newx);
        }

        private void Btn2_Click(object sender, RoutedEventArgs e)
        {
            btn2.IsEnabled = false;
            utils.Delay(200);
            btn2.IsEnabled = true;
            if (App.cmditems.Count != 0 && dgi.SelectedIndex > 0 && dgi.SelectedIndex < App.cmditems.Count)
            {
                var i = dgi.SelectedIndex;
                cmditem nec = new(App.cmditems[i - 1].page, App.cmditems[i - 1].id, App.cmditems[i].name, App.cmditems[i].command);
                App.cmditems[i] = new(App.cmditems[i].page, App.cmditems[i].id, App.cmditems[i - 1].name, App.cmditems[i - 1].command);
                App.cmditems[i - 1] = nec;
                var inipath = App.dconfig;
                utils.Write_Ini(inipath, i.ToString(), "title", nec.name);
                utils.Write_Ini(inipath, i.ToString(), "command", "(" + nec.command + ")");
                utils.Write_Ini(inipath, (i + 1).ToString(), "title", App.cmditems[i].name);
                utils.Write_Ini(inipath, (i + 1).ToString(), "command", "(" + App.cmditems[i].command + ")");
                dgi.SelectedIndex = i - 1;
                App.Load_Menu();
            }
            GC.Collect();
        }

        private void Btn3_Click(object sender, RoutedEventArgs e)
        {
            btn3.IsEnabled = false;
            utils.Delay(200);
            btn3.IsEnabled = true;
            if (App.cmditems.Count != 0 && dgi.SelectedIndex > -1 && dgi.SelectedIndex < App.cmditems.Count - 1)
            {
                var i = dgi.SelectedIndex;
                cmditem nec = new(App.cmditems[i + 1].page, App.cmditems[i + 1].id, App.cmditems[i].name, App.cmditems[i].command);
                App.cmditems[i] = new(App.cmditems[i].page, App.cmditems[i].id, App.cmditems[i + 1].name, App.cmditems[i + 1].command);
                App.cmditems[i + 1] = nec;
                var inipath = App.dconfig;
                utils.Write_Ini(inipath, (i + 1).ToString(), "title", App.cmditems[i].name);
                utils.Write_Ini(inipath, (i + 1).ToString(), "command", "(" + App.cmditems[i].command + ")");
                utils.Write_Ini(inipath, (i + 2).ToString(), "title", App.cmditems[i + 1].name);
                utils.Write_Ini(inipath, (i + 2).ToString(), "command", "(" + App.cmditems[i + 1].command + ")");
                dgi.SelectedIndex = i + 1;
                App.Load_Menu();
            }
            GC.Collect();
        }

        private void Btn5_Click(object sender, RoutedEventArgs e)
        {
            btn5.IsEnabled = false;
            utils.Delay(200);
            btn5.IsEnabled = true;
            if (App.cmditems.Count != 0 && dgi.SelectedIndex > -1 && dgi.SelectedIndex < App.cmditems.Count)
            {
                var i = dgi.SelectedIndex;
                var inipath = App.dconfig;
                while (i < App.cmditems.Count - 1)
                {
                    App.cmditems[i].name = App.cmditems[i + 1].name;
                    App.cmditems[i].command = App.cmditems[i + 1].command;
                    utils.Write_Ini(inipath, (i + 1).ToString(), "title", utils.Read_Ini(inipath, (i + 2).ToString(), "title", " "));
                    utils.Write_Ini(inipath, (i + 1).ToString(), "command", utils.Read_Ini(inipath, (i + 2).ToString(), "command", " "));
                    i++;
                    System.Windows.Forms.Application.DoEvents();
                }
                utils.Write_Ini(inipath, (i + 1).ToString(), "title", " ");
                utils.Write_Ini(inipath, (i + 1).ToString(), "command", " ");
                App.cmditems.RemoveAt(i);
                var total = (App.cmditems.Count % 10 == 0) ? App.cmditems.Count / 10 : App.cmditems.Count / 10 + 1;
                if (App.page > total - 1 && App.page > 0)
                    App.page--;
                    App.Load_Menu();
            }
            GC.Collect();
        }

        private void Btn6_Click(object sender, RoutedEventArgs e)
        {
            if (App.cmditems.Count != 0 && dgi.SelectedIndex > -1 && dgi.SelectedIndex < App.cmditems.Count)
            {
                newflag = 1;
                tb7.Text = App.cmditems[dgi.SelectedIndex].name;
                tb8.Text = App.cmditems[dgi.SelectedIndex].command;
                Set_Label(newx);
            }
        }

        private void Rbtn3_Unchecked(object sender, RoutedEventArgs e)
        {
            tb1.IsEnabled = false;
        }

        private void Rbtn3_Checked(object sender, RoutedEventArgs e)
        {
            tb1.IsEnabled = true;
            utils.Write_Ini(App.dconfig, "config", "leftclick", "3");
        }

        private void Rbtn1_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "leftclick", "1");
        }

        private void Rbtn2_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "leftclick", "2");
        }

        private void Tb1_TextChanged(object sender, TextChangedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "leftaction", tb1.Text);
        }

        private void Rbtn6_Unchecked(object sender, RoutedEventArgs e)
        {
            tb2.IsEnabled = false;
        }

        private void Rbtn6_Checked(object sender, RoutedEventArgs e)
        {
            tb2.IsEnabled = true;
            utils.Write_Ini(App.dconfig, "config", "middleclick", "3");
        }

        private void Rbtn4_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "middleclick", "1");
        }

        private void Rbtn5_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "middleclick", "2");
        }

        private void Rbtn9_Checked(object sender, RoutedEventArgs e)
        {
            tb3.IsEnabled = true;
            utils.Write_Ini(App.dconfig, "config", "rightclick", "3");
        }

        private void Rbtn9_Unchecked(object sender, RoutedEventArgs e)
        {
            tb3.IsEnabled = false;
        }

        private void Rbtn7_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "rightclick", "1");
        }

        private void Rbtn8_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "rightclick", "2");
        }

        private void Rbtn10_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "customuser", "1");
            App.CustomUsernameFlag = 0;
            App.Username = App.EnvironmentUsername;
        }

        private void Rbtn11_Checked(object sender, RoutedEventArgs e)
        {
            tb4.IsEnabled = true;
            utils.Write_Ini(App.dconfig, "config", "customuser", "2");
            App.CustomUsernameFlag = 1;
            App.Username = utils.Read_Ini(App.dconfig, "config", "customname", "");
        }

        private void Rbtn11_Unchecked(object sender, RoutedEventArgs e)
        {
            tb4.IsEnabled = false;
        }

        private void Tb4_TextChanged(object sender, TextChangedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "customname", tb4.Text);
            App.Username = tb4.Text;
        }

        private void Rbtn12_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "autoexe", "1");
        }

        private void Rbtn13_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "autoexe", "2");
            tb5.IsEnabled = true;
        }

        private void Rbtn13_Unchecked(object sender, RoutedEventArgs e)
        {
            tb5.IsEnabled = false;
        }

        private void Tb5_TextChanged(object sender, TextChangedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "autoaction", tb5.Text);
        }

        private void Cb_box_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "min", "1");
        }

        private void Cb_box_Unchecked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "min", "0");
        }

        private void Btn7_Click(object sender, RoutedEventArgs e)
        {
            btn7.IsEnabled = false;
            System.ComponentModel.BackgroundWorker bw = new();
            bw.RunWorkerCompleted += delegate
            {
                App.Locked = true;
                versionlabel.Content = App.AppVersion + " 🔒";
                Set_Label(homex);
                btn7.IsEnabled = true;
            };
            bw.RunWorkerAsync();
                
        }

        private void Chk_btn_Click(object sender, RoutedEventArgs e)
        {
            if (chk_btn.Content.Equals(utils.Get_Transalte("checkup")))
            {
                chk_btn.Content = utils.Get_Transalte("checkcancel");
                pb.Visibility = Visibility.Visible;
                if (upbw != null)
                    upbw.CancelAsync();
                upbw = new();
                upbw.WorkerSupportsCancellation = true;
                upbw.DoWork += delegate
                {
                    ups = utils.GetWebContent("https://ftp.rexio.cn/hiro/hiro.php?r=update&v=" + res.ApplicationUpdateVersion + "&lang=" + App.lang);
                };
                upbw.RunWorkerCompleted += delegate
                {
                    check_update();
                };
                upbw.RunWorkerAsync();
            }
            else
            {
                chk_btn.Content = utils.Get_Transalte("checkup");
                pb.Visibility = Visibility.Hidden;

            }
        }

        private void Ntn1_Click(object sender, RoutedEventArgs e)
        {
            string val = tb8.Text;
            if (val.EndsWith(":\\"))
            {
                val = val[0..^2];
            }
            else if (val.EndsWith("\\"))
            {
                val = val[0..^1];
                val = val[(val.LastIndexOf("\\") + 1)..];
            }
            else
            {
                var poi = val.LastIndexOf('.');
                var slas = val.LastIndexOf("\\");
                if (poi != -1 && poi > slas)
                {
                    val = val[(val.LastIndexOf("\\") + 1)..];
                    val = val[..val.LastIndexOf(".")];
                }
                else
                {
                    val = val[(val.LastIndexOf("\\") + 1)..];
                }
            }
            tb7.Text = val;
        }

        private void Ntn2_Click(object sender, RoutedEventArgs e)
        {
            if (tb7.Text.IndexOf("[H]") + tb7.Text.IndexOf("[h]") > -2)
            {
                tb7.Text = tb7.Text.Replace("[h]", "").Replace("[H]", "");
            }
            else
            {
                tb7.Text = "[H]" + tb7.Text;
            }
        }

        private void Ntn3_Click(object sender, RoutedEventArgs e)
        {
            int val = tb7.SelectionStart;
            tb7.Text = string.Concat(tb7.Text.AsSpan(0, val), "[\\\\]", tb7.Text.AsSpan(val));
            tb7.SelectionStart = val + 4;
        }

        private void ntn4_Click(object sender, RoutedEventArgs e)
        {
            if (tb7.Text.IndexOf("[N]") + tb7.Text.IndexOf("[n]") > -2)
            {
                tb7.Text = tb7.Text.Replace("[N]", "").Replace("[n]", "");
            }
            else
            {
                tb7.Text = "[N]" + tb7.Text;
            }
        }

        private void ntn5_Click(object sender, RoutedEventArgs e)
        {
            if (tb7.Text.IndexOf("[S]") + tb7.Text.IndexOf("[s]") > -2)
            {
                tb7.Text = tb7.Text.Replace("[S]", "").Replace("[s]", "");
            }
            else
            {
                tb7.Text = "[S]" + tb7.Text;
            }
        }

        private void ntn6_Click(object sender, RoutedEventArgs e)
        {
            int val = tb8.SelectionStart;
            int val2 = tb8.SelectionLength;
            string str = tb8.SelectedText;
            if (val2 > 0)
            {
                if (str.StartsWith("\"") && str.EndsWith("\""))
                {
                    tb8.Text = string.Concat(tb8.Text.AsSpan(0, val), str.AsSpan(1, str.Length - 2), tb8.Text.AsSpan(val + val2));
                    tb8.SelectionStart = val + 1;
                    tb8.Select(val, str.Length - 2);
                }
                else
                {
                    tb8.Text = tb8.Text[..val] + "\"" + tb8.Text.Substring(val, val2) + "\"" + tb8.Text[(val + val2)..];
                    tb8.SelectionStart = val + 1;
                    tb8.Select(val, str.Length + 2);
                }

            }
            else
            {
                tb8.Text = string.Concat(tb8.Text.AsSpan(0, val), "\"\"", tb8.Text.AsSpan(val));
                tb8.SelectionStart = val + 1;
            }
        }

        private void Ntn7_Click(object sender, RoutedEventArgs e)
        {
            if (tb8.Text.StartsWith("explorer "))
                tb8.Text = tb8.Text[9..];
            else
                tb8.Text = "explorer " + tb8.Text;
        }

        private void Ntn8_Click(object sender, RoutedEventArgs e)
        {
            string strFileName = "";
            Microsoft.Win32.OpenFileDialog ofd = new();
            ofd.Filter = utils.Get_Transalte("allfiles") + "|*.*";
            ofd.ValidateNames = true; // 验证用户输入是否是一个有效的Windows文件名
            ofd.CheckFileExists = true; //验证路径的有效性
            ofd.CheckPathExists = true;//验证路径的有效性
            ofd.Title = utils.Get_Transalte("openfile") + " - " + App.AppTitle;
            if (ofd.ShowDialog() == true) //用户点击确认按钮，发送确认消息
            {
                strFileName = ofd.FileName;//获取在文件对话框中选定的路径或者字符串

            }
            if (System.IO.File.Exists(strFileName))
            {
                if (strFileName.ToLower().EndsWith(".lnk"))
                {
                    IWshRuntimeLibrary.WshShell shell = new();
                    IWshRuntimeLibrary.IWshShortcut lnkPath = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(strFileName);
                    strFileName = lnkPath.TargetPath.Equals("") ? strFileName : lnkPath.TargetPath;
                }
                if (strFileName.IndexOf(" ") != -1)
                    strFileName = "\"" + strFileName + "\"";
                strFileName = utils.Anti_Path_Prepare(strFileName);
                tb8.Text = strFileName;
                strFileName = strFileName[(strFileName.LastIndexOf("\\") + 1)..];
                if (strFileName.LastIndexOf(".") != -1)
                    strFileName = strFileName[..strFileName.LastIndexOf(".")];
                if (tb7.Text == "")
                    tb7.Text = strFileName;
            }
        }

        private void ntn9_Click(object sender, RoutedEventArgs e)
        {
            tb7.Text = App.cmditems[dgi.SelectedIndex].name;
            tb8.Text = App.cmditems[dgi.SelectedIndex].command;
        }

        private void ntnx_Click(object sender, RoutedEventArgs e)
        {
            tb7.Text = "";
            tb8.Text = "";
        }

        private void ntnx2_Click(object sender, RoutedEventArgs e)
        {
            tb7.Text = "";
            tb8.Text = "";
            Set_Label(itemx);
        }

        private void ntnx1_Click(object sender, RoutedEventArgs e)
        {
            if (tb7.Text.Equals(string.Empty) || tb8.Text.Equals(string.Empty))
            {
                return;
            }
            if (newflag == 0)
            {
                var i = App.cmditems.Count + 1;
                var p = (i % 10 == 0) ? i / 10 : i / 10 + 1;
                App.cmditems.Add(new cmditem(p, i, tb7.Text, tb8.Text));
                utils.Write_Ini(App.dconfig, i.ToString(), "title", tb7.Text);
                utils.Write_Ini(App.dconfig, i.ToString(), "command", "(" + tb8.Text + ")");
                tb7.Text = "";
                tb8.Text = "";
            }
            else
            {
                var i = dgi.SelectedIndex;
                App.cmditems[i].name = tb7.Text;
                App.cmditems[i].command = tb8.Text;
                utils.Write_Ini(App.dconfig, (i + 1).ToString(), "title", tb7.Text);
                utils.Write_Ini(App.dconfig, (i + 1).ToString(), "command", "(" + tb8.Text + ")");
                tb7.Text = "";
                tb8.Text = "";
            }
            App.Load_Menu();
            newflag = 0;
            Set_Label(itemx);
        }

        private void dgi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Btn6_Click(sender, e);
        }

        private void rbtn15_Checked(object sender, RoutedEventArgs e)
        {
            rbtn15.IsEnabled = false;
            rbtn14.IsEnabled = false;
            btn10.IsEnabled = true;
            utils.Write_Ini(App.dconfig, "config", "background", "2");
            utils.Set_Bgimage(bgimage);
            Blurbgi(utils.ConvertInt(utils.Read_Ini(App.dconfig, "config", "blur", "0")));
            rbtn15.IsEnabled = true;
            rbtn14.IsEnabled = true;
        }

        private void rbtn15_Unchecked(object sender, RoutedEventArgs e)
        {
            btn10.IsEnabled = false;
        }

        private void rbtn14_Checked(object sender, RoutedEventArgs e)
        {
            rbtn15.IsEnabled = false;
            rbtn14.IsEnabled = false;
            utils.Write_Ini(App.dconfig, "config", "background", "1");
            bgimage.Background = new SolidColorBrush(App.AppAccentColor);
            Blurbgi(0);
            rbtn15.IsEnabled = true;
            rbtn14.IsEnabled = true;
        }

        private void btn10_Click(object sender, RoutedEventArgs e)
        {
            string strFileName = "";
            Microsoft.Win32.OpenFileDialog ofd = new()
            {
                Filter = utils.Get_Transalte("picfiles") + "|*.jpg;*.jpeg;*.bmp;*.gif;*.png|" + utils.Get_Transalte("allfiles") + "|*.*",
                ValidateNames = true, // 验证用户输入是否是一个有效的Windows文件名
                CheckFileExists = true, //验证路径的有效性
                CheckPathExists = true,//验证路径的有效性
                Title = utils.Get_Transalte("openfile") + " - " + App.AppTitle
            };
            if (ofd.ShowDialog() == true) //用户点击确认按钮，发送确认消息
            {
                strFileName = ofd.FileName;//获取在文件对话框中选定的路径或者字符串

            }
            if (System.IO.File.Exists(strFileName))
            {
                utils.Write_Ini(App.dconfig, "config", "backimage", strFileName);
                utils.Set_Bgimage(bgimage);
                Blurbgi(Convert.ToInt16(utils.Read_Ini(App.dconfig, "config", "blur", "0")));
            }
        }

        private void btn4_Click(object sender, RoutedEventArgs e)
        {
            btn4.IsEnabled = false;
            utils.Delay(200);
            btn4.IsEnabled = true;
            if (App.cmditems.Count != 0 && dgi.SelectedIndex > -1 && dgi.SelectedIndex < App.cmditems.Count)
            {
                utils.RunExe(App.cmditems[dgi.SelectedIndex].command);
            }
        }

        private void tb8_Drop(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string path = (string)e.Data.GetData(DataFormats.FileDrop);
                tb8.Text = path;
                if (tb7.Text == "")
                    Ntn1_Click(sender, e);

            }
            if(e.Data.GetDataPresent(DataFormats.Text))
            {
                string path = (string)e.Data.GetData(DataFormats.Text);
                tb8.Text = path;
                if (tb7.Text == "")
                    Ntn1_Click(sender, e);
            }
            return;
        }

        private void tb7_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string path = (string)e.Data.GetData(DataFormats.FileDrop);
                var st = tb8.Text;
                tb8.Text = path;
                Ntn1_Click(sender, e);
                if (st != "")
                    tb8.Text = st;

            }
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string path = (string)e.Data.GetData(DataFormats.FileDrop);
                var st = tb8.Text;
                tb8.Text = path;
                Ntn1_Click(sender, e);
                if (st != "")
                    tb8.Text = st;
            }
            return;
        }

        private void tb7_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void tb8_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void tb8_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void tb7_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Minbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void tb2_TextChanged(object sender, TextChangedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "middleaction", tb2.Text);
        }

        private void tb3_TextChanged(object sender, TextChangedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "rightaction", tb3.Text);
        }

        private void ui_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (utils.Read_Ini(App.dconfig, "config", "min", "1").Equals("1"))
            {
                Visibility = Visibility.Hidden;
            }
            else
            {
                utils.RunExe("exit()");
                utils.LogtoFile("[INFOMATION]Main UI: Closing " + e.GetType().ToString());  
            }
            e.Cancel = true;
        }

        private void configbar_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            configbar.Value += e.Delta * (configbar.Maximum - configbar.ViewportSize) / configbar.ViewportSize;
        }

        private void titlelabel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Thickness thickness = versionlabel.Margin;
            thickness.Left = titlelabel.Margin.Left + titlelabel.ActualWidth + 5;
            versionlabel.Margin = thickness;
        }

        private void rbtn17_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "customnick", "2");
            tb10.IsEnabled = true;
            App.AppTitle = tb10.Text;
            Title = App.AppTitle + " - " + utils.Get_Transalte("version").Replace("%c", App.AppVersion);
            titlelabel.Content = App.AppTitle;
            if (App.wnd != null)
                App.wnd.ti.ToolTipText = tb10.Text;
        }

        private void rbtn17_Unchecked(object sender, RoutedEventArgs e)
        {
            
        }

        private void tb10_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(utils.Read_Ini(App.dconfig, "config", "customnick", "1").Equals("2"))
            {
                utils.Write_Ini(App.dconfig, "config", "customhiro", tb10.Text);
                App.AppTitle = tb10.Text;
                Title = App.AppTitle + " - " + utils.Get_Transalte("version").Replace("%c", App.AppVersion);
                titlelabel.Content = App.AppTitle;
                if (App.wnd != null)
                    App.wnd.ti.ToolTipText = tb10.Text;
            }
            
        }

        private void autorun_Checked(object sender, RoutedEventArgs e)
        {
            if (autorun.Tag != null && autorun.Tag.Equals("1"))
            {
                autorun.Tag = "2";
                try
                {
                    if (Environment.ProcessPath != null)
                    {
                        System.Diagnostics.ProcessStartInfo pinfo = new();
                        pinfo.UseShellExecute = true;
                        pinfo.FileName = Environment.ProcessPath;
                        pinfo.Arguments = "autostart_on";
                        pinfo.Verb = "runas";
                        System.Diagnostics.Process.Start(pinfo);
                    }
                }
                catch (Exception ex)
                {
                    autorun.IsChecked = false;
                    utils.LogtoFile("[ERROR]" + ex.Message);
                }
            }
            autorun.Tag = "1";
        }

        private void autorun_Unchecked(object sender, RoutedEventArgs e)
        {
            if (autorun.Tag != null && autorun.Tag.Equals("1"))
            {
                autorun.Tag = "2";
                try
                {
                    if (Environment.ProcessPath != null)
                    {
                        System.Diagnostics.ProcessStartInfo pinfo = new();
                        pinfo.UseShellExecute = true;
                        pinfo.FileName = Environment.ProcessPath;
                        pinfo.Arguments = "autostart_off";
                        pinfo.Verb = "runas";
                        System.Diagnostics.Process.Start(pinfo);
                    }
                }
                catch (Exception ex)
                {
                    autorun.IsChecked = true;
                    utils.LogtoFile("[ERROR]" + ex.Message);
                }
            }
            autorun.Tag = "1";
        }

        private void rbtn16_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "customnick", "1");
            tb10.IsEnabled = false;
            App.AppTitle = res.ApplicationName;
            Title = App.AppTitle + " - " + utils.Get_Transalte("version").Replace("%c", App.AppVersion);
            titlelabel.Content = App.AppTitle;
            if (App.wnd != null)
                App.wnd.ti.ToolTipText = App.AppTitle;
        }

        private void schedulex_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(schedulex);
        }

        private void scbtn_1_Click(object sender, RoutedEventArgs e)
        {
            newflag = 3;
            tb11.Text = "";
            tb12.Text = "";
            tb13.Text = "";
            tb14.Text = "";
            Set_Label(newx);
        }

        private void scbtn_3_Click(object sender, RoutedEventArgs e)
        {
            if (App.scheduleitems.Count != 0 && dgs.SelectedIndex > -1 && dgs.SelectedIndex < App.scheduleitems.Count)
            {
                newflag = 2;
                tb11.Text = App.scheduleitems[dgs.SelectedIndex].name;
                tb12.Text = App.scheduleitems[dgs.SelectedIndex].time;
                tb13.Text = App.scheduleitems[dgs.SelectedIndex].command;
                tb14.Text = "";
                switch (App.scheduleitems[dgs.SelectedIndex].re)
                {
                    case -2.0:
                        rbtn18.IsChecked = true;
                        break;
                    case -1.0:
                        rbtn19.IsChecked = true;
                        break;
                    case 0.0:
                        rbtn20.IsChecked = true;
                        break;
                    default:
                        rbtn21.IsChecked = true;
                        tb14.Text = App.scheduleitems[dgs.SelectedIndex].re.ToString();
                        break;
                }
                Set_Label(newx);
            }
        }

        private void scbtn_2_Click(object sender, RoutedEventArgs e)
        {
            if (App.scheduleitems.Count != 0 && dgs.SelectedIndex > -1 && dgs.SelectedIndex < App.scheduleitems.Count)
            {
                utils.Delete_Alarm(dgs.SelectedIndex);
                
            }
        }

        private void scbtn_4_Click(object sender, RoutedEventArgs e)
        {
            tb11.Text = App.scheduleitems[dgs.SelectedIndex].name;
            tb12.Text = App.scheduleitems[dgs.SelectedIndex].time;
            tb13.Text = App.scheduleitems[dgs.SelectedIndex].command;
            tb14.Text = "";
            switch (App.scheduleitems[dgs.SelectedIndex].re)
            {
                case -2.0:
                    rbtn18.IsChecked = true;
                    break;
                case -1.0:
                    rbtn19.IsChecked = true;
                    break;
                case 0.0:
                    rbtn20.IsChecked = true;
                    break;
                default:
                    rbtn21.IsChecked = true;
                    tb14.Text = App.scheduleitems[dgs.SelectedIndex].re.ToString();
                    break;
            }
        }

        private void scbtn_5_Click(object sender, RoutedEventArgs e)
        {
            if (tb11.Text.Equals(string.Empty) || tb12.Text.Equals(string.Empty) || tb13.Text.Equals(string.Empty) || (tb14.Text.Equals(string.Empty) && tb14.IsEnabled == true) )
            {
                return;
            }
            double re = -2.0;
            if (rbtn19.IsChecked == true)
                re = -1.0;
            if (rbtn20.IsChecked == true)
                re = 0.0;
            if(rbtn21.IsChecked == true)
            {
                try
                {
                    re = double.Parse(tb14.Text);
                }
                catch(Exception ex)
                {
                    utils.LogtoFile("[ERROR]" + ex.Message);
                    re = -2.0;
                }
            }
            if (newflag == 3)
            {
                var i = App.scheduleitems.Count + 1;
                App.scheduleitems.Add(new scheduleitem(i, tb11.Text, tb12.Text, tb13.Text,re));
                utils.Write_Ini(App.sconfig, i.ToString(), "name", tb11.Text);
                utils.Write_Ini(App.sconfig, i.ToString(), "time", tb12.Text);
                utils.Write_Ini(App.sconfig, i.ToString(), "command", "(" + tb13.Text + ")");
                utils.Write_Ini(App.sconfig, i.ToString(), "repeat", re.ToString());
            }
            else
            {
                var i = dgs.SelectedIndex;
                App.scheduleitems[i].name = tb11.Text;
                App.scheduleitems[i].time = tb12.Text;
                App.scheduleitems[i].command = tb13.Text;
                App.scheduleitems[i].re = re;
                utils.Write_Ini(App.sconfig, (i + 1).ToString(), "name", tb11.Text);
                utils.Write_Ini(App.sconfig, (i + 1).ToString(), "time", tb12.Text);
                utils.Write_Ini(App.sconfig, (i + 1).ToString(), "command", "(" + tb13.Text + ")");
                utils.Write_Ini(App.sconfig, (i + 1).ToString(), "repeat", re.ToString());
            }
            System.Globalization.DateTimeFormatInfo dtFormat = new()
            {
                ShortDatePattern = "yyyy/MM/dd HH:mm:ss"
            };
            DateTime dt = Convert.ToDateTime(tb12.Text, dtFormat);
            DateTime now = DateTime.Now;
            TimeSpan ts = dt - now;
            int day, hour, minute;
            if (ts.TotalDays < 0)
            {
                tb11.Text = "";
                tb12.Text = "";
                tb13.Text = "";
                newflag = 0;
                Set_Label(schedulex);
                App.Notify(new noticeitem(utils.Get_Transalte("sctimepassed"), 2, schedulex.Content.ToString()));
            }
            else
            {
                if (ts.TotalDays > 1.0)
                    day = Convert.ToInt32(Math.Truncate(ts.TotalDays));
                else
                    day = 0;
                if (ts.TotalHours - 24 * day > 1.0)
                    hour = Convert.ToInt32(Math.Truncate(ts.TotalHours - 24 * day));
                else
                    hour = 0;
                if (ts.TotalMinutes - 60 * hour - 24 * 60 * day > 1.0)
                    minute = Convert.ToInt32(Math.Truncate(ts.TotalMinutes - 60 * hour - 24 * 60 * day));
                else
                    minute = 0;
                tb11.Text = "";
                tb12.Text = "";
                tb13.Text = "";
                newflag = 0;
                Set_Label(schedulex);
                if (day > 0)
                {
                    App.Notify(new noticeitem(utils.Get_Transalte("sctimeday").Replace("%d", day.ToString()).Replace("%h", hour.ToString()).Replace("%m", minute.ToString()), 2, schedulex.Content.ToString()));
                }
                else if (hour > 0)
                {
                    App.Notify(new noticeitem(utils.Get_Transalte("sctimehour").Replace("%d", day.ToString()).Replace("%h", hour.ToString()).Replace("%m", minute.ToString()), 2, schedulex.Content.ToString()));
                }
                else
                {
                    App.Notify(new noticeitem(utils.Get_Transalte("sctimemin").Replace("%d", day.ToString()).Replace("%h", hour.ToString()).Replace("%m", minute.ToString()), 2, schedulex.Content.ToString()));
                }
            }
            
        }

        private void scbtn_7_Click(object sender, RoutedEventArgs e)
        {
            tb11.Text = "";
            tb12.Text = "";
            tb13.Text = "";
            Set_Label(schedulex);
        }

        private void scbtn_6_Click(object sender, RoutedEventArgs e)
        {
            tb11.Text = "";
            tb12.Text = "";
            tb13.Text = "";
            rbtn18.IsChecked = true;
            tb14.Text = "";
        }

        private void Dgs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            scbtn_3_Click(sender, e);
        }

        private void Scbtn_8_Click(object sender, RoutedEventArgs e)
        {
            tb12.Text = DateTime.Now.AddMinutes(15.0).ToString("yyyy/MM/dd HH:mm:ss");
        }

        private void Scbtn_9_Click(object sender, RoutedEventArgs e)
        {
            tb12.Text = DateTime.Now.AddHours(1.0).ToString("yyyy/MM/dd HH:mm:ss");
        }

        private void Scbtn_10_Click(object sender, RoutedEventArgs e)
        {
            tb12.Text = DateTime.Now.AddDays(1.0).ToString("yyyy/MM/dd HH:mm:ss");
        }

        private void Rbtn21_Checked(object sender, RoutedEventArgs e)
        {
            tb14.IsEnabled = true;
        }

        private void Rbtn21_Unchecked(object sender, RoutedEventArgs e)
        {
            tb14.IsEnabled = false;
        }

        private void Tb12_GotFocus(object sender, RoutedEventArgs e)
        {
            Go_TimePicker();
        }

        private void Tb12_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Go_TimePicker();
        }

        private void Go_TimePicker()
        {
            DateTime dt = DateTime.Now;
            try
            {
                System.Globalization.DateTimeFormatInfo dtFormat = new()
                {
                    ShortDatePattern = "yyyy/MM/dd HH:mm:ss"
                };
                dt = Convert.ToDateTime(tb12.Text, dtFormat);
            }
            catch (Exception ex)
            {
                utils.LogtoFile("[ERROR]" + ex.Message);
            }
            year.Text = dt.Year.ToString();
            month.Text = dt.Month.ToString();
            day.Text = dt.Day.ToString();
            hour.Text = dt.Hour.ToString();
            minute.Text = dt.Minute.ToString();
            second.Text = dt.Second.ToString();
            Set_Label(timex);
        }

        private void Blureff_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "blur", "1");
            App.blurradius = 50.0;
            Blurbgi(1);
        }
        private void Blurbgi(int direction)
        {
            if (bflag == 1)
                return;
            bflag = 1;
            blureff.IsEnabled = false;
            rbtn14.IsEnabled = false;
            rbtn15.IsEnabled = false;
            btn10.IsEnabled = false;
            foreach (Window win in Application.Current.Windows)
            {
                if (win is Alarm a)
                    a.Loadbgi(direction);
                if (win is message e)
                    e.Loadbgi(direction);
                if (win is Sequence c)
                    c.Loadbgi(direction);
                if (win is Download d)
                    d.Loadbgi(direction);
                System.Windows.Forms.Application.DoEvents();
            }
            bool animation = !utils.Read_Ini(App.dconfig, "config", "ani", "1").Equals("0");
            System.ComponentModel.BackgroundWorker bw = new();
            bw.RunWorkerCompleted += delegate
            {
                if (App.mn != null)
                {
                    blureff.IsEnabled = true;
                    rbtn14.IsEnabled = true;
                    rbtn15.IsEnabled = true;
                    if (rbtn15.IsChecked == true)
                        btn10.IsEnabled = true;
                    blureff.IsEnabled = btn10.IsEnabled;
                }
            };
            utils.Blur_Animation(direction, animation, bgimage, this, bw);
            bflag = 0;
        }
        private void Blureff_Unchecked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "blur", "0");
            App.blurradius = 50.0;
            Blurbgi(0);
        }

        private void verbose_Unchecked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "verbose", "0");
        }

        private void verbose_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "verbose", "1");
        }

        private void animation_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "ani", "1");
        }

        private void animation_Unchecked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "ani", "0");
        }

        private void tc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void tb8_TextChanged(object sender, TextChangedEventArgs e)
        {
            tb9.Text = utils.Get_CMD_Translation(tb8.Text);
            if(tb9.Text.Equals(""))
            {
                tb9.Visibility = Visibility.Hidden;
            }
            else
            {
                tb9.Visibility = Visibility.Visible;
            }
        }

        private void btn8_Click(object sender, RoutedEventArgs e)
        {
            btn8.IsEnabled = false;
            utils.RunExe("mailto:xboriver@live.cn");
            utils.Delay(200);
            btn8.IsEnabled = true;
        }

        private void btn9_Click(object sender, RoutedEventArgs e)
        {
            btn9.IsEnabled = false;
            if (whatsbw != null)
            {
                return;
            }
            whatsbw = new();
            string wps = "";
            whatsbw.DoWork += delegate
            {
                wps = utils.GetWebContent("https://ftp.rexio.cn/hiro/new.php?ver=" + App.AppVersion + "&lang=" + App.lang);
            };
            whatsbw.RunWorkerCompleted += delegate
            {
                try
                {
                    string ti = wps.Substring(wps.IndexOf("<title>") + "<title>".Length);
                    ti = ti.Substring(0, ti.IndexOf("<"));
                    wps = wps.Substring(wps.IndexOf("</head>") + "</head>".Length);
                    utils.RunExe("alarm(" + ti + "," + wps.Replace("<br>", "\\n") + ")");
                    whatsbw.Dispose();
                    whatsbw = null;
                }
                catch (Exception ex)
                {
                    utils.LogtoFile("[ERROR]" + ex.Message);
                }
                
            };
            whatsbw.RunWorkerAsync();
            utils.Delay(200);
            btn9.IsEnabled = true;
        }

        public void check_update()
        {
            chk_btn.Content = utils.Get_Transalte("checkup");
            pb.Visibility = Visibility.Hidden;
            if (ups == "latest")
            {
                App.Notify(new noticeitem(utils.Get_Transalte("updatelatest"), 2, utils.Get_Transalte("checkup")));
            }
            else if (ups == "Error")
            {
                App.Notify(new noticeitem(utils.Get_Transalte("updateerror"), 2, utils.Get_Transalte("checkup")));
            }
            else
            {
                try
                {
                    string version = ups.Substring(ups.IndexOf("version:[") + "version:[".Length);
                    version = version.Substring(0, version.IndexOf("]"));
                    string info = ups.Substring(ups.IndexOf("info:[") + "info:[".Length);
                    info = info.Substring(0, info.IndexOf("]")).Replace("\\n", Environment.NewLine);
                    string url = ups.Substring(ups.IndexOf("url:[") + "url:[".Length);
                    url = url.Substring(0, url.IndexOf("]"));
                    if (utils.Read_Ini(App.dconfig, "config", "toast", "0").Equals("1"))
                    {
                        new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                                .AddText(utils.Get_Transalte("updatetitle"))
                                .AddText(utils.Get_Transalte("updatecontent").Replace("%v", version).Replace("%n", info).Replace("\\n", Environment.NewLine))
                                .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                            .SetContent(utils.Get_Transalte("updateok"))
                                            .AddArgument("action", "uok"))
                                .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                            .SetContent(utils.Get_Transalte("updateskip"))
                                            .AddArgument("action", "uskip"))
                                .AddArgument("url", url)
                            .Show();
                    }
                    else
                    {
                        Alarm up = new(-415, utils.Get_Transalte("updatetitle"), utils.Path_Prepare_EX(utils.Get_Transalte("updatecontent").Replace("%v", version).Replace("%n", info).Replace("\\n", Environment.NewLine).Replace("<br>", Environment.NewLine)), 2);
                        up.url = url;
                        up.Show();
                    }
                }
                catch (Exception ex)
                {
                    utils.LogtoFile("[ERROR]" + ex.Message);
                    App.Notify(new noticeitem(utils.Get_Transalte("updateerror"), 2, utils.Get_Transalte("checkup")));
                }
                
                    
            }

        }

        private void schedulex_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(App.dflag)
            {
                DateTime dt = DateTime.Now.AddSeconds(5);
                for (int i = 0; i < 1; i++)
                {
                    dt = dt.AddSeconds(2);
                    App.scheduleitems.Add(new scheduleitem(App.scheduleitems.Count + 1, "Test" + i.ToString(), dt.ToString("yyyy/MM/dd HH:mm:ss"), "alarm", -1.0));

                }
            }
            
        }

        private void Extend_Animation()
        {
            extended.IsEnabled = false;
            System.ComponentModel.BackgroundWorker bw = new();
            bw.RunWorkerCompleted += delegate
            {
                if (App.mn != null)
                    extended.IsEnabled = true;
            };
            utils.Blur_Out(extended, bw);
        }
        private void extend_background_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            extended.IsEnabled = false;
            extend_background.IsEnabled = false;
            Storyboard? sb = new();
            sb = utils.AddDoubleAnimaton(0, App.blursec, extended, "Opacity", sb);
            sb = utils.AddDoubleAnimaton(0, App.blursec, extend_background, "Opacity", sb);
            sb.Completed += delegate
            {
                extended.Opacity = 0;
                extend_background.Opacity = 0;
                extended.Visibility = Visibility.Hidden;
                extend_background.Visibility = Visibility.Hidden;
                sb = null;
            };
            sb.Begin();
        }

        private void extended_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Extend_Animation();
            touch++;
            utils.RunExe("notify(https://ftp.rexio.cn/hiro/hiro.php?r=touch&t=" + touch.ToString() + "&lang=" + App.lang + ",2)");
        }

        private void avatar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Thickness th = extend_background.Margin;
            th.Left = 0;
            th.Top = 0;
            extend_background.Margin = th;
            extend_background.Width = Width;
            extend_background.Height = Height;
            extend_background.Background = new SolidColorBrush(Colors.Coral);
            extended.Visibility = Visibility.Visible;
            extend_background.Visibility = Visibility.Visible; 
            Storyboard? sb = new();
            sb = utils.AddDoubleAnimaton(1, App.blursec, extended, "Opacity", sb, 0);
            sb = utils.AddDoubleAnimaton(1, App.blursec, extend_background, "Opacity", sb, 0);
            sb.Completed += delegate
            {
                extended.Opacity = 1;
                extend_background.Opacity = 1;
                extended.IsEnabled = true;
                extend_background.IsEnabled = true;
                sb = null;
            };
            sb.Begin();
        }

        private void win_style_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "toast", "1");
        }

        private void win_style_Unchecked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "toast", "0");
        }

        private void langbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.lang = App.la[langbox.SelectedIndex].name;
            App.LangFilePath = App.CurrentDirectory + "\\system\\lang\\" + App.la[langbox.SelectedIndex].name + ".hlp";
            utils.Write_Ini(App.dconfig, "config", "lang", App.lang);
            Load_Translate();
            Load_Position();
            App.Load_Menu();
        }

        private void ui_Loaded(object sender, RoutedEventArgs e)
        {
            langbox.Items.Clear();
            langbox.ItemsSource = App.la;
            langbox.DisplayMemberPath = "langname";
            langbox.SelectedValuePath = "langname";
            for (int i = 0; i < App.la.Count; i++)
            {
                if (App.lang.Equals(App.la[i].name))
                {
                    langbox.SelectedIndex = i;
                }
            }
        }

        private void versionlabel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (App.dflag)
            {
                Load_Translate();
                Load_Position();
            }
            else
            {
                if (App.Locked)
                    utils.RunExe("auth()");
                else
                    utils.RunExe("lock()");
            }
        }

        private void reverse_style_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "reverse", "1");
            if (App.wnd != null)
                App.wnd.Load_All_Colors();
        }

        private void reverse_style_Unchecked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "reverse", "0");
            if (App.wnd != null)
                App.wnd.Load_All_Colors();
        }

        private void lock_style_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "lock", "1");
            utils.Write_Ini(App.dconfig, "config", "lockcolor", string.Format("#{0:X2}{1:X2}{2:X2}", App.AppAccentColor.R, App.AppAccentColor.G, App.AppAccentColor.B));
        }

        private void lock_style_Unchecked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "lock", "0");
            if (App.wnd != null)
                App.wnd.Load_All_Colors();
        }

        private void Blureff_Indeterminate(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "blur", "2");
            Blurbgi(3);
            App.blurradius = 25;
        }

        private void timex_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(timex);
        }
        private void Tp_Cancel(object sender, RoutedEventArgs e)
        {
            Set_Label(newx);
        }

        private String string_deal(String a, int b)
        {
            try
            {
                a = int.Parse(a).ToString();
            }
            catch (Exception ex)
            {
                a = "0";
                utils.LogtoFile("[ERROR]" + ex.Message);
            }
            if (a.Length > b)
            {
                a = a.Substring(0, b);
            }
            else if (a.Length < b)
            {
                while (a.Length < b)
                    a = "0" + a;
            }
            return a;
        }

        private void Tp_Go(object sender, RoutedEventArgs e)
        {
            tb12.Text = string_deal(year.Text, 4) + "/" + string_deal(month.Text, 2) + "/" + string_deal(day.Text, 2) + " " + string_deal(hour.Text, 2) + ":" + string_deal(minute.Text, 2) + ":" + string_deal(second.Text, 2);
            Set_Label(newx);
        }

        private void year_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                try
                {
                    year.Text = (int.Parse(year.Text) + 1).ToString();
                }
                catch
                {
                    year.Text = DateTime.Now.Year.ToString();
                }
            }
            else
            {
                try
                {
                    year.Text = (int.Parse(year.Text) - 1).ToString();
                }
                catch
                {
                    year.Text = DateTime.Now.Year.ToString();
                }
            }
        }

        private void month_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                try
                {
                    if (int.Parse(month.Text) + 1 < 13)
                        month.Text = (int.Parse(month.Text) + 1).ToString();
                    else
                        month.Text = "12";
                }
                catch
                {
                    month.Text = DateTime.Now.Month.ToString();
                }
            }
            else
            {
                try
                {
                    if (int.Parse(month.Text) - 1 > 0)
                        month.Text = (int.Parse(month.Text) - 1).ToString();
                    else
                        month.Text = "01";
                }
                catch
                {
                    month.Text = DateTime.Now.Month.ToString();
                }
            }
        }

        private void hour_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                try
                {
                    if (int.Parse(hour.Text) + 1 < 24)
                        hour.Text = (int.Parse(hour.Text) + 1).ToString();
                    else
                        hour.Text = "23";
                }
                catch
                {
                    hour.Text = DateTime.Now.Hour.ToString();
                }
            }
            else
            {
                try
                {
                    if (int.Parse(hour.Text) - 1 > -1)
                        hour.Text = (int.Parse(hour.Text) - 1).ToString();
                    else
                        hour.Text = "00";
                }
                catch
                {
                    hour.Text = DateTime.Now.Hour.ToString();
                }
            }
        }

        private void minute_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                try
                {
                    if (int.Parse(minute.Text) + 1 < 60)
                        minute.Text = (int.Parse(minute.Text) + 1).ToString();
                    else
                        minute.Text = "59";
                }
                catch
                {
                    minute.Text = DateTime.Now.Minute.ToString();
                }
            }
            else
            {
                try
                {
                    if (int.Parse(minute.Text) - 1 > -1)
                        minute.Text = (int.Parse(minute.Text) - 1).ToString();
                    else
                        minute.Text = "00";
                }
                catch
                {
                    minute.Text = DateTime.Now.Minute.ToString();
                }
            }
        }

        private void second_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                try
                {
                    if (int.Parse(second.Text) + 1 < 60)
                        second.Text = (int.Parse(second.Text) + 1).ToString();
                    else
                        second.Text = "59";
                }
                catch
                {
                    second.Text = DateTime.Now.Second.ToString();
                }
            }
            else
            {
                try
                {
                    if (int.Parse(second.Text) - 1 > -1)
                        second.Text = (int.Parse(second.Text) - 1).ToString();
                    else
                        second.Text = "00";
                }
                catch
                {
                    second.Text = DateTime.Now.Second.ToString();
                }
            }
        }

        private void day_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                try
                {
                    if (int.Parse(day.Text) + 1 <= MaxDay)
                        day.Text = (int.Parse(day.Text) + 1).ToString();
                    else
                        day.Text = MaxDay.ToString();
                }
                catch
                {
                    day.Text = DateTime.Now.Day.ToString();
                }
            }
            else
            {
                try
                {
                    if (int.Parse(day.Text) - 1 > -1)
                        day.Text = (int.Parse(day.Text) - 1).ToString();
                    else
                        day.Text = "01";
                }
                catch
                {
                    day.Text = DateTime.Now.Day.ToString();
                }
            }
        }

        private void month_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (month != null)
                {
                    month.Text = int.Parse(month.Text).ToString();
                    if (int.Parse(month.Text) < 1)
                        month.Text = "1";
                    if (int.Parse(month.Text) > 12)
                        month.Text = "12";
                }
            }
            catch
            {
                month.Text = DateTime.Now.Month.ToString();
            }

            try
            {
                if (month != null && year != null)
                    MaxDay = System.Threading.Thread.CurrentThread.CurrentUICulture.Calendar.GetDaysInMonth(int.Parse(year.Text), int.Parse(month.Text));
                else
                    MaxDay = 28;
            }
            catch
            {
                MaxDay = 28;
            }
            if (day != null && int.Parse(day.Text) > MaxDay)
                day.Text = MaxDay.ToString();
        }

        private void day_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (day != null)
                {
                    day.Text = int.Parse(day.Text).ToString();
                    if (int.Parse(day.Text) < 1)
                        day.Text = "1";
                    if (int.Parse(day.Text) > MaxDay)
                        day.Text = MaxDay.ToString();
                }
            }
            catch
            {
                day.Text = DateTime.Now.Day.ToString();
            }
        }

        private void year_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (year != null)
                {
                    year.Text = int.Parse(year.Text).ToString();
                }
            }
            catch
            {
                year.Text = DateTime.Now.Year.ToString();
            }
            try
            {
                if (month != null && year != null)
                    MaxDay = System.Threading.Thread.CurrentThread.CurrentUICulture.Calendar.GetDaysInMonth(int.Parse(year.Text), int.Parse(month.Text));
                else
                    MaxDay = 28;
            }
            catch
            {
                MaxDay = 28;
            }
            if (day != null && int.Parse(day.Text) > MaxDay)
                day.Text = MaxDay.ToString();
        }

        private void hour_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                if (hour != null)
                {
                    hour.Text = int.Parse(hour.Text).ToString();
                    if (int.Parse(hour.Text) < 0)
                        hour.Text = "0";
                    if (int.Parse(hour.Text) > 23)
                        hour.Text = "23";
                }
            }
            catch
            {
                hour.Text = DateTime.Now.Hour.ToString();
            }
        }

        private void minute_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (minute != null)
                {
                    minute.Text = int.Parse(minute.Text).ToString();
                    if (int.Parse(minute.Text) < 0)
                        minute.Text = "0";
                    if (int.Parse(minute.Text) > 59)
                        minute.Text = "59";
                }
            }
            catch
            {
                minute.Text = DateTime.Now.Minute.ToString();
            }
        }

        private void second_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (second != null)
                {
                    second.Text = int.Parse(second.Text).ToString();
                    if (int.Parse(second.Text) < 0)
                        second.Text = "0";
                    if (int.Parse(second.Text) > 59)
                        second.Text = "59";
                }
            }
            catch
            {
                second.Text = DateTime.Now.Second.ToString();
            }
        }
    }
}
