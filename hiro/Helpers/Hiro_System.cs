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

namespace hiro.Helpers
{
    internal class Hiro_System
    {
        internal static void ShowWebConfirmDialog(bool autoClose, string path, string? source)
        {
            var acbak = autoClose;
            var confrimWin = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare("<capp>\\<lang>\\url.hms"));
            Hiro_Utils.HiroInvoke(() =>
            {
                Hiro_Background? bg = null;
                if (Hiro_Utils.Read_Ini(confrimWin, "Action", "Background", "true").ToLower().Equals("true"))
                    bg = new();
                Hiro_Msg msg = new(confrimWin)
                {
                    bg = bg,
                    Title = Hiro_Utils.Path_Prepare(Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Read_Ini(confrimWin, "Message", "Title", Hiro_Utils.Get_Translate("syntax")))) + " - " + App.appTitle
                };
                msg.backtitle.Content = Hiro_Utils.Path_Prepare(Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Read_Ini(confrimWin, "Message", "Title", Hiro_Utils.Get_Translate("syntax")))));
                msg.acceptbtn.Content = Hiro_Utils.Read_Ini(confrimWin, "Message", "accept", Hiro_Utils.Get_Translate("msgaccept"));
                msg.rejectbtn.Content = Hiro_Utils.Read_Ini(confrimWin, "Message", "reject", Hiro_Utils.Get_Translate("msgreject"));
                msg.cancelbtn.Content = Hiro_Utils.Read_Ini(confrimWin, "Message", "cancel", Hiro_Utils.Get_Translate("msgcancel"));
                confrimWin = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(confrimWin, "Message", "content", Hiro_Utils.Get_Translate("syntax"))));
                if (confrimWin.ToLower().StartsWith("http://") || confrimWin.ToLower().StartsWith("https://"))
                {
                    msg.sv.Content = Hiro_Utils.Get_Translate("msgload");
                    BackgroundWorker bw = new();
                    bw.DoWork += delegate
                    {
                        confrimWin = Hiro_Net.GetWebContent(confrimWin).Replace("<br>", "\\n");
                    };
                    bw.RunWorkerCompleted += delegate
                    {
                        msg.sv.Content = confrimWin.Replace("\\n", Environment.NewLine);
                    };
                    bw.RunWorkerAsync();
                }
                else if (System.IO.File.Exists(confrimWin))
                    msg.sv.Content = Hiro_Utils.Path_Prepare(Hiro_Utils.Path_Prepare_EX(System.IO.File.ReadAllText(confrimWin))).Replace("\\n", Environment.NewLine);
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
    }
}
