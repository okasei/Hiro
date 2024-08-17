using System;
using System.IO;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Hiro.ModelViews;
using static Hiro.Helpers.HSet;

namespace Hiro.Helpers
{
    internal class HUI
    {
        internal static void SetCustomWindowIcon(Window win)
        {
            var iconP = HSet.Read_PPDCIni("CustomIcon", "");
            if (System.IO.File.Exists(iconP))
            {
                try
                {
                    win.Icon = Hiro_Utils.GetBitmapImage(iconP);
                }
                catch (Exception e)
                {
                    HLogger.LogError(e, "Hiro.Window.CustomIcon");
                }
            }
        }

        internal static void CopyFontFromLabel(Label from, TextBlock to)
        {

            to.FontFamily = from.FontFamily;
            to.FontSize = from.FontSize;
            to.FontStretch = from.FontStretch;
            to.FontWeight = from.FontWeight;
            to.FontStyle = from.FontStyle;
        }

        internal static void CopyPostionFromLabel(Label from, TextBlock to)
        {
            to.Margin = from.Margin;
            to.HorizontalAlignment = from.HorizontalAlignment;
            to.VerticalAlignment = from.VerticalAlignment;
            to.Width = from.Width;
            to.Height = from.Height;
            to.Padding = from.Padding;
        }

        #region UI 相关
        public static void Get_Text_Visual_Width(ContentControl sender, double pixelPerDip, out Size size)
        {
            var formattedText = new FormattedText(
                sender.Content.ToString(), System.Globalization.CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface(sender.FontFamily, sender.FontStyle, sender.FontWeight, sender.FontStretch),
                sender.FontSize, Brushes.Black, pixelPerDip);
            size.Width = formattedText.Width + sender.Padding.Left + sender.Padding.Right;
            size.Height = formattedText.Height + sender.Padding.Top + sender.Padding.Bottom;
        }

        public static void Set_Acrylic(Label? sender, Window win, WindowChrome? windowChrome = null, WindowAccentCompositor? compositor = null)
        {
            if (win != null && compositor != null)
            {
                win.Background = null;
                if (sender != null)
                    sender.Visibility = Visibility.Collapsed;
                if (windowChrome != null)
                    windowChrome.GlassFrameThickness = new(0, 0, 1, 0);
                var colorOptions = Read_Ini(App.dConfig, "Config", "AcrylicMode", "2");//0 - White, 1 - Black, 2 = Customize
                var colorTransparency = 0;
                if (!int.TryParse(Read_Ini(App.dConfig, "Config", "AcrylicTransparency", "71"), out colorTransparency))
                    colorTransparency = 0x47;
                colorTransparency = Math.Max(colorTransparency, 1);
                colorTransparency = Math.Min(colorTransparency, 255);
                Color acrylicColor = (colorOptions) switch
                {
                    "0" => Colors.White,
                    "1" => Colors.Black,
                    _ => App.AppAccentColor
                };
                compositor.Color = Color.FromArgb((byte)colorTransparency, acrylicColor.R, acrylicColor.G, acrylicColor.B);
                compositor.IsEnabled = true;
                if (windowChrome != null)
                    windowChrome.GlassFrameThickness = new(0);
                return;
            }
        }
        public static void Set_Bgimage(Label sender, Window win, string? strFileName = null, bool? ignoreAnimation = false, WindowChrome? windowChrome = null, WindowAccentCompositor? compositor = null)
        {
            if (Read_Ini(App.dConfig, "Config", "Background", "1").Equals("3"))
            {
                sender.Visibility = Visibility.Collapsed;
                win.Background = null;
                return;
            }
            if (sender.Visibility != Visibility.Visible)
                sender.Visibility = Visibility.Visible;
            //Bgimage
            Set_Opacity(sender, win);
            strFileName ??= HSet.Read_PPDCIni("BackImage", "");
            if (HSet.Read_DCIni("Background", "1").Equals("1") || !File.Exists(strFileName))
            {
                if (ignoreAnimation != false && !HSet.Read_DCIni("Ani", "2").Equals("0"))
                {
                    Storyboard? sb = new();
                    try
                    {
                        sb = HAnimation.AddColorAnimaton(App.AppAccentColor, 150, sender, "Background.Color", sb);
                        sb.Completed += delegate
                        {
                            sender.Background = new SolidColorBrush(App.AppAccentColor);
                            sb = null;
                        };
                        sb.Begin();
                    }
                    catch (Exception ex)
                    {
                        HLogger.LogError(ex, "Hiro.Exception.Animation");
                        sender.Background = new SolidColorBrush(App.AppAccentColor);
                    }

                }
                else
                    sender.Background = new SolidColorBrush(App.AppAccentColor);
            }
            else
            {
                BitmapImage? bi = Hiro_Utils.GetBitmapImage(strFileName);
                ImageBrush ib = new()
                {
                    Stretch = Stretch.UniformToFill,
                    ImageSource = bi
                };
                sender.Background = ib;
            }

        }
        public static Brush Set_Bgimage(Brush sender, Control win, string? strFileName = null)
        {
            //Bgimage
            strFileName ??= Read_PPDCIni("BackImage", "");
            if (Read_Ini(App.dConfig, "Config", "Background", "1").Equals("1") || !File.Exists(strFileName))
            {
                if (!Read_Ini(App.dConfig, "Config", "Ani", "2").Equals("0"))
                {
                    Storyboard? sb = new();
                    try
                    {
                        sb = HAnimation.AddColorAnimaton(App.AppAccentColor, 150, sender, "Color", sb);
                        sb.Completed += delegate
                        {
                            sender = new SolidColorBrush(App.AppAccentColor);
                            sb = null;
                        };
                        sb.Begin();
                    }
                    catch (Exception ex)
                    {
                        HLogger.LogError(ex, "Hiro.Exception.Animation");
                        sender = new SolidColorBrush(App.AppAccentColor);
                    }

                }
                else
                    sender = new SolidColorBrush(App.AppAccentColor);
            }
            else
            {
                BitmapImage? bi = Hiro_Utils.GetBitmapImage(strFileName);
                ImageBrush ib = new()
                {
                    Stretch = Stretch.UniformToFill,
                    ImageSource = bi
                };
                sender = ib;
            }
            return sender;
        }

