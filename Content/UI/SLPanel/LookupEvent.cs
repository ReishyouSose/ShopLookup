using ShopLookup.Content.Data;
using ShopLookup.Content.UI.ExtraUI;
using static ShopLookup.Content.Data.ShopNPCData;
using static ShopLookup.ShopLookup;

namespace ShopLookup.Content.UI.SLPanel
{
    public partial class SLPanel
    {

        private void LookupIndex()
        {
            void AddToIndexList(UIShopName index)
            {
                index.SetSize(index.TextSize);
                index.HoverToGold();
                indexList.AddElement(index);
                index.Events.OnLeftDown += evt =>
                {
                    if (focus.item.type > ItemID.None)
                        focus.item.SetDefaults(0);
                    LookupShop();
                };
            }
            if (focus.item.type > ItemID.None)
                focus.item.SetDefaults(0);
            indexList.ClearAllElements();
            if (FocusShop(out _, out int npcType, out string exShopType, out string modName))
            {
                foreach (var exShop in ExtraShopDataBase.AllShops)
                {
                    if (exShop.ModName == modName && exShop.ExShopType == exShopType)
                    {
                        AddToIndexList(new(exShop.DisplayName, exShop.Name));
                    }
                }
            }
            else
            {
                foreach (var shop in NPCShopDatabase.AllShops)
                {
                    if (shop.NpcType == npcType)
                    {
                        string name = shop.Name;
                        if (name == "Shop")
                        {
                            AddToIndexList(new(Lang.inter[28].Value, name));
                            continue;
                        }
                        else if (ShopNames.TryGetValue(npcType, out var shops) && shops.TryGetValue(name, out LocalizedText shopLocal))
                        {
                            AddToIndexList(new(shopLocal.Value, name));
                            continue;
                        }
                        AddToIndexList(new(name));
                    }
                }
            }
            indexList.ChangeShowElement(0);
            var uie = indexList.expandView.InnerUIE[0];
            uie.Events.LeftDown(uie);
        }
        public void LookupItem(int type)
        {
            focus.item.SetDefaults(type);
            LookupShop();
        }

