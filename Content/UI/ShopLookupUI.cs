using ShopLookup.Content.UI.ExtraUI;
using System.Linq;
using static RUIModule.RUISys.AssetLoader;
using static ShopLookup.Content.Sys.ShopNPCData;

namespace ShopLookup.Content.UI;

public class ShopLookupUI : ContainerElement
{
    private int modIndex;
    private UIContainerPanel npcs;
    private List<string> tempIndex;
    private UIBottom area;
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

        npcs = new();
        npcs.SetSize(-144, 72, 1);
        npcs.SetPos(72, 0);
        npcs.Info.SetMargin(0);
        bg.Register(npcs);

        HorizontalScrollbar scroll = new(62);
        scroll.Info.Top.Pixel += 3;
        npcs.SetHorizontalScrollbar(scroll);

        UIImage filter = new(ExtraAssets["Slot"]);
        filter.SetPos(-52, 0, 1);
        filter.hoverText = GTV("Filter", "ALL") + "\n" + GTV("Switch");
        filter.Events.OnLeftDown += uie => SwitchMod(uie, false);
        filter.Events.OnRightDown += uie => SwitchMod(uie, true);
        bg.Register(filter);

        UIAdjust adjust = new(null);
        adjust.SetPos(0, 0, 1, 1);
        bg.Register(adjust);

        area = new(0, -72, 1, 1);
        area.SetPos(0, 72);
        area.DrawRec[0] = Color.Red;
        bg.Register(area);
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
        npcs.ClearAllElements();
        int x = 0;
        if (modIndex == 0)
        {
            foreach (var (mod, mnpcs) in ModNPCs)
            {
                foreach (int type in mnpcs)
                {
                    UINPCSlot slot = new(type, mod);
                    slot.SetPos(x, 0);
                    slot.Events.OnLeftDown += evt => { int t = type; ChangeShopView_NPC(t); };
                    slot.Events.OnRightDown += evt => { int t = type; ChangeShopView_NPC(t); };
                    npcs.AddElement(slot);
                    x += 62;
                }
            }
        }
        else
        {
            Mod mod = ModID[modIndex - 1];
            foreach (int type in ModNPCs[mod])
            {
                UINPCSlot Hslot = new(type, mod);
                Hslot.SetPos(x, 0);
                npcs.AddElement(Hslot);
                x += 62;
            }
        }
    }
    private void ChangeShopView_NPC(int type)
    {
        tempIndex.Clear();
        area.RemoveAll();

        UIContainerPanel indexView = new();
        indexView.SetSize(0, 30, 1);
        area.Register(indexView);

        HorizontalScrollbar scroll = new();
        scroll.Info.Top.Pixel += 3;

        int x = 0;
        foreach (AbstractNPCShop shop in NPCShopDatabase.AllShops)
        {
            if (shop.NpcType == type)
            {
                tempIndex.Add(shop.FullName);
                UIText name = new(shop.Name, drawStyle: 0);
                name.SetPos(x, 0);
            }
        }
        LookupNPC(0);
    }
    private void LookupNPC(int index)
    {
        if (NPCShopDatabase.TryGetNPCShop(tempIndex[index], out AbstractNPCShop shop))
        {
            int count = shop.ActiveEntries.Count();
            int y = 0;
            foreach (AbstractNPCShop.Entry entry in shop.ActiveEntries)
            {
                y++;
            }
        }
    }
    public void ExtraDrawInfo(SpriteBatch sb)
    {

    }
}
