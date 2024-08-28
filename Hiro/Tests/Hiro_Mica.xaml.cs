using Hiro.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Hiro.Tests
{
    /// <summary>
    /// Hiro_Mica.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Mica : Window
    {
        const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20; // For dark mode, if you want to enable it
        const int DWMWA_SYSTEMBACKDROP_TYPE = 38; // Backdrop type for Mica effect
        const int DWMWA_MICA_EFFECT = 1029; // To enable Mica effect
        private const int WM_SYSCOMMAND = 0x112;
        private const int SC_MAXIMIZE = 0xF030;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int pvAttribute, int cbAttribute);

        public Hiro_Mica()
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            Loaded += (e, args) =>
            {
                var windowHandle = new WindowInteropHelper(this).Handle;

                // 设置窗口的背景材质为 Mica
                int micaEffect = 1; // 1 to enable Mica, 0 to disable
                DwmSetWindowAttribute(windowHandle, DWMWA_MICA_EFFECT, ref micaEffect, sizeof(int));

                // Optional: Enable immersive dark mode
                int darkMode = 1; // 1 for dark mode, 0 for light mode
                DwmSetWindowAttribute(windowHandle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkMode, sizeof(int));
            };
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            var hWnd = new WindowInteropHelper(this).Handle;
            PostMessage(hWnd, WM_SYSCOMMAND, (IntPtr)SC_MAXIMIZE, IntPtr.Zero);
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;
            var source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source?.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lparam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0084:
                    // for fixing #886
                    // https://developercommunity.visualstudio.com/t/overflow-exception-in-windowchrome/167357
                    try
                    {
                        if (HUI.IsInRange(Btn, lparam))
                        {
                            handled = true;
                        }
                        return new IntPtr(9);
                    }
                    catch (OverflowException)
                    {
                        handled = true;
                    }
                    break;
                case 0x00A1:
                    
                    {
                        if (HUI.IsInRange(Btn, lparam))
                        {
                            handled = true;
                            IInvokeProvider? invokeProv = new ButtonAutomationPeer(Btn).GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                            invokeProv?.Invoke();
                        }
                    }
                    break;
                default:
                    //Console.WriteLine("Msg: " + m.Msg + ";LParam: " + m.LParam + ";WParam: " + m.WParam + ";Result: " + m.Result);
                    break;
            }
            return IntPtr.Zero;

        }
    }
}
