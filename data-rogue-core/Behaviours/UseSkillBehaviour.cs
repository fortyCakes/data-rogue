using System;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.Behaviours
{
    public class UseRandomSkillBehaviour : BaseBehaviour
    {
        private readonly ISystemContainer _systemContainer;

        public UseRandomSkillBehaviour(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public override ActionEventData ChooseAction(IEntity entity)
        {
            var position = _systemContainer.PositionSystem.CoordinateOf(entity);
            var playerPosition = _systemContainer.PositionSystem.CoordinateOf(_systemContainer.PlayerSystem.Player);

            var skills = _systemContainer.SkillSystem.KnownSkills(entity).ToList();

            if (skills.Any())
            {
                var randomSkill = _systemContainer.Random.PickOne(skills);

                return new ActionEventData {Action = ActionType.UseSkill, Parameters = randomSkill.Get<Prototype>().Name, Speed = entity.Get<Actor>().Speed};
            }

            return null;
        }
    }
}