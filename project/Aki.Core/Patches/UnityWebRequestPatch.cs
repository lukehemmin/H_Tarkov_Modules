using System.Reflection;
using UnityEngine.Networking;
using Aki.Reflection.Patching;
using Aki.Core.Models;

namespace Aki.Core.Patches
{
    public class UnityWebRequestPatch : ModulePatch
    {
        private static CertificateHandler _certificateHandler = new FakeCertificateHandler();

        protected override MethodBase GetTargetMethod()
        {
            return typeof(UnityWebRequestTexture).GetMethod(nameof(UnityWebRequestTexture.GetTexture), new[] { typeof(string) });
        }

        [PatchPostfix]
        private static void PatchPostfix(UnityWebRequest __result)
        {
            __result.certificateHandler = _certificateHandler;
            __result.disposeCertificateHandlerOnDispose = false;
            __result.timeout = 1000;
        }
    }
}
