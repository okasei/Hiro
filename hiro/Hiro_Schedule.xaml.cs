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
        private Hiro_MainUI? Hiro_Main = null;
        public Hiro_Schedule(Hiro_MainUI? Parent)
        {
            InitializeComponent();
            Hiro_Main = Parent;
            Hiro_Initialize();
            Loaded += delegate
            {
                HiHiro();
            };
        }

        public void HiHiro()
        {
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                Storyboard sb = new();
                Hiro_Utils.AddPowerAnimation(1, scbtn_1, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(1, scbtn_2, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(1, scbtn_3, sb, 50, null);
                sb.Begin();
            }
        }

        private void Hiro_Initialize()
        {
            Load_Color();
            Load_Translate();
            Load_Position();
            dgs.ItemsSource = App.scheduleitems;
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 80));
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        public void Load_Translate()
        {
            scbtn_1.Content = Hiro_Utils.Get_Transalte("scnew");
            scbtn_2.Content = Hiro_Utils.Get_Transalte("scdelete");
            scbtn_3.Content = Hiro_Utils.Get_Transalte("scmodify");
            dgs.Columns[0].Header = Hiro_Utils.Get_Transalte("sid");
            dgs.Columns[1].Header = Hiro_Utils.Get_Transalte("sname");
            dgs.Columns[2].Header = Hiro_Utils.Get_Transalte("stime");
            dgs.Columns[3].Header = Hiro_Utils.Get_Transalte("scommand");
        }

        public void Load_Position()
        {
            Hiro_Utils.Set_Control_Location(scbtn_1, "scnew");
            Hiro_Utils.Set_Control_Location(scbtn_2, "scdelete");
            Hiro_Utils.Set_Control_Location(scbtn_3, "scmodify");
            Hiro_Utils.Set_Control_Location(dgs, "sdata");
        }

        private void Dgs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Scbtn_3_Click(sender, e);
        }

        private void Scbtn_1_Click(object sender, RoutedEventArgs e)
        {
            if (Hiro_Main != null)
            {
                if (Hiro_Main.hiro_newschedule == null)
                    Hiro_Main.hiro_newschedule = new(Hiro_Main);
                Hiro_Main.hiro_newschedule.scbtn_4.Visibility = Visibility.Hidden;
                Hiro_Main.hiro_newschedule.tb11.Text = "";
                Hiro_Main.hiro_newschedule.tb12.Text = "";
                Hiro_Main.hiro_newschedule.tb13.Text = "";
                Hiro_Main.hiro_newschedule.tb14.Text = "";
                Hiro_Main.newx.Content = Hiro_Utils.Get_Transalte("new");
                Hiro_Main.current = Hiro_Main.hiro_newschedule;
                Hiro_Main.Set_Label(Hiro_Main.newx);
            }
            
        }

        private void Scbtn_3_Click(object sender, RoutedEventArgs e)
        {
            if (App.scheduleitems.Count != 0 && dgs.SelectedIndex > -1 && dgs.SelectedIndex < App.scheduleitems.Count && Hiro_Main != null)
            { 
                if (Hiro_Main.hiro_newschedule == null)
                    Hiro_Main.hiro_newschedule = new(Hiro_Main);
                Hiro_Main.hiro_newschedule.index = dgs.SelectedIndex;
                Hiro_Main.hiro_newschedule.scbtn_4.Visibility = Visibility.Visible;
                Hiro_Main.hiro_newschedule.tb11.Text = App.scheduleitems[dgs.SelectedIndex].Name;
                Hiro_Main.hiro_newschedule.tb12.Text = App.scheduleitems[dgs.SelectedIndex].Time;
                Hiro_Main.hiro_newschedule.tb13.Text = App.scheduleitems[dgs.SelectedIndex].Command;
                Hiro_Main.hiro_newschedule.tb14.Text = "";
                switch (App.scheduleitems[dgs.SelectedIndex].re)
                {
                    case -2.0:
                        Hiro_Main.hiro_newschedule.rbtn18.IsChecked = true;
                        break;
                    case -1.0:
                        Hiro_Main.hiro_newschedule.rbtn19.IsChecked = true;
                        break;
                    case 0.0:
                        Hiro_Main.hiro_newschedule.rbtn20.IsChecked = true;
                        break;
                    default:
                        Hiro_Main.hiro_newschedule.rbtn21.IsChecked = true;
                        Hiro_Main.hiro_newschedule.tb14.Text = App.scheduleitems[dgs.SelectedIndex].re.ToString();
                        break;
                }
                Hiro_Main.newx.Content = Hiro_Utils.Get_Transalte("mod");
                Hiro_Main.current = Hiro_Main.hiro_newschedule;
                Hiro_Main.Set_Label(Hiro_Main.newx);
            }
        }

        private void Scbtn_2_Click(object sender, RoutedEventArgs e)
        {
            if (App.scheduleitems.Count != 0 && dgs.SelectedIndex > -1 && dgs.SelectedIndex < App.scheduleitems.Count)
            {
                Hiro_Utils.Delete_Alarm(dgs.SelectedIndex);

            }
        }
    }
}
