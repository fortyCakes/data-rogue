using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Systems.Interfaces
{
    public interface IAnimationSystem : ISystem
    {
        void Tick();

        AnimationFrame GetFrame(IEntity entity);
        void SetFrame(IEntity entity, int frame);
        void SetAnimation(IEntity entity, AnimationType animationType);
        void StartAnimatedMovement(IEntity entity, List<AnimationMovement> movements);
        bool IsBlockingAnimationPlaying();
    }
}
