using ReLogic.Graphics;
using Terraria.Localization;

namespace ShopLookup.Content
{
    public class ItemInfo : BaseUIElement
    {
        public List<string> infos;
        public List<float> yOff;
        public Condition[] cds;
        public DynamicSpriteFont font;
        public string text;
        public bool noCondition;
        private bool MouseHover;
        private static readonly Color G = new(0, 230, 100, 255);
        private static readonly Color R = new(255, 75, 100, 255);
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
                    string cdesc = c.Description.Value;
                    cdesc = font.CreateWrappedText(cdesc, width);
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
            MouseHover = ParentElement.Info.IsMouseHover;
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            Rectangle hitbox = HitBox();
            Vector2 drawPos = hitbox.TopLeft();
            if (MouseHover)
            {
                int count = infos.Count;
                bool sell = true;
                for (int i = 0; i < count; i++)
                {
                    string text = infos[i];
                    Color c = Color.White;
                    if (i > 0)
                    {
                        if (noCondition)
                        {
                            c = Color.LightGreen;
                            sell = true;
                        }
                        else
                        {
                            if (cds[i - 1].IsMet()) c = Color.LightGreen;
                            else
                            {
                                c = R;
                                if (sell) sell = false;
                            }
                        }
                    }
                    ChatManager.DrawColorCodedStringWithShadow(sb, font, text, drawPos, c,
                        0, Vector2.Zero, Vector2.One, -1, 1.5f);
                    drawPos.Y += yOff[i] + font.LineSpacing;
                }
                ((UIImage)ParentElement.ChildrenElements.First(x => x is UIImage))
                    .color = sell ? G : R;
            }
            else
            {
                ChatManager.DrawColorCodedStringWithShadow(sb, font, text, drawPos, Color.White, 0, Vector2.Zero, Vector2.One, -1, 1.5f);
                ((UIImage)ParentElement.ChildrenElements.First(x => x is UIImage))
                    .color = Color.White;
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
        private static string Decription(string info, IEnumerable<Condition> cds, float width, out float height)
        {
            string conditions = "";
            var font = FontAssets.MouseText.Value;
            height = 0;
            int count = cds.Count();
            if (count > 0)
            {
                int i = 1;
                foreach (Condition c in cds)
                {
                    string cdesc = c.Description.Value;
                    conditions += cdesc;
                    /*if (c.Description.Key == cdesc)
                    {

                    }*/
                    if (i == 1 && count > 1)
                    {
                        height = font.MeasureString(c.Description.Value).Y;
                    }
                    if (i < count) conditions += "\n";
                    i++;
                }
                float oldH = font.MeasureString(conditions).Y;
                conditions = font.CreateWrappedText(conditions, width);
                float newH = font.MeasureString(conditions).Y;
                if (height > 0)
                {
                    height = newH - height;
                }
                else
                {
                    if (newH > oldH) height = newH - oldH;
                }

                return info + "\n" + conditions;
            }
            //else return info + "\n" + Language.GetTextValue(LocalKey + "NoCondition");
            return info + "\n" + conditions;
        }
    }
}
