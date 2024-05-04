# Shop Lookup
English | [简体中文](README.md)
## Mod Call

### ShopName

A plugin for specifying localization information for a shop index.

#### Parameters

- `int MethodType`: Required parameter, should be set to `0`.
- `int npcType`: Required parameter, specifies the NPC whose shop index is being customized.
- `Dictionary<string, LocalizedText>`: Required parameter, key is specifies the registration name of the shop (used in `ModNPC.AddShop`), value is specifies the localization information of the shop.

#### Usage

```csharp
if (ModLoader.TryGetMod("ShopLookup", out Mod slu))
{
	Dictionary<string, LocalizedText> shopLocals = [];
	shopLocals.Add("Shop", Language.GetText("Mods.YourMod.Shops.Index"));
	slu.Call(0, npcType, shopLocals);
}
```


### NonPermanent

A plugin for specifying purchase conditions for non-permanent NPCs in your mod.

#### Parameters

- `int MethodType`: Required parameter, should be set to `1`.
- `int npcType`: Required parameter, specifies the type of non-permanent NPC.
- `params Condition[] conditions`: Optional parameter, specifies the purchase conditions.

#### Usage

```csharp
if (ModLoader.TryGetMod("ShopLookup", out Mod slu))
{
    slu.Call(1, npcType, condition1, condition2, condition3...);
}
```

If you do not need to specify purchase conditions, you can omit the `params Condition[] conditions` parameter.

### Head Texture

The initial loading will look for all NPCs head texture, you can add texture files with "_Head" based on ModNPC.Texture
No need for [AutoloadHead], this mod can finds it

For example：
- ExampleTravelNPC.png
- ExampleTravelNPC_Head.png

Or use ModCall.

#### Parameters

- `int MethodType`: Required parameter, should be set to `2`
- `int npcType`: Required parameter, the ID of the target NPC
- `Texture2D head`: Required parameter, specifies the head texture of the target NPC

#### Usage

```csharp
if (ModLoader.TryGetMod("ShopLookup", out Mod slu))
{
	Texture2D head = ModContent.Request<Texture2D>(path, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
	slu.Call(2, npcType, head);
}
```

### FakeShop

This mod feature allows the addition of fake shops, with the following parameters required:

- `int MethodType`：Required parameter, must be set to`3`
- `Mod mod`：Required parameter, pass in your mod instance
- `string fakeShopType`：Required parameter, serves as the type of the shop, analogous to `npcType` for the shop
- `Texture2D icon`：Required parameter, icon for the shop, analogous to the town NPC's head texture
- `NPCShop shop`：Required parameter, allows a type to have multiple shops, such as a Painter's regular shop and a decor shop

#### Usage

For instance, taking Quality of Terraria(QoT) as an example

```csharp
Mod qot = ImproveGame.Instance;
if (ModLoader.TryGetMod("ShopLookup", out Mod mod))
{
    Texture2D wandIcon = TextureAssets.Item[ModContent.ItemType<StarburstWand>()].Value;
    //"Wand" has two shops
    mod.Call(3, qot, "Wand", wandIcon, new NPCShop(-1, "Normal") .Add<CreateWand>();
    mod.Call(3, qot, "Wand", wandIcon, new NPCShop(-1, "HardMode") .Add<ConstructWand>(Condition.Hardmode));
    mod.Call(3, qot, "Locator", TextureAssets.Item[ModContent.ItemType<AetherGlobe>()].Value,
        new NPCShop(-1, "LocatorShop")
        .Add<FloatingIslandGlobe>()
        .Add<TempleGlobe>(Condition.DownedPlantera));
    mod.Call(3, qot, "Other", TextureAssets.Item[ModContent.ItemType<ExtremeStorage>()].Value,
        new NPCShop(-1, "Other")
        .Add<Dummy>()
        .Add<WeatherBook>(Condition.DownedEowOrBoc));
}
```

You will also need to edit localization file, as done with QoT, for example

```hjson
Mods: {
    ImproveGame: {
        // ...other localization content
        // Use FakeShops as Key
        FakeShops: {
            // Fake shop type
            Wand: {
                // Fake shop's name (displayed when mouse hovers over shop icon, analogous to NPC name)
                Label: Shop of Wands
                // Empty shop prompt
                Disable: No results
                // Shop's name, Key name must match the name in new NPCShop(npctype, name)
                Normal: Standard Wands
                HardMode: Hardmode Wands
            }
            Locator: {
                Label: Locator Globes
                Disable: Sold out
                LocaltorShop: Locator Shop
            }
            Other: {
                Label: Miscellaneous Items
                Disable: No results
                Other: Other Items Shop
            }
        }
    }
}
```