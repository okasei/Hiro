using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using static Hiro.Helpers.Hiro_Class;

namespace Hiro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal System.Windows.Controls.ContextMenu? cm = null;
        internal Windows.Networking.Connectivity.NetworkConnectivityLevel ncl = Windows.Networking.Connectivity.NetworkConnectivityLevel.None;
        internal string rec_nc = "";
        internal string disks = ";";
        public MainWindow()
        {
            InitializeComponent();
            System.Windows.Controls.Canvas.SetTop(this, -23333);
            System.Windows.Controls.Canvas.SetLeft(this, -23333);
            ScanDiskUnload();
        }

        public void InitializeInnerParameters()
        {
            trayText.Text = App.appTitle;
            Title = App.appTitle;
            InitializeMethod();
            SourceInitialized += OnSourceInitialized;
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged += new System.Net.NetworkInformation.NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);
            var profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            if (profile != null)
                ncl = profile.GetNetworkConnectivityLevel();
            try
            {
                Windows.System.Power.PowerManager.RemainingChargePercentChanged += PowerManager_RemainingChargePercentChanged;
                Windows.System.Power.PowerManager.EnergySaverStatusChanged += PowerManager_EnergySaverStatusChanged;
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.Power");
            }
            Helpers.Hiro_UI.SetCustomWindowIcon(this);
            var iconP = Hiro_Utils.Read_PPDCIni("CustomTrayIcon", "");
            if (File.Exists(iconP))
            {
                try
                {
                    Hiro_Tray.Icon = new System.Drawing.Icon(iconP);
                }
                catch(Exception e)
                {
                    Hiro_Utils.LogError(e, "Hiro.Tray.CustomeIcon");
                }
            }
        }


        private void PowerManager_EnergySaverStatusChanged(object? sender, object e)
        {
            try
            {
                var p = Windows.System.Power.PowerManager.EnergySaverStatus;
                if (Hiro_Utils.Read_DCIni("Verbose", "0").Equals("1"))
                {
                    switch (p)
                    {
                        case Windows.System.Power.EnergySaverStatus.On:
                            Dispatcher.Invoke(() =>
                            {
                                App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("basaveron"), 2, Hiro_Utils.Get_Translate("battery")));
                            });
                            break;
                        case Windows.System.Power.EnergySaverStatus.Disabled:
                            Dispatcher.Invoke(() =>
                            {
                                App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("basaveroff"), 2, Hiro_Utils.Get_Translate("battery")));
                            });
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Power.StatusChanged");
            }
        }

        private void PowerManager_RemainingChargePercentChanged(object? sender, object e)
        {
            try
            {
                int p = Windows.System.Power.PowerManager.RemainingChargePercent;
                if (Hiro_Utils.Read_DCIni("Verbose", "0").Equals("1"))
                {
                    if (Windows.System.Power.PowerManager.BatteryStatus ==
                        Windows.System.Power.BatteryStatus.Charging)
                        return;
                    var low = Hiro_Utils.Read_Ini(App.langFilePath, "local", "lowpower", "[0,1,2,3,4,6,8,10,20,30]").Replace("[", "[,").Replace("]", ",]").Trim();
                    var notice = Hiro_Utils.Read_Ini(App.langFilePath, "local", "tippower", "").Replace("[", "[,").Replace("]", ",]").Trim();
                    if (low.IndexOf(p.ToString()) != -1)
                        App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("powerlow").Replace("%p", p.ToString()), 2, Hiro_Utils.Get_Translate("battery")));
                    else if (notice.IndexOf(p.ToString()) != -1)
                        App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("powertip").Replace("%p", p.ToString()), 2, Hiro_Utils.Get_Translate("battery")));
                }
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Power.PercentChanged");
            }

        }

        private void NetworkChange_NetworkAddressChanged(object? sender, EventArgs e)
        {
            if (App.dflag)
            {
                System.Net.NetworkInformation.NetworkInterface[] nfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                foreach (System.Net.NetworkInformation.NetworkInterface ni in nfaces)
                {
                    Hiro_Utils.LogtoFile(ni.Description + " - " + ni.NetworkInterfaceType.ToString());
                }
            }

            if (!Hiro_Utils.Read_DCIni("Verbose", "0").Equals("1"))
                return;
            var profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            string ext = "";
            if (profile != null)
            {
                try
                {
                    if (profile.IsWlanConnectionProfile)
                    {
                        if (profile.WlanConnectionProfileDetails.GetConnectedSsid().Equals(string.Empty))
                            return;
                        ext = Hiro_Utils.Get_Translate("netwlan").Replace("%s", profile.WlanConnectionProfileDetails.GetConnectedSsid());
                    }
                    else if (profile.IsWwanConnectionProfile)
                    {
                        if (profile.WwanConnectionProfileDetails.AccessPointName.Equals(string.Empty))
                            return;
                        ext = Hiro_Utils.Get_Translate("netwwan").Replace("%s", profile.WwanConnectionProfileDetails.AccessPointName);
                    }
                    else
                    {
                        ext = Hiro_Utils.Get_Translate("neteth");
                    }
                }
                catch (Exception ex)
                {
                    Hiro_Utils.LogError(ex, "Hiro.Exception.Wifi.Network");
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
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                switch (ncl)
                {
                    case Windows.Networking.Connectivity.NetworkConnectivityLevel.InternetAccess:
                        App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("neton").Replace("%s", ext), 2, Hiro_Utils.Get_Translate("net")));
                        break;
                    case Windows.Networking.Connectivity.NetworkConnectivityLevel.LocalAccess:
                        App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("netlan").Replace("%s", ext), 2, Hiro_Utils.Get_Translate("net")));
                        break;
                    case Windows.Networking.Connectivity.NetworkConnectivityLevel.ConstrainedInternetAccess:
                        App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("netlimit").Replace("%s", ext), 2, Hiro_Utils.Get_Translate("net")));
                        break;
                    default:
                        App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("netoff"), 2, Hiro_Utils.Get_Translate("net")));
                        break;
                };
            }));
        }


        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            App.WND_Handle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            var source = System.Windows.Interop.HwndSource.FromHwnd(App.WND_Handle);
            source?.AddHook(WndProc);
            Hiro_Utils.LogtoFile("[HIROWEGO]Main Window: AddHook WndProc");
            AddClipboardFormatListener(App.WND_Handle);
        }


        private DriveInfo? ScanDisk()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                if (drive.DriveType != DriveType.Unknown)
                {
                    var label = drive.Name.ToLower() + ";";
                    if (!disks.Contains(label + ";"))
                    {
                        disks = disks + label + ";";
                        return drive;
                    }
                }
            }
            return null;
        }

        private void ScanDiskUnload()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            disks = ";";
            foreach (var drive in drives)
            {
                if (drive.DriveType != DriveType.Unknown)
                {
                    var label = drive.Name.ToLower() + ";";
                    disks = disks + label + ";";
                }
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0320://系统颜色改变
                    if (Hiro_Utils.Read_DCIni("LockColor", "default").Equals("default"))
                        App.ColorCD = 3;
                    break;
                case 0x0083://prevent system from drawing outline
                    handled = true;
                    break;
                case 0x0218:
                    if (Hiro_Utils.Read_DCIni("Verbose", "0").Equals("1"))
                    {
                        try
                        {
                            GetSystemPowerStatus(out SYSTEM_POWER_STATUS p);
                            if (p.ACLineStatus == 1 && p.BatteryFlag == 8)
                                App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("bacharge"), 2, Hiro_Utils.Get_Translate("battery")));
                            if (p.ACLineStatus == 1 && p.BatteryFlag != 8)
                                App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("baconnect"), 2, Hiro_Utils.Get_Translate("battery")));
                            if (p.ACLineStatus == 0)
                                App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("baremove"), 2, Hiro_Utils.Get_Translate("battery")));
                        }
                        catch (Exception ex)
                        {
                            Hiro_Utils.LogError(ex, "Hiro.Power.Impl");
                        }

                    }
                    break;
                case 0x0219:
                    if (Hiro_Utils.Read_DCIni("Verbose", "0").Equals("1"))
                    {
                        Action? ac = null;
                        var mms = Hiro_Utils.Get_Translate("deinfo") + " - ";
                        switch (wParam.ToInt32())
                        {
                            case 0x0018:
                                mms += Hiro_Utils.Get_Translate("deconfig");
                                break;
                            case 0x8000:
                                {
                                    var a = ScanDisk();
                                    if (null != a)
                                    {
                                        ac = new(() =>
                                        {
                                            Hiro_Utils.RunExe(a.RootDirectory.ToString(), Hiro_Utils.Get_Translate("device"), false);
                                        });
                                        mms += Hiro_Utils.Get_Translate("medconname").Replace("%n", a.VolumeLabel).Replace("%l", a.Name);
                                    }
                                    else
                                    {
                                        mms += Hiro_Utils.Get_Translate("medconnect");
                                    }
                                    break;
                                }
                            case 0x8004:
                                {
                                    ScanDiskUnload();
                                    mms += Hiro_Utils.Get_Translate("medremove");
                                    break;
                                }
                            default:
                                return IntPtr.Zero;
                        }
                        App.Notify(new Hiro_Notice(mms, 2, Hiro_Utils.Get_Translate("device"), ac));
                    }
                    break;
                case 0x031D:
                    //MessageBox.Show(Clipboard.GetDataObject().GetFormats()[0]);
                    break;
                case 0x0312:
                    handled = true;
                    new System.Threading.Thread(() =>
                    {
                        try
                        {
                            var indexid = wParam.ToInt32();
                            if (App.dflag)
                                Hiro_Utils.LogtoFile($"[DEBUG]Hotkey Triggered as Number {indexid}");
                            for (int vsi = 0; vsi < App.vs.Count - 1; vsi += 2)
                            {
                                if (App.vs[vsi] == indexid)
                                {
                                    var name = "***NOT INITIALIZED***";
                                    var cmd = "<nop>";
                                    Dispatcher.Invoke(delegate
                                    {
                                        name = App.cmditems[App.vs[vsi + 1]].Name;
                                        cmd = App.cmditems[App.vs[vsi + 1]].Command;
                                    });
                                    if (App.dflag)
                                        Hiro_Utils.LogtoFile($"[DEBUG]Hotkey Corresponded Item {{ Name: {name} , Command: {cmd} }}");
                                    Hiro_Utils.RunExe(cmd, name);
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Hiro_Utils.LogError(ex, "Hiro.Hotkey.Run");
                        }
                    }).Start();
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;

        }

        public void Load_All_Colors()
        {
            Hiro_Utils.IntializeColorParameters();
            App.ed?.Load_Color();
            App.noti?.Load_Color();
            App.hisland?.Load_Color();
            App.mn?.Load_Colors();
            foreach (Window win in Application.Current.Windows)
            {
                switch (win)
                {
                    case Hiro_Alarm a:
                        a.Load_Colors();
                        break;
                    case Hiro_Download b:
                        b.Load_Colors();
                        break;
                    case Hiro_LockScreen c:
                        c.Load_Colors();
                        break;
                    case Hiro_Msg f:
                        f.Load_Colors();
                        break;
                    case Hiro_Sequence d:
                        d.Load_Colors();
                        break;
                    case Hiro_Web e:
                        e.Load_Color();
                        break;
                    case Hiro_Finder g:
                        g.Load_Color();
                        break;
                    case Hiro_Player h:
                        h.Load_Color();
                        break;
                    case Hiro_Ticker i:
                        i.Load_Color();
                        break;
                    case Hiro_Encrypter j:
                        j.Load_Colors();
                        break;
                    case Hiro_Splash k:
                        k.Load_Color();
                        break;
                    case Hiro_Badge l:
                        l.LoadColor();
                        break;
                }
                System.Windows.Forms.Application.DoEvents();
            }
        }

        private void InitializeMethod()
        {
            Hiro_Tray.TrayMiddleMouseDown += delegate
            {
                var mc = Hiro_Utils.Read_DCIni("MiddleClick", "2");
                switch (mc)
                {
                    case "2":
                        Hiro_Utils.RunExe("menu()"); ;
                        break;
                    case "3":
                        var mce = Hiro_Utils.Read_DCIni("MiddleAction", "");
                        Hiro_Utils.RunExe(mce);
                        break;
                    default:
                        if (App.mn != null)
                        {
                            Hiro_Utils.RunExe(App.mn.Visibility == Visibility.Visible ? "hide()" : "show()");
                        }
                        else
                        {
                            Hiro_Utils.RunExe("show()");
                        }
                        break;
                }
            };
            Hiro_Tray.TrayRightMouseDown += delegate
            {
                var rc = Hiro_Utils.Read_DCIni("RightClick", "2");
                switch (rc)
                {
                    case "2":
                        Hiro_Utils.RunExe("menu()");
                        break;
                    case "3":
                        var rce = Hiro_Utils.Read_DCIni("RightAction", "");
                        Hiro_Utils.RunExe(rce);
                        break;
                    default:
                        if (App.mn != null)
                        {
                            Hiro_Utils.RunExe(App.mn.Visibility == Visibility.Visible ? "hide()" : "show()");
                        }
                        else
                        {
                            Hiro_Utils.RunExe("show()");
                        }
                        break;
                }
            };
            Hiro_Tray.TrayLeftMouseDown += delegate
            {
                var lc = Hiro_Utils.Read_DCIni("LeftClick", "1");
                switch (lc)
                {
                    case "2":
                        Hiro_Utils.RunExe("menu()");
                        break;
                    case "3":
                        var lce = Hiro_Utils.Read_DCIni("LeftAction", "");
                        Hiro_Utils.RunExe(lce);
                        break;
                    default:
                        if (App.mn != null)
                        {
                            Hiro_Utils.RunExe(App.mn.Visibility == Visibility.Visible ? "hide()" : "show()");
                        }
                        else
                        {
                            Hiro_Utils.RunExe("show()");
                        }
                        break;
                }
            };
        }

        private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hiro_Tray.Dispose();
            Microsoft.Toolkit.Uwp.Notifications.ToastNotificationManagerCompat.Uninstall();
            Hiro_Utils.LogtoFile("[INFOMATION]Main UI: Closing " + e.GetType().ToString());
            RemoveClipboardFormatListener(App.WND_Handle);
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            System.Windows.Controls.Canvas.SetTop(this, -233);
            System.Windows.Controls.Canvas.SetLeft(this, -233);
        }

        #region 获取系统电量

        struct SYSTEM_POWER_STATUS
        {
            public byte ACLineStatus;
            public byte BatteryFlag;
            public byte BatteryLifePercent;
            public byte Reserved1;
            public int BatteryLifeTime;
            public int BatteryFullLifeTime;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool GetSystemPowerStatus(out SYSTEM_POWER_STATUS lpSystemPowerStatus);
        #endregion

        #region 监听剪辑版
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool AddClipboardFormatListener(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RemoveClipboardFormatListener(IntPtr hWnd);
        #endregion
    }
}
