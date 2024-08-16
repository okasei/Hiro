using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static Hiro.Helpers.HSet;

namespace Hiro.Helpers
{
    internal class HAnimation
    {
        #region 动画相关

        #region 模糊动画
        public static void Blur_Animation(int direction, bool animation, Control label, System.Windows.Window win, BackgroundWorker? bw = null)
        {
            if (Read_Ini(App.dConfig, "Config", "Background", "1").Equals("3"))
            {
                if (bw != null)
                    bw.RunWorkerAsync();
                return;
            }
            //0: 25->0 12s  1:0->50 25s 2:0->25 12s 3:50->25 12s
            double start = direction switch
            {
                1 => 0.0,
                2 => 0.0,
                3 => 50.0,
                _ => 25.0
            };
            double end = direction switch
            {
                1 => 50.0,
                2 => 25.0,
                3 => 25.0,
                _ => 0.0
            };
            double time = direction switch
            {
                1 => 500.0,
                _ => 250.0
            };
            if (!animation)
            {
                Set_Animation_Label(end, label, win);
                if (bw != null)
                    bw.RunWorkerAsync();
                return;
            }
            else
            {
                bool comp = win.Width > win.Height;
                double dest = comp ? -end : -end * win.Height / win.Width;
                double stat = comp ? -start : -start * win.Height / win.Width;
                double desl = !comp ? -end : -end * win.Width / win.Height;
                double stal = !comp ? -start : -start * win.Width / win.Height;
                Set_Animation_Label(start, label, win);
                Storyboard? sb = new();
                sb = AddDoubleAnimaton(end, time, label, "Effect.Radius", sb, start);
                sb = AddDoubleAnimaton(win.Height - dest * 2, time, label, "Height", sb, win.Height - stat * 2);
                sb = AddDoubleAnimaton(win.Width - desl * 2, time, label, "Width", sb, win.Width - stal * 2);
                sb = AddThicknessAnimaton(new(desl, dest, 0, 0), time, label, "Margin", sb, new(stal, stat, 0, 0));
                sb.Completed += delegate
                {
                    Set_Animation_Label(end, label, win);
                    if (bw != null)
                        bw.RunWorkerAsync();
                    sb = null;
                };
                sb.Begin();
            }
        }

        public static void Blur_Out(Control ct, BackgroundWorker? bw = null)
        {
            if (!HSet.Read_Ini(App.dConfig, "Config", "Ani", "2").Equals("0"))
            {
                ct.Effect = new System.Windows.Media.Effects.BlurEffect()
                {
                    Radius = App.blurradius,
                    RenderingBias = System.Windows.Media.Effects.RenderingBias.Performance
                };
                Storyboard? sb = new();
                DoubleAnimation? da = new()
                {
                    From = App.blurradius,
                    To = 0.0,
                    Duration = TimeSpan.FromMilliseconds(App.blursec)
                };
                da.EasingFunction = GetEasingFunction(da.EasingFunction, true);
                Storyboard.SetTarget(da, ct);
                Storyboard.SetTargetProperty(da, new PropertyPath("Effect.Radius"));
                sb.Children.Add(da);
                sb.Completed += delegate
                {
                    ct.Effect = null;
                    if (bw != null)
                        bw.RunWorkerAsync();
                    da = null;
                    sb = null;
                };
                sb.Begin();
            }
            else
            {
                if (bw != null)
                    bw.RunWorkerAsync();
            }
        }
        private static void Set_Animation_Label(double rd, Control label, Window win)
        {
            label.Effect = new System.Windows.Media.Effects.BlurEffect()
            {
                Radius = rd,
                RenderingBias = System.Windows.Media.Effects.RenderingBias.Performance
            };
            Thickness tn = label.Margin;
            var WinWidth = win.WindowState == WindowState.Maximized ? win.ActualWidth : win.Width;
            var WinHeight = win.WindowState == WindowState.Maximized ? win.ActualHeight : win.Height;
            if (WinWidth > WinHeight)
            {
                tn.Top = -rd;
                tn.Left = -rd * WinWidth / WinHeight;
            }
            else
            {
                tn.Left = -rd;
                tn.Top = -rd * WinHeight / WinWidth;
            }
            label.Margin = tn;
            label.Width = WinWidth - tn.Left * 2;
            label.Height = WinHeight - tn.Top * 2;
        }
        #endregion

