using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System.Linq;

namespace data_rogue_core.EventSystem.Rules
{
    public class HotbarAction : ApplyActionRule
    {
        public HotbarAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.Hotbar;
        public override ActivityType activityType => ActivityType.Gameplay;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            var index = int.Parse(eventData.Parameters);

            var skills = _systemContainer.SkillSystem.KnownSkills(sender).ToList();

            if (skills.Count() > index)
            {
                _systemContainer.SkillSystem.Use(sender, skills[index]);
            }

            return false;
        }
    }
}