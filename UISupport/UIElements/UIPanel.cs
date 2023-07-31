﻿namespace ShopLookup.UISupport.UIElements
{
    public class UIPanel : UIBottom
    {
        public Texture2D Tex;
        public Color color;
        public float opacity = 0.5f;
        public UIPanel(string texKey, float x, float y, Color? color = null, float opacity = 0.5f) : base(x, y)
        {
            texKey = texKey == default ? "ShopLookup/UISupport/Asset/SkillTree_bg9" : texKey;
            Tex = T2D(texKey);
            Info.CanBeInteract = true;
            CanDrag = false;
            this.color = color ?? Color.White;
            this.opacity = opacity;
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            int dis = Tex.Width / 3;
            Rectangle[] coords = Rec3x3(dis, dis);
            Rectangle rec = HitBox();
            Vector2 size = new(Tex.Width / 6f);
            //sb.Draw(ShadowTex, NewRec(rec.TopLeft(), rec.Width, rec.Height), null, color * 0.5f);
            sb.Draw(Tex, rec, coords[4], color * opacity);
            sb.Draw(Tex, NewRec(rec.TopLeft() - new Vector2(0, dis / 2), rec.Width, dis), coords[1], Color.White);
            sb.Draw(Tex, NewRec(rec.TopLeft() - new Vector2(dis / 2, 0), dis, rec.Height), coords[3], Color.White);
            sb.Draw(Tex, NewRec(rec.TopRight() - new Vector2(dis / 2, 0), dis, rec.Height), coords[5], Color.White);
            sb.Draw(Tex, NewRec(rec.BottomLeft() - new Vector2(0, dis / 2), rec.Width, dis), coords[7], Color.White);
            Draw(sb, Tex, rec.TopLeft(), coords[0], size);
            Draw(sb, Tex, rec.TopRight(), coords[2], size);
            Draw(sb, Tex, rec.BottomLeft(), coords[6], size);
            Draw(sb, Tex, rec.BottomRight(), coords[8], size);
        }
    }
}
