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
        public bool IgnoreUnknownCds;
        public override void OnLoaded() => Ins = this;
    }
}
