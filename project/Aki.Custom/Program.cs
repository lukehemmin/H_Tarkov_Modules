using Aki.Common.Utils;
using Aki.Custom.Patches;

namespace Aki.Custom
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Info("Loading: Aki.Custom");

            new BossSpawnChancePatch().Enable();
            new BotDifficultyPatch().Enable();
            new CoreDifficultyPatch().Enable();
            new OfflineRaidMenuPatch().Enable();
            new SessionIdPatch().Enable();
            new VersionLabelPatch().Enable();
            new IsEnemyPatch().Enable();
            new AfkTimerPatch().Enable();
            new BotEnemyTargetPatch().Enable();
        }
    }
}
