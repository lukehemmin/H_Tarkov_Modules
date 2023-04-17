﻿using System;
using Aki.Debugging.Patches;
using BepInEx;

namespace Aki.Debugging
{
    [BepInPlugin("com.spt-aki.debugging", "AKI.Debugging", "1.0.0")]
    public class AkiDebuggingPlugin : BaseUnityPlugin
    {
        public AkiDebuggingPlugin()
        {
            Logger.LogInfo("Loading: Aki.Debugging");

            try
            {
                // new CoordinatesPatch().Enable();
            }
            catch (Exception ex)
            {
                Logger.LogError($"{GetType().Name}: {ex}");
                throw;
            }

            Logger.LogInfo("Completed: Aki.Debugging");
        }
    }
}
