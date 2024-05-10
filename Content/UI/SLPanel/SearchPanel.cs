using ShopLookup.Content.Data;
using ShopLookup.Content.UI.ExtraUI;
using System.Linq;
using System.Text;
using static ShopLookup.Content.Data.ShopNPCData;

namespace ShopLookup.Content.UI.SLPanel
{
    public partial class SLPanel
    {
        private UIContainerPanel searchShop, searchItem;
        private UIBottom searchPanel;
        private void ReigsterSearchPanel(UIBottom bg)
        {
            int top = 0;
            UIVnlPanel inputBg = new(0, 0);
            inputBg.SetSize(0, 30, 1);
            bg.Register(inputBg);
            top += inputBg.Height + 10;
            savingsHide.Add(inputBg);

            input = new(GTV("Info.SearchAny"), color: Color.White);
            input.SetSize(0, 0, 1, 1);
            input.OnInputText += SearchAny;
            inputBg.Register(input);

            UIClose clear = new();
            clear.SetCenter(-20, 0, 1, 0.5f);
            clear.Events.OnLeftDown += evt => input.ClearText();
            inputBg.Register(clear);

            UIVnlPanel npcBg = new(0, 0);
            npcBg.SetPos(0, top);
            npcBg.SetSize(0, 90, 1);
            npcBg.Info.SetMargin(10);
            bg.Register(npcBg);
            top += npcBg.Height + 10;

            searchShop = new();
            searchShop.SetSize(0, 0, 1, 1);
            searchShop.autoPos[1] = 10;
            npcBg.Register(searchShop);

            HorizontalScrollbar npcH = new(62);
            npcH.Info.Top.Pixel += 10;
            searchShop.SetHorizontalScrollbar(npcH);
            npcBg.Register(npcH);

            UIVnlPanel itemBg = new(0, 0);
            itemBg.SetPos(0, top);
            itemBg.SetSize(0, -top, 1, 1);
            itemBg.Info.SetMargin(10);
            bg.Register(itemBg);

            searchItem = new();
            searchItem.SetSize(-30, 0, 1, 1);
            searchItem.autoPos[0] = 10;
            itemBg.Register(searchItem);

            VerticalScrollbar itemV = new(110, true);
            itemV.Info.Left.Pixel += 10;
            searchItem.SetVerticalScrollbar(itemV);
            itemBg.Register(itemV);
        }
        private void SearchAny(string text)
        {
            searchShop.ClearAllElements();
            searchItem.ClearAllElements();
            if (text.Length != 0)
            {
                foreach (var (mod, info) in ModsByName)
                {
                    var npcAndHead = info.npcAndHead;
                    if (npcAndHead != null)
                    {
                        foreach (var (npc, head) in npcAndHead)
                        {
                            if (ContentSamples.NpcsByNetId[npc].TypeName.Contains(text))
                            {
                                UIShopSlotForNPC slot = new(npc);
                                slot.hoverText += "\n" + GTV("Info.Source", " " + (info.mod.DisplayName ?? "Terraria"));
                                string modName = mod;
                                int npcType = npc;
                                slot.Events.OnLeftDown += evt =>
                                {
                                    ChangePanel(0);
                                    foreach (UIModSlot modSlot in modList.expandView.InnerUIE.Cast<UIModSlot>())
                                    {
                                        if (modSlot.modName == modName)
                                        {
                                            modSlot.Events.LeftDown(modSlot);
                                            break;
                                        }
                                    }
                                    foreach (UIShopSlot shopSlot in shopList.expandView.InnerUIE.Cast<UIShopSlot>())
                                    {
                                        if (shopSlot is UIShopSlotForNPC npcSlot && npcSlot.npcType == npcType)
                                        {
                                            shopSlot.Events.LeftDown(shopSlot);
                                            break;
                                        }
                                    }
                                };
                                searchShop.AddElement(slot);
                            }
                        }
                    }
                }
                HashSet<string> added = [];
                foreach (ExtraShop exShop in ExtraShopDataBase.AllShops)
                {
                    if (exShop.TypeName.Contains(text) && added.Add(exShop.LocalPath))
                    {
                        UIShopSlotForEx slot = new(exShop);
                        slot.hoverText += "\n" + GTV("Info.Source", " " + (ModsByName[exShop.ModName].mod.DisplayName ?? "Terraria"));
                        slot.Events.OnLeftDown += evt =>
                        {
                            UIShopSlotForEx thisSlot = evt as UIShopSlotForEx;
                            ChangePanel(0);
                            foreach (UIModSlot modSlot in modList.expandView.InnerUIE.Cast<UIModSlot>())
                            {
                                if (modSlot.modName == exShop.ModName)
                                {
                                    modSlot.Events.LeftDown(modSlot);
                                    break;
                                }
                            }
                            foreach (UIShopSlot shopSlot in shopList.expandView.InnerUIE.Cast<UIShopSlot>())
                            {
                                if (shopSlot is UIShopSlotForEx exSlot && exSlot.exShopType == thisSlot.exShopType)
                                {
                                    shopSlot.Events.LeftDown(shopSlot);
                                    break;
                                }
                            }
                        };
                        searchShop.AddElement(slot);
                    }
                }
                if (searchShop.InnerUIE.Count == 0)
                {
                    UIText none = new(GTV("Info.NoResult"));
                    searchShop.AddElement(none);
                }
                foreach (var shop in NPCShopDatabase.AllShops)
                {
                    foreach (var entry in shop.ActiveEntries)
                    {
                        if (entry.Item.Name.Contains(text) && !PylonIDs.Contains(entry.Item.type))
                        {
                            string name = shop.Name;
                            UIShopItemForNPC slot = new(shop.NpcType, flowLayout, entry) { OnlyCanBuy = onlyCanBuy };
                            if (!flowLayout)
                            {
                                slot.hoverText = "";
                                string[] tooltips = shop.FullName.Split('/');
                                int i = 0;
                                foreach (var tooltip in tooltips)
                                {
                                    slot.hoverText += i++ switch
                                    {
                                        0 => ModsByName[tooltip].mod.DisplayName ?? "Terraria" + '\n',
                                        1 => ContentSamples.NpcsByNetId[shop.NpcType].TypeName + '\n',
                                        2 => tooltip,
                                        _ => ""
                                    };
                                }
                            }
                            searchItem.AddElement(slot);
                        }
                    }
                }
                foreach (var shop in ExtraShopDataBase.AllShops)
                {
                    foreach (var entry in shop.Entries)
                    {
                        if (entry.Item.Name.Contains(text))
                        {
                            UIShopItemForEx slot = new(shop.ExShopType, shop.ModName, flowLayout, entry) { OnlyCanBuy = onlyCanBuy };
                            if (!flowLayout)
                            {
                                slot.hoverText += new StringBuilder(ModsByName[shop.ModName].mod.DisplayName)
                                    .AppendLine().Append(shop.DisplayName)
                                    .AppendLine().Append(shop.TypeName);
                            }
                            searchItem.AddElement(slot);
                        }
                    }
                }
                if (searchItem.InnerUIE.Count == 0)
                {
                    UIText none = new(GTV("Info.NoResult"));
                    searchItem.AddElement(none);
                }
            }
        }
    }
}
