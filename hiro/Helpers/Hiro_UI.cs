using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiro.Helpers
{
    internal class Hiro_UI
    {
        internal static void SetCustomWindowIcon(System.Windows.Window win)
        {
            var iconP = Hiro_Utils.Read_PPDCIni("CustomIcon", "");
            if (System.IO.File.Exists(iconP))
            {
                try
                {
                    win.Icon = Hiro_Utils.GetBitmapImage(iconP);
                }
                catch (Exception e)
                {
                    Hiro_Utils.LogError(e, "Hiro.Window.CustomIcon");
                }
            }
        }
    }
}
