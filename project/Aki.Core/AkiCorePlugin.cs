using Aki.Core.Patches;
using BepInEx;

namespace Aki.Core
{
    [BepInPlugin("com.spt-aki.core", "AKI.Core", "1.0.0")]
	class AkiCorePlugin : BaseUnityPlugin
	{
        public AkiCorePlugin()
        {
            Logger.LogInfo("Loading: Aki.Core");

            new ConsistencySinglePatch().Enable();
            new ConsistencyMultiPatch().Enable();
            new BattlEyePatch().Enable();
            new SslCertificatePatch().Enable();
            new UnityWebRequestPatch().Enable();
            new WebSocketPatch().Enable();
        }
    }
}
