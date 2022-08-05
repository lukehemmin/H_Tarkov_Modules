using Aki.Common.Http;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System.Linq;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.RaidFix
{
    class MaxBotPatch : ModulePatch 
    {
        protected override MethodBase GetTargetMethod()
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            var methodName = "SetSettings";
            return PatchConstants.EftTypes.Single(x => x.GetMethod(methodName, flags) != null && IsTargetMethod(x.GetMethod(methodName, flags)))
                .GetMethod(methodName, flags);
        }
        
        private static bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return parameters.Length == 3
                && parameters[0].Name == "maxCount"
                && parameters[1].Name == "botPresets"
                && parameters[2].Name == "botScatterings";
        }

        [PatchPrefix]
        private static void PatchPreFix(ref int maxCount)
        {
            var json = RequestHandler.GetJson("/singleplayer/settings/bot/maxCap");
            var isParsable = int.TryParse(json, out maxCount);
            maxCount = isParsable ? maxCount : 20;
        }
    }
}