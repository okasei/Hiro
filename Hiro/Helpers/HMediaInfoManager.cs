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

            try
            {
                _smtcManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                _smtcManager.CurrentSessionChanged += OnCurrentSessionChanged;
            }
            catch (Exception ex) { 
                HLogger.LogError(ex, "Hiro.Exception.SMTC.Listener"); 
            }
            UpdateMediaInfo();
        }

        private static void OnCurrentSessionChanged(GlobalSystemMediaTransportControlsSessionManager sender, CurrentSessionChangedEventArgs args)
        {
            UpdateMediaInfo();
        }


        private static void UpdateMediaInfo()
        {
            try
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
            catch (Exception ex)
            {
                HLogger.LogError(ex, "Hiro.Exception.SMTC.Session.Update");
            }
        }

        private static void _session_MediaPropertiesChanged(GlobalSystemMediaTransportControlsSession sender, MediaPropertiesChangedEventArgs args)
        {
            if (App.dflag)
                HLogger.LogtoFile("New meida found.");
            try
            {
                ShowNotification();
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, "Hiro.Exception.SMTC.Media.Update");
            }
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
                    if (HSet.Read_DCIni("Verbose", "false").Equals("true", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var _app = _session?.SourceAppUserModelId ?? "Unknown";
                        var _mConfig = HText.Path_PPX("<current>\\system\\music\\music.hus");
                        if (HSet.Read_Ini(_mConfig, "Config", "BanApp", string.Empty).Contains($";{_app};"))
                            return;
                        var _ico = HSet.Read_Ini(_mConfig, "Icon", _app, string.Empty);
                        var _hero = HSet.Read_Ini(_mConfig, "Hero", _app, string.Empty);
                        var _tra = HSet.Read_Ini(_mConfig,"Translation",_app, "music");
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
                                _hicon ??= new();
                                _hicon.Image = _thumbi;
                            }
                            if (!_ico.Equals(string.Empty))
                            {
                                _hicon ??= new();
                                _hicon.Location = _ico;
                            }
                            if (!_hero.Equals(string.Empty))
                            {
                                _hicon ??= new();
                                _hicon.HeroImage = _hero;
                            }
                            App.Notify(new(HText.Get_Translate("musicInfo").Replace("%a", artist).Replace("%t", title), 2, HText.Get_Translate(_tra), null, _hicon));
                        });
                    }
                }
            }
        }
    }

}
