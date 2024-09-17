using Hiro.Widgets;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using static Hiro.APIs.ADesktop;
using static Hiro.Helpers.HClass;

namespace Hiro.Helpers
{
    internal class HDesktop
    {
        static PerformanceCounter? _performanceCounter = null;
        static Hiro_Wallvideo? wallPaperPlayer = null;
        static Hiro_Wallpaper? wallPaperWin = null;
        static IntPtr wallPaperPlayerIntPtr = IntPtr.Zero;
        static IntPtr wallPaperWinIntPtr = IntPtr.Zero;
        static nint programHandle = 0;

        internal static void SendMsgToProgman()
        {
            // 桌面窗口句柄，在外部定义，用于后面将我们自己的窗口作为子窗口放入
            programHandle = FindWindow("Progman", null);

            IntPtr result = IntPtr.Zero;
            // 向 Program Manager 窗口发送消息 0x52c 的一个消息，超时设置为2秒
            SendMessageTimeout(programHandle, 0x52c, IntPtr.Zero, IntPtr.Zero, 0, 2, result);

            // 遍历顶级窗口
            EnumWindows((hwnd, lParam) =>
            {
                // 找到第一个 WorkerW 窗口，此窗口中有子窗口 SHELLDLL_DefView，所以先找子窗口
                if (FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null) != IntPtr.Zero)
                {
                    // 找到当前第一个 WorkerW 窗口的，后一个窗口，及第二个 WorkerW 窗口。
                    IntPtr tempHwnd = FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", null);

                    // 隐藏第二个 WorkerW 窗口
                    ShowWindow(tempHwnd, 0);
                }
                return true;
            }, IntPtr.Zero);
        }

        internal static void PauseVideo()
        {
            Hiro_Utils.HiroInvoke(() =>
            {
                if (wallPaperPlayer != null && wallPaperPlayer.Media.IsPlaying)
                {
                    wallPaperPlayer.Media.Pause();
                }
            });

        }
        internal static void PlayVideo()
        {
            Hiro_Utils.HiroInvoke(() =>
            {
                if (wallPaperPlayer != null && wallPaperPlayer.Media.IsPaused)
                {
                    wallPaperPlayer.Media.Play();
                }
            });
        }

        internal static bool? IsPlaying()
        {
            if (wallPaperPlayer == null)
                return null;
            var p = false;
            Hiro_Utils.HiroInvoke(() =>
            {
                p = wallPaperPlayer.isPlaying;
            });
            return p;
        }

