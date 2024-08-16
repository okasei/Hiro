using Hiro.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hiro
{
    /// <summary>
    /// Hiro_Lock.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Lock : Window
    {
        internal static uint CD = 0;
        internal static BackgroundWorker? Fin = null;
        private string ipwd = "";
        private string rpwd = "415417";
        private bool cflag = false;
        private bool bflag = false;
        public Hiro_Lock(BackgroundWorker Finished)
        {
            InitializeComponent();
            Helpers.HUI.SetCustomWindowIcon(this);
            SourceInitialized += OnSourceInitialized;
            Loaded += delegate
            {
                OpacityBgi();
                Load_Colors();
                Load_Position();
                Load_Translate();
                HiHiro();
                SetLockState();
                UpdatePwd(null);
            };
        }

        public void Load_Translate()
        {
            titlelabel.Content = HText.Get_Translate("locked").Replace("%h", App.appTitle);
            InfoLabel.Content = HText.Get_Translate("lockedinfo").Replace("%h", App.appTitle);
            InfoLabel2.Content = HText.Get_Translate("lockedwait").Replace("%h", App.appTitle);
            Pwd_BtnB.Content = HText.Get_Translate("lockback").Replace("%h", App.appTitle);
            Pwd_BtnE.Content = HText.Get_Translate("lockenter").Replace("%h", App.appTitle);
        }

        public void Load_Position()
        {
            HUI.Set_Control_Location(titlelabel, "locked");
            HUI.Set_Control_Location(InfoLabel, "lockedinfo");
            HUI.Set_Control_Location(InfoLabel2, "lockedwait");
            HUI.Set_FrameworkElement_Location(Pwd, "lockedpwd");
            HUI.Set_Control_Location(Pwd_Btn1, "lockedpwdbtn", location: false);
            HUI.Set_Control_Location(PwdInput, "lockedpwdinput");
        }

        public void Load_Colors()
        {
            HUI.Set_Bgimage(bgimage, this);
            #region 颜色
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 80));
            Resources["AppForeDimColor"] = Hiro_Utils.Color_Transparent(App.AppForeColor, 80);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
            Resources["InfoAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 200));
            #endregion
        }

        public void OpacityBgi()
        {
            HUI.Set_Opacity(bgimage, this);
        }

        public void SetLockState(int state = 0)
        {
            Grid tosee = state switch
            {
                1 => InputPwd,
                _ => Info
            };
            Info.Visibility = Visibility.Hidden;
            InputPwd.Visibility = Visibility.Hidden;
            tosee.Visibility = Visibility.Visible;
            if (HSet.Read_DCIni("Ani", "2").Equals("1"))
            {
                Storyboard sb = new();
                HAnimation.AddPowerAnimation(0, tosee, sb, -50, null);
                sb.Begin();
            }
        }

        private void UpdatePwd(string? a)
        {
            char cover = '\uEA3A';
            char un = '\uEA3B';
            if (a != null)
                ipwd += a;
            PwdInput.Content = rpwd.Length > ipwd.Length ? new string(un, ipwd.Length) + new string(cover, rpwd.Length - ipwd.Length) : new string(un, rpwd.Length);
            if (rpwd.Length == ipwd.Length)
            {
                if (rpwd.Equals(ipwd))
                {
                    Fin?.RunWorkerAsync();
                    TryClose();
                }
            }
        }

        private void TryClose()
        {
            if (!bflag)
            {
                cflag = true;
                bflag = true;
                Close();
            }
        }

        private void Get_Password()
        {
            /*var pw = HText.Read_DCIni("Password");
            System.Security.Cryptography.HMACSHA512 hmac = new System.Security.Cryptography.HMACSHA512();
            hmac.
                StringBuilder sBuilder = new(131);
                for (int i = 0; i < sourceString.Length; i++)
                {
                    int Unicode = sourceString[i];
                    if (Unicode >= 16)
                    {
                        sBuilder.Append(sourceString[i]);
                    }
                }
                return sBuilder.ToString();*/
        }

        private void Lock_StateChanged(object sender, EventArgs e)
        {

        }

        private void Lock_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!cflag)
            {
                e.Cancel = true;
            }
        }

        private void Titlelabel_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Bglabel_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        public void HiHiro()
        {
            titlelabel.Visibility = Visibility.Visible;
            if (HSet.Read_DCIni("Ani", "2").Equals("1"))
            {
                Storyboard sb = new();
                HAnimation.AddPowerAnimation(0, titlelabel, sb, -50, null);
                sb.Begin();
            }
            Hiro_Utils.SetWindowToForegroundWithAttachThreadInput(this);
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;
            var source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source?.AddHook(WndProc);
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
                    break;
            }
            return IntPtr.Zero;

        }

        private void InfoLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetLockState(1);
        }

        private void Pwd_BtnB_Click(object sender, RoutedEventArgs e)
        {
            ipwd = string.Empty;
            UpdatePwd(null);
            SetLockState(0);
        }

        private void Pwd_Btn1_Click(object sender, RoutedEventArgs e)
        {
            UpdatePwd("1");
        }

        private void Pwd_Btn2_Click(object sender, RoutedEventArgs e)
        {
            UpdatePwd("2");
        }

        private void Pwd_Btn3_Click(object sender, RoutedEventArgs e)
        {
            UpdatePwd("3");
        }

        private void Pwd_Btn4_Click(object sender, RoutedEventArgs e)
        {
            UpdatePwd("4");
        }

        private void Pwd_Btn5_Click(object sender, RoutedEventArgs e)
        {
            UpdatePwd("5");
        }

        private void Pwd_Btn6_Click(object sender, RoutedEventArgs e)
        {
            UpdatePwd("6");
        }

        private void Pwd_Btn7_Click(object sender, RoutedEventArgs e)
        {
            UpdatePwd("7");
        }

        private void Pwd_Btn8_Click(object sender, RoutedEventArgs e)
        {
            UpdatePwd("8");
        }

        private void Pwd_Btn9_Click(object sender, RoutedEventArgs e)
        {
            UpdatePwd("9");
        }

        private void Pwd_Btn0_Click(object sender, RoutedEventArgs e)
        {
            UpdatePwd("0");
        }

        private void Pwd_BtnE_Click(object sender, RoutedEventArgs e)
        {
            if (ipwd.Length == 1)
            {
                ipwd = string.Empty;
            }
            else if(ipwd.Length > 0)
            {
                ipwd = ipwd[..^1];
            }
            UpdatePwd(null);
        }
    }
}
