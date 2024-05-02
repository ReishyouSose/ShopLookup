using ShopLookup.Content.Data;

namespace ShopLookup.Content.UI.ExtraUI
{
    internal class UIExShopSlot : UINPCSlot
    {
        public readonly ExShopType exShopType;
        public UIExShopSlot(ExShop exShop) : base(0)
        {
            icon = exShop.icon;
            exShopType = exShop.exType;
            hoverText = GTV("SpecialShop." + exShopType + ".Label");
        }
    }
}
