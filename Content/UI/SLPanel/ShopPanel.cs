using ShopLookup.Content.Sys;
using ShopLookup.Content.UI.ExtraUI;
using System.Linq;
using System.Text;
using static ShopLookup.Content.Data.ShopNPCData;

namespace ShopLookup.Content.UI.SLPanel;

public partial class SLPanel : ContainerElement
{
    private UIContainerPanel shopView;
    private UIBottom shopPanel;
    private UIItemSlot focus;
    private UIDropDownList<UIShopName> indexList;
    private UIDropDownList<UIModSlot> modList;
    private UIDropDownList<UIShopSlot> shopList;
    private UIInputBox searcher;
    private List<UIItemFilter> filters;
    private bool anyFilterActive;
    private bool onlyCanBuy;
    private bool flowLayout;
    public override bool SkipInMenu => true;
    public override void OnInitialization()
    {
        base.OnInitialization();
        RemoveAll();
        savingsHide = [];
        if (filters != null)
        {
            foreach (UIItemFilter filter in filters)
            {
                filter.Active = false;
            }
        }
        anyFilterActive = false;
        onlyCanBuy = false;
        flowLayout = false;

        UIVnlPanel bg = new(535, 40 * 9 + 10);
        bg.Info.SetMargin(10);
        bg.SetCenter(0, 0, 0.5f, 0.5f);
        bg.canDrag = true;
        Register(bg);

        int right = -40;

        shopPanel = new(right, 0, 1, 1);
        bg.Register(shopPanel);
        RegisterShopPanel(shopPanel);

        searchPanel = new(right, 0, 1, 1);
        searchPanel.Info.IsVisible = false;
        bg.Register(searchPanel);
        ReigsterSearchPanel(searchPanel);

        sellPanel = new(right, 0, 1, 1);
        sellPanel.Info.IsVisible = false;
        bg.Register(sellPanel);
        RegisterSellPanel(sellPanel);

        RegisterSavings(bg);
        RegisterStackPanel();

        Texture2D UIButton = AssetLoader.ExtraAssets["UIButton"];

        int top = 0;
        UIClose close = new(UIButton) { scissors = new(0, 0, 60, 30) };
        close.SetPos(-30, top, 1);
        close.SetSize(30, 30);
        close.Events.OnLeftDown += evt =>
        {
            Info.IsVisible = false;
            SLPlayer.SLTime = Main.GameUpdateCount;
            ClearSell();
        };
        bg.Register(close);
        top += 40;

        UIMove move = new(UIButton)
        {
            scissors = new(0, 3 * 30, 90, 30),
            hoverText = GTV("UIButton.Move")
        };
        move.SetPos(-30, top, 1);
        move.SetSize(30, 30);
        bg.Register(move);
        top += 40;

        UI3FrameImage shop = new(UIButton, _ => shopPanel.IsVisible)
        {
            scissors = new(0, 7 * 30, 90, 30),
            hoverText = GTV("UIButton.Shop")
        };
        shop.SetPos(-30, top, 1);
        shop.SetSize(30, 30);
        shop.Events.OnLeftDown += evt => ChangePanel(0);
        bg.Register(shop);
        top += 40;

        UI3FrameImage search = new(UIButton, _ => searchPanel.IsVisible)
        {
            scissors = new(0, 6 * 30, 90, 30),
            hoverText = GTV("UIButton.Search")
        };
        search.SetPos(-30, top, 1);
        search.SetSize(30, 30);
        search.Events.OnLeftDown += evt => ChangePanel(1);
        bg.Register(search);
        top += 40;

        UI3FrameImage sell = new(UIButton, _ => sellPanel.IsVisible)
        {
            scissors = new(0, 5 * 30, 90, 30),
            hoverText = GTV("UIButton.Sell")
        };
        sell.SetPos(-30, top, 1);
        sell.SetSize(30, 30);
        sell.Events.OnLeftDown += evt => ChangePanel(2);
        bg.Register(sell);
        top += 40;

        UI3FrameImage strip = new(UIButton, _ => !flowLayout)
        {
            scissors = new(0, 30, 90, 30),
            hoverText = GTV("UIButton.Strip", GTV("UIButton.OnlyCanBuy"))
        };
        strip.SetPos(-30, top, 1);
        strip.SetSize(30, 30);
        strip.Events.OnLeftDown += evt =>
        {
            if (!onlyCanBuy)
                ChangeLayout(false);
        };
        bg.Register(strip);
        top += 40;

        UI3FrameImage flow = new(UIButton, _ => flowLayout)
        {
            scissors = new(0, 4 * 30, 90, 30),
            hoverText = GTV("UIButton.Flow")
        };
        flow.SetPos(-30, top, 1);
        flow.SetSize(30, 30);
        flow.Events.OnLeftDown += evt => ChangeLayout(true);
        bg.Register(flow);
        top += 40;

        UI3FrameImage stack = new(UIButton, _ => stackPanel.IsVisible)
        {
            scissors = new(0, 8 * 30, 90, 30),
        };
        stack.SetPos(-30, top, 1);
        stack.SetSize(30, 30);
        stack.Events.OnMouseOver += evt =>
        {
            StringBuilder build = new(GTV("UIButton.Stack"));
            build.AppendLine();
            build.Append(GTV("UIButton.StackCurrent"));
            build.Append(stacker.text);
            evt.hoverText = build.ToString();
        };
        stack.Events.OnLeftDown += evt => stackPanel.Info.IsVisible = !stackPanel.IsVisible;
        bg.Register(stack);

        stackPanel.SetPos(bg.Right + 10, bg.Bottom - stackPanel.Height);
        bg.Events.PostCalculation += evt => stackPanel.SetPos(bg.Right + 10, bg.Bottom - stackPanel.Height);

        UIAdjust adjust = new(UIButton)
        {
            scissors = new(0, 60, 90, 30),
            hoverText = GTV("UIButton.Adjust")
        };
        adjust.SetSize(30, 30);
        bg.Register(adjust);

        HoverHidden();
    }
    public override void Update(GameTime gt)
    {
        UIShopItem.HoverSlot = null;
        base.Update(gt);
        UpdateSavingsColor();
    }
    public override void OnCloseByInv()
    {
        SLPlayer.SLTime = Main.GameUpdateCount;
        ClearSell();
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
            LookupShop();
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
            filter.scissors = new((x + y * 5) * 30, 0, 30, 30);
            filter.SetPos(left + 36 * x++, y * 36);
            filter.SetSize(30, 30);
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
        other.scissors = new(300, 0, 30, 30);
        other.SetPos(left + 180, 0);
        other.SetSize(30, 30);
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


        UIImage only = new(AssetLoader.ExtraAssets["Filter"]) { scissors = new(330, 0, 30, 30) };
        only.SetPos(left + 180, 36);
        only.SetSize(30, 30);
        only.Events.OnLeftDown += evt =>
        {
            onlyCanBuy = !onlyCanBuy;
            only.color = onlyCanBuy ? Color.Gold : Color.White;
            ChangeLayout(true);
        };
        only.Events.OnRightDown += evt =>
        {
            shopView.ClearAllElements();
            ChangeLayout(true, false);
            Chest.SetupTravelShop();
            foreach (int itemID in Main.travelShop)
            {
                if (itemID > ItemID.None)
                    shopView.AddElement(new UIShopItem(ContentSamples.ItemsByType[itemID], true) { OnlyCanBuy = true });
            }
        };
        only.Events.OnMouseOver += evt =>
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
            evt.hoverText += "\n" + GTV("UIButton.TravelMerchant");
        };
        bg.Register(only);

        shopView = new();
        shopView.SetSize(-30, 0, 1, 1);
        shopView.autoPos = [10, null];
        shopBg.Register(shopView);

        indexList = new(bg, shopView, x => x.Clone());
        indexList.SetWhellPixel(30);
        savingsHide.Add(indexList.showArea);

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
    /// <summary>
    /// 0买1搜2卖
    /// </summary>
    /// <param name="panel"></param>
    private void ChangePanel(int panel)
    {
        shopPanel.Info.IsVisible = panel == 0;
        searchPanel.Info.IsVisible = panel == 1;
        sellPanel.Info.IsVisible = panel == 2;
    }
    private void ChangeLayout(bool flow, bool reLoadView = true)
    {
        flowLayout = flow;
        shopView.autoPos = searchItem.autoPos = [10, flowLayout ? 10 : null];
        shopView.Vscroll.WheelPixel = searchItem.Vscroll.WheelPixel = flowLayout ? 62 : 110;
        if (reLoadView)
        {
            LookupShop();
            SearchAny(searcher.Text);
        }
    }
}
