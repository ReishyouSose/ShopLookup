using ShopLookup.Content;
using Terraria.GameInput;

namespace ShopLookup.UISupport.UIElements
{
    public class UIContainerPanel : BaseUIElement
    {
        public static Effect eff;
        private class InnerPanel : BaseUIElement
        {
            public override Rectangle HiddenOverflowRectangle => ParentElement.HiddenOverflowRectangle;
            public override Rectangle GetCanHitBox() => Rectangle.Intersect(ParentElement.GetCanHitBox(), ParentElement.Info.TotalHitBox);
            public InnerPanel()
            {
                Info.Width.Percent = 1f;
                Info.Height.Percent = 1f;
            }
        }
        private InnerPanel _innerPanel;
        public List<BaseUIElement> InnerUIE => _innerPanel.ChildrenElements;
        private VerticalScrollbar _verticalScrollbar;
        public VerticalScrollbar Vscroll => _verticalScrollbar;
        private HorizontalScrollbar _horizontalScrollbar;
        public HorizontalScrollbar Hscroll => _horizontalScrollbar;
        private float verticalWhellValue;
        private float horizontalWhellValue;
        private Vector2 innerPanelMinLocation;
        private Vector2 innerPanelMaxLocation;
        public Vector2 MovableSize
        {
            get
            {
                float maxX = Math.Max(innerPanelMinLocation.X, innerPanelMaxLocation.X - _innerPanel.Info.TotalSize.X);
                float maxY = Math.Max(innerPanelMinLocation.Y, innerPanelMaxLocation.Y - _innerPanel.Info.TotalSize.Y);
                return new(maxX, maxY);
            }
        }
        public UIContainerPanel()
        {
            Info.HiddenOverflow = true;
            Info.Width.Percent = 1f;
            Info.Height.Percent = 1f;
            if (_innerPanel == null)
            {
                _innerPanel = new InnerPanel();
                Register(_innerPanel);
            }
            //eff ??= ModContent.Request<Effect>(SLUI.AssetKey + "AddColor", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            eff ??= ModContent.Request<Effect>(SLUI.AssetKey + "InvisibleBlur", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }
        public void SetVerticalScrollbar(VerticalScrollbar scrollbar)
        {
            _verticalScrollbar = scrollbar;
            /*Info.TopMargin.Pixel = 10;
            Info.ButtomMargin.Pixel = 10;
            Calculation();*/
            scrollbar.View = this;
        }

        public void SetHorizontalScrollbar(HorizontalScrollbar scrollbar)
        {
            _horizontalScrollbar = scrollbar;
            /*Info.LeftMargin.Pixel = 10;
            Info.RightMargin.Pixel = 10;
            Calculation();*/
            scrollbar.View = this;
        }

        public override void OnInitialization()
        {
            base.OnInitialization();
            if (_innerPanel == null)
            {
                _innerPanel = new InnerPanel();
                Register(_innerPanel);
            }
            Info.IsSensitive = true;
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (HitBox().Contains(Main.MouseScreen.ToPoint()) && _verticalScrollbar != null | _horizontalScrollbar != null)
            {
                PlayerInput.LockVanillaMouseScroll("VIScroll");
            }
            if (_verticalScrollbar != null && verticalWhellValue != _verticalScrollbar.RealWheelValue)
            {
                verticalWhellValue = _verticalScrollbar.RealWheelValue;
                float maxY = MovableSize.Y;/* innerPanelMaxLocation.Y - _innerPanel.Info.TotalSize.Y;
                if (maxY < innerPanelMinLocation.Y)
                {
                    maxY = innerPanelMinLocation.Y;
                }*/

                _innerPanel.Info.Top.Pixel = -MathHelper.Lerp(innerPanelMinLocation.Y, maxY, verticalWhellValue);
                Calculation();
            }
            if (_horizontalScrollbar != null && horizontalWhellValue != _horizontalScrollbar.RealWheelValue)
            {
                horizontalWhellValue = _horizontalScrollbar.RealWheelValue;
                float maxX = MovableSize.X;/*innerPanelMaxLocation.X - _innerPanel.Info.TotalSize.X;
                if (maxX < innerPanelMinLocation.X)
                {
                    maxX = innerPanelMinLocation.X;
                }*/

                _innerPanel.Info.Left.Pixel = -MathHelper.Lerp(innerPanelMinLocation.X, maxX, horizontalWhellValue);
                Calculation();
            }
        }
        public bool AddElement(BaseUIElement element)
        {
            bool flag = _innerPanel.Register(element);
            if (flag)
            {
                Calculation();
            }

            return flag;
        }
        public bool RemoveElement(BaseUIElement element)
        {
            bool flag = _innerPanel.Remove(element);
            if (flag)
            {
                Calculation();
            }

            return flag;
        }
        public void ClearAllElements()
        {
            _innerPanel.ChildrenElements.Clear();
            Calculation();
        }
        private void CalculationInnerPanelSize()
        {
            innerPanelMinLocation = Vector2.Zero;
            innerPanelMaxLocation = Vector2.Zero;
            Vector2 v = Vector2.Zero;
            _innerPanel.ForEach(element =>
            {
                v.X = element.Info.TotalLocation.X - _innerPanel.Info.Location.X;
                v.Y = element.Info.TotalLocation.Y - _innerPanel.Info.Location.Y;
                if (innerPanelMinLocation.X > v.X)
                {
                    innerPanelMinLocation.X = v.X;
                }

                if (innerPanelMinLocation.Y > v.Y)
                {
                    innerPanelMinLocation.Y = v.Y;
                }

                v.X = element.Info.TotalLocation.X + element.Info.TotalSize.X - _innerPanel.Info.Location.X;
                v.Y = element.Info.TotalLocation.Y + element.Info.TotalSize.Y - _innerPanel.Info.Location.Y;

                if (innerPanelMaxLocation.X < v.X)
                {
                    innerPanelMaxLocation.X = v.X;
                }

                if (innerPanelMaxLocation.Y < v.Y)
                {
                    innerPanelMaxLocation.Y = v.Y;
                }
            });
        }
        public override void Calculation()
        {
            base.Calculation();
            CalculationInnerPanelSize();
            _innerPanel.Calculation();
        }
        public override void DrawChildren(SpriteBatch sb)
        {
            /*if (InnerUIE.Count > 0)
            {
                RasterizerState overflowHiddenRasterizerState = new()
                {
                    CullMode = CullMode.None,
                    ScissorTestEnable = true
                };
                GraphicsDevice gd = sb.GraphicsDevice;
                var r = sb.GraphicsDevice.GetRenderTargets();

                sb.End();
                gd.SetRenderTarget(SLUISys.render);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                    DepthStencilState.None, overflowHiddenRasterizerState, null, Main.UIScaleMatrix);
                base.DrawChildren(sb);
                sb.End();

                gd.SetRenderTargets(Main.screenTarget);
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                eff.Parameters["hitbox"].SetValue(HitBox().ToVector4());
                eff.Parameters["inner"].SetValue(HitBox(false).ToVector4());
                eff.CurrentTechnique.Passes[0].Apply();
                sb.Draw(SLUISys.render, Vector2.Zero, Color.White);

                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                    DepthStencilState.None, overflowHiddenRasterizerState, null, Main.UIScaleMatrix);
                return;
            }*/
            base.DrawChildren(sb);
        }
    }
}
