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
            RemoveAllChildren();

            // --- COLLAPSE BUTTON ---
            UIPanel toggleButton = new UIPanel();
            toggleButton.Width.Set(0, 1f);
            toggleButton.Height.Set(40, 0f);
            toggleButton.BackgroundColor = new Color(45, 55, 85);
            Append(toggleButton);

            UIText toggleText = new UIText("Mods to Censor (Click to Expand/Collapse)");
            toggleText.HAlign = 0.5f;
            toggleText.VAlign = 0.5f;
            toggleButton.Append(toggleText);

            // --- SCROLLING CONTAINER ---
            UIPanel listContainer = new UIPanel();
            listContainer.Width.Set(0, 1f);
            listContainer.Height.Set(400, 0f);
            listContainer.Top.Set(45, 0f); // Placed just below the toggle button
            listContainer.BackgroundColor = new Color(33, 43, 79) * 0.8f;

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

            // Generate Mod Panels
            foreach (var mod in ModLoader.Mods.Where(m => m.Name != "ModLoader" && m.Name != "TSC"))
            {
                var modPanel = new UIPanel();
                modPanel.Width.Set(0, 1f);
                modPanel.Height.Set(40, 0f);

                // Fetch current list to check initial state
                var initialList = (List<string>)MemberInfo.GetValue(Item) ?? new List<string>();
                bool isCensored = initialList.Contains(mod.Name);
                modPanel.BackgroundColor = isCensored ? new Color(150, 40, 40) : new Color(40, 150, 40);

                var modText = new UIText(mod.DisplayName);
                modText.VAlign = 0.5f;
                modText.Left.Set(10, 0f);
                modPanel.Append(modText);

                var statusText = new UIText(isCensored ? "[NSFW]" : "[SFW]");
                statusText.VAlign = 0.5f;
                statusText.HAlign = 0.95f;
                statusText.TextColor = isCensored ? Color.LightCoral : Color.LightGreen;
                modPanel.Append(statusText);

                modPanel.OnLeftClick += (evt, element) =>
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);

                    // FIX: Always grab the latest state and CLONE it.
                    var currentList = (List<string>)MemberInfo.GetValue(Item) ?? new List<string>();
                    var newList = new List<string>(currentList); // The clone!

                    if (newList.Contains(mod.Name))
                    {
                        newList.Remove(mod.Name);
                        modPanel.BackgroundColor = new Color(40, 150, 40);
                        statusText.SetText("[SFW]");
                        statusText.TextColor = Color.LightGreen;
                    }
                    else
                    {
                        newList.Add(mod.Name);
                        modPanel.BackgroundColor = new Color(150, 40, 40);
                        statusText.SetText("[NSFW]");
                        statusText.TextColor = Color.LightCoral;
                    }

                    // Passing a completely new list forces tModLoader to recognize the change!
                    MemberInfo.SetValue(Item, newList);
                };

                modList.Add(modPanel);
            }

            // --- TOGGLE LOGIC ---
            bool isExpanded = true; // Default to open
            Append(listContainer);
            Height.Set(445, 0f); // 40 (button) + 5 (padding) + 400 (list)

            toggleButton.OnLeftClick += (evt, element) =>
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                isExpanded = !isExpanded;

                if (isExpanded)
                {
                    Append(listContainer);
                    Height.Set(445, 0f);
                }
                else
                {
                    RemoveChild(listContainer);
                    Height.Set(40, 0f);
                }
                
                // Recalculates the parent config menu so it collapses properly
                Recalculate(); 
            };
        }
    }
}
