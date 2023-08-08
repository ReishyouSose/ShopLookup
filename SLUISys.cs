using ReLogic.Content;
using ShopLookup.Content;
using Terraria.UI;
using static Terraria.UI.Gamepad.UILinkPointNavigator;

namespace ShopLookup
{
    public class SLUISys : ModSystem
    {
        public Point size;
        public static ModKeybind Keybind { get; private set; }
        internal static Asset<Texture2D> coins;
        internal static int[] coinCount ;
        internal static bool DrawCoins;
        public override void Load()
        {
            Keybind = KeybindLoader.RegisterKeybind(Mod, Mod.Name, "L");
            if (!Main.dedServ)
            {
                ExtraNPCInfo.Load();
                ShopLookup.Ins.uis = new();
                ShopLookup.Ins.uis.Load();
                Main.OnResolutionChanged += (evt) =>
                {
                    ShopLookup.Ins.uis.Calculation();
                    ShopLookup.Ins.uis.OnResolutionChange();
                };
                coins = ModContent.Request<Texture2D>("ShopLookup/UISupport/Asset/Coins");
                coinCount = new int[4];
            }
        }
        public override void UpdateUI(GameTime gt)
        {
            base.UpdateUI(gt);
            DrawCoins = false;
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
                    if (hoverNPC != -1)
                    {
                        if (TryGetNPCShop(Main.npc[hoverNPC].type, out var shop))
                        {
                            if (!visable)
                            {
                                visable = true;
                                ui.ChangeNPC(shop.NpcType);
                                return;
                            }
                            else
                            {
                                if (ui.focusNPC.npcType == shop.NpcType)
                                {
                                    visable = false;
                                    return;
                                }
                                else
                                {
                                    ui.ChangeNPC(shop.NpcType);
                                    return;
                                }
                            }
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
                       var sb = Main.spriteBatch;
                       ShopLookup.Ins.uis.Draw(sb);
                       if (DrawCoins)
                       {
                           Vector2 pos = Main.MouseScreen + Vector2.One * 16;
                           sb.Draw(coins.Value, pos, null, Color.White);
                           Vector2 center = pos + new Vector2(3 + 24 + 12 + 8, 22);
                           for (int i = 0; i < 4; i++)
                           {
                               var font = FontAssets.MouseText.Value;
                               string text = coinCount[i].ToString();
                               Vector2 origin = font.MeasureString(text);
                               origin = new(origin.X, origin.Y / 2f);
                               ChatManager.DrawColorCodedString(sb, font, text, center + i * Vector2.UnitX * 51,
                                   Color.White, 0, origin, Vector2.One * 0.8f);
                           }
                       }
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
