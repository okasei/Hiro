using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace hiro
{
    /// <summary>
    /// editor.xaml の相互作用ロジック
    /// </summary>
    public partial class Editor : Window
    {
        public int saveflag = 0;
        public int savetime = 0;
        public int runoutflag = 0;
        public int allow = 0;
        public int bflag = 0;
        public Editor()
        {
            InitializeComponent();
            utils.Set_Control_Location(previous, "edipre", bottom: true);
            utils.Set_Control_Location(next, "edinext", bottom: true);
            utils.Set_Control_Location(status, "estatus", bottom: true);
            slider.Style = new Style();
            this.Title = utils.Get_Transalte("edititle") + " - " + App.AppTitle;
            System.Windows.Threading.DispatcherTimer timer = new();
            timer.Interval = new TimeSpan(10000000);
            timer.Tick += delegate
             {
                 utils.Delay(1);
                 if (savetime > 0 && saveflag == 1)
                 {
                     savetime--;
                     if (savetime == 5)
                     {
                         Save();
                     }
                     if(savetime < 5)
                     {
                         saveflag = 0;
                         status.Content = utils.Get_Transalte("estatus").Replace("%p", App.editpage.ToString()).Replace("%w", con.Text.Length.ToString()) + " - " + utils.Get_Transalte("eready");
                     }
                 }
                 if(saveflag == 0 && !status.Content.Equals(utils.Get_Transalte("estatus").Replace("%p", App.editpage.ToString()).Replace("%w", con.Text.Length.ToString())))
                 {
                     status.Content = utils.Get_Transalte("estatus").Replace("%p", App.editpage.ToString()).Replace("%w", con.Text.Length.ToString());
                 }
             };
            timer.Start();
            Load();
            con.Focus();
            slider.Value = double.Parse(utils.Read_Ini(App.dconfig, "Configuration", "EditOpacity", "1"));
            allow = 1;
            slider.IsEnabled = true;
            this.Opacity = (float)slider.Value;
        }
        public void Save()
        {
            if(con.IsEnabled)
            {
                string path = App.CurrentDirectory + "\\users\\" + App.EnvironmentUsername + "\\editor\\" + App.editpage.ToString() + ".het";
                if (!System.IO.File.Exists(path))
                    System.IO.File.Create(path).Close();
                System.IO.FileStream fs = new(path, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite);
                System.IO.StreamWriter sw = new(fs);
                sw.Write(con.Text);
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            status.Content = utils.Get_Transalte("estatus").Replace("%p", App.editpage.ToString()).Replace("%w", con.Text.Length.ToString()) + " - " + utils.Get_Transalte("esaved").Replace("%t", DateTime.Now.ToString("HH:mm:ss"));
            saveflag = 1;
            savetime = 2;
        }
        public void Load()
        {
            string path = App.CurrentDirectory + "\\users\\" + App.EnvironmentUsername + "\\editor\\" + App.editpage.ToString() + ".het";
            if (!System.IO.File.Exists(path))
            {
                System.IO.File.Create(path).Close();
                con.Text = "";
                return;
            }
            System.IO.FileStream fs = new(path, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite);
            System.IO.StreamReader sr = new(fs);
            con.Text = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            fs.Close();
            utils.Write_Ini(App.dconfig, "Configuration", "EditPage", App.editpage.ToString());
            saveflag = 1;
            savetime = 2;
        }
        public void Load_Color()
        {
            con.Foreground = new SolidColorBrush(App.AppForeColor);
            status.Foreground = con.Foreground;
            previous.Foreground = con.Foreground;
            next.Foreground = con.Foreground;
        }
        public void Load_Position()
        {
            //main
            this.SetValue(LeftProperty, 0.0);
            this.SetValue(TopProperty, SystemParameters.PrimaryScreenHeight * -6 / 10);
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight * 6 / 10;
            //status
            utils.Set_Control_Location(con, "etext");
            //textbox
            con.SetValue(LeftProperty, 0.0);
            con.SetValue(TopProperty, 0.0);
            con.Width = this.ActualWidth;
            con.Height = this.Height - previous.Margin.Bottom - previous.Height - 2;
            status.Content = utils.Get_Transalte("estatus").Replace("%p", App.editpage.ToString()).Replace("%w", con.Text.Length.ToString());
            Loadbgi();
        }
        private void Run_In()
        {
            bool animation;
            if (utils.Read_Ini(App.dconfig, "Configuration", "ani", "1").Equals("0"))
                animation = false;
            else
                animation = true;
            if (animation)
            {
                double i = -ActualHeight;
                while (i < 0)
                {
                    System.Windows.Forms.Application.DoEvents();
                    System.Threading.Thread.Sleep(1);
                    i += 5;
                    SetValue(TopProperty, i);
                }
            }
                SetValue(TopProperty, 0.0);
            
        }
        private void Run_Out()
        {
            if (runoutflag == 1)
                return;
            runoutflag = 1;
            Save();
            con.IsEnabled = false;
            bool animation;
            if (utils.Read_Ini(App.dconfig, "Configuration", "ani", "1").Equals("0"))
                animation = false;
            else
                animation = true;
            if(animation)
            {
                double i = 0.0;
                while (i > -this.ActualHeight)
                {
                    i -= 16;
                    Canvas.SetTop(this, i);
                    utils.Delay(1);
                }
            }
            App.ed = null;
            this.Close();
        }
        private void Edi_Loaded(object sender, RoutedEventArgs e)
        {
            Load_Position();
            Load_Color();
            Run_In();
        }
        private void Previous_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Save();
            App.editpage--;
            Load();
        }

        private void Next_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Save();
            App.editpage++;
            Load();
        }

        private void Con_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Return) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Save();
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.X) && Keyboard.Modifiers == ModifierKeys.Alt)
            {
                if (slider.Value < 0.95)
                    slider.Value += 0.05;
                else
                    slider.Value = 1;
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Z) && Keyboard.Modifiers == ModifierKeys.Alt)
            {
                if (slider.Value > 0.05)
                    slider.Value -= 0.05;
                else
                    slider.Value = 0;
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Z) && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                Save();
                App.editpage--;
                Load();
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.X) && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                Save();
                App.editpage++;
                Load();
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.S) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Save();
                e.Handled = true;
            }
        }

        private void Con_TextChanged(object sender, TextChangedEventArgs e)
        {
            saveflag = 1;
            savetime = 10;
            status.Content = utils.Get_Transalte("estatus").Replace("%p", App.editpage.ToString()).Replace("%w", con.Text.Length.ToString());
        }

        private void Edi_Deactivated(object sender, EventArgs e)
        {
            Run_Out();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.Opacity = (float)slider.Value;
            if(allow == 1)
                utils.Write_Ini(App.dconfig, "Configuration", "EditOpacity", slider.Value.ToString());
        }

        private void Edi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Return) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Save();
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.X) && Keyboard.Modifiers == ModifierKeys.Alt)
            {
                if (slider.Value < 0.95)
                    slider.Value += 0.05;
                else
                    slider.Value = 1;
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Z) && Keyboard.Modifiers == ModifierKeys.Alt)
            {
                if (slider.Value > 0.05)
                    slider.Value -= 0.05;
                else
                    slider.Value = 0;
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Z) && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                Save();
                App.editpage--;
                Load();
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.X) && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                Save();
                App.editpage++;
                Load();
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.S) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Save();
                e.Handled = true;
            }
        }

        private void Edi_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }
        public void Loadbgi()
        {
            if (bflag == 1)
                return;
            bflag = 1;
            if (App.mn != null)
            {
                bool animation;
                if (utils.Read_Ini(App.dconfig, "Configuration", "ani", "1").Equals("0"))
                    animation = false;
                else
                    animation = true;
                bgimage.Background = App.mn.bgimage.Background;
                if (utils.Read_Ini(App.dconfig, "Configuration", "blur", "0").Equals("1"))
                {
                    utils.Blur_Animation(true, animation, bgimage, this);
                }
                else
                {
                    utils.Blur_Animation(false, animation, bgimage, this);
                }
            }
            else
            {
                Thickness tn = bgimage.Margin;
                tn.Left = 0.0;
                tn.Top = 0.0;
                bgimage.Margin = tn;
                bgimage.Width = Width;
                bgimage.Height = Height;
                bgimage.Background = new SolidColorBrush(App.AppAccentColor);
            }
            bflag = 0;
        }
    }
}
