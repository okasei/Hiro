using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Hiro.Helpers;
using Hiro.ModelViews;

namespace Hiro
{
    /// <summary>
    /// Hiro_Finder.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Finder : Window
    {
        private int bflag = 0;
        private int cflag = 0;
        private bool load = false;
        internal WindowAccentCompositor? compositor = null;
        public Hiro_Finder()
        {
            InitializeComponent();
            Width = SystemParameters.PrimaryScreenWidth / 5 * 4;
            Height = SystemParameters.PrimaryScreenHeight / 10;
            Title = App.appTitle;
            Hiro_Initialize();
            Helpers.HUI.SetCustomWindowIcon(this);
            SourceInitialized += OnSourceInitialized;
            ContentRendered += delegate
             {
                 Size msize = new();
                 HUI.Get_Text_Visual_Width(PlaceHolder, VisualTreeHelper.GetDpi(this).PixelsPerDip, out msize);
                 while (msize.Height <= PlaceHolder.ActualHeight)
                 {
                     HUI.Get_Text_Visual_Width(PlaceHolder, VisualTreeHelper.GetDpi(this).PixelsPerDip, out msize);
                     PlaceHolder.FontSize++;
                     if (PlaceHolder.FontSize > 36)
                         break;
                 }
                 PlaceHolder.FontSize--;
                 HiHiro();
             };
            Loaded += delegate
            {
                Hiro_Utils.SetCaptureImpl(new System.Windows.Interop.WindowInteropHelper(this).Handle);
                Hiro_Utils.SetWindowToForegroundWithAttachThreadInput(this);
                Keyboard.Focus(Hiro_Text);
            };
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var source = System.Windows.Interop.HwndSource.FromHwnd(new System.Windows.Interop.WindowInteropHelper(this).Handle);
            source?.AddHook(WndProc);
            WindowStyle = WindowStyle.SingleBorderWindow;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0083:
                    handled = true;
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        private void Hiro_Initialize()
        {
            Load_Color();
            Load_Translate();
            Load_Position();
            Loadbgi(Hiro_Utils.ConvertInt(HSet.Read_DCIni("Blur", "0")));
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 80));
        }

        public void Load_Translate()
        {
            PlaceHolder.Content = Helpers.HText.Path_PPX(Helpers.HText.Get_Translate("hirogo"));
        }

        public void Load_Position()
        {
            HUI.Set_Control_Location(PlaceHolder, "hirogo", location: false);
            HUI.Set_Control_Location(Hiro_Text, "hirogotb", location: false);
        }

        public void HiHiro()
        {
            if (Hiro_Text.Text.Equals("") || Hiro_Text.Text.Equals(string.Empty))
                PlaceHolder.Visibility = Visibility.Visible;
            if (!HSet.Read_DCIni("Ani", "2").Equals("1"))
                return;
            Storyboard sb = new();
            HAnimation.AddPowerAnimation(0, PlaceHolder, sb, 50, null);
            sb.Completed += delegate
            {
                if (Hiro_Text.Text.Equals("") || Hiro_Text.Text.Equals(string.Empty))
                    PlaceHolder.Visibility = Visibility.Visible;
                else
                    PlaceHolder.Visibility = Visibility.Hidden;
            };
            sb.Begin();
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

        private void Hiro_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Hiro_Text.Text.Equals("") || Hiro_Text.Text.Equals(string.Empty))
            {
                PlaceHolder.Visibility = Visibility.Visible;
                if (!HSet.Read_DCIni("Ani", "2").Equals("1"))
                    return;
                Storyboard sb = new();
                HAnimation.AddPowerAnimation(0, PlaceHolder, sb, 50, null);
                sb.Completed += delegate
                {
                    if (Hiro_Text.Text.Equals("") || Hiro_Text.Text.Equals(string.Empty))
                        PlaceHolder.Visibility = Visibility.Visible;
                    else
                        PlaceHolder.Visibility = Visibility.Hidden;
                };
                sb.Begin();
            }
            else
            {

                if (!HSet.Read_DCIni("Ani", "2").Equals("1"))
                {
                    PlaceHolder.Visibility = Visibility.Hidden;
                }
                else
                {
                    Storyboard sb = new();
                    HAnimation.AddDoubleAnimaton(0, 250, PlaceHolder, "Opacity", sb);
                    sb.Completed += delegate
                    {
                        if (Hiro_Text.Text.Equals("") || Hiro_Text.Text.Equals(string.Empty))
                            PlaceHolder.Visibility = Visibility.Visible;
                        else
                            PlaceHolder.Visibility = Visibility.Hidden;
                    };
                    sb.Begin();

                }
            }
        }

        private void Hiro_Text_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                TryClose();
                e.Handled = true;
            }
            if (e.Key == Key.Enter)
            {
                TryClose(Hiro_Text.Text);
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.D9) && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                Pair_Brackets("()");
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.OemOpenBrackets) && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                Pair_Brackets("{}");
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.OemOpenBrackets) && Keyboard.Modifiers == ModifierKeys.None)
            {
                Pair_Brackets("[]");
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.OemComma) && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                Pair_Brackets("<>");
                e.Handled = true;
            }
        }

        private void Pair_Brackets(string brackets)
        {
            var index = Hiro_Text.CaretIndex;
            var text = Hiro_Text.Text;
            text = string.Concat(text.AsSpan(0, index), brackets, text.AsSpan(index, text.Length - index));
            Hiro_Text.Text = text;
            Hiro_Text.CaretIndex = index + 1;
        }

        private void TryClose(string? uri = null)
        {
            if (!load || cflag != 0)
                return;
            cflag = 1;
            Hiro_Utils.CancelWindowToForegroundWithAttachThreadInput(this);
            Hiro_Utils.ReleaseCaptureImpl();
            if (uri != null)
            {
                Hiro_Utils.RunExe(uri);
            }
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (cflag == 0)
            {
                Hiro_Utils.CancelWindowToForegroundWithAttachThreadInput(this);
                Hiro_Utils.ReleaseCaptureImpl();
            }
        }

        private void Window_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            load = true;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            TryClose();
        }
    }
}
