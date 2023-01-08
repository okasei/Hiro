using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace hiro
{
    /// <summary>
    /// Hiro_Island.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Island : Window
    {
        internal double former_left = 0;
        internal double former_width = 0;
        internal double former_height = 0;
        internal int CD = 2;
        internal DispatcherTimer timer;
        internal List<string> notifications = new();
        internal int former_count = 0;
        internal string former_title = string.Empty;
        internal int former_CD = 2;
        public Hiro_Island()
        {
            InitializeComponent();
            ContentLabel.MaxWidth = SystemParameters.FullPrimaryScreenWidth * 4 / 5;
            Title = $"{Hiro_Utils.Get_Translate("notitle")} - {App.AppTitle}";
            var t = App.noticeitems[0].msg.Replace("\\n", Environment.NewLine).Replace("<br>", Environment.NewLine);
            if (t.IndexOf(Environment.NewLine) != -1)
            {
                notifications = t.Replace("<nop>", "").Split(Environment.NewLine).ToList();
                for (int i = 0; i < notifications.Count; i++)
                {
                    if (notifications[i].Replace(" ", "").Equals(string.Empty))
                    {
                        notifications.RemoveAt(i);
                        i--;
                    }
                }
            }
            else
                notifications = new()
                {
                    t
                };
            former_count = notifications.Count;
            ContentLabel.Text = notifications[0];
            TitleLabel.Content = App.noticeitems[0].title;
            if (TitleLabel.Content == null || TitleLabel.Content.Equals(string.Empty))
                TitleLabel.Content = Hiro_Utils.Get_Translate("notitle");
            if (TitleLabel.Content != null)
                former_title = TitleLabel.Content.ToString();
            if (former_count > 1)
                TitleLabel.Content = $"{former_title} ({former_count + 1 - notifications.Count}/{former_count})";
            CD = App.noticeitems[0].time;
            former_CD = CD;
            Loaded += delegate
            {
                Island_In();
                timer = new()
                {
                    Interval = new TimeSpan(10000000)
                };
                timer.Tick += delegate
                {
                    TimerTick();
                };
                App.noticeitems.RemoveAt(0);
                notifications.RemoveAt(0);
            };
        }

        private void TimerTick()
        {
            CD--;
            if (CD <= 0)
            {
                NextNotification();
            }
        }

        private void NextNotification()
        {
            if (notifications.Count > 0)
            {
                ContentLabel.Text = notifications[0];
                CD = former_CD;
                if (former_count > 1)
                    TitleLabel.Content = $"{former_title} ({former_count + 1 - notifications.Count}/{former_count})";
                notifications.RemoveAt(0);
                SetAutoSize(ContentGrid);
                Island_Switch();
            }
            else if (App.noticeitems.Count < 1)
            {
                Island_Out();
                timer.Stop();
                App.hisland = null;
            }
            else
            {
                CD = App.noticeitems[0].time;
                former_CD = CD;
                var t = App.noticeitems[0].msg.Replace("\\n", Environment.NewLine).Replace("<br>", Environment.NewLine);
                if (t.IndexOf(Environment.NewLine) != -1)
                {
                    notifications = t.Replace("<nop>", "").Split(Environment.NewLine).ToList();
                    for (int i = 0; i < notifications.Count; i++)
                    {
                        if (notifications[i].Replace(" ", "").Equals(string.Empty))
                        {
                            notifications.RemoveAt(i);
                            i--;
                        }
                    }
                }
                else
                    notifications = new()
                {
                    t
                };
                former_count = notifications.Count;
                ContentLabel.Text = notifications[0];
                TitleLabel.Content = App.noticeitems[0].title;
                if (TitleLabel.Content == null || TitleLabel.Content.Equals(string.Empty))
                    TitleLabel.Content = Hiro_Utils.Get_Translate("notitle");
                if (TitleLabel.Content != null)
                    former_title = TitleLabel.Content.ToString();
                if (former_count > 1)
                    TitleLabel.Content = $"{former_title} ({former_count + 1 - notifications.Count}/{former_count})";
                App.noticeitems.RemoveAt(0);
                notifications.RemoveAt(0);
                SetAutoSize(ContentGrid);
                Island_Switch();
            }
        }

        private void Island_In()
        {
            former_width = BaseGrid.ActualWidth;
            former_height = BaseGrid.ActualHeight;
            former_left = SystemParameters.FullPrimaryScreenWidth / 2 - former_width / 2;

            Storyboard sb = new();
            sb = Hiro_Utils.AddDoubleAnimaton(50, 600, this, TopProperty.Name, sb, -former_height);
            sb = Hiro_Utils.AddDoubleAnimaton(former_left, 600, this, LeftProperty.Name, sb, SystemParameters.FullPrimaryScreenWidth / 2);
            sb = Hiro_Utils.AddDoubleAnimaton(former_width, 600, BaseGrid, "Width", sb, 1);
            sb = Hiro_Utils.AddDoubleAnimaton(former_height, 600, BaseGrid, "Height", sb, 1);
            sb.Completed += delegate
            {
                Canvas.SetTop(this, 50);
                Canvas.SetLeft(this, former_left);
                SetAutoSize(BaseGrid);
                timer.Start();
            };
            sb.Begin();
        }

        private void Island_Out()
        {
            var w = ContentGrid.ActualWidth;
            var h = ContentGrid.ActualHeight;
            Storyboard sb = new();
            sb = Hiro_Utils.AddDoubleAnimaton(-h, 600, this, TopProperty.Name, sb, 50);
            sb = Hiro_Utils.AddDoubleAnimaton(SystemParameters.FullPrimaryScreenWidth / 2, 600, this, LeftProperty.Name, sb, SystemParameters.FullPrimaryScreenWidth / 2 - former_width / 2);
            sb = Hiro_Utils.AddDoubleAnimaton(1, 600, BaseGrid, "Width", sb, former_width, 0.4);
            sb = Hiro_Utils.AddDoubleAnimaton(1, 600, BaseGrid, "Height", sb, former_height, 0.4);
            sb.Completed += delegate
            {
                Visibility = Visibility.Hidden;
                new Thread(() =>
                {
                    //weird...
                    Thread.Sleep(100);
                    Dispatcher.Invoke(() =>
                    {
                        Close();
                    });
                }).Start();
            };
            sb.Begin();
        }

        private void SetAutoSize(FrameworkElement fe)
        {
            fe.Width = double.NaN;
            fe.Height = double.NaN;
            fe.InvalidateVisual();
            fe.UpdateLayout();
        }

        private void Island_Switch()
        {
            BaseGrid.Width = former_width;
            BaseGrid.Height = former_height;
            var w = ContentGrid.ActualWidth;
            var h = ContentGrid.ActualHeight;
            var offset = Math.Min(w * 0.1, 50);
            var sb = new Storyboard();
            sb = Hiro_Utils.AddDoubleAnimaton(former_left + offset, 180, this, LeftProperty.Name, sb, former_left, 0.7);
            sb = Hiro_Utils.AddDoubleAnimaton(former_width - offset * 2, 180, BaseGrid, "Width", sb, former_width, 0.7);
            sb.Completed += delegate
            {
                former_left = SystemParameters.FullPrimaryScreenWidth / 2 - w / 2;
                var sb = new Storyboard();
                sb = Hiro_Utils.AddDoubleAnimaton(former_left, 180, this, LeftProperty.Name, sb, null, 0.7);
                sb = Hiro_Utils.AddDoubleAnimaton(w + ContentGrid.Margin.Left * 2, 180, BaseGrid, "Width", sb, null, 0.7);
                sb = Hiro_Utils.AddDoubleAnimaton(h + ContentGrid.Margin.Top * 2, 180, BaseGrid, "Height", sb, null, 0.7);
                sb.Completed += delegate
                {
                    SetAutoSize(BaseGrid);
                    former_width = BaseGrid.ActualWidth;
                    former_height = BaseGrid.ActualHeight;
                };
                sb.Begin();
            };
            sb.Begin();
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NextNotification();
        }

        private void Label_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            NextNotification();
        }
    }
}
