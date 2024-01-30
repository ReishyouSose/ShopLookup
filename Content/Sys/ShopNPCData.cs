using System.Linq;
using static Terraria.ModLoader.NPCShop;

namespace ShopLookup.Content.Sys;

internal static class ShopNPCData
{
    internal static Mod FakeMod { get; private set; }
    internal static Dictionary<int, Mod> ModID { get; private set; }
    internal static Dictionary<Mod, HashSet<int>> ModNPCs { get; private set; }
    internal static Dictionary<int, Texture2D> NPCHeads { get; private set; }
    internal static Dictionary<int, Dictionary<int, int>> Currencys { get; private set; }
    internal static HashSet<Entry> Pylons { get; private set; }
    internal static void Load()
    {
        FakeMod = new();
        ModID = new();
        ModNPCs = new();
        NPCHeads = new();
        foreach (AbstractNPCShop shop in NPCShopDatabase.AllShops)
        {
            int type = shop.NpcType;
            ModNPC mn = ContentSamples.NpcsByNetId[type].ModNPC;
            Mod mod = mn?.Mod ?? FakeMod;
            if (!ModID.ContainsValue(mod))
            {
                ModID.Add(ModID.Count, mod);
            }
            ModNPCs.TryAdd(mod, new());
            var npcList = ModNPCs[mod];
            if (!npcList.Contains(type)) npcList.Add(type);
            NPCHeads.TryAdd(type, RequestNPCHead(type, mn, mod));
        }
        Pylons = NPCShopDatabase.GetPylonEntries().ToHashSet();
    }
    private const string _Head = "_Head";
    private static Texture2D RequestNPCHead(int type, ModNPC mn, Mod mod)
    {
        int headIndex = NPC.TypeToDefaultHeadIndex(type);
        if (headIndex > -1)
        {
            return TextureAssets.NpcHead[headIndex].Value;
        }
        if (mn != null)
        {
            string modFolder = mod.Name + "/";
            string path = mn.Texture + _Head;
            if (mod.HasAsset(path.Replace(modFolder, "")))
            {
                return T2D(path);
            }
            path = mn.GetType().FullName.Replace(modFolder, "") + _Head;
            if (mod.HasAsset(path))
            {
                return T2D(path);
            }
        }
        return null;
    }
    internal static void ReflectCurrency()
    {
        Currencys = new();
        Type target = typeof(CustomCurrencyManager);
        var info = target.GetField("_currencies", BindingFlags.Static | BindingFlags.NonPublic);
        var currencies = info.GetValue(target) as Dictionary<int, CustomCurrencySystem>;
        foreach (var (id, currencys) in currencies)
        {
            target = currencys.GetType();
            info = target.GetField("_valuePerUnit", BindingFlags.Instance | BindingFlags.NonPublic);
            var values = info.GetValue(target) as Dictionary<int, int>;
            var list = values.ToList();
            list.Sort((x, y) => y.Value.CompareTo(x.Value));
            Currencys[id] = list.ToDictionary(x => x.Key, y => y.Value);
        }
    }
}
