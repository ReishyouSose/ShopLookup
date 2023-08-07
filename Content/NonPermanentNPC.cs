using Terraria.Localization;

namespace ShopLookup.Content
{
    public static class NonPermanentNPC
    {
        private static Dictionary<int, IEnumerable<Condition>> NonPermanent;
        public static readonly Condition NoC = new(Language.GetText(SLUI.LocalKey + "NoCondition"), () => true);
        public static bool IsNull() => NonPermanent == null;
        public static void Load()
        {
            NonPermanent = new();
            Add(NPCID.TravellingMerchant, new Condition(Language.GetText(SLUI.LocalKey + "Travel"),
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
            Add(NPCID.SkeletonMerchant, Condition.InRockLayerHeight);
        }
        public static void Add(int type, params Condition[] conditions)
        {
            NonPermanent.Add(type, conditions);
        }
        public static void Add(int type, IEnumerable<Condition> conditions)
        {
            NonPermanent.Add(type, conditions);
        }
        public static void Conbine(int type, IEnumerable<Condition> source, IEnumerable<Condition> news)
        {
            NonPermanent[type] = source.AddRange(news);
        }
        public static bool TryGet(int type, out IEnumerable<Condition> c) => NonPermanent.TryGetValue(type, out c);
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

    }
}
