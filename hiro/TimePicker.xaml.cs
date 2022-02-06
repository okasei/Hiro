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
    /// TimePicker.xaml の相互作用ロジック
    /// </summary>
    /// 
    public partial class TimePicker : Window
    {
    int MaxDay = 28;
        public TimePicker()
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            Load_Colors();
            Load_Position();
            Load_Transalte();
            Loaded += delegate
            {
                if (App.mn != null)
                {
                    this.Margin = App.mn.Margin;
                    this.Width = App.mn.Width;
                    this.Height = App.mn.Height;
                    bgimage.Background = App.mn.bgimage.Background;
                    var animation = true;
                    if (utils.Read_Ini(App.dconfig, "Configuration", "ani", "1").Equals("0"))
                        animation = false;
                    else
                        animation = true;
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
                    Width = 550;
                    Height = 450;
                    Thickness tn = bgimage.Margin;
                    tn.Left = 0.0;
                    tn.Top = 0.0;
                    bgimage.Margin = tn;
                    bgimage.Width = Width;
                    bgimage.Height = Height;
                    bgimage.Background = new SolidColorBrush(App.AppAccentColor);
                }
            };
            utils.SetShadow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
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
        public void Load_Colors()
        {
            tpbtn1.BorderThickness = new Thickness(1, 1, 1, 1);
            tpbtn2.BorderThickness = new Thickness(1, 1, 1, 1);
            tpbtn1.BorderBrush = new SolidColorBrush(App.AppForeColor);
            tpbtn2.BorderBrush = new SolidColorBrush(App.AppForeColor);
            tpbtn1.Foreground = new SolidColorBrush(App.AppForeColor);
            tpbtn2.Foreground = tpbtn1.Foreground;
            tpbtn1.Background = new SolidColorBrush(Color.FromArgb(160, App.AppAccentColor.R, App.AppAccentColor.G, App.AppAccentColor.B));
            tpbtn2.Background = tpbtn1.Background;
            title.Foreground = tpbtn1.Foreground;
            year.Foreground = tpbtn1.Foreground;
            month.Foreground = tpbtn1.Foreground;
            day.Foreground = tpbtn1.Foreground;
            hour.Foreground = tpbtn1.Foreground;
            minute.Foreground = tpbtn1.Foreground;
            second.Foreground = tpbtn1.Foreground;
            borderlabel.BorderBrush = new SolidColorBrush(App.AppForeColor);

        }
        private void Load_Transalte()
        {
            title.Content = utils.Get_Transalte("timetitle");
            Title = title.Content + " - " + App.AppTitle;
            tpbtn1.Content = utils.Get_Transalte("timeok");
            tpbtn2.Content = utils.Get_Transalte("timecancel");
        }
        private void Load_Position()
        {
            utils.Set_Control_Location(title, "timetitle");
            utils.Set_Control_Location(year, "timeyear");
            utils.Set_Control_Location(month, "timemonth");
            utils.Set_Control_Location(day, "timeday");
            utils.Set_Control_Location(hour, "timehour");
            utils.Set_Control_Location(minute, "timemin");
            utils.Set_Control_Location(second, "timesec");
            utils.Set_Control_Location(tpbtn1, "timeok");
            utils.Set_Control_Location(tpbtn2, "timecancel");
        }
        private void scbtn_5_Click(object sender, RoutedEventArgs e)
        {
            App.tp = null;
            this.Close();
        }

        private String string_deal(String a,int b)
        {
            try
            {
                a = int.Parse(a).ToString();
            }
            catch(Exception ex)
            {
                a = "0";
                utils.LogtoFile("[ERROR]" + ex.Message);
            }
            if (a.Length > b)
            {
                a = a.Substring(0, b);
            }
            else if(a.Length < b)
            {
                while(a.Length < b)
                    a = "0" + a;
            }
            return a;
        }

        private void scbtn_7_Click(object sender, RoutedEventArgs e)
        {
            if (App.mn != null)
            {
                App.mn.tb12.Text = string_deal(year.Text, 4) + "/" + string_deal(month.Text, 2) + "/" + string_deal(day.Text, 2) + " " + string_deal(hour.Text, 2) + ":" + string_deal(minute.Text, 2) + ":" + string_deal(second.Text, 2);
            }
            App.tp = null;
            this.Close();
        }

        private void year_MouseWheel(object sender, MouseWheelEventArgs e)
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

        private void month_MouseWheel(object sender, MouseWheelEventArgs e)
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

        private void hour_MouseWheel(object sender, MouseWheelEventArgs e)
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

        private void minute_MouseWheel(object sender, MouseWheelEventArgs e)
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

        private void second_MouseWheel(object sender, MouseWheelEventArgs e)
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

        private void day_MouseWheel(object sender, MouseWheelEventArgs e)
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

        private void month_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if(month != null)
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

        private void day_TextChanged(object sender, TextChangedEventArgs e)
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

        private void year_TextChanged(object sender, TextChangedEventArgs e)
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

        private void hour_TextChanged(object sender, TextChangedEventArgs e)
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

        private void minute_TextChanged(object sender, TextChangedEventArgs e)
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

        private void second_TextChanged(object sender, TextChangedEventArgs e)
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
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }

        private void tp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }

        private void tp_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.tp = null;
        }

        private void tp_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            borderlabel.BorderThickness = new Thickness(2, 2, 2, 2);
            Thickness th = borderlabel.Margin;
            th.Left = 0;
            th.Top = 0;
            borderlabel.Margin = th;
            borderlabel.Width = Width;
            borderlabel.Height = Height;
        }
    }
}
