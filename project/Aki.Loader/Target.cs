using System;
using System.Linq;
using NLog.Targets;

namespace Aki.Loader
{
	[Target("Aki.Loader")]
	public sealed class Target : TargetWithLayout
	{
		public Target()
		{
            // Hacky workaround to prevent infinite loader looping
            // Game exit triggers logging, which would cause this logging target to be initialized endlessly
            if (AppDomain.CurrentDomain.GetAssemblies().Any(x => x.FullName.StartsWith("aki-core")))
            {
                return;
            }

            Program.Main(null);
        }
    }
}
