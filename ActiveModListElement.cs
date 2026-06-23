using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;

namespace TSC
{
    public class ActiveModListElement : ConfigElement
    {
        public override void OnBind()
        {
            base.OnBind();

            // Clear the default tModLoader list UI. We are drawing our own.
            RemoveAllChildren();

            // The 'MemberInfo' represents the List<string> from our TSCConfig.
            // We cast its current value to interact with it.
            var censoredList = (List<string>)MemberInfo.GetValue(Item);
            if (censoredList == null)
            {
                censoredList = new List<string>();
                MemberInfo.SetValue(Item, censoredList);
            }

            // Create a scrolling container for our mod list.
            UIPanel listContainer = new UIPanel();
            listContainer.Width.Set(0, 1f);
            listContainer.Height.Set(400, 0f); // Adjust height as needed
            listContainer.Top.Set(30, 0f);
            listContainer.BackgroundColor = new Color(33, 43, 79) * 0.8f;
            Append(listContainer);

            UIList modList = new UIList();
            modList.Width.Set(0, 1f);
            modList.Height.Set(0, 1f);
            modList.ListPadding = 5f;
            listContainer.Append(modList);

            UIScrollbar scrollbar = new UIScrollbar();
            scrollbar.SetView(100f, 1000f);
            scrollbar.Height.Set(0, 1f);
            scrollbar.HAlign = 1f;
            listContainer.Append(scrollbar);
            modList.SetScrollbar(scrollbar);

            // Loop through all currently active mods in memory.
            foreach (var mod in ModLoader.Mods.Where(m => m.Name != "ModLoader" && m.Name != "TSC"))
            {
                var modPanel = new UIPanel();
                modPanel.Width.Set(0, 1f);
                modPanel.Height.Set(40, 0f);

                // Check if the mod is currently in our saved list to set initial color.
                bool isCensored = censoredList.Contains(mod.Name);
                modPanel.BackgroundColor = isCensored ? new Color(150, 40, 40) : new Color(40, 150, 40);

                var modText = new UIText(mod.DisplayName);
                modText.VAlign = 0.5f;
                modText.Left.Set(10, 0f);
                modPanel.Append(modText);

                // Add the Click Event to toggle the censor state.
                modPanel.OnLeftClick += (evt, element) =>
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    
                    if (censoredList.Contains(mod.Name))
                    {
                        censoredList.Remove(mod.Name);
                        modPanel.BackgroundColor = new Color(40, 150, 40); // Green
                    }
                    else
                    {
                        censoredList.Add(mod.Name);
                        modPanel.BackgroundColor = new Color(150, 40, 40); // Red
                    }

                    // Tell tModLoader the config has been modified so the Save button works.
                    MemberInfo.SetValue(Item, censoredList);
                };

                modList.Add(modPanel);
            }
            
            // Adjust the total height of our custom element so it fits in the parent config menu.
            Height.Set(450, 0f);
        }
    }
}
