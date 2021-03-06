﻿using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems;

namespace data_rogue_core.Behaviours
{
    public interface IBehaviour : IEntityComponent
    {
        int BehaviourPriority { get; }

        double BehaviourChance { get; }

        ActionEventData ChooseAction(IEntity entity);
    }
}
