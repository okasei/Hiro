using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace hiro
{
    /// <summary>
    /// message.xaml の相互作用ロジック
    /// </summary>
    public partial class Message : Window
    {
        internal Background? bg = null;
        internal String? toolstr = null;
        internal int aflag = -1;
        internal int bflag = 0;
        public Message()
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            Loaded += delegate
            {
                Loadbgi(utils.ConvertInt(utils.Read_Ini(App.dconfig, "config", "blur", "0")));
                if (App.dflag)
                {
                    utils.LogtoFile("[MESSAGE]Title: " + backtitle.Content);
                    utils.LogtoFile("[MESSAGE]Content: " + backcontent.Content);
                }
            };
            utils.SetShadow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
        }
        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;
            System.Windows.Interop.HwndSource source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source.AddHook(WndProc);
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

        public void Load_Position()
        {
            if (bg != null)
            {
                bg.Show();
            }
            Width = (toolstr != null && System.IO.File.Exists(toolstr) && !utils.Read_Ini(toolstr, "location", "width", "-1").Equals("-1")) ? double.Parse(utils.Read_Ini(toolstr, "location", "width", "-1")) : SystemParameters.PrimaryScreenWidth;
            Height = (toolstr != null && System.IO.File.Exists(toolstr) && !utils.Read_Ini(toolstr, "location", "width", "-1").Equals("-1")) ? double.Parse(utils.Read_Ini(toolstr, "location", "height", "-1")) : 200;
            if (toolstr != null && System.IO.File.Exists(toolstr))
            {
                utils.Set_Control_Location(backtitle, "title", extra: true, path: toolstr);
                utils.Set_Control_Location(cancelbtn, "cancel", extra: true, bottom: true, right: true, path: toolstr);
                utils.Set_Control_Location(rejectbtn, "reject", extra: true, bottom: true, right: true, path: toolstr);
                utils.Set_Control_Location(acceptbtn, "accept", extra: true, bottom: true, right: true, path: toolstr);
                backcontent.Height = Height - backtitle.Height - backtitle.Margin.Top * 2 - acceptbtn.Margin.Bottom * 2 - acceptbtn.Height;
                var le = backcontent.Width;
                utils.Set_Control_Location(backcontent, "content", extra: true, path: toolstr);
                if (le == backcontent.Width)
                    backcontent.Width = Width - backcontent.Margin.Left * 2;
                sv.FontFamily = backcontent.FontFamily;
                sv.FontSize = backcontent.FontSize;
                sv.Height = backcontent.Height - SystemParameters.HorizontalScrollBarHeight;
                sv.Width = backcontent.Width - SystemParameters.VerticalScrollBarWidth;
            }
            Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - Width / 2);
            Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - Height / 2);
            Load_Colors();
        }

        public void Load_Colors()
        {
            borderlabel.BorderBrush = new SolidColorBrush(App.AppForeColor);
            backtitle.Foreground = new SolidColorBrush(App.AppForeColor);
            backcontent.Foreground = new SolidColorBrush(App.AppForeColor);
            acceptbtn.Foreground = new SolidColorBrush(App.AppForeColor);
            rejectbtn.Foreground = new SolidColorBrush(App.AppForeColor);
            cancelbtn.Foreground = new SolidColorBrush(App.AppForeColor);
            acceptbtn.Background = new SolidColorBrush(Color.FromArgb(160, App.AppAccentColor.R, App.AppAccentColor.G, App.AppAccentColor.B));
            rejectbtn.Background = acceptbtn.Background;
            cancelbtn.Background = acceptbtn.Background;
            acceptbtn.BorderBrush = new SolidColorBrush(App.AppForeColor);
            rejectbtn.BorderBrush = new SolidColorBrush(App.AppForeColor);
            cancelbtn.BorderBrush = new SolidColorBrush(App.AppForeColor);
        }
        public void Loadbgi(int direction)
        {
            if (bflag == 1)
                return;
            bflag = 1;
            if (toolstr != null && System.IO.File.Exists(utils.Path_Prepare_EX(utils.Path_Prepare(utils.Read_Ini(toolstr, "Message", "Background", "")))))
                utils.Set_Bgimage(bgimage, utils.Path_Prepare_EX(utils.Path_Prepare(utils.Read_Ini(toolstr, "Message", "Background", ""))));
            else
                utils.Set_Bgimage(bgimage);
            bool animation = !utils.Read_Ini(App.dconfig, "config", "ani", "1").Equals("0");
            utils.Blur_Animation(direction, animation, bgimage, this);
            bflag = 0;
        }

        private void Acceptbtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.dflag)
                utils.LogtoFile("[MESSAGE]Accept Button Clicked");
            if (toolstr != null)
                utils.RunExe(utils.Read_Ini(toolstr, "Action", "accept", "nop"));
            if (bg != null)
                bg.Fade_Out();
            Close();
        }

        private void Rejectbtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.dflag)
                utils.LogtoFile("[MESSAGE]Reject Button Clicked");
            if (toolstr != null)
                utils.RunExe(utils.Read_Ini(toolstr, "Action", "reject", "nop"));
            if (bg != null)
                bg.Fade_Out();
            Close();
        }

        private void Cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.dflag)
                utils.LogtoFile("[MESSAGE]Cancel Button Clicked");
            if (toolstr != null)
                utils.RunExe(utils.Read_Ini(toolstr, "Action", "cancel", "nop"));
            if (bg != null)
                bg.Fade_Out();
            Close();
        }

        private void Msg_Closing(object sender, CancelEventArgs e)
        {
            if (bg != null)
                bg.Fade_Out();
            if (toolstr != null && utils.Read_Ini(toolstr, "Action", "Delete", "false").ToLower().Equals("true"))
            {
                System.IO.File.Delete(toolstr);
            }
        }

        private void Msg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (bg == null)
            {
                utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
            }
        }

    }
}