        #region 添加double动画
        public static Storyboard AddDoubleAnimaton(double? to, double mstime, DependencyObject value, string PropertyPath, Storyboard? sb, double? from = null, double decelerationRatio = 0.9, double accelerationRatio = 0)
        {
            sb ??= new();
            DoubleAnimation? da = new();
            if (from != null)
                da.From = from;
            if (to != null)
                da.To = to;
            da.Duration = TimeSpan.FromMilliseconds(mstime);
            da.DecelerationRatio = decelerationRatio;
            da.AccelerationRatio = accelerationRatio;
            da.EasingFunction = GetEasingFunction(da.EasingFunction, !".width;.height;.opacity;".Contains($".{PropertyPath};", StringComparison.CurrentCultureIgnoreCase));
            Storyboard.SetTarget(da, value);
            Storyboard.SetTargetProperty(da, new PropertyPath(PropertyPath));
            sb.Children.Add(da);
            sb.FillBehavior = FillBehavior.Stop;
            sb.Completed += (sender, args) =>
            {
                da = null;
                sb = null;
            };
            return sb;
        }
        #endregion

        #region 添加thickness动画
        public static Storyboard AddThicknessAnimaton(Thickness? to, double mstime, DependencyObject value, string PropertyPath, Storyboard? sb, Thickness? from = null, double DecelerationRatio = 0.9, double AccelerationRatio = 0)
        {
            sb ??= new();
            ThicknessAnimation? da = new();
            if (from != null)
                da.From = from;
            if (to != null)
                da.To = to;
            da.Duration = TimeSpan.FromMilliseconds(mstime);
            da.DecelerationRatio = DecelerationRatio;
            da.AccelerationRatio = AccelerationRatio;
            da.EasingFunction = GetEasingFunction(da.EasingFunction, true);
            Storyboard.SetTarget(da, value);
            Storyboard.SetTargetProperty(da, new PropertyPath(PropertyPath));
            sb.Children.Add(da);
            sb.FillBehavior = FillBehavior.Stop;
            sb.Completed += delegate
            {
                da = null;
                sb = null;
            };
            return sb;
        }
        #endregion 

        #region 添加Color动画
        public static Storyboard AddColorAnimaton(Color to, double mstime, DependencyObject value, string PropertyPath, Storyboard? sb, Color? from = null)
        {
            sb ??= new();
            ColorAnimation? da;
            if (from != null)
                da = new((Color)from, to, TimeSpan.FromMilliseconds(mstime));
            else
                da = new(to, TimeSpan.FromMilliseconds(mstime));
            da.DecelerationRatio = 0.9;
            da.EasingFunction = GetEasingFunction(da.EasingFunction, true);
            Storyboard.SetTarget(da, value);
            Storyboard.SetTargetProperty(da, new PropertyPath(PropertyPath));
            sb.Children.Add(da);
            sb.FillBehavior = FillBehavior.Stop;
            sb.Completed += delegate
            {
                da = null;
                sb = null;
            };
            return sb;
        }
        #endregion

        #region 重设缓动函数

