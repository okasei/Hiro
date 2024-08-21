using Hiro.Helpers;
using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace Hiro
{
    /// <summary>
    /// Hiro_Splash.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Splash : Window
    {
        bool Movable = false;
        int tick = -5;
        string next = "";
        internal int bflag = 2;
        string backgroundFIle = "";
        public Hiro_Splash()
        {
            InitializeComponent();
            Helpers.HUI.SetCustomWindowIcon(this);
            LoadTimer();
            HiHiro();
            Hiro_Utils.SetShadow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
        }
        private void VirtualTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }

        public Hiro_Splash(string filePath)
        {
            InitializeComponent();
            ala_title.Content = HText.Path_PPX(HSet.Read_Ini(filePath, "Config", "Title", "<???>"));
            sv.Content = HText.Path_PPX(HSet.Read_Ini(filePath, "Config", "Content", "<???>"));
            loading.Content = HText.Path_PPX(HSet.Read_Ini(filePath, "Config", "Loading", "<???>"));
            backgroundFIle = HText.Path_PPX(HSet.Read_Ini(filePath, "Config", "Background", "<???>"));
            int.TryParse(HSet.Read_Ini(filePath, "Config", "Tick", "-1"), out tick);
            Topmost = HSet.Read_Ini(filePath, "Config", "Topmost", "false").Equals("true", StringComparison.CurrentCultureIgnoreCase);
            Movable = HSet.Read_Ini(filePath, "Config", "Movable", "false").Equals("true", StringComparison.CurrentCultureIgnoreCase);
            HUI.Set_Control_Location(ala_title, "Title", extra: true, path: filePath);
            HUI.Set_Control_Location(sv, "Content", extra: true, path: filePath);
            HUI.Set_Control_Location(loading, "Loading", extra: true, path: filePath);
            var w = 800;
            int.TryParse(HSet.Read_Ini(filePath, "Config", "Width", "800"), out w);
            w = w < 100 ? 100 : w;
            var h = 450;
            int.TryParse(HSet.Read_Ini(filePath, "Config", "Height", "450"), out h);
            h = h < 100 ? 100 : h;
            Width = w;
            Height = h;
            LoadTimer();
            if (tick > 0)
            {
                if (HSet.Read_Ini(filePath, "Config", "Closable", "true").Equals("false", StringComparison.CurrentCultureIgnoreCase))
                    closebtn.Visibility = Visibility.Collapsed;
            }
            next = HSet.Read_Ini(filePath, "Config", "Next", "");
            HiHiro();
            Loadbgi(Hiro_Utils.ConvertInt(HSet.Read_DCIni("Blur", "0")));
        }

        public Hiro_Splash(string title, string content, string loading, string background, int tick, bool topmost, bool movable, bool closable, string nextCMD)
        {
            InitializeComponent();
            ala_title.Content = HText.Path_PPX(title);
            sv.Content = HText.Path_PPX(content);
            this.loading.Content = HText.Path_PPX(loading);
            backgroundFIle = HText.Path_PPX(background);
            this.tick = tick;
            Topmost = topmost;
            Movable = movable;
            next = nextCMD;
            LoadTimer();
            if (tick > 0)
            {
                if (!closable)
                    closebtn.Visibility = Visibility.Collapsed;
            }
            HiHiro();
            Loadbgi(Hiro_Utils.ConvertInt(HSet.Read_DCIni("Blur", "0")));
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 80));
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        public void HiHiro()
        {
            bool animation = !HSet.Read_DCIni("Ani", "2").Equals("0");
            if (animation)
            {
                Storyboard sb = new();
                HAnimation.AddPowerAnimation(0, ala_title, sb, -50, null);
                HAnimation.AddPowerAnimation(0, content, sb, -50, null);
                HAnimation.AddPowerAnimation(0, loading, sb, -50, null);
                if (closebtn.Visibility == Visibility.Visible)
                {
                    HAnimation.AddPowerAnimation(2, closebtn, sb, -50, null);
                }
                if (minbtn.Visibility == Visibility.Visible)
                {
                    HAnimation.AddPowerAnimation(2, minbtn, sb, -50, null);
                }
                sb.Begin();
            }
            Loadbgi(Hiro_Utils.ConvertInt(HSet.Read_DCIni("Blur", "0")));
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
                case 0x0083:
                    handled = true;
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;

        }

        private void LoadTimer()
        {
            SourceInitialized += OnSourceInitialized;
            Title = HText.Get_Translate("splashTitle").Replace("%t", ala_title.Content.ToString()).Replace("%a", App.appTitle);
            Load_Color();
            ContentRendered += (e, args) =>
            {
                ProgressLabel.Width = ActualWidth;
            };
            closebtn.ToolTip = HText.Get_Translate("close");
            var timer = new DispatcherTimer();
            ProgressLabel.Tag = tick;
            ProgressLabel.Margin = new Thickness();
            minbtn.Visibility = tick > 0 ? Visibility.Visible : Visibility.Collapsed;
            minbtn.Content = tick.ToString();
            if (tick > 0)
            {
                Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Normal, new System.Windows.Interop.WindowInteropHelper(this).Handle);
            }
            timer.Tick += delegate
            {
                if (tick >= 0)
                {
                    tick--;
                    minbtn.Content = tick.ToString();
                    bool animation = !HSet.Read_DCIni("Ani", "2").Equals("0");
                    var t = new Thickness(ActualWidth - tick * ActualWidth / (int)ProgressLabel.Tag, 0, 0, 0);
                    if (animation)
                    {
                        var sb = HAnimation.AddThicknessAnimaton(t, 1000, ProgressLabel, "Margin", null, null, 1);
                        sb.Completed += delegate
                        {
                            ProgressLabel.Margin = t;
                            sb = null;
                        };
                        sb.Begin();
                    }
                    else
                    {
                        ProgressLabel.Margin = t;
                    }
                    Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressValue(100 - (int)(tick * 100 / (int)ProgressLabel.Tag), 100, new System.Windows.Interop.WindowInteropHelper(this).Handle);
                    if (tick < 0)
                    {
                        Close();
                    }
                }
            };
            timer.Interval = new TimeSpan(10000000);
            timer.Start();
            Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - Width / 2);
            Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - Height / 2);
        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Ala_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Movable)
                Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (next.Length != 0)
            {
                Hiro_Utils.RunExe(next, App.appTitle);
            }
        }

        public void Loadbgi(int direction)
        {
            if (bflag == 1 || (System.IO.File.Exists(backgroundFIle) && bflag != 2))
                return;
            bflag = 1;
            if (System.IO.File.Exists(backgroundFIle))
                bgimage.Background = HUI.Set_Bgimage(bgimage.Background, this, backgroundFIle);
            else
                bgimage.Background = HUI.Set_Bgimage(bgimage.Background, this);
            bool animation = !HSet.Read_DCIni("Ani", "2").Equals("0");
            HAnimation.Blur_Animation(direction, animation, bgimage, this);
            bflag = 0;
        }
    }
}
