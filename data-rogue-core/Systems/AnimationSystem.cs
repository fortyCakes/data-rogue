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
        private readonly IRandom _random;
        private long _lastKnownTime;
        public const int TICK_LENGTH = 33;

        public AnimationSystem( IStopwatch stopwatch, IRandom random)
        {
            _stopwatch = stopwatch;
            _random = random;
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

            foreach (IEntity entity in Entities)
            {
                ResolveAnimationFrameUpdates(tickBy, entity);
            }
        }

        private void ResolveAnimationFrameUpdates(int tickBy, IEntity entity)
        {
            var component = entity.Get<Animated>();
            if (component.CurrentAnimation == null)
            {
                component.CurrentAnimation = GetComponentAnimation(entity, component.DefaultAnimation);
            }

            var currentAnimation = component.CurrentAnimation;
            currentAnimation.CurrentTick += tickBy;

            while (currentAnimation.CurrentTick >= currentAnimation.FrameTicks)
            {
                currentAnimation.CurrentTick -= currentAnimation.FrameTicks;
                if (currentAnimation.RandomiseTicks)
                {
                    currentAnimation.CurrentTick -= _random.PickOneFrom(0, 1);
                }

                currentAnimation.CurrentFrame++;

                if (currentAnimation.CurrentFrame == currentAnimation.FrameCount)
                {
                    if (currentAnimation.Repeat)
                    {
                        currentAnimation.CurrentFrame = 0;
                    }
                    else
                    {
                        SetAnimation(entity, component.DefaultAnimation);
                    }
                }
            }
        }      

        private Animation GetComponentAnimation(IEntity entity, AnimationType animationType)
        {
            return entity.Components.OfType<Animation>().SingleOrDefault(a => a.AnimationType == animationType);
        }

        public AnimationFrame GetFrame(IEntity entity)
        {
            Animation currentAnimation = entity.Get<Animated>().CurrentAnimation;
            return currentAnimation.Frames[currentAnimation.CurrentFrame];
        }

        public void SetFrame(IEntity entity, int frame)
        {
            Animated component = entity.Get<Animated>();
            component.CurrentAnimation.CurrentFrame = frame;
            component.CurrentAnimation.CurrentTick = 0;
        }

        public void SetAnimation(IEntity entity, AnimationType animationType)
        {
            Animated component = entity.Get<Animated>();
            Animation animation = GetComponentAnimation(entity, animationType);

            if (animation != null)
            {
                animation.CurrentFrame = 0;
                animation.CurrentTick = 0;

                component.CurrentAnimation = animation;
            }
        }
    }
}
