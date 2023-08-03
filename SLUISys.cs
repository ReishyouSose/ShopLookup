using ShopLookup.Content;
using Terraria.UI;
using static Terraria.UI.Gamepad.UILinkPointNavigator;

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
                if (!ui.firstLoad)
                {
                    ui.RemoveAll();
                    ui.OnInitialization();
                    ui.LoadShopNPC();
                    ui.firstLoad = true;
                }
                if (Main.HoverItem.type == ItemID.None)
                {
                    int hoverNPC = Main.LocalPlayer.talkNPC;
                    if (Main.LocalPlayer.talkNPC != -1)
                    {
                        if (TryGetNPCShop(Main.npc[hoverNPC].type, out var shop))
                        {
                            if (!visable) visable = true;
                            ui.ChangeNPC(shop.NpcType);
                            return;
                        }
                    }
                    hoverNPC = Shortcuts.NPCS_LastHovered;
                    if (hoverNPC < -10)
                    {
                        if (TryGetNPCShop(-(hoverNPC + 10), out var shop))
                        {
                            if (!visable) visable = true;
                            ui.ChangeNPC(shop.NpcType);
                            return;
                        }
                    }
                    if (hoverNPC >= 0)
                    {
                        if (TryGetNPCShop(Main.npc[hoverNPC].type, out var shop))
                        {
                            if (!visable) visable = true;
                            ui.ChangeNPC(shop.NpcType);
                            return;
                        }
                    }
                    visable = !visable;
                }
                else
                {
                    if (!visable) visable = true;
                    ui.ChangeItem(Main.HoverItem.type);
                }
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
        public override void PreSaveAndQuit()
        {
            SLUI ui = ShopLookup.Ins.uis.Elements[SLUI.NmakeKey] as SLUI;
            ui.Info.IsVisible = false;
            ui.firstLoad = false;
        }
    }
}
