using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TSC
{
    public class TSCConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Blacklist Settings")]
        [Tooltip("Add the exact internal names or Workshop IDs of mods you want to hide.")]
        public List<string> CensoredModNames = new List<string> { "SpicyModInternalName", "123456789" };
        
        [DefaultValue(false)]
        [Tooltip("Is Safe Mode currently active? (Can also be toggled via Hotkey)")]
        public bool SafeModeActive;
    }
}
