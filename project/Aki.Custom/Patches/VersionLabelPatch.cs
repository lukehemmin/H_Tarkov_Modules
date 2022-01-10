using Aki.Common.Http;
using Aki.Common.Utils;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using Aki.Custom.Models;
using EFT.UI;
using HarmonyLib;
using System.Linq;
using System.Reflection;

namespace Aki.Custom.Patches
{
    public class VersionLabelPatch : ModulePatch
    {
        private static string _versionLabel;

        protected override MethodBase GetTargetMethod()
        {
            return PatchConstants.EftTypes
                .Single(x => x.GetField("Taxonomy", BindingFlags.Public | BindingFlags.Instance) != null)
                .GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
        }

        [PatchPostfix]
        private static void PatchPostfix(object __result)
        {
            if (string.IsNullOrEmpty(_versionLabel))
            {
                var json = RequestHandler.GetJson("/singleplayer/settings/version");
                _versionLabel = Json.Deserialize<VersionResponse>(json).Version;
                Log.Info($"Server version: {_versionLabel}");
            }

            Traverse.Create(MonoBehaviourSingleton<PreloaderUI>.Instance).Field("_alphaVersionLabel").Property("LocalizationKey").SetValue("{0}");
            Traverse.Create(MonoBehaviourSingleton<PreloaderUI>.Instance).Field("string_1").SetValue(_versionLabel);
            Traverse.Create(__result).Field("Major").SetValue(_versionLabel);
        }
    }
}