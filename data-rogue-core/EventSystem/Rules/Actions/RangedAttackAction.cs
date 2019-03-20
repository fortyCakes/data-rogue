using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Linq;

namespace data_rogue_core.EventSystem.Rules
{

    public class RangedAttackAction : ApplyActionRule
    {
        public RangedAttackAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.RangedAttack;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            var splits = eventData.Parameters.Split(',');
            var attacker = _systemContainer.EntityEngine.Get(uint.Parse(splits[0]));

            CheckIfAttackerCanMakeRangedAttack();

            _systemContainer.TargetingSystem.GetTarget(sender, new TargetingData { Range = 10 }, (mapCoordinate) => ResolveTarget(sender, mapCoordinate));
            
            return false;
        }

        private void CheckIfAttackerCanMakeRangedAttack()
        {
            throw new NotImplementedException();
        }

        private void ResolveTarget(IEntity attacker, MapCoordinate mapCoordinate)
        {
            var defender = _systemContainer.PositionSystem
                .EntitiesAt(mapCoordinate)
                .FirstOrDefault(e => e.Has<Health>());

            if (defender != null)
            {
                var weaponClass = GetRangedWeaponClass(attacker);

                _systemContainer.FighterSystem.Attack(attacker, defender, "Launcher");
            }
            else
            {
                _systemContainer.MessageSystem.Write("No valid target there.");
            }
        }

        private object GetRangedWeaponClass(IEntity attacker)
        {
            throw new NotImplementedException();
        }
    }
}
