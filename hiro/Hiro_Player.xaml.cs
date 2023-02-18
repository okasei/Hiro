using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Vlc.DotNet.Wpf;
using System.Windows.Media;
using System.Windows.Input;
using System.Text;
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.Threading;

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
        internal ContextMenu? cm = null;
        internal static System.Collections.ObjectModel.ObservableCollection<Cmditem> playlist = new();
        internal int index = -1;
        internal int pcd = -1;
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
                Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - Width / 2);
                Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - Height / 2);
                Update_Progress();
                Focus();
                Update_Layout();
            };
        }

        private void AddDanmaku(string content, Color color, FontWeight fontWeight, FontStretch fontStretch, FontStyle fontStyle, int Position)
        {
            Label label = new()
            {
                Content = content,
                Width = double.NaN,
                Foreground = new SolidColorBrush(color),
                FontWeight = fontWeight,
                FontStretch = fontStretch,
                FontStyle = fontStyle,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Visibility = Visibility.Visible,
                Margin = new Thickness(Width, Position, 0, 0)
            };
            Controller.Children.Add(label);
            Storyboard sb = new();
            Hiro_Utils.AddThicknessAnimaton(new(-label.ActualWidth, label.Margin.Top, 0, 0), 150, label, "Margin", sb);
            sb.Completed += delegate
            {
                label.DataContext = null;
                Controller.Children.Remove(label);
            };
            sb.Begin();

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
                        Dgi.ItemsSource = playlist;
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
                        var audio = "*.3ga;*.669;*.a52;*.aac;*.ac3;*.adt;*.adts;*.aif;*.aifc;*.aiff;*.amb;*.amr;*.aob;*.ape;*.au;*.awb;*.caf;*.dts;*.flac;*.it;*.kar;*.m4a;*.m4b;*.m4p;*.m5p;*.mid;*.mka;*.mlp;*.mod;*.mpa;*.mp1;*.mp2;*.mp3;*.mpc;*.mpga;*.mus;*.oga;*.ogg;*.oma;*.opus;*.qcp;*.ra;*.rmi;*.s3m;*.sid;*.spx;*.tak;*.thd;*.tta;*.voc;*.vqf;*.w64;*.wav;*.wma;*.wv;*.xa;*.xm;";
                        var ext = System.IO.Path.GetExtension(toplay).ToLower();
                        if (audio.IndexOf("*" + ext + ";") != -1)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Width = 500;
                                Height = 500;
                            });
                        }
                        Play(toplay);
                    }
                }
                catch (Exception ex)
                {
                    Hiro_Utils.LogError(ex, "Hiro.Exception.MediaPlayer.Initialize");
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
                            wid = wid >= 0 ? wid : 0;
                            Ctrl_Time.Content = ParseDuration(hiro_provider.MediaPlayer.Position * hiro_provider.MediaPlayer.GetMedia().Duration.TotalSeconds) + "/" + ParseDuration(hiro_provider.MediaPlayer.GetMedia().Duration.TotalSeconds);
                            if (pnlClient != null)
                            {
                                Ctrl_Progress_Bg.Width = pnlClient.ActualWidth - Ctrl_Time.Margin.Right - Ctrl_Time.ActualWidth - 15;
                            }
                            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
                            {
                                var len = Math.Abs((wid - Ctrl_Progress.Width) * 2000 / Ctrl_Progress_Bg.Width);
                                if (len < 150)
                                {
                                    Ctrl_Progress.Width = wid;
                                    return;
                                }
                                Storyboard sb = new();
                                sb = Hiro_Utils.AddDoubleAnimaton(wid, len, Ctrl_Progress, "Width", sb);
                                sb.Begin();
                                sb.Completed += delegate
                                {
                                    Ctrl_Progress.Width = wid;
                                };
                            }
                            else
                                Ctrl_Progress.Width = wid;
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
                Dgi.IsEnabled = true;
                Player_Container.Tag = "Playing";
                Update_Progress();
            });
        }

        internal void Play(string uri)
        {
            new Thread(() =>
            {
                if (hiro_provider != null)
                {
                    try
                    {
                        Dispatcher.Invoke(() =>
                        {
                            playlist.Add(new(-1, -1, System.IO.Path.GetFileNameWithoutExtension(uri), uri, string.Empty));
                            if (hiro_provider != null)
                            {
                                var a = Player_Container.Tag != null && !((string)Player_Container.Tag).Equals("Playing") && !((string)Player_Container.Tag).Equals("Paused");
                                var b = Player_Container.Tag == null;
                                if (a || b)
                                {
                                    index = playlist.Count - 1;
                                    hiro_provider.MediaPlayer.Play(new Uri(uri));
                                    Ctrl_Text.Text = uri;
                                    Title = Hiro_Utils.GetFileName(uri) + " - " + App.AppTitle;
                                    Player_Container.Visibility = Visibility.Visible;
                                    Player_Container.Tag = "Playing";
                                    Update_Progress();
                                }
                            }
                        });

                    }
                    catch (Exception ex)
                    {
                        Hiro_Utils.LogError(ex, "Hiro.Exception.MediaPlayer.Play");
                    }
                }
            }).Start();
        }

        internal void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeReverse"] = App.AppForeColor == Colors.White ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 80));
            Resources["AppForeDimColor"] = Hiro_Utils.Color_Transparent(App.AppForeColor, 80);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 80));
            Resources["AppAccentDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 20));
            if (cm != null)
            {
                cm.Background = new SolidColorBrush(App.AppAccentColor);
                cm.Foreground = new SolidColorBrush(App.AppForeColor);
                foreach (var obj in cm.Items)
                {
                    if (obj is MenuItem mi)
                    {
                        if (mi.Items.Count > 0)
                        {
                            mi.Background = new SolidColorBrush(App.AppAccentColor);
                            mi.Foreground = new SolidColorBrush(App.AppForeColor);
                        }
                    }
                }
            }
            Dgi.Background = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 200));
        }

        public void Loadbgi(int direction, bool? animation = null)
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
            new System.Threading.Thread(() =>
            {
                System.Threading.Thread.Sleep(150);
                if (index < playlist.Count - 1)
                    PlayIndex(index + 1);
            }).Start();

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

        private void Ctrl_Progress_MouseDown(object sender, MouseButtonEventArgs e)
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
            Dgi.Visibility = Visibility.Hidden;
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
                var info = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (info != null && info.Length > 0)
                {
                    foreach (var inf in info)
                    {
                        var li = inf as string;
                        Play(li);
                    }
                }
            }
            else if (formats.Equals("System.String"))
            {
                var info = e.Data.GetData(DataFormats.FileDrop) as string;
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
                else if (cflag == 2)
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.MediaPlay.Close");
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
                        Player_Notify(Hiro_Utils.Get_Translate("playerpause"));
                    }
                }
                else if (((string)Player_Container.Tag).Equals("Paused"))
                {
                    if (hiro_provider.MediaPlayer.CouldPlay)
                    {
                        Player_Container.Tag = "Playing";
                        hiro_provider.MediaPlayer.Play();
                        Player_Notify(Hiro_Utils.Get_Translate("playerplay"));
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
                Chrome.CornerRadius = new(0);
                Chrome.NonClientFrameEdges = (System.Windows.Shell.NonClientFrameEdges)13;
                Chrome.GlassFrameThickness = new(0, 1, 0, 0);
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
                Chrome.CornerRadius = new(0);
                Chrome.NonClientFrameEdges = 0;
                Chrome.GlassFrameThickness = new(2);
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
                Switch_UI();
                e.Handled = true;
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.O) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Switch_Bar();
                e.Handled = true;
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.L) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Switch_List();
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
                Switch_UI();
                e.Handled = true;
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.O) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Switch_Bar();
                e.Handled = true;
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.L) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Switch_List();
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
            {
                PlayPause();
                e.Handled = true;
            }
            else if (e.ButtonState == e.RightButton)
            {
                if (cm != null)
                    cm.IsOpen = true;
                e.Handled = true;
            }
        }

        private void Open_Files()
        {
            Microsoft.Win32.OpenFileDialog ofd = new()
            {
                Filter = Hiro_Utils.Get_Translate("vidfiles") +
                "|*.3g2;*.3gp;*.3gp2;*.3gpp;*.amv;*.asf;*.avi;*.bik;*.bin;*.crf;*.dav;*.divx;*.drc;*.dv;*.dvr-ms;*.evo;*.f4v;*.flv;*.gvi;*.gxf;*.m1v;*.m2v;*.m2t;*.m2ts;" +
                "*.m4v;*.mkv;*.mov;*.mp2;*.mp2v;*.mp4;*.mp4v;*.mpe;*.mpeg;*.mpeg1;*.mpeg2;*.mpeg4;*.mpg;*.mpv2;*.mts;*.mtv;*.mxf;*.mxg;*.nsv;*.nuv;*.ogm;*.ogv;*.ogx;*.ps;" +
                "*.rec;*.rm;*.rmvb;*.rpl;*.thp;*.tod;*.tp;*.ts;*.tts;*.txd;*.vob;*.vro;*.webm;*.wm;*.wmv;*.wtv;*.xesc|"
                + Hiro_Utils.Get_Translate("audfiles") +
                "|*.3ga;*.669;*.a52;*.aac;*.ac3;*.adt;*.adts;*.aif;*.aifc;*.aiff;*.amb;*.amr;*.aob;*.ape;*.au;*.awb;*.caf;*.dts;*.flac;*.it;*.kar;*.m4a;*.m4b;*.m4p;*.m5p;" +
                "*.mid;*.mka;*.mlp;*.mod;*.mpa;*.mp1;*.mp2;*.mp3;*.mpc;*.mpga;*.mus;*.oga;*.ogg;*.oma;*.opus;*.qcp;*.ra;*.rmi;*.s3m;*.sid;*.spx;*.tak;*.thd;*.tta;*.voc;" +
                "*.vqf;*.w64;*.wav;*.wma;*.wv;*.xa;*.xm|"
                + Hiro_Utils.Get_Translate("allfiles") + "|*.*",
                ValidateNames = true, // 验证用户输入是否是一个有效的Windows文件名
                CheckFileExists = true, //验证路径的有效性
                CheckPathExists = true,//验证路径的有效性
                Multiselect = true,
                Title = Hiro_Utils.Get_Translate("openfile") + " - " + App.AppTitle
            };
            if (ofd.ShowDialog() == true) //用户点击确认按钮，发送确认消息
            {
                foreach (var strFileName in ofd.FileNames)
                {
                    if (System.IO.File.Exists(strFileName))
                    {
                        Play(strFileName);
                    }
                }
            }
        }

        private void Switch_List()
        {
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            if (Dgi.Visibility == Visibility.Visible)
            {
                Thickness to = new()
                {
                    Left = ActualWidth,
                    Top = 0
                };
                Thickness from = new()
                {
                    Left = ActualWidth - Dgi.ActualWidth,
                    Top = 0
                };
                Dgi.Height = ActualHeight - to.Top;
                if (animation)
                {
                    Storyboard sb = new();
                    Hiro_Utils.AddThicknessAnimaton(to, 150, Dgi, "Margin", sb, from);
                    sb.Completed += delegate
                    {
                        Dgi.Margin = to;
                        Dgi.Visibility = Visibility.Hidden;
                    };
                    sb.Begin();
                }
                else
                {
                    Dgi.Margin = to;
                    Dgi.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                Canvas.SetLeft(Dgi, ActualWidth);
                Dgi.Visibility = Visibility.Visible;
                Thickness to = new()
                {
                    Left = ActualWidth,
                    Top = 0
                };
                Thickness from = new()
                {
                    Left = ActualWidth - Dgi.ActualWidth,
                    Top = 0
                };
                Dgi.Height = ActualHeight - to.Top;
                if (animation)
                {
                    Storyboard sb = new();
                    Hiro_Utils.AddThicknessAnimaton(from, 150, Dgi, "Margin", sb, to);
                    sb.Completed += delegate
                    {
                        Dgi.Margin = from;
                    };
                    sb.Begin();
                }
                else
                {
                    Dgi.Margin = from;
                }
            }
        }

        private void Switch_Bar()
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
        }

        private void Switch_UI()
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
            }
        }

        private void Load_Menu()
        {
            cm?.Items.Clear();
            cm ??= new()
            {
                CacheMode = null,
                Foreground = new SolidColorBrush(App.AppForeColor),
                Background = new SolidColorBrush(App.AppAccentColor),
                BorderBrush = null,
                Style = (Style)App.Current.Resources["HiroContextMenu"],
                Padding = new(1, 10, 1, 10)
            };
            MenuItem open = new()
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Header = Hiro_Utils.Get_Translate("playermopen")
            };
            open.Click += delegate
            {
                Open_Files();
            };
            cm.Items.Add(open);
            MenuItem speed = new()
            {
                CacheMode = null,
                Foreground = new SolidColorBrush(App.AppForeColor),
                Background = new SolidColorBrush(App.AppAccentColor),
                BorderBrush = null,
                Header = Hiro_Utils.Get_Translate("playermspeed")
            };
            MenuItem uu = new()
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Header = Hiro_Utils.Get_Translate("playermspeeduu")
            };
            uu.Click += delegate
            {
                if (hiro_provider != null)
                {
                    hiro_provider.MediaPlayer.Rate = 2;
                    Player_Notify(Hiro_Utils.Get_Translate("playerspeed").Replace("%s", "2"));
                }
            };
            MenuItem u = new()
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Header = Hiro_Utils.Get_Translate("playermspeedu")
            };
            u.Click += delegate
            {
                if (hiro_provider != null)
                {
                    hiro_provider.MediaPlayer.Rate = 1.5f;
                    Player_Notify(Hiro_Utils.Get_Translate("playerspeed").Replace("%s", "1.5"));
                }
            };
            MenuItem n = new()
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Header = Hiro_Utils.Get_Translate("playermspeedn")
            };
            n.Click += delegate
            {
                if (hiro_provider != null)
                {
                    hiro_provider.MediaPlayer.Rate = 1;
                    Player_Notify(Hiro_Utils.Get_Translate("playerspeed").Replace("%s", "1"));
                }
            };
            MenuItem s = new()
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Header = Hiro_Utils.Get_Translate("playermspeeds")
            };
            s.Click += delegate
            {
                if (hiro_provider != null)
                {
                    hiro_provider.MediaPlayer.Rate = 0.75f;
                    Player_Notify(Hiro_Utils.Get_Translate("playerspeed").Replace("%s", "0.75"));
                }
            };
            MenuItem ss = new()
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Header = Hiro_Utils.Get_Translate("playermspeedss")
            };
            ss.Click += delegate
            {
                if (hiro_provider != null)
                {
                    hiro_provider.MediaPlayer.Rate = 0.5f;
                    Player_Notify(Hiro_Utils.Get_Translate("playerspeed").Replace("%s", "0.5"));
                }
            };
            speed.Items.Add(uu);
            speed.Items.Add(u);
            speed.Items.Add(n);
            speed.Items.Add(s);
            speed.Items.Add(ss);
            cm.Items.Add(speed);
            MenuItem to = new()
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Header = Hiro_Utils.Get_Translate("playermpin")
            };
            to.Click += delegate
            {
                Topmost = !Topmost;
                to.Header = Topmost == true ? Hiro_Utils.Get_Translate("playermunpin") : Hiro_Utils.Get_Translate("playermpin");

            };
            cm.Items.Add(to);
            MenuItem list = new()
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Header = Hiro_Utils.Get_Translate("playermlist")
            };
            list.Click += delegate
            {
                Switch_List();
            };
            cm.Items.Add(list);
            MenuItem ui = new()
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Header = Hiro_Utils.Get_Translate("playermui")
            };
            ui.Click += delegate
            {
                Switch_UI();
            };
            cm.Items.Add(ui);
            MenuItem exi = new()
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Header = Hiro_Utils.Get_Translate("playermexit")
            };
            exi.Click += delegate
            {
                Close();
            };
            cm.Items.Add(exi);
            foreach (var obj in cm.Items)
            {
                if (obj is MenuItem mi)
                {
                    Hiro_Utils.Set_Control_Location(mi, "context", location: false);
                    if (mi.Items.Count > 0)
                    {
                        foreach (var mobj in cm.Items)
                        {
                            if (mobj is MenuItem mii)
                                Hiro_Utils.Set_Control_Location(mii, "context", location: false);
                        }
                    }
                }
            }
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
            minbtn.ToolTip = Hiro_Utils.Get_Translate("Min");
            closebtn.ToolTip = Hiro_Utils.Get_Translate("close");
            maxbtn.ToolTip = Hiro_Utils.Get_Translate("max");
            resbtn.ToolTip = Hiro_Utils.Get_Translate("restore");
            Load_Menu();
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
            pcd = 2;
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
                            while (pcd > 0)
                            {
                                pcd--;
                                System.Threading.Thread.Sleep(2000);
                            }
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
                    Player_Notify(Hiro_Utils.Get_Translate("playerspeed").Replace("%s", vol.ToString()));
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
                    Player_Notify(Hiro_Utils.Get_Translate("playervol").Replace("%v", vol.ToString()));
                    e.Handled = true;
                }
            }
        }

        private void Dgi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (Dgi.SelectedIndex > -1)
                {
                    Dgi.IsEnabled = false;
                    PlayIndex(Dgi.SelectedIndex);
                }
            });

        }

        private void PlayIndex(int i)
        {
            Dispatcher.Invoke(() =>
            {
                if (hiro_provider != null && index < playlist.Count)
                {
                    try
                    {
                        index = i;
                        var uri = playlist[i].Command;
                        hiro_provider.MediaPlayer.Play(new Uri(uri));
                        Ctrl_Text.Text = uri;
                        Title = Hiro_Utils.GetFileName(uri) + " - " + App.AppTitle;
                        Player_Container.Visibility = Visibility.Visible;
                        Player_Container.Tag = "Playing";
                    }
                    catch (Exception ex)
                    {
                        Hiro_Utils.LogError(ex, "Hiro.Player.List.PlayIndex");
                    }
                }
            });
        }

        private void Dgi_Drop(object sender, DragEventArgs e)
        {
            DealDragEventArgs(e);
        }

        private void Dgi_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Dgi.Visibility == Visibility.Visible)
                Switch_List();
        }

        private void Dgi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (Dgi.SelectedIndex >= 0)
                {
                    if (index >= 0 && Dgi.SelectedIndex <= index)
                        index--;
                    playlist.RemoveAt(Dgi.SelectedIndex);
                }
                e.Handled = true;
            }
        }
    }
}
