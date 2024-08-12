using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static hiro.Helpers.Hiro_Class;

namespace hiro.Helpers
{
    internal class Hiro_Win
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetProcessEfficiencyMode(int mode);

        internal static void SetProcessPriority(string para, string? source = null)
        {
            switch (para.Trim().ToLower())
            {
                case "realtime":
                case "real":
                case "256":
                case "5":
                    {
                        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
                        break;
                    }
                case "high":
                case "h":
                case "128":
                case "4":
                    {
                        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
                        break;
                    }
                case "abovenormal":
                case "above":
                case "32768":
                case "3":
                    {
                        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;
                        break;
                    }
                case "normal":
                case "middle":
                case "32":
                case "2":
                    {
                        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
                        break;
                    }
                case "belownormal":
                case "below":
                case "16384":
                case "1":
                    {
                        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
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
                        break;
                    }
                default:
                    {
                        App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("error"), 2, source));
                        break;
                    }
            }
        }

        internal static void SetEffiencyMode(string para, string? source = null)
        {
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
                        break;
                    }
                default:
                    {
                        App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("error"), 2, source));
                        break;
                    }
            }
        }

        // 声明导入函数
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetProcessInformation([In] IntPtr hProcess, [In] PROCESS_INFORMATION_CLASS ProcessInformationClass, IntPtr ProcessInformation, uint ProcessInformationSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetPriorityClass(IntPtr handle, uint priorityClass);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, uint processId);

        private enum PROCESS_INFORMATION_CLASS
        {
            ProcessMemoryPriority,
            ProcessMemoryExhaustionInfo,
            ProcessAppMemoryInfo,
            ProcessInPrivateInfo,
            ProcessPowerThrottling,
            ProcessReservedValue1,
            ProcessTelemetryCoverageInfo,
            ProcessProtectionLevelInfo,
            ProcessLeapSecondInfo,
            ProcessInformationClassMax,
        }

        //实现
        public static bool SetProcessEcoQoS(IntPtr hProcess, bool bFlag)
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
    }
}
