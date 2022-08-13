using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
{
    /// <summary>
    /// Sequence.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Sequence : Window
    {
        internal int bflag = 0;
        internal int ci = 0;
        internal int tick = 0;
        internal Hiro_Sequence? parent = null;
        private System.Collections.ObjectModel.ObservableCollection<string> cmds = new();
        public Hiro_Sequence()
        {
            InitializeComponent();
            Load_Colors();
            Loadbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
            Load_Position();
            Load_Translate();
            Title = Hiro_Utils.Get_Translate("seqtitle") + " - " + App.AppTitle;
            var maxwidth = SystemParameters.PrimaryScreenWidth / 5;
            var btnwidth = skipbtn.Width + skipbtn.Margin.Right + 5;
            maxwidth = (maxwidth > btnwidth) ? maxwidth : btnwidth;
            con.MaxWidth = maxwidth;
            textblock.MaxWidth = maxwidth;
            Width = maxwidth;
            SourceInitialized += OnSourceInitialized;
            Hiro_Utils.SetShadow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
            Loaded += delegate
            {
                HiHiro();
            };
        }

        public void HiHiro()
        {
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1") && Visibility == Visibility.Visible)
            {
                Storyboard sb = new();
                Hiro_Utils.AddPowerAnimation(3, skipbtn, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(3, pausebtn, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(3, cancelbtn, sb, -50, null);
                sb.Begin();
            }
            if (Visibility == Visibility.Visible)
                Hiro_Utils.SetWindowToForegroundWithAttachThreadInput(this);
        }

        private void TimerTick()
        {
            if (pausebtn.Content.Equals(Hiro_Utils.Get_Translate("seqconti")))
                return;
            tick--;
            Resizel(ci + 1, cmds.Count);                
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
                    break;
            }
            return IntPtr.Zero;

        }

        public void Load_Translate()
        {
            skipbtn.Content = Hiro_Utils.Get_Translate("seqskip");
            textblock.Text = Hiro_Utils.Get_Translate("seqload");
            pausebtn.Content = Hiro_Utils.Get_Translate("seqpause");
            cancelbtn.Content = Hiro_Utils.Get_Translate("seqcancel");
        }
        public void Load_Colors()
        {
            textblock.Foreground = new SolidColorBrush(App.AppForeColor);
            skipbtn.Foreground = textblock.Foreground;
            pausebtn.Foreground = textblock.Foreground;
            cancelbtn.Foreground = textblock.Foreground;
            skipbtn.Background = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
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
            Hiro_Utils.Set_Control_Location(skipbtn, "seqskip", bottom: true, right: true);
            Hiro_Utils.Set_Control_Location(pausebtn, "seqpause", bottom: true, right: true);
            Hiro_Utils.Set_Control_Location(cancelbtn, "seqcancel", bottom: true, right: true);
            Hiro_Utils.Set_Control_Location(con, "seqcontent");
            textblock.FontFamily = con.FontFamily;
            textblock.FontSize = con.FontSize;
        }

        public void SeqExe(String path)
        {
            ThreadSeq(path);
        }
        internal void Next_CMD()
        {
            if (pausebtn.Content.Equals(Hiro_Utils.Get_Translate("seqconti")))
                return;
            if (cmds.Count <= ci)
            {
                Close();
                return;
            }
            var sc = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(cmds[ci]));
            if (App.dflag)
                Hiro_Utils.LogtoFile("[SEQUENCE]" + sc);
            skipbtn.Visibility = Visibility.Hidden;
            Resizel(ci + 1, cmds.Count);
            ci++;
            if (sc.ToLower().Equals("trap") || sc.ToLower().Equals("trap()"))
            {
                pausebtn.Content = Hiro_Utils.Get_Translate("seqconti");
                return;
            }
            if (sc.ToLower().StartsWith("pause("))
            {
                var scp = Hiro_Utils.HiroParse(sc);
                try
                {
                    tick = scp.Count == 0 ? 5 : int.Parse(scp[0]);
                }
                catch(Exception ex)
                {
                    Hiro_Utils.LogError(ex, "Hiro.Exception.Sequence.Pause");
                    tick = 5;
                }
                skipbtn.Visibility = Visibility.Visible;
                System.Windows.Threading.DispatcherTimer dt = new()
                {
                    Interval = new TimeSpan(10000000)
                };
                dt.Tick += delegate
                {
                    TimerTick();
                    if (tick >= 1)
                        return;
                    Next_CMD();
                    dt.Stop();
                };
                dt.Start();
                Resizel((ci + 1), cmds.Count);
                return;
            }
            if (sc.Length > 4 && sc[..4].ToLower() == "seq(")
            {
                if (sc.LastIndexOf(")") != -1)
                {
                    var toolstr = sc[4..sc.LastIndexOf(")")];
                    if (toolstr.StartsWith("\""))
                        toolstr = toolstr[1..];
                    if (toolstr.EndsWith("\""))
                        toolstr = toolstr.Substring(0, toolstr.Length - 1);
                    if (System.IO.File.Exists(toolstr))
                    {
                        Hiro_Sequence sq = new()
                        {
                            parent = this
                        };
                        sq.Show();
                        sq.ThreadSeq(toolstr);
                        Visibility = Visibility.Hidden;
                        return;
                    }
                }
                Next_CMD();
                return;
            }
            Hiro_Utils.RunExe(sc, Hiro_Utils.Get_Translate("seqtitle"));
            if (App.dflag)
                Hiro_Utils.LogtoFile(sc);
            Next_CMD();
        }
        public void ThreadSeq(String path)
        {
            if(!System.IO.File.Exists(path))
            {
                Close();
                return;
            }
            var filec = System.IO.File.ReadAllLines(path);
            foreach (var cm in filec)
            {
                cmds.Add(cm);
            }
            Next_CMD();
        }

        private void Resizel(int cir, int all)
        {
            var changed = false;
            try
            {
                var next = Hiro_Utils.Get_Translate("seqnext");
                var sc = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(cmds[ci]));
                var current = Hiro_Utils.Get_Translate("seqcurrent") + sc;
                var inde = (ci + 1).ToString() + "/" + cmds.Count.ToString();
                if (tick > 0)
                    current = current + Environment.NewLine + Hiro_Utils.Get_Translate("seqcd").Replace("%s", tick.ToString());
                if (ci >= cmds.Count - 1)
                    next += Hiro_Utils.Get_Translate("seqfinish");
                else
                    next += cmds[ci + 1];
                changed = textblock.Text == inde + Environment.NewLine + current + Environment.NewLine + next;
                textblock.Text = inde + Environment.NewLine + current + Environment.NewLine + next;
            }
            catch
            {
                // ignored
            }

            Height = con.ActualHeight + 5 + cancelbtn.Height + cancelbtn.Margin.Bottom + 5;
            progress.Value = cir;
            progress.Maximum = all;
            APPBARDATA pdat = new();
            SHAppBarMessage(0x00000005, ref pdat);
            switch (pdat.uEdge)
            {
                //左上右下0123
                case 3:
                    Canvas.SetTop(this, pdat.rc.top - Height);
                    Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth - Width);
                    break;
                case 2:
                    Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight - Height);
                    Canvas.SetLeft(this, pdat.rc.left - Width);
                    break;
                default:
                    Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight - Height);
                    Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth - Width);
                    break;
            }

            if (Visibility != Visibility.Visible || !changed || !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
                return;
            Storyboard sb = new();
            Hiro_Utils.AddPowerAnimation(0, textblock, sb, 50, null);
            sb.Begin();
        }

        private void Con_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Height = con.ActualHeight + 5 + cancelbtn.Height + cancelbtn.Margin.Bottom + 5;
            APPBARDATA pdat = new();
            SHAppBarMessage(0x00000005, ref pdat);
            switch (pdat.uEdge)
            {
                //左上右下0123
                case 3:
                    Canvas.SetTop(this, pdat.rc.top - Height);
                    Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth - Width);
                    break;
                case 2:
                    Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight - Height);
                    Canvas.SetLeft(this, pdat.rc.left - Width);
                    break;
                default:
                    Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight - Height);
                    Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth - Width);
                    break;
            }
            Load_Position();
        }

        private void Rejectbtn_Click(object sender, RoutedEventArgs e)
        {
            if (pausebtn.Content.Equals(Hiro_Utils.Get_Translate("seqconti")))
            {
                pausebtn.Content = Hiro_Utils.Get_Translate("seqpause");
                Next_CMD();
            }
            else
                pausebtn.Content = Hiro_Utils.Get_Translate("seqconti");
        }

        private void Cancelbtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Acceptbtn_Click(object sender, RoutedEventArgs e)
        {
            tick = 0;
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
            Close();
        }

        public void Loadbgi(int direction)
        {
            if (bflag == 1)
                return;
            bflag = 1;
            Hiro_Utils.Set_Bgimage(bgimage, this);
            var animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            Hiro_Utils.Blur_Animation(direction, animation, bgimage, this);
            bflag = 0;
        }

        private void Seq_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Loadbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
        }

        private void Seq_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            pausebtn.Content = Hiro_Utils.Get_Translate("seqconti");
            tick = 0;
            if (parent == null)
                return;
            try
            {
                parent.Visibility = Visibility.Visible;
                parent.Next_CMD();
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.Sequence.Parent");
            }
        }
    }
}
