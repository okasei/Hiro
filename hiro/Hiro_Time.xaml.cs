using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
{
    /// <summary>
    /// Hiro_Time.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Time : Page
    {
        private Mainui? Hiro_Main = null;
        private Hiro_NewSchedule? hns = null;
        private Hiro_Time? ht = null;
        int MaxDay = 28;
        public Hiro_Time(Mainui? Parent, Hiro_NewSchedule Schedule)
        {
            InitializeComponent();
            Hiro_Main = Parent;
            hns = Schedule;
            Hiro_Initialize();
            Loaded += Hiro_Time_Loaded;
        }

        private void Hiro_Time_Loaded(object sender, RoutedEventArgs e)
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
            tp_title.Content = utils.Get_Transalte("time");
            tpbtn1.Content = utils.Get_Transalte("timeok");
            tpbtn2.Content = utils.Get_Transalte("timecancel");
        }

        public void Load_Position()
        {
            utils.Set_Control_Location(tp_title, "timet");
            utils.Set_Control_Location(year, "timeyear");
            utils.Set_Control_Location(month, "timemonth");
            utils.Set_Control_Location(day, "timeday");
            utils.Set_Control_Location(hour, "timehour");
            utils.Set_Control_Location(minute, "timemin");
            utils.Set_Control_Location(second, "timesec");
            utils.Set_Control_Location(tpbtn1, "timeok", bottom: true, right: true);
            utils.Set_Control_Location(tpbtn2, "timecancel", bottom: true, right: true);
        }

        private void Timex_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Hiro_Main != null)
            {
                Hiro_Main.Set_Label(Hiro_Main.timex);
                Hiro_Main.Content = ht;
                Hiro_Main.current = ht;
            }
        }
        private void Tp_Cancel(object sender, RoutedEventArgs e)
        {
            if (Hiro_Main != null)
            {
                Hiro_Main.Set_Label(Hiro_Main.newx);
                Hiro_Main.Content = hns;
                Hiro_Main.current = hns;
            }
        }

        private static String String_deal(String a, int b)
        {
            try
            {
                a = int.Parse(a).ToString();
            }
            catch (Exception ex)
            {
                a = "0";
                utils.LogtoFile("[ERROR]" + ex.Message);
            }
            if (a.Length > b)
            {
                a = a[..b];
            }
            else if (a.Length < b)
            {
                while (a.Length < b)
                    a = "0" + a;
            }
            return a;
        }

        private void Tp_Go(object sender, RoutedEventArgs e)
        {
            if (hns != null)
                hns.tb12.Text = String_deal(year.Text, 4) + "/" + String_deal(month.Text, 2) + "/" + String_deal(day.Text, 2) + " " + String_deal(hour.Text, 2) + ":" + String_deal(minute.Text, 2) + ":" + String_deal(second.Text, 2);
            if (Hiro_Main != null)
            {
                Hiro_Main.Set_Label(Hiro_Main.newx);
                Hiro_Main.Content = hns;
                Hiro_Main.current = hns;
            }
        }

        private void Year_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                try
                {
                    year.Text = (int.Parse(year.Text) + 1).ToString();
                }
                catch
                {
                    year.Text = DateTime.Now.Year.ToString();
                }
            }
            else
            {
                try
                {
                    year.Text = (int.Parse(year.Text) - 1).ToString();
                }
                catch
                {
                    year.Text = DateTime.Now.Year.ToString();
                }
            }
        }

        private void Month_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                try
                {
                    if (int.Parse(month.Text) + 1 < 13)
                        month.Text = (int.Parse(month.Text) + 1).ToString();
                    else
                        month.Text = "12";
                }
                catch
                {
                    month.Text = DateTime.Now.Month.ToString();
                }
            }
            else
            {
                try
                {
                    if (int.Parse(month.Text) - 1 > 0)
                        month.Text = (int.Parse(month.Text) - 1).ToString();
                    else
                        month.Text = "01";
                }
                catch
                {
                    month.Text = DateTime.Now.Month.ToString();
                }
            }
        }

        private void Hour_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                try
                {
                    if (int.Parse(hour.Text) + 1 < 24)
                        hour.Text = (int.Parse(hour.Text) + 1).ToString();
                    else
                        hour.Text = "23";
                }
                catch
                {
                    hour.Text = DateTime.Now.Hour.ToString();
                }
            }
            else
            {
                try
                {
                    if (int.Parse(hour.Text) - 1 > -1)
                        hour.Text = (int.Parse(hour.Text) - 1).ToString();
                    else
                        hour.Text = "00";
                }
                catch
                {
                    hour.Text = DateTime.Now.Hour.ToString();
                }
            }
        }

        private void Minute_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                try
                {
                    if (int.Parse(minute.Text) + 1 < 60)
                        minute.Text = (int.Parse(minute.Text) + 1).ToString();
                    else
                        minute.Text = "59";
                }
                catch
                {
                    minute.Text = DateTime.Now.Minute.ToString();
                }
            }
            else
            {
                try
                {
                    if (int.Parse(minute.Text) - 1 > -1)
                        minute.Text = (int.Parse(minute.Text) - 1).ToString();
                    else
                        minute.Text = "00";
                }
                catch
                {
                    minute.Text = DateTime.Now.Minute.ToString();
                }
            }
        }

        private void Second_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                try
                {
                    if (int.Parse(second.Text) + 1 < 60)
                        second.Text = (int.Parse(second.Text) + 1).ToString();
                    else
                        second.Text = "59";
                }
                catch
                {
                    second.Text = DateTime.Now.Second.ToString();
                }
            }
            else
            {
                try
                {
                    if (int.Parse(second.Text) - 1 > -1)
                        second.Text = (int.Parse(second.Text) - 1).ToString();
                    else
                        second.Text = "00";
                }
                catch
                {
                    second.Text = DateTime.Now.Second.ToString();
                }
            }
        }

        private void Day_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                try
                {
                    if (int.Parse(day.Text) + 1 <= MaxDay)
                        day.Text = (int.Parse(day.Text) + 1).ToString();
                    else
                        day.Text = MaxDay.ToString();
                }
                catch
                {
                    day.Text = DateTime.Now.Day.ToString();
                }
            }
            else
            {
                try
                {
                    if (int.Parse(day.Text) - 1 > -1)
                        day.Text = (int.Parse(day.Text) - 1).ToString();
                    else
                        day.Text = "01";
                }
                catch
                {
                    day.Text = DateTime.Now.Day.ToString();
                }
            }
        }

        private void Month_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (month != null)
                {
                    month.Text = int.Parse(month.Text).ToString();
                    if (int.Parse(month.Text) < 1)
                        month.Text = "1";
                    if (int.Parse(month.Text) > 12)
                        month.Text = "12";
                }
            }
            catch
            {
                month.Text = DateTime.Now.Month.ToString();
            }

            try
            {
                if (month != null && year != null)
                    MaxDay = System.Threading.Thread.CurrentThread.CurrentUICulture.Calendar.GetDaysInMonth(int.Parse(year.Text), int.Parse(month.Text));
                else
                    MaxDay = 28;
            }
            catch
            {
                MaxDay = 28;
            }
            if (day != null && int.Parse(day.Text) > MaxDay)
                day.Text = MaxDay.ToString();
        }

        private void Day_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (day != null)
                {
                    day.Text = int.Parse(day.Text).ToString();
                    if (int.Parse(day.Text) < 1)
                        day.Text = "1";
                    if (int.Parse(day.Text) > MaxDay)
                        day.Text = MaxDay.ToString();
                }
            }
            catch
            {
                day.Text = DateTime.Now.Day.ToString();
            }
        }

        private void Year_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (year != null)
                {
                    year.Text = int.Parse(year.Text).ToString();
                }
            }
            catch
            {
                year.Text = DateTime.Now.Year.ToString();
            }
            try
            {
                if (month != null && year != null)
                    MaxDay = System.Threading.Thread.CurrentThread.CurrentUICulture.Calendar.GetDaysInMonth(int.Parse(year.Text), int.Parse(month.Text));
                else
                    MaxDay = 28;
            }
            catch
            {
                MaxDay = 28;
            }
            if (day != null && int.Parse(day.Text) > MaxDay)
                day.Text = MaxDay.ToString();
        }

        private void Hour_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                if (hour != null)
                {
                    hour.Text = int.Parse(hour.Text).ToString();
                    if (int.Parse(hour.Text) < 0)
                        hour.Text = "0";
                    if (int.Parse(hour.Text) > 23)
                        hour.Text = "23";
                }
            }
            catch
            {
                hour.Text = DateTime.Now.Hour.ToString();
            }
        }

        private void Minute_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (minute != null)
                {
                    minute.Text = int.Parse(minute.Text).ToString();
                    if (int.Parse(minute.Text) < 0)
                        minute.Text = "0";
                    if (int.Parse(minute.Text) > 59)
                        minute.Text = "59";
                }
            }
            catch
            {
                minute.Text = DateTime.Now.Minute.ToString();
            }
        }

        private void Second_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (second != null)
                {
                    second.Text = int.Parse(second.Text).ToString();
                    if (int.Parse(second.Text) < 0)
                        second.Text = "0";
                    if (int.Parse(second.Text) > 59)
                        second.Text = "59";
                }
            }
            catch
            {
                second.Text = DateTime.Now.Second.ToString();
            }
        }
    }
}
