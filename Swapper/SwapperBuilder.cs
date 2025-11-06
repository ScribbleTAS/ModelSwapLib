using ModelSwapLib.Swapper.Modules;

namespace ModelSwapLib.Swapper;

public class SwapperBuilder
{
    private string ModName;
    private string SwapperName;
    private List<string> ObjectNames = new();
    private string BundleName;
    private List<IAssetModule> AssetModules;
    private List<ITransformModule> TransformModules;
    private List<string> Deactivations;

    public SwapperBuilder(string ModName)
    {
        this.ModName = ModName;
    }

    public SwapperBuilder()
    {
        
    }

    public SwapperBuilder SetModName(string ModName)
    {
        this.ModName = ModName;
        return this;
    }
    
    public SwapperBuilder SetSwapperName(string SwapperName)
    {
        this.SwapperName = SwapperName;
        return this;
    }

    public SwapperBuilder AddObjectName(string ObjectName)
    {
        this.ObjectNames.Add(ObjectName);
        return this;
    }

    public SwapperBuilder AddObjectNames(List<string> BundleNames)
    {
        this.ObjectNames.AddRange(BundleNames);
        return this;
    }

    public SwapperBuilder SetBundleName(string BundleName)
    {
        this.BundleName = BundleName;
        return this;
    }
    
    public SwapperBuilder AddAssetModule(IAssetModule assetModule)
    {
        if (AssetModules == null) AssetModules = new();
        this.AssetModules.Add(assetModule);
        return this;
    }

    public SwapperBuilder AddAssetModules(List<IAssetModule> assetModules)
    {
        if (AssetModules == null) AssetModules = new();
        this.AssetModules.AddRange(assetModules);
        return this;
    }

    public SwapperBuilder AddTransformModule(ITransformModule transformModule)
    {
        if (TransformModules == null) TransformModules = new();
        this.TransformModules.Add(transformModule);
        return this;
    }

    public SwapperBuilder AddTransformModules(List<ITransformModule> transformModules)
    {
        if (TransformModules == null) TransformModules = new();
        this.TransformModules.AddRange(transformModules);
        return this;
    }

    public SwapperBuilder AddDeactivation(string deactivation)
    {
        if (this.Deactivations == null) this.Deactivations = new();
        this.Deactivations.Add(deactivation);
        return this;
    }

    public SwapperBuilder AddDeactivations(List<string> deactivations)
    {
        if (this.Deactivations == null) this.Deactivations = new();
        this.Deactivations.AddRange(deactivations);
        return this;
    }

    public Swapper Build()
    {
        if (ModName == null) return null; // Required
        if(ObjectNames == null || ObjectNames.Count == 0) return null; // Without this, useless swapper
        if(BundleName == null && (AssetModules != null && AssetModules.Count > 0)) return null; // With no bundle name, there is no way to access the assets for the AssetModules
        if((AssetModules == null || AssetModules.Count == 0) 
           && (TransformModules == null || TransformModules.Count == 0)
           && (Deactivations == null || Deactivations.Count == 0)) return null; // All Actions are empty and this is a useless swapper
        
        Swapper swapper = new Swapper(
            ModName, 
            SwapperName, 
            ObjectNames, 
            BundleName, 
            AssetModules, 
            TransformModules, 
            Deactivations);
        return swapper;
    }
}