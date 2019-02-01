using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.EntityEngine;

namespace data_rogue_core.Systems.Interfaces
{
    public interface IFighterSystem : ISystem
    {
        void BasicAttack(IEntity sender, IEntity defender);

        IEnumerable<IEntity> GetEntitiesWithFighter(IEnumerable<IEntity> entities);
    }
}
