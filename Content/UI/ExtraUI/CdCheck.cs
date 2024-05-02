using Terraria.UI.Chat;
using static ShopLookup.Content.Sys.SLConfig;

namespace ShopLookup.Content.UI.ExtraUI
{
    public class CdCheck
    {
        public readonly Condition condition;
        public readonly bool ignore;
        private readonly string desc;
        private int blinkTime;
        public Color Color { get; private set; }
        public bool Blink
        {
            get
            {
                if (blinkTime > 0)
                {
                    blinkTime--;
                    return true;
                }
                return false;
            }
        }
        public Color BlinkColor => blinkTime / 6 % 2 == 0 ? R : Color.White;
        public string Desc { get; private set; }
        public float TextY { get; private set; }
        public bool IsMet => condition.IsMet();
        public CdCheck(Condition condition, float maxWidth)
        {
            this.condition = condition;
            ignore = IgnoreCondition(out desc);
            Desc = desc;
            Calculate(maxWidth);
        }
        public void Update(bool mouseHover, bool buying)
        {
            if (buying)
            {
                Color = G;
                return;
            }
            if (Blink)
            {
                Color = BlinkColor;
                return;
            }
            if (mouseHover)
            {
                if (ignore)
                {
                    Color = Y;
                    return;
                }
                if (Ins.Portable || Ins.PermanentTips)
                {
                    Color = condition.IsMet() ? G : R;
                    return;
                }
            }
            Color = Color.White;
        }
        public void Calculate(float maxWidth)
        {
            Desc = FontAssets.MouseText.Value.CreateWrappedText(desc, maxWidth);
            TextY = ChatManager.GetStringSize(FontAssets.MouseText.Value, Desc, Vector2.One).Y;
        }
        public void StartBlink()
        {
            blinkTime = 36;
        }

        private bool IgnoreCondition(out string desc)
        {
            if (condition.Description.Key == "" || condition.Description.Value == "")
            {
                desc = GTV("UnknowCds");
                return true;
            }
            desc = condition.Description.Value;
            return condition == Condition.AnotherTownNPCNearby || condition == Condition.HappyEnoughToSellPylons;
        }
    }
}
