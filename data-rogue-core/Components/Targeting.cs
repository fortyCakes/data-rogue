using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class Targeting : IEntityComponent
    {
        public int Range;

        public bool MoveToCell;

        public VectorList CellsHit = new VectorList();

        public bool Rotatable;

        public bool TargetOrigin = true;

        public bool Friendly;

        public VectorList ValidVectors = new VectorList();

        public bool PathToTarget;
    }
}