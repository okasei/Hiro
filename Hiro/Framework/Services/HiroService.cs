using Hiro.Helpers;
using Hiro.Helpers.Database;
using Hiro.Helpers.Plugin;
using Hiro.Plugin.Modelviews;
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
        string pluginID = string.Empty;

        public HiroService(string pluginID)
        {
            this.pluginID = pluginID;
        }

        List<object>? IHiroService.GetData(string key, List<object>? param)
        {
            switch (key.ToLower())
            {
                case "hiro.appname":
                    {
                        return FillList(App.appTitle);
                    }
                case "hiro.accentcolor":
                    {
                        return FillList(App.AppAccentColor);
                    }
                case "hiro.forecolor":
                    {
                        return FillList(App.AppForeColor);
                    }
                case "hiro.lang":
                    {
                        return FillList(App.lang);
                    }
                case "hiro.dconfig":
                    {
                        return FillList(App.dConfig);
                    }
                case "hiro.dflag":
                    {
                        return FillList(App.dflag);
                    }
            }
            return FillList(new Exception("KeyNotFound"));
        }

        List<object> FillList(object obj, List<object>? list = null)
        {
            list ??= new List<object>();
            list.Add(obj);
            return list;
        }

        void IHiroService.Log(string message, string? source)
        {
            HLogger.LogtoFile($"[{source ?? "Unknown Plugin}"}]${message}");
        }

        bool? IHiroService.RunExe(string path, string? source)
        {
            Hiro_Utils.RunExe(path, source);
            return true;
        }

        void IHiroService.LogError(Exception ex, string Module)
        {
            HLogger.LogError(ex, Module);
        }

        bool? IHiroService.Link(string protocol, string RunParam)
        {
            var _tpl = HPluginManager.GetPluginByPath(pluginID);
            if (_tpl == null)
            {
                return null;
            }
            var id = DB_HIRO_PLUGINS.GetPluginInfo("Path", pluginID, "ID");
            if (id == null)
            {
                return null;
            }
            return DB_HIRO_LINKS.AddOrUpdateRecord(id, protocol, pluginID, RunParam);
        }

        void IHiroService.Notify(Hiro_Notice i)
        {
            App.Notify(new()
            {
                act = i.act,
                icon = new()
                {
                    HeroImage = i.icon?.HeroImage?? string.Empty,
                    Image = i.icon?.Image,
                    Location = i.icon?.Location ?? string.Empty
                },
                time = i.time,
                title = i.title,
                msg = i.msg,
            });
        }

        string IHiroService.ReadConfig(string Section, string Key, string defaultText)
        {
            return HSet.Read_Ini(App.dConfig, Section, Key, defaultText);
        }
    }
}
