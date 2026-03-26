using RimWorld;
using Verse;

namespace SmartAllow
{
    /// <summary>
    /// Determines whether a thing should be auto-allowed based on its category
    /// and the current settings.
    /// </summary>
    public static class CategoryChecker
    {
        public static bool ShouldAutoAllow(Thing thing)
        {
            if (thing == null || thing.def == null) return false;

            // Weapons
            if (thing.def.IsWeapon)
                return SmartAllowSettings.allowWeapons;

            // Apparel
            if (thing.def.IsApparel)
                return SmartAllowSettings.allowApparel;

            // Drugs
            if (thing.def.IsDrug)
                return SmartAllowSettings.allowDrugs;

            // Medicine (check before food since some medicine is ingestible)
            if (thing.def.IsMedicine)
                return SmartAllowSettings.allowMedicine;

            // Food (not drugs, not medicine)
            if (thing.def.IsIngestible || thing.def.IsNutritionGivingIngestible)
                return SmartAllowSettings.allowFood;

            // Corpses
            if (thing is Corpse)
                return SmartAllowSettings.allowCorpses;

            // Chunks (stone, slag)
            if (thing.def.thingCategories != null)
            {
                foreach (var cat in thing.def.thingCategories)
                {
                    if (cat.defName == "StoneChunks" || cat.defName == "Chunks"
                        || cat.defName.Contains("Chunk"))
                        return SmartAllowSettings.allowChunks;
                }
            }

            // Body parts (bionics, prosthetics)
            if (thing.def.isTechHediff)
                return SmartAllowSettings.allowBodyParts;

            // Artifacts / special items
            if (thing.def.thingCategories != null)
            {
                foreach (var cat in thing.def.thingCategories)
                {
                    if (cat.defName == "Artifacts" || cat.defName.Contains("Artifact"))
                        return SmartAllowSettings.allowArtifacts;
                }
            }

            // Resources (steel, wood, components, etc.)
            if (thing.def.IsStuff || thing.def.smallVolume
                || (thing.def.thingCategories != null && thing.def.resourceReadoutPriority != ResourceCountPriority.Uncounted))
                return SmartAllowSettings.allowResources;

            // Everything else
            return SmartAllowSettings.allowOther;
        }
    }
}
