using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.Healing
{
    public class MainMenuControllerPatch : ModulePatch
    {
        static MainMenuControllerPatch()
        {
            _ = nameof(IHealthController.HydrationChangedEvent);
            _ = nameof(MainMenuController.HealthController);
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainMenuController).GetMethod("method_1", PatchConstants.PrivateFlags);
        }

        [PatchPostfix]
        private static void PatchPostfix(MainMenuController __instance)
        {
            var healthController = __instance.HealthController;
            var listener = Utils.Healing.HealthListener.Instance;
            listener.Init(healthController, false);
        }
    }
}
