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
            ShopNPCData.Load();
            SLUI.ReLoadNPCView();
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Check.JustPressed)
            {
                SLUI.OnInitialization();
                SLUI.Info.IsVisible = true;
                SLUI.Calculation();
                //SLUI.Info.IsVisible = !SLUI.Info.IsVisible;
            }
        }
    }
}
