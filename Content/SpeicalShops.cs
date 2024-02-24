namespace ShopLookup.Content
{
    public static class SpeicalShops
    {
        public static NPCShop Add(this NPCShop shop, int itemType, int? customPrice, params Condition[] conditions)
        {
            shop.Add(new Item(itemType) { shopCustomPrice = customPrice }, conditions);
            return shop;
        }
        public static NPCShop Add(this NPCShop shop, Mod mod, string itemName, params Condition[] conditions)
        {
            if (mod.TryFind(itemName, out ModItem mi))
            {
                shop.Add(new Item(mi.Type), conditions);
            }
            return shop;
        }
        public static NPCShop Add(this NPCShop shop, Mod mod, string[] itemName, params Condition[] condition)
        {
            if (itemName.Any())
            {
                for (int i = 0; i < itemName.Length; i++)
                {
                    shop.Add(mod, itemName[i], condition);
                }
            }
            return shop;
        }

        /// <param name="itemarray">id, 货币id, 需求量</param>
        public static NPCShop Add(this NPCShop shop, Mod mod, (string itemName, int currency, int value)[] itemarray, params Condition[] condition)
        {
            if (itemarray.Any())
            {
                for (int i = 0; i < itemarray.Length; i++)
                {
                    shop.Add(new Item(mod.Find<ModItem>(itemarray[i].itemName).Type)
                    {
                        shopSpecialCurrency = itemarray[i].currency,
                        shopCustomPrice = itemarray[i].value,
                    }, condition);
                }
            }
            return shop;
        }
        /// <param name="currency">货币id</param>
        public static NPCShop Add(this NPCShop shop, Mod mod, (string itemName, int value)[] itemarray, int currency = -1, params Condition[] condition)
        {
            if (itemarray.Length > 0)
            {
                for (int i = 0; i < itemarray.Length; i++)
                {
                    shop.Add(new Item(mod.Find<ModItem>(itemarray[i].itemName).Type)
                    {
                        shopSpecialCurrency = currency,
                        shopCustomPrice = itemarray[i].value,
                    }, condition);
                }
            }
            return shop;
        }
        public static IEnumerable<Entry> GetVanillaShop(out int count)
        {
            NPCShop shop = new NPCShop(-1)
                .Add(361, Item.buyPrice(0, 5))//哥布林战旗
                .Add(4271, Item.buyPrice(0, 1))//血泪
                .Add(3124, Item.buyPrice(0, 12))//手机
                .Add(ItemID.FallenStar, Item.buyPrice(0, 0, 3))
                .Add(2674, Item.buyPrice(0, 0, 3))//三种鱼饵
                .Add(2675, Item.buyPrice(0, 0, 5))
                .Add(2676, Item.buyPrice(0, 0, 15))
                .Add(2422, Condition.Hardmode)//熔岩钓竿
                .Add(2294)//金鱼竿
                .Add(2373)//渔夫饰品
                .Add(2374)
                .Add(2375)
                .Add(4881, Condition.DownedEowOrBoc)
                .Add(3183)//金虫网
                .Add(275, Item.buyPrice(0, 0, 5), Condition.InBeach)//珊瑚
                .Add(5, Item.buyPrice(0, 0, 3), Condition.InShoppingZoneForest)//四种蘑菇
                .Add(2887, Item.buyPrice(0, 0, 0, 50), Condition.InCrimson)
                .Add(60, Item.buyPrice(0, 0, 0, 50), Condition.InCorrupt)
                .Add(183, Item.buyPrice(0, 0, 1), Condition.InGlowshroom)
                .Add(4361, Item.buyPrice(0, 1), Condition.InShoppingZoneForest)//瓢虫
                .Add(ItemID.TargetDummy, Item.buyPrice(0, 0, 10))
                .Add(2673, Item.buyPrice(0, 20), Condition.InGlowshroom, Condition.Hardmode)//松露虫
                .Add(4961, Item.buyPrice(0, 10), Condition.InHallow, Condition.TimeNight)/*七彩草蛉*/
                .Add(ItemID.TerrasparkBoots, Condition.DownedGoblinArmy);
            count = shop.ActiveEntries.Count();
            return shop.ActiveEntries;
        }
        public static IEnumerable<Entry> GetQoTItems(out int count)
        {
            Mod qot = ModLoader.GetMod("ImproveGame");
            NPCShop shop = new NPCShop(-1)
                .Add(qot, new string[] { "BannerChest", "ExtremeStorage", "Autofisher",
                    "CreateWand", "MagickWand", "DetectorDrone", "PaintWand"})
                .Add(qot, new (string, int)[] { ("PotionBag", Item.buyPrice(0,2,50)),
                    ("Dummy",Item.buyPrice(0,1)), ("MoveChest", Item.buyPrice(0,5)) })
                .Add(qot, new string[] { "WallPlace", "SpaceWand" }, Condition.DownedKingSlime)
                .Add(qot, "LiquidWand", Condition.DownedEowOrBoc)
                .Add(qot, new string[] { "StarburstWand", "ConstructWand" }, Condition.Hardmode);
            //List<Entry> items = new();
            //foreach (Entry entry in shop.ActiveEntries) items.Add(entry);
            count = shop.ActiveEntries.Count();
            return shop.ActiveEntries;
            //return items;
        }
    }
}
