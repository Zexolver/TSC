using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using MonoMod.RuntimeDetour;

namespace TSC
{
    public class TSC : Mod
    {
        private Hook assetValueHook;
        
        // We define the delegate for the Value property getter
        private delegate Texture2D orig_GetValue(Asset<Texture2D> self);

        public override void Load()
        {
            // Dedicated servers don't load graphics, so we skip hooking if running a server
            if (Main.dedServ) return;

            // 1. Grab the "get" method of the Asset<Texture2D>.Value property
            MethodInfo getValueMethod = typeof(Asset<Texture2D>).GetProperty("Value").GetGetMethod();

            // 2. Hook it! Because get_Value is NOT a generic method itself, MonoMod allows this perfectly.
            assetValueHook = new Hook(
                getValueMethod,
                new Func<orig_GetValue, Asset<Texture2D>, Texture2D>(AssetGetValueHook)
            );
            
            assetValueHook.Apply();
        }

        public override void Unload()
        {
            // Clean up the hook when the mod unloads
            assetValueHook?.Dispose();
            assetValueHook = null;
        }

        // 3. The interceptor: Called every time the game tries to grab a texture to draw
        private Texture2D AssetGetValueHook(orig_GetValue orig, Asset<Texture2D> self)
        {
            var config = ModContent.GetInstance<TSCConfig>();
            
            // If Safe Mode is on, and the asset actually has a name...
            if (config != null && config.SafeModeActive && self.Name != null)
            {
                foreach (string blacklisted in config.CensoredModNames)
                {
                    // Check if the texture's internal path contains a blacklisted mod name
                    if (self.Name.Contains(blacklisted, StringComparison.OrdinalIgnoreCase))
                    {
                        // Hand them the vanilla 1x1 invisible pixel instead of the spicy texture!
                        return TextureAssets.MagicPixel.Value;
                    }
                }
            }
            
            // Otherwise, return the normal texture
            return orig(self);
        }
    }
}
