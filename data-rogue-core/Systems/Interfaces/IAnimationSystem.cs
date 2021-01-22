using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Systems.Interfaces
{
    public interface IAnimationSystem : ISystem
    {
        void Tick();

        AnimationFrame GetFrame(IEntity entity);
        void SetFrame(IEntity entity, int frame);
        void SetAnimation(IEntity entity, AnimationType animationType);
    }

    public interface IAnimatedMovementSystem: ISystem
    {
        void Tick();

        void StartAnimatedMovement(IEntity entity, List<AnimationMovement> movements);
        void StartAnimatedMovement(IEntity entity, VectorDouble movement, int duration);
        bool IsBlockingAnimationPlaying();
    }
}
