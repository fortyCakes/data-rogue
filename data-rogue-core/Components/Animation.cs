using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class Animation : IEntityComponent
    {
        public AnimationType AnimationType = AnimationType.Idle;
        public FrameList Frames = new FrameList {AnimationFrame.Idle0, AnimationFrame.Idle1};
        public int FrameTicks = 10;
        public int CurrentTick;
        public int CurrentFrame;
        public int FrameCount => Frames.Count;
        public bool Repeat = false;
        public bool RandomiseTicks = false;
    }
}