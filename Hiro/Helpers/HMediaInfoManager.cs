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
using System.Windows.Threading;
using Windows.Foundation;

namespace Hiro.Helpers
{
    internal class HMediaInfoManager
    {
        private static GlobalSystemMediaTransportControlsSessionManager? _smtcManager;
        private static GlobalSystemMediaTransportControlsSession? _session;
        private static string _title = string.Empty;
        private static string _artist = string.Empty;
        private static string _sid = string.Empty;

        public static async void Initialize()
        {

            try
            {
                _smtcManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                _smtcManager.CurrentSessionChanged += OnCurrentSessionChanged;
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, "Hiro.Exception.SMTC.Listener");
            }
            UpdateMediaInfo();
        }

        private static void OnCurrentSessionChanged(GlobalSystemMediaTransportControlsSessionManager sender, CurrentSessionChangedEventArgs args)
        {
            UpdateMediaInfo();
        }

        internal static IAsyncOperation<bool>? TryTogglePrevious()
        {
            return _session?.TrySkipPreviousAsync();
        }
        internal static IAsyncOperation<bool>? TryToggleNext()
        {
            return _session?.TrySkipNextAsync();
        }
        internal static IAsyncOperation<bool>? TryTogglePlay()
        {
            return _session?.TryTogglePlayPauseAsync();
        }


        private static bool UpdateMediaInfo()
        {
            try
            {
                var session = _smtcManager?.GetCurrentSession();
                if (session == null)
                {
                    Hiro_Utils.HiroInvoke(() =>
                    {
                        if (App.tb != null)
                        {
                            if (App.tb.BasicGrid.Visibility != Visibility.Visible)
                            {
                                App.tb.RemoveGrid(App.tb.MusicControlGrid);
                            }
                        }
                    });
                    return false;
                }
                UpdatePlayInfo();
                if (_session != null)
                {
                    _session.MediaPropertiesChanged -= _session_MediaPropertiesChanged;
                    _session.PlaybackInfoChanged -= _session_PlaybackInfoChanged;
                    _session = null;
                }
                _session = session;
                if (!HText.IsOnlyBlank(_sid) && !_sid.Equals(_session.SourceAppUserModelId))
                {
                    _title = string.Empty;
                    _artist = string.Empty;
                }
                _sid = _session.SourceAppUserModelId ?? string.Empty;
                if (App.dflag)
                    HLogger.LogtoFile("New session found.[" + (_session?.SourceAppUserModelId ?? "Unknown") + "]");
                _session.MediaPropertiesChanged += _session_MediaPropertiesChanged;
                _session.PlaybackInfoChanged += _session_PlaybackInfoChanged;
                ShowNotification();
                UpdatePlayInfo();
                return true;
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, "Hiro.Exception.SMTC.Session.Update");
                return false;
            }
        }

        private static void _session_PlaybackInfoChanged(GlobalSystemMediaTransportControlsSession sender, PlaybackInfoChangedEventArgs args)
        {
            UpdatePlayInfo();
        }

        internal static void UpdatePlayInfo()
        {
            if (_session != null)
            {
                if (App.tb != null)
                {
                    var _pi = _session.GetPlaybackInfo().PlaybackStatus;

                    switch (_pi)
                    {
                        case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing:
                            Hiro_Utils.HiroInvoke(() =>
                            {
                                if (App.tb.MusicControlGrid.Visibility != Visibility.Visible)
                                {
                                    App.tb.LoadGrid(App.tb.MusicControlGrid);
                                }
                                App.tb.Pause.Visibility = Visibility.Visible;
                                App.tb.Play.Visibility = Visibility.Collapsed;
                            });
                            break;
                        case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused:
                            Hiro_Utils.HiroInvoke(() =>
                            {
                                if (App.tb.MusicControlGrid.Visibility != Visibility.Visible)
                                {
                                    App.tb.LoadGrid(App.tb.MusicControlGrid);
                                }
                                App.tb.Pause.Visibility = Visibility.Collapsed;
                                App.tb.Play.Visibility = Visibility.Visible;
                            });
                            break;
                        case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Stopped:
                            break;
                        case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Closed:
                            break;
                        default:
                            break;
                    }
                }
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
            if (mediaProperties == null)
                return;
            string title = mediaProperties.Title;
            string artist = mediaProperties.Artist;
            if (App.dflag)
                HLogger.LogtoFile($"Title: {mediaProperties.Title} Artist:{mediaProperties.Artist} Album:{mediaProperties.AlbumTitle} AlbumArtist:{mediaProperties.AlbumArtist} Subtitle:{mediaProperties.Subtitle}");
            if (_title.Equals(title) && _artist.Equals(artist))
                return;
            var _mConfig = HText.Path_PPX("<current>\\system\\music\\music.hus");
            if (HSet.Read_Ini(_mConfig, "Config", "BanUnknownArtist", "false").Equals("true", StringComparison.CurrentCultureIgnoreCase) && HText.IsOnlyBlank(artist))
                return;
            if (HSet.Read_Ini(_mConfig, "Config", "BanUnknownTitle", "false").Equals("true", StringComparison.CurrentCultureIgnoreCase) && HText.IsOnlyBlank(title))
                return;
            _title = title;
            _artist = artist;
            title = title.Equals(string.Empty) ? HText.Get_Translate("musicTitle") : title;
            artist = artist.Equals(string.Empty) ? HText.Get_Translate("musicArtist") : artist;
            //Music
            if (HSet.Read_DCIni("Verbose", "false").Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                var _app = _session?.SourceAppUserModelId ?? "Unknown";
                if (HSet.Read_Ini(_mConfig, "Config", "BanApp", string.Empty).Contains($";{_app};"))
                    return;
                var _ico = HSet.Read_Ini(_mConfig, "Icon", _app, string.Empty);
                var _hero = HSet.Read_Ini(_mConfig, "Hero", _app, string.Empty);
                var _tra = HSet.Read_Ini(_mConfig, "Translation", _app, "music");
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
