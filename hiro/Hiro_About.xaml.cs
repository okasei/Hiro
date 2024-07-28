using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace hiro
{
    public partial class Hiro_About : Page
    {
        private Hiro_MainUI? Hiro_Main = null;
        private System.ComponentModel.BackgroundWorker? upbw = null;
        private string ups = "latest";
        internal bool ischecking = false;
        internal string formerIcon = "";
        internal string extendIcon = "";
        public Hiro_About(Hiro_MainUI? Parent)
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
            string iconPath = Hiro_Utils.Path_Prepare(Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Read_Ini(App.dConfig, "Config", "CustomizeAbout", "")));
            Storyboard sb = new();
            if (!iconPath.Equals(formerIcon) && System.IO.File.Exists(iconPath))
            {
                BaseIcon.ImageSource = Hiro_Utils.GetBitmapImage(iconPath);
                sb = Hiro_Utils.AddDoubleAnimaton(1, 450, BaseIcon, "Opacity", sb, 0, 0.7);
                formerIcon = iconPath;
            }
            bool animation = !Hiro_Utils.Read_Ini(App.dConfig, "Config", "Ani", "2").Equals("0");
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
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        public void Load_Translate()
        {
            if (ischecking)
                chk_btn.Content = Hiro_Utils.Get_Translate("checkcancel");
            else
                chk_btn.Content = Hiro_Utils.Get_Translate("checkup");
        }

        public void Load_Position()
        {
            Hiro_Utils.Set_Control_Location(chk_btn, "checkup");
            Hiro_Utils.Set_FrameworkElement_Location(avatar, "avatar");
        }

        private void Chk_btn_Click(object sender, RoutedEventArgs e)
        {
            if (ischecking)
            {
                ischecking = false;
                chk_btn.Content = Hiro_Utils.Get_Translate("checkup");
                if (Hiro_Main != null)
                    Hiro_Main.pb.Visibility = Visibility.Hidden;
            }
            else
            {
                ischecking = true;
                chk_btn.Content = Hiro_Utils.Get_Translate("checkcancel");
                if (Hiro_Main != null)
                    Hiro_Main.pb.Visibility = Visibility.Visible;
                upbw?.CancelAsync();
                upbw = new();
                upbw.WorkerSupportsCancellation = true;
                upbw.DoWork += delegate
                {
                    ups = Hiro_Utils.GetWebContent("https://hiro.rexio.cn/Update/hiro.php?r=update&v=" + Hiro_Resources.ApplicationUpdateVersion + "&lang=" + App.lang);
                };
                upbw.RunWorkerCompleted += delegate
                {
                    Hiro_Utils.LogtoFile(ups);
                    Check_update();
                };
                upbw.RunWorkerAsync();
            }
        }

        public void Check_update()
        {
            ischecking = false;
            chk_btn.Content = Hiro_Utils.Get_Translate("checkup");
            if (Hiro_Main != null)
                Hiro_Main.pb.Visibility = Visibility.Hidden;
            switch (ups)
            {
                case "latest":
                    App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("updatelatest"), 2, Hiro_Utils.Get_Translate("checkup")));
                    break;
                case "Error":
                    App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("updateerror"), 2, Hiro_Utils.Get_Translate("checkup")));
                    break;
                default:
                    try
                    {
                        if (System.IO.Directory.Exists(Hiro_Utils.Path_Prepare(Hiro_Utils.Path_Prepare_EX("<current>\\update\\"))))
                            Hiro_Utils.RunExe("Delete(<current>\\update\\");
                        string version = ups[(ups.IndexOf("version:[") + "version:[".Length)..];
                        version = version[..version.IndexOf("]")];
                        string info = ups[(ups.IndexOf("info:[") + "info:[".Length)..];
                        info = info[..info.IndexOf("]")].Replace("\\n", Environment.NewLine);
                        string url = ups[(ups.IndexOf("url:[") + "url:[".Length)..];
                        url = url[..url.IndexOf("]")];
                        var os = Hiro_Utils.Get_OSVersion();
                        if (os.IndexOf(".") != -1)
                            os = os[..os.IndexOf(".")];
                        if (Hiro_Utils.Read_Ini(App.dConfig, "Config", "Toast", "0").Equals("1") && int.TryParse(os, out int a) && a >= 10)
                        {
                            new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                            .AddText(Hiro_Utils.Get_Translate("updatetitle"))
                            .AddText(Hiro_Utils.Get_Translate("updatecontent").Replace("%v", version).Replace("%n", info).Replace("\\n", Environment.NewLine))
                            .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                .SetContent(Hiro_Utils.Get_Translate("updateok"))
                                .AddArgument("action", "uok"))
                            .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                .SetContent(Hiro_Utils.Get_Translate("updateskip"))
                                .AddArgument("action", "uskip"))
                            .AddArgument("url", url)
                            .Show();
                        }
                        else
                        {
                            Hiro_Alarm up = new(-415, Hiro_Utils.Get_Translate("updatetitle"), Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Get_Translate("updatecontent").Replace("%v", version).Replace("%n", info).Replace("\\n", Environment.NewLine).Replace("<br>", Environment.NewLine)), 2);
                            up.url = url;
                            up.Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        Hiro_Utils.LogError(ex, "Hiro.Exception.Update.Check");
                        App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("updateerror"), 2, Hiro_Utils.Get_Translate("checkup")));
                    }

                    break;
            }

        }

        private void Avatar_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!Hiro_Utils.Read_Ini(App.dConfig, "Config", "Ani", "2").Equals("0"))
            {
                Hiro_Utils.Set_FrameworkElement_Location(avatar, "avatarx", animation: true, animationTime: 250);
                Hiro_Utils.Set_Control_Location(chk_btn, ischecking ? "checkcancelx" : "checkupx", animation: true, animationTime: 250);
            }


        }

        private void Avatar_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!Hiro_Utils.Read_Ini(App.dConfig, "Config", "Ani", "2").Equals("0"))
            {
                Hiro_Utils.Set_FrameworkElement_Location(avatar, "avatar", animation: true, animationTime: 250);
                Hiro_Utils.Set_Control_Location(chk_btn, ischecking ? "checkcancel" : "checkup", animation: true, animationTime: 250);
            }
        }
    }
}
