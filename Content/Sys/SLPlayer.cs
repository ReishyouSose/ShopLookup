using ShopLookup.Content.Data;
using Terraria.GameInput;

namespace ShopLookup.Content.Sys
{
    public class SLPlayer : ModPlayer
    {
        internal static ModKeybind Check;
        internal static bool loaded;
        public override void Load()
        {
            Check = KeybindLoader.RegisterKeybind(Mod, "Look up", Microsoft.Xna.Framework.Input.Keys.L);
        }
        public override void OnEnterWorld()
        {
            ShopNPCData.Load(Mod);
            //SLUI.ReLoadNPCView();
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Check.JustPressed)
            {
                /*if (loaded)
                {
                    SLUI.Info.IsVisible = !SLUI.IsVisible;
                    if (SLUI.IsVisible)
                    {
                        SLUI.LookupItem(Main.HoverItem);
                    }
                }
                else
                {
                    SLUI.OnInitialization();
                    SLUI.Info.IsVisible = true;
                    SLUI.Calculation();
                    loaded = true;
                }*/
                SLUI.OnInitialization();
                SLUI.Info.IsVisible = true;
                SLUI.Calculation();
                SLUI.LookupItem(Main.HoverItem);
            }
        }
    }
}
