using data_rogue_core.Components;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.IOSystems;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Controls
{
    public class SkillBarControl : BaseInfoControl
    {
        public override bool CanHandleMouse => true;

        public override ActionEventData HandleMouse(MouseData mouse, IDataRogueControlRenderer renderer, ISystemContainer systemContainer)
        {
            if (mouse.IsLeftClick)
            {
                var hoveredSkill = renderer.EntityFromMouseData(this, systemContainer, mouse);
                if (hoveredSkill != null)
                {
                    return new ActionEventData
                    {
                        Action = ActionType.UseSkill,
                        IsAction = true,
                        Parameters = hoveredSkill.Get<Prototype>().Name
                    };
                }
            }

            return null;
        }
    }
    
}
