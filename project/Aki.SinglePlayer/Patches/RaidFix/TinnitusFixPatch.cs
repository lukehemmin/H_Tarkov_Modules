using Aki.Common.Utils;
using Aki.Reflection.Patching;
using EFT;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Aki.SinglePlayer.Patches.RaidFix
{
    public class TinnitusFixPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("OnHealthEffectAdded", BindingFlags.Instance | BindingFlags.Public);
        }

        /// <summary>
        /// Fixes the problem of the tinnitus sound effect being played on the player if any AI on the map get the contusion effect applied to them
        /// The patch adds an extra condition to the check before playing the sound effect, making sure the sound is only played if contusion occurred on the player
        /// </summary>
        [PatchTranspiler]
        private static IEnumerable<CodeInstruction> PatchTranspile(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var searchCode = new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(BetterAudio), "StartTinnitusEffect"));

            // Locate the reference instruction from which we can locate all the other relevant instructions
            var searchIndex = -1;
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == searchCode.opcode && codes[i].operand == searchCode.operand)
                {
                    searchIndex = i;
                    break;
                }
            }

            if (searchIndex == -1)
            {
                Log.Error($"Patch {nameof(TinnitusFixPatch)} failed: Could not find reference code.");
                return instructions;
            }

            // The next instruction after our reference point should be a 'br' with the condition exit label
            if (codes[searchIndex + 1].opcode != OpCodes.Br)
            {
                Log.Error($"Patch {nameof(TinnitusFixPatch)} failed: Could not locate 'br' instruction");
                return instructions;
            }

            // We grab the target label that we can use to exit the condition if it's not satisfied
            var skipLabel = (Label)codes[searchIndex + 1].operand;

            // Locate the index at which our instructions should be inserted
            var insertIndex = -1;
            for (var i = searchIndex; i > searchIndex - 10; i--)
            {
                if (codes[i].opcode == OpCodes.Brtrue)
                {
                    insertIndex = i + 1;
                    break;
                }
            }

            if (insertIndex == -1)
            {
                Log.Error($"Patch {nameof(TinnitusFixPatch)} failed: Could not find instruction insert location.");
            }

            // Add a new condition that checks if your player is the one who has the contusion effect applied
            var newCodes = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Player), "get_IsYourPlayer")),
                new CodeInstruction(OpCodes.Brfalse, skipLabel)
            };

            /* Original code line:
             * else if (@class.effect is GInterface157 && !(this.Equipment.GetSlot(EquipmentSlot.Earpiece).ContainedItem is GClass1707))
             *
             * Updated code line:
             * else if (@class.effect is GInterface157 && !(this.Equipment.GetSlot(EquipmentSlot.Earpiece).ContainedItem is GClass1707) && this.IsYourPlayer)
             */

            codes.InsertRange(insertIndex, newCodes);

            return codes;
        }
    }
}