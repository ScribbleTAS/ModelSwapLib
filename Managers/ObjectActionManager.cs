using System.Collections;
using ModelSwapLib.ObjectTracking;
using UnityEngine;

namespace ModelSwapLib.Swapper;

public class ObjectActionManager
{
    private static ObjectActionManager instance;

    private ObjectActionManager()
    {
        instance = this;
    }

    public static ObjectActionManager GetInstance()
    {
        if (instance == null)
        {
            instance = new ObjectActionManager();
        }
        return instance;
    }
    
    private Dictionary<string, List<Swapper>> ObjectActions = new Dictionary<string, List<Swapper>>();
    private Dictionary<Guid, Swapper> SwapperCache = new Dictionary<Guid, Swapper>();
    private HashSet<string> SkipCache = new HashSet<string>();
    
    internal IEnumerator HandleObject(GameObject obj)
    {
        if(!obj) yield break;
        if (SkipCache.Contains(obj.name)) yield break;
        // The instantiated object is not listed as skip
        
        if (!ObjectActions.TryGetValue(obj.name, out List<Swapper> actions))
        {
            // no swapper list associated with this smr name
            SkipCache.Add(obj.name);
            yield break;
        }

        if (actions == null || actions.Count == 0)
        {
            
            SkipCache.Add(obj.name);
            yield break;
        }
        
        yield return new WaitForEndOfFrame();
        
        if(!obj) yield break; // It is possible for obj to become null, something something race conditions

        foreach (Swapper swapper in actions)
        {
            swapper.RunAllModules(obj);
            swapper.DeactivateObjects();
        }
    }
    
    /// <summary>
    /// Used to register a swapper
    /// </summary>
    /// <param name="swapper">The swapper</param>
    /// <returns></returns>
    public Guid RegisterSwapper(Swapper swapper)
    {
        if (!swapper.Validate()) return Guid.Empty;
        if(SwapperCache.ContainsKey(swapper.SwapperGuid))
        {
            ConsoleUtils.Msg($"Swapper {swapper.SwapperGuid} already registered");
            SwapperCache.Add(swapper.SwapperGuid, swapper);
            return swapper.SwapperGuid;
        }

        List<string> names = swapper.ObjectNames;
        foreach (string name in names)
        {
            ObjectActions.TryGetValue(name, out List<Swapper> objectSwappers);
            // List might be null, meaning no swappers were registered for this object
            if (objectSwappers == null)
            {
                objectSwappers = new List<Swapper>();
                objectSwappers.Add(swapper);
                ObjectActions[name] = objectSwappers;
                continue;
            }
            // List not null, might already contain the swapper instance
            // Don't want to add again
            if(objectSwappers.Contains(swapper)) continue;
            
            // List does not contain the swapper instance
            // We can add without duplicating
            objectSwappers.Add(swapper);
        }

        

        foreach (GameObject go in TrackingManager.GetObjectsFromNames(swapper.ObjectNames))
        {
            swapper.RunAllModules(go);
        }
        swapper.DeactivateObjects();

        foreach (string name in swapper.ObjectNames)
        {
            if(SkipCache.Contains(name)) SkipCache.Remove(name);
        }
        
        return swapper.SwapperGuid;
    }
    
    /// <summary>
    /// Will clear the SkipCache and cause a re-evaluation of all newly instantiated objects.
    /// </summary>
    public void ClearSkipCache()
    {
        SkipCache.Clear();
        TrackingManager.RebuildAllTrackingIds();
    }
    
    /// <summary>
    /// Used to obtain the swapper instance associated with the guid
    /// </summary>
    /// <param name="swapperGuid">The guid of the swapper</param>
    /// <returns></returns>
    public Swapper GetSwapper(Guid swapperGuid)
    {
        return SwapperCache.GetValueOrDefault(swapperGuid, null);
    }

    /// <summary>
    /// Used to Unregister a swapper
    /// </summary>
    /// <param name="swapperGuid">The guid of the swapper</param>
    public void UnregisterSwapper(Guid swapperGuid)
    {
        if(swapperGuid == null || swapperGuid == Guid.Empty) return;
        UnregisterSwapper(SwapperCache.GetValueOrDefault(swapperGuid, null));
    }
    
    /// <summary>
    /// Used to Unregister a swapper
    /// </summary>
    /// <param name="swapper">The swapper</param>
    public void UnregisterSwapper(Swapper swapper)
    {
        if(swapper == null) return;
        if(!swapper.Validate()) return;
        foreach (KeyValuePair<string, List<Swapper>> kvp in ObjectActions)
        {
            List<Swapper> eventSwappers = kvp.Value;
            if (eventSwappers.Contains(swapper)) eventSwappers.Remove(swapper);
        }
        
        SwapperCache.Remove(swapper.SwapperGuid);
    }
}