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

        // tModLoader does not support dynamic dropdowns, so players must type the mod's internal name.
        public List<string> CensoredMods = new List<string>();
    }
}
