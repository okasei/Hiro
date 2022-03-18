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

namespace hiro
{
    /// <summary>
    /// Navigate.xaml の相互作用ロジック
    /// </summary>
    public partial class Navigate : Window
    {
        int id = 1;
        public Navigate()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (id == 1)
            {
                id = 0;
                Framex.Navigate(new Uri("/SubPage.xaml", UriKind.Relative));
            }
            else
            {
                id = 1;
                Framex.Navigate(new Uri("/HomePage.xaml", UriKind.Relative));
            }
            //Framex.Navigate(new Uri("SubPage.xaml"), UriKind.Relative);
        }
    }
}
