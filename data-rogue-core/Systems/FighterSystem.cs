using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Systems
{
    public class FighterSystem : BaseSystem, IFighterSystem
    {
        public override SystemComponents RequiredComponents => new SystemComponents { typeof(Fighter) };
        public override SystemComponents ForbiddenComponents => new SystemComponents { typeof(Prototype) };

        public FighterSystem(IEntityEngine engine)
        {
            Engine = engine;
        }

        public IEntityEngine Engine { get; }

        public void Attack(IEntity attacker, IEntity defender)
        {
            var attackingFighter = attacker.Get<Fighter>();
            var defendingFighter = defender.Get<Fighter>();

            defendingFighter.Health.Subtract(attackingFighter.Attack);

            if (defendingFighter.Health.Current == 0)
            {
                // TODO on die methods, esp. player
                Engine.Destroy(defender.EntityId);
            }
        }
    }
}
