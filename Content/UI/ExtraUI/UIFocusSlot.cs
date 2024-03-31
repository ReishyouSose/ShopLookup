namespace ShopLookup.Content.UI.ExtraUI
{
    public class UIFocusSlot : BaseUIElement
    {
        public readonly UIItemSlot itemSlot;
        public readonly UINPCSlot npcSlot;
        public readonly UIImage iconSlot;
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
                    return;
                }
                if (entity is NPC npc)
                {
                    npcSlot.ChangeNPC(npc.type);
                    npcSlot.Info.IsVisible = true;
                    itemSlot.Info.IsVisible = false;
                    iconSlot.Info.IsVisible = false;
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
        public override void LoadEvents()
        {
            Events.OnLeftDown += evt => ReSetFocus();
        }
        private void ReSetFocus()
        {
            itemSlot.ContainedItem = null;
            itemSlot.Info.IsVisible = true;
            npcSlot.Info.IsVisible = false;
            iconSlot.Info.IsVisible = false;
        }
    }
}
