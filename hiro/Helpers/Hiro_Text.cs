using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hiro.Helpers
{
    internal class Hiro_Text
    {
        internal static bool StartsWith(string text, string start)
        {
            return text.StartsWith(start, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
