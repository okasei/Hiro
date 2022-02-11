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
            if (!utils.Read_Ini(App.dconfig, "Configuration", "ani", "1").Equals("0"))
            {
                System.Windows.Media.Animation.Storyboard? sb = new();
                sb = utils.AddDoubleAnimaton(0.8, 1000, this, "Opacity", sb);
                sb.Completed += delegate
                {
                    Opacity = 0.8;
                };
                sb.Begin();
            }
                Opacity = 0.8;
        }
    }
}
