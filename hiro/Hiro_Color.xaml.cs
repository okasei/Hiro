using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
{
    /// <summary>
    /// Hiro_Color.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Color : Page
    {
        private Mainui? Hiro_Main = null;
        private Hiro_Config? hc = null;
        public Hiro_Color(Mainui? Parent, Hiro_Config config)
        {
            InitializeComponent();
            Hiro_Main = Parent;
            hc = config;
            Hiro_Initialize();
            Loaded += Hiro_Color_Loaded;
        }

        private void Hiro_Color_Loaded(object sender, RoutedEventArgs e)
        {
            bool animation = !utils.Read_Ini(App.dconfig, "config", "ani", "1").Equals("0");
            if (animation)
                BeginStoryboard(Application.Current.Resources["AppLoad"] as Storyboard);
        }

        private void Hiro_Initialize()
        {
            color_picker.MouseMove += Color_picker_ColorChanged;
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
            color_title.Content = utils.Get_Transalte("cotitle");
            color_ex.Content = utils.Get_Transalte("coex").Replace("\\n", Environment.NewLine);
            cobtn1.Content = utils.Get_Transalte("cook");
            cobtn2.Content = utils.Get_Transalte("cocancel");
            cobtn3.Content = utils.Get_Transalte("coreset");
        }

        public void Load_Position()
        {
            utils.Set_Control_Location(color_title, "cotitle");
            utils.Set_Control_Location(color_picker, "copicker");
            utils.Set_Control_Location(color_text, "coval");
            utils.Set_Control_Location(color_ex, "coex");
            utils.Set_Control_Location(cobtn1, "cook", bottom: true, right: true);
            utils.Set_Control_Location(cobtn2, "cocancel", bottom: true, right: true);
            utils.Set_Control_Location(cobtn3, "coreset", bottom: true);
        }


        private void Cobtn3_Click(object sender, RoutedEventArgs e)
        {
            utils.Write_Ini(App.dconfig, "config", "lockcolor", "default");
            if (App.wnd != null)
                App.wnd.Load_All_Colors();
            if (Hiro_Main != null)
            {
                Hiro_Main.Set_Label(Hiro_Main.configx);
                Hiro_Main.current = hc;
                Hiro_Main.frame.Content = hc;
            }
            
        }

        private void Cobtn1_Click(object sender, RoutedEventArgs e)
        {
            App.AppAccentColor = color_picker.Color;
            utils.Write_Ini(App.dconfig, "config", "lockcolor", string.Format("#{0:X2}{1:X2}{2:X2}", App.AppAccentColor.R, App.AppAccentColor.G, App.AppAccentColor.B));
            if (App.wnd != null)
                App.wnd.Load_All_Colors();
            if (Hiro_Main != null)
            {
                Hiro_Main.Set_Label(Hiro_Main.configx);
                Hiro_Main.current = hc;
                Hiro_Main.frame.Content = hc;
            }
        }

        private void Cobtn2_Click(object sender, RoutedEventArgs e)
        {
            if (Hiro_Main != null)
            {
                Hiro_Main.Set_Label(Hiro_Main.configx);
                Hiro_Main.current = hc;
                Hiro_Main.frame.Content = hc;
            }
        }

        private void Color_picker_ColorChanged(object sender, MouseEventArgs e)
        {
            Unify_Color();
        }

        internal void Unify_Color(bool force = false)
        {
            if (color_picker.Color != App.AppAccentColor || force)
            {
                color_text.Text = string.Format("#{0:X2}{1:X2}{2:X2}", color_picker.Color.R, color_picker.Color.G, color_picker.Color.B);
                color_ex.Background = new SolidColorBrush(color_picker.Color);
                color_ex.Foreground = new SolidColorBrush(utils.Get_ForeColor(color_picker.Color, utils.Read_Ini(App.dconfig, "config", "reverse", "0").Equals("1")));
            }
        }
        private void Color_text_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!color_text.Text.StartsWith("#"))
                    color_text.Text = "#" + color_text.Text;
                if (color_text.Text.Length == 7)
                {
                    try
                    {
                        color_picker.Color = (Color)ColorConverter.ConvertFromString(color_text.Text);
                    }
                    catch (Exception ex)
                    {
                        utils.LogtoFile("[ERROR]" + ex.Message);
                    }
                }
                Unify_Color();
                e.Handled = true;
            }
        }
    }
}
