using TouchSocket.Core;

namespace MFramework.CommSystem.Extensions
{
    public static class IPluginObjectExtension
    {
        public static T GetPlugin<T>(this IPluginObject pluginObject) where T : class, IPlugin =>
            pluginObject.PluginManager.GetPlugin<T>();

        public static bool TryGetPlugin<T>(this IPluginObject pluginObject, out T plugin) where T : class, IPlugin =>
            pluginObject.PluginManager.TryGetPlugin(out plugin);
    }
}