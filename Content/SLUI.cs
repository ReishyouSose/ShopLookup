using Terraria.Localization;

namespace ShopLookup.Content
{
    public class SLUI : ContainerElement
    {
        public const string LocalKey = "Mods.ShopLookup.";
        public const string NmakeKey = "ShopLookup.Content.SLUI";
        public const string AssetKey = "ShopLookup/UISupport/Asset/";
        public static Texture2D LineTex => TextureAssets.MagicPixel.Value;
        public UIPanel bg;
        public UIItemSlot focusItem;
        public ShopNPCSlot focusNPC;
        public TextUIE indexText;
        public UIImage _switch;
        public UIBottom ItemBg, side, ShopBg, indexBg;
        public UIContainerPanel view, indexView;
        public bool firstLoad;
        private int FocusMod;
        private static bool noIcon;
        internal static List<int> pylons;
        internal static Dictionary<int, string> added;
        internal static List<(string name, string inName)> mods;
        internal static int modCount;
        public override void OnInitialization()
        {
            base.OnInitialization();
            RemoveAll();
            Info.IsVisible = false;

            bg = new(default, 500, 360)
            {
                opacity = 0.5f,
                color = Color.White,
                CanDrag = true,
            };
            bg.SetCenter(0, 0, 0.5f, 0.5f);
            Register(bg);

            UIImage close = new(T2D(AssetKey + "Close"));
            close.SetPos(bg.Width - 18, 0);
            close.Events.OnLeftClick += (evt) =>
            {
                Info.IsVisible = false;
            };
            bg.Register(close);

            focusItem = new(null, TextureAssets.InventoryBack9.Value)
            {
                CanPutInSlot = new((item) => false),
                CanTakeOutSlot = new((item) => false),
            };
            focusItem.Events.OnLeftClick += (evt) =>
            {
                ChangeItem(Main.LocalPlayer.inventory[58].type);
            };
            focusItem.Events.OnRightClick += (evt) =>
            {
                ChangeItem(ItemID.None);
            };
            focusItem.SetPos(20, 20);
            bg.Register(focusItem);

            focusNPC = new(NPCID.None);
            focusNPC.SetPos(20, 20);
            focusNPC.Info.IsVisible = false;
            focusNPC.Events.OnLeftClick += (evt) =>
            {
                ChangeItem(Main.mouseItem.type);
            };
            focusNPC.Events.OnRightClick += (evt) =>
            {
                ChangeItem(ItemID.None);
                ItemBg.RemoveAll();
            };
            bg.Register(focusNPC);

            int offset = 20 + 52 + 10;

            UIImage shopLine = new(LineTex, 460, 2, 0, 0);
            shopLine.SetCenter(0, offset, 0.5f);
            bg.Register(shopLine);

            UIImage vLine = new(LineTex, 2, 52, 0, 0);
            vLine.SetCenter(offset, 20 + 26);
            bg.Register(vLine);

            side = new(500 - offset - 30 - 10 - 52, 52);
            side.SetPos(offset + 10, 20);
            bg.Register(side);

            _switch = new(T2D(AssetKey + "Slot"));
            _switch.Info.IsSensitive = true;
            _switch.SetPos(offset + 10 + side.Width + 10, 20);
            _switch.ReDraw = (sb) =>
            {
                _switch.DrawSelf(sb);
                if (_switch.Info.IsMouseHover)
                {
                    Main.hoverItemName = Language.GetText(LocalKey + "Filter")
                    .WithFormatArgs(mods[FocusMod].name).Value
                    + "\n" + $"[{FocusMod + 1}/{modCount}]"
                    + "\n" + Language.GetTextValue(LocalKey + "Switch");
                    if (noIcon) Main.hoverItemName += "\n" + Language.GetTextValue(LocalKey + "NoIcon");
                }
            };

            UIImage icon = new(T2D(AssetKey + "All"), 52, 52)
            {
                DrawStyle = 1,
            };
            icon.SetCenter(0, 0, 0.5f, 0.5f);
            _switch.Register(icon);
            _switch.Events.OnLeftClick += (evt) =>
            {
                if (++FocusMod == modCount) FocusMod = 0;
                icon.ChangeImage(GetIcon());
                SwitchShopNPC();
            };
            _switch.Events.OnRightClick += (evt) =>
            {
                if (--FocusMod < 0) FocusMod = modCount - 1;
                icon.ChangeImage(GetIcon());
                SwitchShopNPC();
            };
            bg.Register(_switch);

            ItemBg = new(500, 360 - offset);
            ItemBg.SetCenter(0, offset / 2f, 0.5f, 0.5f);
            bg.Register(ItemBg);

            ShopBg = new(500, 30);
            ShopBg.SetPos(0, 20 + 52 + 10);
            bg.Register(ShopBg);

            indexText = new(Language.GetTextValue(LocalKey + "Index"));
            indexText.SetPos(20 + 26, indexText.TextSize.Y / 2f + 6);
            ShopBg.Register(indexText);

            UIImage vline = new(LineTex, 2, 20);
            vline.SetPos(20 + 52 + 9, 5);
            ShopBg.Register(vline);

            UIImage hline = new(LineTex, 460, 2);
            hline.SetPos(20, 30);
            ShopBg.Register(hline);

            indexBg = new(side.Width + 62, 30);
            indexBg.SetPos(offset + 10, 0);
            ShopBg.Register(indexBg);
            ShopBg.Info.IsVisible = false;
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (!Main.playerInventory && Main.LocalPlayer.controlInv)
            {
                Info.IsVisible = false;
            }
        }
        private Texture2D GetIcon()
        {
            if (FocusMod < 2)
            {
                noIcon = false;
                return T2D(AssetKey + (FocusMod == 0 ? "All" : "Vanilla"));
            }
            else
            {
                try
                {
                    Texture2D tex = T2D(mods[FocusMod].inName + "/icon_small");
                    if (tex != null)
                    {
                        noIcon = false;
                        return tex;
                    }
                }
                catch (Exception) { }
                noIcon = true;
                return T2D(AssetKey + "NoIcon");
            }
        }
        public void ChangeItem(int type)
        {
            focusNPC.Info.IsVisible = false;
            ShopBg.Info.IsVisible = false;
            focusItem.Info.IsVisible = true;
            focusItem.ContainedItem = new(type);
            LookupOne();
        }
        public void ChangeNPC(int type)
        {
            focusItem.Info.IsVisible = false;
            ShopBg.Info.IsVisible = true;

            focusNPC.Info.IsVisible = true;
            focusNPC.ChangeNPC(type);
            LookupShop(type);
        }
        private void LookupOne()
        {
            RegisterScroll(false, ref view);
            int itemid = focusItem.ContainedItem.type;
            if (itemid == ItemID.None) return;
            List<(int npcType, string shopName, Entry entry)> shops = new();
            foreach (AbstractNPCShop shop in NPCShopDatabase.AllShops)
            {
                foreach (Entry entry in shop.ActiveEntries)
                {
                    if (entry.Item.type == itemid)
                    {
                        shops.Add((shop.NpcType, shop.Name, entry));
                        break;
                    }
                }
            }
            if (shops.Any())
            {
                float y = 26;
                int count = 1;
                foreach ((int type, string shopName, Entry entry) in shops)
                {
                    UIBottom bottom = new(480, 72);
                    bottom.Info.IsSensitive = true;
                    bottom.SetCenter(0, y, 0.5f);
                    view.AddElement(bottom);

                    NPC npc = new();
                    npc.SetDefaults(type);
                    string info = npc.FullName + "  " + Language.GetTextValue(LocalKey + "Index") + " | " + shopName;

                    TextUIE condition = new(Decription(info, entry.Conditions, 480 - 82, out float h), drawStyle: 2);
                    condition.SetPos(82, 5, 0, 0.5f);
                    bottom.Info.Height.Pixel += h;
                    bottom.Calculation();
                    bottom.Register(condition);

                    UIImage vLine = new(LineTex, 2, bottom.Info.Height.Pixel - 20);
                    vLine.SetCenter(72, 0, 0, 0.5f);
                    bottom.Register(vLine);

                    ShopNPCSlot slot = new(type, 1f);
                    slot.SetPos(10, -26, 0, 0.5f);
                    bottom.Register(slot);

                    if (count < shops.Count)
                    {
                        y += h;
                        UIImage hLine = new(LineTex, 450, 2, 0, 0);
                        hLine.SetPos(20, y + 30);
                        view.AddElement(hLine);
                        y += 62;
                        count++;
                    }
                }
            }
            else
            {
                TextUIE noSell = new(Language.GetTextValue(LocalKey + "NoSell"));
                noSell.SetCenter(0, 0, 0.5f, 0.5f);
                ItemBg.Register(noSell);
            }
        }
        private void LookupShop(int npcType)
        {
            List<string> shopIndex = new();
            foreach (AbstractNPCShop shop in NPCShopDatabase.AllShops)
            {
                if (shop.NpcType == npcType)
                {
                    shopIndex.Add(shop.Name);
                }
            }
            RegisterScroll(true, ref view);

            indexBg.RemoveAll();

            UIContainerPanel indexView = new();
            indexView.SetSize(0, 0, 1, 1);
            indexView.Info.SetMargin(0);
            indexBg.Register(indexView);

            HorizontalScrollbar scroll = new();
            scroll.Info.IsHidden = true;
            scroll.Info.Top.Pixel += 11;
            indexView.SetHorizontalScrollbar(scroll);
            indexBg.Register(scroll);

            int j = 10;
            for (int i = 0; i < shopIndex.Count; i++)
            {
                TextUIE index = new(shopIndex[i], null, new(1));
                index.SetSize(index.TextSize);
                index.SetCenter(index.Width / 2f + j, 5, 0, 0.5f);
                j += index.Width + 15;
                index.Events.OnLeftClick += (evt) =>
                {
                    ViewShop(npcType, index.text);
                };
                index.ReDraw = (sb) =>
                {
                    if (index.Info.IsMouseHover)
                    {
                        Rectangle rec = index.HitBox();
                        Vector2 drawPos = rec.Center();
                        var font = FontAssets.MouseText.Value;
                        Vector2 origin = index.TextSize / 2f;
                        ChatManager.DrawColorCodedStringWithShadow(sb, font, index.text, drawPos, Color.Gold, 0, origin, index.scale, -1, 1.5f);
                    }
                    else index.DrawSelf(sb);
                };
                indexView.AddElement(index);
            }
            ViewShop(npcType, shopIndex[0]);
        }
        private void ViewShop(int npcType, string shopName)
        {
            RegisterScroll(true, ref view);
            foreach (AbstractNPCShop shop in NPCShopDatabase.AllShops)
            {
                if (shop.NpcType == npcType && shop.Name == shopName)
                {
                    float y = 36 + 10;
                    int count = 1;
                    List<Entry> entrys = new();
                    foreach (Entry entry in shop.ActiveEntries)
                    {
                        if (pylons.Contains(entry.Item.type)) continue;
                        if (entry.Item.type == ItemID.None) continue;
                        entrys.Add(entry);
                    }
                    //float pos = 0;

                    bool permanent = !NonPermanentNPC.TryGet(npcType, out IEnumerable<Condition> conditions);
                    foreach (Entry entry in entrys)
                    {
                        UIBottom bottom = new(480, 80);
                        bottom.SetCenter(0, y, 0.5f);
                        view.AddElement(bottom);

                        Item item = entry.Item;
                        string info = entry.Item.Name;

                        ItemInfo itemInfo = new(info, entry.Conditions, 480 - 120);
                        itemInfo.SetPos(100, -itemInfo.Height / 2f, 0, 0.5f);
                        int yoff = Math.Max(80, itemInfo.Height + 20) - 80;
                        bottom.Info.Height.Pixel += yoff;
                        bottom.Calculation();
                        bottom.Info.IsSensitive = true;
                        bottom.Register(itemInfo);
                        ShopItem shopitem = new(entry, npcType, permanent, conditions);
                        shopitem.SetPos(0, -40, 0, 0.5f);
                        bottom.Register(shopitem);

                        UIImage vLine = new(LineTex, 2, bottom.Info.Height.Pixel - 20);
                        vLine.SetCenter(90, -5, 0, 0.5f);
                        bottom.Register(vLine);

                        if (count < entrys.Count)
                        {
                            y += yoff;
                            UIImage hLine = new(LineTex, 450, 1);
                            hLine.SetPos(20, y + 40);
                            view.AddElement(hLine);
                            count++;
                            y += 90;
                        }
                    }
                }
            }
        }
        public void LoadShopNPC()
        {
            if (pylons == null)
            {
                pylons = new();
                foreach (Entry entry in NPCShopDatabase.GetPylonEntries())
                {
                    pylons.Add(entry.Item.type);
                }
            }

            side.RemoveAll();

            UIContainerPanel view = new();
            view.SetSize(0, 0, 1, 1);
            view.Info.SetMargin(0);
            side.Register(view);

            HorizontalScrollbar scroll = new()
            {
                UseScrollWheel = true,
            };
            scroll.Info.IsHidden = true;
            scroll.Info.Top.Pixel += 20;
            view.SetHorizontalScrollbar(scroll);
            side.Register(scroll);
            int x = 0;

            added = new();
            mods = new() { ("All", "All") };
            foreach (AbstractNPCShop shop in NPCShopDatabase.AllShops)
            {
                if (!added.ContainsKey(shop.NpcType))
                {
                    ShopNPCSlot slot = new(shop.NpcType, 1f, TextureAssets.InventoryBack2.Value);
                    slot.Info.Left.Pixel = x + 5;
                    slot.Events.OnRightClick += (evt) => ChangeNPC(slot.npcType); ;
                    view.AddElement(slot);
                    x += 62;
                    NPC npc = new();
                    npc.SetDefaults(shop.NpcType);
                    ModNPC mn = npc.ModNPC;
                    string modName = "Terraria";
                    string inName = modName;
                    if (mn != null)
                    {
                        modName = mn.Mod.DisplayName;
                        inName = mn.Mod.Name;
                    }
                    added.Add(shop.NpcType, modName);
                    if (!mods.Contains((modName, inName)))
                    {
                        mods.Add((modName, inName));
                    }
                }
            }
            modCount = mods.Count;
            FocusMod = 0;
        }
        public void SwitchShopNPC()
        {
            side.RemoveAll();

            UIContainerPanel view = new();
            view.SetSize(0, 0, 1, 1);
            view.Info.SetMargin(0);
            side.Register(view);

            HorizontalScrollbar scroll = new()
            {
                UseScrollWheel = true,
            };
            scroll.Info.IsHidden = true;
            scroll.Info.Top.Pixel += 20;
            view.SetHorizontalScrollbar(scroll);
            side.Register(scroll);
            int x = 0;

            foreach ((int npcType, string modName) in added)
            {
                if (FocusMod == 0 || modName == mods[FocusMod].name)
                {
                    ShopNPCSlot slot = new(npcType, 1f, TextureAssets.InventoryBack2.Value);
                    slot.Info.Left.Pixel = x + 5;
                    slot.Events.OnRightClick += (evt) => ChangeNPC(slot.npcType);
                    view.AddElement(slot);
                    x += 62;
                }
            }
        }
        private static string Decription(string info, IEnumerable<Condition> cds, float width, out float height)
        {
            string conditions = "";
            var font = FontAssets.MouseText.Value;
            height = 0;
            int count = cds.Count();
            if (count > 0)
            {
                int i = 1;
                foreach (Condition c in cds)
                {
                    string cdesc = c.Description.Value;
                    conditions += cdesc;
                    /*if (c.Description.Key == cdesc)
                    {

                    }*/
                    if (i == 1 && count > 1)
                    {
                        height = font.MeasureString(c.Description.Value).Y;
                    }
                    if (i < count) conditions += "\n";
                    i++;
                }
                float oldH = font.MeasureString(conditions).Y;
                conditions = font.CreateWrappedText(conditions, width);
                float newH = font.MeasureString(conditions).Y;
                if (height > 0)
                {
                    height = newH - height;
                }
                else
                {
                    if (newH > oldH) height = newH - oldH;
                }

                return info + "\n" + conditions;
            }
            else return info + "\n" + Language.GetTextValue(LocalKey + "NoCondition");
        }
        private UIContainerPanel RegisterScroll(bool npc, ref UIContainerPanel view)
        {
            ItemBg.RemoveAll();
            if (npc)
            {
                UIBottom bbg = new(ItemBg.Width, ItemBg.Height - 50);
                bbg.SetPos(0, 40);
                ItemBg.Register(bbg);

                this.view = new();
                view.SetSize(0, 0, 1, 1);
                view.Info.SetMargin(0);
                bbg.Register(view);

                VerticalScrollbar scroll = new()
                {
                    UseScrollWheel = true,
                };
                scroll.Info.IsHidden = true;
                scroll.Info.Left.Pixel -= 10;
                view.SetVerticalScrollbar(scroll);
                bbg.Register(scroll);

                return null;
            }
            else
            {
                view = new();
                view.Info.Top.Pixel = 10;
                view.SetSize(0, -30, 1, 1);
                view.Info.SetMargin(0);
                ItemBg.Register(view);

                VerticalScrollbar scroll = new()
                {
                    UseScrollWheel = true,
                };
                scroll.Info.IsHidden = true;
                scroll.Info.Left.Pixel -= 10;
                view.SetVerticalScrollbar(scroll);
                ItemBg.Register(scroll);
                return null;
            }
        }
    }
}
