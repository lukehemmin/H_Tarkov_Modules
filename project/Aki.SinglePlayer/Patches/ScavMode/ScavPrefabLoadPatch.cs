using Aki.Reflection.CodeWrapper;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Aki.SinglePlayer.Patches.ScavMode
{
    public class ScavPrefabLoadPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainApplication)
                .GetNestedTypes(PatchConstants.PrivateFlags)
                .Single(x => x.GetField("timeAndWeather") != null
                              && x.GetField("mainApplication_0") != null
                              && x.GetField("timeHasComeScreenController") == null
                              && x.Name.Contains("Struct"))
                .GetMethods(PatchConstants.PrivateFlags)
                .FirstOrDefault(x => x.Name == "MoveNext");
        }

        [PatchTranspiler]
        private static IEnumerable<CodeInstruction> PatchTranspile(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            // Search for code where backend.Session.getProfile() is called.
            var searchCode = new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(PatchConstants.SessionInterfaceType, "get_Profile"));
            var searchIndex = -1;

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == searchCode.opcode && codes[i].operand == searchCode.operand)
                {
                    searchIndex = i;
                    break;
                }
            }

            // Patch failed.
            if (searchIndex == -1)
            {
                Logger.LogError(string.Format("Patch {0} failed: Could not find reference code.", MethodBase.GetCurrentMethod()));
                return instructions;
            }

            // Move back by 3. This is the start of IL chain that we're interested in.
            searchIndex -= 3;

            var brFalseLabel = generator.DefineLabel();
            var brLabel = generator.DefineLabel();

            var newCodes = CodeGenerator.GenerateInstructions(new List<Code>()
            {
                new Code(OpCodes.Ldloc_1),
                new Code(OpCodes.Ldfld, typeof(ClientApplication), "_backEnd"),
                new Code(OpCodes.Callvirt, PatchConstants.BackendInterfaceType, "get_Session"),
                new Code(OpCodes.Ldloc_1),
                new Code(OpCodes.Ldfld, typeof(MainApplication), "_raidSettings"),
                new Code(OpCodes.Callvirt, typeof(RaidSettings), "get_IsPmc"),
                new Code(OpCodes.Brfalse, brFalseLabel),
                new Code(OpCodes.Callvirt, PatchConstants.SessionInterfaceType, "get_Profile"),
                new Code(OpCodes.Br, brLabel),
                new CodeWithLabel(OpCodes.Callvirt, brFalseLabel, PatchConstants.SessionInterfaceType, "get_ProfileOfPet"),
                new CodeWithLabel(OpCodes.Ldc_I4_1, brLabel)
            });

            codes.RemoveRange(searchIndex, 5);
            codes.InsertRange(searchIndex, newCodes);
            return codes.AsEnumerable();
        }
    }
}
