using ShopLookup.Content.UI.ExtraUI;

namespace ShopLookup.Content.Sys
{
    public class SLItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public uint SLTime;
        public int SLCurrency = -1;
        public int SLValue;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            UIShopItem.HoverSlot?.ModifyToolTips(item, tooltips);
            SLUI.ModifyToolTip(item, tooltips);
        }
        public override bool CanStack(Item destination, Item source) => StackCheck(destination, source);
        public override bool CanStackInWorld(Item destination, Item source) => StackCheck(destination, source);
        private static bool StackCheck(Item destination, Item source)
        {
            bool d = destination.CanRefund(out int dv, out _);
            bool s = source.CanRefund(out int sv, out _);
            return (!d && !s) || d == s && dv == sv;
        }
    }
}
