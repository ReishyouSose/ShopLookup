using ShopLookup.Content.Data;
using ShopLookup.Content.Sys;
using ShopLookup.Content.UI.ExtraUI;
using System.Linq;
using static ShopLookup.Content.Data.ShopNPCData;

namespace ShopLookup.Content.UI;

public class ReBuild : ContainerElement
{
    private UIContainerPanel filter, search, shop;
    private UIFocusSlot focus;
    private UIDropDownList<UIText> index;
    public override void OnInitialization()
    {
        base.OnInitialization();
        if (Main.gameMenu) return;
        RemoveAll();

        UIVnlPanel bg = new(500, 300);
        bg.Info.SetMargin(10);
        bg.SetCenter(0, 0, 0.5f, 0.5f);
        Register(bg);

        UIVnlPanel fltBg = new(0, 0);
        fltBg.SetPos(-122, 62, 1);
        fltBg.SetSize(122, -62, 0, 1);
        fltBg.Info.SetMargin(10);
        bg.Register(fltBg);

        focus = new();
        focus.Events.OnLeftDown += evt =>
        {
            if (focus.HasFocus)
            {
                ClearLookup();
            }
            else
            {
                if (Main.HoverItem?.type > ItemID.None)
                {
                    focus.ChangeFocus(Main.HoverItem);
                }
            }
        };
        bg.Register(focus);

        UIVnlPanel inputBg = new(122, 28);
        inputBg.SetPos(-122, 24, 1);
        bg.Register(inputBg);

        UIInputBox input = new(GTV("SearchNPC"), color: Color.White);
        input.SetSize(0, 0, 1, 1);
        input.OnInputText += FindNPC;
        inputBg.Register(input);

        UIClose clear = new();
        clear.SetPos(-25, 4, 1);
        clear.Events.OnLeftDown += evt => input.ClearText();
        inputBg.Register(clear);

        filter = new();
        filter.SetSize(0, 0, 1, 1);
        fltBg.Register(filter);

        VerticalScrollbar fscroll = new(52, true, false);
        fscroll.Info.Left.Pixel += 10;
        filter.SetVerticalScrollbar(fscroll);
        fltBg.Register(fscroll);

        search = new();
        search.SetSize(0, 0, 1, 1);
        search.autoPos[0] = true;
        search.spaceY = 10;
        search.Info.IsVisible = false;
        fltBg.Register(search);

        VerticalScrollbar sscroll = new(52, true, false);
        sscroll.Info.Left.Pixel += 10;
        sscroll.Info.IsVisible = false;
        search.SetVerticalScrollbar(sscroll);
        fltBg.Register(sscroll);

        int modCount = ModID.Count - 1;
        foreach (var (id, mod) in ModID)
        {
            UIIconSlot modSlot = new(ModIcon[id], 6)
            {
                hoverText = mod.DisplayName ?? "Terraria"
            };
            modSlot.Events.OnLeftClick += ReCalculateFilterPanel;
            filter.AddElement(modSlot);

            UIBottom npcBg = new(82, 62 * Math.Min(ModNPCs[mod].Count, 3) - 10);
            npcBg.Info.IsVisible = false;
            filter.AddElement(npcBg);

            UIContainerPanel npcView = new();
            npcView.SetSize(0, 0, 1, 1);
            npcView.Events.OnMouseOver += evt =>
            {
                if (npcView.MovableSize.Y > 0)
                    filter.Info.CanBeInteract = false;
            };
            npcView.Events.OnMouseOut += evt => filter.Info.CanBeInteract = true;
            npcBg.Register(npcView);

            VerticalScrollbar vs = new(62);
            vs.Info.Left.Set(0, 0);
            npcView.SetVerticalScrollbar(vs);
            npcBg.Register(vs);

            int i = 0;
            if (id == modCount)
            {
                foreach (ExShop exShop in ExtraShop.extraShops)
                {
                    UIIconSlot slot = new(exShop.icon);
                    slot.SetPos(20, i++ * 62);
                    slot.Events.OnLeftDown += evt =>
                    {
                        ClearLookup();
                    };
                    npcView.AddElement(slot);
                }
            }
            else
            {
                foreach (int npc in ModNPCs[mod])
                {
                    UINPCSlot slot = new(npc, mod);
                    slot.SetPos(20, i++ * 62);
                    slot.Events.OnLeftDown += evt =>
                    {
                        LookupIndex(slot.npcType);
                        LookupNPCShop(slot.npcType);
                    };
                    npcView.AddElement(slot);
                }
            }
        }
        ReCalculateFilterPanel(null);

        UIVnlPanel shopBg = new(0, 0);
        shopBg.SetPos(0, 62);
        shopBg.SetSize(-132, -62, 1, 1);
        shopBg.Info.SetMargin(10);
        bg.Register(shopBg);

        shop = new();
        shop.SetSize(0, 0, 1, 1);
        shop.autoPos[0] = true;
        shop.spaceY = 10;
        shopBg.Register(shop);

        VerticalScrollbar shopvsl = new(110, true);
        shopvsl.Info.Left.Pixel += 10;
        shop.SetVerticalScrollbar(shopvsl);
        shopBg.Register(shopvsl);

        index = new(shop, x =>
        {
            UIText uie = new(x.text);
            uie.SetPos(10, 5);
            return uie;
        });
        index.SetSize(0, 0, 1, 1);
        index.SetWhellPixel(30);

        index.showArea.SetPos(62, 24);
        index.showArea.SetSize(-194, 28, 1);

        index.expandArea.SetPos(62, 62);
        index.expandArea.SetSize(-194, 82 + 20, 1);

        index.expandView.Info.Height.Pixel -= 10;
        index.expandView.autoPos[0] = true;

        HorizontalScrollbar hsl = new(null, false, true)
        {
            useScrollWheel = false
        };
        hsl.Info.Width.Pixel -= 20;
        hsl.Info.Top.Pixel += 15;
        index.expandView.SetHorizontalScrollbar(hsl);
        index.expandArea.Register(hsl);

        bg.Register(index);

        UIClose close = new();
        close.SetPos(-20, 0, 1);
        close.Events.OnLeftDown += evt => Info.IsVisible = false;
        bg.Register(close);

        UIMove move = new() { hoverText = GTV("UIButton.Move") };
        move.SetPos(-50, 0, 1);
        move.Events.OnMouseOver += evt => inputBg.Info.IsVisible = false;
        move.Events.OnMouseOut += evt => inputBg.Info.IsVisible = true;
        bg.Register(move);

        UIAdjust adjust = new(AssetLoader.VnlAdjust) { hoverText = GTV("UIButton.Adjust") };
        adjust.SetPos(-80, 0, 1);
        adjust.Events.OnMouseOver += evt => inputBg.Info.IsVisible = false;
        adjust.Events.OnMouseOut += evt => inputBg.Info.IsVisible = true;
        bg.Register(adjust);
    }
    private void ReCalculateFilterPanel(BaseUIElement uie)
    {
        int y = 0;
        float yp = filter.Vscroll.WheelValue * filter.Vscroll.ViewMovableY;
        foreach (BaseUIElement e in filter.InnerUIE)
        {
            e.SetPos(0, y, cal: false);
            if (e is UIIconSlot slot)
            {
                ref int id = ref slot.slotID;
                if (e == uie)
                {
                    id = id == 14 ? 6 : 14;
                }
                else id = 6;
            }
            if (e is UIBottom)
            {
                if (e.id - 1 == uie?.id)
                {
                    e.Info.IsVisible = !e.IsVisible;
                }
                else e.Info.IsVisible = false;
                if (e.IsVisible)
                {
                    y += e.Height;
                }
            }
            else y += e.Height;
            y += 10;
        }
        filter.Calculation();
        filter.Vscroll.ForceSetPixel(yp);
    }
    private void LookupIndex(int npc, string target = "")
    {
        focus.ChangeFocus(ContentSamples.NpcsByNetId[npc]);
        index.ClearAllElements();
        bool change = true;
        foreach (string name in NPCShopDatabase.AllShops.Where(x => x.NpcType == npc).Select(x => x.Name))
        {
            UIText shop = new(name);
            shop.SetSize(shop.TextSize);
            shop.Events.OnMouseOver += evt => shop.color = Color.Gold;
            shop.Events.OnMouseOut += evt => shop.color = Color.White;
            shop.Events.OnLeftDown += evt => LookupNPCShop(npc, shop.text);
            index.AddElement(shop);
            if (name == target || change)
            {
                index.ChangeShowElement(shop);
                change = false;
            }
        }
    }
    private void LookupNPCShop(int npc, string name = null, int itemTarget = -1)
    {
        shop.ClearAllElements();
        int id = 0;
        bool firster(AbstractNPCShop x) => x.NpcType == npc && (name is null || name == x.Name);
        bool wherer(AbstractNPCShop.Entry x) => !Pylons.Select(y => y.Item.type).Contains(x.Item.type) && x.Item.type > ItemID.None;
        foreach (AbstractNPCShop.Entry entry in NPCShopDatabase.AllShops.First(firster).ActiveEntries.Where(wherer))
        {
            UIShopSlot slot = new(entry, npc);
            shop.AddElement(slot);
            if (entry.Item.type == itemTarget) id = slot.id;
        }
        shop.Calculation();
        if (itemTarget >= 0)
        {
            shop.Vscroll.ForceSetPixel(shop.InnerUIE[id].Info.Top.Pixel);
        }
    }
    private void ClearLookup()
    {
        index.ClearAllElements();
        shop.ClearAllElements();
    }
    private void FindNPC(string text)
    {
        if (text.Any())
        {
            search.ClearAllElements();
            search.Info.IsVisible = true;
            search.Vscroll.Info.IsVisible = true;
            filter.Info.IsVisible = false;
            filter.Vscroll.Info.IsVisible = false;
            foreach (var (mod, npcs) in ModNPCs)
            {
                foreach (int npc in npcs)
                {
                    if (ContentSamples.NpcsByNetId[npc].TypeName.Contains(text))
                    {
                        UINPCSlot slot = new(npc, mod);
                        slot.hoverText += "\n" + GTV("Source", " " + (mod.DisplayName ?? "Terraria"));
                        slot.Events.OnLeftDown += evt =>
                        {
                            LookupIndex(slot.npcType);
                            LookupNPCShop(slot.npcType);
                        };
                        search.AddElement(slot);
                    }
                }
            }
        }
        else
        {
            filter.Info.IsVisible = true;
            filter.Vscroll.Info.IsVisible = true;
            search.Info.IsVisible = false;
            search.Vscroll.Info.IsVisible = false;
        }
    }
    public void LookupItem(Item item)
    {
        if (ChildrenElements[0].ContainsPoint(Main.MouseScreen)) return;
        if (item?.type > 0)
        {
            focus.ChangeFocus(item);
            index.ClearAllElements();
            index.ChangeShowElement(new(GTV("Navigate")));
            shop.ClearAllElements();
            foreach (var shops in NPCShopDatabase.AllShops.Concat(ExtraShop.extraShops.Select(x => x.shop)))
            {
                foreach (var entry in shops.ActiveEntries)
                {
                    if (entry.Item.type == item.type)
                    {
                        string name = shops.Name;
                        UIShopSlot slot = new(entry, shops.NpcType);
                        slot.hoverText = $"{ContentSamples.NpcsByNetId[slot.npcType].TypeName} [{name}]";
                        slot.Events.OnRightClick += evt =>
                        {
                            LookupIndex(slot.npcType, name);
                            LookupNPCShop(slot.npcType, name, slot.itemSlot.ContainedItem.type);
                        };
                        shop.AddElement(slot);
                    }
                }
            }
            if (!shop.InnerUIE.Any())
            {
                UIText none = new(GTV("NoSell"));
                shop.AddElement(none);
            }
        }
    }
    private void LookupExShop(ExType exType)
    {

    }
    public override void OnSaveAndQuit()
    {
        Info.IsVisible = false;
        SLPlayer.loaded = false;
    }
}
