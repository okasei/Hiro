using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace hiro
{
    /// <summary>
    /// Sequence.xaml の相互作用ロジック
    /// </summary>
    public partial class Sequence : Window
    {
        internal string? path = null;
        internal int depth = 0;
        internal int flag = 0;//0=ready,1=gg,2=skip
        internal int aflag = -1;
        internal int bflag = 0;
        public Sequence()
        {
            InitializeComponent();
            Load_Colors();
            Loadbgi(utils.ConvertInt(utils.Read_Ini(App.dconfig, "config", "blur", "0")));
            Load_Position();
            Load_Translate();
            Title = utils.Get_Transalte("seqtitle") + " - " + App.AppTitle;
            var maxwidth = SystemParameters.PrimaryScreenWidth / 5;
            var btnwidth = skipbtn.Width + skipbtn.Margin.Right + 5;
            maxwidth = (maxwidth > btnwidth) ? maxwidth : btnwidth;
            con.MaxWidth = maxwidth;
            textblock.MaxWidth = maxwidth;
            this.Width = maxwidth;
            SourceInitialized += OnSourceInitialized;
            utils.SetShadow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
        }
        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;
            System.Windows.Interop.HwndSource source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source.AddHook(WndProc);
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

        public void Load_Translate()
        {
            skipbtn.Content = utils.Get_Transalte("seqskip");
            textblock.Text = utils.Get_Transalte("seqload");
            pausebtn.Content = utils.Get_Transalte("seqpause");
            cancelbtn.Content = utils.Get_Transalte("seqcancel");
        }
        public void Load_Colors()
        {
            textblock.Foreground = new SolidColorBrush(App.AppForeColor);
            skipbtn.Foreground = textblock.Foreground;
            pausebtn.Foreground = textblock.Foreground;
            cancelbtn.Foreground = textblock.Foreground;
            skipbtn.Background = new SolidColorBrush(Color.FromArgb(160, App.AppAccentColor.R, App.AppAccentColor.G, App.AppAccentColor.B));
            pausebtn.Background = skipbtn.Background;
            cancelbtn.Background = skipbtn.Background;
            skipbtn.BorderThickness = new Thickness(1, 1, 1, 1);
            pausebtn.BorderThickness = skipbtn.BorderThickness;
            cancelbtn.BorderThickness = skipbtn.BorderThickness;
            skipbtn.BorderBrush = new SolidColorBrush(App.AppForeColor);
            pausebtn.BorderBrush = new SolidColorBrush(App.AppForeColor);
            cancelbtn.BorderBrush = new SolidColorBrush(App.AppForeColor);
            progress.Foreground = skipbtn.Foreground;
            borderlabel.BorderBrush = new SolidColorBrush(App.AppForeColor);
        }
        public void Load_Position()
        {
            utils.Set_Control_Location(skipbtn, "seqskip", bottom: true, right: true);
            utils.Set_Control_Location(pausebtn, "seqpause", bottom: true, right: true);
            utils.Set_Control_Location(cancelbtn, "seqcancel", bottom: true, right: true);
            utils.Set_Control_Location(con, "seqcontent");
            textblock.FontFamily = con.FontFamily;
            textblock.FontSize = con.FontSize;
        }

        public void SeqExe(String path)
        {
            this.path = path;
            /*th = new System.Threading.Thread(ThreadSeq);
            th.Start();*/
            ThreadSeq(path);
        }
        public void ThreadSeq(String path)
        {
            depth++;
            int thd = depth;
            string[] filec = System.IO.File.ReadAllLines(path);
            for (int cir = 0; cir < filec.Length; cir++)
            {
                if (thd != depth || flag == 1)
                    break;
                var all = filec.Length;
                var val = utils.Path_Prepare(filec[cir]);
                if (App.dflag)
                    utils.LogtoFile("[SEQUENCE]" + val);
                var next = utils.Get_Transalte("seqnext");
                var current = utils.Get_Transalte("seqcurrent") + val;
                var inde = (cir + 1).ToString() + "/" + filec.Length.ToString();
                skipbtn.Visibility = Visibility.Hidden;
                if (cir >= filec.Length - 1)
                    next += utils.Get_Transalte("seqfinish");
                else
                    next += filec[cir + 1];
                while (pausebtn.Content.Equals(utils.Get_Transalte("seqconti")))
                {
                    current = utils.Get_Transalte("seqpause");
                    Resizel(inde + Environment.NewLine + current + Environment.NewLine + next, (cir + 1), all);
                    utils.Delay(500);
                    skipbtn.IsEnabled = false;
                }
                current = utils.Get_Transalte("seqcurrent") + val;
                Resizel(inde + Environment.NewLine + current + Environment.NewLine + next, (cir + 1), all);
                if (val.Length > 6 && val.Substring(0, 6).ToLower() == "pause(")
                {
                    String toolstr;
                    if (val.LastIndexOf(")") != -1)
                    {
                        toolstr = val.Substring(6, val.LastIndexOf(")") - 6);
                        var num = 0;
                        try
                        {
                            num = Convert.ToInt32(toolstr);
                        }
                        catch
                        {
                            num = 5;
                        }
                        skipbtn.Visibility = (skipbtn.IsEnabled) ? Visibility.Visible : Visibility.Hidden;
                        for (var inn = 0; inn < num; inn++)
                        {
                            while (pausebtn.Content.Equals(utils.Get_Transalte("seqconti")))
                            {
                                utils.Delay(500);
                                if (flag > 0)
                                    break;
                            }
                            if (flag > 0)
                            {
                                flag = 0;
                                break;
                            }
                            current = utils.Get_Transalte("seqcurrent") + val + Environment.NewLine + utils.Get_Transalte("seqcd").Replace("%s", (num - inn).ToString());
                            Resizel(inde + Environment.NewLine + current + Environment.NewLine + next, (cir + 1), all);
                            utils.Delay(1000);
                        }
                    }
                    else
                    {
                        skipbtn.Visibility = (skipbtn.IsEnabled) ? Visibility.Visible : Visibility.Hidden;
                        for (var inn = 0; inn < 5; inn++)
                        {
                            while (pausebtn.Content.Equals(utils.Get_Transalte("seqconti")))
                            {
                                utils.Delay(500);
                                if (flag > 0)
                                    break;
                            }
                            if (flag > 0)
                            {
                                flag = 0;
                                break;
                            }
                            current = utils.Get_Transalte("seqcurrent") + val + Environment.NewLine + utils.Get_Transalte("seqcd").Replace("%s", (5 - inn).ToString());
                            Resizel(inde + Environment.NewLine + current + Environment.NewLine + next, (cir + 1), all);
                            utils.Delay(1000);
                        }
                    }
                    continue;
                }
                if (val.Length == 5 && val.Substring(0, 5).ToLower() == "pause")
                {
                    current = utils.Get_Transalte("seqcurrent") + utils.Get_Transalte("seqcd").Replace("%s", "1");
                    Resizel(inde + Environment.NewLine + current + Environment.NewLine + next, (cir + 1), all);
                    utils.Delay(1000);
                    continue;
                }
                if (val.Length > 4 && val.Substring(0, 4).ToLower() == "seq(")
                {
                    if (val.LastIndexOf(")") != -1)
                    {
                        var toolstr = val.Substring(4, val.LastIndexOf(")") - 4);
                        if (toolstr.StartsWith("\""))
                            toolstr = toolstr.Substring(1);
                        if (toolstr.EndsWith("\""))
                            toolstr = toolstr.Substring(0, toolstr.Length - 1);
                        if (!System.IO.File.Exists(toolstr))
                        {
                            continue;
                        }
                        ThreadSeq(toolstr);
                    }
                    continue;
                }
                if (val.ToLower() == "trap()")
                {
                    pausebtn.Content = utils.Get_Transalte("seqconti");
                    while (pausebtn.Content.Equals(utils.Get_Transalte("seqconti")))
                    {
                        utils.Delay(500);
                    }
                    continue;
                }
                val = val.Trim();
                val.Replace(Environment.NewLine, "");
                if (val.Length > 0)
                    utils.RunExe(val);
                utils.Delay(100);
            }
            depth--;
            if (depth == 0)
                this.Close();
        }

        private void Resizel(string title, int cir, int all)
        {
            textblock.Text = title;
            Height = con.ActualHeight + 5 + cancelbtn.Height + cancelbtn.Margin.Bottom + 5;
            progress.Value = cir;
            progress.Maximum = all;
            APPBARDATA pdat = new();
            SHAppBarMessage(0x00000005, ref pdat);
            //左上右下0123
            if (pdat.uEdge == 3)
            {
                Canvas.SetTop(this, pdat.rc.top - Height);
                Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth - Width);
            }
            else if (pdat.uEdge == 2)
            {
                Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight - Height);
                Canvas.SetLeft(this, pdat.rc.left - Width);
            }
            else
            {
                Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight - Height);
                Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth - Width);
            }
        }

        private void Con_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Height = con.ActualHeight + 5 + cancelbtn.Height + cancelbtn.Margin.Bottom + 5;
            APPBARDATA pdat = new();
            SHAppBarMessage(0x00000005, ref pdat);
            //左上右下0123
            if (pdat.uEdge == 3)
            {
                Canvas.SetTop(this, pdat.rc.top - Height);
                Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth - Width);
            }
            else if (pdat.uEdge == 2)
            {
                Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight - Height);
                Canvas.SetLeft(this, pdat.rc.left - Width);
            }
            else
            {
                Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight - Height);
                Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth - Width);
            }
            Load_Position();
        }

        private void Rejectbtn_Click(object sender, RoutedEventArgs e)
        {
            if (pausebtn.Content.Equals(utils.Get_Transalte("seqconti")))
                pausebtn.Content = utils.Get_Transalte("seqpause");
            else
                pausebtn.Content = utils.Get_Transalte("seqconti");
        }

        private void Cancelbtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    break;
                case MouseButton.Right:
                    depth = 0;
                    flag = 1;
                    if (pausebtn.Content.Equals(utils.Get_Transalte("seqconti")))
                        pausebtn.Content = utils.Get_Transalte("seqpause");
                    Close();
                    break;
                default:
                    break;
            }
        }

        private void Acceptbtn_Click(object sender, RoutedEventArgs e)
        {
            flag = (flag == 0) ? 2 : flag;
            skipbtn.Visibility = Visibility.Hidden;
        }

        #region 任务栏位置获取

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SystemParametersInfo(int uAction, int uParam, ref RECT re, int fuWinTni);

        [System.Runtime.InteropServices.DllImport("SHELL32", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static extern uint SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct APPBARDATA
        {

            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;//属性代表上、下、左、右
            public RECT rc;
            public IntPtr lParam;

        }


        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        #endregion

        private void Cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            depth--;
            if (depth == 0)
            {
                flag = 1;
                if (pausebtn.Content.Equals(utils.Get_Transalte("seqconti")))
                    pausebtn.Content = utils.Get_Transalte("seqpause");
                Close();
            }
        }

        public void Loadbgi(int direction)
        {
            if (bflag == 1)
                return;
            bflag = 1;
            utils.Set_Bgimage(bgimage);
            bool animation = !utils.Read_Ini(App.dconfig, "config", "ani", "1").Equals("0");
            utils.Blur_Animation(direction, animation, bgimage, this);
            bflag = 0;
        }

        private void seq_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Loadbgi(utils.ConvertInt(utils.Read_Ini(App.dconfig, "config", "blur", "0")));
        }
    }
}
