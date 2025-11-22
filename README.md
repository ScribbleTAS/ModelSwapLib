# Model Swap Lib

Model Swap Lib is an abstraction originally based on [this Asset Swap Template](https://github.com/SamGarratt17/ModelSwapTemplate-JumpSpace/tree/master) for the game *Jump Space*.

This library is intended for use by mod developers and will have no effect if installed alone.

# Usage Steps:
1. Add `ModelSwapLib.dll` as a project reference
2. Include `[assembly: MelonAdditionalDependencies("ModelSwapLib")]` in your core file
3. Instantiate a swapper using one of the options below
4. Register your swapper with `ObjectActionManager.GetInstance().RegisterSwapper(swapper);` - This will return a populated Guid if this succeeded, or a Guid.Empty if ModelSwapLib was unable to validate the Swapper structure

There are 2 ways to create a swapper:

## Option 1 - Builder Pattern:
```csharp
Swapper swapper = new SwapperBuilder("Optional ModName")
    .SetModName("YourModName")
    .SetSwapperName("Any Name You Want")
    .SetBundleName("YourBundle.bundle")
    .AddObjectName("ObjectName")
    .AddAssetModule(new MeshModule("Path/To/Your/Model.fbx"))
    .AddAssetModule(new Texture2DModule("Path/To/Your/Texture.png"))
    .AddTransformModule(new MoveMeshModule(0f, 0f, 0f))
    .AddTransformModule(new RotateMeshModule(0f, 0f, 0f))
    .AddTransformModule(new ScaleMeshModule(0f, 0f, 0f))
    .AddDeactivation("ObjectYouWantDeactivated")
    .Build();
```
new Swapper() - The string parameter for ModName is optional here but is required at some point<br>

SetModName(string) - ModName is a required parameter for a Swapper<br>

SetSwapperName(string) - SwapperName is not required and will default to "Unnamed Swapper", this is mainly for logging<br>

SetBundleName(string) - BundleName is only required if you use an IAssetModule<br>

AddObjectName(string) - Swapper must contain at least 1 object name, otherwise it will not apply to anything and will be rejected<br>

AddAssetModule(IAssetModule) - List of modules that use an asset from a provided AssetBundle<br>

AddTransformModule(ITransformModule) - List of modules that manipulate the transform of a mesh<br>

AddDeactivation(string) - List of object names to be deactivated

Additional methods include:<br>
- AddObjectNames(List&lt;string&gt;)
- AddAssetModules(List&lt;IAssetModule&gt;)
- AddTransformModules(List&lt;ITransformModule&gt;)
- AddDeactivations(List&lt;string&gt;)

Build() - Returns the swapper object, or null if the configuration is not valid

## Option 2 - Manual creation:
```csharp
Swapper swapper = new Swapper
{
    ModName = "YourModName",
    SwapperName = "Any Name You Want",
    ObjectNames = new List<string>([
                    "ObjectName1",
                    "ObjectName2"
        ]),
    BundleName = "YourBundle.bundle",
    AssetModules = new List<IAssetModule>
    {
        new MeshModule("Path/To/Your/Model.fbx"),
        new Texture2DModule("Path/To/Your/Texture.png")
    },
    TransformModules = new List<ITransformModule>
    {
        new MoveMeshModule(0f, 0f, 0f),
        new RotateMeshModule(0f, 0f, 0f),
        new ScaleMeshModule(0f, 0f, 0f)
    },
    Deactivations = new List<string>
    {
        "ObjectYouWantDeactivated"
    }
};
```

# Full Implementation Template
```csharp
[assembly: MelonAdditionalDependencies("ModelSwapLib")]

namespace YourMod
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Swapper swapper = new SwapperBuilder()
                .SetModName("YourModName")
                .SetSwapperName("Any Name You Want")
                .AddObjectNames(["ObjectName1", "ObjectName2"])
                .SetBundleName("YourBundle.bundle")
                .AddAssetModule(new MeshModule("Path/To/Your/Model.fbx"))
                .AddAssetModule(new Texture2DModule("Path/To/Your/Texture.png"))
                .AddTransformModule(new MoveMeshModule(0f, 0f, 0f))
                .AddTransformModule(new RotateMeshModule(0f, 0f, 0f))
                .AddTransformModule(new ScaleMeshModule(0f, 0f, 0f))
                .AddDeactivation("ObjectYouWantDeactivated")
                .Build();
            
            if(swapper == null)
            {
                Melon<Core>.Logger.Warning($"Invalid swapper");
            }else{
                Guid guid = ObjectActionManager.GetInstance().RegisterSwapper(swapper);
            
                if(guid == Guid.Empty) // If you receive a Guid.Empty then the Swapper.Validate() failed
                {
                    Melon<Core>.Logger.Warning($"Failed to register swapper: {swapper.SwapperName}");
                } else
                {
                    Melon<Core>.Logger.Msg($"Successfully registered swapper: {swapper.SwapperName} with guid: {swapper.SwapperGuid}");
                }
            }
            
            Melon<Core>.Logger.Msg($"YourMod Initialized");
        }
    }
}
```

# Required Parameters:
ModName<br>
At least 1 ObjectName<br>
At least 1 IAssetModule, ITransformModule OR Deactivation<br>
BundleName if you use an IAssetModule

Many thanks to [GraciousCub5622](https://github.com/SamGarratt17) @graciouscub5622 on discord for his Asset Swap Template, upon which this entire project is based.<br>
Many thanks to [ScribbleTAS](https://github.com/ScribbleTAS) for MeshUtils and also for his help in debugging and brainstorming.