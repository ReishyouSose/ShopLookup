using ShopLookup.Content.UI.ExtraUI;

namespace ShopLookup.Content.Sys
{
    public class SLItem : GlobalItem
    {
        private const string CD = "Condition";
        private const string PlaceHolder = "              .";
        private static Color fake = new Color(23, 25, 81, 255) * 0.925f;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            UIShopItem slot = UIShopItem.HoverSlot;
            if (slot != null)
            {
                if (slot is UIShopItemForNPC npc)
                {
                    int npcType = npc.npcType;
                    int index = NPC.FindFirstNPC(npcType);
                    tooltips.Add(new(Mod, "NPC", GTV("UIButton.Find", ContentSamples.NpcsByNetId[npcType].TypeName))
                    { OverrideColor = index > -1 ? Color.Green : Color.Red });
                }
                tooltips.Add(new(Mod, "Currency", slot.currency.ToItemText()));
                tooltips.Add(new(Mod, "HasCrcs", Lang.inter[66].Value + PlaceHolder) { OverrideColor = fake });

                int i = 0;
                foreach (CdCheck cd in slot.cdChecks)
                {
                    tooltips.Add(new(Mod, CD + i++, cd.condition.Description.Value) { OverrideColor = cd.Color });
                }
            }
        }
        public override void PostDrawTooltipLine(Item item, DrawableTooltipLine line)
        {
            UIShopItem slot = UIShopItem.HoverSlot;
            if (slot != null && line.Mod == Mod.Name && line.Name == "HasCrcs")
            {
                slot.currency.DrawHasCurrency(Main.spriteBatch, new(line.X, line.Y));
            }
        }
    }
}
