using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Hiro.Helpers
{
    public class HHook
    {
        private static IntPtr _hookID = IntPtr.Zero;
        private static HookProc _proc = HookCallback;

        public static void SetHook()
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                _hookID = SetWindowsHookEx(WH_GETMESSAGE, _proc, GetModuleHandle(curModule.ModuleName), 0);
                if(_hookID == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Hiro_Utils.HiroInvoke(() =>
                    {

                        // Handle the drop files here
                        MessageBox.Show("Not Hooked");
                        MessageBox.Show($"SetWindowsHookEx failed with error code {errorCode}");
                    });
                }
            }
        }

        public static void Unhook()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // 处理钩子回调
            if (nCode >= 0)
            {
                // Check for WM_DROPFILES message
                int msg = Marshal.ReadInt32(wParam);
                if (msg == 0x0233)
                {
                    Hiro_Utils.HiroInvoke(() =>
                    {

                        // Handle the drop files here
                        MessageBox.Show("Files were dropped!");
                    });
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private const int WH_GETMESSAGE = 3;
    }

}
