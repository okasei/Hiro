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
    public partial class notification : Window
    {
        internal string msg;
        private DispatcherTimer timer;
        internal int loop;
        internal int loops;
        internal int flag = 0;//0=进入,1=显示,2=退出
        internal double i = 0.01;
        internal double ai = 0.01;
        internal bool animation = true;
        internal int firstflag = 1;
        public notification()
        {
            InitializeComponent();
            this.Title = utils.Get_Transalte("notitle") + " - " + App.AppTitle;
            Load_Color();
            utils.Set_Control_Location(notinfo, "notify");
            this.Title = App.AppTitle;
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = 32;
            this.SetValue(Canvas.LeftProperty, 0.0);
            this.SetValue(Canvas.TopProperty, 0.0);
            this.Opacity = 0.01;
            this.msg = "";
            if (utils.Read_Ini(App.dconfig, "Configuration", "ani", "1").Equals("0"))
                animation = false;
            else
                animation = true;
            timer = new();
            timer.Interval = new TimeSpan(100000);
            timer.Tick += delegate
            {
                TimerTick();
            };
            timer.Start();
        }
        private void TimerTick()
        {
            switch(flag)
            {
                case 0:
                    if (animation)
                    {
                        i += ai;
                        ai += 0.01;
                        if(i > 1)
                        {
                            i = 1;
                            flag = 1;
                            timer.Interval = new TimeSpan(10000000);
                        }
                        this.Opacity = i;
                    }
                    else
                    {
                        this.Opacity = 1;
                        flag = 1;
                        timer.Interval = new TimeSpan(10000000);
                    }
                    break;
                case 1:
                    loop--;
                    if (loop <= 0)
                    {
                        Next_Msg();
                    }
                    if (loop <= 0)
                    {
                        flag = 2;
                        i = 1;
                        ai = 0.01;
                        timer.Interval = new TimeSpan(10000);
                    }
                    break;
                case 2:
                    if (animation)
                    {
                        i -= ai;
                        ai += 0.01;
                        if (i < 0)
                        {
                            i = 0;
                            App.noti = null;
                            this.Close();
                            timer.Stop();
                        }
                        this.Opacity = i;
                        
                    }
                    else
                    {
                        App.noti = null;
                        this.Close();
                        timer.Stop();
                    }
                    break;
                default:
                    break;


            }
        }
        public void Load_Color()
        {
            this.notinfo.Foreground = new SolidColorBrush(App.AppForeColor);
            this.Background = new SolidColorBrush(App.AppAccentColor);
        }

        private void Next_Msg()
        {
            if (msg.IndexOf("\\n") != -1)
            {
                string newmsg = "";
                while (newmsg.Equals("") && msg.IndexOf("\\n") != -1)
                {
                    newmsg = msg[..msg.IndexOf("\\n")];
                    notinfo.Content = newmsg;
                    msg = msg[(newmsg.Length + 2)..];
                    Ani();
                    loop = loops;
                    
                }
                if (newmsg.Equals("") && msg.Equals(""))
                    return;
                
            }
            else if(!msg.Equals(""))
            {
                notinfo.Content = msg;
                Ani();
                msg = "";
                loop = loops;
            }
            else if (App.noticeitems.Count != 0)
            {
                msg = App.noticeitems[0].msg;
                loop = App.noticeitems[0].time;
                loops = loop;
                if (msg.IndexOf("\\n") != -1)
                {
                    string newmsg = msg[..msg.IndexOf("\\n")];
                    notinfo.Content = newmsg;
                    Ani();
                    msg = msg[(newmsg.Length + 2)..];
                }
                else
                {
                    notinfo.Content = msg;
                    Ani();
                    msg = "";
                }
                App.noticeitems.RemoveAt(0);
            }
            else
            {
                if (flag == 2)
                    return;
                flag = 2;
                i = 1;
                ai = 0.01;
                timer.Interval = new TimeSpan(10000);
            }
        }

        private void Ani()
        {
            if (firstflag == 1)
            {
                firstflag = 0;
                return;
            }
            if (animation)
            {
                var rd = App.blurradius;
                var step = App.blurradius / App.blursec;
                while (rd > 0.0)
                {
                    rd -= step;
                    if (rd < 0.0)
                    {
                        rd = 0.0;
                        notinfo.Effect = null;
                        return;
                    }
                    notinfo.Effect = new System.Windows.Media.Effects.BlurEffect()
                    {
                        Radius = rd,
                        RenderingBias = System.Windows.Media.Effects.RenderingBias.Performance
                    };
                    utils.Delay(App.blurdelay);
                }
            }
        }
        private void noti_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.noticeitems[0].msg.IndexOf("\\n") != -1)
            {
                notinfo.Content = App.noticeitems[0].msg[..App.noticeitems[0].msg.IndexOf("\\n")];
            }
            else
            {
                notinfo.Content = App.noticeitems[0].msg;
            }
            
        }

        private void noti_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Next_Msg();
        }

        private void notinfo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Next_Msg();
        }
    }
}
