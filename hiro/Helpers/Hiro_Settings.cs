using Hiro;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using Hiro.Helpers;

namespace Hiro.Helpers
{
    internal class Hiro_Settings
    {
        #region 读Ini文件
        public static string Read_Ini(string iniFilePath, string Section, string Key, string defaultText)
        {
            if (File.Exists(iniFilePath))
            {
                byte[] buffer = new byte[1024];
                int ret = GetPrivateProfileString(Encoding.GetEncoding("utf-8").GetBytes(Section), Encoding.GetEncoding("utf-8").GetBytes(Key), Encoding.GetEncoding("utf-8").GetBytes(defaultText), buffer, 1024, iniFilePath);
                return Hiro_Text.DeleteUnVisibleChar(Encoding.GetEncoding("utf-8").GetString(buffer, 0, ret)).Trim();
            }
            else
            {
                return defaultText;
            }
        }

        public static string Read_PPIni(string iniFilePath, string Section, string Key, string defaultText)
        {
            return Hiro_Text.Path_Prepare_EX(Read_Ini(iniFilePath, Section, Key, defaultText));
        }

        public static string Read_PPDCIni(string Key, string defaultText)
        {
            return Read_PPIni(App.dConfig, "Config", Key, defaultText);
        }
        public static string Read_DCIni(string Key, string defaultText)
        {
            return Read_Ini(App.dConfig, "Config", Key, defaultText);
        }
        #endregion

        #region 写Ini文件
        public static bool Write_Ini(string iniFilePath, string Section, string Key, string Value)
        {
            try
            {
                if (!File.Exists(iniFilePath))
                    File.Create(iniFilePath).Close();
                long OpStation = WritePrivateProfileString(Encoding.GetEncoding("utf-8").GetBytes(Section), Encoding.GetEncoding("utf-8").GetBytes(Key), Encoding.GetEncoding("utf-8").GetBytes(Value), iniFilePath);
                if (OpStation == 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                Hiro_Logger.LogError(ex, "Hiro.Exception.Config.Update");
                return false;
            }

        }
        #endregion

        #region 读文件
        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(byte[] section, byte[] key, byte[] val, string filePath);
        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern int GetPrivateProfileString(byte[] section, byte[] key, byte[] def, byte[] retVal, int size, string filePath);
        #endregion

    }
}
