using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Text;
using System.Windows.Media.Animation;
using System.Threading;
using System.IO;
using Hiro.Helpers;
using Hiro.ModelViews;
using static Hiro.Helpers.HSet;
using System.Windows.Threading;
using System.Runtime;

namespace Hiro
{
    /// <summary>
    /// Hiro_Player.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Player : Window
    {
        internal string? toplay = null;
        private int bflag = 0;
        private int rflag = 1;
        private Size mSize = new(450, 800);
        private int cflag = 1;
        internal ContextMenu? cm = null;
        internal static System.Collections.ObjectModel.ObservableCollection<HClass.Cmditem> playlist = new();
        internal int index = -1;
        internal int pcd = -1;
        internal WindowAccentCompositor? compositor = null;
        bool isMaxed = false;
        DispatcherTimer _dst = new DispatcherTimer();
        HLrycis? _lrc = null;
        double _nextTime = -1;
        bool _lrcFlag = false;
        /// <summary>
        /// 0 - 未在播放, 1 - 暂停, 2 - 正在播放
        /// </summary>
        int _currentCondition = 0;
        private void VirtualTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }
        public Hiro_Player(string? play = null)
        {
            InitializeComponent();
            HUI.SetCustomWindowIcon(this);
            toplay = play;
            Title = App.appTitle;
            Loaded += delegate
            {
                Load_Color();
                Load_Translate();
                Load_Position();
                Loadbgi(Hiro_Utils.ConvertInt(Read_DCIni("Blur", "0")));
                Initialize_Player();
                Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - Width / 2);
                Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - Height / 2);
                Update_Progress();
                Focus();
                Update_Layout();
                _dst.Interval = Read_DCIni("Performance", "0") switch
                {
                    "1" => TimeSpan.FromMilliseconds(500),
                    "2" => TimeSpan.FromMilliseconds(1000),
                    _ => TimeSpan.FromMilliseconds(150)
                };
                _dst.Tick += delegate
                {
                    Update_Progress();
                };
            };
            SourceInitialized += OnSourceInitialized;
        }

        private void Load_Position()
        {
            HUI.Set_FrameworkElement_Location(MusicGrid, "musicg");
            HUI.Set_FrameworkElement_Location(LyricsGrid, "lyricg");
            HUI.Set_Control_Location(FakeMusic, "musicTitle");
            HUI.CopyFontFromLabel(FakeMusic, MusicTitle);
            HUI.CopyPostionFromLabel(FakeMusic, MusicTitle);
            HUI.Set_Control_Location(FakeMusic, "musicArtist");
            HUI.CopyFontFromLabel(FakeMusic, MusicArtist);
            HUI.CopyPostionFromLabel(FakeMusic, MusicArtist);
            HUI.Set_Control_Location(FakeMusic, "musicAlbum");
            HUI.CopyFontFromLabel(FakeMusic, MusicAlbum);
            HUI.CopyPostionFromLabel(FakeMusic, MusicAlbum);
            HUI.Set_Control_Location(FakeMusic, "musicLyric");
            HUI.CopyFontFromLabel(FakeMusic, LyricsBlockFirst);
            HUI.CopyPostionFromLabel(FakeMusic, LyricsBlockFirst, false);
            HUI.Set_Control_Location(FakeMusic, "musicLyricx");
            HUI.CopyFontFromLabel(FakeMusic, LyricsBlock);
            HUI.CopyPostionFromLabel(FakeMusic, LyricsBlock, false);
            LyricsBlockFirst.LineHeight = LyricsBlockFirst.Height;
            LyricsBlock.LineHeight = LyricsBlockFirst.Height;
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;
            var source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source?.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0084://NCHITTEST
                    if (maxbtn.IsMouseOver || resbtn.IsMouseOver)
                    {
                        if (!isMaxed)
                        {
                            isMaxed = true;
                            handled = true;
                            return new(9);
                        }
                    }
                    else
                    {
                        isMaxed = false;
                    }
                    break;
                default:
                    //Console.WriteLine("Msg: " + m.Msg + ";LParam: " + m.LParam + ";WParam: " + m.WParam + ";Result: " + m.Result);
                    break;
            }
            return IntPtr.Zero;

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
            HAnimation.AddThicknessAnimaton(new(-label.ActualWidth, label.Margin.Top, 0, 0), 150, label, "Margin", sb);
            sb.Completed += delegate
            {
                label.DataContext = null;
                Controller.Children.Remove(label);
            };
            sb.Begin();

        }

        private void Initialize_Player()
        {
            try
            {
                Dgi.ItemsSource = playlist;
                Media.MediaEnded += Media_MediaEnded;
                Media.IsMuted = false;
                if (toplay != null)
                {
                    var audio = "*.3ga;*.669;*.a52;*.aac;*.ac3;*.adt;*.adts;*.aif;*.aifc;*.aiff;*.amb;*.amr;*.aob;*.ape;*.au;*.awb;*.caf;*.dts;*.flac;*.it;*.kar;*.m4a;*.m4b;*.m4p;*.m5p;*.mid;*.mka;*.mlp;*.mod;*.mpa;*.mp1;*.mp2;*.mp3;*.mpc;*.mpga;*.mus;*.oga;*.ogg;*.oma;*.opus;*.qcp;*.ra;*.rmi;*.s3m;*.sid;*.spx;*.tak;*.thd;*.tta;*.voc;*.vqf;*.w64;*.wav;*.wma;*.wv;*.xa;*.xm;*.dsf;";
                    var ext = System.IO.Path.GetExtension(toplay).ToLower();
                    if (audio.IndexOf("*" + ext + ";") != -1)
                    {
                        Width = 300;
                        Height = 300;
                    }
                    Play(toplay);
                }
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, "Hiro.Exception.MediaPlayer.Initialize");
            }
        }

        private void Media_MediaEnded(object? sender, EventArgs e)
        {
            if (!Media.IsAtEndOfStream)
                return;
            Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.NoProgress, new System.Windows.Interop.WindowInteropHelper(this).Handle);
            _dst.Stop();
            _currentCondition = 0;
            Media.Stop();
            Media.Close();
            HideContainer(Player_Container, 100, 200);
            HideContainer(MusicGrid, 100, 200);
            HideContainer(LyricsGrid, 100, 200);
            LyricsBlockFirst.Text = string.Empty;
            LyricsBlock.Text = string.Empty;
            Ctrl_Progress.Width = 0;
            Title = App.appTitle;
            Ctrl_Time.Content = "00:00";
            if (App.dflag)
                HLogger.LogtoFile($"[Player] MediaEnded");
            System.Threading.Tasks.Task.Delay(Read_DCIni("Performance", "0") switch
            {
                "1" => TimeSpan.FromMilliseconds(500),
                "2" => TimeSpan.FromMilliseconds(1000),
                _ => TimeSpan.FromMilliseconds(150)
            }).ContinueWith(t =>
            {
                if (index < playlist.Count - 1 && index > -1)
                    PlayIndex(index + 1);
                else
                {
                    index = -1;
                }
            });

        }

        private void Update_Progress()
        {
            try
            {
                bool zero = _currentCondition == 0;
                if (zero)
                    return;
                var _ts = Media.Position.TotalSeconds;
                if (_nextTime >= 0 && _ts > _nextTime && _lrc != null && !_lrcFlag)
                {
                    _lrcFlag = true;
                    var ls = _lrc.GetLyrics(_ts, out _nextTime);
                    if (Read_DCIni("Ani", "2").Equals("1"))
                    {
                        var sb = HAnimation.AddThicknessAnimaton(LyricsBlockFirst.Margin, 200, LyricsBlock, "Margin", null);
                        sb = HAnimation.AddPowerOutAnimation(0, LyricsBlockFirst, sb, null, 150, 200, 200);
                        sb.Completed += delegate
                        {
                            LyricsBlockFirst.Text = ls[0];
                            LyricsBlock.Text = ls[1] + Environment.NewLine + ls[2] + Environment.NewLine + ls[3] + Environment.NewLine + ls[4];
                            _lrcFlag = false;
                        };
                        sb.Begin();
                    }
                    else
                    {
                        LyricsBlockFirst.Text = ls[0];
                        LyricsBlock.Text = ls[1] + Environment.NewLine + ls[2] + Environment.NewLine + ls[3] + Environment.NewLine + ls[4];
                        _lrcFlag = false;
                    }
                }
                Ctrl_Time.Content = zero ? "00:00" : ParseDuration(_ts) + "/" + ParseDuration(Media.MediaInfo.Duration.TotalSeconds);
                Ctrl_Progress_Bg.Width = ActualWidth - Ctrl_Time.Margin.Right - Ctrl_Time.ActualWidth - 15;
                var wid = zero ? 0 : Ctrl_Progress_Bg.Width * _ts / Media.MediaInfo.Duration.TotalSeconds;
                Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressValue((int)(100 * _ts / Media.MediaInfo.Duration.TotalSeconds), 100, new System.Windows.Interop.WindowInteropHelper(this).Handle);
                wid = wid >= 0 ? wid : 0;
                if (Read_DCIni("Ani", "2").Equals("1"))
                {
                    var len = Math.Abs((wid - Ctrl_Progress.Width) * 2000 / Ctrl_Progress_Bg.Width);
                    if (len < 150)
                    {
                        Ctrl_Progress.Width = wid;
                        return;
                    }
                    Storyboard sb = new();
                    sb = HAnimation.AddDoubleAnimaton(wid, len, Ctrl_Progress, "Width", sb);
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
            catch (Exception e)
            {
                HLogger.LogError(e, "Hiro.Player.UpdateProgress");
                Ctrl_Progress.Width = 0;
                Ctrl_Progress_Bg.Width = ActualWidth - Ctrl_Time.Margin.Right - Ctrl_Time.ActualWidth - 15;
                Ctrl_Time.Content = "00:00";
            }
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
            if (txt.StartsWith("hiro.vol:"))
            {
                Media.Volume = Convert.ToInt32(txt[9..]) / 100.0;
            }
            else if (txt.StartsWith("hiro.speed:"))
            {
                Media.SpeedRatio = float.Parse(txt[11..]);
            }
            else
            {
                Play(txt);
            }
        }

        internal void Play(string uri)
        {
            new Thread(() =>
            {
                try
                {
                    Dispatcher.Invoke(async () =>
                    {
                        if (_currentCondition == 2)
                            await Media.Pause();
                        playlist.Add(new(-1, -1, Path.GetFileNameWithoutExtension(uri), uri, string.Empty));
                        index = playlist.Count - 1;
                        if (App.dflag)
                            HLogger.LogtoFile($"[Player] Opening file: {uri}");
                        if (await Media.Open(new Uri(uri)))
                        {
                            Ctrl_Text.Text = uri;
                            Title = HText.Get_Translate("playerTitle").Replace("%t", HFile.GetFileName(uri)).Replace("%a", App.appTitle);
                            Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Normal, new System.Windows.Interop.WindowInteropHelper(this).Handle);
                            ShowContainer(Player_Container, -100, 250, 350);
                            LoadMusicGrid();
                            _currentCondition = 2;
                            _dst.Start();
                            Dgi.IsEnabled = true;
                        }
                    });

                }
                catch (Exception ex)
                {
                    HLogger.LogError(ex, "Hiro.Exception.MediaPlayer.Play");
                }
            }).Start();
        }

        private void LoadMusicGrid()
        {
            try
            {
                var mt = Media.MediaInfo.Metadata;
                var _artist = mt.ContainsKey("ARTIST") ? mt["ARTIST"] : null;
                var _title = mt.ContainsKey("TITLE") ? mt["TITLE"] : null;
                var _album = mt.ContainsKey("ALBUM") ? mt["ALBUM"] : null;
                var _lrcs = mt.ContainsKey("LYRICS") ? mt["LYRICS"] : (mt.ContainsKey("lyrics-XXX") ? mt["lyrics-XXX"] : null);
                if (_lrcs != null && HSet.Read_DCIni("EnableLyrics", "true").Equals("true", StringComparison.CurrentCultureIgnoreCase))
                {
                    _lrc = HLrycis.InitLrc(_lrcs);
                    ShowContainer(LyricsGrid, -100, 150, 200);
                    _nextTime = 0;
                }
                else
                {
                    HideContainer(LyricsGrid, 100, 200);
                    _lrc = null;
                    _nextTime = -1;
                }
                if (App.dflag)
                {
                    foreach (var k in Media.MediaInfo.Metadata.Keys)
                    {
                        HLogger.LogtoFile($"{k}:{Media.MediaInfo.Metadata[k]}");
                    }
                }
                if (!IsEmpty(_artist) || !IsEmpty(_title) || !IsEmpty(_album))
                {
                    MusicTitle.Text = _title ?? HText.Get_Translate("musicTitle");
                    MusicArtist.Text = _artist ?? HText.Get_Translate("musicArtist");
                    MusicAlbum.Text = _album ?? HText.Get_Translate("musicAlbum");
                    ShowContainer(MusicGrid, -100, 200, 300);
                    return;
                }
                HideContainer(MusicGrid, 100, 200);
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, "Hiro.Play.Music.LoadTags");
            }
        }

        private bool IsEmpty(string? str)
        {
            if (str == null)
                return true;
            var _s = str.ToString();
            return _s.Equals(string.Empty);
        }
        internal void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeReverseColor"] = HSet.Read_DCIni("PlayerColor", "0").Equals("1") ? (App.AppForeColor == Colors.White ? Colors.Black : Colors.White) : App.AppAccentColor;
            Resources["AppForeReverse"] = App.AppForeColor == Colors.White ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 80));
            Resources["AppForeDimColor"] = Hiro_Utils.Color_Transparent(App.AppForeColor, 80);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 80));
            Resources["AppAccentDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 20));
            Dgi.Background = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 200));
        }

        public void Loadbgi(int direction, bool? animation = null)
        {
            if (Read_DCIni("Background", "1").Equals("3"))
            {
                compositor ??= new(this);
                HUI.Set_Acrylic(bgimage, this, null, compositor);
                return;
            }
            if (compositor != null)
            {
                compositor.IsEnabled = false;
            }
            if (bflag == 1)
                return;
            bflag = 1;
            HUI.Set_Bgimage(bgimage, this);
            bool ani = animation == null ? !Read_DCIni("Ani", "2").Equals("0") : (bool)animation;
            HAnimation.Blur_Animation(direction, ani, bgimage, this);
            bflag = 0;
        }

        private void Ctrl_Progress_Bg_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == e.LeftButton)
            {
                if (_currentCondition != 0)
                {
                    var a = e.GetPosition(Ctrl_Progress_Bg);
                    var _set = (float)(a.X / Ctrl_Progress_Bg.Width) * Media.MediaInfo.Duration.TotalSeconds;
                    Media.Position = TimeSpan.FromSeconds(_set);
                    if (_lrc != null)
                    {
                        _nextTime = _set;
                    }
                }
                e.Handled = true;
            }
        }

        private void Ctrl_Progress_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == e.LeftButton)
            {
                if (_currentCondition != 0)
                {
                    var a = e.GetPosition(Ctrl_Progress_Bg);
                    var _set = (float)(a.X / Ctrl_Progress_Bg.Width) * Media.MediaInfo.Duration.TotalSeconds;
                    Media.Position = TimeSpan.FromSeconds(_set);
                    if (_lrc != null)
                    {
                        _nextTime = _set;
                    }
                }
                e.Handled = true;
            }
        }

        private void Hiro_Player1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Dgi.Visibility = Visibility.Hidden;
            Loadbgi(Hiro_Utils.ConvertInt(Read_DCIni("Blur", "0")), false);
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
                    var _i = playlist.Count;
                    if (_currentCondition == 2)
                        Media.Pause();
                    foreach (var inf in info)
                    {
                        var li = inf as string;
                        playlist.Add(new(-1, -1, System.IO.Path.GetFileNameWithoutExtension(li), li, string.Empty));
                    }
                    if (index < playlist.Count - 1 && index > -1)
                        PlayIndex(index + 1);
                    else
                    {
                        if (playlist.Count > 0)
                            PlayIndex(_i);
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
                    new System.Threading.Thread(async () =>
                    {
                        cflag = 2;
                        e.Cancel = true;
                        await Media.Close();
                        Media.Dispose();
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
                HLogger.LogError(ex, "Hiro.Exception.MediaPlay.Close");
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
            if (_currentCondition != 0)
            {
                if (_currentCondition == 2)
                {
                    Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Paused, new System.Windows.Interop.WindowInteropHelper(this).Handle);
                    Media.Pause();
                    _currentCondition = 1;
                    Player_Notify(HText.Get_Translate("playerpause"));
                }
                else
                {
                    Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Normal, new System.Windows.Interop.WindowInteropHelper(this).Handle);
                    Media.Play();
                    _currentCondition = 2;
                    Player_Notify(HText.Get_Translate("playerplay"));
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
                Ctrl_Btns.Visibility = Controller.Visibility;
                Move_Label.Visibility = Controller.Visibility;
                HUI.Set_FrameworkElement_Location(MusicGrid, Controller.Visibility == Visibility.Hidden ? "musicgx" : "musicg", false);
                HUI.Set_FrameworkElement_Location(LyricsGrid, "lyricgx", false);
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
                Ctrl_Btns.Visibility = Visibility.Hidden;
                Move_Label.Visibility = Visibility.Hidden;
                HUI.Set_FrameworkElement_Location(MusicGrid, Controller.Visibility == Visibility.Hidden ? "musicgx" : "musicg", false);
                HUI.Set_FrameworkElement_Location(LyricsGrid, Controller.Visibility == Visibility.Hidden ? "lyricgx" : "lyricg", false);
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
                if (_currentCondition != 0)
                {
                    Media.Position = TimeSpan.FromSeconds(Math.Min(Media.Position.TotalSeconds + 0.01 * Media.MediaInfo.Duration.TotalSeconds, Media.MediaInfo.Duration.TotalSeconds));
                }
                e.Handled = true;
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.Left))
            {
                if (_currentCondition != 0)
                {
                    Media.Position = TimeSpan.FromSeconds(Math.Max(Media.Position.TotalSeconds - 0.01 * Media.MediaInfo.Duration.TotalSeconds, 0));
                }
                e.Handled = true;
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
            Player_Cover_KeyDown(sender, e);
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
                Filter = HText.Get_Translate("vidfiles") +
                "|*.3g2;*.3gp;*.3gp2;*.3gpp;*.amv;*.asf;*.avi;*.bik;*.bin;*.crf;*.dav;*.divx;*.drc;*.dv;*.dvr-ms;*.evo;*.f4v;*.flv;*.gvi;*.gxf;*.m1v;*.m2v;*.m2t;*.m2ts;" +
                "*.m4v;*.mkv;*.mov;*.mp2;*.mp2v;*.mp4;*.mp4v;*.mpe;*.mpeg;*.mpeg1;*.mpeg2;*.mpeg4;*.mpg;*.mpv2;*.mts;*.mtv;*.mxf;*.mxg;*.nsv;*.nuv;*.ogm;*.ogv;*.ogx;*.ps;" +
                "*.rec;*.rm;*.rmvb;*.rpl;*.thp;*.tod;*.tp;*.ts;*.tts;*.txd;*.vob;*.vro;*.webm;*.wm;*.wmv;*.wtv;*.xesc|"
                + HText.Get_Translate("audfiles") +
                "|*.3ga;*.669;*.a52;*.aac;*.ac3;*.adt;*.adts;*.aif;*.aifc;*.aiff;*.amb;*.amr;*.aob;*.ape;*.au;*.awb;*.caf;*.dts;*.flac;*.it;*.kar;*.m4a;*.m4b;*.m4p;*.m5p;" +
                "*.mid;*.mka;*.mlp;*.mod;*.mpa;*.mp1;*.mp2;*.mp3;*.mpc;*.mpga;*.mus;*.oga;*.ogg;*.oma;*.opus;*.qcp;*.ra;*.rmi;*.s3m;*.sid;*.spx;*.tak;*.thd;*.tta;*.voc;" +
                "*.vqf;*.w64;*.wav;*.wma;*.wv;*.xa;*.xm;*.dsf|"
                + HText.Get_Translate("allfiles") + "|*.*",
                ValidateNames = true, // 验证用户输入是否是一个有效的Windows文件名
                CheckFileExists = true, //验证路径的有效性
                CheckPathExists = true,//验证路径的有效性
                Multiselect = true,
                Title = HText.Get_Translate("ofdTitle").Replace("%t", HText.Get_Translate("openfile")).Replace("%a", App.appTitle)
            };
            if (ofd.ShowDialog() == true) //用户点击确认按钮，发送确认消息
            {
                var _i = playlist.Count;
                if (_currentCondition == 2)
                    Media.Pause();
                foreach (var strFileName in ofd.FileNames)
                {
                    if (System.IO.File.Exists(strFileName))
                    {
                        playlist.Add(new(-1, -1, System.IO.Path.GetFileNameWithoutExtension(strFileName), strFileName, string.Empty));
                    }
                }
                if (index < playlist.Count - 1 & index > -1)
                    PlayIndex(index + 1);
                else
                {
                    if (playlist.Count > 0)
                        PlayIndex(_i);
                }
            }
        }

        private void Switch_List()
        {
            bool animation = !Read_DCIni("Ani", "2").Equals("0");
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
                    HAnimation.AddThicknessAnimaton(to, 150, Dgi, "Margin", sb, from);
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
                    HAnimation.AddThicknessAnimaton(from, 150, Dgi, "Margin", sb, to);
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
                if (!Read_DCIni("Ani", "2").Equals("0"))
                {
                    Ctrl_Address.Visibility = Visibility.Visible;
                    Storyboard sb = new();
                    HAnimation.AddPowerAnimation(1, Ctrl_Address, sb, -30, null);
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
                if (!Read_DCIni("Ani", "2").Equals("0"))
                {
                    Ctrl_Address.Visibility = Visibility.Visible;
                    Storyboard sb = new();
                    HAnimation.AddPowerAnimation(1, Ctrl_Address, sb, null, -30);
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

        internal void Switch_UI()
        {
            if (Controller.Visibility == Visibility.Hidden)
            {
                if (!Read_DCIni("Ani", "2").Equals("0"))
                {
                    Controller.Visibility = Visibility.Visible;
                    Storyboard sb = new();
                    if (WindowStyle != WindowStyle.None)
                    {
                        Ctrl_Btns.Visibility = Visibility.Visible;
                        Move_Label.Visibility = Visibility.Visible;
                        HAnimation.AddPowerAnimation(1, Ctrl_Btns, sb, -30, null);
                        HAnimation.AddPowerAnimation(1, Move_Label, sb, -30, null);
                    }
                    HAnimation.AddPowerAnimation(3, Controller, sb, -30, null);
                    HUI.Set_FrameworkElement_Location(MusicGrid, "musicg", true);
                    HUI.Set_FrameworkElement_Location(LyricsGrid, "lyricg", true);
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
                    HUI.Set_FrameworkElement_Location(MusicGrid, "musicg", false);
                    HUI.Set_FrameworkElement_Location(LyricsGrid, "lyricg", false);
                }
            }
            else
            {
                if (!Read_DCIni("Ani", "2").Equals("0"))
                {
                    Controller.Visibility = Visibility.Visible;
                    Storyboard sb = new();
                    if (WindowStyle != WindowStyle.None)
                    {
                        Ctrl_Btns.Visibility = Visibility.Visible;
                        Move_Label.Visibility = Visibility.Visible;
                        HAnimation.AddPowerAnimation(1, Ctrl_Btns, sb, null, -30);
                        HAnimation.AddPowerAnimation(1, Move_Label, sb, null, -30);
                    }
                    HAnimation.AddPowerAnimation(3, Controller, sb, null, -30);
                    sb.Completed += delegate
                    {
                        Controller.Visibility = Visibility.Hidden;
                        Ctrl_Btns.Visibility = Visibility.Hidden;
                        Move_Label.Visibility = Visibility.Hidden;
                    };
                    sb.Begin();
                    HUI.Set_FrameworkElement_Location(MusicGrid, "musicgx", true);
                    HUI.Set_FrameworkElement_Location(LyricsGrid, "lyricgx", true);
                }
                else
                {
                    Controller.Visibility = Visibility.Hidden;
                    Ctrl_Btns.Visibility = Visibility.Hidden;
                    Move_Label.Visibility = Visibility.Hidden;
                    HUI.Set_FrameworkElement_Location(MusicGrid, "musicgx", false);
                    HUI.Set_FrameworkElement_Location(LyricsGrid, "lyricgx", false);
                }
            }
        }

        private void Load_Menu()
        {
            cm?.Items.Clear();
            cm ??= new()
            {
                CacheMode = null,
                BorderBrush = null
            };
            MenuItem open = new()
            {
                Header = HText.Get_Translate("playermopen")
            };
            open.Click += delegate
            {
                Open_Files();
            };
            cm.Items.Add(open);
            MenuItem speed = new()
            {
                CacheMode = null,
                BorderBrush = null,
                Header = HText.Get_Translate("playermspeed")
            };
            MenuItem uu = new()
            {
                Header = HText.Get_Translate("playermspeeduu")
            };
            uu.Click += delegate
            {
                Media.SpeedRatio = 2;
                Player_Notify(HText.Get_Translate("playerspeed").Replace("%s", "2"));
            };
            MenuItem u = new()
            {
                Header = HText.Get_Translate("playermspeedu")
            };
            u.Click += delegate
            {
                Media.SpeedRatio = 1.5f;
                Player_Notify(HText.Get_Translate("playerspeed").Replace("%s", "1.5"));
            };
            MenuItem n = new()
            {
                Header = HText.Get_Translate("playermspeedn")
            };
            n.Click += delegate
            {
                Media.SpeedRatio = 1;
                Player_Notify(HText.Get_Translate("playerspeed").Replace("%s", "1"));
            };
            MenuItem s = new()
            {
                Header = HText.Get_Translate("playermspeeds")
            };
            s.Click += delegate
            {
                Media.SpeedRatio = 0.75f;
                Player_Notify(HText.Get_Translate("playerspeed").Replace("%s", "0.75"));
            };
            MenuItem ss = new()
            {
                Header = HText.Get_Translate("playermspeedss")
            };
            ss.Click += delegate
            {
                Media.SpeedRatio = 0.5f;
                Player_Notify(HText.Get_Translate("playerspeed").Replace("%s", "0.5"));
            };
            speed.Items.Add(uu);
            speed.Items.Add(u);
            speed.Items.Add(n);
            speed.Items.Add(s);
            speed.Items.Add(ss);
            cm.Items.Add(speed);
            MenuItem to = new()
            {
                Header = HText.Get_Translate("playermpin")
            };
            to.Click += delegate
            {
                Topmost = !Topmost;
                to.Header = Topmost == true ? HText.Get_Translate("playermunpin") : HText.Get_Translate("playermpin");

            };
            cm.Items.Add(to);
            MenuItem list = new()
            {
                Header = HText.Get_Translate("playermlist")
            };
            list.Click += delegate
            {
                Switch_List();
            };
            cm.Items.Add(list);
            MenuItem ui = new()
            {
                Header = HText.Get_Translate("playermui")
            };
            ui.Click += delegate
            {
                Switch_UI();
            };
            cm.Items.Add(ui);
            MenuItem exi = new()
            {
                Header = HText.Get_Translate("playermexit")
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
                    HUI.Set_Control_Location(mi, "context", location: false);
                    if (mi.Items.Count > 0)
                    {
                        foreach (var mobj in cm.Items)
                        {
                            if (mobj is MenuItem mii)
                                HUI.Set_Control_Location(mii, "context", location: false);
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
                if (!Read_DCIni("Ani", "2").Equals("0"))
                {
                    Ctrl_Address.Visibility = Visibility.Visible;
                    Storyboard sb = new();
                    HAnimation.AddPowerAnimation(1, Ctrl_Address, sb, null, -30);
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
            minbtn.ToolTip = HText.Get_Translate("Min");
            closebtn.ToolTip = HText.Get_Translate("close");
            maxbtn.ToolTip = HText.Get_Translate("max");
            resbtn.ToolTip = HText.Get_Translate("restore");
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
            if (!Read_DCIni("Ani", "2").Equals("0"))
            {
                if (Player_Info.Visibility != Visibility.Visible)
                {
                    Player_Info.Visibility = Visibility.Visible;
                    Storyboard sb = new();
                    HAnimation.AddPowerAnimation(1, Player_Info, sb, -30, null);
                    sb.Completed += delegate
                    {
                        new System.Threading.Thread(() =>
                        {
                            while (pcd > 0)
                            {
                                pcd--;
                                System.Threading.Thread.Sleep(2000);
                            }
                            if (!Read_DCIni("Ani", "2").Equals("0"))
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    Storyboard sb = new();
                                    HAnimation.AddPowerAnimation(1, Player_Info, sb, null, -30);
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
                        if (!Read_DCIni("Ani", "2").Equals("0"))
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Storyboard sb = new();
                                HAnimation.AddPowerAnimation(1, Player_Info, sb, null, -30);
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
                var vol = Media.SpeedRatio;
                var del = (float)e.Delta > 0 ? 0.25f : -0.25f;
                if (vol + del <= 0)
                    vol = 0.25f;
                else
                    vol = vol + del;
                Media.SpeedRatio = vol;
                Player_Notify(HText.Get_Translate("playerspeed").Replace("%s", vol.ToString()));
                e.Handled = true;
            }
            else
            {
                var vol = Media.Volume;
                var del = e.Delta / 10000.0;
                if (vol + del < 0)
                    vol = 0;
                else
                    vol = vol + del;
                Media.Volume = vol;
                Player_Notify(HText.Get_Translate("playervol").Replace("%v", (Math.Min((int)(vol * 100), 100)).ToString()));
                e.Handled = true;
            }
        }

        private void Dgi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Dgi.SelectedIndex > -1)
            {
                Dgi.IsEnabled = false;
                PlayIndex(Dgi.SelectedIndex);
            }

        }

        private void PlayIndex(int i)
        {
            new Thread(() =>
            {
                Dispatcher.Invoke(async () =>
                {
                    if (i < playlist.Count)
                    {
                        try
                        {
                            index = i;
                            var uri = playlist[i].Command;
                            if (App.dflag)
                                HLogger.LogtoFile($"[Player] Opening file: {uri}");
                            if (await Media.Open(new Uri(uri)))
                            {
                                Ctrl_Text.Text = uri;
                                Title = HText.Get_Translate("playerTitle").Replace("%t", HFile.GetFileName(uri)).Replace("%a", App.appTitle);
                                Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Normal, new System.Windows.Interop.WindowInteropHelper(this).Handle);
                                ShowContainer(Player_Container, -100, 250, 350);
                                LoadMusicGrid();
                                _currentCondition = 2;
                                _dst.Start();
                                Dgi.IsEnabled = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            HLogger.LogError(ex, "Hiro.Player.List.PlayIndex");
                        }
                    }
                });
            }).Start();
        }

        private void ShowContainer(Grid container, int distance, int time, int otime)
        {
            container.Visibility = Visibility.Visible;
            if (Read_DCIni("Ani", "2").Equals("1"))
            {
                var s = HAnimation.AddPowerAnimation(0, container, null, distance, null, time, otime);
                s.Completed += delegate
                {
                    container.Visibility = Visibility.Visible;
                };
                s.Begin();
            }
        }

        private void HideContainer(Grid container, int distance, int time)
        {
            if (container.Visibility != Visibility.Visible)
                return;
            if (Read_DCIni("Ani", "2").Equals("1"))
            {
                container.Visibility = Visibility.Visible;
                var s = HAnimation.AddPowerOutAnimation(0, container, null, null, distance, time, time);
                s.Completed += delegate
                {
                    container.Visibility = Visibility.Collapsed;
                };
                s.Begin();
            }
            else
            {
                container.Visibility = Visibility.Collapsed;
            }
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
