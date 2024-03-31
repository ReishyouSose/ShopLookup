namespace ShopLookup.Content.Data
{
    internal static class ExtraShop
    {
        internal enum ExType
        {
            None,
            Vanilla,
            QoT,
        }
        internal static Dictionary<ExType, AbstractNPCShop> extraShops;
        internal static void Load()
        {
            NPCShop vanilla = new(0);
            NPCShop qot = new(0);
            extraShops = new()
            {
                { ExType.Vanilla, vanilla },
                { ExType.QoT, qot },
            };
        }
    }
}
