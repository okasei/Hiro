using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Media.Animation;
using Windows.Devices.WiFi;
using Windows.Devices.Radios;
using Windows.Security.Credentials;
using Windows.Security.Credentials.UI;
using System.Windows.Media.Imaging;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Windows.Shell;
using Hiro.Helpers;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using static Hiro.Helpers.HClass;
using static Hiro.Helpers.HText;
using static Hiro.Helpers.HLogger;
using static Hiro.Helpers.HSet;
using Hiro.ModelViews;
using Hiro.Resources;
using System.Windows.Media.Media3D;
using System.Buffers.Text;

namespace Hiro
{
    public partial class Hiro_Utils : Component
    {
        static int keyid = 0;

        #region 自动生成
        public Hiro_Utils()
        {
            InitializeComponent();
        }

        public Hiro_Utils(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
        #endregion

        #region 延时
        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)//毫秒
            {
                System.Windows.Forms.Application.DoEvents();//可执行某无聊的操作
            }
        }
        #endregion

        #region 运行文件

        public static void HiroInvoke(Action callback)
        {
            Application.Current.Dispatcher.Invoke(callback);
        }


        private static List<string> LoadPaths(string runPath, out string path, bool replaceBrackets = true)
        {
            if (runPath.StartsWith('\"') && runPath.EndsWith('\"'))
            {
                var _path = runPath[1..^1];
                if (_path.IndexOf('\"') == -1)
                {
                    _path = SearchAny(_path);
                    if (File.Exists(_path) || Directory.Exists(_path))
                    {
                        path = _path;
                        return new List<string>() { _path };
                    }
                    else
                    {
                        path = _path;
                        return HiroCmdParse(path, replaceBrackets);
                    }
                }
            }
            else
            {
                var _path = SearchAny(runPath);
                if (File.Exists(_path))
                {
                    path = _path;
                    return new List<string>() { _path };
                }
            }
            path = runPath;
            return HiroCmdParse(path, replaceBrackets);
        }

        private static string SearchAny(string pi)
        {
            if (pi.ToLower().EndsWith("<any>"))
            {
                pi = pi[..^5];
                if (Directory.Exists(pi))
                {
                    DirectoryInfo directory = new DirectoryInfo(pi);
                    var files = directory.GetFiles("*", SearchOption.TopDirectoryOnly);
                    var ImgList = files.Select(s => s.FullName).ToList();
                    if (ImgList.Count > 1)
                        return ImgList[new Random().Next(0, ImgList.Count - 1)];
                }
            }
            if (pi.ToLower().EndsWith("<xany>"))
            {
                pi = pi[..^6];
                if (Directory.Exists(pi))
                {
                    DirectoryInfo directory = new DirectoryInfo(pi);
                    var files = directory.GetFiles("*", SearchOption.AllDirectories);
                    var ImgList = files.Select(s => s.FullName).ToList();
                    if (ImgList.Count > 1)
                        return ImgList[new Random().Next(0, ImgList.Count - 1)];
                }
            }
            return pi;
        }

