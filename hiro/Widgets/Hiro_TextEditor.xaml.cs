using Hiro.Helpers;
using Hiro.ModelViews;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Hiro.Helpers.Hiro_Settings;

namespace Hiro
{
    /// <summary>
    /// Hiro_Text.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_TextEditor : Window
    {
        internal WindowAccentCompositor? compositor = null;
        internal int bflag = 0;
        internal System.Collections.Generic.List<string>? files = null;
        internal int fileIndex = -1;
        internal bool dealed = true;
        internal bool loaded = false;
        public Hiro_TextEditor(string? filePath)
        {
            InitializeComponent(); SourceInitialized += OnSourceInitialized;
            Hiro_UI.SetCustomWindowIcon(this);
            Load_Color();
            Loaded += delegate
            {
                Refreash_Layout();
                Load_Translate();
                Loadbgi(Hiro_Utils.ConvertInt(Read_DCIni("Blur", "0")), false);
                loaded = true;
                LoadText(filePath);
            };
        }

        private void LoadText(string? path)
        {
            if (path == null || !File.Exists(path))
                return;
            MainText.Text = File.ReadAllText(path);
            Title = Hiro_Text.Get_Translate("textEditorTitle").Replace("%t", new FileInfo(path).Name).Replace("%a", App.appTitle);
            TitleLabel.Text = Title;
        }

        public void Load_Translate()
        {
            minbtn.ToolTip = Hiro_Text.Get_Translate("Min");
            closebtn.ToolTip = Hiro_Text.Get_Translate("close");
            maxbtn.ToolTip = Hiro_Text.Get_Translate("max");
            resbtn.ToolTip = Hiro_Text.Get_Translate("restore");
            Hiro_Utils.Set_Control_Location(MainText, "textWidget", location: false);
            Hiro_Utils.Set_Control_Location(FakeTitle, "textWidgetTitle", location: false);
            Hiro_UI.CopyFontFromLabel(FakeTitle, TitleLabel);
        }
        public void Refreash_Layout()
        {
            TitleGrid.Height = WindowState == WindowState.Maximized ? 26 : 32;
            maxbtn.Visibility = ResizeMode == ResizeMode.NoResize || ResizeMode == ResizeMode.CanMinimize ? Visibility.Collapsed : WindowState == WindowState.Maximized ? Visibility.Collapsed : Visibility.Visible;
            resbtn.Visibility = ResizeMode == ResizeMode.NoResize || ResizeMode == ResizeMode.CanMinimize ? Visibility.Collapsed : WindowState == WindowState.Maximized ? Visibility.Visible : Visibility.Collapsed;
            closebtn.Margin = WindowState == WindowState.Maximized ? new(0, -5, 0, 0) : new(0, -2, 0, 0);
            closebtn.Height = WindowState == WindowState.Maximized ? 30 : 32;
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

        private void VirtualTitle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (filePaths.Length > 0)
                {
                    LoadText(filePaths[0]);
                    e.Handled = true;
                }
            }
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                var f = (string)e.Data.GetData(DataFormats.Text);
                LoadText(f);
                e.Handled = true;
            }
        }
    }
}