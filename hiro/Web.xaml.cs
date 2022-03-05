using System;
using System.Windows;
using System.Windows.Media;

namespace hiro
{
    /// <summary>
    /// Web.xaml の相互作用ロジック
    /// </summary>
    public partial class Web : Window
    {
        internal bool self = false;
        internal string? fixed_title = null;
        private ResizeMode rm = ResizeMode.NoResize;
        private WindowState ws = WindowState.Normal;
        private WindowStyle wt = WindowStyle.None;
        private string prefix = "";
        public Web(string? uri = null,string? title = null)
        {
            InitializeComponent();
            if (uri != null)
            {
                if (uri.ToLower().Equals("hiro://clear"))
                {
                    wv2.Source = new Uri("about:blank");
                    wv2.CoreWebView2InitializationCompleted += ClearCache;
                    Width = Height = 1;
                    ShowInTaskbar = false;
                    Margin = new(-1, -1, 0, 0);
                    WindowStyle = WindowStyle.None;
                    WindowStartupLocation = WindowStartupLocation.Manual;
                    Show();
                    Hide();
                }
                else
                {
                    wv2.Source = new Uri(uri);
                    Show();
                }
            }
            fixed_title = title;
            wv2.CoreWebView2InitializationCompleted += Wv2_CoreWebView2InitializationCompleted;
        }

        private void CoreWebView2_DocumentTitleChanged(object? sender, object e)
        {
            string ti = fixed_title ?? utils.Get_Transalte("webtitle");
            Title = ti.Replace("%t", wv2.CoreWebView2.DocumentTitle).Replace("%i", "").Replace("%p", prefix).Replace("%h", App.AppTitle);
        }

        private void ClearCache(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            wv2.CoreWebView2.CallDevToolsProtocolMethodAsync("Network.clearBrowserCache", "{}");
            App.Notify(new(utils.Get_Transalte("webclear"), 2, utils.Get_Transalte("Web")));
            Close();
        }

        private void Wv2_CoreWebView2InitializationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            wv2.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
            wv2.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
            wv2.CoreWebView2.WindowCloseRequested += CoreWebView2_WindowCloseRequested;
            wv2.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting;
            wv2.CoreWebView2.FrameNavigationStarting += CoreWebView2_FrameNavigationStarting;
            wv2.CoreWebView2.FrameNavigationCompleted += CoreWebView2_FrameNavigationCompleted;
            wv2.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            wv2.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
            wv2.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            wv2.CoreWebView2.ContainsFullScreenElementChanged += CoreWebView2_ContainsFullScreenElementChanged;
            wv2.CoreWebView2.IsDocumentPlayingAudioChanged += CoreWebView2_IsDocumentPlayingAudioChanged;
            wv2.CoreWebView2.IsDefaultDownloadDialogOpenChanged += CoreWebView2_IsDefaultDownloadDialogOpenChanged;
            wvpb.Foreground = new SolidColorBrush(App.AppAccentColor);
            wv2.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.102 Safari/537.36 Edg/98.0.1108.62";
        }

        private void CoreWebView2_IsDefaultDownloadDialogOpenChanged(object? sender, object e)
        {
            if (wv2.CoreWebView2.IsDefaultDownloadDialogOpen == true)
            {
                wv2.CoreWebView2.CloseDefaultDownloadDialog();
            }
        }

        private void CoreWebView2_IsDocumentPlayingAudioChanged(object? sender, object e)
        {
            prefix = wv2.CoreWebView2.IsDocumentPlayingAudio ? utils.Get_Transalte("webmusic") : "";
            string ti = fixed_title ?? utils.Get_Transalte("webtitle");
            Title = ti.Replace("%t", wv2.CoreWebView2.DocumentTitle).Replace("%i", "").Replace("%p", prefix).Replace("%h", App.AppTitle);
        }

        private void CoreWebView2_ContainsFullScreenElementChanged(object? sender, object e)
        {
            if (wv2.CoreWebView2.ContainsFullScreenElement)
            {
                wt = WindowStyle;
                ws = WindowState;
                rm = ResizeMode;
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Normal;
                WindowState = WindowState.Maximized;
                ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                WindowStyle = wt;
                WindowState = ws;
                ResizeMode = rm;
                System.Windows.Controls.Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - Width / 2);
                System.Windows.Controls.Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - Height / 2);
            }
        }

        private void CoreWebView2_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            string ti = fixed_title ?? utils.Get_Transalte("webtitle");
            Title = ti.Replace("%t", wv2.CoreWebView2.DocumentTitle).Replace("%i", "").Replace("%p", prefix).Replace("%h", App.AppTitle);
            Loading(false);
        }

        private void CoreWebView2_NavigationStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            string ti = fixed_title ?? utils.Get_Transalte("webtitle");
            Title = ti.Replace("%t", wv2.CoreWebView2.DocumentTitle).Replace("%i", utils.Get_Transalte("loading")).Replace("%p", prefix).Replace("%h", App.AppTitle);
            Loading(true);
        }

        private void CoreWebView2_WebMessageReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var msg = e.TryGetWebMessageAsString();
                App.Notify(new(msg, 2, utils.Get_Transalte("web")));
            }
            catch (Exception ex)
            {
                utils.LogtoFile("[ERROR]" + ex.Message);
            }
        }

        private void CoreWebView2_FrameNavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            Loading(false);
        }

        private void CoreWebView2_FrameNavigationStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            Loading(true);
        }

        private void Loading(bool state)
        {
            if (state)
            {
                wvframe.Visibility = Visibility.Visible;
                Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Indeterminate, new System.Windows.Interop.WindowInteropHelper(this).Handle);
            }
            else
            {
                wvframe.Visibility = Visibility.Collapsed;
                Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.NoProgress, new System.Windows.Interop.WindowInteropHelper(this).Handle);
            }
        }
        private void CoreWebView2_DownloadStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2DownloadStartingEventArgs e)
        {
            utils.RunExe("Download(" + e.DownloadOperation.Uri + ")");
            if (wv2.CoreWebView2.DocumentTitle.Trim().Equals(String.Empty) || wv2.CoreWebView2.Source.ToLower().StartsWith("about"))
                Close();
            e.Cancel = true;
            e.Handled = true;
        }

        private void CoreWebView2_WindowCloseRequested(object? sender, object e)
        {
            Close();
        }

        private void CoreWebView2_NewWindowRequested(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs e)
        {
            if (!e.Uri.ToLower().StartsWith("about"))
            {
                if (self)
                    wv2.Source = new(e.Uri);
                else
                {
                    Web web = new(e.Uri);
                    web.WindowState = WindowState;
                    web.Show();
                }
            }
            e.Handled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            wv2.Dispose();
        }
    }
}
