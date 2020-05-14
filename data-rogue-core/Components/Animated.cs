using data_rogue_core.EntityEngineSystem;

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
    }
}