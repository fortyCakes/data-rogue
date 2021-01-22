using data_rogue_core.Maps;

namespace data_rogue_core.Data
{
    public class AnimationMovement
    {
        public AnimationMovement(VectorDouble vector, int duration)
        {
            Vector = vector;
            TimeLeft = Duration = duration;
        }

        public VectorDouble Vector;
        public int Duration;
        public int TimeLeft;
    }
}
