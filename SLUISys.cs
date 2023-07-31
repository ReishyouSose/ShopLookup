using ShopLookup.Content;
using Terraria.UI;

namespace ShopLookup
{
    public class SLUISys : ModSystem
    {
        public Point size;
        public static ModKeybind Keybind { get; private set; }
        public override void Load()
        {
            Keybind = KeybindLoader.RegisterKeybind(Mod, Mod.Name, "L");
            if (!Main.dedServ)
            {
                ShopLookup.Ins.uis = new();
                ShopLookup.Ins.uis.Load();
                Main.OnResolutionChanged += (evt) =>
                {
                    ShopLookup.Ins.uis.Calculation();
                    ShopLookup.Ins.uis.OnResolutionChange();
                };
            }

        }
        public override void UpdateUI(GameTime gt)
        {
            base.UpdateUI(gt);
            if (size != Main.ScreenSize)
            {
                size = Main.ScreenSize;
                ShopLookup.Ins.uis.Calculation();
            }
            ShopLookup.Ins.uis.Update(gt);
            if (Keybind.JustPressed)
            {
                SLUI ui = ShopLookup.Ins.uis.Elements[SLUI.NmakeKey] as SLUI;
                ref var visable = ref ui.Info.IsVisible;
                if (Main.HoverItem.type == ItemID.None)
                {
                    visable = !visable;
                }
                else
                {
                    if (!visable) visable = true;
                    ui.ChangeItem(Main.HoverItem.type);
                }
                ui.LoadShopNPC();
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (MouseTextIndex != -1)
            {
                layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer(
                   "ShopLookup : SLUISys",
                   delegate
                   {
                       ShopLookup.Ins.uis.Draw(Main.spriteBatch);
                       return true;
                   },
                   InterfaceScaleType.UI)
               );
            }
        }
    }
}
