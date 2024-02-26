using ShopLookup.Content.UI.ExtraUI;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ShopLookup.Content.Sys
{
    public class SLConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        internal static SLConfig Ins;
        public SLConfig() => Ins = this;

        [DefaultValue(false)]
        public bool Portable;

        [DefaultValue(true)]
        public bool PermanentTips;

        [DefaultValue(false)]
        public bool IgnoreUnknowCds;
        public override void OnChanged()
        {
            UIShopSlot.IgnoreUnknowCds = IgnoreUnknowCds;
            UIShopSlot.PermanentTips = PermanentTips;
            UIShopSlot.Portable = Portable;
        }
    }
}
