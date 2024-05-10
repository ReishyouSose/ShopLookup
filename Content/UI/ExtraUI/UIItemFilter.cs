using Terraria.UI;

namespace ShopLookup.Content.UI.ExtraUI
{
    public abstract class UIItemFilter() : UIImage(null)
    {
        private bool active;
        public bool Active
        {
            get => active;
            set
            {
                active = value;
                overrideColor = active ? Color.Gold : null;
            }
        }
        public override void OnInitialization()
        {
            base.OnInitialization();
            Tex = AssetLoader.ExtraAssets["Filter"];
            hoverText = Language.GetTextValue(GetDisplayNameKey());
        }
        public abstract bool FitsFilter(Item entry);
        public abstract string GetDisplayNameKey();
    }
    public class BuildingBlock() : UIItemFilter()
    {
        public override bool FitsFilter(Item entry)
        {
            if (entry.createWall != -1)
                return true;

            if (entry.tileWand != -1)
                return true;

            if (entry.createTile == -1)
                return false;

            return !Main.tileFrameImportant[entry.createTile];
        }
        public override string GetDisplayNameKey() => "CreativePowers.TabBlocks";
    }
    public class Furniture() : UIItemFilter()
    {
        public override bool FitsFilter(Item entry)
        {
            int createTile = entry.createTile;
            if (createTile == -1)
                return false;

            return Main.tileFrameImportant[createTile];
        }

        public override string GetDisplayNameKey() => "CreativePowers.TabFurniture";
    }
    public class Tools() : UIItemFilter()
    {
        //打表
        #region
        private HashSet<int> _itemIdsThatAreAccepted = [
            509,
            850,
            851,
            3612,
            3625,
            3611,
            510,
            849,
            3620,
            1071,
            1543,
            1072,
            1544,
            1100,
            1545,
            50,
            3199,
            3124,
            5358,
            5359,
            5360,
            5361,
            5437,
            1326,
            5335,
            3384,
            4263,
            4819,
            4262,
            946,
            4707,
            205,
            206,
            207,
            1128,
            3031,
            4820,
            5302,
            5364,
            4460,
            4608,
            4872,
            3032,
            5303,
            5304,
            1991,
            4821,
            3183,
            779,
            5134,
            1299,
            4711,
            4049,
            114
        ];
        #endregion
        public override bool FitsFilter(Item entry)
        {
            if (entry.pick > 0)
                return true;

            if (entry.axe > 0)
                return true;

            if (entry.hammer > 0)
                return true;

            if (entry.fishingPole > 0)
                return true;

            if (entry.tileWand != -1)
                return true;

            if (_itemIdsThatAreAccepted.Contains(entry.type))
                return true;

            return false;
        }

        public override string GetDisplayNameKey() => "CreativePowers.TabTools";
    }
    public class Weapon() : UIItemFilter()
    {
        public override bool FitsFilter(Item entry) => entry.damage > 0;
        public override string GetDisplayNameKey() => "CreativePowers.TabWeapons";
    }
    public class Armor() : UIItemFilter()
    {
        public override bool FitsFilter(Item entry)
            => !entry.vanity && (entry.bodySlot != -1 || entry.headSlot != -1 || entry.legSlot != -1);
        public override string GetDisplayNameKey() => "CreativePowers.TabArmor";
    }
    public class Vanity() : UIItemFilter()
    {
        public override bool FitsFilter(Item entry) => entry.vanity;
        public override string GetDisplayNameKey() => "CreativePowers.TabVanity";
    }
    public class Accessories() : UIItemFilter()
    {
        public override bool FitsFilter(Item entry) => entry.accessory && !ItemSlot.IsMiscEquipment(entry);
        public override string GetDisplayNameKey() => "CreativePowers.TabAccessories";
    }
    public class MiscAccessories() : UIItemFilter()
    {
        public override bool FitsFilter(Item entry) => ItemSlot.IsMiscEquipment(entry);
        public override string GetDisplayNameKey() => "CreativePowers.TabAccessoriesMisc";
    }
    public class Consumables() : UIItemFilter()
    {
        public override bool FitsFilter(Item entry)
        {
            int type = entry.type;
            if (type == 267 || type == 1307)
                return true;

            bool flag = entry.createTile != -1 || entry.createWall != -1 || entry.tileWand != -1;
            if (entry.consumable)
                return !flag;

            return false;
        }

        public override string GetDisplayNameKey() => "CreativePowers.TabConsumables";
    }
    public class Materials() : UIItemFilter()
    {
        public override bool FitsFilter(Item entry) => entry.material;
        public override string GetDisplayNameKey() => "CreativePowers.TabMaterials";
    }
    public class MiscFallback : UIItemFilter
    {
        private bool[] _fitsFilterByItemType;

        public MiscFallback(List<UIItemFilter> otherFilters) : base()
        {
            int count = ItemLoader.ItemCount;
            _fitsFilterByItemType = new bool[count];
            for (int i = 1; i < count; i++)
            {
                _fitsFilterByItemType[i] = true;
                Item entry = ContentSamples.ItemsByType[i];
                foreach (UIItemFilter filter in otherFilters)
                {
                    if (filter.FitsFilter(entry))
                    {
                        _fitsFilterByItemType[i] = false;
                        break;
                    }
                }
            }
        }
        public override bool FitsFilter(Item entry)
        {
            if (_fitsFilterByItemType.IndexInRange(entry.type))
                return _fitsFilterByItemType[entry.type];
            return false;
        }

        public override string GetDisplayNameKey() => "CreativePowers.TabMisc";
    }
}