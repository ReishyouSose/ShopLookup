using RUIModule;
using ShopLookup.Content.Data;
using Terraria.UI.Chat;

namespace ShopLookup.Content.UI.ExtraUI;

public class UICurrency : BaseUIElement
{
    public Color color;
    public readonly int currencyID;
    /// <summary>
    /// id, stack
    /// </summary>
    private readonly Dictionary<int, int> values;
    public readonly int value;
    //private readonly ;
    public UICurrency(int value, int currencyID = -1)
    {
        SetSize(100, 24);
        this.currencyID = currencyID;
        this.value = value;
        color = Color.White;
        values = [];
        foreach (var (itemID, rank) in ShopNPCData.Currencys[currencyID])
        {
            int stack = value / rank;
            if (stack > 0)
            {
                values[itemID] = stack;
            }
            value %= rank;
        }
    }
    public override void DrawSelf(SpriteBatch sb)
    {
        Vector2 pos = HitBox().TopLeft();
        var font = FontAssets.MouseText.Value;
        string text = Lang.tip[49].Value;
        Vector2 scale = Vector2.One;
        Vector2 size = ChatManager.GetStringSize(font, text, scale);
        Vector2 z = Vector2.Zero;
        ChatManager.DrawColorCodedStringWithShadow(sb, font, text, pos, color, 0, z, scale, -1, 1.5f);
        pos.X += size.X;
        foreach (var (coin, stack) in values)
        {
            text = RUIHelper.ItemText(coin, stack);
            size = ChatManager.GetStringSize(font, text, size);
            ChatManager.DrawColorCodedStringWithShadow(sb, font, text, pos, color, 0, z, scale, -1, 1.5f);
            CheckDrawItem(pos, coin, stack);
            pos.X += size.X;
        }
    }

    private void CheckDrawItem(Vector2 pos, int itemID, int stack)
    {
        if (Info.CanBeInteract && !Info.IsLocked)
        {
            if (RUIHelper.NewRec(pos, new(24)).Contains(Main.MouseScreen.ToPoint()))
            {
                Main.HoverItem = new(itemID, stack);
                Main.hoverItemName = Main.HoverItem.Name;
            }
            UIShopItem.HoverSlot = null;
        }
    }
}
