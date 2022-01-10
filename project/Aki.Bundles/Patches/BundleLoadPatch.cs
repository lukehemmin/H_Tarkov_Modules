using Aki.Bundles.Utils;
using Aki.Common.Http;
using Aki.Common.Utils;
using Aki.Reflection.Patching;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aki.Bundles.Patches
{
    public class BundleLoadPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return EasyBundleHelper.Type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(IsTargetMethod);
        }

        private static bool IsTargetMethod(MethodInfo method)
        {
            return method.GetParameters().Length == 0 && method.ReturnType == typeof(Task);
        }

        [PatchPrefix]
        private static bool PatchPrefix(object __instance, string ___string_1, ref Task __result)
        {
            if (___string_1.IndexOf("http") == -1)
            {
                return true;
            }

            __result = LoadBundleFromServer(__instance);
            return false;
        }

        private static async Task LoadBundleFromServer(object instance)
        {
            var easyBundle = new EasyBundleHelper(instance);
            var path = easyBundle.Path;
            var bundleKey = Regex.Split(path, "bundle/", RegexOptions.IgnoreCase)[1];
            var cachePath = BundleSettings.CachePath;
            var filepath = cachePath + bundleKey;

            if (path.Contains("http"))
            {
                var data = RequestHandler.GetData(path);

                if (data != null)
                {
                    VFS.WriteFile(filepath, data);
                    easyBundle.Path = filepath;
                }
            }

            await easyBundle.LoadingCoroutine();
        }
    }
}
