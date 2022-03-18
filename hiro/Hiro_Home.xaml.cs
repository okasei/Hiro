using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
{
    /// <summary>
    /// Hiro_Home.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Home : Page
    {
        public Hiro_Home()
        {
            InitializeComponent();
            Hiro_Initialize();
            Loaded += Hiro_Home_Loaded;
        }

        private void Hiro_Initialize()
        {
            Load_Color();
        }

        private void Hiro_Home_Loaded(object sender, RoutedEventArgs e)
        {
            bool animation = !utils.Read_Ini(App.dconfig, "config", "ani", "1").Equals("0");
            if (animation)
                BeginStoryboard(Application.Current.Resources["AppLoad"] as Storyboard);
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
        }
    }
}
