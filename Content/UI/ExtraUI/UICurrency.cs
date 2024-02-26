using ShopLookup.Content.Sys;
using System.Linq;
using Terraria.UI.Chat;

namespace ShopLookup.Content.UI.ExtraUI;

public class UICurrency : BaseUIElement
{
    public Color color;
    private readonly int currencyID;
    private readonly Dictionary<int, int> values;
    private readonly string valueText;
    private readonly UIText hasCrc;
    private readonly int value;
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
    //private readonly ;
    public UICurrency(int value, int currencyID = -1)
    {
        SetSize(100, 24);
        this.currencyID = currencyID;
        this.value = value;
        color = Color.White;
        values = [];
        float i = 0;
        float offsetX = FontAssets.MouseText.Value.MeasureString(GTV("SellPrice")).X + 10;
        foreach (var (itemID, rank) in ShopNPCData.Currencys[currencyID])
        {
            int stack = value / rank;
            if (stack > 0)
            {
                values[itemID] = stack;
                UIItem c = new(itemID, stack);
                //c.DrawRec[0] = Color.Red;
                c.SetPos(offsetX + i, 0);
                Register(c);
                i += 26;
                //valueText += $"[i/s{stack}:{itemID}]";
            }
            value %= rank;
        }
        hasCrc = new("");
        hasCrc.SetPos(offsetX + i, 0);
        hasCrc.ReDraw = DrawHasCurrency;
        hasCrc.Info.IsVisible = false;
        Register(hasCrc);
    }
    public override void LoadEvents()
    {
        ParentElement.Events.OnMouseOver += evt => hasCrc.Info.IsVisible = true;
        ParentElement.Events.OnMouseOut += evt => hasCrc.Info.IsVisible = false;
    }
    public override void DrawSelf(SpriteBatch sb)
    {
        Vector2 pos = HitBox().TopLeft();
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value,
            GTV("SellPrice"), pos, Color.White, 0, Vector2.Zero, Vector2.One, -1, 1.5f);
    }
    private void DrawHasCurrency(SpriteBatch sb)
    {
        Player player = Main.LocalPlayer;
        var font = FontAssets.MouseText.Value;
        List<Item> inv = [];
        inv.AddRange(player.inventory);
        inv.AddRange(player.bank.item);
        inv.AddRange(player.bank2.item);
        inv.AddRange(player.bank3.item);
        inv.AddRange(player.bank4.item);
        Dictionary<int, int> crcs = ShopNPCData.Currencys[currencyID];
        long count = 0;
        foreach (Item item in inv)
        {
            if (crcs.TryGetValue(item.type, out int value))
            {
                count += item.stack * (long)value;
            }
        }
        color = count >= value ? G : R;
        if (Blink) color = BlinkColor;
        Vector2 pos = hasCrc.HitBox().TopLeft();
        TextSnippet[] savings = [new(" / "), new(Lang.inter[66].Value, color), new(" ")];
        ChatManager.DrawColorCodedStringWithShadow(sb, font, savings, pos, 0, Vector2.Zero, Vector2.One, out _, -1, 1.5f);
        pos.X += ChatManager.GetStringSize(font, savings, Vector2.One).X;
        if (count == 0)
        {
            ChatManager.DrawColorCodedStringWithShadow(sb, font, GTV("NoSavings"),
                pos, Color.White, 0, Vector2.Zero, Vector2.One, -1, 1.5f);
            return;
        }
        pos.Y -= 3;
        Dictionary<int, int> hasCrcs = crcs.ToDictionary(x => x.Key, x => 0);
        foreach (var (itemID, rank) in crcs)
        {
            int stack = (int)(count / rank);
            count %= rank;
            hasCrcs[itemID] = stack;
        }
        foreach (var (itemID, stack) in hasCrcs)
        {
            if (stack > 0)
            {
                ChatManager.DrawColorCodedStringWithShadow(sb, font, ItemText(itemID, stack),
                    pos, Color.White, 0, Vector2.Zero, Vector2.One, -1, 1.5f);
                CheckDrawItem(pos, itemID, stack);
                pos.X += 26;
            }
        }
    }
    private static void CheckDrawItem(Vector2 pos, int itemID, int stack)
    {
        if (NewRec(pos, new(24)).Contains(Main.MouseScreen.ToPoint()))
        {
            Main.HoverItem = new(itemID, stack);
            Main.hoverItemName = Main.HoverItem.Name;
        }
    }
    public void StartBlink() => blinkTime = 36;
}
