using System.Linq;
using Terraria.ModLoader.IO;

namespace ShopLookup.Content.Data;

internal class ShopNPCData : ModSystem
{
    private const string PREFIX = "Mods.";
    internal static Mod FakeMod { get; private set; }
    internal static Dictionary<int, Mod> ModID { get; private set; }
    internal static Dictionary<int, Texture2D> ModIcon { get; private set; }
    internal static Dictionary<Mod, HashSet<int>> ModNPCs { get; private set; }
    internal static Dictionary<int, Texture2D> NPCHeads { get; private set; }
    internal static Dictionary<int, Dictionary<int, int>> Currencys { get; private set; }
    internal static IEnumerable<NPCShop.Entry> Pylons { get; private set; }
    internal static HashSet<int> PylonIDs { get; private set; }
    internal static HashSet<int> VisitedNPCs { get; private set; }
    internal static void Load(Mod slMod)
    {
        FakeMod = new();
        ModID = [];
        ModIcon = [];
        ModNPCs = [];
        NPCHeads = [];
        VisitedNPCs = [];
        static Texture2D GetModIcon(Mod mod)
        {
            if (mod == FakeMod)
            {
                return AssetLoader.ExtraAssets["Vanilla"];
            }
            if (mod.HasAsset("icon_small"))
            {
                return T2D(mod.Name + "/icon_small");
            }
            else return T2D(mod.Name + "icon");
        }
        foreach (AbstractNPCShop shop in NPCShopDatabase.AllShops)
        {
            int type = shop.NpcType;
            ModNPC mn = ContentSamples.NpcsByNetId[type].ModNPC;
            Mod mod = mn?.Mod ?? FakeMod;
            if (!ModID.ContainsValue(mod))
            {
                ModID.Add(ModID.Count, mod);
                ModIcon.Add(ModIcon.Count, GetModIcon(mod));
            }
            ModNPCs.TryAdd(mod, []);
            var npcList = ModNPCs[mod];
            if (!npcList.Contains(type)) npcList.Add(type);
            NPCHeads.TryAdd(type, RequestNPCHead(type, mn, mod));
        }
        ModID.Add(ModID.Count, slMod);
        ModIcon.Add(ModIcon.Count, GetModIcon(slMod));
        Pylons = NPCShopDatabase.GetPylonEntries();
        PylonIDs = Pylons.Select(x => x.Item.type).ToHashSet();
        ExtraShop.Load();
        ReflectCurrency();
    }
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
            string _Head = "_Head";
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
        Currencys = new() { { -1, new() } };
        int v = 1000000;
        for (int i = 74; i >= 71; i--)
        {
            Currencys[-1].Add(i, v);
            v /= 100;
        }
        var info = typeof(CustomCurrencyManager).GetField("_currencies", BindingFlags.Static | BindingFlags.NonPublic);
        var currencies = info.GetValue(null) as Dictionary<int, CustomCurrencySystem>;
        foreach (var (id, currencys) in currencies)
        {
            info = currencys.GetType().GetField("_valuePerUnit", BindingFlags.Instance | BindingFlags.NonPublic);
            var values = info.GetValue(currencys) as Dictionary<int, int>;
            var list = values.ToList();
            list.Sort((x, y) => y.Value.CompareTo(x.Value));
            Currencys[id] = list.ToDictionary(x => x.Key, y => y.Value);
            foreach (int itemid in Currencys[id].Keys)
            {
                Main.instance.LoadItem(itemid);
            }
        }
    }
    public override void SaveWorldData(TagCompound tag) => tag["visitedNPC"] = VisitedNPCs.ToArray();
    public override void LoadWorldData(TagCompound tag) => VisitedNPCs = [.. tag.Get<int[]>("visitedNPC")];
}
