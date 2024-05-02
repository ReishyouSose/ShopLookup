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

- `int MethodType`: required parameter, should be set to `2`
- `int npcType`: required parameter, the ID of the target NPC
- `Texture2D head`: required parameter, specifies the head texture of the target NPC

#### Usage

```csharp
if (ModLoader.TryGetMod("ShopLookup", out Mod slu))
{
	Texture2D head = ModContent.Request<Texture2D>(path, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
	slu.Call(2, npcType, head);
}
```
