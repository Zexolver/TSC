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
        [Tooltip("Turn this on to hide everything from the censored list.")]
        public bool SafeModeActive;

        [Header("ModCensoring")]
        [CustomModConfigItem(typeof(ActiveModListElement))]
        [Label("Mods to Censor")]
        [Tooltip("Click a mod to add it to the censor list. Green means visible, Red means hidden during Safe Mode.")]
        public List<string> CensoredMods = new List<string>();

        [Header("ResourcePackCensoring")]
        [CustomModConfigItem(typeof(ActiveResourcePackListElement))]
        [Label("Resource Packs to Censor")]
        [Tooltip("Click a pack to add it to the censor list. Green means active, Red means disabled during Safe Mode.")]
        public List<string> CensoredResourcePacks = new List<string>();
    }
}
