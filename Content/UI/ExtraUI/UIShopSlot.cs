using ShopLookup.Content.Data;
using System.Linq;
using Terraria.UI.Chat;

namespace ShopLookup.Content.UI.ExtraUI
{
    public class UIShopSlot : UIVnlPanel
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
                return condition == Condition.AnotherTownNPCNearby || condition == Condition.HappyEnoughToSellPylons;
            }
        }
        internal static bool Portable;
        internal static bool PermanentTips;
        internal static bool IgnoreUnknowCds;
        internal static readonly Condition empty = new("Mods.ShopLookup.NoCondition", () => true);

        public readonly UIItemSlot itemSlot;
        public readonly int npcType;
        public readonly ExType exType;
        private readonly UIImage vline;
        private readonly CdCheck[] cdChecks;
        private readonly UICurrency currency;
        private bool buying;
        private int buyTime;
        private int buyCD;
        private int buyStack;
        public UIShopSlot(AbstractNPCShop.Entry entry, int npcType) : base(0, 0, opacity: 0)
        {
            Info.SetMargin(10);
            Info.IsSensitive = true;
            Info.Width.Set(-30, 1);

            this.npcType = npcType;

            Item item = entry.Item;
            item.isAShopItem = true;
            itemSlot = new(item);
            itemSlot.SetCenter(26, 0, 0, 0.5f);
            Register(itemSlot);

            UIText name = new(item.Name);
            name.SetPos(62, 0);
            name.SetSize(-62, 30, 1);
            name.SetMaxWidth(name.Width);
            Register(name);

            currency = new(item.shopCustomPrice ?? item.value, item.shopSpecialCurrency);
            currency.SetPos(62, 30);
            Register(currency);

            var cds = entry.Conditions;
            if (cds.Any())
            {
                int i = 0;
                cdChecks = new CdCheck[cds.Count()];
                foreach (Condition c in cds)
                    cdChecks[i++] = new(c, Width - 82);
            }
            else
                cdChecks = [new(empty, Width - 82)];
            ReSetBuy();
        }
        public UIShopSlot(AbstractNPCShop.Entry entry, ExType exType) : base(0, 0, opacity: 0)
        {
            Info.SetMargin(10);
            Info.IsSensitive = true;
            Info.Width.Set(-30, 1);

            this.exType = exType;

            Item item = entry.Item;
            item.isAShopItem = true;
            itemSlot = new(item);
            itemSlot.SetCenter(26, 0, 0, 0.5f);
            Register(itemSlot);

            UIText name = new(item.Name);
            name.SetPos(62, 0);
            name.SetSize(-62, 30, 1);
            name.SetMaxWidth(name.Width);
            Register(name);

            currency = new(item.shopCustomPrice ?? item.value, item.shopSpecialCurrency);
            currency.SetPos(62, 30);
            Register(currency);

            var cds = entry.Conditions;
            if (cds.Any())
            {
                int i = 0;
                cdChecks = new CdCheck[cds.Count()];
                foreach (Condition c in cds)
                    cdChecks[i++] = new(c, Width - 82);
            }
            else
                cdChecks = [new(empty, Width - 82)];
        }
        public override void LoadEvents()
        {
            itemSlot.Events.OnLeftDown += CheckBuyItem;
            itemSlot.Events.OnRightDown += CheckBuyItem;
        }
        public override void Calculation()
        {
            if (ParentElement == null)
                return;
            float width = Info.Width.GetPixelBaseParent(ParentElement.Width);
            float height = 0;
            foreach (CdCheck cd in cdChecks)
            {
                cd.Calculate(width - 82);
                height += cd.TextY;
            }
            Info.Height.Pixel = height + 70;
            base.Calculation();
        }
        public override void Update(GameTime gt)
        {
            if (buying)
                BuyItem();
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            float y = 70;
            foreach (CdCheck cd in cdChecks)
            {
                Color color = Color.White;
                if (Info.IsMouseHover && !buying)
                {
                    if (Portable || PermanentTips)
                        color = cd.IsMet ? G : R;
                    if (cd.ignore)
                        color = Y;
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
        private bool CheckNPCAcitve()
        {
            if (ShopLookup.NonPermanentNPCs.TryGetValue(npcType, out var cds) && cds.All(x => x.IsMet()))
            {
                return true;
            }
            if (exType == ExType.None && npcType >= 0 && NPC.FindFirstNPC(npcType) >= 0)
            {
                return true;
            }
            return false;
        }
        private void CheckBuyItem(BaseUIElement uie)
        {
            if (!CheckNPCAcitve())
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
            if (!Main.LocalPlayer.CanAfford(currency.value, currency.currencyID))
            {
                currency.StartBlink();
                Main.NewText(GTV("CantAfford"));
                return;
            }
            Main.playerInventory = true;
            buying = true;
        }
        private void BuyItem()
        {
            if (!ContainsPoint(Main.MouseScreen))
            {
                ReSetBuy();
            }
            ref Item mouse = ref Main.mouseItem;
            int type = itemSlot.ContainedItem.type;
            if (mouse.type != type && mouse.type > 0)
            {
                ReSetBuy();
                return;
            }
            Player p = Main.LocalPlayer;
            if (Main.mouseLeft)
            {
                Item.NewItem(p.GetSource_GiftOrReward(), p.Hitbox, type);
                p.BuyItem(currency.value, currency.currencyID);
                ReSetBuy();
                return;
            }
            if (Main.mouseRight)
            {
                if (buyTime == 30)
                {
                    if (mouse.type == 0)
                    {
                        mouse = new(type);
                    }
                    else
                    {
                        mouse.stack++;
                    }
                    p.BuyItem(currency.value, currency.currencyID);
                    buyCD = buyTime = 20;
                }
                else if (buyCD <= 0)
                {
                    for (int i = 0; i < buyStack; i++)
                    {
                        if (ItemCheck(mouse, p))
                        {
                            mouse.stack++;
                        }
                        else
                            return;
                    }
                    buyCD = --buyTime / 2;
                    if (buyTime == 0)
                    {
                        buyTime = 3;
                        buyStack++;
                    }
                }
                buyCD--;
            }
        }
        private bool ItemCheck(Item hover, Player p)
        {
            if (hover.stack < hover.maxStack && p.CanAfford(currency.value, currency.currencyID))
            {
                p.BuyItem(currency.value, currency.currencyID);
                return true;
            }
            return ReSetBuy();
        }
        private bool ReSetBuy()
        {
            buying = false;
            buyTime = 30;
            buyCD = 0;
            buyStack = 1;
            return false;
        }
    }
}
