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
        internal Windows.Networking.Connectivity.NetworkConnectivityLevel ncl = Windows.Networking.Connectivity.NetworkConnectivityLevel.None;
        public MainWindow()
        {
            InitializeComponent();
            System.Windows.Controls.Canvas.SetTop(this, -233);
            System.Windows.Controls.Canvas.SetLeft(this, -233);
        }
        public void InitializeInnerParameters()
        {
            ti.ToolTipText = App.AppTitle;
            Title = App.AppTitle;
            InitializeMethod();
            SourceInitialized += OnSourceInitialized;
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged += new System.Net.NetworkInformation.NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);
            Windows.Networking.Connectivity.ConnectionProfile profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            if (profile != null)
                ncl = profile.GetNetworkConnectivityLevel();
            try
            {
                Windows.System.Power.PowerManager.RemainingChargePercentChanged += PowerManager_RemainingChargePercentChanged;
                Windows.System.Power.PowerManager.EnergySaverStatusChanged += PowerManager_EnergySaverStatusChanged;
            }
            catch (Exception ex)
            {
                utils.LogtoFile("[ERROR]" + ex.Message);
            }
        }

        private void PowerManager_EnergySaverStatusChanged(object? sender, object e)
        {
            var p = Windows.System.Power.PowerManager.EnergySaverStatus;
            if (utils.Read_Ini(App.dconfig, "Configuration", "verbose", "1").Equals("1"))
            {
                switch (p)
                {
                    case Windows.System.Power.EnergySaverStatus.On:
                        App.Notify(new noticeitem(utils.Get_Transalte("basaveron"), 2));
                        break;
                    case Windows.System.Power.EnergySaverStatus.Disabled:
                        App.Notify(new noticeitem(utils.Get_Transalte("basaveroff"), 2));
                        break;
                    default:
                        break;
                }
            }
        }

        private void PowerManager_RemainingChargePercentChanged(object? sender, object e)
        {
            int p = Windows.System.Power.PowerManager.RemainingChargePercent;
            if (utils.Read_Ini(App.dconfig, "Configuration", "verbose", "1").Equals("1"))
            {
                if (Windows.System.Power.PowerManager.BatteryStatus != Windows.System.Power.BatteryStatus.Charging)
                {
                    var low = utils.Read_Ini(App.LangFilePath, "local", "lowpower", "[0,1,2,3,4,6,8,10,20,30]").Replace("[", "[,").Replace("]", ",]").Trim();
                    if (low.IndexOf(p.ToString()) != -1)
                        App.Notify(new noticeitem(utils.Get_Transalte("powerlow").Replace("%p", p.ToString()), 2));
                    else if (p % 10 == 0)
                        App.Notify(new noticeitem(utils.Get_Transalte("powertip").Replace("%p", p.ToString()), 2));
                }
            }
        }

        private void NetworkChange_NetworkAddressChanged(object? sender, EventArgs e)
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
                Windows.Networking.Connectivity.ConnectionProfile profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
                if (profile != null)
                {
                    if (ncl == profile.GetNetworkConnectivityLevel())
                        return;
                    ncl = profile.GetNetworkConnectivityLevel();
                }
                else
                {
                    if (ncl == Windows.Networking.Connectivity.NetworkConnectivityLevel.None)
                        return;
                    ncl = Windows.Networking.Connectivity.NetworkConnectivityLevel.None;
                }
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    switch (ncl)
                    {
                        case Windows.Networking.Connectivity.NetworkConnectivityLevel.InternetAccess:
                            App.Notify(new noticeitem(utils.Get_Transalte("neton"), 2));
                            break;
                        case Windows.Networking.Connectivity.NetworkConnectivityLevel.LocalAccess:
                            App.Notify(new noticeitem(utils.Get_Transalte("netlan"), 2));
                            break;
                        case Windows.Networking.Connectivity.NetworkConnectivityLevel.ConstrainedInternetAccess:
                            App.Notify(new noticeitem(utils.Get_Transalte("netlimit"), 2));
                            break;
                        default:
                            App.Notify(new noticeitem(utils.Get_Transalte("netoff"), 2));
                            break;
                    };
                }));
            }
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
            Microsoft.Toolkit.Uwp.Notifications.ToastNotificationManagerCompat.Uninstall();
            utils.LogtoFile("[INFOMATION]Main UI: Closing " + e.GetType().ToString());
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            System.Windows.Controls.Canvas.SetTop(this, -233);
            System.Windows.Controls.Canvas.SetLeft(this, -233);
        }
    }
}
