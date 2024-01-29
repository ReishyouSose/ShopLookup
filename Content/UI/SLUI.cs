using RUIModule.RUIElements;

namespace ShopLookup.Content.UI
{
    public class SLUI : ContainerElement
    {
        public override void OnInitialization()
        {
            base.OnInitialization();

            UIPanel bg = new(500, 360);
            bg.SetCenter(0, 0, 0.5f, 0.5f);
            bg.Info.SetMargin(20);
            Register(bg);

            UIContainerPanel npcs = new();
            npcs.SetSize(-144, 52, 1);
            npcs.SetPos(72, 0);
            bg.Register(npcs);

            HorizontalScrollbar hscroll = new(62);
            npcs.SetHorizontalScrollbar(hscroll);
            bg.Register(hscroll);

            
        }
    }
}
