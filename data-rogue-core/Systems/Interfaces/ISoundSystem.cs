﻿using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Systems.Interfaces
{
    public interface ISoundSystem : ISystem
    {
        void Play(Sound sound);

        void Play(IEntity soundEntity);
    }
}