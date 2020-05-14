using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{

    public class SpriteSheet : IEntityComponent
    {
        public string Name;
        public string Path;
        public string Type;
        public int SpriteWidth;
        public int SpriteHeight;
        public int Scaling;
        public FrameList Frames = new FrameList { AnimationFrame.Rest0 };

        public bool HasMultipleFrames => Frames.Count > 1;
    }
}
