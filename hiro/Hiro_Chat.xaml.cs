using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace hiro
{
    /// <summary>
    /// Hiro_Chat.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Chat : Page
    {
        private Hiro_MainUI? Hiro_Main = null;
        private string UserId = "rex";
        internal string LocalId = "unknown";
        private string Aite = "unknown-user";
        private bool load = false;
        private bool eload = false;
        private bool hload = true;
        private int ernum = 0;
        private bool xflag = false;
        internal Inline? inline = null;
        internal object? cop = null;
        internal string cops = string.Empty;
        public Hiro_Chat(Hiro_MainUI? Parent)
        {
            InitializeComponent();
            Hiro_Main = Parent;
            Hiro_Initialize();
            Loaded += delegate
            {
                HiHiro();
            };
            /*DataObject.AddCopyingHandler(SendContent, new DataObjectCopyingEventHandler((sender, e) =>
            {
                cop = e.Source;
                if (!SendContent.Selection.IsEmpty)
                {
                    var doc = new FlowDocument(SendContent.Selection.);
                }
                
            }));
            DataObject.AddPastingHandler(SendContent, new DataObjectPastingEventHandler((sender, e) =>
            {
                Hiro_Utils.LogtoFile("Pasting data");
                if (e.Source == cop)
                {
                    e.CancelCommand();
                    e.Handled = true;
                }
            }));
            
             
            var curCaret = richTextBox1.CaretPosition;
var curBlock = richTextBox1.Document.Blocks.Where(x => x.ContentStart.CompareTo(curCaret) == -1 && x.ContentEnd.CompareTo(curCaret) == 1).FirstOrDefault(); 


             */
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
            new System.Threading.Thread(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    Emoji_Cate.Children.Clear();
                });
                var basedir = new DirectoryInfo(System.IO.Path.GetDirectoryName(Hiro_Utils.Path_Prepare(@"<current>\system\emoji\")) ?? "").GetDirectories();
                var w = -25.0;
                foreach (var d in basedir)
                {
                    var cf = d.FullName + "\\" + d.Name + ".hes";
                    if (App.dflag)
                        Hiro_Utils.LogtoFile(cf);
                    if (File.Exists(cf))
                    {
                        var ifi = d.FullName + "\\" + Hiro_Utils.Read_Ini(cf, "Typical", App.lang, "<???>");
                        if (File.Exists(ifi))
                        {
                            Image? im = null;
                            Dispatcher.Invoke(() =>
                            {
                                im = new Image()
                                {
                                    Source = new BitmapImage(new Uri(ifi, UriKind.Absolute)),
                                    Stretch = Stretch.Uniform,
                                    Width = 25,
                                    Height = 25,
                                    VerticalAlignment = VerticalAlignment.Top,
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    ToolTip = Hiro_Utils.Read_Ini(cf, "Name", App.lang, "<???>"),
                                    Tag = d.FullName
                                };
                            });
                            im.MouseEnter += delegate
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    var m = new Thickness(im.Margin.Left - 5, im.Margin.Top - 5, 0, 0);
                                    if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
                                    {
                                        Storyboard sb = new();
                                        Hiro_Utils.AddThicknessAnimaton(m, 250, FocusLabelR, "Margin", sb);
                                        Hiro_Utils.AddDoubleAnimaton(im.Height + 10, 250, FocusLabelR, "Height", sb);
                                        Hiro_Utils.AddDoubleAnimaton(im.Width + 10, 250, FocusLabelR, "Width", sb);
                                        Hiro_Utils.AddDoubleAnimaton(1, 250, FocusLabelR, "Opacity", sb);
                                        sb.Completed += delegate
                                        {
                                            FocusLabelR.Width = im.Width + 10;
                                            FocusLabelR.Height = im.Height + 10;
                                            FocusLabelR.Margin = m;
                                            FocusLabelR.Opacity = 1;
                                        };
                                        sb.Begin();
                                    }
                                    else
                                    {
                                        FocusLabelR.Width = im.Width + 10;
                                        FocusLabelR.Height = im.Height + 10;
                                        FocusLabelR.Margin = m;
                                        FocusLabelR.Opacity = 1;
                                    }
                                });
                            };
                            im.MouseDown += delegate
                            {
                                var imt = im.Tag.ToString();
                                if (imt != null)
                                    try
                                    {
                                        Load_Emoji(imt);
                                    }
                                    catch (Exception ex)
                                    {
                                        Hiro_Utils.LogError(ex, "Hiro.Emoji.Load");
                                    }

                            };
                            im.MouseLeave += delegate
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
                                    {
                                        Storyboard sb = new();
                                        Hiro_Utils.AddDoubleAnimaton(0, 250, FocusLabelR, "Opacity", sb);
                                        sb.Completed += delegate
                                        {
                                            FocusLabelR.Opacity = 0;
                                        };
                                        sb.Begin();
                                    }
                                    else
                                        FocusLabelR.Opacity = 0;
                                });

                            };
                            w = w + 35;
                            Dispatcher.Invoke(() =>
                            {
                                im.Margin = new Thickness(w, 5, 0, 0);
                                Emoji_Cate.Children.Add(im);
                                Emoji_Cate.Width = w + 35;
                            });
                        }
                        else
                            continue;
                    }
                    else
                        continue;
                }
                var str = string.Empty;
                Dispatcher.Invoke(() =>
                {
                    if (Emoji_Cate.Children.Count > 0)
                        str = (Emoji_Cate.Children[0] as Image ?? new Image() { Tag = string.Empty }).Tag.ToString();
                });
                if (!str.Equals(string.Empty))
                    try
                    {
                        Load_Emoji(str);
                    }
                    catch (Exception ex)
                    {
                        Hiro_Utils.LogError(ex, "Hiro.Emoji.Load");
                    }
            }).Start();
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
                    ChatContentBak.Text = File.Exists(file) ? File.ReadAllText(file) : "";
                    Profile_Mac.Content = UserId;
                });
                Load_Avatar();
                Hiro_Utils.LogtoFile("[INFO]Hiro.Chat.Avatar Initialized");
                Load_Background();
                Hiro_Utils.LogtoFile("[INFO]Hiro.Chat.Background Initialized");
                Load_Profile();
                Hiro_Utils.LogtoFile("[INFO]Hiro.Chat.Profile Initialized");
                Load_Chat();
                load = true;
            }).Start();
        }

        private void Load_Avatar()
        {
            var StrFileName = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\friends\\list.hfl");
            StrFileName = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(StrFileName, UserId, "Avatar", string.Empty)));
            try
            {
                if (File.Exists(StrFileName))
                {
                    Dispatcher.Invoke(() =>
                    {
                        BitmapImage? bi = Hiro_Utils.GetBitmapImage(StrFileName);
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
                if (File.Exists(StrFileName))
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
            Aite = Hiro_Utils.Read_Ini(StrFileName, "Config", "Name", Hiro_Utils.Get_Translate("chatuser").Replace("%m", UserId));
            var aflag = false;
            Dispatcher.Invoke(() =>
            {
                aflag = !Profile_Nickname.Content.Equals(Aite);
                if (aflag)
                {
                    Profile_Nickname.Content = Aite;
                }
            });
            if (aflag && load)
                Load_Chat();
            Dispatcher.Invoke(() =>
            {
                Profile_Signature.Content = Hiro_Utils.Read_Ini(StrFileName, "Config", "Signature", string.Empty);
                var psc = Profile_Signature.Content.ToString();
                if (psc == null)
                    Profile_Signature.Content = Hiro_Utils.Get_Translate("profilesign");
                else if (psc.Trim().Equals(string.Empty))
                    Profile_Signature.Content = Hiro_Utils.Get_Translate("profilesign");
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
            Resources["InfoAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 200));
            Hiro_Utils.Set_Opacity(Profile_Background, BackControl);
            if (load)
                new System.Threading.Thread(() =>
                {
                    Load_Chat();
                }).Start();
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
                    while (File.Exists(baseDir + hus + ".hus"))
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
                    if (!r.Equals(string.Empty))
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
                        if (File.Exists(pformer))
                            File.Delete(pformer);
                        Hiro_Utils.Write_Ini(StrFileName, UserId, "Profile", "<hiapp>\\chat\\friends\\profile\\" + hus + ".hus");
                        Load_Profile();
                    }
                    else
                    {
                        File.Delete(baseDir + hus + ".hus");
                    }
                    baseDir = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\friends\\avatar\\");
                    while (File.Exists(baseDir + hus + ".hap"))
                    {
                        hus = Guid.NewGuid();
                    }
                    r = Hiro_Utils.Read_Ini(tmp, "Profile", "Iavatar", string.Empty);
                    if (!r.Equals(string.Empty))
                    {
                        var former = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(StrFileName, UserId, "Avatar", string.Empty)));
                        if (File.Exists(former))
                            File.Delete(former);
                        Hiro_Utils.Write_Ini(StrFileName, UserId, "Avatar", "<hiapp>\\chat\\friends\\avatar\\" + hus + ".hap");
                        Hiro_Utils.GetWebContent(r, true, baseDir + hus + ".hap");
                        Load_Avatar();
                    }
                    baseDir = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\friends\\back\\");
                    while (File.Exists(baseDir + hus + ".hpp"))
                    {
                        hus = Guid.NewGuid();
                    }
                    r = Hiro_Utils.Read_Ini(tmp, "Profile", "Back", string.Empty);
                    if (!r.Equals(string.Empty))
                    {
                        var former = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(StrFileName, UserId, "BackImage", string.Empty)));
                        if (File.Exists(former))
                            File.Delete(former);
                        Hiro_Utils.Write_Ini(StrFileName, UserId, "BackImage", "<hiapp>\\chat\\friends\\back\\" + hus + ".hpp");
                        Hiro_Utils.GetWebContent(r, true, baseDir + hus + ".hpp");
                        Load_Background();
                    }
                    File.Delete(tmp);
                    var hcf = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + UserId + ".hcf");
                    var content = Hiro_Utils.GetChat(App.LoginedUser, App.LoginedToken, UserId, hcf);
                    if (content.Equals("success"))
                    {
                        content = File.ReadAllText(hcf);
                        if (content.Equals("IDENTIFY_ERROR"))
                        {
                            Hiro_Utils.Logout();
                            Dispatcher.Invoke(() =>
                            {
                                Hiro_Main?.Set_Label(Hiro_Main.loginx);
                                App.Notify(new(Hiro_Utils.Get_Translate("lgexpired"), 2, Hiro_Utils.Get_Translate("chat")));
                            });
                        }
                        else
                        {
                            Load_Chat();
                        }
                    }

                }
                catch (Exception ex)
                {
                    Hiro_Utils.LogError(ex, "Hiro.Exception.Chat.Update");
                    Dispatcher.Invoke(() =>
                    {
                        AddErrorMsg(Hiro_Utils.Get_Translate("chatsys"), Hiro_Utils.Get_Translate("chatnofetch"));
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
                var pcon = File.Exists(file) ? File.ReadAllText(file) : string.Empty;
                var ccbt = string.Empty;
                if (eload)
                {
                    Dispatcher.Invoke(() =>
                    {
                        ccbt = ChatContentBak.Text;
                    });
                    if (pcon.Equals(ccbt))
                        return;
                    differ = pcon.StartsWith(ccbt) ? pcon[ccbt.Length..] : pcon;
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        ChatContentBak.Text = pcon;
                    });
                    ccbt = pcon;
                    differ = pcon;
                    eload = true;
                }
                var con = differ.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (var item in con)
                {
                    var icon = item.Split(',');
                    if (icon.Length < 3)
                        continue;
                    Paragraph? nick = null;
                    Dispatcher.Invoke(() =>
                    {
                        nick = new();
                        Run nrun = new()
                        {
                            Text = icon[0].Replace(LocalId, App.Username).Replace(UserId, Aite) + " " + icon[1] + Environment.NewLine,
                            FontWeight = FontWeights.Bold,
                            Foreground = icon[0].StartsWith(LocalId) ? (Brush)Resources["AppForeDim"] : (Brush)Resources["AppFore"]
                        };
                        nick.Inlines.Add(nrun);
                    });
                    var pre = icon[2].Replace("\\\\", "<hisplash>").Replace("\\n", Environment.NewLine).Replace("<hisplash>", "\\\\");
                    if (pre.Contains("[Hiro.Predefinited:LikeAvatar]"))
                        pre = Hiro_Utils.Get_Translate("avatarlike");
                    if (pre.Contains("[Hiro.Predefinited:LikeProfile]"))
                        pre = Hiro_Utils.Get_Translate("profilelike");
                    if (pre.Contains("[Hiro.Predefinited:LikeName]"))
                        pre = Hiro_Utils.Get_Translate("namelike");
                    if (pre.Contains("[Hiro.Predefinited:LikeSign]"))
                        pre = Hiro_Utils.Get_Translate("signlike");
                    var chatText = pre;
                    for (int i = 3; i < icon.Length; i++)
                    {
                        chatText += "," + icon[i].Replace("\\\\", "<hisplash>").Replace("\\n", Environment.NewLine).Replace("<hisplash>", "\\\\");
                    }
                    var rgx = new Regex(@"\[Hiro.Emoji:[\S].*?\]");
                    while (rgx.IsMatch(chatText))
                    {
                        var m = rgx.Match(chatText);
                        if (m.Index > 0)
                        {
                            var stext = chatText[..m.Index];
                            var sgx = new Regex(@"\[Hiro.[\S].*?\]");
                            stext = sgx.Replace(stext, Hiro_Utils.Get_Translate("chatnotsupport"));
                            Dispatcher.Invoke(() =>
                            {
                                nick.Inlines.Add(new Run()
                                {
                                    Text = stext,
                                    FontWeight = FontWeights.Normal,
                                    Foreground = icon[0].StartsWith(LocalId) ? (Brush)Resources["AppForeDim"] : (Brush)Resources["AppFore"]
                                });
                            });
                        }
                        if (chatText.Length > m.Index + m.Length)
                            chatText = chatText[(m.Index + m.Length)..];
                        else
                            chatText = string.Empty;
                        var path = Hiro_Utils.Path_Prepare(@"<current>\system\emoji\" + m.Value.Substring("[Hiro.Emoji:".Length, m.Value.Length - 1 - "[Hiro.Emoji:".Length));
                        var bflag = File.Exists(path);
                        if (bflag)
                        {
                            if (App.dflag)
                                Hiro_Utils.LogtoFile(path);
                            var bw = 25.0;
                            try
                            {
                                var etxt = Hiro_Utils.Read_Ini(System.IO.Path.GetDirectoryName(path) + "\\" + new DirectoryInfo(System.IO.Path.GetDirectoryName(path)).Name + ".hes", "Emoji", "IsExtra", string.Empty).ToLower();
                                if (etxt.Equals("true"))
                                {
                                    bw = 50.0;
                                }
                                else if (etxt.Equals("extra"))
                                {
                                    bw = 100.0;
                                };
                            }
                            catch { }
                            Dispatcher.Invoke(() =>
                            {
                                nick.Inlines.Add(new InlineUIContainer(
                                    new Image()
                                    {
                                        Source = new BitmapImage(new Uri(path, UriKind.Absolute)),
                                        Stretch = Stretch.UniformToFill,
                                        Width = bw,
                                        Height = bw
                                    })
                                { Tag = m.Value });
                            });
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                nick.Inlines.Add(new Run()
                                {
                                    Text = Hiro_Utils.Get_Translate("emojinotsupport"),
                                    FontWeight = FontWeights.Normal,
                                    Foreground = icon[0].StartsWith(LocalId) ? (Brush)Resources["AppForeDim"] : (Brush)Resources["AppFore"]
                                });
                            });

                        }
                    };

                    var sctext = chatText;
                    var scgx = new Regex(@"\[Hiro.[\S].*?\]");
                    sctext = scgx.Replace(sctext, Hiro_Utils.Get_Translate("chatnotsupport"));
                    Dispatcher.Invoke(() =>
                    {
                        if (sctext.Length > 0)
                            nick.Inlines.Add(new Run()
                            {
                                Text = sctext,
                                FontWeight = FontWeights.Normal,
                                Foreground = icon[0].StartsWith(LocalId) ? (Brush)Resources["AppForeDim"] : (Brush)Resources["AppFore"]
                            });
                        ChatContent.Document.Blocks.Add(nick);
                    });
                }
                Dispatcher.Invoke(() =>
                {
                    ChatContent.ScrollToEnd();
                });
                ernum = 0;
                Differ_Chat(pcon);
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.Chat.Load");
                Dispatcher.Invoke(() =>
                {
                    AddErrorMsg(Hiro_Utils.Get_Translate("chatsys"), ex.Message);
                });
            }
        }

        private void Differ_Chat(string con)
        {
            var ccbt = string.Empty;
            Dispatcher.Invoke(() =>
            {
                ccbt = ChatContentBak.Text;
            });
            if (con.Equals(ccbt))
                return;
            var differ = con.StartsWith(ccbt) ? con[ccbt.Length..] : con;
            Dispatcher.Invoke(() =>
            {
                ChatContentBak.Text = con;
            });
            ccbt = con;
            var all = differ.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
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
                        content = Hiro_Utils.Get_Translate("msgnew").Replace("%u", user);
                        break;
                    case 2:
                        if (App.Locked)
                            content = Hiro_Utils.Get_Translate("msgnew").Replace("%u", user);
                        break;
                    default:
                        break;
                }
                var cont = content.Trim();
                if (cont.Contains("[Hiro.Predefinited:LikeAvatar]"))
                    content = Hiro_Utils.Get_Translate("avatarlike");
                if (cont.Contains("[Hiro.Predefinited:LikeProfile]"))
                    content = Hiro_Utils.Get_Translate("profilelike");
                if (cont.Contains("[Hiro.Predefinited:LikeName]"))
                    content = Hiro_Utils.Get_Translate("namelike");
                if (cont.Contains("[Hiro.Predefinited:LikeSign]"))
                    content = Hiro_Utils.Get_Translate("signlike");
                var rgx = new Regex(@"\[Hiro.Emoji:[\S].*?\]");
                content = rgx.Replace(cont, Hiro_Utils.Get_Translate("emojitxt"));

                if (i != 0)
                {
                    var StrFileName = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\friends\\list.hfl");
                    StrFileName = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(StrFileName, UserId, "Avatar", string.Empty)));
                    App.Notify(new(user + ": " + content, 2, user, new(() => { Hiro_Utils.RunExe("chat()", user, false); }), new() { Location = StrFileName }));
                }
                if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "MessageAudio", "1").Equals("1"))
                    try
                    {
                        var fileP = Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(App.dconfig, "Config", "MessageAudioPath", string.Empty));
                        if (!File.Exists(fileP))
                            fileP = Hiro_Utils.Path_Prepare("<win>\\Media\\Windows Notify Messaging.wav");
                        System.Media.SoundPlayer sndPlayer = new(fileP);
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
                        AddErrorMsg(Hiro_Utils.Get_Translate("chatsys"), Hiro_Utils.Get_Translate("chatsenderror"));
                    });
                else if (res.Equals("IDENTIFY_ERROR"))
                {
                    Hiro_Utils.Logout();
                    Dispatcher.Invoke(() =>
                    {
                        Hiro_Main?.Set_Label(Hiro_Main.loginx);
                        App.Notify(new(Hiro_Utils.Get_Translate("lgexpired"), 2, Hiro_Utils.Get_Translate("chat")));
                    });
                }

            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.Chat.Send");
                Dispatcher.Invoke(() =>
                {
                    AddErrorMsg(Hiro_Utils.Get_Translate("chatsys"), Hiro_Utils.Get_Translate("chatsenderror"));
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
                        AddErrorMsg(Hiro_Utils.Get_Translate("chatsys"), Hiro_Utils.Get_Translate("chatdeve"));
                    });
                }
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.Chat.Update.Nickname");
                Dispatcher.Invoke(() =>
                {
                    AddErrorMsg(Hiro_Utils.Get_Translate("chatsys"), Hiro_Utils.Get_Translate("chatnonick"));
                });
            }
        }
        public void Load_Translate()
        {
            SendButton.Content = Hiro_Utils.Get_Translate("chatsend");
            EMojiButton.Content = Hiro_Utils.Get_Translate("emojibtn");
        }

        public void Load_Position()
        {
            Hiro_Utils.Set_Control_Location(ChatContent, "chatcontent");
            Hiro_Utils.Set_Control_Location(SendContent, "chatsendcontent");
            Hiro_Utils.Set_Control_Location(SendButton, "chatsend", right: true, bottom: true);
            Hiro_Utils.Set_Control_Location(EMojiButton, "emojibtn", right: false, bottom: true);
            Hiro_Utils.Set_Control_Location(Profile_Nickname_Indexer, "chatname");
            Hiro_Utils.Set_Control_Location(Profile_Signature_Indexer, "chatsign");
            Hiro_Utils.Set_Control_Location(Profile_Mac, "chatid");
            Hiro_Utils.Set_Control_Location(Profile_Background, "chatback");
            Hiro_Utils.Set_FrameworkElement_Location(Profile_Ellipse, "chatavatar");
            Hiro_Utils.Set_FrameworkElement_Location(Profile_Rectangle, "chatavatar");
            Hiro_Utils.Set_FrameworkElement_Location(Profile, "chatprofile");
            Hiro_Utils.Set_FrameworkElement_Location(Emoji_Container, "emojipanel");
            Hiro_Utils.Set_FrameworkElement_Location(Emoji_Platte, "emojipalette");
            Hiro_Utils.Set_FrameworkElement_Location(scrollviewer, "emojiscroll");
            Hiro_Utils.Set_FrameworkElement_Location(CateViewer, "emojicate");
            Profile_Mac.Margin = new Thickness(Profile_Nickname.Margin.Left + Profile_Nickname.ActualWidth + 5, Profile_Nickname.Margin.Top + Profile_Nickname.ActualHeight - Profile_Mac.ActualHeight, 0, 0);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            Send(GetSendContent());
            SendContent.Document.Blocks.Clear();
        }

        private string GetSendContent()
        {
            var ret = string.Empty;
            for (int i = 0; i < SendContent.Document.Blocks.Count; i++)
            {
                if (i == 0)
                    ret = GetBlockContent(SendContent.Document.Blocks.ElementAt(i));
                else
                    ret = ret + Environment.NewLine + GetBlockContent(SendContent.Document.Blocks.ElementAt(i));
            }
            return ret;
        }

        private string GetBlockContent(Block block)
        {
            var ret = string.Empty;
            var para = block as Paragraph;
            if (para == null)
                return ret;
            for (int i = 0; i < para.Inlines.Count; i++)
            {
                var inline = para.Inlines.ElementAt(i);
                switch (inline)
                {
                    case Run r:
                        ret += r.Text;
                        break;
                    case LineBreak l:
                        ret += Environment.NewLine;
                        break;
                    case InlineUIContainer iuc:
                        try
                        {
                            var id = iuc.Tag;
                            if (id != null)
                                ret += id.ToString();
                        }
                        catch { };
                        break;
                    default:
                        break;
                }
            }
            return ret;
        }

        private void Send(string text)
        {
            if (text.ToLower().Equals("refreash"))
            {
                SendContent.Document.Blocks.Clear();
                Hiro_Get_Chat();
                return;
            }
            if (text.ToLower().StartsWith("version:"))
            {
                Hiro_Utils.version = "v" + text[8..];
                Hiro_Utils.LogtoFile("[INFO]Chat Version Updated " + "v" + text[8..]);
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "AppVer", text[8..]);
                Hiro_Get_Chat();
                return;
            }
            if (text.ToLower().StartsWith("talkto:"))
            {
                UserId = text[7..];
                Hiro_Utils.LogtoFile("[INFO]Talk to " + UserId);
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "ChatID", UserId);
                SendContent.Document.Blocks.Clear();
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
                var fp = SendContent.CaretPosition.Paragraph;
                SendContent.CaretPosition.InsertLineBreak();
                var lb = SendContent.CaretPosition.GetAdjacentElement(LogicalDirection.Forward) as Inline;
                if (lb != null)
                {
                    SendContent.CaretPosition = lb.ContentEnd;
                }
                else
                if (fp == null)
                {

                    SendContent.CaretPosition = SendContent.Document.ContentEnd;
                }
                else
                {
                    var fn = fp.NextBlock;
                    if (fn == null)
                    {
                        var fae = SendContent.CaretPosition.Paragraph.NextBlock;
                        if (fae != null)
                            SendContent.CaretPosition = fae.ContentStart;
                    }
                    else
                        SendContent.CaretPosition = fn.PreviousBlock.ContentStart;

                }
                e.Handled = true;
            }
            else
            {
                if (e.KeyStates == Keyboard.GetKeyStates(Key.Return))
                {
                    Send(GetSendContent());
                    SendContent.Document.Blocks.Clear();
                    e.Handled = true;
                }
            }

        }

        private void EMojiButton_Click(object sender, RoutedEventArgs e)
        {
            if (Emoji_Container.Visibility == Visibility.Visible)
            {
                if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
                {
                    var sb = Hiro_Utils.AddDoubleAnimaton(0, 450, Emoji_Container, "Opacity", null);
                    sb.Completed += delegate
                    {
                        Emoji_Container.Visibility = Visibility.Hidden;
                    };
                    sb.Begin();
                }
                else
                    Emoji_Container.Visibility = Visibility.Hidden;
            }
            else
            {
                if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
                {
                    Emoji_Container.Visibility = Visibility.Visible;
                    var sb = Hiro_Utils.AddPowerAnimation(1, Emoji_Container, null, 50);
                    sb.Begin();
                }
                else
                    Emoji_Container.Visibility = Visibility.Visible;
            }
        }

        internal void Load_Emoji(string directory)
        {
            var cf = directory + "\\" + new DirectoryInfo(directory).Name + ".hes";
            var w = 0.0;
            var h = 5.0;
            var cw = -30.0;
            var bw = 25.0;
            var etxt = Hiro_Utils.Read_Ini(cf, "Emoji", "IsExtra", string.Empty).ToLower();
            if (etxt.Equals("true"))
            {
                bw = 50.0;
                cw = -55.0;
            }
            else if (etxt.Equals("extra"))
            {
                bw = 100.0;
                cw = -105.0;
            };
            Dispatcher.Invoke(() =>
            {
                Emoji_Platte.Children.Clear();
                Emoji_Platte.Margin = new Thickness(0);
                w = scrollviewer.Width;
            });
            for (int i = 1; true; i++)
            {
                var img = directory + "\\" + i.ToString("D4");
                if (!File.Exists(img))
                {
                    break;
                }
                Image? im = null;
                Dispatcher.Invoke(() =>
                {
                    im = new Image()
                    {
                        Source = new BitmapImage(new Uri(img, UriKind.Absolute)),
                        Stretch = Stretch.Uniform,
                        Width = bw,
                        Height = bw,
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        ToolTip = Hiro_Utils.Read_Ini(cf, App.lang, System.IO.Path.GetFileName(img), Hiro_Utils.Read_Ini(cf, "zh-CN", System.IO.Path.GetFileName(img), "<???>"))
                    };
                });
                im.MouseDown += delegate
                {
                    Dispatcher.Invoke(() =>
                    {
                        SendContent.Selection.Select(SendContent.Selection.End, SendContent.Selection.End);
                        var cp = SendContent.CaretPosition;
                        var pi = cp.Parent as Inline;
                        var par = cp.Paragraph;
                        inline = pi ?? inline;
                        var tagtxt = $"[Hiro.Emoji:{new DirectoryInfo(directory).Name + "\\" + System.IO.Path.GetFileName(img)}]";
                        if (par == null)
                        {
                            par = SendContent.Document.Blocks.LastBlock as Paragraph;
                            if (par == null)
                            {
                                par = new();
                                SendContent.Document.Blocks.Add(par);
                            }
                        }
                        if (SendContent.Document.Blocks.Count <= 0 || par.Inlines.Count <= 0)
                        {
                            pi = null;
                            inline = null;
                        }
                        if (pi != null)
                        {
                            if (pi is InlineUIContainer)
                            {
                                Insert_Picture_After(par, new BitmapImage(new Uri(img, UriKind.Absolute)), pi, tagtxt, im.Width);
                                inline = pi;
                            }
                            else
                            {
                                var ppre = pi.PreviousInline;
                                var pnex = pi.NextInline;
                                var pitext = new TextRange(pi.ContentStart, SendContent.CaretPosition).Text;
                                var pitextend = new TextRange(SendContent.CaretPosition, pi.ContentEnd).Text;
                                var p1 = new Run(pitext);
                                var p2 = new Run(pitextend);
                                if (ppre == null)
                                {
                                    if (pnex == null)
                                    {
                                        //pi is the only inline
                                        par.Inlines.Clear();
                                        par.Inlines.Add(p1);
                                        Insert_Picture_After(par, new BitmapImage(new Uri(img, UriKind.Absolute)), p1, tagtxt, im.Width);
                                        par.Inlines.Add(p2);
                                    }
                                    else
                                    {
                                        //pi is the first inline
                                        par.Inlines.Remove(pi);
                                        par.Inlines.InsertBefore(pnex, p1);
                                        Insert_Picture_Before(par, new BitmapImage(new Uri(img, UriKind.Absolute)), pnex, tagtxt, im.Width);
                                        par.Inlines.InsertBefore(pnex, p2);
                                    }
                                }
                                else
                                {
                                    //pi is not the first inline
                                    par.Inlines.Remove(pi);
                                    par.Inlines.InsertAfter(ppre, p2);
                                    Insert_Picture_After(par, new BitmapImage(new Uri(img, UriKind.Absolute)), ppre, tagtxt, im.Width);
                                    par.Inlines.InsertAfter(ppre, p1);
                                }
                                if (p1.NextInline != null)
                                    inline = p1.NextInline;
                                else
                                    inline = p1;
                            }

                        }
                        else
                        {
                            var pnex = SendContent.CaretPosition.GetAdjacentElement(LogicalDirection.Forward) as Inline;
                            var ppre = SendContent.CaretPosition.GetAdjacentElement(LogicalDirection.Backward) as Inline;
                            if (pnex == null)
                            {
                                if (ppre == null)
                                {
                                    par.Inlines.Clear();
                                }
                                inline = Insert_Picture_After(par, new BitmapImage(new Uri(img, UriKind.Absolute)), ppre, tagtxt, im.Width);
                            }
                            else
                            {
                                inline = Insert_Picture_Before(par, new BitmapImage(new Uri(img, UriKind.Absolute)), pnex, tagtxt, im.Width);
                            }
                        }
                        SendContent.CaretPosition = inline?.ElementEnd;
                        if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
                        {
                            var sb = Hiro_Utils.AddDoubleAnimaton(0, 450, Emoji_Container, "Opacity", null);
                            sb.Completed += delegate
                            {
                                Emoji_Container.Visibility = Visibility.Hidden;
                            };
                            sb.Begin();
                        }
                        else
                            Emoji_Container.Visibility = Visibility.Hidden;
                    });
                };
                im.MouseEnter += delegate
                {
                    Dispatcher.Invoke(() =>
                    {
                        var m = new Thickness(im.Margin.Left - 5, im.Margin.Top - 5, 0, 0);
                        if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
                        {
                            Storyboard sb = new();
                            Hiro_Utils.AddThicknessAnimaton(m, 250, FocusLabel, "Margin", sb);
                            Hiro_Utils.AddDoubleAnimaton(im.Height + 10, 250, FocusLabel, "Height", sb);
                            Hiro_Utils.AddDoubleAnimaton(im.Width + 10, 250, FocusLabel, "Width", sb);
                            Hiro_Utils.AddDoubleAnimaton(1, 250, FocusLabel, "Opacity", sb);
                            sb.Completed += delegate
                            {
                                FocusLabel.Width = im.Width + 10;
                                FocusLabel.Height = im.Height + 10;
                                FocusLabel.Margin = m;
                                FocusLabel.Opacity = 1;
                            };
                            sb.Begin();
                        }
                        else
                        {
                            FocusLabel.Width = im.Width + 10;
                            FocusLabel.Height = im.Height + 10;
                            FocusLabel.Margin = m;
                            FocusLabel.Opacity = 1;
                        }


                    });

                };
                if (cw + (bw + 10) * 2 > w)
                {
                    h = h + bw + 10;
                    cw = 5;
                }
                else
                {
                    cw = cw + bw + 10;
                }
                Dispatcher.Invoke(() =>
                {
                    im.Margin = new Thickness(cw, h, 0, 0);
                    Emoji_Platte.Children.Add(im);
                });
            }
            Dispatcher.Invoke(() =>
            {
                Emoji_Platte.Height = h + bw + 10;
                scrollviewer.ScrollToHome();
                if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
                    Hiro_Utils.AddPowerAnimation(1, Emoji_Platte, null, 50).Begin();
            });
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
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            var tag = xflag ? "chatidx" : "chatid";
            Hiro_Utils.Set_Mac_Location(Profile_Mac, tag, Profile_Nickname, animation: animation, animationTime: 250);
        }

        private void Profile_Mac_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(Profile_Mac.Content.ToString());
            var StrFileName = Hiro_Utils.Path_Prepare("<hiapp>\\chat\\friends\\list.hfl");
            StrFileName = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(StrFileName, UserId, "Avatar", string.Empty)));
            App.Notify(new(Hiro_Utils.Get_Translate("chatmcopy").Replace("%u", Aite), 2, Hiro_Utils.Get_Translate("chat"), null, new() { Location = StrFileName }));
        }

        private void Profile_MouseEnter(object sender, MouseEventArgs e)
        {
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            xflag = true;
            Hiro_Utils.Set_FrameworkElement_Location(Profile_Ellipse, "chatavatarx", animation: animation, animationTime: 250);
            Hiro_Utils.Set_FrameworkElement_Location(Profile_Rectangle, "chatavatarx", animation: animation, animationTime: 250);
            Hiro_Utils.Set_FrameworkElement_Location(Profile, "chatprofilex", animation: animation, animationTime: 250);
            Hiro_Utils.Set_Control_Location(Profile_Nickname_Indexer, "chatnamex", animation: animation, animationTime: 250);
            Hiro_Utils.Set_Control_Location(Profile_Signature_Indexer, "chatsignx", animation: animation, animationTime: 250);
            Hiro_Utils.Set_Control_Location(Profile_Background, "chatbackx", animation: animation, animationTime: 250);
            e.Handled = true;
        }

        private void Profile_MouseLeave(object sender, MouseEventArgs e)
        {
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            xflag = false;
            Hiro_Utils.Set_FrameworkElement_Location(Profile_Ellipse, "chatavatar", animation: animation, animationTime: 250);
            Hiro_Utils.Set_FrameworkElement_Location(Profile_Rectangle, "chatavatar", animation: animation, animationTime: 250);
            Hiro_Utils.Set_FrameworkElement_Location(Profile, "chatprofile", animation: animation, animationTime: 250);
            Hiro_Utils.Set_Control_Location(Profile_Nickname_Indexer, "chatname", animation: animation, animationTime: 250);
            Hiro_Utils.Set_Control_Location(Profile_Signature_Indexer, "chatsign", animation: animation, animationTime: 250);
            Hiro_Utils.Set_Control_Location(Profile_Background, "chatback", animation: animation, animationTime: 250);
            e.Handled = true;
        }



        private void Emoji_Platte_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender == Emoji_Platte)
            {
                if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
                {
                    Storyboard sb = new();
                    Hiro_Utils.AddDoubleAnimaton(0, 250, FocusLabel, "Opacity", sb);
                    sb.Completed += delegate
                    {
                        FocusLabel.Opacity = 0;
                    };
                    sb.Begin();
                }
                else
                    FocusLabel.Opacity = 0;
            }

        }

        private InlineUIContainer Insert_Picture_After(Paragraph pa, BitmapImage bi, Inline? current, string id, double bw)
        {
            InlineUIContainer iuc = new InlineUIContainer(new Image() { Source = bi, Stretch = Stretch.UniformToFill, Width = bw, Height = bw }) { Tag = id, BaselineAlignment = BaselineAlignment.Center };
            if (current == null)
                pa.Inlines.Add(iuc);
            else
                pa.Inlines.InsertAfter(current, iuc);
            return iuc;
        }

        private InlineUIContainer Insert_Picture_Before(Paragraph pa, BitmapImage bi, Inline? current, string id, double bw)
        {
            InlineUIContainer iuc = new InlineUIContainer(new Image() { Source = bi, Stretch = Stretch.UniformToFill, Width = bw, Height = bw }) { Tag = id, BaselineAlignment = BaselineAlignment.Center };
            if (current == null)
                pa.Inlines.Add(iuc);
            else
                pa.Inlines.InsertBefore(current, iuc);
            return iuc;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {

        }
    }
}
