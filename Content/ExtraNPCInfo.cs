using Terraria.Localization;

namespace ShopLookup.Content
{
    public static class ExtraNPCInfo
    {
        private static Dictionary<int, IEnumerable<Condition>> NonPermanent;
        private static Dictionary<int, (string, LocalizedText)> ShopNameLT;
        public static readonly Condition NoC = new(Language.GetText(SLUI.LocalKey + "NoCondition"), () => true);
        private static bool Loaded;
        public static bool IsNull() => !Loaded;
        public static void Load()
        {
            Loaded = true;
            NonPermanent = new();
            NonAdd(NPCID.TravellingMerchant, new Condition(Language.GetText(SLUI.LocalKey + "Travel"),
                () =>
                {
                    int count = 0;
                    foreach (NPC npc in Main.npc)
                    {
                        if (npc.active && npc.townNPC)
                        {
                            count++;
                            if (count >= 2) return true;
                        }
                    }
                    return false;
                }));
            NonAdd(NPCID.SkeletonMerchant, Condition.InRockLayerHeight);
            ShopNameLT = new();
        }
        public static void NonAdd(int type, params Condition[] conditions)
        {
            NonPermanent.Add(type, conditions);
        }
        public static void NonAdd(int type, IEnumerable<Condition> conditions)
        {
            NonPermanent.Add(type, conditions);
        }
        public static void NonConbine(int type, IEnumerable<Condition> source, IEnumerable<Condition> news)
        {
            NonPermanent[type] = source.AddRange(news);
        }
        public static bool NonTryGet(int type, out IEnumerable<Condition> c) => NonPermanent.TryGetValue(type, out c);
        private static IEnumerable<Condition> AddRange(this IEnumerable<Condition> source, IEnumerable<Condition> news)
        {
            if (source == null || news == null)
            {
                yield return NoC;
                yield break;
            }
            foreach (Condition t in source)
            {
                yield return t;
            }
            foreach (Condition t in news)
            {
                yield return t;
            }
        }
        public static bool NameTryGet(int type, out (string index, LocalizedText text) info) => ShopNameLT.TryGetValue(type, out info);
        public static void NameAdd(int type, string index, LocalizedText text) => ShopNameLT.Add(type, (index, text));
    }
}
