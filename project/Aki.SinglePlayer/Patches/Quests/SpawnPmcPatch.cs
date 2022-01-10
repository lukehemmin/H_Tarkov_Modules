using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.Quests
{
    public class SpawnPmcPatch : ModulePatch
    {
        private static Type _targetInterface;
        private static Type _targetType;
        private static FieldInfo _wildSpawnTypeField;
        private static FieldInfo _botDifficultyField;

        static SpawnPmcPatch()
        {
            _targetInterface = PatchConstants.EftTypes.Single(IsTargetInterface);
            _targetType = PatchConstants.EftTypes.Single(IsTargetType);
            _wildSpawnTypeField = _targetType.GetField("wildSpawnType_0", PatchConstants.PrivateFlags);
            _botDifficultyField = _targetType.GetField("botDifficulty_0", PatchConstants.PrivateFlags);
        }

        private static bool IsTargetInterface(Type type)
        {
            return type.IsInterface && type.GetMethod("ChooseProfile", new[] { typeof(List<Profile>), typeof(bool) }) != null;
        }

        private static bool IsTargetType(Type type)
        {
            if (!_targetInterface.IsAssignableFrom(type) || type.GetMethod("method_1", PatchConstants.PrivateFlags) == null)
            {
                return false;
            }

            var fields = type.GetFields(PatchConstants.PrivateFlags);
            return fields.Any(f => f.FieldType != typeof(WildSpawnType)) && fields.Any(f => f.FieldType == typeof(BotDifficulty));
        }

        protected override MethodBase GetTargetMethod()
        {
            return _targetType.GetMethod("method_1", PatchConstants.PrivateFlags);
        }

        [PatchPrefix]
        private static bool PatchPrefix(ref bool __result, object __instance, Profile x)
        {
            var botType = (WildSpawnType)_wildSpawnTypeField.GetValue(__instance);
            var botDifficulty = (BotDifficulty)_botDifficultyField.GetValue(__instance);

            __result = x.Info.Settings.Role == botType && x.Info.Settings.BotDifficulty == botDifficulty;
            return false;
        }
    }
}