        private static IEasingFunction GetEasingFunction(IEasingFunction _former, bool canBeNegative)
        {
            double _par = 0;
            int _ipar = 0;
            return (Read_DCIni("EasingFunction", string.Empty)) switch
            {
                "0" => new BackEase()
                {
                    EasingMode = (Read_DCIni("EasingMode", "0")) switch
                    {
                        "1" => EasingMode.EaseOut,
                        "2" => EasingMode.EaseInOut,
                        _ => EasingMode.EaseIn
                    },
                    Amplitude = canBeNegative ? (double.TryParse(Read_DCIni("EasingExtra", "0"), out _par) ? _par : 1) : 0
                },

                "1" => new BounceEase()
                {
                    EasingMode = (Read_DCIni("EasingMode", "0")) switch
                    {
                        "1" => EasingMode.EaseOut,
                        "2" => EasingMode.EaseInOut,
                        _ => EasingMode.EaseIn
                    },
                    Bounces = int.TryParse(Read_DCIni("EasingExtra", "0"), out _ipar) ? _ipar : 1,
                    Bounciness = double.TryParse(Read_DCIni("EasingExtraP", "0"), out _par) ? _par : 1
                },

                "2" => new CircleEase()
                {
                    EasingMode = (Read_DCIni("EasingMode", "0")) switch
                    {
                        "1" => EasingMode.EaseOut,
                        "2" => EasingMode.EaseInOut,
                        _ => EasingMode.EaseIn
                    }
                },

                "3" => new PowerEase()
                {
                    EasingMode = (Read_DCIni("EasingMode", "0")) switch
                    {
                        "1" => EasingMode.EaseOut,
                        "2" => EasingMode.EaseInOut,
                        _ => EasingMode.EaseIn
                    },
                    Power = double.TryParse(Read_DCIni("EasingExtra", "0"), out _par) ? _par : 1
                },

                "4" => new ElasticEase()
                {
                    EasingMode = (Read_DCIni("EasingMode", "0")) switch
                    {
                        "1" => EasingMode.EaseOut,
                        "2" => EasingMode.EaseInOut,
                        _ => EasingMode.EaseIn
                    },
                    Springiness = double.TryParse(Read_DCIni("EasingExtra", "0"), out _par) ? Math.Abs(_par) : 1,
                    Oscillations = int.TryParse(Read_DCIni("EasingExtraP", "0"), out _ipar) ? Math.Abs(_ipar) : 1
                },

                "5" => new ExponentialEase()
                {
                    EasingMode = (Read_DCIni("EasingMode", "0")) switch
                    {
                        "1" => EasingMode.EaseOut,
                        "2" => EasingMode.EaseInOut,
                        _ => EasingMode.EaseIn
                    },
                    Exponent = double.TryParse(Read_DCIni("EasingExtra", "0"), out _par) ? _par : 1
                },

                "6" => new SineEase()
                {
                    EasingMode = (Read_DCIni("EasingMode", "0")) switch
                    {
                        "1" => EasingMode.EaseOut,
                        "2" => EasingMode.EaseInOut,
                        _ => EasingMode.EaseIn
                    }
                },
                _ => _former

            };
        }

        #endregion

        #region 增强动效
        public static Storyboard AddPowerAnimation(int Direction, FrameworkElement value, Storyboard? sb, double? from = null, double? to = null, double mstime = 450, double opacityTime = 350)
        {

            sb = AddPower(Direction, value, sb, from, to, mstime);
            AddDoubleAnimaton(null, opacityTime, value, "Opacity", sb, 0);
            return sb;
        }
        public static Storyboard AddPowerOutAnimation(int Direction, FrameworkElement value, Storyboard? sb, double? from = null, double? to = null, double mstime = 450, double opacityTime = 350)
        {
            sb = AddPower(Direction, value, sb, from, to, mstime);
            AddDoubleAnimaton(0, opacityTime, value, "Opacity", sb, null);
            return sb;
        }

        private static Storyboard AddPower(int Direction, FrameworkElement value, Storyboard? sb, double? from = null, double? to = null, double mstime = 450)
        {
            sb ??= new();
            var th1 = value.Margin;
            var th2 = value.Margin;
            if (to != null && from != null)
            {
                if (Direction == 0)
                {
                    th1.Left += (double)from;
                    th2.Left += (double)to;
                }
                if (Direction == 1)
                {
                    th1.Top += (double)from;
                    th2.Top += (double)to;
                }
                if (Direction == 2)
                {
                    th1.Right += (double)from;
                    th2.Right += (double)to;
                }
                if (Direction == 3)
                {
                    th1.Bottom += (double)from;
                    th2.Bottom += (double)to;
                }
                AddThicknessAnimaton(th2, mstime, value, "Margin", sb, th1);
            }
            if (to != null && from == null)
            {
                if (Direction == 0)
                {
                    th2.Left += (double)to;
                }
                if (Direction == 1)
                {
                    th2.Top += (double)to;
                }
                if (Direction == 2)
                {
                    th2.Right += (double)to;
                }
                if (Direction == 3)
                {
                    th2.Bottom += (double)to;
                }
                AddThicknessAnimaton(th2, mstime, value, "Margin", sb, null);
            }
            if (to == null && from != null)
            {
                if (Direction == 0)
                {
                    th1.Left += (double)from;
                }
                if (Direction == 1)
                {
                    th1.Top += (double)from;
                }
                if (Direction == 2)
                {
                    th1.Right += (double)from;
                }
                if (Direction == 3)
                {
                    th1.Bottom += (double)from;
                }
                AddThicknessAnimaton(null, mstime, value, "Margin", sb, th1);
            }
            return sb;
        }
        #endregion

        #endregion
    }
}
