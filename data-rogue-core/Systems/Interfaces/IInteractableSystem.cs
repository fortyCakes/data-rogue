using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Systems.Interfaces
{
    public interface IInteractionSystem : ISystem
    {
        List<IEntity> GetInteractablesNear(IEntity player);

        (IEntity, Interaction) GetCurrentInteractionFor(IEntity player);

        void NextInteraction(IEntity sender);
    }
}
