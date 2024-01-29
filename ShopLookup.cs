using RUIModule.RUISys;

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
            throw new NotImplementedException();
        }
    }
}