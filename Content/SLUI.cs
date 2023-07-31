using Terraria.Localization;

namespace ShopLookup.Content
{
    public class SLUI : ContainerElement
    {
        public const string LocalKey = "Mods.ShopLookup.";
        public const string NmakeKey = "ShopLookup.Content.SLUI";
        public UIPanel bg;
        public UIItemSlot focusItem;
        public UIBottom bottom, side;
        public bool firstLoad;
        internal static List<int> pylons = new();
        public override void OnInitialization()
        {
            base.OnInitialization();
            Info.IsVisible = false;

            bg = new(default, 500, 360)
            {
                opacity = 0.85f,
                color = Color.LightGray,
                CanDrag = true,
            };
            bg.SetCenter(0, 0, 0.5f, 0.5f);
            Register(bg);

            focusItem = new(null, TextureAssets.InventoryBack9.Value)
            {
                CanTakeOutSlot = new((Item) => false),
            };
            focusItem.SetPos(20, 20);
            bg.Register(focusItem);

            UIImage vLine = new(TextureAssets.MagicPixel.Value, 2, 52, 0, 0);
            vLine.SetCenter(20 + 52 + 10, 20 + 26);
            bg.Register(vLine);

            UIImage hLine = new(TextureAssets.MagicPixel.Value, 460, 2, 0, 0);
            hLine.SetCenter(0, 20 + 52 + 10, 0.5f);
            bg.Register(hLine);

            int offset = 20 + 52 + 10;
            bottom = new(500, 360 - offset - 30);
            bottom.SetCenter(0, offset / 2f, 0.5f, 0.5f);
            bg.Register(bottom);

            side = new(500 - offset - 30, 52);
            side.SetPos(offset + 10, 20);
            bg.Register(side);
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
            bg.color = Color.White;
            bg.opacity = 0.5f;
        }
        public void ChangeItem(int type)
        {
            focusItem.ContainedItem = new(type);
            LookupOne();
        }
        private void LookupOne()
        {
            RegisterScroll(out UIContainerPanel view);
            List<(int npcType, Entry entry)> shops = new();
            foreach (AbstractNPCShop shop in NPCShopDatabase.AllShops)
            {
                foreach (Entry entry in shop.ActiveEntries)
                {
                    if (entry.Item.type == focusItem.ContainedItem.type)
                    {
                        shops.Add((shop.NpcType, entry));
                        break;
                    }
                }
            }
            if (shops.Any())
            {
                float y = 26;
                int count = 1;
                foreach ((int type, Entry entry) in shops)
                {
                    UIBottom bottom = new(480, 72);
                    bottom.SetCenter(0, y, 0.5f);
                    view.AddElement(bottom);

                    NPC npc = new();
                    npc.SetDefaults(type);
                    string info = npc.FullName + "  " + npc.ModNPC == null ? Language.GetTextValue
                        (LocalKey + "Vanilla") : npc.ModNPC.Mod.DisplayName;
                    TextUIE condition = new(Decription(info, entry.Conditions, out float h), drawStyle: 2);
                    condition.SetPos(72, 5, 0, 0.5f);
                    bottom.Register(condition);

                    NPCHeadSlot slot = new(type, 1f);
                    if (h > 0)
                    {
                        bottom.Info.Height.Pixel += h;
                    }
                    slot.SetPos(10, -26, 0, 0.5f);
                    bottom.Register(slot);

                    if (count < shops.Count)
                    {
                        y += h;
                        UIImage hLine = new(TextureAssets.MagicPixel.Value, 450, 2, 0, 0);
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
                bottom.Register(noSell);
            }
        }
        private void LookupAll(int npcType)
        {
            TextSnippet[] t = new TextSnippet[]
            {
                new($"[i/s9999:{ItemID.PlatinumCoin}]")
                /*new($"[i/s9999:{ItemID.PlatinumCoin}][i/s9999:{ItemID.GoldCoin}]" +
                $"[i/s9999:{ItemID.SilverCoin}][i/s9999:{ItemID.CopperCoin}]")*/
            };
            Main.NewText(t[0]);
            Main.NewText(ChatManager.GetStringSize(FontAssets.MouseText.Value, t, Vector2.One));
            RegisterScroll(out UIContainerPanel view);
            foreach (AbstractNPCShop shop in NPCShopDatabase.AllShops)
            {
                if (shop.NpcType == npcType)
                {
                    float y = 36;
                    int count = 1;
                    foreach (Entry entry in shop.ActiveEntries)
                    {
                        if (pylons.Contains(entry.Item.type)) continue;
                        /*UIBottom bottom = new(480, 72);
                        //bottom.DrawRec[0] = true;
                        bottom.SetCenter(0, y, 0.5f);
                        view.AddElement(bottom);

                        Item item = entry.Item;
                        string info = entry.Item.Name + "  " + (item.ModItem == null ? Language.GetTextValue
                            (LocalKey + "Vanilla") : item.ModItem.Mod.DisplayName);
                        TextUIE condition = new(Decription(info, entry.Conditions, out float h), drawStyle: 2);
                        condition.SetPos(72, 5, 0, 0.5f);
                        bottom.Register(condition);

                        UIItemSlot slot = new(entry.Item, TextureAssets.InventoryBack2.Value)
                        {
                            CanTakeOutSlot = new((Item) => false)
                        };
                        if (h > 0)
                        {
                            bottom.Info.Height.Pixel += h;
                        }
                        slot.SetPos(10, -26, 0, 0.5f);
                        bottom.Register(slot);

                        if (count < shop.ActiveEntries.Count())
                        {
                            y += h;
                            UIImage hLine = new(TextureAssets.MagicPixel.Value, 450, 1, 0, 0);
                            hLine.SetPos(20, y + 30);
                            view.AddElement(hLine);
                            y += 62;
                            count++;
                        }*/
                        UISnippet text = new(t);
                        text.SetPos(250, 10 + y);
                        view.AddElement(text);
                        y += 62;

                    }
                    return;
                }
            }
        }
        public void LoadShopNPC()
        {
            if (firstLoad) return;
            firstLoad = true;

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
            int x = 10;

            foreach (AbstractNPCShop shop in NPCShopDatabase.AllShops)
            {
                NPCHeadSlot slot = new(shop.NpcType, 1f, TextureAssets.InventoryBack2.Value);
                slot.Info.Left.Pixel = x;
                slot.Events.OnRightClick += (evt) =>
                {
                    LookupAll(shop.NpcType);
                };
                view.AddElement(slot);
                x += 62;
            }
        }
        private string Decription(string info, IEnumerable<Condition> cds, out float height)
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
                conditions = font.CreateWrappedText(conditions, 480 - 82);
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
        private void RegisterScroll(out UIContainerPanel view)
        {
            bottom.RemoveAll();

            view = new();
            view.SetSize(0, 0, 1, 1);
            view.Info.SetMargin(0);
            bottom.Register(view);

            VerticalScrollbar scroll = new()
            {
                UseScrollWheel = true,
            };
            scroll.Info.IsHidden = true;
            scroll.Info.Left.Pixel -= 10;
            view.SetVerticalScrollbar(scroll);
            bottom.Register(scroll);
        }
    }
}
