using Hiro.Helpers;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Hiro
{
    /// <summary>
    /// Hiro_Internet.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Proxy : Page
    {
        private Hiro_MainUI? Hiro_Main = null;
        public Hiro_Proxy(Hiro_MainUI? Parent)
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
            var animation = !HSet.Read_DCIni("Ani", "2").Equals("0");
            Storyboard sb = new();
            if (HSet.Read_DCIni("Ani", "2").Equals("1"))
            {
                HAnimation.AddPowerAnimation(1, Internet_Title, sb, -50, null);
                HAnimation.AddPowerAnimation(3, IBtn_1, sb, -50, null);
                HAnimation.AddPowerAnimation(3, IBtn_2, sb, -50, null);
            }
            if (animation)
            {
                HAnimation.AddPowerAnimation(0, this, sb, 50, null);
                sb.Begin();
            }
            EnableProxy.IsChecked = HSet.Read_Ini(App.dConfig, "Network", "Proxy", "0") switch
            {
                "1" => null,
                "2" => true,
                _ => false
            };
            AddressBox.Text = HSet.Read_Ini(App.dConfig, "Network", "Server", string.Empty);
            PortBox.Text = HSet.Read_Ini(App.dConfig, "Network", "Port", string.Empty);
            UsernameBox.Text = HSet.Read_Ini(App.dConfig, "Network", "Username", string.Empty);
            PwdBox.Password = HSet.Read_Ini(App.dConfig, "Network", "Password", string.Empty);
        }

        private void Hiro_Initialize()
        {
            Load_Color();
            Load_Translate();
            Load_Position();
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        public void Load_Translate()
        {
            Internet_Title.Content = HText.Get_Translate("proxytitle");
            EnableProxy.Content = EnableProxy.IsChecked == true ? HText.Get_Translate("proxyenable") : EnableProxy.IsChecked == null ? HText.Get_Translate("proxyie") : HText.Get_Translate("proxydisable");
            ProxyAddress.Content = HText.Get_Translate("proxyserver");
            ProxyPort.Content = HText.Get_Translate("proxyport");
            ProxyUsername.Content = HText.Get_Translate("proxyuser");
            ProxyPwd.Content = HText.Get_Translate("proxypwd");
            IBtn_1.Content = HText.Get_Translate("proxyok");
            IBtn_2.Content = HText.Get_Translate("proxycancel");
        }

        public void Load_Position()
        {
            HUI.Set_Control_Location(Internet_Title, "proxytitle");
            HUI.Set_Control_Location(EnableProxy, "proxyenable");
            HUI.Set_Control_Location(ProxyAddress, "proxyserver");
            HUI.Set_Control_Location(ProxyPort, "proxyport");
            HUI.Set_Control_Location(ProxyUsername, "proxyuser");
            HUI.Set_Control_Location(ProxyPwd, "proxypwd");
            HUI.Set_Control_Location(AddressBox, "proxyservertb");
            HUI.Set_Control_Location(PortBox, "proxyporttb");
            HUI.Set_Control_Location(UsernameBox, "proxyusertb");
            HUI.Set_Control_Location(PwdBox, "proxypwdtb");
            HUI.Set_Control_Location(IBtn_1, "proxyok", right: true, bottom: true);
            HUI.Set_Control_Location(IBtn_2, "proxycancel", right: true, bottom: true);
        }

        private void EnableProxy_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            EnableProxy.Content = HText.Get_Translate("proxyenable");
            AddressBox.IsEnabled = true;
            PortBox.IsEnabled = true;
            UsernameBox.IsEnabled = true;
            PwdBox.IsEnabled = true;
        }

        private void EnableProxy_Indeterminate(object sender, System.Windows.RoutedEventArgs e)
        {
            EnableProxy.Content = HText.Get_Translate("proxyie");
            AddressBox.IsEnabled = false;
            PortBox.IsEnabled = false;
            UsernameBox.IsEnabled = false;
            PwdBox.IsEnabled = false;
        }

        private void EnableProxy_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            EnableProxy.Content = HText.Get_Translate("proxydisable");
            AddressBox.IsEnabled = false;
            PortBox.IsEnabled = false;
            UsernameBox.IsEnabled = false;
            PwdBox.IsEnabled = false;
        }

        private void IBtn_1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var proxy = EnableProxy.IsChecked == true ? "2" : EnableProxy.IsChecked == false ? "0" : "1";
            HSet.Write_Ini(App.dConfig, "Network", "Proxy", proxy);
            HSet.Write_Ini(App.dConfig, "Network", "Server", AddressBox.Text);
            HSet.Write_Ini(App.dConfig, "Network", "Port", PortBox.Text);
            HSet.Write_Ini(App.dConfig, "Network", "Username", AddressBox.Text);
            HSet.Write_Ini(App.dConfig, "Network", "Username", UsernameBox.Text);
            HSet.Write_Ini(App.dConfig, "Network", "Password", PwdBox.Password);
            App.Notify(new(HText.Get_Translate("restart"), 2, HText.Get_Translate("proxyer")));
            Hiro_Main?.Set_Label(Hiro_Main.configx);
        }

        private void IBtn_2_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Hiro_Main?.Set_Label(Hiro_Main.configx);
        }
    }
}
