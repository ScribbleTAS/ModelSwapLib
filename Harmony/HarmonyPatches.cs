using HarmonyLib;
using Il2CppKeepsake;
using Il2CppKeepsake.Pickupables.GenericWeapon;
using Il2CppLudiq;
using MelonLoader;
using ModelSwapLib.ObjectTracking;
using ModelSwapLib.Swapper;
using UnityEngine;

namespace ModelSwapLib.Harmony;

[HarmonyPatch]
public class HarmonyPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Instantiate), new Type[] { typeof(UnityEngine.Object), typeof(Transform), typeof(bool) })]
    [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Instantiate), new Type[] { typeof(UnityEngine.Object), typeof(Vector3), typeof(Quaternion) })]
    [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Instantiate), new Type[] { typeof(UnityEngine.Object), typeof(Vector3), typeof(Quaternion), typeof(Transform) })]
    static void InstantiatePostfix(ref UnityEngine.Object __result)
    {
        if (__result == null) return;
        
        var gameObject = __result.GameObject();
        if(gameObject == null) return;
        
        TrackingManager.AddTrackingDetails(gameObject);
        
        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.gameObject == __result) continue;
            TrackingManager.AddTrackingDetails(child.gameObject);
        }
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(UnityEngine.Object), "Destroy", new[] { typeof(UnityEngine.Object) })]
    static void Prefix_Object(UnityEngine.Object obj)
    {
        if(!obj) return;
        if (obj is GameObject go)
        {
            TrackingManager.RemoveTrackingDetails(go);
        }
    }
}