namespace ShopLookup.Content.Data
{
    internal static class ExtraShopDataBase
    {
        public static Dictionary<string, Dictionary<string, Texture2D>> ModShops { get; private set; }
        private static Dictionary<string, ExtraShop> extraShops;
        public static IEnumerable<ExtraShop> AllShops => extraShops.Values;
        public static void Load()
        {
            ModShops = [];
            extraShops = [];
            var slu = ShopLookup.Ins.Name;
            int pylonID = ItemID.TeleportationPylonVictory;
            Main.instance.LoadItem(pylonID);
            Register(slu, "Pylon", TextureAssets.Item[pylonID].Value, [.. Pylon()]);
            Register(slu, "Vanilla", AssetLoader.ExtraAssets["Vanilla"], [.. Vanilla()]);
        }
        public static void Register(string modNmae, string exShopType, Texture2D icon, params NPCShop[] shops)
        {
            if (ModShops.TryGetValue(modNmae, out var shopAndIcon))
                shopAndIcon.Add(exShopType, icon);
            else
                ModShops[modNmae] = new() { { exShopType, icon } };
            foreach (NPCShop shop in shops)
            {
                ExtraShop exShop = new()
                {
                    ModName = modNmae,
                    ExShopType = exShopType,
                    Name = shop.Name,
                    Entries = shop.Entries,
                };
                extraShops.Add(exShop.FullName, exShop);
            }
        }
        public static bool TryGetExtraShop(string modName, string exShopType, string name, out ExtraShop exShop)
        {
            exShop = default;
            foreach (ExtraShop ex in AllShops)
            {
                if (ex.ModName == modName && ex.ExShopType == exShopType && ex.Name == name)
                {
                    exShop = ex;
                    return true;
                }
            }
            return false;
        }
        private static int BuyPrice(int p = 0, int g = 0, int s = 0, int c = 0) => Item.buyPrice(p, g, s, c);
        private static IEnumerable<NPCShop> Pylon()
        {
            NPCShop vanilla = new(-1, "Vanilla");
            NPCShop mods = new(-1, "Mods");
            foreach (var entry in ShopNPCData.Pylons)
                (entry.Item.type < ItemID.Count ? vanilla : mods).Add(entry);
            vanilla.Add(ItemID.TeleportationPylonVictory, Condition.BestiaryFilledPercent(100));
            yield return vanilla;
            yield return mods;
        }
        private static IEnumerable<NPCShop> Vanilla()
        {
            yield return new NPCShop(-1, "Spawner")
                .Add([ItemID.SlimeCrown, ItemID.SuspiciousLookingEye])
                .Add([ItemID.WormFood, ItemID.BloodySpine], Condition.SmashedShadowOrb)
                .Add(ItemID.GoblinBattleStandard, BuyPrice(0, 5))
                .Add(ItemID.BloodMoonStarter, BuyPrice(0, 1))
                .Add(ItemID.Abeemination, BuyPrice(0, 3))
                .Add([ItemID.DeerThing, ItemID.ClothierVoodooDoll, ItemID.GuideVoodooDoll], BuyPrice(0, 5))
                .Add([ItemID.QueenSlimeCrystal, ItemID.SnowGlobe], BuyPrice(0, 5), Condition.Hardmode)
                .Add([ItemID.MechanicalEye, ItemID.MechanicalWorm, ItemID.MechanicalSkull], BuyPrice(0, 10), Condition.Hardmode)
                .Add(ItemID.MechdusaSummon, BuyPrice(0, 15), Condition.Hardmode, Condition.ZenithWorld)
                .Add(ItemID.TruffleWorm, BuyPrice(0, 20), Condition.Hardmode, Condition.InGlowshroom)
                .Add(ItemID.EmpressButterfly, BuyPrice(0, 20), Condition.DownedPlantera, Condition.InHallow, Condition.TimeNight)
                .Add([ItemID.PumpkinMoonMedallion, ItemID.NaughtyPresent], BuyPrice(0, 10), Condition.DownedPlantera)
                .Add(ItemID.LihzahrdPowerCell, BuyPrice(0, 10), Condition.DownedPlantera)
                .Add(ItemID.CelestialSigil, BuyPrice(1), Condition.DownedSolarPillar, Condition.DownedVortexPillar, Condition.DownedNebulaPillar, Condition.DownedStardustPillar);
            yield return new NPCShop(-1, "Fish")
                .Add(ItemID.ApprenticeBait, BuyPrice(0, 0, 3))
                .Add(ItemID.JourneymanBait, BuyPrice(0, 0, 5))
                .Add(ItemID.MasterBait, BuyPrice(0, 0, 15))
                .Add([ItemID.HotlineFishingHook, ItemID.LavaFishingHook], Condition.Hardmode)
                .Add([ItemID.GoldenFishingRod, ItemID.HighTestFishingLine, ItemID.AnglerEarring, ItemID.TackleBox]);
            yield return new NPCShop(-1, "Other")
                .Add(ItemID.FallenStar, BuyPrice(0, 0, 10))
                .Add(ItemID.Shellphone, BuyPrice(1))
                .Add(ItemID.GoldenBugNet, Condition.SmashedShadowOrb)
                .Add(ItemID.Coral, BuyPrice(0, 0, 5), Condition.InBeach)//珊瑚
                .Add(ItemID.Mushroom, BuyPrice(0, 0, 3), Condition.InShoppingZoneForest)//四种蘑菇
                .Add(ItemID.ViciousMushroom, BuyPrice(0, 0, 0, 50), Condition.InCrimson)
                .Add(ItemID.VileMushroom, BuyPrice(0, 0, 0, 50), Condition.InCorrupt)
                .Add(ItemID.GlowingMushroom, BuyPrice(0, 0, 1), Condition.InGlowshroom)
                .Add(ItemID.LadyBug, BuyPrice(0, 1), Condition.InShoppingZoneForest)//瓢虫
                .Add(ItemID.TargetDummy, BuyPrice(0, 0, 10))
                .Add(ItemID.TerrasparkBoots, BuyPrice(1, 14, 51, 4), Condition.DownedGoblinArmy);
        }
    }
}
