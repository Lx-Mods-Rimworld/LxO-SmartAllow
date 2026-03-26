using UnityEngine;
using Verse;

namespace SmartAllow
{
    public class SmartAllowMod : Mod
    {
        public static SmartAllowSettings settings;

        public SmartAllowMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<SmartAllowSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            SmartAllowSettings.DrawSettings(inRect);
        }

        public override string SettingsCategory()
        {
            return "LxO - Smart Allow";
        }
    }
}
