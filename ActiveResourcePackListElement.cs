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

            var censoredList = (List<string>)MemberInfo.GetValue(Item);
            if (censoredList == null)
            {
                censoredList = new List<string>();
                MemberInfo.SetValue(Item, censoredList);
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

            // CHANGED: Now looks at ActiveResourcePackList instead of the non-existent Available list
            foreach (var pack in Main.AssetSourceController.ActiveResourcePackList.AllPacks)
            {
                var packPanel = new UIPanel();
                packPanel.Width.Set(0, 1f);
                packPanel.Height.Set(40, 0f);

                bool isCensored = censoredList.Contains(pack.Name);
                packPanel.BackgroundColor = isCensored ? new Color(150, 40, 40) : new Color(40, 150, 40);

                var packText = new UIText(pack.Name);
                packText.VAlign = 0.5f;
                packText.Left.Set(10, 0f);
                packPanel.Append(packText);

                var statusText = new UIText(isCensored ? "[NSFW]" : "[SFW]");
                statusText.VAlign = 0.5f;
                statusText.HAlign = 0.95f;  
                statusText.TextColor = isCensored ? Color.LightCoral : Color.LightGreen;
                packPanel.Append(statusText);

                packPanel.OnLeftClick += (evt, element) =>
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    
                    if (censoredList.Contains(pack.Name))
                    {
                        censoredList.Remove(pack.Name);
                        packPanel.BackgroundColor = new Color(40, 150, 40);
                        statusText.SetText("[SFW]");
                        statusText.TextColor = Color.LightGreen;
                    }
                    else
                    {
                        censoredList.Add(pack.Name);
                        packPanel.BackgroundColor = new Color(150, 40, 40);
                        statusText.SetText("[NSFW]");
                        statusText.TextColor = Color.LightCoral;
                    }

                    MemberInfo.SetValue(Item, censoredList);
                };

                packList.Add(packPanel);
            }
            
            Height.Set(400, 0f);
        }
    }
}
