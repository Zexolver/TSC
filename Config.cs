using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TSC
{
    public class TSCConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(false)]
        [Label("Safe Mode Active")]
        [Tooltip("Turn this on to hide everything from the censored mods list.")]
        public bool SafeModeActive;

        // This attribute tells tModLoader to use our custom UI class to draw this setting.
        [CustomModConfigItem(typeof(ActiveModListElement))]
        [Label("Mods to Censor")]
        [Tooltip("Click a mod to add it to the censor list. Green means visible, Red means censored during Safe Mode.")]
        public List<string> CensoredMods = new List<string>();
    }
}
