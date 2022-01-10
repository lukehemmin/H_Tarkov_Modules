using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT.UI;
using EFT.UI.Matchmaker;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.MainMenu
{
    public class SelectLocationScreenPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(MatchMakerSelectionLocationScreen).GetMethod("Awake", PatchConstants.PrivateFlags);
        }

        [PatchPostfix]
        private static void PatchPostfix(DefaultUIButton ____readyButton)
        {
            ____readyButton.Interactable = false;
        }
    }
}
