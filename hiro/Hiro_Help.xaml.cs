using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
{
    /// <summary>
    /// Hiro_Help.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Help : Page
    {
        internal System.ComponentModel.BackgroundWorker? whatsbw = null;
        public Hiro_Help()
        {
            InitializeComponent();
            Hiro_Initialize();
            Loaded += Hiro_Help_Loaded;
        }

        private void Hiro_Help_Loaded(object sender, RoutedEventArgs e)
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
            btn8.Content = utils.Get_Transalte("feedback");
            btn9.Content = utils.Get_Transalte("whatsnew");
            tb6.Text = utils.Get_Transalte("helptext") + utils.Get_Transalte("helptext_ext") + utils.Get_Transalte("helptext_ext2");
        }

        public void Load_Position()
        {
            utils.Set_Control_Location(btn8, "feedback");
            utils.Set_Control_Location(btn9, "whatsnew");
            utils.Set_Control_Location(tb6, "helptb");
        }

        private void Btn8_Click(object sender, RoutedEventArgs e)
        {
            btn8.IsEnabled = false;
            utils.RunExe("mailto:xboriver@live.cn");
            utils.Delay(200);
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
            string wps = "";
            whatsbw.DoWork += delegate
            {
                wps = utils.GetWebContent("https://ftp.rexio.cn/hiro/new.php?ver=" + res.ApplicationVersion + "&lang=" + App.lang);
            };
            whatsbw.RunWorkerCompleted += delegate
            {
                try
                {
                    string ti = wps[(wps.IndexOf("<title>") + "<title>".Length)..];
                    ti = ti[..ti.IndexOf("<")];
                    wps = wps[(wps.IndexOf("</head>") + "</head>".Length)..];
                    utils.RunExe("alarm(" + ti + "," + wps.Replace("<br>", "\\n") + ")");
                    whatsbw.Dispose();
                    whatsbw = null;
                }
                catch (Exception ex)
                {
                    utils.LogtoFile("[ERROR]" + ex.Message);
                }

            };
            whatsbw.RunWorkerAsync();
            utils.Delay(200);
            btn9.IsEnabled = true;
        }
    }
}
