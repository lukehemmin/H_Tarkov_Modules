using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System.Linq;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.MainMenu
{
    class InsuranceScreenPatch : ModulePatch
    {
        static InsuranceScreenPatch()
        {
            _ = nameof(MainMenuController.InventoryController);
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainMenuController).GetMethods(PatchConstants.PrivateFlags).FirstOrDefault(IsTargetMethod);
        }

        [PatchPrefix]
        private static void PrefixPatch(ref bool local)
        {
            local = false;
        }

        [PatchPostfix]
        private static void PostfixPatch(ref bool ___bool_0)
        {
            ___bool_0 = true;
        }

        private static bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return (parameters.Length == 4
                && parameters[0].Name == "local"
                && parameters[1].Name == "weatherSettings"
                && parameters[2].Name == "botsSettings"
                && parameters[3].Name == "wavesSettings"
                && parameters[0].ParameterType == typeof(bool));
        }
    }
}
