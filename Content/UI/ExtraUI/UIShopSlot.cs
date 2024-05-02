using ShopLookup.Content.Data;
using System.Linq;
using Terraria.UI.Chat;
using static ShopLookup.Content.Sys.SLConfig;

namespace ShopLookup.Content.UI.ExtraUI
{
    public class UIShopSlot : UIVnlPanel
    {
        internal static UIShopSlot HoverSlot;
        internal static readonly Condition empty = new("Mods.ShopLookup.NoCondition", () => true);
        public readonly UIItemSlot itemSlot;
        public readonly int npcType;
        public readonly ExShopType exType;
        public readonly CdCheck[] cdChecks;
        public readonly UICurrency currency;
        private bool buying;
        private int buyTime;
        private int buyCD;
        private int buyStack;
        public readonly bool hardCode;
        public UIShopSlot(Item item, IEnumerable<Condition> cds = null, bool hardCode = true) : base(0, 0, opacity: 0)
        {
            item.isAShopItem = true;
            this.hardCode = hardCode;
            if (Ins.FlowLayout || hardCode)
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
        public UIShopSlot(AbstractNPCShop.Entry entry, int npcType) : this(entry.Item, entry.Conditions, false)
        {
            this.npcType = npcType;
        }
        public UIShopSlot(AbstractNPCShop.Entry entry, ExShopType exType) : this(entry.Item, entry.Conditions, false)
        {
            this.exType = exType;
        }
        public override void LoadEvents()
        {
            if (Ins.FlowLayout && !hardCode)
            {
                Events.OnMouseOver += evt => HoverSlot = this;
                Events.OnMouseOut += evt => HoverSlot = null;
            }
            itemSlot.Events.OnLeftDown += CheckBuyItem;
            itemSlot.Events.OnRightDown += CheckBuyItem;
        }
        public override void Calculation()
        {
            if (ParentElement == null)
                return;
            if (!Ins.FlowLayout && !hardCode)
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
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            float y = 70;
            foreach (CdCheck cd in cdChecks)
            {
                cd.Update(Info.IsMouseHover, buying);
                if (!Ins.FlowLayout && !hardCode)
                {
                    ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, cd.Desc,
                     HitBox().TopLeft() + new Vector2(72, y), cd.Color, 0, Vector2.Zero, Vector2.One, -1, 1.5f);
                    y += cd.TextY;
                }
            }
        }
        private bool CheckNPCAcitve()
        {
            if (hardCode)
                return true;
            if (ShopLookup.NonPermanentNPCs.TryGetValue(npcType, out var cds) && cds.All(x => x.IsMet()))
            {
                return true;
            }
            if (exType == ExShopType.None && npcType >= 0 && NPC.FindFirstNPC(npcType) >= 0)
            {
                return true;
            }
            return false;
        }
        private void CheckBuyItem(BaseUIElement uie)
        {
            if (!hardCode && exType == ExShopType.None && !CheckNPCAcitve())
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
