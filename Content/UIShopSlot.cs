using Terraria.Localization;

namespace ShopLookup.Content
{
    public class UIShopSlot : BaseUIElement
    {
        public UIItemSlot itemSlot;
        public UINPCSlot npcSlot;
        public UICurrency currency;
        public int npcType;
        public bool canBuy;
        private int buyTime;
        private int buySpeed;
        private int buyStack;
        private int firstCD;
        /// <summary>
        /// 生成条件是否满足
        /// </summary>
        public bool Spawn => conditions.All(x => x.IsMet());
        /// <summary>
        /// 非常驻NPC的生成条件
        /// </summary>
        private readonly IEnumerable<Condition> conditions;
        public bool Buying { get; private set; }
        public bool Permanent { get; private set; }
        public UIShopSlot(Entry entry, int npcType, bool npc, bool permanent, IEnumerable<Condition> conditions)
        {
            SetSize(80, 80);
            itemSlot = new(entry.Item)
            {
                CanPutInSlot = new(Item => false),
                CanTakeOutSlot = new(Item => false),
            };
            itemSlot.ContainedItem.isAShopItem = true;
            itemSlot.SetCenter(0, 26, 0.5f);
            itemSlot.Events.OnLeftDown += evt => CanBuyItem();
            itemSlot.Events.OnRightDown += evt => CanBuyItem();
            itemSlot.Events.OnLeftUp += evt => Reset(false);
            itemSlot.Events.OnRightUp += evt => Reset(false);
            itemSlot.Events.OnMouseOut += evt => Reset(false);
            Register(itemSlot);
            if (npc)
            {
                itemSlot.Info.IsHidden = true;
                itemSlot.Info.IsSensitive = true;
                itemSlot.hoverDisplay = false;
                npcSlot = new(npcType);
                npcSlot.SetCenter(0, 26, 0.5f);
                Register(npcSlot);
            }

            this.npcType = npcType;
            Permanent = permanent;
            this.conditions = conditions;

            currency = new(entry);
            currency.SetPos(0, 52);
            Register(currency);
        }
        private bool CanBuyItem()
        {
            if (!ShopLookup.Portable) return false;
            int type = Main.mouseItem.type;
            if (type > 0 && type != itemSlot.ContainedItem.type) return false;
            if (!canBuy)
            {
                Main.NewText(Language.GetTextValue(SLUI.LocalKey + "CantBuy"));
                return false;
            }
            if (npcType == -1 || VisitedNPCSys.Contains(npcType) || NPC.FindFirstNPC(npcType) >= 0)
            {
                Reset(true);
                Main.playerInventory = true;
                return true;
            }
            else
            {
                if (!Permanent)
                {
                    if (Spawn)
                    {
                        Reset(true);
                        Main.playerInventory = true;
                        return true;
                    }
                    else
                    {
                        Main.NewText(Language.GetTextValue(SLUI.LocalKey + "NoSpawn"));
                        return false;
                    }
                }
                else
                {
                    Main.NewText(Language.GetTextValue(SLUI.LocalKey + "NoActive"));
                    return false;
                }
            }
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (Buying)
            {
                Item item = itemSlot.ContainedItem;
                Player p = Main.LocalPlayer;
                ref Item i = ref Main.mouseItem;
                if ((buySpeed == 0 || buyTime % buySpeed == 0) && i.stack < item.maxStack && firstCD <= 0)
                {
                    if (firstCD == -1) firstCD = 30;
                    SoundCoins();
                    for (int j = 0; j < buyStack; j++)
                    {
                        if (p.BuyItem(item.shopCustomPrice ?? item.value, item.shopSpecialCurrency))
                        {
                            if (i.type == ItemID.None) i = new(item.type);
                            else i.stack++;

                            if (i.stack == i.maxStack)
                            {
                                Reset(false);
                                return;
                            }
                        }
                        else
                        {
                            Reset(false);
                            Main.NewText(Language.GetTextValue(SLUI.LocalKey + "NoEnough"));
                            return;
                        }
                    }
                }
                if (buySpeed > 0)
                {
                    if (buyTime >= buySpeed * 2)
                    {
                        buyTime = 0;
                        buySpeed--;
                    }
                }
                else
                {
                    if (buyTime % 10 == 0)
                    {
                        buyStack++;
                        buyTime = 0;
                    }
                }
                if (firstCD > 0) firstCD--;
                buyTime++;
            }
        }
        private void Reset(bool buying)
        {
            Buying = buying;
            buyTime = 0;
            buySpeed = 10;
            buyStack = 1;
            firstCD = -1;
        }
    }
}
