namespace ShopLookup.Content
{
    public class ShopItem : BaseUIElement
    {
        public UIItemSlot slot;
        public UICurrency currency;
        public ShopItem(Entry entry)
        {
            SetSize(80, 80);
            //DrawRec[0] = true;
            slot = new(entry.Item)
            {
                CanPutInSlot = (new(Item => false)),
                CanTakeOutSlot = (new(Item => false)),
            };
            slot.SetCenter(0, 26, 0.5f);
            Register(slot);

            currency = new(entry);
            currency.SetPos(0, 56);
            Register(currency);
            /*if (CurrencyType(item.shopSpecialCurrency, out int coinType))
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
            }*/
        }
    }
}
