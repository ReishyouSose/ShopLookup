using ShopLookup.Content;
using Terraria.Localization;
using static Terraria.UI.Gamepad.UILinkPointNavigator;

namespace ShopLookup.UISupport.UIElements
{
    public class UINPCSlot : BaseUIElement
    {
        public Texture2D bg;
        public float scale;
        public int npcType;
        public int headIndex;
        public string npcName;
        public string SourceModName;
        public bool Permanent;
        private Texture2D head;
        private IEnumerable<Condition> conditions;
        public override Rectangle GetCanHitBox()
        {
            if (ParentElement.ParentElement != null)
            {
                return Rectangle.Intersect(ParentElement.ParentElement.HitBox(), HitBox());
            }
            return base.GetCanHitBox();
        }
        public UINPCSlot(int npcType, float scale = 1f, Texture2D bg = null)
        {
            SetSize(52 * scale, 52 * scale);
            this.scale = scale;
            this.npcType = npcType;
            this.bg = bg ?? TextureAssets.InventoryBack2.Value;
            Main.instance.LoadNPC(npcType);
            headIndex = NPC.TypeToDefaultHeadIndex(npcType);
            NPC npc = new();
            npc.SetDefaults(npcType);
            ModNPC mn = npc.ModNPC;
            Mod mod = mn?.Mod;
            if (headIndex == -1 && mn != null)
            {
                string path = mn.Texture + "_Head";
                if (mod.HasAsset(path.Replace(mod.Name + "/", "")))
                {
                    head = T2D(path);
                }
            }
            Permanent = !ExtraNPCInfo.NonTryGet(npcType, out conditions);
            npcName = npc.FullName;
            SourceModName = mn == null ? Language
                .GetTextValue("Mods.ShopLookup.Vanilla") : mod.DisplayName;
        }
        public override void LoadEvents()
        {
            Events.OnMouseOver += (evt) => Shortcuts.NPCS_LastHovered = -10 - npcType;
            Events.OnMouseOut += (evt) => Shortcuts.NPCS_LastHovered = -2;
        }
        public void ChangeNPC(int npcType)
        {
            this.npcType = npcType;
            Main.instance.LoadNPC(npcType);
            headIndex = NPC.TypeToDefaultHeadIndex(npcType);
            Permanent = !ExtraNPCInfo.NonTryGet(npcType, out conditions);
            NPC npc = new();
            npc.SetDefaults(npcType);
            ModNPC mn = npc.ModNPC;
            Mod mod = mn?.Mod;
            if (headIndex == -1 && mn != null)
            {
                string path = mn.Texture + "_Head";
                if (mod.HasAsset(path.Replace(mod.Name + "/", "")))
                {
                    head = T2D(path);
                }
            }
            Permanent = !ExtraNPCInfo.NonTryGet(npcType, out conditions);
            npcName = npc.FullName;
            SourceModName = mn == null ? Language
                .GetTextValue("Mods.ShopLookup.Vanilla") : mod.DisplayName;
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            var rec = HitBox();
            Texture2D tempBg = Permanent ? bg : (conditions.All(x => x.IsMet()) ? TextureAssets.InventoryBack3 : TextureAssets.InventoryBack2).Value;
            sb.Draw(tempBg, rec.TopLeft(), null, Color.White);
            Texture2D head;
            if (headIndex != -1)
            {
                head = TextureAssets.NpcHead[headIndex].Value;
            }
            else head = this.head;
            if (head != null)
            {
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
                if (!Permanent && (ShopLookup.Portable || ShopLookup.PermanentTips))
                {
                    Main.hoverItemName += "\n" + $"[c/FFA500:{Language.GetTextValue(SLUI.LocalKey + "NonPermanent")}]";
                    foreach (Condition c in conditions)
                    {
                        Main.hoverItemName += "\n" + (c.IsMet() ?
                            "[c/00E664:" : "[c/FF3264:") + $"{c.Description.Value}]";
                    }
                }
            }
        }
    }
}
