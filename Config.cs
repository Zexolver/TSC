using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TSC
{
    public class TSCConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(false)]
        public bool SafeModeActive;

        // Using strings so the compiler stays happy
        public List<string> CensoredMods = new List<string>();
    }
}
