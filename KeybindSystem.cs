using Terraria;
using Terraria.ModLoader;
using Terraria.GameInput;
using Microsoft.Xna.Framework.Input;
using Terraria.IO;
using System.Collections.Generic;
using System.Linq;

namespace TSC
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind TogglePanicButton { get; private set; }
        
        // Added this to remember what packs were on before the panic button was hit
        public static ResourcePackList PreviousPacks { get; private set; } 

        public override void Load()
        {
            TogglePanicButton = KeybindLoader.RegisterKeybind(Mod, "Toggle Safe Mode", Keys.P);
        }

        public override void Unload()
        {
            TogglePanicButton = null;
            PreviousPacks = null;
        }

        // Helper method to let us write to the previous packs state
        public static void SetPreviousPacks(ResourcePackList packs)
        {
            PreviousPacks = packs;
        }
    }

    public class TSCPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.TogglePanicButton.JustPressed)
            {
                var config = ModContent.GetInstance<TSCConfig>();
                config.SafeModeActive = !config.SafeModeActive;

                string message = config.SafeModeActive ? "[TSC] Safe Mode ACTIVE. Purging textures..." : "[TSC] Safe Mode DEACTIVATED. Restoring textures...";
                Main.NewText(message, config.SafeModeActive ? byte.MinValue : byte.MaxValue, 255, byte.MinValue);

                ApplyResourcePacks(config);
            }
        }

        private void ApplyResourcePacks(TSCConfig config)
        {
            if (config.CensoredResourcePacks == null || config.CensoredResourcePacks.Count == 0)
                return;

            if (config.SafeModeActive)
            {
                // Save the current state of textures
                KeybindSystem.SetPreviousPacks(Main.AssetSourceController.ActiveResourcePackList);

                // Filter out the naughty ones
                var safePacks = KeybindSystem.PreviousPacks.AllPacks
                    .Where(pack => !config.CensoredResourcePacks.Contains(pack.Name))
                    .ToList();

                // Apply the clean list
                Main.AssetSourceController.UseResourcePacks(new ResourcePackList(safePacks));
            }
            else
            {
                // Restore the original state if we have it saved
                if (KeybindSystem.PreviousPacks != null)
                {
                    Main.AssetSourceController.UseResourcePacks(KeybindSystem.PreviousPacks);
                }
            }
        }
    }
}
