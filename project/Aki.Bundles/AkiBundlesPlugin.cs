using Aki.Bundles.Patches;
using Aki.Bundles.Utils;
using BepInEx;

namespace Aki.Bundles
{
    [BepInPlugin("com.spt-aki.bundles", "AKI.Bundles", "1.0.0")]
    public class AkiBundlesPlugin : BaseUnityPlugin
    {
        public AkiBundlesPlugin()
        {
            Logger.LogInfo("Loading: Aki.Bundles");
            BundleManager.GetBundles();
            new EasyAssetsPatch().Enable();
            new EasyBundlePatch().Enable();
        }
    }
}
