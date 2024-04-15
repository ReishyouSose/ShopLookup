using RUIModule;
using System.Linq;
using static ShopLookup.Content.Data.ShopNPCData;
using static ShopLookup.ShopLookup;

namespace ShopLookup.Content.UI.ExtraUI
{
    public class UINPCSlot : UIIconSlot
    {
        public int npcType;
        public readonly bool nonPermanent;
        public UINPCSlot(int npcType) : base(AssetLoader.Slot)
        {
            this.npcType = npcType;
            if (npcType == 0)
                return;
            Main.instance.LoadNPC(npcType);
        }
        public UINPCSlot(int npcType, Mod mod) : base(SpecialNPCHeads.TryGetValue(npcType, out Texture2D head) ?
            head : NPCHeads[npcType], NonPermanentNPCs.ContainsKey(npcType) ? 2 : 1)
        {
            this.npcType = npcType;
            if (npcType == 0)
                return;
            Main.instance.LoadNPC(npcType);
            hoverText = ContentSamples.NpcsByNetId[npcType].TypeName;/* + "\n" +
                GTV("Source", " " + (mod.DisplayName ?? "Terraria"));*/
            nonPermanent = NonPermanentNPCs.ContainsKey(npcType);
            if (nonPermanent)
            {
                overrideSlot = AssetLoader.ExtraAssets["Permanent"];
            }
        }
        public override void Update(GameTime gt)
        {
            if (nonPermanent)
            {
                if (overrideSlot == null)
                {
                    if (NonPermanentNPCs[npcType].Any(x => !x.IsMet()))
                    {
                        overrideSlot = AssetLoader.ExtraAssets["Permanent"];
                    }
                }
                else if (NonPermanentNPCs[npcType].All(x => x.IsMet()))
                {
                    overrideSlot = null;
                    slotID = 2;
                }
            }
            else
                slotID = VisitedNPCs.Contains(npcType) ? 2 : 1;
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            DrawSlot(sb);
            if (npcType == 0)
                return;
            if (icon == null)
            {
                Texture2D npc = TextureAssets.Npc[npcType].Value;
                int height = npc.Height / Main.npcFrameCount[npcType];
                Rectangle frame = new(0, 0, npc.Width, height);
                sb.Draw(npc, Center(), frame, Color.White, 0, frame.Size() / 2f, frame.AutoScale(), 0, 0);
            }
            else
                sb.SimpleDraw(icon, Center(), null, icon.Size() / 2f);
        }
        public void ChangeNPC(int npcType)
        {
            this.npcType = npcType;
            if (npcType == 0)
                return;
            Main.instance.LoadNPC(npcType);
            icon = NPCHeads[npcType];
        }
    }
}
