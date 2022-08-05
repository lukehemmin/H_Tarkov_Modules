using Aki.Debugging.Patches;
using BepInEx;

namespace Aki.Debugging
{
    [BepInPlugin("com.spt-aki.debugging", "AKI.Debugging", "1.0.0")]
    public class AkiDebuggingPlugin : BaseUnityPlugin
    {
        public AkiDebuggingPlugin()
        {
            Logger.LogInfo("Loading: Aki.Debugging");

            // new CoordinatesPatch().Enable();
        }
    }
}
