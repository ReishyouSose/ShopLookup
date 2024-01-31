using ShopLookup.Content.Sys;
using System.Linq;

namespace ShopLookup.Content.UI.ExtraUI;

public class UICurrency : BaseUIElement
{
    //private readonly int currencyID;
    //private readonly int value;
    //private readonly ;
    public UICurrency(int value, int currencyID = -1)
    {
        SetSize(80, 24);
        //this.value = value;
        //this.currencyID = currencyID;
    }
    public override void DrawSelf(SpriteBatch sb)
    {
    }
}
