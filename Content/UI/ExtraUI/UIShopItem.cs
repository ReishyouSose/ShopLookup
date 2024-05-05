using ShopLookup.Content.Data;
using System.Linq;
using Terraria.UI.Chat;
using static ShopLookup.Content.Sys.SLConfig;

namespace ShopLookup.Content.UI.ExtraUI
{
    public class UIShopItem : UIVnlPanel
    {
        internal static UIShopItem HoverSlot;
        internal static readonly Condition empty = new("Mods.ShopLookup.NoCondition", () => true);
        public readonly UIItemSlot itemSlot;
        public readonly CdCheck[] cdChecks;
        public readonly UICurrency currency;
        private bool buying;
        private int buyTime;
        private int buyCD;
        private int buyStack;
        public readonly bool onlyCanBuy;
        public UIShopItem(Item item, IEnumerable<Condition> cds = null, bool onlyCanBuy = true) : base(0, 0, opacity: 0)
        {
            item.isAShopItem = true;
            this.onlyCanBuy = onlyCanBuy;
            if (SLPanel.flowLayout || onlyCanBuy)
            {
                Info.IsSensitive = true;
                SetSize(52, 52);
                itemSlot = new(item);
                Register(itemSlot);
                currency = new(item.shopCustomPrice ?? item.value, item.shopSpecialCurrency);

                if (cds?.Any() == true)
                {
                    int i = 0;
                    cdChecks = new CdCheck[cds.Count()];
                    foreach (Condition c in cds)
                        cdChecks[i++] = new(c, -1);
                }
                else
                    cdChecks = [new(empty, -1)];
            }
            else
            {
                Info.SetMargin(10);
                Info.IsSensitive = true;
                Info.Width.Set(0, 1);

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

                if (cds?.Any() == true)
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
            if (!SLPanel.flowLayout && !onlyCanBuy)
            {
                float width = Info.Width.GetPixelBaseParent(ParentElement.Width);
                float height = 0;
                foreach (CdCheck cd in cdChecks)
                {
                    cd.Calculate(width - 82);
                    height += cd.TextY;
                }
                Info.Height.Pixel = height + 70;
            }
            base.Calculation();
        }
        public override void Update(GameTime gt)
        {
            if (buying)
                BuyItem();
            if (Info.IsMouseHover && SLPanel.flowLayout && !onlyCanBuy)
                HoverSlot = this;
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            float y = 70;
            foreach (CdCheck cd in cdChecks)
            {
                if (Ins.Portable || Ins.PermanentTips)
                {
                    cd.Update(Info.IsMouseHover, buying);
                }
                if (!SLPanel.flowLayout && !onlyCanBuy)
                {
                    ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, cd.Desc,
                     HitBox().TopLeft() + new Vector2(72, y), cd.Color, 0, Vector2.Zero, Vector2.One, -1, 1.5f);
                    y += cd.TextY;
                }
            }
        }
        public virtual bool CheckAcitve() => true;
        private void CheckBuyItem(BaseUIElement uie)
        {
            if (!Ins.Portable)
                return;
            if (!onlyCanBuy && !CheckAcitve())
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
            if (!Info.IsMouseHover)
            {
                ReSetBuy();
            }
            ref Item mouse = ref Main.mouseItem;
            int type = itemSlot.item.type;
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
            else
                ReSetBuy();
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
    public class UIShopItemForNPC(int npcType, AbstractNPCShop.Entry entry) : UIShopItem(entry.Item, entry.Conditions, false)
    {
        public readonly int npcType = npcType;
        public override bool CheckAcitve()
        {
            if (ShopLookup.NonPermanentNPCs.TryGetValue(npcType, out var cds) && cds.AllMet())
                return true;
            if (ShopNPCData.VisitedNPCs.Contains(npcType))
                return true;
            return NPC.FindFirstNPC(npcType) > -1;
        }
    }
    public class UIShopItemForEx(string exShopType, string modName, AbstractNPCShop.Entry entry) : UIShopItem(entry.Item, entry.Conditions, false)
    {
        public readonly string exShopType;
        public readonly string modName;
    }
}
