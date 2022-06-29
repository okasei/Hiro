using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Vlc.DotNet.Wpf;
using System.Windows.Media;
using System.Windows.Input;
using System.Text;
using System.Windows.Media.Animation;

namespace hiro
{
    /// <summary>
    /// Hiro_Player.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Player : Window
    {
        internal VlcVideoSourceProvider? hiro_provider = null;
        internal string? toplay = null;
        private int bflag = 0;
        private int rflag = 1;
        private Size mSize = new(450, 800);
        private int cflag = 1;
        public Hiro_Player(string? play = null)
        {
            InitializeComponent();
            toplay = play;
            Title = App.AppTitle;
            Loaded += delegate
            {
                Load_Color();
                Load_Translate();
                Loadbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
                Initialize_Player();
                Update_Progress();
                Focus();
                Update_Layout();
            };
        }

        private void Initialize_Player()
        {
            new System.Threading.Thread(() =>
            {
                try
                {
                    Object? obj = null;
                    Dispatcher.Invoke(() =>
                    {
                        obj = Player_Container.Tag;
                    });
                    hiro_provider ??= new(Dispatcher);
                    if (obj == null)
                    {
                        hiro_provider.CreatePlayer(new(@Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare("<current>")) + @"\runtimes\win-vlc"), new[] { "" });
                        Player_Container.Dispatcher.Invoke(() => {
                            Player_Container.SetBinding(Image.SourceProperty,
                            new Binding(nameof(VlcVideoSourceProvider.VideoSource)) { Source = hiro_provider });
                        });
                    }
                    hiro_provider.MediaPlayer.Audio.IsMute = false;
                    hiro_provider.MediaPlayer.EndReached += MediaPlayer_EndReached;
                    hiro_provider.MediaPlayer.LengthChanged += MediaPlayer_LengthChanged;
                    hiro_provider.MediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
                    if (toplay != null)
                    {
                        Play(toplay);
                    }
                }
                catch (Exception ex)
                {
                    Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.MediaPlayer.Initialize: " + ex.Message);
                }
            }).Start();
        }

        private void MediaPlayer_PositionChanged(object? sender, Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs e)
        {
            Update_Progress();
        }

        private void Update_Progress()
        {
            Dispatcher.Invoke(() =>
            {
                FrameworkElement? pnlClient = Content as FrameworkElement;
                if (hiro_provider == null)
                {
                    Ctrl_Progress.Width = 0;
                    Ctrl_Time.Content = "00:00";
                }
                else
                {
                    if (Player_Container.Tag != null)
                    {
                        var tag = (string)Player_Container.Tag;
                        if (tag.Equals("Playing") || tag.Equals("Paused"))
                        {
                            var wid = Ctrl_Progress_Bg.Width * hiro_provider.MediaPlayer.Position;
                            Ctrl_Progress.Width = wid >= 0 ? wid : 0;
                            Ctrl_Time.Content = ParseDuration(hiro_provider.MediaPlayer.Position * hiro_provider.MediaPlayer.GetMedia().Duration.TotalSeconds) + "/" + ParseDuration(hiro_provider.MediaPlayer.GetMedia().Duration.TotalSeconds);
                            if (pnlClient != null)
                            {
                                Ctrl_Progress_Bg.Width = pnlClient.ActualWidth - Ctrl_Time.Margin.Right - Ctrl_Time.ActualWidth - 15;
                            }
                            return;
                        }
                    }
                    Ctrl_Time.Content = "00:00";
                    Ctrl_Progress.Width = 0;
                }
                if (pnlClient != null)
                {
                    Ctrl_Progress_Bg.Width = pnlClient.ActualWidth - Ctrl_Time.Margin.Right - Ctrl_Time.ActualWidth - 15;
                }
            });
        }

        private static string ParseDuration(double time)
        {
            StringBuilder sb = new();
            if (time < 3600)
            {
                var a = Convert.ToInt32(Math.Floor(time / 60)).ToString();
                a = a.Length == 2 ? a : "0" + a;
                sb.Append(a);
                time %= 60;
                sb.Append(":");
                a = Convert.ToInt32(Math.Floor(time)).ToString();
                a = a.Length == 2 ? a : "0" + a;
                sb.Append(a);
            }
            else
            {
                var a = Convert.ToInt32(Math.Floor(time / 3600)).ToString();
                a = a.Length == 2 ? a : "0" + a;
                sb.Append(a);
                time = time % 3600;
                sb.Append(":");
                a = Convert.ToInt32(Math.Floor(time / 60)).ToString();
                a = a.Length == 2 ? a : "0" + a;
                sb.Append(a);
                time = time % 60;
                sb.Append(":");
                a = Convert.ToInt32(Math.Floor(time)).ToString();
                a = a.Length == 2 ? a : "0" + a;
                sb.Append(a);
            }
            return sb.ToString();
        }

