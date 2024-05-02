using RUIModule;

namespace ShopLookup
{
    public class ShopLookup : Mod
    {
        internal static Dictionary<int, Dictionary<string, LocalizedText>> ShopNames { get; private set; }
        internal static Dictionary<int, Condition[]> NonPermanentNPCs { get; private set; }
        internal static Dictionary<int, Texture2D> SpecialNPCHeads { get; private set; }
        public override void Load()
        {
            RUIManager.mod = this;
            AssetLoader.ExtraLoad += AssetLoader_ExtraLoad;
            AddContent<RUIManager>();
            ShopNames = [];
            NonPermanentNPCs = [];
            NonPermanentNPC(NPCID.TravellingMerchant, new Condition(Language.GetText("Mods.ShopLookup.Travel"),
                () =>
                {
                    int count = 0;
                    foreach (NPC npc in Main.npc)
                    {
                        if (npc.active && npc.townNPC)
                        {
                            if (++count >= 2)
                                return true;
                        }
                    }
                    return false;
                }));
            NonPermanentNPC(NPCID.SkeletonMerchant, Condition.InRockLayerHeight);
            SpecialNPCHeads = [];
        }

        private void AssetLoader_ExtraLoad(Dictionary<string, Texture2D> extraAssets)
        {
            string[] files = ["All", "Coins", "NoIcon", "QoT", "Slot", "Vanilla", "Permanent",
                "StripLayout", "FlowLayout", "Search", "OnlyCanBuy"];
            string path = GetType().Namespace + "/Assets/";
            foreach (string file in files)
            {
                extraAssets[file] = RUIHelper.T2D(path + file);
            }
            string[] itemFilters = ["Weapon", "Armor", "Vanity", "Tools", "Materials", "Furniture",
                "BuildingBlock", "Accessories", "MiscAccessories", "MiscFallback", "Consumables"];
            path += "ItemFilter/";
            foreach (string filter in itemFilters)
            {
                extraAssets[filter] = RUIHelper.T2D(path + filter);
            }
            //AssetLoader.edgeBlur = ModContent.Request<Effect>("ShopLookup/Assets/EdgeBlur", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }
        public override object Call(params object[] args)
        {
            var (index, type) = ModCall(args);
            if (index == -1)
            {
                Logger.Info("Success");
                return true;
            }
            if (index == 0)
            {
                Logger.Warn("Wrong method type");
                return false;
            }
            else
            {
                Logger.Warn($"params {index + 1} error,should be {type.Name}");
                return false;
            }
        }
        public static (int, Type) ModCall(params object[] args)
        {
            try
            {
                if (args[0] is int method)
                {
                    return method switch
                    {
                        0 => LocalizedTextShopName(args),
                        1 => NonPermanentNPC(args),
                        2 => SpecialNPCHead(args),
                        _ => (0, typeof(int)),
                    };
                    ;
                }
                return (0, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (0, null);
            }
        }
        private static (int, Type) LocalizedTextShopName(params object[] args)
        {
            if (args[1] is int npc)
            {
                if (args[2] is Dictionary<string, LocalizedText> shopLocals)
                {
                    LocalizedTextShopName(npc, shopLocals);
                    return (-1, null);
                }
                else
                    return (2, typeof(Dictionary<string, LocalizedText>));
            }
            else
                return (1, typeof(int));
        }
        private static (int, Type) NonPermanentNPC(params object[] args)
        {
            if (args[1] is int npc)
            {
                if (args[2] is Condition[] cds)
                {
                    NonPermanentNPC(npc, cds);
                    return (-1, null);
                }
                else
                    return (2, typeof(Condition[]));
            }
            else
                return (1, typeof(int));
        }
        private static (int, Type) SpecialNPCHead(params object[] args)
        {
            if (args[1] is int npc)
            {
                if (args[2] is Texture2D head)
                {
                    SpecialNPCHead(npc, head);
                    return (-1, null);
                }
                else
                    return (2, typeof(Texture2D));
            }
            else
                return (1, typeof(int));
        }
        /// <summary>
        /// ModCall Index => 0
        /// </summary>
        /// <param name="npc">The type for target shop npc</param>
        /// <param name="shopLocals">Key is ShopName, Value is LocalizeText's Key (Don't need "Mods.YourMod.")</param>
        private static void LocalizedTextShopName(int npc, Dictionary<string, LocalizedText> shopLocals)
        {
            ShopNames[npc] = shopLocals;
        }
        /// <summary>
        /// ModCall Index => 1
        /// </summary>
        /// <param name="type">The type for target NonPermanent npc</param>
        /// <param name="cds">The spawn conditions for this npc</param>
        private static void NonPermanentNPC(int type, params Condition[] cds) => NonPermanentNPCs.Add(type, cds);
        /// <summary>
        /// ModCall Index => 2
        /// </summary>
        /// <param name="type">The type for target npc</param>
        /// <param name="head">Special head sprite</param>
        private static void SpecialNPCHead(int type, Texture2D head) => SpecialNPCHeads[type] = head;
    }
}