using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
{
    /// <summary>
    /// Hiro_Config.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Config : Page
    {
        private Hiro_MainUI? Hiro_Main = null;
        private bool Load = false;
        public Hiro_Config(Hiro_MainUI? Parent)
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
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            Storyboard sb = new();
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                Hiro_Utils.AddPowerAnimation(0, BaseGrid, sb, -100, null);
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
            langbox.Items.Clear();
            langbox.ItemsSource = App.la;
            langbox.DisplayMemberPath = "Langname";
            langbox.SelectedValuePath = "Langname";
            for (int i = 0; i < App.la.Count; i++)
            {
                if (App.lang.Equals(App.la[i].Name))
                {
                    langbox.SelectedIndex = i;
                }
            }
            tb1.Text = Hiro_Utils.Read_Ini(App.dconfig, "Config", "LeftAction", "");
            tb2.Text = Hiro_Utils.Read_Ini(App.dconfig, "Config", "MiddleAction", "");
            tb3.Text = Hiro_Utils.Read_Ini(App.dconfig, "Config", "RightAction", "");
            tb4.Text = Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomName", "");
            tb5.Text = Hiro_Utils.Read_Ini(App.dconfig, "Config", "AutoAction", "");
            tb10.Text = Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomHIRO", "");
            switch (Hiro_Utils.Read_Ini(App.dconfig, "Config", "LeftClick", "1"))
            {
                case "2":
                    rbtn2.IsChecked = true;
                    break;
                case "3":
                    rbtn3.IsChecked = true;
                    break;
                default:
                    rbtn1.IsChecked = true;
                    break;
            }
            switch (Hiro_Utils.Read_Ini(App.dconfig, "Config", "MiddleClick", "2"))
            {
                case "2":
                    rbtn5.IsChecked = true;
                    break;
                case "3":
                    rbtn6.IsChecked = true;
                    break;
                default:
                    rbtn4.IsChecked = true;
                    break;
            }
            switch (Hiro_Utils.Read_Ini(App.dconfig, "Config", "RightClick", "2"))
            {
                case "2":
                    rbtn8.IsChecked = true;
                    break;
                case "3":
                    rbtn9.IsChecked = true;
                    break;
                default:
                    rbtn7.IsChecked = true;
                    break;
            }
            switch (Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomUser", "1"))
            {
                case "2":
                    rbtn11.IsChecked = true;
                    App.CustomUsernameFlag = 1;
                    App.Username = Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomName", "");
                    break;
                default:
                    rbtn10.IsChecked = true;
                    App.CustomUsernameFlag = 0;
                    App.Username = App.EnvironmentUsername;
                    break;
            }
            rbtn13.IsChecked = Hiro_Utils.Read_Ini(App.dconfig, "Config", "AutoExe", "1").Equals("2");
            rbtn12.IsChecked = !rbtn13.IsChecked;
            cb_box.IsChecked = Hiro_Utils.Read_Ini(App.dconfig, "Config", "Min", "1").Equals("1");
            rbtn15.IsChecked = Hiro_Utils.Read_Ini(App.dconfig, "Config", "Background", "1").Equals("2");
            rbtn14.IsChecked = !rbtn15.IsChecked;
            rbtn17.IsChecked = Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomNick", "1").Equals("2");
            rbtn16.IsChecked = !rbtn17.IsChecked;
            if (rbtn17.IsChecked == true)
            {
                App.AppTitle = Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomHIRO", "Hiro");
                if (App.wnd != null)
                    App.wnd.ti.ToolTipText = Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomHIRO", "Hiro");
            }
            Autorun.IsChecked = Hiro_Utils.Read_Ini(App.dconfig, "Config", "AutoRun", "0").Equals("1");
            blureff.IsChecked = Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0") switch
            {
                "2" => null,
                "1" => true,
                _ => false,
            };
            Verbose.IsChecked = Hiro_Utils.Read_Ini(App.dconfig, "Config", "Verbose", "0").Equals("1");
            animation.IsChecked = Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2") switch
            {
                "2" => null,
                "1" => true,
                _ => false,
            };
            win_style.IsChecked = Hiro_Utils.Read_Ini(App.dconfig, "Config", "Toast", "0").Equals("1");
            reverse_style.IsChecked = Hiro_Utils.Read_Ini(App.dconfig, "Config", "Reverse", "0").Equals("1");
            tr_btn.IsChecked = Hiro_Utils.Read_Ini(App.dconfig, "Config", "TRBtn", "0").Equals("1");
            Autorun.Tag = "1";
            Load = true;
        }

        public void Load_Position()
        {
            Hiro_Utils.Set_Control_Location(btn10, "bgcustom");
            Hiro_Utils.Set_Control_Location(rbtn1, "showwin");
            Hiro_Utils.Set_Control_Location(rbtn2, "showmenu");
            Hiro_Utils.Set_Control_Location(rbtn3, "runcmd");
            Hiro_Utils.Set_Control_Location(rbtn4, "showwin2");
            Hiro_Utils.Set_Control_Location(rbtn5, "showmenu2");
            Hiro_Utils.Set_Control_Location(rbtn6, "runcmd2");
            Hiro_Utils.Set_Control_Location(rbtn7, "showwin3");
            Hiro_Utils.Set_Control_Location(rbtn8, "showmenu3");
            Hiro_Utils.Set_Control_Location(rbtn9, "runcmd3");
            Hiro_Utils.Set_Control_Location(rbtn10, "calluser");
            Hiro_Utils.Set_Control_Location(rbtn11, "callcus");
            Hiro_Utils.Set_Control_Location(rbtn12, "disabled");
            Hiro_Utils.Set_Control_Location(rbtn13, "runcmd4");
            Hiro_Utils.Set_Control_Location(rbtn14, "colortheme");
            Hiro_Utils.Set_Control_Location(rbtn15, "imagetheme");
            Hiro_Utils.Set_Control_Location(rbtn16, "namehiro");
            Hiro_Utils.Set_Control_Location(rbtn17, "namecus");
            Hiro_Utils.Set_Control_Location(cb_box, "minclose");
            Hiro_Utils.Set_Control_Location(Autorun, "autorun");
            Hiro_Utils.Set_Control_Location(blureff, "blurbox");
            Hiro_Utils.Set_Control_Location(win_style, "winbox");
            Hiro_Utils.Set_Control_Location(reverse_style, "reversebox");
            Hiro_Utils.Set_Control_Location(tr_btn, "trbtnbox");
            Hiro_Utils.Set_Control_Location(Verbose, "verbosebox");
            Hiro_Utils.Set_Control_Location(animation, "anibox");
            Hiro_Utils.Set_Control_Location(lc_label, "leftclick");
            Hiro_Utils.Set_Control_Location(mc_label, "middleclick");
            Hiro_Utils.Set_Control_Location(rc_label, "rightclick");
            Hiro_Utils.Set_Control_Location(call_label, "callmethod");
            Hiro_Utils.Set_Control_Location(name_label, "namelabel");
            Hiro_Utils.Set_Control_Location(ar_label, "autoExe");
            Hiro_Utils.Set_Control_Location(bg_label, "background");
            Hiro_Utils.Set_Control_Location(langlabel, "language");
            Hiro_Utils.Set_Control_Location(langbox, "langbox");
            Hiro_Utils.Set_Control_Location(langname, "langbox");
            Hiro_Utils.Set_Control_Location(moreandsoon, "morecome");
            Hiro_Utils.Set_Control_Location(btn7, "lock");
            Hiro_Utils.Set_Control_Location(btnx1, "proxybtn");
            Hiro_Utils.Set_Control_Location(tb1, "lefttb");
            Hiro_Utils.Set_Control_Location(tb2, "middletb");
            Hiro_Utils.Set_Control_Location(tb3, "righttb");
            Hiro_Utils.Set_Control_Location(tb4, "calltb");
            Hiro_Utils.Set_Control_Location(tb5, "autoexetb");
            Hiro_Utils.Set_Control_Location(tb10, "hirotb");
            Hiro_Utils.Set_Grid_Location(BaseGrid, "configg");
            Hiro_Utils.Set_Grid_Location(rc_grid, "rightg");
            Hiro_Utils.Set_Grid_Location(mc_grid, "middleg");
            Hiro_Utils.Set_Grid_Location(lc_grid, "leftg");
            Hiro_Utils.Set_Grid_Location(ar_grid, "autog");
            Hiro_Utils.Set_Grid_Location(cm_grid, "callg");
            Hiro_Utils.Set_Grid_Location(bg_grid, "backg");
            Hiro_Utils.Set_Grid_Location(name_grid, "nameg");
            Thickness thickness = BaseGrid.Margin;
            thickness.Top = 0.0;
            BaseGrid.Margin = thickness;
            configbar.Maximum = BaseGrid.Height - 420;
            configbar.Value = 0.0;
            configbar.ViewportSize = 420;
            foreach (var obj in langbox.Items)
            {
                if (obj is ComboBoxItem mi)
                    Hiro_Utils.Set_Control_Location(mi, "combo", location: false);
            }
        }

        private void Configbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var thickness = BaseGrid.Margin;
            thickness.Top = -configbar.Value;
            BaseGrid.Margin = thickness;
        }

        public void Load_Translate()
        {
            btn10.Content = Hiro_Utils.Get_Transalte("bgcustom");
            rbtn1.Content = Hiro_Utils.Get_Transalte("showwin");
            rbtn2.Content = Hiro_Utils.Get_Transalte("showmenu");
            rbtn3.Content = Hiro_Utils.Get_Transalte("runcmd");
            rbtn4.Content = Hiro_Utils.Get_Transalte("showwin");
            rbtn5.Content = Hiro_Utils.Get_Transalte("showmenu");
            rbtn6.Content = Hiro_Utils.Get_Transalte("runcmd");
            rbtn7.Content = Hiro_Utils.Get_Transalte("showwin");
            rbtn8.Content = Hiro_Utils.Get_Transalte("showmenu");
            rbtn9.Content = Hiro_Utils.Get_Transalte("runcmd");
            rbtn10.Content = Hiro_Utils.Get_Transalte("calluser");
            rbtn11.Content = Hiro_Utils.Get_Transalte("callcus");
            rbtn12.Content = Hiro_Utils.Get_Transalte("disabled");
            rbtn13.Content = Hiro_Utils.Get_Transalte("runcmd");
            rbtn14.Content = Hiro_Utils.Get_Transalte("colortheme");
            rbtn15.Content = Hiro_Utils.Get_Transalte("imagetheme");
            rbtn16.Content = Hiro_Utils.Get_Transalte("namehiro");
            rbtn17.Content = Hiro_Utils.Get_Transalte("namecus");
            cb_box.Content = Hiro_Utils.Get_Transalte("minclose");
            Autorun.Content = Hiro_Utils.Get_Transalte("AutoRun");
            blureff.Content = Hiro_Utils.Get_Transalte("blurbox");
            win_style.Content = Hiro_Utils.Get_Transalte("winbox");
            reverse_style.Content = Hiro_Utils.Get_Transalte("reversebox");
            tr_btn.Content = Hiro_Utils.Get_Transalte("trbtnbox");
            Verbose.Content = Hiro_Utils.Get_Transalte("verbosebox");
            animation.Content = Hiro_Utils.Get_Transalte("anibox");
            lc_label.Content = Hiro_Utils.Get_Transalte("leftclick");
            mc_label.Content = Hiro_Utils.Get_Transalte("middleclick");
            rc_label.Content = Hiro_Utils.Get_Transalte("rightclick");
            call_label.Content = Hiro_Utils.Get_Transalte("callmethod");
            ar_label.Content = Hiro_Utils.Get_Transalte("autoexe");
            bg_label.Content = Hiro_Utils.Get_Transalte("background");
            langlabel.Content = Hiro_Utils.Get_Transalte("language");
            name_label.Content = Hiro_Utils.Get_Transalte("namelabel");
            moreandsoon.Content = Hiro_Utils.Get_Transalte("morecome");
            btn7.Content = Hiro_Utils.Get_Transalte("lock");
            btnx1.Content = Hiro_Utils.Get_Transalte("proxybtn");
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        private void Rbtn3_Unchecked(object sender, RoutedEventArgs e)
        {
            tb1.IsEnabled = false;
        }

        private void Rbtn3_Checked(object sender, RoutedEventArgs e)
        {
            tb1.IsEnabled = true;
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "LeftClick", "3");
        }

        private void Rbtn1_Checked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "LeftClick", "1");
        }

        private void Rbtn2_Checked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "LeftClick", "2");
        }

        private void Tb1_TextChanged(object sender, TextChangedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "LeftAction", tb1.Text);
        }

        private void Rbtn6_Unchecked(object sender, RoutedEventArgs e)
        {
            tb2.IsEnabled = false;
        }

        private void Rbtn6_Checked(object sender, RoutedEventArgs e)
        {
            tb2.IsEnabled = true;
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "MiddleClick", "3");
        }

        private void Rbtn4_Checked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "Middleclick", "1");
        }

        private void Rbtn5_Checked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "Middleclick", "2");
        }

        private void Rbtn9_Checked(object sender, RoutedEventArgs e)
        {
            tb3.IsEnabled = true;
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "Rightclick", "3");
        }

        private void Rbtn9_Unchecked(object sender, RoutedEventArgs e)
        {
            tb3.IsEnabled = false;
        }

        private void Rbtn7_Checked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "RightClick", "1");
        }

        private void Rbtn8_Checked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "RightClick", "2");
        }

        private void Rbtn10_Checked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "CustomUser", "1");
            App.CustomUsernameFlag = 0;
            App.Username = App.EnvironmentUsername;
        }

        private void Rbtn11_Checked(object sender, RoutedEventArgs e)
        {
            tb4.IsEnabled = true;
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "CustomUser", "2");
            App.CustomUsernameFlag = 1;
            App.Username = Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomName", "");
        }

        private void Rbtn11_Unchecked(object sender, RoutedEventArgs e)
        {
            tb4.IsEnabled = false;
        }

        private void Tb4_TextChanged(object sender, TextChangedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "CustomName", tb4.Text);
            App.Username = tb4.Text;
        }

        private void Rbtn12_Checked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "AutoExe", "1");
        }

        private void Rbtn13_Checked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "AutoExe", "2");
            tb5.IsEnabled = true;
        }

        private void Rbtn13_Unchecked(object sender, RoutedEventArgs e)
        {
            tb5.IsEnabled = false;
        }

        private void Tb5_TextChanged(object sender, TextChangedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "AutoAction", tb5.Text);
        }

        private void Cb_box_Checked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "Min", "1");
        }

        private void Cb_box_Unchecked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "Min", "0");
        }

        private void Btn7_Click(object sender, RoutedEventArgs e)
        {
            btn7.IsEnabled = false;
            System.ComponentModel.BackgroundWorker bw = new();
            bw.RunWorkerCompleted += delegate
            {
                App.Locked = true;
                btn7.IsEnabled = true;
                if (Hiro_Main != null)
                    Hiro_Main.versionlabel.Content = res.ApplicationVersion + " 🔒";
                Hiro_Main?.Set_Label(Hiro_Main.homex);
            };
            bw.RunWorkerAsync();

        }

        private void Verbose_Unchecked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "Verbose", "0");
        }

        private void Verbose_Checked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "Verbose", "1");
        }

        private void Animation_Checked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "Ani", "1");
        }
        private void Animation_Indeterminate(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "Ani", "2");
        }
        private void Animation_Unchecked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "Ani", "0");
        }

        private void Win_style_Checked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "Toast", "1");
        }

        private void Win_style_Unchecked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "Toast", "0");
        }

        private void Langbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.lang = App.la[langbox.SelectedIndex].Name;
            langname.Content = App.la[langbox.SelectedIndex].Langname;
            App.LangFilePath = App.CurrentDirectory + "\\system\\lang\\" + App.la[langbox.SelectedIndex].Name + ".hlp";
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "Lang", App.lang);
            App.Load_Menu();
            Hiro_Main?.Load_Translate();
            Hiro_Main?.Load_Position();
        }

        private void Reverse_style_Checked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "Reverse", "1");
                if (App.wnd != null)
                    App.wnd.Load_All_Colors();
            }
            
        }

        private void Reverse_style_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "Reverse", "0");
                if (App.wnd != null)
                    App.wnd.Load_All_Colors();
            }
        }

        private void Blureff_Indeterminate(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "Blur", "2");
                App.blurradius = 25.0;
                if (Hiro_Main != null)
                    Hiro_Main.Blurbgi(3);
            }
        }

        private void Blureff_Checked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "Blur", "1");
                App.blurradius = 50.0;
                if (Hiro_Main != null)
                    Hiro_Main.Blurbgi(1);
            }
        }

        private void Blureff_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "Blur", "0");
                App.blurradius = 50.0;
                if (Hiro_Main != null)
                    Hiro_Main.Blurbgi(0);
            }
        }

        private void Configbar_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            configbar.Value += e.Delta * (configbar.Maximum - configbar.ViewportSize) / configbar.ViewportSize;
        }

        private void Rbtn17_Checked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "CustomNick", "2");
            tb10.IsEnabled = true;
            App.AppTitle = tb10.Text;
            Title = App.AppTitle + " - " + Hiro_Utils.Get_Transalte("version").Replace("%c", res.ApplicationVersion);
            if (Hiro_Main != null)
                Hiro_Main.titlelabel.Content = App.AppTitle;
            if (App.wnd != null)
                App.wnd.ti.ToolTipText = tb10.Text;
        }

        private void Tb10_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomNick", "1").Equals("2"))
            {
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "CustomHIRO", tb10.Text);
                App.AppTitle = tb10.Text;
                Title = App.AppTitle + " - " + Hiro_Utils.Get_Transalte("version").Replace("%c", res.ApplicationVersion);
                if (Hiro_Main != null)
                    Hiro_Main.titlelabel.Content = App.AppTitle;
                if (App.wnd != null)
                    App.wnd.ti.ToolTipText = tb10.Text;
            }

        }

        private void Autorun_Checked(object sender, RoutedEventArgs e)
        {
            if (Autorun.Tag != null && Autorun.Tag.Equals("1") && Load)
            {
                Autorun.Tag = "2";
                try
                {
                    if (Environment.ProcessPath != null)
                    {
                        System.Diagnostics.ProcessStartInfo pinfo = new();
                        pinfo.UseShellExecute = true;
                        pinfo.FileName = Environment.ProcessPath;
                        pinfo.Arguments = "autostart_on";
                        pinfo.Verb = "runas";
                        System.Diagnostics.Process.Start(pinfo);
                    }
                }
                catch (Exception ex)
                {
                    Autorun.IsChecked = false;
                    Hiro_Utils.LogtoFile("[ERROR]" + ex.Message);
                }
            }
            Autorun.Tag = "1";
        }

        private void Autorun_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Autorun.Tag != null && Autorun.Tag.Equals("1") && Load)
            {
                Autorun.Tag = "2";
                try
                {
                    if (Environment.ProcessPath != null)
                    {
                        System.Diagnostics.ProcessStartInfo pinfo = new();
                        pinfo.UseShellExecute = true;
                        pinfo.FileName = Environment.ProcessPath;
                        pinfo.Arguments = "autostart_off";
                        pinfo.Verb = "runas";
                        System.Diagnostics.Process.Start(pinfo);
                    }
                }
                catch (Exception ex)
                {
                    Autorun.IsChecked = true;
                    Hiro_Utils.LogtoFile("[ERROR]" + ex.Message);
                }
            }
            Autorun.Tag = "1";
        }

        private void Rbtn16_Checked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "CustomNick", "1");
            tb10.IsEnabled = false;
            App.AppTitle = res.ApplicationName;
            Title = App.AppTitle + " - " + Hiro_Utils.Get_Transalte("version").Replace("%c", res.ApplicationVersion);
            if (Hiro_Main != null)
                Hiro_Main.titlelabel.Content = App.AppTitle;
            if (App.wnd != null)
                App.wnd.ti.ToolTipText = App.AppTitle;
        }

        private void Tb2_TextChanged(object sender, TextChangedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "MiddleAction", tb2.Text);
        }

        private void Tb3_TextChanged(object sender, TextChangedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "RightAction", tb3.Text);
        }


        private void Rbtn15_Checked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                rbtn15.IsEnabled = false;
                rbtn14.IsEnabled = false;
                btn10.IsEnabled = true;
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "Background", "2");
                if (Hiro_Main != null)
                {
                    Hiro_Utils.Set_Bgimage(Hiro_Main.bgimage);
                    Hiro_Main.Blurbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
                }
                rbtn15.IsEnabled = true;
                rbtn14.IsEnabled = true;
            }
        }

        private void Rbtn14_Checked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                rbtn15.IsEnabled = false;
                rbtn14.IsEnabled = false;
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "Background", "1");
                if (Hiro_Main != null)
                {
                    Hiro_Main.bgimage.Background = new SolidColorBrush(App.AppAccentColor);
                    Hiro_Main.Blurbgi(0);
                }
                rbtn15.IsEnabled = true;
                rbtn14.IsEnabled = true;
            }
        }

        private void Btn10_Click(object sender, RoutedEventArgs e)
        {
            if (rbtn15.IsChecked == true)
            {
                string strFileName = "";
                Microsoft.Win32.OpenFileDialog ofd = new()
                {
                    Filter = Hiro_Utils.Get_Transalte("picfiles") + "|*.jpg;*.jpeg;*.bmp;*.gif;*.png|" + Hiro_Utils.Get_Transalte("allfiles") + "|*.*",
                    ValidateNames = true, // 验证用户输入是否是一个有效的Windows文件名
                    CheckFileExists = true, //验证路径的有效性
                    CheckPathExists = true,//验证路径的有效性
                    Title = Hiro_Utils.Get_Transalte("openfile") + " - " + App.AppTitle
                };
                if (ofd.ShowDialog() == true) //用户点击确认按钮，发送确认消息
                {
                    strFileName = ofd.FileName;//获取在文件对话框中选定的路径或者字符串

                }
                if (System.IO.File.Exists(strFileName))
                {
                    Hiro_Utils.Write_Ini(App.dconfig, "Config", "BackImage", strFileName);
                    if (Hiro_Main != null)
                    {
                        Hiro_Utils.Set_Bgimage(Hiro_Main.bgimage);
                        Hiro_Main.Blurbgi(Convert.ToInt16(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
                    }
                }
            }
            else
            {
                if (Hiro_Main != null)
                {
                    Hiro_Main.Set_Label(Hiro_Main.colorx);
                }   
            }
        }
        private void Tr_btn_Checked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "TRBtn", "1");
                App.trval = 0;
                if (App.wnd != null)
                    App.wnd.Load_All_Colors();
            }
        }

        private void Tr_btn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "TRBtn", "0");
                App.trval = 160;
                if (App.wnd != null)
                    App.wnd.Load_All_Colors();
            }
        }

        private void Btnx1_Click(object sender, RoutedEventArgs e)
        {
            if (Hiro_Main != null)
            {
                Hiro_Main.Set_Label(Hiro_Main.proxyx);
            }
        }

        private void BaseGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            configbar.Value += e.Delta * (configbar.Maximum - configbar.ViewportSize) / configbar.ViewportSize;
        }
    }
}
