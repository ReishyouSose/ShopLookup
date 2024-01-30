using ShopLookup.Content.Sys;

namespace ShopLookup
{
    public class ShopLookup : Mod
    {
        public override void Load()
        {
            RUIManager.mod = this;
            AssetLoader.ExtraLoad += AssetLoader_ExtraLoad;
            AddContent<RUIManager>();
        }

        private void AssetLoader_ExtraLoad(Dictionary<string, Texture2D> extraAssets)
        {
            string[] files = ["All", "Coins", "NoIcon", "QoT", "Slot", "Vanilla"];
            string path = GetType().Namespace + "/Assets/";
            foreach (string file in files)
            {
                extraAssets[file] = T2D(path + file);
            }
        }
        public override void PostSetupContent()
        {
            RUIManager.Ins.ExtraDrawOver += SLUI.ExtraDrawInfo;
            ShopNPCData.ReflectCurrency();
        }
    }
}