namespace ShopLookup.Content.UI.ExtraUI
{
    public class UIShopName : UIText
    {
        public readonly string key;

        public UIShopName(string value, string? key = null) : base(value)
        {
            key ??= value;
            this.key = key;
        }
        public UIShopName Clone() => new(text, key);
    }
}
