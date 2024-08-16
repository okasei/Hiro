using Hiro.Helpers;
using Hiro.ModelViews;
using Microsoft.Web.WebView2.Core;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using static Hiro.Helpers.HSet;
using static Hiro.Helpers.HLogger;
using static Hiro.Helpers.HText;

namespace Hiro
{
    public partial class Hiro_Web : Window
    {
        internal bool self = false;
        internal string? fixed_title = null;
        private ResizeMode rm = ResizeMode.NoResize;
        private WindowState ws = WindowState.Normal;
        private WindowStyle wt = WindowStyle.None;
        private string prefix = "";
        private bool secure = false;
        internal int bflag = 0;
        internal double UniversalLeft = 0;
        private string FlowTitle = "";
        private string startUri = "<hiuser>";
        private string configPath = Path_PPX($"<hiapp>\\web\\web.config");
        private string currentUrl = "hiro";
        private string iconUrl = "/Resources/hiro_circle.ico";
        private ImageSource? savedicon = null;//Hiro Icon
        private ContextMenu? favMenu = null;
        private bool iconLoaded = false;
        internal WindowAccentCompositor? compositor = null;
        private void VirtualTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }
        public Hiro_Web(string? uri = null, string? title = null, string startUri = "<hiuser>")
        {
            InitializeComponent();
            Helpers.HUI.SetCustomWindowIcon(this);
            if (!startUri.Equals("<hiuser>"))
            {
                uribtn.Visibility = Visibility.Visible;
                uribtn.Content = startUri;
            }
            this.startUri = startUri;
            Title = App.appTitle;
            configPath = startUri.Equals("<hiuser>") ? Path_PPX($"<hiapp>\\web\\web.config") : Path_PPX($"<hiapp>\\web\\{startUri}\\web.config");
            Load_Color();
            Load_Translate();
            Load_Menu();
            Refreash_Layout();
            if (uri != null && uri.ToLower().Equals("hiro://crash"))
            {
                CrashedGrid.Visibility = Visibility.Visible;
                Show();
            }
            else
            {
                //Allow the webview to access Video and Audio
                var options = new CoreWebView2EnvironmentOptions();
                options.AdditionalBrowserArguments = "--enable-features=MediaCaptureAPI";
                var env = CoreWebView2Environment.CreateAsync(userDataFolder: Path_PPX($"<hiapp>\\web\\{startUri}\\"), options: options);
                wv2.EnsureCoreWebView2Async(env.Result);
                CrashedGrid.Visibility = Visibility.Visible;
                try
                {
                    string edgever = CoreWebView2Environment.GetAvailableBrowserVersionString();
                    if (string.IsNullOrEmpty(edgever))
                    {
                        Hiro_Utils.RunExe($"notify({HText.Get_Translate("webnotinstall")},2)", HText.Get_Translate("web"));
                        Close();
                    }
                    else
                    {
                        if (uri != null && uri.ToLower().Equals("hiro://clear"))
                        {
                            Width = Height = 1;
                            ShowInTaskbar = false;
                            Margin = new(-1, -1, 0, 0);
                            WindowStyle = WindowStyle.None;
                            WindowStartupLocation = WindowStartupLocation.Manual;
                            Show();
                            Hide();
                            wv2.CoreWebView2InitializationCompleted += ClearCache;
                        }
                        else
                        {
                            if (uri != null)
                            {
                                if (uri.Trim().Equals(string.Empty))
                                {
                                    uri = Read_Ini(configPath, "Web", "Home", "https://rex.as/");
                                }

                                try
                                {
                                    wv2.Source = new Uri(uri);
                                }
                                catch
                                {
                                    wv2.Source = new Uri($"http://{uri}");
                                }
                            }
                            Show();
                            fixed_title = title;
                            wv2.CoreWebView2InitializationCompleted += Wv2_CoreWebView2InitializationCompleted;
                            Loaded += delegate
                            {
                                HiHiro();
                            };
                        }
                    }
                    LogtoFile($"Edge Webview2 Version: {edgever}");
                }
                catch (Exception ex)
                {
                    LogError(ex, "Hiro.Web.EdgeWebviewNotInstalled");
                    Close();
                }
            }
        }

        public void HiHiro()
        {
            UpdateUniversalLeft();
            if (Read_DCIni("Ani", "2").Equals("1"))
            {
                Storyboard sb = new();
                HAnimation.AddPowerAnimation(1, TitleGrid, sb, -50, null);
                sb.Begin();
            }
        }

