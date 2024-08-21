using System;
using System.Windows;
using System.Windows.Threading;

namespace Hiro
{
    /// <summary>
    /// Hiro_Timer.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Timer : Window
    {
        internal DispatcherTimer? dt = null;
        internal string timeFomrat = "HH   mm   ss";
        internal string timeFomratTick = "HH : mm : ss";
        internal int flag = 0;
        public Hiro_Timer()
        {
            InitializeComponent();
            Loaded += delegate
            {
                dt = new();
                dt.Interval = new TimeSpan(1000000);
                dt.Tick += delegate
                {
                    TickLabel.Content = DateTime.Now.ToString(timeFomratTick);
                    TimeLabel.Content = DateTime.Now.ToString(timeFomrat);
                    if (flag == 10)
                    {
                        TickLabel.Visibility = Visibility.Hidden; 
                        flag = 0;
                    }
                    else
                    {
                        TickLabel.Visibility = Visibility.Visible;
                        flag++;
                    }
                };
                dt.Start();
            };
            SourceInitialized += OnSourceInitialized;
        }

        private void TimeLabel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
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
                    //Console.WriteLine("Msg: " + m.Msg + ";LParam: " + m.LParam + ";WParam: " + m.WParam + ";Result: " + m.Result);
                    break;
            }
            return IntPtr.Zero;

        }
    }
}
