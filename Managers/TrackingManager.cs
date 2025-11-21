using MelonLoader;
using ModelSwapLib.Swapper;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ModelSwapLib.ObjectTracking;

internal static class TrackingManager
{
    private static bool Initialized = false;
    private static HashSet<int> _trackedIds = null; // int = GameObject instanceid
    private static Dictionary<string, HashSet<int>> _nameMap = null; // string = object name, int = GameObject instanceid
    
    internal static IEnumerable<int> TrackedIds => _trackedIds;
    
    internal static Dictionary<string, HashSet<int>> NameMap => _nameMap;
    
    internal static void Initialize()
    {
        if(Initialized) return;
        
        ConsoleUtils.Msg("Initializing tracking manager...");
        
        _trackedIds = new HashSet<int>();
        _nameMap = new Dictionary<string, HashSet<int>>();
        
        Initialized = true;
    }

    
    // Retrieve the objects and validate their names as the instanceid could have been recycled into a different object
    // Unlikely as it updates on scene load but possible in some cases like dynamically instantiated objects, enemies, projectiles and items
    internal static IEnumerable<GameObject> GetObjectsFromNames(IEnumerable<string> names)
    {
        List<GameObject> objects = new List<GameObject>();
        foreach (var name in names)
        {
            objects.AddRange(GetObjectsFromName(name));
        }
        return objects;
    }
    internal static IEnumerable<GameObject> GetObjectsFromName(string name)
    {
        List<GameObject> objects = new List<GameObject>();
        try
        {
            _nameMap.TryGetValue(name, out var ids);
            foreach (var id in ids)
            {
                GameObject obj = Object.FindObjectFromInstanceID(id) as GameObject;
                if (!obj)
                {
                    // Object was destroyed at some point
                    ForceObjectReprocess(name, id);
                    continue;
                }

                if (obj.name != name)
                {
                    ForceObjectReprocess(name, id);
                    continue;
                }
                
                // If we get here, the object exists and it's name matches
                objects.Add(obj);
            }
        }
        catch (Exception ignored) { }
        return objects;
    }

    private static void ForceObjectReprocess(string apparentName, int objectId)
    {
        RemoveTrackingDetails(apparentName, objectId);
        var obj = Object.FindObjectFromInstanceID(objectId); // Might not be a GameObject now
        if(!obj) return; // Could not find object matching the instance id
        if (obj is GameObject go)
        {
            AddTrackingDetails(go.name, go.GetInstanceID());
        }
    }
    internal static IEnumerable<int> GetObjectIdsByName(string name)
    {
        try
        {
            _nameMap.TryGetValue(name, out var instanceIds);
            return instanceIds;
        }
        catch (Exception ignored)
        {
            return Enumerable.Empty<int>();;
        }
        
    }

    internal static IEnumerable<int> GetAllObjectsFromNames(IEnumerable<string> names)
    {
        List<int> objectIds = new List<int>();
        foreach (var name in names)
        {
            try
            {
                objectIds.AddRange(GetObjectIdsByName(name));
            }
            catch (Exception ignored)
            {
            }
        }
        return objectIds;
    }
    
    internal static void RebuildAllTrackingIds()
    {
        _trackedIds.Clear();
        _nameMap.Clear();
        
        List<GameObject> objects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None).ToList();
        
        foreach (GameObject gameObject in objects)
        {
            AddTrackingDetails(gameObject.name, gameObject.GetInstanceID());
            MelonCoroutines.Start(ObjectActionManager.GetInstance().HandleObject(gameObject));
        }
    }

    private static void AddTrackingDetails(GameObject gameObject)
    {
        AddTrackingDetails(gameObject.name, gameObject.GetInstanceID());
    }
    
    private static void AddTrackingDetails(string name, int trackingId)
    {
        if (_trackedIds.Contains(trackingId)) return;
        _trackedIds.Add(trackingId);
        
        if (!_nameMap.TryGetValue(name, out var trackingIds))
        {
            // No list associated, create a new one
            _nameMap.Add(name, new HashSet<int>( [trackingId]));
        }
        else
        {
            trackingIds.Add(trackingId);
            _nameMap[name] = trackingIds;
        }
    }

    private static void RemoveTrackingDetails(GameObject gameObject)
    {
        RemoveTrackingDetails(gameObject.name, gameObject.GetInstanceID());
    }

    private static void RemoveTrackingDetails(string name, int trackingId)
    {
        if (!_nameMap.TryGetValue(name, out var trackingIds))
        {
            // No tracking ids for the provided name
            return;
        }
        trackingIds.Remove(trackingId);
        _trackedIds.Remove(trackingId);
    }
}