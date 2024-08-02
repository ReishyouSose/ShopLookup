using static ShopLookup.Content.Data.ShopNPCData;

namespace ShopLookup.Content.UI.ExtraUI
{
    internal class UIModSlot : UIIconSlot
    {
        public readonly string modName;
        public UIModSlot(string modName, ModInfo modInfo) : base(modInfo.icon, 6)
        {
            this.modName = modName;
            hoverText = modInfo.mod.DisplayName ?? modName;
            SetSize(52, 52);
        }
    }
}