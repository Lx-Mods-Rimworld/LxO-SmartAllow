using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace SmartAllow
{
    /// <summary>
    /// Per-map component that periodically scans for forbidden items
    /// and auto-allows them based on settings. Tracks permanently
    /// forbidden items (double-forbid).
    /// </summary>
    public class MapComponent_SmartAllow : MapComponent
    {
        // Items the player has double-forbidden (forbid -> auto-allow -> forbid again = permanent)
        private HashSet<int> permanentlyForbidden = new HashSet<int>();

        // Items we've auto-allowed: thingID -> tick when auto-allowed
        // Used for double-forbid detection with a cooldown to avoid false positives
        // from game code re-forbidding items (death, drop, etc.)
        private Dictionary<int, int> autoAllowedTicks = new Dictionary<int, int>();

        // Cooldown: if item is re-forbidden within this many ticks after auto-allow,
        // it's game code, not the player. Only count as double-forbid after this window.
        private const int DoubleForbidCooldown = 500; // ~8 seconds

        // Track combat state
        private bool wasInCombat;

        public MapComponent_SmartAllow(Map map) : base(map) { }

        public override void MapComponentTick()
        {
            if (!SmartAllowSettings.enabled) return;

            int tick = Find.TickManager.TicksGame;
            if (tick % SmartAllowSettings.scanInterval != 0) return;

            // Combat check: if waiting for combat to end, skip during active raids
            if (SmartAllowSettings.waitForCombat)
            {
                bool inCombat = IsMapInCombat();
                if (inCombat)
                {
                    wasInCombat = true;
                    return; // Don't allow during combat
                }

                // Combat just ended - process all forbidden items
                if (wasInCombat)
                {
                    wasInCombat = false;
                    // Fall through to process
                }
            }

            ProcessForbiddenItems();
        }

        private void ProcessForbiddenItems()
        {
            int tick = Find.TickManager.TicksGame;
            List<Thing> allThings = map.listerThings.AllThings;
            for (int i = 0; i < allThings.Count; i++)
            {
                Thing thing = allThings[i];
                if (thing == null || thing.Destroyed || !thing.Spawned) continue;
                if (!thing.def.EverHaulable) continue;

                // Check if forbidden
                CompForbiddable forbiddable = thing.TryGetComp<CompForbiddable>();
                if (forbiddable == null || !forbiddable.Forbidden) continue;

                int thingID = thing.thingIDNumber;

                // Skip permanently forbidden items
                if (permanentlyForbidden.Contains(thingID)) continue;

                // Double-forbid detection: if we auto-allowed this item AND enough time
                // passed (cooldown), the player must have re-forbidden it = permanent lock.
                // If re-forbidden within the cooldown, it was game code (death, drop, etc.)
                // and we just auto-allow again.
                if (autoAllowedTicks.TryGetValue(thingID, out int allowedTick))
                {
                    int elapsed = tick - allowedTick;
                    if (elapsed > DoubleForbidCooldown)
                    {
                        // Player re-forbade after cooldown = intentional = permanent lock
                        permanentlyForbidden.Add(thingID);
                        autoAllowedTicks.Remove(thingID);
                        continue;
                    }
                    // Within cooldown = game code re-forbade, just allow again (fall through)
                }

                // Home zone check
                if (SmartAllowSettings.homeZoneOnly)
                {
                    if (!map.areaManager.Home[thing.Position])
                        continue;
                }

                // Category check
                if (!CategoryChecker.ShouldAutoAllow(thing)) continue;

                // Auto-allow
                forbiddable.Forbidden = false;
                autoAllowedTicks[thingID] = tick;
            }

            // Clean up tracking sets periodically (every 2500 ticks = ~42 seconds)
            if (Find.TickManager.TicksGame % 2500 == 0)
            {
                CleanupTracking();
            }
        }

        private bool IsMapInCombat()
        {
            // Check if there are hostile pawns on the map that are not downed
            foreach (var pawn in map.mapPawns.AllPawnsSpawned)
            {
                if (pawn.Dead || pawn.Downed) continue;
                Faction player = Find.FactionManager?.OfPlayer;
                if (player != null && pawn.HostileTo(player))
                    return true;
            }
            return false;
        }

        private void CleanupTracking()
        {
            // Remove entries for things that no longer exist
            // Clean autoAllowedTicks: remove entries for things that no longer exist
            var keysToRemove = new List<int>();
            foreach (var kvp in autoAllowedTicks)
            {
                bool found = false;
                var things = map.listerThings.AllThings;
                for (int i = 0; i < things.Count; i++)
                {
                    if (things[i].thingIDNumber == kvp.Key) { found = true; break; }
                }
                if (!found) keysToRemove.Add(kvp.Key);
            }
            for (int i = 0; i < keysToRemove.Count; i++)
                autoAllowedTicks.Remove(keysToRemove[i]);

            permanentlyForbidden.RemoveWhere(id =>
            {
                var things = map.listerThings.AllThings;
                for (int i = 0; i < things.Count; i++)
                {
                    if (things[i].thingIDNumber == id) return false;
                }
                return true;
            });
        }

        /// <summary>
        /// Called when a player manually forbids something.
        /// If we auto-allowed it before, this is a "double-forbid" = permanent.
        /// </summary>
        public void OnPlayerForbid(Thing thing)
        {
            if (thing == null) return;
            int id = thing.thingIDNumber;

            if (autoAllowedTicks.TryGetValue(id, out int allowedTick))
            {
                int elapsed = Find.TickManager.TicksGame - allowedTick;
                if (elapsed > DoubleForbidCooldown)
                {
                    // Player deliberately re-forbade after cooldown
                    permanentlyForbidden.Add(id);
                }
                // Either way, remove from tracking
                autoAllowedTicks.Remove(id);
            }
        }

        /// <summary>
        /// Called when a player manually allows something.
        /// Remove it from permanently forbidden if applicable.
        /// </summary>
        public void OnPlayerAllow(Thing thing)
        {
            if (thing == null) return;
            int id = thing.thingIDNumber;
            permanentlyForbidden.Remove(id);
            autoAllowedTicks.Remove(id);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref permanentlyForbidden, "sa_permForbidden", LookMode.Value);
            // autoAllowedTicks is transient -- don't save, rebuild from scratch on load
            // (avoids save compat issues from changing HashSet to Dictionary)
            if (permanentlyForbidden == null) permanentlyForbidden = new HashSet<int>();
            if (autoAllowedTicks == null) autoAllowedTicks = new Dictionary<int, int>();
        }
    }
}
