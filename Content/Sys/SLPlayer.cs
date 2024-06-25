using Terraria.GameInput;

namespace ShopLookup.Content.Sys
{
    public class SLPlayer : ModPlayer
    {
        internal static ModKeybind Check;
        internal static uint SLTime = 1;
        public override void Load()
        {
            Check = KeybindLoader.RegisterKeybind(Mod, "Look up", Microsoft.Xna.Framework.Input.Keys.L);
        }
        public override void OnEnterWorld() => SLUI.OnInitialization();
        public override bool HoverSlot(Item[] inventory, int context, int slot) => SLUI.HoverCheck(context);
        public override bool ShiftClickSlot(Item[] inventory, int context, int slot) => SLUI.SellCheck(inventory, context, slot);
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Check.JustPressed)
            {
                if (Main.HoverItem.type > ItemID.None)
                {
                    SLUI.Info.IsVisible = true;
                    SLUI.LookupItem(Main.HoverItem.type);
                }
                else
                {
                    SLUI.Info.IsVisible = !SLUI.IsVisible;
                    if (!SLUI.IsVisible)
                        SLUI.ClearSell();
                    SLTime = Main.GameUpdateCount;
                }
            }
        }
    }
}
