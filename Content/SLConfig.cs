using System.ComponentModel;
using Terraria.ModLoader.Config;
using static Terraria.GameContent.Animations.Actions.NPCs;

namespace ShopLookup.Content
{
    internal class SLConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

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
