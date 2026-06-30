using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TSC
{
    public enum SpiceLevel
    {
        Safe,
        Spicy,
        NSFW
    }

    public class TSCConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("GlobalSettings")]
        [DefaultValue(SpiceLevel.NSFW)]
        [DrawTicks]
        public SpiceLevel CurrentSpiceLevel { get; set; }

        [Header("ModCensoring")]
        [CustomModConfigItem(typeof(ActiveModListElement))]
        public Dictionary<string, int> CensoredMods { get; set; } = new Dictionary<string, int>();

        [Header("ResourcePackCensoring")]
        [CustomModConfigItem(typeof(ActiveResourcePackListElement))]
        public Dictionary<string, int> CensoredResourcePacks { get; set; } = new Dictionary<string, int>();

        // Hidden from the default UI, handled entirely by ActiveResourcePackListElement
        [Browsable(false)]  
        public Dictionary<string, int> ResourcePackPriorities { get; set; } = new Dictionary<string, int>();
    }
}
