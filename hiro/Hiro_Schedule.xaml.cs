using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
{
    /// <summary>
    /// Hiro_Schedule.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Schedule : Page
    {
        private Mainui? Hiro_Main = null;
        public Hiro_Schedule(Mainui? Parent)
        {
            InitializeComponent();
            Hiro_Main = Parent;
            Hiro_Initialize();
            Loaded += Hiro_Schedule_Loaded;
        }

        private void Hiro_Initialize()
        {
            Load_Color();
            Load_Translate();
            Load_Position();
            dgs.ItemsSource = App.scheduleitems;
        }

        private void Hiro_Schedule_Loaded(object sender, RoutedEventArgs e)
        {
            bool animation = !utils.Read_Ini(App.dconfig, "config", "ani", "1").Equals("0");
            if (animation)
                BeginStoryboard(Application.Current.Resources["AppLoad"] as Storyboard);
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDim"] = new SolidColorBrush(utils.Color_Transparent(App.AppForeColor, 80));
            Resources["AppAccent"] = new SolidColorBrush(utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        public void Load_Translate()
        {
            scbtn_1.Content = utils.Get_Transalte("scnew");
            scbtn_2.Content = utils.Get_Transalte("scdelete");
            scbtn_3.Content = utils.Get_Transalte("scmodify");
            dgs.Columns[0].Header = utils.Get_Transalte("sid");
            dgs.Columns[1].Header = utils.Get_Transalte("sname");
            dgs.Columns[2].Header = utils.Get_Transalte("stime");
            dgs.Columns[3].Header = utils.Get_Transalte("scommand");
        }

        public void Load_Position()
        {
            utils.Set_Control_Location(scbtn_1, "scnew");
            utils.Set_Control_Location(scbtn_2, "scdelete");
            utils.Set_Control_Location(scbtn_3, "scmodify");
            utils.Set_Control_Location(dgs, "sdata");
        }

        private void Dgs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Scbtn_3_Click(sender, e);
        }

        private void Scbtn_1_Click(object sender, RoutedEventArgs e)
        {
            Hiro_NewSchedule hn = new(Hiro_Main);
            hn.scbtn_4.Visibility = Visibility.Hidden;
            hn.tb11.Text = "";
            hn.tb12.Text = "";
            hn.tb13.Text = "";
            hn.tb14.Text = "";
            if (Hiro_Main != null)
            {
                Hiro_Main.newx.Content = utils.Get_Transalte("new");
                Hiro_Main.Set_Label(Hiro_Main.newx);
                Hiro_Main.current = hn;
                Hiro_Main.frame.Content = hn;
            }
        }

        private void Scbtn_3_Click(object sender, RoutedEventArgs e)
        {
            if (App.scheduleitems.Count != 0 && dgs.SelectedIndex > -1 && dgs.SelectedIndex < App.scheduleitems.Count)
            {
                Hiro_NewSchedule hn = new(Hiro_Main);
                hn.index = dgs.SelectedIndex;
                hn.scbtn_4.Visibility = Visibility.Visible;
                hn.tb11.Text = App.scheduleitems[dgs.SelectedIndex].Name;
                hn.tb12.Text = App.scheduleitems[dgs.SelectedIndex].Time;
                hn.tb13.Text = App.scheduleitems[dgs.SelectedIndex].Command;
                hn.tb14.Text = "";
                switch (App.scheduleitems[dgs.SelectedIndex].re)
                {
                    case -2.0:
                        hn.rbtn18.IsChecked = true;
                        break;
                    case -1.0:
                        hn.rbtn19.IsChecked = true;
                        break;
                    case 0.0:
                        hn.rbtn20.IsChecked = true;
                        break;
                    default:
                        hn.rbtn21.IsChecked = true;
                        hn.tb14.Text = App.scheduleitems[dgs.SelectedIndex].re.ToString();
                        break;
                }
                if (Hiro_Main != null)
                {
                    Hiro_Main.newx.Content = utils.Get_Transalte("mod");
                    Hiro_Main.Set_Label(Hiro_Main.newx);
                    Hiro_Main.current = hn;
                    Hiro_Main.frame.Content = hn;
                }
            }
        }

        private void Scbtn_2_Click(object sender, RoutedEventArgs e)
        {
            if (App.scheduleitems.Count != 0 && dgs.SelectedIndex > -1 && dgs.SelectedIndex < App.scheduleitems.Count)
            {
                utils.Delete_Alarm(dgs.SelectedIndex);

            }
        }
    }
}
