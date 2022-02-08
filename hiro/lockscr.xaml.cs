using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace hiro
{
    /// <summary>
    /// lockscr.xaml の相互作用ロジック
    /// </summary>
    public partial class Lockscr : Window
    {
        internal bool ca = true;
        internal bool exist = false;
        public Lockscr()
        {
            InitializeComponent();
            this.Title = utils.Get_Transalte("locktitle") + " - " + App.AppTitle;
            Load_Colors();
            SetValue(Canvas.LeftProperty, 0.0);
            Canvas.SetTop(this, -SystemParameters.PrimaryScreenHeight);
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            utils.Set_Control_Location(timelabel, "locktime", bottom: true);
            utils.Set_Control_Location(datelabel, "lockdate", bottom: true);
            Thickness tn = bgimage.Margin;
            tn.Left = 0.0;
            tn.Top = 0.0;
            bgimage.Margin = tn;
            bgimage.Width = Width;
            bgimage.Height = Height;
            ShowCursor(0);
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
                        utils.LogtoFile("[ERROR]" + ex.Message);
                    }
                    StringBuilder wallPaperPath = new(200);
                    if (SystemParametersInfo(0x0073, 200, wallPaperPath, 0))
                    {
                        utils.LogtoFile(wallPaperPath.ToString());
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
                        ImageBrush ib = new()
                        {
                            Stretch = Stretch.UniformToFill,
                            ImageSource = new BitmapImage(new Uri(filep))
                        };
                        bool animation = !utils.Read_Ini(App.dconfig, "Configuration", "ani", "1").Equals("0");
                        bgimage.Background = ib;
                        utils.Blur_Animation(false, animation, bgimage, this);
                    }
                };
                bw.RunWorkerAsync();
            }
            else
            {
                ImageBrush ib = new()
                {
                    Stretch = Stretch.UniformToFill,
                    ImageSource = new BitmapImage(new Uri(filep))
                };
                bgimage.Background = ib;
                exist = true;
            }
        }

        public void Load_Colors()
        {
            timelabel.Foreground = new SolidColorBrush(App.AppForeColor);
            datelabel.Foreground = new SolidColorBrush(App.AppForeColor);
            Background = new SolidColorBrush(App.AppAccentColor);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        static extern bool SystemParametersInfo(uint uAction, uint uParam, StringBuilder lpvParam, uint init);

        #region 隐藏鼠标的方法 0/1 隐藏/显示
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern void ShowCursor(int status);
        #endregion

        private void Run_In()
        {
            bool animation;
            if (utils.Read_Ini(App.dconfig, "Configuration", "ani", "1").Equals("0"))
                animation = false;
            else
                animation = true;
            if(animation)
            {
                double i = -SystemParameters.PrimaryScreenHeight;
                while (i < -10)
                {
                    i += 10;
                    Canvas.SetTop(this, i);
                    utils.Delay(1);
                }
            }
            Canvas.SetTop(this, 0);
            //if(exist)
                //utils.Blur_Animation(false, animation, bgimage, this);
        }
        private void Run_Out()
        {
            ShowCursor(1);
            bool animation;
            if (utils.Read_Ini(App.dconfig, "Configuration", "ani", "1").Equals("0"))
                animation = false;
            else
                animation = true;
            if (animation)
            {
                double i = 0.0;
                while (i > -SystemParameters.PrimaryScreenHeight)
                {
                    i -= 20;
                    Canvas.SetTop(this, i);
                    utils.Delay(1);
                }
            }
            App.ls = null;
            ca = false;
            this.Close();
        }

        private void Ls_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.ComponentModel.BackgroundWorker sc = new();
            System.ComponentModel.BackgroundWorker fa = new();
            sc.RunWorkerCompleted += delegate
            {
                this.Run_Out();
            };
            fa.RunWorkerCompleted += delegate
            {
            };
            utils.Register(sc, fa, fa);
        }
        private void Ls_Loaded(object sender, RoutedEventArgs e)
        {
            Run_In();
        }

        private void Ls_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = ca;
        }
    }
}
