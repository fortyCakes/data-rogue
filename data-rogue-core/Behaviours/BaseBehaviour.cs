﻿using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems;

namespace data_rogue_core.Behaviours
{
    public abstract class BaseBehaviour : IBehaviour, IEntityComponent
    {
        public int Priority;
        public int BehaviourPriority => Priority;

        public double Chance = 1;

        public double BehaviourChance => Chance;

        public abstract ActionEventData ChooseAction(IEntity entity);
    }
}