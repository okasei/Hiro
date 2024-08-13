using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Threading;
using static Hiro.Helpers.Hiro_Class;

namespace Hiro.Helpers
{
    internal class Hiro_Wallpaper
    {
        static PerformanceCounter? _performanceCounter = null;
        static Hiro_Wallvideo? wallPaperPlayer = null;
        static IntPtr wallPaperPlayerIntPtr = IntPtr.Zero;
        static nint programHandle = 0;

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string className, string winName);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageTimeout(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, uint fuFlage, uint timeout, IntPtr result);

        //查找窗口的委托 查找逻辑
        public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc proc, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string className, string winName);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hwnd, IntPtr parentHwnd);

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

        internal static void RemoveWallpaperSigns()
        {
            wallPaperPlayerIntPtr = IntPtr.Zero;
            wallPaperPlayer = null;
        }

        internal static void SetWallpaperVideo(IntPtr wallPaperPlayer)
        {
            wallPaperPlayerIntPtr = wallPaperPlayer;
            SendMsgToProgman();
            SetParent(wallPaperPlayer, programHandle);
        }

        internal static int Set_Wallpaper(List<string> parameter)
        {
            var source = Hiro_Utils.Get_Translate("wallpaper");
            if (wallPaperPlayer != null)
            {
                var oflag = false;
                switch (parameter[0].Trim())
                {
                    case "close":
                    case "close()":
                        {
                            Hiro_Utils.HiroInvoke(() =>
                            {
                                wallPaperPlayer.Close();
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
                                Hiro_Utils.Write_Ini(App.dConfig, "Config", "WallVideoMute", "true");
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
                                Hiro_Utils.Write_Ini(App.dConfig, "Config", "WallVideoMute", "false");
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
                if (Hiro_File.isVideo(parameter[0]) == true)
                {
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
                    App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("wpchanged"), 2, source));
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
                    Hiro_Utils.RunExe($"notify({Hiro_Utils.Get_Translate("wpnexist")},2)", source);
                }
                return -1;
            }
        }

        #region 设置壁纸

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        public enum Style : int
        {
            Fill,
            Fit,
            Span,
            Stretch,
            Tile,
            Center
        }
        #endregion
    }
}
