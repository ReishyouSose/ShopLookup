using Terraria.Localization;

namespace ShopLookup.Content
{
    public class ShopItem : BaseUIElement
    {
        public UIItemSlot slot;
        public UICurrency currency;
        public int npcType;
        public bool canBuy;
        private int buyTime;
        private int buySpeed = 10;
        private int buyStack = 1;
        /// <summary>
        /// 生成条件是否满足
        /// </summary>
        public bool Spawn { get; private set; }
        private readonly IEnumerable<Condition> conditions;
        public bool Buying { get; private set; }
        public bool Permanent { get; private set; }
        public override Rectangle GetCanHitBox()
        {
            return Rectangle.Intersect(HitBox(), ParentElement.ParentElement.ParentElement.GetCanHitBox());
        }
        public ShopItem(Entry entry, int npcType, bool permanent, IEnumerable<Condition> conditions)
        {
            SetSize(80, 80);
            slot = new(entry.Item)
            {
                CanPutInSlot = new(Item => false),
                CanTakeOutSlot = new(Item => false),
            };
            slot.SetCenter(0, 26, 0.5f);
            this.npcType = npcType;
            Permanent = permanent;
            this.conditions = conditions;
            slot.Events.OnLeftClick += evt =>
            {
                if (!CanBuyItem()) return;
                Item item = entry.Item;
                Player p = Main.LocalPlayer;
                ref Item i = ref Main.mouseItem;
                if (i.type == ItemID.None)
                {
                    if (p.BuyItem(item.shopCustomPrice ?? item.value, item.shopSpecialCurrency))
                    {
                        i = new(item.type);
                    }
                    else Main.NewText(Language.GetTextValue(SLUI.LocalKey + "NoEnough"));
                }
            };
            slot.Events.OnRightDown += evt =>
            {
                CanBuyItem(true);
            };
            slot.Events.OnRightUp += evt =>
            {
                Buying = false;
            };
            slot.Events.OnMouseOut += evt =>
            {
                Buying = false;
            };
            Register(slot);

            currency = new(entry);
            currency.SetPos(0, 56);
            Register(currency);
        }
        private bool CanBuyItem(bool right = false)
        {
            if (!ShopLookup.Portable) return false;
            if (!canBuy)
            {
                Main.NewText(Language.GetTextValue(SLUI.LocalKey + "CantBuy"));
                return false;
            }
            if (NPC.FindFirstNPC(npcType) >= 0)
            {
                Buying = right;
                Main.playerInventory = true;
                return true;
            }
            else
            {
                if (!Permanent)
                {
                    if (conditions.All(x => x.IsMet()))
                    {
                        Buying = right;
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
                Item item = slot.ContainedItem;
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
                    if ((buySpeed < 1 || buyTime % buySpeed == 0) && i.stack < i.maxStack)
                    {
                        for (int j = 0; j < buyStack; j++)
                        {
                            if (p.BuyItem(item.shopCustomPrice ?? item.value, item.shopSpecialCurrency))
                            {
                                i.stack++;
                                if (i.stack == i.maxStack) return;
                            }
                            else
                            {
                                Buying = false;
                                Main.NewText(Language.GetTextValue(SLUI.LocalKey + "NoEnough"));
                                return;
                            }
                        }
                        if (buySpeed > 1)
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
