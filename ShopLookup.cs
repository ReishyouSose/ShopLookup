using ShopLookup.Content;
using ShopLookup.UISupport;

namespace ShopLookup
{
    public class ShopLookup : Mod
    {
        internal static ShopLookup Ins;
        public UISystem uis;
        public ShopLookup()
        {
            Ins = this;
        }
    }
}