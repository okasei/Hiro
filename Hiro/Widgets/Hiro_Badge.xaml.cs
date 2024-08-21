using Hiro.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Windows.Devices.AllJoyn;

namespace Hiro
{
    /// <summary>
    /// Hiro_Badge.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Badge : Window
    {
        Thickness formerThickness = new Thickness(0);
        bool animating = false;
        bool rotating = false;
        public Hiro_Badge()
        {
            InitializeComponent();
            Helpers.HUI.SetCustomWindowIcon(this);
            Loaded += (e, args) =>
            {
                AdjustSize();
                FadeIn();
                LoadColor();
            };
        }

        internal void FadeIn()
        {
            var sb = new System.Windows.Media.Animation.Storyboard();
            var h = Height;
            if (EllipseBorder.Visibility == Visibility.Visible)
            {
                sb = HAnimation.AddDoubleAnimaton(h, Math.Min(h * 10, 700), Badge_Ellipse, "Height", sb, h * 1.2, 0.7);
                sb = HAnimation.AddDoubleAnimaton(h, Math.Min(h * 10, 700), Badge_Ellipse, "Width", sb, h * 1.2, 0.7);
                sb = HAnimation.AddDoubleAnimaton(1, Math.Min(h * 10, 700), Badge_Ellipse, "Opacity", sb, 0, 0.7);
            }
            else
            {
                sb = HAnimation.AddDoubleAnimaton(h, Math.Min(h * 10, 700), Badge_Rectangle, "Height", sb, h * 1.2, 0.7);
                sb = HAnimation.AddDoubleAnimaton(h, Math.Min(h * 10, 700), Badge_Rectangle, "Width", sb, h * 1.2, 0.7);
                sb = HAnimation.AddDoubleAnimaton(1, Math.Min(h * 10, 700), Badge_Rectangle, "Opacity", sb, 0, 0.7);
            }
            sb.Begin();
        }

        public Hiro_Badge(string pic)
        {
            InitializeComponent();
            Loaded += (e, args) =>
            {
                AdjustSize();
                FadeIn();
                LoadColor();
            };
            var icon = HText.Path_PPX(pic);
            if (System.IO.File.Exists(icon))
            {
                BitmapImage? bi = Hiro_Utils.GetBitmapImage(icon);
                (Resources["BadgeImage"] as ImageBrush).ImageSource = bi;
                Badge_Ellipse.Fill = (ImageBrush)Resources["BadgeImage"];
                Badge_Rectangle.Fill = (ImageBrush)Resources["BadgeImage"];
            }
        }

        internal void AdjustSize()
        {
            var h = Height;
            Badge_Ellipse.Width = h;
            Badge_Ellipse.Height = h;
            Badge_Rectangle.Width = h;
            Badge_Rectangle.Height = h;
            formerThickness = new(-1.3 * h, -1.3 * h, 0, 0);
            if (!animating)
            {
                EllipseLight.Margin = formerThickness;
                RectagleLight.Margin = formerThickness;
                EllipseLight.Width = h * 0.3;
                RectagleLight.Width = h * 0.3;
                EllipseLight.Height = h * 4;
                RectagleLight.Height = h * 4;
            }
        }

        internal void LoadColor()
        {
            Resources["AppAccent"] = new SolidColorBrush(App.AppAccentColor);
        }

        internal void CallLight(int obj = 0)
        {
            if (!animating)
            {
                animating = true;
                AdjustSize();
                var h = Height;
                var targetThickness = new Thickness(h * 2, h * 2, 0, 0);
                var sb = obj == 0 ? HAnimation.AddThicknessAnimaton(targetThickness, Math.Min(h * 50, 3000), EllipseLight, "Margin", null) : HAnimation.AddThicknessAnimaton(targetThickness, Math.Min(h * 50, 3000), RectagleLight, "Margin", null);
                sb.Completed += (e, args) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        animating = false;
                        AdjustSize();
                    });
                };
                sb.Begin();
            }
        }

        private void RectangleBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            CallLight(1);
        }

        private void EllipseBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            CallLight(0);
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (rotating)
            {
                return;
            }
            rotating = true;
            e.Cancel = true;
            var sb = new System.Windows.Media.Animation.Storyboard();
            var h = Height;
            if (EllipseBorder.Visibility == Visibility.Visible)
            {
                sb = HAnimation.AddDoubleAnimaton(h * 1.2, Math.Min(h * 10, 700), Badge_Ellipse, "Height", sb, h, 0.7);
                sb = HAnimation.AddDoubleAnimaton(h * 1.2, Math.Min(h * 10, 700), Badge_Ellipse, "Width", sb, h, 0.7);
                sb = HAnimation.AddDoubleAnimaton(0, Math.Min(h * 10, 700), Badge_Ellipse, "Opacity", sb, 1, 0.7);
            }
            else
            {
                sb = HAnimation.AddDoubleAnimaton(h * 1.2, Math.Min(h * 10, 700), Badge_Rectangle, "Height", sb, h, 0.7);
                sb = HAnimation.AddDoubleAnimaton(h * 1.2, Math.Min(h * 10, 700), Badge_Rectangle, "Width", sb, h, 0.7);
                sb = HAnimation.AddDoubleAnimaton(0, Math.Min(h * 10, 700), Badge_Rectangle, "Opacity", sb, 1, 0.7);
            }
            sb.Completed += (e, args) =>
            {
                Close();
            };
            sb.Begin();
        }

        private void EllipseBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }

        private void RectangleBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }
    }
}
