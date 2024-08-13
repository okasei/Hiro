using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace hiro
{
    /// <summary>
    /// Hiro_Snow.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Snow : Window
    {
        internal int maxSnow = 50;
        internal int type = 0;//0 - Snow, 1 - Triangle, 2 - Circle
        private int direction = 0;//0 - l2r, 1 - r2l, 2 - b2t, 3 - t2b
        private int nowSnow = 0;
        internal DispatcherTimer? timer = null;
        public Hiro_Snow()
        {
            InitializeComponent();
            Helpers.Hiro_UI.SetCustomWindowIcon(this);
            timer = new DispatcherTimer();
            timer.Tick += (e, args) =>
            {
                if (nowSnow < maxSnow)
                {
                    switch (type)
                    {

                        case 1:
                            {
                                AddRandomTriangle();
                                break;
                            }
                        case 2:
                            {
                                AddRandomTriangle();
                                break;
                            }
                        default:
                            {
                                AddRandomTriangle();
                                break;
                            }
                    }
                };
            };
            timer.Interval = TimeSpan.FromMilliseconds(10);
        }

        public void AddRandomTriangle()
        {
            nowSnow++;
            var tri = new Polygon();
            var len = new Random().Next(13, 25);
            var hei = new Random().Next(((int)ActualHeight));
            if (hei > ActualHeight - len)
            {
                hei = (int)(ActualHeight - len);
            }
            tri.Points.Add(new Point(0, hei + len * Math.Sqrt(3) / 2));
            tri.Points.Add(new Point(len / 2, hei));
            tri.Points.Add(new Point(len, hei + len * Math.Sqrt(3) / 2));
            tri.Fill = System.Windows.Media.Brushes.White;
            tri.Stroke = Brushes.Transparent;
            BasicCanvas.Children.Add(tri);
            tri.HorizontalAlignment = HorizontalAlignment.Left;
            tri.VerticalAlignment = VerticalAlignment.Top;
            var sb = Hiro_Utils.AddThicknessAnimaton(new Thickness(ActualWidth, hei, 0, 0), 2000, tri, "Margin", null, new Thickness(0, hei, 0, 0), 0);
            sb.Completed += (e, args) =>
            {
                Dispatcher.Invoke(new(() =>
                {
                    BasicCanvas.Children.Remove(tri);
                    tri = null;
                    nowSnow--;
                }));
            };
            sb.Begin();
        }

        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            AddRandomTriangle();
        }
    }
}
