
namespace ShopLookup.Content.UI.ExtraUI
{
    public class UIModSlot(Texture2D tex) : UIImage(tex, new(52))
    {
        public override void DrawSelf(SpriteBatch sb)
        {
            Rectangle rect = HitBox();
            sb.Draw(AssetLoader.Slot, rect, color);
            if(Tex != null )
            {
                sb.Draw(Tex, rect.Center(), null, Color.White, 0, Tex.Size() / 2f, 1f, 0, 0);
            }
        }
    }
}
