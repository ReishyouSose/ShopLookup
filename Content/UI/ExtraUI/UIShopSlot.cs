using RUIModule;
using ShopLookup.Content.Data;
using System.Linq;
using System.Text;
using static RUIModule.RUIHelper;
using static ShopLookup.Content.Data.ShopNPCData;
using static ShopLookup.ShopLookup;

namespace ShopLookup.Content.UI.ExtraUI
{
    public class UIShopSlot(Texture2D icon) : UIIconSlot(icon)
    {
        public virtual UIShopSlot Clone() => new(icon);
    }
    public class UIShopSlotForNPC : UIShopSlot
    {
        public int npcType;
        public readonly bool nonPermanent;
        public UIShopSlotForNPC(int npcType) : this(npcType, FindHead(npcType)) { }
        private UIShopSlotForNPC(int npcType, Texture2D head) : base(head)
        {
            if (npcType <= 0)
                throw new Exception("NPCID must be > 0");
            this.npcType = npcType;
            Main.instance.LoadNPC(npcType);
            hoverText = ContentSamples.NpcsByNetId[npcType].TypeName;
            nonPermanent = NonPermanentNPCs.ContainsKey(npcType);
            if (nonPermanent)
            {
                overrideSlot = AssetLoader.ExtraAssets["Permanent"];
                Events.OnMouseOver += evt =>
                {
                    hoverText = ContentSamples.NpcsByNetId[npcType].TypeName;
                    foreach (Condition c in NonPermanentNPCs[npcType])
                    {
                        hoverText += new StringBuilder()
                            .AppendLine()
                            .Append("[c/")
                            .Append(c.IsMet() ? "00FF00" : "FF0000")
                            .Append(':')
                            .Append(c.Description.Value)
                            .Append(']');
                    }
                };
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
                Texture2D npc = TextureAssets.Npc[npcType].Value;
                int height = npc.Height / Main.npcFrameCount[npcType];
                Rectangle frame = new(0, 0, npc.Width, height);
                Vector2 size = frame.Size();
                sb.Draw(npc, Center(), frame, Color.White, 0, size / 2f, size.AutoScale(), 0, 0);
            }
            else
                sb.SimpleDraw(icon, Center(), null, icon.Size() / 2f);
        }
        public override UIShopSlot Clone() => new UIShopSlotForNPC(npcType, icon);
        private static Texture2D FindHead(int npcType)
        {
            if (SpecialNPCHeads.TryGetValue(npcType, out var head))
                return head;
            foreach (var info in ModsByName.Values)
            {
                if (info.npcAndHead?.TryGetValue(npcType, out head) == true)
                    return head;
            }
            return null;
        }
    }
    public class UIShopSlotForEx : UIShopSlot
    {
        public readonly string modName;
        public readonly string exShopType;

        public UIShopSlotForEx(string modName, string exShopType) : base(ExtraShopDataBase.ModShops[modName][exShopType])
        {
            this.modName = modName;
            this.exShopType = exShopType;
            hoverText = Language.GetTextValue($"Mods.{modName}.FakeShops.{exShopType}.Label");
            slotID = 14;
        }
        internal UIShopSlotForEx(ExtraShop extraShop) : this(extraShop.ModName, extraShop.ExShopType)
        {
        }

        public override UIShopSlot Clone() => new UIShopSlotForEx(modName, exShopType);
    }
}
