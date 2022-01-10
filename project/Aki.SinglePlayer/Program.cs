using Aki.Common.Utils;
using Aki.SinglePlayer.Patches.Healing;
using Aki.SinglePlayer.Patches.MainMenu;
using Aki.SinglePlayer.Patches.Progression;
using Aki.SinglePlayer.Patches.Quests;
using Aki.SinglePlayer.Patches.RaidFix;
using Aki.SinglePlayer.Patches.ScavMode;

namespace Aki.SinglePlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Info("Loading: Aki.SinglePlayer");

            new OfflineSaveProfilePatch().Enable();
            new OfflineSpawnPointPatch().Enable();
            new ExperienceGainPatch().Enable();
            new MainMenuControllerPatch().Enable();
            new PlayerPatch().Enable();
            new SelectLocationScreenPatch().Enable();
            new InsuranceScreenPatch().Enable();
            new BotTemplateLimitPatch().Enable();
            new GetNewBotTemplatesPatch().Enable();
            new RemoveUsedBotProfilePatch().Enable();
            new DogtagPatch().Enable();
            new LoadOfflineRaidScreenPatch().Enable();
            new ScavPrefabLoadPatch().Enable();
            new ScavProfileLoadPatch().Enable();
            new ScavExfilPatch().Enable();
            new ExfilPointManagerPatch().Enable();
            new TinnitusFixPatch().Enable();
            new MaxBotPatch().Enable();
            new SpawnPmcPatch().Enable();
            
        }
    }
}
