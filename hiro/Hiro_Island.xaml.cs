using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Security.Cryptography;
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
        internal Action? act = null;
        public Hiro_Island()
        {
            InitializeComponent();
            var icon = Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomizeIcon", "");
            icon = Hiro_Utils.Path_Prepare(Hiro_Utils.Path_Prepare_EX(icon));
            if (File.Exists(icon))
            {
                BitmapImage? bi = Hiro_Utils.GetBitmapImage(icon);
                (Resources["PrimaryIcon"] as ImageBrush).ImageSource = bi;
            }
            ContentLabel.MaxWidth = SystemParameters.FullPrimaryScreenWidth * 4 / 5;
            Title = $"{Hiro_Utils.Get_Translate("notitle")} - {App.AppTitle}";
            Load_One();
            Loaded += delegate
            {
                Load_Color();
                Island_In();
                timer = new()
                {
                    Interval = new TimeSpan(10000000)
                };
                timer.Tick += delegate
                {
                    TimerTick();
                };
            };
        }

        public void Load_Color()
        {
            TitleLabel.Foreground = new SolidColorBrush(App.AppForeColor);
            ContentLabel.Foreground = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 160));
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Toast", "0").Equals("3"))
            {
                BaseBorderBak.Visibility = Visibility.Visible;
                BaseBorder.Background = Hiro_Utils.Set_Bgimage(BaseBorder.Background, this);
                Hiro_Utils.Set_Foreground_Opacity(BaseBorderBak);
            }
            else if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
            {
                BaseBorderBak.Visibility = Visibility.Hidden;
                Storyboard? sc = new();
                Hiro_Utils.AddColorAnimaton(App.AppAccentColor, 150, BaseBorder, "Background.Color", sc);
                sc.Completed += delegate
                {
                    BaseBorder.Background = new SolidColorBrush(App.AppAccentColor);
                    sc = null;
                };
                sc.Begin();
            }
            else
            {
                BaseBorderBak.Visibility = Visibility.Hidden;
                BaseBorder.Background = new SolidColorBrush(App.AppAccentColor);
            }
            Hiro_Utils.Set_Control_Location(TitleLabel, "noticetitle", location: false);
            Hiro_Utils.Set_Control_Location(content, "noticecontent", location: false);
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
                Load_One();
                Island_Switch();
            }
        }

        private void Load_One()
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
            act = App.noticeitems[0].act;
            if (act != null)
            {
                ContentGrid.Cursor = Cursors.Hand;
            }
            else
            {
                ContentGrid.Cursor = null;
            }
            if (TitleLabel.Content == null || TitleLabel.Content.Equals(string.Empty))
                TitleLabel.Content = Hiro_Utils.Get_Translate("notitle");
            if (TitleLabel.Content != null)
                former_title = TitleLabel.Content.ToString() ?? string.Empty;
            if (former_count > 1)
                TitleLabel.Content = $"{former_title} ({former_count + 1 - notifications.Count}/{former_count})";
            Load_Icon();
            App.noticeitems.RemoveAt(0);
            notifications.RemoveAt(0);
            SetAutoSize(ContentGrid);
        }

        private void Load_Icon()
        {
            Hiro_Icon? icon = App.noticeitems[0].icon;
            bool set = false;
            if (icon != null)
            {
                if (icon.Image != null)
                {
                    BaseIcon.ImageSource = icon.Image;
                    set = true;
                }
                else
                {
                    var iconLocation = Hiro_Utils.Path_Prepare(Hiro_Utils.Path_Prepare_EX(icon.Location));
                    if (File.Exists(iconLocation))
                    {
                        BitmapImage? bi = Hiro_Utils.GetBitmapImage(iconLocation);
                        BaseIcon.ImageSource = bi;
                        set = true;
                    }
                }
            }
            if (!set)
            {
                BaseIcon.ImageSource = (Resources["PrimaryIcon"] as ImageBrush).ImageSource;
            }
        }

        private void Island_In()
        {
            SetAutoSize(BaseGrid);
            former_width = BaseGrid.ActualWidth;
            former_height = BaseGrid.ActualHeight;
            former_left = SystemParameters.FullPrimaryScreenWidth / 2 - former_width / 2;
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                Storyboard sb = new();
                sb = Hiro_Utils.AddDoubleAnimaton(50, 850, this, TopProperty.Name, sb, -former_height, 0.7);
                sb = Hiro_Utils.AddDoubleAnimaton(former_left, 850, this, LeftProperty.Name, sb, SystemParameters.FullPrimaryScreenWidth / 2, 0.7);
                sb = Hiro_Utils.AddDoubleAnimaton(former_width, 850, BaseGrid, "Width", sb, 1, 0.7);
                sb = Hiro_Utils.AddDoubleAnimaton(former_height, 850, BaseGrid, "Height", sb, 1, 0.7);
                sb.Completed += delegate
                {
                    Canvas.SetTop(this, 50);
                    Canvas.SetLeft(this, former_left);
                    SetAutoSize(BaseGrid);
                    timer.Start();
                };
                sb.Begin();
            }
            else
            {
                Canvas.SetTop(this, 50);
                Canvas.SetLeft(this, former_left);
                SetAutoSize(BaseGrid);
                timer.Start();
            }

        }

        private void Island_Out()
        {
            var w = ContentGrid.ActualWidth;
            var h = ContentGrid.ActualHeight;
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                Storyboard sb = new();
                sb = Hiro_Utils.AddDoubleAnimaton(-h, 700, this, TopProperty.Name, sb, 50, 0.7);
                sb = Hiro_Utils.AddDoubleAnimaton(SystemParameters.FullPrimaryScreenWidth / 2, 700, this, LeftProperty.Name, sb, SystemParameters.FullPrimaryScreenWidth / 2 - former_width / 2, 0.7);
                sb = Hiro_Utils.AddDoubleAnimaton(1, 700, BaseGrid, "Width", sb, former_width, 0.7);
                sb = Hiro_Utils.AddDoubleAnimaton(1, 700, BaseGrid, "Height", sb, former_height, 0.7);
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
            else
            {
                Visibility = Visibility.Hidden;
                Close();
            }

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
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                var sb = new Storyboard();
                sb = Hiro_Utils.AddDoubleAnimaton(former_left + offset, 300, this, LeftProperty.Name, sb, former_left);
                sb = Hiro_Utils.AddDoubleAnimaton(former_width - offset * 2, 180, BaseGrid, "Width", sb, former_width);
                sb.Completed += delegate
                {
                    former_left = SystemParameters.FullPrimaryScreenWidth / 2 - w / 2;
                    var sb = new Storyboard();
                    sb = Hiro_Utils.AddDoubleAnimaton(former_left, 300, this, LeftProperty.Name, sb, null);
                    sb = Hiro_Utils.AddDoubleAnimaton(w + ContentGrid.Margin.Left * 2, 300, BaseGrid, "Width", sb, null);
                    sb = Hiro_Utils.AddDoubleAnimaton(h + ContentGrid.Margin.Top * 2, 300, BaseGrid, "Height", sb, null);
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
            else
            {
                SetAutoSize(BaseGrid);
                former_width = BaseGrid.ActualWidth;
                former_height = BaseGrid.ActualHeight;
            }

        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NextNotification();
        }

        private void Label_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            NextNotification();
        }

        private void TitleLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (act != null)
                act.Invoke();
            act = null;
            ContentGrid.Cursor = null;
        }

        private void Content_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (act != null)
                act.Invoke();
            act = null;
            ContentGrid.Cursor = null;
        }

        private void BaseIconBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (act != null)
                act.Invoke();
            act = null;
            ContentGrid.Cursor = null;
        }
    }
}
