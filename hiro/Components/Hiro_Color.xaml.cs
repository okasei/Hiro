using Hiro.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Hiro
{
    /// <summary>
    /// Hiro_Color.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Color : Page
    {
        private Hiro_MainUI? Hiro_Main = null;
        private double dpiScaleX = 1.0;
        private double dpiScaleY = 1.0;
        private Color currentColor = App.AppAccentColor;
        private Color _color = App.AppAccentColor;
        public Hiro_Color(Hiro_MainUI? Parent)
        {
            InitializeComponent();
            Hiro_Main = Parent;
            Hiro_Initialize();
            Loaded += delegate
            {
                HiHiro();
                GetDpiScale();
                SetIndicatorPosition(currentColor);
                color_text.Text = $"#{_color.R:X2}{_color.G:X2}{_color.B:X2}";
            };
        }

        public void HiHiro()
        {
            bool animation = !HSet.Read_DCIni("Ani", "2").Equals("0");
            Storyboard sb = new();
            if (HSet.Read_DCIni("Ani", "2").Equals("1"))
            {
                HAnimation.AddPowerAnimation(1, color_title, sb, -50, null);
                HAnimation.AddPowerAnimation(1, color_text, sb, -50, null);
                HAnimation.AddPowerAnimation(3, cobtn1, sb, -50, null);
                HAnimation.AddPowerAnimation(3, cobtn2, sb, -50, null);
                HAnimation.AddPowerAnimation(3, cobtn3, sb, -50, null);
            }
            if (!animation)
                return;
            HAnimation.AddPowerAnimation(0, this, sb, 50, null);
            sb.Begin();
        }

        private void Hiro_Initialize()
        {
            //color_picker.MouseMove += Color_picker_ColorChanged;
            Load_Color();
            Load_Translate();
            Load_Position();
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        public void Load_Translate()
        {
            color_title.Content = HText.Get_Translate("cotitle");
            color_ex.Content = HText.Get_Translate("coex").Replace("\\n", Environment.NewLine);
            cobtn1.Content = HText.Get_Translate("cook");
            cobtn2.Content = HText.Get_Translate("cocancel");
            cobtn3.Content = HText.Get_Translate("coreset");
        }

        public void Load_Position()
        {
            HUI.Set_FrameworkElement_Location(ColorGrid, "copickerg");
            HUI.Set_FrameworkElement_Location(ColorBorder, "copickerb");
            HUI.Set_FrameworkElement_Location(ColorCanvas, "copickerm");
            HUI.Set_FrameworkElement_Location(HSVCanvas, "copickerx");
            HUI.Set_Control_Location(color_title, "cotitle");
            //HUI.Set_Control_Location(color_picker, "copicker");
            HUI.Set_Control_Location(color_text, "coval");
            HUI.Set_Control_Location(color_ex, "coex");
            HUI.Set_Control_Location(cobtn1, "cook", bottom: true, right: true);
            HUI.Set_Control_Location(cobtn2, "cocancel", bottom: true, right: true);
            HUI.Set_Control_Location(cobtn3, "coreset", bottom: true);
        }


        private void Cobtn3_Click(object sender, RoutedEventArgs e)
        {
            HSet.Write_Ini(App.dConfig, "Config", "Lockcolor", "default");
            if (App.wnd != null)
                App.wnd.Load_All_Colors();
            if (Hiro_Main != null)
            {
                Hiro_Main.Set_Label(Hiro_Main.configx);
                if (HSet.Read_DCIni("Background", "1").Equals("3"))
                    Hiro_Main?.Blurbgi(0);
            }

        }

        private void Cobtn1_Click(object sender, RoutedEventArgs e)
        {
            App.AppAccentColor = _color;
            HSet.Write_Ini(App.dConfig, "Config", "Lockcolor", string.Format("#{0:X2}{1:X2}{2:X2}", App.AppAccentColor.R, App.AppAccentColor.G, App.AppAccentColor.B));
            if (App.wnd != null)
                App.wnd.Load_All_Colors();
            if (Hiro_Main != null)
            {
                Hiro_Main.Set_Label(Hiro_Main.configx);
            }
        }

        private void Cobtn2_Click(object sender, RoutedEventArgs e)
        {
            Hiro_Main?.Set_Label(Hiro_Main.configx);
        }

        internal void Unify_Color(bool force = false)
        {
            var _ac = HSVCanvas.ActualWidth;
            (var h, var s, var v) = RgbToHsv(currentColor);
            _color = HsvToRgb(h, s, (ColorIndexer.Margin.Left + ColorIndexer.Width / 2) / _ac);
            if (_color == App.AppAccentColor && !force)
                return;
            color_text.Text = $"#{_color.R:X2}{_color.G:X2}{_color.B:X2}";
            color_ex.Background = new SolidColorBrush(_color);
            color_ex.Foreground = new SolidColorBrush(Hiro_Utils.Get_ForeColor(_color, HSet.Read_DCIni("Reverse", "0").Equals("1")));
        }
        private void Color_text_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            if (!color_text.Text.StartsWith("#"))
                color_text.Text = "#" + color_text.Text;
            if (color_text.Text.Length == 7)
            {
                try
                {
                    currentColor = (Color)ColorConverter.ConvertFromString(color_text.Text);
                    SetIndicatorPosition(currentColor);
                }
                catch (Exception ex)
                {
                    HLogger.LogError(ex, "Hiro.Exception.Color.Pick");
                }
            }
            Unify_Color();
            e.Handled = true;
        }
        private void GetDpiScale()
        {
            var source = PresentationSource.FromVisual(this);
            if (source != null)
            {
                dpiScaleX = source.CompositionTarget.TransformToDevice.M11;
                dpiScaleY = source.CompositionTarget.TransformToDevice.M22;
            }
            else
            {
                dpiScaleX = 1.0;
                dpiScaleY = 1.0;
            }
        }

        public RenderTargetBitmap GetRenderTargetBitmap(UIElement element)
        {
            var rect = new Rect(element.RenderSize);
            var visual = new DrawingVisual();

            using (var dc = visual.RenderOpen())
            {
                dc.DrawRectangle(new VisualBrush(element), null, rect);
            }

            var bitmap = new RenderTargetBitmap(
                (int)rect.Width, (int)rect.Height, 96, 96, PixelFormats.Default);
            bitmap.Render(visual);
            return bitmap;
        }

        private void Canvas_MouseAction(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var _cc = ColorCanvas.ActualWidth;
                var _ch = ColorCanvas.ActualHeight;
                var _ci = ColorIndicator.ActualWidth / 2;
                // 获取鼠标在 Canvas 上的坐标
                Point mousePosition = e.GetPosition(ColorCanvas);
                mousePosition.X = Math.Clamp(mousePosition.X, 0, _cc);
                mousePosition.Y = Math.Clamp(mousePosition.Y, 0, _ch);
                ColorIndicator.Margin = new(mousePosition.X - _ci, mousePosition.Y - _ci, 0, 0);
                double hue = (mousePosition.X / _cc) * 360; // 色相范围是 0-360
                double saturation = 1 - (mousePosition.Y / _ch);
                // 解析颜色
                Color color = HsvToRgb(hue, saturation, 1);
                GridentColor.Color = color;
                currentColor = color;
                Unify_Color();
                e.Handled = true;
            }

        }

        private void SetIndicatorPosition(Color color)
        {
            // 将 RGB 颜色转换为 HSV
            var (h, s, v) = RgbToHsv(color);
            GridentColor.Color = HsvToRgb(h, s, 1);
            // 计算指示器的位置
            double x = (h / 360.0) * ColorCanvas.ActualWidth;
            double y = (1 - s) * ColorCanvas.ActualHeight;
            ColorIndicator.Margin = new Thickness(x - ColorIndicator.ActualWidth / 2, y - ColorIndicator.ActualHeight / 2, 0, 0);
            ColorIndexer.Margin = new Thickness(v * HSVCanvas.ActualWidth - ColorIndexer.Width / 2, 0, 0, 0);
            Unify_Color(true);
        }

        private void ColorCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ColorCanvas.CaptureMouse();

            var _cc = ColorCanvas.ActualWidth;
            var _ch = ColorCanvas.ActualHeight;
            var _ci = ColorIndicator.ActualWidth / 2;
            // 获取鼠标在 Canvas 上的坐标
            Point mousePosition = e.GetPosition(ColorCanvas);
            mousePosition.X = Math.Clamp(mousePosition.X, 0, _cc);
            mousePosition.Y = Math.Clamp(mousePosition.Y, 0, _ch);
            ColorIndicator.Margin = new(mousePosition.X - _ci, mousePosition.Y - _ci, 0, 0);
            double hue = (mousePosition.X / _cc) * 360; // 色相范围是 0-360
            double saturation = 1 - (mousePosition.Y / _ch);
            // 解析颜色
            Color color = HsvToRgb(hue, saturation, 1);
            GridentColor.Color = color;
            currentColor = color;
            Unify_Color();
            e.Handled = true;
        }

        private void ColorCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ColorCanvas.ReleaseMouseCapture();
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HSVCanvas.CaptureMouse();
            var _ac = HSVCanvas.ActualWidth;
            var _ci = ColorIndexer.ActualWidth / 2;
            // 获取鼠标在 Canvas 上的坐标
            Point mousePosition = e.GetPosition(HSVCanvas);
            var process = Math.Clamp(mousePosition.X, 0, _ac);
            ColorIndexer.Margin = new(process - _ci, 0, 0, 0);
            Unify_Color();
            e.Handled = true;
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            HSVCanvas.ReleaseMouseCapture();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var _ac = HSVCanvas.ActualWidth;
                var _ci = ColorIndexer.ActualWidth / 2;
                // 获取鼠标在 Canvas 上的坐标
                Point mousePosition = e.GetPosition(HSVCanvas);
                var process = Math.Clamp(mousePosition.X, 0, _ac);
                ColorIndexer.Margin = new(process - _ci, 0, 0, 0);
                Unify_Color();
                e.Handled = true;
            }
        }

        public static (double H, double S, double V) RgbToHsv(Color color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            double h = 0;
            double s = (max == 0) ? 0 : delta / max;
            double v = max;

            if (delta != 0)
            {
                if (max == r)
                {
                    h = 60 * (((g - b) / delta) % 6);
                }
                else if (max == g)
                {
                    h = 60 * (((b - r) / delta) + 2);
                }
                else if (max == b)
                {
                    h = 60 * (((r - g) / delta) + 4);
                }

                if (h < 0)
                {
                    h += 360;
                }
            }

            return (h, s, v);
        }

        public static Color HsvToRgb(double h, double s, double v)
        {
            double c = v * s; // Chroma
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = v - c;

            double r = 0, g = 0, b = 0;

            if (h >= 0 && h < 60)
            {
                r = c; g = x; b = 0;
            }
            else if (h >= 60 && h < 120)
            {
                r = x; g = c; b = 0;
            }
            else if (h >= 120 && h < 180)
            {
                r = 0; g = c; b = x;
            }
            else if (h >= 180 && h < 240)
            {
                r = 0; g = x; b = c;
            }
            else if (h >= 240 && h < 300)
            {
                r = x; g = 0; b = c;
            }
            else if (h >= 300 && h < 360)
            {
                r = c; g = 0; b = x;
            }

            // Adjust the colors to fit the RGB model
            r = (r + m) * 255;
            g = (g + m) * 255;
            b = (b + m) * 255;

            return Color.FromRgb((byte)r, (byte)g, (byte)b);
        }
    }
}
