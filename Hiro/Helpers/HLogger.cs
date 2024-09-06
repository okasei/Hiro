using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Hiro.Helpers
{
    internal class HLogger
    {
        #region 写日志相关

        public static void LogError(Exception ex, string Module)
        {
            StringBuilder str = new StringBuilder();
            var _ex = ex;
            str.Append($"{Environment.NewLine}[ERROR]{Module}{Environment.NewLine}");
            str.Append($"Object: {ex.Source}{Environment.NewLine}");
            str.Append($"Exception: {ex.GetType().Name}{Environment.NewLine}");
            str.Append($"Details: {ex.Message}");
            if (App.dflag)
            {
                str.Append($"{Environment.NewLine}StackTrace: {ex.StackTrace}");
                str.Append($"{Environment.NewLine}Functions: {GetStackTraceModelName()}");
                if (_ex.InnerException != null) {

                    _ex = _ex.InnerException;
                    str.Append($"{Environment.NewLine}InnerException: "); 
                    str.Append($"Object: {_ex.Source}{Environment.NewLine}");
                    str.Append($"Exception: {_ex.GetType().Name}{Environment.NewLine}");
                    str.Append($"Details: {_ex.Message}");
                    str.Append($"{Environment.NewLine}StackTrace: {_ex.StackTrace}");
                }
            }
            LogtoFile(str.ToString());
        }

        private static string GetStackTraceModelName()
        {
            //当前堆栈信息
            System.Diagnostics.StackTrace? st = new System.Diagnostics.StackTrace();
            System.Diagnostics.StackFrame[]? sfs = st.GetFrames();
            //过虑的方法名称,以下方法将不会出现在返回的方法调用列表中
            string? _fullName = string.Empty, _methodName = string.Empty;
            for (int i = 1; i < sfs.Length; ++i)
            {
                //非用户代码,系统方法及后面的都是系统调用，不获取用户代码调用结束
                if (System.Diagnostics.StackFrame.OFFSET_UNKNOWN == sfs[i].GetILOffset()) break;
                _methodName = sfs[i].GetMethod()?.Name;//方法名称
                _fullName = _methodName + "()->" + _fullName;
            }
            st = null;
            sfs = null;
            _methodName = null;
            return _fullName.TrimEnd('-', '>');
        }

        public static void LogtoFile(string val)
        {
            if (HSet.Read_DCIni("DisableLogger", "false").Equals("true", StringComparison.CurrentCultureIgnoreCase))
                return;
            try
            {
                var logDirectory = HText.Path_PPX(@$"<current>\users\{App.eUserName}\log\");
                if (!Directory.Exists(logDirectory))
                    HFile.CreateFolder(App.logFilePath);
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
                StringBuilder str = new StringBuilder();
                var _ex = ex;
                str.Append($"{Environment.NewLine}[ERROR]{"Hiro.Exception.Log"}{Environment.NewLine}");
                str.Append($"Object: {_ex.Source}{Environment.NewLine}");
                str.Append($"Exception: {_ex.GetType().Name}{Environment.NewLine}");
                str.Append($"Details: {_ex.Message}");
                if (App.dflag)
                {
                    str.Append($"{Environment.NewLine}StackTrace: {_ex.StackTrace}");
                    str.Append($"{Environment.NewLine}Functions: {GetStackTraceModelName()}");
                    if (_ex.InnerException != null)
                    {

                        _ex = _ex.InnerException;
                        str.Append($"{Environment.NewLine}InnerException: ");
                        str.Append($"Object: {_ex.Source}{Environment.NewLine}");
                        str.Append($"Exception: {_ex.GetType().Name}{Environment.NewLine}");
                        str.Append($"Details: {_ex.Message}");
                        str.Append($"{Environment.NewLine}StackTrace: {_ex.StackTrace}");
                    }
                }
                MessageBox.Show(str.ToString());
                
            }
        }
        #endregion

    }
}
