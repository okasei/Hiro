using Hiro.Framework.Loader;
using Hiro.Framework.Services;
using Hiro.Helpers;
using Hiro.Plugin.Modelviews;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hiro.ModelViews
{
    internal class HiroPlugin : IDisposable
    {
        private HiroLoaderContext? _context = null;
        private IHiroPlugin? _plugin = null;
        private HiroService? _service = null;
        private Assembly? _assembly = null;
        public string Name => _plugin?.GetName(App.lang) ?? "Unknown Plugin";
        public string Version => _plugin?.Version ?? string.Empty;

        public string PackageName => _plugin?.Id ?? "com.hiro.unknown." + DateTime.UtcNow.ToString();
        public string Author => _plugin?.Author ?? string.Empty;
        internal string Icon => _plugin?.Icon ?? string.Empty;

        public HiroPlugin(string path)
        {
            _context = new HiroLoaderContext();
            // 加载程序集
            _assembly = _context.LoadFromAssemblyPath(path);

            // 在此处使用加载的程序集
            foreach (var type in _assembly.GetTypes())
            {
                if (typeof(IHiroPlugin).IsAssignableFrom(type) && !type.IsInterface)
                {
                    // 创建插件实例
                    _plugin = (IHiroPlugin?)Activator.CreateInstance(type);

                    // 初始化插件
                    _service = new HiroService(); // 传入主程序的服务实例
                    _plugin?.Initialize(_service);

                }
            }
        }

        internal string? ProcessRequest(string str, List<object>? para = null)
        {
            return _plugin?.HiroWeGo(str, para);
        }

        internal bool Unload()
        {
            if (_context != null)
            {
                var _id = _plugin?.Id ?? "Unknown";
                _plugin?.Dispose();
                WeakReference weakRef = new WeakReference(_context);
                // 卸载插件上下文
                _context.Unload();
                _context = null;

                for (int i = 0; weakRef.IsAlive && (i < 10); i++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                if (weakRef.IsAlive)
                {
                    if (App.dflag)
                    {
                        HLogger.LogtoFile($"[Plugin]Unloading plugin [{_id}] failed.");
                    }
                    _context = (HiroLoaderContext?)weakRef.Target;
                }
                return !weakRef.IsAlive;
            }
            return true;
        }
        void IDisposable.Dispose()
        {
            Unload();
        }
    }
}
