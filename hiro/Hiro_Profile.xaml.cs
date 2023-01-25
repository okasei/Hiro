using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace hiro
{
    /// <summary>
    /// Hiro_Profile.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Profile : Page
    {
        internal BackgroundWorker? whatsbw = null;
        private Hiro_MainUI? Hiro_Main = null;
        private bool Load = false;
        private bool xflag = false;
        public Hiro_Profile(Hiro_MainUI? Parent)
        {
            InitializeComponent();
            Hiro_Main = Parent;
            Hiro_Initialize();
            Loaded += delegate
            {
                SetAvatar(Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(App.dconfig, "Config", "UserAvatar", ""))), false);
                SetBackground(Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(App.dconfig, "Config", "UserBackground", ""))), false);
                HiHiro();
            };
        }

        public void HiHiro()
        {
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            Storyboard sb = new();
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                Hiro_Utils.AddPowerAnimation(0, this, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(0, BaseGrid, sb, -100, null);
                Hiro_Utils.AddPowerAnimation(1, btn10, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(1, btn11, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(1, btn9, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(1, btn8, sb, 50, null);
            }
            if (animation)
            {
                Hiro_Utils.AddPowerAnimation(0, this, sb, 50, null);
                sb.Begin();
            }
        }
        internal void Hiro_Initialize()
        {
            Profile_Background.Background = new ImageBrush()
            {
                Stretch = Stretch.UniformToFill,
                ImageSource = null
            };
            Resources["Avatar"] = new ImageBrush()
            {
                Stretch = Stretch.UniformToFill,
                ImageSource = null
            };
            Load_Color();
            Load_Data();
            Load_Translate();
            Load_Position();
        }

        public void Load_Color()
        {
            var sign = Profile_Signature.Foreground == (SolidColorBrush)Resources["AppFore"];
            var nick = Profile_Nickname.Foreground == (SolidColorBrush)Resources["AppFore"];
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDisabled"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 180));
            Resources["AppForeDim"] = Hiro_Utils.Color_Transparent(App.AppForeColor, 80);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
            Profile_Signature.Foreground = sign ? (SolidColorBrush)Resources["AppFore"] : (SolidColorBrush)Resources["AppForeDisabled"];
            Profile_Nickname.Foreground = nick ? (SolidColorBrush)Resources["AppFore"] : (SolidColorBrush)Resources["AppForeDisabled"];
            Hiro_Utils.Set_Opacity(Profile_Background, BackControl);
        }

        public void Load_Data()
        {
            msg_level.Value = Convert.ToInt32(double.Parse(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Message", "3")));
            disturb_level.Value = Convert.ToInt32(double.Parse(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Disturb", "2")));
            rbtn17.IsChecked = Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomNick", "1").Equals("2");
            rbtn16.IsChecked = !rbtn17.IsChecked;
            if (rbtn17.IsChecked == true)
            {
                App.AppTitle = Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomHIRO", "Hiro");
                if (App.wnd != null)
                    App.wnd.Hiro_Tray.ToolTipText = Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomHIRO", "Hiro");
            }
            msg_audio.IsChecked = Hiro_Utils.Read_Ini(App.dconfig, "Config", "MessageAudio", "1").Equals("1");
            msg_auto.IsChecked = Hiro_Utils.Read_Ini(App.dconfig, "Config", "AutoChat", "1").Equals("1");
            tb10.Text = Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomHIRO", "");
            var str = Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomSign", string.Empty);
            if (str.Trim().Equals(string.Empty))
            {
                Profile_Signature.Content = Hiro_Utils.Get_Translate("profilesign");
                Profile_Signature.Foreground = (SolidColorBrush)Resources["AppForeDisabled"];
            }
            else
            {
                Profile_Signature.Content = str;
                Profile_Signature.Foreground = (SolidColorBrush)Resources["AppFore"];
            }
            str = Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomName", string.Empty);
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomUser", "1").Equals("1") || str.Trim().Equals(string.Empty))
            {
                Profile_Nickname.Content = App.Username;
                Profile_Nickname.Foreground = (SolidColorBrush)Resources["AppForeDisabled"];
            }
            else
            {
                Profile_Nickname.Content = str;
                Profile_Nickname.Foreground = (SolidColorBrush)Resources["AppFore"];
            }
            switch (Hiro_Utils.Read_Ini(App.dconfig, "Config", "UserAvatarStyle", "1"))
            {
                case "0":
                    Profile_Rectangle.Visibility = Visibility.Hidden;
                    Profile_Ellipse.Visibility = Visibility.Visible;
                    break;
                default:
                    Profile_Rectangle.Visibility = Visibility.Visible;
                    Profile_Ellipse.Visibility = Visibility.Hidden;
                    break;
            }
            var mac = Hiro_Utils.Read_Ini(App.dconfig, "Config", "User", Hiro_Utils.Get_Translate("idnull"));
            Profile_Mac.Content = mac;
            Load = true;
        }

        public void Load_Translate()
        {
            btn8.Content = Hiro_Utils.Get_Translate("feedback");
            btn9.Content = Hiro_Utils.Get_Translate("whatsnew");
            btn10.Content = Hiro_Utils.Get_Translate("lgout");
            btn11.Content = Hiro_Utils.Get_Translate("document");
            name_label.Content = Hiro_Utils.Get_Translate("namelabel");
            rbtn16.Content = Hiro_Utils.Get_Translate("namehiro");
            rbtn17.Content = Hiro_Utils.Get_Translate("namecus");
            msg_label.Content = Hiro_Utils.Get_Translate("msglabel");
            msg_audio.Content = Hiro_Utils.Get_Translate("msgaudio");
            msg_auto.Content = Hiro_Utils.Get_Translate("msgauto");
            msg_status.Content = Convert.ToInt32(msg_level.Value) switch
            {
                1 => Hiro_Utils.Get_Translate("msghide"),
                2 => Hiro_Utils.Get_Translate("msglock"),
                3 => Hiro_Utils.Get_Translate("msgalways"),
                _ => Hiro_Utils.Get_Translate("msgnever")
            };
            disturb_label.Content = Hiro_Utils.Get_Translate("disturblabel");
            disturb_status.Content = Convert.ToInt32(disturb_level.Value) switch
            {
                1 => Hiro_Utils.Get_Translate("disturbfs"),
                2 => Hiro_Utils.Get_Translate("disturbok"),
                _ => Hiro_Utils.Get_Translate("disturbno")
            };
            if (Profile_Signature.Foreground == (SolidColorBrush)Resources["AppForeDisabled"])
                Profile_Signature.Content = Hiro_Utils.Get_Translate("profilesign");
        }

        public void Load_Position()
        {
            
            Hiro_Utils.Set_Control_Location(Profile_Nickname_Indexer, "profilename");
            Hiro_Utils.Set_Control_Location(Profile_Signature_Indexer, "profilesign");
            Hiro_Utils.Set_Control_Location(Profile_Mac, "profileid");
            Hiro_Utils.Set_Control_Location(Profile_Background, "profileback");
            Hiro_Utils.Set_Control_Location(btn8, "feedback");
            Hiro_Utils.Set_Control_Location(btn9, "whatsnew");
            Hiro_Utils.Set_Control_Location(btn10, "lgout");
            Hiro_Utils.Set_Control_Location(btn11, "document");
            Hiro_Utils.Set_Control_Location(tb10, "hirotb");
            Hiro_Utils.Set_Control_Location(name_label, "namelabel");
            Hiro_Utils.Set_Control_Location(rbtn16, "namehiro");
            Hiro_Utils.Set_Control_Location(rbtn17, "namecus");
            Hiro_Utils.Set_Control_Location(msg_label, "msglabel");
            Hiro_Utils.Set_Control_Location(msg_level, "msgslider");
            Hiro_Utils.Set_Control_Location(msg_status, "msgstatus");
            Hiro_Utils.Set_Control_Location(msg_audio, "msgaudio");
            Hiro_Utils.Set_Control_Location(msg_auto, "msgauto");
            Hiro_Utils.Set_Control_Location(disturb_label, "disturblabel");
            Hiro_Utils.Set_Control_Location(disturb_level, "disturbslider");
            Hiro_Utils.Set_Control_Location(disturb_status, "disturbstatus");
            Hiro_Utils.Set_FrameworkElement_Location(Profile_Ellipse, "profileavatar");
            Hiro_Utils.Set_FrameworkElement_Location(Profile_Rectangle, "profileavatar");
            Hiro_Utils.Set_FrameworkElement_Location(Profile, "profilegrid");
            Hiro_Utils.Set_FrameworkElement_Location(BaseGrid, "profileg");
            Hiro_Utils.Set_FrameworkElement_Location(name_grid, "nameg");
            Hiro_Utils.Set_FrameworkElement_Location(msg_grid, "msgg");
            Hiro_Utils.Set_FrameworkElement_Location(disturb_grid, "disturbg");
            Thickness thickness = BaseGrid.Margin;
            thickness.Top = 0.0;
            BaseGrid.Margin = thickness;
            configbar.Maximum = BaseGrid.Height - 420 > 0 ? BaseGrid.Height - 420 : 0;
            configbar.Visibility = configbar.Maximum > 0 ? Visibility.Visible : Visibility.Hidden; 
            configbar.Value = 0.0;
            configbar.ViewportSize = 420;
            Profile_Mac.Margin = new Thickness(Profile_Nickname.Margin.Left + Profile_Nickname.ActualWidth + 5, Profile_Nickname.Margin.Top + Profile_Nickname.ActualHeight - Profile_Mac.ActualHeight, 0, 0);
        }

        private void Btn8_Click(object sender, RoutedEventArgs e)
        {
            btn8.IsEnabled = false;
            Hiro_Utils.RunExe("https://i.rexio.cn/hiro-fb",App.AppTitle);
            btn8.IsEnabled = true;
        }

        private void Btn9_Click(object sender, RoutedEventArgs e)
        {
            btn9.IsEnabled = false;
            if (whatsbw != null)
            {
                return;
            }
            whatsbw = new();
            var wps = "";
            whatsbw.DoWork += delegate
            {
                wps = Hiro_Utils.GetWebContent("https://hiro.rexio.cn/Update/new.php?ver=" + Hiro_Resources.ApplicationVersion + "&lang=" + App.lang);
            };
            whatsbw.RunWorkerCompleted += delegate
            {
                try
                {
                    var ti = wps[(wps.IndexOf("<title>") + "<title>".Length)..];
                    ti = ti[..ti.IndexOf("<")];
                    wps = wps[(wps.IndexOf("</head>") + "</head>".Length)..];
                    Hiro_Utils.RunExe("alarm(" + ti + "," + wps.Replace("<br>", "\\n") + ")");
                    whatsbw.Dispose();
                    whatsbw = null;
                }
                catch (Exception ex)
                {
                    Hiro_Utils.LogError(ex, "Hiro.Exception.Update.Parse");
                }

            };
            whatsbw.RunWorkerAsync();
            Hiro_Utils.Delay(200);
            btn9.IsEnabled = true;
        }

        private void Rbtn16_Checked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "CustomNick", "1");
            tb10.IsEnabled = false;
            App.AppTitle = Hiro_Resources.ApplicationName;
            Title = App.AppTitle + " - " + Hiro_Utils.Get_Translate("version").Replace("%c", Hiro_Resources.ApplicationVersion);
            if (Hiro_Main != null)
            {
                Hiro_Main.titlelabel.Content = App.AppTitle;
                Hiro_Main.Title = App.AppTitle + " - " + Hiro_Utils.Get_Translate("version").Replace("%c", Hiro_Resources.ApplicationVersion);
            }
            if (App.wnd != null)
                App.wnd.Hiro_Tray.ToolTipText = App.AppTitle;
        }
        private void Rbtn17_Checked(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "CustomNick", "2");
            tb10.IsEnabled = true;
            App.AppTitle = tb10.Text;
            Title = App.AppTitle + " - " + Hiro_Utils.Get_Translate("version").Replace("%c", Hiro_Resources.ApplicationVersion);
            if (Hiro_Main != null)
            {
                Hiro_Main.titlelabel.Content = App.AppTitle;
                Hiro_Main.Title = App.AppTitle + " - " + Hiro_Utils.Get_Translate("version").Replace("%c", Hiro_Resources.ApplicationVersion);
            }
            if (App.wnd != null)
                App.wnd.Hiro_Tray.ToolTipText = tb10.Text;
        }

        private void Tb10_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomNick", "1").Equals("2"))
            {
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "CustomHIRO", tb10.Text);
                App.AppTitle = tb10.Text;
                Title = App.AppTitle + " - " + Hiro_Utils.Get_Translate("version").Replace("%c", Hiro_Resources.ApplicationVersion);
                if (Hiro_Main != null)
                {
                    Hiro_Main.titlelabel.Content = App.AppTitle;
                    Hiro_Main.Title = App.AppTitle + " - " + Hiro_Utils.Get_Translate("version").Replace("%c", Hiro_Resources.ApplicationVersion);
                }
                if (App.wnd != null)
                    App.wnd.Hiro_Tray.ToolTipText = tb10.Text;
            }

        }

        private void Msg_level_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Load)
            {
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "Message", (Convert.ToInt32(msg_level.Value)).ToString());
                msg_status.Content = Convert.ToInt32(msg_level.Value) switch
                {
                    1 => Hiro_Utils.Get_Translate("msghide"),
                    2 => Hiro_Utils.Get_Translate("msglock"),
                    3 => Hiro_Utils.Get_Translate("msgalways"),
                    _ => Hiro_Utils.Get_Translate("msgnever")
                };
            }

        }

        private void Disturb_level_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Load)
            {
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "Disturb", (Convert.ToInt32(disturb_level.Value)).ToString());
                disturb_status.Content = Convert.ToInt32(disturb_level.Value) switch
                {
                    1 => Hiro_Utils.Get_Translate("disturbfs"),
                    2 => Hiro_Utils.Get_Translate("disturbok"),
                    _ => Hiro_Utils.Get_Translate("disturbno")
                };
            }
        }

        private void Configbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var thickness = BaseGrid.Margin;
            thickness.Top = -configbar.Value;
            BaseGrid.Margin = thickness;
        }

        private void Configbar_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            configbar.Value += e.Delta * (configbar.Maximum - configbar.ViewportSize) / configbar.ViewportSize;
        }

        private void Msg_audio_Checked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "MessageAudio", "1");
            }
        }

        private void Msg_audio_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "MessageAudio", "0");
            }
        }

        private void Msg_auto_Checked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "AutoChat", "1");
                if (Hiro_Main != null)
                    Hiro_Main.hiro_chat ??= new(Hiro_Main);
            }
        }

        private void Msg_auto_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Load)
            {
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "AutoChat", "0");
            }
        }

        private void Profile_Nickname_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Profile_Nickname_Textbox.Text = Profile_Nickname.Foreground == (SolidColorBrush)Resources["AppForeDisabled"] ? string.Empty : (string)Profile_Nickname.Content;
            Profile_Nickname.Visibility = Visibility.Hidden;
            Profile_Nickname_Textbox.Visibility = Visibility.Visible;
            Profile_Nickname_Textbox.Focus();
        }

        private void Profile_Signature_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Profile_Signature_Textbox.Text = Profile_Signature.Foreground == (SolidColorBrush)Resources["AppForeDisabled"] ? string.Empty : (string)Profile_Signature.Content;
            Profile_Signature.Visibility = Visibility.Hidden;
            Profile_Signature_Textbox.Visibility = Visibility.Visible;
            Profile_Signature_Textbox.Focus();
        }

        private void Update_Profile()
        {
            new System.Threading.Thread(() =>
            {
                try
                {
                    var res = Hiro_Utils.UploadProfileSettings(
                    App.LoginedUser, App.LoginedToken, App.Username,
                    Hiro_Utils.Read_Ini(App.dconfig, "Config", "CustomSign", string.Empty),
                    Hiro_Utils.Read_Ini(App.dconfig, "Config", "UserAvatarStyle", "1"),
                    Hiro_Utils.GetMD5(Hiro_Utils.Read_Ini(App.dconfig, "Config", "UserBackground", "")),
                    Hiro_Utils.GetMD5(Hiro_Utils.Read_Ini(App.dconfig, "Config", "UserAvatar", "")),
                    "update"
                    );
                    if (!res.Equals("success"))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            Hiro_Utils.RunExe("notify(" + Hiro_Utils.Get_Translate("chatdeve") + ",2)", Hiro_Utils.Get_Translate("profile"));
                        });
                    }
                }
                catch (Exception ex)
                {
                    Hiro_Utils.LogError(ex, "Hiro.Exception.Profile.Update.Nickname");
                }
            }).Start();
        }

        private void Profile_Nickname_Textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Profile_Nickname_Textbox.Text.Trim().Equals(string.Empty))
                {
                    Hiro_Utils.Write_Ini(App.dconfig, "Config", "CustomUser", "1");
                    App.CustomUsernameFlag = 0;
                    App.Username = App.EnvironmentUsername;
                    Profile_Nickname.Foreground = (SolidColorBrush)Resources["AppForeDisabled"];
                }
                else
                {
                    Hiro_Utils.Write_Ini(App.dconfig, "Config", "CustomUser", "2");
                    Hiro_Utils.Write_Ini(App.dconfig, "Config", "CustomName", Profile_Nickname_Textbox.Text);
                    App.CustomUsernameFlag = 1;
                    App.Username = Profile_Nickname_Textbox.Text;
                    Profile_Nickname.Foreground = (SolidColorBrush)Resources["AppFore"];
                }
                Update_Profile();
                Profile_Nickname.Content = App.Username;
                Profile_Nickname.Visibility = Visibility.Visible;
                Profile_Nickname_Textbox.Visibility = Visibility.Hidden;
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                Profile_Nickname.Visibility = Visibility.Visible;
                Profile_Nickname_Textbox.Visibility = Visibility.Hidden;
                e.Handled = true;
            }
        }

        private void Profile_Signature_Textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Profile_Signature_Textbox.Text = Profile_Signature_Textbox.Text.Trim().Equals(string.Empty) ? string.Empty : Profile_Signature_Textbox.Text;
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "CustomSign", Profile_Signature_Textbox.Text);
                Profile_Signature.Content = Profile_Signature_Textbox.Text.Equals(string.Empty) ? Hiro_Utils.Get_Translate("profilesign") : Profile_Signature_Textbox.Text;
                Profile_Signature.Foreground = Profile_Signature_Textbox.Text.Equals(string.Empty) ? (SolidColorBrush)Resources["AppForeDisabled"] : (SolidColorBrush)Resources["AppFore"];
                Profile_Signature.Visibility = Visibility.Visible;
                Profile_Signature_Textbox.Visibility = Visibility.Hidden;
                Update_Profile();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                Profile_Signature.Visibility = Visibility.Visible;
                Profile_Signature_Textbox.Visibility = Visibility.Hidden;
                e.Handled = true;
            }
        }

        private void Profile_Background_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == e.LeftButton)
            {
                string strFileName = "";
                Microsoft.Win32.OpenFileDialog ofd = new()
                {
                    Filter = Hiro_Utils.Get_Translate("picfiles") + "|*.jpg;*.jpeg;*.bmp;*.gif;*.png|" + Hiro_Utils.Get_Translate("allfiles") + "|*.*",
                    ValidateNames = true, // 验证用户输入是否是一个有效的Windows文件名
                    CheckFileExists = true, //验证路径的有效性
                    CheckPathExists = true,//验证路径的有效性
                    Title = Hiro_Utils.Get_Translate("openfile") + " - " + App.AppTitle
                };
                if (ofd.ShowDialog() == true) //用户点击确认按钮，发送确认消息
                {
                    strFileName = ofd.FileName;//获取在文件对话框中选定的路径或者字符串
                    SetBackground(strFileName);
                }
                e.Handled = true;
            }
        }

        private void Profile_Avatar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == e.LeftButton)
            {
                string strFileName = "";
                Microsoft.Win32.OpenFileDialog ofd = new()
                {
                    Filter = Hiro_Utils.Get_Translate("picfiles") + "|*.jpg;*.jpeg;*.bmp;*.gif;*.png|" + Hiro_Utils.Get_Translate("allfiles") + "|*.*",
                    ValidateNames = true, // 验证用户输入是否是一个有效的Windows文件名
                    CheckFileExists = true, //验证路径的有效性
                    CheckPathExists = true,//验证路径的有效性
                    Title = Hiro_Utils.Get_Translate("openfile") + " - " + App.AppTitle
                };
                if (ofd.ShowDialog() == true) //用户点击确认按钮，发送确认消息
                {
                    strFileName = ofd.FileName;//获取在文件对话框中选定的路径或者字符串
                    SetAvatar(strFileName);
                }
                e.Handled = true;
            }
            else if (e.ButtonState == e.RightButton)
            {
                switch (Hiro_Utils.Read_Ini(App.dconfig, "Config", "UserAvatarStyle", "1"))
                {
                    case "0":
                        Profile_Rectangle.Visibility = Visibility.Visible;
                        Profile_Rectangle.Fill = (ImageBrush)Resources["Avatar"];
                        Profile_Ellipse.Visibility = Visibility.Hidden;
                        Profile_Ellipse.Fill = null;
                        Hiro_Utils.Write_Ini(App.dconfig, "Config", "UserAvatarStyle", "1");
                        break;
                    default:
                        Profile_Rectangle.Visibility = Visibility.Hidden;
                        Profile_Rectangle.Fill = null;
                        Profile_Ellipse.Visibility = Visibility.Visible;
                        Profile_Ellipse.Fill = (ImageBrush)Resources["Avatar"];
                        Hiro_Utils.Write_Ini(App.dconfig, "Config", "UserAvatarStyle", "0");
                        break;
                }
                Update_Profile();
                e.Handled = true;
            }
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public static ImageSource ImageSourceFromBitmap(System.Drawing.Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        private void Profile_Background_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data;
            var formats = data.GetData(DataFormats.FileDrop).GetType().ToString();
            if (formats.Equals("System.String[]"))
            {
                var info = e.Data.GetData(DataFormats.FileDrop) as String[];
                if (info != null && info.Length > 0)
                    SetBackground(info[0]);
            }
            else if (formats.Equals("System.String"))
            {
                var info = e.Data.GetData(DataFormats.FileDrop) as String;
                if (info != null)
                    SetBackground(info);
            }
        }

        private void Profile_Avatar_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data;
            var formats = data.GetData(DataFormats.FileDrop).GetType().ToString();
            if (formats.Equals("System.String[]"))
            {
                var info = e.Data.GetData(DataFormats.FileDrop) as String[];
                if (info != null && info.Length > 0)
                    SetAvatar(info[0]);
            }
            else if (formats.Equals("System.String"))
            {
                var info = e.Data.GetData(DataFormats.FileDrop) as String;
                if (info != null)
                    SetAvatar(info);
            }
        }

        private void SetAvatar(string strFileName, bool append = true)
        {
            new System.Threading.Thread(() =>
            {
                try
                {
                    System.Drawing.Image img = System.Drawing.Image.FromFile(@strFileName);
                    double w = img.Width;
                    double h = img.Height;
                    double ww = 60 * 2;
                    double hh = 60 * 2;
                    Dispatcher.Invoke(() =>
                    {
                        ww = Profile_Rectangle.Width * 2;
                        hh = Profile_Rectangle.Height * 2;
                    });
                    bool m = false;
                    if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Compression", "1").Equals("1"))
                    {
                        if (ww < w && hh < h)
                        {
                            var r = 0;
                            while (ww < w && hh < h)
                            {
                                w /= 2;
                                h /= 2;
                                r++;
                            }
                            w *= 2;
                            h *= 2;
                            if (r > 1)
                            {
                                img = Hiro_Utils.ZoomImage(img, Convert.ToInt32(h), Convert.ToInt32(w));
                                m = true;
                            }

                        }
                        long len = 0;
                        using (var stream = new System.IO.MemoryStream())
                        {
                            img.Save(stream, Hiro_Utils.GetImageFormat(img));
                            len = stream.Length;
                        }
                        if (len > 1024 * 1024)
                        {
                            img = Hiro_Utils.ZipImage(img, Hiro_Utils.GetImageFormat(img), 1024);
                            m = true;
                        }
                    }
                    if (m)
                    {
                        System.Drawing.Bitmap b = new(img);
                        img.Dispose();
                        strFileName = @"<hiapp>\images\avatar\" + strFileName[strFileName.LastIndexOf("\\")..];
                        strFileName = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(strFileName));
                        Hiro_Utils.CreateFolder(strFileName);
                        if (System.IO.File.Exists(strFileName))
                            System.IO.File.Delete(strFileName);
                        b.Save(strFileName);
                        b.Dispose();
                    }
                    else
                        img.Dispose();
                    Dispatcher.Invoke(() =>
                    {
                        BitmapImage bi = new();
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.UriSource = new Uri(@strFileName);
                        ImageBrush ib = new()
                        {
                            Stretch = Stretch.UniformToFill,
                            ImageSource = bi
                        };
                        Resources["Avatar"] = ib;
                        bi.EndInit();
                        bi.Freeze();
                    });
                    if (m || append)
                    {
                        var file = new string(strFileName);
                        new System.Threading.Thread(() =>
                        {
                            var trfile = "avatarsucc";
                            if (new System.IO.FileInfo(file).Length > 1024 * 1024)
                                trfile = "avatarlarge";
                            else
                            {
                                var res = Hiro_Utils.UploadProfileImage(file, App.LoginedUser, App.LoginedToken, "hap");
                                if (res.Equals("success"))
                                    trfile = "avatarsucc";
                                else
                                    trfile = "avatarfail";
                            }
                            Dispatcher.Invoke(() =>
                            {
                                Hiro_Utils.RunExe("notify(" + Hiro_Utils.Get_Translate(trfile) + ",2)", Hiro_Utils.Get_Translate("profile"));
                            });
                        }).Start();
                        strFileName = Hiro_Utils.Anti_Path_Prepare(strFileName).Replace("\\\\", "\\");
                        Hiro_Utils.Write_Ini(App.dconfig, "Config", "UserAvatar", strFileName);
                    }
                    
                }
                catch (Exception ex)
                {
                    Hiro_Utils.LogError(ex, "Hiro.Exception.Profile.Avatar");
                }
                finally
                {
                    Dispatcher.Invoke(() =>
                    {
                        Profile_Rectangle.Fill = Profile_Rectangle.Visibility == Visibility.Visible ? (ImageBrush)Resources["Avatar"] : null;
                        Profile_Ellipse.Fill = Profile_Ellipse.Visibility == Visibility.Visible ? (ImageBrush)Resources["Avatar"] : null;
                    });
                }
            }).Start();
        }

        private void SetBackground(string strFileName, bool append = true)
        {
            new System.Threading.Thread(() =>
            {
                try
                {
                    System.Drawing.Image img = System.Drawing.Image.FromFile(@strFileName);
                    double w = img.Width;
                    double h = img.Height;
                    double ww = 420 * 1.5;
                    double hh = 80 * 1.5;
                    Dispatcher.Invoke(() =>
                    {
                        ww = Profile_Background.Width * 1.5;
                        hh = Profile_Background.Height * 1.5;
                    });
                    bool m = false;
                    if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Compression", "1").Equals("1"))
                    {
                        if (ww < w && hh < h)
                        {
                            var r = 0;
                            while (ww < w && hh < h)
                            {
                                w /= 2;
                                h /= 2;
                                r++;
                            }
                            w *= 2;
                            h *= 2;
                            if (r > 1)
                            {
                                img = Hiro_Utils.ZoomImage(img, Convert.ToInt32(h), Convert.ToInt32(w));
                                m = true;
                            }
                        }
                        long len = 0;
                        using (var stream = new System.IO.MemoryStream())
                        {
                            img.Save(stream, Hiro_Utils.GetImageFormat(img));
                            len = stream.Length;
                        }
                        if (len > 2048 * 1024)
                        {
                            img = Hiro_Utils.ZipImage(img, Hiro_Utils.GetImageFormat(img), 2048);
                            m = true;
                        }
                    }
                    if (m)
                    {
                        System.Drawing.Bitmap b = new(img);
                        img.Dispose();
                        strFileName = @"<hiapp>\images\profile\" + strFileName[strFileName.LastIndexOf("\\")..];
                        strFileName = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(strFileName));
                        Hiro_Utils.CreateFolder(strFileName);
                        if (System.IO.File.Exists(strFileName))
                            System.IO.File.Delete(strFileName);
                        b.Save(strFileName);
                        b.Dispose();
                    }
                    else
                        img.Dispose();
                    Dispatcher.Invoke(() =>
                    {
                        BitmapImage bi = new();
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.UriSource = new Uri(@strFileName);
                        ImageBrush ib = new()
                        {
                            Stretch = Stretch.UniformToFill,
                            ImageSource = bi
                        };
                        Profile_Background.Background = ib;
                        bi.EndInit();
                        bi.Freeze();
                    });
                    if (m || append)
                    {
                        var file = new string(strFileName);
                        new System.Threading.Thread(() =>
                        {
                            var trfile = "profilesucc";
                            if (new System.IO.FileInfo(file).Length > 2048 * 1024)
                                trfile = "profilelarge";
                            else
                            {
                                var res = Hiro_Utils.UploadProfileImage(file, App.LoginedUser, App.LoginedToken, "hpp");
                                if (res.Equals("success"))
                                    trfile = "profilesucc";
                                else
                                    trfile = "profilefail";
                            }
                            Dispatcher.Invoke(() =>
                            {
                                Hiro_Utils.RunExe("notify(" + Hiro_Utils.Get_Translate(trfile) + ",2)", Hiro_Utils.Get_Translate("profile"));
                            });
                        }).Start();
                        strFileName = Hiro_Utils.Anti_Path_Prepare(strFileName).Replace("\\\\", "\\");
                        Hiro_Utils.Write_Ini(App.dconfig, "Config", "UserBackground", strFileName);
                    }
                    
                }
                catch (Exception ex)
                {
                    Hiro_Utils.LogError(ex, "Hiro.Exception.Profile.Background");
                }
            }).Start();
        }

        private void Profile_Nickname_Textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            Profile_Nickname.Visibility = Visibility.Visible;
            Profile_Nickname_Textbox.Visibility = Visibility.Hidden;
        }

        private void Profile_Signature_Textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            Profile_Signature.Visibility = Visibility.Visible;
            Profile_Signature_Textbox.Visibility = Visibility.Hidden;
        }

        private void Profile_Nickname_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            var tag = xflag ? "profileidx" : "profileid";
            Hiro_Utils.Set_Mac_Location(Profile_Mac, tag, Profile_Nickname, animation: animation, animationTime: 250);
        }

        private void Profile_Mac_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!Profile_Mac.Content.Equals(Hiro_Utils.Get_Translate("idnull")))
            {
                Clipboard.SetText(Profile_Mac.Content.ToString());
                Hiro_Utils.RunExe("notify(" + Hiro_Utils.Get_Translate("idcopy") + ",2)", Hiro_Utils.Get_Translate("profile"));
            }
        }

        private void Btn10_Click(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Logout();
            Hiro_Main?.Set_Label(Hiro_Main.homex);
        }

        private void Profile_MouseEnter(object sender, MouseEventArgs e)
        {
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            xflag = true;
            Hiro_Utils.Set_Control_Location(Profile_Nickname_Indexer, "profilenamex", animation: animation, animationTime: 250);
            Hiro_Utils.Set_Control_Location(Profile_Signature_Indexer, "profilesignx", animation: animation, animationTime: 250);
            Hiro_Utils.Set_Control_Location(Profile_Background, "profilebackx", animation: animation, animationTime: 250);
            Hiro_Utils.Set_FrameworkElement_Location(Profile_Ellipse, "profileavatarx", animation: animation, animationTime: 250);
            Hiro_Utils.Set_FrameworkElement_Location(Profile_Rectangle, "profileavatarx", animation: animation, animationTime: 250);
            Hiro_Utils.Set_FrameworkElement_Location(Profile, "profilegridx", animation: animation, animationTime: 250);
            e.Handled = true;
        }

        private void Profile_MouseLeave(object sender, MouseEventArgs e)
        {
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            xflag = false;
            Hiro_Utils.Set_Control_Location(Profile_Nickname_Indexer, "profilename", animation: animation, animationTime: 250);
            Hiro_Utils.Set_Control_Location(Profile_Signature_Indexer, "profilesign", animation: animation, animationTime: 250);
            Hiro_Utils.Set_Control_Location(Profile_Background, "profileback", animation: animation, animationTime: 250);
            Hiro_Utils.Set_FrameworkElement_Location(Profile_Ellipse, "profileavatar", animation: animation, animationTime: 250);
            Hiro_Utils.Set_FrameworkElement_Location(Profile_Rectangle, "profileavatar", animation: animation, animationTime: 250);
            Hiro_Utils.Set_FrameworkElement_Location(Profile, "profilegrid", animation: animation, animationTime: 250);
            e.Handled = true;
        }

        private void Btn11_Click(object sender, RoutedEventArgs e)
        {
            btn11.IsEnabled = false;
            Hiro_Utils.RunExe("https://i.rexio.cn/hiro-doc", App.AppTitle);
            btn11.IsEnabled = true;
        }
    }
}
