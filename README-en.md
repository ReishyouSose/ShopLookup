# Shop Lookup
English | [简体中文](README.md)
## Mod Call

### NonPermanent

A plugin for specifying purchase conditions for non-permanent NPCs in your mod.

#### Parameters

- `string MethodName`: Required parameter, should be set to `"NonPermanent"`.
- `int npcType`: Required parameter, specifies the type of non-permanent NPC.
- `params Condition[] conditions`: Optional parameter, specifies the purchase conditions.

#### Usage

```csharp
if (ModLoader.TryGetMod("ShopLookup", out Mod slu))
{
    slu.Call("NonPermanent", npcType, condition1, condition2, condition3...);
}
```

If you do not need to specify purchase conditions, you can omit the `params Condition[] conditions` parameter.

### ShopName

A plugin for specifying localization information for a shop index.

#### Parameters

- `string MethodName`: Required parameter, should be set to `"ShopName"`.
- `int npcType`: Required parameter, specifies the NPC whose shop index is being customized.
- `string shopIndex`: Required parameter, specifies the registration name of the shop (used in `ModNPC.AddShop`).
- `LocalizeText text`: Required parameter, specifies the localization information of the shop.

#### Usage

```csharp
if (ModLoader.TryGetMod("ShopLookup", out Mod slu))
{
    slu.Call("ShopName", npcType, shopIndex, text);
}
```

## Head Texture

The initial loading will look for all NPCs head texture, you can add texture files with "_Head" based on ModNPC.Texture
No need for [AutoloadHead], this mod can finds it

For example：
- ExampleTravelNPC.png
- ExampleTravelNPC_Head.png
