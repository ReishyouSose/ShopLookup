using ShopLookup.Content.UI.ExtraUI;
using System.Linq;
using static RUIModule.RUISys.AssetLoader;
using static ShopLookup.Content.Sys.ShopNPCData;

namespace ShopLookup.Content.UI;

public class ShopLookupUI : ContainerElement
{
    private int modIndex;
    private UIContainerPanel[] views;
    private UIContainerPanel View_NPC => views[0];
    /// <summary>
    /// NPC商店索引
    /// </summary>
    private UIContainerPanel View_Index => views[1];
    /// <summary>
    /// NPC商店视图
    /// </summary>
    private UIContainerPanel View_Item => views[2];
    /// <summary>
    /// 用于非NPC商店
    /// </summary>
    private UIContainerPanel View_Shop => views[3];
    private UIItemSlot focusItem;
    private List<string> tempIndex;
    public override void OnInitialization()
    {
        base.OnInitialization();
        RemoveAll();

        tempIndex = new();

        UIPanel bg = new(500, 360);
        bg.SetCenter(0, 0, 0.5f, 0.5f);
        bg.Info.SetMargin(20);
        bg.canDrag = true;
        Register(bg);

        focusItem = new();
        bg.Register(focusItem);

        views = [new(), new(), new(), new()];

        View_NPC.SetSize(-144, 72, 1);
        View_NPC.SetPos(72, 0);
        View_NPC.DrawRec[0] = Color.Black;
        bg.Register(View_NPC);

        HorizontalScrollbar npcScroll = new(62);
        npcScroll.Info.Top.Pixel += 3;
        View_NPC.SetHorizontalScrollbar(npcScroll);

        View_Index.SetSize(0, 30, 1);
        View_Index.SetPos(0, 72);
        View_Index.DrawRec[0] = Color.Red;
        View_Index.Info.IsVisible = false;
        bg.Register(View_Index);

        HorizontalScrollbar indexScroll = new(62);
        indexScroll.Info.Top.Pixel += 3;
        View_Index.SetHorizontalScrollbar(indexScroll);

        View_Item.SetSize(0, -112, 1, 1);
        View_Item.SetPos(0, 102);
        View_Item.DrawRec[0] = Color.Red;
        View_Item.Info.IsVisible = false;
        bg.Register(View_Item);

        VerticalScrollbar shopScroll = new(80);
        View_Item.SetVerticalScrollbar(shopScroll);

        View_Shop.SetSize(0, -82, 1, 1);
        View_Shop.SetPos(0, 82);
        View_Shop.Info.IsVisible = false;
        View_Shop.DrawRec[0] = Color.Red;
        bg.Register(View_Shop);

        VerticalScrollbar itemScroll = new(80);
        View_Shop.SetVerticalScrollbar(itemScroll);

        UIImage filter = new(ExtraAssets["Slot"]);
        filter.SetPos(-52, 0, 1);
        filter.hoverText = GTV("Filter", "ALL") + "\n" + GTV("Switch");
        filter.Events.OnLeftDown += uie => SwitchMod(uie, false);
        filter.Events.OnRightDown += uie => SwitchMod(uie, true);
        bg.Register(filter);

        UIAdjust adjust = new(null);
        adjust.SetPos(0, 0, 1, 1);
        bg.Register(adjust);
    }
    public override void Update(GameTime gt)
    {
        base.Update(gt);
    }

    private void SwitchMod(BaseUIElement filter, bool reverse)
    {
        int count = ModID.Count + 1;
        if (reverse)
        {
            modIndex--;
            if (modIndex < 0) modIndex = count - 1;
        }
        else modIndex = ++modIndex % count;
        filter.hoverText = $"[{modIndex + 1}/{count}]" + "\n" + GTV("Filter",
            modIndex == 0 ? "All" : ModID[modIndex - 1].DisplayName ?? "Terraria") + "\n" + GTV("Switch");
        ReLoadNPCView();
    }
    public void ReLoadNPCView()
    {
        View_NPC.ClearAllElements();
        List<Mod> mods = modIndex == 0 ? ModNPCs.Keys.ToList() : [ModID[modIndex - 1]];
        int x = 0;
        foreach (var mod in mods)
        {
            foreach (int type in ModNPCs[mod])
            {
                UINPCSlot slot = new(type, mod);
                slot.SetPos(x, 0);
                slot.Events.OnLeftDown += evt => ChangeShopView_NPC(slot.npcType);
                View_NPC.AddElement(slot);
                x += 62;
            }
        }
    }
    private void ChangeShopView_NPC(int type)
    {
        tempIndex.Clear();
        SwitchView(true);
        int i = 0;
        int x = 0;
        foreach (AbstractNPCShop shop in NPCShopDatabase.AllShops)
        {
            if (shop.NpcType == type)
            {
                tempIndex.Add(shop.FullName);
                UIText name = new(shop.Name, drawStyle: 0);
                name.SetPos(x, 0);
                name.SetSize(name.TextSize);
                name.Events.OnLeftDown += uie => LookupNPC(uie.id);
                View_Index.AddElement(name);
                x += name.Width + 10;
                i++;
            }
        }
        LookupNPC(0);
    }
    private void LookupNPC(int index)
    {
        if (NPCShopDatabase.TryGetNPCShop(tempIndex[index], out AbstractNPCShop shop))
        {
            View_Item.ClearAllElements();
            int count = shop.ActiveEntries.Count();
            int y = 0;
            foreach (AbstractNPCShop.Entry entry in shop.ActiveEntries)
            {
                UIShopSlot slot = new(entry);
                slot.SetPos(0, y);
                slot.DrawRec[0] = Color.White;
                View_Item.AddElement(slot);
                y += 110;
            }
        }
    }
    private void SwitchView(bool npc)
    {
        View_Index.ClearAllElements();
        View_Item.ClearAllElements();
        View_Shop.ClearAllElements();
        if (npc)
        {
            View_Index.Info.IsVisible = true;
            View_Item.Info.IsVisible = true;
            View_Shop.Info.IsVisible = false;
        }
        else
        {
            View_Index.Info.IsVisible = false;
            View_Item.Info.IsVisible = false;
            View_Shop.Info.IsVisible = true;
        }
    }
    public void ExtraDrawInfo(SpriteBatch sb)
    {
    }
}
