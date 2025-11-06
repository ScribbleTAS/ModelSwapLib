using MelonLoader;
using ModelSwapLib.Managers;

[assembly: MelonInfo(typeof(ModelSwapLib.Core), "ModelSwapLib", "1.2.0", "CocoPopEater", null)]
[assembly: MelonGame("Keepsake Games", "Jump Space")]
[assembly: MelonColor(255,0,255,0)]

namespace ModelSwapLib
{
    public class Core : MelonMod
    {
        
        public override void OnInitializeMelon()
        { 
            BundleManager.GetInstance().InitializeBundles(); // Ensure BundleManager and bundles have been initialized
            ConsoleUtils.Msg("Initialized");
        }
        
        public override void OnDeinitializeMelon()
        {
            BundleManager.GetInstance().Shutdown(); // Ensure BundleManager unloads all cached bundles and clears the dict
        }
    }
}