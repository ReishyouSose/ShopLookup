using ReLogic.Graphics;

namespace ShopLookup.UISupport.UIElements
{
    public class UIFuncTextSnippet : BaseUIElement
    {
        /// <summary>
        /// 0是中心绘制，1是左上角起始，2是左中
        /// </summary>
        public int Style { get; set; }
        public Color Color;
        public Vector2 scale;
        private readonly Func<TextSnippet[]> text;
        public Vector2 size;
        public TextSnippet[] Text => text();

        public UIFuncTextSnippet(Func<TextSnippet[]> t, Vector2? size = null, Vector2 scale = default, float maxWidth = -1)
        {
            text = t;
            Color = Color.White;
            this.scale = scale == default ? Vector2.One : scale;
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            this.size = size ?? ChatManager.GetStringSize(font, text.Invoke(), this.scale, maxWidth);
            SetSize(this.size * scale);
        }

        public override void DrawSelf(SpriteBatch sb)
        {
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            Vector2 offY = Vector2.UnitY * TextYoffset * scale.Y;
            if (Style == 0)
            {
                ChatManager.DrawColorCodedString(sb, font, text.Invoke(), Center() + offY,
                    Color, 0, size / 2f, scale, out _, size.X * scale.X);
            }
            else if (Style == 1)
            {
                ChatManager.DrawColorCodedString(sb, font, text.Invoke(), Pos() + offY,
                    Color, 0, Vector2.Zero, scale, out _, size.X * scale.X);
            }
            else if (Style == 2)
            {
                ChatManager.DrawColorCodedString(sb, font, text.Invoke(), Pos() + offY,
                    Color, 0, new Vector2(0, size.Y / 2f), scale, out _, size.X * scale.X);
                //DrawStr(sb, font, t, Pos() + offY + Vector2.UnitY * Height / 2, new Vector2(0, size.Y / 2f), scale);
            }
        }
    }
}
