using ShopLookup.Content.UI.ExtraUI;
using System.Linq;

namespace ShopLookup.Content.UI.SLPanel
{
    public partial class SLPanel
    {
        private UIVnlPanel stackPanel;
        private UIText stacker;
        private int cd;
        public int BuyStack { get; private set; } = 1;
        private void RegisterStackPanel()
        {
            stackPanel = new(150, 30 * 8 - 10);
            stackPanel.Info.IsVisible = false;
            stackPanel.Info.SetMargin(10);
            Register(stackPanel);

            int t = 0;
            stacker = new("1", drawStyle: 1);
            stacker.SetSize(50, 30);
            stacker.SetCenter(0, 15, 0.5f);
            stackPanel.Register(stacker);
            t += 30;

            for (int i = 1; i <= 4; i++)
            {
                int j = (int)Math.Pow(10, i - 1);
                UIText stack = new(j.ToString(), drawStyle: 1);
                stack.SetSize(50, 30);
                stack.SetPos(40, t);
                stack.HoverToGold();
                stack.Events.OnLeftDown += evt => SetStack(j);
                stackPanel.Register(stack);

                UIImage decrease = new(AssetLoader.Decrease);
                decrease.SetPos(0, t);
                decrease.Events.OnLeftDown += evt =>
                {
                    int old = BuyStack;
                    BuyStack = Math.Max(BuyStack - j, 1);
                    stacker.ChangeText(BuyStack.ToString(), false);
                    if (old == BuyStack)
                        return;
                    SetStack();
                };
                stackPanel.Register(decrease);

                UIImage increase = new(AssetLoader.Increase);
                increase.SetPos(-20, t, 1);
                increase.Events.OnLeftDown += evt =>
                {
                    int old = BuyStack;
                    BuyStack = Math.Min(BuyStack + j, 9999);
                    stacker.ChangeText(BuyStack.ToString(), false);
                    if (old == BuyStack)
                        return;
                    SetStack();
                };
                stackPanel.Register(increase);

                t += 30;
            }

            UIText min = new("Min", drawStyle: 1);
            min.SetSize(50, 30);
            min.SetPos(0, t);
            min.HoverToGold();
            min.Events.OnLeftDown += evt => SetStack(1);
            stackPanel.Register(min);

            UIText max = new("Max", drawStyle: 1);
            max.SetSize(50, 30);
            max.SetPos(-50, t, 1);
            max.HoverToGold();
            max.Events.OnLeftDown += evt => SetStack(9999);
            stackPanel.Register(max);
            t += 30;

            UIVnlPanel stackBg = new(0, 0);
            stackBg.SetSize(0, 30, 1);
            stackBg.SetPos(0, -30, 0, 1);
            stackPanel.Register(stackBg);

            UIInputBox stackInputer = new(GTV("Info.InputStack"));
            stackInputer.SetSize(-40, 0, 1, 1);
            stackInputer.OnInputText += text =>
            {
                if (int.TryParse(text, out int value))
                {
                    value = Math.Clamp(value, 1, 9999);
                    SetStack(value);
                }
            };
            stackBg.Register(stackInputer);

            UIClose clear = new();
            clear.SetPos(-30, 5, 1);
            clear.Events.OnLeftDown += evt => stackInputer.ClearText();
            stackBg.Register(clear);
        }
        private void SetStack(int? stack = null)
        {
            if (stack.HasValue)
            {
                BuyStack = stack.Value;
            }
            stacker.ChangeText(BuyStack.ToString(), false);
            foreach (UIShopItem slot in shopView.InnerUIE.Concat(searchItem.InnerUIE).Cast<UIShopItem>())
            {
                Item item = slot.itemSlot.item;
                item.stack = Math.Min(item.maxStack, BuyStack);
                slot.currency.ResetValue((item.shopCustomPrice ?? item.value) * item.stack);
            }
        }
        private bool CDCompleted()
        {
            if (cd-- <= 0)
            {
                cd = 15;
                return true;
            }
            return false;
        }
    }
}
