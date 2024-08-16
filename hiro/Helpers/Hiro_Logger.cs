using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiro.Helpers
{
    internal class Hiro_Logger
    {
        #region 写日志相关

        public static void LogError(Exception ex, string Module)
        {
            StringBuilder str = new StringBuilder();
            if (ex.InnerException == null)
            {
                str.Append($"{Environment.NewLine}[ERROR]{Module}{Environment.NewLine}");
                str.Append($"Object: {ex.Source}{Environment.NewLine}");
                str.Append($"Exception: {ex.GetType().Name}{Environment.NewLine}");
                str.Append($"Details: {ex.Message}");
                if (App.dflag)
                {
                    str.Append($"{Environment.NewLine}StackTrace: {ex.StackTrace}");
                }
                
            }
            else
            {
                str.Append($"{Environment.NewLine}[ERROR]{Module}.InnerException{Environment.NewLine}");
                str.Append($"Object: {ex.InnerException.Source}{Environment.NewLine}");
                str.Append($"Exception: {ex.InnerException.GetType().Name}{Environment.NewLine}");
                str.Append($"Details: {ex.InnerException.Message}");
                if (App.dflag)
                {
                    str.Append($"{Environment.NewLine}StackTrace: {ex.InnerException.StackTrace}");
                }
            }
            LogtoFile(str.ToString());
        }

        public static void LogtoFile(string val)
        {
            try
            {
                var logDirectory = Hiro_Text.Path_PPX(@$"<current>\users\{App.eUserName}\log\");
                if (!Directory.Exists(logDirectory))
                    Hiro_File.CreateFolder(App.logFilePath);
                if (!File.Exists(App.logFilePath))
                    File.Create(App.logFilePath).Close();
                FileStream fs = new(App.logFilePath, FileMode.Open, FileAccess.ReadWrite);
                StreamReader sr = new(fs);
                var str = sr.ReadToEnd();
                StreamWriter sw = new(fs);
                sw.Write(DateTime.Now.ToString("[HH:mm:ss]") + val + Environment.NewLine);
                sw.Flush();
                sw.Close();
                sr.Close();
                sr.Dispose();
                fs.Close();
            }
            catch (Exception ex)
            {
                try
                {
                    LogError(ex, "Hiro.Exception.Log");
                }
                catch
                {

                }
            }
        }
        #endregion

    }
}
