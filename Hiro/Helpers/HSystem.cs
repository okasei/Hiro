using Hiro.Widgets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Hiro.Helpers
{
    internal class HSystem
    {
        internal static void ShowWebConfirmDialog(bool autoClose, string path, string? source)
        {
            var acbak = autoClose;
            var confrimWin = HText.Path_PPX("<capp>\\<lang>\\url.hms");
            Hiro_Utils.HiroInvoke(() =>
            {
                Hiro_Background? bg = null;
                if (HSet.Read_Ini(confrimWin, "Action", "Background", "true").ToLower().Equals("true"))
                    bg = new();
                Hiro_Msg msg = new(confrimWin)
                {
                    bg = bg,
                    Title = HText.Get_Translate("msgTitle").Replace("%t", HText.Path_PPX(HSet.Read_Ini(confrimWin, "Message", "Title", HText.Get_Translate("syntax")))).Replace("%a", App.appTitle)
                };
                msg.backtitle.Content = HSet.Read_PPIni(confrimWin, "Message", "Title", HText.Get_Translate("syntax"));
                msg.acceptbtn.Content = HSet.Read_Ini(confrimWin, "Message", "accept", HText.Get_Translate("msgaccept"));
                msg.rejectbtn.Content = HSet.Read_Ini(confrimWin, "Message", "reject", HText.Get_Translate("msgreject"));
                msg.cancelbtn.Content = HSet.Read_Ini(confrimWin, "Message", "cancel", HText.Get_Translate("msgcancel"));
                confrimWin = HSet.Read_PPIni(confrimWin, "Message", "content", HText.Get_Translate("syntax"));
                if (HText.StartsWith(confrimWin, "http://") || HText.StartsWith(confrimWin, "https://"))
                {
                    msg.sv.Content = HText.Get_Translate("msgload");
                    BackgroundWorker bw = new();
                    bw.DoWork += delegate
                    {
                        confrimWin = HNet.GetWebContent(confrimWin).Replace("<br>", "\\n");
                    };
                    bw.RunWorkerCompleted += delegate
                    {
                        msg.sv.Content = confrimWin.Replace("\\n", Environment.NewLine);
                    };
                    bw.RunWorkerAsync();
                }
                else if (System.IO.File.Exists(confrimWin))
                    msg.sv.Content = HText.Path_PPX(System.IO.File.ReadAllText(confrimWin)).Replace("\\n", Environment.NewLine);
                else
                    msg.sv.Content = confrimWin.Replace("\\n", Environment.NewLine);
                msg.Load_Position();
                msg.OKButtonPressed += delegate
                {
                    Hiro_Utils.RunExe(path, source, acbak, false);
                };
                msg.CancelButtonPressed += delegate
                {
                    if (acbak && App.mn == null)
                        Hiro_Utils.RunExe("exit()");
                };
                msg.RejectButtonPressed += delegate
                {
                    if (acbak && App.mn == null)
                        Hiro_Utils.RunExe("exit()");
                };
                msg.Show();
            });
        }

        internal static void ShowBadge(List<string> parameter)
        {
            var pn = parameter.Count;
            if (pn > 0)
            {
                var badge = parameter[0];
                if (System.IO.File.Exists(badge))
                {
                    Hiro_Utils.HiroInvoke(() =>
                    {
                        var hib = new Hiro_Badge(badge);
                        if (pn > 1)
                        {
                            if (int.Parse(parameter[1]) == 1)
                            {
                                hib.EllipseBorder.Visibility = Visibility.Collapsed;
                                hib.RectangleBorder.Visibility = Visibility.Visible;
                            }
                            if (pn > 2)
                            {
                                hib.Width = double.Parse(parameter[2]);
                                hib.Height = double.Parse(parameter[2]);
                                if (pn > 3)
                                {
                                    hib.Badge_Ellipse.StrokeThickness = double.Parse(parameter[3]);
                                    hib.Badge_Rectangle.StrokeThickness = double.Parse(parameter[3]);
                                    if (pn > 6)
                                    {
                                        var p = int.Parse(parameter[4]);
                                        var l = double.Parse(parameter[5]);
                                        var t = double.Parse(parameter[6]);
                                        switch (p)
                                        {
                                            case 1:
                                                {
                                                    hib.Loaded += (e, args) =>
                                                    {
                                                        Hiro_Utils.HiroInvoke(() =>
                                                        {
                                                            Canvas.SetLeft(hib, SystemParameters.PrimaryScreenWidth - l);
                                                            Canvas.SetTop(hib, t);
                                                        });
                                                    };
                                                    break;
                                                }

                                            case 2:
                                                {

                                                    hib.Loaded += (e, args) =>
                                                    {
                                                        Hiro_Utils.HiroInvoke(() =>
                                                        {
                                                            Canvas.SetLeft(hib, l);
                                                            Canvas.SetTop(hib, SystemParameters.PrimaryScreenHeight - t);
                                                        });
                                                    };
                                                    break;
                                                }

                                            case 3:
                                                {
                                                    hib.Loaded += (e, args) =>
                                                    {
                                                        Hiro_Utils.HiroInvoke(() =>
                                                        {
                                                            Canvas.SetLeft(hib, SystemParameters.PrimaryScreenWidth - l);
                                                            Canvas.SetTop(hib, SystemParameters.PrimaryScreenHeight - t);
                                                        });
                                                    };
                                                    break;
                                                }
                                            default:
                                                {
                                                    hib.Loaded += (e, args) =>
                                                    {
                                                        Hiro_Utils.HiroInvoke(() =>
                                                        {
                                                            Canvas.SetLeft(hib, l);
                                                            Canvas.SetTop(hib, t);
                                                        });
                                                    };
                                                    break;
                                                }
                                        }
                                    }
                                }
                            }
                        }
                        hib.Show();
                    });
                }
            }
        }

        internal static void TakeScreenShot(List<string> parameter)
        {
            if (parameter.Count == 0)
            {
                new Hiro_Screenshot().Show();
                return;
            }
            else
            {
                var _fs = false;
                var _oc = false;
                if (parameter.Count > 0)
                {
                    var _p0 = $",{parameter[0]},";
                    if (_p0.Contains("f", StringComparison.CurrentCultureIgnoreCase))
                    {
                        //直接截取全屏
                        _fs = true;
                    }
                    if (_p0.Contains("o", StringComparison.CurrentCultureIgnoreCase))
                    {
                        //直接截取全屏
                        _oc = true;
                    }
                    if (parameter.Count > 1)
                    {
                        //倒计时截屏
                        var cd = 5.0;
                        double.TryParse(parameter[1], out cd);
                        DispatcherTimer _dt = new DispatcherTimer()
                        {
                            Interval = TimeSpan.FromSeconds(cd)
                        };
                        _dt.Tick += (e, args) =>
                        {
                            new Hiro_Screenshot(_fs, _oc).Show();
                            _dt.Stop();
                        };
                        _dt.Start();
                    }
                    else
                    {
                        new Hiro_Screenshot(_fs, _oc).Show();
                    }
                }
            }
        }
    }
}
