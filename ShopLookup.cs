using ShopLookup.Content;
using ShopLookup.UISupport;

namespace ShopLookup
{
    public class ShopLookup : Mod
    {
        internal static ShopLookup Ins;
        public UISystem uis;
        public ShopLookup()
        {
            Ins = this;
        }
        public override void Load()
        {
            On_WorldGen.SaveAndQuit += On_WorldGen_SaveAndQuit;
        }

        private void On_WorldGen_SaveAndQuit(On_WorldGen.orig_SaveAndQuit orig, Action callback)
        {
            orig(callback);
            foreach (ContainerElement ui in Ins.uis.Elements.Values)
            {
                ui.Info.IsVisible = false;
                if (ui is SLUI slui)
                {
                    slui.firstLoad = false;
                }
            }
        }
    }
}