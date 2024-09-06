using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Hiro.Helpers
{
    internal class HTaskbar
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private const int ABM_GETTASKBARPOS = 5;

        public static Rect? GetTaskbarSize()
        {
            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            IntPtr result = SHAppBarMessage(ABM_GETTASKBARPOS, ref abd);

            if (result == IntPtr.Zero)
                return null;

            return new Rect(abd.rc.left, abd.rc.top, abd.rc.right - abd.rc.left, abd.rc.bottom - abd.rc.top);
        }

        public static int GetTaskbarPosition()
        {
            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            IntPtr result = SHAppBarMessage(ABM_GETTASKBARPOS, ref abd);

            if (result == IntPtr.Zero)
                return 3;

            return abd.uEdge; // 0:左, 1:上, 2:右, 3:下
        }
    }
}
