using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace hiro
{
    /// <summary>
    /// Logique d'interaction pour Hiro_Box.xaml
    /// </summary>
    public partial class Hiro_Box : Window
    {
        double maxWidth = SystemParameters.FullPrimaryScreenWidth / 3;
        const int innerMargin = 10;
        const int boxInLen = 600;
        const int fadeInLen = 300;
        const int fadeOutLen = 200;
        const int textInLen = 800;
        const int contentInLen = 1500;
        const int boxOutLen = 400;
        internal List<string> notifications = new();
        internal Action? act = null;
        internal int temps = 2;
        internal string formerTitle = "";
        private Hiro_Icon? formerIcon = null;
        private bool flag = false;
        public Hiro_Box()
        {
            InitializeComponent();
            Hiro_Utils.Set_Control_Location(TestTitle, "boxtitle");
            Hiro_Utils.Set_Control_Location(TestLabel, "boxcontent");
            ContentLabel.FontFamily = TestLabel.FontFamily;
            ContentLabel.FontSize = TestLabel.FontSize;
            ContentLabel.FontStretch = TestLabel.FontStretch;
            ContentLabel.FontWeight = TestLabel.FontWeight;
            ContentLabel.FontStyle = TestLabel.FontStyle;
            TitleLabel.FontFamily = TestTitle.FontFamily;
            TitleLabel.FontSize = TestTitle.FontSize;
            TitleLabel.FontStretch = TestTitle.FontStretch;
            TitleLabel.FontWeight = TestTitle.FontWeight;
            TitleLabel.FontStyle = TestTitle.FontStyle;
            var icon = Hiro_Utils.Read_Ini(App.dConfig, "Config", "CustomizeIcon", "");
            icon = Hiro_Utils.Path_Prepare(Hiro_Utils.Path_Prepare_EX(icon));
            if (File.Exists(icon))
            {
                BitmapImage? bi = Hiro_Utils.GetBitmapImage(icon);
                (Resources["PrimaryIcon"] as ImageBrush).ImageSource = bi;
            }
            Load_Color();
            Title = $"{Hiro_Utils.Get_Translate("notitle")} - {App.appTitle}";
            Canvas.SetLeft(this, SystemParameters.FullPrimaryScreenWidth / 2 - Width / 2);
            Canvas.SetTop(this, SystemParameters.FullPrimaryScreenHeight * 9 / 10 - Height);
            Load_One();
            Loaded += (e, args) =>
            {
                Box_In();
            };
        }

        private void Load_One()
        {
            var t = App.noticeitems[0].msg.Replace("\r\n", Environment.NewLine).Replace("\n", Environment.NewLine).Replace("\\n", Environment.NewLine).Replace("<br>", Environment.NewLine);
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
            TitleLabel.Text = App.noticeitems[0].title;
            act = App.noticeitems[0].act;
            if (act != null)
            {
                BaseGrid.Cursor = Cursors.Hand;
            }
            else
            {
                BaseGrid.Cursor = null;
            }
            if (TitleLabel.Text == null || TitleLabel.Text.Equals(string.Empty))
                TitleLabel.Text = Hiro_Utils.Get_Translate("notitle");
            temps = App.noticeitems[0].time;
            Reset_Width();
            Load_Icon();
            App.noticeitems.RemoveAt(0);
            notifications.RemoveAt(0);
        }

        private void Load_Icon()
        {
            Hiro_Icon? icon = App.noticeitems[0].icon;
            if (!flag)
            {
                Set_Icon(icon);
                formerIcon = icon;
                flag = true;
            }
            else
            {
                if (icon != formerIcon)
                {
                    formerIcon = icon;
                    var sb = Hiro_Utils.AddDoubleAnimaton(1, 300, BaseIconBorder, "Opacity", null, 0, 0.7);
                    Set_Icon(icon);
                    sb.Begin();
                }
            }
        }

        private void Set_Icon(Hiro_Icon? icon)
        {
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
                        if (bi != null)
                        {
                            BaseIcon.ImageSource = bi;
                            set = true;
                        }
                    }
                }
            }
            if (!set)
            {
                BaseIcon.ImageSource = (Resources["PrimaryIcon"] as ImageBrush).ImageSource;
            }
        }

        internal void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 250));
        }

        private void Reset_Width()
        {
            ContentLabel.Text = notifications[0].Trim();
            Size fsize = new();
            Size msize = new();
            fsize.Width = 0;
            foreach (var noi in notifications)
            {
                TestLabel.Content = noi;
                Hiro_Utils.Get_Text_Visual_Width(TestLabel, VisualTreeHelper.GetDpi(this).PixelsPerDip, out msize);
                if (msize.Width > fsize.Width)
                    fsize.Width = msize.Width;
            }
            Width = Math.Max(Math.Min(maxWidth, fsize.Width + 3 * innerMargin + Height), 200);
            Canvas.SetLeft(this, SystemParameters.FullPrimaryScreenWidth / 2 - Width / 2);
        }

        private void Box_In()
        {
            if (Hiro_Utils.Read_Ini(App.dConfig, "Config", "HiBoxAudio", "1").Equals("1"))
                try
                {
                    var fileP = Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(App.dConfig, "Config", "BoxAudioPath", "<current>\\system\\sounds\\achievement.wav"));
                    if (!System.IO.File.Exists(fileP))
                        //fileP = Hiro_Utils.Path_Prepare("C:\\Users\\Rex\\Downloads\\Music\\xbox_one_rare_achiev.wav");
                        fileP = Hiro_Utils.Path_Prepare("<win>\\Media\\Windows Notify Messaging.wav");
                    System.Media.SoundPlayer sndPlayer = new(fileP);
                    sndPlayer.Play();
                }
                catch (Exception ex)
                {
                    Hiro_Utils.LogError(ex, "Hiro.Exception.Chat.Sound");
                }
            InnerBorder.Margin = new Thickness(0);
            var sb = Hiro_Utils.AddDoubleAnimaton(Height, boxInLen, OuterBorder, "Height", null, 0, 0.7);
            sb = Hiro_Utils.AddDoubleAnimaton(Height, boxInLen, OuterBorder, "Width", sb, 0, 0.7);
            sb.Completed += (e, args) =>
            {
                OuterBorder.Height = Height;
                OuterBorder.Width = Height;

            };
            BaseGrid.Visibility = Visibility.Visible;
            sb.Begin();
            Task.Delay(200).ContinueWith(t =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    var sb = Hiro_Utils.AddDoubleAnimaton(Height - innerMargin, boxInLen, InnerBorder, "Height", null, 0, 0.7);
                    sb = Hiro_Utils.AddDoubleAnimaton(Height - innerMargin, boxInLen, InnerBorder, "Width", sb, 0, 0.7);
                    sb = Hiro_Utils.AddDoubleAnimaton(45, boxInLen, BaseIconBorder, "Height", sb, 0, 0.7);
                    sb = Hiro_Utils.AddDoubleAnimaton(45, boxInLen, BaseIconBorder, "Width", sb, 0, 0.7);
                    InnerBorder.Height = Height - 5;
                    InnerBorder.Width = Height - 5;
                    sb.Completed += (e, args) =>
                    {
                        Extend_Out();
                    };
                    sb.Begin();
                }));
            });
        }

        private void Extend_Out()
        {
            if (formerTitle == TitleLabel.Text)
            {
                var sb = Hiro_Utils.AddThicknessAnimaton(new Thickness(-Width + InnerBorder.Width + innerMargin, 0, 0, 0), Width - InnerBorder.Width - innerMargin + 100, InnerBorder, "Margin", null, null, 0.7);
                sb = Hiro_Utils.AddDoubleAnimaton(Width, Width - InnerBorder.Width - innerMargin + 100, OuterBorder, "Width", sb, null, 0.7);
                sb.Completed += (e, args) =>
                {
                    InnerBorder.Margin = new Thickness(-Width + InnerBorder.Width + innerMargin, 0, 0, 0);
                    OuterBorder.Width = Width;
                    Content_FadeIn();
                };
                sb.Begin();
            }
            else
            {
                var sb = Hiro_Utils.AddThicknessAnimaton(new Thickness(Width - InnerBorder.Width - innerMargin, 0, 0, 0), Width - InnerBorder.Width - innerMargin + 250, InnerBorder, "Margin", null, null, 0.7);
                sb = Hiro_Utils.AddDoubleAnimaton(Width, Width - InnerBorder.Width - innerMargin + 250, OuterBorder, "Width", sb, null, 0.7);
                sb.Completed += (e, args) =>
                {
                    InnerBorder.Margin = new Thickness(Width - InnerBorder.Width - innerMargin, 0, 0, 0);
                    OuterBorder.Width = Width;
                    Title_FadeIn();
                };
                sb.Begin();
            }
            formerTitle = TitleLabel.Text;

        }

        private void Switch_Infomation()
        {
            var sb = Hiro_Utils.AddThicknessAnimaton(new Thickness(-Width + InnerBorder.Width + innerMargin, 0, 0, 0), 2 * (Width - InnerBorder.Width - innerMargin) + 200, InnerBorder, "Margin", null, null, 0.7);
            sb.Completed += (e, args) =>
            {
                InnerBorder.Margin = new Thickness(-Width + InnerBorder.Width + innerMargin, 0, 0, 0);
                Content_FadeIn();
            };
            sb.Begin();
        }

        private void Title_FadeIn()
        {
            TitleGrid.Margin = new Thickness(innerMargin, 0, 0, 0);
            TitleGrid.Height = Height;
            TitleGrid.Width = Width - innerMargin * 3 - Height;
            var sb = Hiro_Utils.AddDoubleAnimaton(1, fadeInLen, TitleLabel, "Opacity", null, 0, 0.7);
            sb.Completed += (e, args) =>
            {
                TitleLabel.Opacity = 1;
                Task.Delay(textInLen).ContinueWith(t =>
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        Title_FadeOut();
                    }));
                });
            };
            sb.Begin();
        }

        private void Title_FadeOut()
        {
            var sb = Hiro_Utils.AddDoubleAnimaton(0, 250, TitleLabel, "Opacity", null, 1, 0.7);
            sb.Completed += (e, args) =>
            {
                TitleLabel.Opacity = 0;
                Task.Delay(fadeOutLen).ContinueWith(t =>
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        Switch_Infomation();
                    }));
                });
            };
            sb.Begin();
        }

        private void Content_FadeIn()
        {
            ContentGrid.Margin = new Thickness(10 + InnerBorder.Width, 0, 0, 0);
            ContentGrid.Height = Height;
            ContentGrid.Width = Width - innerMargin * 3 - Height;
            var sb = Hiro_Utils.AddDoubleAnimaton(1, fadeInLen, ContentLabel, "Opacity", null, 0, 0.7);
            sb = Hiro_Utils.AddDoubleAnimaton(Width, fadeInLen, OuterBorder, "Width", sb, null, 0.7);
            sb.Completed += (e, args) =>
            {
                ContentLabel.Opacity = 1;
                Task.Delay(temps * 1000).ContinueWith(t =>
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        Content_FadeOut();
                    }));
                });
            };
            sb.Begin();
        }

        private void Content_FadeOut()
        {
            var sb = Hiro_Utils.AddDoubleAnimaton(0, fadeOutLen, ContentLabel, "Opacity", null, 1, 0.7);
            sb.Completed += (e, args) =>
            {
                ContentLabel.Opacity = 0;
                Task.Delay(200).ContinueWith(t =>
                {

                    Dispatcher.Invoke(new Action(() =>
                    {
                        if (notifications.Count > 0)
                        {
                            ContentLabel.Text = notifications[0];
                            notifications.RemoveAt(0);
                            Content_FadeIn();
                        }
                        else
                            Extend_Back();
                    }));
                });
            };
            sb.Begin();
        }

        private void Extend_Back()
        {
            var sb = Hiro_Utils.AddThicknessAnimaton(new Thickness(0), Width - InnerBorder.Width - innerMargin + 100, InnerBorder, "Margin", null, null, 0.7);
            sb = Hiro_Utils.AddDoubleAnimaton(OuterBorder.Height, Width - InnerBorder.Width - innerMargin + 100, OuterBorder, "Width", sb, null, 0.7);
            sb.Completed += (e, args) =>
            {
                InnerBorder.Margin = new Thickness(0, 0, 0, 0);
                OuterBorder.Width = OuterBorder.Height;
                if (App.noticeitems.Count > 0)
                {
                    Load_One();
                    Task.Delay(100).ContinueWith(t =>
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            Extend_Out();
                        }));
                    });
                }
                else
                    Task.Delay(80).ContinueWith(t =>
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            Box_Out();
                        }));
                    });
            };
            sb.Begin();
        }

        private void Box_Out()
        {
            var sb = Hiro_Utils.AddDoubleAnimaton(0, boxOutLen, InnerBorder, "Height", null, null, 0.7);
            sb = Hiro_Utils.AddDoubleAnimaton(0, boxOutLen, InnerBorder, "Width", sb, null, 0.7);
            sb.Completed += (e, args) =>
            {
                InnerBorder.Height = 0;
                InnerBorder.Width = 0;
                App.hiBox = null;
                Close();
            };
            sb.Begin();
            Task.Delay(100).ContinueWith(t =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    var sb = Hiro_Utils.AddDoubleAnimaton(0, boxOutLen, OuterBorder, "Height", null, null, 0.7);
                    sb = Hiro_Utils.AddDoubleAnimaton(0, boxOutLen, OuterBorder, "Width", sb, null, 0.7);
                    sb.Completed += (e, args) =>
                    {
                        Close();
                    };
                    sb.Begin();
                }));
            });
        }

        private void InnerBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (act != null)
                act.Invoke();
            act = null;
            BaseGrid.Cursor = null;
        }

        private void OuterBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (act != null)
                act.Invoke();
            act = null;
            BaseGrid.Cursor = null;
        }
    }
}
