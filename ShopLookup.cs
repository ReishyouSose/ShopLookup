using ShopLookup.UISupport;
using Terraria.Localization;
using static ShopLookup.Content.ExtraNPCInfo;

namespace ShopLookup
{
    public class ShopLookup : Mod
    {
        internal static ShopLookup Ins;
        internal static bool Portable;
        internal static bool PermanentTips;
        internal static bool IgnoreUnknowCds;
        internal static bool EnableQoT;

        public UISystem uis;
        public ShopLookup()
        {
            Ins = this;
        }
        public override void PostSetupContent()
        {
            EnableQoT = ModLoader.HasMod("ImproveGame");
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
                                if (NonTryGet(npcType, out var c))
                                {
                                    NonConbine(npcType, c, conditions);
                                }
                                else
                                {
                                    NonAdd(npcType, conditions);
                                }
                            }
                            else NonAdd(npcType, NoC);
                        }
                        else Logger.Warn("Please call in Mod.PostSetupContent / ModSystem.PostSetupContent");
                    }
                    else Logger.Warn("parameter 2 type error");
                }
                else if (type == "ShopName")
                {
                    if (args[1] is int npcType)
                    {
                        if (args[2] is string index)
                        {
                            if (args[3] is LocalizedText text)
                            {
                                if (!NameTryGet(npcType, out _))
                                {
                                    NameAdd(npcType, index, text);
                                }
                            }
                            else Logger.Warn("parameter 4 type error");
                        }
                        else Logger.Warn("parameter 3 type error");
                    }
                    else Logger.Warn("parameter 2 type error");
                }
                else Logger.Warn("No method with this name");
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