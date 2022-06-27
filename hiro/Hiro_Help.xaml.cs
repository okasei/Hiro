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
        private Hiro_MainUI? Hiro_Main = null;
        public Hiro_Help(Hiro_MainUI? Parent)
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
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            Storyboard sb = new();
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                Hiro_Utils.AddPowerAnimation(0, this, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(1, btn9, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(1, btn8, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(0, tb6, sb, -100, null);
            }
            if (animation)
            {
                Hiro_Utils.AddPowerAnimation(0, this, sb, 50, null);
                sb.Begin();
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

        public void Load_Translate()
        {
            btn8.Content = Hiro_Utils.Get_Transalte("feedback");
            btn9.Content = Hiro_Utils.Get_Transalte("whatsnew");
            tb6.Text = Hiro_Utils.Get_Transalte("helptext") + Hiro_Utils.Get_Transalte("helptext_ext") + Hiro_Utils.Get_Transalte("helptext_ext2");
        }

        public void Load_Position()
        {
            Hiro_Utils.Set_Control_Location(btn8, "feedback");
            Hiro_Utils.Set_Control_Location(btn9, "whatsnew");
            Hiro_Utils.Set_Control_Location(tb6, "helptb");
        }

        private void Btn8_Click(object sender, RoutedEventArgs e)
        {
            btn8.IsEnabled = false;
            Hiro_Main?.Set_Label(Hiro_Main.chatx);
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
                wps = Hiro_Utils.GetWebContent("https://ftp.rexio.cn/hiro/new.php?ver=" + res.ApplicationVersion + "&lang=" + App.lang);
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
                    Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.Update.Parse: " + ex.Message);
                }

            };
            whatsbw.RunWorkerAsync();
            Hiro_Utils.Delay(200);
            btn9.IsEnabled = true;
        }
    }
}
