using Hiro.Helpers;
using Hiro.Resources;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static Hiro.Helpers.HSet;

namespace Hiro
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
            bool animation = !Read_DCIni("Ani", "2").Equals("0");
            Storyboard sb = new();
            if (Read_DCIni("Ani", "2").Equals("1"))
            {
                HAnimation.AddPowerAnimation(0, BaseGrid, sb, -100, null);
            }
            if (!animation)
                return;
            HAnimation.AddPowerAnimation(0, this, sb, 50, null);
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
            fr_box.Items.Clear();
            fr_box.Items.Add(new ComboBoxItem()
            {
                Content = "1"
            });
            fr_box.Items.Add(new ComboBoxItem()
            {
                Content = "2"
            });
            fr_box.Items.Add(new ComboBoxItem()
            {
                Content = "3"
            });
            fr_box.Items.Add(new ComboBoxItem()
            {
                Content = "15"
            });
            fr_box.Items.Add(new ComboBoxItem()
            {
                Content = "30"
            });
            fr_box.Items.Add(new ComboBoxItem()
            {
                Content = "45"
            });
            fr_box.Items.Add(new ComboBoxItem()
            {
                Content = "60"
            });
            fr_box.Items.Add(new ComboBoxItem()
            {
                Content = "75"
            });
            fr_box.Items.Add(new ComboBoxItem()
            {
                Content = "90"
            });
            fr_box.Items.Add(new ComboBoxItem()
            {
                Content = "120"
            });
            fr_box.Items.Add(new ComboBoxItem()
            {
                Content = "144"
            });
            fr_box.Items.Add(new ComboBoxItem()
            {
                Content = "240"
            });
            int frame = Convert.ToInt32(Read_DCIni("FPS", "60"));
            for (int i = 0; i < fr_box.Items.Count; i++)
            {
                if (frame.ToString().Equals((fr_box.Items[i] as ComboBoxItem).Content.ToString()))
                {
                    fr_box.SelectedIndex = i;
                    break;
                }
            }
            for (int i = 0; i < App.la.Count; i++)
            {
                if (App.lang.Equals(App.la[i].Name))
                {
                    langbox.SelectedIndex = i;
                    break;
                }
            }
            tb1.Text = Read_DCIni("LeftAction", "");
            tb2.Text = Read_DCIni("MiddleAction", "");
            tb3.Text = Read_DCIni("RightAction", "");
            tb5.Text = Read_DCIni("AutoAction", "");
            switch (Read_DCIni("LeftClick", "1"))
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
            switch (Read_DCIni("MiddleClick", "2"))
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
            switch (Read_DCIni("RightClick", "2"))
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
            switch (Read_DCIni("Toast", "0"))
            {
                case "1":
                    rbtn18.IsChecked = true;
                    break;
                case "2":
                    rbtn19.IsChecked = true;
                    break;
                case "3":
                    rbtn20.IsChecked = true;
                    break;
                case "4":
                    rbtn21.IsChecked = true;
                    break;
                case "5":
                    rbtn22.IsChecked = true;
                    break;
                default:
                    rbtn17.IsChecked = true;
                    break;
            }
            rbtn13.IsChecked = Read_DCIni("Autoexe", "false").Equals("true", StringComparison.InvariantCulture);
            rbtn12.IsChecked = !rbtn13.IsChecked;
            cb_box.IsChecked = Read_DCIni("Min", "true").Equals("true", StringComparison.CurrentCultureIgnoreCase);
            rbtn15.IsChecked = Read_DCIni("Background", "1").Equals("2");
            rbtn14.IsChecked = Read_DCIni("Background", "1").Equals("1");
            acrylic_btn.IsChecked = Read_DCIni("Background", "1").Equals("3");
            video_btn.IsChecked = Read_DCIni("Background", "1").Equals("4");
            Autorun.IsChecked = Read_DCIni("AutoRun", "false").Equals("true", StringComparison.CurrentCultureIgnoreCase);
            blureff.IsChecked = Read_DCIni("Blur", "0") switch
            {
                "2" => null,
                "1" => true,
                _ => false,
            };
            Verbose.IsChecked = Read_DCIni("Verbose", "false").Equals("true", StringComparison.CurrentCultureIgnoreCase);
            animation.IsChecked = Read_DCIni("Ani", "2") switch
            {
                "2" => null,
                "1" => true,
                _ => false,
            };
            reverse_style.IsChecked = Read_DCIni("Reverse", "false").Equals("true", StringComparison.CurrentCultureIgnoreCase);
            tr_btn.IsChecked = Read_DCIni("TRBtn", "false").Equals("true", StringComparison.CurrentCultureIgnoreCase);
            image_compress.IsChecked = Read_DCIni("Compression", "true").Equals("true", StringComparison.CurrentCultureIgnoreCase);
            UrlConfirmBox.IsChecked = Read_DCIni("URLConfirm", "false").Equals("true", StringComparison.CurrentCultureIgnoreCase);
            MonitorSysBox.IsChecked = Read_DCIni("MonitorSys", "true").Equals("true", StringComparison.CurrentCultureIgnoreCase);
            LowPerfermanceBox.IsChecked = Read_DCIni("Performance", "0") switch
            {
                "1" => null,
                "2" => true,
                _ => false,
            };
            Autorun.Tag = "1";
            Load = true;
        }

        public void Load_Position()
        {
            HUI.Set_Control_Location(btn10, "bgcustom");
            HUI.Set_Control_Location(rbtn1, "showwin");
            HUI.Set_Control_Location(rbtn2, "showmenu");
            HUI.Set_Control_Location(rbtn3, "runcmd");
            HUI.Set_Control_Location(rbtn4, "showwin2");
            HUI.Set_Control_Location(rbtn5, "showmenu2");
            HUI.Set_Control_Location(rbtn6, "runcmd2");
            HUI.Set_Control_Location(rbtn7, "showwin3");
            HUI.Set_Control_Location(rbtn8, "showmenu3");
            HUI.Set_Control_Location(rbtn9, "runcmd3");
            HUI.Set_Control_Location(rbtn12, "disabled");
            HUI.Set_Control_Location(rbtn13, "runcmd4");
            HUI.Set_Control_Location(rbtn14, "colortheme");
            HUI.Set_Control_Location(rbtn15, "imagetheme");
            HUI.Set_Control_Location(acrylic_btn, "acrylictheme");
            HUI.Set_Control_Location(video_btn, "videotheme");
            HUI.Set_Control_Location(cb_box, "minclose");
            HUI.Set_Control_Location(Autorun, "autorun");
            HUI.Set_Control_Location(blureff, "blurbox");
            HUI.Set_Control_Location(no_label, "noticelabel");
            HUI.Set_Control_Location(rbtn17, "noticenormal");
            HUI.Set_Control_Location(rbtn18, "noticewin");
            HUI.Set_Control_Location(rbtn19, "noticeisland");
            HUI.Set_Control_Location(rbtn20, "noticeimgland");
            HUI.Set_Control_Location(rbtn21, "noticehibox");
            HUI.Set_Control_Location(rbtn22, "noticehiboxie");
            HUI.Set_Control_Location(reverse_style, "reversebox");
            HUI.Set_Control_Location(tr_btn, "trbtnbox");
            HUI.Set_Control_Location(image_compress, "imgzip");
            HUI.Set_Control_Location(UrlConfirmBox, "urlconfirm");
            HUI.Set_Control_Location(MonitorSysBox, "monitorsys");
            HUI.Set_Control_Location(LowPerfermanceBox, "performance");
            HUI.Set_Control_Location(Verbose, "verbosebox");
            HUI.Set_Control_Location(animation, "anibox");
            HUI.Set_Control_Location(lc_label, "leftclick");
            HUI.Set_Control_Location(mc_label, "middleclick");
            HUI.Set_Control_Location(rc_label, "rightclick");
            HUI.Set_Control_Location(ar_label, "autoExe");
            HUI.Set_Control_Location(bg_label, "background");
            HUI.Set_Control_Location(bg_darker, "bgdarker");
            HUI.Set_Control_Location(bg_brighter, "bgbrighter");
            HUI.Set_Control_Location(bg_slider, "bgslider");
            HUI.Set_Control_Location(fr_label, "frame");
            HUI.Set_Control_Location(fr_box, "frbox");
            HUI.Set_Control_Location(langlabel, "language");
            HUI.Set_Control_Location(langbox, "langbox");
            HUI.Set_Control_Location(moreandsoon, "morecome");
            HUI.Set_Control_Location(btn7, "lock");
            HUI.Set_Control_Location(btnx1, "proxybtn");
            HUI.Set_Control_Location(tb1, "lefttb");
            HUI.Set_Control_Location(tb2, "middletb");
            HUI.Set_Control_Location(tb3, "righttb");
            HUI.Set_Control_Location(tb5, "autoexetb");
            HUI.Set_FrameworkElement_Location(BaseGrid, "configg");
            HUI.Set_FrameworkElement_Location(rc_grid, "rightg");
            HUI.Set_FrameworkElement_Location(mc_grid, "middleg");
            HUI.Set_FrameworkElement_Location(lc_grid, "leftg");
            HUI.Set_FrameworkElement_Location(ar_grid, "autog");
            HUI.Set_FrameworkElement_Location(bg_grid, "backg");
            HUI.Set_FrameworkElement_Location(no_grid, "noticeg");
            HUI.Set_FrameworkElement_Location(fr_grid, "frameg");
            Thickness thickness = BaseGrid.Margin;
            thickness.Top = 0.0;
            BaseGrid.Margin = thickness;
            foreach (var obj in langbox.Items)
            {
                if (obj is ComboBoxItem mi)
                    HUI.Set_Control_Location(mi, "combo", location: false);
            }
            foreach (var obj in fr_box.Items)
            {
                if (obj is ComboBoxItem mi)
                    HUI.Set_Control_Location(mi, "combo", location: false);
            }
        }


        public void Load_Translate()
        {
            btn10.Content = HText.Get_Translate("bgcustom");
            rbtn1.Content = HText.Get_Translate("showwin");
            rbtn2.Content = HText.Get_Translate("showmenu");
            rbtn3.Content = HText.Get_Translate("runcmd");
            rbtn4.Content = HText.Get_Translate("showwin");
            rbtn5.Content = HText.Get_Translate("showmenu");
            rbtn6.Content = HText.Get_Translate("runcmd");
            rbtn7.Content = HText.Get_Translate("showwin");
            rbtn8.Content = HText.Get_Translate("showmenu");
            rbtn9.Content = HText.Get_Translate("runcmd");
            rbtn12.Content = HText.Get_Translate("disabled");
            rbtn13.Content = HText.Get_Translate("runcmd");
            rbtn14.Content = HText.Get_Translate("colortheme");
            rbtn15.Content = HText.Get_Translate("imagetheme");
            acrylic_btn.Content = HText.Get_Translate("acrylictheme");
            video_btn.Content = HText.Get_Translate("videotheme");
            if (!double.TryParse(Read_DCIni("OpacityMask", "255"), out double to))
                to = 255;
            bg_slider.Value = to;
            cb_box.Content = HText.Get_Translate("minclose");
            Autorun.Content = HText.Get_Translate("autorun");
            blureff.Content = HText.Get_Translate("blurbox");
            no_label.Content = HText.Get_Translate("noticelabel");
            rbtn17.Content = HText.Get_Translate("noticenormal");
            rbtn18.Content = HText.Get_Translate("noticewin");
            rbtn19.Content = HText.Get_Translate("noticeisland");
            rbtn20.Content = HText.Get_Translate("noticeimgland");
            rbtn21.Content = HText.Get_Translate("noticehibox");
            rbtn22.Content = HText.Get_Translate("noticehiboxie");
            reverse_style.Content = HText.Get_Translate("reversebox");
            tr_btn.Content = HText.Get_Translate("trbtnbox");
            image_compress.Content = HText.Get_Translate("imgzip");
            UrlConfirmBox.Content = HText.Get_Translate("urlconfirm");
            MonitorSysBox.Content = HText.Get_Translate("monitorsys");
            LowPerfermanceBox.Content = HText.Get_Translate("performance");
            Verbose.Content = HText.Get_Translate("verbosebox");
            animation.Content = HText.Get_Translate("anibox");
            lc_label.Content = HText.Get_Translate("leftclick");
            mc_label.Content = HText.Get_Translate("middleclick");
            rc_label.Content = HText.Get_Translate("rightclick");
            ar_label.Content = HText.Get_Translate("autoexe");
            bg_label.Content = HText.Get_Translate("background");
            bg_darker.Content = HText.Get_Translate("bgdarker");
            bg_brighter.Content = HText.Get_Translate("bgbrighter");
            fr_label.Content = HText.Get_Translate("frame");
            langlabel.Content = HText.Get_Translate("language");
            moreandsoon.Content = HText.Get_Translate("morecome");
            btn7.Content = HText.Get_Translate("lock");
            btnx1.Content = HText.Get_Translate("proxybtn");
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeReverse"] = new SolidColorBrush(App.AppForeColor == Colors.White ? Colors.Black : Colors.White);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        private void Rbtn3_Unchecked(object sender, RoutedEventArgs e)
        {
            tb1.IsEnabled = false;
        }

        private void Rbtn3_Checked(object sender, RoutedEventArgs e)
        {
            tb1.IsEnabled = true;
            Write_Ini(App.dConfig, "Config", "LeftClick", "3");
        }

        private void Rbtn1_Checked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "LeftClick", "1");
        }

        private void Rbtn2_Checked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "LeftClick", "2");
        }

        private void Tb1_TextChanged(object sender, TextChangedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "LeftAction", tb1.Text);
        }

        private void Rbtn6_Unchecked(object sender, RoutedEventArgs e)
        {
            tb2.IsEnabled = false;
        }

        private void Rbtn6_Checked(object sender, RoutedEventArgs e)
        {
            tb2.IsEnabled = true;
            Write_Ini(App.dConfig, "Config", "MiddleClick", "3");
        }

        private void Rbtn4_Checked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "Middleclick", "1");
        }

        private void Rbtn5_Checked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "Middleclick", "2");
        }

        private void Rbtn9_Checked(object sender, RoutedEventArgs e)
        {
            tb3.IsEnabled = true;
            Write_Ini(App.dConfig, "Config", "Rightclick", "3");
        }

        private void Rbtn9_Unchecked(object sender, RoutedEventArgs e)
        {
            tb3.IsEnabled = false;
        }

        private void Rbtn7_Checked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "RightClick", "1");
        }

        private void Rbtn8_Checked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "RightClick", "2");
        }

        private void Rbtn12_Checked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "AutoExe", "false");
        }

        private void Rbtn13_Checked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "AutoExe", "true");
            tb5.IsEnabled = true;
        }

        private void Rbtn13_Unchecked(object sender, RoutedEventArgs e)
        {
            tb5.IsEnabled = false;
        }

        private void Tb5_TextChanged(object sender, TextChangedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "AutoAction", tb5.Text);
        }

        private void Cb_box_Checked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "Min", "true");
        }

        private void Cb_box_Unchecked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "Min", "false");
        }

        private void Btn7_Click(object sender, RoutedEventArgs e)
        {
            btn7.IsEnabled = false;
            App.Locked = true;
            btn7.IsEnabled = true;
            if (Hiro_Main != null)
                Hiro_Main.versionlabel.Content = Hiro_Resources.ApplicationVersion + " 🔒";
            Hiro_Main?.Set_Label(Hiro_Main.homex);
        }

        private void Verbose_Unchecked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "Verbose", "false");
        }

        private void Verbose_Checked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "Verbose", "true");
        }

        private void Animation_Checked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "Ani", "1");
        }
        private void Animation_Indeterminate(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "Ani", "2");
        }
        private void Animation_Unchecked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "Ani", "0");
        }

        private void Langbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.lang = App.la[langbox.SelectedIndex].Name;
            if (Load)
            {
                App.langFilePath = App.currentDir + "\\system\\lang\\" + App.la[langbox.SelectedIndex].Name + ".hlp";
                Write_Ini(App.dConfig, "Config", "Lang", App.lang);
                App.Load_Menu();
                App.Load_LocalTime();
                if (Read_DCIni("Ani", "2").Equals("1"))
                {
                    if (Hiro_Main != null)
                    {
                        Storyboard sb = new();
                        Hiro_Main.foremask.Visibility = Visibility.Visible;
                        HAnimation.AddDoubleAnimaton(1, 150, Hiro_Main.foremask, "Opacity", sb, 0);
                        sb.Completed += delegate
                        {
                            Hiro_Main.foremask.Opacity = 1;
                            Dispatcher.Invoke(() =>
                            {
                                Hiro_Main?.Load_Translate();
                                Hiro_Main?.Load_Position();
                            });
                            Storyboard sb2 = new();
                            HAnimation.AddDoubleAnimaton(0, 150, Hiro_Main.foremask, "Opacity", sb2, 1);
                            sb2.Completed += delegate
                            {
                                Hiro_Main.foremask.Opacity = 0;
                                Hiro_Main.foremask.Visibility = Visibility.Hidden;
                            };
                            sb2.Begin();
                        };
                        sb.Begin();
                    }

                }
                else
                {
                    Hiro_Main?.Load_Translate();
                    Hiro_Main?.Load_Position();
                }
            }
        }

        private void Reverse_style_Checked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Write_Ini(App.dConfig, "Config", "Reverse", "true");
                if (App.wnd != null)
                    App.wnd.Load_All_Colors();
            }

        }

        private void Reverse_style_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Write_Ini(App.dConfig, "Config", "Reverse", "false");
                if (App.wnd != null)
                    App.wnd.Load_All_Colors();
            }
        }

        private void Blureff_Indeterminate(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Write_Ini(App.dConfig, "Config", "Blur", "2");
                App.blurradius = 25.0;
                if (Hiro_Main != null)
                    Hiro_Main.Blurbgi(3);
            }
        }

        private void Blureff_Checked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Write_Ini(App.dConfig, "Config", "Blur", "1");
                App.blurradius = 50.0;
                if (Hiro_Main != null)
                    Hiro_Main.Blurbgi(1);
            }
        }

        private void Blureff_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Write_Ini(App.dConfig, "Config", "Blur", "0");
                App.blurradius = 50.0;
                if (Hiro_Main != null)
                    Hiro_Main.Blurbgi(0);
            }
        }

        private void Autorun_Checked(object sender, RoutedEventArgs e)
        {
            if (Autorun.Tag != null && Autorun.Tag.Equals("1") && Load)
            {
                Autorun.Tag = "2";
                if (!Hiro_Utils.OpenInNewHiro("autostart_on", true))
                {
                    Autorun.IsChecked = false;
                }
            }
            Autorun.Tag = "1";
        }

        private void Autorun_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Autorun.Tag != null && Autorun.Tag.Equals("1") && Load)
            {
                Autorun.Tag = "2";

                if (!Hiro_Utils.OpenInNewHiro("autostart_off", true))
                {
                    Autorun.IsChecked = true;
                }
            }
            Autorun.Tag = "1";
        }

        private void Tb2_TextChanged(object sender, TextChangedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "MiddleAction", tb2.Text);
        }

        private void Tb3_TextChanged(object sender, TextChangedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "RightAction", tb3.Text);
        }


        private void Rbtn15_Checked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                rbtn15.IsEnabled = false;
                rbtn14.IsEnabled = false;
                acrylic_btn.IsEnabled = false;
                video_btn.IsEnabled = false;
                Write_Ini(App.dConfig, "Config", "Background", "2");
                if (Hiro_Main != null)
                {
                    HUI.Set_Bgimage(Hiro_Main.bgimage, Hiro_Main);
                    Hiro_Main.Blurbgi(Hiro_Utils.ConvertInt(Read_DCIni("Blur", "0")));
                }
                else
                {
                    blureff.IsEnabled = true;
                    rbtn15.IsEnabled = true;
                    rbtn14.IsEnabled = true;
                    acrylic_btn.IsEnabled = true;
                    video_btn.IsEnabled = true;
                    btn10.IsEnabled = true;
                }
            }
            bg_slider.IsEnabled = true;
        }

        private void Rbtn14_Checked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                rbtn15.IsEnabled = false;
                rbtn14.IsEnabled = false;
                video_btn.IsEnabled = false;
                acrylic_btn.IsEnabled = false;
                Write_Ini(App.dConfig, "Config", "Background", "1");
                if (Hiro_Main != null)
                {
                    Hiro_Main.bgimage.Background = new SolidColorBrush(App.AppAccentColor);
                    Hiro_Main.Blurbgi(0);
                }
                else
                {
                    blureff.IsEnabled = false;
                    rbtn15.IsEnabled = true;
                    rbtn14.IsEnabled = true;
                    video_btn.IsEnabled = true;
                    acrylic_btn.IsEnabled = true;
                    btn10.IsEnabled = true;
                }
            }
            bg_slider.IsEnabled = true;
        }

        private void Btn10_Click(object sender, RoutedEventArgs e)
        {
            if (rbtn15.IsChecked == true)
            {
                string strFileName = "";
                Microsoft.Win32.OpenFileDialog ofd = new()
                {
                    Filter = HText.Get_Translate("picfiles") + "|*.jpg;*.jpeg;*.jpe;*.jfif;*.bmp;*.dib;*.gif;*.png;*.apng;*.tiff;*.heic;*.heif|" + HText.Get_Translate("allfiles") + "|*.*",
                    ValidateNames = true, // 验证用户输入是否是一个有效的Windows文件名
                    CheckFileExists = true, //验证路径的有效性
                    CheckPathExists = true,//验证路径的有效性
                    Title = HText.Get_Translate("ofdTitle").Replace("%t", HText.Get_Translate("openfile")).Replace("%a", App.appTitle)
                };
                if (ofd.ShowDialog() == true) //用户点击确认按钮，发送确认消息
                {
                    strFileName = ofd.FileName;
                    var newStrFileName = strFileName;
                    strFileName = HText.Path_Prepare("<hiapp>\\images\\crop\\" + System.IO.Path.GetFileName(strFileName));
                    HFile.CreateFolder(strFileName);
                    new Hiro_Cropper(newStrFileName, strFileName, new Point(550, 450), (x) =>
                    {
                        if (x == true)
                        {
                            if (System.IO.File.Exists(@strFileName))
                            {
                                new System.Threading.Thread(() =>
                                {
                                    try
                                    {
                                        System.Drawing.Image img = System.Drawing.Image.FromFile(@strFileName);
                                        double w = img.Width;
                                        double h = img.Height;
                                        double ww = 550 * 2;
                                        double hh = 450 * 2;
                                        Dispatcher.Invoke(() =>
                                        {
                                            if (Hiro_Main != null)
                                            {
                                                ww = Hiro_Main.Width * 2;
                                                hh = Hiro_Main.Height * 2;
                                            }
                                        });
                                        if (ww < w && hh < h && Read_DCIni("Compression", "true").Equals("true", StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            while (ww < w && hh < h)
                                            {
                                                w /= 2;
                                                h /= 2;
                                            }
                                            w *= 2;
                                            h *= 2;
                                            img = HMedia.ZoomImage(img, Convert.ToInt32(h), Convert.ToInt32(w));
                                            img = HMedia.ZipImage(img, HMedia.GetImageFormat(img), 2048);
                                            strFileName = @"<hiapp>\images\background\" + strFileName.Substring(strFileName.LastIndexOf("\\"));
                                            strFileName = HText.Path_PPX(strFileName);
                                            HFile.CreateFolder(strFileName);
                                            if (System.IO.File.Exists(strFileName))
                                                System.IO.File.Delete(strFileName);
                                            img.Save(strFileName);
                                        }
                                        strFileName = HText.Anti_Path_Prepare(strFileName).Replace("\\\\", "\\");
                                        Write_Ini(App.dConfig, "Config", "BackImage", strFileName);
                                        Dispatcher.Invoke(() =>
                                        {
                                            if (Hiro_Main != null)
                                            {
                                                HUI.Set_Bgimage(Hiro_Main.bgimage, Hiro_Main);
                                                Hiro_Main.Blurbgi(Convert.ToInt16(Read_DCIni("Blur", "0")));
                                            }
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        HLogger.LogError(ex, "Hiro.Exception.Background.Image.Select");
                                    }
                                }).Start();
                            }
                        }
                    }).Show();
                }

            }
            else if (acrylic_btn.IsChecked == true)
            {
                Hiro_Main?.Set_Label(Hiro_Main.acrylicx);
            }
            else if (video_btn.IsChecked == true)
            {
                Microsoft.Win32.OpenFileDialog ofd = new()
                {
                    Filter = HText.Get_Translate("vidfiles") +
                "|*.3g2;*.3gp;*.3gp2;*.3gpp;*.amv;*.asf;*.avi;*.bik;*.bin;*.crf;*.dav;*.divx;*.drc;*.dv;*.dvr-ms;*.evo;*.f4v;*.flv;*.gvi;*.gxf;*.m1v;*.m2v;*.m2t;*.m2ts;" +
                "*.m4v;*.mkv;*.mov;*.mp2;*.mp2v;*.mp4;*.mp4v;*.mpe;*.mpeg;*.mpeg1;*.mpeg2;*.mpeg4;*.mpg;*.mpv2;*.mts;*.mtv;*.mxf;*.mxg;*.nsv;*.nuv;*.ogm;*.ogv;*.ogx;*.ps;" +
                "*.rec;*.rm;*.rmvb;*.rpl;*.thp;*.tod;*.tp;*.ts;*.tts;*.txd;*.vob;*.vro;*.webm;*.wm;*.wmv;*.wtv;*.xesc|"
                + HText.Get_Translate("audfiles") +
                "|*.3ga;*.669;*.a52;*.aac;*.ac3;*.adt;*.adts;*.aif;*.aifc;*.aiff;*.amb;*.amr;*.aob;*.ape;*.au;*.awb;*.caf;*.dts;*.flac;*.it;*.kar;*.m4a;*.m4b;*.m4p;*.m5p;" +
                "*.mid;*.mka;*.mlp;*.mod;*.mpa;*.mp1;*.mp2;*.mp3;*.mpc;*.mpga;*.mus;*.oga;*.ogg;*.oma;*.opus;*.qcp;*.ra;*.rmi;*.s3m;*.sid;*.spx;*.tak;*.thd;*.tta;*.voc;" +
                "*.vqf;*.w64;*.wav;*.wma;*.wv;*.xa;*.xm;*.dsf|"
                + HText.Get_Translate("allfiles") + "|*.*",
                    ValidateNames = true, // 验证用户输入是否是一个有效的Windows文件名
                    CheckFileExists = true, //验证路径的有效性
                    CheckPathExists = true,//验证路径的有效性
                    Multiselect = false,
                    Title = HText.Get_Translate("ofdTitle").Replace("%t", HText.Get_Translate("openfile")).Replace("%a", App.appTitle)
                };
                if (ofd.ShowDialog() == true) //用户点击确认按钮，发送确认消息
                {
                    Write_Ini(App.dConfig, "Config", "BackVideo", ofd.FileName);
                    Hiro_Main?.Blurbgi(0);
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
                Write_Ini(App.dConfig, "Config", "TRBtn", "true");
                App.trval = 0;
                if (App.wnd != null)
                    App.wnd.Load_All_Colors();
            }
        }

        private void Tr_btn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Write_Ini(App.dConfig, "Config", "TRBtn", "false");
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

        private void Bg_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Load)
            {
                Write_Ini(App.dConfig, "Config", "OpacityMask", Convert.ToInt32(bg_slider.Value).ToString());
                Hiro_Main?.OpacityBgi();
            }
        }

        private void ArcylicBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                rbtn15.IsEnabled = false;
                rbtn14.IsEnabled = false;
                acrylic_btn.IsEnabled = false;
                video_btn.IsEnabled = false;
                blureff.IsEnabled = false;
                btn10.IsEnabled = false;
                Write_Ini(App.dConfig, "Config", "Background", "3");
                Hiro_Main?.Blurbgi(0);
                rbtn15.IsEnabled = true;
                rbtn14.IsEnabled = true;
                acrylic_btn.IsEnabled = true;
                video_btn.IsEnabled = true;
                btn10.IsEnabled = true;
            }
            bg_slider.IsEnabled = false;
        }

        private void Image_compress_Checked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                image_compress.IsEnabled = false;
                Write_Ini(App.dConfig, "Config", "Compression", "true");
                image_compress.IsEnabled = true;
            }
        }

        private void Image_compress_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Write_Ini(App.dConfig, "Config", "Compression", "false");
            }
        }

        private void Rbtn17_Checked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "Toast", "0");
        }

        private void Rbtn18_Checked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "Toast", "1");
        }

        private void Rbtn19_Checked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "Toast", "2");
        }

        private void Rbtn20_Checked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "Toast", "3");
        }
        private void Rbtn21_Checked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "Toast", "4");
        }

        private void UrlConfirmBox_Checked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "URLConfirm", "true");
        }

        private void UrlConfirmBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "URLConfirm", "false");
        }
        private void MonitorSysBox_Checked(object sender, RoutedEventArgs e)
        {
            if (rbtn18.IsChecked == true)
            {
                rbtn17.IsChecked = true;
                rbtn18.IsChecked = false;
            }
            rbtn18.IsEnabled = false;
            if (Read_DCIni("Toast", "0").Equals("1"))
                Write_Ini(App.dConfig, "Config", "Toast", "0");
            Write_Ini(App.dConfig, "Config", "MonitorSys", "true");
        }

        private void MonitorSysBox_Unchecked(object sender, RoutedEventArgs e)
        {
            rbtn18.IsEnabled = true;
            Write_Ini(App.dConfig, "Config", "MonitorSys", "false");
        }

        private void LowPerformBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Write_Ini(App.dConfig, "Config", "Performance", "2");
                if (App.timer != null)
                    App.timer.Interval = TimeSpan.FromSeconds(10);
                HWin.SetProcessPriority("idle", null, true);
                HWin.SetEffiencyMode("on", null, true);
            }
        }

        private void LowPerformBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Write_Ini(App.dConfig, "Config", "Performance", "0");
                if (App.timer != null)
                    App.timer.Interval = TimeSpan.FromSeconds(1);
                HWin.SetProcessPriority("normal", null, true);
                HWin.SetEffiencyMode("off", null, true);
            }
        }

        private void LowPerformBox_Indeterminate(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Write_Ini(App.dConfig, "Config", "Performance", "1");
                if (App.timer != null)
                    App.timer.Interval = TimeSpan.FromSeconds(3);
                HWin.SetProcessPriority("belownormal", null, true);
                HWin.SetEffiencyMode("off", null, true);
            }
        }
        //LowPerformBox_Indeterminate

        private void Fr_box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Load)
            {
                var fritems = new String[] { "1", "2", "3", "15", "30", "45", "60", "75", "90", "120", "144", "240" };
                Write_Ini(App.dConfig, "Config", "FPS", fritems[fr_box.SelectedIndex]);
                App.Notify(new(HText.Get_Translate("restart"), 2, HText.Get_Translate("frame")));
            }
        }

        private void Rbtn22_Checked(object sender, RoutedEventArgs e)
        {
            Write_Ini(App.dConfig, "Config", "Toast", "5");
        }

        private void video_btn_Checked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                rbtn15.IsEnabled = false;
                rbtn14.IsEnabled = false;
                acrylic_btn.IsEnabled = false;
                video_btn.IsEnabled = false;
                blureff.IsEnabled = false;
                btn10.IsEnabled = false;
                Write_Ini(App.dConfig, "Config", "Background", "4");
                Hiro_Main?.Blurbgi(0);
                rbtn15.IsEnabled = true;
                rbtn14.IsEnabled = true;
                acrylic_btn.IsEnabled = true;
                video_btn.IsEnabled = true;
                btn10.IsEnabled = true;
            }
            bg_slider.IsEnabled = false;
        }
    }
}
