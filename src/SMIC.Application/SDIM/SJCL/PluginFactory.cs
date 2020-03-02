using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMIC.SJCL
{
    public class PluginFactory
    {
        public static async Task<BasePluginsService> GetPlugin(IMemoryCache _cache, PluginsOptions _options, string type)
        {
            string cacheKey = $"plugin:{type}";

            if (_cache.TryGetValue(cacheKey, out BasePluginsService service))
            {
                return service;
            }
            else
            {
                //E:\ABP\Src\MyProjectVue(Abp4.9_Dapper)\4.9.0\aspnet-core\src\MyProjectVue.Web.Host\wwwroot\Plugins\netcoreapp2.2\Plugins.SJCL1000.dll
                var baseDirectory = Directory.GetCurrentDirectory();
                var dll = $"Plugins.SJCL.{type}.dll";
                var path = Path.Combine(baseDirectory, _options.PluginsPath, dll);
                try
                {
                    byte[] bytes = await System.IO.File.ReadAllBytesAsync(path);
                    var assembly = Assembly.Load(bytes);
                    //var obj = (BasePluginsService)assembly.CreateInstance($"Plugins.{type}.PluginsService");
                    var obj = (BasePluginsService)assembly.CreateInstance("SMIC.SJCL.PluginsService");
                    if (obj != null)
                    {
                        _cache.Set(cacheKey, obj, DateTimeOffset.Now.AddSeconds(60)); //1分钟
                    }

                    //Debug.WriteLine(bytes.Length);
                    bytes = null;

                    return obj;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

    }

}
