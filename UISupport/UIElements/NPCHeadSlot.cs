namespace ShopLookup.UISupport.UIElements
{
    public class NPCHeadSlot : BaseUIElement
    {
        public Texture2D bg;
        public float scale;
        public int npcType;
        public int headIndex;
        public string npcName;
        public NPCHeadSlot(int npcType, float scale = 1f, Texture2D bg = null)
        {
            SetSize(52 * scale, 52 * scale);
            this.scale = scale;
            this.npcType = npcType;
            Main.instance.LoadNPC(npcType);
            headIndex = NPC.TypeToDefaultHeadIndex(npcType);
            this.bg = bg ?? TextureAssets.InventoryBack2.Value;
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
                float ZoomX = frame.Size().X / (52 * 0.75f);
                float ZoomY = frame.Size().Y / (52 * 0.75f);
                float zoom = MathF.Sqrt(ZoomX * ZoomX + ZoomY * ZoomY);
                sb.Draw(npc, rec.Center(), frame, Color.White, 0, frame.Size() / 2f, scale / (zoom > 1 ? zoom : 1), 0, 0);
            }
            if (Info.IsMouseHover)
            {
                Main.hoverItemName = npcName;
                /*ChatManager.DrawColorCodedString(sb, FontAssets.MouseText.Value, npcName,
                    Main.MouseScreen + Vector2.One * 16f, Color.White, 0, Vector2.Zero, Vector2.One);*/
            }
        }
    }
}
