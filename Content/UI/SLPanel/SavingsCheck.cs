using ShopLookup.Content.UI.ExtraUI;

namespace ShopLookup.Content.UI.SLPanel
{
    public partial class SLPanel
    {
        private List<BaseUIElement> savingsHide;
        private UIVnlPanel savingsBg;
        private UIText savingsTitle;
        private UIText savingsValue;
        private int blinkTime;
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
        private void RegisterSavings(UIVnlPanel bg)
        {
            savingsBg = new(0, 0);
            savingsBg.SetSize(-40, 30, 1);
            savingsBg.Info.IsVisible = false;
            bg.Register(savingsBg);

            savingsTitle = new(Lang.inter[66].Value) { color = G };
            savingsTitle.SetPos(10, 5);
            savingsTitle.SetSize(savingsTitle.TextSize);
            savingsBg.Register(savingsTitle);

            savingsValue = new(GetSavings(-1, out _));
            savingsValue.SetPos(savingsTitle.Width + 15, 3);
            savingsValue.SetSize(savingsValue.TextSize);
            savingsValue.Events.OnUpdate += UpdateSavings;
            savingsBg.Register(savingsValue);
        }
        private void ChangeSavingsVisable(bool visable)
        {
            savingsBg.Info.IsVisible = visable;
            savingsBg.SetPos(0, shopPanel.IsVisible ? 72 : 0);
            foreach (BaseUIElement uie in savingsHide)
            {
                uie.Info.IsVisible = !visable;
            }
        }
        private void UpdateSavings(BaseUIElement uie)
        {
            Item item = UIShopItem.HoverSlot?.itemSlot.item;
            if (item != null)
            {
                string text = GetSavings(item.shopSpecialCurrency, out long s);
                savingsValue.Info.Top.Pixel = s == 0 ? 5 : 3;
                savingsValue.ChangeText(text);
                savingsTitle.color = s > (item.shopCustomPrice ?? item.value) ? G : R;
            }
        }
        private void UpdateSavingsColor()
        {
            if (!savingsBg.IsVisible)
                blinkTime = 0;
            savingsTitle.overrideColor = Blink ? BlinkColor : null;
        }
        public bool CanAfford
        {
            get
            {
                if (savingsTitle.color == G)
                    return true;
                blinkTime = 36;
                return false;
            }
        }
        private void HoverHidden()
        {
            HoverHidden(shopView);
            HoverHidden(searchItem);
            HoverHidden(sellView);
            shopView.Info.IsSensitive = true;
            searchItem.Info.IsSensitive = true;
            sellView.Info.IsSensitive = true;
        }
        private void HoverHidden(UIContainerPanel view)
        {
            view.Events.OnMouseOver += evt => ChangeSavingsVisable(true);
            view.Events.OnMouseOut += evt => ChangeSavingsVisable(false);
        }
    }
}
