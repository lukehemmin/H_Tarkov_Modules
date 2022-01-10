using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Aki.Bundles.Models;
using Aki.Common.Http;
using Aki.Common.Utils;

namespace Aki.Bundles.Utils
{
    public class BundleSettings
    {
        public const string CachePath = "Cache/StreamingAssets/windows/";
        public static Dictionary<string, BundleInfo> Bundles { get; private set; }

        static BundleSettings()
        {
            Bundles = new Dictionary<string, BundleInfo>();

            // clear cache
            if (VFS.Exists(CachePath))
            {
                VFS.DeleteDirectory(CachePath);
            }
        }

        public static void GetBundles()
        {
            var json = RequestHandler.GetJson("/singleplayer/bundles");
            var jArray = JArray.Parse(json);

            foreach (var jObj in jArray)
            {
                if (!Bundles.TryGetValue(jObj["key"].ToString(), out BundleInfo bundle))
                {
                    bundle = new BundleInfo(jObj["key"].ToString(),
                                            jObj["path"].ToString(),
                                            jObj["dependencyKeys"].ToObject<List<string>>().ToArray());
                    Bundles.Add(bundle.Key, bundle);
                }
            }
        }
    }
}
