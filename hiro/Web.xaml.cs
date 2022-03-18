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
        private bool secure = false;
        internal int bflag = 0;
        public Web(string? uri = null, string? title = null)
        {
            InitializeComponent();
            Load_Color();
            Load_Translate();
            Refreash_Layout();
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
        public void Loadbgi(int direction, bool animation)
        {
            if (bflag == 1)
                return;
            bflag = 1;
            utils.Set_Bgimage(bgimage);
            utils.Blur_Animation(direction, animation, bgimage, this);
            bflag = 0;
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
            wv2.CoreWebView2.HistoryChanged += CoreWebView2_HistoryChanged;
            wv2.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.102 Safari/537.36 Edg/98.0.1108.62";
            if (fixed_title == null) 
                URLGrid.Visibility = Visibility.Visible;
        }

        private void CoreWebView2_HistoryChanged(object? sender, object e)
        {
            PreBtn.Visibility = wv2.CoreWebView2.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            NextBtn.Visibility = wv2.CoreWebView2.CanGoForward ? Visibility.Visible : Visibility.Collapsed;
        }

        public void Load_Color()
        {
            Background = new SolidColorBrush(App.AppAccentColor);
            TitleLabel.Foreground = new SolidColorBrush(App.AppForeColor);
            wvpb.Foreground = new SolidColorBrush(App.AppForeColor);
            CBtn.Foreground = new SolidColorBrush(App.AppForeColor);
            CBtn.Background = new SolidColorBrush(utils.Color_Transparent(App.AppForeColor, 80));
            utils.Set_Bgimage(bgimage);
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
            soundbtn.Visibility = wv2.CoreWebView2.IsDocumentPlayingAudio ? Visibility.Visible : Visibility.Collapsed;
            string ti = fixed_title ?? utils.Get_Transalte("webtitle");
            Title = ti.Replace("%t", wv2.CoreWebView2.DocumentTitle).Replace("%i", "").Replace("%p", prefix).Replace("%h", App.AppTitle);
        }

        private void CoreWebView2_ContainsFullScreenElementChanged(object? sender, object e)
        {
            if (wv2.CoreWebView2.ContainsFullScreenElement)
            {
                TitleGrid.Visibility = Visibility.Collapsed;
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
                TitleGrid.Visibility = Visibility.Visible;
                WindowStyle = wt;
                WindowState = ws;
                ResizeMode = rm;
                System.Windows.Controls.Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - Width / 2);
                System.Windows.Controls.Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - Height / 2);
                Refreash_Layout();
            }
        }

        private void CoreWebView2_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            string ti = fixed_title ?? utils.Get_Transalte("webtitle");
            Title = ti.Replace("%t", wv2.CoreWebView2.DocumentTitle).Replace("%i", "").Replace("%p", prefix).Replace("%h", App.AppTitle);
            URLBtn.Content = secure ? utils.Get_Transalte("websecure") : utils.Get_Transalte("webinsecure");
            URLSign.Content = secure ? "\uF61A" : "\uF618";
            URLBtn.ToolTip = secure ? utils.Get_Transalte("websecuretip") : utils.Get_Transalte("webinsecuretip");
            Loading(false);
        }

        private void CoreWebView2_NavigationStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            string ti = fixed_title ?? utils.Get_Transalte("webtitle");
            Title = ti.Replace("%t", wv2.CoreWebView2.DocumentTitle).Replace("%i", utils.Get_Transalte("loading")).Replace("%p", prefix).Replace("%h", App.AppTitle);
            secure = true;
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
            if (e.Uri.ToLower().StartsWith("http://"))
                secure = false;
            Loading(true);
        }

        private void Loading(bool state)
        {
            if (state)
            {
                wvpb.Visibility = Visibility.Visible;
                Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Indeterminate, new System.Windows.Interop.WindowInteropHelper(this).Handle);
            }
            else
            {
                wvpb.Visibility = Visibility.Collapsed;
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
                    web.Refreash_Layout();
                }
            }
            e.Handled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            wv2.Dispose();
        }

        private void Minbtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
            e.Handled = true;
        }

        private void Closebtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }

        private void Maxbtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WindowState = WindowState.Maximized;
            e.Handled = true;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            Refreash_Layout();
        }

        public void Refreash_Layout()
        {

            TitleGrid.Height = WindowState == WindowState.Maximized ? 26 : 32;
            WebGrid.Margin = TitleGrid.Visibility == Visibility.Collapsed ? new(0) : WindowState == WindowState.Maximized ? new(0, 26, 0, 0) : new(0, 32, 0, 0);
            maxbtn.Visibility = ResizeMode == ResizeMode.NoResize || ResizeMode == ResizeMode.CanMinimize ? Visibility.Collapsed : WindowState == WindowState.Maximized ? Visibility.Collapsed : Visibility.Visible;
            resbtn.Visibility = ResizeMode == ResizeMode.NoResize || ResizeMode == ResizeMode.CanMinimize ? Visibility.Collapsed : WindowState == WindowState.Maximized ? Visibility.Visible : Visibility.Collapsed;
            closebtn.Margin = WindowState == WindowState.Maximized ? new(0, -5, 0, 0) : new(0, -2, 0, 0);
            closebtn.Height = WindowState == WindowState.Maximized ? 30 : 32;
            BaseGrid.Margin = WindowState == WindowState.Maximized ? new(6) : new(0);
            utils.Set_Control_Location(URLBtn, "websecure", location: false);
            utils.Set_Control_Location(URLBox, "weburl", location: false);
            utils.Set_Control_Location(PreBtn, "webpre", location: false);
            utils.Set_Control_Location(NextBtn, "webnext", location: false);
            Loadbgi(utils.ConvertInt(utils.Read_Ini(App.dconfig, "config", "blur", "0")), false);
        }

        public void Load_Translate()
        {
            minbtn.ToolTip = utils.Get_Transalte("min");
            closebtn.ToolTip = utils.Get_Transalte("close");
            maxbtn.ToolTip = utils.Get_Transalte("max");
            resbtn.ToolTip = utils.Get_Transalte("restore");
            PreBtn.ToolTip = utils.Get_Transalte("webpre");
            PreBtn.Content = utils.Get_Transalte("webprec").Replace("%s", "◀");
            NextBtn.ToolTip = utils.Get_Transalte("webnext");
            NextBtn.Content = utils.Get_Transalte("webnextc").Replace("%s", "▶");
            URLBtn.Content = utils.Get_Transalte("webinsecure");
            URLSign.Content = "\uF618";
            URLBtn.ToolTip = utils.Get_Transalte("webinsecuretip");
            topbtn.ToolTip = Topmost ? utils.Get_Transalte("webbottom") : utils.Get_Transalte("webtop");
            soundbtn.ToolTip = utils.Get_Transalte("webmute");
        }

        private void Resbtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WindowState = WindowState.Normal;
            e.Handled = true;
        }

        private void TitleGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
            e.Handled = true;
        }

        private void PreBtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            wv2.CoreWebView2.GoBack();
            e.Handled = true;
        }

        private void NextBtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            wv2.CoreWebView2.GoForward();
            e.Handled = true;
        }

        private void URLBtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TitleLabel.Visibility = URLBox.Visibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
            URLBox.Visibility = URLBox.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            URLBox.Text = wv2.CoreWebView2.Source;
            e.Handled = true;
        }

        private void URLBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                if (URLBox.Text.ToLower().Equals("topmost:on") || URLBox.Text.ToLower().Equals("topmost:true"))
                    Topmost = true;
                if (URLBox.Text.ToLower().Equals("topmost:off") || URLBox.Text.ToLower().Equals("topmost:false"))
                    Topmost = false;
                if (URLBox.Text.ToLower().Equals("mute:on") || URLBox.Text.ToLower().Equals("mute:true"))
                    wv2.CoreWebView2.IsMuted = true;
                if (URLBox.Text.ToLower().Equals("mute:off") || URLBox.Text.ToLower().Equals("mute:false"))
                    wv2.CoreWebView2.IsMuted = false;
                URLBox.Visibility = Visibility.Collapsed;
                TitleLabel.Visibility = Visibility.Visible;
                if (fixed_title == null)
                    wv2.CoreWebView2.Navigate(URLBox.Text);
                e.Handled = true;
            }
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                URLBox.Visibility = Visibility.Collapsed;
                TitleLabel.Visibility = Visibility.Visible;
                e.Handled = true;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Refreash_Layout();
        }

        private void Window_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (maxbtn.Visibility == Visibility.Visible)
                WindowState = WindowState.Maximized;
            else if (resbtn.Visibility == Visibility.Visible)
                WindowState = WindowState.Normal;
            e.Handled = true;
        }

        private void Soundbtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            wv2.CoreWebView2.IsMuted = !wv2.CoreWebView2.IsMuted;
            soundbtn.Content = wv2.CoreWebView2.IsMuted ? "\uE198" : "\uE995";
            soundbtn.ToolTip = wv2.CoreWebView2.IsMuted ? utils.Get_Transalte("websound") : utils.Get_Transalte("webmute");
            e.Handled = true;
        }

        private void Topbtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Topmost = !Topmost;
            topbtn.Content = Topmost ? "\uE77A" : "\uE718";
            topbtn.ToolTip = Topmost ? utils.Get_Transalte("webbottom") : utils.Get_Transalte("webtop");
            e.Handled = true;
        }
    }
}
