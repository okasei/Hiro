using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Vlc.DotNet.Wpf;
using Vlc.DotNet.Core.Interops;

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
        internal Hiro_Profile? hiro_profile = null;
        internal Hiro_About? hiro_about = null;
        internal Hiro_NewItem? hiro_newitem = null;
        internal Hiro_NewSchedule? hiro_newschedule = null;
        internal Hiro_Time? hiro_time = null;
        internal Hiro_Color? hiro_color = null;
        internal Hiro_Proxy? hiro_proxy = null;
        internal Hiro_Chat? hiro_chat = null;
        internal VlcVideoSourceProvider? hiro_provider = null;

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

        private void Reset_Vlc()
        {
            var videoPath = Hiro_Utils.Read_Ini(App.dconfig, "Config", "BackVideo", "");
            videoPath = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(videoPath));
            if (!System.IO.File.Exists(videoPath))
                return;
            System.Threading.Thread.Sleep(100);
            if (hiro_provider != null)
            {
                hiro_provider.MediaPlayer.Stop();
                hiro_provider.MediaPlayer.Position = 0;
                hiro_provider.MediaPlayer.Play(new Uri(@videoPath));
            }
        }

        private void Create_Vlc()
        {
            new System.Threading.Thread(() =>
            {
                try
                {
                    var videoPath = Hiro_Utils.Read_Ini(App.dconfig, "Config", "BackVideo", "");
                    videoPath = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(videoPath));
                    Dispatcher.Invoke(() =>
                    {
                        vlimage.Visibility = Visibility.Visible;
                    });
                    if (!System.IO.File.Exists(videoPath))
                        return;
                    Object? obj = null;
                    Dispatcher.Invoke(() =>
                    {
                        vlimage.Visibility = Visibility.Hidden;
                        obj = vlcPlayer.Tag;
                    });
                    hiro_provider ??= new(Dispatcher);
                    if (obj == null)
                    {
                        hiro_provider.CreatePlayer(new(@Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare("<current>")) + @"\runtimes\win-vlc"), new[] { "--input-repeat=65535" });
                        vlcPlayer.Dispatcher.Invoke(() => {
                            vlcPlayer.SetBinding(Image.SourceProperty,
                            new Binding(nameof(VlcVideoSourceProvider.VideoSource)) { Source = hiro_provider });
                        });
                    }
                    hiro_provider.MediaPlayer.Play(new Uri(@videoPath));
                    hiro_provider.MediaPlayer.Audio.IsMute = true;
                    hiro_provider.MediaPlayer.EndReached += delegate
                    {
                        new System.Threading.Thread(() =>
                        {
                            Reset_Vlc();
                        }).Start();
                    };
                    hiro_provider.MediaPlayer.LengthChanged += delegate
                    {
                        Dispatcher.Invoke(() =>
                        {
                            vlcPlayer.Tag = "Playing";
                            Load_VlcPlayer();
                        });
                    };
                }
                catch (Exception ex)
                {
                    Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.Player.Initialize: " + ex.Message);
                }
            }).Start();
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
            OpacityBgi();
            Hiro_Utils.LogtoFile("[HIROWEGO]Main UI: Load OpacityMask");
            Load_Position();
        }

        public void MainUI_FirstInitialize()
        {
            Hiro_Utils.LogtoFile("[HIROWEGO]Main UI: Set Home");
            Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - Width / 2);
            Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - Height / 2);
            Hiro_Utils.LogtoFile("[HIROWEGO]Main UI: Intitalized");
            new System.Threading.Thread(() =>
            {
                var strFileName = Hiro_Utils.Read_Ini(App.dconfig, "Config", "BackImage", string.Empty);
                if (System.IO.File.Exists(@strFileName))
                {
                    try
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromFile(@strFileName);
                        double w = img.Width;
                        double h = img.Height;
                        double ww = 550 * 2;
                        double hh = 450 * 2;
                        Dispatcher.Invoke(() =>
                        {
                            ww = Width * 2;
                            hh = Height * 2;
                        });
                        bool m = false;
                        if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Compression", "1").Equals("1"))
                        {
                            if (ww < w && hh < h)
                            {
                                var r = 0;
                                while (ww < w && hh < h)
                                {
                                    w /= 2;
                                    h /= 2;
                                    r++;
                                }
                                w *= 2;
                                h *= 2;
                                if (r > 1)
                                {
                                    img = Hiro_Utils.ZoomImage(img, Convert.ToInt32(h), Convert.ToInt32(w));
                                    m = true;
                                }

                            }
                            long len = 0;
                            using (var stream = new System.IO.MemoryStream())
                            {
                                img.Save(stream, Hiro_Utils.GetImageFormat(img));
                                len = stream.Length;
                            }
                            if (len > 2048 * 1024)
                            {
                                img = Hiro_Utils.ZipImage(img, Hiro_Utils.GetImageFormat(img), 2048);
                                m = true;
                            }
                        }
                        if (m)
                        {
                            System.Drawing.Bitmap b = new(img);
                            img.Dispose();
                            strFileName = @"<hiapp>\images\background\" + strFileName.Substring(strFileName.LastIndexOf("\\"));
                            strFileName = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(strFileName));
                            Hiro_Utils.CreateFolder(strFileName);
                            if (System.IO.File.Exists(strFileName))
                                System.IO.File.Delete(strFileName);
                            b.Save(strFileName);
                            b.Dispose();
                            strFileName = Hiro_Utils.Anti_Path_Prepare(strFileName);
                            Hiro_Utils.Write_Ini(App.dconfig, "Config", "BackImage", strFileName);
                        }
                        else
                            img.Dispose();
                        Dispatcher.Invoke(() =>
                        {
                            Hiro_Utils.Set_Bgimage(bgimage, this);
                        });
                    }
                    catch (Exception ex)
                    {
                        Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.Background.Image.Select Details: " + ex.Message);
                    }
                }
            }).Start();
            Hiro_Utils.LogtoFile("[HIROWEGO]Main UI: Loaded");
        }

        public void HiHiro()
        {
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Background", "1").Equals("3"))
                Update_VlcPlayer_Status();
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Background", "1").Equals("2"))
                Blurbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
            if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
            {
                Set_Label(selected ?? homex);
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
                if (infolabel.Visibility == Visibility.Visible)
                    Hiro_Utils.AddPowerAnimation(0, infolabel, sb, -50, null);
                sb.Begin();
            }
            Hiro_Utils.SetWindowToForegroundWithAttachThreadInput(this);
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Background", "1").Equals("3"))
                BlurVideo();
            Hiro_Utils.LogtoFile("[HIROWEGO]Main UI: Hi!Hiro!");
        }

        public void AddToInfoCenter(string text)
        {
            infotext.AppendText(text);
            infolabel.Visibility = Visibility.Visible;
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                Storyboard sb = new();
                Hiro_Utils.AddPowerAnimation(0, infolabel, sb, -50, null);
                sb.Begin();
            }
        }

        public void InitializeUIWindow()
        {
            if (App.Locked)
                versionlabel.Content = Hiro_Resources.ApplicationVersion + " 🔒";
            else
                versionlabel.Content = Hiro_Resources.ApplicationVersion;
            Hiro_Utils.SetShadow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
        }
        
        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;
            var source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source?.AddHook(WndProc);
            Hiro_Utils.LogtoFile("[HIROWEGO]Main UI: AddHook WndProc");
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
            Hiro_Utils.Set_Control_Location(titlelabel, "title");
            Hiro_Utils.Set_Control_Location(versionlabel, "version");
            Hiro_Utils.Set_Control_Location(infotitle, "infotitle");
            Hiro_Utils.Set_Control_Location(infotext, "infotext");
            Hiro_Utils.Set_Control_Location(homex, "home", location: false);
            Hiro_Utils.Set_Control_Location(itemx, "item", location: false);
            Hiro_Utils.Set_Control_Location(schedulex, "schedule", location: false);
            Hiro_Utils.Set_Control_Location(configx, "config", location: false);
            Hiro_Utils.Set_Control_Location(profilex, "profile", location: false);
            Hiro_Utils.Set_Control_Location(aboutx, "about", location: false);
            Hiro_Utils.Set_Control_Location(newx, "new", location: false);
            Hiro_Utils.Set_Control_Location(colorx, "color", location: false);
            Hiro_Utils.Set_Control_Location(timex, "time", location: false);
            Hiro_Utils.Set_Control_Location(proxyx, "proxy", location: false);
            Hiro_Utils.Set_Control_Location(chatx, "chat", location: false);
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
            Hiro_Utils.Set_Bgimage(bgimage, this);
            switch (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Background", "1"))
            {
                case "1":
                    Blurbgi(0);
                    break;
                case "2":
                    Blurbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
                    break;
            };
            Foreground = new SolidColorBrush(App.AppForeColor);
            #region 颜色
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 80));
            Resources["AppForeDimColor"] =Hiro_Utils.Color_Transparent(App.AppForeColor, 80);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
            Resources["InfoAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 200));
            #endregion
            minbtn.Background = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 0));
            if (App.wnd != null && App.wnd.cm != null)
            {
                App.wnd.cm.Foreground = new SolidColorBrush(App.AppForeColor);
                App.wnd.cm.Background = new SolidColorBrush(App.AppAccentColor);
            }
            Load_Labels(false);
            hiro_home?.Load_Color();
            hiro_items?.Load_Color();
            hiro_schedule?.Load_Color();
            hiro_config?.Load_Color();
            hiro_profile?.Load_Color();
            hiro_about?.Load_Color();
            hiro_newitem?.Load_Color();
            hiro_newschedule?.Load_Color();
            hiro_time?.Load_Color();
            hiro_color?.Load_Color();
            hiro_chat?.Load_Color();
        }

        public static void Load_Data()
        {
            App.cmditems.Clear();
            var i = 1;
            var p = 1;
            var inipath = App.dconfig;
            var ti = Hiro_Utils.Read_Ini(inipath, i.ToString(), "Title", "");
            var co = Hiro_Utils.Read_Ini(inipath, i.ToString(), "Command", "");
            bool reged = App.vs.Count > 0;
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
                            Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.Hotkey.Register: Error occurred while trying to register hotkey " + mo + "+" + vkey + ":" + ex.Message);
                        }

                    }

                }
                catch (Exception ex)
                {
                    Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.Hotkey.Register.Format: " + ex.Message);
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
                switch (double.Parse(re))
                {
                    case -1.0:
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
                        break;
                    }
                    case 0.0:
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
                        break;
                    }
                    case -2.0:
                        break;
                    default:
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
                        break;
                    }
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
            Title = App.AppTitle + " - " + Hiro_Utils.Get_Transalte("version").Replace("%c", Hiro_Resources.ApplicationVersion);
            titlelabel.Content = App.AppTitle;
            infotitle.Content = Hiro_Utils.Get_Transalte("infotitle");
            minbtn.ToolTip = Hiro_Utils.Get_Transalte("min");
            closebtn.ToolTip = Hiro_Utils.Get_Transalte("close");
            infolabel.ToolTip = Hiro_Utils.Get_Transalte("info");
            homex.Content = Hiro_Utils.Get_Transalte("home");
            itemx.Content = Hiro_Utils.Get_Transalte("item");
            schedulex.Content = Hiro_Utils.Get_Transalte("schedule");
            configx.Content = Hiro_Utils.Get_Transalte("config");
            profilex.Content = Hiro_Utils.Get_Transalte("profile");
            aboutx.Content = Hiro_Utils.Get_Transalte("about");
            newx.Content = Hiro_Utils.Get_Transalte("new");
            colorx.Content = Hiro_Utils.Get_Transalte("color");
            timex.Content = Hiro_Utils.Get_Transalte("time");
            proxyx.Content = Hiro_Utils.Get_Transalte("proxy");
            chatx.Content = Hiro_Utils.Get_Transalte("chat");
            hiro_home?.Update_Labels();
            hiro_items?.Load_Translate();
            hiro_items?.Load_Position();
            hiro_schedule?.Load_Translate();
            hiro_schedule?.Load_Position();
            hiro_config?.Load_Translate();
            hiro_config?.Load_Position();
            hiro_profile?.Load_Translate();
            hiro_profile?.Load_Position();
            hiro_about?.Load_Translate();
            hiro_about?.Load_Position();
            hiro_newitem?.Load_Translate();
            hiro_newitem?.Load_Position();
            hiro_newschedule?.Load_Translate();
            hiro_newschedule?.Load_Position();
            hiro_time?.Load_Translate();
            hiro_time?.Load_Position();
            hiro_color?.Load_Translate();
            hiro_color?.Load_Position();
            hiro_proxy?.Load_Translate();
            hiro_proxy?.Load_Position();
            hiro_chat?.Load_Translate();
            hiro_chat?.Load_Position();
        }

        public void Load_Labels(bool reload = true)
        {
            if (reload)
            {
                homex.Background = new SolidColorBrush(Colors.Transparent);
                itemx.Background = new SolidColorBrush(Colors.Transparent);
                schedulex.Background = new SolidColorBrush(Colors.Transparent);
                configx.Background = new SolidColorBrush(Colors.Transparent);
                profilex.Background = new SolidColorBrush(Colors.Transparent);
                aboutx.Background = new SolidColorBrush(Colors.Transparent);
                newx.Background = new SolidColorBrush(Colors.Transparent);
                colorx.Background = new SolidColorBrush(Colors.Transparent);
                timex.Background = new SolidColorBrush(Colors.Transparent);
                proxyx.Background = new SolidColorBrush(Colors.Transparent);
                chatx.Background = new SolidColorBrush(Colors.Transparent);
                homex.IsEnabled = true;
                itemx.IsEnabled = true;
                schedulex.IsEnabled = true;
                configx.IsEnabled = true;
                profilex.IsEnabled = true;
                aboutx.IsEnabled = true;
                newx.IsEnabled = true;
                colorx.IsEnabled = true;
                timex.IsEnabled = true;
                proxyx.IsEnabled = true;
                chatx.IsEnabled = true;

            }
            homex.Foreground = Foreground;
            itemx.Foreground = Foreground;
            schedulex.Foreground = Foreground;
            configx.Foreground = Foreground;
            profilex.Foreground = Foreground;
            aboutx.Foreground = Foreground;
            newx.Foreground = Foreground;
            colorx.Foreground = Foreground;
            timex.Foreground = Foreground;
            proxyx.Foreground = Foreground;
            chatx.Foreground = Foreground;
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
                if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
                {
                    Storyboard? sb = new();
                    sb = Hiro_Utils.AddDoubleAnimaton(0, App.blursec, extended, "Opacity", sb);
                    sb = Hiro_Utils.AddDoubleAnimaton(0, App.blursec, extend_background, "Opacity", sb);
                    sb.Completed += delegate
                    {
                        extended.Opacity = 0;
                        extend_background.Opacity = 0;
                        extended.Visibility = Visibility.Hidden;
                        extend_background.Visibility = Visibility.Hidden;
                        Update_VlcPlayer_Status();
                        sb = null;
                    };
                    sb.Begin();
                }
                else
                {
                    extended.Opacity = 0;
                    extend_background.Opacity = 0;
                    extended.Visibility = Visibility.Hidden;
                    extend_background.Visibility = Visibility.Hidden;
                    Update_VlcPlayer_Status();
                }
                
            }
            else if(infocenter.Visibility == Visibility.Visible)
            {
                if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
                {
                    Storyboard? sb = new();
                    sb = Hiro_Utils.AddDoubleAnimaton(0, App.blursec, infocenter, "Opacity", sb);
                    sb.Completed += delegate
                    {
                        infocenter.Opacity = 0;
                        infocenter.Visibility = Visibility.Hidden;
                        if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
                        {
                            infocenter.IsEnabled = false;
                            Storyboard? sbe = new();
                            Hiro_Utils.AddPowerAnimation(1, infolabel, sbe, null, -50);
                            sbe.Completed += delegate
                            {
                                infolabel.Visibility = Visibility.Hidden;
                            };
                            sbe.Begin();
                        }
                        else
                        {
                            infolabel.Visibility = Visibility.Hidden;
                        }
                        sb = null;
                    };
                    sb.Begin();
                }
                else
                {
                    infocenter.Opacity = 0;
                    infocenter.Visibility = Visibility.Hidden;
                    infolabel.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                Hiro_Utils.RunExe(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Min", "1").Equals("1")
                    ? "hide()"
                    : "exit()");
            }
        }

        private void Homex_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Set_Label(homex);
        }

        public void Set_Home_Labels(string val)
        {
            val = (App.CustomUsernameFlag == 0) ? Hiro_Utils.Get_Transalte(val).Replace("%u", App.EnvironmentUsername) : Hiro_Utils.Get_Transalte(val + "cus").Replace("%u", App.Username);
            if (current is not Hiro_Home hh)
                return;
            if (!hh.Hello.Text.Equals(val))
                hh.Hello.Text = val;
            val = Hiro_Utils.Path_Prepare(Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Get_Transalte("copyright")));
            if (!hh.Copyright.Text.Equals(val))
                hh.Copyright.Text = val;
        }

        public void Set_Label(Label label)
        {
            var animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
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
            if (label != proxyx)
            {
                proxyx.Visibility = Visibility.Hidden;
            }
            var duration = Math.Abs(label.Margin.Top - bgx.Margin.Top);
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
                    switch (current)
                    {
                        case Hiro_Items his:
                            his.HiHiro();
                            break;
                        case Hiro_Schedule hs:
                            hs.HiHiro();
                            break;
                        case Hiro_Config hc:
                            hc.HiHiro();
                            break;
                        case Hiro_Profile hp:
                            hp.HiHiro();
                            break;
                        case Hiro_NewItem hni:
                            hni.HiHiro();
                            break;
                        case Hiro_NewSchedule hns:
                            hns.HiHiro();
                            break;
                        case Hiro_Time ht:
                            ht.HiHiro();
                            break;
                        case Hiro_Color hcr:
                            hcr.HiHiro();
                            break;
                        case Hiro_Proxy hpy:
                            hpy.HiHiro();
                            break;
                        case Hiro_Chat hct:
                            hct.HiHiro();
                            break;
                    }

                    sb.Begin();
                }
                label.IsEnabled = true;
                return;
            }
            if (label == homex)
            {
                hiro_home ??= new();
                current = hiro_home;
                frame.Content = current;
            }
            if (label == itemx || label == schedulex || label == profilex || label == configx || label == chatx)
            {
                if (App.Locked)
                {
                    System.ComponentModel.BackgroundWorker sc = new();
                    System.ComponentModel.BackgroundWorker fa = new();
                    sc.RunWorkerCompleted += delegate
                    {
                        App.Locked = false;
                        if (App.mn != null)
                            App.mn.versionlabel.Content = Hiro_Resources.ApplicationVersion;
                        App.mn?.Set_Label(label);
                    };
                    fa.RunWorkerCompleted += delegate
                    {
                        if (App.mn == null) 
                            return;
                        App.mn.versionlabel.Content = Hiro_Resources.ApplicationVersion + (App.Locked ? " 🔒" : "");
                        App.mn.Set_Label(selected ?? homex);
                    };
                    Hiro_Utils.Register(sc, fa, fa);
                    return;
                }
                if (label == itemx)
                {
                    hiro_items ??= new(this);
                    current = hiro_items;
                    frame.Content = current;
                }
                if (label == schedulex)
                {
                    hiro_schedule ??= new(this);
                    current = hiro_schedule;
                    frame.Content = current;
                }
                if (label == profilex)
                {
                    hiro_profile ??= new(this);
                    current = hiro_profile;
                    frame.Content = current;
                }
                if (label == configx)
                {
                    hiro_config ??= new(this);
                    current = hiro_config;
                    frame.Content = current;
                }
                if (label == chatx)
                {
                    hiro_chat ??= new(this);
                    current = hiro_chat;
                    frame.Content = hiro_chat;
                }
            }
            if (label == aboutx)
            {
                hiro_about ??= new(this);
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
                hiro_color ??= new(this);
                hiro_color.color_picker.Color = App.AppAccentColor;
                hiro_color.Unify_Color(true);
                current = hiro_color;
                frame.Content = hiro_color;
            }
            if (label == proxyx)
            {
                proxyx.Visibility = Visibility.Visible;
                hiro_proxy ??= new(this);
                current = hiro_proxy;
                frame.Content = hiro_proxy;
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

        private void Profilex_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(profilex);
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
                Update_VlcPlayer_Status();
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
            var thickness = versionlabel.Margin;
            thickness.Left = titlelabel.Margin.Left + titlelabel.ActualWidth + 2;
            versionlabel.Margin = thickness;
            thickness = infolabel.Margin;
            thickness.Left = versionlabel.Margin.Left + versionlabel.ActualWidth + 2;
            infolabel.Margin = thickness;
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
            foreach (Window win in Application.Current.Windows)
            {
                switch (win)
                {
                    case Hiro_Alarm a:
                        a.Loadbgi(direction);
                        break;
                    case Hiro_Msg e:
                        e.Loadbgi(direction);
                        break;
                    case Hiro_Sequence c:
                        c.Loadbgi(direction);
                        break;
                    case Hiro_Download d:
                        d.Loadbgi(direction);
                        break;
                    case Hiro_Web f:
                        f.Loadbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")), false);
                        break;
                    case Hiro_Finder g:
                        g.Loadbgi(direction);
                        break;
                    case Hiro_Player h:
                        h.Loadbgi(direction);
                        break;
                }

                System.Windows.Forms.Application.DoEvents();
            }
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            System.ComponentModel.BackgroundWorker bw = new();
            bw.RunWorkerCompleted += delegate
            {
                if (current is not Hiro_Config hc) 
                    return;
                hc.blureff.IsEnabled = false;
                hc.rbtn14.IsEnabled = true;
                hc.rbtn15.IsEnabled = true;
                hc.btn10.IsEnabled = true;
                hc.video_btn.IsEnabled = true;
                if (hc.rbtn15.IsChecked == true)
                    hc.blureff.IsEnabled = true;
            };
            vlcGrid.Visibility = Visibility.Hidden;
            bgimage.Visibility = Visibility.Visible;
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Background", "1").Equals("2"))
                Hiro_Utils.Blur_Animation(direction, animation, bgimage, this, bw);
            else
                bw.RunWorkerAsync();
            if (vlcPlayer.Tag != null && ((String)vlcPlayer.Tag).Equals("Playing"))
            {
                new System.Threading.Thread(() =>
                {
                    hiro_provider?.Dispose();
                    hiro_provider = null;
                }).Start();
                vlcPlayer.Tag = null;
            }
            bflag = 0;
        }

        internal void BlurVideo()
        {
            vlcGrid.Visibility = Visibility.Visible;
            bgimage.Visibility = Visibility.Hidden;
            bgimage.Background = new SolidColorBrush(App.AppAccentColor);
            Create_Vlc();
            Load_VlcPlayer();
        }

        internal void OpacityBgi()
        {
            Hiro_Utils.Set_Opacity(bgimage, this);
            Hiro_Utils.Set_Opacity(vlcPlayer, this);
            foreach (Window win in Application.Current.Windows)
            {
                switch (win)
                {
                    case Hiro_Alarm a:
                        Hiro_Utils.Set_Opacity(a.bgimage, a);
                        break;
                    case Hiro_Msg e:
                        Hiro_Utils.Set_Opacity(e.bgimage, e);
                        break;
                    case Hiro_Sequence c:
                        Hiro_Utils.Set_Opacity(c.bgimage, c);
                        break;
                    case Hiro_Download d:
                        Hiro_Utils.Set_Opacity(d.bgimage, d);
                        break;
                    case Hiro_Web f:
                        Hiro_Utils.Set_Opacity(f.bgimage, f);
                        break;
                    case Hiro_Finder g:
                        Hiro_Utils.Set_Opacity(g.bgimage, g);
                        break;
                    case Hiro_Player h:
                        Hiro_Utils.Set_Opacity(h.bgimage, h);
                        break;
                }
                if (hiro_profile != null)
                    Hiro_Utils.Set_Opacity(hiro_profile.Profile_Background, hiro_profile.BackControl);
                if (hiro_chat != null)
                    Hiro_Utils.Set_Opacity(hiro_chat.Profile_Background, hiro_chat.BackControl);
                System.Windows.Forms.Application.DoEvents();
            }
        }

        private void Schedulex_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!App.dflag) 
                return;
            DateTime dt = DateTime.Now.AddSeconds(5);
            for (int i = 0; i < 1; i++)
            {
                dt = dt.AddSeconds(2);
                App.scheduleitems.Add(new Scheduleitem(App.scheduleitems.Count + 1, "Test" + i.ToString(), dt.ToString("yyyy/MM/dd HH:mm:ss"), "alarm", -1.0));

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
            var th = extend_background.Margin;
            th.Left = 0;
            th.Top = 0;
            extend_background.Margin = th;
            extend_background.Width = Width;
            extend_background.Height = Height;
            extend_background.Background = new SolidColorBrush(Colors.Coral);
            extended.Visibility = Visibility.Visible;
            extend_background.Visibility = Visibility.Visible;
            if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
            {
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
            else
            {
                extended.Opacity = 1;
                extend_background.Opacity = 1;
                extended.IsEnabled = true;
                extend_background.IsEnabled = true;
            }
            Update_VlcPlayer_Status();
        }
        internal void Hiro_We_Info()
        {
            var th = infoimage.Margin;
            th.Left = 0;
            th.Top = 0;
            infocenter.Margin = th;
            infocenter.Width = Width;
            infocenter.Height = Height;
            infocenter.Visibility = Visibility.Visible;
            if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
            {
                Storyboard? sb = new();
                sb = Hiro_Utils.AddDoubleAnimaton(1, App.blursec, infocenter, "Opacity", sb, 0);
                sb.Completed += delegate
                {
                    infocenter.Opacity = 1;
                    infocenter.IsEnabled = true;
                    sb = null;
                };
                sb.Begin();
            }
            else
            {
                infocenter.Opacity = 1;
                infocenter.IsEnabled = true;
            }  
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
                Hiro_Utils.RunExe(App.Locked ? "auth()" : "lock()");
            }
        }

        private void Load_VlcPlayer()
        {
            if (WindowState == WindowState.Normal && Visibility == Visibility.Visible)
            {
                vlcGrid.Width = Width;
                vlcGrid.Height = Height;
                try
                {
                    var track = hiro_provider?.MediaPlayer.GetMedia().Tracks[0].TrackInfo as VideoTrack;
                    if (track != null)
                    {
                        var w = track.Width;
                        var h = track.Height;
                        var ww = Width;
                        var hh = Height;
                        var wi = ww / w;
                        var hi = hh / h;
                        var www = hh * w / h;
                        var hhh = ww * h / w;
                        if (wi < hi)
                        {
                            vlcGrid.Width = www;
                            vlcGrid.Height = hh;
                        }
                        else
                        {
                            vlcGrid.Width = ww;
                            vlcGrid.Height = hhh;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.Player.Resize: " + ex.Message);
                }
            }
        }

        private void Ui_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            if (WindowState != WindowState.Minimized)
            {
                if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Background", "1").Equals("2"))
                    Blurbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
            }
                
            Update_VlcPlayer_Status();
        }

        public void Update_VlcPlayer_Status()
        {
            if (WindowState == WindowState.Minimized || Visibility != Visibility.Visible || extended.Visibility == Visibility.Visible || vlcGrid.Visibility != Visibility.Visible)
            {
                if (vlcPlayer.Tag != null && ((String)vlcPlayer.Tag).Equals("Playing"))
                {
                    hiro_provider?.MediaPlayer.Pause();
                    vlcPlayer.Tag = "Paused";
                }
            }
            else
            {
                if (vlcPlayer.Tag != null && ((String)vlcPlayer.Tag).Equals("Paused"))
                {
                    hiro_provider?.MediaPlayer.Play();
                    vlcPlayer.Tag = "Playing";
                }
            }
        }

        private void Frame_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!frame.CanGoBack) 
                return;
            var rb = frame.RemoveBackEntry();
            while (rb != null)
            {
                rb = frame.RemoveBackEntry();
            }
        }

        private void Proxyx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(proxyx);
        }

        private void Chatx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(chatx);
        }

        private void Infotitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }

        private void Infolabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_We_Info();
        }

        private void Versionlabel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var thickness = infolabel.Margin;
            thickness.Left = versionlabel.Margin.Left + versionlabel.ActualWidth + 2;
            infolabel.Margin = thickness;
        }
    }
}
