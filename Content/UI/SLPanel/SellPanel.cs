using ShopLookup.Content.UI.ExtraUI;
using static Terraria.UI.ItemSlot;

namespace ShopLookup.Content.UI.SLPanel
{
    public partial class SLPanel
    {
        private UIBottom sellPanel;
        private UIContainerPanel sellView;
        public bool InSellPanel => sellPanel?.IsVisible == true;
        private void RegisterSellPanel(UIBottom bg)
        {
            UIVnlPanel tipBg = new(0, 0);
            tipBg.SetSize(0, 30, 1);
            bg.Register(tipBg);
            savingsHide.Add(tipBg);

            UIText tip = new(GTV("Info.SellTip"));
            tip.SetPos(10, 5);
            tip.SetSize(tip.TextSize);
            tipBg.Register(tip);

            UIVnlPanel sellBg = new(0, 0);
            sellBg.SetPos(0, 40);
            sellBg.SetSize(0, -40, 1, 1);
            sellBg.Info.SetMargin(10);
            bg.Register(sellBg);

            sellView = new();
            sellView.SetSize(-30, 0, 1, 1);
            sellView.autoPos = [10, 10];
            sellView.Events.OnLeftDown += evt =>
            {
                if (Main.mouseItem.type > ItemID.None)
                    SellItem(ref Main.mouseItem);
            };
            sellBg.Register(sellView);

            VerticalScrollbar vs = new(62, true, false);
            vs.Info.Left.Pixel += 10;
            sellView.SetVerticalScrollbar(vs);
            sellBg.Register(vs);
        }
        private void SellItem(ref Item item)
        {
            if (item.type == ItemID.None)
                return;
            item.shopCustomPrice = item.value / 5;
            UIShopItemForSell slot = new(item);
            slot.itemSlot.item.SLTime() = 0;
            sellView.AddElement(slot);
            foreach (var (coin, stack) in ToCoins((item.CanRefund(out int value, out int currency) ? value : item.value / 5) * item.stack, currency))
                Item.NewItem(item.GetSource_DropAsItem(), Main.LocalPlayer.Hitbox, coin, stack);
            item.SetDefaults(0);
            sellView.Calculation();
        }
        public void ClearSell() => sellView.ClearAllElements();
        private bool Active(int context) => IsVisible && context is Context.InventoryItem or Context.ChestItem && sellPanel.IsVisible;
        public bool HoverCheck(int context)
        {
            if (Main.keyState.PressingShift() && Active(context))
            {
                Main.cursorOverride = CursorOverrideID.QuickSell;
                return true;
            }
            return false;
        }
        public bool SellCheck(Item[] inv, int context, int slot)
        {
            if (Active(context))
            {
                SellItem(ref inv[slot]);
                return true;
            }
            return false;
        }
        public void ModifyToolTip(Item item, List<TooltipLine> tooltips)
        {
            if (item.CanRefund(out int value, out int currency))
                tooltips.Add(new(ShopLookup.Ins, "ReFund", GTV("Info.Refund") + GetPriceText(value * item.stack, currency))
                { OverrideColor = value > 0 ? Y : Color.Gray });
            else if (IsVisible && sellPanel.IsVisible && UIShopItem.HoverSlot == null)
            {
                tooltips.Add(new(ShopLookup.Ins, "SellPrice", Lang.tip[49].Value + GetPriceText(item.value / 5 * item.stack))
                { OverrideColor = item.value > 0 ? Color.White : Color.Gray });
            }
        }
    }
}
