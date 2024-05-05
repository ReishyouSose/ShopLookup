global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using RUIModule.RUIElements;
global using RUIModule.RUISys;
global using ShopLookup.Content.UI;
global using System;
global using System.Collections.Generic;
global using System.Reflection;
global using Terraria;
global using Terraria.Audio;
global using Terraria.GameContent;
global using Terraria.GameContent.UI;
global using Terraria.ID;
global using Terraria.Localization;
global using Terraria.ModLoader;
global using static ShopLookup.MiscHelper;


namespace ShopLookup
{
    public static class MiscHelper
    {
        public static IEnumerable<(int itemId, int count)> ToCoins(int money)
        {
            int copper = money % 100;
            money /= 100;
            int silver = money % 100;
            money /= 100;
            int gold = money % 100;
            money /= 100;
            int plat = money;

            yield return (ItemID.PlatinumCoin, plat);
            yield return (ItemID.GoldCoin, gold);
            yield return (ItemID.SilverCoin, silver);
            yield return (ItemID.CopperCoin, copper);
        }
        public static bool HasShop(int npcType)
        {
            foreach (AbstractNPCShop nshop in NPCShopDatabase.AllShops)
            {
                if (nshop.NpcType == npcType)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 播放拿起物品的音效
        /// </summary>
        public static void SoundCoins()
        {
            SoundEngine.PlaySound(SoundID.Coins);
        }
        public static Vector4 ToVector4(this Rectangle rec, bool ToShader = true)
        {
            float x = rec.X;
            float y = rec.Y;
            float w = rec.Width;
            float h = rec.Height;
            if (ToShader)
            {
                int rx = Main.screenWidth;
                int ry = Main.screenHeight;
                x = Utils.GetLerpValue(0, rx, x, true);
                y = Utils.GetLerpValue(0, ry, y, true);
                w = Utils.GetLerpValue(0, rx, w, true);
                h = Utils.GetLerpValue(0, ry, h, true);
            }
            return new Vector4(x, y, w, h);
        }
        private const string LocalKey = "Mods.ShopLookup.";
        public static string GTV(string key) => Language.GetTextValue(LocalKey + key);
        public static string GTV(string key, params string[] obj) => Language.GetText(LocalKey + key).WithFormatArgs(obj).Value;
        public static Vector2 ScrResolution => new(Main.screenWidth, Main.screenHeight);
        public static SLPanel SLUI => RUIManager.UIEs[typeof(SLPanel).FullName] as SLPanel;
        public static readonly Color G = new(0, 230, 100, 255);
        public static readonly Color R = new(255, 50, 100, 255);
        public static readonly Color Y = new(255, 165, 0, 255);
        public static bool AllMet(this IEnumerable<Condition> cds)
        {
            foreach (Condition c in cds)
            {
                if (!c.IsMet())
                    return false;
            }
            return true;
        }

        /// <returns>对应NPC存活</returns>
        public static bool TryGetCanBuyEntrys(this AbstractNPCShop shop, out Item[] result)
        {
            result = new Item[40];
            int index = NPC.FindFirstNPC(shop.NpcType);
            if (index == -1)
                return false;
            int i = 0;
            foreach (var entry in shop.ActiveEntries)
            {
                if (!entry.Conditions.AllMet())
                    continue;
                result[i++] = entry.Item;
            }
            NPC npc = Main.npc[index];
            NPCLoader.ModifyActiveShop(npc, shop.FullName, result);
            return true;
        }
        public static NPCShop Add(this NPCShop shop, int[] itemTypes, params Condition[] conditions)
        {
            foreach (int itemType in itemTypes)
            {
                shop.Add(itemType, conditions);
            }
            return shop;
        }
        public static NPCShop Add(this NPCShop shop, int itemType, int? customPrice, params Condition[] conditions)
        {
            shop.Add(new Item(itemType) { shopCustomPrice = customPrice }, conditions);
            return shop;
        }
        public static NPCShop Add(this NPCShop shop, int[] itemTypes, int? customPrice, params Condition[] conditions)
        {
            foreach (int itemType in itemTypes)
            {
                shop.Add(new Item(itemType) { shopCustomPrice = customPrice }, conditions);
            }
            return shop;
        }
        public static NPCShop Add(this NPCShop shop, Mod mod, string itemName, params Condition[] conditions)
        {
            if (mod.TryFind(itemName, out ModItem mi))
            {
                shop.Add(new Item(mi.Type), conditions);
            }
            return shop;
        }
        public static NPCShop Add(this NPCShop shop, Mod mod, string itemName, int value, params Condition[] conditions)
        {
            if (mod.TryFind(itemName, out ModItem mi))
            {
                shop.Add(new Item(mi.Type) { shopCustomPrice = value }, conditions);
            }
            return shop;
        }
        public static NPCShop Add(this NPCShop shop, Mod mod, string[] itemName, params Condition[] condition)
        {
            if (itemName.Length != 0)
            {
                for (int i = 0; i < itemName.Length; i++)
                {
                    shop.Add(mod, itemName[i], condition);
                }
            }
            return shop;
        }

        /// <param name="itemarray">id, 货币id, 需求量</param>
        public static NPCShop Add(this NPCShop shop, Mod mod, (string itemName, int currency, int value)[] itemarray, params Condition[] condition)
        {
            if (itemarray.Length != 0)
            {
                for (int i = 0; i < itemarray.Length; i++)
                {
                    shop.Add(new Item(mod.Find<ModItem>(itemarray[i].itemName).Type)
                    {
                        shopSpecialCurrency = itemarray[i].currency,
                        shopCustomPrice = itemarray[i].value,
                    }, condition);
                }
            }
            return shop;
        }
        /// <param name="currency">货币id</param>
        public static NPCShop Add(this NPCShop shop, Mod mod, (string itemName, int value)[] itemarray, int currency = -1, params Condition[] condition)
        {
            if (itemarray.Length > 0)
            {
                for (int i = 0; i < itemarray.Length; i++)
                {
                    shop.Add(new Item(mod.Find<ModItem>(itemarray[i].itemName).Type)
                    {
                        shopSpecialCurrency = currency,
                        shopCustomPrice = itemarray[i].value,
                    }, condition);
                }
            }
            return shop;
        }
    }
}
