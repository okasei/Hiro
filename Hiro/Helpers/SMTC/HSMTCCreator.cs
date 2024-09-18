using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Foundation;
using Windows.Media;
using Windows.Storage.Streams;

namespace Hiro.Helpers.SMTC
{
    public enum SMTCMediaStatus
    {
        Playing,
        Paused,
        Stopped
    }
    public class HSMTCUpdater
    {
        private readonly SystemMediaTransportControlsDisplayUpdater _updater;
        private SystemMediaTransportControlsTimelineProperties tl = new SystemMediaTransportControlsTimelineProperties();
        public HSMTCUpdater(SystemMediaTransportControlsDisplayUpdater Updater, string AppMediaId)
        {
            _updater = Updater;
            _updater.AppMediaId = AppMediaId;
            _updater.Type = MediaPlaybackType.Music;
        }
        public HSMTCUpdater SetTitle(string title)
        {
            _updater.MusicProperties.Title = title;
            return this;
        }
        public HSMTCUpdater SetAlbumTitle(string albumTitle)
        {
            _updater.MusicProperties.AlbumTitle = albumTitle;
            return this;
        }
        public HSMTCUpdater SetArtist(string artist)
        {
            _updater.MusicProperties.Artist = artist;
            return this;
        }
        public HSMTCUpdater SetThumbnail(string ImgUrl)
        {
            _updater.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(ImgUrl));
            return this;
        }
        public HSMTCUpdater SetThumbnail(BitmapImage ImgUrl)
        {
            try
            {
                _updater.Thumbnail = RandomAccessStreamReference.CreateFromStream(ConvertMemoryStreamToIRandomAccessStream(ConvertBitmapImageToStream(ImgUrl)));
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, "Hiro.Exception.SMTC.SetThumbnail");
            }
            return this;
        }

        public IRandomAccessStream ConvertMemoryStreamToIRandomAccessStream(MemoryStream memoryStream)
        {
            var randomAccessStream = new InMemoryRandomAccessStream();
            memoryStream.Position = 0;
            memoryStream.CopyTo(randomAccessStream.AsStreamForWrite());
            randomAccessStream.FlushAsync().AsTask().Wait();
            randomAccessStream.Seek(0);

            return randomAccessStream;
        }

        public MemoryStream ConvertBitmapImageToStream(BitmapImage bitmapImage)
        {
            // 创建一个 MemoryStream 来保存图像
            var memoryStream = new MemoryStream();

            // 将 BitmapImage 的内容保存到 MemoryStream
            var encoder = new PngBitmapEncoder(); // 你可以选择其他编码器，如 JpegBitmapEncoder
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            encoder.Save(memoryStream);

            // 将流的当前位置重置到开头
            memoryStream.Position = 0;

            return memoryStream;
        }

        public void Update() => _updater.Update();
    }
    public class HSMTCCreator : IDisposable
    {
        private readonly Windows.Media.Playback.MediaPlayer _player = new();
        private readonly SystemMediaTransportControls _smtc;
        private readonly HSMTCUpdater _updater;
        public HSMTCCreator(string MediaId)
        {
            //先禁用系统播放器的命令
            _player.CommandManager.IsEnabled = false;
            //直接创建SystemMediaTransportControls对象被平台限制，神奇的是MediaPlayer对象可以创建该NativeObject
            _smtc = _player.SystemMediaTransportControls;
            _updater = new HSMTCUpdater(_smtc.DisplayUpdater, MediaId);

            //启用状态
            _smtc.IsEnabled = true;
            _smtc.IsPlayEnabled = true;
            _smtc.IsPauseEnabled = true;
            _smtc.IsNextEnabled = true;
            _smtc.IsPreviousEnabled = true;
            //响应系统播放器的命令
            _smtc.ButtonPressed += _smtc_ButtonPressed;
            _smtc.PlaybackPositionChangeRequested += (e, args) =>
            {
                PositionChanged?.Invoke(e, args);
            };
        }

        internal void SetTimelineProperties(double Start, double Position, double End)
        {
            var minSeek = HSet.Read_DCIni("Performance", "0") switch
            {
                "1" => 1.5,
                "2" => 3,
                _ => 0.08
            };
            var tl = new SystemMediaTransportControlsTimelineProperties();
            var _lastSeek = tl.Position.TotalMilliseconds / (tl.EndTime - tl.StartTime).TotalMilliseconds;
            var _newSeek = Position / (End - Start);
            if (Math.Abs(_lastSeek - _newSeek) * 100 > minSeek || double.IsNaN(_lastSeek))
            {
                tl.StartTime = TimeSpan.FromMilliseconds(Start);
                tl.EndTime = TimeSpan.FromMilliseconds(End);
                tl.Position = TimeSpan.FromMilliseconds(Position);
                _smtc.UpdateTimelineProperties(tl);
            }
        }
        public void Dispose()
        {
            _smtc.IsEnabled = false;
            _player.Dispose();
        }
        public HSMTCUpdater Info { get => _updater; }
        public event EventHandler PlayOrPause, Previous, Next;
        public event TypedEventHandler<SystemMediaTransportControls, PlaybackPositionChangeRequestedEventArgs> PositionChanged;
        public void SetMediaStatus(SMTCMediaStatus status)
        {
            _smtc.PlaybackStatus = status switch
            {
                SMTCMediaStatus.Playing => MediaPlaybackStatus.Playing,
                SMTCMediaStatus.Paused => MediaPlaybackStatus.Paused,
                SMTCMediaStatus.Stopped => MediaPlaybackStatus.Stopped,
                _ => throw new NotImplementedException(),
            };
        }


        private void _smtc_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    PlayOrPause?.Invoke(this, EventArgs.Empty);
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    PlayOrPause?.Invoke(this, EventArgs.Empty);
                    break;
                case SystemMediaTransportControlsButton.Next:
                    Next?.Invoke(this, EventArgs.Empty);
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    Previous?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }
    }
}
