using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class Animated : IEntityComponent
    {
        public FrameList Frames = new FrameList { AnimationFrame.Rest0 };
        public int FrameTicks = 30;
        public int CurrentTick;
        public int CurrentFrame;
        public int FrameCount => Frames.Count;
    }
}