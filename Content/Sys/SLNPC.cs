using ShopLookup.Content.Data;

namespace ShopLookup.Content.Sys
{
    public class SLNPC : GlobalNPC
    {
        public override void PostAI(NPC npc)
        {
            if (Main.GameUpdateCount % 60 == 0 && !npc.homeless)
            {
                ShopNPCData.VisitedNPCs.Add(npc.type);
            }
        }
    }
}
