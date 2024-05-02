using ShopLookup.Content.Data;
using ShopLookup.Content.Sys;
using ShopLookup.Content.UI.ExtraUI;
using System.Linq;
using static ShopLookup.Content.Data.ShopNPCData;
using static ShopLookup.ShopLookup;

namespace ShopLookup.Content.UI;

public class ReBuild : ContainerElement
{
    // TODO:
    // range的按钮
    private enum LookupRange
    {
        Normal,
        NPCAll,
        ModAll,
        All,
    }
    private UIContainerPanel searchNPC, searchItem, shopView;
    private UIBottom shopPanel, searchPanel;
    private UIItemSlot focus;
    private UIDropDownList<UIText> indexList;
    private UIDropDownList<UIModSlot> modList;
    private UIDropDownList<UINPCSlot> npcList;
    private UIInputBox input;
    private List<UIItemFilter> filters;
    private bool inShopPanel;
    private bool onlyCanBuy;
    private bool anyFilterActive;
    private LookupRange range;
    private bool InExShop => modList.ShowUIE.modName == "ShopLookup";
    private ref bool FlowLayout => ref SLConfig.Ins.FlowLayout;
    public override void OnInitialization()
    {
        base.OnInitialization();
        if (Main.gameMenu)
            return;
        RemoveAll();
        FlowLayout = false;

        UIVnlPanel bg = new(530, 350);
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

        UI2FrameImage search = new(AssetLoader.ExtraAssets["Search"]) { hoverText = GTV("UIButton.Search") };
        search.SetPos(-20, top, 1);
        search.Events.OnLeftDown += evt =>
        {
            ChangePanel();
            evt.hoverText = GTV(inShopPanel ? "UIButton.Search" : "UIButton.Return");
        };
        bg.Register(search);
        top += 30;

        UI2FrameImage strip = new(AssetLoader.ExtraAssets["StripLayout"]) { hoverText = GTV("UIButton.Strip") };
        strip.SetPos(-20, top, 1);
        strip.Events.OnLeftDown += evt =>
        {
            FlowLayout = false;
            shopView.autoPos = [10, null];
            shopView.Vscroll.WheelPixel = 110;
            LookupShop();
            if (!inShopPanel)
            {
                input.OnInputText(input.Text);
            }
        };
        bg.Register(strip);
        top += 30;

        UI2FrameImage flow = new(AssetLoader.ExtraAssets["FlowLayout"]) { hoverText = GTV("UIButton.Flow") };
        flow.SetPos(-20, top, 1);
        flow.Events.OnLeftDown += evt =>
        {
            FlowLayout = true;
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

        UI2FrameImage onlyCanBuy = new(AssetLoader.ExtraAssets["OnlyCanBuy"]) { hoverText = GTV("UIButton.OnlyCanBuy") };
        onlyCanBuy.SetPos(-20, top, 1);
        onlyCanBuy.Events.OnLeftDown += evt =>
        {
            this.onlyCanBuy = true;
            LookupShop();
        };
        bg.Register(onlyCanBuy);
        top += 30;

        UIAdjust adjust = new(AssetLoader.VnlAdjust);
        bg.Register(adjust);
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
            if (type > ItemID.None)
            {
                LookupItem(type);
            }
        };
        bg.Register(focus);
        left += focus.Width + 10;

        UIVnlPanel shopBg = new(0, 0);
        shopBg.Info.SetMargin(10);
        bg.Register(shopBg);

        npcList = new(bg, shopBg, x => new(x.npcType) { icon = x.icon, hoverText = x.hoverText });

        npcList.showArea.SetPos(left, top - 2);
        npcList.showArea.SetSize(90, 52);
        npcList.showArea.Info.RightMargin.Pixel = 0;
        npcList.showArea.Info.IsSensitive = true;
        left += npcList.showArea.Width + 10;

        modList = new(bg, shopBg, x => new(x.modName, ModsByName[x.modName]) { hoverText = x.hoverText });

        modList.showArea.SetPos(left, top - 2);
        modList.showArea.SetSize(90, 52);
        modList.showArea.Info.RightMargin.Pixel = 0;
        modList.showArea.Info.IsSensitive = true;
        left += modList.showArea.Width + 10;

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
        other.SetPos(left + 180, 20);
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

        shopView = new();
        shopView.SetSize(-30, 0, 1, 1);
        shopView.autoPos[0] = 10;
        shopBg.Register(shopView);

        indexList = new(bg, shopView, x =>
        {
            UIText uie = new(x.text);
            uie.SetPos(10, 5);
            return uie;
        });
        indexList.SetWhellPixel(30);

        indexList.showArea.SetPos(0, top);
        indexList.showArea.SetSize(0, 30, 1);
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

        npcList.expandArea.SetPos(0, top);
        npcList.expandArea.SetSize(0, -top, 1, 1);

        npcList.expandView.autoPos = [10, 10];

        foreach (var (name, modInfo) in ModsByName)
        {
            UIModSlot modSlot = new(name, modInfo);
            modSlot.BorderHoverToGold();
            if (name == "ShopLookup")
            {
                modSlot.Events.OnLeftDown += evt =>
                {
                    npcList.ClearAllElements();
                    foreach (ExShop exShop in ExtraShop.extraShops)
                    {
                        UIExShopSlot slot = new(exShop);
                        slot.Events.OnLeftDown += evt => LookupShop();
                        slot.BorderHoverToGold();
                        npcList.AddElement(slot);
                    }
                    npcList.ChangeShowElement(0);
                    BaseUIElement uie = npcList.expandView.InnerUIE[0];
                    uie.Events.LeftDown(uie);
                };
            }
            else
            {
                modSlot.Events.OnLeftDown += evt =>
                {
                    npcList.ClearAllElements();
                    UIModSlot modSlot = evt as UIModSlot;
                    foreach (var (npcType, head) in ModsByName[modSlot.modName].npcAndHead)
                    {
                        UINPCSlot slot = new(npcType, SpecialNPCHeads.TryGetValue(npcType, out Texture2D spHead) ? spHead : head);
                        slot.Events.OnLeftDown += evt =>
                        {
                            LookupIndex(slot.npcType);
                            LookupShop();
                        };
                        slot.BorderHoverToGold();
                        npcList.AddElement(slot);
                    }
                    npcList.ChangeShowElement(0);
                    BaseUIElement uie = npcList.expandView.InnerUIE[0];
                    uie.Events.LeftDown(uie);
                };
            }
            modList.AddElement(modSlot);
            modList.ChangeShowElement(0);
            BaseUIElement uie = modList.expandView.InnerUIE[0];
            uie.Events.LeftDown(uie);
            npcList.ChangeShowElement(0);
            uie = npcList.expandView.InnerUIE[0];
            uie.Events.LeftDown(uie);
        }
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
        clear.SetPos(-25, 4, 1);
        clear.Events.OnLeftDown += evt => input.ClearText();
        inputBg.Register(clear);

        UIVnlPanel npcBg = new(0, 0);
        npcBg.SetPos(0, top);
        npcBg.SetSize(0, 90, 1);
        npcBg.Info.SetMargin(10);
        bg.Register(npcBg);
        top += npcBg.Height + 10;

        searchNPC = new();
        searchNPC.SetSize(0, 0, 1, 1);
        searchNPC.autoPos[1] = 5;
        npcBg.Register(searchNPC);

        HorizontalScrollbar npcH = new(62);
        npcH.Info.Top.Pixel += 10;
        searchNPC.SetHorizontalScrollbar(npcH);
        npcBg.Register(npcH);

        UIVnlPanel itemBg = new(0, 0);
        itemBg.SetPos(0, top);
        itemBg.SetSize(0, -top, 1, 1);
        itemBg.Info.SetMargin(10);
        bg.Register(itemBg);

        searchItem = new();
        searchItem.SetSize(0, 0, 1, 1);
        searchItem.autoPos[0] = 10;
        itemBg.Register(searchItem);

        VerticalScrollbar itemV = new(110, true);
        itemV.Info.Left.Pixel += 10;
        searchItem.SetVerticalScrollbar(itemV);
        itemBg.Register(itemV);
    }
    private void LookupIndex(int npc)
    {
        indexList.ClearAllElements();
        foreach (string name in NPCShopDatabase.AllShops.Where(x => x.NpcType == npc).Select(x => x.Name))
        {
            string tempName = name;
            if (ShopNames.TryGetValue(npc, out var shops) && shops.TryGetValue(name, out LocalizedText localName))
            {
                tempName = localName.Value;
            }
            UIText shop = new(tempName);
            shop.SetSize(shop.TextSize);
            shop.Events.OnMouseOver += evt => shop.color = Color.Gold;
            shop.Events.OnMouseOut += evt => shop.color = Color.White;
            string shopIndex = name;
            shop.Events.OnLeftDown += evt =>
            {
                range = LookupRange.Normal;
                LookupShop();
            };
            indexList.AddElement(shop);
        }
        indexList.ChangeShowElement(0);
    }
    private void SearchAny(string text)
    {
        searchNPC.ClearAllElements();
        searchItem.ClearAllElements();
        if (text.Length != 0)
        {
            foreach (var (mod, info) in ModsByName)
            {
                var npcAndHead = info.npcAndHead;
                if (npcAndHead == null)
                    continue;
                foreach (var (npc, head) in npcAndHead)
                {
                    if (ContentSamples.NpcsByNetId[npc].TypeName.Contains(text))
                    {
                        UINPCSlot slot = new(npc, head);
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
                            foreach (UINPCSlot npcSlot in npcList.expandView.InnerUIE.Cast<UINPCSlot>())
                            {
                                if (npcSlot.npcType == npcType)
                                {
                                    npcSlot.Events.LeftDown(npcSlot);
                                    break;
                                }
                            }
                        };
                        searchNPC.AddElement(slot);
                    }
                }
            }
            if (searchNPC.InnerUIE.Count == 0)
            {
                UIText none = new(GTV("NoResult"));
                searchNPC.AddElement(none);
            }
            foreach (var shops in NPCShopDatabase.AllShops)
            {
                foreach (var entry in shops.ActiveEntries)
                {
                    if (entry.Item.Name.Contains(text))
                    {
                        string name = shops.Name;
                        UIShopSlot slot = new(entry, shops.NpcType);
                        if (!FlowLayout)
                        {
                            slot.hoverText = $"{ContentSamples.NpcsByNetId[slot.npcType].TypeName} [{name}]";
                        }
                        searchItem.AddElement(slot);
                    }
                }
            }
            foreach (var shops in ExtraShop.extraShops)
            {
                foreach (var entry in shops.ActiveEntries)
                {
                    if (entry.Item.Name.Contains(text))
                    {
                        UIShopSlot slot = new(entry, shops.exType);
                        if (!FlowLayout)
                        {
                            slot.hoverText = GTV("SpecialShop." + shops.exType + ".Label");
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
        if (type > 0)
        {
            indexList.ClearAllElements();
            shopView.ClearAllElements();
            indexList.ChangeShowElement(new UIText(GTV("LookupItem", ContentSamples.ItemsByType[type].Name)));
            foreach (var shops in NPCShopDatabase.AllShops)
            {
                foreach (var entry in shops.ActiveEntries)
                {
                    if (entry.Item.type == type)
                    {
                        string mod = shops.FullName[..shops.FullName.IndexOf('/')];
                        UIShopSlot slot = new(entry, shops.NpcType)
                        {
                            hoverText = ModsByName[mod].mod.DisplayName + "\n" +
                            $"{ContentSamples.NpcsByNetId[shops.NpcType].TypeName} [{shops.Name}]"
                        };
                        shopView.AddElement(slot);
                    }
                }
            }
            foreach (var shops in ExtraShop.extraShops)
            {
                foreach (var entry in shops.ActiveEntries)
                {
                    if (entry.Item.type == type)
                    {
                        UIShopSlot slot = new(entry, shops.exType)
                        {
                            hoverText = GTV("SpecialShop." + shops.exType + ".Label")
                        };
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

    private bool FitsFilter(Item item)
    {
        if (PylonIDs.Contains(item.type))
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
        if (InExShop)
        {
            foreach (ExShop exshop in ExtraShop.extraShops)
            {
                if (range switch
                {
                    LookupRange.Normal => npcList.ShowUIE is UIExShopSlot slot && slot.exShopType == exshop.exType,
                    _ => true
                })
                {
                    foreach (var entry in exshop.ActiveEntries)
                    {
                        if (!FitsFilter(entry.Item))
                            continue;
                        UIShopSlot slot = new(entry, exshop.exType);
                        shopView.AddElement(slot);
                    }
                }
            }
        }
        else
        {
            foreach (AbstractNPCShop shop in NPCShopDatabase.AllShops)
            {
                int npc = shop.NpcType;
                int current = npcList.ShowUIE.npcType;
                if (range switch
                {
                    LookupRange.Normal => npc == current && shop.Name == indexList.ShowUIE.text,
                    LookupRange.NPCAll => npc == current,
                    LookupRange.ModAll => ModsByName[modList.ShowUIE.modName].npcAndHead.ContainsKey(npc),
                    _ => true
                })
                {
                    if (onlyCanBuy)
                    {
                        if (shop.TryGetCanBuyEntrys(out Item[] contents))
                        {
                            foreach (Item item in contents)
                            {
                                if (!FitsFilter(item))
                                    continue;
                                UIShopSlot slot = new(item);
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
                            UIShopSlot slot = new(entry, shop.NpcType);
                            shopView.AddElement(slot);
                        }
                    }
                }
            }
        }
        shopView.Calculation();
    }
}
