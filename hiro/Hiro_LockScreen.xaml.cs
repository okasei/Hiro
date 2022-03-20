using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace hiro
{
    public partial class Hiro_LockScreen : Window
    {
        internal bool ca = true;
        internal bool exist = false;
        private bool authing = false;
        public Hiro_LockScreen()
        {
            InitializeComponent();
            Title = Hiro_Utils.Get_Transalte("locktitle") + " - " + App.AppTitle;
            Load_Colors();
            SetValue(Canvas.LeftProperty, 0.0);
            Canvas.SetTop(this, -SystemParameters.PrimaryScreenHeight);
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            Keyboard.Focus(this);
            Mouse.Capture(this);
            Hiro_Utils.Set_Control_Location(timelabel, "locktime", bottom: true);
            Hiro_Utils.Set_Control_Location(datelabel, "lockdate", bottom: true);
            Hiro_Utils.ShowCursor(0);
            var filep = App.CurrentDirectory + "\\system\\wallpaper\\" + DateTime.Now.ToString("yyyyMMdd") + ".jpg";
            string wp = "";
            if (!System.IO.File.Exists(filep))
            { 
                bgimage.Background = Background;
                System.Net.Http.HttpRequestMessage request = new(System.Net.Http.HttpMethod.Get, "https://api.rexio.cn/v1/rex.php?r=wallpaper");
                request.Headers.Add("UserAgent", "Rex/2.1.0 (Hiro Inside)");
                request.Content = new System.Net.Http.StringContent("");
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                System.ComponentModel.BackgroundWorker bw = new();
                bw.DoWork += delegate {
                    try
                    {
                        System.Net.Http.HttpResponseMessage response = App.hc.Send(request);
                        if (response.Content != null)
                        {
                            System.IO.Stream stream = response.Content.ReadAsStream();
                            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                            image.Save(filep);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Hiro_Utils.LogtoFile("[ERROR]" + ex.Message);
                    }
                    StringBuilder wallPaperPath = new(200);
                    if (Hiro_Utils.SystemParametersInfo(0x0073, 200, wallPaperPath, 0))
                    {
                        Hiro_Utils.LogtoFile(wallPaperPath.ToString());
                        wp = wallPaperPath.ToString();
                        exist = true;
                        return;
                    }
                    
                    if (App.mn != null)
                    {
                        bgimage.Background = App.mn.bgimage.Background;
                        exist = true;
                    }
                };

                bw.RunWorkerCompleted += delegate
                {
                    filep = System.IO.File.Exists(filep) ? filep : wp;
                    if (System.IO.File.Exists(filep))
                    {
                        BitmapImage bi = new();
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.UriSource = new Uri(filep);
                        ImageBrush ib = new()
                        {
                            Stretch = Stretch.UniformToFill,
                            ImageSource = bi
                        };
                        bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
                        bgimage.Background = ib;
                        bi.EndInit();
                        bi.Freeze();
                        Hiro_Utils.Blur_Animation(0, animation, bgimage, this);
                    }
                };
                bw.RunWorkerAsync();
            }
            else
            {
                BitmapImage bi = new();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.UriSource = new Uri(filep);
                ImageBrush ib = new()
                {
                    Stretch = Stretch.UniformToFill,
                    ImageSource = bi
                };
                bgimage.Background = ib;
                bi.EndInit();
                bi.Freeze();
                exist = true;
            }
        }

        public void Load_Colors()
        {
            timelabel.Foreground = new SolidColorBrush(App.AppForeColor);
            datelabel.Foreground = new SolidColorBrush(App.AppForeColor);
            Background = new SolidColorBrush(App.AppAccentColor);
        }

        private void Run_In()
        {
            if(!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
            {
                System.Windows.Media.Animation.DoubleAnimation dou = new(-SystemParameters.PrimaryScreenHeight, 0, TimeSpan.FromMilliseconds(800));
                dou.FillBehavior = System.Windows.Media.Animation.FillBehavior.Stop;
                dou.DecelerationRatio = 0.9;
                dou.Completed += delegate
                {
                    Canvas.SetTop(this, 0);
                };
                BeginAnimation(TopProperty, dou);
            }
            else
                Canvas.SetTop(this, 0);
        }
        private void Run_Out()
        {
            Hiro_Utils.ShowCursor(1);
            if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0"))
            {
                System.Windows.Media.Animation.DoubleAnimation dou = new(-SystemParameters.PrimaryScreenHeight, TimeSpan.FromMilliseconds(600));
                dou.FillBehavior = System.Windows.Media.Animation.FillBehavior.Stop;
                dou.DecelerationRatio = 0.9;
                dou.Completed += delegate
                {
                    SetValue(TopProperty, -SystemParameters.PrimaryScreenHeight);
                    App.ls = null;
                    ca = false;
                    Close();
                };
                BeginAnimation(TopProperty, dou);
            }
            else
            {
                App.ls = null;
                ca = false;
                Close();
            }
        }

        private void Request_Authentication()
        {
            if (authing)
                return;
            authing = true;
            System.ComponentModel.BackgroundWorker sc = new();
            System.ComponentModel.BackgroundWorker fa = new();
            sc.RunWorkerCompleted += delegate
            {
                Run_Out();
            };
            fa.RunWorkerCompleted += delegate
            {
                authing = false;
                Activate();
            };
            Hiro_Utils.Register(sc, fa, fa);
        }
        private void Ls_Loaded(object sender, RoutedEventArgs e)
        {
            Run_In();
        }

        private void Ls_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = ca;
        }

        private void Ls_KeyDown(object sender, KeyEventArgs e)
        {
            Request_Authentication();
        }

        private void ls_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Request_Authentication();
        }
    }
}
