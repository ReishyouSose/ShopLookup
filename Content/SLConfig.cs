using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ShopLookup.Content
{
    internal class SLConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        public bool Portable;

        [DefaultValue(true)]
        public bool PermanentTips;
        public override void OnChanged()
        {
            ShopLookup.Portable = Portable;
            ShopLookup.PermanentTips = PermanentTips;
        }
    }
}
