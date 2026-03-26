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

    /// <summary>
    /// Detect when the player manually forbids/allows something.
    /// Used for double-forbid (permanent) detection.
    /// </summary>
    [HarmonyPatch(typeof(CompForbiddable), nameof(CompForbiddable.Forbidden), MethodType.Setter)]
    public static class Patch_ForbiddenSet
    {
        public static void Postfix(CompForbiddable __instance, bool value)
        {
            try
            {
                if (!SmartAllowSettings.enabled) return;

                Thing thing = __instance.parent;
                if (thing == null || thing.Map == null) return;

                // Only track player-initiated forbid/allow changes
                // We detect this by checking if a human is interacting
                // (the auto-allow system sets Forbidden directly, but we
                // track our own auto-allows in the MapComponent)
                var comp = thing.Map.GetComponent<MapComponent_SmartAllow>();
                if (comp == null) return;

                // We can't easily distinguish player vs code forbid changes here.
                // The MapComponent handles the double-forbid logic internally
                // by tracking what it auto-allowed.
            }
            catch (Exception) { }
        }
    }
}
