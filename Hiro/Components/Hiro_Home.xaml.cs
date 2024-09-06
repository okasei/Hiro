using Hiro.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Hiro
{
    /// <summary>
    /// Hiro_Home.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Home : Page
    {
        public Hiro_Home()
        {
            InitializeComponent();
            Hiro_Initialize();
            Loaded += delegate
            {
                HiHiro();
            };
        }

        public void HiHiro()
        {
            bool animation = !HSet.Read_DCIni("Ani", "2").Equals("0");
            if (!animation) 
                return;
            Storyboard sb = new();
            HAnimation.AddPowerAnimation(0, this, sb, 50, null);
            sb.Begin();
        }
        public void Load_Position()
        {
            HUI.Set_Control_Location(Copyright, "copyright", location: false);
            HUI.Set_Control_Location(Hello, "chello", location: false);
        }

        private void Hiro_Initialize()
        {
            Load_Color();
            Update_Labels();
            Load_Position();
        }

        public void Update_Labels()
        {
            var hr = DateTime.Now.Hour;
            var morning = HSet.Read_Ini(App.langFilePath, "local", "morning", "[6,7,8,9,10]");
            var noon = HSet.Read_Ini(App.langFilePath, "local", "noon", "[11,12,13]");
            var afternoon = HSet.Read_Ini(App.langFilePath, "local", "afternoon", "[14,15,16,17,18]");
            var evening = HSet.Read_Ini(App.langFilePath, "local", "evening", "[19,20,21,22]");
            var night = HSet.Read_Ini(App.langFilePath, "local", "night", "[23,0,1,2,3,4,5]");
            morning = morning.Replace("[", "[,").Replace("]", ",]").Trim();
            noon = noon.Replace("[", "[,").Replace("]", ",]").Trim();
            afternoon = afternoon.Replace("[", "[,").Replace("]", ",]").Trim();
            evening = evening.Replace("[", "[,").Replace("]", ",]").Trim();
            night = night.Replace("[", "[,").Replace("]", ",]").Trim();
            if (morning.IndexOf($",{hr},") != -1)
            {
                Set_Labels("morning");
            }
            else if (noon.IndexOf($",{hr},") != -1)
            {
                Set_Labels("noon");
            }
            else if (afternoon.IndexOf($",{hr},") != -1)
            {
                Set_Labels("afternoon");
            }
            else if (evening.IndexOf($",{hr},") != -1)
            {
                Set_Labels("evening");
            }
            else if (night.IndexOf($",{hr},") != -1)
            {
                Set_Labels("night");
            }
        }

        private void Set_Labels(string val)
        {
            val = (App.CustomUsernameFlag == 0) ? HText.Get_Translate(val).Replace("%u", App.eUserName) : HText.Get_Translate(val + "cus").Replace("%u", App.username);
            if (!Hello.Text.Equals(val))
                Hello.Text = val;
            val = HText.Path_PPX(HText.Get_Translate("copyright"));
            if (!Copyright.Text.Equals(val))
                Copyright.Text = val;
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
        }
    }
}