        static void Initialize_PerformanceCounter()
        {
            _performanceCounter ??= new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        internal static void RemoveWallpaperPlayerSigns()
        {
            wallPaperPlayerIntPtr = IntPtr.Zero;
            wallPaperPlayer = null;
        }
        internal static void RemoveWallpaperSigns()
        {
            wallPaperWinIntPtr = IntPtr.Zero;
            wallPaperWin = null;
        }

        internal static void SetWallpaperVideo(IntPtr wallPaperPlayer)
        {
            wallPaperPlayerIntPtr = wallPaperPlayer;
            SendMsgToProgman();
            SetParent(wallPaperPlayer, programHandle);
        }

        internal static void SetWallpaperWin(IntPtr wallPaperPlayer)
        {
            wallPaperWinIntPtr = wallPaperPlayer;
            SendMsgToProgman();
            SetParent(wallPaperPlayer, programHandle);
        }

        internal static void ResetTaskbarWin()
        {
            Hiro_Utils.HiroInvoke(() =>
            {
                if (App.tb != null)
                {
                    App.tb.Close();
                    App.tb = null;
                }
                NewTaskbarWin();
            });
        }

        internal static void NewTaskbarWin()
        {
            Hiro_Utils.HiroInvoke(() =>
            {
                if (App.mn != null)
                {
                    App.tb ??= new Hiro_Taskbar();
                    App.tb.Show();
                    HMediaInfoManager.UpdatePlayInfo();
                }
            });
        }

        internal static void CloseTaskbarWin()
        {
            Hiro_Utils.HiroInvoke(() =>
            {
                var t = App.tb;
                App.tb = null;
                t?.Close();
                t = null;
            });
        }

        internal static Rectangle SetTaskbarWin(IntPtr winHandle)
        {
            var hShell = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", null);
            var hBar = FindWindowEx(hShell, IntPtr.Zero, "ReBarWindow32", null);
            var hMin = FindWindowEx(hBar, IntPtr.Zero, "MSTaskSwWClass", null);
            var hTray = FindWindowEx(hShell, IntPtr.Zero, "TrayNotifyWnd", null);
            Rectangle rTray = new(), rWin = new();
            GetWindowRect(hBar, ref rTray);
            GetWindowRect(winHandle, ref rWin);
            SetParent(winHandle, hBar);
            return rTray;
        }

        internal static IntPtr GetReBarIntPtr()
        {
            var hShell = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", null);
            return FindWindowEx(hShell, IntPtr.Zero, "ReBarWindow32", null);
        }

        internal static Rectangle GetTaskBarRect()
        {

            var hShell = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", null);
            var hBar = FindWindowEx(hShell, IntPtr.Zero, "ReBarWindow32", null);
            Rectangle rTray = new();
            GetWindowRect(hBar, ref rTray);
            return rTray;
        }

        internal static void MoveWin32(IntPtr winHandle, int left, int top, int width, int height)
        {
            MoveWindow(winHandle, left, top, width, height, true);
        }

        internal static int Set_Wallpaper(List<string> parameter)
        {
            var source = HText.Get_Translate("wallpaper");
            if (wallPaperPlayer != null)
            {
                var oflag = false;
                switch (parameter[0].Trim())
                {
                    case "close":
                    case "close()":
                    case "off":
                    case "off()":
                        {
                            Hiro_Utils.HiroInvoke(() =>
                            {
                                var _w = wallPaperPlayer;
                                RemoveWallpaperPlayerSigns();
                                _w.Close();
                            });
                            oflag = true;
                            break;
                        }
                    case "pause":
                    case "pause()":
                        {
                            Hiro_Utils.HiroInvoke(() =>
                            {
                                wallPaperPlayer.Media.Pause();
                                wallPaperPlayer.isPlaying = false;
                            });
                            oflag = true;
                            break;
                        }
                    case "play":
                    case "play()":
                    case "on":
                    case "on()":
                        {
                            Hiro_Utils.HiroInvoke(() =>
                            {
                                wallPaperPlayer.Media.Play();
                                wallPaperPlayer.isPlaying = true;
                            });
                            oflag = true;
                            break;
                        }
                    case "trimon":
                    case "trimon()":
                        {
                            Hiro_Utils.HiroInvoke(() =>
                            {
                                wallPaperPlayer.SetAutoCrop();
                            });
                            oflag = true;
                            break;
                        }
                    case "trimoff":
                    case "trimoff()":
                        {
                            Hiro_Utils.HiroInvoke(() =>
                            {
                                wallPaperPlayer.DisableAutoCrop();
                            });
                            oflag = true;
                            break;
                        }
                    case "muteon":
                    case "muteon()":
                        {
                            Hiro_Utils.HiroInvoke(() =>
                            {
                                wallPaperPlayer.Media.IsMuted = true;
                                HSet.Write_Ini(App.dConfig, "Config", "WallVideoMute", "true");
                            });
                            oflag = true;
                            break;
                        }
                    case "muteoff":
                    case "muteoff()":
                        {
                            Hiro_Utils.HiroInvoke(() =>
                            {
                                wallPaperPlayer.Media.IsMuted = false;
                                HSet.Write_Ini(App.dConfig, "Config", "WallVideoMute", "false");
                            });
                            oflag = true;
                            break;
                        }
                }
                if (oflag)
                    return 0;
            }
            if (wallPaperWin != null)
            {
                var oflag = false;
                switch (parameter[0].Trim())
                {
                    case "close":
                    case "close()":
                    case "off":
                    case "off()":
                        {
                            Hiro_Utils.HiroInvoke(() =>
                            {
                                var _w = wallPaperWin;
                                RemoveWallpaperSigns();
                                _w.Close();
                            });
                            oflag = true;
                            break;
                        }
                }
                if (oflag)
                    return 0;
            }
            if (File.Exists(parameter[0]))
            {
                if (HFile.isVideo(parameter[0]) == true)
                {
                    if (wallPaperWin != null)
                    {
                        wallPaperWin.Close();
                    }
                    if (wallPaperPlayer != null)
                    {
                        wallPaperPlayer.Play(parameter[0]);
                    }
                    else
                    {
                        Hiro_Utils.HiroInvoke(() =>
                        {
                            wallPaperPlayer ??= new();
                            wallPaperPlayer.Show();
                            wallPaperPlayer.Play(parameter[0]);
                        });
                    }
                    return 0;

                }
                else
                {
                    if (wallPaperPlayer != null)
                    {
                        wallPaperPlayer.Close();
                    }
                    if (parameter.Count > 2)
                    {
                        bool _f = false;
                        switch (parameter[2].ToLower())
                        {
                            case "temp":
                            case "temporary":
                            case "t":
                            case "fake":
                            case "f":
                                {
                                    if (wallPaperWin != null)
                                    {
                                        wallPaperWin.Wallpaper.Source = Hiro_Utils.GetBitmapImage(parameter[0]);
                                        wallPaperWin.ResetUniform(Convert.ToInt32(parameter[1]));
                                    }
                                    else
                                    {
                                        Hiro_Utils.HiroInvoke(() =>
                                        {
                                            wallPaperWin ??= new();
                                            wallPaperWin.Show();
                                            wallPaperWin.Wallpaper.Source = Hiro_Utils.GetBitmapImage(parameter[0]);
                                            wallPaperWin.ResetUniform(Convert.ToInt32(parameter[1]));
                                        });
                                    }
                                    _f = true;
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                        if (_f)
                            return 0;
                    }
                    using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
                    {
                        int[] para = [10, 6, 22, 2, 0, 0];
                        int[] par = [0, 0, 0, 0, 1, 0];
                        var v = Convert.ToInt32(parameter[1]);
                        v = v < 0 ? 0 : v > 5 ? 5 : v;
                        if (key != null)
                        {
                            key.SetValue(@"WallpaperStyle", para[v].ToString());
                            key.SetValue(@"TileWallpaper", par[v].ToString());
                        }
                    }
                    _ = SystemParametersInfo(20, 0, parameter[0], 0x01 | 0x02);
                    App.Notify(new Hiro_Notice(HText.Get_Translate("wpchanged"), 2, source));
                    return 0;
                }
            }
            else
            {
                if (wallPaperPlayer != null)
                {
                    wallPaperPlayer.Close();
                    return 0;
                }
                else
                {
                    Hiro_Utils.RunExe($"notify({HText.Get_Translate("wpnexist")},2)", source);
                }
                return -1;
            }
        }
    }
}
