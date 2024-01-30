using RUIModule;
using RUIModule.RUIElements;
using RUIModule.RUISys;
using static ShopLookup.Content.Sys.ShopNPCData;

namespace ShopLookup.Content.UI.ExtraUI
{
    public class UINPCSlot : UIImage
    {
        public int npcType;
        private readonly Texture2D head;
        public UINPCSlot(int npcType, Mod mod, Vector2? size = null, Color? color = null) : base(AssetLoader.ExtraAssets["Slot"], size, color)
        {
            this.npcType = npcType;
            Main.instance.LoadNPC(npcType);
            head = NPCHeads[npcType];
            hoverText = ContentSamples.NpcsByNetId[npcType].TypeName + "\n" +
                GTV("Source", " " + (mod.DisplayName ?? "Terraria"));
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            if (head == null)
            {
                Texture2D npc = TextureAssets.Npc[npcType].Value;
                int height = npc.Height / Main.npcFrameCount[npcType];
                Rectangle frame = new(0, 0, npc.Width, height);
                sb.Draw(npc, Center(), frame, Color.White, 0, frame.Size() / 2f, frame.AutoScale(), 0, 0);
            }
            else sb.SimpleDraw(head, Center(), null, head.Size() / 2f);
        }
    }
}
