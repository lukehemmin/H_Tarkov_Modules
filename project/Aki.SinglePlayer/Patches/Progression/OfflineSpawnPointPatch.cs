using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT;
using EFT.Game.Spawning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.Progression
{
    public class OfflineSpawnPointPatch : ModulePatch
    {
        static OfflineSpawnPointPatch()
        {
            _ = nameof(ISpawnPoints.CreateSpawnPoint);
        }

        protected override MethodBase GetTargetMethod()
        {
            var desiredType = PatchConstants.EftTypes.First(IsTargetType);
            var desiredMethod = desiredType
                .GetMethods(PatchConstants.PrivateFlags)
                .First(m => m.Name.Contains("SelectSpawnPoint"));

            Logger.LogDebug($"{this.GetType().Name} Type: {desiredType?.Name}");
            Logger.LogDebug($"{this.GetType().Name} Method: {desiredMethod?.Name}");

            return desiredMethod;
        }

        private static bool IsTargetType(Type type)
        {
            // GClass1812 as of 17349
            // GClass1886 as of 18876
            return (type.GetMethods(PatchConstants.PrivateFlags).Any(x => x.Name.IndexOf("CheckFarthestFromOtherPlayers", StringComparison.OrdinalIgnoreCase) != -1)
                && type.IsClass);
        }

        [PatchPrefix]
        private static bool PatchPrefix(
            ref ISpawnPoint __result,
            object __instance,
            ESpawnCategory category,
            EPlayerSide side,
            string groupId,
            IAIDetails person,
            string infiltration)
        {
            var spawnPointsField = (ISpawnPoints)__instance.GetType().GetFields(PatchConstants.PrivateFlags).SingleOrDefault(f => f.FieldType == typeof(ISpawnPoints))?.GetValue(__instance);
            if (spawnPointsField == null)
            {
                throw new Exception($"OfflineSpawnPointPatch: Failed to locate field of {nameof(ISpawnPoints)} on class instance ({__instance.GetType().Name})");
            }

            var mapSpawnPoints = spawnPointsField.ToList();
            var unfilteredFilteredSpawnPoints = mapSpawnPoints.ToList();

            // filter by e.g. 'Boiler Tanks' (always seems to be map name?)
            if (!string.IsNullOrEmpty(infiltration))
            {
                mapSpawnPoints = mapSpawnPoints.Where(sp => sp?.Infiltration != null && (string.IsNullOrEmpty(infiltration) || sp.Infiltration.Equals(infiltration))).ToList();
            }

            mapSpawnPoints = FilterByPlayerSide(mapSpawnPoints, category, side);

            __result = mapSpawnPoints.Count == 0
                    ? GetFallBackSpawnPoint(unfilteredFilteredSpawnPoints, category, side, infiltration)
                    : mapSpawnPoints.RandomElement();

            Logger.LogInfo($"Desired spawnpoint: [{category}] [{side}] [{infiltration}]");
            Logger.LogInfo($"PatchPrefix SelectSpawnPoint: [{__result.Id}] [{__result.Name}] [{__result.Categories}] [{__result.Sides}] [{__result.Infiltration}]");
            return false;
        }

        private static List<ISpawnPoint> FilterByPlayerSide(List<ISpawnPoint> mapSpawnPoints, ESpawnCategory category, EPlayerSide side)
        {
            // Filter by category 'player' and by side ('usec', 'bear')
            return mapSpawnPoints.Where(sp => sp.Categories.Contain(category) && sp.Sides.Contain(side)).ToList();
        }

        private static ISpawnPoint GetFallBackSpawnPoint(List<ISpawnPoint> spawnPoints, ESpawnCategory category, EPlayerSide side, string infiltration)
        {
            Logger.LogWarning($"PatchPrefix SelectSpawnPoint: Couldn't find any spawn points for: {category} | {side} | {infiltration} using random filtered spawn instead");
            return spawnPoints.Where(sp => sp.Categories.Contain(ESpawnCategory.Player)).RandomElement();
        }
    }
}
