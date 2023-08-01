using Terraria.Localization;

namespace ShopLookup.UISupport.UIElements
{
    public class ShopNPCSlot : BaseUIElement
    {
        public Texture2D bg;
        public float scale;
        public int npcType;
        public int headIndex;
        public string npcName;
        public string shopName;
        public string SourceModName;
        public ShopNPCSlot(int npcType, string shopName, float scale = 1f, Texture2D bg = null)
        {
            SetSize(52 * scale, 52 * scale);
            this.scale = scale;
            this.npcType = npcType;
            this.shopName = shopName;
            this.bg = bg ?? TextureAssets.InventoryBack2.Value;
            Main.instance.LoadNPC(npcType);
            headIndex = NPC.TypeToDefaultHeadIndex(npcType);
            NPC npc = new();
            npc.SetDefaults(npcType);
            npcName = npc.FullName;
            SourceModName = npc.ModNPC == null ? Language
                .GetTextValue("Mods.ShopLookup.Vanilla") : npc.ModNPC.Mod.DisplayName;
        }
        public void ChangeNPC(int npcType, string shopName)
        {
            this.npcType = npcType;
            this.shopName = shopName;
            Main.instance.LoadNPC(npcType);
            headIndex = NPC.TypeToDefaultHeadIndex(npcType);
            NPC npc = new();
            npc.SetDefaults(npcType);
            npcName = npc.FullName;
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            var rec = HitBox();
            sb.Draw(bg, rec.TopLeft(), null, Color.White);
            if (headIndex != -1)
            {
                Texture2D head = TextureAssets.NpcHead[headIndex].Value;
                sb.Draw(head, rec.Center(), null, Color.White, 0, head.Size() / 2f, scale, 0, 0);
            }
            else
            {
                Texture2D npc = TextureAssets.Npc[npcType].Value;
                int height = npc.Height / Main.npcFrameCount[npcType];
                Rectangle frame = new(0, 0, npc.Width, height);
                sb.Draw(npc, rec.Center(), frame, Color.White, 0, frame.Size() / 2f, scale * frame.AutoScale(), 0, 0);
            }
            if (Info.IsMouseHover)
            {
                Main.hoverItemName = npcName + "\n" + SourceModName;
            }
        }
    }
}
