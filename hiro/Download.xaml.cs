using System;
using System.Windows;
using System.Windows.Media;

namespace hiro
{
    /// <summary>
    /// Download.xaml の相互作用ロジック
    /// </summary>
    public partial class Download : Window
    {
        internal string clips = "";
        private string mSaveFileName = "";//下载文件的保存文件名

        internal long startpos = 0;
        internal int mode = 0;//1=更新模式
        internal string product = "";
        internal int bflag = 0;
        private int stopflag = 0;
        private int successflag = 0;
        private int index = 0;
        public Download(int i, string st)
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            mode = i;
            product = st;
            Load_Colors();
            Load_Position();
            Load_Translate();
            Loaded += delegate {
                    Loadbgi(utils.ConvertInt(utils.Read_Ini(App.dconfig, "Configuration", "blur", "0")));
            };
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;
            System.Windows.Interop.HwndSource source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source.AddHook(WndProc);
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
            Title = utils.Get_Transalte("dltitle") + " - " + App.AppTitle;
            ala_title.Content = utils.Get_Transalte("dltitle");
            albtn_1.Content = utils.Get_Transalte("dlstart");
            if (mode == 1)
                urllabel.Content = utils.Get_Transalte("dlupdate").Replace("%u", product);
            else
                urllabel.Content = utils.Get_Transalte("dllink");
            pathlabel.Content = utils.Get_Transalte("dlpath");
            autorun.Content = utils.Get_Transalte("dlrun");
            minbtn.ToolTip = utils.Get_Transalte("min");
            closebtn.ToolTip = utils.Get_Transalte("close");
        }
        void Load_Position()
        {
            utils.Set_Control_Location(this, "dlwin");
            utils.Set_Control_Location(albtn_1, "dlstart", bottom: true, right: true);
            utils.Set_Control_Location(pb, "dlprogress");
            utils.Set_Control_Location(ala_title, "dltitle");
            if (mode == 1) 
            {
                utils.Set_Control_Location(urllabel, "dlupdate");
                pathlabel.Visibility = Visibility.Hidden;
                textBoxHttpUrl.Visibility = Visibility.Hidden;
                SavePath.Visibility = Visibility.Hidden;
                autorun.Visibility = Visibility.Hidden;
                return;
            }
            utils.Set_Control_Location(urllabel, "dllink");
            utils.Set_Control_Location(pathlabel, "dlpath");
            utils.Set_Control_Location(textBoxHttpUrl, "dlurl");
            utils.Set_Control_Location(SavePath, "dlsave");
            utils.Set_Control_Location(autorun, "dlrun", bottom: true);
            System.Windows.Controls.Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - this.Width / 2);
            System.Windows.Controls.Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - this.Height / 2);

        }
        public void Load_Colors()
        {
            ala_title.Foreground = new SolidColorBrush(App.AppForeColor);
            albtn_1.Foreground = new SolidColorBrush(App.AppForeColor);
            urllabel.Foreground = new SolidColorBrush(App.AppForeColor);
            pathlabel.Foreground = new SolidColorBrush(App.AppForeColor);
            minbtn.Foreground = new SolidColorBrush(App.AppForeColor);
            closebtn.Foreground = new SolidColorBrush(App.AppForeColor);
            textBoxHttpUrl.Foreground = new SolidColorBrush(App.AppForeColor);
            SavePath.Foreground = new SolidColorBrush(App.AppForeColor);
            autorun.Foreground = new SolidColorBrush(App.AppForeColor);
            albtn_1.Background = new SolidColorBrush(Color.FromArgb(160, App.AppAccentColor.R, App.AppAccentColor.G, App.AppAccentColor.B));
            albtn_1.BorderThickness = new Thickness(1, 1, 1, 1);
            albtn_1.BorderBrush = new SolidColorBrush(App.AppForeColor);
            pb.Foreground = albtn_1.Foreground;
            borderlabel.BorderBrush = new SolidColorBrush(App.AppForeColor);
            coloruse1.Background = new SolidColorBrush(Color.FromArgb(80, App.AppForeColor.R, App.AppForeColor.G, App.AppForeColor.B));
        }

        public async void StartDownload()
        {
            //获取http下载路径
            stopflag = 0;
            pb.Value = 0;
            albtn_1.Content = utils.Get_Transalte("dlend");
            textBoxHttpUrl.IsEnabled = false;
            SavePath.IsEnabled = false;
            string httpUrl = textBoxHttpUrl.Text.Trim();
            if (!httpUrl.StartsWith("http://") && !httpUrl.StartsWith("https://"))
            {
                App.Notify(new noticeitem(utils.Get_Transalte("syntax"), 2));
                textBoxHttpUrl.Focus();//url地址栏获取焦点
                Stop_Download(false);
                return;
            }
            string strFileName = textBoxHttpUrl.Text;
            strFileName = strFileName[(strFileName.LastIndexOf("/") + 1)..];
            if (strFileName.LastIndexOf("?") != -1)
                strFileName = strFileName[..strFileName.LastIndexOf("?")];
            if (strFileName.Equals(String.Empty))
                strFileName = "index.html";
            if (SavePath.Text.Equals(String.Empty))
            {
                SavePath.Text = "<idocument>\\<filename>";
            }
            mSaveFileName = utils.Path_Prepare(SavePath.Text);
            mSaveFileName = utils.Path_Prepare_EX(mSaveFileName);
            mSaveFileName = utils.Path_Replace(mSaveFileName, "<filename>", strFileName);
            if (mSaveFileName.IndexOf("<index>") != -1)
            {
                while (System.IO.File.Exists(utils.Path_Replace(mSaveFileName, "<index>", index.ToString())) || System.IO.File.Exists(utils.Path_Replace(mSaveFileName + ".hdp", "<index>", index.ToString())))
                    index++;
                mSaveFileName = utils.Path_Replace(mSaveFileName, "<index>", index.ToString());
                index++;
            }
            utils.CreateFolder(mSaveFileName);
            if (mSaveFileName.EndsWith("\\"))
                mSaveFileName = mSaveFileName + strFileName;
            var response = await App.hc.GetAsync(httpUrl, System.Net.Http.HttpCompletionOption.ResponseHeadersRead);
            var totalLength = response.Content.Headers.ContentLength;
            System.IO.FileStream? fileStream = null;
            try
            {
                fileStream = System.IO.File.OpenWrite(mSaveFileName + ".hdp");
            }
            catch (Exception ex)
            {
                utils.LogtoFile("[ERROR]" + ex.Message);
                App.Notify(new noticeitem(utils.Get_Transalte("dlerror"), 2));
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
                            //App.Notify(new noticeitem("继续下载于" + startpos.ToString()));
                            fileStream.Seek(startpos, System.IO.SeekOrigin.Begin);
                        }

                    }
                    catch (Exception e)
                    {
                        utils.LogtoFile("[ERROR]" + e.Message);
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
                System.Net.Http.HttpRequestMessage request = new(System.Net.Http.HttpMethod.Get, httpUrl);
                request.Headers.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.71 Safari/537.36");
                if (startpos >= totalLength)
                {
                    successflag = 1;
                    goto DownloadFinish;
                }
                request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(startpos, totalLength);
                response = await App.hc.SendAsync(request, System.Net.Http.HttpCompletionOption.ResponseHeadersRead);
            }
            
            var contentStream = await response.Content.ReadAsStreamAsync();
            byte[] buffer = new byte[4 * 1024];//4KB缓存
            long readLength = 0L;
            int length;
            successflag = 1;
            while ((length = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                readLength += length;
                try
                {
                    if (fileStream != null)
                        fileStream.Write(buffer, 0, length);
                }
                catch (Exception ex)
                {
                    utils.LogtoFile("[ERROR]" + ex.Message);
                    App.Notify(new noticeitem(utils.Get_Transalte("dlerror"), 2));
                    successflag = 0;
                    break;
                }
                if (totalLength > 0)
                {
                    ala_title.Content = string.Format("{0:F2}", Math.Round(((double)readLength + startpos) / totalLength.Value * 100, 2)) + "%" + "(" + formateSize(readLength + startpos) + "/" + formateSize(totalLength.Value) + ")";
                    Title = ala_title.Content.ToString() + " - " + App.AppTitle;
                    pb.Value = Math.Round(((double)readLength + startpos) / totalLength.Value * 100, 2);
                    pb.IsIndeterminate = false;
                    Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressValue((int)(((double)readLength + startpos) / totalLength.Value * 100), 100, new System.Windows.Interop.WindowInteropHelper(this).Handle);
                    if (readLength + startpos >= totalLength)
                    {
                        successflag = 1;
                        break;
                    }
                }
                else
                {
                    ala_title.Content = formateSize(readLength + startpos) + "/" + utils.Get_Transalte("dlunknown");
                    Title = ala_title.Content.ToString() + " - " + App.AppTitle;
                    pb.IsIndeterminate = true;
                }
                if (stopflag != 0)
                {
                    successflag = 0;
                    break;
                }
            }
            if (fileStream != null)
                fileStream.Close();
            DownloadFinish:
            if (successflag == 1)
            {
                try
                {
                    System.IO.FileInfo file = new(mSaveFileName + ".hdp");
                    file.MoveTo(mSaveFileName);
                }
                catch (Exception ex)
                {
                    utils.LogtoFile("[ERROR]" + ex.Message);
                }
                App.Notify(new noticeitem(utils.Get_Transalte("dlsuccess"), 2));
                if (autorun.IsChecked == true)
                {
                    try
                    {
                        utils.RunExe("explorer \"" + mSaveFileName + "\"");

                    }
                    catch (Exception ex)
                    {
                        utils.LogtoFile("[ERROR]" + ex.Message);
                        App.Notify(new noticeitem(utils.Get_Transalte("dlrunerror"), 2));
                    }
                }
                Stop_Download(true);
            }
            else
                Stop_Download(false);
            if (autorun.IsEnabled == false)
                Close();
        }
        private void Stop_Download(bool success)
        {
            Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.NoProgress, new System.Windows.Interop.WindowInteropHelper(this).Handle);
            pb.IsIndeterminate = false;
            pb.Value = 0;
            albtn_1.Content = utils.Get_Transalte("dlstart");
            textBoxHttpUrl.IsEnabled = true;
            SavePath.IsEnabled = true;
            ala_title.Content = success ? utils.Get_Transalte("dlsuccess") : utils.Get_Transalte("dltitle");
            Title = ala_title.Content.ToString() + " - " + App.AppTitle;
            stopflag = 1;
        }
        public static string formateSize(double size)
        {
            string[] units = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
            double mod = 1024.0;
            int i = 0;
            while (size >= mod)
            {
                size /= mod;
                i++;
            }
            return size.ToString("f2") + units[i];
        }
        private void albtn_1_Click(object sender, RoutedEventArgs e)
        {
            if (albtn_1.Content.Equals(utils.Get_Transalte("dlstart")))
            {
                StartDownload();
            }
            else//停止
            {
                Stop_Download(false);
                if (autorun.IsEnabled == false)
                    Close();
            }
        }

        private void alarmgrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }

        private void minbtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void closebtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void copyclip_Checked(object sender, RoutedEventArgs e)
        {
            clips = Clipboard.GetText();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Stop_Download(false);
        }

        private void textBoxHttpUrl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(Clipboard.ContainsText())
                textBoxHttpUrl.Text = Clipboard.GetText();
        }

        private void urllabel_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Clipboard.ContainsText())
                textBoxHttpUrl.Text = Clipboard.GetText();
        }

        public void Loadbgi(int direction)
        {
            if (bflag == 1)
                return;
            bflag = 1;
            utils.Set_Bgimage(bgimage);
            bool animation = !utils.Read_Ini(App.dconfig, "Configuration", "ani", "1").Equals("0");
            utils.Blur_Animation(direction, animation, bgimage, this);
            bflag = 0;
        }
    }

}
