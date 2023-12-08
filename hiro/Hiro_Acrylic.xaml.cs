using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
{
    /// <summary>
    /// Hiro_Acrylic.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Acrylic : Page
    {
        private Hiro_MainUI? Hiro_Main = null;
        private bool bflag = false;
        public Hiro_Acrylic(Hiro_MainUI? Parent)
        {
            InitializeComponent();
            Hiro_Main = Parent;
            Hiro_Initialize();
            Loaded += delegate
            {
                HiHiro();
                Load_Settings();
            };
        }

        internal void Load_Settings()
        {
            switch (Hiro_Utils.Read_Ini(App.dConfig, "Config", "AcrylicMode", "2"))
            {
                case "0":
                    {
                        ColorWhite.IsChecked = true;
                        break;
                    }
                case "1":
                    {
                        ColorBlack.IsChecked = true;
                        break;
                    }
                default:
                    {
                        ColorCustomize.IsChecked = true;
                        break;
                    }
            }
            ColorBtn.IsEnabled = ColorCustomize.IsChecked ?? false;
            var trans = 0x47;
            if (!int.TryParse(Hiro_Utils.Read_Ini(App.dConfig, "Config", "AcrylicTransparency", "71"), out trans))
                trans = 0x47;
            trans = Math.Max(trans, 1);
            trans = Math.Min(trans, 255);
            TransparentSlider.Value = trans;
            switch (Hiro_Utils.Read_Ini(App.dConfig, "Config", "AcrylicMain", "0"))
            {
                case "1":
                    {
                        HalfPure.IsChecked = true;
                        break;
                    }
                case "2":
                    {
                        HalfImage.IsChecked = true;
                        break;
                    }
                default:
                    {
                        NoHalf.IsChecked = true;
                        break;
                    }
            }
            bflag = true;
        }
        public void HiHiro()
        {
            bool animation = !Hiro_Utils.Read_Ini(App.dConfig, "Config", "Ani", "2").Equals("0");
            Storyboard sb = new();
            if (Hiro_Utils.Read_Ini(App.dConfig, "Config", "Ani", "2").Equals("1"))
            {
                Hiro_Utils.AddPowerAnimation(1, AcrylicTitle, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(1, AcrylicColorGrid, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(1, AcrylicTransparentGrid, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(1, AcrylicColorGrid, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(3, AcrylicCancel, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(3, AcrylicOK, sb, -50, null);
            }
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
            if (!bflag)
            {
                TransparentSlider.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(TransparentSlider_MouseLeftButtonUp), true);
            }
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        public void Load_Translate()
        {
            AcrylicTitle.Content = Hiro_Utils.Get_Translate("AcrylicTitle");
            ColorTitle.Content = Hiro_Utils.Get_Translate("AcrylicColor");
            ColorWhite.Content = Hiro_Utils.Get_Translate("AcrylicWhite");
            ColorBlack.Content = Hiro_Utils.Get_Translate("AcrylicBlack");
            ColorCustomize.Content = Hiro_Utils.Get_Translate("AcrylicCustomize");
            ColorBtn.Content = Hiro_Utils.Get_Translate("AcrylicColorBtn");
            TransparentTitle.Content = Hiro_Utils.Get_Translate("AcrylicTransparent");
            HalfTitle.Content = Hiro_Utils.Get_Translate("AcrylicHalf");
            HalfPure.Content = Hiro_Utils.Get_Translate("AcrylicPure");
            HalfImage.Content = Hiro_Utils.Get_Translate("AcrylicImage");
            NoHalf.Content = Hiro_Utils.Get_Translate("AcrylicNoHalf");
            AcrylicCancel.Content = Hiro_Utils.Get_Translate("AcrylicCancel");
            AcrylicOK.Content = Hiro_Utils.Get_Translate("AcrylicOK");
        }

        public void Load_Position()
        {
            Hiro_Utils.Set_FrameworkElement_Location(AcrylicColorGrid, "AcrylicColorGrid");
            Hiro_Utils.Set_FrameworkElement_Location(AcrylicHalfGrid, "AcrylicHalfGrid");
            Hiro_Utils.Set_FrameworkElement_Location(AcrylicTransparentGrid, "AcrylicTransparentGrid");
            Hiro_Utils.Set_Control_Location(AcrylicTitle, "AcrylicTitle");
            Hiro_Utils.Set_Control_Location(ColorTitle, "AcrylicColor");
            Hiro_Utils.Set_Control_Location(ColorWhite, "AcrylicWhite");
            Hiro_Utils.Set_Control_Location(ColorBlack, "AcrylicBlack");
            Hiro_Utils.Set_Control_Location(ColorCustomize, "AcrylicCustomize");
            Hiro_Utils.Set_Control_Location(ColorBtn, "AcrylicColorBtn");
            Hiro_Utils.Set_Control_Location(TransparentTitle, "AcrylicTransparent");
            Hiro_Utils.Set_Control_Location(TransparentSlider, "AcrylicSlider");
            Hiro_Utils.Set_Control_Location(HalfTitle, "AcrylicHalf");
            Hiro_Utils.Set_Control_Location(HalfPure, "AcrylicPure");
            Hiro_Utils.Set_Control_Location(HalfImage, "AcrylicImage");
            Hiro_Utils.Set_Control_Location(NoHalf, "AcrylicNoHalf");
            Hiro_Utils.Set_Control_Location(AcrylicCancel, "AcrylicCancel", bottom: true, right: true);
            Hiro_Utils.Set_Control_Location(AcrylicOK, "AcrylicOK", bottom: true, right: true);
        }

        private void ColorWhite_Checked(object sender, RoutedEventArgs e)
        {
            if (!bflag)
                return;
            ColorBtn.IsEnabled = false;
            Update_MainUI();
        }

        private void ColorBlack_Checked(object sender, RoutedEventArgs e)
        {
            if (!bflag)
                return;
            ColorBtn.IsEnabled = false;
            Update_MainUI();
        }

        private void ColorCustomize_Checked(object sender, RoutedEventArgs e)
        {
            if (!bflag)
                return;
            ColorBtn.IsEnabled = true;
            Update_MainUI();
        }

        private void ColorBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!bflag)
                return;
            Hiro_Main?.Set_Label(Hiro_Main.colorx);
        }

        private void NoHalf_Checked(object sender, RoutedEventArgs e)
        {
            if (!bflag)
                return;
            Hiro_Main?.Set_AcrylicStyle(true, 0);
        }

        private void HalfPure_Checked(object sender, RoutedEventArgs e)
        {

            if (!bflag)
                return;
            Hiro_Main?.Set_AcrylicStyle(true, 1);
        }

        private void HalfImage_Checked(object sender, RoutedEventArgs e)
        {

            if (!bflag)
                return;
            Hiro_Main?.Set_AcrylicStyle(true, 2);
        }

        private void AcrylicCancel_Click(object sender, RoutedEventArgs e)
        {
            Hiro_Main?.Set_Label(Hiro_Main.configx);
            Hiro_Main?.Blurbgi(0);
        }

        private void AcrylicOK_Click(object sender, RoutedEventArgs e)
        {
            if (ColorWhite.IsChecked == true)
                Hiro_Utils.Write_Ini(App.dConfig, "Config", "AcrylicMode", "0");
            if (ColorBlack.IsChecked == true)
                Hiro_Utils.Write_Ini(App.dConfig, "Config", "AcrylicMode", "1");
            if (ColorCustomize.IsChecked == true)
                Hiro_Utils.Write_Ini(App.dConfig, "Config", "AcrylicMode", "2");
            if (NoHalf.IsChecked == true)
                Hiro_Utils.Write_Ini(App.dConfig, "Config", "AcrylicMain", "0");
            if (HalfPure.IsChecked == true)
                Hiro_Utils.Write_Ini(App.dConfig, "Config", "AcrylicMain", "1");
            if (HalfImage.IsChecked == true)
                Hiro_Utils.Write_Ini(App.dConfig, "Config", "AcrylicMain", "2");
            Hiro_Main?.Set_Label(Hiro_Main.configx);
            Hiro_Main?.Blurbgi(0);
        }

        private void Update_MainUI()
        {
            if (Hiro_Main != null)
            {
                Hiro_Main.compositor ??= new(Hiro_Main);
                Color color = App.AppAccentColor;
                if (ColorWhite.IsChecked == true)
                    color = Colors.White;
                if (ColorBlack.IsChecked == true)
                    color = Colors.Black;
                Hiro_Main.compositor.Color = Color.FromArgb((byte)TransparentSlider.Value, color.R, color.G, color.B);
                Hiro_Main.compositor.IsEnabled = true;
            }
        }

        private void TransparentSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!bflag)
                return;
            var trans = TransparentSlider.Value;
            trans = Math.Max(trans, 1);
            trans = Math.Min(trans, 255);
            Hiro_Utils.Write_Ini(App.dConfig, "Config", "AcrylicTransparency", trans.ToString());
            Update_MainUI();
        }
    }
}
