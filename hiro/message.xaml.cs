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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace hiro
{
    /// <summary>
    /// message.xaml の相互作用ロジック
    /// </summary>
    public partial class message : Window
    {
        internal Background? bg = null;
        internal BackgroundWorker? accept = null;
        internal BackgroundWorker? reject = null;
        internal BackgroundWorker? cancel = null;
        internal String? toolstr = null;
        internal int aflag = -1;
        internal int bflag = 0;
        public message()
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            Loaded += delegate
            {
                loadbgi();
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
            if (toolstr != null)
            {
                if (utils.Read_Ini(toolstr, "location", "width", "-1").Equals("-1"))
                {
                    Width = SystemParameters.PrimaryScreenWidth;
                }
                else
                {
                    Width = double.Parse(utils.Read_Ini(toolstr, "location", "width", "-1"));
                }
            }
            else
                Width = SystemParameters.PrimaryScreenWidth;
            if (toolstr != null)
                Height = double.Parse(utils.Read_Ini(toolstr, "location", "height", "200"));
            else
                Height = 200.0;
            Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - Width / 2);
            Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - Height / 2);
            utils.Set_Control_Location(backtitle, "title", extra: true, path: toolstr);
            utils.Set_Control_Location(cancelbtn, "cancel", extra: true, bottom: true, right: true, path: toolstr);
            utils.Set_Control_Location(rejectbtn, "reject", extra: true, bottom: true, right: true, path: toolstr);
            utils.Set_Control_Location(acceptbtn, "accept", extra: true, bottom: true, right: true, path: toolstr);
            backcontent.Height = Height - backtitle.Height - backtitle.Margin.Top * 2 - acceptbtn.Margin.Bottom * 2 - acceptbtn.Height;
            utils.Set_Control_Location(backcontent, "content", extra: true, path: toolstr);
            backcontent.Width = Width - backcontent.Margin.Left * 2;
            sv.FontFamily = backcontent.FontFamily;
            sv.FontSize = backcontent.FontSize;
            sv.Height = backcontent.Height - SystemParameters.HorizontalScrollBarHeight;
            sv.Width = backcontent.Width - SystemParameters.VerticalScrollBarWidth;
            borderlabel.BorderThickness = new Thickness(5, 5, 5, 5);
            Thickness th = borderlabel.Margin;
            th.Left = 0;
            th.Top = 0;
            borderlabel.Margin = th;
            borderlabel.Width = Width;
            borderlabel.Height = Height;
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
        public void loadbgi()
        {
            if (bflag == 1)
                return;
            bflag = 1;
            if (App.mn != null)
            {
                var animation = true;
                if (utils.Read_Ini(App.dconfig, "Configuration", "ani", "1").Equals("0"))
                    animation = false;
                else
                    animation = true;
                bgimage.Background = App.mn.bgimage.Background;
                if (utils.Read_Ini(App.dconfig, "Configuration", "blur", "0").Equals("1"))
                {
                    utils.Blur_Animation(true, animation, bgimage, this);
                }
                else
                {
                    utils.Blur_Animation(false, animation, bgimage, this);
                }
            }
            else
            {
                Thickness tn = bgimage.Margin;
                tn.Left = 0.0;
                tn.Top = 0.0;
                bgimage.Margin = tn;
                bgimage.Width = Width;
                bgimage.Height = Height;
                bgimage.Background = new SolidColorBrush(App.AppAccentColor);
            }
            bflag = 0;
        }

        private void Acceptbtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.dflag)
                utils.LogtoFile("[MESSAGE]Accept Button Clicked");
            if (accept != null)
                accept.RunWorkerAsync();
            this.Close();
            if (bg != null)
                bg.Close();
        }

        private void Rejectbtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.dflag)
                utils.LogtoFile("[MESSAGE]Reject Button Clicked");
            if (reject != null)
                reject.RunWorkerAsync();
            this.Close();
            if (bg != null)
                bg.Close();
        }

        private void Cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.dflag)
                utils.LogtoFile("[MESSAGE]Cancel Button Clicked");
            if (cancel != null)
                cancel.RunWorkerAsync();
            this.Close();
            if (bg != null)
                bg.Close();
        }

        private void Msg_Closing(object sender, CancelEventArgs e)
        {
            if (bg != null)
                bg.Close();
            if (toolstr != null && utils.Read_Ini(toolstr, "Action", "Delete", "false").ToLower().Equals("true"))
            {
                System.IO.File.Delete(toolstr);
            }
        }

        private void msg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (bg == null)
            {
                utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
            }
        }

    }
}
