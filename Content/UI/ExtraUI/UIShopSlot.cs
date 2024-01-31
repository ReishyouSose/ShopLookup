using ShopLookup.Content.Sys;
using System.Linq;

namespace ShopLookup.Content.UI.ExtraUI
{
    public class UIShopSlot : BaseUIElement
    {
        public UIItemSlot itemSlot;
        public UIShopSlot(AbstractNPCShop.Entry entry)
        {
            SetSize(100, 100);
            Item item = entry.Item;
            item.isAShopItem = true;
            itemSlot = new(item);
            itemSlot.SetCenter(0, 36, 0.5f);
            itemSlot.DrawRec[0] = Color.Blue;
            Register(itemSlot);

            int value = item.shopCustomPrice ?? item.value;
            Dictionary<int, int> values = new();
            foreach (var (itemID, rank) in ShopNPCData.Currencys[item.shopSpecialCurrency])
            {
                values[itemID] = value / rank;
                value %= rank;
            }
            var hasValue = values.Where(x => x.Value > 0);
            int count = hasValue.Count() + 1;
            float i = 1;
            foreach (var (itemType, stack) in hasValue)
            {
                UIItem currency = new(itemType, stack);
                currency.SetSize(24, 24);
                currency.SetPos(i / count, -24, 0, 1);
                currency.DrawRec[0] = Color.Gold;
                Register(currency);
                i++;
            }
            Info.IsSensitive = true;
        }
        public override void LoadEvents()
        {
            base.LoadEvents();
        }
    }
}
