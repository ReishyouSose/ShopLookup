namespace ShopLookup.Content
{
    public class ShopItem : BaseUIElement
    {
        public UIItemSlot slot;
        public UIItem[] vanilla;
        public UIItem value;
        public ShopItem(Item item)
        {
            SetSize(80, 80);
            DrawRec[0] = true;
            slot = new(item);
            slot.SetCenter(0, 26, 0.5f);
            Register(slot);

            if (CurrencyType(item.shopSpecialCurrency, out int coinType))
            {
                value = new(coinType, item.shopCustomPrice.Value);
                value.SetSize(16, 16);
                value.SetCenter(0, 52, 0.5f);
                value.Ignore = true;
                Register(value);
            }
            else
            {
                List<UIItem> list = new();
                foreach ((int coin, int count) in ToCoins(item.shopCustomPrice ?? item.value))
                {
                    UIItem coins = new(coin, count);
                    list.Add(coins);
                }
                vanilla = list.ToArray();
                for (int i = 0; i < vanilla.Length; i++)
                {
                    UIItem coins = vanilla[i];
                    coins.Ignore = true;
                    coins.SetSize(16, 16);
                    coins.SetCenter(0, 52 + 10, (i + 1) / ((float)vanilla.Length + 1));
                    Register(coins);
                    vanilla[i] = coins;
                }
            }
        }
        public static IEnumerable<(int itemId, int count)> ToCoins(int money)
        {
            int copper = money % 100;
            money /= 100;
            int silver = money % 100;
            money /= 100;
            int gold = money % 100;
            money /= 100;
            int plat = money;

            if (plat > 0) yield return (ItemID.PlatinumCoin, plat);
            if (gold > 0) yield return (ItemID.GoldCoin, gold);
            if (silver > 0) yield return (ItemID.SilverCoin, silver);
            if (copper > 0) yield return (ItemID.CopperCoin, copper);
        }
    }
}
