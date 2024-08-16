using Hiro;
using Hiro.Helpers;
using Hiro.ModelViews;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static Hiro.Helpers.Hiro_Settings;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Hiro
{
    /// <summary>
    /// Hiro_ImageViewer.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_ImageViewer : Window
    {
        internal WindowAccentCompositor? compositor = null;
        internal int bflag = 0;
        internal System.Collections.Generic.List<string>? files = null;
        internal int fileIndex = -1;
        internal bool dealed = true;
        internal bool loaded = false;

        public Hiro_ImageViewer(string? filePath)
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            Hiro_UI.SetCustomWindowIcon(this);
            Load_Color();
            Loaded += delegate
            {
                SetImage(filePath);
                Refreash_Layout();
                LoadPicIndex(filePath);
                Load_Translate();
                Loadbgi(Hiro_Utils.ConvertInt(Read_DCIni("Blur", "0")), false);
                loaded = true;
            };
        }

        public void Load_Translate()
        {
            minbtn.ToolTip = Hiro_Text.Get_Translate("Min");
            closebtn.ToolTip = Hiro_Text.Get_Translate("close");
            maxbtn.ToolTip = Hiro_Text.Get_Translate("max");
            resbtn.ToolTip = Hiro_Text.Get_Translate("restore");
        }

        private void LoadPicIndex(string? filePath)
        {
            if (filePath == null)
            {
                files?.Clear();
                return;
            }
            try
            {
                var list = new FileInfo(filePath).Directory.GetFiles("*", SearchOption.TopDirectoryOnly);
                var exts = "*.jpg;*.jpeg;*.jpe;*.jfif;*.bmp;*.dib;*.gif;*.png;*.apng;*.tiff;*.heic;*.heif;*.hvp;*.hpp;*.hap;*.hfp;";
                files?.Clear();
                files = list.Where(x => exts.Contains($"*{x.Extension};", StringComparison.CurrentCultureIgnoreCase)).Select(x => x.FullName).ToList();
                for (int i = 0; i < files.Count; i++)
                {
                    if (files[i].Equals(filePath))
                    {
                        fileIndex = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Hiro_Logger.LogError(ex, "Hiro.Exception.PicViewer.LoadList");
                files?.Clear();
                files ??= new();
                files.Add(filePath);
                fileIndex = 0;
            }
        }

        private void SetImage(string? filePath, bool fromLeft = true)
        {
            dealed = false;
            if (filePath == null)
            {
                dealed = true;
                Title = App.appTitle;
                return;
            }
            bool animation = !Hiro_Settings.Read_DCIni("Ani", "2").Equals("0");
            var t1 = Math.Max(140 + ActualWidth / SystemParameters.PrimaryScreenWidth * 560, 700);
            var t2 = Math.Max(200 + ActualWidth / SystemParameters.PrimaryScreenWidth * 600, 800);
            if (File.Exists(filePath))
            {
                Title = Hiro_Text.Get_Translate("imageTitle").Replace("%t", new FileInfo(filePath).Name).Replace("%a", App.appTitle);
                if (imageContiner.Visibility == Visibility.Visible)
                {
                    if (animation)
                    {
                        imageContiner2.Visibility = Visibility.Visible;
                        imageContiner2.Margin = new Thickness(-ActualWidth, 0, 0, 0);
                        imageContiner2.Source = Hiro_Utils.GetBitmapImage(filePath);
                        var sb = Hiro_Utils.AddDoubleAnimaton(1, t1, imageContiner2, "Opacity", null);
                        sb = Hiro_Utils.AddDoubleAnimaton(0, t1, imageContiner, "Opacity", sb);
                        sb = Hiro_Utils.AddThicknessAnimaton(new Thickness(0), t2, imageContiner2, "Margin", sb, fromLeft ? null : new Thickness(ActualWidth, 0, 0, 0));
                        sb = Hiro_Utils.AddThicknessAnimaton(fromLeft ? new Thickness(ActualWidth, 0, 0, 0) : new Thickness(-ActualWidth, 0, 0, 0), t2, imageContiner, "Margin", sb);
                        sb.Completed += delegate
                        {
                            imageContiner.Source = null;
                            imageContiner.Opacity = 0;
                            imageContiner2.Margin = new Thickness(0);
                            imageContiner2.Opacity = 1;
                            imageContiner.Visibility = Visibility.Collapsed;
                            dealed = true;
                            sb = null;
                        };
                        sb.Begin();
                    }
                    else
                    {
                        imageContiner.Visibility = Visibility.Visible;
                        imageContiner.Source = Hiro_Utils.GetBitmapImage(filePath);
                        dealed = true;
                    }
                }
                else
                {
                    if (animation)
                    {
                        imageContiner.Visibility = Visibility.Visible;
                        imageContiner.Margin = new Thickness(-ActualWidth, 0, 0, 0);
                        imageContiner.Source = Hiro_Utils.GetBitmapImage(filePath);
                        var sb = Hiro_Utils.AddDoubleAnimaton(1, t1, imageContiner, "Opacity", null);
                        sb = Hiro_Utils.AddDoubleAnimaton(0, t1, imageContiner2, "Opacity", sb);
                        sb = Hiro_Utils.AddThicknessAnimaton(new Thickness(0), t2, imageContiner, "Margin", sb, fromLeft ? null : new Thickness(ActualWidth, 0, 0, 0));
                        sb = Hiro_Utils.AddThicknessAnimaton(fromLeft ? new Thickness(ActualWidth, 0, 0, 0) : new Thickness(-ActualWidth, 0, 0, 0), t2, imageContiner2, "Margin", sb);
                        sb.Completed += delegate
                        {
                            imageContiner2.Source = null;
                            imageContiner2.Opacity = 0;
                            imageContiner.Margin = new Thickness(0);
                            imageContiner.Opacity = 1;
                            imageContiner2.Visibility = Visibility.Collapsed;
                            dealed = true;
                            sb = null;
                        };
                        sb.Begin();
                    }
                    else
                    {
                        imageContiner2.Visibility = Visibility.Visible;
                        imageContiner2.Source = Hiro_Utils.GetBitmapImage(filePath);
                        dealed = true;
                    }
                }
            }
        }

        public void Refreash_Layout()
        {
            TitleGrid.Height = WindowState == WindowState.Maximized ? 26 : 32;
            maxbtn.Visibility = ResizeMode == ResizeMode.NoResize || ResizeMode == ResizeMode.CanMinimize ? Visibility.Collapsed : WindowState == WindowState.Maximized ? Visibility.Collapsed : Visibility.Visible;
            resbtn.Visibility = ResizeMode == ResizeMode.NoResize || ResizeMode == ResizeMode.CanMinimize ? Visibility.Collapsed : WindowState == WindowState.Maximized ? Visibility.Visible : Visibility.Collapsed;
            closebtn.Margin = WindowState == WindowState.Maximized ? new(0, -5, 0, 0) : new(0, -2, 0, 0);
            closebtn.Height = WindowState == WindowState.Maximized ? 30 : 32;
            imageContiner.Width = ActualWidth;
            imageContiner.Height = ActualHeight;
            imageContiner2.Width = ActualWidth;
            imageContiner2.Height = ActualHeight;
            if (loaded)
                Loadbgi(Hiro_Utils.ConvertInt(Read_DCIni("Blur", "0")), false);
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppAccent"] = new SolidColorBrush(App.AppAccentColor);
            Resources["AppForeDimColor"] = Hiro_Utils.Color_Transparent(App.AppForeColor, 80);
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;
            var source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source?.AddHook(WndProc);
            WindowStyle = WindowStyle.SingleBorderWindow;
        }

        public void Loadbgi(int direction, bool animation)
        {
            if (Read_DCIni("Background", "1").Equals("3"))
            {
                compositor ??= new(this);
                Hiro_Utils.Set_Acrylic(bgimage, this, windowChrome, compositor);
                return;
            }
            if (compositor != null)
            {
                compositor.IsEnabled = false;
            }
            if (bflag == 1)
                return;
            bflag = 1;
            Hiro_Utils.Set_Bgimage(bgimage, this);
            Hiro_Utils.Blur_Animation(direction, animation, bgimage, this);
            bflag = 0;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0083://prevent system from drawing outline
                    //handled = true;
                    break;
                default:
                    //Console.WriteLine("Msg: " + m.Msg + ";LParam: " + m.LParam + ";WParam: " + m.WParam + ";Result: " + m.Result);
                    break;
            }
            return IntPtr.Zero;

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

        private void VirtualTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }
        private void Resbtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WindowState = WindowState.Normal;
            e.Handled = true;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Refreash_Layout();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (dealed && e.Key == Key.Right)
            {
                if (files != null && fileIndex < files.Count - 1)
                {
                    fileIndex++;
                    SetImage(files[fileIndex]);
                }
                else
                {
                    Bounce(true);
                }
                e.Handled = true;
            }
            if (dealed && e.Key == Key.Left)
            {
                if (files != null && fileIndex > 0)
                {
                    fileIndex--;
                    SetImage(files[fileIndex], false);
                }
                else
                {
                    Bounce(false);
                }
                e.Handled = true;
            }
        }

        private void Bounce(bool fromLeft)
        {
            dealed = false;
            var sb = Hiro_Utils.AddThicknessAnimaton(fromLeft ? new Thickness(250, 0, 0, 0) : new Thickness(-250, 0, 0, 0), 350, imageContiner.Visibility == Visibility.Visible ? imageContiner : imageContiner2, "Margin", null);
            sb.Completed += delegate
            {
                var s = Hiro_Utils.AddThicknessAnimaton(new Thickness(0), 350, imageContiner.Visibility == Visibility.Visible ? imageContiner : imageContiner2, "Margin", null, fromLeft ? new Thickness(250, 0, 0, 0) : new Thickness(-250, 0, 0, 0));
                s.Completed += delegate
                {
                    dealed = true;
                };
                s.Begin();
            };
            sb.Begin();
        }

        private void VirtualTitle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                if(filePaths.Length > 0)
                {
                    SetImage(filePaths[0]); 
                    LoadPicIndex(filePaths[0]);
                    e.Handled = true;
                }
            }
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                var f = (string)e.Data.GetData(DataFormats.Text);
                SetImage(f);
                LoadPicIndex(f);
                e.Handled = true;
            }
        }
    }
}
