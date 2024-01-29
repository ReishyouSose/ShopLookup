using RUIModule.RUISys;
using ShopLookup.Content.UI;
using Terraria.GameInput;

namespace ShopLookup.Content.Sys
{
    public class SLPlayer : ModPlayer
    {
        internal static ModKeybind Check;
        private static readonly string SLUIKey = typeof(SLUI).FullName;
        private static SLUI SLUI => RUIManager.UIEs[SLUIKey] as SLUI;
        public override void Load()
        {
            Check = KeybindLoader.RegisterKeybind(Mod, "Look up", Microsoft.Xna.Framework.Input.Keys.L);
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Check.JustPressed)
                SLUI.Info.IsVisible = !SLUI.Info.IsVisible;
        }
    }
}
