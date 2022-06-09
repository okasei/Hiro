using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
{
    /// <summary>
    /// Hiro_Chat.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Chat : Page
    {
        private Hiro_MainUI? Hiro_Main = null;
        private string MacAddress = "D03C1F3C2094";
        private string LocalMacAddress = "unknown";
        private string Aite = "unknown-D03C1F3C2094";
        public Hiro_Chat(Hiro_MainUI? Parent)
        {
            InitializeComponent();
            Hiro_Main = Parent;
            Hiro_Initialize();
            LocalMacAddress = GetMacAddress();
            MacAddress = Hiro_Utils.Read_Ini(App.dconfig, "Config", "ChatMAC", "D03C1F3C2094");
            Hiro_Utils.LogtoFile("[INFO]Current Mac Address : " + LocalMacAddress);
            Loaded += delegate
            {
                HiHiro();
            };
        }

        public string GetMacAddress()
        {
            try
            {
                var res = (from nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                        where nic.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up
                        select nic.GetPhysicalAddress().ToString()
                        ).FirstOrDefault();
                return res == null ? "unknown" : res;
            }
            catch
            {
                return "unknown";
            }
        }

        public void HiHiro()
        {
            var animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            Storyboard sb = new();
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                Hiro_Utils.AddPowerAnimation(1, ChatContent, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(1, SendContent, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(3, SendButton, sb, -50, null);
            }
            if (animation)
            {
                Hiro_Utils.AddPowerAnimation(0, this, sb, 50, null);
                sb.Begin();
                sb.Completed += delegate
                {
                    Hiro_Chat_Initialize();
                };
            }
            else
            {
                Hiro_Chat_Initialize();
            }
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

        private void Hiro_Chat_Initialize()
        {
            var folder = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\");
            Hiro_Utils.CreateFolder(folder);
            System.Threading.Thread th = new(() =>
            {
                Hiro_Get_Chat();
            });
            th.Start();
        }

        private void Hiro_Get_Chat()
        {
            Aite = Hiro_Utils.GetWebContent("https://api.rexio.cn/v2/hiro/chat/query.php?mac=" + MacAddress + "&mode=query");
            var content = Hiro_Utils.GetWebContent("https://api.rexio.cn/v2/hiro/chat/log.php?from=" + LocalMacAddress + "&to=" + MacAddress,
                true, Hiro_Utils.Path_Prepare("<hiapp>\\chat\\" + MacAddress + ".hcf"));
            Dispatcher.Invoke(() =>
            {
                Load_Chat();
            });
        }

        private void Load_Chat()
        {
            var file = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\") + MacAddress + ".hcf";
            ChatContent.Clear();
            var con = System.IO.File.ReadAllLines(file);
            foreach (var item in con)
            {
                var icon = item.Split(',');
                if (icon.Length < 3)
                    continue;
                Dispatcher.Invoke(() =>
                {
                    ChatContent.AppendText(icon[0].Replace(LocalMacAddress, App.Username).Replace(MacAddress, Aite) + " " + icon[1] + Environment.NewLine);
                    ChatContent.AppendText(icon[2].Replace("\\\\", "<hisplash>").Replace("\\n", Environment.NewLine).Replace("<hisplash>", "\\\\"));
                });
                for (int i = 3; i < icon.Length; i++)
                {
                    Dispatcher.Invoke(() =>
                    {
                        ChatContent.AppendText("," + icon[i].Replace("\\\\", "<hisplash>").Replace("\\n", Environment.NewLine).Replace("<hisplash>", "\\\\"));
                    });
                }
                Dispatcher.Invoke(() =>
                {
                    ChatContent.AppendText(Environment.NewLine);
                });
            }
        }

        private void Hiro_Send_Chat(string msg)
        {
            var content = Hiro_Utils.GetWebContent("https://api.rexio.cn/v2/hiro/chat/send.php?from=" + LocalMacAddress
                + "&to=" + MacAddress
                + "&content=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                + "," + msg);
            if(!content.Equals("success"))
            {
                Dispatcher.Invoke(() =>
                {
                    Hiro_Utils.RunExe("notify(" + Hiro_Utils.Get_Transalte("chatsenderror") + ",2)", Hiro_Utils.Get_Transalte("chat"));
                });
            }
        }
        private void Hiro_Update_Name()
        {
            Hiro_Utils.GetWebContent("https://api.rexio.cn/v2/hiro/chat/query.php?mac=" + LocalMacAddress
                    + "&mode=update"
                    + "&name=" + App.Username); ;
        }
        public void Load_Translate()
        {
            SendButton.Content = Hiro_Utils.Get_Transalte("chatsend");
        }

        public void Load_Position()
        {
            Hiro_Utils.Set_Control_Location(ChatContent, "chatcontent");
            Hiro_Utils.Set_Control_Location(SendContent, "chatsendcontent");
            Hiro_Utils.Set_Control_Location(SendButton, "chatsend", right: true, bottom: true);
        }

        private void SendButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Send();
        }

        private void Send()
        {
            if (SendContent.Text.ToLower().Equals("refreash"))
            {
                SendContent.Clear();
                Hiro_Get_Chat();
                return;
            }
            if (SendContent.Text.ToLower().StartsWith("talkto:"))
            {
                MacAddress = SendContent.Text.Substring(7);
                Hiro_Utils.LogtoFile(MacAddress);
                SendContent.Clear();
                Hiro_Get_Chat();
                return;
            }
            if (!SendContent.Text.Equals(string.Empty))
            {
                SendButton.IsEnabled = false;
                var tmp = SendContent.Text.Replace(Environment.NewLine, "\\n");
                new System.Threading.Thread(() =>
                {
                    Hiro_Send_Chat(tmp);
                    Hiro_Update_Name();
                    Hiro_Get_Chat();
                }).Start();
                SendContent.Clear();
                SendButton.IsEnabled = true;
            }
        }
        private void SendButton_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyStates == Keyboard.GetKeyStates(Key.R) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Send();
                e.Handled = true;
            }
        }
    }
}
