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
        private string UserId = "rex";
        private string LocalId = "unknown";
        private string Aite = "unknown-user";
        private bool load = false;
        private bool eload = false;
        private bool hload = true;
        private int ernum = 0;
        public Hiro_Chat(Hiro_MainUI? Parent)
        {
            InitializeComponent();
            Hiro_Main = Parent;
            Hiro_Initialize();
            Loaded += delegate
            {
                HiHiro();
            };
        }


        public void HiHiro(bool first = false)
        {
            var animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            Storyboard sb = new();
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                Hiro_Utils.AddPowerAnimation(0, ChatContent, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(0, SendContent, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(2, SendButton, sb, -50, null);
            }
            if (animation)
            {
                Hiro_Utils.AddPowerAnimation(0, this, sb, 50, null);
                sb.Begin();
                sb.Completed += delegate
                {
                    if (hload)
                        Load_Friend_Info_First();
                    hload = false;
                };
            }
            else
            {
                if (hload)
                    Load_Friend_Info_First();
                hload = false;
            }
        }

        private void Hiro_Initialize()
        {
            Hiro_Utils.LogtoFile("[INFO]Hiro.Chat Initializing");
            Load_Color();
            Load_Translate();
            Load_Position();
        }

        public void Load_Friend_Info_First()
        {
            new System.Threading.Thread(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    ChatContentBak.Clear();
                    ChatContent.Document.Blocks.Clear();
                });
                load = false;
                eload = false;
                var folder = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\");
                Hiro_Utils.CreateFolder(folder);
                var file = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + UserId + ".hcf");
                UserId = Hiro_Utils.Read_Ini(App.dconfig, "Config", "ChatID", "rex");
                LocalId = App.LoginedUser;
                Dispatcher.Invoke(() =>
                {
                    ChatContentBak.Text = System.IO.File.Exists(file) ? System.IO.File.ReadAllText(file) : "";
                    Profile_Mac.Content = UserId;
                });
                Load_Avatar();
                Hiro_Utils.LogtoFile("[INFO]Hiro.Chat.Avatar Initialized");
                Load_Background();
                Hiro_Utils.LogtoFile("[INFO]Hiro.Chat.Background Initialized");
                Load_Profile();
                Hiro_Utils.LogtoFile("[INFO]Hiro.Chat.Profile Initialized");
                Dispatcher.Invoke(() =>
                {
                    Load_Chat();
                    load = true;
                });
            }).Start();
        }

        private void Load_Avatar()
        {
            var StrFileName = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\friends\\list.hfl");
            StrFileName = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(StrFileName, UserId, "Avatar", string.Empty)));
            try
            {
                if (System.IO.File.Exists(StrFileName))
                {
                    Dispatcher.Invoke(() =>
                    {
                        System.Windows.Media.Imaging.BitmapImage? bi = Hiro_Utils.GetBitmapImage(StrFileName);
                        ImageBrush ib = new()
                        {
                            Stretch = Stretch.UniformToFill,
                            ImageSource = bi
                        };
                        Resources["ChatAvatar"] = ib;
                        Profile_Rectangle.Fill = Profile_Rectangle.Visibility == Visibility.Visible ? (ImageBrush)Resources["ChatAvatar"] : null;
                        Profile_Ellipse.Fill = Profile_Ellipse.Visibility == Visibility.Visible ? (ImageBrush)Resources["ChatAvatar"] : null;
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        Resources["ChatAvatar"] = new ImageBrush()
                        {
                            Stretch = Stretch.UniformToFill,
                            ImageSource = Hiro_Profile.ImageSourceFromBitmap(Hiro_Resources.Default_User)
                        };
                        Profile_Rectangle.Fill = Profile_Rectangle.Visibility == Visibility.Visible ? (ImageBrush)Resources["ChatAvatar"] : null;
                        Profile_Ellipse.Fill = Profile_Ellipse.Visibility == Visibility.Visible ? (ImageBrush)Resources["ChatAvatar"] : null;
                    });
                }
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.Chat.Update.Friend.Avatar");
                Dispatcher.Invoke(() =>
                {
                    Resources["ChatAvatar"] = new ImageBrush()
                    {
                        Stretch = Stretch.UniformToFill,
                        ImageSource = Hiro_Profile.ImageSourceFromBitmap(Hiro_Resources.Default_User)
                    };
                    Profile_Rectangle.Fill = Profile_Rectangle.Visibility == Visibility.Visible ? (ImageBrush)Resources["ChatAvatar"] : null;
                    Profile_Ellipse.Fill = Profile_Ellipse.Visibility == Visibility.Visible ? (ImageBrush)Resources["ChatAvatar"] : null;
                });
            }
        }

        private void Load_Background()
        {
            var StrFileName = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\friends\\list.hfl");
            StrFileName = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(StrFileName, UserId, "BackImage", string.Empty)));
            try
            {
                if (System.IO.File.Exists(StrFileName))
                {
                    Dispatcher.Invoke(() =>
                    {
                        System.Windows.Media.Imaging.BitmapImage? bi = Hiro_Utils.GetBitmapImage(StrFileName);
                        ImageBrush ib = new()
                        {
                            Stretch = Stretch.UniformToFill,
                            ImageSource = bi
                        };
                        Profile_Background.Background = ib;
                    });
                    return;
                }
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.Chat.Update.Friend.Profile");
                Dispatcher.Invoke(() =>
                {
                    Profile_Background.Background = new ImageBrush()
                    {
                        Stretch = Stretch.UniformToFill,
                        ImageSource = Hiro_Profile.ImageSourceFromBitmap(Hiro_Resources.Default_Background)
                    };
                });
            }
        }

        private void Load_Profile()
        {
            var StrFileName = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\friends\\list.hfl");
            StrFileName = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(StrFileName, UserId, "Profile", string.Empty)));
            Aite = Hiro_Utils.Read_Ini(StrFileName, "Config", "Name", Hiro_Utils.Get_Transalte("chatuser").Replace("%m", UserId));
            Dispatcher.Invoke(() =>
            {
                if (!Profile_Nickname.Content.Equals(Aite))
                {
                    Profile_Nickname.Content = Aite;
                    if (load)
                        Load_Chat();
                }
                Profile_Signature.Content = Hiro_Utils.Read_Ini(StrFileName, "Config", "Signature", string.Empty);
                var psc = Profile_Signature.Content.ToString();
                if (psc == null)
                    Profile_Signature.Content = Hiro_Utils.Get_Transalte("profilesign");
                else if (psc.Trim().Equals(string.Empty))
                    Profile_Signature.Content = Hiro_Utils.Get_Transalte("profilesign");
                switch (Hiro_Utils.Read_Ini(StrFileName, "Config", "Avatar", "1"))
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
                Profile_Rectangle.Fill = Profile_Rectangle.Visibility == Visibility.Visible ? (ImageBrush)Resources["ChatAvatar"] : null;
                Profile_Ellipse.Fill = Profile_Ellipse.Visibility == Visibility.Visible ? (ImageBrush)Resources["ChatAvatar"] : null;
            });
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 180));
            Resources["AppForeDisabled"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 180));
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
            Resources["AppAccentDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 180));
            Hiro_Utils.Set_Opacity(Profile_Background, BackControl);
            if (load)
                Load_Chat();
        }

        public void Hiro_Get_Chat()
        {
            new System.Threading.Thread(() =>
            {
                try
                {
                    Hiro_Utils.CreateFolder(Hiro_Utils.Path_Prepare("<hiapp>\\chat\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM-dd") + "\\"));
                    Hiro_Utils.CreateFolder(Hiro_Utils.Path_Prepare("<hiapp>\\chat\\friends\\profile\\"));
                    Hiro_Utils.CreateFolder(Hiro_Utils.Path_Prepare("<hiapp>\\chat\\friends\\back\\"));
                    Hiro_Utils.CreateFolder(Hiro_Utils.Path_Prepare("<hiapp>\\chat\\friends\\avatar\\"));
                    var hus = Guid.NewGuid();
                    var StrFileName = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\friends\\list.hfl");
                    var baseDir = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\friends\\profile\\");
                    var pformer = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(StrFileName, UserId, "Profile", string.Empty)));
                    while (System.IO.File.Exists(baseDir + hus + ".hus"))
                    {
                        hus = Guid.NewGuid();
                    }
                    var tmp = System.IO.Path.GetTempFileName();
                    var res = Hiro_Utils.UploadProfileSettings(
                        UserId, "token", Aite,
                        Hiro_Utils.Read_Ini(pformer, "Config", "Signature", string.Empty),
                        Hiro_Utils.Read_Ini(pformer, "Config", "Avatar", "1"),
                        Hiro_Utils.GetMD5(Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(StrFileName, UserId, "Avatar", string.Empty)))).Replace("-", ""),
                        Hiro_Utils.GetMD5(Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(StrFileName, UserId, "BackImage", string.Empty)))).Replace("-", ""),
                        "check",
                        tmp
                        );
                    var u = false;
                    var r = Hiro_Utils.Read_Ini(tmp, "Profile", "Name", string.Empty);
                    if (!r.Equals(string.Empty))
                    {
                        Aite = r;
                        Hiro_Utils.Write_Ini(baseDir + hus + ".hus", "Config", "Name", r);
                        u = true;
                    }
                    else
                    {
                        Hiro_Utils.Write_Ini(baseDir + hus + ".hus", "Config", "Name", Aite);
                    }
                    r = Hiro_Utils.Read_Ini(tmp, "Profile", "Sign", string.Empty);
                    if (!r.Equals(string.Empty))
                    {
                        Hiro_Utils.Write_Ini(baseDir + hus + ".hus", "Config", "Signature", r);
                        u = true;
                    }
                    else
                    {
                        Hiro_Utils.Write_Ini(baseDir + hus + ".hus", "Config", "Signature", Hiro_Utils.Read_Ini(pformer, "Config", "Signature", string.Empty));
                    }
                    r = Hiro_Utils.Read_Ini(tmp, "Profile", "Avatar", string.Empty);
                    if(!r.Equals(string.Empty))
                    {
                        Hiro_Utils.Write_Ini(baseDir + hus + ".hus", "Config", "Avatar", r);
                        u = true;
                    }
                    else
                    {
                        Hiro_Utils.Write_Ini(baseDir + hus + ".hus", "Config", "Avatar", Hiro_Utils.Read_Ini(pformer, "Config", "Avatar", "1"));
                    }
                    if (u)
                    {
                        if (System.IO.File.Exists(pformer))
                            System.IO.File.Delete(pformer);
                        Hiro_Utils.Write_Ini(StrFileName, UserId, "Profile", "<hiapp>\\chat\\friends\\profile\\" + hus + ".hus");
                        Load_Profile();
                    }
                    else
                    {
                        System.IO.File.Delete(baseDir + hus + ".hus");
                    }
                    baseDir = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\friends\\avatar\\");
                    while (System.IO.File.Exists(baseDir + hus + ".hap")) 
                    {
                        hus = Guid.NewGuid();
                    }
                    r = Hiro_Utils.Read_Ini(tmp, "Profile", "Iavatar", string.Empty);
                    if (!r.Equals(string.Empty))
                    {
                        var former = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(StrFileName, UserId, "Avatar", string.Empty)));
                        if (System.IO.File.Exists(former))
                            System.IO.File.Delete(former);
                        Hiro_Utils.Write_Ini(StrFileName, UserId, "Avatar", "<hiapp>\\chat\\friends\\avatar\\" + hus + ".hap");
                        Hiro_Utils.GetWebContent(r, true, baseDir + hus + ".hap");
                        Load_Avatar();
                    }
                    baseDir = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\friends\\back\\");
                    while (System.IO.File.Exists(baseDir + hus + ".hpp"))
                    {
                        hus = Guid.NewGuid();
                    }
                    r = Hiro_Utils.Read_Ini(tmp, "Profile", "Back", string.Empty);
                    if (!r.Equals(string.Empty))
                    {
                        var former = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(StrFileName, UserId, "BackImage", string.Empty)));
                        if (System.IO.File.Exists(former))
                            System.IO.File.Delete(former);
                        Hiro_Utils.Write_Ini(StrFileName, UserId, "BackImage", "<hiapp>\\chat\\friends\\back\\" + hus + ".hpp");
                        Hiro_Utils.GetWebContent(r, true, baseDir + hus + ".hpp");
                        Load_Background();
                    }
                    System.IO.File.Delete(tmp);
                    var hcf = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + UserId + ".hcf");
                    var content = Hiro_Utils.GetChat(App.LoginedUser, App.LoginedToken, UserId, hcf);
                    if (content.Equals("success"))
                    {
                        content = System.IO.File.ReadAllText(hcf);
                        if (content.Equals("IDENTIFY_ERROR"))
                        {
                            Hiro_Utils.Logout();
                            Dispatcher.Invoke(() =>
                            {
                                Hiro_Main?.Set_Label(Hiro_Main.loginx);
                                Hiro_Utils.RunExe("notify(" + Hiro_Utils.Get_Transalte("lgexpired") + ",2)", Hiro_Utils.Get_Transalte("chat"));
                            });
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Load_Chat();
                            });
                        }
                    }
                        
                }
                catch (Exception ex)
                {
                    Hiro_Utils.LogError(ex, "Hiro.Exception.Chat.Update");
                    Dispatcher.Invoke(() =>
                    {
                        AddErrorMsg(Hiro_Utils.Get_Transalte("chatsys"), Hiro_Utils.Get_Transalte("chatnofetch"));
                    });
                }
            }).Start();
        }

        private void AddErrorMsg(string user, string msg)
        {
            ernum++;
            if (ernum > 5)
                return;
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
                string differ = string.Empty;
                var file = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + UserId + ".hcf");
                var pcon = System.IO.File.Exists(file) ? System.IO.File.ReadAllText(file) : string.Empty;
                if (eload)
                {
                    if (pcon.Equals(ChatContentBak.Text))
                        return;
                    differ = pcon.StartsWith(ChatContentBak.Text) ? pcon[ChatContentBak.Text.Length..] : pcon;
                }
                else
                {
                    ChatContentBak.Text = pcon;
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
                        Text = icon[0].Replace(LocalId, App.Username).Replace(UserId, Aite) + " " + icon[1] + Environment.NewLine
                    };
                    var pre = icon[2].Replace("\\\\", "<hisplash>").Replace("\\n", Environment.NewLine).Replace("<hisplash>", "\\\\");
                    if (pre.Contains("[Hiro.Predefinited:LikeAvatar]"))
                        pre = Hiro_Utils.Get_Transalte("avatarlike");
                    if (pre.Contains("[Hiro.Predefinited:LikeProfile]"))
                        pre = Hiro_Utils.Get_Transalte("profilelike");
                    if (pre.Contains("[Hiro.Predefinited:LikeName]"))
                        pre = Hiro_Utils.Get_Transalte("namelike");
                    if (pre.Contains("[Hiro.Predefinited:LikeSign]"))
                        pre = Hiro_Utils.Get_Transalte("signlike");
                    Run wrun = new()
                    {
                        Text = pre
                    };
                    for (int i = 3; i < icon.Length; i++)
                    {
                        wrun.Text += "," + icon[i].Replace("\\\\", "<hisplash>").Replace("\\n", Environment.NewLine).Replace("<hisplash>", "\\\\");
                    }
                    nrun.FontWeight = FontWeights.Bold;
                    nrun.Foreground = icon[0].StartsWith(LocalId) ? (Brush)Resources["AppForeDim"] : (Brush)Resources["AppFore"];
                    wrun.FontWeight = FontWeights.Normal;
                    wrun.Foreground = nrun.Foreground;
                    nick.Inlines.Add(nrun);
                    nick.Inlines.Add(wrun);
                    ChatContent.Document.Blocks.Add(nick);
                }
                ChatContent.ScrollToEnd();
                ernum = 0;
                Differ_Chat(pcon);
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.Chat.Load");
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
                if (user.Equals(LocalId))
                    continue;
                user = user.Replace(LocalId, App.Username).Replace(UserId, Aite);
                index = item.IndexOf(",", index + 1);
                var content = item[(index + 1)..];
                content = content.Replace(Environment.NewLine, "");
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
                var cont = content.Trim();
                if (cont.Contains("[Hiro.Predefinited:LikeAvatar]"))
                    content = Hiro_Utils.Get_Transalte("avatarlike");
                if (cont.Contains("[Hiro.Predefinited:LikeProfile]"))
                    content = Hiro_Utils.Get_Transalte("profilelike");
                if (cont.Contains("[Hiro.Predefinited:LikeName]"))
                    content = Hiro_Utils.Get_Transalte("namelike");
                if (cont.Contains("[Hiro.Predefinited:LikeSign]"))
                    content = Hiro_Utils.Get_Transalte("signlike");
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
                        Hiro_Utils.LogError(ex, "Hiro.Exception.Chat.Sound");
                    }
            }
        }

        private void Hiro_Send_Chat(string msg)
        {
            try
            {
                var res = Hiro_Utils.SendMsg(App.LoginedUser, App.LoginedToken, UserId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                + "," + msg);
                if (!res.Equals("success"))
                    Dispatcher.Invoke(() =>
                    {
                        AddErrorMsg(Hiro_Utils.Get_Transalte("chatsys"), Hiro_Utils.Get_Transalte("chatsenderror"));
                    });
                else if (res.Equals("IDENTIFY_ERROR"))
                {
                    Hiro_Utils.Logout();
                    Dispatcher.Invoke(() =>
                    {
                        Hiro_Main?.Set_Label(Hiro_Main.loginx);
                        Hiro_Utils.RunExe("notify(" + Hiro_Utils.Get_Transalte("lgexpired") + ",2)", Hiro_Utils.Get_Transalte("chat"));
                    });
                }
                    
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.Chat.Send");
                Dispatcher.Invoke(() =>
                {
                    AddErrorMsg(Hiro_Utils.Get_Transalte("chatsys"), Hiro_Utils.Get_Transalte("chatsenderror"));
                });
            }
            
        }
        private void Hiro_Update_Name()
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
                        AddErrorMsg(Hiro_Utils.Get_Transalte("chatsys"), Hiro_Utils.Get_Transalte("chatdeve"));
                    });
                }
            }
            catch(Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.Chat.Update.Nickname");
                Dispatcher.Invoke(() =>
                {
                    AddErrorMsg(Hiro_Utils.Get_Transalte("chatsys"), Hiro_Utils.Get_Transalte("chatnonick"));
                });
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
            Hiro_Utils.Set_Control_Location(Profile_Nickname_Indexer, "chatname");
            Hiro_Utils.Set_Control_Location(Profile_Signature_Indexer, "chatsign");
            Hiro_Utils.Set_Control_Location(Profile_Mac, "chatid");
            Hiro_Utils.Set_Control_Location(Profile_Background, "chatback");
            Hiro_Utils.Set_FrameworkElement_Location(Profile_Ellipse, "chatavatar");
            Hiro_Utils.Set_FrameworkElement_Location(Profile_Rectangle, "chatavatar");
            Hiro_Utils.Set_FrameworkElement_Location(Profile, "chatprofile");
            Profile_Mac.Margin = new Thickness(Profile_Nickname.Margin.Left + Profile_Nickname.ActualWidth, Profile_Nickname.Margin.Top + Profile_Nickname.ActualHeight - Profile_Mac.ActualHeight, 0, 0);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            Send(SendContent.Text);
            SendContent.Clear();
        }

        private void Send(string text)
        {
            if (text.ToLower().Equals("refreash"))
            {
                SendContent.Clear();
                Hiro_Get_Chat();
                return;
            }
            if (text.ToLower().StartsWith("talkto:"))
            {
                UserId = text[7..];
                Hiro_Utils.LogtoFile("[INFO]Talk to " + UserId);
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "ChatID", UserId);
                SendContent.Clear();
                Load_Friend_Info_First();
                return;
            }
            if (!text.Equals(string.Empty))
            {
                SendButton.IsEnabled = false;
                var tmp = text.Replace(Environment.NewLine, "\\n");
                new System.Threading.Thread(() =>
                {
                    Hiro_Send_Chat(tmp);
                    Hiro_Update_Name();
                    Hiro_Get_Chat();
                }).Start();
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
                Send(SendContent.Text);
                SendContent.Clear();
                e.Handled = true;
            }
        }

        private void EMojiButton_Click(object sender, RoutedEventArgs e)
        {
            ChatContent.Paste();
        }

        private void Profile_Background_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Send("[Hiro.Predefinited:LikeProfile]");
        }

        private void Profile_Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Send("[Hiro.Predefinited:LikeAvatar]");
        }

        private void Profile_Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Send("[Hiro.Predefinited:LikeAvatar]");
        }

        private void Profile_Nickname_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Send("[Hiro.Predefinited:LikeName]");
        }

        private void Profile_Signature_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Send("[Hiro.Predefinited:LikeSign]");
        }

        private void Profile_Nickname_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Profile_Mac.Margin = new Thickness(Profile_Nickname.Margin.Left + Profile_Nickname.ActualWidth, Profile_Nickname.Margin.Top + Profile_Nickname.ActualHeight - Profile_Mac.ActualHeight, 0, 0);
        }

        private void Profile_Mac_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(Profile_Mac.Content.ToString());
            Hiro_Utils.RunExe("notify(" + Hiro_Utils.Get_Transalte("chatmcopy").Replace("%u", Aite) + ",2)", Hiro_Utils.Get_Transalte("chat"));
        }
    }
}
