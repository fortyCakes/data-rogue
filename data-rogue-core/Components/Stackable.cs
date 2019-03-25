using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{

    public class Stackable:IEntityComponent
    {
        public int StackSize = 1;
        public string StacksWith;
    }
}
