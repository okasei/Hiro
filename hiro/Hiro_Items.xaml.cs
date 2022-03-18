using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
{
    /// <summary>
    /// Hiro_Items.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Items : Page
    {
        private Mainui? Hiro_Main = null;
        public Hiro_Items(Mainui? Parent)
        {
            InitializeComponent();
            Hiro_Main = Parent;
            Hiro_Initialize();
            Loaded += Hiro_Items_Loaded;
        }

        private void Hiro_Initialize()
        {
            Load_Color();
            Load_Translate();
            Load_Position();
            dgi.ItemsSource = App.cmditems;
        }

        private void Hiro_Items_Loaded(object sender, RoutedEventArgs e)
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
            btn1.Content = utils.Get_Transalte("inew");
            btn2.Content = utils.Get_Transalte("iup");
            btn3.Content = utils.Get_Transalte("idown");
            btn4.Content = utils.Get_Transalte("ilaunch");
            btn5.Content = utils.Get_Transalte("idelete");
            btn6.Content = utils.Get_Transalte("imodify");
            dgi.Columns[0].Header = utils.Get_Transalte("page");
            dgi.Columns[1].Header = utils.Get_Transalte("id");
            dgi.Columns[2].Header = utils.Get_Transalte("name");
            dgi.Columns[3].Header = utils.Get_Transalte("command");
        }

        public void Load_Position()
        {
            utils.Set_Control_Location(btn1, "inew");
            utils.Set_Control_Location(btn2, "iup");
            utils.Set_Control_Location(btn3, "idown");
            utils.Set_Control_Location(btn4, "ilaunch");
            utils.Set_Control_Location(btn5, "idelete");
            utils.Set_Control_Location(btn6, "imodify");
            utils.Set_Control_Location(dgi, "data");
        }

        private void Dgi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Btn6_Click(sender, e);
        }

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            Hiro_NewItem hn = new(Hiro_Main);
            hn.Load_ComboBox();
            hn.ntn9.Visibility = Visibility.Hidden;
            hn.tb7.Text = "";
            hn.tb8.Text = "";
            hn.keybox.SelectedIndex = 0;
            hn.modibox.SelectedIndex = 0;
            if (Hiro_Main != null)
            {
                Hiro_Main.newx.Content = utils.Get_Transalte("new");
                Hiro_Main.Set_Label(Hiro_Main.newx);
                Hiro_Main.current = hn;
                Hiro_Main.frame.Content = hn;
            }
            
        }

        private void Btn2_Click(object sender, RoutedEventArgs e)
        {
            btn2.IsEnabled = false;
            utils.Delay(200);
            btn2.IsEnabled = true;
            if (App.cmditems.Count != 0 && dgi.SelectedIndex > 0 && dgi.SelectedIndex < App.cmditems.Count)
            {
                var i = dgi.SelectedIndex;
                Cmditem nec = new(App.cmditems[i - 1].Page, App.cmditems[i - 1].Id, App.cmditems[i].Name, App.cmditems[i].Command, App.cmditems[i].HotKey);
                App.cmditems[i] = new(App.cmditems[i].Page, App.cmditems[i].Id, App.cmditems[i - 1].Name, App.cmditems[i - 1].Command, App.cmditems[i - 1].HotKey);
                App.cmditems[i - 1] = nec;
                var inipath = App.dconfig;
                utils.Write_Ini(inipath, i.ToString(), "title", nec.Name);
                utils.Write_Ini(inipath, i.ToString(), "command", "(" + nec.Command + ")");
                utils.Write_Ini(inipath, i.ToString(), "hotkey", nec.HotKey);
                utils.Write_Ini(inipath, (i + 1).ToString(), "title", App.cmditems[i].Name);
                utils.Write_Ini(inipath, (i + 1).ToString(), "command", "(" + App.cmditems[i].Command + ")");
                utils.Write_Ini(inipath, (i + 1).ToString(), "hotkey", App.cmditems[i].HotKey);
                dgi.SelectedIndex = i - 1;
                App.Load_Menu();
                var vsi = utils.FindHotkeyById(i - 1);
                var vsx = utils.FindHotkeyById(i);
                if (vsi > -1)
                    App.vs[vsi + 1] = i;
                if (vsx > -1)
                    App.vs[vsx + 1] = i - 1;
            }
            GC.Collect();
        }

        private void Btn3_Click(object sender, RoutedEventArgs e)
        {
            btn3.IsEnabled = false;
            utils.Delay(200);
            btn3.IsEnabled = true;
            if (App.cmditems.Count != 0 && dgi.SelectedIndex > -1 && dgi.SelectedIndex < App.cmditems.Count - 1)
            {
                var i = dgi.SelectedIndex;
                Cmditem nec = new(App.cmditems[i + 1].Page, App.cmditems[i + 1].Id, App.cmditems[i].Name, App.cmditems[i].Command, App.cmditems[i].HotKey);
                App.cmditems[i] = new(App.cmditems[i].Page, App.cmditems[i].Id, App.cmditems[i + 1].Name, App.cmditems[i + 1].Command, App.cmditems[i + 1].HotKey);
                App.cmditems[i + 1] = nec;
                var inipath = App.dconfig;
                utils.Write_Ini(inipath, (i + 1).ToString(), "title", App.cmditems[i].Name);
                utils.Write_Ini(inipath, (i + 1).ToString(), "command", "(" + App.cmditems[i].Command + ")");
                utils.Write_Ini(inipath, (i + 1).ToString(), "hotkey", App.cmditems[i].HotKey);
                utils.Write_Ini(inipath, (i + 2).ToString(), "title", App.cmditems[i + 1].Name);
                utils.Write_Ini(inipath, (i + 2).ToString(), "command", "(" + App.cmditems[i + 1].Command + ")");
                utils.Write_Ini(inipath, (i + 2).ToString(), "hotkey", App.cmditems[i + 1].HotKey);
                dgi.SelectedIndex = i + 1;
                App.Load_Menu();
                var vsi = utils.FindHotkeyById(i + 1);
                var vsx = utils.FindHotkeyById(i);
                if (vsi > -1)
                    App.vs[vsi + 1] = i;
                if (vsx > -1)
                    App.vs[vsx + 1] = i + 1;
            }
            GC.Collect();
        }

        private void Btn5_Click(object sender, RoutedEventArgs e)
        {
            btn5.IsEnabled = false;
            utils.Delay(200);
            if (App.cmditems.Count != 0 && dgi.SelectedIndex > -1 && dgi.SelectedIndex < App.cmditems.Count)
            {
                var i = dgi.SelectedIndex;
                var inipath = App.dconfig;
                while (i < App.cmditems.Count - 1)
                {
                    App.cmditems[i].Name = App.cmditems[i + 1].Name;
                    App.cmditems[i].Command = App.cmditems[i + 1].Command;
                    App.cmditems[i].HotKey = App.cmditems[i + 1].HotKey;
                    utils.Write_Ini(inipath, (i + 1).ToString(), "title", utils.Read_Ini(inipath, (i + 2).ToString(), "title", " "));
                    utils.Write_Ini(inipath, (i + 1).ToString(), "command", utils.Read_Ini(inipath, (i + 2).ToString(), "command", " "));
                    utils.Write_Ini(inipath, (i + 1).ToString(), "hotkey", utils.Read_Ini(inipath, (i + 2).ToString(), "hotkey", " "));
                    i++;
                    System.Windows.Forms.Application.DoEvents();
                    var vst = utils.FindHotkeyById(i);
                    if (vst > -1)
                    {
                        App.vs[vst + 1]--;
                    }
                }
                utils.Write_Ini(inipath, (i + 1).ToString(), "title", " ");
                utils.Write_Ini(inipath, (i + 1).ToString(), "command", " ");
                utils.Write_Ini(inipath, (i + 1).ToString(), "hotkey", " ");
                App.cmditems.RemoveAt(i);
                var total = (App.cmditems.Count % 10 == 0) ? App.cmditems.Count / 10 : App.cmditems.Count / 10 + 1;
                if (App.page > total - 1 && App.page > 0)
                    App.page--;
                App.Load_Menu();
            }
            var vsi = utils.FindHotkeyById(dgi.SelectedIndex);
            if (vsi > -1)
            {
                utils.UnregisterKey(vsi);
            }
            btn5.IsEnabled = true;
            GC.Collect();
        }

        private void Btn6_Click(object sender, RoutedEventArgs e)
        {
            if (App.cmditems.Count != 0 && dgi.SelectedIndex > -1 && dgi.SelectedIndex < App.cmditems.Count)
            {
                Hiro_NewItem hn = new(Hiro_Main);
                hn.index = dgi.SelectedIndex;
                hn.Load_ComboBox();
                hn.ntn9.Visibility = Visibility.Visible;
                hn.tb7.Text = App.cmditems[dgi.SelectedIndex].Name;
                hn.tb8.Text = App.cmditems[dgi.SelectedIndex].Command;
                var key = App.cmditems[dgi.SelectedIndex].HotKey;
                try
                {
                    if (key.IndexOf(",") != -1)
                    {
                        var mo = int.Parse(key[..key.IndexOf(",")]);
                        var vkey = int.Parse(key.Substring(key.IndexOf(",") + 1, key.Length - key.IndexOf(",") - 1));
                        hn.modibox.SelectedIndex = utils.Index_Modifier(false, mo);
                        hn.keybox.SelectedIndex = utils.Index_vKey(false, vkey);
                    }
                    else
                    {
                        hn.modibox.SelectedIndex = 0;
                        hn.keybox.SelectedIndex = 0;
                    }
                }
                catch (Exception ex)
                {
                    utils.LogtoFile("[ERROR]" + ex.Message);
                    hn.modibox.SelectedIndex = 0;
                    hn.keybox.SelectedIndex = 0;
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

        private void Btn4_Click(object sender, RoutedEventArgs e)
        {
            btn4.IsEnabled = false;
            utils.Delay(200);
            btn4.IsEnabled = true;
            if (App.cmditems.Count != 0 && dgi.SelectedIndex > -1 && dgi.SelectedIndex < App.cmditems.Count)
            {
                utils.RunExe(App.cmditems[dgi.SelectedIndex].Command);
            }
        }
    }
}
