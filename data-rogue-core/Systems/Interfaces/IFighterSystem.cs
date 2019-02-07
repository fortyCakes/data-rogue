using System.Collections.Generic;
using data_rogue_core.EntityEngine;

namespace data_rogue_core.Systems.Interfaces
{
    public interface IFighterSystem : ISystem
    {
        void BasicAttack(IEntity sender, IEntity defender);

        IEnumerable<IEntity> GetEntitiesWithFighter(IEnumerable<IEntity> entities);
    }
}
