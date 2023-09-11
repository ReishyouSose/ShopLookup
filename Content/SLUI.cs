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
        public UINPCSlot focusNPC;
        public TextUIE indexText;
        public UIImage _switch;
        public UIBottom ItemBg, side, ShopBg, indexBg;
        public UIContainerPanel view, indexView;
        public bool firstLoad;
        private int FocusMod;
        private bool dragging;
        private bool viewPylon;
        private Vector2 startPoint = Vector2.Zero;
        private static bool noIcon;
        internal static List<Entry> pylons;
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
            close.SetPos(-18, 0, 1, 0);
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

            UIImage shopLine = new(LineTex, -40, 2, 1, 0);
            shopLine.SetCenter(0, offset);
            bg.Register(shopLine);

            UIImage vLine = new(LineTex, 2, 52, 0, 0);
            vLine.SetCenter(offset, 20 + 26);
            bg.Register(vLine);

            side = new(-offset - 30 - (10 + 52) * 2, 52, 1);
            side.SetPos(offset + 10, 20);
            bg.Register(side);
            side.Calculation();

            UIImage pylon = new(T2D(AssetKey + "Slot"));
            pylon.SetPos(-20 - 52 - 10 - 52, 20, 1);
            Main.instance.LoadItem(4951);
            pylon.ReDraw = sb =>
            {
                pylon.DrawSelf(sb);
                Texture2D tex = TextureAssets.Item[ItemID.TeleportationPylonVictory].Value;
                float mult = pylon.Info.IsMouseHover ? 1f : 0.75f;
                sb.Draw(tex, pylon.Center(), null, Color.White * mult, 0, tex.Size() / 2f, 0.75f, 0, 0);
            };
            pylon.Events.OnLeftClick += evt => LookupPylons();
            pylon.Events.OnRightClick += evt => LookupPylons();
            bg.Register(pylon);

            _switch = new(T2D(AssetKey + "Slot"));
            _switch.SetPos(-20 - 52, 20, 1);
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
            _switch.Info.IsSensitive = true;
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

            ItemBg = new(-20, -offset - 10, 1, 1);
            ItemBg.SetPos(10, offset);
            bg.Register(ItemBg);

            ShopBg = new(0, 30, 1);
            ShopBg.SetPos(0, 20 + 52 + 10);
            bg.Register(ShopBg);

            indexText = new(Language.GetTextValue(LocalKey + "Index"));
            indexText.SetPos(20 + 26, indexText.TextSize.Y / 2f + 6);
            ShopBg.Register(indexText);

            UIImage vline = new(LineTex, 2, 20);
            vline.SetPos(20 + 52 + 9, 5);
            ShopBg.Register(vline);

            UIImage hline = new(LineTex, -40, 2, 1);
            hline.SetCenter(0, 30);
            ShopBg.Register(hline);

            indexBg = new(-offset - 30, 30, 1);
            indexBg.SetPos(offset + 10, 0);
            ShopBg.Register(indexBg);
            ShopBg.Info.IsVisible = false;

            UIImage adjust = new(T2D(AssetKey + "Adjust"));
            adjust.SetPos(-18, -18, 1, 1);
            adjust.Events.OnLeftDown += (evt) =>
            {
                dragging = true;
                startPoint = Main.MouseScreen;
            };
            adjust.Events.OnLeftUp += (evt) =>
            {
                if (viewPylon)
                {
                    LookupPylons();
                }
                else if (focusItem.Info.IsVisible && focusItem.ContainedItem.type > ItemID.None)
                {
                    LookupOne();
                }
                else if (focusNPC.Info.IsVisible)
                {
                    LookupShop(focusNPC.npcType);
                }
                dragging = false;
            };
            adjust.Events.OnLeftDoubleClick += (evt) => dragging = false;
            bg.Register(adjust);
        }
        private static void Clamp(ref float value, float offset, float min, float max)
        {
            value = Math.Clamp(value + offset, min, max);
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (!Main.playerInventory && Main.LocalPlayer.controlInv)
            {
                Info.IsVisible = false;
            }
            Vector2 pos = Main.MouseScreen;
            if (dragging)
            {
                static bool CanMove(float offset, float mouse, float origin) => (offset > 0 && mouse > origin) || (offset < 0 && mouse < origin);
                if (startPoint.X != pos.X)
                {
                    float right = bg.Left + bg.Width;
                    float offset = pos.X - startPoint.X;
                    if (CanMove(offset, pos.X, right))
                    {
                        Clamp(ref bg.Info.Width.Pixel, offset, 500, 1000);
                        bg.Calculation();
                    }
                }
                if (startPoint.Y != pos.Y)
                {
                    float bottom = bg.Top + bg.Height;
                    float offset = pos.Y - startPoint.Y;
                    if (CanMove(offset, pos.Y, bottom))
                    {
                        Clamp(ref bg.Info.Height.Pixel, offset, 360, 720);
                        bg.Calculation();
                    }
                }
                startPoint = pos;
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
                string name = mods[FocusMod].inName;
                if (ModLoader.GetMod(name).HasAsset("icon_small"))
                {
                    Texture2D tex = T2D(name + "/icon_small");
                    if (tex != null)
                    {
                        noIcon = false;
                        return tex;
                    }
                }
            }
            noIcon = true;
            return T2D(AssetKey + "NoIcon");
        }
        public void ChangeItem(int type, bool lookup = true)
        {
            focusNPC.Info.IsVisible = false;
            ShopBg.Info.IsVisible = false;
            focusItem.Info.IsVisible = true;
            focusItem.ContainedItem = new(type);
            viewPylon = false;
            if (lookup) LookupOne();
        }
        public void ChangeNPC(int type)
        {
            focusItem.Info.IsVisible = false;
            ShopBg.Info.IsVisible = true;
            focusNPC.Info.IsVisible = true;
            focusNPC.ChangeNPC(type);
            viewPylon = false;
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
                float y = 0;
                int count = 1;
                foreach ((int type, string shopName, Entry entry) in shops)
                {
                    NPC npc = new();
                    npc.SetDefaults(type);
                    string info = npc.FullName + "  " + Language.GetTextValue(LocalKey + "Index") + " | " + GetShopLT(type, shopName);
                    bool permanent = !ExtraNPCInfo.NonTryGet(type, out IEnumerable<Condition> conditions);
                    RegisterInfo(y, info, entry, type, false, permanent, conditions, out float yoff);

                    if (count < shops.Count)
                    {
                        y += yoff;
                        UIImage hLine = new(LineTex, -40, 1, 1);
                        hLine.SetPos(20, y + 79);
                        view.AddElement(hLine);
                        count++;
                        y += 90;
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
            scroll.UseScrollWheel = true;
            indexView.SetHorizontalScrollbar(scroll);
            indexBg.Register(scroll);

            int j = 10;
            for (int i = 0; i < shopIndex.Count; i++)
            {
                TextUIE index = new(GetShopLT(npcType, shopIndex[i]), null, new(1));
                index.SetSize(index.TextSize);
                index.SetCenter(index.Width / 2f + j, 5, 0, 0.5f);
                j += index.Width + 15;
                string shopid = shopIndex[i];
                index.Events.OnLeftClick += (evt) =>
                {
                    RegisterScroll(true, ref view);
                    ViewShop(npcType, shopid);
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
        private static string GetShopLT(int npcType, string index)
        {
            if (ExtraNPCInfo.NameTryGet(npcType, out var info))
            {
                if (index == info.index)
                {
                    return info.text.Value;
                }
            }
            if (index == "Shop")
            {
                return Language.GetTextValue("LegacyInterface.28");
            }
            else
            {
                if (npcType == NPCID.Painter)
                {
                    index = Language.GetTextValue("GameUI.PainterDecor");
                }
            }
            return index;
        }
        private void ViewShop(int npcType, string shopName)
        {
            foreach (AbstractNPCShop shop in NPCShopDatabase.AllShops)
            {
                if (shop.NpcType == npcType && shop.Name == shopName)
                {
                    float y = 0;
                    int count = 1;
                    List<Entry> entrys = new();
                    foreach (Entry entry in shop.ActiveEntries)
                    {
                        if (pylons.Any(x => x.Item.type == entry.Item.type)) continue;
                        if (entry.Item.type == ItemID.None) continue;
                        entrys.Add(entry);
                    }
                    if (!entrys.Any())
                    {
                        TextUIE empty = new(Language.GetTextValue(LocalKey + "EmptyShop"));
                        empty.SetCenter(0, 0, 0.5f, 0.5f);
                        view.Register(empty);
                        return;
                    }
                    bool permanent = !ExtraNPCInfo.NonTryGet(npcType, out IEnumerable<Condition> conditions);
                    foreach (Entry entry in entrys)
                    {
                        RegisterInfo(y, entry.Item.Name, entry, npcType, false, permanent, conditions, out float yoff);
                        if (count < entrys.Count)
                        {
                            y += yoff;
                            UIImage hLine = new(LineTex, -40, 1, 1);
                            hLine.SetPos(20, y + 79);
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
                    pylons.Add(entry);
                }
            }

            side.RemoveAll();

            UIContainerPanel view = new();
            view.SetSize(0, 0, 1, 1);
            view.Info.SetMargin(0);
            side.Register(view);

            HorizontalScrollbar scroll = new(62)
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
                    UINPCSlot slot = new(shop.NpcType, 1f, TextureAssets.InventoryBack2.Value);
                    slot.Info.Left.Pixel = x + 5;
                    slot.Events.OnLeftClick += (evt) => ChangeNPC(slot.npcType);
                    slot.Events.OnRightClick += (evt) => ChangeNPC(slot.npcType);
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
                    UINPCSlot slot = new(npcType, 1f, TextureAssets.InventoryBack2.Value);
                    slot.Info.Left.Pixel = x + 5;
                    slot.Events.OnLeftClick += (evt) => ChangeNPC(slot.npcType);
                    slot.Events.OnRightClick += (evt) => ChangeNPC(slot.npcType);
                    view.AddElement(slot);
                    x += 62;
                }
            }
        }
        private UIContainerPanel RegisterScroll(bool npc, ref UIContainerPanel view)
        {
            ItemBg.RemoveAll();
            if (npc)
            {
                UIBottom bbg = new(0, -30, 1, 1);
                bbg.SetPos(0, 30);
                ItemBg.Register(bbg);

                view = new();
                view.Info.Top.Pixel += 10;
                view.SetSize(0, -20, 1, 1);
                view.Info.SetMargin(0);
                bbg.Register(view);

                VerticalScrollbar scroll = new(90)
                {
                    UseScrollWheel = true,
                };
                scroll.Info.IsHidden = true;
                view.SetVerticalScrollbar(scroll);
                bbg.Register(scroll);

                return null;
            }
            else
            {
                view = new();
                view.Info.Top.Pixel += 10;
                view.SetSize(0, -20, 1, 1);
                view.Info.SetMargin(0);
                ItemBg.Register(view);

                VerticalScrollbar scroll = new()
                {
                    UseScrollWheel = true,
                };
                scroll.Info.IsHidden = true;
                view.SetVerticalScrollbar(scroll);
                ItemBg.Register(scroll);
                return null;
            }
        }
        private void RegisterInfo(float y, string info, Entry entry, int npcType, bool npc, bool permanent, IEnumerable<Condition> conditions, out float yoff)
        {
            UIBottom bottom = new(0, 80, 1);
            bottom.SetPos(0, y);
            bottom.Info.IsSensitive = true;
            view.AddElement(bottom);

            ItemInfo itemInfo = new(info, entry.Conditions, bottom.Width - 120);
            itemInfo.SetPos(100, -itemInfo.Height / 2f, 0, 0.5f);
            yoff = Math.Max(80, itemInfo.Height + 20) - 80;
            bottom.Info.Height.Pixel += yoff;
            bottom.Calculation();
            bottom.Register(itemInfo);

            UIShopSlot shopitem = new(entry, npcType, npc, permanent, conditions);
            shopitem.SetPos(0, -40, 0, 0.5f);
            bottom.Register(shopitem);

            UIImage vLine = new(LineTex, 2, bottom.Info.Height.Pixel - 20);
            vLine.SetCenter(90, -5, 0, 0.5f);
            bottom.Register(vLine);
        }
        private void LookupPylons()
        {
            RegisterScroll(false, ref view);
            ChangeItem(ItemID.TeleportationPylonVictory, false);
            viewPylon = true;
            ShopBg.Info.IsVisible = false;
            float y = 0;
            int count = 1;
            foreach (Entry pylon in pylons)
            {
                RegisterInfo(y, pylon.Item.Name, pylon, -1, false, false, null, out float yoff);
                if (count < pylons.Count)
                {
                    y += yoff;
                    UIImage hLine = new(LineTex, -40, 1, 1);
                    hLine.SetPos(20, y + 79);
                    view.AddElement(hLine);
                    count++;
                    y += 90;
                }
            }
        }
    }
}
