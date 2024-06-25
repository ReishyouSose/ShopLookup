using ShopLookup.Content.Data;
using ShopLookup.Content.Sys;
using System.Linq;
using Terraria.UI.Chat;

namespace ShopLookup.Content.UI.ExtraUI
{
    public class UIShopItem : UIVnlPanel
    {
        protected const string CD = "Condition";
        internal static UIShopItem HoverSlot;
        internal static readonly Condition empty = new("Mods.ShopLookup.Info.NoCondition", () => true);
        public readonly UIItemSlot itemSlot;
        public readonly CdCheck[] cdChecks;
        public readonly UICurrency currency;
        private bool buying;
        private int buyTime;
        private int buyCD;
        private int buyStack;
        private readonly bool flow;
        public bool OnlyCanBuy { get; init; }
        protected static Mod Mod => ShopLookup.Ins;
        public UIShopItem(Item item, bool flow, IEnumerable<Condition> cds = null) : base(0, 0, opacity: 0)
        {
            Info.IsSensitive = true;
            item.stack = NotUseGlobalStack() ? item.stack : Math.Min(SLUI.BuyStack, item.maxStack);
            currency = new((item.shopCustomPrice ?? item.value) * (NotUseGlobalStack() ? 1 : item.stack), item.shopSpecialCurrency);
            Register(currency);
            itemSlot = new(item);
            Register(itemSlot);
            if (flow)
            {
                this.flow = true;
                SetSize(52, 52);
                currency.Info.IsVisible = false;
            }
            else
            {
                Info.SetMargin(10);
                Info.Width.Set(0, 1);

                itemSlot.SetCenter(26, 0, 0, 0.5f);

                UIText name = new(item.Name);
                name.SetPos(62, 0);
                name.SetSize(-62, 30, 1);
                name.SetMaxWidth(name.Width);
                Register(name);

                currency.SetPos(62, 30);
            }

            if (cds?.Any() == true)
            {
                int i = 0;
                cdChecks = new CdCheck[cds.Count()];
                foreach (Condition c in cds)
                    cdChecks[i++] = new(c, -1);
            }
            else
                cdChecks = [new(empty, -1)];
            ReSetBuy();
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
            if (!flow)
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
            if (Info.IsMouseHover)
                HoverSlot = this;
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            float y = 70;
            foreach (CdCheck cd in cdChecks)
            {
                cd.Update(Info.IsMouseHover, buying);
                if (!flow)
                {
                    ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, cd.Desc,
                     HitBox().TopLeft() + new Vector2(72, y), cd.Color, 0, Vector2.Zero, Vector2.One, -1, 1.5f);
                    y += cd.TextY;
                }
            }
        }
        public virtual bool NotUseGlobalStack() => false;
        public virtual bool CheckAcitve() => true;
        public virtual bool CheckEnough(Item item) => true;
        public virtual void ModifyToolTips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new(Mod, "BuyPrice", Lang.tip[50].Value + GetPriceText(currency))
            { OverrideColor = currency.value > 0 ? Color.White : Color.Gray });
            if (flow && !OnlyCanBuy)
            {
                int i = 0;
                foreach (CdCheck cd in cdChecks)
                {
                    tooltips.Add(new(Mod, CD + i++, cd.condition.Description.Value) { OverrideColor = cd.Color });
                }
            }
        }
        private void CheckBuyItem(BaseUIElement uie)
        {
            if (!OnlyCanBuy && !CheckAcitve())
            {
                Main.NewText(GTV("Report.NoActive"));
                return;
            }
            bool noMet = false;
            foreach (CdCheck cd in cdChecks)
            {
                if (!cd.ignore && !cd.IsMet)
                {
                    cd.StartBlink();
                    noMet = true;
                }
            }
            if (noMet)
            {
                //Main.NewText(GTV("Report.NoMet"));
                return;
            }
            if (!SLUI.CanAfford)
            {
                //Main.NewText(GTV("Report.CantAfford"));
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
            int stack = NotUseGlobalStack() ? 1 : itemSlot.item.stack;
            if (mouse.type != type && mouse.type > ItemID.None)
            {
                ReSetBuy();
                return;
            }
            Player p = Main.LocalPlayer;
            if (Main.mouseLeft)
            {
                int b = Item.NewItem(p.GetSource_DropAsItem(), p.Hitbox, type, stack);
                Main.item[b].BuyFromSL(currency);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, b, 1f);
                p.BuyItem(currency.value, currency.currencyID);
                if (!CheckEnough(itemSlot.item))
                    Info.NeedRemove = true;
                ReSetBuy();
                return;
            }
            if (Main.mouseRight && SLUI.CanAfford)
            {
                if (buyTime == 30)
                {
                    if (mouse.type == ItemID.None)
                    {
                        mouse = new(type, stack);
                        mouse.BuyFromSL(currency);
                        p.BuyItem(currency.value, currency.currencyID);
                    }
                    else if (mouse.SLTime() == SLPlayer.SLTime)
                    {
                        if (mouse.stack + stack <= mouse.maxStack)
                        {
                            mouse.stack += stack;
                            p.BuyItem(currency.value, currency.currencyID);
                        }
                    }
                    else
                    {
                        Main.NewText(GTV("Report.NotSameStack"));
                        ReSetBuy();
                        return;
                    }
                    buyCD = buyTime = 20;
                    if (!CheckEnough(itemSlot.item))
                        Info.NeedRemove = true;
                }
                else if (buyCD <= 0)
                {
                    for (int i = 0; i < buyStack; i++)
                    {
                        if (mouse.stack + stack <= mouse.maxStack)
                        {
                            p.BuyItem(currency.value, currency.currencyID);
                            mouse.stack += stack;
                            if (!CheckEnough(itemSlot.item))
                            {
                                Info.NeedRemove = true;
                                return;
                            }
                        }
                        else
                        {
                            ReSetBuy();
                            return;
                        }
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
        private bool ReSetBuy()
        {
            buying = false;
            buyTime = 30;
            buyCD = 0;
            buyStack = 1;
            return false;
        }
    }
    public class UIShopItemForNPC(int npcType, bool flow, AbstractNPCShop.Entry entry) : UIShopItem(entry.Item, flow, entry.Conditions)
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
        public override void ModifyToolTips(Item item, List<TooltipLine> tooltips) { }
    }
    public class UIShopItemForEx(string exShopType, string modName, bool flow, AbstractNPCShop.Entry entry) : UIShopItem(entry.Item, flow, entry.Conditions)
    {
        public readonly string exShopType = exShopType;
        public readonly string modName = modName;
    }
    public class UIShopItemForSell : UIShopItem
    {
        public UIShopItemForSell(Item item) : base(item, true)
        {
            OnlyCanBuy = true;
        }
        public override bool NotUseGlobalStack() => true;
        public override bool CheckEnough(Item item) => --item.stack > 0;

        public override void ModifyToolTips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new(Mod, "RePurchase", GTV("Info.Repurchase") + GetPriceText(currency))
            { OverrideColor = currency.value > 0 ? Color.White : Color.Gray });
        }
    }
}
