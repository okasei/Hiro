using System;
using System.Runtime.InteropServices;

namespace Hiro.APIs
{
    internal class AWin
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetProcessEfficiencyMode(int mode);

        // 声明导入函数
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetProcessInformation([In] IntPtr hProcess, [In] PROCESS_INFORMATION_CLASS ProcessInformationClass, IntPtr ProcessInformation, uint ProcessInformationSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetPriorityClass(IntPtr handle, uint priorityClass);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, uint processId);

        internal enum PROCESS_INFORMATION_CLASS
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
    }
}
