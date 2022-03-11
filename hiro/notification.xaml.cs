using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace hiro
{
    /// <summary>
    /// notification.xaml の相互作用ロジック
    /// </summary>
    public partial class Notification : Window
    {
        internal string msg;
        internal DispatcherTimer timer;
        internal int[] flag = { 0, 0, 0 };//0=进入,1=显示,2=退出//loop//loops
        internal double i = 0.01;
        internal double ai = 0.01;
        internal bool[] animation = { true, false };
        public Notification()
        {
            InitializeComponent();
            Title = utils.Get_Transalte("notitle") + " - " + App.AppTitle;
            Load_Color();
            utils.Set_Control_Location(notinfo, "notify");
            Title = App.AppTitle;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = 32;
            SetValue(Canvas.LeftProperty, 0.0);
            SetValue(Canvas.TopProperty, 0.0);
            Opacity = 0.01;
            msg = "";
            animation[0] = !utils.Read_Ini(App.dconfig, "config", "ani", "1").Equals("0");
            timer = new()
            {
                Interval = new TimeSpan(100000)
            };
            timer.Tick += delegate
            {
                TimerTick();
            };
            timer.Start();
        }
        private void TimerTick()
        {
            switch(flag[0])
            {
                case 0:
                    timer.Interval = new TimeSpan(10000000);
                    if (animation[0])
                    {
                        timer.Stop();
                        System.Windows.Media.Animation.Storyboard? sb = new();
                        sb = utils.AddDoubleAnimaton(1, 300, this, "Opacity", sb);
                        sb.Completed += delegate
                        {
                            Opacity = 1;
                            flag[0] = 1;
                            timer.Start();
                            sb = null;
                        };
                        sb.Begin();
                    }
                    else
                    {
                        Opacity = 1;
                        flag[0] = 1;
                    }
                    break;
                case 1:
                    if (animation[1])
                        break;
                    flag[1]--;
                    if (flag[1] <= 0)
                        Next_Msg();
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
                        System.Windows.Media.Animation.Storyboard? sb = new();
                        sb = utils.AddDoubleAnimaton(0, 300, this, "Opacity", sb);
                        sb.Completed += delegate
                        {
                            Opacity = 0;
                            App.noti = null;
                            Close();
                            sb = null;
                        };
                        sb.Begin();
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
            notinfo.Foreground = new SolidColorBrush(App.AppForeColor);
            if (!utils.Read_Ini(App.dconfig, "config", "ani", "1").Equals("0"))
            {
                System.Windows.Media.Animation.Storyboard? sb = new();
                utils.AddColorAnimaton(App.AppAccentColor, 150, this, "Background.Color", sb);
                sb.Completed += delegate
                {
                    Background = new SolidColorBrush(App.AppAccentColor);
                    sb = null;
                };
                sb.Begin();
            }
            else
                Background = new SolidColorBrush(App.AppAccentColor);

        }

        private void Next_Msg()
        {
            if (msg.Equals(String.Empty))
            {
                if (App.noticeitems.Count != 0)
                {
                    msg = App.noticeitems[0].msg;
                    flag[1] = App.noticeitems[0].time;
                    flag[2] = flag[1];
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
                string newmsg = "";
                while (newmsg.Trim().Equals("") && msg.IndexOf("\\n") != -1)
                {
                    newmsg = msg[..msg.IndexOf("\\n")];
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
                if (!notinfo.Content.Equals(msg))
                {
                    notinfo.Content = msg;
                    Ani();
                }
                msg = "";
                flag[1] = flag[2];
            }
        }
        private void Load_Noti_Position()
        {
            Size msize = new();
            utils.Get_Text_Visual_Width(notinfo, VisualTreeHelper.GetDpi(this).PixelsPerDip, out msize);
            Thickness th = notinfo.Margin;
            notinfo.Width = msize.Width;
            th.Left = Width / 2 - msize.Width / 2;
            if (th.Left < 0)
            {
                animation[1] = true;
                th.Left = Width - msize.Width;
                double time = (notinfo.Content != null) ? ((string)notinfo.Content).Length * 50 : 3000;
                System.Windows.Media.Animation.Storyboard? sb = new();
                sb = utils.AddThicknessAnimaton(th, time, notinfo, "Margin", sb, new(Width, th.Top, th.Right, th.Bottom));
                sb.Completed += delegate
                {
                    sb = null;
                    notinfo.Margin = th;
                    animation[1] = false;
                };
                sb.Begin();
            }
            else
                notinfo.Margin = th;
        }
        private void Ani()
        {
            Load_Noti_Position();
            utils.Blur_Out(notinfo);
        }
        private void Noti_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.noticeitems[0].msg.IndexOf("\\n") != -1)
            {
                notinfo.Content = App.noticeitems[0].msg[..App.noticeitems[0].msg.IndexOf("\\n")];
            }
            else
            {
                notinfo.Content = App.noticeitems[0].msg;
            }
            Load_Noti_Position();
        }

        private void Noti_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            flag[1] = 0;
        }

        private void Notinfo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            flag[1] = 0;
        }
    }
}
