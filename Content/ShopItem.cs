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
        private bool Buying;
        public ShopItem(Entry entry, int npcType)
        {
            SetSize(80, 80);
            //DrawRec[0] = true;
            slot = new(entry.Item)
            {
                CanPutInSlot = new(Item => false),
                CanTakeOutSlot = new(Item => false),
            };
            slot.SetCenter(0, 26, 0.5f);
            slot.Events.OnLeftClick += evt =>
            {
                if (!ShopLookup.Portable) return;
                if (!canBuy)
                {
                    Main.NewText(Language.GetTextValue(SLUI.LocalKey + "CantBuy"));
                    return;
                }
                if (NPC.FindFirstNPC(npcType) == -1)
                {
                    Main.NewText(Language.GetTextValue(SLUI.LocalKey + "NoActive"));
                    return;
                }
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
                if (!ShopLookup.Portable) return;
                if (!canBuy)
                {
                    Main.NewText(Language.GetTextValue(SLUI.LocalKey + "CantBuy"));
                    return;
                }
                if (NPC.FindFirstNPC(npcType) >= 0)
                {
                    Buying = true;
                }
                else Main.NewText(Language.GetTextValue(SLUI.LocalKey + "NoActive"));
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
                else
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
