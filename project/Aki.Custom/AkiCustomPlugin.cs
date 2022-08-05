using Aki.Custom.Patches;
using BepInEx;

namespace Aki.Custom
{
    [BepInPlugin("com.spt-aki.custom", "AKI.Custom", "1.0.0")]
    class AkiCustomPlugin : BaseUnityPlugin
    {
        public AkiCustomPlugin()
        {
            Logger.LogInfo("Loading: Aki.Custom");

            new BossSpawnChancePatch().Enable();
            new BotDifficultyPatch().Enable();
            new CoreDifficultyPatch().Enable();
            new OfflineRaidMenuPatch().Enable();
            new SessionIdPatch().Enable();
            new VersionLabelPatch().Enable();
            new IsEnemyPatch().Enable();
            new AddEnemyPatch().Enable();
            new CheckAndAddEnemyPatch().Enable();
            new BotSelfEnemyPatch().Enable();
            //new AddEnemyToAllGroupsInBotZonePatch().Enable();
            //new AirdropBoxPatch().Enable();
            //new AirdropPatch().Enable();
        }
    }
}
