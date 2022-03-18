using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
{
    /// <summary>
    /// Hiro_About.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_About : Page
    {
        private Mainui? Hiro_Main = null;
        internal System.ComponentModel.BackgroundWorker? upbw = null;
        private string ups = "latest";
        public Hiro_About(Mainui? Parent)
        {
            InitializeComponent();
            Hiro_Main = Parent;
            Hiro_Initialize();
            Loaded += Hiro_About_Loaded;
        }

        private void Hiro_About_Loaded(object sender, RoutedEventArgs e)
        {
            bool animation = !utils.Read_Ini(App.dconfig, "config", "ani", "1").Equals("0");
            if (animation)
                BeginStoryboard(Application.Current.Resources["AppLoad"] as Storyboard);
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
            Resources["AppAccent"] = new SolidColorBrush(utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        public void Load_Translate()
        {
            chk_btn.Content = utils.Get_Transalte("checkup");
        }

        public void Load_Position()
        {
            utils.Set_Control_Location(chk_btn, "checkup");
        }

        private void Avatar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Hiro_Main != null)
                Hiro_Main.Hiro_We_Extend();
        }

        private void Chk_btn_Click(object sender, RoutedEventArgs e)
        {
            if (chk_btn.Content.Equals(utils.Get_Transalte("checkup")))
            {
                chk_btn.Content = utils.Get_Transalte("checkcancel");
                if (Hiro_Main != null)
                    Hiro_Main.pb.Visibility = Visibility.Visible;
                if (upbw != null)
                    upbw.CancelAsync();
                upbw = new();
                upbw.WorkerSupportsCancellation = true;
                upbw.DoWork += delegate
                {
                    ups = utils.GetWebContent("https://ftp.rexio.cn/hiro/hiro.php?r=update&v=" + res.ApplicationUpdateVersion + "&lang=" + App.lang);
                };
                upbw.RunWorkerCompleted += delegate
                {
                    Check_update();
                };
                upbw.RunWorkerAsync();
            }
            else
            {
                chk_btn.Content = utils.Get_Transalte("checkup");
                if (Hiro_Main != null)
                    Hiro_Main.pb.Visibility = Visibility.Hidden;

            }
        }

        public void Check_update()
        {
            chk_btn.Content = utils.Get_Transalte("checkup");
            if (Hiro_Main != null)
                Hiro_Main.pb.Visibility = Visibility.Hidden;
            if (ups == "latest")
            {
                App.Notify(new noticeitem(utils.Get_Transalte("updatelatest"), 2, utils.Get_Transalte("checkup")));
            }
            else if (ups == "Error")
            {
                App.Notify(new noticeitem(utils.Get_Transalte("updateerror"), 2, utils.Get_Transalte("checkup")));
            }
            else
            {
                try
                {
                    string version = ups[(ups.IndexOf("version:[") + "version:[".Length)..];
                    version = version[..version.IndexOf("]")];
                    string info = ups[(ups.IndexOf("info:[") + "info:[".Length)..];
                    info = info[..info.IndexOf("]")].Replace("\\n", Environment.NewLine);
                    string url = ups[(ups.IndexOf("url:[") + "url:[".Length)..];
                    url = url[..url.IndexOf("]")];
                    if (utils.Read_Ini(App.dconfig, "config", "toast", "0").Equals("1"))
                    {
                        new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                                .AddText(utils.Get_Transalte("updatetitle"))
                                .AddText(utils.Get_Transalte("updatecontent").Replace("%v", version).Replace("%n", info).Replace("\\n", Environment.NewLine))
                                .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                            .SetContent(utils.Get_Transalte("updateok"))
                                            .AddArgument("action", "uok"))
                                .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                            .SetContent(utils.Get_Transalte("updateskip"))
                                            .AddArgument("action", "uskip"))
                                .AddArgument("url", url)
                            .Show();
                    }
                    else
                    {
                        Alarm up = new(-415, utils.Get_Transalte("updatetitle"), utils.Path_Prepare_EX(utils.Get_Transalte("updatecontent").Replace("%v", version).Replace("%n", info).Replace("\\n", Environment.NewLine).Replace("<br>", Environment.NewLine)), 2);
                        up.url = url;
                        up.Show();
                    }
                }
                catch (Exception ex)
                {
                    utils.LogtoFile("[ERROR]" + ex.Message);
                    App.Notify(new noticeitem(utils.Get_Transalte("updateerror"), 2, utils.Get_Transalte("checkup")));
                }


            }

        }
    }
}
