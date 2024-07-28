using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace hiro
{
    /// <summary>
    /// Hiro_Test.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Test : Window
    {
        internal Inline? inline = null;
        public Hiro_Test()
        {
            InitializeComponent();
            Helpers.Hiro_UI.SetCustomWindowIcon(this);
            Load_Emoji();
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 180));
            Resources["AppForeDisabled"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 180));
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
            Resources["AppAccentDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 180));
        }

        internal void Load_Emoji()
        {
            var w = 0.0;
            var h = 0.0;
            var cw = -25.0;
            Dispatcher.Invoke(() =>
            {
                Emoji_Platte.Children.Clear();
                Emoji_Platte.Margin = new Thickness(0);
                w = Emoji_Platte.Width;
            });
            var imgList = new DirectoryInfo(Path.GetDirectoryName(Hiro_Utils.Path_Prepare(@"<current>\system\emoji\")) ?? "")
                .GetFiles("*", SearchOption.TopDirectoryOnly).Select(s => s.FullName).ToList();
            foreach (var img in imgList)
            {
                var im = new Image()
                {
                    Source = new BitmapImage(new Uri(img, UriKind.Absolute)),
                    Stretch = Stretch.Uniform,
                    Width = 25,
                    Height = 25,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                im.MouseDown += delegate
                {
                    Dispatcher.Invoke(() =>
                    {
                        ChatContent.Selection.Select(ChatContent.Selection.End, ChatContent.Selection.End);
                        var cp = ChatContent.CaretPosition;
                        var pi = cp.Parent as Inline;
                        var par = cp.Paragraph;
                        inline = pi ?? inline;
                        if (ChatContent.Document.Blocks.Count <= 0 || par.Inlines.Count <= 0)
                        {
                            pi = null;
                            inline = null;
                        }
                        if (pi != null)
                        {
                            if (pi is InlineUIContainer)
                            {
                                Insert_Picture_After(par, new BitmapImage(new Uri(img, UriKind.Absolute)), pi, $"[Hiro.Emoji:{Path.GetFileName(img)}]");
                                inline = pi;
                            }
                            else
                            {
                                var ppre = pi.PreviousInline;
                                var pnex = pi.NextInline;
                                var pitext = new TextRange(pi.ContentStart, ChatContent.CaretPosition).Text;
                                var pitextend = new TextRange(ChatContent.CaretPosition, pi.ContentEnd).Text;
                                var p1 = new Run(pitext);
                                var p2 = new Run(pitextend);
                                if (ppre == null)
                                {
                                    if (pnex == null)
                                    {
                                        //pi is the only inline
                                        par.Inlines.Clear();
                                        par.Inlines.Add(p1);
                                        Insert_Picture_After(par, new BitmapImage(new Uri(img, UriKind.Absolute)), p1, $"[Hiro.Emoji:{Path.GetFileName(img)}]");
                                        par.Inlines.Add(p2);
                                    }
                                    else
                                    {
                                        //pi is the first inline
                                        par.Inlines.Remove(pi);
                                        par.Inlines.InsertBefore(pnex, p1);
                                        Insert_Picture_Before(par, new BitmapImage(new Uri(img, UriKind.Absolute)), pnex, $"[Hiro.Emoji:{Path.GetFileName(img)}]");
                                        par.Inlines.InsertBefore(pnex, p2);
                                    }
                                }
                                else
                                {
                                    //pi is not the first inline
                                    par.Inlines.Remove(pi);
                                    par.Inlines.InsertAfter(ppre, p2);
                                    Insert_Picture_After(par, new BitmapImage(new Uri(img, UriKind.Absolute)), ppre, $"[Hiro.Emoji:{Path.GetFileName(img)}]");
                                    par.Inlines.InsertAfter(ppre, p1);
                                }
                                if (p1.NextInline != null)
                                    inline = p1.NextInline;
                                else
                                    inline = p1;
                            }

                        }
                        else
                        {
                            var pnex = ChatContent.CaretPosition.GetAdjacentElement(LogicalDirection.Forward) as Inline;
                            var ppre = ChatContent.CaretPosition.GetAdjacentElement(LogicalDirection.Backward) as Inline;
                            if (pnex == null)
                            {
                                if (ppre == null)
                                {
                                    par.Inlines.Clear();
                                }
                                inline = Insert_Picture_After(par, new BitmapImage(new Uri(img, UriKind.Absolute)), ppre, $"[Hiro.Emoji:{Path.GetFileName(img)}]");

                            }
                            else
                            {
                                inline = Insert_Picture_Before(par, new BitmapImage(new Uri(img, UriKind.Absolute)), pnex, $"[Hiro.Emoji:{Path.GetFileName(img)}]");
                            }
                        }
                        ChatContent.CaretPosition = inline?.ElementEnd;
                    });
                    
                    /*foreach (var i in ChatContent.CaretPosition.Paragraph.Inlines)
                    {
                        switch(i)
                        {
                            case InlineUIContainer iuc:
                                MessageBox.Show(iuc.Tag.ToString());
                                break;
                            case Inline ic:
                                var textRange = new TextRange(ic.ContentStart, ic.ContentEnd);
                                MessageBox.Show(textRange.Text);
                                break;
                            default:
                                break;
                        }
                    }*/
                };
                im.MouseEnter += delegate
                {
                    Dispatcher.Invoke(() =>
                    {
                        Storyboard sb = new();
                        Hiro_Utils.AddThicknessAnimaton(im.Margin, 250, FocusLabel, "Margin", sb);
                        Hiro_Utils.AddDoubleAnimaton(im.Height, 250, FocusLabel, "Height", sb);
                        Hiro_Utils.AddDoubleAnimaton(im.Width, 250, FocusLabel, "Width", sb);
                        Hiro_Utils.AddDoubleAnimaton(1, 250, FocusLabel, "Opacity", sb);
                        sb.Completed += delegate
                        {
                            FocusLabel.Width = im.Width;
                            FocusLabel.Height = im.Height;
                            FocusLabel.Margin = im.Margin;
                            FocusLabel.Opacity = 1;
                        };
                        sb.Begin();
                    });
                    
                };
                if (cw + 35 > w)
                {
                    h += 30;
                    cw = 5;
                }
                else
                {
                    cw += 30;
                }
                Dispatcher.Invoke(() =>
                {
                    im.Margin = new Thickness(cw, h, 0, 0);
                    Emoji_Platte.Children.Add(im);
                });
            }
            Dispatcher.Invoke(() =>
            {
                Emoji_Platte.Height = h + 30;
            });
        }

        private void Emoji_Platte_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender == Emoji_Platte)
            {
                Storyboard sb = new();
                Hiro_Utils.AddDoubleAnimaton(0, 250, FocusLabel, "Opacity", sb);
                sb.Completed += delegate
                {
                    FocusLabel.Opacity = 0;
                };
                sb.Begin();
            }

        }

        private InlineUIContainer Insert_Picture_After(Paragraph pa, BitmapImage bi, Inline? current, string id)
        {
            InlineUIContainer iuc = new InlineUIContainer(new Image() { Source = bi, Stretch = Stretch.UniformToFill, Width = 25 }) { Tag = id };
            if (current == null)
                pa.Inlines.Add(iuc);
            else
                pa.Inlines.InsertAfter(current, iuc);
            return iuc;
        }

        private InlineUIContainer Insert_Picture_Before(Paragraph pa, BitmapImage bi, Inline? current, string id)
        {
            InlineUIContainer iuc = new InlineUIContainer(new Image() { Source = bi, Stretch = Stretch.UniformToFill, Width = 25 }) { Tag = id };
            if (current == null)
                pa.Inlines.Add(iuc);
            else
                pa.Inlines.InsertBefore(current, iuc);
            return iuc;
        }

        private void HiroBtn_Click(object sender, RoutedEventArgs e)
        {
            var txt = string.Empty;
            var cs = ChatContent.Document.ContentStart;
            var ce = ChatContent.Document.ContentEnd;
            var ps = ChatContent.CaretPosition.Paragraph.ContentStart;
            var pe = ChatContent.CaretPosition.Paragraph.ContentEnd;
            txt = new TextRange(cs, ChatContent.CaretPosition).Text + "\r\n"
                + new TextRange(ChatContent.CaretPosition, ce).Text + "\r\n"
                + new TextRange(ps, ChatContent.CaretPosition).Text + "\r\n"
                + new TextRange(ChatContent.CaretPosition, pe).Text + "\r\n";
            MessageBox.Show(txt);
        }
    }
}
