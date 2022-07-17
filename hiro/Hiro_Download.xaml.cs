using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
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
        public Hiro_Download(int i, string st)
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            mode = i;
            product = st;
            Load_Colors();
            Load_Position();
            Load_Translate();
            Loaded += delegate {
                HiHiro();
            };
        }

        public void HiHiro()
        {
            Loadbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                Storyboard sb = new();
                Hiro_Utils.AddPowerAnimation(1, ala_title, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(2, minbtn, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(2, closebtn, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(0, Autorun, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(3, albtn_1, sb, -50, null);
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
            Title = Hiro_Utils.Get_Transalte("dltitle") + " - " + App.AppTitle;
            ala_title.Content = Hiro_Utils.Get_Transalte("dltitle");
            albtn_1.Content = Hiro_Utils.Get_Transalte("dlstart");
            urllabel.Content = mode == 1 ? Hiro_Utils.Get_Transalte("dlupdate").Replace("%u", product) : Hiro_Utils.Get_Transalte("dllink");
            pathlabel.Content = Hiro_Utils.Get_Transalte("dlpath");
            Autorun.Content = Hiro_Utils.Get_Transalte("dlrun");
            minbtn.ToolTip = Hiro_Utils.Get_Transalte("min");
            closebtn.ToolTip = Hiro_Utils.Get_Transalte("close");
        }
        void Load_Position()
        {
            Hiro_Utils.Set_Control_Location(this, "dlwin");
            Hiro_Utils.Set_Control_Location(albtn_1, "dlstart", bottom: true, right: true);
            Hiro_Utils.Set_Control_Location(pb, "dlprogress");
            Hiro_Utils.Set_Control_Location(ala_title, "dltitle");
            if (mode == 1) 
            {
                Hiro_Utils.Set_Control_Location(urllabel, "dlupdate");
                pathlabel.Visibility = Visibility.Hidden;
                textBoxHttpUrl.Visibility = Visibility.Hidden;
                SavePath.Visibility = Visibility.Hidden;
                Autorun.Visibility = Visibility.Hidden;
                return;
            }
            Hiro_Utils.Set_Control_Location(urllabel, "dllink");
            Hiro_Utils.Set_Control_Location(pathlabel, "dlpath");
            Hiro_Utils.Set_Control_Location(textBoxHttpUrl, "dlurl");
            Hiro_Utils.Set_Control_Location(SavePath, "dlsave");
            Hiro_Utils.Set_Control_Location(Autorun, "dlrun", bottom: true);
            System.Windows.Controls.Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - this.Width / 2);
            System.Windows.Controls.Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - this.Height / 2);

        }
        public void Load_Colors()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDimColor"] = Hiro_Utils.Color_Transparent(App.AppForeColor, 80);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        public async void StartDownload()
        {
            //获取http下载路径
            stopflag = 0;
            pb.Value = 0;
            albtn_1.Content = Hiro_Utils.Get_Transalte("dlend");
            textBoxHttpUrl.IsEnabled = false;
            SavePath.IsEnabled = false;
            if (!rurl.StartsWith("http://") && !rurl.StartsWith("https://"))
            {
                if (System.IO.File.Exists(rurl))
                {
                    mSaveFileName = rurl;
                    Stop_Download(true);
                    return;
                }
                else
                {
                    App.Notify(new Hiro_Notice(Hiro_Utils.Get_Transalte("syntax"), 2, Hiro_Utils.Get_Transalte("download")));
                    textBoxHttpUrl.Focus();//url地址栏获取焦点
                    Stop_Download(false);
                    return;
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
            mSaveFileName = Hiro_Utils.Path_Prepare(rpath);
            mSaveFileName = Hiro_Utils.Path_Prepare_EX(mSaveFileName);
            mSaveFileName = Hiro_Utils.Path_Replace(mSaveFileName, "<filename>", strFileName);
            if (mSaveFileName.IndexOf("<index>") != -1)
            {
                while (System.IO.File.Exists(Hiro_Utils.Path_Replace(mSaveFileName, "<index>", index.ToString())))
                    index++;
                mSaveFileName = Hiro_Utils.Path_Replace(mSaveFileName, "<index>", index.ToString());
                index++;
            }
            Hiro_Utils.CreateFolder(mSaveFileName);
            if (mSaveFileName.EndsWith("\\"))
                mSaveFileName += strFileName;
            if (System.IO.File.Exists(mSaveFileName) || App.hc == null)
            {
                Stop_Download(true);
                return;
            }
            var response = await App.hc.GetAsync(rurl, System.Net.Http.HttpCompletionOption.ResponseHeadersRead);
            var totalLength = response.Content.Headers.ContentLength;
            System.IO.FileStream? fileStream;
            try
            {
                fileStream = System.IO.File.OpenWrite(mSaveFileName + ".hdp");
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.Download.Continue");
                App.Notify(new Hiro_Notice(Hiro_Utils.Get_Transalte("dlerror"), 2, Hiro_Utils.Get_Transalte("download")));
                Stop_Download(false);
                return;
            }
            if (totalLength > 0)
            {
                Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Normal, new System.Windows.Interop.WindowInteropHelper(this).Handle);
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
                        Hiro_Utils.LogError(ex, "Hiro.Exception.Download.Stream");
                        startpos = 0;
                    }
                }
            }
            else
            {
                Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Indeterminate, new System.Windows.Interop.WindowInteropHelper(this).Handle);
                startpos = 0;
            }
            if (startpos > 0)
            {
                System.Net.Http.HttpRequestMessage request = new(System.Net.Http.HttpMethod.Get, rurl);
                request.Headers.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.71 Safari/537.36");
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
                    Hiro_Utils.LogError(ex, "Hiro.Exception.Download.Write");
                    App.Notify(new Hiro_Notice(Hiro_Utils.Get_Transalte("dlerror"), 2, Hiro_Utils.Get_Transalte("download")));
                    successflag = false;
                    break;
                }
                if (totalLength > 0)
                {
                    ala_title.Content = progress +
                                        $"{Math.Round(((double) readLength + startpos) / totalLength.Value * 100, 2):F2}" + "%" + "(" + FormateSize(readLength + startpos) + "/" + FormateSize(totalLength.Value) + ")";
                    Title = ala_title.Content.ToString() + " - " + App.AppTitle;
                    pb.Value = Math.Round(((double)readLength + startpos) / totalLength.Value * 100, 2);
                    pb.IsIndeterminate = false;
                    Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressValue((int)(((double)readLength + startpos) / totalLength.Value * 100), 100, new System.Windows.Interop.WindowInteropHelper(this).Handle);
                    if (readLength + startpos >= totalLength)
                    {
                        break;
                    }
                }
                else
                {
                    ala_title.Content = progress + FormateSize(readLength + startpos) + "/" + Hiro_Utils.Get_Transalte("dlunknown");
                    Title = ala_title.Content.ToString() + " - " + App.AppTitle;
                    pb.IsIndeterminate = true;
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
                    Hiro_Utils.LogError(ex, "Hiro.Exception.Download.Save");
                }
            }
            Stop_Download(successflag);
        }
        private void Stop_Download(bool success)
        {
            if (success && mSaveFileName.ToLower().EndsWith(".hidl"))
            {
                if(current < 0)
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
                                Hiro_Utils.RunExe(str);
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
                        Hiro_Utils.LogError(ex, "Hiro.Exception.Download.ListFile");
                    }
                }
            }
            progress = "";
            Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.NoProgress, new System.Windows.Interop.WindowInteropHelper(this).Handle);
            pb.IsIndeterminate = false;
            pb.Value = 0;
            albtn_1.Content = Hiro_Utils.Get_Transalte("dlstart");
            textBoxHttpUrl.IsEnabled = true;
            SavePath.IsEnabled = true;
            ala_title.Content = progress + (success ? Hiro_Utils.Get_Transalte("dlsuccess") : Hiro_Utils.Get_Transalte("dltitle"));
            Title = ala_title.Content.ToString() + " - " + App.AppTitle;
            stopflag = 1;
            if (success)
            {
                App.Notify(new Hiro_Notice(Hiro_Utils.Get_Transalte("dlsuccess"), 2, Hiro_Utils.Get_Transalte("download")));
                current = -1;
                listfile = "";
            }
            if (success && Autorun.IsChecked == true)
            {
                    Hiro_Utils.RunExe("explorer \"" + mSaveFileName + "\"");
            }
            if (success && Autorun.IsChecked == null)
            {
                Hiro_Utils.RunExe(mSaveFileName[..mSaveFileName.LastIndexOf("\\")]);
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
            if (albtn_1.Content.Equals(Hiro_Utils.Get_Transalte("dlstart")))
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
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (stopflag == 0)
                Stop_Download(false);
        }

        private void TextBoxHttpUrl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(Clipboard.ContainsText())
                textBoxHttpUrl.Text = Clipboard.GetText();
        }

        private void Urllabel_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Clipboard.ContainsText())
                textBoxHttpUrl.Text = Clipboard.GetText();
        }

        public void Loadbgi(int direction)
        {
            if (bflag == 1)
                return;
            bflag = 1;
            Hiro_Utils.Set_Bgimage(bgimage, this);
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            Hiro_Utils.Blur_Animation(direction, animation, bgimage, this);
            bflag = 0;
        }

        private void Autorun_Indeterminate(object sender, RoutedEventArgs e)
        {
            Autorun.Content = Hiro_Utils.Get_Transalte("dlopen");
            Hiro_Utils.Set_Control_Location(Autorun, "dlopen", bottom: true);
        }

        private void Autorun_Unchecked(object sender, RoutedEventArgs e)
        {
            Autorun.Content = Hiro_Utils.Get_Transalte("dlrun");
            Hiro_Utils.Set_Control_Location(Autorun, "dlrun", bottom: true);
        }

        private void Minbtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
            e.Handled = true;
        }
    }

}
