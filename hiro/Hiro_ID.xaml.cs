using System;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace hiro
{
    /// <summary>
    /// Hiro_ID.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_ID : Window
    {
        internal int bflag = 0;
        public Hiro_ID()
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            Load_Color();
            Load_Translate();
            Loaded += delegate
            {
                HiHiro();
            };
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;
            var source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source?.AddHook(WndProc);
            WindowStyle = WindowStyle.SingleBorderWindow;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0083://prevent system from drawing outline
                    handled = true;
                    break;
                default:
                    //Console.WriteLine("Msg: " + m.Msg + ";LParam: " + m.LParam + ";WParam: " + m.WParam + ";Result: " + m.Result);
                    break;
            }
            return IntPtr.Zero;

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }

        private void Closebtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
            e.Handled = true;
        }

        private void minbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
            e.Handled = true;
        }

        internal void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeReverse"] = App.AppForeColor == Colors.White ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 80));
            Resources["AppForeDimColor"] = Hiro_Utils.Color_Transparent(App.AppForeColor, 80);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 80));
            Resources["AppAccentDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 20));
        }

        internal void Load_Translate()
        {
            Title = Hiro_Utils.Get_Translate("httitle").Replace("%h", App.AppTitle);
            minbtn.ToolTip = Hiro_Utils.Get_Translate("min");
            closebtn.ToolTip = Hiro_Utils.Get_Translate("close");
        }

        public void HiHiro()
        {
            Loadbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                System.Windows.Media.Animation.Storyboard sb = new();
                Hiro_Utils.AddPowerAnimation(2, Ctrl_Btns, sb, -50, null);
                sb.Begin();
            }
            Hiro_Utils.SetWindowToForegroundWithAttachThreadInput(this);
        }

        public void Loadbgi(int direction)
        {
            if (bflag == 1)
                return;
            bflag = 1;
            Hiro_Utils.Set_Bgimage(bgimage, this);
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            Hiro_Utils.Blur_Animation(direction, animation, bgimage, this);
            bflag = 0;
        }

        private void Get_ID(string ID)
        {
            var msg = $"https://api.bilibili.com/x/polymer/web-dynamic/v1/feed/space?&host_mid={ID}";
            var following = $"https://api.bilibili.com/x/relation/followings?vmid={ID}";
            try
            {
                var jo = JsonObject.Parse(Hiro_Utils.GetWebContent(msg));
                if (jo == null || jo["code"] == null || jo["code"].ToString() != "0")
                    return;
                try
                {
                    for (int i = 0; i < jo["data"]["items"].AsArray().Count; i++)
                    {
                        for (int j = 0; j < jo["data"]["items"][i]["modules"]["module_dynamic"]["desc"]["rich_text_nodes"].AsArray().Count; j++)
                        {
                            var dynamicText = jo["data"]["items"][i]["modules"]["module_dynamic"]["desc"]["rich_text_nodes"][j]["text"].ToString();
                            Hiro_Utils.LogtoFile(dynamicText);
                            if (jo["data"]["items"][i].AsObject().ContainsKey("orig"))
                            {
                                var origin = jo["data"]["items"][i]["orig"]["modules"]["module_dynamic"]["desc"]["text"].ToString();
                                Hiro_Utils.LogtoFile(origin);
                            }

                        }

                    }
                }
                catch
                {

                }



            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.IDTracer.Exception");
            }
        }

        private void IDBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Get_ID(IDBox.Text);
                e.Handled = true;
            }
        }
    }
}
