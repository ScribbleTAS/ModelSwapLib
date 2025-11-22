using ModelSwapLib.Managers;
using ModelSwapLib.ObjectTracking;
using ModelSwapLib.Swapper.Modules;
using UnityEngine;

namespace ModelSwapLib.Swapper;

public class Swapper
{
    public string ModName { get; set; }
    public string SwapperName { get; set; } = "Default Name";
    public List<string> ObjectNames { get; set; }
    public string BundleName { get; set; }
    public List<IAssetModule> AssetModules { get; set; }
    public List<ITransformModule> TransformModules { get; set; }
    public List<string> Deactivations { get; set; }
    public Guid SwapperGuid { get; internal set; }

    public Swapper(string modName,
        string swapperName,
        List<string> objectNames,
        string bundleName,
        List<IAssetModule> assetModules,
        List<ITransformModule> transformModules,
        List<string> deactivations)
    {
        ModName = modName;
        SwapperName = swapperName;
        ObjectNames = objectNames;
        BundleName = bundleName;
        AssetModules = assetModules;
        TransformModules = transformModules;
        Deactivations = deactivations;
    }

    public Swapper()
    {
        
    }

    internal void RunAllModules(GameObject target)
    {
        if (!target) return;
        
        if (AssetModules != null)
        {
            var bundle = BundleManager.GetInstance().GetBundle(this);
            if (bundle == null)
            {
                ConsoleUtils.Error($"Failed to load AssetBundle from Mod:Swapper : {ModName}:{SwapperName}");
            }
            else
            {
                foreach (IAssetModule module in AssetModules)
                {
                    module.Apply(target, bundle);
                }
            }
        }

        if (TransformModules != null)
        {
            foreach (ITransformModule module in TransformModules)
            {
                module.Apply(target);
            } 
        }
    }

    internal void DeactivateObjects()
    {
        // Handle Deactivations
        if (Deactivations == null || Deactivations.Count == 0) return; // There are no objects to deactivate, just return
        
        foreach (GameObject obj in TrackingManager.GetObjectsFromNames(Deactivations))
        {
            obj.SetActive(false);
        }
    }
    
    internal Guid GenerateSwapperGuid()
    {
        if (SwapperGuid == Guid.Empty)
        {
            SwapperGuid = Guid.NewGuid();
        }
        return SwapperGuid;
    }
    
    public bool Validate()
    {
        if(string.IsNullOrEmpty(ModName)) return false;
        if(ObjectNames == null || ObjectNames.Count == 0) return false;
        
        if (AssetModules != null && AssetModules.Count > 0)
        {
            if(BundleName == null) return false;
            if(!BundleName.EndsWith(".bundle")) BundleName = string.Concat(BundleName, ".bundle");
        }
        else if(TransformModules == null || TransformModules.Count == 0)
        {
            return false; 
        } else if (Deactivations == null || Deactivations.Count == 0)
        {
            return false; // AssetModules, TransformModules and Deactivations are null or empty// and this is a pointless swapper
        }
        
        GenerateSwapperGuid();
        return true;
    }
}