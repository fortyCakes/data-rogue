﻿using System.Collections.Generic;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Systems.Interfaces
{
    public interface IFighterSystem : ISystem
    {
        bool BasicAttack(IEntity attacker, IEntity defender);

        bool Attack(IEntity attacker, IEntity defender, string attackClass = null, int? attackDamage = null, string[] attackTags = null, bool spendTime = true, IEntity weapon = null);

        IEnumerable<IEntity> GetEntitiesWithFighter(IEnumerable<IEntity> entities);
    }
}
