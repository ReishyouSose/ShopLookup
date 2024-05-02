# Shop Lookup
[English](README-en.md) | 简体中文

## Mod Call

### ShopName

用于给商店索引定制本地化信息。需要填写以下参数：

- `int MethodType`：必填参数，应填写为`0`。
- `int npcType`：必填参数，目标NPC的ID
- `Dictionary<string, LocalizedText>`：必填参数，键是这个商店的注册名（ModNPC.AddShop中使用的那个），值是商店名的本地化实例

#### 使用方法

```csharp
if (ModLoader.TryGetMod("ShopLookup", out Mod slu))
{
	Dictionary<string, LocalizedText> shopLocals = [];
	shopLocals.Add("Shop", Language.GetText("Mods.YourMod.Shops.Index"));
	slu.Call(0, npcType, shopLocals);
}
```

### NonPermanent

用于给你的Mod的非常驻NPC指定售卖条件。需要填写以下参数：

- `int MethodType`：必填参数，应填写为`1`
- `int npcType`：必填参数，这个常驻NPC的ID
- `params Condition[] conditions`：可选参数，指定允许直接购买的条件

#### 使用方法

```csharp
if (ModLoader.TryGetMod("ShopLookup", out Mod slu))
{
	slu.Call(1, npcType, condition1, condition2, condition3...);
}
```

如果您不需要指定生成条件，可以不传入`params Condition[] conditions`参数。

### Head Texture

初次加载会寻找所有NPC的头像贴图，你可以添加基于ModNPC.Texture的带“_Head”的贴图文件。无需[AutoloadHead]，这个Mod可以找到它。

例如：
- ExampleTravelNPC.png
- ExampleTravelNPC_Head.png

或者使用ModCall，需要填写以下参数：

- `int MethodType`：必填参数，应填写为`2`
- `int npcType`：必填参数，目标NPC的ID
- `Texture2D head`：必填参数，指定目标NPC的头部贴图

#### 使用方法

```csharp
if (ModLoader.TryGetMod("ShopLookup", out Mod slu))
{
	Texture2D head = ModContent.Request<Texture2D>(path, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
	slu.Call(2, npcType, head);
}
```