using Hiro.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hiro.Widgets
{
    /// <summary>
    /// HiroTaskbar.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Taskbar : Window
    {
        private double _width = 70;
        private double _eWidth = 70;
        private double _x = 1.0;
        private double _y = 1.0;
        internal bool _dragFlag = false;
        private HClass.Hiro_Notice? _notif = null;
        private List<Grid> _grids = new List<Grid>();
        private string _title = string.Empty;
        private string _format = "<HH>:<mm>";
        private string _formatx = "<battery>%|<memory>%";
        private string _formatex = "<HH>:<mm>|<memory>%";

        public Hiro_Taskbar()
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            Loaded += (e, args) =>
            {
                Embbed();
                var _s = HWin.GetDpiScale();
                _x = _s.Width;
                _y = _s.Height;
                Height = 32 * _y;
                _width = 70;
                Width = _width + _eWidth;
                NotiGrid.Width = (_width + _eWidth + 20) * _x;
                Notification.Margin = new((_width + _eWidth + 20) * _x, 0, 0, 0);
                double.TryParse(HSet.Read_Ini(App.dConfig, "Taskbar", "ExtraWidth", "70"), out _eWidth);
                double.TryParse(HSet.Read_Ini(App.dConfig, "Taskbar", "BasicWidth", "70"), out _width);
                BasicGrid.Tag = _width.ToString();
                _format = HSet.Read_Ini(App.dConfig, "Taskbar", "Format", _format);
                _formatx = HSet.Read_Ini(App.dConfig, "Taskbar", "FormatX", _formatx);
                _formatex = HSet.Read_Ini(App.dConfig, "Taskbar", "FormatEX", _formatex);
                UpdateColors();
                BasicGrid.Width = 70;
                _grids.Add(BasicGrid);
                HUI.Set_Control_Location(Notification, "TaskbarNotice");
                HUI.Set_Control_Location(MsgLabel, "TaskbarMsg");
                HUI.Set_Control_Location(InfoLabel, "TaskbarInfo");
                HUI.Set_Control_Location(ExtraLabel, "TaskbarExtra");
                var _sb = HAnimation.AddPowerAnimation(0, TotalGrid, null, -100);
                _sb.Completed += (e, args) =>
                {
                    UpdatePosition();
                    UpdateLabels();
                };
                _sb.Begin();
                HSystem.HideInAltTab(new WindowInteropHelper(this).Handle);
            };
        }

        internal void UpdateLabels()
        {
            InfoLabel.Text = HText.Path_PPX(_format);
            ExtraLabel.Text = HText.Path_PPX(BasicGrid.Visibility == Visibility.Visible ? _formatx : _formatex);
        }

        internal void Embbed()
        {
            var _handle = new WindowInteropHelper(this).Handle;
            HDesktop.SetTaskbarWin(_handle);
            UpdatePosition();
        }


        internal void LoadGrid(Grid g)
        {
            g.Visibility = Visibility.Visible;
            if (_grids.Contains(g))
            {
                if (_grids[^1] != g)
                {
                    _grids.Remove(g);
                    _grids.Add(g);
                }
            }
            else
            {
                _grids.Add(g);
            }
            for (int i = 0; i < _grids.Count - 1; i++)
            {
                _grids[i].Visibility = Visibility.Collapsed;
            }
            bool animation = !HSet.Read_DCIni("Ani", "2").Equals("0");
            g.Width = double.Parse(g.Tag.ToString() ?? "90");
            _width = double.Parse(g.Tag.ToString() ?? "90");
            NotiGrid.Width = (_width + _eWidth + 20) * _x;
            Notification.Margin = new((_width + _eWidth + 20) * _x, 0, 0, 0);
            UpdatePosition();
            if (animation)
            {
                var s = HAnimation.AddPowerAnimation(0, g, null, -100, null);
                s.Begin();
            }
        }

        internal void RemoveGrid(Grid g)
        {
            if (_grids.Contains(g))
            {
                if (_grids[_grids.Count - 1] == g)
                {
                    if (_grids.Count > 1)
                    {
                        g.Visibility = Visibility.Collapsed;
                        _grids.Remove(g);
                        bool animation = !HSet.Read_DCIni("Ani", "2").Equals("0");
                        var _tg = _grids[_grids.Count - 1];
                        _tg.Visibility = Visibility.Visible;
                        _width = double.Parse(_tg.Tag.ToString() ?? "90");
                        NotiGrid.Width = (_width + _eWidth + 20) * _x;
                        Notification.Margin = new((_width + _eWidth + 20) * _x, 0, 0, 0);
                        UpdatePosition();
                        if (animation)
                        {
                            var s = HAnimation.AddPowerAnimation(0, _tg, null, -100, null);
                            s.Begin();
                        }
                    }
                }
                else
                {
                    _grids.Remove(g);
                }
            }


        }

        internal void UpdateColors()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 80));
            Resources["AppAccent"] = new SolidColorBrush(App.AppAccentColor);
            Resources["AppAccentDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 80));
            Resources["AppForeDimColor"] = Hiro_Utils.Color_Transparent(App.AppForeColor, 80);
        }

        internal void UpdatePosition()
        {
            UpdateLabels();
            var _handle = new WindowInteropHelper(this).Handle;
            var r = HDesktop.GetTaskBarRect();
            var _u = HTaskbar.GetTaskbarPosition();
            var _hh = 50.0;
            var _ww = 75.0;
            double.TryParse(HSet.Read_Ini(App.dConfig, "Taskbar", "HeightShfit", "50"), out _hh);
            double.TryParse(HSet.Read_Ini(App.dConfig, "Taskbar", "WidthShfit", "75"), out _ww);
            switch (_u)
            {
                case 0:
                    {
                        MainGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                        MainGrid.VerticalAlignment = VerticalAlignment.Top;
                        ExtraGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                        ExtraGrid.VerticalAlignment = VerticalAlignment.Top;
                        MainGrid.Margin = new(0);
                        MainGrid.Width = r.Width / _x;
                        BasicGrid.Width = r.Width / _x;
                        MsgGrid.Width = r.Width / _x;
                        MusicControlGrid.Width = r.Width / _x;
                        MusicControlGrid.HorizontalAlignment = HorizontalAlignment.Center;
                        MusicControlGrid.VerticalAlignment = VerticalAlignment.Center;
                        MainGrid.Height = 35;
                        ExtraGrid.Margin = new(0, 35, 0, 0);
                        ExtraGrid.Width = r.Width / _x;
                        ExtraGrid.Height = 35;
                        HDesktop.MoveWin32(_handle, 0, (int)(r.Height - 70 * _y - _hh * _y), (int)(r.Right), (int)(70 * _y));
                        break;
                    }
                case 1:
                    {
                        MainGrid.HorizontalAlignment = HorizontalAlignment.Left;
                        MainGrid.VerticalAlignment = VerticalAlignment.Stretch;
                        ExtraGrid.HorizontalAlignment = HorizontalAlignment.Left;
                        ExtraGrid.VerticalAlignment = VerticalAlignment.Stretch;
                        MainGrid.Margin = new(0);
                        MainGrid.Width = _width;
                        BasicGrid.Width = _width;
                        MsgGrid.Width = _width;
                        MusicControlGrid.Width = _width;
                        MusicControlGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                        MusicControlGrid.VerticalAlignment = VerticalAlignment.Stretch;
                        MainGrid.Height = (r.Height - r.Top) / _y;
                        ExtraGrid.Margin = new(_width, 0, 0, 0);
                        ExtraGrid.Width = _eWidth;
                        ExtraGrid.Height = (r.Height - r.Top) / _y;
                        HDesktop.MoveWin32(_handle, (int)(r.Right - (_width + _eWidth) * _x - _ww * _x), 0, (int)((_width + _eWidth) * _x), (int)(r.Height - r.Top));
                        break;
                    }
                case 2:
                    {
                        MainGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                        MainGrid.VerticalAlignment = VerticalAlignment.Top;
                        ExtraGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                        ExtraGrid.VerticalAlignment = VerticalAlignment.Top;
                        MainGrid.Margin = new(0);
                        MainGrid.Width = (r.Width - r.Left) / _x;
                        BasicGrid.Width = (r.Width - r.Left) / _x;
                        MsgGrid.Width = (r.Width - r.Left) / _x; ;
                        MusicControlGrid.Width = (r.Width - r.Left) / _x;
                        MusicControlGrid.HorizontalAlignment = HorizontalAlignment.Center;
                        MusicControlGrid.VerticalAlignment = VerticalAlignment.Center;
                        MainGrid.Height = 35;
                        ExtraGrid.Margin = new(0, 35, 0, 0);
                        ExtraGrid.Width = (r.Width - r.Left) / _x;
                        ExtraGrid.Height = 35;
                        HDesktop.MoveWin32(_handle, 0, (int)(r.Height - 70 * _y - _hh * _y), (int)(r.Width - r.Left), (int)(70 * _y));
                        break;
                    }
                default:
                    {
                        MainGrid.HorizontalAlignment = HorizontalAlignment.Left;
                        MainGrid.VerticalAlignment = VerticalAlignment.Stretch;
                        ExtraGrid.HorizontalAlignment = HorizontalAlignment.Left;
                        ExtraGrid.VerticalAlignment = VerticalAlignment.Stretch;
                        MainGrid.Margin = new(0);
                        MainGrid.Width = _width;
                        BasicGrid.Width = _width;
                        MsgGrid.Width = _width;
                        MusicControlGrid.Width = _width;
                        MusicControlGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                        MusicControlGrid.VerticalAlignment = VerticalAlignment.Stretch;
                        MainGrid.Height = (r.Height - r.Top) / _y;
                        ExtraGrid.Margin = new(_width, 0, 0, 0);
                        ExtraGrid.Width = _eWidth;
                        ExtraGrid.Height = (r.Height - r.Top) / _y;
                        HDesktop.MoveWin32(_handle, (int)(r.Right - (_width + _eWidth) * _x - _ww * _y), 0, (int)((_width + _eWidth) * _x), (int)(r.Height - r.Top));
                        break;
                    }
            }

        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (filePaths.Length > 0)
                {
                    Hiro_Utils.OpenInNewHiro($"\"base({Convert.ToBase64String(Encoding.Default.GetBytes(filePaths[0]))})\"", false);
                    e.Handled = true;
                }
            }
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                var f = (string)e.Data.GetData(DataFormats.Text);
                Hiro_Utils.OpenInNewHiro($"\"base({Convert.ToBase64String(Encoding.Default.GetBytes(f))})\"", false);
                e.Handled = true;
            }
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
        }

        private void Window_DragLeave(object sender, DragEventArgs e)
        {
        }
        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;
            var source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source?.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {/*
                case WM_WINDOWPOSCHANGED:
                    // 窗口位置或大小发生变化时的处理
                    WINDOWPOS windowPos = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                    // 根据 windowPos 结构体中的信息进行相应处理
                    break;
*/
                case 0x0005://WM_SIZE:
                            // 窗口大小变化时的处理
                    UpdatePosition();
                    // 根据新的宽度和高度进行相应处理
                    break;
                /*
                                case WM_MOVE:
                                    // 窗口位置变化时的处理
                                    int x = (int)(lParam.ToInt64() & 0xFFFF);
                                    int y = (int)((lParam.ToInt64() >> 16) & 0xFFFF);
                                    // 根据新的 x 和 y 坐标进行相应处理
                                    break;*/
                default:
                    break;

                    // 其他消息处理...
            }

            return IntPtr.Zero;

        }

        private async void Prev_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var _r = await HMediaInfoManager.TryTogglePrevious();
            if (_r != true)
                Hiro_Utils.RunExe("media(previous)", "HiroTaskbar[Beta]");
        }

        private async void Pause_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var _r = await HMediaInfoManager.TryTogglePlay();
            if (_r != true)
                Hiro_Utils.RunExe("media(pause)", "HiroTaskbar[Beta]");
        }

        private async void Play_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var _r = await HMediaInfoManager.TryTogglePlay();
            if (_r != true)
                Hiro_Utils.RunExe("media(play)", "HiroTaskbar[Beta]");
        }

        private async void Next_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var _r = await HMediaInfoManager.TryToggleNext();
            if (_r != true)
                Hiro_Utils.RunExe("media(next)", "HiroTaskbar[Beta]");
        }

        private void MsgLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (App.mn != null)
            {
                Hiro_Utils.RunExe("show()", "HiroTaskbar[Beta]");
                App.mn.Hiro_We_Info();
            }
        }

        internal void Load_Notification()
        {
            if (_notif != null)
            {
                return;
            }
            if (App.noticeitems.Count != 0)
            {
                _notif = App.noticeitems[0];
                App.noticeitems.RemoveAt(0);
                if (HText.IsOnlyBlank(_notif.title) || _title.Equals(_notif.title))
                {
                    Notification.Text = _notif.msg.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\\n", " ").Replace("<br>", " ");
                }
                else
                {
                    _title = _notif.title ?? string.Empty;
                    Notification.Text = "[" + _notif.title + "] " + _notif.msg.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\\n", " ").Replace("<br>", " ");
                }
                var tsize = new Size();
                HUI.Get_Text_Visual_Width(Notification, VisualTreeHelper.GetDpi(this).PixelsPerDip, out tsize);
                Notification.Width = tsize.Width;
                if (NotiGrid.Visibility != Visibility.Visible)
                {
                    new System.Threading.Thread(() =>
                    {
                        if (HSet.Read_DCIni("HiBoxAudio", "true").Equals("true", StringComparison.CurrentCultureIgnoreCase))
                            try
                            {
                                var fileP = HSet.Read_PPDCIni("BoxAudioPath", "<current>\\system\\sounds\\achievement.wav");
                                if (!System.IO.File.Exists(fileP))
                                    //fileP = HText.Path_Prepare("C:\\Users\\Rex\\Downloads\\Music\\xbox_one_rare_achiev.wav");
                                    fileP = HText.Path_Prepare("<win>\\Media\\Windows Notify Messaging.wav");
                                System.Media.SoundPlayer sndPlayer = new(fileP);
                                sndPlayer.Play();
                            }
                            catch (Exception ex)
                            {
                                HLogger.LogError(ex, "Hiro.Exception.TaskBar.Notification.Sound");
                            }
                    }).Start();
                    NotiGrid.Visibility = Visibility.Visible;
                    var _s = HAnimation.AddPowerAnimation(0, NotiGrid, null, 100);
                    if (TotalGrid.Visibility == Visibility.Visible)
                        _s = HAnimation.AddPowerOutAnimation(0, TotalGrid, _s, null, -100);
                    _s.Completed += (e, args) =>
                    {
                        TotalGrid.Visibility = Visibility.Collapsed;
                    };
                    _s.Begin();
                }
                var _t = (tsize.Width) switch
                {
                    < 100 => 3000,
                    double n when (n >= 100 && n < 500) => (1500 + n * 15),
                    double n when (n >= 500) => (n * 20),
                    double n => (n * 20)

                };
                var s = HAnimation.AddThicknessAnimaton(new(Math.Min(-tsize.Width, -_width - _eWidth) * _x, 0, 0, 0), _t, Notification, "Margin", null, new((_width + _eWidth) * _x, 0, 0, 0), 0, 0);
                s.Completed += (e, args) =>
                {
                    _notif = null;
                    Load_Notification();
                };
                s.Begin();
            }
            else
            {
                if (NotiGrid.Visibility == Visibility.Visible)
                {
                    Notification.Margin = new(NotiGrid.ActualWidth, 0, 0, 0);
                    TotalGrid.Visibility = Visibility.Visible;
                    var s = HAnimation.AddPowerOutAnimation(0, NotiGrid, null, null, 100);
                    HAnimation.AddPowerAnimation(0, TotalGrid, s, -100);
                    s.Completed += (e, args) =>
                    {
                        NotiGrid.Visibility = Visibility.Collapsed;
                        Notification.Margin = new Thickness(NotiGrid.ActualWidth, 0, 0, 0);
                        _title = string.Empty;
                        if (App.noticeitems.Count > 0)
                            Load_Notification();
                    };
                    s.Begin();
                }
            }
        }

        private void Notification_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_notif != null && _notif.act != null)
            {
                _notif.act.Invoke();
                _notif.act = null;
            }
        }



        private void InfoLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.RunExe(HSet.Read_Ini(App.dConfig, "Taskbar", "LeftAction", "nop"), "HiroTaskbar[Beta]");
        }

        private void InfoLabel_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.RunExe(HSet.Read_Ini(App.dConfig, "Taskbar", "RightAction", "nop"), "HiroTaskbar[Beta]");
        }
        private void ExtraLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.RunExe(HSet.Read_Ini(App.dConfig, "Taskbar", BasicGrid.Visibility == Visibility.Visible ? "LeftActionX" : "LeftActionEX", "nop"), "HiroTaskbar[Beta]");
        }

        private void ExtraLabel_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.RunExe(HSet.Read_Ini(App.dConfig, "Taskbar", BasicGrid.Visibility == Visibility.Visible ? "RightActionX" : "RightActionEX", "nop"), "HiroTaskbar[Beta]");
        }

        private void ExtraLabel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.RunExe(HSet.Read_Ini(App.dConfig, "Taskbar", BasicGrid.Visibility == Visibility.Visible ? "DoubleActionX" : "DoubleActionEX", "nop"), "HiroTaskbar[Beta]");
        }

        private void InfoLabel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.RunExe(HSet.Read_Ini(App.dConfig, "Taskbar", "DoubleAction", "nop"), "HiroTaskbar[Beta]");
        }

        private void InfoLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var section = (e.ChangedButton) switch
            {
                MouseButton.Left => "Left",
                MouseButton.Right => "Right",
                MouseButton.Middle => "Middle",
                MouseButton.XButton1 => "XButton1",
                MouseButton.XButton2 => "XButton2",
                _ => "Default"
            };
            section += (e.ClickCount) switch
            {
                1 => "Action",
                2 => "DoubleAction",
                3 => "TripleAction",
                int n => n.ToString() + "Action"
            };
            Hiro_Utils.RunExe(HSet.Read_Ini(App.dConfig, "Taskbar", section, "nop"), "HiroTaskbar[Beta]");
        }

        private void ExtraLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var section = (e.ChangedButton) switch
            {
                MouseButton.Left => "Left",
                MouseButton.Right => "Right",
                MouseButton.Middle => "Middle",
                MouseButton.XButton1 => "XButton1",
                MouseButton.XButton2 => "XButton2",
                _ => "Default"
            };
            section += (e.ClickCount) switch
            {
                1 => "Action",
                2 => "DoubleAction",
                3 => "TripleAction",
                int n => n.ToString() + "Action"
            };
            if (BasicGrid.Visibility == Visibility.Visible)
                section += "X";
            else
                section += "EX";
            Hiro_Utils.RunExe(HSet.Read_Ini(App.dConfig, "Taskbar", section, "nop"), "HiroTaskbar[Beta]");
        }
    }
}
