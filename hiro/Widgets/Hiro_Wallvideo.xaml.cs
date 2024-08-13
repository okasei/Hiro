using Hiro.Helpers;
using System;
using System.Windows;
using System.Windows.Interop;

namespace Hiro
{
    /// <summary>
    /// Hiro_Wallvideo.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Wallvideo : Window
    {
        internal bool isPlaying = true;
        public Hiro_Wallvideo()
        {
            InitializeComponent();
            Loaded += delegate
            {
                Hiro_Wallpaper.SetWallpaperVideo(new WindowInteropHelper(this).Handle);
                Media.IsMuted = Hiro_Settings.Read_DCIni("WallVideoMute", "true").Equals("true");
                WindowState = WindowState.Maximized;
            };
        }

        internal void Play(string? path)
        {
            if (System.IO.File.Exists(path))
            {
                Media.Open(new Uri(path));
                Media.Play();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Media.Stop();
            Media.Close();
            Hide();
            Hiro_Wallpaper.RemoveWallpaperSigns();
        }

        private void Media_MediaOpened(object sender, Unosquare.FFME.Common.MediaOpenedEventArgs e)
        {
            if (Media.MediaInfo.Streams.Count > 0 && Hiro_Settings.Read_DCIni("WallVideoCrop", "true").Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                SetAutoCrop();
            }
            else
            {
                DisableAutoCrop();
            }
        }

        internal void DisableAutoCrop()
        {
            Hiro_Settings.Write_Ini(App.dConfig, "Config", "WallVideoCrop", "false");
            Media.Width = ActualWidth;
            Media.Height = ActualHeight;
        }

        internal void SetAutoCrop()
        {
            Hiro_Settings.Write_Ini(App.dConfig, "Config", "WallVideoCrop", "true");
            var w = Media.MediaInfo.Streams[0].PixelWidth;
            var h = Media.MediaInfo.Streams[0].PixelHeight;
            var ww = ActualWidth;
            var hh = ActualHeight;
            var wi = ww / w;
            var hi = hh / h;
            var www = hh * w / h;
            var hhh = ww * h / w;
            if (wi < hi)
            {
                Media.Width = www;
                Media.Height = hh;
            }
            else
            {
                Media.Width = ww;
                Media.Height = hhh;
            }
        }
    }
}
