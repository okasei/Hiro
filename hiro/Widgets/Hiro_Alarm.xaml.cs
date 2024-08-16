using Hiro.Helpers;
using Hiro.ModelViews;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Windows.UI.Composition;

namespace Hiro
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
        internal WindowAccentCompositor? compositor = null;
        public Hiro_Alarm(int iid, string? CustomedTitle = null, string? CustomedContnet = null, int OneButtonOnly = 0)
        {
            InitializeComponent();
            Helpers.HUI.SetCustomWindowIcon(this);
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
            if (CustomedTitle != null)
            {
                if (CustomedTitle.ToLower().StartsWith("http://") || CustomedTitle.ToLower().StartsWith("https://"))
                {
                    ala_title.Content = HText.Get_Translate("msgload");
                    BackgroundWorker bw = new();
                    bw.DoWork += delegate
                    {
                        CustomedTitle = HNet.GetWebContent(CustomedTitle);
                    };
                    bw.RunWorkerAsync();
                    bw.RunWorkerCompleted += delegate
                    {
                        ala_title.Content = CustomedTitle;
                        Title = HText.Get_Translate("alarmTitle").Replace("%t", ala_title.Content.ToString()).Replace("%a", App.appTitle);
                        if (HSet.Read_DCIni("Ani", "2").Equals("1"))
                        {
                            Storyboard sb = new();
                            HAnimation.AddPowerAnimation(1, ala_title, sb, -50, null);
                            sb.Begin();
                        }
                    };
                }
                else
                    ala_title.Content = CustomedTitle;
            }
            else
                ala_title.Content = HText.Get_Translate("alarmtitle");
            Title = HText.Get_Translate("alarmTitle").Replace("%t", ala_title.Content.ToString()).Replace("%a", App.appTitle);
            if (CustomedContnet != null)
            {
                if (CustomedContnet.ToLower().StartsWith("http://") || CustomedContnet.ToLower().StartsWith("https://"))
                {
                    sv.Content = HText.Get_Translate("msgload");
                    BackgroundWorker bw = new();
                    bw.DoWork += delegate
                    {
                        CustomedContnet = HNet.GetWebContent(CustomedContnet);
                    };
                    bw.RunWorkerAsync();
                    bw.RunWorkerCompleted += delegate
                    {
                        CustomedContnet = CustomedContnet.Replace("\\n", Environment.NewLine).Replace("<br>", Environment.NewLine);
                        sv.Content = CustomedContnet;
                        if (HSet.Read_DCIni("Ani", "2").Equals("1"))
                        {
                            Storyboard sb = new();
                            HAnimation.AddPowerAnimation(1, content, sb, -50, null);
                            sb.Begin();
                        }
                    };
                }
                else
                {
                    CustomedContnet = CustomedContnet.Replace("\\n", Environment.NewLine).Replace("<br>", Environment.NewLine);
                    sv.Content = CustomedContnet;
                }
            }
            id = iid;
            Hiro_Utils.SetShadow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
            if (!App.dflag)
                return;
            HLogger.LogtoFile("[ALARM]Title: " + sv.Content);
            var con = content.Content?.ToString();
            if (con != null)
                HLogger.LogtoFile("[ALARM]Content: " + con.Replace(Environment.NewLine, "\\n"));
        }
        public void HiHiro()
        {
            Loadbgi(Hiro_Utils.ConvertInt(HSet.Read_DCIni("Blur", "0")));
            if (HSet.Read_DCIni("Ani", "2").Equals("1"))
            {
                Storyboard sb = new();
                HAnimation.AddPowerAnimation(1, ala_title, sb, -50, null);
                HAnimation.AddPowerAnimation(1, content, sb, -50, null);
                HAnimation.AddPowerAnimation(3, albtn_1, sb, -50, null);
                if (albtn_2.Visibility == Visibility.Visible)
                    HAnimation.AddPowerAnimation(3, albtn_2, sb, -50, null);
                if (albtn_3.Visibility == Visibility.Visible)
                    HAnimation.AddPowerAnimation(3, albtn_3, sb, -50, null);
                sb.Begin();
            }
            Hiro_Utils.SetWindowToForegroundWithAttachThreadInput(this);
        }

        public void Load_Position(int OneButtonOnly = 0)
        {
            if (OneButtonOnly == 1)
            {
                albtn_1.Content = HText.Get_Translate("alarmone");
                albtn_2.Visibility = Visibility.Hidden;
                albtn_3.Visibility = Visibility.Hidden;
                HUI.Set_Control_Location(albtn_1, "alarmone", bottom: true, right: true);
            }
            else if (OneButtonOnly == 2)
            {
                albtn_1.Content = HText.Get_Translate("updateok");
                albtn_3.Content = HText.Get_Translate("updateskip");
                albtn_2.Visibility = Visibility.Hidden;
                HUI.Set_Control_Location(albtn_1, "updateok", bottom: true, right: true);
                HUI.Set_Control_Location(albtn_3, "updateskip", bottom: true, right: true);
            }
            else
            {
                albtn_1.Content = HText.Get_Translate("alarmok");
                albtn_2.Content = HText.Get_Translate("alarmdelete");
                albtn_3.Content = HText.Get_Translate("alarmdelay");
                HUI.Set_Control_Location(albtn_1, "alarmok", bottom: true, right: true);
                HUI.Set_Control_Location(albtn_2, "alarmdelete", bottom: true, right: true);
                HUI.Set_Control_Location(albtn_3, "alarmdelay", bottom: true, right: true);
            }
            HUI.Set_Control_Location(ala_title, "alarmtitle");
            HUI.Set_Control_Location(content, "alarmcontent");
            content.Height = Height - albtn_1.Margin.Bottom * 3 - albtn_1.Height - content.Margin.Top;
            content.Width = Width - content.Margin.Left * 2;
            sv.FontFamily = content.FontFamily;
            sv.FontSize = content.FontSize;
            sv.Height = content.Height - SystemParameters.HorizontalScrollBarHeight;
            sv.Width = content.Width - SystemParameters.VerticalScrollBarWidth;
        }

        public void Loadbgi(int direction)
        {
            if (HSet.Read_DCIni("Background", "1").Equals("3"))
            {
                compositor ??= new(this);
                HUI.Set_Acrylic(bgimage, this, windowChrome, compositor);
                return;
            }
            if (compositor != null)
            {
                compositor.IsEnabled = false;
            }
            if (bflag == 1)
                return;
            bflag = 1;
            HUI.Set_Bgimage(bgimage, this, null, null, windowChrome, compositor);
            bool animation = !HSet.Read_DCIni("Ani", "2").Equals("0");
            HAnimation.Blur_Animation(direction, animation, bgimage, this);
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
            var source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source?.AddHook(WndProc);
            WindowStyle = WindowStyle.None;
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
            switch (id)
            {
                case -415:
                    {
                        if (url != null)
                            Hiro_Utils.RunExe(url);
                        break;
                    }
                case > -1:
                    {
                        Hiro_Utils.OK_Alarm(App.aw[id].id);
                        App.aw.RemoveAt(id);
                        while (id < App.aw.Count)
                        {
                            App.aw[id].win.id--;
                            id++;
                        }
                        id = -1;
                        break;
                    }
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

        private void VirtualTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }
    }
}
