﻿using Terraria.Localization;
using static Terraria.UI.Gamepad.UILinkPointNavigator;

namespace ShopLookup.Content
{
    public class SLUI : ContainerElement
    {
        public const string LocalKey = "Mods.ShopLookup.";
        public const string NmakeKey = "ShopLookup.Content.SLUI";
        public static Texture2D LineTex => TextureAssets.MagicPixel.Value;
        public UIPanel bg;
        public UIItemSlot focusItem;
        public ShopNPCSlot focusNPC;
        public UIBottom ItemBg, side, ShopBg, indexBg;
        public UIContainerPanel view, indexView;
        public bool firstLoad;
        internal static List<int> pylons = new();
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

            UIImage close = new(T2D("ShopLookup/UISupport/Asset/Close"));
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
                int id = Main.LocalPlayer.inventory[58].type;
                if (id > ItemID.None)
                {
                    ChangeItem(id);
                }
            };
            focusItem.Events.OnRightClick += (evt) =>
            {
                ChangeItem(ItemID.None);
            };
            focusItem.SetPos(20, 20);
            bg.Register(focusItem);

            focusNPC = new(NPCID.None, null);
            focusNPC.SetPos(20, 20);
            focusNPC.Info.IsVisible = false;
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

            side = new(500 - offset - 30, 52);
            side.SetPos(offset + 10, 20);
            bg.Register(side);

            ItemBg = new(500, 360 - offset);
            ItemBg.SetCenter(0, offset / 2f, 0.5f, 0.5f);
            bg.Register(ItemBg);

            ShopBg = new(500, 30);
            ShopBg.SetPos(0, 20 + 52 + 10);
            bg.Register(ShopBg);

            TextUIE text = new(Language.GetTextValue(LocalKey + "Index"));
            text.SetPos(20 + 26, text.TextSize.Y / 2f + 6);
            ShopBg.Register(text);

            UIImage vline = new(LineTex, 2, 20);
            vline.SetPos(20 + 52 + 9, 5);
            ShopBg.Register(vline);

            UIImage hline = new(LineTex, 460, 2);
            hline.SetPos(20, 30);
            ShopBg.Register(hline);

            indexBg = new(side.Width, 30);
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
            focusNPC.ChangeNPC(type, null);
            LookupShop(type);
        }
        private void LookupOne()
        {
            RegisterScroll(false, ref view);
            if (focusItem.ContainedItem.type == ItemID.None) return;
            List<(int npcType, string shopName, Entry entry)> shops = new();
            foreach (AbstractNPCShop shop in NPCShopDatabase.AllShops)
            {
                foreach (Entry entry in shop.ActiveEntries)
                {
                    if (entry.Item.type == focusItem.ContainedItem.type)
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
                    bottom.SetCenter(0, y, 0.5f);
                    view.AddElement(bottom);

                    NPC npc = new();
                    npc.SetDefaults(type);
                    string info = npc.FullName + "  " + Language.GetTextValue(LocalKey + "Index") + " | " + shopName;
                    TextUIE condition = new(Decription(info, entry.Conditions, 480 - 82, out float h), drawStyle: 2);
                    condition.SetPos(82, 5, 0, 0.5f);
                    bottom.Register(condition);

                    UIImage vLine = new(LineTex, 2, bottom.Info.Height.Pixel - 20);
                    vLine.SetCenter(72, 0, 0, 0.5f);
                    bottom.Register(vLine);

                    ShopNPCSlot slot = new(type, shopName, 1f);
                    if (h > 0)
                    {
                        bottom.Info.Height.Pixel += h;
                    }
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
            scroll.Info.Top.Pixel += 10;
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
                        ChatManager.DrawColorCodedStringWithShadow(sb, font, index.text, drawPos, Color.Black, 0, origin, index.scale);
                        ChatManager.DrawColorCodedString(sb, font, index.text, drawPos, Color.Gold, 0, origin, index.scale);
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
                    foreach (Entry entry in entrys)
                    {
                        UIBottom bottom = new(480, 80);
                        //bottom.DrawRec[0] = true;
                        bottom.SetCenter(0, y, 0.5f);
                        view.AddElement(bottom);

                        Item item = entry.Item;
                        string info = entry.Item.Name/* + "  " + (item.ModItem == null ? Language.GetTextValue
                            (LocalKey + "Vanilla") : item.ModItem.Mod.DisplayName)*/;
                        TextUIE condition = new(Decription(info, entry.Conditions, 480 - 120, out float h), drawStyle: 2);
                        condition.SetPos(100, 0, 0, 0.5f);
                        bottom.Register(condition);

                        ShopItem shopitem = new(entry);
                        bottom.Info.Height.Pixel += h;
                        shopitem.SetPos(0, -40, 0, 0.5f);
                        bottom.Calculation();
                        bottom.Register(shopitem);

                        UIImage vLine = new(LineTex, 2, bottom.Info.Height.Pixel - 20);
                        vLine.SetCenter(90, -5, 0, 0.5f);
                        bottom.Register(vLine);

                        if (count < entrys.Count())
                        {
                            y += h;
                            UIImage hLine = new(LineTex, 450, 1);
                            hLine.SetPos(20, y + 40);
                            view.AddElement(hLine);
                            y += 90;
                            count++;
                        }
                    }
                    return;
                }
            }
        }
        public void LoadShopNPC()
        {
            foreach (Entry entry in NPCShopDatabase.GetPylonEntries())
            {
                pylons.Add(entry.Item.type);
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

            Dictionary<int, bool> added = new();
            foreach (AbstractNPCShop shop in NPCShopDatabase.AllShops)
            {
                if (!added.ContainsKey(shop.NpcType))
                {
                    ShopNPCSlot slot = new(shop.NpcType, shop.Name, 1f, TextureAssets.InventoryBack2.Value);
                    slot.Info.Left.Pixel = x + 5;
                    slot.Events.OnMouseOver += (evt) => Shortcuts.NPCS_LastHovered = -10 - slot.npcType;
                    slot.Events.OnMouseOut += (evt) => Shortcuts.NPCS_LastHovered = -2;
                    slot.Events.OnRightClick += (evt) => ChangeNPC(slot.npcType); ;
                    view.AddElement(slot);
                    x += 62;
                    added.Add(shop.NpcType, true);
                }
            }
        }
        private string Decription(string info, IEnumerable<Condition> cds, float width, out float height)
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
                    conditions += c.Description.Value;
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
