using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace hiro
{
    /// <summary>
    /// alarm.xaml の相互作用ロジック
    /// </summary>
    public partial class Alarm : Window
    {
        internal int id = -1;
        internal int flag = -1;
        internal int aflag = -1;
        internal int bflag = 0;
        public Alarm(int iid, bool CustomTitle = false, string CustomedTitle = "Time is Up!", bool CustomContent = false, string CustomedContnet = "Time up!", bool OneButtonOnly = false)
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            this.Width = SystemParameters.PrimaryScreenWidth * 3 / 4;
            this.Height = SystemParameters.PrimaryScreenHeight * 3 / 4;
            Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 8);
            Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 8);
            /*try
            {
                System.Media.SoundPlayer sndPlayer = new(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\Media\\Windows Notify System Generic.wav");
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
            Load_Position(OneButtonOnly);
            Loaded += delegate
            {
                Loadbgi();
            };
            if (CustomTitle)
                ala_title.Content = CustomedTitle;
            else
                ala_title.Content = utils.Get_Transalte("alarmtitle");
            Title = ala_title.Content + " - " + App.AppTitle;
            if (CustomContent)
                sv.Content = CustomedContnet;
            id = iid;
            utils.SetShadow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
            if (App.dflag)
            {
                utils.LogtoFile("[ALARM]Title: " + ala_title.Content);
                if (content.Content != null)
                {
                    var con = content.Content.ToString();
                    if (con != null)
                        utils.LogtoFile("[ALARM]Content: " + con.Replace(Environment.NewLine, "\\n"));
                }
            }
        }
            
        public void Load_Position(bool OneButtonOnly = false)
        {
            if (OneButtonOnly)
            {
                albtn_1.Content = utils.Get_Transalte("alarmone");
                albtn_2.Visibility = Visibility.Hidden;
                albtn_3.Visibility = Visibility.Hidden;
                utils.Set_Control_Location(albtn_1, "alarmone", bottom: true, right: true);
            }
            else
            {
                albtn_1.Content = utils.Get_Transalte("alarmok");
                albtn_2.Content = utils.Get_Transalte("alarmdelete");
                albtn_3.Content = utils.Get_Transalte("alarmdelay");
                utils.Set_Control_Location(albtn_1, "alarmok", bottom: true, right: true);
                utils.Set_Control_Location(albtn_2, "alarmdelete", bottom: true, right: true);
                utils.Set_Control_Location(albtn_3, "alarmdelay", bottom: true, right: true);
            }
            utils.Set_Control_Location(ala_title, "alarmtitle");
            utils.Set_Control_Location(content, "alarmcontent");
            borderlabel.BorderThickness = new Thickness(2, 2, 2, 2);
            Thickness th = borderlabel.Margin;
            th.Left = 0;
            th.Top = 0;
            borderlabel.Margin = th;
            borderlabel.Width = Width;
            borderlabel.Height = Height;
            content.Height = Height - albtn_1.Margin.Bottom * 3 - albtn_1.Height - content.Margin.Top;
            content.Width = Width - content.Margin.Left * 2;
            sv.FontFamily = content.FontFamily;
            sv.FontSize = content.FontSize;
            sv.Height = content.Height - SystemParameters.HorizontalScrollBarHeight;
            sv.Width = content.Width - SystemParameters.VerticalScrollBarWidth;
        }

        public void Loadbgi()
        {
            if (bflag == 1)
                return;
            bflag = 1;
            if (App.mn != null)
            {
                bool animation;
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
        public void Load_Colors()
        { 
            ala_title.Foreground = new SolidColorBrush(App.AppForeColor);
            content.Foreground = new SolidColorBrush(App.AppForeColor);
            albtn_1.Background = new SolidColorBrush(Color.FromArgb(160, App.AppAccentColor.R, App.AppAccentColor.G, App.AppAccentColor.B));
            albtn_2.Background = albtn_1.Background;
            albtn_3.Background = albtn_1.Background;
            albtn_1.Foreground = new SolidColorBrush(App.AppForeColor);
            albtn_2.Foreground = new SolidColorBrush(App.AppForeColor);
            albtn_3.Foreground = new SolidColorBrush(App.AppForeColor);
            albtn_1.BorderThickness = new Thickness(1, 1, 1, 1);
            albtn_2.BorderThickness = albtn_1.BorderThickness;
            albtn_3.BorderThickness = albtn_1.BorderThickness;
            albtn_1.BorderBrush = new SolidColorBrush(App.AppForeColor);
            albtn_2.BorderBrush = new SolidColorBrush(App.AppForeColor);
            albtn_3.BorderBrush = new SolidColorBrush(App.AppForeColor);
            borderlabel.BorderBrush = new SolidColorBrush(App.AppForeColor);
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

        private void Albtn_1_Click(object sender, RoutedEventArgs e)
        {
            if (id > -1)
            {
                utils.OK_Alarm(App.aw[id].id);
                App.aw.RemoveAt(id);
                while (id < App.aw.Count)
                {
                    App.aw[id].win.id--;
                    id++;
                }
                id = -1;
            }    
            flag = 0;
            this.Close();
        }

        private void Albtn_2_Click(object sender, RoutedEventArgs e)
        {
            if (App.scheduleitems.Count != 0 && id > -1 && App.aw[id].id < App.scheduleitems.Count)
            {
                utils.Delete_Alarm(App.aw[id].id);
                var a = 0;
                var i = App.aw[id].id;
                if (i > -1)
                {
                    while (a < App.aw.Count)
                    {
                        if (App.aw[a].id == i)
                            App.aw[a].id = -1;
                        if (App.aw[a].id > i)
                            App.aw[a].id--;
                        a++;
                    }
                }
                App.aw.RemoveAt(id);
                while (id < App.aw.Count)
                {
                    App.aw[id].win.id--;
                    id++;
                }
                id = -1;
            }
            flag = 0;
            this.Close();

        }

        private void Ala_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (flag == -1)
                e.Cancel = true;
        }

        private void Albtn_3_Click(object sender, RoutedEventArgs e)
        {
            if (App.scheduleitems.Count != 0 && id > -1 && App.aw[id].id < App.scheduleitems.Count)
            {
                utils.Delay_Alarm(App.aw[id].id);
            }
            if (id > -1)
            {
                App.aw.RemoveAt(id);
                while (id < App.aw.Count)
                {
                    App.aw[id].win.id--;
                    id++;
                }
                id = -1;
            }
            flag = 0;
            this.Close();
        }

        private void Alarmgrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }

        private void Ala_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }
    }
}
