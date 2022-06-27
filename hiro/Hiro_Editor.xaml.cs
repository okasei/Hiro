using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
{
    public partial class Hiro_Editor : Window
    {
        public int saveflag = 0;
        public int savetime = 0;
        public int runoutflag = 0;
        public int allow = 0;
        public int bflag = 0;
        internal int editpage = 0;
        private bool load = false;
        public Hiro_Editor()
        {
            InitializeComponent();
            editpage = int.Parse(Hiro_Utils.Read_Ini(App.dconfig, "Config", "EditPage", "0"));
            Load_Position();
            Title = Hiro_Utils.Get_Transalte("edititle") + " - " + App.AppTitle;
            Load();
            con.Focus();
            slider.Value = double.Parse(Hiro_Utils.Read_Ini(App.dconfig, "Config", "EditOpacity", "1"));
            allow = 1;
            slider.IsEnabled = true;
            Opacity = (float)slider.Value;
            System.Windows.Threading.DispatcherTimer timer = new()
            {
                Interval = new TimeSpan(10000000)
            };
            timer.Tick += delegate
            {
                Hiro_Utils.Delay(1);
                if (savetime > 0 && saveflag == 1)
                {
                    savetime--;
                    if (savetime == 10)
                    {
                        Save(true);
                    }
                    if (savetime < 5 && savetime > 1)
                    {
                        var FinalText = Hiro_Utils.Get_Transalte("eready").Replace("%p", editpage.ToString()).Replace("%w", con.Text.Length.ToString());
                        if (!status.Content.Equals(FinalText))
                        {
                            status.Content = FinalText;
                            Hiro_Utils.LogtoFile("1");
                            Update_Animation();
                        }
                    }
                    if (savetime < 1)
                    {
                        saveflag = 0;
                    }
                }
                var StatusText = Hiro_Utils.Get_Transalte("estatus").Replace("%p", editpage.ToString()).Replace("%w", con.Text.Length.ToString());
                if (saveflag != 0 || status.Content.Equals(StatusText)) 
                    return;
                status.Content = StatusText;
                Update_Animation();
            };
            timer.Start();
        }

        private void Update_Animation()
        {
            if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1")) 
                return;
            Storyboard sb = new();
            Hiro_Utils.AddPowerAnimation(3, status, sb, -50, null);
            sb.Begin();
        }

        public void Save(bool show = false)
        {
            if(con.IsEnabled)
            {
                try
                {
                    string path = App.CurrentDirectory + "\\users\\" + App.EnvironmentUsername + "\\editor\\" + editpage.ToString() + ".het";
                    if (!System.IO.File.Exists(path))
                        System.IO.File.Create(path).Close();
                    using(System.IO.FileStream fs = new(path, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite))
                    {
                        using(System.IO.StreamWriter sw = new(fs))
                        {
                            sw.Write(con.Text);
                            sw.Flush();
                            sw.Close();
                        }
                        fs.Close();
                    }
                }
                catch (Exception ex)
                {
                    con.Text = "";
                    Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.Editor.Write: " + ex.Message);
                }
                
            }
            if (show)
            {
                status.Content = Hiro_Utils.Get_Transalte("esaved").Replace("%p", editpage.ToString()).Replace("%w", con.Text.Length.ToString()).Replace("%t", DateTime.Now.ToString("HH:mm:ss"));
                saveflag = 1;
                savetime = 9;
                Update_Animation();
                Hiro_Utils.LogtoFile("3");
            }
            else
                savetime = -1;
        }
        public void Load()
        {
            var path = App.CurrentDirectory + "\\users\\" + App.EnvironmentUsername + "\\editor\\" + editpage.ToString() + ".het";
            try
            {
                con.Text = System.IO.File.ReadAllText(path);
            }
            catch(Exception ex)
            {
                con.Text = "";
                Hiro_Utils.LogtoFile("[ERROR]Hiro.Exception.Editor.Read: " + ex.Message);
            }
            status.Content = Hiro_Utils.Get_Transalte("estatus").Replace("%p", editpage.ToString()).Replace("%w", con.Text.Length.ToString());
            Update_Animation();
            Hiro_Utils.Write_Ini(App.dconfig, "Config", "EditPage", editpage.ToString());
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
            Hiro_Utils.Set_Control_Location(previous, "edipre", bottom: true);
            Hiro_Utils.Set_Control_Location(next, "edinext", bottom: true);
            Hiro_Utils.Set_Control_Location(status, "estatus", bottom: true);
            slider.Style = new Style();
            //main
            SetValue(LeftProperty, 0.0);
            SetValue(TopProperty, SystemParameters.PrimaryScreenHeight * -6 / 10);
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight * 6 / 10;
            //status
            Hiro_Utils.Set_Control_Location(con, "etext");
            //textbox
            con.SetValue(LeftProperty, 0.0);
            con.SetValue(TopProperty, 0.0);
            con.Width = SystemParameters.PrimaryScreenWidth;
            con.Height = SystemParameters.PrimaryScreenHeight * 6 / 10 - previous.Margin.Bottom - previous.Height - 10;
            status.Content = Hiro_Utils.Get_Transalte("estatus").Replace("%p", editpage.ToString()).Replace("%w", con.Text.Length.ToString());
            Loadbgi();
        }
        private void Run_In()
        {
            if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
            {
                DoubleAnimation dou = new(-ActualHeight, 0, TimeSpan.FromMilliseconds(600))
                {
                    DecelerationRatio = 0.9,
                    FillBehavior = FillBehavior.Stop
                };
                dou.Completed += delegate
                {
                    SetValue(TopProperty, 0.0);
                    Hiro_Utils.SetCaptureImpl(new System.Windows.Interop.WindowInteropHelper(this).Handle);
                    Hiro_Utils.SetWindowToForegroundWithAttachThreadInput(this);
                    Keyboard.Focus(con);
                    load = true;
                };
                BeginAnimation(TopProperty, dou);
            }
            else
            {
                SetValue(TopProperty, 0.0);
                Hiro_Utils.SetCaptureImpl(new System.Windows.Interop.WindowInteropHelper(this).Handle);
                Hiro_Utils.SetWindowToForegroundWithAttachThreadInput(this);
                Keyboard.Focus(con);
                load = true;
            }
            
        }
        private void Run_Out()
        {
            if (runoutflag == 1 || !load)
                return;
            runoutflag = 1;
            Save();
            con.IsEnabled = false;
            if(!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
            {
                DoubleAnimation dou = new(-ActualHeight, TimeSpan.FromMilliseconds(450))
                {
                    FillBehavior = FillBehavior.Stop,
                    DecelerationRatio = 0.9
                };
                dou.Completed += delegate
                {
                    SetValue(TopProperty, -ActualHeight);
                    App.ed = null;
                    Close();
                };
                BeginAnimation(TopProperty, dou);
            }
            else
            {
                App.ed = null;
                Close();
            }
        }
        private void Edi_Loaded(object sender, RoutedEventArgs e)
        {
            Load_Color();
            Run_In();
        }
        private void Previous_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PreviousPage();
        }

        private void PreviousPage()
        {
            if (savetime > 0 && saveflag == 1)
                Save();
            editpage--;
            Load();
        }
        private void Next_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NextPage();
        }

        private void NextPage()
        {
            if (savetime > 0 && saveflag == 1)
                Save();
            editpage++;
            Load();
        }

        private void Con_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyStates == Keyboard.GetKeyStates(Key.Return) && Keyboard.Modifiers == ModifierKeys.Control) || e.KeyStates == Keyboard.GetKeyStates(Key.Escape))
            {
                Save(true);
                Run_Out();
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
                PreviousPage();
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.X) && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                NextPage();
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.S) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Save(true);
                e.Handled = true;
            }
        }

        private void Con_TextChanged(object sender, TextChangedEventArgs e)
        {
            saveflag = 1;
            savetime = 15;
            status.Content = Hiro_Utils.Get_Transalte("estatus").Replace("%p", editpage.ToString()).Replace("%w", con.Text.Length.ToString());
        }

        private void Edi_Deactivated(object sender, EventArgs e)
        {
            Run_Out();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Opacity = (float)slider.Value;
            if(allow == 1)
                Hiro_Utils.Write_Ini(App.dconfig, "Config", "EditOpacity", slider.Value.ToString());
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
                editpage--;
                Load();
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.X) && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                Save();
                editpage++;
                Load();
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.S) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Save();
                e.Handled = true;
            }
        }
        public void Loadbgi()
        {
            if (bflag == 1)
                return;
            bflag = 1;
            Hiro_Utils.Set_Bgimage(bgimage, this);
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            Hiro_Utils.Blur_Animation(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")), animation, bgimage, this);
            bflag = 0;
        }

        private void Edi_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hiro_Utils.CancelWindowToForegroundWithAttachThreadInput(this);
            Hiro_Utils.ReleaseCaptureImpl();
        }
    }
}
