using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hiro.APIs
{
    internal class ADesktop
    {
        [DllImport("user32.dll")]
        internal static extern IntPtr FindWindow(string className, string winName);

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessageTimeout(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, uint fuFlage, uint timeout, IntPtr result);

        //查找窗口的委托 查找逻辑
        internal delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);
        [DllImport("user32.dll")]
        internal static extern bool EnumWindows(EnumWindowsProc proc, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string className, string winName);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetParent(IntPtr hwnd, IntPtr parentHwnd);

        #region 设置壁纸

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
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
