using ShopLookup.UISupport;
using static ShopLookup.Content.NonPermanentNPC;

namespace ShopLookup
{
    public class ShopLookup : Mod
    {
        internal static ShopLookup Ins;
        internal static bool Portable;
        internal static bool PermanentTips;

        public UISystem uis;
        public ShopLookup()
        {
            Ins = this;
        }
        public override object Call(params object[] args)
        {
            if (args[0] is string type)
            {
                if (type == "NonPermanent")
                {
                    if (args[1] is int npcType)
                    {
                        if (!IsNull())
                        {
                            var conditions = GetC(args);
                            if (args[2] is Condition[] cds)
                            {
                                conditions = cds;
                            }
                            if (conditions.Any())
                            {
                                if (TryGet(npcType, out var c))
                                {
                                    Conbine(npcType, c, conditions);
                                }
                                else
                                {
                                    Add(npcType, conditions);
                                }
                            }
                            else Add(npcType, NoC);
                        }
                        else Logger.Warn("Please call in Mod.PostSetupContent / ModSystem.PostSetupContent");
                    }
                    else Logger.Warn("parameter 2 type error");
                }
                else Logger.Warn("No method with this name, \"NonPermanent\" should be filled in");
            }
            else Logger.Warn("parameter 1 type error");
            return null;
        }
        private static IEnumerable<Condition> GetC(object[] args)
        {
            for (int i = 2; i < args.Length; i++)
            {
                yield return args[i] as Condition;
            }
        }
    }
}