using System;
using System.Windows;
using System.Windows.Input;
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
        internal string rec_nc = "";
        internal static System.Collections.ObjectModel.ObservableCollection<int> vs = new();
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

        public static int FindHotkeyById(int id)
        {
            for (int vsi = 0; vsi < vs.Count - 1; vsi = vsi + 2)
            {
                if (vs[vsi + 1] == id)
                    return vsi;
            }
            return -1;
        }

        public void UnregisterKey(int id)
        {
            HiroHotKey.UnRegisterKey(new System.Windows.Interop.WindowInteropHelper(this).Handle, id);
            vs.RemoveAt(id);
            vs.RemoveAt(id);
        }
        public void RegisterKey(uint modi, Key id, int cid)
        {
            vs.Add(HiroHotKey.RegisterKey(new System.Windows.Interop.WindowInteropHelper(this).Handle, modi, id));
            vs.Add(cid);
        }

        private void PowerManager_EnergySaverStatusChanged(object? sender, object e)
        {
            var p = Windows.System.Power.PowerManager.EnergySaverStatus;
            if (utils.Read_Ini(App.dconfig, "config", "verbose", "1").Equals("1"))
            {
                switch (p)
                {
                    case Windows.System.Power.EnergySaverStatus.On:
                        App.Notify(new noticeitem(utils.Get_Transalte("basaveron"), 2, utils.Get_Transalte("battery")));
                        break;
                    case Windows.System.Power.EnergySaverStatus.Disabled:
                        App.Notify(new noticeitem(utils.Get_Transalte("basaveroff"), 2, utils.Get_Transalte("battery")));
                        break;
                    default:
                        break;
                }
            }
        }

        private void PowerManager_RemainingChargePercentChanged(object? sender, object e)
        {
            int p = Windows.System.Power.PowerManager.RemainingChargePercent;
            if (utils.Read_Ini(App.dconfig, "config", "verbose", "1").Equals("1"))
            {
                if (Windows.System.Power.PowerManager.BatteryStatus != Windows.System.Power.BatteryStatus.Charging)
                {
                    var low = utils.Read_Ini(App.LangFilePath, "local", "lowpower", "[0,1,2,3,4,6,8,10,20,30]").Replace("[", "[,").Replace("]", ",]").Trim();
                    if (low.IndexOf(p.ToString()) != -1)
                        App.Notify(new noticeitem(utils.Get_Transalte("powerlow").Replace("%p", p.ToString()), 2, utils.Get_Transalte("battery")));
                    else if (p % 10 == 0)
                        App.Notify(new noticeitem(utils.Get_Transalte("powertip").Replace("%p", p.ToString()), 2, utils.Get_Transalte("battery")));
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
            if (utils.Read_Ini(App.dconfig, "config", "verbose", "1").Equals("1"))
            {
                Windows.Networking.Connectivity.ConnectionProfile profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
                string ext = "";
                if (profile != null)
                {
                    try
                    {
                        if (profile.IsWlanConnectionProfile)
                        {
                            if (profile.WlanConnectionProfileDetails.GetConnectedSsid().Equals(string.Empty))
                                return;
                            ext = utils.Get_Transalte("netwlan").Replace("%s", profile.WlanConnectionProfileDetails.GetConnectedSsid());
                        }
                        else if (profile.IsWwanConnectionProfile)
                        {
                            if (profile.WwanConnectionProfileDetails.AccessPointName.Equals(string.Empty))
                                return;
                            ext = utils.Get_Transalte("netwwan").Replace("%s", profile.WwanConnectionProfileDetails.AccessPointName);
                        }
                        else
                        {
                            ext = utils.Get_Transalte("neteth");
                        }
                    }
                    catch(Exception ex)
                    {
                        utils.LogtoFile("[ERROR]" + ex.Message);
                    }
                    if (ncl == profile.GetNetworkConnectivityLevel() && rec_nc.Equals(ext))
                        return;
                    ncl = profile.GetNetworkConnectivityLevel();
                    rec_nc = ext;
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
                            App.Notify(new noticeitem(utils.Get_Transalte("neton").Replace("%s", ext), 2, utils.Get_Transalte("net")));
                            break;
                        case Windows.Networking.Connectivity.NetworkConnectivityLevel.LocalAccess:
                            App.Notify(new noticeitem(utils.Get_Transalte("netlan").Replace("%s", ext), 2, utils.Get_Transalte("net")));
                            break;
                        case Windows.Networking.Connectivity.NetworkConnectivityLevel.ConstrainedInternetAccess:
                            App.Notify(new noticeitem(utils.Get_Transalte("netlimit").Replace("%s", ext), 2, utils.Get_Transalte("net")));
                            break;
                        default:
                            App.Notify(new noticeitem(utils.Get_Transalte("netoff"), 2, utils.Get_Transalte("net")));
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
            utils.LogtoFile("[HIROWEGO]Main Window: Load Data");
            Load_Data();
        }

        public void Load_Data()
        {
            App.cmditems.Clear();
            var i = 1;
            var p = 1;
            var inipath = App.dconfig;
            var ti = utils.Read_Ini(inipath, i.ToString(), "title", "");
            var co = utils.Read_Ini(inipath, i.ToString(), "command", "");
            while (!ti.Trim().Equals("") && co.StartsWith("(") && co.EndsWith(")"))
            {
                var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
                var hwnd = windowInteropHelper.Handle;
                var key = utils.Read_Ini(App.dconfig, i.ToString(), "hotkey", "").Trim();
                try
                {
                    if (key.IndexOf(",") != -1)
                    {
                        var mo = uint.Parse(key.Substring(0, key.IndexOf(",")));
                        var vkey = uint.Parse(key.Substring(key.IndexOf(",") + 1, key.Length - key.IndexOf(",") - 1));
                        try
                        {
                            if (mo != 0 && vkey != 0)
                            {
                                vs.Add(HiroHotKey.RegisterKey(hwnd, mo, (Key)vkey));
                                vs.Add(i - 1);
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

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0320://系统颜色改变
                    if (utils.Read_Ini(App.dconfig, "config", "lock", "0").Equals("1"))
                        break;
                    App.ColorCD = 3;
                    break;
                case 0x0083://prevent system from drawing outline
                    handled = true;
                    break;
                case 0x0218:
                    if (utils.Read_Ini(App.dconfig, "config", "verbose", "1").Equals("1"))
                    {
                        utils.GetSystemPowerStatus(out utils.SYSTEM_POWER_STATUS p);
                        if (p.ACLineStatus == 1 && p.BatteryFlag == 8)
                            App.Notify(new noticeitem(utils.Get_Transalte("bacharge"), 2, utils.Get_Transalte("battery")));
                        if (p.ACLineStatus == 1 && p.BatteryFlag != 8)
                            App.Notify(new noticeitem(utils.Get_Transalte("baconnect"), 2, utils.Get_Transalte("battery")));
                        if (p.ACLineStatus == 0)
                            App.Notify(new noticeitem(utils.Get_Transalte("baremove"), 2, utils.Get_Transalte("battery")));
                    }
                    break;
                case 0x0219:
                    if (utils.Read_Ini(App.dconfig, "config", "verbose", "1").Equals("1"))
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
                        App.Notify(new noticeitem(mms, 2, utils.Get_Transalte("device")));
                    }
                    break;
                case 0x0312:
                    System.ComponentModel.BackgroundWorker bw = new();
                    bw.DoWork += delegate
                    {
                        var indexid = wParam.ToInt32();
                        for (int vsi = 0; vsi < vs.Count - 1; vsi = vsi + 2)
                        {
                            if (vs[vsi] == indexid)
                            {
                                Dispatcher.Invoke(delegate
                                {
                                    utils.RunExe(App.cmditems[vs[vsi + 1]].Command);
                                });
                                break;
                            }
                        }
                    };
                    bw.RunWorkerAsync();
                    break;
                default:
                    if (App.dflag)
                        utils.LogtoFile("[DEBUG]Msg: " + msg.ToString() + ";LParam: " + lParam.ToString() + ";WParam: " + wParam.ToString());
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
                    if (win is Alarm a)
                    {
                        a.Load_Colors();
                    }
                    if (win is Download b)
                    {
                        b.Load_Colors();
                    }
                    if (win is Lockscr c)
                    {
                        c.Load_Colors();
                    }
                    if (win is Message f)
                    {
                        f.Load_Colors();
                    }
                    if (win is Sequence d)
                    {
                        d.Load_Colors();
                    }
                    if (win is Web e)
                    {
                        e.wvpb.Foreground = new SolidColorBrush(App.AppAccentColor);
                    }
                System.Windows.Forms.Application.DoEvents();
            }
        }

        private void InitializeMethod()
        {
            ti.TrayMiddleMouseDown += delegate
            {
                var mc = utils.Read_Ini(App.dconfig, "config", "middleclick", "1");
                switch (mc)
                {
                    case "2":
                        utils.RunExe("menu()");;
                        break;
                    case "3":
                        var mce = utils.Read_Ini(App.dconfig, "config", "middleaction", "");
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
                var rc = utils.Read_Ini(App.dconfig, "config", "rightclick", "1");
                switch (rc)
                {
                    case "2":
                        utils.RunExe("menu()");
                        break;
                    case "3":
                        var rce = utils.Read_Ini(App.dconfig, "config", "rightaction", "");
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
                var lc = utils.Read_Ini(App.dconfig, "config", "leftclick", "1");
                switch (lc)
                {
                    case "2":
                        utils.RunExe("menu()");
                        break;
                    case "3":
                        var lce = utils.Read_Ini(App.dconfig, "config", "leftaction", "");
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

        private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