        public static void Set_Opacity(FrameworkElement sender, Control? win = null)
        {
            if (Read_Ini(App.dConfig, "Config", "Background", "1").Equals("3"))
            {
                return;
            }
            if (!double.TryParse(Read_Ini(App.dConfig, "Config", "OpacityMask", "255"), out double to))
                to = 255;
            Color bg = Colors.White;
            switch (to)
            {
                case > 255:
                    to = 510 - to;
                    bg = Colors.White;
                    break;
                case < 255:
                    bg = Colors.Black;
                    break;
                default:
                    break;
            }
            Color dest = (to >= 0 && to <= 255) ?
                Color.FromArgb(Convert.ToByte(to), 0, 0, 0) : Color.FromArgb(255, 0, 0, 0);
            if (win != null)
                win.Background = new SolidColorBrush(bg);
            sender.OpacityMask = new SolidColorBrush(dest);
        }
        public static void Set_Foreground_Opacity(Border sender, Control? win = null)
        {
            if (!double.TryParse(Read_Ini(App.dConfig, "Config", "OpacityMask", "255"), out double to))
                to = 255;
            Color bg = Colors.White;
            switch (to)
            {
                case > 255:
                    to = 510 - to;
                    bg = Colors.White;
                    break;
                case < 255:
                    bg = Colors.Black;
                    break;
                default:
                    break;
            }
            sender.Background = new SolidColorBrush(bg);
            sender.Opacity = 1 - to / 255;
            if (win != null)
                win.Background = new SolidColorBrush(bg);
            //sender.OpacityMask = new SolidColorBrush(dest);
        }
        public static void Set_Control_Location(Control sender, string val, bool extra = false, string? path = null, bool right = false, bool bottom = false, bool location = true, bool animation = false, double animationTime = 150)
        {
            if (extra == false || path == null || !File.Exists(path))
                path = App.langFilePath;
            try
            {
                if (sender != null)
                {
                    if (right == true)
                        sender.HorizontalAlignment = HorizontalAlignment.Right;
                    if (bottom == true)
                        sender.VerticalAlignment = VerticalAlignment.Bottom;
                    var result = Hiro_Utils.HiroParse(Read_Ini(path, "location", val, string.Empty).Trim().Replace("%b", " ").Replace("{", "(").Replace("}", ")"));
                    if (!result[0].Equals("-1"))
                    {
                        var ff = result[0];
                        if (ff.StartsWith("$cf#", StringComparison.CurrentCultureIgnoreCase))
                        {
                            ff = ff[4..];
                            var rf = App.AddCustomFont(ff);
                            if (rf.Length > 0)
                            {
                                sender.FontFamily = new FontFamily(rf);
                            }
                        }
                        else
                        {
                            sender.FontFamily = new FontFamily(result[0]);
                        }
                    }
                    if (!result[1].Equals("-1"))
                        sender.FontSize = double.Parse(result[1]);
                    sender.FontStretch = result[2] switch
                    {
                        "1" => FontStretches.UltraCondensed,
                        "2" => FontStretches.ExtraCondensed,
                        "3" => FontStretches.Condensed,
                        "4" => FontStretches.SemiCondensed,
                        "5" => FontStretches.Medium,
                        "6" => FontStretches.SemiExpanded,
                        "7" => FontStretches.Expanded,
                        "8" => FontStretches.ExtraExpanded,
                        "9" => FontStretches.UltraExpanded,
                        _ => FontStretches.Normal
                    };
                    sender.FontWeight = result[3] switch
                    {
                        "1" => FontWeights.Thin,
                        "2" => FontWeights.UltraLight,
                        "3" => FontWeights.Light,
                        "4" => FontWeights.Medium,
                        "5" => FontWeights.SemiBold,
                        "6" => FontWeights.Bold,
                        "7" => FontWeights.UltraBold,
                        "8" => FontWeights.Black,
                        "9" => FontWeights.UltraBlack,
                        _ => FontWeights.Normal
                    };
                    sender.FontStyle = result[4] switch
                    {
                        "1" => FontStyles.Italic,
                        "2" => FontStyles.Oblique,
                        _ => FontStyles.Normal
                    };
                    if (location)
                    {
                        Size mSize = new();
                        var auto = result[7].ToLower().Equals("auto") || result[7].ToLower().Equals("nan") || result[7].Equals("-2") || result[7].Equals("0");
                        mSize.Width = auto ? double.NaN : (!result[7].Equals("-1")) ? double.Parse(result[7]) : sender.Width;
                        auto = result[8].ToLower().Equals("auto") || result[8].ToLower().Equals("nan") || result[8].Equals("-2") || result[8].Equals("0");
                        mSize.Height = auto ? double.NaN : (!result[8].Equals("-1")) ? double.Parse(result[8]) : sender.Height;
                        Thickness thickness = new()
                        {
                            Left = (!result[5].Equals("-1")) ? right ? 0.0 : double.Parse(result[5]) : sender.Margin.Left,
                            Right = (!result[5].Equals("-1")) ? !right ? sender.Margin.Right : double.Parse(result[5]) : sender.Margin.Right,
                            Top = (!result[6].Equals("-1")) ? bottom ? 0.0 : double.Parse(result[6]) : sender.Margin.Top,
                            Bottom = (!result[6].Equals("-1")) ? !bottom ? sender.Margin.Bottom : double.Parse(result[6]) : sender.Margin.Bottom
                        };
                        if (!animation)
                        {
                            sender.Width = mSize.Width;
                            sender.Height = mSize.Height;
                            sender.Margin = thickness;
                        }
                        else
                        {
                            Storyboard sb = new();
                            HAnimation.AddThicknessAnimaton(thickness, animationTime, sender, "Margin", sb);
                            if (!double.IsNaN(mSize.Height))
                                HAnimation.AddDoubleAnimaton(mSize.Height, animationTime, sender, "Height", sb);
                            if (!double.IsNaN(mSize.Width))
                                HAnimation.AddDoubleAnimaton(mSize.Width, animationTime, sender, "Width", sb);
                            sb.Completed += delegate
                            {
                                sender.Width = mSize.Width;
                                sender.Height = mSize.Height;
                                sender.Margin = thickness;
                            };
                            sb.Begin();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, $"Hiro.Exception.Location{Environment.NewLine}Path: {val}");
            }

        }

        public static void Set_Mac_Location(Control mac, string mval, Control name, bool animation = false, double animationTime = 150)
        {
            try
            {
                if (mac != null && name != null)
                {
                    var result = Hiro_Utils.HiroParse(Read_Ini(App.langFilePath, "location", mval, string.Empty).Trim().Replace("%b", " ").Replace("{", "(").Replace("}", ")"));
                    if (!result[0].Equals("-1"))
                        mac.FontFamily = new FontFamily(result[0]);
                    if (!result[1].Equals("-1"))
                        mac.FontSize = double.Parse(result[1]);
                    mac.FontStretch = result[2] switch
                    {
                        "1" => FontStretches.UltraCondensed,
                        "2" => FontStretches.ExtraCondensed,
                        "3" => FontStretches.Condensed,
                        "4" => FontStretches.SemiCondensed,
                        "5" => FontStretches.Medium,
                        "6" => FontStretches.SemiExpanded,
                        "7" => FontStretches.Expanded,
                        "8" => FontStretches.ExtraExpanded,
                        "9" => FontStretches.UltraExpanded,
                        _ => FontStretches.Normal
                    };
                    mac.FontWeight = result[3] switch
                    {
                        "1" => FontWeights.Thin,
                        "2" => FontWeights.UltraLight,
                        "3" => FontWeights.Light,
                        "4" => FontWeights.Medium,
                        "5" => FontWeights.SemiBold,
                        "6" => FontWeights.Bold,
                        "7" => FontWeights.UltraBold,
                        "8" => FontWeights.Black,
                        "9" => FontWeights.UltraBlack,
                        _ => FontWeights.Normal
                    };
                    mac.FontStyle = result[4] switch
                    {
                        "1" => FontStyles.Italic,
                        "2" => FontStyles.Oblique,
                        _ => FontStyles.Normal
                    };
                    Thickness thickness = new()
                    {
                        Left = (!result[5].Equals("-1")) ? double.Parse(result[5]) + name.Margin.Left + name.ActualWidth + 5 : name.Margin.Left + name.ActualWidth + 5,
                        Right = 0,
                        Top = (!result[6].Equals("-1")) ? double.Parse(result[6]) : name.Margin.Top,
                        Bottom = 0
                    };
                    var width = (!result[7].Equals("-1")) ? double.Parse(result[7]) : double.NaN;
                    var height = (!result[8].Equals("-1")) ? double.Parse(result[8]) : double.NaN;
                    if (!animation)
                    {
                        mac.Margin = thickness;
                        mac.Width = width;
                        mac.Height = height;
                    }
                    else
                    {
                        Storyboard sb = new();
                        HAnimation.AddThicknessAnimaton(thickness, animationTime, mac, "Margin", sb);
                        if (!double.IsNaN(height))
                            HAnimation.AddDoubleAnimaton(height, animationTime, mac, "Height", sb, null);
                        if (!double.IsNaN(width))
                            HAnimation.AddDoubleAnimaton(width, animationTime, mac, "Width", sb, null);
                        sb.Completed += delegate
                        {
                            mac.Margin = thickness;
                            mac.Width = width;
                            mac.Height = height;
                        };
                        sb.Begin();
                    }
                }
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, $"Hiro.Exception.Location{Environment.NewLine}Path: {mval}");
            }

        }

        public static void Set_MacFrame_Location(FrameworkElement mac, string mval, Control name, bool animation = false, double animationTime = 150)
        {
            try
            {
                if (mac != null && name != null)
                {
                    var result = Hiro_Utils.HiroParse(HSet.Read_Ini(App.langFilePath, "location", mval, string.Empty).Trim().Replace("%b", " ").Replace("{", "(").Replace("}", ")"));
                    Thickness thickness = new()
                    {
                        Left = (!result[0].Equals("-1")) ? double.Parse(result[0]) + name.Margin.Left + name.ActualWidth + 5 : name.Margin.Left + name.ActualWidth + 5,
                        Right = 0,
                        Top = (!result[1].Equals("-1")) ? double.Parse(result[1]) : name.Margin.Top,
                        Bottom = 0
                    };
                    var width = (!result[2].Equals("-1")) ? double.Parse(result[2]) : double.NaN;
                    var height = (!result[3].Equals("-1")) ? double.Parse(result[3]) : double.NaN;
                    if (!animation)
                    {
                        mac.Margin = thickness;
                        mac.Width = width;
                        mac.Height = height;
                    }
                    else
                    {
                        Storyboard sb = new();
                        HAnimation.AddThicknessAnimaton(thickness, animationTime, mac, "Margin", sb);
                        if (!double.IsNaN(height))
                            HAnimation.AddDoubleAnimaton(height, animationTime, mac, "Height", sb, null);
                        if (!double.IsNaN(width))
                            HAnimation.AddDoubleAnimaton(width, animationTime, mac, "Width", sb, null);
                        sb.Completed += delegate
                        {
                            mac.Margin = thickness;
                            mac.Width = width;
                            mac.Height = height;
                        };
                        sb.Begin();
                    }
                }
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, $"Hiro.Exception.Location{Environment.NewLine}Path: {mval}");
            }

        }

