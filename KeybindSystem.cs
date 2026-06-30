using Terraria;
using Terraria.ModLoader;
using Terraria.GameInput;
using Microsoft.Xna.Framework.Input;
using Terraria.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace TSC
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind CycleLevelButton { get; private set; }
        
        public static ResourcePackList PreviousOriginalPacks { get; private set; }  

        public override void Load()
        {
            CycleLevelButton = KeybindLoader.RegisterKeybind(Mod, "Cycle Spice Level", Keys.P);
        }

        public override void Unload()
        {
            CycleLevelButton = null;
            PreviousOriginalPacks = null;
        }

        public static void SetPreviousPacks(ResourcePackList packs)
        {
            PreviousOriginalPacks = packs;
        }
    }

    public class TSCPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.CycleLevelButton.JustPressed)
            {
                var config = ModContent.GetInstance<TSCConfig>();
                
                // Cycle through levels: Safe (0) -> Spicy (1) -> NSFW (2) -> Safe (0)
                config.CurrentSpiceLevel = (SpiceLevel)(((int)config.CurrentSpiceLevel + 1) % 3);

                string modeName = config.CurrentSpiceLevel.ToString().ToUpper();
                Color msgColor = config.CurrentSpiceLevel == SpiceLevel.Safe ? Color.LightGreen : 
                                 (config.CurrentSpiceLevel == SpiceLevel.Spicy ? Color.Yellow : Color.LightCoral);

                Main.NewText($"[TSC] Mode Changed to: {modeName}", msgColor);

                ApplyResourcePacks(config);
            }
        }

        private void ApplyResourcePacks(TSCConfig config)
        {
            if (config.CensoredResourcePacks == null || config.CensoredResourcePacks.Count == 0)
                return;

            // If we are currently allowing everything, restore the original unmodified list
            if (config.CurrentSpiceLevel == SpiceLevel.NSFW)
            {
                if (KeybindSystem.PreviousOriginalPacks != null)
                {
                    Main.AssetSourceController.UseResourcePacks(KeybindSystem.PreviousOriginalPacks);
                    KeybindSystem.SetPreviousPacks(null); // Clear so it grabs a fresh state next time we restrict
                }
            }
            else
            {
                // We are restricting textures. First, save original state if not already saved.
                if (KeybindSystem.PreviousOriginalPacks == null)
                {
                    KeybindSystem.SetPreviousPacks(Main.AssetSourceController.ActiveResourcePackList);
                }

                var safePacks = KeybindSystem.PreviousOriginalPacks.AllPacks
                    .Where(pack => {
                        // Check if pack has a "Spice State" assigned to it
                        if (config.CensoredResourcePacks.TryGetValue(pack.Name, out int state) && state > 0)
                        {
                            // Keep it only if its rating is lesser or equal to our current active mode limit!
                            return state <= (int)config.CurrentSpiceLevel;
                        }
                        return true; 
                    })
                    .OrderByDescending(pack => config.ResourcePackPriorities != null && config.ResourcePackPriorities.TryGetValue(pack.Name, out int priority) ? priority : 0)
                    .ToList();

                // Apply the strictly clean list
                Main.AssetSourceController.UseResourcePacks(new ResourcePackList(safePacks));
            }
        }
    }
}
