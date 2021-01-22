using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using System.Collections.Generic;

namespace data_rogue_core.Components
{
    public class Moving : IEntityComponent
    {
        public List<AnimationMovement> Movements;

        public double OffsetX;
        public double OffsetY;
    }
}