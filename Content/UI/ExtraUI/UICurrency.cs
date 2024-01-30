using ShopLookup.Content.Sys;

namespace ShopLookup.Content.UI.ExtraUI;

public class UICurrency : BaseUIElement
{
    private readonly int currency;
    private readonly int value;
    private readonly Dictionary<int, int> values;
    public UICurrency(int value, int currency = -1)
    {
        this.value = value;
        this.currency = currency;
        values = new();
        foreach (var (itemID, rank) in ShopNPCData.Currencys[currency])
        {
            values[itemID] = value / rank;
            value %= rank;
        }
    }
    public override void LoadEvents()
    {
        
    }
    public override void DrawSelf(SpriteBatch sb)
    {
    }
}
