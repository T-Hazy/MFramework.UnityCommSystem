using System;
using System.Linq;
using TouchSocket.Core;

namespace MFramework.CommSystem.Extensions
{
    public static class IPluginManagerExtension
    {
        public static T GetPlugin<T>(this IPluginManager manager) where T : class, IPlugin
        {
            return manager.Plugins.FirstOrDefault(plugin => plugin is T) as T;
        }

        public static bool TryGetPlugin<T>(this IPluginManager manager, out T plugin) where T : class, IPlugin
        {
            // 查找第一个满足条件的插件
            var foundPlugin = manager.Plugins.FirstOrDefault(plugin => plugin is T);

            // 检查找到的对象是否为null以及类型是否正确
            if (foundPlugin is T typedPlugin)
            {
                plugin = typedPlugin;
                return true;
            }
            plugin = null;
            return false;
        }
    }
}