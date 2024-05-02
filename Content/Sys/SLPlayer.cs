using ShopLookup.Content.Data;
using Terraria.GameInput;

namespace ShopLookup.Content.Sys
{
    public class SLPlayer : ModPlayer
    {
        internal static ModKeybind Check;
        public override void Load()
        {
            Check = KeybindLoader.RegisterKeybind(Mod, "Look up", Microsoft.Xna.Framework.Input.Keys.L);
        }
        public override void OnEnterWorld()
        {
            ShopNPCData.Load(Mod);
            SLUI.OnInitialization();
            SLUI.Calculation();
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Check.JustPressed)
            {
                SLUI.OnInitialization();
                SLUI.Calculation();
                SLUI.Info.IsVisible = !SLUI.IsVisible;
                if (SLUI.IsVisible && !SLUI.ContainsPoint(Main.MouseScreen))
                {
                    SLUI.LookupItem(Main.HoverItem.type);
                }
            }
        }
    }
}
