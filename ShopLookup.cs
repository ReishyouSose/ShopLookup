using rail;
using ShopLookup.UISupport;
using Terraria.ModLoader;

namespace ShopLookup
{
	public class ShopLookup : Mod
	{
		public static ShopLookup Ins;
		public UISystem uis;
		public ShopLookup()
		{
			Ins = this;
		}
    }
}