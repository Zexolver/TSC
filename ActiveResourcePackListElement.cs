using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
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
    public class ActiveResourcePackListElement : ConfigElement
    {
        private UIPanel _listContainer;
        private UIScrollbar _scrollbar;

        public override void OnBind()
        {
            base.OnBind();
            RemoveAllChildren();

            var currentDict = (Dictionary<string, int>)MemberInfo.GetValue(Item);
            if (currentDict == null)
            {
                currentDict = new Dictionary<string, int>();
                MemberInfo.SetValue(Item, currentDict);
            }

            // Grab the config to access our new Priorities dictionary
            var config = ModContent.GetInstance<TSCConfig>();
            if (config.ResourcePackPriorities == null)
            {
                config.ResourcePackPriorities = new Dictionary<string, int>();
            }

            _listContainer = new UIPanel();
            _listContainer.Width.Set(0, 1f);
            _listContainer.Height.Set(450, 0f);   
            _listContainer.Top.Set(0, 0f);   
            _listContainer.BackgroundColor = new Color(33, 43, 79) * 0.8f;
            Append(_listContainer);

            UIList packList = new UIList();
            packList.Width.Set(0, 1f);
            packList.Height.Set(0, 1f);
            packList.ListPadding = 5f;
            _listContainer.Append(packList);

            _scrollbar = new UIScrollbar();
            _scrollbar.SetView(100f, 1000f);
            _scrollbar.Height.Set(0, 1f);
            _scrollbar.HAlign = 1f;
            _listContainer.Append(_scrollbar);
            packList.SetScrollbar(_scrollbar);

            foreach (var pack in Main.AssetSourceController.ActiveResourcePackList.AllPacks)
            {
                var packPanel = new UIPanel();
                packPanel.Width.Set(0, 1f);
                packPanel.Height.Set(75, 0f); // Taller to fit slider

                int currentState = 0;
                if (currentDict.TryGetValue(pack.Name, out int state)) currentState = state;

                var packText = new UIText(pack.Name);
                packText.Top.Set(5, 0f);
                packText.Left.Set(10, 0f);
                packPanel.Append(packText);

                // --- ISOLATED STATUS BUTTON ---
                var statusButton = new UIPanel();
                statusButton.Width.Set(100, 0f);
                statusButton.Height.Set(30, 0f);
                statusButton.HAlign = 1f;
                statusButton.Top.Set(0, 0f);
                packPanel.Append(statusButton);

                var statusText = new UIText("");
                statusText.HAlign = 0.5f;
                statusText.VAlign = 0.5f;
                statusButton.Append(statusText);

                void UpdateVisuals(int s)
                {
                    switch (s)
                    {
                        case 0:
                            statusButton.BackgroundColor = new Color(40, 150, 40);
                            statusText.SetText("[SFW]");
                            statusText.TextColor = Color.LightGreen;
                            break;
                        case 1:
                            statusButton.BackgroundColor = new Color(200, 150, 40);
                            statusText.SetText("[SPICY]");
                            statusText.TextColor = Color.Yellow;
                            break;
                        case 2:
                            statusButton.BackgroundColor = new Color(150, 40, 40);
                            statusText.SetText("[NSFW]");
                            statusText.TextColor = Color.LightCoral;
                            break;
                    }
                }

                UpdateVisuals(currentState);

                statusButton.OnLeftClick += (evt, element) =>
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    int activeState = 0;
                    if (currentDict.TryGetValue(pack.Name, out int s)) activeState = s;
                    activeState = (activeState + 1) % 3;

                    if (activeState == 0) currentDict.Remove(pack.Name);
                    else currentDict[pack.Name] = activeState;

                    ConfigManager.SetPendingChanges();
                    UpdateVisuals(activeState);
                };

                // --- PRIORITY SLIDER ---
                var priorityText = new UIText("Pri: 0");
                priorityText.Top.Set(42, 0f);
                priorityText.Left.Set(10, 0f);
                packPanel.Append(priorityText);

                var sliderBg = new UIPanel();
                sliderBg.Top.Set(45, 0f);
                sliderBg.Left.Set(80, 0f);
                sliderBg.Width.Set(-90, 1f); 
                sliderBg.Height.Set(16, 0f); // Sleeker tracking line height
                sliderBg.BackgroundColor = new Color(25, 30, 45); // Dark track background
                sliderBg.BorderColor = new Color(15, 15, 20); // Very subtle borders
                sliderBg.SetPadding(0); // Erase padding so fill bar fits smoothly
                packPanel.Append(sliderBg);

                var sliderFill = new UIPanel();
                sliderFill.Top.Set(0, 0f);
                sliderFill.Left.Set(0, 0f);
                sliderFill.Height.Set(0, 1f);
                sliderFill.BackgroundColor = new Color(50, 150, 250); // Standard Terraria mana blue slider
                sliderFill.BorderColor = Color.Transparent; // Borderless inner fill
                sliderFill.SetPadding(0);
                sliderBg.Append(sliderFill);

                bool isDragging = false;

                Action updateSliderVisuals = () =>
                {
                    int val = config.ResourcePackPriorities.TryGetValue(pack.Name, out int v) ? v : 0;
                    priorityText.SetText($"Pri: {val}");
                    sliderFill.Width.Set(0, val / 100f); // Width based on percentage of parent
                    sliderFill.Recalculate();
                };

                sliderBg.OnLeftMouseDown += (evt, el) => { isDragging = true; };

                sliderBg.OnUpdate += (el) =>
                {
                    if (isDragging)
                    {
                        // Stop dragging if they let go of the mouse anywhere on the screen
                        if (!Main.mouseLeft)
                        {
                            isDragging = false;
                            ConfigManager.SetPendingChanges(); // Save changes only on release to prevent lag
                            return;
                        }

                        CalculatedStyle inner = sliderBg.GetInnerDimensions();
                        float ratio = Utils.Clamp((Main.mouseX - inner.X) / inner.Width, 0f, 1f);
                        int newVal = (int)Math.Round(ratio * 100);
                         
                        config.ResourcePackPriorities[pack.Name] = newVal;
                        updateSliderVisuals();
                    }
                };

                updateSliderVisuals();
                packList.Add(packPanel);
            }
             
            Height.Set(450, 0f);
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            if (_listContainer != null && _listContainer.ContainsPoint(evt.MousePosition))
            {
                // UIList inner scrolling is handled natively, prevent bubbling outer page scroll.
                return;
            }
            base.ScrollWheel(evt);
        }
    }
}
