using System.Windows;

namespace hiro
{
    /// <summary>
    /// background.xaml の相互作用ロジック
    /// </summary>
    public partial class Background : Window
    {
        public Background()
        {
            InitializeComponent();
            Title = App.AppTitle;
        }

        private void Back_Loaded(object sender, RoutedEventArgs e)
        {
            bool animation;
            if (utils.Read_Ini(App.dconfig, "Configuration", "ani", "1").Equals("0"))
                animation = false;
            else
                animation = true;
            if (animation)
            {
                var opacity = 0.01;
                while (opacity < 0.78)
                {
                    opacity += 0.02;
                    Opacity = opacity;
                    utils.Delay(1);
                }
            }
                Opacity = 0.8;
        }
    }
}
