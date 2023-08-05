using ShopLookup.UISupport;

namespace ShopLookup
{
    public class ShopLookup : Mod
    {
        internal static ShopLookup Ins;
        internal static bool Portable;
        internal static bool PermanentTips;
        public UISystem uis;
        public ShopLookup()
        {
            Ins = this;
        }
    }
}