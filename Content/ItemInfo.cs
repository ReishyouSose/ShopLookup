using ReLogic.Graphics;
using Terraria.Localization;

namespace ShopLookup.Content
{
    public class ItemInfo : BaseUIElement
    {
        private readonly static Condition None = new("", () => true);
        public List<string> infos;
        public List<float> yOff;
        public Condition[] cds;
        public DynamicSpriteFont font;
        public string text;
        public bool noCondition;
        private static readonly Color G = new(0, 230, 100, 255);
        private static readonly Color R = new(255, 50, 100, 255);
        private static readonly Color Y = new(255, 165, 0, 255);
        public ItemInfo(string info, IEnumerable<Condition> cds, float width, DynamicSpriteFont font = null)
        {
            this.font = font ?? FontAssets.MouseText.Value;
            font = this.font;
            yOff = new() { CriterionOffset(font, info) };
            infos = new() { info };
            this.cds = cds.ToArray();
            if (cds.Any())
            {
                foreach (Condition c in cds)
                {
                    if (c.Description.Key == "" || c.Description.Value == "")
                    {
                        string no = Language.GetTextValue("Mods.ShopLookup.UnknowCds");
                        infos.Add(no);
                        yOff.Add(font.MeasureString(no).Y);
                        break;
                    }
                    string cdesc = font.CreateWrappedText(c.Description.Value, width);
                    infos.Add(cdesc);
                    yOff.Add(CriterionOffset(font, cdesc));
                }
            }
            else
            {
                string no = Language.GetTextValue("Mods.ShopLookup.NoCondition");
                infos.Add(no);
                yOff.Add(font.MeasureString(no).Y);
                noCondition = true;
            }
            string text = "";
            int count = infos.Count;
            for (int i = 0; i < count; i++)
            {
                text += infos[i];
                if (i < count - 1)
                {
                    text += "\n";
                }
            }
            this.text = text;
            SetSize(font.MeasureString(text));
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            Rectangle hitbox = HitBox();
            Vector2 drawPos = hitbox.TopLeft();
            if (ShopLookup.Portable || ShopLookup.PermanentTips)
            {
                var parentChild = ParentElement.ChildrenElements;
                if (Info.IsMouseHover || ParentElement.Info.IsMouseHover)
                {
                    int count = infos.Count;
                    bool sell = true;
                    UIShopSlot shopItem = parentChild.First(x => x is UIShopSlot) as UIShopSlot;
                    for (int i = 0; i < count; i++)
                    {
                        string text = infos[i];
                        Color c = Color.White;
                        if (i > 0)
                        {
                            if (shopItem.Buying)
                            {
                                c = Color.LightGreen;
                                sell = true;
                            }
                            else
                            {
                                if (noCondition)
                                {
                                    c = Color.LightGreen;
                                    sell = true;
                                }
                                else
                                {
                                    Condition cd = cds[i - 1];
                                    if (IgnoreCds(cd))
                                    {
                                        c = Y;
                                    }
                                    else if (cd.IsMet())
                                    {
                                        c = Color.LightGreen;
                                    }
                                    else
                                    {
                                        c = R;
                                        sell = false;
                                    }
                                }
                            }
                        }
                        ChatManager.DrawColorCodedStringWithShadow(sb, font, text, drawPos, c,
                            0, Vector2.Zero, Vector2.One, -1, 1.5f);
                        drawPos.Y += yOff[i] + font.LineSpacing;
                    }
                    ((UIImage)parentChild.First(x => x is UIImage)).color = sell ? G : R;
                    shopItem.canBuy = sell;
                }
                else
                {
                    ChatManager.DrawColorCodedStringWithShadow(sb, font, text, drawPos, Color.White,
                        0, Vector2.Zero, Vector2.One, -1, 1.5f);
                    ((UIImage)parentChild.First(x => x is UIImage)).color = Color.White;
                }
            }
            else
            {
                ChatManager.DrawColorCodedStringWithShadow(sb, font, text, drawPos, Color.White,
                    0, Vector2.Zero, Vector2.One, -1, 1.5f);
            }
        }
        private static int CriterionOffset(DynamicSpriteFont font, string text)
        {
            int space = font.LineSpacing;
            float y = font.MeasureString(text).Y;
            int line = (int)(y / space);
            float offset = y % space;
            if (y - offset <= 5)
            {
                line++;
            }
            return Math.Max(line * space - space, 0);
        }
        private static bool IgnoreCds(Condition c)
        {
            return (ShopLookup.IgnoreUnknowCds && (c.Description.Key == "" || c.Description.Value == ""))
                || c == Condition.HappyEnoughToSellPylons
                || c == Condition.AnotherTownNPCNearby
                || c == Condition.HappyEnough;
        }
    }
}
