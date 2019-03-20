using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Menus.DynamicMenus;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class SkillMenuAction: ApplyActionRule
    {
        public SkillMenuAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.SkillMenu;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            _systemContainer.ActivitySystem.Push(new MenuActivity(new SkillMenu(_systemContainer, sender), _systemContainer.RendererSystem.RendererFactory));

            return false;
        }
    }

    public class UseSkillAction : ApplyActionRule
    {
        public UseSkillAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.UseSkill;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            _systemContainer.SkillSystem.Use(sender, eventData.Parameters);

            return false;
        }
    }
}