        public static void RunExe(string RunPath, string? source = null, bool autoClose = true, bool urlCheck = true)
        {
            new System.Threading.Thread(() =>
            {
                if (App.dflag)
                    LogtoFile($"[RUN]Path: {RunPath}, Source: {source ?? "Null"}");
                var path = Path_Prepare_EX(Path_Prepare(RunPath));
                try
                {
                    if (StartsWith(path, "base("))
                    {
                        path = path.Substring(5, path.Length - 6);
                        path = Encoding.Default.GetString(Convert.FromBase64String(path));
                    }
                    var parameter = LoadPaths(path, out path);
                    #region 预处理参数
                    for (var i = 0; i < parameter.Count; i++)
                    {
                        parameter[i] = SearchAny(parameter[i]);
                    }
                    #endregion
                    int disturb = int.Parse(Read_Ini(App.dConfig, "Config", "Disturb", "2"));
                    #region 识别文件类型
                    if (File.Exists(path))
                    {
                        if (HFile.isMediaFile(path))
                        {
                            if (App.dflag)
                                LogtoFile("[RUN]Media file detected");
                            path = $"play(\"{path}\")";
                            parameter = HiroCmdParse(path);
                        }
                        else if (HFile.isImageFile(path))
                        {
                            if (App.dflag)
                                LogtoFile("[RUN]Image file detected");
                            path = $"image(\"{path}\")";
                            parameter = HiroCmdParse(path);
                        }
                        else if (HFile.isTextFile(path))
                        {
                            if (App.dflag)
                                LogtoFile("[RUN]Text file detected");
                            path = $"text(\"{path}\")";
                            parameter = HiroCmdParse(path);
                        }
                        else
                        {
                            if (path.ToLower().EndsWith(".hiro"))
                            {
                                LogtoFile("[RUN]Hiro file detected");
                                path = $"seq(\"{path}\")";
                                parameter = HiroCmdParse(path);
                            }
                            else if (path.ToLower().EndsWith(".hef"))
                            {
                                LogtoFile("[RUN]Encrypted file detected");
                                path = $"decrypt(\"{path}\")";
                                parameter = HiroCmdParse(path);
                            }
                        }
                    }
                    #endregion
                    #region 不会造成打扰的命令
                    if (path.ToLower().Equals("memory()"))
                    {
                        GC.Collect();
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("priority("))
                    {
                        HWin.SetProcessPriority(parameter[0], Get_Translate("prority"));
                        goto RunOK;
                    }
                    if (path.ToLower().StartsWith("effiency("))
                    {
                        HWin.SetEffiencyMode(parameter[0], Get_Translate("effiency"));
                        goto RunOK;
                    }
                    #region 调试
                    if (HText.StartsWith(path, "debug("))
                    {
                        source = Get_Translate("debug");
                        if (!HText.StartsWith(path, "debug()"))
                        {
                            RunExe($"notify({path},2)", source);
                        }
                        else
                        {
                            App.dflag = !App.dflag;
                            if (App.dflag)
                                RunExe($"notify({Get_Translate("debugon")},2)", source);
                            else
                                RunExe($"notify({Get_Translate("debugoff")},2)", source);
                        }
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "hotkey()"))
                    {
                        if (App.dflag)
                            foreach (var hk in HHotKeys.hotkeys)
                            {
                                LogtoFile($"[Hotkeys] {hk.KeyID} => {hk.ItemID}");
                            }
                        goto RunOK;
                    }
                    #endregion
                    if (HText.StartsWith(path, "save("))
                    {
                        source = Get_Translate("download");
                        HNet.Save(source, parameter);
                        goto RunOK;
                    }
                    #region 壁纸相关
                    if (HText.StartsWith(path, "bingw("))
                    {
                        HNet.BingWp(path, parameter);
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "wallpaper("))
                    {
                        HDesktop.Set_Wallpaper(parameter);
                        goto RunOK;
                    }
                    #endregion
                    #region 文件操作
                    if (HText.StartsWith(path, "delete("))
                    {
                        HFile.DeleteFile(path, parameter);
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "move("))
                    {
                        HFile.MoveFile(path, parameter);
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "copy("))
                    {
                        HFile.CopyFile(path, parameter);
                        goto RunOK;
                    }
                    #endregion
                    #region 系统环境
                    if (HText.StartsWith(path, "vol("))
                    {
                        switch (path.ToLower())
                        {
                            case "vol(up)":
                                keybd_event(0xAF, MapVirtualKey(0xAF, 0), 0x0001, 0);
                                keybd_event(0xAF, MapVirtualKey(0xAF, 0), 0x0001 | 0x0002, 0);
                                break;
                            case "vol(down)":
                                keybd_event(0xAE, MapVirtualKey(0xAE, 0), 0x0001, 0);
                                keybd_event(0xAE, MapVirtualKey(0xAE, 0), 0x0001 | 0x0002, 0);
                                break;
                            case "vol(mute)":
                                keybd_event(0xAD, MapVirtualKey(0xAD, 0), 0x0001, 0);
                                keybd_event(0xAD, MapVirtualKey(0xAD, 0), 0x0001 | 0x0002, 0);
                                break;
                            default:
                                break;
                        }
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "bluetooth("))
                    {
                        bool? situation = (parameter.Count > 0 ? parameter[0].ToLower() : "") switch
                        {
                            "0" or "off" => false,
                            "1" or "on" => true,
                            _ => null,
                        };
                        SetBthState(situation, parameter.Count > 1 ? parameter[1] : null);
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "wifi("))
                    {
                        int situation = path.ToLower() switch
                        {
                            "wifi(0)" or "wifi(off)" => 0,
                            "wifi(1)" or "wifi(on)" => 1,
                            "wifi(2)" or "wifi(dis)" or "wifi(disconnect)" => 2,
                            "wifi(3)" or "wifi(con)" or "wifi(connect)" => 3,
                            _ => -1,
                        };
                        if (situation == -1)
                        {
                            if (parameter.Count > 1 && parameter[1].ToLower().IndexOf("o") != -1)
                            {
                                SetWiFiState(3, parameter[0], true);
                            }
                            else
                                SetWiFiState(3, parameter[0]);

                        }
                        else
                            SetWiFiState(situation);
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "media("))
                    {
                        System.Windows.Input.Key situation = path.ToLower() switch
                        {
                            "media(0)" or "media(off)" or "media(down)" or "media(↓)" or "media(stop)" or "media(end)" => System.Windows.Input.Key.MediaStop,
                            "media(1)" or "media(on)" or "media(up)" or "media(↑)" or "media(start)" or "media(begin)" or "media(invoke)" => System.Windows.Input.Key.MediaPlayPause,
                            "media(next)" or "media(2)" or "media(right)" or "media(→)" => System.Windows.Input.Key.MediaNextTrack,
                            "media(previous)" or "media(last)" or "media(3)" or "media(left)" or "media(←)" => System.Windows.Input.Key.MediaPreviousTrack,
                            _ => System.Windows.Input.Key.MediaStop,
                        };
                        var keyinfo = situation switch
                        {
                            System.Windows.Input.Key.MediaStop => "End",
                            System.Windows.Input.Key.MediaPlayPause => "Start",
                            System.Windows.Input.Key.MediaNextTrack => "Next",
                            System.Windows.Input.Key.MediaPreviousTrack => "Previous"
                        };
                        var keyi = (byte)System.Windows.Input.KeyInterop.VirtualKeyFromKey(situation);
                        if (App.dflag)
                            LogtoFile("[MEDIA]Media Control : " + keyinfo);
                        keybd_event(keyi, MapVirtualKey(keyi, 0), 0x0001, 0);
                        keybd_event(keyi, MapVirtualKey(keyi, 0), 0x0001 | 0x0002, 0);
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "key("))
                    {
                        List<byte> modi = new();
                        int pathi = int.Parse(parameter[0]);
                        if (pathi >= 8)
                        {
                            while (pathi >= 8)
                            {
                                pathi -= 8;
                            }
                            modi.Add(0x5B);//Windows
                        }
                        if (pathi >= 4)
                        {
                            while (pathi >= 4)
                            {
                                pathi -= 4;
                            }
                            modi.Add(0x10);
                        }
                        if (pathi >= 2)
                        {
                            while (pathi >= 2)
                            {
                                pathi -= 2;
                            }
                            modi.Add(0x11);
                        }
                        if (pathi >= 1)
                            modi.Add(0x12);
                        byte parai = (byte)System.Windows.Input.KeyInterop.VirtualKeyFromKey((System.Windows.Input.Key)int.Parse(parameter[1]));
                        for (int i = 0; i < modi.Count; i++)
                        {
                            keybd_event(modi[i], MapVirtualKey(modi[i], 0), 0x0001, 0);
                        }
                        keybd_event(parai, MapVirtualKey(parai, 0), 0x0001, 0);
                        keybd_event(parai, MapVirtualKey(parai, 0), 0x0001 | 0x0002, 0);
                        for (int i = modi.Count - 1; i >= 0; i--)
                        {
                            keybd_event(modi[i], MapVirtualKey(modi[i], 0), 0x0001 | 0x0002, 0);
                        }
                        goto RunOK;
                    }
                    #endregion
                    if (HText.StartsWith(path, "ini("))
                    {
                        Write_Ini(parameter[0], parameter[1], parameter[2], parameter[3]);
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "zip("))
                    {
                        HFile.Zip(source, parameter);
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "unzip("))
                    {
                        HFile.Unzip(source, parameter);
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "exit()"))
                    {
                        try
                        {
                            HiroInvoke(() =>
                            {
                                Environment.Exit(Environment.ExitCode);
                                Application.Current.Shutdown();
                            });
                        }
                        catch (Exception ex)
                        {
                            LogError(ex, $"Hiro.Exception.Exit{Environment.NewLine}Path: {path}");
                        }
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "hide()"))
                    {
                        HiroInvoke(() =>
                        {
                            if (App.mn != null)
                            {
                                App.mn.titlelabel.Visibility = Visibility.Hidden;
                                App.mn.versionlabel.Visibility = Visibility.Hidden;
                                App.mn.minbtn.Visibility = Visibility.Hidden;
                                App.mn.closebtn.Visibility = Visibility.Hidden;
                                App.mn.stack.Visibility = Visibility.Hidden;
                                App.mn.Visibility = Visibility.Hidden;
                            }
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "switch()"))
                    {
                        HiroInvoke(() =>
                        {
                            if (App.mn != null)
                            {
                                App.mn.HideAll();
                            }
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "menu()"))
                    {
                        HiroInvoke(() =>
                        {
                            if (App.wnd != null && App.wnd.cm != null)
                            {
                                App.Load_Menu();
                                App.wnd.cm.IsOpen = true;
                            }
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "show()"))
                    {
                        HiroInvoke(() =>
                        {
                            if (App.mn != null)
                            {
                                App.mn.Show();
                                App.mn.Visibility = Visibility.Visible;
                                App.mn.HiHiro();
                            }
                        });
                        goto RunOK;
                    }
                    if (path.ToLower() == "hello" || path.ToLower() == "hello()")
                    {
                        var hr = DateTime.Now.Hour;
                        var morning = Read_Ini(App.langFilePath, "local", "morning", "[6,7,8,9,10]");
                        var noon = Read_Ini(App.langFilePath, "local", "noon", "[11,12,13]");
                        var afternoon = Read_Ini(App.langFilePath, "local", "afternoon", "[14,15,16,17,18]");
                        var evening = Read_Ini(App.langFilePath, "local", "evening", "[19,20,21,22]");
                        var night = Read_Ini(App.langFilePath, "local", "night", "[23,0,1,2,3,4,5]");
                        morning = $",{morning.Replace("[", "").Replace("]", "").Replace(" ", "")},";
                        noon = $",{noon.Replace("[", "").Replace("]", "").Replace(" ", "")},";
                        afternoon = $",{afternoon.Replace("[", "").Replace("]", "").Replace(" ", "")},";
                        evening = $",{evening.Replace("[", "").Replace("]", "").Replace(" ", "")},";
                        night = $",{night.Replace("[", "").Replace("]", "").Replace(" ", "")},";
                        var trstrs = new string[] { "morning", "morningcus", "noon", "nooncus", "afternoon", "afternooncus", "evening", "eveningcus", "night", "nightcus" };
                        int trindex;
                        if (morning.IndexOf("," + hr + ",") != -1)
                            trindex = 0;
                        else if (noon.IndexOf($",{hr},") != -1)
                            trindex = 1;
                        else if (afternoon.IndexOf($",{hr},") != -1)
                            trindex = 2;
                        else if (evening.IndexOf($",{hr},") != -1)
                            trindex = 3;
                        else
                            trindex = 4;
                        if (App.CustomUsernameFlag == 0)
                            App.Notify(new Hiro_Notice(Get_Translate(trstrs[trindex * 2]).Replace("%u", App.eUserName), 2, Get_Translate("hello")));
                        else
                            App.Notify(new Hiro_Notice(Get_Translate(trstrs[trindex * 2 + 1]).Replace("%u", App.username), 2, Get_Translate("hello")));
                        goto RunOK;
                    }
                    //sequence(uri)
                    if (HText.StartsWith(path, "seq("))
                    {

                        var ca = parameter.Count < 2 || (!parameter[1].ToLower().Equals("hide") && !parameter[1].ToLower().Equals("h"));
                        var cb = (disturb == 1 && IsForegroundFullScreen()) || disturb == 0;
                        HiroInvoke(() =>
                        {
                            Hiro_Sequence sq = new();
                            if (ca && !cb)
                                sq.Show();
                            sq.SeqExe(parameter[0]);
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "item(") && !HText.StartsWith(path, "item()"))
                    {
                        var RealPath = parameter[0];
                        for (int i = 1; i < parameter.Count; i++)
                        {
                            RealPath += ($",{parameter[i]}");
                        }
                        HiroInvoke(() =>
                        {
                            foreach (var cmd in App.cmditems)
                            {
                                if (cmd.Name.Equals(RunPath) || cmd.Name.Equals(RealPath) || cmd.Name.Equals(path))
                                {
                                    RunExe(cmd.Command);
                                    break;
                                }
                            }
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "run("))
                    {
                        if (parameter.Count == 0)
                        {
                            App.Notify(new Hiro_Notice(Get_Translate("syntax"), 2, Get_Translate("execute")));
                            goto RunOK;
                        }
                        string? FileName = parameter.Count >= 1 ? parameter[0] : null;
                        string? Arguments = parameter.Count >= 3 ? parameter[2] : null;
                        string? HiroArguments = parameter.Count >= 2 ? parameter[1] : null;
                        HiroArguments = HiroArguments == null ? null : HiroArguments.ToLower();
                        if (FileName == null)
                        {
                            App.Notify(new Hiro_Notice(Get_Translate("syntax"), 2, Get_Translate("execute")));
                            goto RunOK;
                        }
                        try
                        {
                            ProcessStartInfo pinfo = new()
                            {
                                UseShellExecute = true,
                                FileName = FileName,
                                Arguments = Arguments
                            };
                            if (HiroArguments != null)
                            {
                                if (HiroArguments.IndexOf("a") != -1)
                                    pinfo.Verb = "runas";
                                if (HiroArguments.IndexOf("h") != -1)
                                    pinfo.WindowStyle = ProcessWindowStyle.Hidden;
                                if (HiroArguments.IndexOf("i") != -1)
                                    pinfo.WindowStyle = ProcessWindowStyle.Minimized;
                                if (HiroArguments.IndexOf("x") != -1)
                                    pinfo.WindowStyle = ProcessWindowStyle.Maximized;
                                if (HiroArguments.IndexOf("n") != -1)
                                    pinfo.CreateNoWindow = true;
                            }
                            Run_Process(pinfo, path, RunPath);
                        }
                        catch (Exception ex)
                        {
                            RunExe($"alarm({Get_Translate("error")},{ex})");
                            LogError(ex, $"Hiro.Exception.Run.Extra{Environment.NewLine}Path: {path}");
                        }
                        if (App.mn == null)
                            RunExe("exit()");
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "lock()"))
                    {
                        HiroInvoke(() =>
                        {
                            App.Locked = true;
                            if (App.mn == null)
                                return;
                            App.mn.Set_Label(App.mn.homex);
                            App.mn.versionlabel.Content = Hiro_Resources.ApplicationVersion + " 🔒";
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "weather("))
                    {
                        source = Get_Translate("weather");
                        path = path.ToLower() switch
                        {
                            "weather(0)" => $"alarm({Get_Translate("weather")},https://api.rexio.cn/v1/rex.php?r=weather&k=6725dccca57b2998e8fc47cee2a8f72f&lang={App.lang})",
                            "weather(1)" => $"notify(https://api.rexio.cn/v1/rex.php?r=weather&k=6725dccca57b2998e8fc47cee2a8f72f&lang={App.lang},2)",
                            _ => "notify(" + Get_Translate("syntax") + ",2)"
                        };
                        RunExe(path, source);
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "sync()"))
                    {
                        if (App.Logined == true)
                        {
                            if (HID.SyncProfile(App.loginedUser, App.loginedToken).Equals("success"))
                            {
                                HiroInvoke(() =>
                                {
                                    if (App.mn != null)
                                    {
                                        App.mn.hiro_profile?.UpdateProfile();
                                    }
                                    App.Notify(new(HText.Get_Translate("syncsucc"), 2, HText.Get_Translate("sync")));
                                });
                            }
                            else
                            {
                                HiroInvoke(() =>
                                {
                                    App.Notify(new(HText.Get_Translate("syncfailed"), 2, HText.Get_Translate("sync")));
                                });
                            }
                        }
                        else
                        {
                            HiroInvoke(() =>
                            {
                                App.Notify(new(HText.Get_Translate("synclogin"), 2, HText.Get_Translate("sync")));
                                App.mn?.Set_Label(App.mn.loginx);
                            });
                        }
                        goto RunOK;
                    }
                    if (path.ToLower() == "nop" || path.ToLower() == "nop()") goto RunOK;
                    #endregion
                    if ((disturb == 1 && IsForegroundFullScreen()) || disturb == 0)
                    {
                        App.mn?.AddToInfoCenter($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ")}{Environment.NewLine}\t{Get_Translate("infocmd")}:\t{RunPath}{Environment.NewLine}\t{Get_Translate("infosource")}:\t{source} {Environment.NewLine} ");
                        goto RunOK;
                    }
                    #region 可能造成打扰的命令
                    if (path.ToLower().Equals("hirotest()"))
                    {
                        HiroInvoke(() =>
                        {
                            new Hiro_Timer().Show();
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "badge("))
                    {
                        HSystem.ShowBadge(parameter);
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "invoke("))
                    {
                        if (parameter.Count > 0)
                        {
                            var pa = parameter[0];
                            if (HText.StartsWith(pa, "http://") || HText.StartsWith(pa, "https://"))
                            {
                                pa = HNet.GetWebContent(pa).Replace("\\n", string.Empty).Replace("<br>", string.Empty);
                                if (App.dflag)
                                    LogtoFile("[INVOKE]" + pa);
                            }
                            RunExe(pa, source, autoClose);
                        }
                        goto RunOK;
                    }
                    if (path.ToLower().Equals("decrypt"))
                    {
                        HiroInvoke(() =>
                        {
                            new Hiro_Encrypter(1).Show();
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "decrypt("))
                    {
                        HiroInvoke(() =>
                        {
                            string? p = null;
                            string? k = null;
                            if (parameter.Count > 0)
                            {
                                p = parameter[0];
                            }
                            if (parameter.Count > 1)
                            {
                                k = parameter[1];
                            }
                            var he = new Hiro_Encrypter(1, p, k);
                            if (parameter.Count > 2)
                            {
                                var pa = parameter[2].Trim().ToLower();
                                if (pa.Equals("r") || pa.Equals("run") || pa.Equals("1"))
                                    he.Autorun.IsChecked = true;
                                if (pa.Equals("l") || pa.Equals("locate") || pa.Equals("2"))
                                    he.Autorun.IsChecked = null;
                            }
                            if (parameter.Count > 3)
                            {
                                var pa = parameter[3].Trim().ToLower();
                                if (pa.Equals("d") || pa.Equals("delete") || pa.Equals("1"))
                                    he.Autodelete.IsChecked = true;
                            };
                            he.Show();
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "decrypto("))
                    {
                        HiroInvoke(() =>
                        {
                            string? p = null;
                            string? k = null;
                            if (parameter.Count > 1)
                            {
                                p = parameter[0];
                                k = parameter[1];
                                var he = new Hiro_Encrypter(1, p, k, true);
                                if (parameter.Count > 2)
                                {
                                    var pa = parameter[2].Trim().ToLower();
                                    if (pa.Equals("r") || pa.Equals("run") || pa.Equals("1"))
                                        he.Autorun.IsChecked = true;
                                    if (pa.Equals("l") || pa.Equals("locate") || pa.Equals("2"))
                                        he.Autorun.IsChecked = null;
                                }
                                if (parameter.Count > 3)
                                {
                                    var pa = parameter[3].Trim().ToLower();
                                    if (pa.Equals("d") || pa.Equals("delete") || pa.Equals("1"))
                                        he.Autodelete.IsChecked = true;
                                }
                                he.GoStart();
                            }
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().Equals("encrypt"))
                    {
                        HiroInvoke(() =>
                        {
                            new Hiro_Encrypter(0).Show();
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "encrypt("))
                    {
                        HiroInvoke(() =>
                        {
                            string? p = null;
                            string? k = null;
                            if (parameter.Count > 0)
                            {
                                p = parameter[0];
                            }
                            if (parameter.Count > 1)
                            {
                                k = parameter[1];
                            }
                            var he = new Hiro_Encrypter(0, p, k);
                            if (parameter.Count > 2)
                            {
                                var pa = parameter[2].Trim().ToLower();
                                if (pa.Equals("r") || pa.Equals("run") || pa.Equals("1"))
                                    he.Autorun.IsChecked = true;
                                if (pa.Equals("l") || pa.Equals("locate") || pa.Equals("2"))
                                    he.Autorun.IsChecked = null;
                            }
                            if (parameter.Count > 3)
                            {
                                var pa = parameter[3].Trim().ToLower();
                                if (pa.Equals("d") || pa.Equals("delete") || pa.Equals("1"))
                                    he.Autodelete.IsChecked = true;
                            };
                            he.Show();
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "encrypto("))
                    {
                        HiroInvoke(() =>
                        {
                            string? p = null;
                            string? k = null;
                            if (parameter.Count > 1)
                            {
                                p = parameter[0];
                                k = parameter[1];
                                var he = new Hiro_Encrypter(1, p, k, true);
                                if (parameter.Count > 2)
                                {
                                    var pa = parameter[2].Trim().ToLower();
                                    if (pa.Equals("r") || pa.Equals("run") || pa.Equals("1"))
                                        he.Autorun.IsChecked = true;
                                    if (pa.Equals("l") || pa.Equals("locate") || pa.Equals("2"))
                                        he.Autorun.IsChecked = null;
                                }
                                if (parameter.Count > 3)
                                {
                                    var pa = parameter[3].Trim().ToLower();
                                    if (pa.Equals("d") || pa.Equals("delete") || pa.Equals("1"))
                                        he.Autodelete.IsChecked = true;
                                }
                                he.GoStart();
                            }
                        });
                        goto RunOK;
                    }
                    if (path.ToLower().Equals("island") || path.ToLower().Equals("island()"))
                    {
                        HiroInvoke(() =>
                        {
                            new Hiro_Island().Show();
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "ticker("))
                    {
                        var adflag = false;
                        if (parameter.Count < 1)
                            goto RunOK;
                        HiroInvoke(() =>
                        {
                            Hiro_Ticker? ht = null;
                            foreach (var win in Application.Current.Windows)
                            {
                                if (win is Hiro_Ticker hwin)
                                {
                                    if (hwin.id.Equals(parameter[0]))
                                    {
                                        ht = hwin;
                                        adflag = true;
                                    }
                                }
                            }
                            switch (parameter.Count)
                            {
                                case 1:
                                    ht ??= new(parameter[0]);
                                    break;
                                case 2:
                                    ht ??= new(parameter[0]);
                                    ht.format = parameter[1];
                                    ht.RefreshContent();
                                    break;
                                case 3:
                                    var p3 = int.TryParse(parameter[2], out int pa3) ? pa3 : 0;
                                    ht ??= new(parameter[0]);
                                    ht.format = parameter[1];
                                    if (adflag)
                                        ht.OffsetNum(p3);
                                    else
                                        ht.current = p3;
                                    ht.RefreshContent();
                                    break;
                                case 4:
                                    var p4 = int.TryParse(parameter[2], out int pa4) ? pa4 : 0;
                                    var p4a = int.TryParse(parameter[3], out int pa4a) ? pa4a : int.MaxValue;
                                    ht ??= new(parameter[0]);
                                    ht.format = parameter[1];
                                    if (adflag)
                                        ht.OffsetNum(p4a);
                                    else
                                        ht.current = p4;
                                    ht.RefreshContent();
                                    break;
                                case 5:
                                    var p5 = int.TryParse(parameter[2], out int pa5) ? pa5 : 0;
                                    var p5a = int.TryParse(parameter[3], out int pa5a) ? pa5a : int.MaxValue;
                                    var p5b = int.TryParse(parameter[4], out int pa5b) ? pa5b : int.MinValue;
                                    ht ??= new(parameter[0]);
                                    ht.format = parameter[1];
                                    if (adflag)
                                        ht.OffsetNum(p5a);
                                    else
                                        ht.current = p5;
                                    ht.min = p5b;
                                    ht.RefreshContent();
                                    break;
                                case > 5:
                                    var px = int.TryParse(parameter[2], out int pax) ? pax : 0;
                                    var pxa = int.TryParse(parameter[3], out int paxa) ? paxa : int.MaxValue;
                                    var pxb = int.TryParse(parameter[4], out int paxb) ? paxb : int.MinValue;
                                    var pxc = int.TryParse(parameter[5], out int paxc) ? paxc : int.MaxValue;
                                    ht ??= new(parameter[0]);
                                    ht.format = parameter[1];
                                    if (adflag)
                                        ht.OffsetNum(pxa);
                                    else
                                        ht.current = px;
                                    ht.min = pxb;
                                    ht.max = pxc;
                                    ht.RefreshContent();
                                    break;
                            }
                            ht?.Show();
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "hiroad("))
                    {
                        source = Get_Translate("update");
                        HiroInvoke(() =>
                        {
                            Hiro_Download dl = new(1, parameter[2]);
                            dl.textBoxHttpUrl.Text = parameter[0];
                            dl.SavePath.Text = parameter[1];
                            dl.Autorun.IsChecked = true;
                            dl.Autorun.IsEnabled = false;
                            dl.rurl = parameter[0];
                            dl.rpath = parameter[1];
                            dl.Show();
                            dl.StartDownload();
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "download("))
                    {
                        source = Get_Translate("download");
                        HiroInvoke(() =>
                        {
                            Hiro_Download dl = new(0, "");
                            if (parameter.Count > 0)
                                dl.textBoxHttpUrl.Text = parameter[0];
                            if (parameter.Count > 1)
                                dl.SavePath.Text = parameter[1];
                            dl.Show();
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "alarm("))
                    {
                        var pa = parameter[0];
                        var os = Get_OSVersion();
                        if (os.IndexOf(".") != -1)
                            os = os[..os.IndexOf(".")];
                        var boo = Read_Ini(App.dConfig, "Config", "Toast", "0").Equals("1") && int.TryParse(os, out int a) && a >= 10;
                        if (boo)
                        {
                            if (HText.StartsWith(pa, "http://") || HText.StartsWith(pa, "https://"))
                            {
                                pa = HNet.GetWebContent(pa);
                            }
                            if (parameter.Count > 1)
                            {
                                var par = parameter[1];

                                if ((HText.StartsWith(par, "http://") || HText.StartsWith(par, "https://")) && boo)
                                {
                                    par = HNet.GetWebContent(par).Replace("\\n", Environment.NewLine).Replace("<br>", Environment.NewLine);
                                }
                            }
                            if (parameter.Count > 1)
                            {
                                HiroInvoke(() =>
                                {
                                    new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                                .AddArgument("Launch", App.appTitle)
                                .AddText(parameter[0])
                                .AddText(parameter[1].Replace("\\n", Environment.NewLine).Replace("<br>", Environment.NewLine))
                                .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                            .SetContent(Get_Translate("alarmone")))
                                .Show();
                                });

                            }
                            else
                            {
                                HiroInvoke(() =>
                                {
                                    new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                                .AddText(Get_Translate("alarmtitle"))
                                .AddText(parameter[0].Replace("\\n", Environment.NewLine).Replace("<br>", Environment.NewLine))
                                .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                            .SetContent(Get_Translate("alarmone")))
                                .Show();
                                });
                            }
                        }
                        else
                        {
                            HiroInvoke(() =>
                            {
                                if (parameter.Count > 1)
                                    new Hiro_Alarm(-1, CustomedTitle: parameter[0], CustomedContnet: parameter[1], OneButtonOnly: 1).Show();
                                else
                                    new Hiro_Alarm(-1, CustomedContnet: parameter[0], OneButtonOnly: 1).Show();
                            });
                        }
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "splash("))
                    {
                        var pa = parameter[0];
                        HiroInvoke(() =>
                        {
                            if (parameter.Count > 1)
                            {
                                var tick = 0;
                                if (!int.TryParse(parameter.Count >= 5 ? parameter[4] : "-1", out tick))
                                    tick = -1;
                                new Hiro_Splash(parameter[0], parameter[1],
                                    parameter.Count >= 3 ? parameter[2] : HText.Get_Translate("spLoading"),
                                    parameter.Count >= 4 ? parameter[3] : "",
                                    tick,
                                    parameter.Count >= 6 ? parameter[5].Equals("true", StringComparison.CurrentCultureIgnoreCase) : false,
                                    parameter.Count >= 7 ? parameter[6].Equals("true", StringComparison.CurrentCultureIgnoreCase) : false,
                                    parameter.Count >= 8 ? parameter[7].Equals("true", StringComparison.CurrentCultureIgnoreCase) : true,
                                    parameter.Count >= 9 ? parameter[8] : "").Show();
                            }
                            else if (parameter.Count == 1)
                                new Hiro_Splash(parameter[0]).Show();
                            else
                                new Hiro_Splash().Show();
                        });
                        goto RunOK;
                    }
                    if (App.mn != null)
                    {
                        if (HText.StartsWith(path, "home()"))
                        {
                            HiroInvoke(() =>
                            {
                                if (App.mn.Visibility != Visibility.Visible)
                                    RunExe("show()");
                                App.mn.Set_Label(App.mn.homex);
                            });
                            goto RunOK;
                        }
                        if (HText.StartsWith(path, "item()"))
                        {
                            HiroInvoke(() =>
                            {
                                if (App.mn.Visibility != Visibility.Visible)
                                    RunExe("show()");
                                App.mn.Set_Label(App.mn.itemx);
                            });
                            goto RunOK;
                        }
                        if (HText.StartsWith(path, "schedule()"))
                        {
                            HiroInvoke(() =>
                            {
                                if (App.mn.Visibility != Visibility.Visible)
                                    RunExe("show()");
                                App.mn.Set_Label(App.mn.schedulex);
                            });
                            goto RunOK;
                        }
                        if (HText.StartsWith(path, "config()"))
                        {
                            HiroInvoke(() =>
                            {
                                if (App.mn.Visibility != Visibility.Visible)
                                    RunExe("show()");
                                App.mn.Set_Label(App.mn.configx);
                            });
                            goto RunOK;
                        }
                        if (HText.StartsWith(path, "chat()"))
                        {
                            HiroInvoke(() =>
                            {
                                if (App.mn.Visibility != Visibility.Visible)
                                    RunExe("show()");
                                App.mn.Set_Label(App.mn.chatx);
                            });
                            goto RunOK;
                        }
                        if (HText.StartsWith(path, "me()"))
                        {
                            HiroInvoke(() =>
                            {
                                if (App.mn.Visibility != Visibility.Visible)
                                    RunExe("show()");
                                App.mn.Set_Label(App.mn.profilex);
                            });
                            goto RunOK;
                        }
                        if (HText.StartsWith(path, "about()"))
                        {
                            HiroInvoke(() =>
                            {
                                if (App.mn.Visibility != Visibility.Visible)
                                    RunExe("show()");
                                App.mn.Set_Label(App.mn.aboutx);
                            });
                            goto RunOK;
                        }
                    }
                    if (HText.StartsWith(path, "restart("))
                    {
                        if (App.mn == null)
                        {
                            HiroInvoke(() =>
                            {
                                App.mn = new();
                                App.mn.Show();
                            });

                            goto RunOK;
                        }
                        int situation = path.ToLower() switch
                        {
                            "restart(0)" => 0,
                            _ => 1
                        };
                        HiroInvoke(() =>
                        {
                            if (situation == 0)
                            {
                                App.mn.MainUI_Initialize();
                                App.mn.HiHiro();

                            }
                            if (situation == 1)
                            {
                                App.mn.Close();
                                App.mn = new();
                                App.mn.Show();
                            }
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "message("))
                    {
                        HiroInvoke(() =>
                        {
                            Hiro_Background? bg = null;
                            if (Read_Ini(parameter[0], "Action", "Background", "true").ToLower().Equals("true"))
                                bg = new();
                            Hiro_Msg msg = new(parameter[0])
                            {
                                bg = bg,
                                Title = Get_Translate("msgTitle").Replace("%t", Path_PPX(Read_Ini(parameter[0], "Message", "Title", Get_Translate("syntax")))).Replace("%a", App.appTitle)
                            };
                            msg.backtitle.Content = Path_Prepare(Path_Prepare_EX(Path_Prepare_EX(Read_Ini(parameter[0], "Message", "Title", Get_Translate("syntax")))));
                            msg.acceptbtn.Content = Read_Ini(parameter[0], "Message", "accept", Get_Translate("msgaccept"));
                            msg.rejectbtn.Content = Read_Ini(parameter[0], "Message", "reject", Get_Translate("msgreject"));
                            msg.cancelbtn.Content = Read_Ini(parameter[0], "Message", "cancel", Get_Translate("msgcancel"));
                            parameter[0] = Path_Prepare_EX(Path_Prepare(Read_Ini(parameter[0], "Message", "content", Get_Translate("syntax"))));
                            if (parameter[0].ToLower().StartsWith("http://") || parameter[0].ToLower().StartsWith("https://"))
                            {
                                msg.sv.Content = Get_Translate("msgload");
                                BackgroundWorker bw = new();
                                bw.DoWork += delegate
                                {
                                    parameter[0] = HNet.GetWebContent(parameter[0]).Replace("<br>", "\\n");
                                };
                                bw.RunWorkerCompleted += delegate
                                {
                                    msg.sv.Content = parameter[0].Replace("\\n", Environment.NewLine);
                                };
                                bw.RunWorkerAsync();
                            }
                            else if (File.Exists(parameter[0]))
                                msg.sv.Content = Path_Prepare(Path_Prepare_EX(File.ReadAllText(parameter[0]))).Replace("\\n", Environment.NewLine);
                            else
                                msg.sv.Content = parameter[0].Replace("\\n", Environment.NewLine);
                            msg.Load_Position();
                            msg.Show();
                        });
                        goto RunOK;
                    }

                    if (path.Length > 7 && HText.StartsWith(path, "notify("))
                    {
                        string titile = Get_Translate("syntax");
                        int duration = -1;
                        Hiro_Icon? hicon = null;
                        if (parameter.Count > 0)
                        {
                            try
                            {
                                duration = parameter.Count > 1 ? Convert.ToInt32(parameter[1]) : 2;
                                titile = parameter[0];
                                if (titile.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) || titile.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    titile = HNet.GetWebContent(titile).Replace("<br>", "\\n");
                                }
                                Action? act = null;
                                if (parameter.Count > 3)
                                {
                                    var p3 = parameter[3];
                                    act = new(() =>
                                    {
                                        RunExe(p3, parameter[2], false);
                                    });
                                }
                                if (parameter.Count > 4)
                                {
                                    var p4 = parameter[4];
                                    hicon = new()
                                    {
                                        Location = p4
                                    };
                                }
                                if (File.Exists(titile))
                                    titile = File.ReadAllText(titile).Replace(Environment.NewLine, "\\n");
                                if (parameter.Count > 2)
                                    source = parameter[2];
                                duration = duration <= 0 ? 2 : duration;
                                App.Notify(new(titile, duration, source, act, hicon));
                            }
                            catch (Exception ex)
                            {
                                RunExe("alarm(" + Get_Translate("error") + "," + ex.ToString() + ")");
                                LogError(ex, $"Hiro.Exception.Run.Notification{Environment.NewLine}Path: {path}");
                            }
                        }
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "play("))
                    {
                        if (App.mn != null)
                        {
                            RunExe($"run(\"{Hiro_Resources.ApplicationPath}\",,\"base({Convert.ToBase64String(Encoding.Default.GetBytes(path))})\" utils)");
                        }
                        else
                        {
                            HiroInvoke(() =>
                            {
                                if (path.ToLower().Equals("play()"))
                                    new Hiro_Player().Show();
                                else
                                    new Hiro_Player(parameter[0]).Show();
                            });
                        }
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "web("))
                    {
                        if (Read_Ini(App.dConfig, "Config", "URLConfirm", "0").Equals("1") && urlCheck && App.mn == null)
                        {
                            var acbak = autoClose;
                            var confrimWin = Path_Prepare_EX(Path_Prepare("<capp>\\<lang>\\url.hms"));
                            autoClose = false;
                            HiroInvoke(() =>
                            {
                                Hiro_Background? bg = null;
                                if (Read_Ini(confrimWin, "Action", "Background", "true").ToLower().Equals("true"))
                                    bg = new();
                                Hiro_Msg msg = new(confrimWin)
                                {
                                    bg = bg,
                                    Title = Get_Translate("msgTitle").Replace("%t", Path_PPX(Read_Ini(parameter[0], "Message", "Title", Get_Translate("syntax")))).Replace("%a", App.appTitle)
                                };
                                msg.backtitle.Content = Path_Prepare(Path_Prepare_EX(Path_Prepare_EX(Read_Ini(confrimWin, "Message", "Title", Get_Translate("syntax")))));
                                msg.acceptbtn.Content = Read_Ini(confrimWin, "Message", "accept", Get_Translate("msgaccept"));
                                msg.rejectbtn.Content = Read_Ini(confrimWin, "Message", "reject", Get_Translate("msgreject"));
                                msg.cancelbtn.Content = Read_Ini(confrimWin, "Message", "cancel", Get_Translate("msgcancel"));
                                confrimWin = Path_Prepare_EX(Path_Prepare(Read_Ini(confrimWin, "Message", "content", Get_Translate("syntax"))));
                                if (confrimWin.ToLower().StartsWith("http://") || confrimWin.ToLower().StartsWith("https://"))
                                {
                                    msg.sv.Content = Get_Translate("msgload");
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
                                else if (File.Exists(confrimWin))
                                    msg.sv.Content = Path_Prepare(Path_Prepare_EX(File.ReadAllText(confrimWin))).Replace("\\n", Environment.NewLine);
                                else
                                    msg.sv.Content = confrimWin.Replace("\\n", Environment.NewLine);
                                msg.Load_Position();
                                msg.OKButtonPressed += delegate
                                {
                                    RunExe(path, source, acbak, false);
                                };
                                msg.CancelButtonPressed += delegate
                                {
                                    if (acbak && App.mn == null)
                                        RunExe("exit()");
                                };
                                msg.RejectButtonPressed += delegate
                                {
                                    if (acbak && App.mn == null)
                                        RunExe("exit()");
                                };
                                msg.Show();
                            });
                        }
                        else
                        {
                            if (App.mn != null)
                            {
                                RunExe($"run(\"{Hiro_Resources.ApplicationPath}\",,\"base({Convert.ToBase64String(Encoding.Default.GetBytes(path))})\" utils)");
                            }
                            else
                            {
                                HiroInvoke(() =>
                                {
                                    Hiro_Web web;
                                    string webpara = File.Exists(parameter[0]) && parameter[0].EndsWith(".hwb") ? Read_Ini(parameter[0], "Web", "Parameters", "") : parameter.Count > 1 ? parameter[1] : "";
                                    if (File.Exists(parameter[0]) && parameter[0].EndsWith(".hwb"))
                                    {
                                        string? title = null;
                                        if (!Read_Ini(parameter[0], "Web", "Title", string.Empty).Equals(string.Empty))
                                            title = Read_Ini(parameter[0], "Web", "Title", string.Empty).Replace("%b", " ");
                                        var uri = Read_Ini(parameter[0], "Web", "URI", "about:blank");
                                        var UPF = Read_Ini(parameter[0], "Web", "Folder", "<hiuser>");
                                        web = new(uri, title, UPF)
                                        {
                                            Height = Double.Parse(Read_Ini(parameter[0], "Web", "Height", "450")),
                                            Width = Double.Parse(Read_Ini(parameter[0], "Web", "Width", "800"))
                                        };
                                    }
                                    else
                                    {
                                        web = parameter.Count switch
                                        {
                                            1 or 2 => new(parameter[0]),
                                            > 2 => new(parameter[0], null, parameter[2]),
                                            _ => new("https://rex.as/"),
                                        };
                                    }
                                    if (webpara.IndexOf("s") != -1)
                                        web.self = true;
                                    if (webpara.IndexOf("-m") != -1)
                                    {
                                        web.maxbtn.Visibility = Visibility.Collapsed;
                                        web.ResizeMode = ResizeMode.CanMinimize;
                                    }
                                    if (webpara.IndexOf("-r") != -1)
                                    {
                                        web.maxbtn.Visibility = Visibility.Collapsed;
                                        web.minbtn.Visibility = Visibility.Collapsed;
                                        web.ResizeMode = ResizeMode.NoResize;
                                    }
                                    if (webpara.IndexOf("i") != -1)
                                        web.WindowState = WindowState.Minimized;
                                    else if (webpara.IndexOf("x") != -1)
                                        web.WindowState = WindowState.Maximized;
                                    if (webpara.IndexOf("-c") != -1)
                                        web.WindowStartupLocation = WindowStartupLocation.Manual;
                                    if (webpara.IndexOf("t") != -1)
                                    {
                                        web.Topmost = true;
                                        web.topbtn.Content = "\uE77A";
                                        web.topbtn.ToolTip = Get_Translate("webbottom");
                                    }
                                    if (webpara.IndexOf("b") != -1)
                                        web.URLGrid.Visibility = Visibility.Visible;
                                    web.Show();
                                    web.Refreash_Layout();
                                });
                            }
                        }


                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "image("))
                    {
                        HiroInvoke(() =>
                        {
                            new Hiro_ImageViewer(parameter[0]).Show();
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "text("))
                    {
                        HiroInvoke(() =>
                        {
                            new Hiro_TextEditor(parameter[0]).Show();
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "editor()"))
                    {
                        HiroInvoke(() =>
                        {
                            App.ed ??= new Hiro_Editor();
                            App.ed.Show();
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "lockscr("))
                    {
                        var location = parameter.Count > 0 && !parameter[0].Trim().Equals(string.Empty) ? parameter[0] : null;
                        HiroInvoke(() =>
                        {
                            App.ls ??= new(location);
                            App.ls.Show();
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "auth("))
                    {
                        Action sc = new(() =>
                        {
                            if (App.mn != null)
                            {
                                HiroInvoke(() =>
                                {
                                    App.mn.versionlabel.Content = Hiro_Resources.ApplicationVersion;
                                    App.Locked = false;
                                });
                            }
                            if (parameter.Count >= 1)
                            {
                                foreach (var pr in parameter)
                                {
                                    if (pr.Equals(string.Empty) || pr.Trim().Equals(string.Empty))
                                        continue;
                                    RunExe(pr, source);
                                }
                            }
                        });
                        Action fa = new(() =>
                        {
                            HiroInvoke(() =>
                            {
                                if (App.mn != null && App.Locked)
                                    App.mn.versionlabel.Content = Hiro_Resources.ApplicationVersion + " 🔒";
                            });
                        });
                        HiroInvoke(() =>
                        {
                            Register(sc, fa, fa);
                        });
                        goto RunOK;
                    }
                    if (HText.StartsWith(path, "hirowego()") || HText.StartsWith(path, "finder()") || HText.StartsWith(path, "start()"))
                    {
                        HiroInvoke(() =>
                        {
                            if (App.ls == null)
                            {
                                Hiro_Finder hf = new();
                                hf.Show();
                            }
                        });
                        goto RunOK;
                    }
                    if (Read_Ini(App.dConfig, "Config", "URLConfirm", "0").Equals("1") &&
                    (parameter[0].ToLower().StartsWith("https://") || parameter[0].ToLower().StartsWith("http://")
                    || parameter[0].ToLower().Equals("firefox")
                    || parameter[0].ToLower().Equals("chrome")
                    || parameter[0].ToLower().Equals("msedge")
                    || parameter[0].ToLower().Equals("iexplore")
                    || (parameter[0].ToLower().Equals("yandex") && (parameter[2].ToLower().StartsWith("https://") || parameter[2].ToLower().StartsWith("http://"))))
                    && urlCheck)
                    {
                        HSystem.ShowWebConfirmDialog(autoClose, path, source);
                        autoClose = false;
                        goto RunOK;
                    }
                    var parameter_ = LoadPaths(path, out path, false);
                    string? FileName_ = parameter_.Count >= 1 ? parameter_[0] : null;
                    if (FileName_ == null)
                    {
                        App.Notify(new Hiro_Notice(Get_Translate("syntax"), 2, Get_Translate("execute")));
                        goto RunOK;
                    }
                    ProcessStartInfo pinfo_ = new()
                    {
                        UseShellExecute = true,
                        FileName = FileName_,
                    };
                    if (App.dflag)
                        LogtoFile("[DEBUG]FileName " + FileName_);
                    if (parameter_.Count > 1)
                    {
                        pinfo_.Arguments = "\"" + parameter_[1] + "\"";
                        LogtoFile("[DEBUG]Argument " + parameter_[1]);
                        for (var j = 2; j < parameter_.Count; j++)
                        {
                            pinfo_.Arguments += " " + "\"" + parameter_[j] + "\"";
                            if (App.dflag)
                                LogtoFile("[DEBUG]Argument " + parameter_[j]);
                        }
                    }
                    Run_Process(pinfo_, path, RunPath);
                #endregion
                RunOK:
                    var nowin = true;
                    HiroInvoke(() =>
                    {
                        nowin = Application.Current.Windows.Count <= 0;
                    });
                    if (App.mn == null && autoClose && nowin)
                        RunExe("exit()");
                }
                catch (Exception ex)
                {
                    LogError(ex, $"Hiro.Exception.Run{Environment.NewLine}Path: {path}");
                    var nowin = true;
                    HiroInvoke(() =>
                    {
                        nowin = Application.Current.Windows.Count <= 0;
                    });
                    if (App.mn == null && autoClose && nowin)
                        RunExe("exit()");
                    else
                        App.Notify(new Hiro_Notice(Get_Translate("syntax"), 2, source));
                }
            }).Start();
        }

        private static void Run_Process(ProcessStartInfo pinfo, string path, string RunPath)
        {
            if (File.Exists(path))
            {
                pinfo.FileName = "explorer";
                pinfo.Arguments = "\"" + path + "\"";
                _ = Process.Start(pinfo);
                return;
            }
            pinfo.WorkingDirectory = Path_Prepare("<current>");
            try
            {
                _ = Process.Start(pinfo);
            }
            catch (Exception ex)
            {
                pinfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\users\\" + App.eUserName + "\\app";
                pinfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\users\\" + App.eUserName + "\\app\\" + pinfo.FileName;
                try
                {
                    _ = Process.Start(pinfo);
                }
                catch
                {
                    if (IsRegexPattern(RunPath))
                    {
                        foreach (var cmd in App.cmditems)
                        {
                            if (Regex.IsMatch(cmd.Name, RunPath))
                            {
                                RunExe(cmd.Command);
                                return;
                            }
                        }
                    }
                    if (IsRegexPattern(path))
                    {
                        foreach (var cmd in App.cmditems)
                        {
                            if (Regex.IsMatch(cmd.Name, path))
                            {
                                RunExe(cmd.Command);
                                return;
                            }
                        }
                    }
                    string? runstart = FindItemByName(RunPath.Replace("\"", ""), Path_Prepare("<cstart>"));
                    if (runstart != null)
                    {
                        pinfo.FileName = runstart;
                        pinfo.WorkingDirectory = Path_Prepare("<cstart>");
                        pinfo.Arguments = "";
                        try
                        {
                            _ = Process.Start(pinfo);
                        }
                        catch
                        {
                        }
                        return;
                    }
                    runstart = FindItemByName(RunPath.Replace("\"", ""), Path_Prepare("<istart>"));
                    if (runstart != null)
                    {
                        pinfo.FileName = runstart;
                        pinfo.WorkingDirectory = Path_Prepare("<istart>");
                        pinfo.Arguments = "";
                        try
                        {
                            _ = Process.Start(pinfo);
                            return;
                        }
                        catch
                        {
                        }
                    }
                    App.Notify(new(Get_Translate("notfound"), 2, Get_Translate("execute")));
                    LogError(ex, $"Hiro.Exception.Run.Process{Environment.NewLine}");
                }
            }
        }

        private static bool IsRegexPattern(string str)
        {
            bool result = true;
            try
            {
                Regex regex = new Regex(str);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public static string? FindItemByName(string Name, string Location)
        {
            if (!Directory.Exists(Location))
            {
                return null;
            }
            string[] filenames = Directory.GetFileSystemEntries(Location);
            foreach (string file in filenames)
            {
                try
                {
                    if (Directory.Exists(file))
                    {
                        string? filename = FindItemByName(Name, file);
                        if (filename != null)
                            return filename;
                    }
                    else
                    {
                        if (Path.GetFileName(file).Equals("desktop.ini"))
                            continue;
                        var filename = Path.GetFileNameWithoutExtension(file);
                        if (filename != null && IsRegexPattern(Name) && Regex.IsMatch(file, Name))
                            return file;
                    }
                }
                catch (Exception ex)
                {
                    LogError(ex, $"Hiro.Exception.FindItemByName");
                }
            }
            return null;
        }

        public static List<string> HiroParse(string val)
        {
            if (!val.Contains('(', StringComparison.CurrentCulture))
                return new List<string>();
            val = val[(val.IndexOf("(") + 1)..];
            if (val.EndsWith(")"))
                val = val[0..^1];
            return new List<string>(val.Split(','));
        }

        public static List<string> HiroCmdParse(string val, bool replaceBrackets = true)
        {
            if (val.IndexOf(")") != -1 && replaceBrackets)
            {
                val = val[(val.IndexOf("(") + 1)..];
                if (val.EndsWith(")"))
                    val = val[0..^1];
            }
            val = val.Replace("\\\\", "\uAAA3");
            val = val.Replace("\\\"", "\uAAA1");
            List<string> blank = new();
            var startIndex = val.IndexOf('\"');
            var a = 0;
            while (startIndex != -1)
            {
                var endIndex = val.IndexOf('\"', startIndex + 1);
                if (endIndex == -1)
                {
                    break;
                }
                else
                {
                    var left = startIndex == 0 ? "" : val[..startIndex];
                    var right = endIndex == val.Length ? "" : val[(endIndex + 1)..];
                    var tmp = val.Substring(startIndex + 1, endIndex - startIndex - 1);
                    val = left + "\uAAA5" + a.ToString() + "\uAAA6" + right;
                    blank.Add(tmp);
                    a++;
                    startIndex = val.IndexOf('\"');
                }
            }
            a--;
            var res = replaceBrackets ? new List<string>(val.Split(',')) : new List<string>(val.Split(' '));
            for (var r = res.Count - 1; r >= 0; r--)
            {
                if (a < 0)
                    break;
                res[r] = res[r].Replace("\uAAA1", "\"").Replace("\uAAA3", "\\");
                if (res[r].IndexOf("\uAAA5" + a.ToString() + "\uAAA6") != -1)
                {
                    res[r] = res[r].Replace("\uAAA5" + a.ToString() + "\uAAA6", blank[a]);
                    a--;
                }
            }
            return res;
        }

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, UInt32 dwFlags, UInt32 dwExtraInfo);

        [DllImport("user32.dll")]
        static extern Byte MapVirtualKey(UInt32 uCode, UInt32 uMapType);

        private async static void SetBthState(bool? bluetoothState, string? bluetoothMac)
        {
            try
            {
                var access = await Radio.RequestAccessAsync();
                if (access != RadioAccessStatus.Allowed)
                {
                    App.Notify(new Hiro_Notice(Get_Translate("bth") + Get_Translate("dcreject"), 2, Get_Translate("bth")));
                    return;
                }
                Windows.Devices.Bluetooth.BluetoothAdapter adapter = await Windows.Devices.Bluetooth.BluetoothAdapter.GetDefaultAsync();
                if (null != adapter)
                {
                    var btRadio = await adapter.GetRadioAsync();
                    switch (bluetoothState)
                    {
                        case true:
                            {
                                if (bluetoothMac == null)
                                {
                                    await btRadio.SetStateAsync(RadioState.On);
                                    App.Notify(new Hiro_Notice(Get_Translate("bth") + Get_Translate("dcon"), 2, Get_Translate("bth")));
                                }
                                else
                                {
                                    DeviceInformationCollection PairedBluetoothDevices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(true));
                                    foreach (var dev in PairedBluetoothDevices)
                                    {
                                        var idd = dev.Id.ToString().ToLower();
                                        var bdd = bluetoothMac.ToLower().Replace("Bluetooth#Bluetooth", "");
                                        if (dev.Name.ToLower().Contains(bdd) || idd.Equals(bdd.Replace(":", "")))
                                        {
                                            Hiro.APIs.ABluetooth.BluetoothConnection.ConnectToDevice(bdd);
                                        }
                                    }
                                }
                                break;
                            }
                        case false:
                            {
                                if (bluetoothMac == null)
                                {
                                    await btRadio.SetStateAsync(RadioState.Off);
                                    App.Notify(new Hiro_Notice(Get_Translate("bth") + Get_Translate("dcoff"), 2, Get_Translate("bth")));
                                }
                                else
                                {
                                    DeviceInformationCollection PairedBluetoothDevices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(true));
                                    foreach (var dev in PairedBluetoothDevices)
                                    {
                                        var idd = dev.Id.ToString().ToLower();
                                        var bdd = bluetoothMac.ToLower().Replace("Bluetooth#Bluetooth", "");
                                        if (dev.Name.ToLower().Contains(bdd) || idd.Equals(bdd.Replace(":", "")))
                                        {
                                            Hiro.APIs.ABluetooth.BluetoothDisconnection.DisconnectBluetoothDevice(bdd);
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            {
                                if (App.dflag)
                                {
                                    DeviceInformationCollection PairedBluetoothDevices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(true));
                                    foreach (var dev in PairedBluetoothDevices)
                                    {
                                        StringBuilder sb = new();
                                        foreach (var k in dev.Properties.Keys)
                                        {
                                            sb.Append(k?.ToString() ?? "Unknow Key").Append(":").Append(dev.Properties[k]?.ToString() ?? "Unknown Value").Append("|");
                                        }
                                        LogtoFile($"Device: {dev.Name}|{dev.Id}|{dev.Kind}|{dev.Pairing.CanPair}|{dev.Pairing.ProtectionLevel}|{dev.Pairing.IsPaired}|{{{sb?.ToString() ?? "\"No info\""}}}");
                                    }
                                }
                                App.Notify(new Hiro_Notice(Get_Translate("syntax"), 2, Get_Translate("bth")));
                                break;
                            }
                    }
                }
                else
                {
                    App.Notify(new Hiro_Notice(Get_Translate("bth") + Get_Translate("dcnull"), 2, Get_Translate("bth")));
                }

            }
            catch (Exception ex)
            {
                App.Notify(new Hiro_Notice(Get_Translate("error"), 2));
                LogError(ex, "Hiro.Exception.Bluetooth");
            }
        }

        private async static void SetWiFiState(int? WiFiState, string? Ssid = null, bool omit = false)
        {
            try
            {
                if (await WiFiAdapter.RequestAccessAsync() != WiFiAccessStatus.Allowed)
                {
                    App.Notify(new Hiro_Notice(Get_Translate("wifi") + Get_Translate("dcreject"), 2, Get_Translate("wifi")));
                    return;
                }
                var adapters = await WiFiAdapter.FindAllAdaptersAsync();
                if (adapters.Count <= 0)
                {
                    App.Notify(new Hiro_Notice(Get_Translate("wifi") + Get_Translate("dcnull"), 2, Get_Translate("wifi")));
                    return;
                }
                var adapter = adapters[0];
                if (null == adapter)
                {
                    App.Notify(new Hiro_Notice(Get_Translate("wifi") + Get_Translate("dcnull"), 2, Get_Translate("wifi")));
                    return;
                }
                Radio? ra = null;
                foreach (var radio in await Radio.GetRadiosAsync())
                {
                    if (radio.Kind == RadioKind.WiFi)
                    {
                        ra = radio;
                        break;
                    }
                }
                if (null == ra)
                {
                    App.Notify(new Hiro_Notice(Get_Translate("wifi") + Get_Translate("dcnull"), 2, Get_Translate("wifi")));
                    return;
                }
                switch (WiFiState)
                {
                    case 0:
                        await ra.SetStateAsync(RadioState.Off);
                        App.Notify(new Hiro_Notice(Get_Translate("wifi") + Get_Translate("dcoff"), 2, Get_Translate("wifi")));
                        break;
                    case 1:
                        await ra.SetStateAsync(RadioState.On);
                        App.Notify(new Hiro_Notice(Get_Translate("wifi") + Get_Translate("dcon"), 2, Get_Translate("wifi")));
                        await adapter.ScanAsync();
                        break;
                    case 2:
                        adapter.Disconnect();
                        App.Notify(new Hiro_Notice(Get_Translate("wifi") + Get_Translate("dcdiscon"), 2, Get_Translate("wifi")));
                        break;
                    case 3:
                        await adapter.ScanAsync();
                        if (adapter.NetworkReport.AvailableNetworks.Count > 0)
                        {
                            if (App.dflag)
                                LogtoFile($"adapter.NetworkReport.AvailableNetworks.Count {adapter.NetworkReport.AvailableNetworks.Count}");
                            var connect = true;
                            WiFiAvailableNetwork? savedan = null;
                            foreach (var an in adapter.NetworkReport.AvailableNetworks)
                            {
                                if (Ssid != null && an.Ssid.Equals(Ssid))
                                {
                                    if (savedan == null || !savedan.Ssid.Equals(Ssid))
                                    {
                                        if (App.dflag)
                                            LogtoFile($"Matched Wifi Detected {an.Ssid}");
                                        savedan = an;
                                        if (omit)
                                            break;
                                    }
                                    else
                                    {
                                        if (App.dflag)
                                            LogtoFile($"Multi Wifi Detected {an.Ssid}");
                                        connect = false;
                                        break;
                                    }
                                }
                                else if (an.SecuritySettings.NetworkAuthenticationType.ToString().ToLower().StartsWith("open") && savedan == null)
                                {
                                    if (App.dflag)
                                        LogtoFile($"Open Wifi Detected {an.Ssid}");
                                    savedan = an;
                                    break;
                                }
                            }
                            if (!connect)
                                App.Notify(new Hiro_Notice(Get_Translate("wifimis").Replace("%s", Ssid), 2, Get_Translate("wifi")));
                            else
                            {
                                if (savedan == null)
                                    App.Notify(new Hiro_Notice(Get_Translate("wifina").Replace("%s", Ssid), 2, Get_Translate("wifi")));
                                else
                                {
                                    await adapter.ConnectAsync(savedan, Windows.Devices.WiFi.WiFiReconnectionKind.Automatic);
                                    if (Ssid != null && !savedan.Ssid.ToLower().Equals(Ssid.ToLower()))
                                        App.Notify(new Hiro_Notice(Get_Translate("wifi") + Get_Translate("dcrecon").Replace("%s1", Ssid).Replace("%s2", savedan.Ssid), 2, Get_Translate("wifi")));
                                    else
                                        App.Notify(new Hiro_Notice(Get_Translate("wifi") + Get_Translate("dccon").Replace("%s", savedan.Ssid), 2, Get_Translate("wifi")));
                                }
                            }
                        }
                        else
                            App.Notify(new Hiro_Notice(Get_Translate("wifina"), 2, Get_Translate("wifi")));
                        break;
                    default:
                        App.Notify(new Hiro_Notice(Get_Translate("syntax"), 2, Get_Translate("wifi")));
                        break;
                }
            }
            catch (Exception ex)
            {
                App.Notify(new Hiro_Notice(Get_Translate("error"), 2, Get_Translate("wifi")));
                LogError(ex, $"Hiro.Exception.Wifi");
            }
        }

        #endregion

        #region Windows Hello
        private async static void NewUser(String AccountId, Action success, Action falied, Action cancel)
        {
            var keyCreationResult = await KeyCredentialManager.RequestCreateAsync(AccountId, KeyCredentialCreationOption.FailIfExists);
            if (keyCreationResult.Status == KeyCredentialStatus.CredentialAlreadyExists)
            {
                //label5.Text = "User Already Created.";
                UserConsentVerificationResult consentResult = await UserConsentVerifier.RequestVerificationAsync(AccountId);
                if (consentResult.Equals(UserConsentVerificationResult.Verified))
                {
                    success.Invoke();
                }
                else
                {
                    falied.Invoke();
                }
            }
            else if (keyCreationResult.Status == KeyCredentialStatus.Success)
            {

                var userKey = keyCreationResult.Credential;

                var keyAttestationResult = await userKey.GetAttestationAsync();
                /*KeyCredentialAttestationStatus keyAttestationRetryType = 0;*/

                if (keyAttestationResult.Status == KeyCredentialAttestationStatus.Success)
                {
                    success.Invoke();
                }
                else if (keyAttestationResult.Status == KeyCredentialAttestationStatus.TemporaryFailure)
                {
                    falied.Invoke();
                }
                else if (keyAttestationResult.Status == KeyCredentialAttestationStatus.NotSupported)
                {
                    success.Invoke();
                }
            }
            else if (keyCreationResult.Status == KeyCredentialStatus.UserCanceled ||
                keyCreationResult.Status == KeyCredentialStatus.UserPrefersPassword)
            {
                cancel.Invoke();
            }
        }
        private async void DeleteUser(string AccountId)
        {
            var openKeyResult = await KeyCredentialManager.OpenAsync(AccountId);
            if (openKeyResult.Status == KeyCredentialStatus.Success)
            {
                UserConsentVerificationResult consentResult = await UserConsentVerifier.RequestVerificationAsync(AccountId);
                if (consentResult.Equals(UserConsentVerificationResult.Verified))
                {
                    // continue
                    //label5.Text = "User Verified.\nUser Deleted.";
                    await KeyCredentialManager.DeleteAsync(AccountId);
                }
                else
                {
                    //label5.Text = "User Verifying falied.";
                }
                return;
            }
            if (openKeyResult.Status == KeyCredentialStatus.NotFound)
            {
                //label5.Text = AccountId + " doesn't exist!\n";
                return;
            }
            //label5.Text = "Error Occurred.";
            return;
        }
        public async static void Register(Action success, Action falied, Action cancel)
        {
            var os = Get_OSVersion();
            if (os.IndexOf(".") != -1)
                os = os[..os.IndexOf(".")];
            if (int.TryParse(os, out int a) && a >= 10)
            {
                var keyCredentialAvailable = await KeyCredentialManager.IsSupportedAsync();
                if (!keyCredentialAvailable)
                {
                    success.Invoke();
                    return;
                }
                NewUser("N+@" + App.eUserName, success, falied, cancel);
                //Auth(null, "aki-helper@" + textBox1.Text);
            }
            else
            {
                success.Invoke();
                return;
            }
        }
        #endregion

        #region 获取命令翻译
        public static string Get_CMD_Translation(string cmd)
        {
            string a = "";
            int b = 1;
            string c = Read_Ini(App.langFilePath, "Command", b.ToString() + "_cmd", " ");
            string d = Read_Ini(App.langFilePath, "Command", b.ToString() + "_content", " ").Replace("\\n", Environment.NewLine);
            while (!c.Equals(" ") && !d.Equals(" "))
            {
                if (cmd.ToLower().StartsWith(c.ToLower()))
                {
                    a = d;
                    break;
                }
                b++;
                c = Read_Ini(App.langFilePath, "Command", b.ToString() + "_cmd", " ");
                d = Read_Ini(App.langFilePath, "Command", b.ToString() + "_content", " ").Replace("\\n", Environment.NewLine);
            }
            return a;
        }
        #endregion

        #region API函数声明

        #region 窗口拖动
        [DllImport("user32.dll")]//拖动无窗体的控件
        static extern bool ReleaseCapture();
        public static bool ReleaseCaptureImpl()
        {
            return ReleaseCapture();
        }
        [DllImport("user32.dll")]
        static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        #endregion

        #region 获取壁纸

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool SystemParametersInfo(uint uAction, uint uParam, StringBuilder lpvParam, uint init);
        public static bool GetSystemParametersInfo(uint uAction, uint uParam, StringBuilder lpvParam, uint init)
        {
            return SystemParametersInfo(uAction, uParam, lpvParam, init);
        }

        #endregion

        #region 获取用户头像
        [DllImport("shell32.dll", EntryPoint = "#261", CharSet = CharSet.Unicode, PreserveSig = false)]
        static extern void GetUserTilePath(
          string username,
          UInt32 whatever, // 0x80000000
          StringBuilder picpath, int maxLength);

        public static string GetUserTilePath(string username)
        {   // username: use null for current user
            var sb = new StringBuilder(1000);
            GetUserTilePath(username, 0x80000000, sb, sb.Capacity);
            return sb.ToString();
        }
        #endregion

        #region 设置窗口阴影
        private const int CS_DropSHADOW = 0x20000;
        private const int GCL_STYLE = (-26);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SetClassLong(IntPtr hwnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int GetClassLong(IntPtr hwnd, int nIndex);

        public static void SetShadow(IntPtr hwnd)
        {
            _ = SetClassLong(hwnd, GCL_STYLE, GetClassLong(hwnd, GCL_STYLE) | CS_DropSHADOW);
        }

        #endregion

        #region 隐藏鼠标 0/1 隐藏/显示
        [DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = CharSet.Auto)]
        static extern void ShowCursor(int status);

        public static void SetCursor(int status)
        {
            ShowCursor(status);
        }
        #endregion

        #endregion

        #region 窗口拖动
        public static void Move_Window(IntPtr handle)
        {
            //拖动窗体
            ReleaseCapture();
            SendMessage(handle, 0x0112, 0xF010 + 0x0002, 0);
        }
        #endregion

        #region 闹钟功能

        public static void OK_Alarm(int id)
        {
            if (App.dflag)
                LogtoFile("[DEBUG]Alarm ID " + id.ToString());
            if (id > -1)
            {
                DateTimeFormatInfo dtFormat = new()
                {
                    ShortDatePattern = "yyyy/MM/dd HH:mm:ss"
                };
                try
                {
                    DateTime dt = Convert.ToDateTime(App.scheduleitems[id].Time, dtFormat);
                    switch (App.scheduleitems[id].re)
                    {
                        case -2.0:
                            break;
                        case -1.0:
                            App.scheduleitems[id].Time = dt.AddDays(1.0).ToString("yyyy/MM/dd HH:mm:ss");
                            Write_Ini(App.sConfig, (id + 1).ToString(), "Time", App.scheduleitems[id].Time);
                            break;
                        case 0.0:
                            App.scheduleitems[id].Time = dt.AddDays(7.0).ToString("yyyy/MM/dd HH:mm:ss");
                            Write_Ini(App.sConfig, (id + 1).ToString(), "Time", App.scheduleitems[id].Time);
                            break;
                        default:
                            App.scheduleitems[id].Time = dt.AddDays(Math.Abs(App.scheduleitems[id].re)).ToString("yyyy/MM/dd HH:mm:ss");
                            Write_Ini(App.sConfig, (id + 1).ToString(), "Time", App.scheduleitems[id].Time);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LogError(ex, "Hiro.Exception.Alarm.OK");
                }

            }
        }

        public static void Delete_Alarm(int id)
        {
            var inipath = App.sConfig;
            if (id > -1)
            {
                while (id < App.scheduleitems.Count - 1)
                {
                    App.scheduleitems[id].Name = App.scheduleitems[id + 1].Name;
                    App.scheduleitems[id].Command = App.scheduleitems[id + 1].Command;
                    App.scheduleitems[id].Time = App.scheduleitems[id + 1].Time;
                    App.scheduleitems[id].re = App.scheduleitems[id + 1].re;
                    Write_Ini(inipath, (id + 1).ToString(), "name", Read_Ini(inipath, (id + 2).ToString(), "name", " "));
                    Write_Ini(inipath, (id + 1).ToString(), "command", Read_Ini(inipath, (id + 2).ToString(), "command", " "));
                    Write_Ini(inipath, (id + 1).ToString(), "time", Read_Ini(inipath, (id + 2).ToString(), "time", " "));
                    Write_Ini(inipath, (id + 1).ToString(), "repeat", Read_Ini(inipath, (id + 2).ToString(), "repeat", " "));
                    id++;
                    System.Windows.Forms.Application.DoEvents();
                }
                Write_Ini(inipath, (id + 1).ToString(), "Name", " ");
                Write_Ini(inipath, (id + 1).ToString(), "Command", " ");
                Write_Ini(inipath, (id + 1).ToString(), "Time", " ");
                Write_Ini(inipath, (id + 1).ToString(), "repeat", " ");
                App.scheduleitems.RemoveAt(id);
            }
            else
                App.Notify(new Hiro_Notice(Get_Translate("alarmmissing"), 2, Get_Translate("schedule")));

        }

        public static void Delay_Alarm(int id)
        {
            if (id > -1)
                App.scheduleitems[id].Time = DateTime.Now.AddMinutes(5.0).ToString("yyyy/MM/dd HH:mm:ss");
            else
                App.Notify(new Hiro_Notice(Get_Translate("alarmmissing"), 2, Get_Translate("schedule")));
        }
        #endregion

        #region 数字转换
        public static int ConvertInt(string str, int Default = 0)
        {
            try
            {
                return int.Parse(str);
            }
            catch
            {
                return Default;
            }
        }
        #endregion

        #region 设置开机自启
        public static void Set_Autorun(bool boo)
        {
            if (boo)
            {
                try
                {
                    string strName = "\"" + Path_Prepare(Hiro_Resources.ApplicationPath) + "\"";//获取要自动运行的应用程序名
                    Microsoft.Win32.RegistryKey? registry = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//检索指定的子项
                    registry ??= Microsoft.Win32.Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");//则创建指定的子项
                    registry.SetValue("Hiro_Autostart", strName + " silent");//设置该子项的新的“键值对”
                    Write_Ini(App.dConfig, "Config", "AutoRun", "1");
                    LogtoFile("[HIROWEGO]Enable Autorun");
                }
                catch (Exception ex)
                {
                    LogError(ex, "Hiro.Exception.Config.Autorun");
                }
            }
            else
            {
                Microsoft.Win32.RegistryKey? registry = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//读取指定的子项
                if (registry == null)//若指定的子项不存在
                    return;
                registry = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");//则创建指定的子项
                registry.DeleteValue("Hiro_Autostart", false);//删除指定“键名称”的键/值对
                Write_Ini(App.dConfig, "Config", "AutoRun", "0");
                LogtoFile("[HIROWEGO]Disable Autorun");
            }
        }
        #endregion

        #region 获取Windows版本
        public static string Get_OSVersion()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().OSVersion.Trim();
        }

        #endregion

        #region 获取系统主题色

        public static void IntializeColorParameters()
        {
            Color mAppAccentColor = (Color)ColorConverter.ConvertFromString("#00C4FF");
            if (!Read_Ini(App.dConfig, "Config", "LockColor", "default").Equals("default"))
            {
                try
                {
                    mAppAccentColor = (Color)ColorConverter.ConvertFromString(Read_Ini(App.dConfig, "Config", "LockColor", "#00C4FF"));
                }
                catch (Exception ex)
                {
                    LogError(ex, "Hiro.Exception.System.Color");
                }
            }
            else
            {
                mAppAccentColor = GetThemeColor();
            }
            App.AppAccentColor = mAppAccentColor;
            App.AppForeColor = Get_ForeColor(mAppAccentColor, Read_Ini(App.dConfig, "Config", "Reverse", "0").Equals("1"));
            LogtoFile("[HIROWEGO]Accent Color: " + App.AppAccentColor.ToString());
            LogtoFile("[HIROWEGO]Fore Color: " + App.AppForeColor.ToString());
        }

        public static Color Get_ForeColor(Color AccentColor, bool Reverse = false)
        {
            double luminance = (0.299 * AccentColor.R + 0.587 * AccentColor.G + 0.114 * AccentColor.B) / 255;
            if (luminance > 0.5)
            {
                return Reverse ? Colors.White : Colors.Black;
            }
            else
            {
                return Reverse ? Colors.Black : Colors.White;
            }
        }

        [DllImport("uxtheme.dll", EntryPoint = "#95")]
        static extern uint GetImmersiveColorFromColorSetEx(uint dwImmersiveColorSet, uint dwImmersiveColorType, bool bIgnoreHighContrast, uint dwHighContrastCacheMode);
        [DllImport("uxtheme.dll", EntryPoint = "#96")]
        static extern uint GetImmersiveColorTypeFromName(IntPtr pName);
        [DllImport("uxtheme.dll", EntryPoint = "#98")]
        static extern int GetImmersiveUserColorSetPreference(bool bForceCheckRegistry, bool bSkipCheckOnFail);
        // Get theme color
        public static Color GetThemeColor()
        {
            var colorSetEx = GetImmersiveColorFromColorSetEx(
                (uint)GetImmersiveUserColorSetPreference(false, false),
                GetImmersiveColorTypeFromName(Marshal.StringToHGlobalUni("ImmersiveStartSelectionBackground")),
                false, 0);

            var colour = Color.FromArgb((byte)((0xFF000000 & colorSetEx) >> 24), (byte)(0x000000FF & colorSetEx),
                (byte)((0x0000FF00 & colorSetEx) >> 8), (byte)((0x00FF0000 & colorSetEx) >> 16));

            return colour;
        }
        #endregion

        #region 颜色处理
        public static Color Color_Multiple(Color origin, int Multiple)
        {
            Multiple = (Multiple > 255) ? 255 : (Multiple < 0) ? 0 : Multiple;
            double new_R = origin.R * Multiple / 255;
            double new_B = origin.B * Multiple / 255;
            double new_G = origin.G * Multiple / 255;
            return Color.FromRgb((byte)new_R, (byte)new_G, (byte)new_B);
        }

        public static Color Color_Transparent(Color origin, int val)
        {
            return Color.FromArgb((byte)val, origin.R, origin.G, origin.B);
        }
        #endregion

        #region 检测全屏程序
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(HandleRef hWnd, [In, Out] ref RECT rect);

        public static bool IsForegroundFullScreen()
        {
            return IsForegroundFullScreen(null);
        }

        public static bool IsForegroundFullScreen(System.Windows.Forms.Screen? screen)
        {
            screen ??= System.Windows.Forms.Screen.PrimaryScreen;
            RECT rect = new();
            IntPtr hWnd = (IntPtr)GetForegroundWindow();
            GetWindowRect(new HandleRef(null, hWnd), ref rect);
            return screen != null && screen.Bounds.Width == (rect.right - rect.left) && screen.Bounds.Height == (rect.bottom - rect.top);
        }
        #endregion

        #region 非占用读取图片
        public static BitmapImage? GetBitmapImage(string fileName)
        {
            if (File.Exists(fileName) == false)
                return null;
            BitmapImage bitmapimage = new();
            bitmapimage.BeginInit();
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            switch (Read_Ini(App.dConfig, "Config", "ImageRead", "1"))
            {
                case "0":
                    {
                        bitmapimage.StreamSource = new MemoryStream(File.ReadAllBytes(fileName));
                        break;
                    }

                default:
                    {
                        bitmapimage.UriSource = new Uri(fileName);
                        break;
                    }
            }

            bitmapimage.EndInit();
            bitmapimage.Freeze();
            return bitmapimage;
        }

        #endregion

        #region 杂项功能

        public static int GetRandomUnusedPort()
        {
            var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
            listener.Start();
            var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        #region 保持窗口最前

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern IntPtr SetCapture(IntPtr hWnd);
        public static IntPtr SetCaptureImpl(IntPtr hWnd)
        {
            return SetCapture(hWnd);
        }

        public static void SetWindowToForegroundWithAttachThreadInput(Window window)
        {
            var interopHelper = new System.Windows.Interop.WindowInteropHelper(window);
            var thisWindowThreadId = GetWindowThreadProcessId(interopHelper.Handle, IntPtr.Zero);
            var currentForegroundWindow = GetForegroundWindow();
            var currentForegroundWindowThreadId = GetWindowThreadProcessId(currentForegroundWindow, IntPtr.Zero);
            AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, true);
            window.Activate();
        }

        public static void CancelWindowToForegroundWithAttachThreadInput(Window window)
        {
            var interopHelper = new System.Windows.Interop.WindowInteropHelper(window);
            var thisWindowThreadId = GetWindowThreadProcessId(interopHelper.Handle, IntPtr.Zero);
            var currentForegroundWindow = GetForegroundWindow();
            var currentForegroundWindowThreadId = GetWindowThreadProcessId(currentForegroundWindow, IntPtr.Zero);
            AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, false);
        }

        #endregion

        #endregion

        #region 加密解密

        internal static int GetNumInt(byte[] b)
        {
            var str = Encoding.UTF8.GetString(b, 0, 8);
            return Convert.ToInt32(str, 16);
        }

        internal static byte[] GetNumBytes(int length)
        {
            var str = length.ToString("X8");
            return Encoding.UTF8.GetBytes(str);
        }

        public static byte[] EncryptFile(byte[] array, string key, string filename)
        {
            return Encrypt(GetEncryptPrefix(filename).Concat(array).ToArray(), key);
        }

        private static byte[] GetEncryptPrefix(string filename)
        {
            return GetNumBytes(Encoding.UTF8.GetBytes(filename).Length).Concat(Encoding.UTF8.GetBytes(filename)).ToArray();
        }

        public static byte[] DecryptFile(byte[] array, string key, out string filename)
        {
            var ret = Decrypt(array, key);
            var len = GetNumInt(ret.Take(8).ToArray());
            if (App.dflag)
                LogtoFile("[DECRYPT]Length:" + len.ToString());
            ret = ret.Skip(8).ToArray();
            filename = Encoding.UTF8.GetString(ret.Take(len).ToArray());
            if (App.dflag)
                LogtoFile("[DECRYPT]Filename:" + filename);
            ret = ret.Skip(len).ToArray();
            return ret;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="array">要加密的 byte[] 数组</param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] array, string key)
        {
            key = GetStringMD5(key);
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            Aes rDel = Aes.Create();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(array, 0, array.Length);
            return resultArray;
        }

        private static string GetStringMD5(string str)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
            StringBuilder sBuilder = new();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="array">要解密的 byte[] 数组</param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] array, string key)
        {
            key = GetStringMD5(key);
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            Aes rDel = Aes.Create();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(array, 0, array.Length);
            return resultArray;
        }


        #endregion

        #region 设置帧率
        public static void SetFrame(int f)
        {
            Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(Timeline),
                new FrameworkPropertyMetadata { DefaultValue = f }
                );
        }
        #endregion

    }


}
