namespace ShopLookup.UISupport.UIElements
{
    public class UIItem : BaseUIElement
    {
        public int itemid;
        public int stack;
        public Vector2 scale = Vector2.One;
        /// <summary>
        /// 是否忽视堆叠显示限制
        /// </summary>
        public bool Ignore;
        public UIItem(int itemid = -1, int stack = 1)
        {
            this.itemid = itemid;
            this.stack = stack;
            SetSize(32, 32);
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            //调用原版物品介绍
            if (itemid >= 0 && ContainsPoint(Main.MouseScreen))
            {
                Item item = new(itemid, stack);
                Main.hoverItemName = item.Name;
                Main.HoverItem = item;
            }
            if (itemid >= 0)
            {
                Main.instance.LoadItem(itemid);
                Rectangle frame = Main.itemAnimations[itemid] != null ? Main.itemAnimations[itemid].GetFrame(TextureAssets.Item[itemid].Value) : Item.GetDrawHitbox(itemid, null);
                //绘制物品贴图
                var center = Center();
                Draw(sb, TextureAssets.Item[itemid].Value, center, frame, frame.Size() / 2f, scale);
                /* sb.Draw(TextureAssets.Item[itemid].Value, new Vector2(HitBox().X + HitBox().Width / 2,
                     HitBox().Y + HitBox().Height / 2) - (new Vector2(frame.Width, frame.Height) / 2f * scale),
                     new Rectangle?(frame), Color.White * opacity, 0f, Vector2.Zero, scale, 0, 0);*/

                //绘制物品左下角那个代表数量的数字
                if (stack > 1 || Ignore)
                {
                    var font = FontAssets.MouseText.Value;
                    string count = stack.ToString();
                    Vector2 origin = new(font.MeasureString(count).X / 2f, 0);

                    ChatManager.DrawColorCodedStringWithShadow(sb, font, count, center + Vector2.UnitY * 8,
                        Color.Black, 0f, origin, scale * 0.75f);
                    ChatManager.DrawColorCodedString(sb, font, count, center + Vector2.UnitY * 8,
                        Color.White, 0f, origin, scale * 0.75f);
                }
            }
        }
    }
}
