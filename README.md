# ShopLookup
A mod of Terraria TML
<div class="editor-styles-wrapper block-editor-writing-flow" tabindex="-1" style="flex: 1 1 0%;" contenteditable="false" data-listener-added_c57c1c82="true"><div class="edit-post-visual-editor__post-title-wrapper" contenteditable="false"><h1 contenteditable="true" class="wp-block wp-block-post-title block-editor-block-list__block editor-post-title editor-post-title__input rich-text" aria-label="添加标题" role="textbox" aria-multiline="true" style="white-space: pre-wrap; min-width: 1px;" data-listener-added_c57c1c82="true">Mod Call</h1></div><div class="is-root-container is-layout-flow wp-block-post-content block-editor-block-list__layout" data-is-drop-zone="true"><p role="document" aria-multiline="true" aria-label="段落区块" tabindex="0" class="block-editor-rich-text__editable block-editor-block-list__block wp-block wp-block-paragraph rich-text" id="block-4c9016a7-10ec-467e-a90b-49fcf9c2e694" data-block="4c9016a7-10ec-467e-a90b-49fcf9c2e694" data-type="core/paragraph" data-title="段落" data-empty="false" contenteditable="true" style="white-space: pre-wrap; min-width: 1px;" data-listener-added_c57c1c82="true">如果你的Mod添加了类似于旅商的非常驻NPC，你可以用ModCall来让Shop Lookup可以用你提供的特殊判定来检测购买条件。</p><p role="document" aria-multiline="true" aria-label="段落区块" tabindex="0" class="block-editor-rich-text__editable block-editor-block-list__block wp-block is-selected wp-block-paragraph rich-text" id="block-ad9a79f9-7ba0-4f96-976e-0cd3e969980d" data-block="ad9a79f9-7ba0-4f96-976e-0cd3e969980d" data-type="core/paragraph" data-title="段落" data-empty="false" contenteditable="true" style="white-space: pre-wrap; min-width: 1px;" data-listener-added_c57c1c82="true">If your mod adds an Non-Permanent NPC similar to the Traveling Merchant, you can use ModCall to allow Shop Lookup to use the special judgment you provide to detect the sell conditions.</p><div tabindex="0" id="block-c9e629aa-d6d3-4ce7-86bd-0600e9dcc98c" role="document" aria-label="区块：Enlighter Sourcecode" data-block="c9e629aa-d6d3-4ce7-86bd-0600e9dcc98c" data-type="enlighter/codeblock" data-title="Enlighter Sourcecode" class="block-editor-block-list__block wp-block"><div class="enlighter-block-wrapper"><div class="enlighter-header"><div class="enlighter-title">C#</div></div><textarea class="block-editor-plain-text" placeholder="Insert Sourcecode.." aria-label="Code" rows="1" style="overflow: hidden; overflow-wrap: break-word; resize: none; height: 193px;" data-listener-added_c57c1c82="true">// 在Mod中或者ModSystem中。 In Mod/ModSystem
public override void PostSetupContent()
{
    if (ModLoader.TryGetMod("ShopLookup", out Mod slu))
    {
        slu.Call("NonPermanent", ModContent.NPCType&lt;YourNPC&gt;(), Condition.BloodMoon);
    }
}</textarea><div class="enlighter-footer"><div class="enlighter-footer-label"><strong>EnlighterJS</strong> Syntax Highlighter</div></div></div></div><h2 role="document" aria-multiline="true" aria-label="区块：标题" tabindex="0" class="block-editor-rich-text__editable block-editor-block-list__block wp-block wp-block-heading rich-text" id="block-2d11ea3e-dad3-43f1-8cff-056f1f6d29f3" data-block="2d11ea3e-dad3-43f1-8cff-056f1f6d29f3" data-type="core/heading" data-title="标题" contenteditable="true" style="white-space: pre-wrap; min-width: 1px;" data-listener-added_c57c1c82="true">Arguments/参数</h2><ol tabindex="0" class="block-editor-block-list__block wp-block wp-block-list block-editor-block-list__layout" id="block-07f620d9-59d6-40ec-878f-c558aa557ca9" role="document" aria-label="区块：列表" data-block="07f620d9-59d6-40ec-878f-c558aa557ca9" data-type="core/list" data-title="列表" data-is-drop-zone="true"><li tabindex="0" id="block-c492c771-5b4b-4916-8edd-b8f1de43d41c" role="document" aria-label="区块：列表项目" data-block="c492c771-5b4b-4916-8edd-b8f1de43d41c" data-type="core/list-item" data-title="列表项目" class="block-editor-block-list__block wp-block wp-block-list-item block-editor-block-list__layout"><div role="textbox" aria-multiline="true" aria-label="列表文字" contenteditable="true" class="block-editor-rich-text__editable rich-text" style="white-space: pre-wrap; min-width: 1px;" data-listener-added_c57c1c82="true"><strong>string MethodName </strong><br data-rich-text-line-break="true">请填 <code>"NonPermanent"</code><br data-rich-text-line-break="true">Fill in <code>"NonPermanent"</code></div></li><li tabindex="0" id="block-981251c3-905c-4d09-b363-ded382aa5bd9" role="document" aria-label="区块：列表项目" data-block="981251c3-905c-4d09-b363-ded382aa5bd9" data-type="core/list-item" data-title="列表项目" class="block-editor-block-list__block wp-block wp-block-list-item block-editor-block-list__layout"><div role="textbox" aria-multiline="true" aria-label="列表文字" contenteditable="true" class="block-editor-rich-text__editable rich-text" style="white-space: pre-wrap; min-width: 1px;" data-listener-added_c57c1c82="true"><strong>int npcType </strong><br data-rich-text-line-break="true">填入这个NPC的id<br data-rich-text-line-break="true">Fill in the ID of this NPC</div></li><li tabindex="0" id="block-6f2c08c1-853e-495c-9f0a-090c52a27fe9" role="document" aria-label="区块：列表项目" data-block="6f2c08c1-853e-495c-9f0a-090c52a27fe9" data-type="core/list-item" data-title="列表项目" class="block-editor-block-list__block wp-block wp-block-list-item block-editor-block-list__layout"><div role="textbox" aria-multiline="true" aria-label="列表文字" contenteditable="true" class="block-editor-rich-text__editable rich-text" style="white-space: pre-wrap; min-width: 1px;" data-listener-added_c57c1c82="true"><strong data-rich-text-format-boundary="true">params Condition[] conditions</strong><br data-rich-text-line-break="true">填入这个NPC的生成条件，就像给NPC商店添加物品那样<br data-rich-text-line-break="true">Fill in the generation conditions of this NPC, like writing conditions when adding items to the NPC shop.</div></li></ol><span id="monica-writing-entry-btn-root" class="monica-widget" style="position: absolute; left: 0px; top: 0px; pointer-events: none; z-index: 2147483647;"><div class=""><div style="top: 0px; left: 0px; position: absolute;"><style>
.monica-writing-entry-btn {
  position: absolute;
  right: 1px;
  bottom: 1px;
  pointer-events: all;
  cursor: pointer;
  user-select: none;
  -webkit-user-drag: none;
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
  background: transparent;
  transition: all ease 0.2s;
  border-radius: 20px;
  border: 1px solid transparent;
}
.monica-writing-clickable-item {
  cursor: pointer;
  user-select: none;
  -webkit-user-drag: none;
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
  padding: 0 4px;
  height: 26px;
  color: #a0a0a0;
}
.monica-writing-clickable-item.monica-writing-first {
  border-top-left-radius: 20px;
  border-bottom-left-radius: 20px;
}
.monica-writing-clickable-item.monica-writing-last {
  border-top-right-radius: 20px;
  border-bottom-right-radius: 20px;
}
.monica-writing-clickable-item:hover {
  color: #3872e0;
}
.monica-writing-divider {
  background-color: #eeeeee;
  min-width: 1px;
  height: 12px;
}

.monica-writing-entry-btn:hover {
  background: #ffffff;
  border: 1px solid rgba(115, 114, 120, 0.15);
}

.monica-writing-caret {
  width: 1.5px;
  background-color: #3872e0;
  pointer-events: none;
  position: absolute;
  border-radius: 1px;
}
.monica-writing-caret-head {
  background-color: #3872e0;
  width: 6px;
  height: 6px;
  border-radius: 6px;
  position: absolute;
  left: -2.25px;
}
@media print {
  .monica-writing-entry-btn {
    display: none;
  }
}
</style><div id="monica-writing-entry-btn-mirror-node" style="box-sizing: content-box; left: 89.5px; top: 69.875px; width: 819px; height: 53.875px; border: 0px none transparent; border-radius: 0px; padding: 0px; margin: 16px 89.5px; position: absolute; pointer-events: none; overflow: hidden;"><div id="monica-writing-entry-btn" class="monica-writing-entry-btn" style="display: none; opacity: 1;"><span class="monica-writing-clickable-item monica-writing-last"><img src="chrome-extension://fhimbbbmdjiifimnepkibjfjbppnjble/static/writingLogo.png" alt="" style="width: 24px; height: 24px; -webkit-user-drag: none;"></span></div></div></div></div></span></div></div>
