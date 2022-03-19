using System.Windows;

namespace hiro
{
    /// <summary>
    /// background.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Background : Window
    {
        public Hiro_Background()
        {
            InitializeComponent();
            Title = App.AppTitle;
        }

        private void Back_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
            {
                System.Windows.Media.Animation.Storyboard? sb = new();
                sb = Hiro_Utils.AddDoubleAnimaton(0.7, 300, this, "Opacity", sb, 0);
                sb.Completed += delegate
                {
                    Opacity = 0.7;
                };
                sb.Begin();
            }
            else
                Opacity = 0.7;
        }

        internal void Fade_Out()
        {
            if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
            {
                System.Windows.Media.Animation.Storyboard? sb = new();
                sb = Hiro_Utils.AddDoubleAnimaton(0, 300, this, "Opacity", sb);
                sb.Completed += delegate
                {
                    Close();
                };
                sb.Begin();
            }
            else
            {
                Close();
            }
        }
    }
}
