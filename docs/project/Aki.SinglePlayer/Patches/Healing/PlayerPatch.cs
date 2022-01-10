using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT;
using System.Reflection;
using System.Threading.Tasks;

namespace Aki.SinglePlayer.Patches.Healing
{
    public class PlayerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("Init", PatchConstants.PrivateFlags);
        }

        [PatchPostfix]
        private static async void PatchPostfix(Task __result, Player __instance, Profile profile)
        {
            await __result;

            if (profile != null && profile.Id.StartsWith("pmc"))
            {
                var listener = Utils.Healing.HealthListener.Instance;
                listener.Init(__instance.HealthController, true);
            }
        }
    }
}
