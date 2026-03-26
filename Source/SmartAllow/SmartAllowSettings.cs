using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SmartAllow
{
    public class SmartAllowSettings : ModSettings
    {
        // Master toggle
        public static bool enabled = true;

        // Wait for combat to end before allowing
        public static bool waitForCombat = true;

        // Only auto-allow within home zone
        public static bool homeZoneOnly = false;

        // How often to scan (ticks). 250 = ~4 seconds game time
        public static int scanInterval = 250;

        // Category toggles (true = auto-allow this category)
        public static bool allowWeapons = true;
        public static bool allowApparel = true;
        public static bool allowDrugs = false; // Drugs excluded by default
        public static bool allowFood = true;
        public static bool allowResources = true;
        public static bool allowCorpses = true;
        public static bool allowChunks = true;
        public static bool allowBodyParts = true;
        public static bool allowMedicine = true;
        public static bool allowArtifacts = true;
        public static bool allowOther = true;

        // Permanently forbidden items (double-forbid to lock)
        // Stored per-save in MapComponent, not here

        public override void ExposeData()
        {
            Scribe_Values.Look(ref enabled, "enabled", true);
            Scribe_Values.Look(ref waitForCombat, "waitForCombat", true);
            Scribe_Values.Look(ref homeZoneOnly, "homeZoneOnly", false);
            Scribe_Values.Look(ref scanInterval, "scanInterval", 250);
            Scribe_Values.Look(ref allowWeapons, "allowWeapons", true);
            Scribe_Values.Look(ref allowApparel, "allowApparel", true);
            Scribe_Values.Look(ref allowDrugs, "allowDrugs", false);
            Scribe_Values.Look(ref allowFood, "allowFood", true);
            Scribe_Values.Look(ref allowResources, "allowResources", true);
            Scribe_Values.Look(ref allowCorpses, "allowCorpses", true);
            Scribe_Values.Look(ref allowChunks, "allowChunks", true);
            Scribe_Values.Look(ref allowBodyParts, "allowBodyParts", true);
            Scribe_Values.Look(ref allowMedicine, "allowMedicine", true);
            Scribe_Values.Look(ref allowArtifacts, "allowArtifacts", true);
            Scribe_Values.Look(ref allowOther, "allowOther", true);
            base.ExposeData();
        }

        public static void DrawSettings(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.CheckboxLabeled("SA_Enabled".Translate(), ref enabled,
                "SA_Enabled_Desc".Translate());
            listing.GapLine();

            if (enabled)
            {
                listing.CheckboxLabeled("SA_WaitForCombat".Translate(), ref waitForCombat,
                    "SA_WaitForCombat_Desc".Translate());
                listing.CheckboxLabeled("SA_HomeZoneOnly".Translate(), ref homeZoneOnly,
                    "SA_HomeZoneOnly_Desc".Translate());

                listing.GapLine();
                listing.Label("SA_CategoryHeader".Translate());
                listing.Gap(4f);

                listing.CheckboxLabeled("SA_AllowWeapons".Translate(), ref allowWeapons);
                listing.CheckboxLabeled("SA_AllowApparel".Translate(), ref allowApparel);
                listing.CheckboxLabeled("SA_AllowDrugs".Translate(), ref allowDrugs);
                listing.CheckboxLabeled("SA_AllowFood".Translate(), ref allowFood);
                listing.CheckboxLabeled("SA_AllowResources".Translate(), ref allowResources);
                listing.CheckboxLabeled("SA_AllowCorpses".Translate(), ref allowCorpses);
                listing.CheckboxLabeled("SA_AllowChunks".Translate(), ref allowChunks);
                listing.CheckboxLabeled("SA_AllowBodyParts".Translate(), ref allowBodyParts);
                listing.CheckboxLabeled("SA_AllowMedicine".Translate(), ref allowMedicine);
                listing.CheckboxLabeled("SA_AllowArtifacts".Translate(), ref allowArtifacts);
                listing.CheckboxLabeled("SA_AllowOther".Translate(), ref allowOther);

                listing.GapLine();
                listing.Label("SA_DoubleForbidHint".Translate());
            }

            listing.End();
        }
    }
}
