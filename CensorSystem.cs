using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TSC
{
    // A quick helper so we don't repeat the blacklist checking logic
    public static class CensorLogic
    {
        public static bool ShouldCensor(Mod mod)
        {
            if (mod == null) return false;
            
            var config = ModContent.GetInstance<TSCConfig>();
            if (config != null && config.SafeModeActive && config.CensoredMods != null)
            {
                // Instantly checks if the mod's name exists in the player's typed list
                if (config.CensoredMods.Contains(mod.Name))
                {
                    return true;
                }
            }
            return false;
        }
    }

    // 1. Intercepts Items (Inventory and Dropped on ground)
    public class CensorItems : GlobalItem
    {
        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (CensorLogic.ShouldCensor(item.ModItem?.Mod)) return false; // "False" means DON'T DRAW!
            return base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (CensorLogic.ShouldCensor(item.ModItem?.Mod)) return false;
            return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }
    }

    // 2. Intercepts Enemies and Town NPCs
    public class CensorNPCs : GlobalNPC
    {
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (CensorLogic.ShouldCensor(npc.ModNPC?.Mod)) return false;
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }

    // 3. Intercepts Projectiles (Magic attacks, thrown weapons, pets)
    public class CensorProjectiles : GlobalProjectile
    {
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (CensorLogic.ShouldCensor(projectile.ModProjectile?.Mod)) return false;
            return base.PreDraw(projectile, ref lightColor);
        }
    }
}
