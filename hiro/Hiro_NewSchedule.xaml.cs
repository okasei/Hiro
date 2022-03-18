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
        private Mainui? Hiro_Main = null;
        internal int index = -1;
        public Hiro_NewSchedule(Mainui? Parent)
        {
            InitializeComponent();
            Hiro_Main = Parent;
            Hiro_Initialize();
            Loaded += Hiro_NewSchedule_Loaded;
        }

        private void Hiro_NewSchedule_Loaded(object sender, RoutedEventArgs e)
        {
            bool animation = !utils.Read_Ini(App.dconfig, "config", "ani", "1").Equals("0");
            if (animation)
                BeginStoryboard(Application.Current.Resources["AppLoad"] as Storyboard);
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
            Resources["AppAccent"] = new SolidColorBrush(utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        public void Load_Translate()
        {
            rbtn18.Content = utils.Get_Transalte("alarmonce");
            rbtn19.Content = utils.Get_Transalte("alarmed");
            rbtn20.Content = utils.Get_Transalte("alarmew");
            rbtn21.Content = utils.Get_Transalte("alarmat");
            scbtn_4.Content = utils.Get_Transalte("screset");
            scbtn_5.Content = utils.Get_Transalte("scok");
            scbtn_6.Content = utils.Get_Transalte("scclear");
            scbtn_7.Content = utils.Get_Transalte("sccancel");
            scbtn_8.Content = utils.Get_Transalte("sc15m");
            scbtn_9.Content = utils.Get_Transalte("sc1h");
            scbtn_10.Content = utils.Get_Transalte("sc1d");
            sclabel1.Content = utils.Get_Transalte("scname");
            sclabel2.Content = utils.Get_Transalte("sctime");
            sclabel3.Content = utils.Get_Transalte("sccmd");
        }

        public void Load_Position()
        {
            utils.Set_Control_Location(rbtn18, "alarmonce");
            utils.Set_Control_Location(rbtn19, "alarmed");
            utils.Set_Control_Location(rbtn20, "alarmew");
            utils.Set_Control_Location(rbtn21, "alarmat");
            utils.Set_Control_Location(scbtn_4, "screset", bottom: true);
            utils.Set_Control_Location(scbtn_5, "scok", bottom: true, right: true);
            utils.Set_Control_Location(scbtn_6, "scclear", bottom: true, right: true);
            utils.Set_Control_Location(scbtn_7, "sccancel", bottom: true, right: true);
            utils.Set_Control_Location(scbtn_8, "sc15m", right: true);
            utils.Set_Control_Location(scbtn_9, "sc1h", right: true);
            utils.Set_Control_Location(scbtn_10, "sc1d", right: true);
            utils.Set_Control_Location(sclabel1, "scname");
            utils.Set_Control_Location(sclabel2, "sctime");
            utils.Set_Control_Location(sclabel3, "sccommand");
            utils.Set_Control_Location(tb11, "scnametb");
            utils.Set_Control_Location(tb12, "sctimetb");
            utils.Set_Control_Location(tb13, "sccmdtb");
            utils.Set_Control_Location(tb14, "alarmattb");
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
                    utils.LogtoFile("[ERROR]" + ex.Message);
                    re = -2.0;
                }
            }
            if (scbtn_4.Visibility == Visibility.Hidden)
            {
                var i = App.scheduleitems.Count + 1;
                App.scheduleitems.Add(new Scheduleitem(i, tb11.Text, tb12.Text, tb13.Text, re));
                utils.Write_Ini(App.sconfig, i.ToString(), "name", tb11.Text);
                utils.Write_Ini(App.sconfig, i.ToString(), "time", tb12.Text);
                utils.Write_Ini(App.sconfig, i.ToString(), "command", "(" + tb13.Text + ")");
                utils.Write_Ini(App.sconfig, i.ToString(), "repeat", re.ToString());
            }
            else
            {
                var i = index;
                App.scheduleitems[i].Name = tb11.Text;
                App.scheduleitems[i].Time = tb12.Text;
                App.scheduleitems[i].Command = tb13.Text;
                App.scheduleitems[i].re = re;
                utils.Write_Ini(App.sconfig, (i + 1).ToString(), "name", tb11.Text);
                utils.Write_Ini(App.sconfig, (i + 1).ToString(), "time", tb12.Text);
                utils.Write_Ini(App.sconfig, (i + 1).ToString(), "command", "(" + tb13.Text + ")");
                utils.Write_Ini(App.sconfig, (i + 1).ToString(), "repeat", re.ToString());
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
                {
                    Hiro_Main.Set_Label(Hiro_Main.schedulex);
                    App.Notify(new noticeitem(utils.Get_Transalte("sctimepassed"), 2, Hiro_Main.schedulex.Content.ToString()));
                }
            }
            else
            {
                if (ts.TotalDays > 1.0)
                    day = Convert.ToInt32(Math.Truncate(ts.TotalDays));
                else
                    day = 0;
                if (ts.TotalHours - 24 * day > 1.0)
                    hour = Convert.ToInt32(Math.Truncate(ts.TotalHours - 24 * day));
                else
                    hour = 0;
                if (ts.TotalMinutes - 60 * hour - 24 * 60 * day > 1.0)
                    minute = Convert.ToInt32(Math.Truncate(ts.TotalMinutes - 60 * hour - 24 * 60 * day));
                else
                    minute = 0;
                tb11.Text = "";
                tb12.Text = "";
                tb13.Text = "";
                if (Hiro_Main != null)
                {
                    Hiro_Main.Set_Label(Hiro_Main.schedulex);
                    if (day > 0)
                    {
                        App.Notify(new noticeitem(utils.Get_Transalte("sctimeday").Replace("%d", day.ToString()).Replace("%h", hour.ToString()).Replace("%m", minute.ToString()), 2, Hiro_Main.schedulex.Content.ToString()));
                    }
                    else if (hour > 0)
                    {
                        App.Notify(new noticeitem(utils.Get_Transalte("sctimehour").Replace("%d", day.ToString()).Replace("%h", hour.ToString()).Replace("%m", minute.ToString()), 2, Hiro_Main.schedulex.Content.ToString()));
                    }
                    else
                    {
                        App.Notify(new noticeitem(utils.Get_Transalte("sctimemin").Replace("%d", day.ToString()).Replace("%h", hour.ToString()).Replace("%m", minute.ToString()), 2, Hiro_Main.schedulex.Content.ToString()));
                    }
                }
            }

        }

        private void Scbtn_7_Click(object sender, RoutedEventArgs e)
        {
            tb11.Text = "";
            tb12.Text = "";
            tb13.Text = "";
            if (Hiro_Main != null)
            {
                Hiro_Main.Set_Label(Hiro_Main.schedulex);
            }
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
            DateTime dt = DateTime.Now;
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
                utils.LogtoFile("[ERROR]" + ex.Message);
            }
            Hiro_Time ht = new(Hiro_Main, this);
            ht.year.Text = dt.Year.ToString();
            ht.month.Text = dt.Month.ToString();
            ht.day.Text = dt.Day.ToString();
            ht.hour.Text = dt.Hour.ToString();
            ht.minute.Text = dt.Minute.ToString();
            ht.second.Text = dt.Second.ToString();
            if (Hiro_Main != null)
            {
                Hiro_Main.Set_Label(Hiro_Main.timex);
                Hiro_Main.current = ht;
                Hiro_Main.frame.Content = ht;
            }
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
