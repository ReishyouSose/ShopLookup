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

### FakeShop

用于向此模组添加伪商店，需要填写以下参数：

- `int MethodType`：必填参数，应填写为`3`
- `Mod mod`：必填参数，传入你的Mod实例
- `string fakeShopType`：必填参数，作为商店的类型，等同于商店的NPC ID
- `Texture2D icon`：商店的图标，等同于城镇NPC的头部贴图
- `NPCShop shop`：必填参数，允许一个类型有多个商店，比如油漆工有一个普通商店和一个装饰商店

#### 使用方法

以Quality of Terraria更好的体验为例

```csharp
Mod qot = ImproveGame.Instance;
if (ModLoader.TryGetMod("ShopLookup", out Mod mod))
{
    Texture2D wandIcon = TextureAssets.Item[ModContent.ItemType<StarburstWand>()].Value;
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

另外需要编写本地化文件，同样以更好的体验为例

```hjson
Mods: {
    ImproveGame: {
        // ...其他本地化内容
        // 以FakeShops作为索引
        FakeShops: {
            // 伪商店类型
            Wand: {
                // 伪商店的名称（鼠标覆盖在商店图标上时会显示，等同于NPC的名称
                Label: 魔杖商店
                // 商店为空时的提示
                Disable: 无结果
                // 商店的名称，键名要与 new NPCShop(npctype, name) 中的 name 相同
                Normal: 普通魔杖
                HardMode: 困难模式魔杖
            }
            Locator: {
                Label: 定位球
                Disable: 售罄
                LocaltorShop: 定位球商店
            }
            Other: {
                Label: 杂项物品
                Disable: 无结果
                Other: 杂项物品商店
            }
        }
    }
}
```