        private bool FitsFilter(Item item, bool ignorePylon = true)
        {
            if (item == null)
                return false;
            if (ignorePylon && PylonIDs.Contains(item.type))
                return false;
            if (anyFilterActive)
            {
                foreach (UIItemFilter filter in filters)
                {
                    if (filter.Active && filter.FitsFilter(item))
                        return true;
                }
                return false;
            }
            return true;
        }
        private void LookupShop()
        {
            shopView.ClearAllElements();
            int itemType = focus.item.type;
            if (itemType > ItemID.None)
            {
                indexList.ChangeShowElement(new UIShopName(GTV("Info.LookupItem", ContentSamples.ItemsByType[itemType].Name)));
                if (!PylonIDs.Contains(itemType))
                {
                    foreach (var shops in NPCShopDatabase.AllShops)
                    {
                        foreach (var entry in shops.ActiveEntries)
                        {
                            if (entry.Item.type == itemType)
                            {
                                string name = shops.Name;
                                UIShopItemForNPC slot = new(shops.NpcType, flowLayout, entry) { OnlyCanBuy = onlyCanBuy };
                                if (!flowLayout)
                                {
                                    slot.hoverText = $"{ContentSamples.NpcsByNetId[slot.npcType].TypeName} [{name}]";
                                }
                                shopView.AddElement(slot);
                            }
                        }
                    }
                }
                foreach (var shop in ExtraShopDataBase.AllShops)
                {
                    foreach (var entry in shop.Entries)
                    {
                        if (entry.Item.type == itemType)
                        {
                            UIShopItemForEx slot = new(shop.ExShopType, shop.ModName, flowLayout, entry) { OnlyCanBuy = onlyCanBuy };
                            if (!flowLayout)
                            {
                                slot.hoverText = shop.DisplayName;
                            }
                            shopView.AddElement(slot);
                        }
                    }
                }
                if (shopView.InnerUIE.Count == 0)
                {
                    UIText none = new(GTV("Info.NoSell"));
                    shopView.AddElement(none);
                }
            }
            else
            {
                if (FocusShop(out string shopName, out int npcType, out string exShopType, out string modName))
                {
                    if (ExtraShopDataBase.TryGetExtraShop(modName, exShopType, shopName, out ExtraShop exShop))
                    {
                        bool pylon = exShopType == "Pylon";
                        if (onlyCanBuy)
                        {
                            foreach (var entry in exShop.Entries)
                            {
                                if (!entry.Conditions.AllMet() || !FitsFilter(entry.Item, !pylon))
                                    continue;
                                shopView.AddElement(new UIShopItem(entry.Item, true) { OnlyCanBuy = true });
                            }
                        }
                        else
                        {
                            foreach (var entry in exShop.Entries)
                            {
                                if (!FitsFilter(entry.Item, !pylon))
                                    continue;
                                shopView.AddElement(new UIShopItemForEx(exShop.ExShopType, exShop.ModName, flowLayout, entry) { OnlyCanBuy = false });
                            }
                            if (shopView.InnerUIE.Count == 0)
                            {
                                UIText disable = new(exShop.DisableName);
                                disable.SetSize(disable.TextSize);
                                shopView.AddElement(disable);
                            }
                        }
                    }
                }
                else
                {
                    if (NPCShopDatabase.TryGetNPCShop(NPCShopDatabase.GetShopName(npcType, shopName), out var shop))
                    {
                        if (onlyCanBuy)
                        {
                            if (shop.TryGetCanBuyEntrys(out Item[] contents))
                            {
                                foreach (Item item in contents)
                                {
                                    if (!FitsFilter(item))
                                        continue;
                                    UIShopItem slot = new(item, true) { OnlyCanBuy = true };
                                    shopView.AddElement(slot);
                                }
                            }
                        }
                        else
                        {
                            foreach (var entry in shop.ActiveEntries)
                            {
                                if (!FitsFilter(entry.Item))
                                    continue;
                                shopView.AddElement(new UIShopItemForNPC(shop.NpcType, flowLayout, entry) { OnlyCanBuy = false });
                            }
                        }
                    }
                }
            }
            if (shopView.InnerUIE.Count == 0)
            {
                UIText empty = new(GTV("Info.EmptyShop"));
                empty.SetSize(empty.TextSize);
                shopView.AddElement(empty);
            }
            shopView.Calculation();
        }
        private void LookupMod()
        {
            shopList.ClearAllElements();
            void AddToShopList(UIShopSlot slot)
            {
                slot.BorderHoverToGold();
                shopList.AddElement(slot);
                slot.Events.OnLeftDown += evt => LookupIndex();
            }
            string modName = modList.ShowUIE.modName;
            if (ModsByName.TryGetValue(modName, out var info) && info.npcAndHead != null)
            {
                foreach (int npcType in info.npcAndHead.Keys)
                {
                    AddToShopList(new UIShopSlotForNPC(npcType));
                }
            }
            if (ExtraShopDataBase.ModShops.TryGetValue(modName, out var shops) && shops != null)
            {
                foreach (string exShopType in shops.Keys)
                {
                    AddToShopList(new UIShopSlotForEx(modName, exShopType));
                }
            }
            shopList.ChangeShowElement(0);
            if (shopPanel.IsVisible)
            {
                var uie = shopList.expandView.InnerUIE[0];
                uie.Events.LeftDown(uie);
            }
        }

        /// <returns>是否在ExShop</returns>
        private bool FocusShop(out string shopName, out int npcType, out string exShopType, out string modName)
        {
            exShopType = "";
            npcType = 0;
            shopName = indexList.ShowUIE?.key ?? "";
            modName = modList.ShowUIE.modName;
            if (shopList.ShowUIE is UIShopSlotForEx ex)
            {
                exShopType = ex.exShopType;
                return true;
            }
            if (shopList.ShowUIE is UIShopSlotForNPC npc)
            {
                npcType = npc.npcType;
            }
            return false;
        }
    }
}
