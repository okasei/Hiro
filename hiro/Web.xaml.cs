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
        internal string prefix = "";
        internal bool self = false;
        public Web(string? de = null)
        {
            InitializeComponent();
            if (de != null)
            { 
                if (de.ToLower().Equals("hiro://clear"))
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
                    wv2.Source = new Uri(de);
                    Show();
                }
            }
            wv2.CoreWebView2InitializationCompleted += Wv2_CoreWebView2InitializationCompleted;
        }

        private void CoreWebView2_DocumentTitleChanged(object? sender, object e)
        {
            Title = utils.Get_Transalte("webtitle").Replace("%t", wv2.CoreWebView2.DocumentTitle).Replace("%p", prefix).Replace("%h", App.AppTitle);
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
            Title = utils.Get_Transalte("webtitle").Replace("%t", wv2.CoreWebView2.DocumentTitle).Replace("%p", prefix).Replace("%h", App.AppTitle); ;
        }

        private void CoreWebView2_ContainsFullScreenElementChanged(object? sender, object e)
        {
            if (wv2.CoreWebView2.ContainsFullScreenElement)
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                Margin = new(0, 0, 0, 0);
                VerticalAlignment = VerticalAlignment.Stretch;
                HorizontalAlignment = HorizontalAlignment.Stretch;
            }
            else
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
            }
        }

        private void CoreWebView2_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            Title = utils.Get_Transalte("webtitle").Replace("%t", wv2.CoreWebView2.DocumentTitle).Replace("%p", prefix).Replace("%h", App.AppTitle);
            Loading(false);
        }

        private void CoreWebView2_NavigationStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            Title = utils.Get_Transalte("webtitle").Replace("%t", utils.Get_Transalte("loading")).Replace("%p", prefix).Replace("%h", App.AppTitle);
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
