using System.Linq;

namespace ShopLookup.Content.Data
{
    public enum ExType
    {
        Pylon,
        Vanilla,
        QoT,
    }
    internal readonly struct ExShop(ExType exType, Texture2D icon, AbstractNPCShop shop)
    {
        public readonly ExType exType = exType;
        public readonly Texture2D icon = icon;
        public readonly AbstractNPCShop shop = shop;
    }

    internal static class ExtraShop
    {
        public static List<ExShop> extraShops;
        public static void Load()
        {
            NPCShop vanilla = new(0);
            NPCShop qot = new(0);
            extraShops = new();

        }
        private static ExShop Pylon()
        {
            NPCShop shop = new(-1);
            shop.Add(ShopNPCData.Pylons.ToArray());
            int pylonID = ItemID.TeleportationPylonVictory;
            Main.instance.LoadItem(pylonID);
            return new(ExType.Pylon, TextureAssets.Item[pylonID].Value, shop);
        }
        private static ExShop Vanilla()
        {
            NPCShop shop = new(-1);

        }
        private static ExShop Qot()
        {

        }
    }
}
