using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
{
    /// <summary>
    /// Hiro_NewSchedule.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_NewSchedule : Page
    {
        private Hiro_MainUI? Hiro_Main = null;
        internal int index = -1;
        public Hiro_NewSchedule(Hiro_MainUI? Parent)
        {
            InitializeComponent();
            Hiro_Main = Parent;
            Hiro_Initialize();
            Loaded += delegate
            {
                HiHiro();
            };
        }

        public void HiHiro()
        {
            var animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            Storyboard sb = new();
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                Hiro_Utils.AddPowerAnimation(3, scbtn_4, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(3, scbtn_5, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(3, scbtn_6, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(3, scbtn_7, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(1, scbtn_8, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(1, scbtn_9, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(1, scbtn_10, sb, -50, null);
            }

            if (!animation) 
                return;
            Hiro_Utils.AddPowerAnimation(0, this, sb, 50, null);
            sb.Begin();
        }

        private void Hiro_Initialize()
        {
            Load_Color();
            Load_Translate();
            Load_Position();
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        public void Load_Translate()
        {
            rbtn18.Content = Hiro_Utils.Get_Transalte("alarmonce");
            rbtn19.Content = Hiro_Utils.Get_Transalte("alarmed");
            rbtn20.Content = Hiro_Utils.Get_Transalte("alarmew");
            rbtn21.Content = Hiro_Utils.Get_Transalte("alarmat");
            scbtn_4.Content = Hiro_Utils.Get_Transalte("screset");
            scbtn_5.Content = Hiro_Utils.Get_Transalte("scok");
            scbtn_6.Content = Hiro_Utils.Get_Transalte("scclear");
            scbtn_7.Content = Hiro_Utils.Get_Transalte("sccancel");
            scbtn_8.Content = Hiro_Utils.Get_Transalte("sc15m");
            scbtn_9.Content = Hiro_Utils.Get_Transalte("sc1h");
            scbtn_10.Content = Hiro_Utils.Get_Transalte("sc1d");
            sclabel1.Content = Hiro_Utils.Get_Transalte("scname");
            sclabel2.Content = Hiro_Utils.Get_Transalte("sctime");
            sclabel3.Content = Hiro_Utils.Get_Transalte("sccmd");
        }

        public void Load_Position()
        {
            Hiro_Utils.Set_Control_Location(rbtn18, "alarmonce");
            Hiro_Utils.Set_Control_Location(rbtn19, "alarmed");
            Hiro_Utils.Set_Control_Location(rbtn20, "alarmew");
            Hiro_Utils.Set_Control_Location(rbtn21, "alarmat");
            Hiro_Utils.Set_Control_Location(scbtn_4, "screset", bottom: true);
            Hiro_Utils.Set_Control_Location(scbtn_5, "scok", bottom: true, right: true);
            Hiro_Utils.Set_Control_Location(scbtn_6, "scclear", bottom: true, right: true);
            Hiro_Utils.Set_Control_Location(scbtn_7, "sccancel", bottom: true, right: true);
            Hiro_Utils.Set_Control_Location(scbtn_8, "sc15m", right: true);
            Hiro_Utils.Set_Control_Location(scbtn_9, "sc1h", right: true);
            Hiro_Utils.Set_Control_Location(scbtn_10, "sc1d", right: true);
            Hiro_Utils.Set_Control_Location(sclabel1, "scname");
            Hiro_Utils.Set_Control_Location(sclabel2, "sctime");
            Hiro_Utils.Set_Control_Location(sclabel3, "sccommand");
            Hiro_Utils.Set_Control_Location(tb11, "scnametb");
            Hiro_Utils.Set_Control_Location(tb12, "sctimetb");
            Hiro_Utils.Set_Control_Location(tb13, "sccmdtb");
            Hiro_Utils.Set_Control_Location(tb14, "alarmattb");
        }


        private void Scbtn_4_Click(object sender, RoutedEventArgs e)
        {
            tb11.Text = App.scheduleitems[index].Name;
            tb12.Text = App.scheduleitems[index].Time;
            tb13.Text = App.scheduleitems[index].Command;
            tb14.Text = "";
            switch (App.scheduleitems[index].re)
            {
                case -2.0:
                    rbtn18.IsChecked = true;
                    break;
                case -1.0:
                    rbtn19.IsChecked = true;
                    break;
                case 0.0:
                    rbtn20.IsChecked = true;
                    break;
                default:
                    rbtn21.IsChecked = true;
                    tb14.Text = App.scheduleitems[index].re.ToString();
                    break;
            }
        }

        private void Scbtn_5_Click(object sender, RoutedEventArgs e)
        {
            if (tb11.Text.Equals(string.Empty) || tb12.Text.Equals(string.Empty) || tb13.Text.Equals(string.Empty) || (tb14.Text.Equals(string.Empty) && tb14.IsEnabled == true))
            {
                return;
            }
            double re = -2.0;
            if (rbtn19.IsChecked == true)
                re = -1.0;
            if (rbtn20.IsChecked == true)
                re = 0.0;
            if (rbtn21.IsChecked == true)
            {
                try
                {
                    re = double.Parse(tb14.Text);
                }
                catch (Exception ex)
                {
                    Hiro_Utils.LogtoFile("[ERROR]" + ex.Message);
                    re = -2.0;
                }
            }
            if (scbtn_4.Visibility == Visibility.Hidden)
            {
                var i = App.scheduleitems.Count + 1;
                App.scheduleitems.Add(new Scheduleitem(i, tb11.Text, tb12.Text, tb13.Text, re));
                Hiro_Utils.Write_Ini(App.sconfig, i.ToString(), "Name", tb11.Text);
                Hiro_Utils.Write_Ini(App.sconfig, i.ToString(), "Time", tb12.Text);
                Hiro_Utils.Write_Ini(App.sconfig, i.ToString(), "Command", "(" + tb13.Text + ")");
                Hiro_Utils.Write_Ini(App.sconfig, i.ToString(), "Repeat", re.ToString());
            }
            else
            {
                var i = index;
                App.scheduleitems[i].Name = tb11.Text;
                App.scheduleitems[i].Time = tb12.Text;
                App.scheduleitems[i].Command = tb13.Text;
                App.scheduleitems[i].re = re;
                Hiro_Utils.Write_Ini(App.sconfig, (i + 1).ToString(), "Name", tb11.Text);
                Hiro_Utils.Write_Ini(App.sconfig, (i + 1).ToString(), "Time", tb12.Text);
                Hiro_Utils.Write_Ini(App.sconfig, (i + 1).ToString(), "Command", "(" + tb13.Text + ")");
                Hiro_Utils.Write_Ini(App.sconfig, (i + 1).ToString(), "Repeat", re.ToString());
            }
            System.Globalization.DateTimeFormatInfo dtFormat = new()
            {
                ShortDatePattern = "yyyy/MM/dd HH:mm:ss"
            };
            DateTime dt = Convert.ToDateTime(tb12.Text, dtFormat);
            DateTime now = DateTime.Now;
            TimeSpan ts = dt - now;
            int day, hour, minute;
            if (ts.TotalDays < 0)
            {
                tb11.Text = "";
                tb12.Text = "";
                tb13.Text = "";
                if (Hiro_Main != null)
                    App.Notify(new noticeitem(Hiro_Utils.Get_Transalte("sctimepassed"), 2, Hiro_Main.schedulex.Content.ToString()));
                Hiro_Main?.Set_Label(Hiro_Main.schedulex);
            }
            else
            {
                day = ts.TotalDays > 1.0 ? Convert.ToInt32(Math.Truncate(ts.TotalDays)) : 0;
                hour = ts.TotalHours - 24 * day > 1.0 ? Convert.ToInt32(Math.Truncate(ts.TotalHours - 24 * day)) : 0;
                minute = ts.TotalMinutes - 60 * hour - 24 * 60 * day > 1.0 ? Convert.ToInt32(Math.Truncate(ts.TotalMinutes - 60 * hour - 24 * 60 * day)) : 0;
                tb11.Text = "";
                tb12.Text = "";
                tb13.Text = "";
                if (Hiro_Main != null)
                {
                    if (day > 0)
                    {
                        App.Notify(new noticeitem(Hiro_Utils.Get_Transalte("sctimeday").Replace("%d", day.ToString()).Replace("%h", hour.ToString()).Replace("%m", minute.ToString()), 2, Hiro_Main.schedulex.Content.ToString()));
                    }
                    else if (hour > 0)
                    {
                        App.Notify(new noticeitem(Hiro_Utils.Get_Transalte("sctimehour").Replace("%d", day.ToString()).Replace("%h", hour.ToString()).Replace("%m", minute.ToString()), 2, Hiro_Main.schedulex.Content.ToString()));
                    }
                    else
                    {
                        App.Notify(new noticeitem(Hiro_Utils.Get_Transalte("sctimemin").Replace("%d", day.ToString()).Replace("%h", hour.ToString()).Replace("%m", minute.ToString()), 2, Hiro_Main.schedulex.Content.ToString()));
                    }
                }
                Hiro_Main?.Set_Label(Hiro_Main.schedulex);
            }

        }

        private void Scbtn_7_Click(object sender, RoutedEventArgs e)
        {
            tb11.Text = "";
            tb12.Text = "";
            tb13.Text = "";
            Hiro_Main?.Set_Label(Hiro_Main.schedulex);
        }

        private void Scbtn_6_Click(object sender, RoutedEventArgs e)
        {
            tb11.Text = "";
            tb12.Text = "";
            tb13.Text = "";
            rbtn18.IsChecked = true;
            tb14.Text = "";
        }

        private void Scbtn_8_Click(object sender, RoutedEventArgs e)
        {
            tb12.Text = DateTime.Now.AddMinutes(15.0).ToString("yyyy/MM/dd HH:mm:ss");
        }

        private void Scbtn_9_Click(object sender, RoutedEventArgs e)
        {
            tb12.Text = DateTime.Now.AddHours(1.0).ToString("yyyy/MM/dd HH:mm:ss");
        }

        private void Scbtn_10_Click(object sender, RoutedEventArgs e)
        {
            tb12.Text = DateTime.Now.AddDays(1.0).ToString("yyyy/MM/dd HH:mm:ss");
        }

        private void Tb12_GotFocus(object sender, RoutedEventArgs e)
        {
            Go_TimePicker();
        }

        private void Tb12_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Go_TimePicker();
        }

        private void Go_TimePicker()
        {
            var dt = DateTime.Now;
            try
            {
                System.Globalization.DateTimeFormatInfo dtFormat = new()
                {
                    ShortDatePattern = "yyyy/MM/dd HH:mm:ss"
                };
                dt = Convert.ToDateTime(tb12.Text, dtFormat);
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogtoFile("[ERROR]" + ex.Message);
            }

            if (Hiro_Main == null) 
                return;
            Hiro_Main.hiro_time ??= new(Hiro_Main, this);
            Hiro_Main.hiro_time.year.Text = dt.Year.ToString();
            Hiro_Main.hiro_time.month.Text = dt.Month.ToString();
            Hiro_Main.hiro_time.day.Text = dt.Day.ToString();
            Hiro_Main.hiro_time.hour.Text = dt.Hour.ToString();
            Hiro_Main.hiro_time.minute.Text = dt.Minute.ToString();
            Hiro_Main.hiro_time.second.Text = dt.Second.ToString();
            Hiro_Main.Set_Label(Hiro_Main.timex);
        }
        private void Rbtn21_Checked(object sender, RoutedEventArgs e)
        {
            tb14.IsEnabled = true;
        }

        private void Rbtn21_Unchecked(object sender, RoutedEventArgs e)
        {
            tb14.IsEnabled = false;
        }
    }
}
