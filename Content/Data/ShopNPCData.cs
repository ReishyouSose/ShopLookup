using System.Linq;
using Terraria.ModLoader.IO;
using static RUIModule.RUIHelper;

namespace ShopLookup.Content.Data;

internal class ShopNPCData : ModSystem
{
    public readonly struct ModInfo(Mod mod, Texture2D icon = null, Dictionary<int, Texture2D> npcAndHead = null, Dictionary<string, Texture2D> extraShops = null)
    {
        public readonly Mod mod = mod;
        public readonly Texture2D icon = icon ?? GetModIcon(mod);
        public readonly Dictionary<int, Texture2D> npcAndHead = npcAndHead;
        public readonly Dictionary<string, Texture2D> extraShops = extraShops;
    }

    private const string IconSmall = "icon_small";
    private const string Icon = "icon";
    internal static Mod Vanilla { get; private set; }
    internal static Dictionary<string, ModInfo> ModsByName { get; private set; }
    internal static Dictionary<int, Dictionary<int, int>> Currencys { get; private set; }
    internal static IEnumerable<NPCShop.Entry> Pylons { get; private set; }
    internal static HashSet<int> PylonIDs { get; private set; }
    internal static HashSet<int> VisitedNPCs { get; private set; }
    private static Dictionary<Mod, Dictionary<int, Texture2D>> modNpcs = [];
    private static bool init;
    internal static void Load()
    {
        init = false;
        ModsByName = [];
        Vanilla = new();
        modNpcs = [];
        foreach (AbstractNPCShop shop in NPCShopDatabase.AllShops)
        {
            int type = shop.NpcType;
            ModNPC mn = ContentSamples.NpcsByNetId[type].ModNPC;
            Mod mod = mn?.Mod ?? Vanilla;
            modNpcs.TryAdd(mod, []);
            modNpcs[mod].TryAdd(type, RequestNPCHead(type, mn, mod));
        }
        Pylons = NPCShopDatabase.GetPylonEntries();
        PylonIDs = Pylons.Select(x => x.Item.type).ToHashSet();
        PylonIDs.Add(ItemID.TeleportationPylonVictory);
        ModsByName.Add("Terraria", new(Vanilla, AssetLoader.ExtraAssets["Vanilla"], modNpcs[Vanilla]));
        ExtraShopDataBase.Load();
        ReflectCurrency();
        VisitedNPCs = [];
    }
    public static void FinishSetup()
    {
        if (!init)
        {
            foreach (Mod mod in ModLoader.Mods)
            {
                if (modNpcs.TryGetValue(mod, out var npcInfo) | ExtraShopDataBase.ModShops.TryGetValue(mod.Name, out var extra))
                {
                    ModsByName.Add(mod.Name, new(mod, GetModIcon(mod), npcInfo, extra));
                }
            }
            modNpcs = null;
            init = true;
        }
    }
    private static Texture2D GetModIcon(Mod mod) => T2D(mod.Name + "/" + (mod.HasAsset(IconSmall) ? IconSmall : Icon));
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
