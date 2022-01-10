using Aki.Common.Http;
using Aki.Common.Utils;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using Aki.SinglePlayer.Models.Progression;
using Aki.SinglePlayer.Utils.Progression;
using Comfort.Common;
using EFT;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.Progression
{
    public class OfflineSaveProfilePatch : ModulePatch
    {
        private static readonly JsonConverter[] _defaultJsonConverters;

        static OfflineSaveProfilePatch()
        {
            _ = nameof(ClientMetrics.Metrics);

            var converterClass = typeof(AbstractGame).Assembly.GetTypes()
                .First(t => t.GetField("Converters", BindingFlags.Static | BindingFlags.Public) != null);

            _defaultJsonConverters = Traverse.Create(converterClass).Field<JsonConverter[]>("Converters").Value;
        }

        protected override MethodBase GetTargetMethod()
        {
            // method_45 - as of 16432
            var type = PatchConstants.EftTypes.Single(x => x.Name == "MainApplication");
            return Array.Find(type.GetMethods(PatchConstants.PrivateFlags), Match);
        }

        private bool Match(MethodInfo arg)
        {
            var parameters = arg.GetParameters();
            return parameters.Length > 5 && parameters[0]?.Name == "profileId" && parameters[1]?.Name == "savageProfile" && arg.ReturnType == typeof(void);
        }

        [PatchPrefix]
        private static void PatchPrefix(ESideType ___esideType_0, Result<ExitStatus, TimeSpan, ClientMetrics> result)
        {
            var session = PatchConstants.BackEndSession;

            SaveProfileRequest request = new SaveProfileRequest
			{
				Exit = result.Value0.ToString().ToLowerInvariant(),
				Profile = (___esideType_0 == ESideType.Savage) ? session.ProfileOfPet : session.Profile,
				Health = Utils.Healing.HealthListener.Instance.CurrentHealth,
				IsPlayerScav = (___esideType_0 == ESideType.Savage)
			};

			RequestHandler.PutJson("/raid/profile/save", request.ToJson(_defaultJsonConverters.AddItem(new NotesJsonConverter()).ToArray()));
        }
    }
}