        public static Thickness Get_FrameworkElement_Location(string val)
        {
            Thickness thickness = new(0);
            try
            {
                var loc = HSet.Read_Ini(App.langFilePath, "location", val, string.Empty);
                loc = loc.Replace(" ", "").Replace("%b", " ");
                loc = loc[(loc.IndexOf("{") + 1)..];
                loc = loc[..loc.LastIndexOf("}")];
                var left = loc[..loc.IndexOf(",")];
                loc = loc[(left.Length + 1)..];
                var top = loc[..loc.IndexOf(",")];
                loc = loc[(top.Length + 1)..];
                var width = loc[..loc.IndexOf(",")];
                loc = loc[(width.Length + 1)..];
                var height = loc;
                thickness.Right = width.Equals("-1") ? double.NaN : double.Parse(width);
                thickness.Bottom = height.Equals("-1") ? double.NaN : double.Parse(height);
                if (!left.Equals("-1"))
                    thickness.Left = double.Parse(left);
                if (!top.Equals("-1"))
                    thickness.Top = double.Parse(top);
                return thickness;
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, $"Hiro.Exception.Location.Grid{Environment.NewLine}Path: {val}");
                return new(0);
            }
        }

        public static void Set_FrameworkElement_Location(FrameworkElement sender, string val, bool animation = false, double animationTime = 150, double dRatio = 0.9, double aRatio = 0)
        {
            Size mSize = new();
            Thickness thickness = sender.Margin;
            try
            {
                if (sender != null)
                {
                    var loc = HSet.Read_Ini(App.langFilePath, "location", val, string.Empty);
                    loc = loc.Replace(" ", "").Replace("%b", " ");
                    loc = loc[(loc.IndexOf("{") + 1)..];
                    loc = loc[..loc.LastIndexOf("}")];
                    var left = loc[..loc.IndexOf(",")];
                    loc = loc[(left.Length + 1)..];
                    var top = loc[..loc.IndexOf(",")];
                    loc = loc[(top.Length + 1)..];
                    var width = loc[..loc.IndexOf(",")];
                    loc = loc[(width.Length + 1)..];
                    var height = loc;
                    mSize.Width = width.Equals("-1") ? double.NaN : double.Parse(width);
                    mSize.Height = height.Equals("-1") ? double.NaN : double.Parse(height);
                    if (!left.Equals("-1"))
                    {
                        if (sender.HorizontalAlignment == HorizontalAlignment.Right)
                            thickness.Right = double.Parse(left);
                        else
                            thickness.Left = double.Parse(left);
                    }
                    if (!top.Equals("-1"))
                    {
                        if (sender.VerticalAlignment == VerticalAlignment.Bottom)
                            thickness.Bottom = double.Parse(top);
                        else
                            thickness.Top = double.Parse(top);
                    }
                    if (!animation)
                    {
                        sender.Width = mSize.Width;
                        sender.Height = mSize.Height;
                        sender.Margin = thickness;
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Storyboard sb = new();
                            HAnimation.AddThicknessAnimaton(thickness, animationTime, sender, "Margin", sb, null, dRatio, aRatio);
                            if (!double.IsNaN(mSize.Height))
                                HAnimation.AddDoubleAnimaton(mSize.Height, animationTime, sender, "Height", sb, null, dRatio, aRatio);
                            if (!double.IsNaN(mSize.Width))
                                HAnimation.AddDoubleAnimaton(mSize.Width, animationTime, sender, "Width", sb, null, dRatio, aRatio);
                            sb.Completed += delegate
                            {
                                sender.Width = mSize.Width;
                                sender.Height = mSize.Height;
                                sender.Margin = thickness;
                            };
                            sb.Begin();
                        });

                    }

                }
            }
            catch (Exception ex)
            {
                sender.Width = mSize.Width;
                sender.Height = mSize.Height;
                sender.Margin = thickness;
                HLogger.LogError(ex, $"Hiro.Exception.Location.Grid{Environment.NewLine}Path: {val}");
            }

        }
        #endregion
    }
}
