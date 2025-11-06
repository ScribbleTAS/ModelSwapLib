using ModelSwapLib.Swapper.Modules;

namespace ModelSwapLib.Swapper;

public class SwapperBuilder
{
    private string ModName;
    private string SwapperName;
    private List<string> ObjectNames = new();
    private string BundleName;
    private List<IAssetModule> AssetModules = new();
    private List<ITransformModule> TransformModules = new();
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
        this.AssetModules.Add(assetModule);
        return this;
    }

    public SwapperBuilder AddAssetModules(List<IAssetModule> assetModules)
    {
        this.AssetModules.AddRange(assetModules);
        return this;
    }

    public SwapperBuilder AddTransformModule(ITransformModule transformModule)
    {
        this.TransformModules.Add(transformModule);
        return this;
    }

    public SwapperBuilder AddTransformModules(List<ITransformModule> transformModules)
    {
        this.TransformModules.AddRange(transformModules);
        return this;
    }

    public SwapperBuilder AddDeactivation(string Deactivation)
    {
        this.Deactivations.Add(Deactivation);
        return this;
    }

    public SwapperBuilder AddDeactivations(List<string> Deactivations)
    {
        this.Deactivations.AddRange(Deactivations);
        return this;
    }

    public Swapper Build()
    {
        return new Swapper(
            ModName, 
            SwapperName, 
            ObjectNames, 
            BundleName, 
            AssetModules, 
            TransformModules, 
            Deactivations);
    }
}