using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.Media.Control;
using System.Windows.Media;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using static Hiro.Helpers.HClass;

namespace Hiro.Helpers
{
    internal class HMediaInfoManager
    {
        private static GlobalSystemMediaTransportControlsSessionManager? _smtcManager;
        private static GlobalSystemMediaTransportControlsSession? _session;
        private static string _title = string.Empty;
        private static string _artist = string.Empty;

        public static async void Initialize()
        {
            _smtcManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            _smtcManager.CurrentSessionChanged += OnCurrentSessionChanged;
            UpdateMediaInfo();
        }

        private static void OnCurrentSessionChanged(GlobalSystemMediaTransportControlsSessionManager sender, CurrentSessionChangedEventArgs args)
        {
            UpdateMediaInfo();
        }


        private static void UpdateMediaInfo()
        {
            var session = _smtcManager?.GetCurrentSession();
            if (session == null) return;
            if (_session != null)
            {
                _session.MediaPropertiesChanged -= _session_MediaPropertiesChanged;
                _session = null;
                _title = string.Empty;
                _artist = string.Empty;
            }
            _session = session;
            if (App.dflag)
                HLogger.LogtoFile("New session found.[" + (_session?.SourceAppUserModelId ?? "Unknown") + "]");
            _session.MediaPropertiesChanged += _session_MediaPropertiesChanged;
            ShowNotification();
        }

        private static void _session_MediaPropertiesChanged(GlobalSystemMediaTransportControlsSession sender, MediaPropertiesChangedEventArgs args)
        {
            if (App.dflag)
                HLogger.LogtoFile("New meida found.");
            ShowNotification();
        }

        private static void ShowNotification()
        {
            var mediaProperties = _session?.TryGetMediaPropertiesAsync().GetAwaiter().GetResult();
            if (mediaProperties != null)
            {
                string title = mediaProperties.Title;
                string artist = mediaProperties.Artist;
                if (App.dflag)
                    HLogger.LogtoFile($"Title: {mediaProperties.Title} Artist:{mediaProperties.Artist} Album:{mediaProperties.AlbumTitle} AlbumArtist:{mediaProperties.AlbumArtist} Subtitle:{mediaProperties.Subtitle}");
                if (!_title.Equals(title) || !_artist.Equals(artist))
                {
                    _title = title;
                    _artist = artist;
                    title = title.Equals(string.Empty) ? HText.Get_Translate("musicTitle") : title;
                    artist = artist.Equals(string.Empty) ? HText.Get_Translate("musicArtist") : artist;
                    //Music
                    if (HSet.Read_DCIni("Verbose", "0").Equals("1"))
                    {
                        var _ico = string.Empty;
                        var _tra = "music";
                        var _app = _session?.SourceAppUserModelId ?? "Unknown".Replace(".exe", string.Empty);
                        var _exe = new string[] { "qqmusic.exe", "neteasemusic.exe", "kuwo.exe", "kugou.exe", "spotify.exe", "AppleInc.AppleMusicWin_nzyj5cx40ttqa!App", "Hiro.exe" };
                        var _tran = new string[] { "qqmusic", "cloudmusic", "kwmusic", "kgmusic", "spotify", "applemusic", "player" };
                        var _icon = new string[] { "<current>\\system\\icons\\qqmusic.png", "<current>\\system\\icons\\neteasemusic.png", "<current>\\system\\icons\\kuwomusic.png", "<current>\\system\\icons\\kgmusic.png", "<current>\\system\\icons\\spotify.png", "<current>\\system\\icons\\applemusic.png", "<current>\\system\\icons\\hiro.png" };
                        for (int i = 0; i < _exe.Length; i++)
                        {
                            if (_exe[i].Equals(_app, StringComparison.CurrentCultureIgnoreCase))
                            {
                                _tra = _tran[i];
                                _ico = _icon[i];
                                break;
                            }
                        }
                        Hiro_Utils.HiroInvoke(async () =>
                        {
                            BitmapImage? _thumbi = null;
                            if (mediaProperties.Thumbnail != null)
                            {
                                try
                                {

                                    _thumbi = new BitmapImage();
                                    _thumbi.BeginInit();
                                    _thumbi.StreamSource = (await mediaProperties.Thumbnail.OpenReadAsync()).AsStream();
                                    _thumbi.EndInit();
                                }
                                catch (Exception ex)
                                {
                                    HLogger.LogError(ex, "Hiro.Exception.MediaInfoManager.Thumbnail.ReadAsync");
                                    _thumbi = null;
                                }
                            }
                            Hiro_Icon? _hicon = null;
                            if (_thumbi != null)
                            {
                                _hicon = new Hiro_Icon() { Image = _thumbi };
                            }
                            else if (!_icon.Equals(string.Empty))
                            {
                                _hicon = new Hiro_Icon() { Location = _ico };
                            }
                            App.Notify(new(HText.Get_Translate("musicInfo").Replace("%a", artist).Replace("%t", title), 2, HText.Get_Translate(_tra), null, _hicon));
                        });
                    }
                }
            }
        }
    }

}
