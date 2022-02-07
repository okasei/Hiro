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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace hiro
{
    /// <summary>
    /// update.xaml の相互作用ロジック
    /// </summary>
    public partial class update : Window
    {
        internal int bflag = 0;
        internal string url = "";
        public update()
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            this.Width = SystemParameters.PrimaryScreenWidth * 3 / 4;
            this.Height = SystemParameters.PrimaryScreenHeight * 3 / 4;
            Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 8);
            Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 8);
            albtn_1.Content = utils.Get_Transalte("updateok");
            albtn_2.Content = utils.Get_Transalte("updateskip");
            utils.Set_Control_Location(update_title, "updatetitle");
            utils.Set_Control_Location(content, "updatecontent");
            utils.Set_Control_Location(albtn_1, "updateok", bottom: true, right: true);
            utils.Set_Control_Location(albtn_2, "updateskip", bottom: true, right: true);
            content.Height = Height - albtn_1.Margin.Bottom * 3 - albtn_1.Height - content.Margin.Top;
            content.Width = Width - content.Margin.Left * 2;
            sv.FontFamily = content.FontFamily;
            sv.FontSize = content.FontSize;
            sv.Height = content.Height - SystemParameters.HorizontalScrollBarHeight;
            sv.Width = content.Width - SystemParameters.VerticalScrollBarWidth;
            borderlabel.BorderThickness = new Thickness(2, 2, 2, 2);
            Thickness th = borderlabel.Margin;
            th.Left = 0;
            th.Top = 0;
            borderlabel.Margin = th;
            borderlabel.Width = Width;
            borderlabel.Height = Height;
            /*try
            {
                System.Media.SoundPlayer sndPlayer = new(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\Media\\Windows Notify Messaging.wav");
                //循环播放
                // sndPlayer.PlayLooping();
                //播放一次
                sndPlayer.Play();
            }
            catch (Exception ex)
            {
                utils.LogtoFile("[ERROR]" + ex.Message);
            }*/
            Load_Colors();
            Loaded += delegate
            {
                Loadbgi();
                if (App.dflag)
                {
                    utils.LogtoFile("[UPDATE]Title: " + update_title.Content);
                    if (content.Content != null)
                    {
                        var con = content.Content.ToString();
                        if (con != null)
                            utils.LogtoFile("[UPDATE]Content: " + con.Replace(Environment.NewLine, "\\n"));
                    }
                        
                }
            };
            update_title.MouseDoubleClick += delegate
            {
                Clipboard.SetText(update_title.Content.ToString());
                App.Notify(new noticeitem(utils.Get_Transalte("copy"), 2));
            };
            content.MouseDoubleClick += delegate
            {
                Clipboard.SetText(content.Content.ToString());
                App.Notify(new noticeitem(utils.Get_Transalte("copy"), 2));
            };
            utils.SetShadow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
        }
        public void Loadbgi()
        {
            if (bflag == 1)
                return;
            bflag = 1;
            utils.Set_Bgimage(bgimage);
            bool animation = !utils.Read_Ini(App.dconfig, "Configuration", "ani", "1").Equals("0");
            utils.Blur_Animation(utils.Read_Ini(App.dconfig, "Configuration", "blur", "0").Equals("1"), animation, bgimage, this);
            bflag = 0;
        }
        public void Load_Colors()
        {
            update_title.Content = utils.Get_Transalte("updatetitle");
            Title = update_title.Content + " - " + App.AppTitle;
            update_title.Foreground = new SolidColorBrush(App.AppForeColor);
            content.Foreground = new SolidColorBrush(App.AppForeColor);
            albtn_1.Background = new SolidColorBrush(Color.FromArgb(160, App.AppAccentColor.R, App.AppAccentColor.G, App.AppAccentColor.B));
            albtn_2.Background = albtn_1.Background;
            albtn_1.Foreground = new SolidColorBrush(App.AppForeColor);
            albtn_2.Foreground = new SolidColorBrush(App.AppForeColor);
            albtn_1.BorderThickness = new Thickness(1, 1, 1, 1);
            albtn_2.BorderThickness = albtn_1.BorderThickness;
            albtn_1.BorderBrush = new SolidColorBrush(App.AppForeColor);
            albtn_2.BorderBrush = new SolidColorBrush(App.AppForeColor);
            borderlabel.BorderBrush = new SolidColorBrush(App.AppForeColor);
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;
            System.Windows.Interop.HwndSource source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source.AddHook(WndProc);
            utils.LogtoFile("AddHook WndProc");
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

        private void albtn_2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void albtn_1_Click(object sender, RoutedEventArgs e)
        {
            utils.RunExe(url);
            this.Close();
        }
    }
}
