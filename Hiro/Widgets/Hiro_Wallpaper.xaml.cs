using Hiro.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hiro.Widgets
{
    /// <summary>
    /// Hiro_Wallpaper.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Wallpaper : Window
    {
        public Hiro_Wallpaper()
        {
            InitializeComponent();
            Loaded += delegate
            {
                HSystem.HideInAltTab(new WindowInteropHelper(this).Handle);
                HDesktop.SetWallpaperWin(new WindowInteropHelper(this).Handle);
                WindowState = WindowState.Maximized;
            };
        }

        internal void ResetUniform(int i)
        {
            Wallpaper.Stretch = (i) switch
            {
                1 => Stretch.Fill,
                2 => Stretch.Uniform,
                3 => Stretch.UniformToFill,
                _ => Stretch.None
            };
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState != WindowState.Maximized)
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState != WindowState.Maximized)
            {
                WindowState = WindowState.Maximized;
            }
        }
    }
}
