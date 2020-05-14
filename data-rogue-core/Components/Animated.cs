using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Components
{
    public class Animated : IEntityComponent
    {
        public FrameList Frames = new FrameList { AnimationFrame.Rest0, AnimationFrame.Rest1 };
        public int FrameTicks = 10;
        public int CurrentTick;
        public int CurrentFrame;
        public bool RandomiseTicks = false;
        public int FrameCount => Frames.Count;

        public virtual void OnAnimationTick(int tickBy) { }
    }

    public class Fading : Animated, ITickUpdate
    {
        public long Opacity;

        public void Tick(ISystemContainer systemContainer, IEntity entity, ulong currentTime)
        {
            if (Opacity <= 0)
            {
                systemContainer.EntityEngine.Destroy(entity);
            }
        }

        public override void OnAnimationTick(int tickBy)
        {
            Opacity -= tickBy;
            if (Opacity < 0)
            {
                Opacity = 0;
            }
        }
    }
}