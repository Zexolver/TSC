using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria;
using Microsoft.Xna.Framework.Input;

namespace TSC
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind TogglePanicButton { get; private set; }

        public override void Load()
        {
            // Registers the hotkey. Defaults to 'P' but user can change it in controls menu.
            TogglePanicButton = KeybindLoader.RegisterKeybind(Mod, "Toggle Safe Mode", Keys.P);
        }

        public override void Unload()
        {
            TogglePanicButton = null;
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

                string message = config.SafeModeActive ? "[TSC] Safe Mode ACTIVE." : "[TSC] Safe Mode DEACTIVATED.";
                Main.NewText(message, config.SafeModeActive ? byte.MinValue : byte.MaxValue, 255, byte.MinValue);
            }
        }
    }
}
