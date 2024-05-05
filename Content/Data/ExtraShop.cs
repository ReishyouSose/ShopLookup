namespace ShopLookup.Content.Data
{
    internal readonly struct ExtraShop
    {
        public string ModName { get; init; }
        public string ExShopType { get; init; }
        public string Name { get; init; }
        public IEnumerable<AbstractNPCShop.Entry> Entries { get; init; }
        public readonly string FullName => $"{ModName}/{ExShopType}/{Name}";
        public readonly string LocalPath => $"Mods.{ModName}.FakeShops.{ExShopType}";
        public readonly string TypeName => Language.GetTextValue(LocalPath + ".Label");
        public readonly string DisplayName => Language.GetTextValue(LocalPath + "." + Name);
        public readonly string DisableName => Language.GetTextValue(LocalPath + ".Disable");
    }
}
