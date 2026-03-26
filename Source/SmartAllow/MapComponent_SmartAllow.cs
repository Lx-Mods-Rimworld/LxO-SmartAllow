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

        // Items we've auto-allowed once (used for double-forbid detection)
        private HashSet<int> autoAllowedOnce = new HashSet<int>();

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

                // Double-forbid detection: if we already auto-allowed this and the player
                // forbade it again, mark it as permanently forbidden
                if (autoAllowedOnce.Contains(thingID))
                {
                    permanentlyForbidden.Add(thingID);
                    autoAllowedOnce.Remove(thingID);
                    continue;
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
                autoAllowedOnce.Add(thingID);
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
                if (pawn.HostileTo(Faction.OfPlayer))
                    return true;
            }
            return false;
        }

        private void CleanupTracking()
        {
            // Remove entries for things that no longer exist
            autoAllowedOnce.RemoveWhere(id =>
            {
                var things = map.listerThings.AllThings;
                for (int i = 0; i < things.Count; i++)
                {
                    if (things[i].thingIDNumber == id) return false;
                }
                return true; // Thing gone, remove tracking
            });

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

            if (autoAllowedOnce.Contains(id))
            {
                permanentlyForbidden.Add(id);
                autoAllowedOnce.Remove(id);
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
            autoAllowedOnce.Remove(id);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref permanentlyForbidden, "sa_permForbidden", LookMode.Value);
            Scribe_Collections.Look(ref autoAllowedOnce, "sa_autoAllowed", LookMode.Value);
            if (permanentlyForbidden == null) permanentlyForbidden = new HashSet<int>();
            if (autoAllowedOnce == null) autoAllowedOnce = new HashSet<int>();
        }
    }
}
