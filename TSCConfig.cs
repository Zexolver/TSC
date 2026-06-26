using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TSC
{
    public class TSCConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("ModCensoring")]
        [DefaultValue(false)]
        public bool SafeModeActive { get; set; }

        [CustomModConfigItem(typeof(ActiveModListElement))]
        public Dictionary<string, int> CensoredMods { get; set; } = new Dictionary<string, int>();

        [Header("ResourcePackCensoring")]
        [CustomModConfigItem(typeof(ActiveResourcePackListElement))]
        public Dictionary<string, int> CensoredResourcePacks { get; set; } = new Dictionary<string, int>();
    }
}