        private void Load_Menu()
        {
            favMenu?.Items.Clear();
            favMenu ??= new()
            {
                CacheMode = null,
                Foreground = new SolidColorBrush(App.AppForeColor),
                Background = new SolidColorBrush(App.AppAccentColor),
                BorderBrush = null,
                Style = (Style)App.Current.Resources["HiroContextMenu"],
                Padding = new(1, 10, 1, 10)
            };
            MenuItem addToFav = new()
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Header = "AddToFav"
            };
            addToFav.Click += delegate
            {
                new System.Threading.Thread(() =>
                {
                    var i = 0;
                    while (!Read_Ini(configPath, "Fav" + i.ToString(), "Title", string.Empty).Equals(string.Empty))
                    {
                        i++;
                    }
                    Dispatcher.Invoke(() =>
                    {
                        Write_Ini(configPath, "Fav" + i.ToString(), "Title", wv2.CoreWebView2.DocumentTitle);
                        Write_Ini(configPath, "Fav" + i.ToString(), "URL", wv2.CoreWebView2.Source);
                        var ti = wv2.CoreWebView2.DocumentTitle;
                        if (ti.Length > 10)
                            ti = ti[..10] + "...";
                        MenuItem fav = new()
                        {
                            Background = new SolidColorBrush(Colors.Transparent),
                            Header = ti
                        };
                        fav.Click += delegate
                        {
                            Hiro_Web web = new(wv2.CoreWebView2.Source, fixed_title, startUri);
                            web.WindowState = WindowState;
                            web.Show();
                            web.Refreash_Layout();
                        };
                        favMenu.Items.Add(fav);
                        Label label = new Label();
                        SetLabelProperty(label, wv2.CoreWebView2.DocumentTitle, wv2.CoreWebView2.Source);
                        FavGridBase.Children.Add(label);
                        var msize = new Size();
                        HUI.Get_Text_Visual_Width(label, VisualTreeHelper.GetDpi(this).PixelsPerDip, out msize);
                        UniversalLeft = UniversalLeft + msize.Width + 5;
                        UpdateUniversalLeft();
                    });
                }).Start();
            };
            favMenu.Items.Add(addToFav);
            var aflag = false;
            var i = 0;

