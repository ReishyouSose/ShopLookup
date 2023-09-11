using ReLogic.Content;
using ShopLookup.Content;
using Terraria.UI;

namespace ShopLookup
{
    public class SLUISys : ModSystem
    {
        public Point size;
        public static ModKeybind Keybind { get; private set; }
        internal static Asset<Texture2D> coins;
        internal static int[] coinCount;
        internal static bool DrawCoins;
        internal static RenderTarget2D render;
        public override void Load()
        {
            Main.QueueMainThreadAction(() => render ??= new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight));
            Main.OnResolutionChanged += x => render = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
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
                           var font = FontAssets.MouseText.Value;
                           float y = font.LineSpacing;
                           for (int i = 0; i < 4; i++)
                           {
                               string text = coinCount[i].ToString();
                               Vector2 origin = new(font.MeasureString(text).X, y / 2f);
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
