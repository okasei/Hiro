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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using static hiro.Helpers.Hiro_Class;

namespace hiro
{
    /// <summary>
    /// Hiro_Boxie.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Boxie : Window
    {
        internal List<string> notifications = new();
        internal Action? act = null;
        internal int temps = 2;
        private Hiro_Icon? formerIcon = null;
        private bool flag = false;
        double maxWidth = SystemParameters.FullPrimaryScreenWidth / 3;
        const int innerMargin = 10;
        bool finish = false;
        int channel = -1;
        double maxW = 150;
        internal string formerTitle = "";
        public Hiro_Boxie()
        {
            InitializeComponent();
            Load_Color();
            Load_Position();
            Load_Translate();
            Load_PrimaryIcon();
            Load_Icon();
            Helpers.Hiro_UI.SetCustomWindowIcon(this);
            new System.Threading.Thread(() =>
            {
                if (Hiro_Utils.Read_DCIni("HiBoxAudio", "1").Equals("1"))
                    try
                    {
                        var fileP = Hiro_Utils.Read_PPDCIni("BoxAudioPath", "<current>\\system\\sounds\\achievement.wav");
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
                Dispatcher.Invoke(() =>
                {
                    Canvas.SetTop(this, SystemParameters.FullPrimaryScreenHeight * 9 / 10 - Height);
                    Reset_Width(true);
                    Animation_In();
                });
            }).Start();
        }

        private void Load_PrimaryIcon()
        {
            var icon = Hiro_Utils.Read_DCIni("CustomizeIcon", "");
            icon = Hiro_Utils.Path_PPX(icon);
            if (System.IO.File.Exists(icon))
            {
                BitmapImage? bi = Hiro_Utils.GetBitmapImage(icon);
                (Resources["PrimaryIcon"] as ImageBrush).ImageSource = bi;
            }
            else
            {
                icon = Hiro_Utils.Read_PPDCIni("UserAvatar", "");
                if (System.IO.File.Exists(icon) && App.Logined == true)
                {
                    BitmapImage? bi = Hiro_Utils.GetBitmapImage(icon);
                    (Resources["PrimaryIcon"] as ImageBrush).ImageSource = bi;
                }
            }
        }

        private void Animation_In()
        {
            const double t = 0.5;
            const double tt = 0.2;
            Get_Obj_Storyboard(Icon_Background_0, 80, 65, t);
            Get_Obj_Storyboard(Icon_Background_1, 78, 65, t + tt);
            Get_Obj_Storyboard(Icon_Background_2, 74, 65, t + tt * 2);
            Get_Obj_Storyboard(Icon_Background_3, 70, 65, t + tt * 3);
            var sb = Hiro_Utils.AddDoubleAnimaton(1, 1000 * (t + tt * 4), BaseIconBorder, "Opacity", null, 0);
            sb = Hiro_Utils.AddDoubleAnimaton(60, 1000 * (t + tt * 4), BaseIconBorder, "Width", sb, 40);
            sb = Hiro_Utils.AddDoubleAnimaton(60, 1000 * (t + tt * 4), BaseIconBorder, "Height", sb, 40);
            sb.Completed += delegate
            {
                BaseIconBorder.Width = 60;
                BaseIconBorder.Height = 60;
                BaseIconBorder.Opacity = 1;
                sb = null;
                finish = true;
                Load_One();
            };
            sb.Begin();
        }

        internal void Load_Position()
        {
            Hiro_Utils.Set_Control_Location(Test_Text_Display, "BoxText", location: false);
            Text_Display_0.FontFamily = Test_Text_Display.FontFamily;
            Text_Display_0.FontSize = Test_Text_Display.FontSize;
            Text_Display_0.FontStretch = Test_Text_Display.FontStretch;
            Text_Display_0.FontWeight = Test_Text_Display.FontWeight;
            Text_Display_0.FontStyle = Test_Text_Display.FontStyle;
            Text_Display_1.FontFamily = Test_Text_Display.FontFamily;
            Text_Display_1.FontSize = Test_Text_Display.FontSize;
            Text_Display_1.FontStretch = Test_Text_Display.FontStretch;
            Text_Display_1.FontWeight = Test_Text_Display.FontWeight;
            Text_Display_1.FontStyle = Test_Text_Display.FontStyle;
            Hiro_Utils.Set_Control_Location(Hiro_Title_Label, "BoxFinal", location: false);
            Hiro_Utils.Set_Control_Location(Hiro_Extension_Title_Label, "BoxFinal", location: false);
            Hiro_Title.FontFamily = Hiro_Title_Label.FontFamily;
            Hiro_Title.FontSize = Hiro_Title_Label.FontSize;
            Hiro_Title.FontStretch = Hiro_Title_Label.FontStretch;
            Hiro_Title.FontWeight = Hiro_Title_Label.FontWeight;
            Hiro_Title.FontStyle = Hiro_Title_Label.FontStyle;
            Hiro_Extension_Title.FontFamily = Hiro_Extension_Title_Label.FontFamily;
            Hiro_Extension_Title.FontSize = Hiro_Extension_Title_Label.FontSize;
            Hiro_Extension_Title.FontStretch = Hiro_Extension_Title_Label.FontStretch;
            Hiro_Extension_Title.FontWeight = Hiro_Extension_Title_Label.FontWeight;
            Hiro_Extension_Title.FontStyle = Hiro_Extension_Title_Label.FontStyle;
        }
        internal void Load_Translate()
        {
            Hiro_Title.Text = Hiro_Utils.Path_PPX(Hiro_Utils.Get_Translate("BoxFinalLeft"));
            Hiro_Extension_Title.Text = Hiro_Utils.Path_PPX(Hiro_Utils.Get_Translate("BoxFinalRight"));
        }

        private void Animation_Out()
        {
            if (act != null)
            {
                Hiro_Title.Text = Hiro_Utils.Path_PPX(Hiro_Utils.Get_Translate("BoxFinalActLeft"));
                Hiro_Extension_Title.Text = Hiro_Utils.Path_PPX(Hiro_Utils.Get_Translate("BoxFinalActRight"));
            }
            else
            {
                act = new(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        Hiro_Utils.RunExe("show()", Hiro_Resources.ApplicationName, false);
                    });
                });
                BaseGrid.Cursor = Cursors.Hand;
            }
            var s = Hiro_Utils.AddThicknessAnimaton(new Thickness(0), 350, IconGrid, "Margin", null);
            Hiro_Utils.Set_FrameworkElement_Location(LocationGrid, "BoxPress");
            var w = LocationGrid.Width;
            Width = Math.Max(Width, w);
            Canvas.SetLeft(this, SystemParameters.FullPrimaryScreenWidth / 2 - Width / 2);
            s = Hiro_Utils.AddDoubleAnimaton(w, 300 + 0.5 * Math.Abs(Width - w), BackgroundBorder, "Width", s, BackgroundBorder.Width > 100 ? null : 100);
            s.Completed += delegate
            {
                IconGrid.Margin = new Thickness(0);
                //Width = w;
                //Canvas.SetLeft(this, SystemParameters.FullPrimaryScreenWidth / 2 - Width / 2);
                BackgroundBorder.Width = w;
                Title_Grid.Width = w / 2 - Icon_Background_3.Width / 2;
                Title_Grid.Margin = new Thickness(Width / 2 - w / 2, 0, 0, 0);
                Extension_Grid.Width = w / 2 - Icon_Background_3.Width / 2;
                Extension_Grid.Margin = new Thickness(Width / 2 + Icon_Background_3.Width / 2, 0, 0, 0);
                Title_Grid.Visibility = Visibility.Visible;
                Extension_Grid.Visibility = Visibility.Visible;
                var s = Hiro_Utils.AddThicknessAnimaton(new Thickness(0), 450, Hiro_Title, "Margin", null, new Thickness(-100, 0, 0, 0));
                s = Hiro_Utils.AddThicknessAnimaton(new Thickness(0), 450, Hiro_Extension_Title, "Margin", s, new Thickness(100, 0, 0, 0));
                s = Hiro_Utils.AddDoubleAnimaton(1, 250, Hiro_Title, "Opacity", s, 0);
                s = Hiro_Utils.AddDoubleAnimaton(1, 250, Hiro_Extension_Title, "Opacity", s, 0);
                s.Completed += delegate
                {
                    Hiro_Title.Margin = new Thickness(0);
                    Hiro_Extension_Title.Margin = new Thickness(0);
                    Hiro_Title.Opacity = 1;
                    Hiro_Extension_Title.Opacity = 1;
                    Task.Delay(1800).ContinueWith(t =>
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            GoOut();
                        }));
                    });
                };
                s.Begin();
            };
            s.Begin();
        }

        private void GoOut()
        {
            var s = Hiro_Utils.AddDoubleAnimaton(0, 350, Title_Grid, "Opacity", null);
            s = Hiro_Utils.AddDoubleAnimaton(0, 350, Extension_Grid, "Opacity", s);
            s.Completed += delegate
            {
                Title_Grid.Visibility = Visibility.Collapsed;
                Extension_Grid.Visibility = Visibility.Collapsed;
                var t = Hiro_Utils.AddDoubleAnimaton(50, 450, BackgroundBorder, "Width", null);
                t = Hiro_Utils.AddDoubleAnimaton(0, 450, BackgroundBorder, "Opacity", t);
                t.Completed += delegate
                {
                    BackgroundBorder.Visibility = Visibility.Collapsed;
                    const double t = 0.5;
                    const double tt = 0.1;
                    Icon_Background_2.Visibility = Visibility.Collapsed;
                    Icon_Background_1.Visibility = Visibility.Collapsed;
                    Get_Obj_Storyboard(Icon_Background_3, 0, 0, t + tt);
                    Get_Obj_Storyboard(Icon_Background_0, 0, 0, t + tt * 2);
                    var o = Hiro_Utils.AddDoubleAnimaton(0, 700, Icon_Background_0, "Opacity", null);
                    o = Hiro_Utils.AddDoubleAnimaton(0, 700, Icon_Background_3, "Opacity", o);
                    var p = Hiro_Utils.AddDoubleAnimaton(0, 400, BaseIconBorder, "Width", null);
                    p = Hiro_Utils.AddDoubleAnimaton(0, 400, BaseIconBorder, "Height", p);
                    p = Hiro_Utils.AddDoubleAnimaton(0, 200, BaseIconBorder, "Opacity", p);
                    p.Completed += delegate
                    {
                        BaseIconBorder.Visibility = Visibility.Collapsed;
                    };
                    p.Begin();
                    o.Completed += delegate
                    {
                        App.hiBoxie = null;
                        if (App.noticeitems.Count > 0)
                        {
                            App.hiBoxie = new();
                            App.hiBoxie.Show();
                        }
                        Close();
                        o = null;
                    };
                    o.Begin();
                };
                t.Begin();
            };
            s.Begin();
        }

        private Storyboard Get_Obj_Storyboard(Border obj, double value1, double value2, double time)
        {
            var sb0 = Get_StoryBoard(obj, value1, value2, time, "Width", null);
            sb0 = Get_StoryBoard(obj, value1, value2, time, "Height", sb0);
            sb0.Begin();
            return sb0;
        }

        private Storyboard Get_StoryBoard(Border obj, double value1, double value2, double time, string propertyPath, Storyboard? sb = null)
        {
            sb ??= new();
            DoubleAnimationUsingKeyFrames ani = new();
            ani.Duration = TimeSpan.FromSeconds(time);
            ani.KeyFrames.Add(new SplineDoubleKeyFrame(value1, KeyTime.FromPercent(0.9)));
            ani.KeyFrames.Add(new SplineDoubleKeyFrame(value2, KeyTime.FromPercent(1)));
            Storyboard.SetTarget(ani, obj);
            Storyboard.SetTargetProperty(ani, new PropertyPath(propertyPath));
            sb.Children.Add(ani);
            return sb;
        }

        internal void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppAccent"] = new SolidColorBrush(App.AppAccentColor);
            Resources["AppAccentTrans"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 250));
        }

        private void Load_One()
        {
            channel = -1;
            var t = App.noticeitems[0].msg.Replace("\r\n", Environment.NewLine).Replace("\r", Environment.NewLine).Replace("\n", Environment.NewLine).Replace("\\n", Environment.NewLine).Replace("<br>", Environment.NewLine);
            Load_Icon();
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
            var tt = App.noticeitems[0].title?.Trim();
            if (tt == null || tt.Equals(string.Empty))
                tt = Hiro_Utils.Get_Translate("notitle");
            Title = tt + " - " + Hiro_Resources.ApplicationName;
            act = App.noticeitems[0].act;
            BaseGrid.Cursor = act != null ? BaseGrid.Cursor = Cursors.Hand : null;
            temps = App.noticeitems[0].time;
            maxW = 150;
            foreach (var w in notifications)
            {
                Size msize = new();
                Test_Text_Display.Content = w;
                Hiro_Utils.Get_Text_Visual_Width(Test_Text_Display, VisualTreeHelper.GetDpi(this).PixelsPerDip, out msize);
                maxW = Math.Max(maxW, msize.Width);
            }
            Size tsize = new();
            if (tt.Equals(formerTitle))
            {
                Test_Text_Display.Content = notifications[0];
                notifications.RemoveAt(0);
            }
            else
            {
                Test_Text_Display.Content = tt;
                formerTitle = tt;
            }
            Hiro_Utils.Get_Text_Visual_Width(Test_Text_Display, VisualTreeHelper.GetDpi(this).PixelsPerDip, out tsize);
            maxW = Math.Max(maxW, tsize.Width);
            Reset_Width(true);
            App.noticeitems.RemoveAt(0);
        }
        private void HiroGoLeft(double time, double targetWidth)
        {
            var s = Hiro_Utils.AddThicknessAnimaton(new Thickness(Icon_Background_3.Width - targetWidth + 5, 0, 0, 0), time, IconGrid, "Margin", null);
            s.Completed += delegate
            {
                IconGrid.Margin = new Thickness(Icon_Background_3.Width - targetWidth + 5, 0, 0, 0);
                s = null;
            };
            s.Begin();
        }

        private void GoText()
        {
            Text_Display_0.Width = Text_Grid.Width;
            Text_Display_1.Width = Text_Grid.Width;
            switch (channel)
            {
                case -1:
                    {
                        channel = 0;
                        Text_Display_0.Text = Test_Text_Display.Content.ToString();
                        var s = Hiro_Utils.AddThicknessAnimaton(new Thickness(0), 600, Text_Display_0, "Margin", null, new Thickness(0, 150, 0, 0));
                        s = Hiro_Utils.AddDoubleAnimaton(1, 400, Text_Display_0, "Opacity", s, 0);
                        s.Completed += delegate
                        {
                            Text_Display_0.Margin = new Thickness(0);
                            Text_Display_0.Opacity = 1;
                            Text_Display_1.Margin = new Thickness(0, 150, 0, 0);
                            Text_Display_1.Opacity = 0;
                        };
                        s.Begin();
                        //未初始化
                        break;
                    }
                case 0:
                    {
                        channel = 1;
                        Text_Display_1.Text = Test_Text_Display.Content.ToString();
                        var s = Hiro_Utils.AddThicknessAnimaton(new Thickness(0), 600, Text_Display_1, "Margin", null, null);
                        s = Hiro_Utils.AddDoubleAnimaton(1, 400, Text_Display_1, "Opacity", s, 0);
                        s = Hiro_Utils.AddThicknessAnimaton(new Thickness(0, -Text_Grid.ActualHeight - 10, 0, 0), 600, Text_Display_0, "Margin", s, null);
                        s = Hiro_Utils.AddDoubleAnimaton(0, 400, Text_Display_0, "Opacity", s, 1);
                        s.Completed += delegate
                        {
                            Text_Display_1.Margin = new Thickness(0);
                            Text_Display_1.Opacity = 1;
                            Text_Display_0.Margin = new Thickness(0, 150, 0, 0);
                            Text_Display_0.Opacity = 0;
                        };
                        s.Begin();
                        break;
                    }
                case 1:
                    {
                        channel = 0;
                        Text_Display_0.Text = Test_Text_Display.Content.ToString();
                        var s = Hiro_Utils.AddThicknessAnimaton(new Thickness(0), 600, Text_Display_0, "Margin", null, null);
                        s = Hiro_Utils.AddDoubleAnimaton(1, 400, Text_Display_0, "Opacity", s, 0);
                        s = Hiro_Utils.AddThicknessAnimaton(new Thickness(0, -Text_Grid.ActualHeight - 10, 0, 0), 600, Text_Display_1, "Margin", s, null);
                        s = Hiro_Utils.AddDoubleAnimaton(0, 400, Text_Display_1, "Opacity", s, 1);
                        s.Completed += delegate
                        {
                            Text_Display_0.Margin = new Thickness(0);
                            Text_Display_0.Opacity = 1;
                            Text_Display_1.Margin = new Thickness(0, 150, 0, 0);
                            Text_Display_1.Opacity = 0;
                        };
                        s.Begin();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            Task.Delay(temps * 1000).ContinueWith(t =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    Reset_Width();
                }));
            });
        }

        private void Load_Icon()
        {
            Hiro_Icon? icon = App.noticeitems.Count > 0 ? App.noticeitems[0].icon : null;
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
                    var iconLocation = Hiro_Utils.Path_PPX(icon.Location);
                    if (System.IO.File.Exists(iconLocation))
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

        private double Reset_Width(bool first = false)
        {
            if (!first)
            {
                if (notifications.Count > 0)
                {
                    Test_Text_Display.Content = notifications[0];
                    notifications.RemoveAt(0);
                }
                else
                {
                    if (App.noticeitems.Count > 0)
                    {
                        switch (channel)
                        {
                            case 0:
                                {
                                    var s = Hiro_Utils.AddThicknessAnimaton(new Thickness(0, -Text_Grid.ActualHeight - 10, 0, 0), 600, Text_Display_0, "Margin", null, null);
                                    s = Hiro_Utils.AddDoubleAnimaton(0, 400, Text_Display_0, "Opacity", s, 1);
                                    s.Completed += delegate
                                    {
                                        Text_Display_0.Margin = new Thickness(0, 150, 0, 0);
                                        Text_Display_0.Opacity = 0;
                                        Load_One();
                                    };
                                    s.Begin();
                                    break;
                                }
                            case 1:
                                {
                                    channel = 0;
                                    Text_Display_0.Text = Test_Text_Display.Content.ToString();
                                    var s = Hiro_Utils.AddThicknessAnimaton(new Thickness(0, -Text_Grid.ActualHeight - 10, 0, 0), 600, Text_Display_1, "Margin", null, null);
                                    s = Hiro_Utils.AddDoubleAnimaton(0, 400, Text_Display_1, "Opacity", s, 1);
                                    s.Completed += delegate
                                    {
                                        Text_Display_1.Margin = new Thickness(0, 150, 0, 0);
                                        Text_Display_1.Opacity = 0;
                                        Load_One();
                                    };
                                    s.Begin();
                                    break;
                                }
                            default:
                                {
                                    Load_One();
                                    break;
                                }
                        }
                    }
                    else
                    {
                        var s = Hiro_Utils.AddDoubleAnimaton(0, 350, Text_Grid, "Opacity", null);
                        s.Completed += delegate
                        {
                            Text_Grid.Visibility = Visibility.Collapsed;
                            Animation_Out();
                        };
                        s.Begin();
                    }
                    return 0;
                }
            }
            var w = Math.Min(maxWidth, Math.Max(maxW + 3 * innerMargin + Icon_Background_3.Width, maxW));
            Width = Math.Max(w, Width);
            Canvas.SetLeft(this, SystemParameters.FullPrimaryScreenWidth / 2 - Width / 2);
            if (finish)
            {
                Dispatcher.Invoke(() =>
                {
                    if (first)
                        HiroGoLeft(350 + 3 * Math.Abs(Width - w), w);
                    var s = Hiro_Utils.AddDoubleAnimaton(w, 350 + 3 * Math.Abs(Width - w), BackgroundBorder, "Width", null, BackgroundBorder.Width > 100 ? null : 100);
                    if (BackgroundBorder.Opacity < 1)
                        s = Hiro_Utils.AddDoubleAnimaton(1, 350, BackgroundBorder, "Opacity", s);
                    s.Completed += delegate
                    {
                        BackgroundBorder.Width = w;
                        BackgroundBorder.Opacity = 1;
                        Width = w;
                        Canvas.SetLeft(this, SystemParameters.FullPrimaryScreenWidth / 2 - Width / 2);
                        Text_Grid.Visibility = Visibility.Visible;
                        Text_Grid.Margin = new Thickness(Icon_Background_3.Width + 10, 0, 0, 0);
                        Text_Grid.Width = w - Text_Grid.Margin.Left - 10;
                        Text_Grid.Height = Height;
                        GoText();
                    };
                    s.Begin();
                });
                return 350 + 3 * Math.Abs(Width - w);
            }
            return 0;
        }

        private void BaseGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (act != null)
            {
                act.Invoke();
                act = null;
            }
            BaseGrid.Cursor = null;
        }
    }
}
