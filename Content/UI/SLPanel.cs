using ShopLookup.Content.Data;
using ShopLookup.Content.UI.ExtraUI;
using System.Linq;
using System.Text;
using static ShopLookup.Content.Data.ShopNPCData;
using static ShopLookup.ShopLookup;

namespace ShopLookup.Content.UI;

public class SLPanel : ContainerElement
{
    private UIContainerPanel searchShop, searchItem, shopView;
    private UIBottom shopPanel, searchPanel;
    private UIItemSlot focus;
    private UIDropDownList<UIShopName> indexList;
    private UIDropDownList<UIModSlot> modList;
    private UIDropDownList<UIShopSlot> shopList;
    private UIInputBox input;
    private List<UIItemFilter> filters;
    private bool inShopPanel;
    private bool onlyCanBuy;
    private bool anyFilterActive;
    internal static bool flowLayout;
    public override bool SkipInMenu => true;
    public override void OnInitialization()
    {
        base.OnInitialization();
        RemoveAll();
        flowLayout = false;

        UIVnlPanel bg = new(530, 365);
        bg.Info.SetMargin(10);
        bg.SetCenter(0, 0, 0.5f, 0.5f);
        bg.canDrag = true;
        Register(bg);

        shopPanel = new(-30, 0, 1, 1);
        bg.Register(shopPanel);
        RegisterShopPanel(shopPanel);
        inShopPanel = true;

        searchPanel = new(-30, 0, 1, 1);
        searchPanel.Info.IsVisible = false;
        bg.Register(searchPanel);
        ReigsterSearchPanel(searchPanel);

        int top = 0;
        UIClose close = new();
        close.SetPos(-20, top, 1);
        close.Events.OnLeftDown += evt => Info.IsVisible = false;
        bg.Register(close);
        top += 30;

        UIMove move = new() { hoverText = GTV("UIButton.Move") };
        move.SetPos(-20, top, 1);
        bg.Register(move);
        top += 30;

        UI3FrameImage search = new(AssetLoader.ExtraAssets["Search"], x => !inShopPanel) { hoverText = GTV("UIButton.Search") };
        search.SetPos(-20, top, 1);
        search.Events.OnLeftDown += evt =>
        {
            ChangePanel();
            evt.hoverText = GTV(inShopPanel ? "UIButton.Search" : "UIButton.Return");
        };
        bg.Register(search);
        top += 30;

        UI3FrameImage strip = new(AssetLoader.ExtraAssets["StripLayout"], x => !flowLayout) { hoverText = GTV("UIButton.Strip") };
        strip.SetPos(-20, top, 1);
        strip.Events.OnLeftDown += evt =>
        {
            flowLayout = false;
            shopView.autoPos = [10, null];
            shopView.Vscroll.WheelPixel = 110;
            searchItem.autoPos = [10, null];
            searchItem.Vscroll.WheelPixel = 110;
            LookupShop();
            if (!inShopPanel)
            {
                input.OnInputText(input.Text);
            }
        };
        bg.Register(strip);
        top += 30;

        UI3FrameImage flow = new(AssetLoader.ExtraAssets["FlowLayout"], x => flowLayout) { hoverText = GTV("UIButton.Flow") };
        flow.SetPos(-20, top, 1);
        flow.Events.OnLeftDown += evt =>
        {
            flowLayout = true;
            shopView.autoPos = [10, 10];
            shopView.Vscroll.WheelPixel = 62;
            LookupShop();
            searchItem.autoPos = [10, 10];
            searchItem.Vscroll.WheelPixel = 62;
            if (!inShopPanel)
            {
                input.OnInputText(input.Text);
            }
        };
        bg.Register(flow);
        top += 30;

        UIAdjust adjust = new(AssetLoader.VnlAdjust) { hoverText = GTV("UIButton.Adjust") };
        bg.Register(adjust);
    }
    public override void Update(GameTime gt)
    {
        UIShopItem.HoverSlot = null;
        base.Update(gt);
    }
    public override void OnSaveAndQuit()
    {
        Info.IsVisible = false;
        RemoveAll();
    }
    private void RegisterShopPanel(UIBottom bg)
    {
        int top = 10;
        int left = 0;
        focus = new();
        focus.SetPos(0, top - 2);
        focus.Events.OnLeftDown += evt =>
        {
            int type = Main.mouseItem.type;
            focus.item.SetDefaults(type);
            if (type == 0)
            {
                LookupIndex();
                LookupShop();
            }
        };
        bg.Register(focus);
        left += focus.Width + 10;

        UIVnlPanel shopBg = new(0, 0);
        shopBg.Info.SetMargin(10);
        bg.Register(shopBg);

        shopList = new(bg, shopBg, x => x.Clone());

        shopList.showArea.SetPos(left, top - 2);
        shopList.showArea.SetSize(90, 52);
        shopList.showArea.Info.RightMargin.Pixel = 0;
        shopList.showArea.Info.IsSensitive = true;
        left += shopList.showArea.Width + 10;

        modList = new(bg, shopBg, x => new(x.modName, ModsByName[x.modName]) { hoverText = x.hoverText });

        modList.showArea.SetPos(left, top - 2);
        modList.showArea.SetSize(90, 52);
        modList.showArea.Info.RightMargin.Pixel = 0;
        modList.showArea.Info.IsSensitive = true;
        left += modList.showArea.Width + 15;

        top += focus.Height + 10;

        filters = [new Weapon(),new Armor(),new Vanity(),new BuildingBlock(),  new Furniture(),
            new Accessories(),new MiscAccessories(),new Consumables(),new Tools(),new Materials()];
        int x = 0, y = 0;
        foreach (UIItemFilter filter in filters)
        {
            filter.SetPos(left + 36 * x++, y * 36);
            filter.Events.OnLeftDown += evt =>
            {
                UIItemFilter f = evt as UIItemFilter;
                f.Active = !f.Active;
                filters[^1].Active = false;
                anyFilterActive = filters.Any(x => x.Active);
                LookupShop();
            };
            bg.Register(filter);
            if (x == 5)
            {
                x = 0;
                y = 1;
            }
        }
        filters.Add(new MiscFallback(filters));
        UIItemFilter other = filters[^1];
        other.SetPos(left + 180, 0);
        other.Events.OnLeftDown += evt =>
        {
            UIItemFilter f = evt as UIItemFilter;
            anyFilterActive = f.Active = !f.Active;
            foreach (UIItemFilter filter in filters)
            {
                if (filter is not MiscFallback)
                    filter.Active = false;
            }
            LookupShop();
        };
        bg.Register(other);


        UIImage onlyCanBuy = new(AssetLoader.ExtraAssets["OnlyCanBuy"]);
        onlyCanBuy.SetPos(left + 180, 38);
        onlyCanBuy.Events.OnLeftDown += evt =>
        {
            this.onlyCanBuy = !this.onlyCanBuy;
            flowLayout = true;
            shopView.autoPos = [10, 10];
            shopView.Vscroll.WheelPixel = 62;
            onlyCanBuy.color = this.onlyCanBuy ? Color.Gold : Color.White;
            LookupShop();
        };
        onlyCanBuy.Events.OnRightDown += evt =>
        {
            shopView.ClearAllElements();
            flowLayout = true;
            shopView.autoPos = [10, 10];
            Chest.SetupTravelShop();
            foreach (int itemID in Main.travelShop)
            {
                if (itemID > ItemID.None)
                    shopView.AddElement(new UIShopItem(ContentSamples.ItemsByType[itemID]));
            }
        };
        onlyCanBuy.Events.OnMouseOver += evt =>
        {
            evt.hoverText = GTV("UIButton.OnlyCanBuy");
            if (!FocusShop(out _, out int npcType, out _, out _))
            {
                int index = NPC.FindFirstNPC(npcType);
                evt.hoverText += new StringBuilder()
                    .AppendLine()
                    .Append("[c/")
                    .Append(index < 0 ? "FF0000" : "00FF00")
                    .Append(':')
                    .Append(GTV("UIButton.Find", ContentSamples.NpcsByNetId[npcType].TypeName))
                    .Append(']');
            }
            evt.hoverText += "\n" + GTV("TravelMerchant");
        };
        bg.Register(onlyCanBuy);

        shopView = new();
        shopView.SetSize(-30, 0, 1, 1);
        shopView.autoPos = [10, null];
        shopBg.Register(shopView);

        indexList = new(bg, shopView, x => x.Clone());
        indexList.SetWhellPixel(30);

        indexList.showArea.SetPos(0, top);
        indexList.showArea.SetSize(0, 30, 1);
        indexList.showArea.SetMargin(10, 5);
        top += indexList.showArea.Height + 10;
        shopBg.SetPos(0, top);
        shopBg.SetSize(0, -top, 1, 1);

        indexList.expandArea.SetPos(0, top);
        indexList.expandArea.SetSize(0, -top, 1, 1);

        indexList.expandView.Info.Height.Pixel -= 10;
        indexList.expandView.autoPos[0] = 5;

        HorizontalScrollbar hsl = new(null, false, true)
        {
            useScrollWheel = false
        };
        hsl.Info.Width.Pixel -= 20;
        hsl.Info.Top.Pixel += 15;
        indexList.expandView.SetHorizontalScrollbar(hsl);
        indexList.expandArea.Register(hsl);

        VerticalScrollbar shopvsl = new(110, true);
        shopvsl.Info.Left.Pixel += 10;
        shopView.SetVerticalScrollbar(shopvsl);
        shopBg.Register(shopvsl);

        modList.expandArea.SetPos(0, top);
        modList.expandArea.SetSize(0, -top, 1, 1);

        modList.expandView.autoPos = [10, 10];

        shopList.expandArea.SetPos(0, top);
        shopList.expandArea.SetSize(0, -top, 1, 1);

        shopList.expandView.autoPos = [10, 10];

        foreach (var (name, modInfo) in ModsByName)
        {
            UIModSlot modSlot = new(name, modInfo);
            modSlot.BorderHoverToGold();
            modList.AddElement(modSlot);
            modSlot.Events.OnLeftDown += evt => LookupMod();
        }
        modList.ChangeShowElement(0);
        BaseUIElement uie = modList.expandView.InnerUIE[0];
        uie.Events.LeftDown(uie);
        Calculation();
    }
    private void ReigsterSearchPanel(UIBottom bg)
    {
        int top = 0;
        UIVnlPanel inputBg = new(0, 0);
        inputBg.SetSize(0, 30, 1);
        bg.Register(inputBg);
        top += inputBg.Height + 10;

        input = new(GTV("SearchAny"), color: Color.White);
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
        searchShop.autoPos[1] = 5;
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
                            slot.hoverText += "\n" + GTV("Source", " " + (info.mod.DisplayName ?? "Terraria"));
                            string modName = mod;
                            int npcType = npc;
                            slot.Events.OnLeftDown += evt =>
                            {
                                ChangePanel();
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
                    slot.hoverText += "\n" + GTV("Source", " " + (ModsByName[exShop.ModName].mod.DisplayName ?? "Terraria"));
                    slot.Events.OnLeftDown += evt =>
                    {
                        UIShopSlotForEx thisSlot = evt as UIShopSlotForEx;
                        ChangePanel();
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
                UIText none = new(GTV("NoResult"));
                searchShop.AddElement(none);
            }
            foreach (var shop in NPCShopDatabase.AllShops)
            {
                foreach (var entry in shop.ActiveEntries)
                {
                    if (entry.Item.Name.Contains(text) && !PylonIDs.Contains(entry.Item.type))
                    {
                        string name = shop.Name;
                        UIShopItemForNPC slot = new(shop.NpcType, entry);
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
                        UIShopItemForEx slot = new(shop.ExShopType, shop.ModName, entry);
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
                UIText none = new(GTV("NoResult"));
                searchItem.AddElement(none);
            }
        }
    }
    public void LookupItem(int type)
    {
        focus.item.SetDefaults(type);
        LookupShop();
    }
    private void ChangePanel()
    {
        inShopPanel = !inShopPanel;
        if (inShopPanel)
        {
            shopPanel.Info.IsVisible = true;
            searchPanel.Info.IsVisible = false;
        }
        else
        {
            shopPanel.Info.IsVisible = false;
            searchPanel.Info.IsVisible = true;
        }
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
            if (!inShopPanel)
                ChangePanel();
            indexList.ChangeShowElement(new UIShopName(GTV("LookupItem", ContentSamples.ItemsByType[itemType].Name)));
            foreach (var shops in NPCShopDatabase.AllShops)
            {
                foreach (var entry in shops.ActiveEntries)
                {
                    if (entry.Item.type == itemType)
                    {
                        string name = shops.Name;
                        UIShopItemForNPC slot = new(shops.NpcType, entry);
                        if (!flowLayout)
                        {
                            slot.hoverText = $"{ContentSamples.NpcsByNetId[slot.npcType].TypeName} [{name}]";
                        }
                        shopView.AddElement(slot);
                    }
                }
            }
            foreach (var shop in ExtraShopDataBase.AllShops)
            {
                foreach (var entry in shop.Entries)
                {
                    if (entry.Item.type == itemType)
                    {
                        UIShopItemForEx slot = new(shop.ExShopType, shop.ModName, entry);
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
                UIText none = new(GTV("NoSell"));
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
                            shopView.AddElement(new UIShopItem(entry.Item));
                        }
                    }
                    else
                    {
                        foreach (var entry in exShop.Entries)
                        {
                            if (!FitsFilter(entry.Item, !pylon))
                                continue;
                            shopView.AddElement(new UIShopItemForEx(exShop.ExShopType, exShop.ModName, entry));
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
                                UIShopItem slot = new(item);
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
                            shopView.AddElement(new UIShopItemForNPC(shop.NpcType, entry));
                        }
                    }
                }
            }
        }
        if (shopView.InnerUIE.Count == 0)
        {
            UIText empty = new(GTV("EmptyShop"));
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
}
