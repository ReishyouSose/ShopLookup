using Terraria.GameInput;
using static Terraria.UI.Gamepad.UILinkPointNavigator;

namespace ShopLookup.Content
{
    public class SLPlayer : ModPlayer
    {
        public override void OnEnterWorld()
        {
            VisitedNPCSys.CheckActiveTownNPC();
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            //&& !Terraria.GameInput.PlayerInput.WritingText
            if (SLUISys.Keybind.JustPressed)
            {
                SLUI ui = ShopLookup.Ins.uis.Elements[SLUI.NameKey] as SLUI;
                ref var visable = ref ui.Info.IsVisible;
                if (!ui.firstLoad)
                {
                    ui.RemoveAll();
                    ui.OnInitialization();
                    ui.LoadShopNPC();
                    ui.firstLoad = true;
                }
                if (Main.HoverItem.type == ItemID.None)
                {
                    int hoverNPC = Main.LocalPlayer.talkNPC;
                    if (hoverNPC != -1)
                    {
                        int type = Main.npc[hoverNPC].type;
                        if (HasShop(type))
                        {
                            if (!visable)
                            {
                                visable = true;
                                ui.ChangeNPC(type);
                                return;
                            }
                            else
                            {
                                if (ui.focusNPC.npcType == type)
                                {
                                    visable = false;
                                    return;
                                }
                                else
                                {
                                    ui.ChangeNPC(type);
                                    return;
                                }
                            }
                        }
                    }
                    hoverNPC = Shortcuts.NPCS_LastHovered;
                    if (hoverNPC < -10)
                    {
                        if (HasShop(-(hoverNPC + 10)))
                        {
                            if (!visable) visable = true;
                            ui.ChangeNPC(Main.npc[hoverNPC].type);
                            return;
                        }
                    }
                    if (hoverNPC >= 0)
                    {
                        int type = Main.npc[hoverNPC].type;
                        if (HasShop(type))
                        {
                            if (!visable) visable = true;
                            ui.ChangeNPC(type);
                            return;
                        }
                    }
                    visable = !visable;
                }
                else
                {
                    if (!visable) visable = true;
                    ui.ChangeItem(Main.HoverItem.type);
                }
            }
        }
    }
}
