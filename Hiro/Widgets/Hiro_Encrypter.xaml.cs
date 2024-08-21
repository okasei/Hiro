using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Hiro.ModelViews;
using Hiro.Helpers;
using System.Windows.Controls;

namespace Hiro
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
        internal WindowAccentCompositor? compositor = null;
        private void VirtualTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }
        public Hiro_Encrypter(int mode = 0, string? file = null, string? pwd = null, bool flag = false)
        {
            InitializeComponent();
            HUI.SetCustomWindowIcon(this);
            SourceInitialized += OnSourceInitialized;
            this.mode = mode;
            oflag = flag;
            Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - Width / 2);
            Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - Height / 2);
            if (oflag)
            {
                ShowInTaskbar = false;
                Visibility = Visibility.Hidden;
            }
            if (file != null)
                FilePath.Text = file;
            Load_Colors();
            Load_Position();
            Load_Translate();
            Loaded += (e, args) =>
            {
                HiHiro();
                if (pwd != null && !pwd.Equals(string.Empty))
                {
                    PwdPath.Password = pwd;
                    HidePwd();
                }
                Hiro_Utils.SetShadow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
                Hiro_Utils.SetWindowToForegroundWithAttachThreadInput(this);
            };
        }

        public void HiHiro()
        {
            Loadbgi(Hiro_Utils.ConvertInt(HSet.Read_DCIni("Blur", "0")));
            if (HSet.Read_DCIni("Ani", "2").Equals("1"))
            {
                Storyboard sb = new();
                HAnimation.AddPowerAnimation(2, minbtn, sb, -50, null);
                HAnimation.AddPowerAnimation(2, closebtn, sb, -50, null);
                HAnimation.AddPowerAnimation(0, Autorun, sb, -50, null);
                HAnimation.AddPowerAnimation(0, Autodelete, sb, -50, null);
                HAnimation.AddPowerAnimation(3, albtn_1, sb, -50, null);
                sb.Begin();
            }
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;
            var source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source?.AddHook(WndProc);
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
            Title = HText.Get_Translate("encryptTitle").Replace("%t", mode == 0 ? HText.Get_Translate("enentitle") : HText.Get_Translate("endetitle")).Replace("%a", App.appTitle);
            EncryptTitle.Content = HText.Get_Translate("enentitle");
            DecryptTitle.Content = HText.Get_Translate("endetitle");
            albtn_1.Content = mode == 0 ? HText.Get_Translate("enencrypt") : HText.Get_Translate("endecrypt");
            FileLabel.Content = HText.Get_Translate("enfile");
            PwdLabel.Content = HText.Get_Translate("enpwd");
            SeePwd.Content = HText.Get_Translate("enseepwd");
            if (Autorun.IsChecked != null)
                Autorun.Content = HText.Get_Translate("enrun");
            else
                Autorun.Content = HText.Get_Translate("enopen");
            Autodelete.Content = HText.Get_Translate("endelete");
            minbtn.ToolTip = HText.Get_Translate("min");
            closebtn.ToolTip = HText.Get_Translate("close");
        }
        void Load_Position()
        {
            HUI.Set_Control_Location(this, "enwin");
            HUI.Set_Control_Location(albtn_1, "enstart", bottom: true, right: true);
            HUI.Set_Control_Location(pb, "enprogress");
            if (mode == 0)
            {
                HUI.Set_Control_Location(EncryptTitle, "enentitle1", animation: HSet.Read_DCIni("Ani", "2").Equals("1"), animationTime: 250);
                HUI.Set_Control_Location(DecryptTitle, "endetitle0", animation: HSet.Read_DCIni("Ani", "2").Equals("1"), animationTime: 250);
            }
            else
            {
                HUI.Set_Control_Location(EncryptTitle, "enentitle0", animation: HSet.Read_DCIni("Ani", "2").Equals("1"), animationTime: 250);
                HUI.Set_Control_Location(DecryptTitle, "endetitle1", animation: HSet.Read_DCIni("Ani", "2").Equals("1"), animationTime: 250);
            }
            HUI.Set_Control_Location(FileLabel, "enflabel");
            HUI.Set_Control_Location(PwdLabel, "enplabel");
            HUI.Set_Control_Location(SeePwd, "ensplabel");
            HUI.Set_Control_Location(FilePath, "enfbox");
            HUI.Set_Control_Location(PwdPath, "enpbox");
            if (Autorun.IsChecked != null)
            {
                HUI.Set_Control_Location(Autorun, "enrun", bottom: true);
                HUI.Set_Control_Location(Autodelete, "endelete", bottom: true);
            }
            else
            {
                HUI.Set_Control_Location(Autorun, "enopen", bottom: true);
                HUI.Set_Control_Location(Autodelete, "endeleter", bottom: true);
            }
            HUI.Set_Control_Location(Autodelete, "endelete", bottom: true);
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
            var appname = mode == 0 ? HText.Get_Translate("enapp") : HText.Get_Translate("deapp");
            HSet.Write_Ini(App.dConfig, "Config", "DefaultPwd", pwd);
            Hiro_Utils.RunExe($"notify({HText.Get_Translate("enpwdsaved")},2)", appname, false);
        }

        private void SeePwd_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PwdPath.Password = string.Empty;
            PwdPath.Visibility = Visibility.Visible;
            SeePwd.Visibility = Visibility.Hidden;
            PwdPath.Focus();
        }
        internal void StartEncryptThread(string path, string key)
        {
            new Thread(() =>
            {
                StartEncrypt(path, key);
            }).Start();
        }
        internal void StartDecryptThread(string path, string key)
        {
            new Thread(() =>
            {
                StartDecrypt(path, key);
            }).Start();
        }

        internal void EncryptDirectory(string path, string key)
        {
            var appname = mode == 0 ? HText.Get_Translate("enapp") : HText.Get_Translate("deapp");
            if (Directory.Exists(path))
            {
                DirectoryInfo directory = new DirectoryInfo(path);
                var files = directory.GetFiles("*", SearchOption.TopDirectoryOnly);
                var fileList = files.Select(s => s.FullName).ToList();
                foreach (var file in fileList)
                {
                    StartEncrypt(file, key);
                }
            }
            else
            {
                Hiro_Utils.RunExe($"notify({HText.Get_Translate("filenotexist")},2)", appname, false);
                return;
            }
        }

        internal void DecryptDirectory(string path, string key)
        {
            var appname = mode == 0 ? HText.Get_Translate("enapp") : HText.Get_Translate("deapp");
            if (Directory.Exists(path))
            {
                DirectoryInfo directory = new DirectoryInfo(path);
                var files = directory.GetFiles("*", SearchOption.TopDirectoryOnly);
                var fileList = files.Select(s => s.FullName).ToList();
                foreach (var file in fileList)
                {
                    StartDecrypt(file, key);
                }
            }
            else
            {
                Hiro_Utils.RunExe($"notify({HText.Get_Translate("filenotexist")},2)", appname, false);
                return;
            }
        }

        internal bool? StartEncrypt(string path, string key)
        {
            var appname = mode == 0 ? HText.Get_Translate("enapp") : HText.Get_Translate("deapp");
            if (!File.Exists(path))
            {
                Hiro_Utils.RunExe($"notify({HText.Get_Translate("filenotexist")},2)", appname, false);
                return null;
            }
            if (File.Exists(path + ".hef"))
            {
                Hiro_Utils.RunExe($"notify({HText.Get_Translate("enfileexist")},2)", appname, false);
                return null;
            }
            try
            {
                File.WriteAllBytes(path + ".hef", Hiro_Utils.EncryptFile(File.ReadAllBytes(path).ToArray(), key, Path.GetFileName(path)));
                Dispatcher.Invoke(() =>
                {
                    if (Autodelete.IsChecked == true)
                        try
                        {
                            File.Delete(path);
                        }
                        catch (Exception ex)
                        {
                            HLogger.LogError(ex, "Hiro.Exception.Encrypt.Delete");
                        }
                });
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, "Hiro.Exception.Encrypt");
                Hiro_Utils.RunExe($"notify({HText.Get_Translate("enerror")},2)", appname, false);
                return false;
            }
            return true;
        }
        internal bool? StartDecrypt(string path, string key)
        {
            var appname = mode == 0 ? HText.Get_Translate("enapp") : HText.Get_Translate("deapp");
            if (!File.Exists(path))
            {
                Hiro_Utils.RunExe($"notify({HText.Get_Translate("filenotexist")},2)", appname, false);
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
                Dispatcher.Invoke(() =>
                {
                    if (Autodelete.IsChecked == true)
                        try
                        {
                            File.Delete(path);
                        }
                        catch (Exception ex)
                        {
                            HLogger.LogError(ex, "Hiro.Exception.Decrypt.Delete");
                        }
                });

            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, "Hiro.Exception.Decrypt");
                Hiro_Utils.RunExe($"notify({HText.Get_Translate("deerror")},2)", appname, false);
                return false;
            }
            return true;
        }

        private void Albtn_1_Click(object sender, RoutedEventArgs e)
        {
            if (pwd.Equals(string.Empty))
            {
                pwd = HSet.Read_DCIni("DefaultPwd", string.Empty);
            }
            GoStart();
        }

        internal void GoStart()
        {
            if (th != null)
            {
                th.Interrupt();
                th = null;
                pb.Visibility = Visibility.Hidden;
                FilePath.IsEnabled = true;
                PwdPath.IsEnabled = true;
                Autorun.IsEnabled = true;
                Autodelete.IsEnabled = true;
                SeePwd.IsEnabled = true;
                EncryptTitle.IsEnabled = true;
                DecryptTitle.IsEnabled = true;
                albtn_1.Content = mode == 0 ? HText.Get_Translate("enencrypt") : HText.Get_Translate("endecrypt");
            }
            else
            {
                if (Directory.Exists(FilePath.Text) && !pwd.Equals(string.Empty))
                {
                    pb.Visibility = Visibility.Visible;
                    FilePath.IsEnabled = false;
                    PwdPath.IsEnabled = false;
                    Autorun.IsEnabled = false;
                    Autodelete.IsEnabled = false;
                    SeePwd.IsEnabled = false;
                    EncryptTitle.IsEnabled = false;
                    DecryptTitle.IsEnabled = false;
                    fpath = FilePath.Text;
                    albtn_1.Content = mode == 0 ? HText.Get_Translate("enencryptc") : HText.Get_Translate("endecryptc");
                    th = new(() =>
                    {
                        var p = fpath;
                        var k = pwd;
                        if (mode == 0)
                            EncryptDirectory(p, k);
                        else
                            DecryptDirectory(p, k);
                        th = null;
                        if (oflag)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Close();
                            });
                            return;
                        }
                        Dispatcher.Invoke(() =>
                        {
                            if (mode == 0)
                                Hiro_Utils.RunExe($"notify({HText.Get_Translate("ensuccess")},2)", HText.Get_Translate("enapp"), false);
                            else
                                Hiro_Utils.RunExe($"notify({HText.Get_Translate("desuccess")},2)", HText.Get_Translate("deapp"), false);
                            pb.Visibility = Visibility.Hidden;
                            FilePath.IsEnabled = true;
                            PwdPath.IsEnabled = true;
                            Autorun.IsEnabled = true;
                            Autodelete.IsEnabled = true;
                            SeePwd.IsEnabled = true;
                            EncryptTitle.IsEnabled = true;
                            DecryptTitle.IsEnabled = true;
                            albtn_1.Content = mode == 0 ? HText.Get_Translate("enencrypt") : HText.Get_Translate("endecrypt");
                        });
                    });
                    th.Start();
                }
                else if (!FilePath.Text.Equals(string.Empty) && !pwd.Equals(string.Empty))
                {
                    pb.Visibility = Visibility.Visible;
                    FilePath.IsEnabled = false;
                    PwdPath.IsEnabled = false;
                    Autorun.IsEnabled = false;
                    Autodelete.IsEnabled = false;
                    SeePwd.IsEnabled = false;
                    EncryptTitle.IsEnabled = false;
                    DecryptTitle.IsEnabled = false;
                    albtn_1.Content = mode == 0 ? HText.Get_Translate("enencryptc") : HText.Get_Translate("endecryptc");
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
                                    Hiro_Utils.RunExe("explorer \"" + fpath + "\"", mode == 0 ? HText.Get_Translate("enapp") : HText.Get_Translate("deapp"), false);
                                if (Autorun.IsChecked == null)
                                    Hiro_Utils.RunExe(fpath[..fpath.LastIndexOf("\\")], mode == 0 ? HText.Get_Translate("enapp") : HText.Get_Translate("deapp"), false);
                            });
                        if (oflag)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Close();
                            });
                            return;
                        }
                        Dispatcher.Invoke(() =>
                        {
                            if (mode == 0)
                                Hiro_Utils.RunExe($"notify({HText.Get_Translate("ensuccess")},2)", HText.Get_Translate("enapp"), false);
                            else
                                Hiro_Utils.RunExe($"notify({HText.Get_Translate("desuccess")},2)", HText.Get_Translate("deapp"), false);
                            pb.Visibility = Visibility.Hidden;
                            FilePath.IsEnabled = true;
                            PwdPath.IsEnabled = true;
                            Autorun.IsEnabled = true;
                            Autodelete.IsEnabled = true;
                            SeePwd.IsEnabled = true;
                            EncryptTitle.IsEnabled = true;
                            DecryptTitle.IsEnabled = true;
                            albtn_1.Content = mode == 0 ? HText.Get_Translate("enencrypt") : HText.Get_Translate("endecrypt");
                        });
                    });
                    th.Start();
                }

            }
        }

        private void PwdPath_LostFocus(object sender, RoutedEventArgs e)
        {
            HidePwd();
        }

        private void HidePwd()
        {
            if (!pwd.Equals(string.Empty) || !PwdPath.Password.Equals(string.Empty))
            {
                if (!PwdPath.Password.Equals(string.Empty))
                    pwd = PwdPath.Password;
                PwdPath.Password = string.Empty;
                PwdPath.Visibility = Visibility.Hidden;
                SeePwd.Visibility = Visibility.Visible;
                if (HSet.Read_DCIni("Ani", "2").Equals("1"))
                {
                    Storyboard sb = new();
                    HAnimation.AddPowerAnimation(1, SeePwd, sb, 50, null);
                    sb.Begin();
                }
            }
        }

        private void EncryptTitle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (mode != 0)
            {
                mode = 0;
                Load_Translate();
                Load_Position();
            }
        }

        private void DecryptTitle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (mode == 0)
            {
                mode = 1;
                Load_Translate();
                Load_Position();
            }
        }

        private void Autorun_Unchecked(object sender, RoutedEventArgs e)
        {
            Autorun.Content = HText.Get_Translate("enrun");
            HUI.Set_Control_Location(Autodelete, "endelete", bottom: true, animation: HSet.Read_DCIni("Ani", "2").Equals("1"), animationTime: 250);
            HUI.Set_Control_Location(Autorun, "enrun", bottom: true, animation: HSet.Read_DCIni("Ani", "2").Equals("1"), animationTime: 250);
        }

        private void Autorun_Indeterminate(object sender, RoutedEventArgs e)
        {
            Autorun.Content = HText.Get_Translate("enopen");
            HUI.Set_Control_Location(Autodelete, "endeleter", bottom: true, animation: HSet.Read_DCIni("Ani", "2").Equals("1"), animationTime: 250);
            HUI.Set_Control_Location(Autorun, "enopen", bottom: true, animation: HSet.Read_DCIni("Ani", "2").Equals("1"), animationTime: 250);
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
