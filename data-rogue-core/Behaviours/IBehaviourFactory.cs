using System;

namespace data_rogue_core.Behaviours
{
    public interface IBehaviourFactory
    {
        IBehaviour Get(Type behaviourType);
    }
}