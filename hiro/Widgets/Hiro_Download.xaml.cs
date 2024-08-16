using Hiro.Helpers;
using Hiro.Resources;
using Hiro.ModelViews;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static Hiro.Helpers.HClass;
using System.Threading;
using System.Windows.Threading;

namespace Hiro
{
    /// <summary>
    /// Download.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Download : Window
    {
        internal string clips = "";
        private string mSaveFileName = "";//下载文件的保存文件名
        private string progress = "";
        private int current = -1;
        private string listfile = "";
        internal string rurl = "";
        internal string rpath = "";
        internal long startpos = 0;
        internal int mode = 0;//1=更新模式
        internal string product = "";
        internal int bflag = 0;
        private int stopflag = 0;
        private bool successflag = true;
        private int index = 0;
        internal WindowAccentCompositor? compositor = null;
        private string nextTitle = string.Empty;
        private double nextProgess = 0;
        private DispatcherTimer? _timer = null;
        public Hiro_Download(int i, string st)
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            mode = i;
            product = st;
            Load_Colors();
            Load_Position();
            Load_Translate();
            HUI.SetCustomWindowIcon(this);
            Loaded += delegate
            {
                HiHiro();
            };
        }

        public void HiHiro()
        {
            Loadbgi(Hiro_Utils.ConvertInt(HSet.Read_DCIni("Blur", "0")));
            if (HSet.Read_DCIni("Ani", "2").Equals("1"))
            {
                Storyboard sb = new();
                HAnimation.AddPowerAnimation(1, ala_title, sb, -50, null);
                HAnimation.AddPowerAnimation(2, minbtn, sb, -50, null);
                HAnimation.AddPowerAnimation(2, closebtn, sb, -50, null);
                HAnimation.AddPowerAnimation(0, Autorun, sb, -50, null);
                HAnimation.AddPowerAnimation(3, albtn_1, sb, -50, null);
                sb.Begin();
            }
            Hiro_Utils.SetWindowToForegroundWithAttachThreadInput(this);
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;
            var source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source?.AddHook(WndProc);
            WindowStyle = WindowStyle.SingleBorderWindow;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0083://prevent system from drawing outline
                    handled = true;
                    break;
                default:
                    //Console.WriteLine("Msg: " + m.Msg + ";LParam: " + m.LParam + ";WParam: " + m.WParam + ";Result: " + m.Result);
                    break;
            }
            return IntPtr.Zero;

        }
        void Load_Translate()
        {
            Title = HText.Get_Translate("dlTitle").Replace("%t", HText.Get_Translate("dltitle")).Replace("%a", App.appTitle);
            ala_title.Content = HText.Get_Translate("dltitle");
            albtn_1.Content = HText.Get_Translate("dlstart");
            urllabel.Content = mode == 1 ? HText.Get_Translate("dlupdate").Replace("%u", product) : HText.Get_Translate("dllink");
            pathlabel.Content = HText.Get_Translate("dlpath");
            Autorun.Content = HText.Get_Translate("dlrun");
            minbtn.ToolTip = HText.Get_Translate("min");
            closebtn.ToolTip = HText.Get_Translate("close");
        }
        void Load_Position()
        {
            HUI.Set_Control_Location(this, "dlwin");
            HUI.Set_Control_Location(albtn_1, "dlstart", bottom: true, right: true);
            HUI.Set_Control_Location(pb, "dlprogress");
            HUI.Set_Control_Location(ala_title, "dltitle");
            if (mode == 1)
            {
                HUI.Set_Control_Location(urllabel, "dlupdate");
                pathlabel.Visibility = Visibility.Hidden;
                textBoxHttpUrl.Visibility = Visibility.Hidden;
                SavePath.Visibility = Visibility.Hidden;
                Autorun.Visibility = Visibility.Hidden;
                return;
            }
            HUI.Set_Control_Location(urllabel, "dllink");
            HUI.Set_Control_Location(pathlabel, "dlpath");
            HUI.Set_Control_Location(textBoxHttpUrl, "dlurl");
            HUI.Set_Control_Location(SavePath, "dlsave");
            HUI.Set_Control_Location(Autorun, "dlrun", bottom: true);
            System.Windows.Controls.Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - this.Width / 2);
            System.Windows.Controls.Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - this.Height / 2);

        }
        public void Load_Colors()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDimColor"] = Hiro_Utils.Color_Transparent(App.AppForeColor, 80);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        private bool DownloadUIActions()
        {
            //获取http下载路径
            stopflag = 0;
            pb.Value = 0;
            albtn_1.Content = HText.Get_Translate("dlend");
            textBoxHttpUrl.IsEnabled = false;
            SavePath.IsEnabled = false;
            if (!rurl.StartsWith("http://") && !rurl.StartsWith("https://"))
            {
                if (System.IO.File.Exists(rurl))
                {
                    mSaveFileName = rurl;
                    Stop_Download(true);
                    return false;
                }
                else
                {
                    App.Notify(new Hiro_Notice(HText.Get_Translate("syntax"), 2, HText.Get_Translate("download")));
                    textBoxHttpUrl.Focus();//url地址栏获取焦点
                    Stop_Download(false);
                    return false;
                }
            }
            var strFileName = rurl;
            strFileName = strFileName[(strFileName.LastIndexOf("/") + 1)..];
            if (strFileName.LastIndexOf("?") != -1)
                strFileName = strFileName[..strFileName.LastIndexOf("?")];
            if (strFileName.Equals(string.Empty))
                strFileName = "index.html";
            if (SavePath.Text.Equals(string.Empty))
            {
                SavePath.Text = "<idownload>\\HiDownload\\<filename>";
            }
            if (current < 0)
                rpath = SavePath.Text;
            mSaveFileName = HText.Path_Prepare(rpath);
            mSaveFileName = HText.Path_Prepare_EX(mSaveFileName);
            mSaveFileName = HText.Path_Replace(mSaveFileName, "<filename>", strFileName);
            if (mSaveFileName.IndexOf("<index>") != -1)
            {
                while (System.IO.File.Exists(HText.Path_Replace(mSaveFileName, "<index>", index.ToString())))
                    index++;
                mSaveFileName = HText.Path_Replace(mSaveFileName, "<index>", index.ToString());
                index++;
            }
            HFile.CreateFolder(mSaveFileName);
            if (mSaveFileName.EndsWith("\\"))
                mSaveFileName += strFileName;
            if (System.IO.File.Exists(mSaveFileName) || App.hc == null)
            {
                Stop_Download(true);
                return false;
            }
            return true;
        }

        public void StartDownload()
        {
            if (!DownloadUIActions())
                return;
            _timer = new DispatcherTimer()
            {
                Interval = HSet.Read_DCIni("Performance", "0") switch
                {
                    "1" => TimeSpan.FromMilliseconds(150),
                    "2" => TimeSpan.FromMilliseconds(500),
                    _ => TimeSpan.FromMilliseconds(15)
                }
            };
            _timer.Tick += delegate
            {
                UpdateTitle();
            };
            _timer.Start();
            new Thread(async () =>
            {
                var response = await App.hc.GetAsync(rurl, System.Net.Http.HttpCompletionOption.ResponseHeadersRead);
                var totalLength = response.Content.Headers.ContentLength;
                System.IO.FileStream? fileStream;
                try
                {
                    fileStream = System.IO.File.OpenWrite(mSaveFileName + ".hdp");
                }
                catch (Exception ex)
                {
                    HLogger.LogError(ex, "Hiro.Exception.Download.Continue");
                    Dispatcher.Invoke(() =>
                    {
                        App.Notify(new Hiro_Notice(HText.Get_Translate("dlerror"), 2, HText.Get_Translate("download")));
                        Stop_Download(false);
                    });
                    return;
                }
                if (totalLength > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Normal, new System.Windows.Interop.WindowInteropHelper(this).Handle);
                    });
                    if (System.IO.File.Exists(mSaveFileName + ".hdp"))//文件已经存在就继续下载
                    {
                        try
                        {
                            if (null == fileStream)
                                startpos = 0;
                            else
                            {
                                startpos = fileStream.Length;
                                fileStream.Seek(startpos, System.IO.SeekOrigin.Begin);
                            }

                        }
                        catch (Exception ex)
                        {
                            HLogger.LogError(ex, "Hiro.Exception.Download.Stream");
                            startpos = 0;
                        }
                    }
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Indeterminate, new System.Windows.Interop.WindowInteropHelper(this).Handle);
                    });
                    startpos = 0;
                }
                if (startpos > 0)
                {
                    System.Net.Http.HttpRequestMessage request = new(System.Net.Http.HttpMethod.Get, rurl);
                    request.Headers.Add("UserAgent", Hiro_Resources.AppUserAgent);
                    if (startpos >= totalLength)
                    {
                        successflag = true;
                        goto DownloadFinish;
                    }
                    request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(startpos, totalLength);
                    response = await App.hc.SendAsync(request, System.Net.Http.HttpCompletionOption.ResponseHeadersRead);
                }
                var contentStream = await response.Content.ReadAsStreamAsync();
                var buffer = new byte[4 * 1024];//4KB缓存
                long readLength = 0L;
                int length;
                successflag = true;
                while ((length = await contentStream.ReadAsync(buffer)) > 0)
                {
                    readLength += length;
                    try
                    {
                        fileStream?.Write(buffer, 0, length);
                    }
                    catch (Exception ex)
                    {
                        HLogger.LogError(ex, "Hiro.Exception.Download.Write");
                        Dispatcher.Invoke(() =>
                        {
                            App.Notify(new Hiro_Notice(HText.Get_Translate("dlerror"), 2, HText.Get_Translate("download")));
                        });
                        successflag = false;
                        break;
                    }
                    if (totalLength > 0)
                    {
                        nextTitle = progress +
                                        $"{Math.Round(((double)readLength + startpos) / totalLength.Value * 100, 2):F2}" + "%" + "(" + FormateSize(readLength + startpos) + "/" + FormateSize(totalLength.Value) + ")";
                        nextProgess = Math.Round(((double)readLength + startpos) / totalLength.Value * 100, 2);
                        if (readLength + startpos >= totalLength)
                        {
                            break;
                        }
                    }
                    else
                    {
                        nextTitle = progress + FormateSize(readLength + startpos) + "/" + HText.Get_Translate("dlunknown");
                        nextProgess = -1;
                    }
                    if (stopflag != 0)
                    {
                        successflag = false;
                        break;
                    }
                }
                fileStream?.Close();
            DownloadFinish:
                if (successflag)
                {
                    try
                    {
                        System.IO.FileInfo file = new(mSaveFileName + ".hdp");
                        file.MoveTo(mSaveFileName);
                    }
                    catch (Exception ex)
                    {
                        HLogger.LogError(ex, "Hiro.Exception.Download.Save");
                    }
                }
                Dispatcher.Invoke(() =>
                {
                    Stop_Download(successflag);
                });
            }).Start();
        }

        private void UpdateTitle()
        {
            ala_title.Content = nextTitle;
            Title = HText.Get_Translate("dlTitle").Replace("%t", nextTitle).Replace("%a", App.appTitle);
            if (nextProgess >= 0 && nextProgess <= 100)
            {
                pb.Value = nextProgess;
                Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressValue((int)nextProgess, 100, new System.Windows.Interop.WindowInteropHelper(this).Handle);
            }
            else
            {
                if (!pb.IsIndeterminate)
                {
                    pb.IsIndeterminate = true;
                }
            }
        }
        private void Stop_Download(bool success)
        {
            _timer?.Stop();
            _timer = null;
            if (success && mSaveFileName.ToLower().EndsWith(".hidl"))
            {
                if (current < 0)
                {
                    listfile = mSaveFileName;
                    current = 0;
                }
            }
            if (success && current > -1)
            {
                if (System.IO.File.Exists(listfile))
                {
                    try
                    {
                        var filec = System.IO.File.ReadAllLines(listfile);
                        if (current < filec.Length)
                        {
                            var str = filec[current];
                            if (!str.ToLower().StartsWith("http://") && !str.ToLower().StartsWith("https://"))
                            {
                                Hiro_Utils.RunExe(str, HText.Get_Translate("dltitle"), false);
                                Autorun.IsChecked = false;
                            }
                            else
                            {
                                if (str.IndexOf("|") != -1)
                                {
                                    rurl = str[..str.IndexOf("|")];
                                    rpath = str[(str.IndexOf("|") + 1)..];
                                }
                                else
                                {
                                    rurl = str;
                                    rpath = SavePath.Text;
                                }
                                current++;
                                progress = "[" + current.ToString() + "/" + filec.Length.ToString() + "]";
                                StartDownload();
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        HLogger.LogError(ex, "Hiro.Exception.Download.ListFile");
                    }
                }
            }
            progress = "";
            Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.NoProgress, new System.Windows.Interop.WindowInteropHelper(this).Handle);
            pb.IsIndeterminate = false;
            pb.Value = 0;
            albtn_1.Content = HText.Get_Translate("dlstart");
            textBoxHttpUrl.IsEnabled = true;
            SavePath.IsEnabled = true;
            ala_title.Content = progress + (success ? HText.Get_Translate("dlsuccess") : HText.Get_Translate("dltitle"));
            Title = HText.Get_Translate("dlTitle").Replace("%t", ala_title.Content.ToString()).Replace("%a", App.appTitle);
            stopflag = 1;
            if (success)
            {
                App.Notify(new Hiro_Notice(HText.Get_Translate("dlsuccess"), 2, HText.Get_Translate("download")));
                current = -1;
                listfile = "";
            }
            if (success && Autorun.IsChecked == true)
            {
                Hiro_Utils.RunExe("explorer \"" + mSaveFileName + "\"", HText.Get_Translate("dltitle"), false);
            }
            if (success && Autorun.IsChecked == null)
            {
                Hiro_Utils.RunExe(mSaveFileName[..mSaveFileName.LastIndexOf("\\")], HText.Get_Translate("dltitle"), false);
            }
            if (Autorun.IsEnabled == false)
            {
                Close();
            }
        }
        public static string FormateSize(double size)
        {
            var units = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
            var mod = 1024.0;
            var i = 0;
            while (size >= mod)
            {
                size /= mod;
                i++;
            }
            return size.ToString("f2") + units[i];
        }
        private void Albtn_1_Click(object sender, RoutedEventArgs e)
        {
            if (albtn_1.Content.Equals(HText.Get_Translate("dlstart")))
            {
                if (current > 1)
                    progress = "[" + current.ToString() + "/?]";
                rurl = textBoxHttpUrl.Text.Trim();
                StartDownload();
            }
            else//停止
            {

                Stop_Download(false);
                if (Autorun.IsEnabled == false)
                    Close();
            }
        }

        private void Alarmgrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }

        private void Closebtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
            e.Handled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (stopflag == 0)
                Stop_Download(false);
        }

        private void TextBoxHttpUrl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Clipboard.ContainsText())
                textBoxHttpUrl.Text = Clipboard.GetText();
        }

        private void Urllabel_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Clipboard.ContainsText())
                textBoxHttpUrl.Text = Clipboard.GetText();
        }

        public void Loadbgi(int direction)
        {
            if (HSet.Read_DCIni("Background", "1").Equals("3"))
            {
                compositor ??= new(this);
                HUI.Set_Acrylic(bgimage, this, windowChrome, compositor);
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
            bool animation = !HSet.Read_DCIni("Ani", "2").Equals("0");
            HAnimation.Blur_Animation(direction, animation, bgimage, this);
            bflag = 0;
        }

        private void Autorun_Indeterminate(object sender, RoutedEventArgs e)
        {
            Autorun.Content = HText.Get_Translate("dlopen");
            HUI.Set_Control_Location(Autorun, "dlopen", bottom: true, animation: HSet.Read_DCIni("Ani", "2").Equals("1"), animationTime: 250);
        }

        private void Autorun_Unchecked(object sender, RoutedEventArgs e)
        {
            Autorun.Content = HText.Get_Translate("dlrun");
            HUI.Set_Control_Location(Autorun, "dlrun", bottom: true, animation: HSet.Read_DCIni("Ani", "2").Equals("1"), animationTime: 250);
        }

        private void Minbtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
            e.Handled = true;
        }

        private void VirtualTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }
    }

}
