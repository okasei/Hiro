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
        internal Page? current = null;
        private Label? selected = null;
        private int bflag = 0;        
        internal int touch = 0;
        int MaxDay = 28;
        public Mainui()
        {
            InitializeComponent();
            utils.LogtoFile("[HIROWEGO]Main UI: Initializing");
            SourceInitialized += OnSourceInitialized;
            MainUI_Initialize();
            MainUI_FirstInitialize();
        }

        public void MainUI_Initialize()
        {
            InitializeUIWindow();
            utils.LogtoFile("[HIROWEGO]Main UI: Load Transaltation");
            Load_Translate();
            utils.LogtoFile("[HIROWEGO]Main UI: Load Border");
            Load_Border();
            utils.LogtoFile("[HIROWEGO]Main UI: Load Data");
            Load_Data();
            utils.LogtoFile("[HIROWEGO]Main UI: Load Colors");
            Load_Colors();
            utils.LogtoFile("[HIROWEGO]Main UI: Load Position");
            Load_Position();
        }

        public void MainUI_FirstInitialize()
        {
            utils.LogtoFile("[HIROWEGO]Main UI: Set Home");
            Set_Label(homex);
            Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - this.Width / 2);
            Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - this.Height / 2);
            if (utils.Read_Ini(App.dconfig, "config", "ani", "1").Equals("1"))
                Loaded += delegate
                {
                    Blurbgi(utils.ConvertInt(utils.Read_Ini(App.dconfig, "config", "blur", "0")));
                };
            utils.LogtoFile("[HIROWEGO]Main UI: Intitalized");
            utils.LogtoFile("[HIROWEGO]Main UI: Loaded");
        }

        public void InitializeUIWindow()
        {
            if (App.Locked)
                versionlabel.Content = res.ApplicationVersion + " 🔒";
            else
                versionlabel.Content = res.ApplicationVersion;
            utils.SetShadow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
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
            utils.Set_Control_Location(titlelabel, "title");
            utils.Set_Control_Location(versionlabel, "version");
            utils.Set_Control_Location(homex, "home", location: false);
            utils.Set_Control_Location(itemx, "item", location: false);
            utils.Set_Control_Location(schedulex, "schedule", location: false);
            utils.Set_Control_Location(configx, "config", location: false);
            utils.Set_Control_Location(helpx, "help", location: false);
            utils.Set_Control_Location(aboutx, "about", location: false);
            utils.Set_Control_Location(newx, "new", location: false);
            utils.Set_Control_Location(colorx, "color", location: false);
            utils.Set_Control_Location(timex, "time", location: false);
            utils.Set_Control_Location(colorx, "color", location: false);
            Thickness th2 = extended.Margin;
            th2.Left = Width / 2 - Height / 2;
            th2.Top = 0;
            extended.Margin = th2;
            extended.Width = Height;
            extended.Height = Height;
        }

        public void Load_Border()
        {

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
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDim"] = new SolidColorBrush(utils.Color_Transparent(App.AppForeColor, 80));
            Resources["AppForeDimColor"] =utils.Color_Transparent(App.AppForeColor, 80);
            Resources["AppAccent"] = new SolidColorBrush(utils.Color_Transparent(App.AppAccentColor, App.trval));

            #endregion
            coloruse1.Background = new SolidColorBrush(utils.Color_Transparent(App.AppForeColor, 80));
            minbtn.Background = new SolidColorBrush(utils.Color_Transparent(App.AppForeColor, 0));
            if (App.wnd != null && App.wnd.cm != null)
            {
                App.wnd.cm.Foreground = new SolidColorBrush(App.AppForeColor);
                App.wnd.cm.Background = new SolidColorBrush(App.AppAccentColor);
            }
            Load_Labels(false);
        }

        public void Load_Data()
        {
            App.cmditems.Clear();
            var i = 1;
            var p = 1;
            var inipath = App.dconfig;
            var ti = utils.Read_Ini(inipath, i.ToString(), "title", "");
            var co = utils.Read_Ini(inipath, i.ToString(), "command", "");
            bool reged = false;
            if (App.vs.Count > 0)
                reged = true;
            while (!ti.Trim().Equals("") && co.StartsWith("(") && co.EndsWith(")"))
            {
                var key = utils.Read_Ini(App.dconfig, i.ToString(), "hotkey", "").Trim();
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
                                App.vs.Add(utils.RegisterKey(App.WND_Handle, mo, (Key)vkey));
                                App.vs.Add(i - 1);
                            }
                        }
                        catch (Exception ex)
                        {
                            utils.LogtoFile("[ERROR]Error occurred while trying to register hotkey " + mo + "+" + vkey + ":" + ex.Message);
                        }

                    }

                }
                catch (Exception ex)
                {
                    utils.LogtoFile("[ERROR]" + ex.Message);
                }
                co = co[1..^1];
                App.cmditems.Add(new Cmditem(p, i, ti, co, key));
                i++;
                p = (i % 10 == 0) ? i / 10 : i / 10 + 1;
                ti = utils.Read_Ini(inipath, i.ToString(), "title", "");
                co = utils.Read_Ini(inipath, i.ToString(), "command", "");
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
                    utils.Write_Ini(inipath, i.ToString(), "time", ti);
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
                    utils.Write_Ini(inipath, i.ToString(), "time", ti);
                }
                App.scheduleitems.Add(new Scheduleitem(i, na, ti, co, double.Parse(re)));
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

        public void Load_Translate()
        {
            Title = App.AppTitle + " - " + utils.Get_Transalte("version").Replace("%c", res.ApplicationVersion);
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
            colorx.Content = utils.Get_Transalte("color");
            timex.Content = utils.Get_Transalte("time");
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
            if (extended.Visibility == Visibility.Visible)
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

        public void Set_Home_Labels(string val)
        {
            val = (App.CustomUsernameFlag == 0) ? utils.Get_Transalte(val).Replace("%u", App.EnvironmentUsername) : utils.Get_Transalte(val + "cus").Replace("%u", App.Username);
            if (current is Hiro_Home hh)
            {
                if (!hh.Hello.Text.Equals(val))
                    hh.Hello.Text = val;
                val = utils.Path_Prepare(utils.Path_Prepare_EX(utils.Get_Transalte("copyright")));
                if (!hh.Copyright.Text.Equals(val))
                    hh.Copyright.Text = val;
            }
        }

        public void Set_Label(Label label)
        {
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
                current = new Hiro_Home();
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
                            App.mn.Set_Label(homex);
                        }
                    };
                    utils.Register(sc, fa, fa);
                    return;
                }
                if (label == itemx)
                {
                    current = new Hiro_Items(this);
                    frame.Content = current;
                }
                if (label == schedulex)
                {
                    current = new Hiro_Schedule(this);
                    frame.Content = current;
                }
                if (label == configx)
                {
                    current = new Hiro_Config(this);
                    frame.Content = current;
                }
            }
            if (label == helpx)
            {
                current = new Hiro_Help();
                frame.Content = current;
            }
            if (label == aboutx)
            {
                current = new Hiro_About(this);
                frame.Content = current;
            }
            if (label == newx)
            {
                newx.Visibility = Visibility.Visible;
            }
            if (label == timex)
            {
                newx.Visibility = Visibility.Visible;
                timex.Visibility = Visibility.Visible;
            }
            if (label == colorx)
            {
                colorx.Visibility = Visibility.Visible;
            }
            label.IsEnabled = true;
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
                if (win is Alarm a)
                    a.Loadbgi(direction);
                if (win is Message e)
                    e.Loadbgi(direction);
                if (win is Sequence c)
                    c.Loadbgi(direction);
                if (win is Download d)
                    d.Loadbgi(direction);
                if (win is Web f)
                    f.Loadbgi(utils.ConvertInt(utils.Read_Ini(App.dconfig, "config", "blur", "0")), false);
                System.Windows.Forms.Application.DoEvents();
            }
            bool animation = !utils.Read_Ini(App.dconfig, "config", "ani", "1").Equals("0");
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
            utils.Blur_Animation(direction, animation, bgimage, this, bw);
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
            utils.Blur_Out(extended, bw);
        }
        private void Extend_background_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void Extended_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Extend_Animation();
            touch++;
            utils.RunExe("notify(https://ftp.rexio.cn/hiro/hiro.php?r=touch&t=" + touch.ToString() + "&lang=" + App.lang + ",2)");
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

        private void Ui_Loaded(object sender, RoutedEventArgs e)
        {
            
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
                    utils.RunExe("auth()");
                else
                    utils.RunExe("lock()");
            }
        }

        private void Ui_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
        }

        private void Timex_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(timex);
        }
    }
}
