using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hiro.APIs
{
    internal class ASet
    {
        #region 读文件
        [DllImport("kernel32")]//返回0表示失败，非0为成功
        internal static extern long WritePrivateProfileString(byte[] section, byte[] key, byte[] val, string filePath);
        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        internal static extern int GetPrivateProfileString(byte[] section, byte[] key, byte[] def, byte[] retVal, int size, string filePath);
        #endregion
    }
}
