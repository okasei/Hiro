using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Hiro.ModelViews;

namespace Hiro
{
    /// <summary>
    /// notification.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Notification : Window
    {
        internal string msg;
        internal DispatcherTimer timer;
        internal string appName = "Hiro";
        internal int[] flag = { 0, 0, 0 };//0=进入,1=显示,2=退出//loop//loops
        internal double i = 0.01;
        internal double ai = 0.01;
        internal bool[] animation = { true, false };
        internal Storyboard? sb = null;
        internal Action? act = null;
        internal WindowAccentCompositor? compositor = null;
        public Hiro_Notification()
        {
            InitializeComponent();
            Helpers.Hiro_UI.SetCustomWindowIcon(this);
            Title = $"{Hiro_Utils.Get_Translate("notitle")} - {App.appTitle}";
            Load_Color();
            Hiro_Utils.Set_Control_Location(notinfo, "notify");
            Width = SystemParameters.PrimaryScreenWidth;
            Height = 32;
            timer = new()
            {
                Interval = new TimeSpan(100000)
            };
            timer.Tick += delegate
            {
                TimerTick();
            };
            Loaded += delegate
            {
                Visibility = Visibility.Visible;
                Canvas.SetTop(this, -ActualHeight);
                Canvas.SetLeft(this, 0);
                if (timer != null)
                    timer.Start();
            };
            msg = "";
            animation[0] = !Hiro_Utils.Read_DCIni("Ani", "2").Equals("0");

        }
        private void TimerTick()
        {
            switch (flag[0])
            {
                case 0:
                    timer.Interval = new TimeSpan(10000000);
                    if (animation[0])
                    {
                        timer.Stop();
                        DoubleAnimation dou = new(-ActualHeight, 0, TimeSpan.FromMilliseconds(350));
                        dou.DecelerationRatio = 0.8;
                        dou.FillBehavior = FillBehavior.Stop;
                        dou.Completed += delegate
                        {
                            flag[0] = 1;
                            timer.Start();
                            Canvas.SetTop(this, 0);
                        };
                        BeginAnimation(TopProperty, dou);
                    }
                    else
                    {
                        Canvas.SetTop(this, 0);
                        flag[0] = 1;
                    }
                    break;
                case 1:
                    if (animation[1])
                        break;
                    flag[1]--;
                    if (flag[1] <= 0)
                    {
                        Next_Msg();
                        Load_Noti_Position();
                    }
                    if (flag[1] <= 0)
                    {
                        flag[0] = 2;
                        i = 1;
                        ai = 0.01;
                        timer.Interval = new TimeSpan(10000);
                    }
                    break;
                case 2:
                    timer.Stop();
                    if (animation[0])
                    {
                        DoubleAnimation dou = new(0, -ActualHeight, TimeSpan.FromMilliseconds(200));
                        dou.DecelerationRatio = 0.8;
                        dou.FillBehavior = FillBehavior.Stop;
                        dou.Completed += delegate
                        {
                            Canvas.SetTop(this, -ActualHeight);
                            App.noti = null;
                            Close();
                        };
                        BeginAnimation(TopProperty, dou);
                    }
                    else
                    {
                        App.noti = null;
                        Close();
                    }
                    break;
                default:
                    break;


            }
        }
        public void Load_Color()
        {
            if (Hiro_Utils.Read_DCIni("Background", "1").Equals("3"))
            {
                compositor ??= new(this);
                Hiro_Utils.Set_Acrylic(null, this, null, compositor);
                notinfo.Foreground = new SolidColorBrush(App.AppForeColor);
                return;
            }
            if (compositor != null)
            {
                compositor.IsEnabled = false;
            }
            notinfo.Foreground = new SolidColorBrush(App.AppForeColor);
            if (!Hiro_Utils.Read_DCIni("Ani", "2").Equals("0"))
            {
                Storyboard? sc = new();
                Hiro_Utils.AddColorAnimaton(App.AppAccentColor, 150, this, "Background.Color", sc);
                sc.Completed += delegate
                {
                    Background = new SolidColorBrush(App.AppAccentColor);
                    sc = null;
                };
                sc.Begin();
            }
            else
            {
                Background = new SolidColorBrush(App.AppAccentColor);
            }
        }

        private void Next_Msg()
        {
            if (msg.Equals(string.Empty))
            {
                if (App.noticeitems.Count != 0)
                {
                    msg = App.noticeitems[0].msg;
                    appName = App.noticeitems[0].title;
                    flag[1] = App.noticeitems[0].time;
                    flag[2] = flag[1];
                    act = App.noticeitems[0].act;
                    if (act != null)
                        notinfo.Cursor = Cursors.Hand;
                    else
                        notinfo.Cursor = null;
                    App.noticeitems.RemoveAt(0);
                }
                else
                {
                    if (flag[0] == 2)
                        return;
                    flag[0] = 2;
                    i = 1;
                    ai = 0.01;
                    timer.Interval = new TimeSpan(10000);
                }
            }
            if (msg.IndexOf("\\n") != -1)
            {
                var newmsg = "";
                while (newmsg.Trim().Equals("") && msg.IndexOf("\\n") != -1)
                {
                    newmsg = $"[{appName}] {msg[..msg.IndexOf("\\n")]}";
                    if (!notinfo.Content.Equals(newmsg))
                    {
                        notinfo.Content = newmsg;
                        Ani();
                    }
                    msg = msg[(newmsg.Length + 2)..];
                    flag[1] = flag[2];
                }
                if (newmsg.Equals("") && msg.Equals(""))
                    return;

            }
            else if (!msg.Equals(""))
            {
                if (!notinfo.Content.Equals($"[{appName}] {msg}"))
                {
                    notinfo.Content = $"[{appName}] {msg}";
                    Ani();
                }
                msg = "";
                flag[1] = flag[2];
            }
        }
        private void Load_Noti_Position(bool first = false)
        {
            Size msize = new();
            Hiro_Utils.Get_Text_Visual_Width(notinfo, VisualTreeHelper.GetDpi(this).PixelsPerDip, out msize);
            var th = notinfo.Margin;
            notinfo.Width = msize.Width;
            th.Left = ActualWidth / 2 - msize.Width / 2;
            if (th.Left < 0)
            {
                if (!first)
                {
                    th.Left = Width - msize.Width;
                    animation[1] = true;
                    double time = (notinfo.Content != null) ? ((string)notinfo.Content).Length * 50 : 3000;
                    sb = new();
                    sb = Hiro_Utils.AddThicknessAnimaton(th, time, notinfo, "Margin", sb, new(Width, th.Top, th.Right, th.Bottom), 0);
                    sb.Completed += delegate
                    {
                        sb = null;
                        notinfo.Margin = th;
                        animation[1] = false;
                    };
                    sb.Begin();
                }
                else
                {
                    notinfo.Margin = new(Width, th.Top, th.Right, th.Bottom);
                }
            }
            else
            {
                if (sb != null)
                {
                    sb.Stop();
                    notinfo.Margin = th;
                    animation[1] = false;
                }
                notinfo.Margin = th;
            }
        }
        private void Ani()
        {
            Load_Noti_Position();
            Hiro_Utils.Blur_Out(notinfo);
        }
        private void Noti_Loaded(object sender, RoutedEventArgs e)
        {
            notinfo.Content = "[" + App.noticeitems[0].title + "] " + (App.noticeitems[0].msg.Contains("\\n") ? App.noticeitems[0].msg[..App.noticeitems[0].msg.IndexOf("\\n")] : App.noticeitems[0].msg);
            Load_Noti_Position(true);
        }

        private void Noti_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            timer.Stop();
            Next_Msg();
            timer.Start();
        }

        private void Notinfo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            timer.Stop();
            Next_Msg();
            timer.Start();
        }

        private void Noti_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.noti = null;
        }

        private void Notinfo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (act != null)
                act.Invoke();
            act = null;
            notinfo.Cursor = null;
        }
    }
}
