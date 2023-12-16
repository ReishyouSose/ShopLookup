using Terraria.ModLoader.IO;

namespace ShopLookup.Content
{
    public class VisitedNPCSys : ModSystem
    {
        internal static HashSet<int> visited_Vanilla;
        internal static HashSet<string> visited_Mod;
        internal static HashSet<int> visited_ModType;
        public static bool TryAdd(int type)
        {
            if (visited_Vanilla.Contains(type))
            {
                return false;
            }
            visited_Vanilla.Add(type);
            return true;
        }
        public static bool TryAdd(string fullName, bool onUpdate)
        {
            if (visited_Mod.Contains(fullName))
            {
                return false;
            }
            visited_Mod.Add(fullName);
            if (onUpdate)
            {
                visited_ModType.Add(ModContent.Find<ModNPC>(fullName).Type);
            }
            return true;
        }
        public static bool Contains(int type) => visited_Vanilla.Contains(type) || visited_ModType.Contains(type);
        public static void CheckActiveTownNPC()
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.townNPC)
                {
                    if (npc.type < NPCID.Count) TryAdd(npc.type);
                    else TryAdd(npc.ModNPC.FullName, false);
                }
            }
            visited_ModType = new();
            foreach (string fullName in visited_Mod)
            {
                int type = ModContent.Find<ModNPC>(fullName).Type;
                if (ExtraNPCInfo.NonTryGet(type, out _)) continue;
                visited_ModType.Add(type);
            }

        }
        public override void Load()
        {
            visited_Vanilla = new();
            visited_Mod = new();
        }
        public override void SaveWorldData(TagCompound tag)
        {
            if (visited_Vanilla.Any())
                tag["visited_Vanilla"] = visited_Vanilla.ToList();
            if (visited_Mod.Any())
                tag["visited_Mod"] = visited_Mod.ToList();
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.TryGet("visited_Vanilla", out List<int> vanilla))
                visited_Vanilla = vanilla.ToHashSet();
            if (tag.TryGet("visited_Mod", out List<string> modNPCs))
                visited_Mod = modNPCs.ToHashSet();
        }
        public override void PreSaveAndQuit()
        {
            visited_Vanilla.Clear();
            visited_Mod.Clear();
            visited_ModType.Clear();
        }
    }

    public class VisitedNPC : GlobalNPC
    {
        public override void PostAI(NPC npc)
        {
            if (Main.GameUpdateCount % 600 == 0 && npc.townNPC)
            {
                if (npc.type < NPCID.Count) VisitedNPCSys.TryAdd(npc.type);
                else VisitedNPCSys.TryAdd(npc.ModNPC.FullName, true);

                if (ExtraNPCInfo.NonTryGet(npc.type, out _)) return;
                SLUI ui = ShopLookup.Ins.uis.Elements[SLUI.NameKey] as SLUI;
                if (!ui.firstLoad) return;
                UIContainerPanel npcs = (UIContainerPanel)ui.side.ChildrenElements.First(x => x is UIContainerPanel);
                foreach (UINPCSlot slot in npcs.InnerUIE.Cast<UINPCSlot>())
                {
                    if (slot.npcType == npc.type)
                    {
                        slot.bg = TextureAssets.InventoryBack3.Value;
                    }
                }
            }
        }
    }
}
