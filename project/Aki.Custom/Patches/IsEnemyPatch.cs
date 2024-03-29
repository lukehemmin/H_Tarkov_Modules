﻿using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT;
using System;
using System.Linq;
using System.Reflection;

namespace Aki.Custom.Patches
{
    public class IsEnemyPatch : ModulePatch
    {
        private static Type _targetType;
        private readonly string _targetMethodName = "IsEnemy";

        public IsEnemyPatch()
        {
            _targetType = PatchConstants.EftTypes.Single(IsTargetType);
        }

        private bool IsTargetType(Type type)
        {
            if (type.GetMethod("AddEnemy") != null && type.GetMethod("AddEnemyGroupIfAllowed") != null)
            {
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
        /// Goal: Make bots take Side into account when deciding if another player/bot is an enemy
        /// Check enemy cache list first, if not found, check side, if they differ, add to enemy list and return true
        /// Needed to ensure bot checks the enemy side, not just its botType
        /// </summary>
        [PatchPrefix]
        private static bool PatchPrefix(ref bool __result, BotGroupClass __instance, IAIDetails requester)
        {
            var isEnemy = false; // default not an enemy
            
            // Check existing enemies list
            if (__instance.Enemies.Any(x=> x.Value.Player.Id == requester.Id))
            {
                isEnemy = true;
            }
            else
            {
                if (__instance.Side == EPlayerSide.Usec)
                {
                    if (requester.Side == EPlayerSide.Bear || requester.Side == EPlayerSide.Savage ||
                        ShouldAttackUsec(requester))
                    {
                        isEnemy = true;
                        __instance.AddEnemy(requester);
                    }
                }
                else if (__instance.Side == EPlayerSide.Bear)
                {
                    if (requester.Side == EPlayerSide.Usec || requester.Side == EPlayerSide.Savage ||
                        ShouldAttackBear(requester))
                    {
                        isEnemy = true;
                        __instance.AddEnemy(requester);
                    }
                }
                else if (__instance.Side == EPlayerSide.Savage)
                {
                    if (requester.Side != EPlayerSide.Savage)
                    {
                        // everyone else is an enemy to savage (scavs)
                        isEnemy = true;
                        __instance.AddEnemy(requester);
                    }
                }
            }

            __result = isEnemy;

            return true; // Skip original
        }

        /// <summary>
        /// Return True when usec default behavior is attack + bot is usec
        /// </summary>
        /// <param name="requester"></param>
        /// <returns>bool</returns>
        private static bool ShouldAttackUsec(IAIDetails requester)
        {
            var requesterMind = requester?.AIData?.BotOwner?.Settings?.FileSettings?.Mind;

            if (requesterMind == null)
            {
                return false;
            }

            return requester.IsAI && requesterMind.DEFAULT_USEC_BEHAVIOUR == EWarnBehaviour.Attack && requester.Side == EPlayerSide.Usec;
        }

        /// <summary>
        /// Return True when bear default behavior is attack + bot is bear
        /// </summary>
        /// <param name="requester"></param>
        /// <returns></returns>
        private static bool ShouldAttackBear(IAIDetails requester)
        {
            var requesterMind = requester.AIData?.BotOwner?.Settings?.FileSettings?.Mind;

            if (requesterMind == null)
            {
                return false;
            }

            return requester.IsAI && requesterMind.DEFAULT_BEAR_BEHAVIOUR == EWarnBehaviour.Attack && requester.Side == EPlayerSide.Bear;
        }
    }
}
