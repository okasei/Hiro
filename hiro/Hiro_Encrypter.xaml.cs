using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
{
    /// <summary>
    /// Download.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Encrypter : Window
    {
        internal int mode = 0;//1=解密模式
        internal int bflag = 0;
        internal Thread? th = null;
        internal string pwd = string.Empty;
        internal string fpath = string.Empty;
        internal bool oflag = false;
        public Hiro_Encrypter(int mode = 0, string? file = null, string? pwd = null, bool flag = false)
        {
            InitializeComponent();
            this.mode = mode;
            oflag = flag;
            if (oflag)
            {
                ShowInTaskbar = false;
                Visibility = Visibility.Hidden;
            }
            if (file != null)
                FilePath.Text = file;
            if (pwd != null)
                PwdPath.Password = pwd;
            SourceInitialized += OnSourceInitialized;
            Load_Colors();
            Load_Position();
            Load_Translate();
            Loaded += delegate
            {
                HiHiro();
            };
        }

        public void HiHiro()
        {
            Loadbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                Storyboard sb = new();
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
            Title = mode == 0 ? Hiro_Utils.Get_Translate("enentitle") + " - " + App.AppTitle : Hiro_Utils.Get_Translate("endetitle") + " - " + App.AppTitle;
            EncryptTitle.Content = Hiro_Utils.Get_Translate("enentitle");
            DecryptTitle.Content = Hiro_Utils.Get_Translate("endetitle");
            albtn_1.Content = mode == 0 ? Hiro_Utils.Get_Translate("enencrypt") : Hiro_Utils.Get_Translate("endecrypt");
            FileLabel.Content = Hiro_Utils.Get_Translate("enfile");
            PwdLabel.Content = Hiro_Utils.Get_Translate("enpwd");
            SeePwd.Content = Hiro_Utils.Get_Translate("enseepwd");
            if (Autorun.IsChecked != null)
                Autorun.Content = Hiro_Utils.Get_Translate("enrun");
            else
                Autorun.Content = Hiro_Utils.Get_Translate("enopen");
            minbtn.ToolTip = Hiro_Utils.Get_Translate("min");
            closebtn.ToolTip = Hiro_Utils.Get_Translate("close");
        }
        void Load_Position(bool relocate = true)
        {
            Hiro_Utils.Set_Control_Location(this, "enwin");
            Hiro_Utils.Set_Control_Location(albtn_1, "enstart", bottom: true, right: true);
            Hiro_Utils.Set_Control_Location(pb, "enprogress");
            if (mode == 0)
            {
                Hiro_Utils.Set_Control_Location(EncryptTitle, "enentitle1");
                Hiro_Utils.Set_Control_Location(DecryptTitle, "endetitle0");
            }
            else
            {
                Hiro_Utils.Set_Control_Location(EncryptTitle, "enentitle0");
                Hiro_Utils.Set_Control_Location(DecryptTitle, "endetitle1");
            }
            Hiro_Utils.Set_Control_Location(FileLabel, "enflabel");
            Hiro_Utils.Set_Control_Location(PwdLabel, "enplabel");
            Hiro_Utils.Set_Control_Location(SeePwd, "ensplabel");
            Hiro_Utils.Set_Control_Location(FilePath, "enfbox");
            Hiro_Utils.Set_Control_Location(PwdPath, "enpbox");
            if (Autorun.IsChecked != null)
                Hiro_Utils.Set_Control_Location(Autorun, "enrun", bottom: true);
            else
                Hiro_Utils.Set_Control_Location(Autorun, "enopen", bottom: true);
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                Storyboard sb = new();
                Hiro_Utils.AddPowerAnimation(1, EncryptTitle, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(1, DecryptTitle, sb, -50, null);
                sb.Begin();
            }
            if (relocate)
            {
                System.Windows.Controls.Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - this.Width / 2);
                System.Windows.Controls.Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - this.Height / 2);
            }
        }
        public void Load_Colors()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDimColor"] = Hiro_Utils.Color_Transparent(App.AppForeColor, 80);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
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
            if (th != null)
                th.Interrupt();
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

        private void Minbtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
            e.Handled = true;
        }

        private void PwdPath_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Clipboard.ContainsText())
                PwdPath.Password = Clipboard.GetText();
        }

        private void FilePath_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Clipboard.ContainsText())
                FilePath.Text = Clipboard.GetText();
        }

        private void FileLabel_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Clipboard.ContainsText())
                FilePath.Text = Clipboard.GetText();
        }

        private void PwdLabel_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Clipboard.ContainsText())
                PwdPath.Password = Clipboard.GetText();
        }

        private void SeePwd_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PwdPath.Password = string.Empty;
            PwdPath.Visibility = Visibility.Visible;
            SeePwd.Visibility = Visibility.Hidden;
            PwdPath.Focus();
        }

        internal bool? StartEncrypt(string path, string key)
        {
            var appname = mode == 0 ? Hiro_Utils.Get_Translate("enapp") : Hiro_Utils.Get_Translate("deapp");
            if (!File.Exists(path))
            {
                Hiro_Utils.RunExe($"notify({Hiro_Utils.Get_Translate("filenotexist")},2)", appname, false);
                return null;
            }
            if (File.Exists(path + ".hef"))
            {
                Hiro_Utils.RunExe($"notify({Hiro_Utils.Get_Translate("enfileexist")},2)", appname, false);
                return null;
            }
            try
            {
                File.WriteAllBytes(path + ".hef", Hiro_Utils.EncryptFile(File.ReadAllBytes(path).ToArray(), key, Path.GetFileName(path)));
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.Encrypt");
                Hiro_Utils.RunExe($"notify({Hiro_Utils.Get_Translate("enerror")},2)", appname, false);
                return false;
            }
            return true;
        }
        internal bool? StartDecrypt(string path, string key)
        {
            var appname = mode == 0 ? Hiro_Utils.Get_Translate("enapp") : Hiro_Utils.Get_Translate("deapp");
            if (!File.Exists(path))
            {
                Hiro_Utils.RunExe($"notify({Hiro_Utils.Get_Translate("filenotexist")},2)", appname, false);
                return null;
            }
            try
            {
                var filename = string.Empty;
                var b = Hiro_Utils.DecryptFile(File.ReadAllBytes(path), key, out filename);
                var dir = Path.GetDirectoryName(fpath);
                var fi = Path.GetFileNameWithoutExtension(dir + "\\" + filename);
                var ext = Path.GetExtension(dir + "\\" + filename);
                var num = -1;
                filename = fi;
                while (File.Exists($"{dir}\\{filename}{ext}"))
                {
                    num++;
                    filename = $"{fi}({num})";
                }
                fpath = $"{dir}\\{filename}{ext}";
                File.WriteAllBytes(fpath, b);
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.Decrypt");
                Hiro_Utils.RunExe($"notify({Hiro_Utils.Get_Translate("deerror")},2)", appname, false);
                return false;
            }
            return true;
        }

        private void Albtn_1_Click(object sender, RoutedEventArgs e)
        {
            if (th != null)
            {
                th.Interrupt();
                th = null;
                pb.Visibility = Visibility.Hidden;
                FilePath.IsEnabled = true;
                PwdPath.IsEnabled = true;
                Autorun.IsEnabled = true;
                EncryptTitle.IsEnabled = true;
                DecryptTitle.IsEnabled = true;
                albtn_1.Content = mode == 0 ? Hiro_Utils.Get_Translate("enencrypt") : Hiro_Utils.Get_Translate("endecrypt");
            }
            else
            {
                if (!FilePath.Text.Equals(string.Empty) && !pwd.Equals(string.Empty))
                {
                    pb.Visibility = Visibility.Visible;
                    FilePath.IsEnabled = false;
                    PwdPath.IsEnabled = false;
                    Autorun.IsEnabled = false;
                    EncryptTitle.IsEnabled = false;
                    DecryptTitle.IsEnabled = false;
                    albtn_1.Content = mode == 0 ? Hiro_Utils.Get_Translate("enencryptc") : Hiro_Utils.Get_Translate("endecryptc");
                    fpath = FilePath.Text;
                    th = new(() =>
                    {
                        var p = fpath;
                        var k = pwd;
                        bool? r;
                        if (mode == 0)
                            r = StartEncrypt(p, k);
                        else
                            r = StartDecrypt(p, k);
                        th = null;
                        if (r == true)
                            Dispatcher.Invoke(() =>
                            {
                                if (Autorun.IsChecked == true)
                                    Hiro_Utils.RunExe("explorer \"" + fpath + "\"", mode == 0 ? Hiro_Utils.Get_Translate("enapp") : Hiro_Utils.Get_Translate("deapp"), false);
                                if (Autorun.IsChecked == null)
                                    Hiro_Utils.RunExe(fpath[..fpath.LastIndexOf("\\")], mode == 0 ? Hiro_Utils.Get_Translate("enapp") : Hiro_Utils.Get_Translate("deapp"), false);
                                if (oflag)
                                {
                                    Close();
                                    return;
                                }
                                if (mode == 0)
                                    Hiro_Utils.RunExe($"notify({Hiro_Utils.Get_Translate("ensuccess")},2)", Hiro_Utils.Get_Translate("enapp"), false);
                                else
                                    Hiro_Utils.RunExe($"notify({Hiro_Utils.Get_Translate("desuccess")},2)", Hiro_Utils.Get_Translate("deapp"), false);
                            });
                        Dispatcher.Invoke(() =>
                        {
                            if (oflag)
                            {
                                Close();
                                return;
                            }
                            pb.Visibility = Visibility.Hidden;
                            FilePath.IsEnabled = true;
                            PwdPath.IsEnabled = true;
                            Autorun.IsEnabled = true;
                            EncryptTitle.IsEnabled = true;
                            DecryptTitle.IsEnabled = true;
                            albtn_1.Content = mode == 0 ? Hiro_Utils.Get_Translate("enencrypt") : Hiro_Utils.Get_Translate("endecrypt");
                        });
                    });
                    th.Start();
                }

            }
        }

        private void PwdPath_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!pwd.Equals(string.Empty) || !PwdPath.Password.Equals(string.Empty))
            {
                if (!PwdPath.Password.Equals(string.Empty))
                    pwd = PwdPath.Password;
                PwdPath.Password = string.Empty;
                PwdPath.Visibility = Visibility.Hidden;
                SeePwd.Visibility = Visibility.Visible;
            }
        }

        private void EncryptTitle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (mode != 0)
            {
                mode = 0;
                Load_Translate();
                Load_Position(false);
            }
        }

        private void DecryptTitle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (mode == 0)
            {
                mode = 1;
                Load_Translate();
                Load_Position(false);
            }
        }

        private void Autorun_Unchecked(object sender, RoutedEventArgs e)
        {
            Autorun.Content = Hiro_Utils.Get_Translate("enrun");
            Hiro_Utils.Set_Control_Location(Autorun, "enrun", bottom: true);
        }

        private void Autorun_Indeterminate(object sender, RoutedEventArgs e)
        {
            Autorun.Content = Hiro_Utils.Get_Translate("enopen");
            Hiro_Utils.Set_Control_Location(Autorun, "enopen", bottom: true);
        }

        private void FilePath_Drop(object sender, DragEventArgs e)
        {
            DealDragEventArgs(e);
        }

        private void DealDragEventArgs(DragEventArgs e)
        {
            var data = e.Data;
            var formats = data.GetData(DataFormats.FileDrop).GetType().ToString();
            if (formats.Equals("System.String[]"))
            {
                var info = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (info != null && info.Length > 0)
                {
                    FilePath.Text = info[0];
                }
            }
            else if (formats.Equals("System.String"))
            {
                var info = e.Data.GetData(DataFormats.FileDrop) as string;
                if (info != null)
                    FilePath.Text = info;
            }
        }

        private void alarmgrid_Drop(object sender, DragEventArgs e)
        {
            DealDragEventArgs(e);
        }
    }

}
