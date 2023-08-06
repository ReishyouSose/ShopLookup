namespace ShopLookup.UISupport.UIElements
{
    public class UICurrency : BaseUIElement
    {
        public Vector2 scale;
        public bool special;
        public int type;
        public int value;
        public int[] coins;
        public Item currency;
        private readonly Rectangle frame;
        private readonly Texture2D tex;
        public UICurrency(Entry entry)
        {
            scale = Vector2.One;
            SetSize(80, 24);
            Item item = entry.Item;
            if (CurrencyType(item.shopSpecialCurrency, out type))
            {
                special = true;
            }
            if (special)
            {
                Main.instance.LoadItem(type);
                value = item.shopCustomPrice.Value;
                currency = new(type, value);
                var animation = Main.itemAnimations[type];
                tex = currency.Tex();
                frame = animation != null ? animation.GetFrame(tex) : Item.GetDrawHitbox(type, null);
            }
            else
            {
                coins = new int[4];
                int i = 0;
                foreach ((int coin, int count) in ToCoins(item.shopCustomPrice ?? item.value))
                {
                    coins[i] = count;
                    i++;
                }
            }
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            var rec = HitBox();
            Vector2 center = rec.Center();
            var font = FontAssets.MouseText.Value;
            if (Info.IsMouseHover)
            {
                if (special)
                {
                    Main.HoverItem = currency;
                    Main.hoverItemName = currency.Name;
                }
                else if (coins != null)
                {
                    SLUISys.coinCount = coins;
                    SLUISys.DrawCoins = true;
                }
            }
            if (special)
            {
                //绘制物品贴图
                Vector2 offset = Vector2.UnitX * rec.Width / 4f;
                SimpleDraw(sb, tex, center - offset, frame, frame.Size() / 2f, scale * frame.AutoScale(30, 1f));
                string text = value.ToString();
                Vector2 origin = font.MeasureString(text) / 2f;
                offset += Vector2.UnitY * 4 * scale.Y;
                ChatManager.DrawColorCodedStringShadow(sb, font, text, center + offset, Color.Black, 0, origin, scale * 0.9f);
                ChatManager.DrawColorCodedString(sb, font, text, center + offset, Color.White, 0, origin, scale * 0.9f);
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    string text = coins[i].ToString();
                    Vector2 origin = font.MeasureString(text) / 2f;
                    Vector2 pos = new(i switch
                    {
                        0 => 0.125f,
                        1 => 0.375f,
                        2 => 0.625f,
                        3 => 0.875f,
                        _ => 0
                    } * rec.Width + rec.Left, center.Y + 4 * scale.Y);
                    ChatManager.DrawColorCodedStringWithShadow(sb, font, text, pos, i switch
                    {
                        1 => Color.Gold,
                        2 => Color.Silver,
                        3 => Color.Orange,
                        _ => Color.White
                    }, 0, origin, scale * 0.75f, -1, 1.5f);
                }
            }
        }
    }
}