            while (!Read_Ini(configPath, "Fav" + i.ToString(), "Title", string.Empty).Trim().Equals(string.Empty))
            {
                var ti = Read_Ini(configPath, "Fav" + i.ToString(), "Title", string.Empty);
                if (ti.Length > 10)
                    ti = ti[..10] + "...";
                MenuItem fav = new()
                {
                    Background = new SolidColorBrush(Colors.Transparent),
                    Header = ti
                };
                fav.ToolTip = Read_Ini(configPath, "Fav" + i.ToString(), "URL", string.Empty);
                fav.Click += delegate
                {
                    Hiro_Web web = new(fav.ToolTip.ToString(), fixed_title, startUri)
                    {
                        WindowState = WindowState
                    };
                    web.Show();
                    web.Refreash_Layout();
                };
                if (aflag == false)
                {
                    favMenu.Items.Add(new Separator());
                    aflag = true;
                }
                favMenu.Items.Add(fav);
                Label label = new Label();
                SetLabelProperty(label, Read_Ini(configPath, "Fav" + i.ToString(), "Title", string.Empty), Read_Ini(configPath, "Fav" + i.ToString(), "URL", string.Empty));
                FavGridBase.Children.Add(label);
                var msize = new Size();
                HUI.Get_Text_Visual_Width(label, VisualTreeHelper.GetDpi(this).PixelsPerDip, out msize);
                UniversalLeft = UniversalLeft + msize.Width + 5;
                i++;
            }
        }

        private void UpdateUniversalLeft()
        {
            FavGridBase.Width = UniversalLeft;
            var toWidth = Width - PreBtn.ActualWidth - NextBtn.ActualWidth - 10 - URLGrid.ActualWidth - RightStack.ActualWidth;
            if (FavGridBase.Margin.Left < 0)
            {
                FavPreBtn.Visibility = Visibility.Visible;
                FavGridG.Margin = new(50, 0, 0, 0);
                toWidth -= 50;
            }
            else
            {
                FavPreBtn.Visibility = Visibility.Collapsed;
                FavGridG.Margin = new(0, 0, 0, 0);
            }
            if (FavPreBtn.Visibility == Visibility.Visible)
            {
                if (toWidth >= UniversalLeft + FavGridBase.Margin.Left)
                {
                    FavNextBtn.Visibility = Visibility.Collapsed;
                    FavGridG.Width = toWidth;
                }
                else
                {
                    FavNextBtn.Visibility = Visibility.Visible;
                    FavGridG.Width = toWidth - 50;
                }
            }
            else
            {
                if (toWidth >= UniversalLeft)
                {
                    FavNextBtn.Visibility = Visibility.Collapsed;
                    FavGridG.Width = toWidth;
                }
                else
                {
                    FavNextBtn.Visibility = Visibility.Visible;
                    FavGridG.Width = toWidth - 50;
                }
            }
        }

        private void SetLabelProperty(Label label, string ti, string url)
        {
            label.SetBinding(ForegroundProperty, new Binding()
            {
                Source = FavExample,
                Path = new("Foreground")
            });
            label.ToolTip = ti;
            label.Background = new SolidColorBrush(Colors.Transparent);
            label.VerticalAlignment = VerticalAlignment.Stretch;
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.Margin = new(UniversalLeft, 0, 0, 0);
            label.MouseLeftButtonDown += delegate (object sender, MouseButtonEventArgs e)
            {
                Hiro_Web web = new(url, fixed_title, startUri);
                web.WindowState = WindowState;
                web.Show();
                web.Refreash_Layout();
                e.Handled = true;
            };
            label.MouseEnter += delegate (object sender, MouseEventArgs e)
            {
                if (!Read_DCIni("Ani", "2").Equals("0"))
                {
                    Storyboard sb = new();
                    HAnimation.AddColorAnimaton((Color)Resources["AppForeDimColor"], 250, label, "Background.Color", sb);
                    sb.Completed += delegate
                    {
                        label.Background = new SolidColorBrush((Color)Resources["AppForeDimColor"]);
                    };
                    sb.Begin();
                }
                else
                {
                    label.Background = new SolidColorBrush((Color)Resources["AppForeDimColor"]);
                }

                e.Handled = true;
            };
            label.MouseLeave += delegate (object sender, MouseEventArgs e)
            {
                if (!Read_DCIni("Ani", "2").Equals("0"))
                {
                    Storyboard sb = new();
                    HAnimation.AddColorAnimaton(Colors.Transparent, 250, label, "Background.Color", sb);
                    sb.Completed += delegate
                    {
                        label.Background = new SolidColorBrush(Colors.Transparent);
                    };
                    sb.Begin();
                }
                else
                {
                    label.Background = new SolidColorBrush(Colors.Transparent);
                }

                e.Handled = true;
            };
            label.Content = ti.Length > 10 ? ti[..10] + "..." : ti;
        }


        private void CoreWebView2_DocumentTitleChanged(object? sender, object e)
        {
            UpdateTitleLabel();
        }

        private void ClearCache(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            wv2.CoreWebView2.CallDevToolsProtocolMethodAsync("Network.clearBrowserCache", "{}");
            Hiro_Utils.RunExe("Delete(<hiapp>\\images\\web\\)", HText.Get_Translate("web"));
            App.Notify(new(HText.Get_Translate("webclear"), 2, HText.Get_Translate("Web")));
            Close();
        }
        public void Loadbgi(int direction, bool animation)
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
            HAnimation.Blur_Animation(direction, animation, bgimage, this);
            bflag = 0;
        }

        private void Wv2_CoreWebView2InitializationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            savedicon = Icon;
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
            wv2.CoreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested;
            wv2.CoreWebView2.FaviconChanged += (e, args) =>
            {
                UpdateIcon();
            };
            wv2.CoreWebView2.PermissionRequested += (obj, args) =>
            {
                if (args.PermissionKind == CoreWebView2PermissionKind.Camera || args.PermissionKind == CoreWebView2PermissionKind.Microphone)
                {
                    args.State = CoreWebView2PermissionState.Allow;
                    args.Handled = true;
                }
            };
            //wv2.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            wv2.CoreWebView2.Settings.IsGeneralAutofillEnabled = true;
            wv2.CoreWebView2.Settings.IsPasswordAutosaveEnabled = true;
            //wv2.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = false;
            wv2.CoreWebView2.Settings.IsStatusBarEnabled = false;
            if (fixed_title == null)
                URLGrid.Visibility = Visibility.Visible;
            if (fixed_title != null && !Topmost)
                topbtn.Visibility = Visibility.Collapsed;
        }

        private void CoreWebView2_ContextMenuRequested(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2ContextMenuRequestedEventArgs e)
        {
            for (var index = 0; index < e.MenuItems.Count; index++)
            {
                if (!e.MenuItems[index].Name.Equals("other") &&
                    !e.MenuItems[index].Name.Equals("createQrCode"))
                    continue;
                e.MenuItems.RemoveAt(index);
                index--;
            }
        }

        private void CoreWebView2_HistoryChanged(object? sender, object e)
        {
            iconLoaded = false;
            SetSavedPrimitiveIcon();
            bool animation = !Read_DCIni("Ani", "2").Equals("0");
            var visual = PreBtn.Visibility;
            var visual2 = NextBtn.Visibility;
            PreBtn.Visibility = wv2.CoreWebView2.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            NextBtn.Visibility = wv2.CoreWebView2.CanGoForward ? Visibility.Visible : Visibility.Collapsed;
            if (PreBtn.Visibility == Visibility.Visible && visual != PreBtn.Visibility && animation)
            {
                Storyboard sb = new();
                HAnimation.AddPowerAnimation(0, PreBtn, sb, -50, null);
                sb.Begin();
            }
            if (NextBtn.Visibility == Visibility.Visible && visual2 != NextBtn.Visibility && animation)
            {
                Storyboard sb = new();
                HAnimation.AddPowerAnimation(0, NextBtn, sb, -50, null);
                sb.Begin();
            }
        }

        public void Load_Color()
        {
            Background = new SolidColorBrush(App.AppAccentColor);
            CrashedButton.Background = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppAccentDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 20));
            Resources["AppForeDimColor"] = Hiro_Utils.Color_Transparent(App.AppForeColor, 80);
            HUI.Set_Bgimage(bgimage, this);
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

            var animation = !Read_DCIni("Ani", "2").Equals("0");
            var visual = soundbtn.Visibility;
            soundbtn.Visibility = wv2.CoreWebView2.IsDocumentPlayingAudio ? Visibility.Visible : Visibility.Collapsed;
            if (soundbtn.Visibility == Visibility.Visible && visual != soundbtn.Visibility && animation)
            {
                Storyboard sb = new();
                HAnimation.AddPowerAnimation(2, soundbtn, sb, -50, null);
                sb.Begin();
            }
            UpdateTitleLabel();
        }

        private void CoreWebView2_ContainsFullScreenElementChanged(object? sender, object e)
        {
            Web_FullScreen(wv2.CoreWebView2.ContainsFullScreenElement);
        }

        private void Web_FullScreen(bool isFullScreen)
        {
            if (isFullScreen)
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
                Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - Width / 2);
                Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - Height / 2);
                Refreash_Layout();
            }
        }

        private void SetIcon(BitmapImage bi)
        {
            Dispatcher.Invoke(() =>
            {
                uicon.Source = bi;
            });
        }

        private void SetSavedPrimitiveIcon()
        {
            Dispatcher.Invoke(() =>
            {
                if (savedicon != null)
                {
                    iconUrl = "/Resources/hiro_circle.ico";
                    uicon.Source = savedicon;
                }
            });
        }

        private void UpdateIcon()
        {
            if (WebGrid.Visibility != Visibility.Visible)
            {
                SetSavedPrimitiveIcon();
                iconLoaded = false;
            }
            else
            {
                var iconUri = wv2.CoreWebView2.FaviconUri;
                if (iconLoaded)
                    return;
                if (iconUri != null && !iconUri.Trim().Equals(string.Empty))
                {
                    if (!iconUri.Equals(iconUrl))
                    {
                        new System.Threading.Thread(() =>
                        {
                            var output = Path_Prepare("<hiapp>\\images\\web\\") + Guid.NewGuid() + ".hwico";
                            HFile.CreateFolder(output);
                            var result = HNet.GetWebContent(iconUri, true, output);
                            if (result.Equals("saved"))
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    try
                                    {
                                        BitmapImage? bi = Hiro_Utils.GetBitmapImage(output);
                                        if (bi != null)
                                        {
                                            SetIcon(bi);
                                            iconUrl = iconUri;
                                            iconLoaded = true;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LogError(ex, "Hiro.Web.Favicon");
                                    }

                                });
                                try
                                {
                                    System.IO.File.Delete(output);
                                }
                                catch { }
                            }
                        }).Start();
                    }
                }
            }
        }

        private void CoreWebView2_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            UpdateTitleLabel();
            URLBtn.Content = secure ? HText.Get_Translate("websecure") : HText.Get_Translate("webinsecure");
            URLSign.Content = secure ? "\uF61A" : "\uF618";
            URLBtn.ToolTip = secure ? HText.Get_Translate("websecuretip") : HText.Get_Translate("webinsecuretip");
            if (Read_DCIni("Ani", "2").Equals("1") && URLGrid.Visibility == Visibility.Visible)
            {
                Storyboard sb = new();
                HAnimation.AddPowerAnimation(1, URLGrid, sb, -50, null);
                sb.Begin();
            }
            Loading(false);
        }

        private void CoreWebView2_NavigationStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            var blockSite = Read_Ini(configPath, "Settings", "BlockSite", string.Empty);
            foreach (var block in blockSite.Split(";"))
            {
                if (block.Trim().Length > 0 && Regex.IsMatch(e.Uri, block))
                {
                    LogtoFile($"Pattern {block} Blocked Site: {e.Uri}");
                    e.Cancel = true;
                    return;
                }
            }
            currentUrl = wv2.CoreWebView2.Source;
            secure = true;
            Loading(true);
        }

        private void CoreWebView2_WebMessageReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var msg = e.TryGetWebMessageAsString();
                App.Notify(new(msg, 2, HText.Get_Translate("web")));
            }
            catch (Exception ex)
            {
                LogError(ex, "Hiro.Exception.Webview");
            }
        }

        private void CoreWebView2_FrameNavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            Loading(false);
        }

        private void CoreWebView2_FrameNavigationStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            var blockSite = Read_Ini(configPath, "Settings", "BlockSite", string.Empty);
            foreach (var block in blockSite.Split(";"))
            {
                if (Regex.IsMatch(e.Uri, block))
                {
                    LogtoFile($"Pattern {block} Blocked Frame: {e.Uri}");
                    e.Cancel = true;
                    return;
                }
            }
            if (e.Uri.ToLower().StartsWith("http://"))
                secure = false;
            Loading(true);
            if (URLBox.Visibility == Visibility.Visible)
            {
                URLBox.Visibility = Visibility.Collapsed;
                TitleLabel.Visibility = Visibility.Visible;
            }
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
            UpdateTitleLabel(state);
        }
        private void CoreWebView2_DownloadStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2DownloadStartingEventArgs e)
        {
            Hiro_Utils.RunExe($"Download({e.DownloadOperation.Uri})", HText.Get_Translate("web"));
            if (wv2.CoreWebView2.DocumentTitle.Trim().Equals(string.Empty) || wv2.CoreWebView2.Source.ToLower().StartsWith("about"))
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
                    Hiro_Web web = new(e.Uri, fixed_title, startUri);
                    web.WindowState = WindowState;
                    web.Show();
                    web.Refreash_Layout();
                }
            }
            e.Handled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            wv2.Stop();
            wv2.DataContext = null;
            wv2.Dispose();
            e.Cancel = false;
        }

        private void Minbtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
            e.Handled = true;
        }

        private void Closebtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
            e.Handled = true;
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
            soundbtn.Margin = WindowState == WindowState.Maximized ? new(0, 1, soundbtn.Margin.Right, 0) : new(0, 0, soundbtn.Margin.Right, 0);
            uribtn.Margin = WindowState == WindowState.Maximized ? new(0, 1, uribtn.Margin.Right, 0) : new(0, 0, uribtn.Margin.Right, 0);
            topbtn.Margin = WindowState == WindowState.Maximized ? new(0, 1, topbtn.Margin.Right, 0) : new(0, 0, topbtn.Margin.Right, 0);
            soundbtn.Height = WindowState == WindowState.Maximized ? 29 : 30;
            uribtn.Height = WindowState == WindowState.Maximized ? 29 : 30;
            HUI.Set_Control_Location(URLBtn, "websecure", location: false);
            HUI.Set_Control_Location(URLBox, "weburl", location: false);
            HUI.Set_Control_Location(PreBtn, "webpre", location: false);
            HUI.Set_Control_Location(NextBtn, "webnext", location: false);
            HUI.Set_Control_Location(uribtn, "webspace", location: false);
            HUI.Set_Control_Location(CrashedLabel, "webcrashtip");
            HUI.Set_Control_Location(CrashedButton, "webcrashbtn");
            Loadbgi(Hiro_Utils.ConvertInt(Read_DCIni("Blur", "0")), false);
        }

        public void Load_Translate()
        {
            minbtn.ToolTip = HText.Get_Translate("Min");
            closebtn.ToolTip = HText.Get_Translate("close");
            maxbtn.ToolTip = HText.Get_Translate("max");
            resbtn.ToolTip = HText.Get_Translate("restore");
            PreBtn.ToolTip = HText.Get_Translate("webpre");
            PreBtn.Content = HText.Get_Translate("webprec").Replace("%s", "◀");
            NextBtn.ToolTip = HText.Get_Translate("webnext");
            NextBtn.Content = HText.Get_Translate("webnextc").Replace("%s", "▶");
            URLBtn.Content = HText.Get_Translate("webinsecure");
            CrashedLabel.Content = HText.Get_Translate("webcrashtip");
            CrashedButton.Content = HText.Get_Translate("webcrashbtn");
            URLSign.Content = "\uF618";
            URLBtn.ToolTip = HText.Get_Translate("webinsecuretip");
            topbtn.ToolTip = Topmost ? HText.Get_Translate("webbottom") : HText.Get_Translate("webtop");
            soundbtn.ToolTip = HText.Get_Translate("webmute");
            uribtn.ToolTip = HText.Get_Translate("webspace").Replace("%s", startUri);
        }

        private void Resbtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WindowState = WindowState.Normal;
            e.Handled = true;
        }

        private void TitleGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
            e.Handled = true;
        }

        private void PreBtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            wv2.CoreWebView2.GoBack();
            e.Handled = true;
        }

        private void NextBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            wv2.CoreWebView2.GoForward();
            e.Handled = true;
        }

        private void URLBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    {
                        if (Keyboard.Modifiers == ModifierKeys.Control)
                        {
                            Hiro_Web web = new(URLBox.Text, fixed_title, startUri);
                            web.WindowState = WindowState;
                            web.Show();
                            web.Refreash_Layout();
                            e.Handled = true;
                            return;
                        }
                        switch (URLBox.Text.ToLower())
                        {
                            case "topmost:on":
                            case "topmost:true":
                                Topmost = true;
                                break;
                            case "topmost:off":
                            case "topmost:false":
                                Topmost = false;
                                break;
                            case "mute:on":
                            case "mute:true":
                                wv2.CoreWebView2.IsMuted = true;
                                break;
                            case "mute:off":
                            case "mute:false":
                            case "fullscreen":
                                wv2.CoreWebView2.IsMuted = false;
                                break;
                            default:
                                if (fixed_title == null)
                                    try
                                    {
                                        iconLoaded = false;
                                        wv2.CoreWebView2.Navigate(URLBox.Text);
                                    }
                                    catch (Exception ex)
                                    {
                                        try
                                        {
                                            iconLoaded = false;
                                            wv2.CoreWebView2.Navigate($"http://{URLBox.Text}");
                                        }
                                        catch
                                        {
                                            LogError(ex, "Hiro.Exception.Webview.URL");
                                        }
                                    }
                                break;
                        }


                        URLBox.Visibility = Visibility.Collapsed;
                        TitleLabel.Visibility = Visibility.Visible;
                        e.Handled = true;
                        break;
                    }
                case Key.Escape:
                    URLBox.Visibility = Visibility.Collapsed;
                    TitleLabel.Visibility = Visibility.Visible;
                    e.Handled = true;
                    break;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Refreash_Layout();
        }

        private void Soundbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            wv2.CoreWebView2.IsMuted = !wv2.CoreWebView2.IsMuted;
            soundbtn.Content = wv2.CoreWebView2.IsMuted ? "\uE198" : "\uE995";
            soundbtn.ToolTip = wv2.CoreWebView2.IsMuted ? HText.Get_Translate("websound") : HText.Get_Translate("webmute");
            e.Handled = true;
        }

        private void UpdateTitleLabel(bool loading = false)
        {
            if (WebGrid.Visibility != Visibility.Visible)
            {
                TitleLabel.Text = App.appTitle;
            }
            else
            {
                prefix = wv2.CoreWebView2.IsDocumentPlayingAudio ? HText.Get_Translate("webmusic") : "";
                string ti = fixed_title ?? HText.Get_Translate("webtitle");
                string lo = loading ? HText.Get_Translate("loading") : "";
                TitleLabel.Text = ti.Replace("%t", wv2.CoreWebView2.DocumentTitle).Replace("%i", lo).Replace("%p", prefix).Replace("%h", App.appTitle);
                if (!Read_DCIni("Ani", "2").Equals("1") || FlowTitle.Equals(Title))
                {
                    Title = TitleLabel.Text;
                    return;
                }
                FlowTitle = Title;
                if (FavGrid.Visibility != Visibility.Visible)
                {
                    URLBox.Visibility = Visibility.Collapsed;
                    TitleLabel.Visibility = Visibility.Visible;
                    Storyboard sb = new();
                    HAnimation.AddPowerAnimation(1, TitleLabel, sb, -50, null);
                    sb.Begin();
                }

            }
            Title = TitleLabel.Text;
        }

        private void Topbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Topmost = !Topmost;
            topbtn.Content = Topmost ? "\uE77A" : "\uE718";
            topbtn.ToolTip = Topmost ? HText.Get_Translate("webbottom") : HText.Get_Translate("webtop");
            e.Handled = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyStates == Keyboard.GetKeyStates(Key.D) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (favMenu != null)
                    favMenu.IsOpen = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.W) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Close();
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Tab) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (WebGrid.Visibility != Visibility.Visible)
                {
                    WebGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    WebGrid.Visibility = Visibility.Hidden;
                }
                UpdateTitleLabel();
                UpdateIcon();
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.F11))
            {
                Web_FullScreen(TitleGrid.Visibility == Visibility.Visible);
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Escape))
            {
                if (TitleGrid.Visibility != Visibility.Visible)
                {
                    Web_FullScreen(false);
                    e.Handled = true;
                }
            }
        }

        private void Uribtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.RunExe(Path_PPX($"<hiapp>\\web\\{startUri}\\EBWebView\\"), HText.Get_Translate("web"), false);
            e.Handled = true;
        }

        private void CrashedButton_Click(object sender, RoutedEventArgs e)
        {
            Hiro_Web web = new(currentUrl, fixed_title, startUri);
            web.WindowState = WindowState;
            web.Show();
            web.Refreash_Layout();
            Close();
            e.Handled = true;
        }

        private void URLSign_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                switch (e.ChangedButton)
                {
                    case MouseButton.Left:
                        TitleLabel.Visibility = URLBox.Visibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
                        URLBox.Visibility = URLBox.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                        URLBox.Text = wv2.CoreWebView2.Source;
                        if (Read_DCIni("Ani", "2").Equals("1"))
                        {
                            System.Windows.Media.Animation.Storyboard sb = new();
                            if (TitleLabel.Visibility == Visibility.Visible)
                                HAnimation.AddPowerAnimation(1, TitleLabel, sb, -50, null);
                            else
                                HAnimation.AddPowerAnimation(1, URLBox, sb, -50, null);
                            sb.Begin();
                        }
                        e.Handled = true;
                        break;

                    case MouseButton.Right:
                        TitleLabel.Visibility = FavGrid.Visibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
                        FavGrid.Visibility = FavGrid.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                        URLBox.Visibility = Visibility.Collapsed;
                        if (Read_DCIni("Ani", "2").Equals("1"))
                        {
                            System.Windows.Media.Animation.Storyboard sb = new();
                            if (FavGrid.Visibility == Visibility.Visible)
                                HAnimation.AddPowerAnimation(1, FavGrid, sb, -50, null);
                            else
                                HAnimation.AddPowerAnimation(1, TitleLabel, sb, -50, null);
                            sb.Begin();
                        }
                        e.Handled = true;
                        break;

                }
            }
        }

        private void FavPreBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var newLeft = FavGridBase.Margin.Left;
            newLeft = newLeft + FavGridG.ActualWidth > 0 ? 0 : newLeft + FavGridG.ActualWidth;
            FavGridBase.Margin = new Thickness(newLeft, 0, 0, 0);
            UpdateUniversalLeft();
        }

        private void FavNextBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var newLeft = FavGridBase.Margin.Left;
            newLeft = UniversalLeft + newLeft - FavGridG.ActualWidth > FavGridG.ActualWidth ? newLeft - FavGridG.ActualWidth : FavGridG.ActualWidth - UniversalLeft;
            FavGridBase.Margin = new Thickness(newLeft, 0, 0, 0);
            UpdateUniversalLeft();
        }
    }
}
