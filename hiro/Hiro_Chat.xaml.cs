using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
        private string Aite = "unknown-user";
        private bool load = false;
        private bool eload = false;
        public Hiro_Chat(Hiro_MainUI? Parent)
        {
            InitializeComponent();
            Hiro_Main = Parent;
            Hiro_Initialize();
            new System.Threading.Thread(() =>
            {
                LocalMacAddress = GetMacAddress();
                Hiro_Utils.LogtoFile("[INFO]Current Mac Address : " + LocalMacAddress);
            }).Start();
            MacAddress = Hiro_Utils.Read_Ini(App.dconfig, "Config", "ChatMAC", "D03C1F3C2094");
            Loaded += delegate
            {
                HiHiro();
            };
        }

        public string GetMacAddress()
        {
            return Hiro_Utils.GetMacByIpConfig() ?? "unknown";
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
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 180));
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
            Resources["AppAccentDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 180));
            if (load)
                Load_Chat();
        }

        public void Hiro_Chat_Initialize()
        {
            var folder = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\");
            Hiro_Utils.CreateFolder(folder);
            var file = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + MacAddress + ".hcf");
            ChatContentBak.Text = System.IO.File.Exists(file) ? System.IO.File.ReadAllText(file) : "";
            Load_Chat();
            load = true;
        }

        public void Hiro_Get_Chat()
        {
            new System.Threading.Thread(() =>
            {
                try
                {
                    Hiro_Utils.CreateFolder(Hiro_Utils.Path_Prepare("<hiapp>\\chat\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM-dd") + "\\"));
                    var aitea = Hiro_Utils.GetWebContent("https://api.rexio.cn/v2/hiro/chat/query.php?mac=" + MacAddress + "&mode=query");
                    if (!aitea.Equals(Aite))
                    {
                        Aite = aitea;
                        eload = false;
                    }
                    var content = Hiro_Utils.GetWebContent("https://api.rexio.cn/v2/hiro/chat/log.php?from=" + LocalMacAddress + "&to=" + MacAddress,
                        true, Hiro_Utils.Path_Prepare("<hiapp>\\chat\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + MacAddress + ".hcf"));
                    Dispatcher.Invoke(() =>
                    {
                        Load_Chat();
                    });
                }
                catch (Exception ex)
                {
                    Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.Chat.Update: Cannot fetch chatting data." + ex.Message);
                    Dispatcher.Invoke(() =>
                    {
                        AddErrorMsg(Hiro_Utils.Get_Transalte("chatsys"), Hiro_Utils.Get_Transalte("chatnofetch"));
                    });
                }
            }).Start();
        }

        private void AddErrorMsg(string user, string msg)
        {
            Paragraph nick = new();
            Run nrun = new()
            {
                Text = user + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine
            };
            Run wrun = new()
            {
                Text = msg.Replace("\\\\", "<hisplash>").Replace("\\n", Environment.NewLine).Replace("<hisplash>", "\\\\")
            };
            nrun.FontWeight = FontWeights.Bold;
            nrun.Foreground = new SolidColorBrush(Colors.Red);
            wrun.FontWeight = FontWeights.Normal;
            wrun.Foreground = nrun.Foreground;
            nick.Inlines.Add(nrun);
            nick.Inlines.Add(wrun);
            ChatContent.Document.Blocks.Add(nick);
            ChatContent.ScrollToEnd();
        }

        private void Load_Chat()
        {
            try
            {
                String differ = String.Empty;
                var file = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + MacAddress + ".hcf");
                var pcon = System.IO.File.ReadAllText(file);
                if (eload)
                {
                    if (pcon.Equals(ChatContentBak.Text))
                        return;
                    differ = pcon.StartsWith(ChatContentBak.Text) ? pcon[ChatContentBak.Text.Length..] : pcon;
                }
                else
                {
                    ChatContent.Document.Blocks.Clear();
                    differ = pcon;
                    eload = true;
                }
                var con = differ.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (var item in con)
                {
                    var icon = item.Split(',');
                    if (icon.Length < 3)
                        continue;
                    Paragraph nick = new();
                    Run nrun = new()
                    {
                        Text = icon[0].Replace(LocalMacAddress, App.Username).Replace(MacAddress, Aite) + " " + icon[1] + Environment.NewLine
                    };
                    Run wrun = new()
                    {
                        Text = icon[2].Replace("\\\\", "<hisplash>").Replace("\\n", Environment.NewLine).Replace("<hisplash>", "\\\\")
                    };
                    for (int i = 3; i < icon.Length; i++)
                    {
                        wrun.Text += "," + icon[i].Replace("\\\\", "<hisplash>").Replace("\\n", Environment.NewLine).Replace("<hisplash>", "\\\\");
                    }
                    nrun.FontWeight = FontWeights.Bold;
                    nrun.Foreground = icon[0].StartsWith(LocalMacAddress) ? (Brush)Resources["AppForeDim"] : (Brush)Resources["AppFore"];
                    wrun.FontWeight = FontWeights.Normal;
                    wrun.Foreground = nrun.Foreground;
                    nick.Inlines.Add(nrun);
                    nick.Inlines.Add(wrun);
                    ChatContent.Document.Blocks.Add(nick);
                }
                ChatContent.ScrollToEnd();
                Differ_Chat(System.IO.File.ReadAllText(file));
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.Chat.Load: " + ex.Message);
                AddErrorMsg(Hiro_Utils.Get_Transalte("chatsys"), ex.Message);
            }
        }

        private void Differ_Chat(string con)
        {
            if (con.Equals(ChatContentBak.Text))
                return;
            var differ = con.StartsWith(ChatContentBak.Text) ? con[ChatContentBak.Text.Length..] : con;
            ChatContentBak.Text = con;
            var all = differ.Split(new string[] { "\r\n", "\r", "\n" },StringSplitOptions.None);
            foreach (var item in all)
            {
                var index = item.IndexOf(",");
                if (index < 0)
                    continue;
                var user = item[..index];
                if (user.Equals(LocalMacAddress))
                    continue;
                user = user.Replace(LocalMacAddress, App.Username).Replace(MacAddress, Aite);
                index = item.IndexOf(",", index + 1);
                var content = item[(index + 1)..];
                content.Replace(Environment.NewLine, "");
                var i = int.Parse(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Message", "3"));
                switch (i)
                {
                    case 0:
                        break;
                    case 1:
                        content = Hiro_Utils.Get_Transalte("msgnew").Replace("%u", user);
                        break;
                    case 2:
                        if(App.Locked)
                            content = Hiro_Utils.Get_Transalte("msgnew").Replace("%u", user);
                        break;
                    default:
                        break;
                }
                if (i != 0)
                    Hiro_Utils.RunExe("notify(" + user + ": " + content + ",2)", user);
                if(Hiro_Utils.Read_Ini(App.dconfig, "Config", "MessageAudio", "1").Equals("1"))
                    try
                    {
                        System.Media.SoundPlayer sndPlayer = new(Hiro_Utils.Path_Prepare("<win>\\Media\\Windows Notify Messaging.wav"));
                        sndPlayer.Play();
                    }
                    catch (Exception ex)
                    {
                        Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.Chat.Sound: " + ex.Message);
                    }
            }
        }

        private void Hiro_Send_Chat(string msg)
        {
            try
            {
                var content = Hiro_Utils.GetWebContent("https://api.rexio.cn/v2/hiro/chat/send.php?from=" + LocalMacAddress
                + "&to=" + MacAddress
                + "&content=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                + "," + msg);
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.Chat.Send: " + ex.Message);
                AddErrorMsg(Hiro_Utils.Get_Transalte("chatsys"), Hiro_Utils.Get_Transalte("chatsenderror"));
            }
            
        }
        private void Hiro_Update_Name()
        {
            try
            {
                Hiro_Utils.GetWebContent("https://api.rexio.cn/v2/hiro/chat/query.php?mac=" + LocalMacAddress
                        + "&mode=update"
                        + "&name=" + App.Username);
            }
            catch(Exception ex)
            {
                Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.Chat.Update.Nickname: " + ex.Message);
                AddErrorMsg(Hiro_Utils.Get_Transalte("chatsys"), Hiro_Utils.Get_Transalte("chatnonick"));
            }
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

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            Send();
        }

        private void Send()
        {
            if (SendContent.Text.ToLower().Equals("refreash"))
            {
                SendContent.Clear();
                eload = false;
                Hiro_Get_Chat();
                return;
            }
            if (SendContent.Text.ToLower().StartsWith("talkto:"))
            {
                MacAddress = SendContent.Text[7..];
                Hiro_Utils.LogtoFile("[INFO]Talk to " + MacAddress);
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "ChatMAC", MacAddress);
                var file = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + MacAddress + ".hcf");
                ChatContentBak.Text = System.IO.File.Exists(file) ? System.IO.File.ReadAllText(file) : "";
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

        private void SendContent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Return) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                var i = SendContent.SelectionStart;
                var fore = i > 0 ? SendContent.Text[..i] : "";
                var back = i == SendContent.Text.Length ? "" : SendContent.Text[i..];
                SendContent.Text = fore + Environment.NewLine + back;
                SendContent.SelectionStart = i + Environment.NewLine.Length;
                e.Handled = true;
            }
            else if(e.KeyStates == Keyboard.GetKeyStates(Key.Return))
            {
                Send();
                e.Handled = true;
            }
        }

        private void EMojiButton_Click(object sender, RoutedEventArgs e)
        {
            ChatContent.Paste();
        }
    }
}
