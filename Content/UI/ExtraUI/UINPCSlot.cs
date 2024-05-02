using RUIModule;
using System.Linq;
using static RUIModule.RUIHelper;
using static ShopLookup.Content.Data.ShopNPCData;
using static ShopLookup.ShopLookup;

namespace ShopLookup.Content.UI.ExtraUI
{
    public class UINPCSlot : UIIconSlot
    {
        public int npcType;
        public readonly bool nonPermanent;
        public UINPCSlot(int npcType) : this(npcType, null) { }
        public UINPCSlot(int npcType, Texture2D head) : base(head, NonPermanentNPCs.ContainsKey(npcType) ? 2 : 1)
        {
            if (npcType > 0)
            {
                this.npcType = npcType;
                Main.instance.LoadNPC(npcType);
                hoverText = ContentSamples.NpcsByNetId[npcType].TypeName;
                nonPermanent = NonPermanentNPCs.ContainsKey(npcType);
                if (nonPermanent)
                {
                    overrideSlot = AssetLoader.ExtraAssets["Permanent"];
                }
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
            if (icon == null)
            {
                if (npcType == 0)
                    return;
                Texture2D npc = TextureAssets.Npc[npcType].Value;
                int height = npc.Height / Main.npcFrameCount[npcType];
                Rectangle frame = new(0, 0, npc.Width, height);
                Vector2 size = frame.Size();
                sb.Draw(npc, Center(), frame, Color.White, 0, size / 2f, size.AutoScale(), 0, 0);
            }
            else
                sb.SimpleDraw(icon, Center(), null, icon.Size() / 2f);
        }
    }
}
