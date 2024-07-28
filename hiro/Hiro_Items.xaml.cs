using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static MaterialDesignThemes.Wpf.Theme;

namespace hiro
{
    /// <summary>
    /// Hiro_Items.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Items : Page
    {
        private Hiro_MainUI? Hiro_Main = null;
        public Hiro_Items(Hiro_MainUI? Parent)
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
            bool animation = !Hiro_Utils.Read_DCIni("Ani", "2").Equals("0");
            Storyboard sb = new();
            if (Hiro_Utils.Read_DCIni("Ani", "2").Equals("1"))
            {
                Hiro_Utils.AddPowerAnimation(1, btn1, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(1, btn2, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(1, btn3, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(1, btn4, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(1, btn5, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(1, btn6, sb, 50, null);
            }

            if (!animation) 
                return;
            Hiro_Utils.AddPowerAnimation(0, this, sb, 50, null);
            sb.Begin();
        }

        private void Hiro_Initialize()
        {
            Load_Color();
            Load_Translate();
            Load_Position();
            dgi.ItemsSource = App.cmditems;
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 80));
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        public void Load_Translate()
        {
            btn1.Content = Hiro_Utils.Get_Translate("inew");
            btn2.Content = Hiro_Utils.Get_Translate("iup");
            btn3.Content = Hiro_Utils.Get_Translate("idown");
            btn4.Content = Hiro_Utils.Get_Translate("ilaunch");
            btn5.Content = Hiro_Utils.Get_Translate("idelete");
            btn6.Content = Hiro_Utils.Get_Translate("imodify");
            dgi.Columns[0].Header = Hiro_Utils.Get_Translate("page");
            dgi.Columns[1].Header = Hiro_Utils.Get_Translate("id");
            dgi.Columns[2].Header = Hiro_Utils.Get_Translate("Name");
            dgi.Columns[3].Header = Hiro_Utils.Get_Translate("Command");
        }

        public void Load_Position()
        {
            Hiro_Utils.Set_Control_Location(ExCellLabel, "icell", location: false);
            Hiro_Utils.Set_Control_Location(btn1, "inew");
            Hiro_Utils.Set_Control_Location(btn2, "iup");
            Hiro_Utils.Set_Control_Location(btn3, "idown");
            Hiro_Utils.Set_Control_Location(btn4, "ilaunch");
            Hiro_Utils.Set_Control_Location(btn5, "idelete");
            Hiro_Utils.Set_Control_Location(btn6, "imodify");
            Hiro_Utils.Set_Control_Location(dgi, "data");
        }

        private void Dgi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Btn6_Click(sender, e);
        }

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            if (Hiro_Main == null) 
                return;
            Hiro_Main.hiro_newitem ??= new(Hiro_Main);
            Hiro_Main.hiro_newitem.Load_ComboBox();
            Hiro_Main.hiro_newitem.ntn9.Visibility = Visibility.Hidden;
            Hiro_Main.hiro_newitem.tb7.Text = "";
            Hiro_Main.hiro_newitem.tb8.Text = "";
            Hiro_Main.hiro_newitem.keybox.SelectedIndex = 0;
            Hiro_Main.hiro_newitem.modibox.SelectedIndex = 0;
            Hiro_Main.newx.Content = Hiro_Utils.Get_Translate("new");
            Hiro_Main.current = Hiro_Main.hiro_newitem;
            Hiro_Main.Set_Label(Hiro_Main.newx);

        }

        private void Btn2_Click(object sender, RoutedEventArgs e)
        {
            btn2.IsEnabled = false;
            Hiro_Utils.Delay(200);
            btn2.IsEnabled = true;
            if (App.cmditems.Count != 0 && dgi.SelectedIndex > 0 && dgi.SelectedIndex < App.cmditems.Count)
            {
                var i = dgi.SelectedIndex;
                Helpers.Hiro_Class.Cmditem nec = new(App.cmditems[i - 1].Page, App.cmditems[i - 1].Id, App.cmditems[i].Name, App.cmditems[i].Command, App.cmditems[i].HotKey);
                App.cmditems[i] = new(App.cmditems[i].Page, App.cmditems[i].Id, App.cmditems[i - 1].Name, App.cmditems[i - 1].Command, App.cmditems[i - 1].HotKey);
                App.cmditems[i - 1] = nec;
                var inipath = App.dConfig;
                Hiro_Utils.Write_Ini(inipath, i.ToString(), "Title", nec.Name);
                Hiro_Utils.Write_Ini(inipath, i.ToString(), "Command", "(" + nec.Command + ")");
                Hiro_Utils.Write_Ini(inipath, i.ToString(), "HotKey", nec.HotKey);
                Hiro_Utils.Write_Ini(inipath, (i + 1).ToString(), "Title", App.cmditems[i].Name);
                Hiro_Utils.Write_Ini(inipath, (i + 1).ToString(), "Command", "(" + App.cmditems[i].Command + ")");
                Hiro_Utils.Write_Ini(inipath, (i + 1).ToString(), "HotKey", App.cmditems[i].HotKey);
                dgi.SelectedIndex = i - 1;
                App.Load_Menu();
                var vsi = Hiro_Utils.FindHotkeyById(i - 1);
                var vsx = Hiro_Utils.FindHotkeyById(i);
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
            Hiro_Utils.Delay(200);
            btn3.IsEnabled = true;
            if (App.cmditems.Count != 0 && dgi.SelectedIndex > -1 && dgi.SelectedIndex < App.cmditems.Count - 1)
            {
                var i = dgi.SelectedIndex;
                Helpers.Hiro_Class.Cmditem nec = new(App.cmditems[i + 1].Page, App.cmditems[i + 1].Id, App.cmditems[i].Name, App.cmditems[i].Command, App.cmditems[i].HotKey);
                App.cmditems[i] = new(App.cmditems[i].Page, App.cmditems[i].Id, App.cmditems[i + 1].Name, App.cmditems[i + 1].Command, App.cmditems[i + 1].HotKey);
                App.cmditems[i + 1] = nec;
                var inipath = App.dConfig;
                Hiro_Utils.Write_Ini(inipath, (i + 1).ToString(), "Title", App.cmditems[i].Name);
                Hiro_Utils.Write_Ini(inipath, (i + 1).ToString(), "Command", "(" + App.cmditems[i].Command + ")");
                Hiro_Utils.Write_Ini(inipath, (i + 1).ToString(), "HotKey", App.cmditems[i].HotKey);
                Hiro_Utils.Write_Ini(inipath, (i + 2).ToString(), "Title", App.cmditems[i + 1].Name);
                Hiro_Utils.Write_Ini(inipath, (i + 2).ToString(), "Command", "(" + App.cmditems[i + 1].Command + ")");
                Hiro_Utils.Write_Ini(inipath, (i + 2).ToString(), "HotKey", App.cmditems[i + 1].HotKey);
                dgi.SelectedIndex = i + 1;
                App.Load_Menu();
                var vsi = Hiro_Utils.FindHotkeyById(i + 1);
                var vsx = Hiro_Utils.FindHotkeyById(i);
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
            Hiro_Utils.Delay(200);
            var vsi = Hiro_Utils.FindHotkeyById(dgi.SelectedIndex);
            if (vsi > -1)
            {
                Hiro_Utils.UnregisterKey(vsi);
            }
            if (App.cmditems.Count != 0 && dgi.SelectedIndex > -1 && dgi.SelectedIndex < App.cmditems.Count)
            {
                var i = dgi.SelectedIndex;
                var inipath = App.dConfig;
                while (i < App.cmditems.Count - 1)
                {
                    App.cmditems[i].Name = App.cmditems[i + 1].Name;
                    App.cmditems[i].Command = App.cmditems[i + 1].Command;
                    App.cmditems[i].HotKey = App.cmditems[i + 1].HotKey;
                    Hiro_Utils.Write_Ini(inipath, (i + 1).ToString(), "Title", Hiro_Utils.Read_Ini(inipath, (i + 2).ToString(), "Title", " "));
                    Hiro_Utils.Write_Ini(inipath, (i + 1).ToString(), "Command", Hiro_Utils.Read_Ini(inipath, (i + 2).ToString(), "Command", " "));
                    Hiro_Utils.Write_Ini(inipath, (i + 1).ToString(), "HotKey", Hiro_Utils.Read_Ini(inipath, (i + 2).ToString(), "HotKey", " "));
                    i++;
                    System.Windows.Forms.Application.DoEvents();
                    var vst = Hiro_Utils.FindHotkeyById(i);
                    if (vst > -1)
                    {
                        App.vs[vst + 1]--;
                    }
                }
                Hiro_Utils.Write_Ini(inipath, (i + 1).ToString(), "Title", " ");
                Hiro_Utils.Write_Ini(inipath, (i + 1).ToString(), "Command", " ");
                Hiro_Utils.Write_Ini(inipath, (i + 1).ToString(), "HotKey", " ");
                App.cmditems.RemoveAt(i);
                var total = (App.cmditems.Count % 10 == 0) ? App.cmditems.Count / 10 : App.cmditems.Count / 10 + 1;
                if (App.page > total - 1 && App.page > 0)
                    App.page--;
                App.Load_Menu();
            }
            btn5.IsEnabled = true;
            GC.Collect();
        }

        private void Btn6_Click(object sender, RoutedEventArgs e)
        {
            if (App.cmditems.Count == 0 || dgi.SelectedIndex <= -1 || dgi.SelectedIndex >= App.cmditems.Count ||
                Hiro_Main == null) 
                return;
            Hiro_Main.hiro_newitem ??= new(Hiro_Main);
            Hiro_Main.hiro_newitem.index = dgi.SelectedIndex;
            Hiro_Main.hiro_newitem.Load_ComboBox();
            Hiro_Main.hiro_newitem.ntn9.Visibility = Visibility.Visible;
            Hiro_Main.hiro_newitem.tb7.Text = App.cmditems[dgi.SelectedIndex].Name;
            Hiro_Main.hiro_newitem.tb8.Text = App.cmditems[dgi.SelectedIndex].Command;
            var key = App.cmditems[dgi.SelectedIndex].HotKey;
            try
            {
                if (key.IndexOf(",") != -1)
                {
                    var mo = int.Parse(key[..key.IndexOf(",")]);
                    var vkey = int.Parse(key.Substring(key.IndexOf(",") + 1, key.Length - key.IndexOf(",") - 1));
                    Hiro_Main.hiro_newitem.modibox.SelectedIndex = Hiro_Utils.Index_Modifier(false, mo);
                    Hiro_Main.hiro_newitem.keybox.SelectedIndex = Hiro_Utils.Index_vKey(false, vkey);
                }
                else
                {
                    Hiro_Main.hiro_newitem.modibox.SelectedIndex = 0;
                    Hiro_Main.hiro_newitem.keybox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.Items.Bind");
                Hiro_Main.hiro_newitem.modibox.SelectedIndex = 0;
                Hiro_Main.hiro_newitem.keybox.SelectedIndex = 0;
            }
            Hiro_Main.newx.Content = Hiro_Utils.Get_Translate("mod");
            Hiro_Main.current = Hiro_Main.hiro_newitem;
            Hiro_Main.Set_Label(Hiro_Main.newx);
        }

        private void Btn4_Click(object sender, RoutedEventArgs e)
        {
            btn4.IsEnabled = false;
            Hiro_Utils.Delay(200);
            btn4.IsEnabled = true;
            if (App.cmditems.Count != 0 && dgi.SelectedIndex > -1 && dgi.SelectedIndex < App.cmditems.Count)
            {
                Hiro_Utils.RunExe(App.cmditems[dgi.SelectedIndex].Command, App.cmditems[dgi.SelectedIndex].Name);
            }
        }
    }
}
