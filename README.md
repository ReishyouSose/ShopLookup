# Shop Lookup
[English](README-en.md) | 简体中文

## Mod Call

### NonPermanent

用于给你的Mod的非常驻NPC指定售卖条件。需要填写以下参数：

- `string MethodName`：必填参数，应填写为`"NonPermanent"`
- `int npcType`：必填参数，这个常驻NPC的类型
- `params Condition[] conditions`：可选参数，指定允许直接购买的条件

#### 使用方法

```csharp
if (ModLoader.TryGetMod("ShopLookup", out Mod slu))
{
	slu.Call("NonPermanent", npcType, condition1, condition2, condition3...);
}
```

如果您不需要指定生成条件，可以不传入`params Condition[] conditions`参数。

### ShopName

用于给商店索引定制本地化信息。需要填写以下参数：

- `string MethodName`：必填参数，应填写为`"ShopName"`。
- `int npcType`：必填参数，是哪个NPC的商店
- `string shopIndex`：必填参数，这个商店的注册名（ModNPC.AddShop中使用的那个）
- `LocalizeText text`：必填参数，指定商店的本地化信息

#### 使用方法

```csharp
if (ModLoader.TryGetMod("ShopLookup", out Mod slu))
{
	slu.Call("ShopName", npcType, shopIndex, text);
}
```

## Head Texture

初次加载会寻找所有NPC的头像贴图，你可以添加基于ModNPC.Texture的带“_Head”的贴图文件。无需[AutoloadHead]，这个Mod可以找到它。

例如：
- ExampleTravelNPC.png
- ExampleTravelNPC_Head.png