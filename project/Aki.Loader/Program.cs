using Aki.Common.Utils;

namespace Aki.Loader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Info("Loading: Aki.Loader");

            Loader.AddRepository(VFS.Combine(VFS.Cwd, "Aki_Data/Modules/"));
            Loader.AddRepository(VFS.Combine(VFS.Cwd, "user/mods/"));
            Loader.LoadAllAssemblies();
        }
    }
}
