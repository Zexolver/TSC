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
        private UIPanel _listContainer;
        private UIScrollbar _scrollbar;
        private bool _isExpanded = true;

        public override void OnBind()
        {
            base.OnBind();
            RemoveAllChildren();

            UIPanel toggleButton = new UIPanel();
            toggleButton.Width.Set(0, 1f);
            toggleButton.Height.Set(40, 0f);
            toggleButton.BackgroundColor = new Color(45, 55, 85);
            Append(toggleButton);

            UIText toggleText = new UIText("Mods to Censor (Click to Expand/Collapse)");
            toggleText.HAlign = 0.5f;
            toggleText.VAlign = 0.5f;
            toggleButton.Append(toggleText);

            _listContainer = new UIPanel();
            _listContainer.Width.Set(0, 1f);
            _listContainer.Height.Set(400, 0f);
            _listContainer.Top.Set(45, 0f);
            _listContainer.BackgroundColor = new Color(33, 43, 79) * 0.8f;

            UIList modList = new UIList();
            modList.Width.Set(0, 1f);
            modList.Height.Set(0, 1f);
            modList.ListPadding = 5f;
            _listContainer.Append(modList);

            _scrollbar = new UIScrollbar();
            _scrollbar.SetView(100f, 1000f);
            _scrollbar.Height.Set(0, 1f);
            _scrollbar.HAlign = 1f;
            _listContainer.Append(_scrollbar);
            modList.SetScrollbar(_scrollbar);

            // Fetch the current dictionary. Initialize it if null.
            var currentDict = (Dictionary<string, int>)MemberInfo.GetValue(Item);
            if (currentDict == null)
            {
                currentDict = new Dictionary<string, int>();
                MemberInfo.SetValue(Item, currentDict);
            }

            foreach (var mod in ModLoader.Mods.Where(m => m.Name != "ModLoader" && m.Name != "TSC"))
            {
                var modPanel = new UIPanel();
                modPanel.Width.Set(0, 1f);
                modPanel.Height.Set(40, 0f);

                int currentState = 0;
                if (currentDict.TryGetValue(mod.Name, out int state))
                {
                    currentState = state;
                }

                var modText = new UIText(mod.DisplayName);
                modText.VAlign = 0.5f;
                modText.Left.Set(10, 0f);
                modPanel.Append(modText);

                var statusText = new UIText("");
                statusText.VAlign = 0.5f;
                statusText.HAlign = 0.95f;
                modPanel.Append(statusText);

                void UpdateVisuals(int s)
                {
                    switch (s)
                    {
                        case 0:
                            modPanel.BackgroundColor = new Color(40, 150, 40); // Green
                            statusText.SetText("[SFW]");
                            statusText.TextColor = Color.LightGreen;
                            break;
                        case 1:
                            modPanel.BackgroundColor = new Color(200, 150, 40); // Yellow
                            statusText.SetText("[SPICY]");
                            statusText.TextColor = Color.Yellow;
                            break;
                        case 2:
                            modPanel.BackgroundColor = new Color(150, 40, 40); // Red
                            statusText.SetText("[NSFW]");
                            statusText.TextColor = Color.LightCoral;
                            break;
                    }
                }

                UpdateVisuals(currentState);

                modPanel.OnLeftClick += (evt, element) =>
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);

                    int activeState = 0;
                    if (currentDict.TryGetValue(mod.Name, out int s)) activeState = s;

                    activeState = (activeState + 1) % 3;

                    // Mutate the dictionary IN-PLACE so tModLoader detects the changes natively
                    if (activeState == 0)
                        currentDict.Remove(mod.Name);
                    else
                        currentDict[mod.Name] = activeState;

                    // TELLS TMODLOADER TO BRING UP THE SAVE BUTTON!
                    ConfigManager.SetPendingChanges();

                    UpdateVisuals(activeState);
                };

                modList.Add(modPanel);
            }

            if (_isExpanded)
            {
                Append(_listContainer);
                Height.Set(445, 0f);
            }
            else
            {
                Height.Set(40, 0f);
            }

            toggleButton.OnLeftClick += (evt, element) =>
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                _isExpanded = !_isExpanded;

                if (_isExpanded)
                {
                    Append(_listContainer);
                    Height.Set(445, 0f);
                }
                else
                {
                    RemoveChild(_listContainer);
                    Height.Set(40, 0f);
                }
                Recalculate();   
            };
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            if (_isExpanded && _listContainer != null && _listContainer.ContainsPoint(evt.MousePosition))
            {
                // Native UIList automatically absorbs the scroll logic, so all we have to do here
                // is intercept bubbling up to the outer ModConfig element page. 
                return;
            }
            base.ScrollWheel(evt);
        }
    }
}
