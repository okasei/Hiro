using Hiro.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static Hiro.Helpers.Hiro_Settings;

namespace Hiro
{
    /// <summary>
    /// Hiro_Login.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Login : Page
    {
        private Hiro_MainUI? Hiro_Main = null;
        public Hiro_Login(Hiro_MainUI? Parent)
        {
            InitializeComponent();
            Hiro_Main = Parent;
            Loaded += delegate
            {
                HiHiro();
                Name_Textbox.Text = Read_DCIni("User", string.Empty);
            };
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        public void Load_Translate()
        {
            Login_Title.Content = Hiro_Text.Get_Translate("lgtitle");
            Name_Label.Content = Hiro_Text.Get_Translate("lgid");
            Pwd_Label.Content = Hiro_Text.Get_Translate("lgpwd");
            Create_Account.Content = Hiro_Text.Get_Translate("lgnew");
            Login_Btn.Content = Hiro_Text.Get_Translate("lgbtn");
            Auto_Login.Content = Hiro_Text.Get_Translate("lgauto");
        }

        public void Load_Position()
        {
            Hiro_Utils.Set_Control_Location(Login_Title, "lgtitle");
            Hiro_Utils.Set_Control_Location(Name_Label, "lgid");
            Hiro_Utils.Set_Control_Location(Name_Textbox, "lgidtb");
            Hiro_Utils.Set_Control_Location(Pwd_Label, "lgpwd");
            Hiro_Utils.Set_Control_Location(Pwd_Textbox, "lgpwdtb");
            Hiro_Utils.Set_Control_Location(Create_Account, "lgnew");
            Hiro_Utils.Set_Control_Location(Login_Btn, "lgbtn");
            Hiro_Utils.Set_Control_Location(Auto_Login, "lgauto");
        }

        public void HiHiro()
        {
            var animation = !Read_DCIni("Ani", "2").Equals("0");
            Storyboard sb = new();
            if (Read_DCIni("Ani", "2").Equals("1"))
            {
                Hiro_Utils.AddPowerAnimation(0, Login_Title, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(0, Pwd_Label, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(0, Name_Label, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(0, Login_Btn, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(0, Auto_Login, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(0, Create_Account, sb, 50, null);
            }
            if (animation)
            {
                Hiro_Utils.AddPowerAnimation(0, this, sb, 50, null);
                sb.Begin();
            }
            Load_Translate();
            Load_Position();
            Load_Color();
        }

        private void Create_Account_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.RunExe($"https://id.rexio.cn/register/local.php?lang={App.lang}", Hiro_Text.Get_Translate("login"));
        }

        private void Login_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (Name_Textbox.Text.Equals(string.Empty) || Pwd_Textbox.Password.Equals(string.Empty))
                return;
            if (App.Logined == null)
            {
                Hiro_Utils.RunExe($"notify({Hiro_Text.Get_Translate("lgging")},2)", Hiro_Text.Get_Translate("login"));
                return;
            }
            if (App.Logined == true)
            {
                Hiro_Main?.Set_Label(Hiro_Main.profilex);
                return;
            }
            App.Logined = null;
            Login_Btn.IsEnabled = false;
            Name_Textbox.IsEnabled = false;
            Pwd_Textbox.IsEnabled = false;
            Auto_Login.IsEnabled = false;
            var user = Name_Textbox.Text;
            var pwd = Pwd_Textbox.Password;
            new System.Threading.Thread(() =>
            {
                var tmp = System.IO.Path.GetTempFileName();
                try
                {
                    var res = Hiro_ID.Login(user, pwd, false, tmp);
                    if (res.Equals("success") && Read_Ini(tmp, "Login", "res", "-1").Equals("0"))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if (Auto_Login.IsChecked == true)
                                Write_Ini(App.dConfig, "Config", "AutoLogin", "1");
                            else
                                Write_Ini(App.dConfig, "Config", "AutoLogin", "0");
                        });
                        Write_Ini(App.dConfig, "Config", "Token", Read_Ini(tmp, "Login", "msg", string.Empty));
                        Write_Ini(App.dConfig, "Config", "User", Read_Ini(tmp, "Login", "usr", string.Empty));
                        App.loginedUser = Read_Ini(tmp, "Login", "usr", string.Empty);
                        App.loginedToken = Read_Ini(tmp, "Login", "msg", string.Empty);
                        App.Logined = true;
                        Hiro_ID.SyncProfile(App.loginedUser, App.loginedToken);
                        Dispatcher.Invoke(() =>
                        {
                            if (Hiro_Main != null)
                            {
                                if (Hiro_Main.hiro_profile != null)
                                {
                                    Hiro_Main.hiro_profile.Load_Data();
                                    Hiro_Main.hiro_profile.Load_Color();
                                }
                                Hiro_Main.Set_Label(Hiro_Main.profilex);
                                if (Hiro_Main.hiro_profile != null)
                                    Hiro_Main.hiro_profile.Profile_Mac.Content = user;
                                if (Hiro_Main.hiro_chat != null)
                                    Hiro_Main.hiro_chat.LocalId = user;
                            }
                            
                        });
                        App.Logined = true;
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            Hiro_Utils.RunExe($"notify({Read_Ini(tmp, "Login", "msg", Hiro_Text.Get_Translate("lgfailed"))},2)", Hiro_Text.Get_Translate("login"));
                        });
                        App.Logined = false;
                    }
                    Dispatcher.Invoke(() =>
                    {
                        Login_Btn.IsEnabled = true;
                        Name_Textbox.IsEnabled = true;
                        Pwd_Textbox.IsEnabled = true;
                        Auto_Login.IsEnabled = true;
                    });
                    if (System.IO.File.Exists(tmp))
                        System.IO.File.Delete(tmp);
                }
                catch(Exception ex)
                {
                    Hiro_Logger.LogError(ex, "Hiro.Exception.Login.Thread");
                    Dispatcher.Invoke(() =>
                    {
                        Login_Btn.IsEnabled = true;
                        Name_Textbox.IsEnabled = true;
                        Pwd_Textbox.IsEnabled = true;
                        Auto_Login.IsEnabled = true;
                        Hiro_Utils.RunExe($"notify({Hiro_Text.Get_Translate("lgfailed")},2)", Hiro_Text.Get_Translate("login"));
                    });
                    if (System.IO.File.Exists(tmp))
                        System.IO.File.Delete(tmp);
                    App.Logined = false;
                }
            }).Start();
        }

        private void Name_Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Name_Label.Visibility = Name_Textbox.Text.Equals(string.Empty) ? Visibility.Visible : Visibility.Hidden;
        }

        private void Pwd_Textbox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Pwd_Label.Visibility = Pwd_Textbox.Password.Equals(string.Empty) ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
