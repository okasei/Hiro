using System;
using System.Windows;
using System.Windows.Media;

namespace hiro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal System.Windows.Controls.ContextMenu? cm = null;
        public MainWindow()
        {
            InitializeComponent();
            System.Windows.Controls.Canvas.SetTop(this, -233);
            System.Windows.Controls.Canvas.SetLeft(this, -233);
            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged  += new System.Net.NetworkInformation.NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
        }
        void NetworkChange_NetworkAvailabilityChanged(object? sender, System.Net.NetworkInformation.NetworkAvailabilityEventArgs e)
        {
            if (App.dflag)
            {
                System.Net.NetworkInformation.NetworkInterface[] nfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                foreach (System.Net.NetworkInformation.NetworkInterface ni in nfaces)
                {
                    utils.LogtoFile(ni.Description + " - " + ni.NetworkInterfaceType.ToString());
                }
            }
            if (utils.Read_Ini(App.dconfig, "Configuration", "verbose", "1").Equals("1"))
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    if (e.IsAvailable)
                        App.Notify(new noticeitem(utils.Get_Transalte("neton"), 2));
                    else
                        App.Notify(new noticeitem(utils.Get_Transalte("netoff"), 2));
                }));
            }
        }
        public void InitializeInnerParameters()
        {
            ti.ToolTipText = App.AppTitle;
            Title = App.AppTitle;
            InitializeMethod();
            SourceInitialized += OnSourceInitialized;
        }
        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;
            System.Windows.Interop.HwndSource source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source.AddHook(WndProc);
            utils.LogtoFile("[HIROWEGO]Main Window: AddHook WndProc");
        }
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0320://系统颜色改变
                    if (utils.Read_Ini(App.dconfig, "Configuration", "lock", "0").Equals("1"))
                        break;
                    App.ColorCD = 3;
                    break;
                case 0x0083://prevent system from drawing outline
                    handled = true;
                    break;
                /*case 0x001A:
                    if (Program.isWin11)
                        Set_System_Color();
                    break;*/
                case 0x0218:
                    if (utils.Read_Ini(App.dconfig, "Configuration", "verbose", "1").Equals("1"))
                    {
                        utils.GetSystemPowerStatus(out utils.SYSTEM_POWER_STATUS p);
                        if (p.ACLineStatus == 1 && p.BatteryFlag == 8)
                            App.Notify(new noticeitem(utils.Get_Transalte("bacharge"), 2));
                        if (p.ACLineStatus == 1 && p.BatteryFlag != 8)
                            App.Notify(new noticeitem(utils.Get_Transalte("baconnect"), 2));
                        if (p.ACLineStatus == 0)
                            App.Notify(new noticeitem(utils.Get_Transalte("baremove"), 2));
                    }
                    break;
                case 0x0219:
                    if (utils.Read_Ini(App.dconfig, "Configuration", "verbose", "1").Equals("1"))
                    {
                        var mms = utils.Get_Transalte("deinfo") + " - ";
                        switch (wParam.ToInt32())
                        {
                            case 0x0018:
                                mms += utils.Get_Transalte("deconfig");
                                break;
                            case 0x8000:
                                mms += utils.Get_Transalte("medconnect");
                                break;
                            case 0x8004:
                                mms += utils.Get_Transalte("medremove");
                                break;
                            default:
                                return IntPtr.Zero;
                        }
                        App.Notify(new noticeitem(mms, 2));
                    }
                    break;
                default:
                    /*if (App.dflag)
                        utils.LogtoFile("[DEBUG]Msg: " + msg.ToString() + ";LParam: " + lParam.ToString() + ";WParam: " + wParam.ToString());*/
                    break;
            }
            return IntPtr.Zero;

        }

        public void Load_All_Colors()
        {
            utils.IntializeColorParameters();
            if (App.ed != null)
            {
                App.ed.Load_Color();
            }
            if (App.noti != null)
            {
                App.noti.Load_Color();
            }
            if (App.mn != null)
            {
                App.mn.Load_Colors();
            }
            foreach (Window win in Application.Current.Windows)
            {
                if (win.GetType() == typeof(Alarm))
                {
                    Alarm? a = win as Alarm;
                    if (a != null)
                    {
                        a.Load_Colors();
                    }
                }
                if (win.GetType() == typeof(Download))
                {
                    Download? a = win as Download;
                    if (a != null)
                    {
                        a.Load_Colors();
                    }
                }
                if (win.GetType() == typeof(Lockscr))
                {
                    Lockscr? a = win as Lockscr;
                    if (a != null)
                    {
                        a.Load_Colors();
                    }
                }
                if (win.GetType() == typeof(message))
                {
                    message? a = win as message;
                    if (a != null)
                    {
                        a.Load_Colors();
                    }
                }
                if (win.GetType() == typeof(Sequence))
                {
                    Sequence? a = win as Sequence;
                    if (a != null)
                    {
                        a.Load_Colors();
                    }
                }
                System.Windows.Forms.Application.DoEvents();
            }
        }

        private void InitializeMethod()
        {
            ti.TrayMiddleMouseDown += delegate
            {
                var mc = utils.Read_Ini(App.dconfig, "Configuration", "middleclick", "1");
                switch (mc)
                {
                    case "2":
                        utils.RunExe("menu()");;
                        break;
                    case "3":
                        var mce = utils.Read_Ini(App.dconfig, "Configuration", "middleaction", "");
                        utils.RunExe(mce);
                        break;
                    default:
                        if (App.mn != null)
                        {
                            if (App.mn.Visibility == Visibility.Visible)
                                utils.RunExe("hide()");
                            else
                                utils.RunExe("show()");
                        }
                        else
                        {
                            utils.RunExe("show()");
                        }
                        break;
                }
            };
            ti.TrayRightMouseDown += delegate
            {
                var rc = utils.Read_Ini(App.dconfig, "Configuration", "rightclick", "1");
                switch (rc)
                {
                    case "2":
                        utils.RunExe("menu()");
                        break;
                    case "3":
                        var rce = utils.Read_Ini(App.dconfig, "Configuration", "rightaction", "");
                        utils.RunExe(rce);
                        break;
                    default:
                        if (App.mn != null)
                        {
                            if (App.mn.Visibility == Visibility.Visible)
                                utils.RunExe("hide()");
                            else
                                utils.RunExe("show()");
                        }
                        else
                        {
                            utils.RunExe("show()");
                        }
                        break;
                }
            };
            ti.TrayLeftMouseDown += delegate
            {
                var lc = utils.Read_Ini(App.dconfig, "Configuration", "leftclick", "1");
                switch (lc)
                {
                    case "2":
                        utils.RunExe("menu()");
                        break;
                    case "3":
                        var lce = utils.Read_Ini(App.dconfig, "Configuration", "leftaction", "");
                        utils.RunExe(lce);
                        break;
                    default:
                        if (App.mn != null)
                        {
                            if (App.mn.Visibility == Visibility.Visible)
                                utils.RunExe("hide()");
                            else
                                utils.RunExe("show()");
                        }
                        else
                        {
                            utils.RunExe("show()");
                        }
                        break;
                }
            };
        }

        #region 颜色处理
        public static Color Color_Multiple(Color origin, int Multiple)
        {
            Multiple = (Multiple > 255) ? 255 : (Multiple < 0) ? 0 : Multiple;
            double new_R = origin.R * Multiple / 255;
            double new_B = origin.B * Multiple / 255;
            double new_G = origin.G * Multiple / 255;
            return Color.FromRgb((byte)new_R, (byte)new_G, (byte)new_B);
        }
        #endregion

        private void main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ti.Dispose();
            utils.LogtoFile("[INFOMATION]Main UI: Closing " + e.GetType().ToString());
            Microsoft.Toolkit.Uwp.Notifications.ToastNotificationManagerCompat.Uninstall();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            System.Windows.Controls.Canvas.SetTop(this, -233);
            System.Windows.Controls.Canvas.SetLeft(this, -233);
        }
    }
}
