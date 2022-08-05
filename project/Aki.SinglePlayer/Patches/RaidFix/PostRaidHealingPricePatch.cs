using Aki.Reflection.Patching;
using HarmonyLib;
using System;
using System.Reflection;
using TraderInfo = EFT.Profile.GClass1520;

namespace Aki.SinglePlayer.Patches.RaidFix
{
    public class PostRaidHealingPricePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(TraderInfo).GetMethod("UpdateLevel", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPrefix]
        protected static void PatchPrefix(TraderInfo __instance)
        {
            if (__instance.Settings == null)
            {
                return;
            }

            var loyaltyLevel = __instance.Settings.GetLoyaltyLevel(__instance);
            var loyaltyLevelSettings = __instance.Settings.GetLoyaltyLevelSettings(loyaltyLevel);

            if (loyaltyLevelSettings == null)
            {
                throw new IndexOutOfRangeException($"Loyalty level {loyaltyLevel} not found.");
            }

            // Backing field of the "CurrentLoyalty" property
            Traverse.Create(__instance).Field("traderLoyaltyLevel_0").SetValue(loyaltyLevelSettings.Value);
        }
    }
}