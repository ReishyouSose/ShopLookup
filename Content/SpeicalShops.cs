namespace ShopLookup.Content
{
    public static class SpeicalShops
    {
        public static NPCShop Add(this NPCShop shop, Mod mod, string itemName, params Condition[] conditions)
        {
            shop.Add(new Item(mod.Find<ModItem>(itemName).Type), conditions);
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
        public static List<Entry> GetQoTItems(out int count)
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
            List<Entry> items = new();
            foreach (Entry entry in shop.ActiveEntries) items.Add(entry);
            count = items.Count;
            return items;
        }
    }
}
