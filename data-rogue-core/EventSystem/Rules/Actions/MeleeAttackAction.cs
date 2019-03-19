using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class MeleeAttackAction : ApplyActionRule
    {
        public MeleeAttackAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.MeleeAttack;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            var splits = eventData.Parameters.Split(',');
            var attacker = _systemContainer.EntityEngine.Get(uint.Parse(splits[0]));
            var defender = _systemContainer.EntityEngine.Get(uint.Parse(splits[1]));

            var hit = _systemContainer.FighterSystem.BasicAttack(attacker, defender);

            return true;
        }
    }
}
