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

namespace hiro
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
            LoadTimer();
            HiHiro();
        }

        public Hiro_Splash(string filePath)
        {
            InitializeComponent();
            ala_title.Content = Hiro_Utils.Path_Prepare(Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Read_Ini(filePath, "Config", "Title", "<???>")));
            sv.Content = Hiro_Utils.Path_Prepare(Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Read_Ini(filePath, "Config", "Content", "<???>")));
            loading.Content = Hiro_Utils.Path_Prepare(Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Read_Ini(filePath, "Config", "Loading", "<???>")));
            backgroundFIle = Hiro_Utils.Path_Prepare(Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Read_Ini(filePath, "Config", "Background", "<???>")));
            int.TryParse(Hiro_Utils.Read_Ini(filePath, "Config", "Tick", "-1"), out tick);
            Topmost = Hiro_Utils.Read_Ini(filePath, "Config", "Topmost", "false").Equals("true", StringComparison.CurrentCultureIgnoreCase);
            Movable = Hiro_Utils.Read_Ini(filePath, "Config", "Movable", "false").Equals("true", StringComparison.CurrentCultureIgnoreCase);
            Hiro_Utils.Set_Control_Location(ala_title, "Title", extra: true, path: filePath);
            Hiro_Utils.Set_Control_Location(sv, "Content", extra: true, path: filePath);
            Hiro_Utils.Set_Control_Location(loading, "Loading", extra: true, path: filePath);
            var w = 800;
            int.TryParse(Hiro_Utils.Read_Ini(filePath, "Config", "Width", "800"), out w);
            w = w < 100 ? 100 : w;
            var h = 450;
            int.TryParse(Hiro_Utils.Read_Ini(filePath, "Config", "Height", "450"), out h);
            h = h < 100 ? 100 : h;
            Width = w;
            Height = h;
            LoadTimer();
            if (tick > 0)
            {
                if (Hiro_Utils.Read_Ini(filePath, "Config", "Closable", "true").Equals("false", StringComparison.CurrentCultureIgnoreCase))
                    closebtn.Visibility = Visibility.Collapsed;
            }
            next = Hiro_Utils.Read_Ini(filePath, "Config", "Next", "");
            HiHiro();
            Loadbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
        }

        public Hiro_Splash(string title, string content, string loading, string background, int tick, bool topmost, bool movable, bool closable, string nextCMD)
        {
            InitializeComponent();
            ala_title.Content = Hiro_Utils.Path_Prepare(Hiro_Utils.Path_Prepare_EX(title));
            sv.Content = Hiro_Utils.Path_Prepare(Hiro_Utils.Path_Prepare_EX(content));
            this.loading.Content = Hiro_Utils.Path_Prepare(Hiro_Utils.Path_Prepare_EX(loading));
            backgroundFIle = Hiro_Utils.Path_Prepare(Hiro_Utils.Path_Prepare_EX(background));
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
            Loadbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 80));
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        public void HiHiro()
        {
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            if (animation)
            {
                Storyboard sb = new();
                Hiro_Utils.AddPowerAnimation(0, ala_title, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(0, content, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(0, loading, sb, -50, null);
                if (closebtn.Visibility == Visibility.Visible)
                {
                    Hiro_Utils.AddPowerAnimation(2, closebtn, sb, -50, null);
                }
                if (minbtn.Visibility == Visibility.Visible)
                {
                    Hiro_Utils.AddPowerAnimation(2, minbtn, sb, -50, null);
                }
                sb.Begin();
            }
            Loadbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
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
            Title = ala_title.Content.ToString() + " - " + App.AppTitle;
            Load_Color();
            ContentRendered += (e, args) =>
            {
                ProgressLabel.Width = ActualWidth;
            };
            closebtn.ToolTip = Hiro_Utils.Get_Translate("close");
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
                    bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
                    var t = new Thickness(ActualWidth - tick * ActualWidth / (int)ProgressLabel.Tag, 0, 0, 0);
                    if (animation)
                    {
                        var sb = Hiro_Utils.AddThicknessAnimaton(t, 1000, ProgressLabel, "Margin", null, null, 1);
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
                Hiro_Utils.RunExe(next, App.AppTitle);
            }
        }

        public void Loadbgi(int direction)
        {
            if (bflag == 1 || (System.IO.File.Exists(backgroundFIle) && bflag != 2))
                return;
            bflag = 1;
            if (System.IO.File.Exists(backgroundFIle))
                Hiro_Utils.Set_Bgimage(bgimage, this, backgroundFIle);
            else
                Hiro_Utils.Set_Bgimage(bgimage, this);
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            Hiro_Utils.Blur_Animation(direction, animation, bgimage, this);
            bflag = 0;
        }
    }
}
