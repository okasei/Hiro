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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hiro
{
    /// <summary>
    /// Hiro_Blur.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Blur : Window
    {
        public Hiro_Blur()
        {
            InitializeComponent();
            var compositor = new ModelViews.WindowAccentCompositor(this);
            compositor.Color = Color.FromArgb(0x34, App.AppAccentColor.R, App.AppAccentColor.G, App.AppAccentColor.B);
            compositor.IsEnabled = true;
           windowChrome.GlassFrameThickness = new(0);
        }
    }
}
