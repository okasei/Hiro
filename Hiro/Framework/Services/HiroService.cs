using Hiro.Helpers;
using Hiro.Plugin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiro.Framework.Services
{
    internal class HiroService : IHiroService
    {

        string IHiroService.GetData(string key, List<object>? param)
        {
            return "Hiro";
        }

        void IHiroService.Log(string message, string? source)
        {
            HLogger.LogtoFile($"[{source ?? "Unknown Plugin}"}]${message}");
        }

        bool IHiroService.RunExe(string path, string? source)
        {
            Hiro_Utils.RunExe(path, source);
            return true;
        }
    }
}
