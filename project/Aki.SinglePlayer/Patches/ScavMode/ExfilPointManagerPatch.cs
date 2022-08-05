using Aki.Reflection.Patching;
using Comfort.Common;
using EFT;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.ScavMode
{
    public class ExfilPointManagerPatch : ModulePatch
    {   

        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod("OnGameStarted", BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPostfix]
        public static void PatchPostFix()
        {
            var gameWorld = Singleton<GameWorld>.Instance;

            // checks nothing is null otherwise woopsies happen.
            if (gameWorld == null || gameWorld.RegisteredPlayers == null || gameWorld.ExfiltrationController == null)
            {
                Logger.LogError("Unable to Find Gameworld or RegisterPlayers... Can't Disable Extracts for Scav raid");
            }

            // One of the RegisteredPlayers will have the IsYourPlayer flag set, which will be our own Player instance.
            Player player = gameWorld.RegisteredPlayers.Find(p => p.IsYourPlayer);

            // gets exfiltrationController from the gameworld
            var exfilController = gameWorld.ExfiltrationController;

            // only disables PMC extracts if current player is a scav.
            if (player.Fraction == ETagStatus.Scav && player.Location != "hideout")
            {
                // these are PMC extracts only, scav extracts are under a different field called ScavExfiltrationPoints.
                foreach (var exfil in exfilController.ExfiltrationPoints)
                {
                    exfil.Disable();
                }
            }
        }
    }
}
