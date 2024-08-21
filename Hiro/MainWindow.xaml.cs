using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using static Hiro.Helpers.HClass;
using static Hiro.Helpers.HText;
using static Hiro.Helpers.HLogger;
using static Hiro.Helpers.HSet;
using Hiro.Helpers;
using System.Windows.Media;

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
                LogError(ex, "Hiro.Exception.Power");
            }
            Helpers.HUI.SetCustomWindowIcon(this);
            var iconP = Read_PPDCIni("CustomTrayIcon", "");
            if (File.Exists(iconP))
            {
                try
                {
                    Hiro_Tray.Icon = new System.Drawing.Icon(iconP);
                }
                catch(Exception e)
                {
                    LogError(e, "Hiro.Tray.CustomeIcon");
                }
            }
        }


        private void PowerManager_EnergySaverStatusChanged(object? sender, object e)
        {
            try
            {
                var p = Windows.System.Power.PowerManager.EnergySaverStatus;
                if (Read_DCIni("Verbose", "0").Equals("1"))
                {
                    switch (p)
                    {
                        case Windows.System.Power.EnergySaverStatus.On:
                            Dispatcher.Invoke(() =>
                            {
                                App.Notify(new Hiro_Notice(Get_Translate("basaveron"), 2, Get_Translate("battery")));
                            });
                            break;
                        case Windows.System.Power.EnergySaverStatus.Disabled:
                            Dispatcher.Invoke(() =>
                            {
                                App.Notify(new Hiro_Notice(Get_Translate("basaveroff"), 2, Get_Translate("battery")));
                            });
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "Hiro.Power.StatusChanged");
            }
        }

        private void PowerManager_RemainingChargePercentChanged(object? sender, object e)
        {
            try
            {
                int p = Windows.System.Power.PowerManager.RemainingChargePercent;
                if (Read_DCIni("Verbose", "0").Equals("1"))
                {
                    if (Windows.System.Power.PowerManager.BatteryStatus ==
                        Windows.System.Power.BatteryStatus.Charging)
                        return;
                    var low = Read_Ini(App.langFilePath, "local", "lowpower", "[0,1,2,3,4,6,8,10,20,30]").Replace("[", "[,").Replace("]", ",]").Trim();
                    var notice = Read_Ini(App.langFilePath, "local", "tippower", "").Replace("[", "[,").Replace("]", ",]").Trim();
                    if (low.IndexOf(p.ToString()) != -1)
                        App.Notify(new Hiro_Notice(Get_Translate("powerlow").Replace("%p", p.ToString()), 2, Get_Translate("battery")));
                    else if (notice.IndexOf(p.ToString()) != -1)
                        App.Notify(new Hiro_Notice(Get_Translate("powertip").Replace("%p", p.ToString()), 2, Get_Translate("battery")));
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "Hiro.Power.PercentChanged");
            }

        }

        private void NetworkChange_NetworkAddressChanged(object? sender, EventArgs e)
        {
            if (App.dflag)
            {
                System.Net.NetworkInformation.NetworkInterface[] nfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                foreach (System.Net.NetworkInformation.NetworkInterface ni in nfaces)
                {
                    LogtoFile(ni.Description + " - " + ni.NetworkInterfaceType.ToString());
                }
            }

            if (!Read_DCIni("Verbose", "0").Equals("1"))
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
                        ext = Get_Translate("netwlan").Replace("%s", profile.WlanConnectionProfileDetails.GetConnectedSsid());
                    }
                    else if (profile.IsWwanConnectionProfile)
                    {
                        if (profile.WwanConnectionProfileDetails.AccessPointName.Equals(string.Empty))
                            return;
                        ext = Get_Translate("netwwan").Replace("%s", profile.WwanConnectionProfileDetails.AccessPointName);
                    }
                    else
                    {
                        ext = Get_Translate("neteth");
                    }
                }
                catch (Exception ex)
                {
                    LogError(ex, "Hiro.Exception.Wifi.Network");
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
                        App.Notify(new Hiro_Notice(Get_Translate("neton").Replace("%s", ext), 2, Get_Translate("net")));
                        break;
                    case Windows.Networking.Connectivity.NetworkConnectivityLevel.LocalAccess:
                        App.Notify(new Hiro_Notice(Get_Translate("netlan").Replace("%s", ext), 2, Get_Translate("net")));
                        break;
                    case Windows.Networking.Connectivity.NetworkConnectivityLevel.ConstrainedInternetAccess:
                        App.Notify(new Hiro_Notice(Get_Translate("netlimit").Replace("%s", ext), 2, Get_Translate("net")));
                        break;
                    default:
                        App.Notify(new Hiro_Notice(Get_Translate("netoff"), 2, Get_Translate("net")));
                        break;
                };
            }));
        }


        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            App.WND_Handle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            var source = System.Windows.Interop.HwndSource.FromHwnd(App.WND_Handle);
            source?.AddHook(WndProc);
            LogtoFile("[HIROWEGO]Main Window: AddHook WndProc");
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
                    if (Read_DCIni("LockColor", "default").Equals("default"))
                        App.ColorCD = 3;
                    break;
                case 0x0083://prevent system from drawing outline
                    handled = true;
                    break;
                case 0x0218:
                    if (Read_DCIni("Verbose", "0").Equals("1"))
                    {
                        try
                        {
                            GetSystemPowerStatus(out SYSTEM_POWER_STATUS p);
                            if (p.ACLineStatus == 1 && p.BatteryFlag == 8)
                                App.Notify(new Hiro_Notice(Get_Translate("bacharge"), 2, Get_Translate("battery")));
                            if (p.ACLineStatus == 1 && p.BatteryFlag != 8)
                                App.Notify(new Hiro_Notice(Get_Translate("baconnect"), 2, Get_Translate("battery")));
                            if (p.ACLineStatus == 0)
                                App.Notify(new Hiro_Notice(Get_Translate("baremove"), 2, Get_Translate("battery")));
                        }
                        catch (Exception ex)
                        {
                            LogError(ex, "Hiro.Power.Impl");
                        }

                    }
                    break;
                case 0x0219:
                    if (Read_DCIni("Verbose", "0").Equals("1"))
                    {
                        Action? ac = null;
                        var mms = Get_Translate("deinfo") + " - ";
                        switch (wParam.ToInt32())
                        {
                            case 0x0018:
                                mms += Get_Translate("deconfig");
                                break;
                            case 0x8000:
                                {
                                    var a = ScanDisk();
                                    if (null != a)
                                    {
                                        ac = new(() =>
                                        {
                                            Hiro_Utils.RunExe(a.RootDirectory.ToString(), Get_Translate("device"), false);
                                        });
                                        mms += Get_Translate("medconname").Replace("%n", a.VolumeLabel).Replace("%l", a.Name);
                                    }
                                    else
                                    {
                                        mms += Get_Translate("medconnect");
                                    }
                                    break;
                                }
                            case 0x8004:
                                {
                                    ScanDiskUnload();
                                    mms += Get_Translate("medremove");
                                    break;
                                }
                            default:
                                return IntPtr.Zero;
                        }
                        App.Notify(new Hiro_Notice(mms, 2, Get_Translate("device"), ac));
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
                                LogtoFile($"[DEBUG]Hotkey Triggered as Number {indexid}");
                            var itemID = HHotKeys.FindHotkeyByKeyId(indexid);
                            if(itemID >= 0)
                            {
                                var name = "***NOT INITIALIZED***";
                                var cmd = "<nop>";
                                Dispatcher.Invoke(delegate
                                {
                                    name = App.cmditems[itemID].Name;
                                    cmd = App.cmditems[itemID].Command;
                                });
                                if (App.dflag)
                                    LogtoFile($"[DEBUG]Hotkey Corresponded Item {{ Name: {name} , Command: {cmd} }}");
                                Hiro_Utils.RunExe(cmd, name);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogError(ex, "Hiro.Hotkey.Run");
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
            App.LoadDictionaryColors();
            App.mn?.Load_Colors();
            App.ed?.Load_Color();
            App.noti?.Load_Color();
            App.hisland?.Load_Color();
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
                    case Hiro_ImageViewer m:
                        m.Load_Color();
                        break;
                    case Hiro_TextEditor n:
                        n.Load_Color();
                        break;
                }
                System.Windows.Forms.Application.DoEvents();
            }
        }

        private void InitializeMethod()
        {
            Hiro_Tray.TrayMiddleMouseDown += delegate
            {
                var mc = Read_DCIni("MiddleClick", "2");
                switch (mc)
                {
                    case "2":
                        Hiro_Utils.RunExe("menu()"); ;
                        break;
                    case "3":
                        var mce = Read_DCIni("MiddleAction", "");
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
                var rc = Read_DCIni("RightClick", "2");
                switch (rc)
                {
                    case "2":
                        Hiro_Utils.RunExe("menu()");
                        break;
                    case "3":
                        var rce = Read_DCIni("RightAction", "");
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
                var lc = Read_DCIni("LeftClick", "1");
                switch (lc)
                {
                    case "2":
                        Hiro_Utils.RunExe("menu()");
                        break;
                    case "3":
                        var lce = Read_DCIni("LeftAction", "");
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
            LogtoFile("[INFOMATION]Main UI: Closing " + e.GetType().ToString());
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

        private void Hiro_Tray_Drop(object sender, DragEventArgs e)
        {
            Hiro_Utils.RunExe(e.Data.ToString());
        }
    }
}
