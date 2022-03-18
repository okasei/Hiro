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
        private Mainui? Hiro_Main = null;
        public Hiro_Config(Mainui? Parent)
        {
            InitializeComponent();
            Hiro_Main = Parent;
            Hiro_Initialize();
            Loaded += Hiro_Config_Loaded;
        }

        private void Hiro_Initialize()
        {
            Load_Color();
            Load_Translate();
            Load_Position();
            reverse_style.IsEnabled = false;
            tr_btn.IsEnabled = false;
            blureff.IsEnabled = false;
            autorun.IsEnabled = false;
            tb1.Text = utils.Read_Ini(App.dconfig, "config", "leftaction", "");
            tb2.Text = utils.Read_Ini(App.dconfig, "config", "middleaction", "");
            tb3.Text = utils.Read_Ini(App.dconfig, "config", "rightaction", "");
            tb4.Text = utils.Read_Ini(App.dconfig, "config", "customname", "");
            tb5.Text = utils.Read_Ini(App.dconfig, "config", "autoaction", "");
            tb10.Text = utils.Read_Ini(App.dconfig, "config", "customhiro", "");
            switch (utils.Read_Ini(App.dconfig, "config", "leftclick", "1"))
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
            switch (utils.Read_Ini(App.dconfig, "config", "middleclick", "1"))
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
            switch (utils.Read_Ini(App.dconfig, "config", "rightclick", "2"))
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
            switch (utils.Read_Ini(App.dconfig, "config", "customuser", "1"))
            {
                case "2":
                    rbtn11.IsChecked = true;
                    App.CustomUsernameFlag = 1;
                    App.Username = utils.Read_Ini(App.dconfig, "config", "customname", "");
                    break;
                default:
                    rbtn10.IsChecked = true;
                    App.CustomUsernameFlag = 0;
                    App.Username = App.EnvironmentUsername;
                    break;
            }
            rbtn13.IsChecked = utils.Read_Ini(App.dconfig, "config", "autoexe", "1").Equals("2");
            rbtn12.IsChecked = !rbtn13.IsChecked;
            cb_box.IsChecked = utils.Read_Ini(App.dconfig, "config", "min", "1").Equals("1");
            rbtn15.IsChecked = utils.Read_Ini(App.dconfig, "config", "background", "1").Equals("2");
            rbtn14.IsChecked = !rbtn15.IsChecked;
            rbtn17.IsChecked = utils.Read_Ini(App.dconfig, "config", "customnick", "1").Equals("2");
            rbtn16.IsChecked = !rbtn17.IsChecked;
            if (rbtn17.IsChecked == true)
            {
                App.AppTitle = utils.Read_Ini(App.dconfig, "config", "customhiro", "Hiro");
                if (App.wnd != null)
                    App.wnd.ti.ToolTipText = utils.Read_Ini(App.dconfig, "config", "customhiro", "Hiro");
            }
            autorun.IsChecked = utils.Read_Ini(App.dconfig, "config", "autorun", "0").Equals("1");
            blureff.IsChecked = utils.Read_Ini(App.dconfig, "config", "blur", "0") switch
            {
                "2" => null,
                "1" => true,
                _ => false,
            };
            verbose.IsChecked = utils.Read_Ini(App.dconfig, "config", "verbose", "1").Equals("1");
            animation.IsChecked = utils.Read_Ini(App.dconfig, "config", "ani", "1").Equals("1");
            win_style.IsChecked = utils.Read_Ini(App.dconfig, "config", "toast", "0").Equals("1");
            reverse_style.IsChecked = utils.Read_Ini(App.dconfig, "config", "reverse", "0").Equals("1");
            tr_btn.IsChecked = utils.Read_Ini(App.dconfig, "config", "trbtn", "0").Equals("1");
            reverse_style.IsEnabled = true;
            tr_btn.IsEnabled = true;
            blureff.IsEnabled = true;
            autorun.IsEnabled = true;
            autorun.Tag = "1";
        }

        public void Load_Position()
        {
            utils.Set_Control_Location(btn10, "bgcustom");
            utils.Set_Control_Location(rbtn1, "showwin");
            utils.Set_Control_Location(rbtn2, "showmenu");
            utils.Set_Control_Location(rbtn3, "runcmd");
            utils.Set_Control_Location(rbtn4, "showwin2");
            utils.Set_Control_Location(rbtn5, "showmenu2");
            utils.Set_Control_Location(rbtn6, "runcmd2");
            utils.Set_Control_Location(rbtn7, "showwin3");
            utils.Set_Control_Location(rbtn8, "showmenu3");
            utils.Set_Control_Location(rbtn9, "runcmd3");
            utils.Set_Control_Location(rbtn10, "calluser");
            utils.Set_Control_Location(rbtn11, "callcus");
            utils.Set_Control_Location(rbtn12, "disabled");
            utils.Set_Control_Location(rbtn13, "runcmd4");
            utils.Set_Control_Location(rbtn14, "colortheme");
            utils.Set_Control_Location(rbtn15, "imagetheme");
            utils.Set_Control_Location(rbtn16, "namehiro");
            utils.Set_Control_Location(rbtn17, "namecus");
            utils.Set_Control_Location(cb_box, "minclose");
            utils.Set_Control_Location(autorun, "autorun");
            utils.Set_Control_Location(blureff, "blurbox");
            utils.Set_Control_Location(win_style, "winbox");
            utils.Set_Control_Location(reverse_style, "reversebox");
            utils.Set_Control_Location(tr_btn, "trbtnbox");
            utils.Set_Control_Location(verbose, "verbosebox");
            utils.Set_Control_Location(animation, "anibox");
            utils.Set_Control_Location(lc_label, "leftclick");
            utils.Set_Control_Location(mc_label, "middleclick");
            utils.Set_Control_Location(rc_label, "rightclick");
            utils.Set_Control_Location(call_label, "callmethod");
            utils.Set_Control_Location(name_label, "namelabel");
            utils.Set_Control_Location(ar_label, "autoexe");
            utils.Set_Control_Location(bg_label, "background");
            utils.Set_Control_Location(langlabel, "language");
            utils.Set_Control_Location(langbox, "langbox");
            utils.Set_Control_Location(langname, "langbox");
            utils.Set_Control_Location(moreandsoon, "morecome");
            utils.Set_Control_Location(btn7, "lock");
            utils.Set_Control_Location(tb1, "lefttb");
            utils.Set_Control_Location(tb2, "middletb");
            utils.Set_Control_Location(tb3, "righttb");
            utils.Set_Control_Location(tb4, "calltb");
            utils.Set_Control_Location(tb5, "autoexetb");
            utils.Set_Control_Location(tb10, "hirotb");
            utils.Set_Grid_Location(BaseGrid, "configg");
            utils.Set_Grid_Location(rc_grid, "rightg");
            utils.Set_Grid_Location(mc_grid, "middleg");
            utils.Set_Grid_Location(lc_grid, "leftg");
            utils.Set_Grid_Location(ar_grid, "autog");
            utils.Set_Grid_Location(cm_grid, "callg");
            utils.Set_Grid_Location(bg_grid, "backg");
            utils.Set_Grid_Location(name_grid, "nameg");
            Thickness thickness = BaseGrid.Margin;
            thickness.Top = 0.0;
            BaseGrid.Margin = thickness;
            configbar.Maximum = BaseGrid.Height - 420;
            configbar.Value = 0.0;
            configbar.ViewportSize = 420;
            foreach (object obj in langbox.Items)
            {
                if (obj is ComboBoxItem mi)
                    utils.Set_Control_Location(mi, "combo", location: false);
            }
        }

        private void Configbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Thickness thickness = BaseGrid.Margin;
            thickness.Top = -configbar.Value;
            BaseGrid.Margin = thickness;
        }

        public void Load_Translate()
        {
            btn10.Content = utils.Get_Transalte("bgcustom");
            rbtn1.Content = utils.Get_Transalte("showwin");
            rbtn2.Content = utils.Get_Transalte("showmenu");
            rbtn3.Content = utils.Get_Transalte("runcmd");
            rbtn4.Content = utils.Get_Transalte("showwin");
            rbtn5.Content = utils.Get_Transalte("showmenu");
            rbtn6.Content = utils.Get_Transalte("runcmd");
            rbtn7.Content = utils.Get_Transalte("showwin");
            rbtn8.Content = utils.Get_Transalte("showmenu");
            rbtn9.Content = utils.Get_Transalte("runcmd");
            rbtn10.Content = utils.Get_Transalte("calluser");
            rbtn11.Content = utils.Get_Transalte("callcus");
            rbtn12.Content = utils.Get_Transalte("disabled");
            rbtn13.Content = utils.Get_Transalte("runcmd");
            rbtn14.Content = utils.Get_Transalte("colortheme");
            rbtn15.Content = utils.Get_Transalte("imagetheme");
            rbtn16.Content = utils.Get_Transalte("namehiro");
            rbtn17.Content = utils.Get_Transalte("namecus");
            cb_box.Content = utils.Get_Transalte("minclose");
            autorun.Content = utils.Get_Transalte("autorun");
            blureff.Content = utils.Get_Transalte("blurbox");
            win_style.Content = utils.Get_Transalte("winbox");
            reverse_style.Content = utils.Get_Transalte("reversebox");
            tr_btn.Content = utils.Get_Transalte("trbtnbox");
            verbose.Content = utils.Get_Transalte("verbosebox");
            animation.Content = utils.Get_Transalte("anibox");
            lc_label.Content = utils.Get_Transalte("leftclick");
            mc_label.Content = utils.Get_Transalte("middleclick");
            rc_label.Content = utils.Get_Transalte("rightclick");
            call_label.Content = utils.Get_Transalte("callmethod");
            ar_label.Content = utils.Get_Transalte("autoexe");
            bg_label.Content = utils.Get_Transalte("background");
            langlabel.Content = utils.Get_Transalte("language");
            name_label.Content = utils.Get_Transalte("namelabel");
            moreandsoon.Content = utils.Get_Transalte("morecome");
            btn7.Content = utils.Get_Transalte("lock");
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppAccent"] = new SolidColorBrush(utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        private void Hiro_Config_Loaded(object sender, RoutedEventArgs e)
        {
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
            bool animation = !utils.Read_Ini(App.dconfig, "config", "ani", "1").Equals("0");
            if (animation)
                BeginStoryboard(Application.Current.Resources["AppLoad"] as Storyboard);
        }

        private void Rbtn3_Unchecked(object sender, RoutedEventArgs e)
        {
            tb1.IsEnabled = false;
        }

        private void Rbtn3_Checked(object sender, RoutedEventArgs e)
        {
            tb1.IsEnabled = true;
            utils.Write_Ini(App.dconfig, "config", "leftclick", "3");
        }

        private void Rbtn1_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "leftclick", "1");
        }

        private void Rbtn2_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "leftclick", "2");
        }

        private void Tb1_TextChanged(object sender, TextChangedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "leftaction", tb1.Text);
        }

        private void Rbtn6_Unchecked(object sender, RoutedEventArgs e)
        {
            tb2.IsEnabled = false;
        }

        private void Rbtn6_Checked(object sender, RoutedEventArgs e)
        {
            tb2.IsEnabled = true;
            utils.Write_Ini(App.dconfig, "config", "middleclick", "3");
        }

        private void Rbtn4_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "middleclick", "1");
        }

        private void Rbtn5_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "middleclick", "2");
        }

        private void Rbtn9_Checked(object sender, RoutedEventArgs e)
        {
            tb3.IsEnabled = true;
            utils.Write_Ini(App.dconfig, "config", "rightclick", "3");
        }

        private void Rbtn9_Unchecked(object sender, RoutedEventArgs e)
        {
            tb3.IsEnabled = false;
        }

        private void Rbtn7_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "rightclick", "1");
        }

        private void Rbtn8_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "rightclick", "2");
        }

        private void Rbtn10_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "customuser", "1");
            App.CustomUsernameFlag = 0;
            App.Username = App.EnvironmentUsername;
        }

        private void Rbtn11_Checked(object sender, RoutedEventArgs e)
        {
            tb4.IsEnabled = true;
            utils.Write_Ini(App.dconfig, "config", "customuser", "2");
            App.CustomUsernameFlag = 1;
            App.Username = utils.Read_Ini(App.dconfig, "config", "customname", "");
        }

        private void Rbtn11_Unchecked(object sender, RoutedEventArgs e)
        {
            tb4.IsEnabled = false;
        }

        private void Tb4_TextChanged(object sender, TextChangedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "customname", tb4.Text);
            App.Username = tb4.Text;
        }

        private void Rbtn12_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "autoexe", "1");
        }

        private void Rbtn13_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "autoexe", "2");
            tb5.IsEnabled = true;
        }

        private void Rbtn13_Unchecked(object sender, RoutedEventArgs e)
        {
            tb5.IsEnabled = false;
        }

        private void Tb5_TextChanged(object sender, TextChangedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "autoaction", tb5.Text);
        }

        private void Cb_box_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "min", "1");
        }

        private void Cb_box_Unchecked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "min", "0");
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
                {
                    Hiro_Main.versionlabel.Content = res.ApplicationVersion + " 🔒";
                    Hiro_Main.Set_Label(Hiro_Main.homex);
                }
            };
            bw.RunWorkerAsync();

        }

        private void Verbose_Unchecked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "verbose", "0");
        }

        private void Verbose_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "verbose", "1");
        }

        private void Animation_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "ani", "1");
        }

        private void Animation_Unchecked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "ani", "0");
        }

        private void Win_style_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "toast", "1");
        }

        private void Win_style_Unchecked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "toast", "0");
        }

        private void Langbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.lang = App.la[langbox.SelectedIndex].Name;
            langname.Content = App.la[langbox.SelectedIndex].Langname;
            App.LangFilePath = App.CurrentDirectory + "\\system\\lang\\" + App.la[langbox.SelectedIndex].Name + ".hlp";
            utils.Write_Ini(App.dconfig, "config", "lang", App.lang);
            App.Load_Menu();
            if (Hiro_Main != null)
            {
                Hiro_Main.Load_Translate();
                Hiro_Main.Load_Position();
            }
        }

        private void Reverse_style_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "reverse", "1");
            if (App.wnd != null)
                App.wnd.Load_All_Colors();
        }

        private void Reverse_style_Unchecked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "reverse", "0");
            if (App.wnd != null)
                App.wnd.Load_All_Colors();
        }

        private void Blureff_Indeterminate(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "blur", "2");
            App.blurradius = 25.0;
            if (Hiro_Main != null)
                Hiro_Main.Blurbgi(3);
        }

        private void Blureff_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "blur", "1");
            App.blurradius = 50.0;
            if (Hiro_Main != null)
                Hiro_Main.Blurbgi(1);
        }

        private void Blureff_Unchecked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "blur", "0");
            App.blurradius = 50.0;
            if (Hiro_Main != null)
                Hiro_Main.Blurbgi(0);
        }

        private void Configbar_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            configbar.Value += e.Delta * (configbar.Maximum - configbar.ViewportSize) / configbar.ViewportSize;
        }

        private void Rbtn17_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "customnick", "2");
            tb10.IsEnabled = true;
            App.AppTitle = tb10.Text;
            Title = App.AppTitle + " - " + utils.Get_Transalte("version").Replace("%c", res.ApplicationVersion);
            if (Hiro_Main != null)
                Hiro_Main.titlelabel.Content = App.AppTitle;
            if (App.wnd != null)
                App.wnd.ti.ToolTipText = tb10.Text;
        }

        private void Tb10_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (utils.Read_Ini(App.dconfig, "config", "customnick", "1").Equals("2"))
            {
                utils.Write_Ini(App.dconfig, "config", "customhiro", tb10.Text);
                App.AppTitle = tb10.Text;
                Title = App.AppTitle + " - " + utils.Get_Transalte("version").Replace("%c", res.ApplicationVersion);
                if (Hiro_Main != null)
                    Hiro_Main.titlelabel.Content = App.AppTitle;
                if (App.wnd != null)
                    App.wnd.ti.ToolTipText = tb10.Text;
            }

        }

        private void Autorun_Checked(object sender, RoutedEventArgs e)
        {
            if (autorun.Tag != null && autorun.Tag.Equals("1"))
            {
                autorun.Tag = "2";
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
                    autorun.IsChecked = false;
                    utils.LogtoFile("[ERROR]" + ex.Message);
                }
            }
            autorun.Tag = "1";
        }

        private void Autorun_Unchecked(object sender, RoutedEventArgs e)
        {
            if (autorun.Tag != null && autorun.Tag.Equals("1"))
            {
                autorun.Tag = "2";
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
                    autorun.IsChecked = true;
                    utils.LogtoFile("[ERROR]" + ex.Message);
                }
            }
            autorun.Tag = "1";
        }

        private void Rbtn16_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "customnick", "1");
            tb10.IsEnabled = false;
            App.AppTitle = res.ApplicationName;
            Title = App.AppTitle + " - " + utils.Get_Transalte("version").Replace("%c", res.ApplicationVersion);
            if (Hiro_Main != null)
                Hiro_Main.titlelabel.Content = App.AppTitle;
            if (App.wnd != null)
                App.wnd.ti.ToolTipText = App.AppTitle;
        }

        private void Tb2_TextChanged(object sender, TextChangedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "middleaction", tb2.Text);
        }

        private void Tb3_TextChanged(object sender, TextChangedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "rightaction", tb3.Text);
        }


        private void Rbtn15_Checked(object sender, RoutedEventArgs e)
        {
            rbtn15.IsEnabled = false;
            rbtn14.IsEnabled = false;
            btn10.IsEnabled = true;
            utils.Write_Ini(App.dconfig, "config", "background", "2");
            if (Hiro_Main != null)
            {
                utils.Set_Bgimage(Hiro_Main.bgimage);
                Hiro_Main.Blurbgi(utils.ConvertInt(utils.Read_Ini(App.dconfig, "config", "blur", "0")));
            }
            rbtn15.IsEnabled = true;
            rbtn14.IsEnabled = true;
        }

        private void Rbtn14_Checked(object sender, RoutedEventArgs e)
        {
            rbtn15.IsEnabled = false;
            rbtn14.IsEnabled = false;
            utils.Write_Ini(App.dconfig, "config", "background", "1");
            if (Hiro_Main != null)
            {
                Hiro_Main.bgimage.Background = new SolidColorBrush(App.AppAccentColor);
                Hiro_Main.Blurbgi(0);
            }
            rbtn15.IsEnabled = true;
            rbtn14.IsEnabled = true;
        }

        private void Btn10_Click(object sender, RoutedEventArgs e)
        {
            if (rbtn15.IsChecked == true)
            {
                string strFileName = "";
                Microsoft.Win32.OpenFileDialog ofd = new()
                {
                    Filter = utils.Get_Transalte("picfiles") + "|*.jpg;*.jpeg;*.bmp;*.gif;*.png|" + utils.Get_Transalte("allfiles") + "|*.*",
                    ValidateNames = true, // 验证用户输入是否是一个有效的Windows文件名
                    CheckFileExists = true, //验证路径的有效性
                    CheckPathExists = true,//验证路径的有效性
                    Title = utils.Get_Transalte("openfile") + " - " + App.AppTitle
                };
                if (ofd.ShowDialog() == true) //用户点击确认按钮，发送确认消息
                {
                    strFileName = ofd.FileName;//获取在文件对话框中选定的路径或者字符串

                }
                if (System.IO.File.Exists(strFileName))
                {
                    utils.Write_Ini(App.dconfig, "config", "backimage", strFileName);
                    if (Hiro_Main != null)
                    {
                        utils.Set_Bgimage(Hiro_Main.bgimage);
                        Hiro_Main.Blurbgi(Convert.ToInt16(utils.Read_Ini(App.dconfig, "config", "blur", "0")));
                    }
                }
            }
            else
            {
                Hiro_Color hc = new(Hiro_Main, this);
                hc.color_picker.Color = App.AppAccentColor;
                hc.Unify_Color(true);
                if (Hiro_Main != null)
                {
                    Hiro_Main.Set_Label(Hiro_Main.colorx);
                    Hiro_Main.current = hc;
                    Hiro_Main.frame.Content = hc;
                }
                    
            }
        }
        private void Tr_btn_Checked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "trbtn", "1");
            App.trval = 0;
            if (App.wnd != null)
                App.wnd.Load_All_Colors();
        }

        private void Tr_btn_Unchecked(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "trbtn", "0");
            App.trval = 160;
            if (App.wnd != null)
                App.wnd.Load_All_Colors();
        }
    }
}
