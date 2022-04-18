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
        private Hiro_MainUI? Hiro_Main = null;
        public Hiro_Color(Hiro_MainUI? Parent)
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
                Hiro_Utils.AddPowerAnimation(1, color_title, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(1, color_text, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(3, cobtn1, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(3, cobtn2, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(3, cobtn3, sb, -50, null);
            }
            if (!animation) 
                return;
            Hiro_Utils.AddPowerAnimation(0, this, sb, 50, null);
            sb.Begin();
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
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        public void Load_Translate()
        {
            color_title.Content = Hiro_Utils.Get_Transalte("cotitle");
            color_ex.Content = Hiro_Utils.Get_Transalte("coex").Replace("\\n", Environment.NewLine);
            cobtn1.Content = Hiro_Utils.Get_Transalte("cook");
            cobtn2.Content = Hiro_Utils.Get_Transalte("cocancel");
            cobtn3.Content = Hiro_Utils.Get_Transalte("coreset");
        }

        public void Load_Position()
        {
            Hiro_Utils.Set_Control_Location(color_title, "cotitle");
            Hiro_Utils.Set_Control_Location(color_picker, "copicker");
            Hiro_Utils.Set_Control_Location(color_text, "coval");
            Hiro_Utils.Set_Control_Location(color_ex, "coex");
            Hiro_Utils.Set_Control_Location(cobtn1, "cook", bottom: true, right: true);
            Hiro_Utils.Set_Control_Location(cobtn2, "cocancel", bottom: true, right: true);
            Hiro_Utils.Set_Control_Location(cobtn3, "coreset", bottom: true);
        }


        private void Cobtn3_Click(object sender, RoutedEventArgs e)
        {
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "Lockcolor", "default");
            if (App.wnd != null)
                App.wnd.Load_All_Colors();
            if (Hiro_Main != null)
            {
                Hiro_Main.Set_Label(Hiro_Main.configx);
            }
            
        }

        private void Cobtn1_Click(object sender, RoutedEventArgs e)
        {
            App.AppAccentColor = color_picker.Color;
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "Lockcolor", string.Format("#{0:X2}{1:X2}{2:X2}", App.AppAccentColor.R, App.AppAccentColor.G, App.AppAccentColor.B));
            if (App.wnd != null)
                App.wnd.Load_All_Colors();
            if (Hiro_Main != null)
            {
                Hiro_Main.Set_Label(Hiro_Main.configx);
            }
        }

        private void Cobtn2_Click(object sender, RoutedEventArgs e)
        {
            Hiro_Main?.Set_Label(Hiro_Main.configx);
        }

        private void Color_picker_ColorChanged(object sender, MouseEventArgs e)
        {
            Unify_Color();
        }

        internal void Unify_Color(bool force = false)
        {
            if (color_picker.Color == App.AppAccentColor && !force) 
                return;
            color_text.Text = $"#{color_picker.Color.R:X2}{color_picker.Color.G:X2}{color_picker.Color.B:X2}";
            color_ex.Background = new SolidColorBrush(color_picker.Color);
            color_ex.Foreground = new SolidColorBrush(Hiro_Utils.Get_ForeColor(color_picker.Color, Hiro_Utils.Read_Ini(App.dconfig, "Config", "Reverse", "0").Equals("1")));
        }
        private void Color_text_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) 
                return;
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
                    Hiro_Utils.LogtoFile("[ERROR]" + ex.Message);
                }
            }
            Unify_Color();
            e.Handled = true;
        }
    }
}
