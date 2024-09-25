using Hiro.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiro.Helpers.Plugin
{
    internal class HPluginManager
    {
        private static Dictionary<string, HiroPlugin> plugins = new Dictionary<string, HiroPlugin>();

        internal static HiroPlugin CreateOrLoad(string path)
        {
            path = HText.Anti_Path_Prepare(path);
            if (plugins.ContainsKey(path)) 
                return plugins[path];
            var _plugin = new HiroPlugin(path);
            plugins.Add(path, _plugin);
            return _plugin;
        }

        internal static HiroPlugin UpdatePlugin(string path)
        {
            if (plugins.ContainsKey(path))
            {
                plugins[path].Unload();
                plugins[path] = new HiroPlugin(path);
                return plugins[path];
            }
            var _plugin = new HiroPlugin(path);
            plugins.Add(path, _plugin);
            return _plugin;
        }

        internal static HiroPlugin? GetPluginByPath(string path)
        {
            if (plugins.ContainsKey(path))
            {
                return plugins[path];
            }
            return null;
        }
    }
}
