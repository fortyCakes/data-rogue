using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Components
{
    public class Animated : IEntityComponent
    {
        public AnimationType DefaultAnimation = AnimationType.Idle;
        public Animation CurrentAnimation { get; set; } = null;
    }
}