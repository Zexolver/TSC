using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria.IO;

namespace TSC
{
    public class ActiveResourcePackListElement : ConfigElement
    {
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

            UIPanel listContainer = new UIPanel();
            listContainer.Width.Set(0, 1f);
            listContainer.Height.Set(400, 0f);  
            listContainer.Top.Set(0, 0f);  
            listContainer.BackgroundColor = new Color(33, 43, 79) * 0.8f;
            Append(listContainer);

            UIList packList = new UIList();
            packList.Width.Set(0, 1f);
            packList.Height.Set(0, 1f);
            packList.ListPadding = 5f;
            listContainer.Append(packList);

            UIScrollbar scrollbar = new UIScrollbar();
            scrollbar.SetView(100f, 1000f);
            scrollbar.Height.Set(0, 1f);
            scrollbar.HAlign = 1f;
            listContainer.Append(scrollbar);
            packList.SetScrollbar(scrollbar);

            foreach (var pack in Main.AssetSourceController.ActiveResourcePackList.AllPacks)
            {
                var packPanel = new UIPanel();
                packPanel.Width.Set(0, 1f);
                packPanel.Height.Set(40, 0f);

                int currentState = 0;
                if (currentDict.TryGetValue(pack.Name, out int state))
                {
                    currentState = state;
                }

                var packText = new UIText(pack.Name);
                packText.VAlign = 0.5f;
                packText.Left.Set(10, 0f);
                packPanel.Append(packText);

                var statusText = new UIText("");
                statusText.VAlign = 0.5f;
                statusText.HAlign = 0.95f;  
                packPanel.Append(statusText);

                void UpdateVisuals(int s)
                {
                    switch (s)
                    {
                        case 0:
                            packPanel.BackgroundColor = new Color(40, 150, 40); // Green
                            statusText.SetText("[SFW]");
                            statusText.TextColor = Color.LightGreen;
                            break;
                        case 1:
                            packPanel.BackgroundColor = new Color(200, 150, 40); // Yellow
                            statusText.SetText("[SPICY]");
                            statusText.TextColor = Color.Yellow;
                            break;
                        case 2:
                            packPanel.BackgroundColor = new Color(150, 40, 40); // Red
                            statusText.SetText("[NSFW]");
                            statusText.TextColor = Color.LightCoral;
                            break;
                    }
                }

                // Apply initial visuals
                UpdateVisuals(currentState);

                packPanel.OnLeftClick += (evt, element) =>
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    
                    int activeState = 0;
                    if (currentDict.TryGetValue(pack.Name, out int s)) activeState = s;

                    activeState = (activeState + 1) % 3;

                    // Mutate the dictionary IN-PLACE
                    if (activeState == 0)
                        currentDict.Remove(pack.Name);
                    else
                        currentDict[pack.Name] = activeState;

                    UpdateVisuals(activeState);
                };

                packList.Add(packPanel);
            }
            
            Height.Set(400, 0f);
        }
    }
}
