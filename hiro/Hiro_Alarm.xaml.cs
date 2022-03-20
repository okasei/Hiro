using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
{
    /// <summary>
    /// alarm.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Alarm : Window
    {
        internal int id = -1;
        internal int flag = -1;
        internal int aflag = -1;
        internal int bflag = 0;
        internal string? url = null;
        public Hiro_Alarm(int iid, string? CustomedTitle = null, string? CustomedContnet = null, int OneButtonOnly = 0)
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            Width = SystemParameters.PrimaryScreenWidth * 5 / 8;
            Height = SystemParameters.PrimaryScreenHeight * 5 / 8;
            Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth * 3 / 16);
            Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight * 3 / 16);
            Load_Colors();
            Load_Position(OneButtonOnly);
            Loaded += delegate
            {
                HiHiro();
            };
            ala_title.Content = CustomedTitle != null ? CustomedTitle : Hiro_Utils.Get_Transalte("alarmtitle");
            Title = ala_title.Content + " - " + App.AppTitle;
            if (CustomedContnet != null)
                sv.Content = CustomedContnet;
            id = iid;
            Hiro_Utils.SetShadow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
            if (App.dflag)
            {
                Hiro_Utils.LogtoFile("[ALARM]Title: " + ala_title.Content);
                if (content.Content != null)
                {
                    var con = content.Content.ToString();
                    if (con != null)
                        Hiro_Utils.LogtoFile("[ALARM]Content: " + con.Replace(Environment.NewLine, "\\n"));
                }
            }
        }

        public void HiHiro()
        {
            Loadbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                Storyboard sb = new();
                Hiro_Utils.AddPowerAnimation(1, ala_title, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(1, content, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(1, albtn_1, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(1, albtn_2, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(1, albtn_3, sb, 50, null);
                sb.Begin();
            }
            Hiro_Utils.SetWindowToForegroundWithAttachThreadInput(this);
        }

        public void Load_Position(int OneButtonOnly = 0)
        {
            if (OneButtonOnly == 1)
            {
                albtn_1.Content = Hiro_Utils.Get_Transalte("alarmone");
                albtn_2.Visibility = Visibility.Hidden;
                albtn_3.Visibility = Visibility.Hidden;
                Hiro_Utils.Set_Control_Location(albtn_1, "alarmone", bottom: true, right: true);
            }
            else if(OneButtonOnly == 2)
            {
                albtn_1.Content = Hiro_Utils.Get_Transalte("updateok");
                albtn_3.Content = Hiro_Utils.Get_Transalte("updateskip");
                albtn_2.Visibility = Visibility.Hidden;
                Hiro_Utils.Set_Control_Location(albtn_1, "updateok", bottom: true, right: true);
                Hiro_Utils.Set_Control_Location(albtn_3, "updateskip", bottom: true, right: true);
            }
            else
            {
                albtn_1.Content = Hiro_Utils.Get_Transalte("alarmok");
                albtn_2.Content = Hiro_Utils.Get_Transalte("alarmdelete");
                albtn_3.Content = Hiro_Utils.Get_Transalte("alarmdelay");
                Hiro_Utils.Set_Control_Location(albtn_1, "alarmok", bottom: true, right: true);
                Hiro_Utils.Set_Control_Location(albtn_2, "alarmdelete", bottom: true, right: true);
                Hiro_Utils.Set_Control_Location(albtn_3, "alarmdelay", bottom: true, right: true);
            }
            Hiro_Utils.Set_Control_Location(ala_title, "alarmtitle");
            Hiro_Utils.Set_Control_Location(content, "alarmcontent");
            content.Height = Height - albtn_1.Margin.Bottom * 3 - albtn_1.Height - content.Margin.Top;
            content.Width = Width - content.Margin.Left * 2;
            sv.FontFamily = content.FontFamily;
            sv.FontSize = content.FontSize;
            sv.Height = content.Height - SystemParameters.HorizontalScrollBarHeight;
            sv.Width = content.Width - SystemParameters.VerticalScrollBarWidth;
        }

        public void Loadbgi(int direction)
        {
            if (bflag == 1)
                return;
            bflag = 1;
            Hiro_Utils.Set_Bgimage(bgimage);
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            Hiro_Utils.Blur_Animation(direction, animation, bgimage, this);
            bflag = 0;
        }
        public void Load_Colors()
        { 
            ala_title.Foreground = new SolidColorBrush(App.AppForeColor);
            content.Foreground = new SolidColorBrush(App.AppForeColor);
            albtn_1.Background = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
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
                case 0x0083:
                    handled = true;
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;

        }

        private void Albtn_1_Click(object sender, RoutedEventArgs e)
        {
            if (id == -415)
            {
                if (url != null)
                    Hiro_Utils.RunExe(url);
            }
            else if (id > -1)
            {
                Hiro_Utils.OK_Alarm(App.aw[id].id);
                App.aw.RemoveAt(id);
                while (id < App.aw.Count)
                {
                    App.aw[id].win.id--;
                    id++;
                }
                id = -1;
            }    
            flag = 0;
            Close();
        }

        private void Albtn_2_Click(object sender, RoutedEventArgs e)
        {
            if (App.scheduleitems.Count != 0 && id > -1 && App.aw[id].id < App.scheduleitems.Count)
            {
                Hiro_Utils.Delete_Alarm(App.aw[id].id);
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
            Close();

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
                Hiro_Utils.Delay_Alarm(App.aw[id].id);
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
            Close();
        }

        private void Alarmgrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }

        private void Ala_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }
    }
}
