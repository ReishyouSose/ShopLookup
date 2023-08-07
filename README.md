# ShopLookup
A mod of Terraria TML
<div class="editor-styles-wrapper block-editor-writing-flow" tabindex="-1" style="flex: 1 1 0%;" contenteditable="false" data-listener-added_c57c1c82="true"><div class="edit-post-visual-editor__post-title-wrapper" contenteditable="false"><h1 contenteditable="true" class="wp-block wp-block-post-title block-editor-block-list__block editor-post-title editor-post-title__input rich-text" aria-label="添加标题" role="textbox" aria-multiline="true" style="white-space: pre-wrap; min-width: 1px;" data-listener-added_c57c1c82="true">Mod Call</h1></div><div class="is-root-container is-layout-flow wp-block-post-content block-editor-block-list__layout" data-is-drop-zone="true"><p role="document" aria-multiline="true" aria-label="段落区块" tabindex="0" class="block-editor-rich-text__editable block-editor-block-list__block wp-block wp-block-paragraph rich-text" id="block-4c9016a7-10ec-467e-a90b-49fcf9c2e694" data-block="4c9016a7-10ec-467e-a90b-49fcf9c2e694" data-type="core/paragraph" data-title="段落" data-empty="false" contenteditable="true" style="white-space: pre-wrap; min-width: 1px;" data-listener-added_c57c1c82="true">如果你的Mod添加了类似于旅商的非常驻NPC，你可以用ModCall来让Shop Lookup可以用你提供的特殊判定来检测购买条件。</p><p role="document" aria-multiline="true" aria-label="段落区块" tabindex="0" class="block-editor-rich-text__editable block-editor-block-list__block wp-block is-selected wp-block-paragraph rich-text" id="block-ad9a79f9-7ba0-4f96-976e-0cd3e969980d" data-block="ad9a79f9-7ba0-4f96-976e-0cd3e969980d" data-type="core/paragraph" data-title="段落" data-empty="false" contenteditable="true" style="white-space: pre-wrap; min-width: 1px;" data-listener-added_c57c1c82="true">If your mod adds an Non-Permanent NPC similar to the Traveling Merchant, you can use ModCall to allow Shop Lookup to use the special judgment you provide to detect the sell conditions.</p>

<pre><code>public override void PostSetupContent()
{
    if (ModLoader.TryGetMod("ShopLookup", out Mod slu))
    {
        slu.Call("NonPermanent", ModContent.NPCType\<YourNPC\>(), Condition.BloodMoon);
    }
}
</code></pre>



<h2 role="document" aria-multiline="true" aria-label="区块：标题" tabindex="0" class="block-editor-rich-text__editable block-editor-block-list__block wp-block wp-block-heading rich-text" id="block-2d11ea3e-dad3-43f1-8cff-056f1f6d29f3" data-block="2d11ea3e-dad3-43f1-8cff-056f1f6d29f3" data-type="core/heading" data-title="标题" contenteditable="true" style="white-space: pre-wrap; min-width: 1px;" data-listener-added_c57c1c82="true">Arguments/参数</h2><ol tabindex="0" class="block-editor-block-list__block wp-block wp-block-list block-editor-block-list__layout" id="block-07f620d9-59d6-40ec-878f-c558aa557ca9" role="document" aria-label="区块：列表" data-block="07f620d9-59d6-40ec-878f-c558aa557ca9" data-type="core/list" data-title="列表" data-is-drop-zone="true"><li tabindex="0" id="block-c492c771-5b4b-4916-8edd-b8f1de43d41c" role="document" aria-label="区块：列表项目" data-block="c492c771-5b4b-4916-8edd-b8f1de43d41c" data-type="core/list-item" data-title="列表项目" class="block-editor-block-list__block wp-block wp-block-list-item block-editor-block-list__layout"><div role="textbox" aria-multiline="true" aria-label="列表文字" contenteditable="true" class="block-editor-rich-text__editable rich-text" style="white-space: pre-wrap; min-width: 1px;" data-listener-added_c57c1c82="true"><strong>string MethodName </strong><br data-rich-text-line-break="true">请填 <code>"NonPermanent"</code><br data-rich-text-line-break="true">Fill in <code>"NonPermanent"</code></div></li><li tabindex="0" id="block-981251c3-905c-4d09-b363-ded382aa5bd9" role="document" aria-label="区块：列表项目" data-block="981251c3-905c-4d09-b363-ded382aa5bd9" data-type="core/list-item" data-title="列表项目" class="block-editor-block-list__block wp-block wp-block-list-item block-editor-block-list__layout"><div role="textbox" aria-multiline="true" aria-label="列表文字" contenteditable="true" class="block-editor-rich-text__editable rich-text" style="white-space: pre-wrap; min-width: 1px;" data-listener-added_c57c1c82="true"><strong>int npcType </strong><br data-rich-text-line-break="true">填入这个NPC的id<br data-rich-text-line-break="true">Fill in the ID of this NPC</div></li><li tabindex="0" id="block-6f2c08c1-853e-495c-9f0a-090c52a27fe9" role="document" aria-label="区块：列表项目" data-block="6f2c08c1-853e-495c-9f0a-090c52a27fe9" data-type="core/list-item" data-title="列表项目" class="block-editor-block-list__block wp-block wp-block-list-item block-editor-block-list__layout"><div role="textbox" aria-multiline="true" aria-label="列表文字" contenteditable="true" class="block-editor-rich-text__editable rich-text" style="white-space: pre-wrap; min-width: 1px;" data-listener-added_c57c1c82="true"><strong data-rich-text-format-boundary="true">params Condition[] conditions</strong><br data-rich-text-line-break="true">填入这个NPC的生成条件，就像给NPC商店添加物品那样<br data-rich-text-line-break="true">Fill in the generation conditions of this NPC, like writing conditions when adding items to the NPC shop.
