using MelonLoader;
using ModelSwapLib.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

[assembly: MelonInfo(typeof(ModelSwapLib.Core), "ModelSwapLib", "1.0.1", "CocoPopEater", null)]
[assembly: MelonGame("Keepsake Games", "Jump Space")]
[assembly: MelonColor(255,0,255,0)]

namespace ModelSwapLib
{
    public class Core : MelonMod
    {
        private static float reloadMessageStart = -1f;
        
        public override void OnInitializeMelon()
        {
            BundleManager.GetInstance().InitializeBundles(); // Ensure BundleManager and bundles have been initialized
           ConsoleUtils.Msg("Initialized");
        }
        
        public override void OnDeinitializeMelon()
        {
            BundleManager.GetInstance().Shutdown(); // Ensure BundleManager unloads all cached bundles and clears the dict
        }
        
        public override void OnLateUpdate()
        {
            if (Keyboard.current != null && Keyboard.current.f5Key.wasPressedThisFrame)
            {
                ConsoleUtils.Msg("Manual reload triggered.");
                reloadMessageStart = Time.time;
                MelonEvents.OnGUI.Subscribe(DrawReloadText, 100);
            }
        }

        public static void DrawReloadText()
        {
            if (reloadMessageStart > 0 && Time.time - reloadMessageStart < 3f)
            {
                GUI.Label(new Rect(20, 20, 500, 50),
                    "<b><color=black><size=30>Reloading...</size></color></b>");
            }
            else
            {
                MelonEvents.OnGUI.Unsubscribe(DrawReloadText);
            }
        }
    }
}