using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SmartAllow
{
    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        static HarmonyInit()
        {
            var harmony = new Harmony("Lexxers.SmartAllow");

            try
            {
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Log.Message("[SmartAllow] Patches applied.");
            }
            catch (Exception ex)
            {
                Log.Error("[SmartAllow] Failed to apply patches: " + ex);
            }
        }
    }

    /// <summary>
    /// Ensure MapComponent is added to all maps.
    /// </summary>
    [HarmonyPatch(typeof(Map), nameof(Map.FinalizeInit))]
    public static class Patch_MapInit
    {
        public static void Postfix(Map __instance)
        {
            if (__instance.GetComponent<MapComponent_SmartAllow>() == null)
                __instance.components.Add(new MapComponent_SmartAllow(__instance));
        }
    }

    // Patch_ForbiddenSet removed: was a dead stub that fired on every forbid/allow
    // operation but did no work. Double-forbid logic is handled in MapComponent.
}
