using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ShopLookup.Content
{
    internal class SLConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(false)]
        public bool Portable;

        [DefaultValue(false)]
        public bool PermanentTips;
        public override void OnChanged()
        {
            ShopLookup.Portable = Portable;
            ShopLookup.PermanentTips = PermanentTips;
        }
    }
}
