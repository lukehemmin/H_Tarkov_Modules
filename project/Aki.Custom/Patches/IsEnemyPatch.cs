using Aki.Common.Utils;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aki.Custom.Patches
{
    public class IsEnemyPatch : ModulePatch
    {
        private static Type _targetType;
        private static FieldInfo _sideField;
        private static FieldInfo _enemiesField;
        private readonly string _targetMethodName = "IsEnemy";

        public IsEnemyPatch()
        {
            _targetType = PatchConstants.EftTypes.Single(IsTargetType);
            _sideField = _targetType.GetField("Side");
            _enemiesField = _targetType.GetField("Enemies");
        }

        private bool IsTargetType(Type type)
        {
            if (type.GetMethod("AddEnemy") != null && type.GetMethod("AddEnemyGroupIfAllowed") != null)
            {
                Logger.LogInfo($"IsEnemyPatch: {type.FullName}");
                return true;
            }

            return false;
        }

        protected override MethodBase GetTargetMethod()
        {
            return _targetType.GetMethod(_targetMethodName);
        }

        /// <summary>
        /// IsEnemy()
        /// Goal: Make bots take botSide into account when deciding if another player/bot is an enemy
        /// Check enemy cache list first, if not found, choose a value
        /// Needed to ensure bot checks the enemy side, not just its botType
        /// </summary>
        [PatchPrefix]
        private static bool PatchPrefix(ref bool __result, object __instance, IAIDetails requester)
        {
            var side = (EPlayerSide)_sideField.GetValue(__instance);
            var enemies = (Dictionary<IAIDetails, BotSettingsClass>)_enemiesField.GetValue(__instance);

            var result = false; // default not an enemy
            if (enemies.Any(x=> x.Value.Player.Id == requester.Id))
            {
                result = true;
            }
            else
            {
                if (side == EPlayerSide.Usec)
                {
                    if (requester.Side == EPlayerSide.Usec)
                    {
                        result = true;
                    }
                }
                else if (side == EPlayerSide.Bear)
                {
                    if (requester.Side == EPlayerSide.Bear)
                    {
                        result = true;
                    }
                }
                else if (side == EPlayerSide.Savage)
                {
                    if (requester.Side == EPlayerSide.Savage)
                    {
                        result = false;
                    }
                    else
                    {
                        // everyone else is an enemy to savage (scavs)
                        result = true;
                    }
                }
                else // no matches found so no enemies
                {
                    result = false;
                }
            }

            __result = result;

            return false;
        }
    }
}
