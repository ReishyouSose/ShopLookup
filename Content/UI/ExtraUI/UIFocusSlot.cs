namespace ShopLookup.Content.UI.ExtraUI
{
    public class UIFocusSlot : BaseUIElement
    {
        public readonly UIItemSlot itemSlot;
        public readonly UINPCSlot npcSlot;
        public readonly UIImage iconSlot;
        public bool HasFocus { get; private set; }
        public UIFocusSlot()
        {
            SetSize(52, 52);
            itemSlot = new();
            npcSlot = new(0);
            npcSlot.Info.IsVisible = false;
            iconSlot = new(null);
            iconSlot.Info.IsVisible = false;
            Register(itemSlot);
            Register(npcSlot);
            Register(iconSlot);
            Info.IsSensitive = true;
        }
        public void ChangeFocus(Entity entity)
        {
            if (entity != null)
            {
                if (entity is Item item)
                {
                    itemSlot.ContainedItem = ContentSamples.ItemsByType[item.type].Clone();
                    itemSlot.Info.IsVisible = true;
                    npcSlot.Info.IsVisible = false;
                    iconSlot.Info.IsVisible = false;
                    HasFocus = true;
                }
                else if (entity is NPC npc)
                {
                    npcSlot.ChangeNPC(npc.type);
                    npcSlot.Info.IsVisible = true;
                    itemSlot.Info.IsVisible = false;
                    iconSlot.Info.IsVisible = false;
                    HasFocus = true;
                }
            }
        }
        public void ChangeFocus(Texture2D tex)
        {
            iconSlot.ChangeImage(tex);
            iconSlot.Info.IsVisible = true;
            itemSlot.Info.IsVisible = false;
            npcSlot.Info.IsVisible = false;
        }
        public void ReSetFocus()
        {
            if (HasFocus)
            {
                itemSlot.ContainedItem = null;
                itemSlot.Info.IsVisible = true;
                npcSlot.Info.IsVisible = false;
                iconSlot.Info.IsVisible = false;
                HasFocus = false;
            }
        }
    }
}
