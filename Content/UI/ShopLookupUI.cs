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
    private UIContainerPanel View_Shop => views[2];
    /// <summary>
    /// 物品查询和特殊商店
    /// </summary>
    private UIContainerPanel View_Item => views[3];
    private UIItemSlot focusItem;
    private List<string> tempIndex;
    public override void OnInitialization()
    {
        base.OnInitialization();
        RemoveAll();

        tempIndex = [];

        UIPanel bg = new(500, 360);
        bg.SetCenter(0, 0, 0.5f, 0.5f);
        bg.Info.SetMargin(20);
        bg.canDrag = true;
        Register(bg);

        focusItem = new();
        bg.Register(focusItem);

        views = [new(), new(), new(), new()];

        UIImage npcLine = new(TextureAssets.MagicPixel.Value);
        npcLine.SetSize(-40, 2, 1);
        npcLine.SetPos(20, 68);
        bg.Register(npcLine);

        View_NPC.SetSize(-144, 72, 1);
        View_NPC.SetPos(72, 0);
        View_NPC.SetEdgeBlur(1, 20, 0);
        //View_NPC.DrawRec[0] = Color.Black;
        bg.Register(View_NPC);

        HorizontalScrollbar npcScroll = new(62);
        npcScroll.Info.Top.Pixel += 3;
        View_NPC.SetHorizontalScrollbar(npcScroll);

        UIImage indexLine = new(TextureAssets.MagicPixel.Value);
        indexLine.SetSize(-40, 2, 1);
        indexLine.SetPos(20, 101);
        bg.Register(indexLine);

        UIText index = new(GTV("Index"), drawStyle: 1);
        index.SetSize(52, 30);
        index.SetPos(0, 72);
        index.Events.OnMouseOver += evt => index.color = Color.Gold;
        index.Events.OnMouseOut += evt => index.color = Color.White;
        bg.Register(index);

        View_Index.SetSize(-144, 40, 1);
        View_Index.SetPos(72, 72);
        //View_Index.DrawRec[0] = Color.Red;
        View_Index.Info.IsVisible = false;
        bg.Register(View_Index);

        HorizontalScrollbar indexScroll = new(62);
        indexScroll.Info.Top.Pixel += 5;
        View_Index.SetHorizontalScrollbar(indexScroll);

        View_Shop.SetSize(0, -112, 1, 1);
        View_Shop.SetPos(0, 112);
        //View_Item.DrawRec[0] = Color.Red;
        View_Shop.Info.IsVisible = false;
        View_Shop.autoPos[0] = true;
        View_Shop.SetEdgeBlur(0, 0, 20);
        bg.Register(View_Shop);

        VerticalScrollbar shopScroll = new(80);
        shopScroll.Info.Left.Pixel += 8;
        View_Shop.SetVerticalScrollbar(shopScroll);

        View_Item.SetSize(0, -72, 1, 1);
        View_Item.SetPos(0, 82);
        View_Item.Info.IsVisible = false;
        //View_Shop.DrawRec[0] = Color.Red;
        View_Item.autoPos[0] = true;
        View_Item.SetEdgeBlur(0, 0, 20);
        bg.Register(View_Item);

        VerticalScrollbar itemScroll = new(80);
        itemScroll.Info.Left.Pixel += 8;
        View_Item.SetVerticalScrollbar(itemScroll);

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
        List<Mod> mods = modIndex == 0 ? [.. ModNPCs.Keys] : [ModID[modIndex - 1]];
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
                UIText name = new(shop.Name);
                name.SetPos(x + 2, 0);
                name.SetSize(name.TextSize + Vector2.UnitX * 2);
                var evt = name.Events;
                evt.OnLeftDown += uie => LookupNPC(uie.id);
                evt.OnMouseOver += uie => name.color = Color.Gold;
                evt.OnMouseOut += uie => name.color = Color.White;
                View_Index.AddElement(name);
                x += name.Width + 20;
                i++;
            }
        }
        LookupNPC(0);
    }
    private void LookupNPC(int index)
    {
        if (NPCShopDatabase.TryGetNPCShop(tempIndex[index], out AbstractNPCShop shop))
        {
            View_Shop.ClearAllElements();
            var entrys = shop.ActiveEntries.Where(x => !Pylons.Contains(x.Item.type) && x.Item.type > ItemID.None);
            int count = entrys.Count(), i = 0, y = 0;
            foreach (AbstractNPCShop.Entry entry in entrys)
            {
                UIShopSlot slot = new(entry, shop.NpcType, ++i == count);
                slot.SetPos(0, y);
                //slot.DrawRec[0] = Color.White;
                View_Shop.AddElement(slot);
                y += slot.Height;
            }
        }
    }
    private void SwitchView(bool npc)
    {
        View_Index.ClearAllElements();
        View_Shop.ClearAllElements();
        View_Item.ClearAllElements();
        if (npc)
        {
            View_Index.Info.IsVisible = true;
            View_Shop.Info.IsVisible = true;
            View_Item.Info.IsVisible = false;
        }
        else
        {
            View_Index.Info.IsVisible = false;
            View_Shop.Info.IsVisible = false;
            View_Item.Info.IsVisible = true;
        }
    }
    public void ExtraDrawInfo(SpriteBatch sb)
    {
    }
}
