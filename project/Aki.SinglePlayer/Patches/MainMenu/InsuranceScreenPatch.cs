using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT;
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
            return typeof(MainMenuController).GetMethod("method_62", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPrefix]
        private static void PrefixPatch(RaidSettings ___raidSettings_0)
        {
            ___raidSettings_0.RaidMode = ERaidMode.Online;
        }

        [PatchPostfix]
        private static void PostfixPatch(RaidSettings ___raidSettings_0)
        {
            ___raidSettings_0.RaidMode = ERaidMode.Local;
        }
    }
}
