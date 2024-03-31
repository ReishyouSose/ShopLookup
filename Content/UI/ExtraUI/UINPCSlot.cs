using RUIModule;
using static ShopLookup.Content.Data.ShopNPCData;

namespace ShopLookup.Content.UI.ExtraUI
{
    public class UINPCSlot : UIIconSlot
    {
        public int npcType;
        public UINPCSlot(int npcType) : base(AssetLoader.Slot)
        {
            this.npcType = npcType;
            if (npcType == 0) return;
            Main.instance.LoadNPC(npcType);
        }
        public UINPCSlot(int npcType, Mod mod) : base(NPCHeads[npcType],1)
        {
            this.npcType = npcType;
            if (npcType == 0) return;
            Main.instance.LoadNPC(npcType);
            hoverText = ContentSamples.NpcsByNetId[npcType].TypeName;/* + "\n" +
                GTV("Source", " " + (mod.DisplayName ?? "Terraria"));*/
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            DrawSlot(sb);
            if (npcType == 0) return;
            if (icon == null)
            {
                Texture2D npc = TextureAssets.Npc[npcType].Value;
                int height = npc.Height / Main.npcFrameCount[npcType];
                Rectangle frame = new(0, 0, npc.Width, height);
                sb.Draw(npc, Center(), frame, Color.White, 0, frame.Size() / 2f, frame.AutoScale(), 0, 0);
            }
            else sb.SimpleDraw(icon, Center(), null, icon.Size() / 2f);
        }
        public void ChangeNPC(int npcType)
        {
            this.npcType = npcType;
            if (npcType == 0) return;
            Main.instance.LoadNPC(npcType);
            icon = NPCHeads[npcType];
        }
    }
}
