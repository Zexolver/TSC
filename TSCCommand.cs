using Terraria;
using Terraria.ModLoader;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace TSC
{
    public class TSCCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "tsc";
        public override string Usage => "/tsc list";
        public override string Description => "Lists all loaded mods and their current censor status.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            var config = ModContent.GetInstance<TSCConfig>();
            if (config.CensoredMods == null) config.CensoredMods = new List<string>();

            if (args.Length == 0 || args[0].ToLower() == "list")
            {
                caller.Reply("--- Loaded Mods ---", Color.Orange);
                caller.Reply("Open Mod Configuration to change these settings.", Color.Yellow);
                
                foreach (var mod in ModLoader.Mods.Where(m => m.Name != "ModLoader" && m.Name != "TSC"))
                {
                    bool isCensored = config.CensoredMods.Contains(mod.Name);
                    string status = isCensored ? "[CENSORED]" : "[Visible]";
                    Color color = isCensored ? Color.Red : Color.LimeGreen;
                    caller.Reply($"{status} {mod.Name}", color);
                }
            }
        }
    }
}
