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
    public partial class Hiro_MainUI : Window
    {
        internal Page? current = null;
        private Label? selected = null;
        private int bflag = 0;        
        internal int touch = 0;
        internal Hiro_Home? hiro_home = null;
        internal Hiro_Items? hiro_items = null;
        internal Hiro_Schedule? hiro_schedule = null;
        internal Hiro_Config? hiro_config = null;
        internal Hiro_Help? hiro_help = null;
        internal Hiro_About? hiro_about = null;
        internal Hiro_NewItem? hiro_newitem = null;
        internal Hiro_NewSchedule? hiro_newschedule = null;
        internal Hiro_Time? hiro_time = null;
        internal Hiro_Color? hiro_color = null;

        public Hiro_MainUI()
        {
            InitializeComponent();
            Hiro_Utils.LogtoFile("[HIROWEGO]Main UI: Initializing");
            SourceInitialized += OnSourceInitialized;
            MainUI_Initialize();
            MainUI_FirstInitialize();
            Loaded += delegate
            {
                HiHiro();
            };
        }

        public void MainUI_Initialize()
        {
            InitializeUIWindow();
            Hiro_Utils.LogtoFile("[HIROWEGO]Main UI: Load Transaltation");
            Load_Translate();
            Hiro_Utils.LogtoFile("[HIROWEGO]Main UI: Load Data");
            Load_Data();
            Hiro_Utils.LogtoFile("[HIROWEGO]Main UI: Load Colors");
            Load_Colors();
            Hiro_Utils.LogtoFile("[HIROWEGO]Main UI: Load Position");
            Load_Position();
        }

        public void MainUI_FirstInitialize()
        {
            Hiro_Utils.LogtoFile("[HIROWEGO]Main UI: Set Home");
            Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - this.Width / 2);
            Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - this.Height / 2);
            Hiro_Utils.LogtoFile("[HIROWEGO]Main UI: Intitalized");
            Hiro_Utils.LogtoFile("[HIROWEGO]Main UI: Loaded");
        }

        public void HiHiro()
        {
            Blurbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
            if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
            {
                if (selected == null)
                    Set_Label(homex);
                else
                    Set_Label(selected);
            }
            titlelabel.Visibility = Visibility.Visible;
            versionlabel.Visibility = Visibility.Visible;
            minbtn.Visibility = Visibility.Visible;
            closebtn.Visibility = Visibility.Visible;
            stack.Visibility = Visibility.Visible;
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                Storyboard sb = new();
                Hiro_Utils.AddPowerAnimation(0, titlelabel, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(0, versionlabel, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(2, minbtn, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(2, closebtn, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(0, stack, sb, -50, null);
                sb.Begin();
            }
            Hiro_Utils.SetWindowToForegroundWithAttachThreadInput(this);
        }

        public void InitializeUIWindow()
        {
            if (App.Locked)
                versionlabel.Content = res.ApplicationVersion + " 🔒";
            else
                versionlabel.Content = res.ApplicationVersion;
            Hiro_Utils.SetShadow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
        }
        
        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;
            System.Windows.Interop.HwndSource source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source.AddHook(WndProc);
            Hiro_Utils.LogtoFile("[HIROWEGO]Main UI: AddHook WndProc");
            WindowStyle = WindowStyle.SingleBorderWindow;
        }
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hwnd);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);
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
            Hiro_Utils.Set_Control_Location(titlelabel, "title");
            Hiro_Utils.Set_Control_Location(versionlabel, "version");
            Hiro_Utils.Set_Control_Location(homex, "home", location: false);
            Hiro_Utils.Set_Control_Location(itemx, "item", location: false);
            Hiro_Utils.Set_Control_Location(schedulex, "schedule", location: false);
            Hiro_Utils.Set_Control_Location(configx, "config", location: false);
            Hiro_Utils.Set_Control_Location(helpx, "help", location: false);
            Hiro_Utils.Set_Control_Location(aboutx, "about", location: false);
            Hiro_Utils.Set_Control_Location(newx, "new", location: false);
            Hiro_Utils.Set_Control_Location(colorx, "color", location: false);
            Hiro_Utils.Set_Control_Location(timex, "time", location: false);
            Hiro_Utils.Set_Control_Location(colorx, "color", location: false);
            Thickness th2 = extended.Margin;
            th2.Left = Width / 2 - Height / 2;
            th2.Top = 0;
            extended.Margin = th2;
            extended.Width = Height;
            extended.Height = Height;
        }

        public void Load_Colors()
        {
            Hiro_Utils.IntializeColorParameters();
            Hiro_Utils.Set_Bgimage(bgimage);
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Background", "1").Equals("1"))
            {
                Blurbgi(0);
            }
            else
            {
                Blurbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
            }
            Foreground = new SolidColorBrush(App.AppForeColor);
            #region 颜色
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 80));
            Resources["AppForeDimColor"] =Hiro_Utils.Color_Transparent(App.AppForeColor, 80);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
            #endregion
            minbtn.Background = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 0));
            if (App.wnd != null && App.wnd.cm != null)
            {
                App.wnd.cm.Foreground = new SolidColorBrush(App.AppForeColor);
                App.wnd.cm.Background = new SolidColorBrush(App.AppAccentColor);
            }
            Load_Labels(false);
            if (hiro_home != null)
                hiro_home.Load_Color();
            if (hiro_items != null)
                hiro_items.Load_Color();
            if (hiro_schedule != null)
                hiro_schedule.Load_Color();
            if (hiro_config != null)
                hiro_config.Load_Color();
            if (hiro_help != null)
                hiro_help.Load_Color();
            if (hiro_about != null)
                hiro_about.Load_Color();
            if (hiro_newitem != null)
                hiro_newitem.Load_Color();
            if (hiro_newschedule != null)
                hiro_newschedule.Load_Color();
            if (hiro_time != null)
                hiro_time.Load_Color();
            if (hiro_color != null)
                hiro_color.Load_Color();
        }

        public void Load_Data()
        {
            App.cmditems.Clear();
            var i = 1;
            var p = 1;
            var inipath = App.dconfig;
            var ti = Hiro_Utils.Read_Ini(inipath, i.ToString(), "Title", "");
            var co = Hiro_Utils.Read_Ini(inipath, i.ToString(), "Command", "");
            bool reged = false;
            if (App.vs.Count > 0)
                reged = true;
            while (!ti.Trim().Equals("") && co.StartsWith("(") && co.EndsWith(")"))
            {
                var key = Hiro_Utils.Read_Ini(App.dconfig, i.ToString(), "HotKey", "").Trim();
                try
                {
                    if (!reged && key.IndexOf(",") != -1)
                    {
                        var mo = uint.Parse(key[..key.IndexOf(",")]);
                        var vkey = uint.Parse(key.Substring(key.IndexOf(",") + 1, key.Length - key.IndexOf(",") - 1));
                        try
                        {
                            if (mo != 0 && vkey != 0)
                            {
                                Hiro_Utils.RegisterKey(mo, (Key)vkey, i - 1);
                            }
                        }
                        catch (Exception ex)
                        {
                            Hiro_Utils.LogtoFile("[ERROR]Error occurred while trying to register hotkey " + mo + "+" + vkey + ":" + ex.Message);
                        }

                    }

                }
                catch (Exception ex)
                {
                    Hiro_Utils.LogtoFile("[ERROR]" + ex.Message);
                }
                co = co[1..^1];
                App.cmditems.Add(new Cmditem(p, i, ti, co, key));
                i++;
                p = (i % 10 == 0) ? i / 10 : i / 10 + 1;
                ti = Hiro_Utils.Read_Ini(inipath, i.ToString(), "Title", "");
                co = Hiro_Utils.Read_Ini(inipath, i.ToString(), "Command", "");
            }

            App.scheduleitems.Clear();
            i = 1;
            inipath = App.sconfig;
            ti = Hiro_Utils.Read_Ini(inipath, i.ToString(), "Time", "");
            co = Hiro_Utils.Read_Ini(inipath, i.ToString(), "Command", "");
            var na = Hiro_Utils.Read_Ini(inipath, i.ToString(), "Name", "");
            var re = Hiro_Utils.Read_Ini(inipath, i.ToString(), "Repeat", "-2.0");
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
                    Hiro_Utils.Write_Ini(inipath, i.ToString(), "Time", ti);
                }
                else if (double.Parse(re) == 0.0)
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
                    Hiro_Utils.Write_Ini(inipath, i.ToString(), "Time", ti);
                }
                else if (double.Parse(re) == -2.0)
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
                    Hiro_Utils.Write_Ini(inipath, i.ToString(), "Time", ti);
                }
                App.scheduleitems.Add(new Scheduleitem(i, na, ti, co, double.Parse(re)));
                i++;
                ti = Hiro_Utils.Read_Ini(inipath, i.ToString(), "Time", "");
                co = Hiro_Utils.Read_Ini(inipath, i.ToString(), "Command", "");
                na = Hiro_Utils.Read_Ini(inipath, i.ToString(), "Name", "");
                re = Hiro_Utils.Read_Ini(inipath, i.ToString(), "Repeat", "-2.0");
                if (co.Length >= 2)
                    co = co[1..^1];
            }
            App.Load_Menu();
        }

        public void Load_Translate()
        {
            Title = App.AppTitle + " - " + Hiro_Utils.Get_Transalte("version").Replace("%c", res.ApplicationVersion);
            titlelabel.Content = App.AppTitle;
            minbtn.ToolTip = Hiro_Utils.Get_Transalte("Min");
            closebtn.ToolTip = Hiro_Utils.Get_Transalte("close");
            homex.Content = Hiro_Utils.Get_Transalte("home");
            itemx.Content = Hiro_Utils.Get_Transalte("item");
            schedulex.Content = Hiro_Utils.Get_Transalte("schedule");
            configx.Content = Hiro_Utils.Get_Transalte("Config");
            helpx.Content = Hiro_Utils.Get_Transalte("help");
            aboutx.Content = Hiro_Utils.Get_Transalte("about");
            newx.Content = Hiro_Utils.Get_Transalte("new");
            colorx.Content = Hiro_Utils.Get_Transalte("color");
            timex.Content = Hiro_Utils.Get_Transalte("Time");
            if (hiro_home != null)
                hiro_home.Update_Labels();
            if (hiro_items != null)
            {
                hiro_items.Load_Translate();
                hiro_items.Load_Position();
            }
            if (hiro_schedule != null)
            {
                hiro_schedule.Load_Translate();
                hiro_schedule.Load_Position();
            }
            if (hiro_config != null)
            {
                hiro_config.Load_Translate();
                hiro_config.Load_Position();
            }
            if (hiro_help != null)
            {
                hiro_help.Load_Translate();
                hiro_help.Load_Position();
            }
            if (hiro_about != null)
            {
                hiro_about.Load_Translate();
                hiro_about.Load_Position();
            }
            if (hiro_newitem != null)
            {
                hiro_newitem.Load_Translate();
                hiro_newitem.Load_Position();
            }
            if (hiro_newschedule != null)
            {
                hiro_newschedule.Load_Translate();
                hiro_newschedule.Load_Position();
            }
            if (hiro_time != null)
            {
                hiro_time.Load_Translate();
                hiro_time.Load_Position();
            }
            if (hiro_color != null)
            {
                hiro_color.Load_Translate();
                hiro_color.Load_Position();
            }
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
                colorx.Background = new SolidColorBrush(Colors.Transparent);
                timex.Background = new SolidColorBrush(Colors.Transparent);
                homex.IsEnabled = true;
                itemx.IsEnabled = true;
                schedulex.IsEnabled = true;
                configx.IsEnabled = true;
                helpx.IsEnabled = true;
                aboutx.IsEnabled = true;
                newx.IsEnabled = true;
                colorx.IsEnabled = true;
                timex.IsEnabled = true;

            }
            homex.Foreground = Foreground;
            itemx.Foreground = Foreground;
            schedulex.Foreground = Foreground;
            configx.Foreground = Foreground;
            helpx.Foreground = Foreground;
            aboutx.Foreground = Foreground;
            newx.Foreground = Foreground;
            colorx.Foreground = Foreground;
            timex.Foreground = Foreground;
        }
        #endregion

        private void Titlelabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }

        private void Bglabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }

        private void Closebtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (extended.Visibility == Visibility.Visible)
            {
                extended.IsEnabled = false;
                extend_background.IsEnabled = false;
                Storyboard? sb = new();
                sb = Hiro_Utils.AddDoubleAnimaton(0, App.blursec, extended, "Opacity", sb);
                sb = Hiro_Utils.AddDoubleAnimaton(0, App.blursec, extend_background, "Opacity", sb);
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
                if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Min", "1").Equals("1"))
                {
                    Hiro_Utils.RunExe("hide()");
                }
                else
                {
                    Hiro_Utils.RunExe("exit()");
                }
            }
        }

        private void Homex_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Set_Label(homex);
        }

        public void Set_Home_Labels(string val)
        {
            val = (App.CustomUsernameFlag == 0) ? Hiro_Utils.Get_Transalte(val).Replace("%u", App.EnvironmentUsername) : Hiro_Utils.Get_Transalte(val + "cus").Replace("%u", App.Username);
            if (current is Hiro_Home hh)
            {
                if (!hh.Hello.Text.Equals(val))
                    hh.Hello.Text = val;
                val = Hiro_Utils.Path_Prepare(Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Get_Transalte("copyright")));
                if (!hh.Copyright.Text.Equals(val))
                    hh.Copyright.Text = val;
            }
        }

        public void Set_Label(Label label)
        {
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            Load_Labels();
            label.IsEnabled = false;
            if (label != newx && label != timex)
            {
                newx.Visibility = Visibility.Hidden;
            }
            if (label != timex)
            {
                timex.Visibility = Visibility.Hidden;
            }
            if (label != colorx)
            {
                colorx.Visibility = Visibility.Hidden;
            }
            double duration = Math.Abs(label.Margin.Top - bgx.Margin.Top);
            if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
            {
                duration = duration > label.Height * 2 ? 2 * duration : 6 * duration;
                Storyboard? sb = new();
                sb = Hiro_Utils.AddThicknessAnimaton(label.Margin, duration, bgx, "Margin", sb);
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
            if (selected == label)
            {
                if (animation && current != null)
                {
                    Storyboard sb = new();
                    Hiro_Utils.AddPowerAnimation(0, current, sb, 50, null);
                    sb.Begin();
                }
                label.IsEnabled = true;
                return;
            }
            if (label == homex)
            {
                if (hiro_home == null)
                    hiro_home = new();
                current = hiro_home;
                frame.Content = current;
            }
            if (label == itemx || label == schedulex || label == configx)
            {
                if (App.Locked)
                {
                    System.ComponentModel.BackgroundWorker sc = new();
                    System.ComponentModel.BackgroundWorker fa = new();
                    sc.RunWorkerCompleted += delegate
                    {
                        App.Locked = false;
                        if (App.mn != null)
                        {
                            App.mn.versionlabel.Content = res.ApplicationVersion;
                            App.mn.Set_Label(label);
                        }
                    };
                    fa.RunWorkerCompleted += delegate
                    {
                        if (App.mn != null)
                        {
                            if (App.Locked)
                                App.mn.versionlabel.Content = res.ApplicationVersion + " 🔒";
                            else
                                App.mn.versionlabel.Content = res.ApplicationVersion;
                            if (selected != null)
                                App.mn.Set_Label(selected);
                        }
                    };
                    Hiro_Utils.Register(sc, fa, fa);
                    return;
                }
                if (label == itemx)
                {
                    if (hiro_items == null)
                        hiro_items = new(this);
                    current = hiro_items;
                    frame.Content = current;
                }
                if (label == schedulex)
                {
                    if (hiro_schedule == null)
                        hiro_schedule = new(this);
                    current = hiro_schedule;
                    frame.Content = current;
                }
                if (label == configx)
                {
                    if (hiro_config == null)
                        hiro_config = new(this);
                    current = hiro_config;
                    frame.Content = current;
                }
            }
            if (label == helpx)
            {
                if (hiro_help == null)
                    hiro_help = new();
                current = hiro_help;
                frame.Content = current;
            }
            if (label == aboutx)
            {
                if (hiro_about == null)
                    hiro_about = new(this);
                current = hiro_about;
                frame.Content = current;
            }
            if (label == newx)
            {
                newx.Visibility = Visibility.Visible;
                if (current is Hiro_Time ht)
                {
                    ht.GoBack();
                }
                else if (current != null)
                {
                    frame.Content = current;
                }
            }
            if (label == timex)
            {
                newx.Visibility = Visibility.Visible;
                timex.Visibility = Visibility.Visible;
                if (hiro_time != null)
                {
                    current = hiro_time;
                    frame.Content = current;
                }
            }
            if (label == colorx)
            {
                colorx.Visibility = Visibility.Visible;
                Hiro_Color hc = new(this);
                hc.color_picker.Color = App.AppAccentColor;
                hc.Unify_Color(true);
                current = hc;
                frame.Content = hc;
            }
            selected = label;
            label.IsEnabled = true;
        }

        private void Timex_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(timex);
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

        private void Colorx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(colorx);
        }

        private void Minbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Ui_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Min", "1").Equals("1"))
            {
                Visibility = Visibility.Hidden;
            }
            else
            {
                Hiro_Utils.RunExe("exit()");
                Hiro_Utils.LogtoFile("[INFOMATION]Main UI: Closing " + e.GetType().ToString());
            }
            e.Cancel = true;
        }

        private void Titlelabel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Thickness thickness = versionlabel.Margin;
            thickness.Left = titlelabel.Margin.Left + titlelabel.ActualWidth + 5;
            versionlabel.Margin = thickness;
        }

        private void Schedulex_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(schedulex);
        }

        internal void Blurbgi(int direction)
        {
            if (bflag == 1)
                return;
            bflag = 1;
            if (current is Hiro_Config hc)
            {
                hc.blureff.IsEnabled = false;
                hc.rbtn14.IsEnabled = false;
                hc.rbtn15.IsEnabled = false;
                hc.btn10.IsEnabled = false;
            }   
            foreach (Window win in Application.Current.Windows)
            {
                if (win is Hiro_Alarm a)
                    a.Loadbgi(direction);
                if (win is Hiro_Msg e)
                    e.Loadbgi(direction);
                if (win is Hiro_Sequence c)
                    c.Loadbgi(direction);
                if (win is Hiro_Download d)
                    d.Loadbgi(direction);
                if (win is Hiro_Web f)
                    f.Loadbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")), false);
                if (win is Hiro_Finder g)
                    g.Loadbgi(direction);
                System.Windows.Forms.Application.DoEvents();
            }
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            System.ComponentModel.BackgroundWorker bw = new();
            bw.RunWorkerCompleted += delegate
            {
                if (current is Hiro_Config hc)
                {
                    hc.blureff.IsEnabled = true;
                    hc.rbtn14.IsEnabled = true;
                    hc.rbtn15.IsEnabled = true;
                    hc.btn10.IsEnabled = true;
                    if (hc.rbtn15.IsChecked == true)
                        hc.blureff.IsEnabled = true;
                }
            };
            Hiro_Utils.Blur_Animation(direction, animation, bgimage, this, bw);
            bflag = 0;
        }

        private void Schedulex_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (App.dflag)
            {
                DateTime dt = DateTime.Now.AddSeconds(5);
                for (int i = 0; i < 1; i++)
                {
                    dt = dt.AddSeconds(2);
                    App.scheduleitems.Add(new Scheduleitem(App.scheduleitems.Count + 1, "Test" + i.ToString(), dt.ToString("yyyy/MM/dd HH:mm:ss"), "alarm", -1.0));

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
            Hiro_Utils.Blur_Out(extended, bw);
        }
        private void Extend_background_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            extended.IsEnabled = false;
            extend_background.IsEnabled = false;
            Storyboard? sb = new();
            sb = Hiro_Utils.AddDoubleAnimaton(0, App.blursec, extended, "Opacity", sb);
            sb = Hiro_Utils.AddDoubleAnimaton(0, App.blursec, extend_background, "Opacity", sb);
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

        private void Extended_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Extend_Animation();
            touch++;
            Hiro_Utils.RunExe("notify(https://ftp.rexio.cn/hiro/hiro.php?r=touch&t=" + touch.ToString() + "&lang=" + App.lang + ",2)");
        }

        internal void Hiro_We_Extend()
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
            sb = Hiro_Utils.AddDoubleAnimaton(1, App.blursec, extended, "Opacity", sb, 0);
            sb = Hiro_Utils.AddDoubleAnimaton(1, App.blursec, extend_background, "Opacity", sb, 0);
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

        private void Versionlabel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (App.dflag)
            {
                Load_Translate();
                Load_Position();
            }
            else
            {
                if (App.Locked)
                    Hiro_Utils.RunExe("auth()");
                else
                    Hiro_Utils.RunExe("lock()");
            }
        }

        private void Ui_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            if (WindowState != WindowState.Minimized)
                Blurbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
        }

        private void Frame_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (frame.CanGoBack)
            {
                var rb = frame.RemoveBackEntry();
                while (rb != null)
                {
                    rb = frame.RemoveBackEntry();
                }
            }
        }
    }
}