        private void ParseCommand()
        {
            var txt = Ctrl_Text.Text;
            if (hiro_provider != null)
            {
                if (txt.StartsWith("hiro.vol:"))
                {
                    hiro_provider.MediaPlayer.Audio.Volume = Convert.ToInt32(txt[9..]);
                    Title = txt[9..];
                }
                else if (txt.StartsWith("hiro.speed:"))
                {
                    hiro_provider.MediaPlayer.Rate = float.Parse(txt[11..]);
                }
                else
                {
                    Play(txt);
                }
            }
            
        }

        private void MediaPlayer_LengthChanged(object? sender, Vlc.DotNet.Core.VlcMediaPlayerLengthChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Player_Container.Visibility = Visibility.Visible;
                Player_Container.Tag = "Playing";
                Update_Progress();
            });
        }

        internal void Play(string uri)
        {
            if (hiro_provider != null)
            {
                try
                {
                    hiro_provider.MediaPlayer.Play(new Uri(uri));
                    Dispatcher.Invoke(() =>
                    {
                        Ctrl_Text.Text = uri;
                        Title = Hiro_Utils.GetFileName(uri) + " - " + App.AppTitle;
                        Player_Container.Visibility = Visibility.Visible;
                        Player_Container.Tag = "Playing";
                        Update_Progress();
                    });
                }
                catch(Exception ex)
                {
                    Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.MediaPlayer.Play Details: " + ex.Message);
                }
            }
        }

        internal void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeReverse"] = App.AppForeColor == Colors.White ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 80));
            Resources["AppForeDimColor"] = Hiro_Utils.Color_Transparent(App.AppForeColor, 80);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 80));
            Resources["AppAccentDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 20));
        }

        public void Loadbgi(int direction,bool? animation = null)
        {
            if (bflag == 1)
                return;
            bflag = 1;
            Hiro_Utils.Set_Bgimage(bgimage, this);
            bool ani = animation == null ? !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0") : (bool)animation;
            Hiro_Utils.Blur_Animation(direction, ani, bgimage, this);
            bflag = 0;
        }

        private void MediaPlayer_EndReached(object? sender, Vlc.DotNet.Core.VlcMediaPlayerEndReachedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Player_Container.Visibility = Visibility.Hidden;
                Player_Container.Tag = "End";
                Ctrl_Progress.Width = 0;
                Title = App.AppTitle;
                Ctrl_Time.Content = "00:00";
                Update_Progress();
            });
        }

        private void Ctrl_Progress_Bg_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == e.LeftButton)
            {
                if (hiro_provider != null && Player_Container.Tag != null && (((string)Player_Container.Tag).Equals("Playing") || ((string)Player_Container.Tag).Equals("Paused")))
                {
                    var a = e.GetPosition(Ctrl_Progress_Bg);
                    hiro_provider.MediaPlayer.Position = (float)(a.X / Ctrl_Progress_Bg.Width);
                    Update_Progress();
                }
            }
        }

        private void Ctrl_Progress_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == e.LeftButton)
            {
                if (hiro_provider != null && Player_Container.Tag != null && (((string)Player_Container.Tag).Equals("Playing") || ((string)Player_Container.Tag).Equals("Paused")))
                {
                    var a = e.GetPosition(Ctrl_Progress);
                    hiro_provider.MediaPlayer.Position = (float)(a.X / Ctrl_Progress_Bg.Width);
                    Update_Progress();
                }
            }
        }

        private void Hiro_Player1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Update_Progress();
            Loadbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")), false);
            if (rflag == 1)
            {
                mSize.Width = Width;
                mSize.Height = Height;
            }
        }

        private void Bgimage_Drop(object sender, DragEventArgs e)
        {
            DealDragEventArgs(e);
        }

        private void DealDragEventArgs(DragEventArgs e)
        {
            var data = e.Data;
            var formats = data.GetData(DataFormats.FileDrop).GetType().ToString();
            if (formats.Equals("System.String[]"))
            {
                var info = e.Data.GetData(DataFormats.FileDrop) as String[];
                if (info != null && info.Length > 0)
                    Play(info[0]);
            }
            else if (formats.Equals("System.String"))
            {
                var info = e.Data.GetData(DataFormats.FileDrop) as String;
                if (info != null)
                    Play(info);
            }
        }

        private void Player_Container_Drop(object sender, DragEventArgs e)
        {
            DealDragEventArgs(e);
        }

        private void Hiro_Player1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (cflag == 1)
                {
                    new System.Threading.Thread(() =>
                    {
                        cflag = 2;
                        hiro_provider?.Dispose();
                        hiro_provider = null;
                        cflag = 0;
                        Dispatcher.Invoke(() =>
                        {
                            Close();
                        });
                    }).Start();
                    e.Cancel = true;
                }
                else if(cflag == 2)
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.MediaPlay.Close Details: " + ex.Message);
            }
        }

        private void Player_Cover_Drop(object sender, DragEventArgs e)
        {
            DealDragEventArgs(e);
        }

        private void Player_Container_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PlayPause();
        }

        private void Bgimage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PlayPause();
        }

        private void PlayPause()
        {
            if (Player_Container.Tag != null && hiro_provider != null)
            {
                if (((string)Player_Container.Tag).Equals("Playing"))
                {
                    if (hiro_provider.MediaPlayer.IsPausable())
                    {
                        hiro_provider.MediaPlayer.Pause();
                        Player_Container.Tag = "Paused";
                    }
                }
                else if (((string)Player_Container.Tag).Equals("Paused"))
                {
                    if (hiro_provider.MediaPlayer.CouldPlay)
                    {
                        Player_Container.Tag = "Playing";
                        hiro_provider.MediaPlayer.Play();
                    }
                }
            }
        }

        private void FullScreen()
        {
            rflag = 0;
            if (WindowStyle == WindowStyle.None)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                ResizeMode = ResizeMode.CanResize;
                Width = mSize.Width;
                Height = mSize.Height;
                Chrome.NonClientFrameEdges = (System.Windows.Shell.NonClientFrameEdges)13;
                if (hiro_provider != null)
                {
                    hiro_provider.MediaPlayer.Video.FullScreen = false;
                }
                Ctrl_Btns.Visibility = Controller.Visibility;
                Move_Label.Visibility = Controller.Visibility;
                rflag = 1;
            }
            else
            {
                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
                WindowState = WindowState.Normal;
                Width = SystemParameters.PrimaryScreenWidth;
                Height = SystemParameters.PrimaryScreenHeight;
                Chrome.NonClientFrameEdges = (System.Windows.Shell.NonClientFrameEdges)0;
                if (hiro_provider != null)
                {
                    hiro_provider.MediaPlayer.Video.FullScreen = true;
                }
                Ctrl_Btns.Visibility = Visibility.Hidden;
                Move_Label.Visibility = Visibility.Hidden;
            }
            Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - Width / 2);
            Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - Height / 2);
            Update_Layout();
        }


        private void Bgimage_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FullScreen();
        }

        private void Player_Cover_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FullScreen();
        }

        private void Player_Cover_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Right))
            {
                if (hiro_provider != null && Player_Container.Tag != null && (((string)Player_Container.Tag).Equals("Playing") || ((string)Player_Container.Tag).Equals("Paused")))
                {
                    if (hiro_provider.MediaPlayer.Position + 0.01 < 1)
                        hiro_provider.MediaPlayer.Position = (float)(hiro_provider.MediaPlayer.Position + 0.01);
                    Update_Progress();
                    e.Handled = true;
                }
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.Left))
            {
                if (hiro_provider != null && Player_Container.Tag != null && (((string)Player_Container.Tag).Equals("Playing") || ((string)Player_Container.Tag).Equals("Paused")))
                {
                    if (hiro_provider.MediaPlayer.Position - 0.01 > 0)
                        hiro_provider.MediaPlayer.Position = (float)(hiro_provider.MediaPlayer.Position - 0.01);
                    Update_Progress();
                    e.Handled = true;
                }
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.Space))
            {
                PlayPause();
                e.Handled = true;
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.Enter))
            {
                FullScreen();
                e.Handled = true;
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.Escape))
            {
                if (WindowStyle == WindowStyle.None)
                {
                    FullScreen();
                    e.Handled = true;
                }
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.I) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (Controller.Visibility == Visibility.Hidden)
                {
                    if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
                    {
                        Controller.Visibility = Visibility.Visible;
                        Storyboard sb = new();
                        if (WindowStyle != WindowStyle.None)
                        {
                            Ctrl_Btns.Visibility = Visibility.Visible;
                            Move_Label.Visibility = Visibility.Visible;
                            Hiro_Utils.AddPowerAnimation(1, Ctrl_Btns, sb, -30, null);
                            Hiro_Utils.AddPowerAnimation(1, Move_Label, sb, -30, null);
                        }
                        Hiro_Utils.AddPowerAnimation(3, Controller, sb, -30, null);
                        sb.Begin();
                    }
                    else
                    {
                        Controller.Visibility = Visibility.Visible;
                        if (WindowStyle != WindowStyle.None)
                        {
                            Ctrl_Btns.Visibility = Visibility.Visible;
                            Move_Label.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
                    {
                        Controller.Visibility = Visibility.Visible;
                        Storyboard sb = new();
                        if (WindowStyle != WindowStyle.None)
                        {
                            Ctrl_Btns.Visibility = Visibility.Visible;
                            Move_Label.Visibility = Visibility.Visible;
                            Hiro_Utils.AddPowerAnimation(1, Ctrl_Btns, sb, null, -30);
                            Hiro_Utils.AddPowerAnimation(1, Move_Label, sb, null, -30);
                        }
                        Hiro_Utils.AddPowerAnimation(3, Controller, sb, null, -30);
                        sb.Completed += delegate
                        {
                            Controller.Visibility = Visibility.Hidden;
                            Ctrl_Btns.Visibility = Visibility.Hidden;
                            Move_Label.Visibility = Visibility.Hidden;
                        };
                        sb.Begin();
                    }
                    else
                    {
                        Controller.Visibility = Visibility.Hidden;
                        Ctrl_Btns.Visibility = Visibility.Hidden;
                        Move_Label.Visibility = Visibility.Hidden;
                    }
                    e.Handled = true;
                }
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.O) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (Ctrl_Address.Visibility == Visibility.Hidden)
                {
                    if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
                    {
                        Ctrl_Address.Visibility = Visibility.Visible;
                        Storyboard sb = new();
                        Hiro_Utils.AddPowerAnimation(1, Ctrl_Address, sb, -30, null);
                        sb.Completed += delegate
                        {
                            Ctrl_Address.Focus();
                        };
                        sb.Begin();
                    }
                    else
                    {
                        Ctrl_Address.Visibility = Visibility.Visible;
                        Ctrl_Address.Focus();
                    }
                }
                else
                {
                    if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
                    {
                        Ctrl_Address.Visibility = Visibility.Visible;
                        Storyboard sb = new();
                        Hiro_Utils.AddPowerAnimation(1, Ctrl_Address, sb, null, -30);
                        sb.Completed += delegate
                        {
                            Ctrl_Address.Visibility = Visibility.Hidden;
                        };
                        sb.Begin();
                    }
                    else
                        Ctrl_Address.Visibility = Visibility.Hidden;
                }
                e.Handled = true;
            }
        }

        private void Hiro_Player1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Right))
            {
                if (hiro_provider != null && Player_Container.Tag != null && (((string)Player_Container.Tag).Equals("Playing") || ((string)Player_Container.Tag).Equals("Paused")))
                {
                    if (hiro_provider.MediaPlayer.Position + 0.01 < 1)
                        hiro_provider.MediaPlayer.Position = (float)(hiro_provider.MediaPlayer.Position + 0.01);
                    Update_Progress();
                    e.Handled = true;
                }
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.Left))
            {
                if (hiro_provider != null && Player_Container.Tag != null && (((string)Player_Container.Tag).Equals("Playing") || ((string)Player_Container.Tag).Equals("Paused")))
                {
                    if (hiro_provider.MediaPlayer.Position - 0.01 > 0)
                        hiro_provider.MediaPlayer.Position = (float)(hiro_provider.MediaPlayer.Position - 0.01);
                    Update_Progress();
                    e.Handled = true;
                }
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.Space))
            {
                PlayPause();
                e.Handled = true;
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.Enter))
            {
                FullScreen();
                e.Handled = true;
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.Escape))
            {
                if (WindowStyle == WindowStyle.None)
                {
                    FullScreen();
                    e.Handled = true;
                }
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.I) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (Controller.Visibility == Visibility.Hidden)
                {
                    if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
                    {
                        Controller.Visibility = Visibility.Visible;
                        Storyboard sb = new();
                        if (WindowStyle != WindowStyle.None)
                        {
                            Ctrl_Btns.Visibility = Visibility.Visible;
                            Move_Label.Visibility = Visibility.Visible;
                            Hiro_Utils.AddPowerAnimation(1, Ctrl_Btns, sb, -30, null);
                            Hiro_Utils.AddPowerAnimation(1, Move_Label, sb, -30, null);
                        }
                        Hiro_Utils.AddPowerAnimation(3, Controller, sb, -30, null);
                        sb.Begin();
                    }
                    else
                    {
                        Controller.Visibility = Visibility.Visible;
                        if (WindowStyle != WindowStyle.None)
                        {
                            Ctrl_Btns.Visibility = Visibility.Visible;
                            Move_Label.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
                    {
                        Controller.Visibility = Visibility.Visible;
                        Storyboard sb = new();
                        if (WindowStyle != WindowStyle.None)
                        {
                            Ctrl_Btns.Visibility = Visibility.Visible;
                            Move_Label.Visibility = Visibility.Visible;
                            Hiro_Utils.AddPowerAnimation(1, Ctrl_Btns, sb, null, -30);
                            Hiro_Utils.AddPowerAnimation(1, Move_Label, sb, null, -30);
                        }
                        Hiro_Utils.AddPowerAnimation(3, Controller, sb, null, -30);
                        sb.Completed += delegate
                        {
                            Controller.Visibility = Visibility.Hidden;
                            Ctrl_Btns.Visibility = Visibility.Hidden;
                            Move_Label.Visibility = Visibility.Hidden;
                        };
                        sb.Begin();
                    }
                    else
                    {
                        Controller.Visibility = Visibility.Hidden;
                        Ctrl_Btns.Visibility = Visibility.Hidden;
                        Move_Label.Visibility = Visibility.Hidden;
                    }
                    e.Handled = true;
                }
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.O) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (Ctrl_Address.Visibility == Visibility.Hidden)
                {
                    if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
                    {
                        Ctrl_Address.Visibility = Visibility.Visible;
                        Storyboard sb = new();
                        Hiro_Utils.AddPowerAnimation(1, Ctrl_Address, sb, -30, null);
                        sb.Completed += delegate
                        {
                            Ctrl_Address.Focus();
                        };
                        sb.Begin();
                    }
                    else
                    {
                        Ctrl_Address.Visibility = Visibility.Visible;
                        Ctrl_Address.Focus();
                    }
                }
                else
                {
                    if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
                    {
                        Ctrl_Address.Visibility = Visibility.Visible;
                        Storyboard sb = new();
                        Hiro_Utils.AddPowerAnimation(1, Ctrl_Address, sb, null, -30);
                        sb.Completed += delegate
                        {
                            Ctrl_Address.Visibility = Visibility.Hidden;
                        };
                        sb.Begin();
                    }
                    else
                        Ctrl_Address.Visibility = Visibility.Hidden;
                }
                e.Handled = true;
            }
        }

        private void Ctrl_Time_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement? pnlClient = Content as FrameworkElement;
            if (pnlClient != null)
            {
                Ctrl_Progress_Bg.Width = pnlClient.ActualWidth - Ctrl_Time.Margin.Right - Ctrl_Time.ActualWidth - 15;
            }
        }

        private void Player_Cover_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == e.LeftButton)
                PlayPause();
        }

        private void Ctrl_Text_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Enter))
            {
                if (!Ctrl_Text.Text.Equals(string.Empty))
                    ParseCommand();
                if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
                {
                    Ctrl_Address.Visibility = Visibility.Visible;
                    Storyboard sb = new();
                    Hiro_Utils.AddPowerAnimation(1, Ctrl_Address, sb, null, -30);
                    sb.Completed += delegate
                    {
                        Ctrl_Address.Visibility = Visibility.Hidden;
                    };
                    sb.Begin();
                }
                else
                    Ctrl_Address.Visibility = Visibility.Hidden;
                e.Handled = true;
            }
        }

        private void Minbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
            e.Handled = true;
        }

        private void Closebtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
            e.Handled = true;
        }

        private void Maxbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            rflag = 0;
            WindowState = WindowState.Maximized;
            e.Handled = true;
        }

        private void Resbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Normal;
            e.Handled = true;
            rflag = 1;
        }

        private void Hiro_Player1_StateChanged(object sender, EventArgs e)
        {
            Update_Layout();
        }

        private void Load_Translate()
        {
            minbtn.ToolTip = Hiro_Utils.Get_Transalte("Min");
            closebtn.ToolTip = Hiro_Utils.Get_Transalte("close");
            maxbtn.ToolTip = Hiro_Utils.Get_Transalte("max");
            resbtn.ToolTip = Hiro_Utils.Get_Transalte("restore");
        }

        private void Update_Layout()
        {
            rflag = WindowState == WindowState.Maximized ? 0 : rflag;
            Ctrl_Btns.Height = WindowState == WindowState.Maximized ? 35 : 37;
            maxbtn.Visibility = ResizeMode == ResizeMode.NoResize || ResizeMode == ResizeMode.CanMinimize ? Visibility.Collapsed : WindowState == WindowState.Maximized ? Visibility.Collapsed : Visibility.Visible;
            resbtn.Visibility = ResizeMode == ResizeMode.NoResize || ResizeMode == ResizeMode.CanMinimize ? Visibility.Collapsed : WindowState == WindowState.Maximized ? Visibility.Visible : Visibility.Collapsed;
            Ctrl_Btns.Margin = WindowState == WindowState.Maximized ? new(0, 0, 5, 0) : new(0, -2, 5, 0);
            Ctrl_Address.Margin = WindowState == WindowState.Maximized ? new(0) : new(0, -2, 0, 0);
        }

        private void Move_Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowStyle != WindowStyle.None)
            {
                Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
            }
            e.Handled = true;
        }

        private void Move_Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WindowStyle != WindowStyle.None)
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                    e.Handled = true;
                    rflag = 1;
                }
                else
                {
                    rflag = 0;
                    WindowState = WindowState.Maximized;
                    e.Handled = true;
                }
            }
        }

        private void Player_Notify(string val)
        {
            Player_Info.Content = val;
            if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
            {
                if (Player_Info.Visibility != Visibility.Visible)
                {
                    Player_Info.Visibility = Visibility.Visible;
                    Storyboard sb = new();
                    Hiro_Utils.AddPowerAnimation(1, Player_Info, sb, -30, null);
                    sb.Completed += delegate
                    {
                        new System.Threading.Thread(() =>
                        {
                            System.Threading.Thread.Sleep(2000);
                            if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    Storyboard sb = new();
                                    Hiro_Utils.AddPowerAnimation(1, Player_Info, sb, null, -30);
                                    sb.Completed += delegate
                                    {
                                        Player_Info.Visibility = Visibility.Hidden;
                                    };
                                    sb.Begin();
                                });
                            }
                            else
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    Player_Info.Visibility = Visibility.Hidden;
                                });
                            }
                        }).Start();
                    };
                    sb.Begin();
                }
            }
            else
            {
                if (Player_Info.Visibility != Visibility.Visible)
                {
                    Player_Info.Visibility = Visibility.Visible;
                    new System.Threading.Thread(() =>
                    {
                        System.Threading.Thread.Sleep(2000);
                        if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Storyboard sb = new();
                                Hiro_Utils.AddPowerAnimation(1, Player_Info, sb, null, -30);
                                sb.Completed += delegate
                                {
                                    Player_Info.Visibility = Visibility.Hidden;
                                };
                                    sb.Begin();
                            });
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Player_Info.Visibility = Visibility.Hidden;
                            });
                        }
                    }).Start();
                }
            }
        }


        private void Hiro_Player1_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (hiro_provider != null)
                {
                    var vol = hiro_provider.MediaPlayer.Rate;
                    var del = (float)e.Delta > 0 ? 0.25f : -0.25f;
                    if (vol + del <= 0)
                        vol = 0.25f;
                    else
                        vol = vol + del;
                    hiro_provider.MediaPlayer.Rate = vol;
                    Player_Notify(Hiro_Utils.Get_Transalte("playerspeed").Replace("%s", vol.ToString()));
                    e.Handled = true;
                }
            }
            else
            {
                if (hiro_provider != null)
                {
                    var vol = hiro_provider.MediaPlayer.Audio.Volume;
                    var del = e.Delta / 100;
                    if (vol + del < 0)
                        vol = 0;
                    else
                        vol = vol + del;
                    hiro_provider.MediaPlayer.Audio.Volume = vol;
                    Player_Notify(Hiro_Utils.Get_Transalte("playervol").Replace("%v", vol.ToString()));
                    e.Handled = true;
                }
            }
        }
    }
}
