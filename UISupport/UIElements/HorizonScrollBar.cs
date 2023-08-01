﻿using Microsoft.Xna.Framework.Input;

namespace ShopLookup.UISupport.UIElements
{
    public class HorizontalScrollbar : BaseUIElement
    {
        private readonly Texture2D Tex;
        private UIImage inner;
        private float mouseX;
        private float wheelValue;
        private int whell = 0;
        private bool isMouseDown = false;
        private float alpha = 0f;
        private float waitToWheelValue = 0f;
        private bool hide;
        public bool UseScrollWheel = false;
        public float WheelValue
        {
            get { return wheelValue; }
            set { waitToWheelValue = Math.Clamp(value, 0, 1); }
        }
        public HorizontalScrollbar(float wheelValue = 0f, bool hide = false)
        {
            Info.Height.Set(20f, 0f);
            Info.Top.Set(-20f, 1f);
            Info.Width.Set(-20f, 1f);
            Info.Left.Set(10f, 0f);
            Info.LeftMargin.Pixel = 5f;
            Info.RightMargin.Pixel = 5f;
            Info.IsSensitive = true;
            Tex = T2D("ShopLookup/UISupport/Asset/VerticalScrollbarInner");
            WheelValue = wheelValue;
            Info.IsHidden = hide;
            this.hide = hide;
        }
        public override void LoadEvents()
        {
            base.LoadEvents();
            Events.OnLeftDown += element =>
            {
                if (!isMouseDown)
                {
                    isMouseDown = true;
                }
            };
            Events.OnLeftUp += element =>
            {
                isMouseDown = false;
            };
        }
        public override void OnInitialization()
        {
            base.OnInitialization();
            inner = new UIImage(T2D("ShopLookup/UISupport/Asset/HorizontalScrollbarInner"), 26, 16);
            inner.Info.Top.Pixel = -(inner.Info.Height.Pixel - Info.Height.Pixel) / 2f;
            inner.ChangeColor(Color.White * alpha);
            inner.Info.IsHidden = hide;
            Register(inner);
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (ParentElement == null)
            {
                return;
            }
            bool isMouseHover = ParentElement.GetCanHitBox().Contains(Main.MouseScreen.ToPoint());
            if ((isMouseHover || isMouseDown) && alpha < 1f)
            {
                alpha += 0.01f;
            }

            if (!(isMouseHover || isMouseDown) && alpha > 0f)
            {
                alpha -= 0.01f;
            }

            inner.ChangeColor(Color.White * alpha);

            MouseState state = Mouse.GetState();
            float width = Info.Size.X - 26f;
            if (!isMouseHover)
            {
                whell = state.ScrollWheelValue;
            }

            if (UseScrollWheel && isMouseHover && whell != state.ScrollWheelValue)
            {
                WheelValue -= (state.ScrollWheelValue - whell) / 10f / width;
                whell = state.ScrollWheelValue;
            }
            if (isMouseDown && mouseX != Main.mouseX)
            {
                WheelValue = (Main.mouseX - Info.Location.X - 13f) / width;
                mouseX = Main.mouseX;
            }

            inner.Info.Left.Pixel = width * WheelValue;
            wheelValue += (waitToWheelValue - wheelValue) / 6f;

            if (waitToWheelValue != wheelValue)
            {
                Calculation();
            }
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            sb.Draw(Tex, new Rectangle(Info.HitBox.X - 12,
                Info.HitBox.Y + (Info.HitBox.Height - Tex.Height) / 2, 12, Tex.Height),
                new Rectangle(0, 0, 12, Tex.Height), Color.White * alpha);

            sb.Draw(Tex, new Rectangle(Info.HitBox.X,
                Info.HitBox.Y + (Info.HitBox.Height - Tex.Height) / 2, Info.HitBox.Width, Tex.Height),
                new Rectangle(12, 0, Tex.Width - 24, Tex.Height), Color.White * alpha);

            sb.Draw(Tex, new Rectangle(Info.HitBox.X + Info.HitBox.Width,
                Info.HitBox.Y + (Info.HitBox.Height - Tex.Height) / 2, 12, Tex.Height),
                new Rectangle(Tex.Width - 12, 0, 12, Tex.Height), Color.White * alpha);
        }
    }
}
