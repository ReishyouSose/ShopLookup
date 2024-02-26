using System.Linq;
using Terraria.UI.Chat;

namespace ShopLookup.Content.UI.ExtraUI
{
    public class UIShopSlot : BaseUIElement
    {
        private class CdCheck
        {
            public readonly Condition condition;
            public readonly bool ignore;
            private readonly string desc;
            private int blinkTime;
            public bool Blink
            {
                get
                {
                    if (blinkTime > 0)
                    {
                        blinkTime--;
                        return true;
                    }
                    return false;
                }
            }
            public Color BlinkColor => blinkTime / 6 % 2 == 0 ? R : Color.White;
            public string Desc { get; private set; }
            public float TextY { get; private set; }
            public bool IsMet => condition.IsMet();
            public CdCheck(Condition condition, float maxWidth)
            {
                this.condition = condition;
                ignore = IgnoreCondition(out desc);
                Desc = desc;
                Calculate(maxWidth);
            }
            public void Calculate(float maxWidth)
            {
                Desc = FontAssets.MouseText.Value.CreateWrappedText(desc, maxWidth);
                TextY = ChatManager.GetStringSize(FontAssets.MouseText.Value, Desc, Vector2.One).Y;
            }
            public void StartBlink()
            {
                blinkTime = 36;
            }

            private bool IgnoreCondition(out string desc)
            {
                if (condition.Description.Key == "" || condition.Description.Value == "")
                {
                    desc = GTV("UnknowCds");
                    return true;
                }
                desc = condition.Description.Value;
                return condition == Condition.AnotherTownNPCNearby || condition == Condition.HappyEnough;
            }
        }
        internal static bool Portable;
        internal static bool PermanentTips;
        internal static bool IgnoreUnknowCds;
        internal static readonly Condition empty = new("Mods.ShopLookup.NoCondition", () => true);

        public readonly UIItemSlot itemSlot;
        public readonly int npcType;
        private bool buying;
        private readonly UIImage vline;
        private readonly CdCheck[] cdChecks;
        private readonly UICurrency currency;
        public UIShopSlot(AbstractNPCShop.Entry entry, int npcType, bool last)
        {
            Info.IsSensitive = true;
            Info.Width.Percent = 1;

            this.npcType = npcType;

            Item item = entry.Item;
            item.isAShopItem = true;
            itemSlot = new(item);
            itemSlot.SetCenter(26, 0, 0, 0.5f);
            //itemSlot.DrawRec[0] = Color.Blue;
            Register(itemSlot);

            vline = new(TextureAssets.MagicPixel.Value);
            vline.SetSize(2, -20, 0, 1);
            vline.SetPos(62, 10);
            Register(vline);

            UIText name = new(item.Name);
            name.SetPos(72, 10);
            name.SetSize(-72, 30, 1);
            name.SetMaxWidth(name.Width);
            Register(name);

            currency = new(item.shopCustomPrice ?? item.value, item.shopSpecialCurrency);
            currency.SetPos(72, 40);
            Register(currency);

            var cds = entry.Conditions;
            if (cds.Any())
            {
                int i = 0;
                cdChecks = new CdCheck[cds.Count()];
                foreach (Condition c in cds) cdChecks[i++] = new(c, Width - 72);
            }
            else cdChecks = [new(empty, Width - 72)];

            if (!last)
            {
                UIImage hline = new(TextureAssets.MagicPixel.Value);
                hline.SetSize(-20, 2, 1, 0);
                hline.SetPos(10, -2, 0, 1);
                Register(hline);
            }
        }
        public override void LoadEvents()
        {
            itemSlot.Events.OnLeftDown += CheckBuyItem;
            itemSlot.Events.OnRightDown += CheckBuyItem;
        }
        public override void Calculation()
        {
            if (ParentElement == null) return;
            float width = Info.Width.GetPixelBaseParent(ParentElement.Width);
            float height = 0;
            foreach (CdCheck cd in cdChecks)
            {
                cd.Calculate(width - 72);
                height += cd.TextY;
            }
            Info.Height.Pixel = height + 70;
            base.Calculation();
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            float y = 70;
            foreach (CdCheck cd in cdChecks)
            {
                Color color = Color.White;
                if (Info.IsMouseHover && !buying)
                {
                    if (Portable || PermanentTips) color = cd.IsMet ? G : R;
                    if (cd.ignore) color = Y;
                }
                if (cd.Blink)
                {
                    color = cd.BlinkColor;
                }
                ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, cd.Desc,
                     HitBox().TopLeft() + new Vector2(72, y), color, 0, Vector2.Zero, Vector2.One, -1, 1.5f);
                y += cd.TextY;
            }
        }
        private void CheckBuyItem(BaseUIElement uie)
        {
            if (npcType >= 0 && NPC.FindFirstNPC(npcType) < 1 /*|| VisitedNPCSys.Contains(npcType)*/ )
            {
                Main.NewText(GTV("NoActive"));
                return;
            }
            foreach (CdCheck cd in cdChecks)
            {
                bool noMet = false;
                if (!cd.IsMet)
                {
                    cd.StartBlink();
                    noMet = true;
                }
                if (noMet)
                {
                    Main.NewText(GTV("NoMet"));
                    return;
                }
            }
            if (currency.color == R)
            {
                currency.StartBlink();
                Main.NewText(GTV("CantAfford"));
                return;
            }
            buying = true;
        }
        private void BuyItem()
        {
        }
    }
}
