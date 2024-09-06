using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Hiro.Helpers.HClass;
using static Hiro.APIs.AWin;
using System.Windows;
using System.Threading;
using System.Windows.Forms;
using System.Text;

namespace Hiro.Helpers
{
    internal class HWin
    {

        internal static void SetProcessPriority(string para, string? source = null, bool mute = false)
        {
            string? infoKey = null;
            switch (para.Trim().ToLower())
            {
                case "realtime":
                case "real":
                case "256":
                case "5":
                    {
                        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
                        infoKey = "prorityreal";
                        break;
                    }
                case "high":
                case "h":
                case "128":
                case "4":
                    {
                        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
                        infoKey = "prorityhigh";
                        break;
                    }
                case "abovenormal":
                case "an":
                case "above":
                case "32768":
                case "3":
                    {
                        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;
                        infoKey = "prorityan";
                        break;
                    }
                case "normal":
                case "middle":
                case "32":
                case "2":
                    {
                        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
                        infoKey = "proritynormal";
                        break;
                    }
                case "belownormal":
                case "bn":
                case "below":
                case "16384":
                case "1":
                    {
                        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
                        infoKey = "proritybn";
                        break;
                    }
                case "idle":
                case "null":
                case "free":
                case "low":
                case "64":
                case "0":
                    {
                        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Idle;
                        infoKey = "prorityidle";
                        break;
                    }
                default:
                    {
                        infoKey = "prorityerror";
                        break;
                    }
            }
            if (!mute)
                App.Notify(new Hiro_Notice(HText.Get_Translate(infoKey), 2, source));
        }

        internal static void SetEffiencyMode(string para, string? source = null, bool mute = false)
        {
            if (!IsWindows11())
            {
                if (!mute)
                    App.Notify(new Hiro_Notice(HText.Get_Translate("effiencywin"), 2, source));
                return;
            }
            string? infoKey = null;
            switch (para.Trim().ToLower())
            {
                case "normal":
                case "default":
                case "false":
                case "off":
                case "performance":
                case "0":
                    {
                        SetProcessEcoQoS(Process.GetCurrentProcess().Handle, false);
                        infoKey = "effiencynormal";
                        break;
                    }
                case "lowpower":
                case "low":
                case "effiency":
                case "true":
                case "eco":
                case "on":
                case "1":
                    {
                        SetProcessEcoQoS(Process.GetCurrentProcess().Handle, true);
                        infoKey = "effiencyeco";
                        break;
                    }
                default:
                    {
                        infoKey = "effiencyerror";
                        break;
                    }
            }
            if (!mute)
                App.Notify(new Hiro_Notice(HText.Get_Translate(infoKey), 2, source));
        }

        //实现
        static bool SetProcessEcoQoS(IntPtr hProcess, bool bFlag)
        {
            // 此结构有三个字段Version，ControlMask 和 StateMask
            uint controlMask = 0x1; //非权重开关
            uint stateMask = (uint)(bFlag ? 0x1 : 0x0);
            uint version = 1;
            int szControlBlock = 12; // 三个uint的大小
            IntPtr homo = Marshal.AllocHGlobal(szControlBlock);

            // Marshal.WriteInt32 将一个32位整数写入一个指定偏移量的非托管内存指针
            Marshal.WriteInt32(homo, (int)version); //homo 指向内存块开头
            Marshal.WriteInt32(homo + 4, (int)controlMask); // 将 controlMask 值写入第2字段地址，需将 homo 指针加4字节
            Marshal.WriteInt32(homo + 8, (int)stateMask); // 将 stateMask 值写入第3个字段地址，需将 homo 指针加8个字节
            bool bRet = false;

            // 此句柄必须具有 PROCESS_SET_INFORMATION 访问权限
            if (!SetProcessInformation(hProcess, PROCESS_INFORMATION_CLASS.ProcessPowerThrottling, homo, (uint)szControlBlock))
            {
                goto End;
            }
            if (!SetPriorityClass(hProcess, (uint)(bFlag ? 0x40 : 0x20)))
            {
                goto End;
            }
            bRet = true;
            goto End;
        End:
            Marshal.FreeHGlobal(homo);
            return bRet;
        }

        internal static Size GetDpiScale()
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            var dpiFactorX = GetDeviceCaps(hdc, LOGPIXELSX) / 96.0;
            var dpiFactorY = GetDeviceCaps(hdc, LOGPIXELSY) / 96.0;
            ReleaseDC(IntPtr.Zero, hdc);
            return new Size(dpiFactorX, dpiFactorY);
        }

        internal static bool IsWindows11()
        {
            return !IsBelowWinVer(10, 22000);
        }

        internal static bool IsWindows10()
        {
            return !IsBelowWinVer(10, 0);
        }

        internal static bool IsBelowWinVer(int mainVer, int subVer)
        {
            var osDescription = RuntimeInformation.OSDescription;
            var osDescs = osDescription.Split(' ');
            foreach (var osDesc in osDescs)
            {
                if (!osDesc.Contains(".")) continue;
                var versions = osDesc.Split('.');
                if (versions.Length < 2) continue;
                return int.Parse(versions[0]) < mainVer || int.Parse(versions[2]) < subVer;
            }
            return true;
        }

        [DllImport("winmm.dll")]
        private static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        public static int GetSystemVolume()
        {
            // 获取音量
            uint volume;
            waveOutGetVolume(IntPtr.Zero, out volume);

            // 音量的低位和高位分别表示左右声道
            ushort calcVol = (ushort)(volume & 0xFFFF);

            // 将音量转换为0-100之间的数值
            int finalVolume = (int)(calcVol / (ushort.MaxValue / 100));
            return finalVolume;
        }

        public static float GetBatteryPercentage()
        {
            // 获取系统电源状态
            PowerStatus powerStatus = SystemInformation.PowerStatus;

            // 返回电池电量百分比 (0.0 - 1.0)，乘以 100 转换为百分比
            return powerStatus.BatteryLifePercent * 100;
        }

        public static string GetPowerLineStatus()
        {
            // 获取电源线状态
            PowerStatus powerStatus = SystemInformation.PowerStatus;

            // 返回电源线状态（如充电、已连接或未连接）
            return powerStatus.PowerLineStatus.ToString();
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer); 
        
        public static float GetMemoryUsagePercentage()
        {
            MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(memStatus))
            {
                ulong totalMemory = memStatus.ullTotalPhys;
                ulong availableMemory = memStatus.ullAvailPhys;
                float usedMemory = totalMemory - availableMemory;

                return (float)(usedMemory * 100.0 / totalMemory);
            }
            else
            {
                return -1.0f;
            }
        }

        internal static void TryCatch(string module, Action callback)
        {
            try
            {
                callback.Invoke();
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, module);
            }
        }

    }
}
