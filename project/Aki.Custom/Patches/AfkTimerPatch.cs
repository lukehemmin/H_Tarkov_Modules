using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System.Linq;
using System.Reflection;

namespace Aki.Custom.Patches
{
    public class AfkTimerPatch : ModulePatch
    {
        private const float AfkTimeOut = 7 * 24 * 60 * 60; // 1 week

        protected override MethodBase GetTargetMethod()
        {
            return PatchConstants.EftTypes.Single(x => x.Name == "MainApplication")
                 .GetMethods(PatchConstants.PrivateFlags)
                 .SingleOrDefault(IsTargetMethod);
        }

        private static bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return parameters.Length == 1
                && parameters[0].Name == "afkTimeOut";
        }

        [PatchPrefix]
        private static void PatchPrefix(ref float afkTimeOut)
        {
            afkTimeOut = AfkTimeOut;
        }
    }
}   
