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
        private int buySpeed = 10;
        private int buyStack = 1;
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
        public override Rectangle GetCanHitBox()
        {
            return Rectangle.Intersect(HitBox(), ParentElement.ParentElement.ParentElement.GetCanHitBox());
        }
        public UIShopSlot(Entry entry, int npcType, bool npc, bool permanent, IEnumerable<Condition> conditions)
        {
            SetSize(80, 80);
            itemSlot = new(entry.Item)
            {
                CanPutInSlot = new(Item => false),
                CanTakeOutSlot = new(Item => false),
            };
            itemSlot.SetCenter(0, 26, 0.5f);
            itemSlot.Events.OnLeftDown += evt => CanBuyItem();
            itemSlot.Events.OnRightDown += evt => CanBuyItem();
            itemSlot.Events.OnLeftUp += evt => Buying = false;
            itemSlot.Events.OnRightUp += evt => Buying = false;
            itemSlot.Events.OnMouseOut += evt => Buying = false;
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
            if (!canBuy)
            {
                Main.NewText(Language.GetTextValue(SLUI.LocalKey + "CantBuy"));
                return false;
            }
            if (npcType == -1 || NPC.FindFirstNPC(npcType) >= 0)
            {
                Buying = true;
                Main.playerInventory = true;
                return true;
            }
            else
            {
                if (!Permanent)
                {
                    if (Spawn)
                    {
                        Buying = true;
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
                if (i.type == ItemID.None)
                {
                    if (p.BuyItem(item.shopCustomPrice ?? item.value, item.shopSpecialCurrency))
                    {
                        i = new(item.type);
                    }
                    else
                    {
                        Buying = false;
                        Main.NewText(Language.GetTextValue(SLUI.LocalKey + "NoEnough"));
                    }
                }
                else if (i.type == item.type)
                {
                    if ((buySpeed == 0 || (buyTime % buySpeed == 0 && buySpeed < 10)) && i.stack < i.maxStack)
                    {
                        for (int j = 0; j < buyStack; j++)
                        {
                            if (p.BuyItem(item.shopCustomPrice ?? item.value, item.shopSpecialCurrency))
                            {
                                i.stack++;
                                SoundCoins();
                                if (i.stack == i.maxStack) return;
                            }
                            else
                            {
                                Buying = false;
                                Main.NewText(Language.GetTextValue(SLUI.LocalKey + "NoEnough"));
                                return;
                            }
                        }
                    }
                    if (buySpeed > 10)
                    {
                        if (buyTime >= 20)
                        {
                            buySpeed--;
                        }
                    }
                    else if (buySpeed > 0)
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
                    buyTime++;
                }
            }
            else
            {
                buyTime = 0;
                buySpeed = 10;
                buyStack = 1;
            }
        }
    }
}
