using RUIModule.RUIElements;
using ShopLookup.Content.UI.ExtraUI;
using static RUIModule.RUISys.AssetLoader;
using static ShopLookup.Content.Sys.ShopNPCData;

namespace ShopLookup.Content.UI;

public class ShopLookupUI : ContainerElement
{
    private int modIndex;
    private UIContainerPanel npcs;
    public override void OnInitialization()
    {
        base.OnInitialization();

        UIPanel bg = new(500, 360);
        bg.SetCenter(0, 0, 0.5f, 0.5f);
        bg.Info.SetMargin(20);
        bg.canDrag = true;
        Register(bg);

        npcs = new();
        npcs.SetSize(-144, 52, 1);
        npcs.SetPos(72, 0);
        npcs.Info.SetMargin(0);
        bg.Register(npcs);

        HorizontalScrollbar hscroll = new(62, false);
        hscroll.SetPos(72, 62);
        npcs.SetHorizontalScrollbar(hscroll);
        bg.Register(hscroll);

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

    private void SwitchMod(BaseUIElement filter, bool reverse)
    {
        int count = ModID.Count + 1;
        if (reverse)
        {
            modIndex--;
            if (modIndex < 0) modIndex = count - 1;
        }
        else modIndex = ++modIndex % count;
        filter.hoverText = $"[{modIndex + 1}/{count}]" + "\n" +
             GTV("Filter", modIndex == 0 ? "All" : ModID[modIndex - 1].DisplayName) + "\n" + GTV("Switch");
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
                UINPCSlot slot = new(type, mod);
                slot.SetPos(x, 0);
                npcs.AddElement(slot);
                x += 62;
            }
        }
    }
    public void ExtraDrawInfo(SpriteBatch sb)
    {

    }
}
