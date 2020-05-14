using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    public class AnimationSystem: BaseSystem, IAnimationSystem
    {
        private IStopwatch _stopwatch;
        private long _lastKnownTime;
        public const int TICK_LENGTH = 33;

        public AnimationSystem(IStopwatch stopwatch)
        {
            _stopwatch = stopwatch;
            _lastKnownTime = _stopwatch.ElapsedMilliseconds;
            _stopwatch.Start();
        }

        public override SystemComponents RequiredComponents => new SystemComponents{typeof(Animated)};
        public override SystemComponents ForbiddenComponents => new SystemComponents();
        public void Tick()
        {
            var elapsed = _stopwatch.ElapsedMilliseconds - _lastKnownTime;
            int tickBy = (int)(elapsed / TICK_LENGTH);
            _lastKnownTime += tickBy * TICK_LENGTH;

            foreach (Animated component in this.Entities.Select(e => e.Get<Animated>()))
            {
                component.CurrentTick += tickBy;

                while (component.CurrentTick >= component.FrameTicks)
                {
                    component.CurrentTick -= component.FrameTicks;
                    component.CurrentFrame++;

                    if (component.CurrentFrame == component.FrameCount)
                    {
                        component.CurrentFrame = 0;
                    }
                }
            }
        }

        public int GetFrame(IEntity entity)
        {
            return entity.Get<Animated>().CurrentFrame;
        }

        public void SetFrame(IEntity entity, int frame)
        {
            Animated component = entity.Get<Animated>();
            component.CurrentFrame = frame;
            component.CurrentTick = 0;
        }
    }
}
