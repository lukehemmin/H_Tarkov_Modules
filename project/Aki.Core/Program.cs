using Aki.Common.Utils;
using Aki.Core.Patches;

namespace Aki.Core
{
	class Program
	{
		static void Main(string[] args)
		{
            Log.Info("Loading: Aki.Core");

            new ConsistencySinglePatch().Enable();
            new ConsistencyMultiPatch().Enable();
            new BattlEyePatch().Enable();
            new SslCertificatePatch().Enable();
            new UnityWebRequestPatch().Enable();
            new WebSocketPatch().Enable();
        }
	}
}
